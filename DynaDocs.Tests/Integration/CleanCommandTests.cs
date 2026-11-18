namespace DynaDocs.Tests.Integration;

using System.Diagnostics;
using System.Globalization;
using System.Text.Json;
using DynaDocs.Commands;
using DynaDocs.Services;

/// <summary>
/// <c>dydo clean</c> on real temp repositories: a bare <c>origin</c>, a main checkout and linked worktrees
/// under <c>main/.claude/worktrees/</c>, the layout agents use.
/// </summary>
[Collection("Integration")]
[Trait("Job", "contract: dydo-commands-worktrees.md dydo clean; spec N10 (archive before delete)")]
public sealed class CleanCommandTests : IDisposable
{
    private readonly string _root = Path.Combine(Path.GetTempPath(), "dydo-clean-" + Guid.NewGuid().ToString("N")[..8]);
    private readonly string _originalDirectory = Environment.CurrentDirectory;
    private string _main;
    private string _t3Root;

    public CleanCommandTests()
    {
        (_main, _t3Root) = Seed(_root);
    }

    // A bare origin, a main checkout pushed to it and the T3 root beside them, all under root; the
    // current directory is the main checkout.
    private static (string Main, string T3Root) Seed(string root, string objectFormat = "sha1")
    {
        var main = Path.Combine(root, "main");
        Directory.CreateDirectory(main);
        Git(root, "init", "--bare", "-b", "master", $"--object-format={objectFormat}", "origin.git");
        Git(main, "init", "-b", "master", $"--object-format={objectFormat}");
        // No auto maintenance: on Linux it detaches and holds objects/maintenance.lock after the command returns.
        foreach (var repository in new[] { Path.Combine(root, "origin.git"), main })
        {
            Git(repository, "config", "gc.auto", "0");
            Git(repository, "config", "maintenance.auto", "false");
        }
        Git(main, "config", "user.name", "Test");
        Git(main, "config", "user.email", "test@example.com");
        Git(main, "config", "commit.gpgsign", "false");
        File.WriteAllText(Path.Combine(main, ".gitignore"), ".claude/worktrees/\n.claude/skills/\nnode_modules/\nbin/\ndydo/agents/\n");
        File.WriteAllText(Path.Combine(main, "readme.md"), "hello\n");
        File.WriteAllBytes(Path.Combine(main, "logo.bin"), [0, 1, 2, 3, 0, 255]);
        Commit(main, "seed");
        Git(main, "remote", "add", "origin", Path.Combine(root, "origin.git"));
        Git(main, "push", "-u", "origin", "master");
        Git(main, "remote", "set-head", "origin", "master");
        Environment.CurrentDirectory = main;
        return (main, Path.Combine(root, "t3-worktrees"));
    }

    // Replaces the SHA-1 repositories with SHA-256 ones; the test is skipped when this git cannot make them.
    private void UseSha256Repositories()
    {
        var probe = Path.Combine(_root, "sha256-probe");
        Directory.CreateDirectory(probe);
        Skip.IfNot(GitRunner.Run(probe, "init", "--quiet", "--object-format=sha256").Ok, "this git cannot create SHA-256 repositories");
        (_main, _t3Root) = Seed(Path.Combine(_root, "sha256"), "sha256");
    }

    public void Dispose()
    {
        Environment.CurrentDirectory = _originalDirectory;
        LinkSafeDelete.DeleteTree(_root);
    }

    [Fact]
    public async Task Merged_RemovesTheTreeAndDeletesTheBranch()
    {
        var tree = AddWorktree("feature");
        File.WriteAllText(Path.Combine(tree, "feature.txt"), "done\n");
        Commit(tree, "feature");
        // Merged on origin only: the main checkout's master is stale, so git branch -d alone refuses.
        Git(tree, "push", "origin", "feature:master");

        var (exit, _, _) = await Clean("feature");

        Assert.Equal(0, exit);
        Assert.False(Directory.Exists(tree));
        Assert.False(HasBranch("feature"));
        Assert.DoesNotContain("feature", Git(_main, "worktree", "list").Text);
    }

    [Fact]
    public async Task PushedButUnmerged_KeepsTheBranchAndTheRemoteBranch()
    {
        var tree = AddWorktree("feature");
        File.WriteAllText(Path.Combine(tree, "feature.txt"), "wip\n");
        Commit(tree, "wip");
        Git(tree, "push", "-u", "origin", "feature");

        var (exit, _, _) = await Clean(tree);

        Assert.Equal(0, exit);
        Assert.False(Directory.Exists(tree));
        Assert.True(HasBranch("feature"));
        Assert.Contains("refs/heads/feature", Git(_main, "ls-remote", "origin").Text);
    }

    [Fact]
    public async Task NoOriginRemote_KeepsTheBranch()
    {
        Git(_main, "remote", "remove", "origin");
        var tree = AddWorktree("feature");

        var (exit, _, _) = await Clean("feature");

        Assert.Equal(0, exit);
        Assert.True(HasBranch("feature"));
    }

    [Fact]
    public async Task FailedFetch_KeepsTheBranch()
    {
        Git(_main, "remote", "set-url", "origin", Path.Combine(_root, "gone.git"));
        AddWorktree("feature");

        var (exit, _, _) = await Clean("feature");

        Assert.Equal(0, exit);
        Assert.True(HasBranch("feature"));
    }

    [Fact]
    public async Task OriginWithoutADefaultBranch_KeepsTheBranch()
    {
        Git(_main, "remote", "set-head", "origin", "--delete");
        Git(_main, "config", "remote.origin.followRemoteHEAD", "never"); // git 2.48+ would restore origin/HEAD on fetch
        Git(_main, "branch", "-m", "master", "trunk");
        Git(_main, "push", "origin", "trunk");
        // origin's HEAD names a branch that does not exist, so origin advertises no default.
        Git(Path.Combine(_root, "origin.git"), "symbolic-ref", "HEAD", "refs/heads/nothing");
        Git(_main, "push", "origin", "--delete", "master");
        AddWorktree("feature");

        var (exit, _, _) = await Clean("feature");

        Assert.Equal(0, exit);
        Assert.True(HasBranch("feature"));
    }

