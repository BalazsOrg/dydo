namespace DynaDocs.Tests.Integration;

using DynaDocs.Commands;
using DynaDocs.Services;
using System.CommandLine;
using System.Text.Json;
using DynaDocs.Models;
using DynaDocs.Serialization;

[Collection("Integration")]
public sealed class SafeDeleteTests : IDisposable
{
    internal readonly string Root = Path.Combine(Path.GetTempPath(), "dydo-safe-delete-" + Guid.NewGuid().ToString("N"));
    internal string Main => Path.Combine(Root, "main");
    internal string Tree => Path.Combine(Root, "tree");
    internal string Remote => Path.Combine(Root, "origin.git");
    internal string ArchiveRoot => Path.Combine(Main, ".git", "dydo", "safe-delete-archive");
    internal readonly SafeDeleteDependencies Dependencies = new()
    {
        CheckGitHub = _ => null,
        FindUse = _ => null,
        T3 = new SafeDeleteT3Policy(Path.Combine(Path.GetTempPath(), "dydo-no-t3-settings.json"))
    };
    private readonly string _original = Environment.CurrentDirectory;
    private readonly StringWriter _output = new();
    internal string Output => _output.ToString();
    internal string Entry => Directory.GetDirectories(ArchiveRoot).Single(p => !Path.GetFileName(p).StartsWith('.'));
    internal string Id => Path.GetFileName(Entry);

    public SafeDeleteTests()
    {
        Directory.CreateDirectory(Main);
        Git(Main, "init", "-b", "main");
        Git(Main, "config", "user.name", "Safe Delete Test");
        Git(Main, "config", "user.email", "safe-delete@example.invalid");
        Git(Main, "config", "commit.gpgsign", "false");
        Git(Main, "config", "gc.auto", "0");
        Git(Main, "config", "maintenance.auto", "false");
        File.WriteAllText(Path.Combine(Main, "file.txt"), "base\n");
        File.WriteAllText(Path.Combine(Main, ".gitignore"), "ignored/\n");
        Git(Main, "add", ".");
        Git(Main, "commit", "-m", "seed");
        Git(Main, "branch", "topic");
        File.WriteAllText(Path.Combine(Main, "dydo.json"), "{\"archive\":{\"reserveBytes\":0,\"reservePercent\":0}}");
    }

    internal static string Git(string cwd, params string[] args)
    {
        var result = GitRunner.RunIsolated(cwd, new Dictionary<string, string>(), args);
        Assert.True(result.Ok, $"git {string.Join(' ', args)}: {result.Error}");
        return result.Text.TrimEnd('\r', '\n');
    }

    internal int Run(params string[] args) => ArchiveCommand.Create(Main, Dependencies, _output).Parse(args).Invoke();
    internal void AddTree(bool detached = false) => Git(Main, "worktree", "add", detached ? "--detach" : "--checkout", Tree, "topic");
    internal void AddRemote()
    {
        Git(Root, "init", "--bare", "-b", "main", Remote);
        Git(Main, "remote", "add", "origin", Remote);
        Git(Main, "push", "origin", "main", "topic");
    }
    internal bool Has(string reference) => GitRunner.RunIsolated(Main, new Dictionary<string,string>(), "show-ref", "--verify", "--quiet", reference).Ok;

    [Fact] public void LocalBranch_RestoresWithoutSourceRefs() => Journey("local branch")();
    [Fact] public void UnfetchedRemote_RestoresRemoteOnlyTip() => Journey("unfetched remote")();
    [Fact] public void DivergentAndReflogTips_RemainReachableAfterGc() => Journey("divergent and reflog history")();
    [Fact] public void PartiallyStagedVersions_IgnoredBinary_EmptyDirectories_RoundTripAfterGc() => Journey("partially staged ignored binary and empty directories")();
    [Fact] public void DetachedHead_RemainsDetached() => Journey("detached HEAD")();
    [Fact] public void ResolvedStashConflict_UndoRecreatesConflictAfterGc() => Journey("resolved stash conflict undo")();

