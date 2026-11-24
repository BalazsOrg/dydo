namespace DynaDocs.Tests.Integration;

using DynaDocs.Services;
using System.Diagnostics;

[Collection("Integration")]
public sealed class SafeDeleteBoundaryTests
{
    [Trait("Job", "contract: spec N10; DYD-1029 (archive before delete, refuse unproven state)")]
    [Theory]
    [InlineData("shared clone", "nested.git")]
    [InlineData("independent", "repository without suffix")]
    [InlineData("reftable", "packed repository")]
    [InlineData("sha256", "sha256 repository")]
    [InlineData("common directory", "split repository")]
    [InlineData("non-bare config", "administrative directory")]
    public void InitializedNestedGitDirectory_RefusesBeforePublicationOrDeletion(string layout, string name)
    {
        using var fixture = new SafeDeleteTests();
        AssertNestedRepositoryRefused(fixture,layout,name)();
    }

    private static Action AssertNestedRepositoryRefused(SafeDeleteTests fixture, string layout, string name)
    {
        SafeDeleteTests.Git(fixture.Main,"switch","-c","nested-unique");
        File.WriteAllText(Path.Combine(fixture.Main,"nested.txt"),"unique nested work\n");
        SafeDeleteTests.Git(fixture.Main,"add","nested.txt");
        SafeDeleteTests.Git(fixture.Main,"commit","-m","nested unique");
        var unique = SafeDeleteTests.Git(fixture.Main,"rev-parse","HEAD");
        SafeDeleteTests.Git(fixture.Main,"switch","main");
        fixture.AddTree(); fixture.AddRemote();
        var nested = Path.Combine(fixture.Tree,name);
        CreateNestedRepository(fixture,layout,nested,unique);
        SafeDeleteTests.Git(fixture.Main,"branch","-D","nested-unique");
        SafeDeleteTests.Git(fixture.Main,"reflog","expire","--expire=now","--all");
        var files = Snapshot(fixture.Tree);
        var localRefs = SafeDeleteTests.Git(fixture.Main,"show-ref");
        var remoteRefs = SafeDeleteTests.Git(fixture.Remote,"show-ref");
        var nestedRefs = SafeDeleteTests.Git(nested,"--git-dir",nested,"show-ref");
        var registration = SafeDeleteTests.Git(fixture.Main,"worktree","list","--porcelain");
        var checkpoints = new List<string>(); fixture.Dependencies.Checkpoint = checkpoints.Add;
        var exit = fixture.Run("worktree",fixture.Tree);
        return () =>
        {
            Assert.Equal(2,exit);
            Assert.Contains("Initialized nested repository",fixture.Output);
            Assert.Contains(nested,fixture.Output);
            Assert.Contains("independent",fixture.Output);
            Assert.False(Directory.Exists(fixture.ArchiveRoot));
            Assert.Empty(checkpoints);
            Assert.Equal(files,Snapshot(fixture.Tree));
            Assert.Equal(localRefs,SafeDeleteTests.Git(fixture.Main,"show-ref"));
            Assert.Equal(remoteRefs,SafeDeleteTests.Git(fixture.Remote,"show-ref"));
            Assert.Equal(nestedRefs,SafeDeleteTests.Git(nested,"--git-dir",nested,"show-ref"));
            Assert.Equal(registration,SafeDeleteTests.Git(fixture.Main,"worktree","list","--porcelain"));
            Assert.Equal("unique nested work",SafeDeleteTests.Git(nested,"--git-dir",nested,"show","nested-unique:nested.txt"));
        };
    }

    private static void CreateNestedRepository(SafeDeleteTests fixture, string layout, string nested, string unique)
    {
        if (layout == "shared clone")
        {
            SafeDeleteTests.Git(fixture.Main,"clone","--bare","--shared",fixture.Main,nested);
            Assert.DoesNotContain(Directory.GetFiles(Path.Combine(nested,"objects"),"*",SearchOption.AllDirectories),
                path => !path.EndsWith("alternates",StringComparison.Ordinal));
            return;
        }
        var options = layout switch
        {
            "reftable" => new[] { "--ref-format=reftable" },
            "sha256" => new[] { "--object-format=sha256" },
            _ => Array.Empty<string>()
        };
        SafeDeleteTests.Git(fixture.Main,["init","--bare",..options,nested]);
        if (layout == "sha256")
        {
            var seed = Path.Combine(fixture.Root,"sha256-seed"); Directory.CreateDirectory(seed);
            SafeDeleteTests.Git(seed,"init","--object-format=sha256");
            File.WriteAllText(Path.Combine(seed,"nested.txt"),"unique nested work\n");
            SafeDeleteTests.Git(seed,"add","nested.txt");
            SafeDeleteTests.Git(seed,"-c","user.name=Test","-c","user.email=test@example.invalid","-c","commit.gpgsign=false","commit","-m","unique");
            SafeDeleteTests.Git(nested,"fetch",seed,"HEAD:refs/heads/nested-unique");
        }
        else
        {
            var objects = SafeDeleteTests.Git(fixture.Main,"rev-parse","--path-format=absolute","--git-path","objects");
            File.WriteAllText(Path.Combine(nested,"objects","info","alternates"),objects + "\n");
            SafeDeleteTests.Git(nested,"update-ref","refs/heads/nested-unique",unique);
        }
        if (layout == "common directory")
        {
            var common = Path.Combine(fixture.Root,"nested-common"); Directory.CreateDirectory(common);
            foreach (var part in new[] { "objects", "refs" }) Directory.Move(Path.Combine(nested,part),Path.Combine(common,part));
            File.WriteAllText(Path.Combine(nested,"commondir"),common + "\n");
        }
        if (layout == "non-bare config") SafeDeleteTests.Git(nested,"config","core.bare","false");
    }