    [Fact]
    public async Task DefaultBranchChangedOnOrigin_IsJudgedAgainstTheNewDefault_NotAStaleOriginHead()
    {
        Git(_main, "config", "remote.origin.followRemoteHEAD", "never"); // keep the local origin/HEAD stale
        var tree = AddWorktree("feature");
        File.WriteAllText(Path.Combine(tree, "feature.txt"), "done\n");
        Commit(tree, "feature");
        Git(tree, "push", "origin", "feature:master");
        Git(_main, "push", "origin", "HEAD:refs/heads/main"); // main lacks the feature commit
        Git(Path.Combine(_root, "origin.git"), "symbolic-ref", "HEAD", "refs/heads/main");

        var (exit, _, _) = await Clean("feature");

        Assert.Equal(0, exit);
        Assert.True(HasBranch("feature"));
        Assert.Equal("origin/master", Git(_main, "symbolic-ref", "--short", "refs/remotes/origin/HEAD").Text.Trim());
    }

    [Trait("Job", "trap: DYD-363 (a branch moved after the verdict must not be deleted)")]
    [Fact]
    public void BranchFallback_DeletesOnlyTheVerifiedTip_AMovedRefIsKept()
    {
        var tree = AddWorktree("feature");
        File.WriteAllText(Path.Combine(tree, "feature.txt"), "done\n");
        Commit(tree, "feature");
        var verified = Git(tree, "rev-parse", "HEAD").Text.Trim();
        Git(tree, "push", "origin", "feature:master"); // merged on origin; the main checkout's master is stale
        File.WriteAllText(Path.Combine(tree, "late.txt"), "after the verdict\n");
        Commit(tree, "late");
        var moved = Git(tree, "rev-parse", "HEAD").Text.Trim();
        Git(_main, "worktree", "remove", tree);

        var deleted = WorktreeCleaner.DeleteMergedBranch(_main, "feature", verified);

        Assert.False(deleted);
        Assert.Equal(moved, Git(_main, "rev-parse", "refs/heads/feature").Text.Trim());
    }

    [Trait("Job", "trap: DYD-363 (unpushed commits are never deleted)")]
    [Fact]
    public async Task UnpushedCommits_KeepTheBranchAtItsTip()
    {
        var tree = AddWorktree("feature");
        File.WriteAllText(Path.Combine(tree, "local.txt"), "only here\n");
        Commit(tree, "local only");
        var tip = Git(tree, "rev-parse", "HEAD").Text.Trim();

        var (exit, _, _) = await Clean("feature");

        Assert.Equal(0, exit);
        Assert.Equal(tip, Git(_main, "rev-parse", "refs/heads/feature").Text.Trim());
    }

    [Fact]
    public async Task SquashMerged_MultiCommitBranch_IsDeleted()
    {
        var tree = AddWorktree("feature");
        Write(tree, "one.txt", "first\n");
        Commit(tree, "one");
        Write(tree, "two.txt", "second\n");
        Commit(tree, "two");
        SquashIntoOriginMaster("feature");
        Write(_main, "later.txt", "landed after the squash\n");
        Commit(_main, "later");
        Git(_main, "push", "origin", "master");

        var (exit, _, _) = await Clean("feature");

        Assert.Equal(0, exit);
        Assert.False(HasBranch("feature"));
    }

    [Trait("Job", "trap: DYD-363 (work after a squash merge is not merged)")]
    [Fact]
    public async Task SquashMergedThenAnUnmergedCommit_KeepsTheBranch()
    {
        var tree = AddWorktree("feature");
        Write(tree, "one.txt", "first\n");
        Commit(tree, "one");
        SquashIntoOriginMaster("feature");
        Write(tree, "after.txt", "not on origin\n");
        Commit(tree, "after the squash");
        var tip = Git(tree, "rev-parse", "HEAD").Text.Trim();

        var (exit, _, _) = await Clean("feature");

        Assert.Equal(0, exit);
        Assert.Equal(tip, Git(_main, "rev-parse", "refs/heads/feature").Text.Trim());
    }

    // A SHA-256 repository's objects are readable only by a scratch repository of the same format.
    [SkippableFact]
    public async Task SquashMerged_InASha256Repository_IsDeleted()
    {
        UseSha256Repositories();
        var tree = AddWorktree("feature");
        Write(tree, "one.txt", "first\n");
        Commit(tree, "one");
        Write(tree, "two.txt", "second\n");
        Commit(tree, "two");
        SquashIntoOriginMaster("feature");

        var (exit, stdout, stderr) = await Clean("feature");

        Assert.True(exit == 0, stdout + stderr);
        Assert.False(HasBranch("feature"));
    }

    [Trait("Job", "trap: DYD-363 (work after a squash merge is not merged, SHA-256)")]
    [SkippableFact]
    public async Task SquashMergedThenAnUnmergedCommit_InASha256Repository_KeepsTheBranch()
    {
        UseSha256Repositories();
        var tree = AddWorktree("feature");
        Write(tree, "one.txt", "first\n");
        Commit(tree, "one");
        SquashIntoOriginMaster("feature");
        Write(tree, "after.txt", "not on origin\n");
        Commit(tree, "after the squash");
        var tip = Git(tree, "rev-parse", "HEAD").Text.Trim();

        var (exit, stdout, stderr) = await Clean("feature");

        Assert.True(exit == 0, stdout + stderr);
        Assert.Equal(tip, Git(_main, "rev-parse", "refs/heads/feature").Text.Trim());
    }