    internal Action Journey(string journey)
    {
        if (journey is not ("local branch" or "unfetched remote" or "divergent and reflog history"
            or "partially staged ignored binary and empty directories" or "detached HEAD"
            or "resolved stash conflict undo" or "local-only restore" or "explicit remote restore"
            or "proof-enabled settled T3 eligibility"))
            throw new Xunit.Sdk.XunitException("Unwired journey: " + journey);
        if (journey == "resolved stash conflict undo") return ResolvedStashConflictUndo();
        if (journey == "proof-enabled settled T3 eligibility")
        {
            AddTree();
            var primary = Git(Tree,"rev-parse","HEAD");
            Dependencies.T3 = new SafeDeleteFakeT3Policy(null, routeProofEnabled: true);
            Assert.True(Run("worktree", Tree) == 0, Output);
            Assert.False(Directory.Exists(Tree));
            Assert.True(Run("restore", Id) == 0, Output);
            return () =>
            {
                Assert.Equal(primary,Git(Tree,"rev-parse","HEAD"));
                Assert.True(Has("refs/heads/topic"));
                Assert.Equal("base\n",File.ReadAllText(Path.Combine(Tree,"file.txt")));
            };
        }
        var worktree = journey is "partially staged ignored binary and empty directories" or "detached HEAD";
        if (journey is "unfetched remote" or "divergent and reflog history" or "explicit remote restore" or "local-only restore") AddRemote();
        if (journey == "unfetched remote") Git(Main, "update-ref", "-d", "refs/heads/topic");
        string? divergentRemote = null;
        string? reflogOnly = null;
        if (journey == "divergent and reflog history")
        {
            divergentRemote = Git(Remote,"-c","user.name=Test","-c","user.email=test@example.invalid","commit-tree",Git(Main,"rev-parse","main^{tree}"),"-p",Git(Main,"rev-parse","topic"),"-m","remote divergent");
            Git(Remote,"update-ref","refs/heads/topic",divergentRemote);
            AddTree();
            File.WriteAllText(Path.Combine(Tree,"unique.txt"),"reflog only\n");
            Git(Tree,"add","."); Git(Tree,"commit","-m","reflog");
            reflogOnly = Git(Tree,"rev-parse","HEAD");
            Git(Tree,"reset","--hard","HEAD~1");
            File.WriteAllText(Path.Combine(Tree,"local.txt"),"local\n");
            Git(Tree,"add","."); Git(Tree,"commit","-m","local");
            Git(Main,"worktree","remove",Tree);
        }
        string? staged = null;
        if (worktree)
        {
            AddTree(journey == "detached HEAD");
            File.WriteAllText(Path.Combine(Tree,"file.txt"),"staged only\n"); Git(Tree,"add","file.txt");
            staged = Git(Tree,"rev-parse",":file.txt");
            File.WriteAllText(Path.Combine(Tree,"file.txt"),"working version\n");
            Directory.CreateDirectory(Path.Combine(Tree,"ignored","empty"));
            File.WriteAllBytes(Path.Combine(Tree,"ignored","binary"),[0,255,1,0]);
        }
        var primaryTip = Git(journey == "unfetched remote" ? Remote : Main,"rev-parse","refs/heads/topic");
        Assert.True(Run(worktree ? ["worktree",Tree] : ["branch","topic"]) == 0, Output);
        var id = Id;
        Assert.False(worktree ? Directory.Exists(Tree) : Has("refs/heads/topic"));
        Git(Main,"reflog","expire","--expire=now","--all"); Git(Main,"gc","--prune=now");
        if (journey == "divergent and reflog history")
        {
            Assert.False(GitRunner.RunIsolated(Main,new Dictionary<string,string>(),"cat-file","-e",divergentRemote!).Ok);
            Assert.False(GitRunner.RunIsolated(Main,new Dictionary<string,string>(),"cat-file","-e",reflogOnly!).Ok);
        }
        Assert.True(Run(journey == "explicit remote restore" ? ["restore",id,"--remote"] : ["restore",id]) == 0, Output);
        return () => AssertJourney(journey,staged,divergentRemote,reflogOnly,id,primaryTip);
    }