    private static string[] Snapshot(string root) => Directory.GetFiles(root,"*",SearchOption.AllDirectories)
        .Order(StringComparer.Ordinal).Select(path => Path.GetRelativePath(root,path) + ":" + Convert.ToHexString(File.ReadAllBytes(path))).ToArray();

    [Trait("Job", "contract: spec N10; DYD-1029 (archive before delete, refuse unproven state)")]
    [Fact]
    public void NestedGitDirectory_ValidatesFileKindsBeforeGitDiscovery()
    {
        if (!OperatingSystem.IsLinux()) return; // Linux hard-link refusal, not Windows metadata proof.
        using var fixture = new SafeDeleteTests(); fixture.AddTree();
        var nested = Path.Combine(fixture.Tree,"repository");
        SafeDeleteTests.Git(fixture.Main,"init","--bare",nested);
        var head = Path.Combine(nested,"HEAD");
        var start = new ProcessStartInfo("ln");
        start.ArgumentList.Add(head); start.ArgumentList.Add(Path.Combine(fixture.Root,"linked-head"));
        using var process = Process.Start(start)!; process.WaitForExit(); Assert.Equal(0,process.ExitCode);
        var files = Snapshot(fixture.Tree);
        Assert.Equal(2,fixture.Run("worktree",fixture.Tree));
        Assert.Contains("Special files and hard links",fixture.Output);
        Assert.Contains(head,fixture.Output);
        Assert.DoesNotContain("Initialized nested repository",fixture.Output);
        Assert.False(Directory.Exists(fixture.ArchiveRoot));
        Assert.True(fixture.Has("refs/heads/topic"));
        Assert.Equal(files,Snapshot(fixture.Tree));
    }

    [Trait("Job", "contract: spec N10; DYD-1029 (archive before delete, refuse unproven state)")]
    [Theory]
    [InlineData("HEAD")]
    [InlineData("objects")]
    [InlineData("refs")]
    [InlineData("invalid HEAD")]
    public void RepositoryFragments_AreOrdinaryFilesThatRoundTrip(string missing)
    {
        using var fixture = new SafeDeleteTests(); fixture.AddTree();
        var fragments = Path.Combine(fixture.Tree,"ordinary.git"); Directory.CreateDirectory(fragments);
        File.WriteAllText(Path.Combine(fragments,"HEAD"),missing == "invalid HEAD" ? "ordinary document\n" : "ref: refs/heads/main\n");
        File.WriteAllText(Path.Combine(fragments,"config"),"[core]\n\tbare = true\n");
        Directory.CreateDirectory(Path.Combine(fragments,"objects")); Directory.CreateDirectory(Path.Combine(fragments,"refs"));
        if (missing == "HEAD") File.Delete(Path.Combine(fragments,missing));
        else if (missing != "invalid HEAD") Directory.Delete(Path.Combine(fragments,missing));
        Assert.False(SafeDeleteGit.Read(fragments,"rev-parse","--resolve-git-dir",fragments).Ok);
        var before = Snapshot(fixture.Tree);
        Assert.True(fixture.Run("worktree",fixture.Tree) == 0,fixture.Output);
        Assert.True(fixture.Run("restore",fixture.Id) == 0,fixture.Output);
        Assert.Equal(before,Snapshot(fixture.Tree));
    }