    [Fact]
    public async Task RebaseMerged_IsDeleted()
    {
        var tree = AddWorktree("feature");
        Write(tree, "one.txt", "first\n");
        Commit(tree, "one");
        Write(tree, "two.txt", "second\n");
        Commit(tree, "two");
        var fork = Git(_main, "rev-parse", "HEAD").Text.Trim();
        Write(_main, "other.txt", "moved master on\n");
        Commit(_main, "other");
        Git(_main, "cherry-pick", $"{fork}..feature");
        Git(_main, "push", "origin", "master");

        var (exit, _, _) = await Clean("feature");

        Assert.Equal(0, exit);
        Assert.False(HasBranch("feature"));
    }

    // A "keep mine" merge driver would resolve the branch's edit away and make the merge look empty.
    [Trait("Job", "trap: DYD-363 (a keep-mine merge driver makes an unmerged change look merged)")]
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task ConfiguredKeepMineMergeDriver_DoesNotHideAnUnmergedChange(bool committedAttributes)
    {
        Write(_main, "F", "1\n2\n3\n4\n5\n6\n7\n8\n");
        if (committedAttributes)
            Write(_main, ".gitattributes", "F merge=keep\n");
        else
            Write(_main, Path.Combine(".git", "info", "attributes"), "F merge=keep\n");
        Commit(_main, "eight lines");
        Git(_main, "push", "origin", "master");
        Git(_main, "config", "merge.keep.driver", "true");
        var tree = AddWorktree("feature");
        Write(tree, "F", "one\n2\n3\n4\n5\n6\n7\n8\n");
        Commit(tree, "line one");
        Write(_main, "F", "1\n2\n3\n4\n5\n6\n7\neight\n");
        Commit(_main, "line eight");
        Git(_main, "push", "origin", "master");

        var (exit, _, _) = await Clean("feature");

        Assert.Equal(0, exit);
        Assert.True(HasBranch("feature"));
    }

    // Config dydo's own environment carries (GIT_CONFIG_COUNT or GIT_CONFIG_PARAMETERS) must not reach the squash check.
    [Trait("Job", "trap: DYD-363 (inherited GIT_CONFIG_* reaches the squash check)")]
    [Theory]
    [InlineData("count", true)]
    [InlineData("count", false)]
    [InlineData("parameters", true)]
    [InlineData("parameters", false)]
    public async Task KeepMineMergeDriverFromTheEnvironment_DoesNotHideAnUnmergedChange(string source, bool dryRun)
    {
        Write(_main, "F", "1\n2\n3\n4\n5\n6\n7\n8\n");
        Commit(_main, "eight lines");
        Git(_main, "push", "origin", "master");
        var tree = AddWorktree("feature");
        Write(tree, "F", "one\n2\n3\n4\n5\n6\n7\n8\n");
        Commit(tree, "line one");
        Write(_main, "F", "1\n2\n3\n4\n5\n6\n7\neight\n");
        Commit(_main, "line eight");
        Git(_main, "push", "origin", "master");
        var keepMine = source == "count"
            ? new Dictionary<string, string?>
            {
                ["GIT_CONFIG_COUNT"] = "2",
                ["GIT_CONFIG_KEY_0"] = "merge.default",
                ["GIT_CONFIG_VALUE_0"] = "keep",
                ["GIT_CONFIG_KEY_1"] = "merge.keep.driver",
                ["GIT_CONFIG_VALUE_1"] = "true"
            }
            : new Dictionary<string, string?>
            {
                ["GIT_CONFIG_PARAMETERS"] = "'merge.default'='keep' 'merge.keep.driver'='true'"
            };

        var (exit, _, _) = await WithEnvironment(keepMine, () => dryRun ? Clean("feature", "--dry-run") : Clean("feature"));

        Assert.Equal(0, exit);
        Assert.True(HasBranch("feature"));
    }

    // A builtin driver needs no config, so system gitattributes ($(prefix)/etc/gitattributes) must be shut out too:
    // the scratch git carries exactly these GIT_* variables, whatever dydo inherits.
    [Trait("Job", "trap: DYD-363 (system gitattributes reach the scratch merge)")]
    [Fact]
    public async Task ScratchGitEnvironment_ShutsOutSystemAttributesAndConfig_WhateverIsInherited()
    {
        var scratch = Path.Combine(_root, "scratch");
        var inherited = new Dictionary<string, string?>
        {
            ["GIT_ATTR_NOSYSTEM"] = "0",
            ["GIT_ATTR_SOURCE"] = "HEAD",
            ["GIT_CONFIG_NOSYSTEM"] = "0",
            ["GIT_CONFIG_SYSTEM"] = Path.Combine(_root, "system-config"),
            ["GIT_CONFIG_GLOBAL"] = Path.Combine(_root, "global-config"),
            ["GIT_TEMPLATE_DIR"] = _root
        };

        var environment = await WithEnvironment(inherited, () => Task.FromResult(
            GitRunner.IsolatedStartInfo(scratch, MergedBranchJudge.ScratchEnvironment(scratch)).Environment));

        var git = environment.Where(entry => entry.Key.StartsWith("GIT_", StringComparison.OrdinalIgnoreCase))
            .ToDictionary(entry => entry.Key.ToUpperInvariant(), entry => entry.Value);
        Assert.Equal(new Dictionary<string, string?>
        {
            ["GIT_ATTR_NOSYSTEM"] = "1",
            ["GIT_CONFIG_NOSYSTEM"] = "1",
            ["GIT_CONFIG_GLOBAL"] = Path.Combine(scratch, "config"),
            ["GIT_TERMINAL_PROMPT"] = "0",
            ["GIT_OPTIONAL_LOCKS"] = "0"
        }.OrderBy(entry => entry.Key), git.OrderBy(entry => entry.Key));
        Assert.Equal(scratch, environment["XDG_CONFIG_HOME"]);
    }