    private void AssertJourney(string journey, string? staged, string? divergentRemote, string? reflogOnly, string id, string primaryTip)
    {
        Assert.Contains("refs/dydo-restored/",Output);
        if (staged != null)
        {
            Assert.Equal(staged,Git(Tree,"rev-parse",":file.txt"));
            Assert.Equal("staged only",Git(Tree,"show",":file.txt"));
            Assert.Equal("working version\n",File.ReadAllText(Path.Combine(Tree,"file.txt")));
            Assert.Equal(new byte[]{0,255,1,0},File.ReadAllBytes(Path.Combine(Tree,"ignored","binary")));
            Assert.True(Directory.Exists(Path.Combine(Tree,"ignored","empty")));
            if (journey == "detached HEAD") Assert.False(GitRunner.Run(Tree,"symbolic-ref","-q","HEAD").Ok);
        }
        else Assert.Equal(primaryTip,Git(Main,"rev-parse","refs/heads/topic"));
        if (journey == "divergent and reflog history")
        {
            Git(Main,"reflog","expire","--expire=now","--all"); Git(Main,"gc","--prune=now");
            var recovered = Git(Main,"for-each-ref","--format=%(objectname)","refs/dydo-restored/"+id).Split('\n');
            Assert.Contains(divergentRemote!,recovered); Assert.Contains(reflogOnly!,recovered);
            Assert.Equal("remote divergent",Git(Main,"show","-s","--format=%s",divergentRemote!));
            Assert.Equal("reflog",Git(Main,"show","-s","--format=%s",reflogOnly!));
        }
        if (journey is "local-only restore" or "explicit remote restore")
            Assert.Equal(journey == "explicit remote restore",Git(Remote,"for-each-ref","refs/heads/topic").Length > 0);
    }

    // Ordinary stash workflow: after resolving the popped conflict and dropping the stash, the stashed
    // version survives only in the index's resolve-undo record, so the archive alone must preserve it.
    private Action ResolvedStashConflictUndo()
    {
        AddTree();
        var file = Path.Combine(Tree,"file.txt");
        File.WriteAllText(file,"unique stashed version\n");
        Git(Tree,"stash","push","-m","safe-delete stash conflict");
        var stashed = Git(Tree,"rev-parse","stash@{0}:file.txt");
        File.WriteAllText(file,"committed topic version\n");
        Git(Tree,"commit","-am","topic change");
        Assert.False(GitRunner.RunIsolated(Tree,new Dictionary<string,string>(),"stash","pop").Ok);
        File.WriteAllText(file,"resolved working version\n");
        Git(Tree,"add","file.txt");
        Git(Tree,"stash","drop");
        var undo = Git(Tree,"ls-files","--resolve-undo");
        Assert.Contains($" {stashed} 3\tfile.txt",undo);
        var resolved = Git(Tree,"rev-parse",":file.txt");
        Assert.True(Run("worktree",Tree) == 0,Output);
        var id = Id;
        Assert.False(Directory.Exists(Tree));
        Git(Main,"reflog","expire","--expire=now","--all"); Git(Main,"gc","--prune=now");
        Assert.False(GitRunner.RunIsolated(Main,new Dictionary<string,string>(),"cat-file","-e",stashed).Ok);
        Assert.True(Run("restore",id) == 0,Output);
        return () =>
        {
            Assert.Equal(undo,Git(Tree,"ls-files","--resolve-undo"));
            Assert.Equal(resolved,Git(Tree,"rev-parse",":file.txt"));
            Assert.Equal("resolved working version\n",File.ReadAllText(file));
            Assert.Equal("unique stashed version",Git(Tree,"cat-file","blob",stashed));
            Git(Tree,"checkout","-m","--","file.txt");
            Assert.Equal(undo,Git(Tree,"ls-files","--unmerged"));
            var conflict = File.ReadAllText(file);
            Assert.Contains("committed topic version",conflict); Assert.Contains("unique stashed version",conflict);
        };
    }