    [Trait("Job", "contract: spec N10; DYD-1029 (archive before delete, refuse unproven state)")]
    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void AdministrativeNameCasing_PreservesDistinctFilesOrRefusesAliases(bool ignored)
    {
        using var fixture = new SafeDeleteTests(); fixture.AddTree();
        var relative = OperatingSystem.IsWindows() ? Path.Combine("nested",".GIT") : ".GIT";
        var file = Path.Combine(fixture.Tree,relative); Directory.CreateDirectory(Path.GetDirectoryName(file)!);
        File.WriteAllText(file,"user content, not administrative metadata\n");
        if (ignored) File.WriteAllText(Path.Combine(fixture.Tree,".gitignore"),".GIT\n");
        var exit = fixture.Run("worktree",fixture.Tree);
        if (OperatingSystem.IsWindows())
        {
            Assert.Equal(2,exit); Assert.True(fixture.Has("refs/heads/topic"));
            Assert.Equal("user content, not administrative metadata\n",File.ReadAllText(file));
            return;
        }
        Assert.True(exit == 0,fixture.Output);
        Assert.True(fixture.Manifest().Files.Any(item => item.Path == ".GIT"),
            $"Successful archive omitted .GIT; ignored={ignored}; sourceExists={Directory.Exists(fixture.Tree)}");
        Assert.True(fixture.Run("restore",fixture.Id) == 0,fixture.Output);
        Assert.Equal("user content, not administrative metadata\n",File.ReadAllText(file));
    }

    [Trait("Job", "contract: spec N10; DYD-1029 (archive before delete, refuse unproven state)")]
    [Fact]
    public void WorktreeFilemodeOverride_OrdinaryStatusRemainsCleanAfterSourceGcAndRestore()
    {
        if (OperatingSystem.IsWindows()) return; // Unix executable-bit witness; Windows proof remains separate.
        using var fixture = new SafeDeleteTests();
        SafeDeleteTests.Git(fixture.Main,"config","core.filemode","true");
        SafeDeleteTests.Git(fixture.Main,"config","extensions.worktreeConfig","true");
        fixture.AddTree();
        SafeDeleteTests.Git(fixture.Tree,"config","--worktree","core.filemode","false");
        var file = Path.Combine(fixture.Tree,"file.txt");
        File.SetUnixFileMode(file,File.GetUnixFileMode(file) | UnixFileMode.UserExecute | UnixFileMode.GroupExecute | UnixFileMode.OtherExecute);
        Assert.Equal("",SafeDeleteTests.Git(fixture.Tree,"status","--porcelain=v1"));
        var commonConfig = File.ReadAllBytes(Path.Combine(fixture.Main,".git","config"));
        Assert.True(fixture.Run("worktree",fixture.Tree) == 0,fixture.Output);
        var id = fixture.Id;
        SafeDeleteTests.Git(fixture.Main,"reflog","expire","--expire=now","--all");
        SafeDeleteTests.Git(fixture.Main,"gc","--prune=now");
        Assert.True(fixture.Run("restore",id) == 0,fixture.Output);
        Assert.Equal("",SafeDeleteTests.Git(fixture.Tree,"status","--porcelain=v1"));
        Assert.Equal("false",SafeDeleteTests.Git(fixture.Tree,"config","--get","core.filemode"));
        Assert.Equal("true",SafeDeleteTests.Git(fixture.Main,"config","--get","core.filemode"));
        Assert.Equal(commonConfig,File.ReadAllBytes(Path.Combine(fixture.Main,".git","config")));
    }

    [Trait("Job", "contract: spec N10; DYD-1029 (archive before delete, refuse unproven state)")]
    [Fact]
    public void RestoreWithChangedSharedSettings_RefusesWithoutRewritingCommonConfig()
    {
        using var fixture = new SafeDeleteTests();
        SafeDeleteTests.Git(fixture.Main,"config","core.filemode","false");
        fixture.AddTree();
        Assert.True(fixture.Run("worktree",fixture.Tree) == 0,fixture.Output);
        var id = fixture.Id;
        SafeDeleteTests.Git(fixture.Main,"config","core.filemode","true");
        var config = File.ReadAllBytes(Path.Combine(fixture.Main,".git","config"));
        Assert.Equal(2,fixture.Run("restore",id));
        Assert.Contains("require extensions.worktreeConfig; shared configuration retained",fixture.Output);
        Assert.Equal(config,File.ReadAllBytes(Path.Combine(fixture.Main,".git","config")));
        Assert.True(fixture.Manifest().Complete);
    }

    [Trait("Job", "contract: spec N10; DYD-1029 (archive before delete, refuse unproven state)")]
    [Fact]
    public void BranchNameCasing_PreservesDistinctRefsOrRefusesAliases()
    {
        using var fixture = new SafeDeleteTests();
        var original = SafeDeleteTests.Git(fixture.Main,"rev-parse","topic");
        if (!OperatingSystem.IsWindows())
        {
            SafeDeleteTests.Git(fixture.Main,"branch","Topic");
            Assert.True(fixture.Run("branch","Topic") == 0,fixture.Output);
            Assert.True(fixture.Run("restore",fixture.Id) == 0,fixture.Output);
            Assert.Equal(original,SafeDeleteTests.Git(fixture.Main,"rev-parse","Topic"));
        }
        else
        {
            Assert.Equal(2,fixture.Run("branch","TOPIC"));
            Assert.False(Directory.Exists(fixture.ArchiveRoot));
        }
        Assert.Equal(original,SafeDeleteTests.Git(fixture.Main,"rev-parse","topic"));
    }