    // A driver a committed .gitattributes names but nothing defines falls back to git's own merge.
    [Fact]
    public async Task UndefinedMergeDriverInCommittedAttributes_StillJudgesBySquash()
    {
        Write(_main, ".gitattributes", "F merge=nowhere\n");
        Write(_main, "F", "1\n2\n");
        Commit(_main, "attributes");
        Git(_main, "push", "origin", "master");
        var tree = AddWorktree("feature");
        Write(tree, "F", "one\n2\n");
        Commit(tree, "line one");
        SquashIntoOriginMaster("feature");

        var (exit, _, _) = await Clean("feature");

        Assert.Equal(0, exit);
        Assert.False(HasBranch("feature"));
    }

    [Trait("Job", "trap: DYD-363 (a reverted squash is not merged)")]
    [Fact]
    public async Task SquashedThenRevertedOnOrigin_KeepsTheBranch()
    {
        var tree = AddWorktree("feature");
        Write(tree, "one.txt", "first\n");
        Commit(tree, "one");
        SquashIntoOriginMaster("feature");
        Git(_main, "revert", "--no-edit", "HEAD");
        Git(_main, "push", "origin", "master");

        var (exit, _, _) = await Clean("feature");

        Assert.Equal(0, exit);
        Assert.True(HasBranch("feature"));
    }

    // Git splits GIT_ALTERNATE_OBJECT_DIRECTORIES on ';' on Windows, so the object store must not travel in it.
    [Fact]
    public async Task SquashMerged_InARepositoryWhosePathHoldsASemicolonAndASpace_IsDeleted()
    {
        var main = Path.Combine(_root, "odd; path", "main");
        Directory.CreateDirectory(Path.GetDirectoryName(main)!);
        Git(_root, "clone", Path.Combine(_root, "origin.git"), main);
        Git(main, "config", "user.name", "Test");
        Git(main, "config", "user.email", "test@example.com");
        Git(main, "config", "commit.gpgsign", "false");
        var tree = Path.Combine(main, ".claude", "worktrees", "feature");
        Git(main, "worktree", "add", "-b", "feature", tree);
        Write(tree, "one.txt", "first\n");
        Commit(tree, "one");
        Git(main, "merge", "--squash", "feature");
        Git(main, "commit", "-m", "squash feature");
        Git(main, "push", "origin", "master");
        Environment.CurrentDirectory = main;

        var (exit, _, _) = await Clean("feature");

        Assert.Equal(0, exit);
        Assert.False(GitRunner.Run(main, "rev-parse", "--verify", "--quiet", "refs/heads/feature").Ok);
    }

    // One branch merges to origin's tree, the other to a new tree: neither verdict may write an object.
    [Trait("Job", "trap: DYD-376 (a dry run writes objects)")]
    [Fact]
    public async Task DryRun_JudgingASquash_WritesNoObject_AndMovesNoRef()
    {
        var squashed = AddWorktree("squashed");
        var extended = AddWorktree("extended");
        Write(squashed, "one.txt", "first\n");
        Commit(squashed, "one");
        SquashIntoOriginMaster("squashed");
        Write(_main, "later.txt", "landed after the squash\n");
        Commit(_main, "later");
        Git(_main, "push", "origin", "master");
        Write(extended, "one.txt", "first\n");
        Commit(extended, "one");
        Write(extended, "two.txt", "only here\n");
        Commit(extended, "two");
        var objectsBefore = AllObjects(_main);
        var refsBefore = Git(_main, "for-each-ref").Text;

        var (exit, _, _) = await Clean("squashed", "extended", "--dry-run");

        Assert.Equal(0, exit);
        Assert.Equal(objectsBefore, AllObjects(_main));
        Assert.Equal(refsBefore, Git(_main, "for-each-ref").Text);
    }

    [Trait("Job", "trap: DYD-362 (uncommitted work is archived before removal)")]
    [Fact]
    public async Task DirtyTrackedChanges_AreArchivedAsAPatchThatReapplies()
    {
        var tree = AddWorktree("feature");
        File.WriteAllText(Path.Combine(tree, "readme.md"), "hello\nstaged line\n");
        Git(tree, "add", "readme.md");
        File.WriteAllBytes(Path.Combine(tree, "logo.bin"), [9, 0, 8, 0, 7]);
        var head = Git(tree, "rev-parse", "HEAD").Text.Trim();

        var (exit, _, _) = await Clean("feature");

        Assert.Equal(0, exit);
        var entry = ArchiveEntries().Single();
        Assert.False(Directory.Exists(tree));

        var fresh = Path.Combine(_root, "fresh");
        Git(_main, "worktree", "add", "--detach", fresh, head);
        Git(fresh, "apply", "--whitespace=nowarn", Path.Combine(entry, "tracked.patch"));
        Assert.Equal("hello\nstaged line\n", File.ReadAllText(Path.Combine(fresh, "readme.md")).Replace("\r\n", "\n"));
        Assert.Equal([9, 0, 8, 0, 7], File.ReadAllBytes(Path.Combine(fresh, "logo.bin")));

        using var manifest = JsonDocument.Parse(File.ReadAllText(Path.Combine(entry, "manifest.json")));
        Assert.Equal(head, manifest.RootElement.GetProperty("head").GetString());
        Assert.Equal("feature", manifest.RootElement.GetProperty("branch").GetString());
        Assert.Equal(2, manifest.RootElement.GetProperty("files").GetArrayLength());
    }