    internal Action Refusal(string refusal)
    {
        AddRemote();
        AddTree();
        string[] command = ["worktree", Tree];
        string? expected = null;
        FileStream? held = null;
        var branch = "topic";
        switch (refusal)
        {
            case "local default": command = ["branch", "main"]; expected = "Local default"; break;
            case "remote default":
                Git(Remote,"symbolic-ref","HEAD","refs/heads/topic"); expected = "Remote default"; break;
            case "unknown remote default":
                Git(Remote,"symbolic-ref","HEAD","refs/heads/absent"); expected = "unknown"; break;
            case "main worktree": command = ["worktree",Main]; expected = "protected"; break;
            case "current worktree":
                command = ["worktree",Tree];
                Dependencies.Checkpoint = _ => { };
                var currentExit = new SafeDeleteService(Tree,Dependencies,_output).Archive("worktree",Tree,null,false);
                return () =>
                {
                    Assert.Equal(2,currentExit);
                    Assert.Contains("Current worktree",Output);
                    Assert.True(Has("refs/heads/topic"));
                    Assert.Equal("base\n",File.ReadAllText(Path.Combine(Tree,"file.txt")));
                };
            case "locked worktree": Git(Main,"worktree","lock",Tree); expected = "Locked"; break;
            case "busy worktree": Dependencies.FindUse = _ => "injected busy worktree"; expected = "busy"; break;
            case "checked out branch": command = ["branch","topic"]; expected = "checked out"; break;
            case "missing worktree": command = ["worktree",Path.Combine(Root,"missing")]; expected = "registered"; break;
            case "prunable worktree": LinkSafeDelete.DeleteTree(Tree); expected = "Missing/prunable"; break;
            case "feature remote":
                Git(Tree,"branch","-m","feature/protected"); Git(Main,"push","origin","feature/protected");
                branch = "feature/protected"; expected = "feature/*"; break;
            case "PR head": case "PR base": case "offline GitHub":
                Dependencies.CheckGitHub = _ => refusal; expected = refusal; break;
            case "fetch failure":
                Dependencies.Checkpoint = point => { if (point == "before-fetch") throw new IOException("injected fetch failure"); };
                expected = "fetch failure"; break;
            case "copy failure":
                Dependencies.Checkpoint = point => { if (point == "before-copy") throw new IOException("injected copy failure"); };
                expected = "copy failure"; break;
            case "unreadable source": case "nested repository": case "unsupported file":
                held = SetupUnsupported(refusal); break;
            case "in-progress Git":
                File.WriteAllText(Git(Tree,"rev-parse","--git-path","MERGE_HEAD"),Git(Tree,"rev-parse","HEAD")); expected = "In-progress"; break;
            case "local ref race": case "remote ref race": case "source file race":
                SetRace(refusal);
                expected = "changed"; break;
            case "live T3 owner": case "unavailable T3 ownership": case "unproven settled T3 route":
                Dependencies.T3 = new SafeDeleteFakeT3Policy(refusal); expected = refusal; break;
            case "custom T3 root": case "previous T3 root":
                var settings = Path.Combine(Root,"t3-settings.json");
                var key = refusal == "custom T3 root" ? "worktreesDirectory" : "previousWorktreesDirectories";
                File.WriteAllText(settings,refusal == "custom T3 root"
                    ? JsonSerializer.Serialize(new Dictionary<string,string>{{key,Root}})
                    : JsonSerializer.Serialize(new Dictionary<string,string[]>{{key,[Root]}}));
                Dependencies.T3 = new SafeDeleteT3Policy(settings); expected = "T3 ownership/activity unavailable"; break;
            default: throw new Xunit.Sdk.XunitException("Unwired refusal: " + refusal);
        }
        try { return AssertRefusal(refusal, expected, branch, command); }
        finally { held?.Dispose(); }
    }