    [Trait("Job", "contract: spec N10; DYD-1029 (archive before delete, refuse unproven state)")]
    [Fact]
    public void PerWorktreeRefTip_RemainsRecoverableAfterRegistrationAndSourceGc()
    {
        using var fixture = new SafeDeleteTests(); fixture.AddTree();
        var primary = SafeDeleteTests.Git(fixture.Tree,"rev-parse","HEAD");
        SafeDeleteTests.Git(fixture.Tree,"commit","--allow-empty","-m","per-worktree unique");
        var tip = SafeDeleteTests.Git(fixture.Tree,"rev-parse","HEAD");
        SafeDeleteTests.Git(fixture.Tree,"reset","--hard",primary);
        SafeDeleteTests.Git(fixture.Tree,"update-ref","refs/worktree/recovery",tip);
        Assert.True(fixture.Run("worktree",fixture.Tree) == 0,fixture.Output);
        var manifest = fixture.Manifest(); var captured = Assert.Single(manifest.Tips, item => item.Oid == tip);
        Assert.Contains("refs/worktree/recovery",captured.Source);
        SafeDeleteTests.Git(fixture.Main,"reflog","expire","--expire=now","--all"); SafeDeleteTests.Git(fixture.Main,"gc","--prune=now");
        Assert.False(SafeDeleteGit.Read(fixture.Main,"cat-file","-e",tip).Ok);
        Assert.True(fixture.Run("restore",manifest.Id) == 0,fixture.Output);
        Assert.Equal(tip,SafeDeleteTests.Git(fixture.Main,"rev-parse",captured.RecoveryRef(manifest.Id)));
        Assert.Equal(primary,SafeDeleteTests.Git(fixture.Tree,"rev-parse","HEAD"));
    }

    [Trait("Job", "contract: spec N10; DYD-1029 (archive before delete, refuse unproven state)")]
    [Fact]
    public void AllocationVolumeUnavailable_RefusesWithoutArchiveOrDeletion()
    {
        using var fixture = new SafeDeleteTests();
        fixture.Dependencies.Space = _ => throw new IOException("allocation volume unavailable");
        Assert.Equal(2,fixture.Run("branch","topic")); Assert.True(fixture.Has("refs/heads/topic"));
        Assert.False(Directory.Exists(fixture.ArchiveRoot)); Assert.Contains("volume unavailable",fixture.Output);
    }

    [Trait("Job", "contract: spec N10; DYD-1029 (archive before delete, refuse unproven state)")]
    [Theory]
    [InlineData("files",null)]
    [InlineData("tips",null)]
    [InlineData("localOid","invalid")]
    [InlineData("primaryOid","0000000000000000000000000000000000000000")]
    public void MalformedManifestFields_RefuseBeforeCreatingRefs(string field, string? value)
    {
        using var fixture = new SafeDeleteTests();
        Assert.True(fixture.Run("branch","topic") == 0,fixture.Output);
        var path = Path.Combine(fixture.Entry,"manifest.json");
        var node = System.Text.Json.Nodes.JsonNode.Parse(File.ReadAllText(path))!;
        node[field] = value; File.WriteAllText(path,node.ToJsonString());
        Assert.Equal(2,fixture.Run("restore",fixture.Id)); Assert.False(fixture.Has("refs/heads/topic"));
    }

    [Trait("Job", "contract: spec N10; DYD-1029 (archive before delete, refuse unproven state)")]
    [Fact]
    public void LinkedIncompleteEntry_ListRefusesWithoutFollowingTarget()
    {
        using var fixture = new SafeDeleteTests(); Directory.CreateDirectory(fixture.ArchiveRoot);
        var outside = Path.Combine(fixture.Root,"outside"); Directory.CreateDirectory(outside);
        var sentinel = Path.Combine(outside,"sentinel"); File.WriteAllText(sentinel,"retain");
        Directory.CreateSymbolicLink(Path.Combine(fixture.ArchiveRoot,".incomplete-linked"),outside);
        Assert.Equal(2,fixture.Run("list")); Assert.Equal("retain",File.ReadAllText(sentinel));
        Assert.True(fixture.Has("refs/heads/topic"));
    }