    [Trait("Job", "trap: DYD-362 untracked files survive clean")]
    [Fact]
    public async Task UntrackedFile_SurvivesCleanInTheArchive()
    {
        var tree = AddWorktree("feature");
        Directory.CreateDirectory(Path.Combine(tree, "notes"));
        File.WriteAllBytes(Path.Combine(tree, "notes", "todo.bin"), [1, 0, 2, 0, 3]);

        var (exit, _, _) = await Clean("feature");

        Assert.Equal(0, exit);
        Assert.False(Directory.Exists(tree));
        var entry = ArchiveEntries().Single();
        Assert.Equal([1, 0, 2, 0, 3], File.ReadAllBytes(Path.Combine(entry, "untracked", "notes", "todo.bin")));
    }

    [Fact]
    public async Task Archive_ListsEntriesNewestFirst()
    {
        var older = MakeArchiveEntry(DateTime.UtcNow.AddDays(-2), "old", 10);
        var newer = MakeArchiveEntry(DateTime.UtcNow.AddDays(-1), "new", 10);

        var list = await ListOutput();

        Assert.True(list.IndexOf(Path.GetFileName(newer), StringComparison.Ordinal)
            < list.IndexOf(Path.GetFileName(older), StringComparison.Ordinal), list);
    }

    [Fact]
    public async Task Retention_PrunesEntriesOlderThanNinetyDays()
    {
        var old = MakeArchiveEntry(DateTime.UtcNow.AddDays(-100), "old", 10);
        var recent = MakeArchiveEntry(DateTime.UtcNow.AddDays(-10), "recent", 10);

        var (exit, _, _) = await Clean("--all");

        Assert.Equal(0, exit);
        Assert.False(Directory.Exists(old));
        Assert.True(Directory.Exists(recent));
    }

    [Fact]
    public async Task Retention_PrunesTheOldestEntriesPastTheSizeCap()
    {
        File.WriteAllText(Path.Combine(_main, "dydo.json"), """{ "clean": { "archiveSizeCapMb": 1 } }""");
        var oldest = MakeArchiveEntry(DateTime.UtcNow.AddDays(-3), "a", 400_000);
        var middle = MakeArchiveEntry(DateTime.UtcNow.AddDays(-2), "b", 400_000);
        var newest = MakeArchiveEntry(DateTime.UtcNow.AddDays(-1), "c", 400_000);

        var (exit, _, _) = await Clean("--all");

        Assert.Equal(0, exit);
        Assert.False(Directory.Exists(oldest));
        Assert.True(Directory.Exists(middle));
        Assert.True(Directory.Exists(newest));
    }

    [Fact]
    public async Task InvalidSizeCap_IsRefused()
    {
        File.WriteAllText(Path.Combine(_main, "dydo.json"), """{ "clean": { "archiveSizeCapMb": 0 } }""");
        var tree = AddWorktree("feature");

        var (exit, _, _) = await Clean("--all");

        Assert.Equal(2, exit);
        Assert.True(Directory.Exists(tree));
    }

    [Trait("Job", "trap: DYD-362 (deleting a tree follows a link into its target)")]
    [Fact]
    public async Task LinkInsideTheTree_IsUnlinked_ItsTargetSurvives()
    {
        var tree = AddWorktree("feature");
        var outside = Path.Combine(_root, "outside");
        Write(outside, "keep.txt", "keep me\n");
        DirectoryLinks.Create(Path.Combine(tree, ".claude", "skills"), outside);

        var (exit, _, _) = await Clean("feature");

        Assert.Equal(0, exit);
        Assert.False(Directory.Exists(tree));
        Assert.Equal("keep me\n", File.ReadAllText(Path.Combine(outside, "keep.txt")));
    }

    [Trait("Job", "trap: DYD-362 (archiving copies a linked folder from outside the tree)")]
    [Fact]
    public async Task UntrackedLink_IsNotFollowed()
    {
        var tree = AddWorktree("feature");
        var outside = Path.Combine(_root, "outside");
        Write(outside, "secret.txt", "outside the tree");
        DirectoryLinks.Create(Path.Combine(tree, "linked"), outside);

        var (exit, _, _) = await Clean("feature");

        Assert.Equal(0, exit);
        var entry = ArchiveEntries().Single();
        Assert.Empty(Directory.GetFiles(entry, "secret.txt", SearchOption.AllDirectories));
        Assert.Equal("outside the tree", File.ReadAllText(Path.Combine(outside, "secret.txt")));
    }

    [Fact]
    public async Task UntrackedLink_IsRecordedWithItsTarget_SoItCanBeRecreated()
    {
        var tree = AddWorktree("feature");
        var outside = Path.Combine(_root, "outside");
        Write(outside, "secret.txt", "outside the tree");
        DirectoryLinks.Create(Path.Combine(tree, "linked"), outside);

        var (exit, _, _) = await Clean("feature");

        Assert.Equal(0, exit);
        var entry = ArchiveEntries().Single();
        using var manifest = JsonDocument.Parse(File.ReadAllText(Path.Combine(entry, "manifest.json")));
        var link = manifest.RootElement.GetProperty("links").EnumerateArray().Single();
        Assert.Equal("linked", link.GetProperty("path").GetString());
        Assert.Equal("directory", link.GetProperty("linksTo").GetString());
        Assert.Equal(Path.TrimEndingDirectorySeparator(outside), Path.TrimEndingDirectorySeparator(link.GetProperty("target").GetString()!));
    }

    [Fact]
    public async Task PathLongerThan260Characters_IsRemoved()
    {
        var tree = AddWorktree("feature");
        var deep = Path.Combine(tree, "node_modules", string.Join(Path.DirectorySeparatorChar, Enumerable.Repeat("segment-of-forty-characters-long-xxxxxx", 8)));
        Assert.True(deep.Length > 260);
        Directory.CreateDirectory(LinkSafeDelete.Extended(deep));
        File.WriteAllText(LinkSafeDelete.Extended(Path.Combine(deep, "file.js")), "x");

        var (exit, stdout, stderr) = await Clean("feature");

        Assert.True(exit == 0, stdout + stderr);
        Assert.False(Directory.Exists(LinkSafeDelete.Extended(tree)));
        Assert.DoesNotContain("feature", Git(_main, "worktree", "list").Text);
    }

