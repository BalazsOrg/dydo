namespace DynaDocs.Tests.Integration;

using DynaDocs.Services;

[Collection("Integration")]
public sealed class SafeDeleteInputsTests
{
    [Trait("Job", "edge: a proof input that changes during the archive")]
    [Theory]
    [InlineData("exclude", "before-copy", true)]
    [InlineData("attributes", "before-proof", true)]
    [InlineData("exclude", "before-delete", true)]
    [InlineData("attributes", "before-worktree-delete", true)]
    [InlineData("attributes", "before-worktree-delete", false)]
    public void ChangedCommonInput_RetainsTargetEvenWhenStatusIsUnchanged(string name, string checkpoint, bool present)
    {
        using var fixture = new SafeDeleteTests();
        // An absent input is never written, so no entry is staged under rules that later vanish.
        Prepare(fixture,exclude: present || name != "exclude",attributes: present || name != "attributes");
        var status = SafeDeleteTests.Git(fixture.Tree,"status","--porcelain=v1","--ignored");
        fixture.Dependencies.Checkpoint = point =>
        {
            if (point == checkpoint) File.AppendAllText(Input(fixture,name),"# changed proof input\n");
        };
        Assert.Equal(2,fixture.Run("worktree",fixture.Tree));
        Assert.True(fixture.Has("refs/heads/topic"));
        Assert.True(Directory.Exists(fixture.Tree));
        Assert.Equal(status,SafeDeleteTests.Git(fixture.Tree,"status","--porcelain=v1","--ignored"));
        Assert.True(fixture.Output.Contains("proof input",StringComparison.Ordinal),fixture.Output);
        Assert.EndsWith("# changed proof input\n",File.ReadAllText(Input(fixture,name)));
    }

    // file.txt is staged as LF under `text eol=lf`; removing the rule leaves CRLF bytes whose
    // stat-clean index entry Git trusts in the source but re-hashes in the proof's fresh files.
    // The bytes and index round-trip, yet Git's view of the worktree cannot be reproduced: refuse.
    [Trait("Job", "edge: a stat-clean index entry the proof cannot reproduce")]
    [Fact]
    public void StatCleanEntryStaleUnderRemovedAttributes_RefusesAtProofAndRetainsTarget()
    {
        using var fixture = new SafeDeleteTests();
        Prepare(fixture);
        // Older than the index by far more than Git's racy-timestamp granularity, so the source trusts stat.
        File.SetLastWriteTimeUtc(Path.Combine(fixture.Tree,"file.txt"),DateTime.UtcNow.AddHours(-1));
        SafeDeleteTests.Git(fixture.Tree,"update-index","-q","--refresh");
        File.Delete(Input(fixture,"attributes"));
        var status = SafeDeleteTests.Git(fixture.Tree,"status","--porcelain=v1","--ignored");
        Assert.DoesNotContain("file.txt",status);
        // The archive records no timestamps; moving the payload's mtime makes the proof's copied stat
        // differ on every platform (File.Copy keeps mtime, and Windows reports no inode), so it re-hashes.
        fixture.Dependencies.Checkpoint = point =>
        {
            if (point != "before-proof") return;
            var payload = Directory.GetFiles(fixture.ArchiveRoot,"file.txt",SearchOption.AllDirectories).Single();
            File.SetLastWriteTimeUtc(payload,DateTime.UtcNow.AddHours(-2));
        };
        Assert.Equal(2,fixture.Run("worktree",fixture.Tree));
        Assert.True(fixture.Output.Contains("Reconstructed file/index/status snapshot differs",StringComparison.Ordinal),fixture.Output);
        Assert.DoesNotContain("Verified archive",fixture.Output);
        Assert.True(fixture.Has("refs/heads/topic"));
        Assert.True(Directory.Exists(fixture.Tree));
        Assert.Equal(status,SafeDeleteTests.Git(fixture.Tree,"status","--porcelain=v1","--ignored"));
        Assert.Equal("base\r\n",File.ReadAllText(Path.Combine(fixture.Tree,"file.txt")));
    }