    [Trait("Job", "contract: spec N10; DYD-1029 (archive before delete, refuse unproven state)")]
    [Fact]
    public void ExplicitRemoteRestore_UsesOriginalRemoteTipAndRenamesOnlyPrimary()
    {
        using var fixture = new SafeDeleteTests(); fixture.AddRemote(); fixture.AddTree();
        SafeDeleteTests.Git(fixture.Tree,"commit","--allow-empty","-m","local-only");
        var local = SafeDeleteTests.Git(fixture.Tree,"rev-parse","HEAD");
        SafeDeleteTests.Git(fixture.Main,"worktree","remove",fixture.Tree);
        var remote = SafeDeleteTests.Git(fixture.Remote,"-c","user.name=Test","-c","user.email=test@example.invalid","commit-tree",SafeDeleteTests.Git(fixture.Main,"rev-parse","main^{tree}"),"-p",SafeDeleteTests.Git(fixture.Main,"rev-parse","main"),"-m","remote-only");
        SafeDeleteTests.Git(fixture.Remote,"update-ref","refs/heads/topic",remote);
        Assert.True(fixture.Run("branch","topic") == 0,fixture.Output);
        var id = fixture.Id;
        SafeDeleteTests.Git(fixture.Main,"reflog","expire","--expire=now","--all"); SafeDeleteTests.Git(fixture.Main,"gc","--prune=now");
        Assert.True(fixture.Run("restore",id,"--branch","renamed","--remote") == 0,fixture.Output);
        Assert.Equal(local,SafeDeleteTests.Git(fixture.Main,"rev-parse","renamed"));
        Assert.Equal(remote,SafeDeleteTests.Git(fixture.Remote,"rev-parse","topic"));
        Assert.False(fixture.Has("refs/heads/topic"));
    }

    [Trait("Job", "contract: spec N10; DYD-1029 (archive before delete, refuse unproven state)")]
    [Fact]
    public void CorruptManifestGitConfiguration_RefusesWithoutCreatingRefs()
    {
        using var fixture = new SafeDeleteTests();
        Assert.True(fixture.Run("branch","topic") == 0,fixture.Output);
        var manifest = fixture.Manifest(); manifest.GitConfig["include.path"] = "unexpected-config"; fixture.Save(manifest);
        Assert.Equal(2,fixture.Run("restore",fixture.Id)); Assert.False(fixture.Has("refs/heads/topic"));
    }

    [Trait("Job", "contract: spec N10; DYD-1029 (archive before delete, refuse unproven state)")]
    [Fact]
    public void RestorePathCollision_PreservesSentinelAndRefAbsence()
    {
        using var fixture = new SafeDeleteTests(); fixture.AddTree();
        Assert.True(fixture.Run("worktree",fixture.Tree) == 0,fixture.Output);
        var id = fixture.Id;
        Directory.CreateDirectory(fixture.Tree);
        var sentinel = Path.Combine(fixture.Tree,"sentinel"); File.WriteAllText(sentinel,"new owner");
        Assert.Equal(2,fixture.Run("restore",id));
        Assert.Equal("new owner",File.ReadAllText(sentinel)); Assert.False(fixture.Has("refs/heads/topic"));
    }

    [Trait("Job", "contract: spec N10; DYD-1029 (archive before delete, refuse unproven state)")]
    [Fact]
    public void FinalBranchRecheck_RefusesNewlyCheckedOutBranch()
    {
        using var fixture = new SafeDeleteTests();
        fixture.Dependencies.Checkpoint = point => { if (point == "before-local-delete") fixture.AddTree(); };
        Assert.Equal(2,fixture.Run("branch","topic"));
        Assert.True(fixture.Has("refs/heads/topic")); Assert.True(Directory.Exists(fixture.Tree));
    }

    [Trait("Job", "contract: spec N10; DYD-1029 (archive before delete, refuse unproven state)")]
    [Theory]
    [InlineData("reflog")]
    [InlineData("index")]
    [InlineData("root permissions")]
    public void FinalSnapshotRecheck_RefusesChangesWithoutTipMovement(string change)
    {
        using var fixture = new SafeDeleteTests(); fixture.AddTree();
        fixture.Dependencies.Checkpoint = point =>
        {
            if (point != "before-worktree-delete") return;
            if (change == "reflog")
            {
                var log = SafeDeleteTests.Git(fixture.Tree,"rev-parse","--path-format=absolute","--git-path","logs/refs/heads/topic");
                File.AppendAllText(log,File.ReadAllLines(log)[0]+"\n");
            }
            else if (change == "index") SafeDeleteTests.Git(fixture.Tree,"update-index","--assume-unchanged","file.txt");
            else if (OperatingSystem.IsWindows()) File.SetAttributes(fixture.Tree,File.GetAttributes(fixture.Tree) | FileAttributes.Hidden);
            else File.SetUnixFileMode(fixture.Tree,File.GetUnixFileMode(fixture.Tree) ^ UnixFileMode.GroupExecute);
        };
        Assert.Equal(2,fixture.Run("worktree",fixture.Tree));
        Assert.True(fixture.Has("refs/heads/topic")); Assert.True(Directory.Exists(fixture.Tree));
    }
    [Trait("Job", "contract: spec N10; DYD-1029 (archive before delete, refuse unproven state)")]
    [Fact]
    public void NativeBusyProbe_KeepsRefsAndFiles()
    {
        using var fixture = new SafeDeleteTests();
        fixture.AddTree(); fixture.AddRemote();
        fixture.Dependencies.FindUse = WorktreeProcessProbe.FindUse;
        var start = new ProcessStartInfo("git") { WorkingDirectory = fixture.Tree, RedirectStandardInput = true, RedirectStandardOutput = true };
        start.ArgumentList.Add("hash-object"); start.ArgumentList.Add("--stdin");
        using var child = Process.Start(start)!;
        try
        {
            Assert.False(child.HasExited);
            Assert.Equal(2,fixture.Run("worktree",fixture.Tree));
            Assert.True(fixture.Has("refs/heads/topic")); Assert.True(Directory.Exists(fixture.Tree));
            Assert.Contains("running process",fixture.Output);
        }
        finally { child.StandardInput.Close(); child.WaitForExit(); }
    }