    [Trait("Job", "trap: DYD-362 (the main checkout is never removed)")]
    [Fact]
    public async Task MainCheckout_IsRefused()
    {
        var (exit, _, _) = await Clean(_main);

        Assert.Equal(2, exit);
        Assert.True(File.Exists(Path.Combine(_main, "readme.md")));
    }

    [Trait("Job", "trap: DYD-362 (removing the tree the shell stands in)")]
    [Fact]
    public async Task TreeHoldingTheCurrentDirectory_IsRefused()
    {
        var tree = AddWorktree("feature");
        var inside = Path.Combine(tree, "sub");
        Directory.CreateDirectory(inside);
        Environment.CurrentDirectory = inside;

        var (exit, _, _) = await Clean(tree);

        Environment.CurrentDirectory = _main;
        Assert.Equal(2, exit);
        Assert.True(Directory.Exists(tree));
    }

    [Trait("Job", "trap: DYD-407 (T3 worktrees are T3's); DYD-362 (locked trees)")]
    [Fact]
    public async Task All_SkipsTheMainCheckout_LockedTrees_AndT3Trees()
    {
        var locked = AddWorktree("locked");
        Git(_main, "worktree", "lock", locked);
        Directory.CreateDirectory(_t3Root);
        var t3 = Path.Combine(_t3Root, "thread");
        Git(_main, "worktree", "add", "-b", "thread", t3);
        var plain = AddWorktree("plain");

        var (exit, _, _) = await Clean("--all");

        Assert.Equal(0, exit);
        Assert.True(Directory.Exists(locked));
        Assert.True(Directory.Exists(t3));
        Assert.False(Directory.Exists(plain));
    }

    [Trait("Job", "trap: DYD-362 (a tree a process still uses)")]
    [Fact]
    public async Task RunningProcessInside_IsSkipped()
    {
        if (!OperatingSystem.IsWindows() && !OperatingSystem.IsLinux())
            return; // documented: macOS has no busy-worktree detection
        var tree = AddWorktree("feature");
        var start = new ProcessStartInfo("git", "hash-object --stdin")
        {
            WorkingDirectory = tree,
            UseShellExecute = false,
            RedirectStandardInput = true,
            RedirectStandardOutput = true,
            CreateNoWindow = true
        };
        using var process = Process.Start(start)!;
        try
        {
            var (exit, _, _) = await Clean("feature");

            Assert.Equal(2, exit);
            Assert.True(Directory.Exists(tree));
        }
        finally
        {
            // Let the Git wrapper wait for its child before fixture deletion.
            process.StandardInput.Close();
            process.WaitForExit();
            Assert.Equal(0, process.ExitCode);
        }
    }

    [Trait("Job", "trap: DYD-362 (a dry run removes or prunes)")]
    [Fact]
    public async Task DryRun_ChangesNothing()
    {
        var tree = AddWorktree("feature");
        Write(tree, "notes.txt", "draft\n");
        var old = MakeArchiveEntry(DateTime.UtcNow.AddDays(-100), "old", 10);

        var (exit, _, _) = await Clean("--all", "--dry-run");

        Assert.Equal(0, exit);
        Assert.True(File.Exists(Path.Combine(tree, "notes.txt")));
        Assert.True(HasBranch("feature"));
        Assert.Equal([old], ArchiveEntries());
    }

    [Fact]
    public async Task DryRun_RunsNoBusyCheck()
    {
        var tree = AddWorktree("feature");
        var probed = new List<string>();
        var cleaner = new WorktreeCleaner(_main, _t3Root, path => { probed.Add(path); return null; });

        var (exit, _, _) = await ConsoleCapture.AllAsync(() => Task.FromResult(cleaner.Clean(["feature"], all: false, dryRun: true)));

        Assert.Equal(0, exit);
        Assert.Empty(probed);
        Assert.True(Directory.Exists(tree));
    }

    [Trait("Job", "trap: DYD-362 (a busy probe strands a tree)")]
    [Fact]
    public async Task BusyProbeThatCannotMoveTheTreeBack_FailsThatTree_LocksIt_AndTheRunGoesOn()
    {
        var stranded = AddWorktree("stranded");
        var other = AddWorktree("other");
        void MoveBackFails(string from, string to)
        {
            if (to == stranded)
                throw new IOException("the folder is held");
            Directory.Move(from, to);
        }

        var cleaner = new WorktreeCleaner(_main, _t3Root,
            path => path == stranded ? WorktreeProcessProbe.RenameProbe(path, MoveBackFails, TimeSpan.Zero) : null);

        var (exit, _, _) = await ConsoleCapture.AllAsync(() => Task.FromResult(cleaner.Clean(["stranded", "other"], all: false, dryRun: false)));

        Assert.Equal(2, exit);
        var probe = Directory.GetDirectories(Path.GetDirectoryName(stranded)!, "stranded.dydo-clean-probe-*").Single();
        Assert.False(Directory.Exists(other));
        var list = Git(_main, "worktree", "list", "--porcelain").Text;
        Assert.Contains("branch refs/heads/stranded\nlocked", list.ReplaceLineEndings("\n"));
        Assert.True(File.Exists(Path.Combine(probe, "readme.md")));
    }