    [Trait("Job", "edge: an archived proof input corrupted during the archive")]
    [Theory]
    [InlineData("exclude", "before-proof")]
    [InlineData("attributes", "before-proof")]
    [InlineData("exclude", "before-delete")]
    [InlineData("attributes", "before-delete")]
    public void CorruptInputPayload_RetainsTarget(string name, string checkpoint)
    {
        using var fixture = new SafeDeleteTests();
        Prepare(fixture);
        var inputs = ReadInputs(fixture);
        fixture.Dependencies.Checkpoint = point =>
        {
            if (point != checkpoint) return;
            var payload = Directory.GetFiles(fixture.ArchiveRoot,name,SearchOption.AllDirectories).Single();
            File.AppendAllText(payload,"# corrupt but semantically identical\n");
        };
        Assert.Equal(2,fixture.Run("worktree",fixture.Tree));
        Assert.True(fixture.Has("refs/heads/topic"));
        Assert.True(Directory.Exists(fixture.Tree));
        Assert.Equal(inputs,ReadInputs(fixture));
    }

    [Trait("Job", "edge: proof inputs that change between the archive and its restore")]
    [Theory]
    [InlineData("exclude", false)]
    [InlineData("attributes", false)]
    [InlineData("attributes", true)]
    public void ChangedRestoreInputs_RefuseBeforeCreatingRefsAndKeepSharedBytes(string name, bool duringRestore)
    {
        using var fixture = new SafeDeleteTests();
        Prepare(fixture);
        Assert.True(fixture.Run("worktree",fixture.Tree) == 0,fixture.Output);
        var id = fixture.Id;
        if (!duringRestore) File.AppendAllText(Input(fixture,name),"# changed proof input\n");
        fixture.Dependencies.Checkpoint = point =>
        {
            if (duringRestore && point == "before-restore") File.AppendAllText(Input(fixture,name),"# changed proof input\n");
        };
        Assert.Equal(2,fixture.Run("restore",id));
        Assert.Contains("proof inputs changed; shared exclude/attributes retained",fixture.Output);
        Assert.False(fixture.Has("refs/heads/topic"));
        Assert.False(Directory.Exists(fixture.Tree));
        Assert.Empty(SafeDeleteTests.Git(fixture.Main,"for-each-ref","refs/dydo-restored/"+id));
        Assert.EndsWith("# changed proof input\n",File.ReadAllText(Input(fixture,name)));
        Assert.True(Directory.Exists(fixture.Entry));
    }

    [Trait("Job", "contract: DYD-1029 - restore refuses an invalid manifest before it reconstructs into a new path/ref")]
    [Theory]
    [InlineData("missing")]
    [InlineData("unknown path")]
    [InlineData("missing hash")]
    [InlineData("bad hash")]
    [InlineData("null")]
    [InlineData("legacy worktree")]
    [InlineData("future version")]
    public void UnsupportedInputManifest_RefusesRestoreAndPurge(string change)
    {
        using var fixture = new SafeDeleteTests();
        Prepare(fixture);
        Assert.True(fixture.Run("worktree",fixture.Tree) == 0,fixture.Output);
        var id = fixture.Id;
        var path = Path.Combine(fixture.Entry,"manifest.json");
        var json = System.Text.Json.Nodes.JsonNode.Parse(File.ReadAllText(path))!;
        ChangeManifest(json,change);
        File.WriteAllText(path,json.ToJsonString());
        Assert.Equal(2,fixture.Run("restore",id));
        Assert.False(fixture.Has("refs/heads/topic"));
        Assert.False(Directory.Exists(fixture.Tree));
        Assert.Empty(SafeDeleteTests.Git(fixture.Main,"for-each-ref","refs/dydo-restored/"+id));
        fixture.Dependencies.Now = () => DateTimeOffset.UtcNow.AddDays(40);
        SafeDeleteTests.Git(fixture.Main,"branch","another");
        Assert.True(fixture.Run("branch","another") == 0,fixture.Output);
        Assert.True(Directory.Exists(Path.GetDirectoryName(path)));
        Assert.Contains("Retained unrecognized/incomplete archive "+id,fixture.Output);
    }

    private static void ChangeManifest(System.Text.Json.Nodes.JsonNode json, string change)
    {
        switch (change)
        {
            case "missing": json["gitInputs"]!.AsObject().Remove("info/attributes"); break;
            case "unknown path": json["gitInputs"]!["../outside"] = json["gitInputs"]!["info/attributes"]!.GetValue<string>(); break;
            case "missing hash": json["payloadHashes"]!.AsObject().Remove("git-inputs/info/attributes"); break;
            case "bad hash": json["gitInputs"]!["info/attributes"] = "wrong"; break;
            case "null": json["gitInputs"] = null; break;
            case "legacy worktree": json["version"] = 1; json.AsObject().Remove("gitInputs"); break;
            case "future version": json["version"] = 3; break;
            default: throw new ArgumentException("Unknown manifest corruption",nameof(change));
        }
    }