    [Trait("Job", "contract: spec N10; DYD-1029 (archive before delete, refuse unproven state)")]
    [Theory]
    [InlineData("before-remote-delete")]
    [InlineData("before-local-delete")]
    public void CompareAndSwap_ProtectsARefMovingAfterFinalQuery(string checkpoint)
    {
        using var fixture = new SafeDeleteTests(); fixture.AddRemote();
        SafeDeleteTests.Git(fixture.Main,"commit","--allow-empty","-m","concurrent");
        var newer = SafeDeleteTests.Git(fixture.Main,"rev-parse","HEAD");
        SafeDeleteTests.Git(fixture.Main,"push","origin","main");
        fixture.Dependencies.Checkpoint = point =>
        {
            if (point == checkpoint) SafeDeleteTests.Git(checkpoint == "before-remote-delete" ? fixture.Remote : fixture.Main,"update-ref","refs/heads/topic",newer);
        };
        Assert.Equal(2,fixture.Run("branch","topic"));
        Assert.True(fixture.Manifest().Complete);
        var location = checkpoint == "before-remote-delete" ? fixture.Remote : fixture.Main;
        Assert.Equal(newer,SafeDeleteTests.Git(location,"rev-parse","refs/heads/topic"));
        Assert.True(fixture.Has("refs/heads/topic"));
    }

    [Trait("Job", "contract: spec N10; DYD-1029 (archive before delete, refuse unproven state)")]
    [Fact]
    public void PostFetchReserveShortfall_KeepsLocalRemoteAndFiles()
    {
        using var fixture = new SafeDeleteTests(); fixture.AddRemote(); fixture.AddTree();
        var queries = 0;
        fixture.Dependencies.Space = _ => ++queries >= 3 ? (0,100000000000) : (100000000000,100000000000);
        Assert.Equal(2,fixture.Run("worktree",fixture.Tree));
        Assert.True(fixture.Has("refs/heads/topic")); Assert.True(Directory.Exists(fixture.Tree));
        Assert.NotEmpty(SafeDeleteTests.Git(fixture.Remote,"for-each-ref","refs/heads/topic"));
        Assert.Contains("reserve would be violated",fixture.Output);
    }

    [Trait("Job", "contract: spec N10; DYD-1029 (archive before delete, refuse unproven state)")]
    [Fact]
    public void IndexConflictStagesAndIntentToAdd_SurviveSourceGc()
    {
        using var fixture = new SafeDeleteTests(); fixture.AddTree();
        var blob = SafeDeleteTests.Git(fixture.Tree,"rev-parse","HEAD:file.txt");
        var output = Path.Combine(fixture.Root,"index-info-output");
        SafeDeleteGit.Stream(fixture.Tree,$"0 {new string('0',40)}\tfile.txt\n100644 {blob} 1\tfile.txt\n100644 {blob} 2\tfile.txt\n100644 {blob} 3\tfile.txt\n",null,output,"update-index","--index-info");
        File.WriteAllText(Path.Combine(fixture.Tree,"intent.txt"),"not staged\n");
        SafeDeleteTests.Git(fixture.Tree,"add","-N","intent.txt");
        var before = SafeDeleteTests.Git(fixture.Tree,"ls-files","--stage");
        Assert.True(fixture.Run("worktree",fixture.Tree) == 0,fixture.Output);
        var id = fixture.Id;
        SafeDeleteTests.Git(fixture.Main,"reflog","expire","--expire=now","--all"); SafeDeleteTests.Git(fixture.Main,"gc","--prune=now");
        Assert.True(fixture.Run("restore",id) == 0,fixture.Output);
        Assert.Equal(before,SafeDeleteTests.Git(fixture.Tree,"ls-files","--stage"));
        Assert.Contains("A intent.txt",SafeDeleteTests.Git(fixture.Tree,"status","--porcelain"));
    }