    private FileStream? SetupUnsupported(string refusal)
    {
        FileStream? held = null;
        switch (refusal)
        {
            case "unreadable source":
                if (OperatingSystem.IsWindows()) held = new FileStream(Path.Combine(Tree,"file.txt"),FileMode.Open,FileAccess.ReadWrite,FileShare.None);
                else File.SetUnixFileMode(Path.Combine(Tree,"file.txt"),0);
                break;
            case "nested repository":
                Directory.CreateDirectory(Path.Combine(Tree,"nested")); Git(Path.Combine(Tree,"nested"),"init"); break;
            case "unsupported file":
                if (OperatingSystem.IsLinux())
                {
                    var start = new System.Diagnostics.ProcessStartInfo("mkfifo"); start.ArgumentList.Add(Path.Combine(Tree,"pipe"));
                    using var process = System.Diagnostics.Process.Start(start)!; process.WaitForExit(); Assert.Equal(0,process.ExitCode);
                }
                else File.CreateSymbolicLink(Path.Combine(Tree,"unsupported-link"),"file.txt");
                break;
        }
        return held;
    }

    private void SetRace(string refusal)
    {
                Dependencies.Checkpoint = point =>
                {
                    if (point != "before-delete") return;
                    if (refusal == "source file race") File.WriteAllText(Path.Combine(Tree,"file.txt"),"new concurrent work\n");
                    else
                    {
                        Git(Main,"commit","--allow-empty","-m","concurrent");
                        if (refusal == "remote ref race") Git(Main,"push","origin","main");
                        Git(refusal == "local ref race" ? Main : Remote,"update-ref","refs/heads/topic",Git(Main,"rev-parse","HEAD"));
                    }
                };
    }

    private Action AssertRefusal(string refusal, string? expected, string branch, string[] command)
    {
        var originalLocal = Git(Main,"rev-parse","refs/heads/"+branch);
        var originalRemote = Git(Remote,"rev-parse","refs/heads/"+branch);
        var exit = Run(command);
        return () =>
        {
            Assert.Equal(2,exit);
            if (expected != null) Assert.Contains(expected,Output,StringComparison.OrdinalIgnoreCase);
            Assert.True(Has("refs/heads/"+branch));
            if (refusal != "local ref race") Assert.Equal(originalLocal,Git(Main,"rev-parse","refs/heads/"+branch));
            else Assert.Equal(Git(Main,"rev-parse","HEAD"),Git(Main,"rev-parse","refs/heads/topic"));
            if (refusal != "remote ref race") Assert.Equal(originalRemote,Git(Remote,"rev-parse","refs/heads/"+branch));
            else Assert.Equal(Git(Main,"rev-parse","HEAD"),Git(Remote,"rev-parse","refs/heads/topic"));
            if (refusal != "prunable worktree") Assert.True(Directory.Exists(Tree));
            if (refusal != "prunable worktree" && refusal != "unreadable source")
                Assert.Equal(refusal == "source file race" ? "new concurrent work\n" : "base\n",File.ReadAllText(Path.Combine(Tree,"file.txt")));
        };
    }

    internal SafeDeleteManifest Manifest() => JsonSerializer.Deserialize(File.ReadAllText(Path.Combine(Entry,"manifest.json")),SafeDeleteJsonContext.Default.SafeDeleteManifest)!;
    internal void Save(SafeDeleteManifest manifest) => new SafeDeleteStore(ArchiveRoot,_output).Save(Entry,manifest);