    [Trait("Job", "trap: DYD-362 (submodule work is lost with its tree)")]
    [Theory]
    [InlineData("untracked")]
    [InlineData("modified")]
    [InlineData("unpushed")]
    public async Task SubmoduleHoldingWork_KeepsTheTree(string work)
    {
        var tree = AddWorktree("feature");
        var submodule = AddSubmodule(tree);
        if (work == "untracked")
            Write(submodule, "draft.txt", "only here\n");
        else
            File.WriteAllText(Path.Combine(submodule, "lib.txt"), "changed\n");
        if (work == "unpushed")
            CommitAs(submodule, "local only");
        var problem = work == "unpushed" ? "has commits on no remote ref" : "has uncommitted or untracked changes";

        var (exit, _, _) = await Clean("feature");

        Assert.Equal(2, exit);
        Assert.True(Directory.Exists(submodule));
        Assert.True(HasBranch("feature"));
        Assert.Empty(ArchiveEntries());
    }

    [Fact]
    public async Task CleanPushedSubmodule_GoesWithTheTree()
    {
        var tree = AddWorktree("feature");
        AddSubmodule(tree);

        var (exit, stdout, stderr) = await Clean("feature");

        Assert.True(exit == 0, stdout + stderr);
        Assert.False(Directory.Exists(tree));
    }

    // HEAD is clean and pushed; the unpushed commit sits on a ref HEAD does not reach.
    [Trait("Job", "trap: DYD-362 (submodule commits off HEAD are lost)")]
    [Theory]
    [InlineData("branch")]
    [InlineData("tag")]
    [InlineData("stash")]
    public async Task SubmoduleWithUnpushedWorkOffHead_KeepsTheTree(string where)
    {
        var tree = AddWorktree("feature");
        var submodule = AddSubmodule(tree);
        File.WriteAllText(Path.Combine(submodule, "lib.txt"), "side work\n");
        if (where == "stash")
            Git(submodule, "-c", "user.name=Test", "-c", "user.email=test@example.com", "stash");
        else
        {
            Git(submodule, "checkout", "-b", "side");
            CommitAs(submodule, "side work");
            if (where == "tag")
            {
                Git(submodule, "tag", "side-tag");
                Git(submodule, "checkout", "master");
                Git(submodule, "branch", "-D", "side");
            }
            else
                Git(submodule, "checkout", "master");
        }

        var (exit, _, _) = await Clean("feature");

        Assert.Equal(2, exit);
        Assert.True(Directory.Exists(submodule));
    }

    // The stat cache is stale (a tracked file touched, content unchanged) in the tree and its submodule,
    // so a git call that refreshes the index would rewrite it.
    [Trait("Job", "trap: DYD-362 (a dry run refreshes the index)")]
    [Fact]
    public async Task DryRun_LeavesTheIndexOfTheTreeAndItsSubmoduleUnchanged()
    {
        var tree = AddWorktree("feature");
        var submodule = AddSubmodule(tree);
        var indexes = new[] { tree, submodule }
            .Select(repository => Git(repository, "rev-parse", "--path-format=absolute", "--git-path", "index").Text.Trim())
            .ToList();
        var stale = DateTime.UtcNow.AddHours(-1);
        foreach (var index in indexes)
            File.SetLastWriteTimeUtc(index, stale.AddMinutes(-10));
        File.SetLastWriteTimeUtc(Path.Combine(tree, "readme.md"), stale);
        File.SetLastWriteTimeUtc(Path.Combine(submodule, "lib.txt"), stale);
        var before = indexes.Select(index => (File.ReadAllBytes(index), File.GetLastWriteTimeUtc(index))).ToList();

        var (exit, stdout, stderr) = await Clean("feature", "--dry-run");

        Assert.True(exit == 0, stdout + stderr);
        for (var i = 0; i < indexes.Count; i++)
        {
            Assert.Equal(before[i].Item1, File.ReadAllBytes(indexes[i]));
            Assert.Equal(before[i].Item2, File.GetLastWriteTimeUtc(indexes[i]));
        }
    }

    // The archive root, or the folder above it inside the git directory, links outside: nothing behind
    // the link is pruned, and no work is archived there.
    [Trait("Job", "trap: DYD-362 (pruning follows a link out of the archive)")]
    [Theory]
    [InlineData("clean-archive")]
    [InlineData("dydo")]
    public async Task ArchiveRootBehindALink_IsNeitherPrunedNorWritten(string linked)
    {
        var tree = AddWorktree("feature");
        Write(tree, "notes.txt", "draft\n");
        var outside = Path.Combine(_root, "outside");
        var outsideArchive = linked == "dydo" ? Path.Combine(outside, "clean-archive") : outside;
        var oldEntry = Path.Combine(outsideArchive, "20200101T000000Z-old");
        Write(oldEntry, "tracked.patch", "keep me\n");
        var gitDydo = Path.Combine(_main, ".git", "dydo");
        DirectoryLinks.Create(linked == "dydo" ? gitDydo : Path.Combine(gitDydo, "clean-archive"), outside);

        var (exit, _, _) = await Clean("feature");

        Assert.Equal(2, exit);
        Assert.Equal("keep me\n", File.ReadAllText(Path.Combine(oldEntry, "tracked.patch")));
        Assert.Equal([oldEntry], Directory.GetDirectories(outsideArchive));
        Assert.Equal("draft\n", File.ReadAllText(Path.Combine(tree, "notes.txt")));
    }

    [Fact]
    public async Task SkippedTrees_KeepTheirBranch()
    {
        var locked = AddWorktree("locked");
        Git(_main, "worktree", "lock", locked);
        var detached = Path.Combine(_main, ".claude", "worktrees", "detached");
        Git(_main, "worktree", "add", "--detach", detached);
        Git(_main, "worktree", "lock", detached);

        var (exit, _, _) = await Clean(locked, detached);

        Assert.Equal(2, exit);
        Assert.True(HasBranch("locked"));
        Assert.True(Directory.Exists(locked) && Directory.Exists(detached));
    }