    // Records three unique versions as a resolved conflict: afterwards only resolve-undo references them.
    private static string[] ResolveThreeUniqueStages(SafeDeleteTests fixture)
    {
        var oids = new[] { "unique ancestor", "unique ours", "unique theirs" }.Select((text, index) =>
        {
            var source = Path.Combine(fixture.Root,$"stage-{index}");
            File.WriteAllText(source,text + "\n");
            return SafeDeleteTests.Git(fixture.Tree,"hash-object","-w","--",source);
        }).ToArray();
        var output = Path.Combine(fixture.Root,"resolve-undo-index-info");
        SafeDeleteGit.Stream(fixture.Tree,$"0 {new string('0',40)}\tfile.txt\n" + string.Concat(oids.Select((oid, index) => $"100644 {oid} {index + 1}\tfile.txt\n")),null,output,"update-index","--index-info");
        File.WriteAllText(Path.Combine(fixture.Tree,"file.txt"),"resolved working version\n");
        SafeDeleteTests.Git(fixture.Tree,"add","file.txt");
        var undo = SafeDeleteTests.Git(fixture.Tree,"ls-files","--resolve-undo");
        Assert.All(oids, oid => Assert.Contains(oid,undo));
        return oids;
    }

    [Trait("Job", "contract: spec N10; DYD-1029 (archive before delete, refuse unproven state)")]
    [Fact]
    public void EveryResolveUndoStage_SurvivesSourceGcAndRecreatesTheConflict()
    {
        using var fixture = new SafeDeleteTests(); fixture.AddTree();
        var oids = ResolveThreeUniqueStages(fixture);
        var undo = SafeDeleteTests.Git(fixture.Tree,"ls-files","--resolve-undo");
        Assert.True(fixture.Run("worktree",fixture.Tree) == 0,fixture.Output);
        var id = fixture.Id;
        SafeDeleteTests.Git(fixture.Main,"reflog","expire","--expire=now","--all"); SafeDeleteTests.Git(fixture.Main,"gc","--prune=now");
        Assert.All(oids, oid => Assert.False(SafeDeleteGit.Read(fixture.Main,"cat-file","-e",oid).Ok));
        Assert.True(fixture.Run("restore",id) == 0,fixture.Output);
        Assert.Equal(undo,SafeDeleteTests.Git(fixture.Tree,"ls-files","--resolve-undo"));
        Assert.Equal(["unique ancestor","unique ours","unique theirs"],oids.Select(oid => SafeDeleteTests.Git(fixture.Tree,"cat-file","blob",oid)));
        SafeDeleteTests.Git(fixture.Tree,"checkout","-m","--","file.txt");
        Assert.Equal(undo,SafeDeleteTests.Git(fixture.Tree,"ls-files","--unmerged"));
    }

    [Trait("Job", "contract: spec N10; DYD-1029 (archive before delete, refuse unproven state)")]
    [Fact]
    public void ResolveUndoBlobMissingFromProofPack_RefusesBeforePublishingOrDeleting()
    {
        using var fixture = new SafeDeleteTests(); fixture.AddTree();
        ResolveThreeUniqueStages(fixture);
        var undo = SafeDeleteTests.Git(fixture.Tree,"ls-files","--resolve-undo");
        fixture.Dependencies.Checkpoint = point =>
        {
            if (point != "before-proof") return;
            // Replace the archived pack with one holding only the current index stages.
            var pack = Directory.GetFiles(fixture.ArchiveRoot,"index-objects.pack",SearchOption.AllDirectories).Single();
            var current = Path.Combine(fixture.Root,"current-stages.pack");
            var stages = SafeDeleteTests.Git(fixture.Tree,"ls-files","--stage").Split('\n').Select(line => line.Split(' ')[1]);
            SafeDeleteGit.Stream(fixture.Tree,string.Concat(stages.Select(oid => oid + "\n")),null,current,"pack-objects","--stdout");
            File.Copy(current,pack,true);
        };
        Assert.Equal(2,fixture.Run("worktree",fixture.Tree));
        Assert.True(Directory.Exists(fixture.Tree)); Assert.True(fixture.Has("refs/heads/topic"));
        Assert.Equal(undo,SafeDeleteTests.Git(fixture.Tree,"ls-files","--resolve-undo"));
        Assert.DoesNotContain(Directory.GetDirectories(fixture.ArchiveRoot), p => !Path.GetFileName(p).StartsWith('.'));
    }