    internal Action Policy(string policy)
    {
        switch (policy)
        {
            case "default byte reserve": case "default percentage reserve": case "configured reserve": case "invalid reserve":
                return Reserve(policy);
            case "dry-run estimates": case "dry-run remote lower bound":
                return DryRun(policy);
            case "expiry boundary": case "read-only commands never purge":
                return Retention(policy);
            case "init archive prefix rules":
                return () =>
                {
                    Assert.Contains("[\"dydo\", \"dydo.exe\"], \"archive\"",InitCommand.CodexRules);
                    Assert.Contains("dydo archive branch topic",InitCommand.CodexRules);
                    Assert.Contains("dydo archive worktree tree",InitCommand.CodexRules);
                    Assert.Contains("dydo archive restore ID",InitCommand.CodexRules);
                };
            case "differently named upstream kept":
                AddRemote(); Git(Main,"branch","feature/integration"); Git(Main,"push","origin","feature/integration");
                Git(Main,"branch","--set-upstream-to=origin/feature/integration","topic");
                var integration = Git(Remote,"rev-parse","refs/heads/feature/integration");
                Assert.True(Run("branch","topic") == 0,Output);
                return () =>
                {
                    Assert.Equal(integration,Git(Remote,"rev-parse","refs/heads/feature/integration"));
                    Assert.Contains("upstream",Output);
                };
            case "link round trip or refusal":
                AddTree(); File.CreateSymbolicLink(Path.Combine(Tree,"link"),"file.txt");
                var exit = Run("worktree",Tree);
                if (OperatingSystem.IsWindows()) return () => { Assert.Equal(2,exit); Assert.True(Directory.Exists(Tree)); };
                Assert.True(exit == 0,Output);
                var restored = Run("restore",Id);
                return () =>
                {
                    Assert.Equal(0,restored);
                    Assert.Equal("file.txt",new FileInfo(Path.Combine(Tree,"link")).LinkTarget);
                };
            case "partial deletion failure":
                AddRemote(); AddTree(); Dependencies.Checkpoint = point => { if (point == "before-worktree-delete") throw new IOException("injected removal failure"); };
                var partialExit = Run("worktree",Tree);
                return () =>
                {
                    Assert.Equal(2,partialExit);
                    Assert.True(Manifest().Complete); Assert.True(Directory.Exists(Tree)); Assert.True(Has("refs/heads/topic"));
                    Assert.Empty(Git(Remote,"for-each-ref","refs/heads/topic")); Assert.Contains("remote deleted",Output);
                };
            case "archive symlink":
                Directory.CreateDirectory(Path.GetDirectoryName(ArchiveRoot)!); Directory.CreateSymbolicLink(ArchiveRoot,Root);
                var symlinkExit = Run("branch","topic");
                return () => { Assert.Equal(2,symlinkExit); Assert.True(Has("refs/heads/topic")); };
        }
        if (policy == "remote restore collision") AddRemote();
        Assert.True(Run("branch","topic") == 0,Output);
        var id = Id;
        var manifest = Manifest();
        switch (policy)
        {
            case "empty bundle":
                File.WriteAllBytes(Path.Combine(Entry,"commits.bundle"),[]); manifest.PayloadHashes["commits.bundle"] = SafeDeletePaths.Hash(Path.Combine(Entry,"commits.bundle")); Save(manifest); break;
            case "corrupt payload": File.AppendAllText(Path.Combine(Entry,"commits.bundle"),"corruption"); break;
            case "incomplete archive": manifest.Complete = false; Save(manifest); break;
            case "restore collision": Git(Main,"branch","topic"); break;
            case "recovery ref collision":
                Git(Main,"commit","--allow-empty","-m","collision"); Git(Main,"update-ref",manifest.Tips[0].RecoveryRef(id),Git(Main,"rev-parse","HEAD")); break;
            case "remote restore collision":
                Git(Main,"push","origin","main:topic");
                var remoteExit = Run("restore",id,"--remote");
                return () => { Assert.Equal(2,remoteExit); Assert.True(Directory.Exists(Entry)); };
            case "restore failure": Dependencies.Checkpoint = point => { if (point == "before-restore") throw new IOException("injected restore failure"); }; break;
            case "archive traversal":
                manifest.Files.Add(new SafeDeleteFile("../escape","file","",0,0,0,null)); Save(manifest); break;
            default: throw new Xunit.Sdk.XunitException("Unwired policy: " + policy);
        }
        var restoreExit = Run("restore",id);
        return () =>
        {
            Assert.Equal(2,restoreExit); Assert.True(Directory.Exists(Entry));
            if (policy != "restore collision") Assert.False(Has("refs/heads/topic"));
        };
    }