    [Trait("Job", "contract: DYD-1142 - v1 branch archives stay readable")]
    [Fact]
    public void LegacyBranchArchive_RemainsReadable()
    {
        using var fixture = new SafeDeleteTests();
        Assert.True(fixture.Run("branch","topic") == 0,fixture.Output);
        var manifest = fixture.Manifest(); manifest.Version = 1; fixture.Save(manifest);
        Assert.True(fixture.Run("restore",fixture.Id) == 0,fixture.Output);
        Assert.True(fixture.Has("refs/heads/topic"));
    }

    [Trait("Job", "contract: DYD-1133 - preserve ordinary Git behavior during restore")]
    [Theory]
    [InlineData(true, false)]
    [InlineData(false, true)]
    [InlineData(true, true)]
    public void CommonInputs_ArchiveAndRestoreOrdinaryGitBehavior(bool exclude, bool attributes)
    {
        using var fixture = new SafeDeleteTests();
        Prepare(fixture,exclude,attributes);
        var status = SafeDeleteTests.Git(fixture.Tree,"status","--porcelain=v1","--ignored","--untracked-files=all");
        Assert.DoesNotContain("file.txt",status);
        Assert.Contains(exclude ? "!! local-only.log" : "?? local-only.log",status);
        var inputs = ReadInputs(fixture);
        var staged = SafeDeleteTests.Git(fixture.Tree,"rev-parse",":unique.txt");
        Assert.True(fixture.Run("worktree",fixture.Tree) == 0,fixture.Output);
        Assert.False(Directory.Exists(fixture.Tree));
        Assert.False(fixture.Has("refs/heads/topic"));
        Assert.Contains("Independent proof passed",fixture.Output);
        Assert.Equal(inputs,ReadInputs(fixture));
        SafeDeleteTests.Git(fixture.Main,"reflog","expire","--expire=now","--all");
        SafeDeleteTests.Git(fixture.Main,"gc","--prune=now");
        Assert.False(GitRunner.RunIsolated(fixture.Main,new Dictionary<string,string>(),"cat-file","-e",staged).Ok);
        Assert.True(fixture.Run("restore",fixture.Id) == 0,fixture.Output);
        Assert.Equal(status,SafeDeleteTests.Git(fixture.Tree,"status","--porcelain=v1","--ignored","--untracked-files=all"));
        Assert.Equal("private ignored bytes\n",File.ReadAllText(Path.Combine(fixture.Tree,"local-only.log")));
        Assert.Equal(attributes ? "base\r\n" : "base\n",File.ReadAllText(Path.Combine(fixture.Tree,"file.txt")));
        Assert.Equal("unique staged bytes",SafeDeleteTests.Git(fixture.Tree,"show",":unique.txt"));
        Assert.Equal(inputs,ReadInputs(fixture));
        Assert.Equal(attributes ? "file.txt: archive-proof: local" : "file.txt: archive-proof: unspecified",
            SafeDeleteTests.Git(fixture.Tree,"check-attr","archive-proof","--","file.txt"));
    }

    internal static void Prepare(SafeDeleteTests fixture, bool exclude = true, bool attributes = true)
    {
        SafeDeleteTests.Git(fixture.Main,"config","core.autocrlf","false");
        if (exclude) File.WriteAllBytes(Input(fixture,"exclude"),System.Text.Encoding.UTF8.GetBytes("# local rules\r\nlocal-only.log\r\n"));
        if (attributes) File.WriteAllText(Input(fixture,"attributes"),"*.txt text eol=lf archive-proof=local\n");
        fixture.AddTree();
        File.WriteAllText(Path.Combine(fixture.Tree,"local-only.log"),"private ignored bytes\n");
        File.WriteAllText(Path.Combine(fixture.Tree,"file.txt"),"base\r\n");
        if (attributes) SafeDeleteTests.Git(fixture.Tree,"add","file.txt");
        else File.WriteAllText(Path.Combine(fixture.Tree,"file.txt"),"base\n");
        File.WriteAllText(Path.Combine(fixture.Tree,"unique.txt"),"unique staged bytes\n");
        SafeDeleteTests.Git(fixture.Tree,"add","unique.txt");
    }

    private static string Input(SafeDeleteTests fixture, string name) => Path.Combine(fixture.Main,".git","info",name);

    private static string[] ReadInputs(SafeDeleteTests fixture) => new[] { "exclude", "attributes" }
        .Select(name => File.Exists(Input(fixture,name)) ? Convert.ToBase64String(File.ReadAllBytes(Input(fixture,name))) : "absent").ToArray();
}