    [Trait("Job", "contract: spec N10; DYD-1029 (archive before delete, refuse unproven state)")]
    [Fact]
    public void ResolveUndoListingDisagreeingWithTheArchivedIndex_RefusesRestore()
    {
        using var fixture = new SafeDeleteTests(); fixture.AddTree();
        ResolveThreeUniqueStages(fixture);
        Assert.True(fixture.Run("worktree",fixture.Tree) == 0,fixture.Output);
        var manifest = fixture.Manifest();
        Assert.NotEqual("",manifest.ResolveUndoEntries);
        manifest.ResolveUndoEntries = "";
        fixture.Save(manifest);
        Assert.Equal(2,fixture.Run("restore",manifest.Id));
        Assert.Contains("index/status snapshot differs",fixture.Output);
        Assert.False(Directory.Exists(fixture.Tree)); Assert.False(fixture.Has("refs/heads/topic"));
    }

    public static TheoryData<string> AllocationTargets() => new()
    {
        // A missing target on the root volume, and one under the (8.3 short-named on hosted Windows) temp path.
        Path.Combine(Path.GetPathRoot(Path.GetTempPath())!,"dydo-missing-" + Guid.NewGuid().ToString("N"),"child"),
        Path.Combine(Path.GetTempPath(),"dydo-missing-" + Guid.NewGuid().ToString("N"),"child")
    };

    [Trait("Job", "edge: DYD-1029 (the default space probe finds the containing volume of a missing target, on the root volume and under the temp path)")]
    [Theory]
    [MemberData(nameof(AllocationTargets))]
    public void DefaultAllocationVolume_IdentifiesTheContainingVolumeOfMissingTargets(string target)
    {
        Assert.False(Path.Exists(target));
        var (available, total) = new SafeDeleteDependencies().Space(target);
        Assert.InRange(total,1,long.MaxValue); Assert.InRange(available,0,total);
    }

    [Trait("Job", "contract: spec N10; DYD-1029 (archive before delete, refuse unproven state)")]
    [Fact]
    public void DefaultAllocationVolume_RefusesAnUnknownVolume()
    {
        if (!OperatingSystem.IsWindows()) return; // Every absolute Unix path lies on some mount; Windows UNC shares do not.
        Assert.Throws<IOException>(() => new SafeDeleteDependencies().Space(@"\\dydo-unknown-host\share\child"));
    }

    [Trait("Job", "contract: spec N10; DYD-1029 (archive before delete, refuse unproven state)")]
    [Fact]
    public void WindowsStreamMetadata_AcceptsDefaultStreamsAndRefusesAlternateStreams()
    {
        if (!OperatingSystem.IsWindows()) return; // NTFS stream enumeration is Windows-native.
        using var fixture = new SafeDeleteTests();
        var empty = Path.Combine(fixture.Root,"empty"); Directory.CreateDirectory(empty);
        var file = Path.Combine(fixture.Root,"file.bin"); File.WriteAllBytes(file,[0,255,1,0]);
        Assert.NotEmpty(SafeDeleteWindowsMetadata.Read(empty));
        Assert.NotEmpty(SafeDeleteWindowsMetadata.Read(fixture.Root));
        Assert.NotEmpty(SafeDeleteWindowsMetadata.Read(file));
        File.WriteAllText(file + ":hidden","alternate stream\n");
        Assert.Contains("Alternate Windows streams",Assert.Throws<InvalidDataException>(() => SafeDeleteWindowsMetadata.Read(file)).Message);
    }

    [Trait("Job", "contract: spec N10; DYD-1029 (archive before delete, refuse unproven state)")]
    [Theory]
    [InlineData("{\"archive\":{\"reserveBytes\":1.5}}")]
    [InlineData("{\"archive\":{\"reservePercent\":-1}}")]
    [InlineData("{\"archive\":{\"reservePercent\":101}}")]
    [InlineData("{\"archive\":{\"reserveBytes\":null}}")]
    public void InvalidReserveConfiguration_RefusesBeforeAllocating(string config)
    {
        using var fixture = new SafeDeleteTests(); File.WriteAllText(Path.Combine(fixture.Main,"dydo.json"),config);
        Assert.Equal(2,fixture.Run("branch","topic")); Assert.True(fixture.Has("refs/heads/topic"));
        Assert.False(Directory.Exists(fixture.ArchiveRoot));
    }

    [Trait("Job", "contract: spec N10; DYD-1029 (archive before delete, refuse unproven state)")]
    [Fact]
    public void CorruptProofPayload_RefusesBeforePublishingOrDeleting()
    {
        using var fixture = new SafeDeleteTests();
        fixture.Dependencies.Checkpoint = point =>
        {
            if (point == "before-proof") File.WriteAllBytes(Directory.GetFiles(fixture.ArchiveRoot,"commits.bundle",SearchOption.AllDirectories).Single(),[]);
        };
        Assert.Equal(2,fixture.Run("branch","topic")); Assert.True(fixture.Has("refs/heads/topic"));
        Assert.DoesNotContain(Directory.GetDirectories(fixture.ArchiveRoot), p => !Path.GetFileName(p).StartsWith('.'));
    }
}