    private Action Reserve(string policy)
    {
        const long gib = 1024L*1024*1024;
        var config = policy switch
        {
            "configured reserve" => "{\"archive\":{\"reserveBytes\":0,\"reservePercent\":20}}",
            "invalid reserve" => "{\"archive\":{\"reserveBytes\":-1}}",
            _ => "{}"
        };
        File.WriteAllText(Path.Combine(Main,"dydo.json"),config);
        Dependencies.Space = _ => policy == "default byte reserve" ? (5*gib,10*gib) : (9*gib,100*gib);
        var exit = Run("branch","topic");
        return () =>
        {
            Assert.Equal(2,exit); Assert.True(Has("refs/heads/topic"));
            Assert.Contains(policy == "invalid reserve" ? "nonnegative" : "reserve",Output);
        };
    }

    private Action DryRun(string policy)
    {
        if (policy == "dry-run remote lower bound")
        {
            AddRemote();
            var tip = Git(Remote,"-c","user.name=Test","-c","user.email=test@example.invalid","commit-tree",Git(Main,"rev-parse","main^{tree}"),"-p",Git(Main,"rev-parse","topic"),"-m","remote only");
            Git(Remote,"update-ref","refs/heads/topic",tip);
            Assert.False(GitRunner.Run(Main,"cat-file","-e",tip).Ok);
        }
        var exit = Run("branch","topic","--dry-run");
        return () =>
        {
            Assert.True(exit == 0,Output);
            Assert.False(Directory.Exists(ArchiveRoot)); Assert.True(Has("refs/heads/topic"));
            Assert.Contains("archiveBytes=",Output); Assert.Contains("proofScratchBytes=",Output); Assert.Contains("sumBytes=",Output);
            if (policy == "dry-run remote lower bound") Assert.Contains("unknown; lower-bound",Output);
        };
    }

    private Action Retention(string policy)
    {
        var now = new DateTimeOffset(2026,1,1,12,0,0,TimeSpan.Zero);
        Dependencies.Now = () => now;
        Assert.True(Run("branch","topic") == 0,Output);
        var entry = Entry; var id = Id;
        now = now.AddDays(30);
        Git(Main,"branch","second");
        Assert.True(Run("branch","second") == 0,Output); Assert.True(Directory.Exists(entry));
        now = now.AddTicks(1);
        if (policy == "read-only commands never purge")
        {
            Assert.Equal(0,Run("list")); Assert.True(Directory.Exists(entry));
            Git(Main,"branch","third"); Assert.Equal(0,Run("branch","third","--dry-run")); Assert.True(Directory.Exists(entry));
            var exit = Run("restore",id);
            return () => { Assert.Equal(0,exit); Assert.True(Directory.Exists(entry)); };
        }
        else
        {
            Git(Main,"branch","third"); var exit = Run("branch","third");
            return () => { Assert.True(exit == 0,Output); Assert.False(Directory.Exists(entry)); };
        }
    }

    public void Dispose()
    {
        Environment.CurrentDirectory = _original;
        LinkSafeDelete.DeleteTree(Root);
        _output.Dispose();
    }
}