    [Trait("Job", "trap: DYD-362 (a failed archive must remove nothing)")]
    [Fact]
    public async Task ArchiveThatCannotBeWritten_FailsClosed_AndRemovesNothing()
    {
        var tree = AddWorktree("feature");
        Write(tree, "notes.txt", "draft\n");
        var outside = Path.Combine(_root, "outside");
        Write(outside, "keep.txt", "keep me\n");
        var link = Path.Combine(tree, ".claude", "skills");
        DirectoryLinks.Create(link, outside);
        var gone = AddWorktree("gone");
        LinkSafeDelete.DeleteTree(gone); // a prunable admin entry: still listed only if nothing ran git worktree prune
        // A file where the archive folder belongs: creating the entry fails.
        Directory.CreateDirectory(Path.GetDirectoryName(ArchiveRoot)!);
        File.WriteAllText(ArchiveRoot, "not a folder");

        var (exit, _, _) = await Clean("feature");

        Assert.Equal(2, exit);
        Assert.Equal("draft\n", File.ReadAllText(Path.Combine(tree, "notes.txt")));
        Assert.NotNull(new DirectoryInfo(link).LinkTarget);
        Assert.Equal("keep me\n", File.ReadAllText(Path.Combine(outside, "keep.txt")));
        Assert.True(HasBranch("feature"));
        var list = Git(_main, "worktree", "list", "--porcelain").Text;
        Assert.Contains("branch refs/heads/feature", list);
        Assert.Contains("branch refs/heads/gone", list);
    }

    [Theory]
    [InlineData]
    [InlineData("--all", "feature")]
    public async Task NeitherOrBothTargetForms_IsAUsageError(params string[] args)
    {
        var (exit, _, _) = await Clean(args);

        Assert.Equal(2, exit);
    }

    [Fact]
    public async Task UnknownTarget_IsRefused()
    {
        var (exit, _, _) = await Clean("no-such-tree");

        Assert.Equal(2, exit);
        Assert.True(Directory.Exists(_main));
    }

    private async Task<(int Exit, string Stdout, string Stderr)> Clean(params string[] args) =>
        await ConsoleCapture.AllAsync(() => CleanCommand.Create(_t3Root).Parse(args).InvokeAsync());

    // Runs action with variables set in this process, restoring them after; test parallelization is off assembly-wide.
    private static async Task<T> WithEnvironment<T>(IReadOnlyDictionary<string, string?> variables, Func<Task<T>> action)
    {
        var saved = variables.Keys.ToDictionary(name => name, Environment.GetEnvironmentVariable);
        try
        {
            foreach (var (name, value) in variables)
                Environment.SetEnvironmentVariable(name, value);
            return await action();
        }
        finally
        {
            foreach (var (name, value) in saved)
                Environment.SetEnvironmentVariable(name, value);
        }
    }

    private async Task<string> ListOutput()=> (await Clean("--archive")).Stdout;

    private string AddWorktree(string branch)
    {
        var path = Path.Combine(_main, ".claude", "worktrees", branch);
        Git(_main, "worktree", "add", "-b", branch, path);
        return path;
    }

    private string ArchiveRoot => Path.Combine(_main, ".git", "dydo", "clean-archive");

    private List<string> ArchiveEntries() =>
        Directory.Exists(ArchiveRoot) ? [.. Directory.GetDirectories(ArchiveRoot).Order(StringComparer.Ordinal)] : [];

    private string MakeArchiveEntry(DateTime utc, string label, int bytes)
    {
        var path = Path.Combine(ArchiveRoot, utc.ToString("yyyyMMdd'T'HHmmss'Z'", CultureInfo.InvariantCulture) + "-" + label);
        Directory.CreateDirectory(path);
        File.WriteAllBytes(Path.Combine(path, "tracked.patch"), new byte[bytes]);
        return path;
    }

    private static void Write(string root, string relativePath, string content)
    {
        var path = Path.Combine(root, relativePath);
        Directory.CreateDirectory(Path.GetDirectoryName(path)!);
        File.WriteAllText(path, content);
    }

    // A submodule at tree/lib, cloned from a repository under the temp root and committed in the tree.
    private string AddSubmodule(string tree)
    {
        var library = Path.Combine(_root, "lib-origin");
        if (!Directory.Exists(library))
        {
            Directory.CreateDirectory(library);
            Git(library, "init", "-b", "master");
            File.WriteAllText(Path.Combine(library, "lib.txt"), "library\n");
            CommitAs(library, "library");
        }

        Git(tree, "-c", "protocol.file.allow=always", "submodule", "add", library, "lib");
        Commit(tree, "add the library");
        return Path.Combine(tree, "lib");
    }

    // A commit in a repository that has no identity of its own configured.
    private static void CommitAs(string directory, string message)
    {
        Git(directory, "add", "-A");
        Git(directory, "-c", "user.name=Test", "-c", "user.email=test@example.com", "-c", "commit.gpgsign=false",
            "commit", "-m", message);
    }

    // Every object in the store, loose or packed, one "<id> <type> <size>" line each, sorted; packing changes no line.
    private static string[] AllObjects(string repository) =>
        [.. Git(repository, "cat-file", "--batch-all-objects", "--batch-check").Text
            .Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Order(StringComparer.Ordinal)];

    private static void Commit(string directory, string message)
    {
        Git(directory, "add", "-A");
        Git(directory, "commit", "-m", message);
    }

    // Lands branch's changes on origin's master as one squash commit from the main checkout.
    private void SquashIntoOriginMaster(string branch)
    {
        Git(_main, "merge", "--squash", branch);
        Git(_main, "commit", "-m", $"squash {branch}");
        Git(_main, "push", "origin", "master");
    }

    private bool HasBranch(string branch) =>
        GitRunner.Run(_main, "rev-parse", "--verify", "--quiet", $"refs/heads/{branch}").Ok;

    private static GitResult Git(string directory, params string[] args)
    {
        var result = GitRunner.Run(directory, args);
        Assert.True(result.Ok, $"git {string.Join(' ', args)} failed: {result.Error}");
        return result;
    }
}