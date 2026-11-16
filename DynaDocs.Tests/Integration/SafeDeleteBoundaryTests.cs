namespace DynaDocs.Tests.Integration;

using DynaDocs.Services;
using System.Diagnostics;

[Collection("Integration")]
public sealed class SafeDeleteBoundaryTests
{
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

    [Fact]
    public void AllocationVolumeUnavailable_RefusesWithoutArchiveOrDeletion()
    {
        using var fixture = new SafeDeleteTests();
        fixture.Dependencies.Space = _ => throw new IOException("allocation volume unavailable");
        Assert.Equal(2,fixture.Run("branch","topic")); Assert.True(fixture.Has("refs/heads/topic"));
        Assert.False(Directory.Exists(fixture.ArchiveRoot)); Assert.Contains("volume unavailable",fixture.Output);
    }

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

    [Fact]
    public void CorruptManifestGitConfiguration_RefusesWithoutCreatingRefs()
    {
        using var fixture = new SafeDeleteTests();
        Assert.True(fixture.Run("branch","topic") == 0,fixture.Output);
        var manifest = fixture.Manifest(); manifest.GitConfig["include.path"] = "unexpected-config"; fixture.Save(manifest);
        Assert.Equal(2,fixture.Run("restore",fixture.Id)); Assert.False(fixture.Has("refs/heads/topic"));
    }

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

    [Fact]
    public void FinalBranchRecheck_RefusesNewlyCheckedOutBranch()
    {
        using var fixture = new SafeDeleteTests();
        fixture.Dependencies.Checkpoint = point => { if (point == "before-local-delete") fixture.AddTree(); };
        Assert.Equal(2,fixture.Run("branch","topic"));
        Assert.True(fixture.Has("refs/heads/topic")); Assert.True(Directory.Exists(fixture.Tree));
    }

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
