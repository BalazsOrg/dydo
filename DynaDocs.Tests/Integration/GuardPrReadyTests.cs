namespace DynaDocs.Tests.Integration;

using System.Text.Json;
using System.Text.Json.Nodes;
using DynaDocs.Commands;
using DynaDocs.Services;
using static DynaDocs.Tests.Integration.PushedRepository;

/// <summary>
/// The guard's PR-ready rule through the real hook entry point (DYD-374, DYD-391): the project is a
/// git tree pushed to a bare origin, and a PR-opening gh call is refused while the tree it opens the
/// PR from is not ready, wherever and however the call is written. Each table row names the tree,
/// the lane, the command and the outcome; refusal text is not asserted.
/// </summary>
[Collection("Integration")]
[Trait("Job", "contract: DYD-374/DYD-391 pr-ready guard, DR 061")]
public class GuardPrReadyTests : IntegrationTestBase
{
    public enum Tree
    {
        /// <summary>Clean and pushed: HEAD matches origin/feature.</summary>
        Clean,
        /// <summary>Clean main with an untracked wip.txt.</summary>
        Dirty,
        /// <summary>Clean main and a clean pushed worktree wt on branch topic.</summary>
        CleanWithWorktree,
        /// <summary>Dirty main (wip.txt) and a clean pushed worktree on topic.</summary>
        DirtyMainCleanWorktree,
        /// <summary>Clean main and a dirty worktree on topic (draft.txt).</summary>
        DirtyWorktree,
        /// <summary>Both main (wip.txt) and the topic worktree (draft.txt) dirty.</summary>
        BothDirty,
        /// <summary>No git repository at all.</summary>
        OutsideGit,
    }

    public enum Outcome
    {
        /// <summary>The first try is allowed.</summary>
        Allowed,
        /// <summary>The identical retry is allowed: at most the once-only review-block warn meets the first.</summary>
        Passes,
        /// <summary>The first try is refused and the identical retry passes: a warn, not a block.</summary>
        Warned,
        /// <summary>The first try and the identical retry are both refused.</summary>
        Refused,
    }

    private const string PrCreate = "gh pr create --title t --body '## Independent review'";

    private const string Origin = "https://github.com/Acme/LC.git";

    private const string ReviewedBody = "DYD-1\n\n## Independent review\n\nPASS - rubric/type: code\n";

    private const string PlainBody = "DYD-1\n\n## Summary\n";

    // Shell idioms the PR scanner cannot read (DYD-515): a case arm in a substitution, an echo of
    // one, an escaped apostrophe pwsh and bash read apart, and both at once.
    private const string ArchIdiom = "arch=\"$(case \"$(uname -m)\" in\n  x86_64) echo amd64;;\n  *) echo other;;\nesac)\"\necho \"$arch\"";
    private const string EchoCase = "echo \"$(echo case)\"";
    private const string EscapedApostrophe = "v=$(date); echo it\\'s";
    private const string FailsBothReadings = "v=$(date); echo it\\'s \"$(echo case)\"";
    private const string EncodedGhPrReady = "ZwBoACAAcAByACAAcgBlAGEAZAB5AA==";

    private string Worktree => Path.Combine(TestDir, "wt");

    // The origin is a bare repository on disk; after the pushes it is renamed to LC's URL so a
    // call naming acme/lc names this tree's repository. Upstream refs stay local, so nothing fetches.
    private async Task InitPushedProject(bool withWorktree = false)
    {
        await InitProjectAsync("none");
        File.AppendAllText(Path.Combine(TestDir, ".gitignore"), "\n/origin.git/\n/wt/\n");
        Create(TestDir, Path.Combine(TestDir, "origin.git"));
        if (withWorktree)
        {
            Git(TestDir, "worktree", "add", "-b", "topic", Worktree);
            Git(Worktree, "push", "-u", "origin", "topic");
        }
        Git(TestDir, "remote", "set-url", "origin", Origin);
    }

    private async Task Arrange(Tree tree)
    {
        if (tree == Tree.OutsideGit)
        {
            await InitProjectAsync("none");
            return;
        }
        await InitPushedProject(withWorktree: tree is not (Tree.Clean or Tree.Dirty));
        if (tree is Tree.Dirty or Tree.DirtyMainCleanWorktree or Tree.BothDirty) WriteFile("wip.txt", "w\n");
        if (tree is Tree.DirtyWorktree or Tree.BothDirty) File.WriteAllText(Path.Combine(Worktree, "draft.txt"), "d\n");
    }

    // "Bash" and "PowerShell" are Claude's tools, "exec" a Codex shell call, "cli" dydo guard
    // --command; an argv array is a Codex call.
    private string Payload(object command, bool withCwd = true, bool codex = false,
        string? cwd = null, string? toolCwd = null, string? workdir = null, string toolName = "Bash")
    {
        var toolInput = new Dictionary<string, object?> { ["command"] = command };
        if (toolCwd != null) toolInput["cwd"] = toolCwd;
        if (workdir != null) toolInput["workdir"] = workdir;
        var payload = new Dictionary<string, object?>
        {
            ["session_id"] = TestSessionId,
            ["tool_name"] = toolName,
            ["tool_input"] = toolInput,
        };
        if (withCwd) payload["cwd"] = cwd ?? TestDir;
        if (codex) payload["turn_id"] = "turn-1";
        return JsonSerializer.Serialize(payload);
    }

    private async Task<CommandResult> Attempt(string lane, object command, string? toolCwd = null)
    {
        if (lane == "cli")
            return await RunAsync(GuardCommand.Create(), "--command", (string)command);
        return await GuardWithStdinAsync(Payload(command, codex: lane == "exec" || command is string[], toolCwd: toolCwd, toolName: lane));
    }

    private static bool IsAllowed(CommandResult result) => result.ExitCode == 0 && result.Stdout.Length == 0;

    private async Task AssertOutcome(Outcome outcome, Func<Task<CommandResult>> attempt, string label)
    {
        var first = IsAllowed(await attempt());
        var retry = IsAllowed(await attempt());
        var (wantFirst, wantRetry) = outcome switch
        {
            Outcome.Allowed => (true, true),
            Outcome.Warned => (false, true),
            Outcome.Refused => (false, false),
            _ => (first, true),
        };
        Assert.True(first == wantFirst && retry == wantRetry,
            $"{outcome} expected, got first {(first ? "allowed" : "refused")}, retry {(retry ? "allowed" : "refused")}: {label}");
    }

    private static TheoryData<Tree, string, object, Outcome> Rows(Tree tree, string lane, Outcome outcome, params object[] commands)
    {
        var data = new TheoryData<Tree, string, object, Outcome>();
        foreach (var command in commands)
            data.Add(tree, lane, command, outcome);
        return data;
    }

    private static TheoryData<Tree, string, object, Outcome> Join(params TheoryData<Tree, string, object, Outcome>[] tables)
    {
        var data = new TheoryData<Tree, string, object, Outcome>();
        foreach (var table in tables)
            foreach (var row in table)
                data.Add((Tree)row[0], (string)row[1], row[2], (Outcome)row[3]);
        return data;
    }

    // Every way a PR-opening call is written reaches the tree check: plain, chained, wrapped by a
    // runner or env, behind assignments, in a substitution, after a literal body, as PowerShell
    // reads it, in a Codex argv envelope. A dirty tree refuses each on every try.
    public static TheoryData<Tree, string, object, Outcome> PrCallsFromADirtyTree => Join(
        Rows(Tree.Dirty, "Bash", Outcome.Refused,
            PrCreate, "gh pr create --fill", "gh pr new --fill", "gh pr ready 12", "git status && gh pr ready",
            "git status; gh pr new --fill", @"printf '%s\n' 'C:\'; gh pr ready 12", "echo a#b; gh pr ready",
            "gh pr create --title \"fix #12\" --body '## Independent review'", "gh pr create --help; " + PrCreate,
            "cat <<'EOF'\nnotes\nEOF\n" + PrCreate, "cat <<-EOF\n\tnotes\n\tEOF\n" + PrCreate,
            "Write-Output @'\nIt's ready.\n'@\n" + PrCreate,
            "git status && gh -R acme/lc pr create --fill", "gh.exe pr create --fill", "GH_TOKEN=x gh pr create --fill",
            "env gh pr create --fill", "time gh pr create --fill", "{ gh pr create --fill; }", "if true; then gh pr create --fill; fi",
            "for i in 1; do gh pr create --fill; done", "if false; then :; else gh pr create --fill; fi",
            "env -u GH_TOKEN gh pr create --fill", "env GH_TOKEN=x gh pr create --fill", "timeout 60 gh pr create --fill",
            "nice gh pr create --fill", "nice -n 5 gh pr create --fill", "GH pr create --fill", "Gh.exe pr create --fill",
            "C:/bin/GH.EXE pr ready 12", "\"C:/Program Files/GitHub CLI/gh.exe\" pr create --fill", "(gh pr create --fill)",
            "gh -R acme/lc pr new --fill", "gh --repo acme/lc pr create --fill", "gh pr -R acme/lc create --fill",
            "gh pr create -R acme/lc --fill", "gh pr create --repo=Acme/LC.git --fill", "gh pr create -Racme/lc --fill",
            "GH_REPO=acme/lc gh pr create --fill", "  gh  pr create   --fill  ", "gh pr create --fill > out.txt",
            "git status\ngh pr create --fill", "cat <<EOF\njust notes\nEOF\ngh pr create --fill",
            "cat <<EOF; gh pr create --fill\njust notes\nEOF", "gh api repos/acme/lc/pulls -f title=t -f base=master",
            "gh api -X POST repos/acme/lc/pulls", "gh api --method=post repos/acme/lc/pulls -ftitle=x",
            "gh api -XPOST repos/acme/lc/pulls --field=title=x", "gh api https://api.github.com/repos/acme/lc/pulls -f title=t",
            "gh api REPOS/ACME/LC/PULLS -f title=t", "env GH_TOKEN=x gh api repos/acme/lc/pulls -f title=t",
            "gh api repos/{owner}/{repo}/pulls -f title=t",
            "gh api graphql -f query='mutation{markPullRequestReadyForReview(input:{pullRequestId:\"P\"}){clientMutationId}}'",
            "gh api graphql -f query='mutation{createPullRequest(input:{repositoryId:\"R\",baseRefName:\"master\",headRefName:\"h\",title:\"t\"}){clientMutationId}}'",
            "gh api https://ghe.example.com/api/graphql -f query='mutation{createPullRequest(input:{headRefName:\"h\"}){clientMutationId}}'",
            "`gh pr create --fill`", "echo $(gh pr create --fill)", "git commit -m \"$(gh pr create --fill)\"",
            "echo `gh pr create --fill`", "git commit -m \"`gh pr create --fill`\"", "nohup gh pr create --fill",
            "command gh pr create --fill", "exec gh pr ready 12", "env -- gh pr create --fill", "sudo -u me gh pr create --fill",
            "gh pr ready https://GitHub.com/acme/LC/pull/12", "env -S 'gh pr ready 12'", "env --split-string='gh pr ready 12'",
            "env --split-string 'gh pr ready 12'", "x=\"$(case a in a) echo y;; esac)\"; " + PrCreate,
            "cat <<EOF\n$(case a in a) echo y;; esac)\nEOF\n" + PrCreate, "echo \"$(case a in a) echo y;; esac)\" && gh pr ready",
            "arch=\"$(case \"$(uname -m)\" in\n  x86_64) echo amd64;;\n  *) echo other;;\nesac)\"\ngh pr create --fill",
            "x=\"$(echo y)\"; " + PrCreate),
        Rows(Tree.Dirty, "PowerShell", Outcome.Refused,
            @"C:\tools\gh.exe pr create --fill", @"& C:\tools\gh.exe pr create --fill",
            @"& 'C:\Program Files\GitHub CLI\gh.exe' pr ready 12", "echo 'x'\rgh pr create --fill",
            "echo x # note\rgh pr create --fill", "echo ‘a'; gh pr create --fill; ’b'", "echo \"a”; gh pr create --fill; “b\"",
            "echo a^;gh pr create --fill"),
        Rows(Tree.Dirty, "exec", Outcome.Refused,
            new[] { "gh", "pr", "new", "--fill" }, new[] { "bash", "-lc", "gh pr create --fill" },
            new[] { @"C:\Program Files\PowerShell\7\pwsh.exe", "-NoProfile", "-Command", "gh pr ready" },
            new[] { "pwsh", "-Command", @"& C:\tools\gh.exe pr create --fill" },
            new[] { @"C:\Windows\System32\WindowsPowerShell\v1.0\powershell.exe", "-NoProfile", "-Command", "gh pr create `\n  --fill" },
            new[] { "/bin/zsh", "-c", "gh pr ready 12" }, new[] { "sh", "-c", "gh pr ready 12" }));

    // What opens no PR passes a dirty tree untouched: a read, a mention, quoted or literal text,
    // help, a REST PATCH (GitHub's REST API cannot take a PR out of draft), a wrapped read.
    public static TheoryData<Tree, string, object, Outcome> NonPrCallsFromADirtyTree => Join(
        Rows(Tree.Dirty, "Bash", Outcome.Allowed,
            "gh pr view 12", "git status", "echo gh pr create", "git status # reminder; gh pr ready 12",
            "cat <<'EOF'\ngh pr ready 12\nEOF", "cat <<\"EOF\"\ngh pr ready 12\nEOF", "cat <<EOF\ngh pr ready 12\nEOF",
            "cat <<-EOF\n\tgh pr ready 12\n\tEOF", "Write-Output @'\nIt's ready.\ngh pr ready 12\n'@",
            "Write-Output @\"\nFits a 13\" screen.\ngh pr new --fill\n\"@", "ls", "grep -rn \"gh pr create\" dydo",
            "gh pr view $n", "echo gh pr create --fill", "gh pr view 12 --json title",
            "gh api graphql -F query=@viewer.graphql", "gh api graphql -f query='{viewer{login}}'", "gh api repos/acme/lc/pulls",
            "gh api repos/acme/lc/pulls/12/comments -f body=x", "bash -c \"gh pr view 12\"", "pwsh -NoProfile -Command \"git status\"",
            "cmd /c dir", "eval \"$(ssh-agent -s)\"", "bash -c 'gh api repos/acme/lc/pulls'", "bash -c 'gh pr list --search ready'",
            "sh -c 'gh pr view 12 --json title'", "pwsh -Command \"gh api repos/acme/lc/pulls?state=open\"",
            "bash -c 'gh api repos/acme/lc/pulls/12/comments -f body=x'", "cmd /c gh pr list --search create",
            "eval \"gh pr status\"", "bash -c 'echo gh pr create'", "gh api -X PATCH repos/acme/lc/pulls/12 -f draft=false",
            "env -S 'gh pr view 12'", "env --split-string='gh pr list --search ready'",
            "git commit -m '`gh pr create --fill`'"),
        Rows(Tree.Dirty, "Bash", Outcome.Passes,
            "git commit -m \"gh pr create later\"", "gh pr new -h", "gh pr create-notes",
            "git commit -m \"$(cat <<'EOF'\nfix the hook\n\ngh pr create now checks the tree\nEOF\n)\"",
            "cat <<'EOF'\ngh pr create --fill\nEOF", "cat > /tmp/b.md <<'EOF'\n## Steps\ngh pr create --title t --fill\nEOF",
            "gh pr create --title 'Independent review' --help; gh pr ready -h",
            "cat <<'EOF'\nx=\"$(case a in a) echo y;; esac)\"; gh pr ready\nEOF", "echo '$(case a in a) gh pr ready;; esac)'"),
        Rows(Tree.Dirty, "exec", Outcome.Passes, "gh version; gh pr create --help; gh pr ready --help"),
        Rows(Tree.Dirty, "PowerShell", Outcome.Allowed, "Write-Host `\"hi`\"", "Write-Host \"`gh pr create --fill`\""),
        Rows(Tree.Dirty, "exec", Outcome.Allowed,
            new[] { "bash", "-lc", "gh pr view 12" }, new[] { "gh", "api", "repos/acme/lc/pulls/12/comments", "-f", "body=x" }));

    // A clean pushed tree lets every PR call through, unless the call names another repository or
    // host, or hides itself in a wrapper the PR check cannot read: those fail closed (Balazs's
    // 2026-10-27 ruling), and outside git the rule stays out of the way.
    public static TheoryData<Tree, string, object, Outcome> PrCallsFromACleanTree => Join(
        Rows(Tree.Clean, "Bash", Outcome.Passes,
            PrCreate, "gh pr ready 12", "gh pr new --fill", "gh pr create -R Acme/LC --fill",
            "gh pr create --repo https://github.com/acme/lc.git --fill", "gh pr create -R git@github.com:Acme/LC.git --fill",
            "GH_REPO=github.com/acme/lc gh pr create --fill", "gh pr create -R GitHub.com/ACME/lc.git --fill",
            "gh pr create -R ssh://git@GITHUB.COM/acme/LC.git --fill",
            "gh api https://api.github.com/repos/Acme/LC/pulls -f head=feature -f title=t",
            "gh api --hostname github.com repos/acme/lc/pulls -f head=feature -f title=t",
            "gh pr ready https://GitHub.com/acme/LC/pull/12", "GH_REPO=other/x gh pr create -R acme/lc --fill",
            "GH_REPO=other/x gh -R acme/lc pr create --fill", "gh api repos/{owner}/{repo}/pulls -X POST -f title=t",
            "gh api repos/{owner}/LC/pulls -X POST -f title=t", "gh api repos/Acme/{repo}/pulls -X POST -f title=t"),
        Rows(Tree.Clean, "exec", Outcome.Passes, new[] { "bash", "-lc", "gh pr create --fill" }),
        Rows(Tree.Clean, "Bash", Outcome.Refused,
            "gh pr ready https://github.com/other/x/pull/12", "gh pr ready https://ghe.example.com/acme/lc/pull/12",
            "gh api repos/{owner}/other/pulls -X POST", "gh api repos/other/{repo}/pulls -X POST",
            "bash -c \"gh pr create --fill\"", "sh -c 'git status; gh pr ready 12'", "pwsh -NoProfile -Command \"gh pr create --fill\"",
            "powershell -c \"gh pr create --fill\"", "eval \"gh pr create --fill\"", "eval gh pr create --fill",
            "cmd /c gh pr create --fill", "cmd.exe /c \"gh pr create --fill\"", "cmd /k gh pr create --fill",
            "cmd //c gh pr create --fill", "cmd.exe /s /C \"git status && gh pr ready 12\"", "bash -lc 'gh -R acme/lc pr create --fill'",
            "bash -c 'gh api repos/acme/lc/pulls -f head=feature'", "pwsh -NoProfile -Command \"gh api repos/acme/lc/pulls -f head=feature\"",
            "bash -c \"gh api -X POST repos/acme/lc/pulls -F head=feature -f base=main\"",
            "bash -c 'pwsh -NoProfile -Command \"gh pr create --head feature --fill\"'",
            "pwsh -EncodedCommand ZwBoACAAcAByACAAYwByAGUAYQB0AGUAIAAtAC0AZgBpAGwAbAA=",
            "powershell.exe -ec ZwBoACAAcAByACAAYwByAGUAYQB0AGUAIAAtAC0AZgBpAGwAbAA=",
            "bash -c 'gh api -X POST -H \"Content-Type: application/json; charset=utf-8\" repos/acme/lc/pulls --input body.json'",
            "pwsh -Command \"gh pr create --title 'a; b' --fill\"", "bash -c 'echo \"x | y\" && gh pr ready 12'",
            "gh pr create --title \"$(case a in a) echo t;; esac)\" --head missing",
            "gh pr create --title \"$(case a in a) echo t;; esac)\" --repo other/repo",
            ArchIdiom + "\n/usr/bin/gh pr create --fill", EchoCase + " && GH.EXE pr ready", EchoCase + " \"$(gh pr ready)\"",
            "bash -c '" + EchoCase + "; gh pr ready'", "bash -c '" + ArchIdiom + "\ngh pr ready'",
            EchoCase + "; pwsh -EncodedCommand " + EncodedGhPrReady, ArchIdiom + "\n/usr/bin/PWSH.EXE -enc " + EncodedGhPrReady,
            "bash -c '" + EchoCase + "; pwsh -enc " + EncodedGhPrReady + "'",
            ArchIdiom + "\ngh pr create --head topic", EchoCase + "; GH_REPO=other/repo gh pr create",
            EchoCase + "; gh api repos/other/repo/pulls -f head=topic", "gh pr create --title \"unfinished"),
        Rows(Tree.Clean, "PowerShell", Outcome.Refused,
            EscapedApostrophe + "; C:\\tools\\gh.exe pr ready", EscapedApostrophe + "; powershell -ec " + EncodedGhPrReady,
            "bash -c '" + EchoCase + "; Gh pr create --fill'",
            "bash -c '" + EchoCase + "; powershell.exe -EncodedCommand " + EncodedGhPrReady + "'", "Write-Host \"$(gh pr create"),
        Rows(Tree.Clean, "cli", Outcome.Refused,
            FailsBothReadings + "; gh pr ready", EscapedApostrophe + "; gh pr ready topic --repo other/repo",
            FailsBothReadings + "; gh api graphql -f query=createPullRequest"),
        Rows(Tree.Clean, "exec", Outcome.Refused,
            new[] { "pwsh", "-Command", "gh", "pr", "create", "--fill" }, new[] { "bash", "-lc", "bash -c 'gh pr create --fill'" },
            new[] { "pwsh", "-Command", "cmd /c gh pr ready 12" }, new[] { "cmd", "/c", "gh pr create --fill" },
            new[] { "cmd.exe", "/c", "gh pr ready 12" }, new[] { "eval", "gh pr ready 12" },
            new[] { "bash", "-lc", "git push --force origin main" }, new[] { "bash", "-lc", EchoCase + "; gh pr ready" },
            new[] { "pwsh", "-Command", EscapedApostrophe + "; gh pr ready" }, new[] { "echo", FailsBothReadings + "; gh pr ready" },
            new[] { "node", "-e", FailsBothReadings + "; gh pr ready" },
            new[] { "bash", "-lc", EchoCase + "; pwsh -EncodedCommand " + EncodedGhPrReady },
            new[] { "pwsh", "-Command", EscapedApostrophe + "; pwsh -ec " + EncodedGhPrReady },
            new[] { "bash", "-lc", "x=\"$(case a in a) echo y;; esac)\"; gh pr ready" },
            new[] { "node", "-e", "x=\"$(case a in a) echo y;; esac)\"; gh pr ready" }),
        Rows(Tree.CleanWithWorktree, "Bash", Outcome.Refused,
            "gh pr create -R other/x --fill", "gh -R other/x pr create --head topic", "gh pr create --repo=github.com/other/x --head topic",
            "gh pr create -R https://github.com/Other/X.git --fill", "GH_REPO=other/x gh pr create --fill",
            "env GH_REPO=other/x gh pr create --fill", "gh -R acme/lc pr create --repo other/x --fill",
            "gh pr ready topic -R github.com/other/x", "gh pr create -R acme/lc-docs --fill", "gh api repos/other/x/pulls -f head=topic",
            "gh pr create -R ghe.example.com/acme/lc --fill", "gh pr create --repo https://ghe.example.com/acme/lc.git --fill",
            "GH_REPO=ghe.example.com/acme/lc gh pr create --fill", "gh api https://ghe.example.com/api/v3/repos/acme/lc/pulls -f head=topic",
            "gh api --hostname ghe.example.com repos/acme/lc/pulls -f head=topic", "gh pr create --head nope --fill",
            "gh pr ready nope", "gh api repos/acme/lc/pulls -f head=nope"),
        Rows(Tree.OutsideGit, "Bash", Outcome.Passes,
            PrCreate, "pwsh -EncodedCommand ZwBpAHQAIABzAHQAYQB0AHUAcwA=", "bash -c 'gh pr create --fill'", "cmd /c gh pr ready 12"));

    // A call naming its branch is checked where that branch is checked out (DYD-391), every call
    // against its own tree; a value that only looks like --head names no branch.
    public static TheoryData<Tree, string, object, Outcome> BranchNamedCalls => Join(
        Rows(Tree.DirtyMainCleanWorktree, "Bash", Outcome.Passes,
            "gh pr create --head topic --fill", "gh pr create -H topic --fill", "gh pr create -Htopic --fill",
            "gh pr create --head=topic --fill", "gh pr create -dH \"topic\" --title x", "gh pr create --head other --fill --head me:topic",
            "gh -R acme/lc pr create --repo acme/lc --head 'topic'", "gh pr new --head topic --fill", "gh pr ready topic",
            "gh pr ready -R acme/lc topic", "gh pr ready --undo me:topic", "gh api repos/acme/lc/pulls -f head=topic -f base=master -f title=x",
            "gh api -X POST /repos/acme/lc/pulls -f head=me:topic", "gh api 'repos/{owner}/{repo}/pulls' -F head=topic",
            "gh pr create --head topic --fill && gh pr ready topic",
            "gh pr create `\n  --head topic `\n  --title t --body '## Independent review'",
            "gh pr create `\r\n  --head topic `\r\n  --title t --body '## Independent review'"),
        Rows(Tree.DirtyMainCleanWorktree, "Bash", Outcome.Refused,
            "gh --head topic pr create --fill", "gh -H topic pr create --fill", "gh pr create --title -H topic --fill",
            "gh pr create --title=x --body --head topic", "gh pr ready 12", "gh pr ready https://github.com/acme/lc/pull/12",
            "gh pr ready topic && gh pr ready feature"),
        Rows(Tree.DirtyWorktree, "Bash", Outcome.Refused,
            "gh pr create --head topic --fill", "gh pr ready topic", "gh api repos/acme/lc/pulls -f head=topic",
            "gh pr create `\n  --head topic `\n  --fill", "gh pr create `\r\n  --head topic `\r\n  --fill",
            @"& C:\Tools\gh.exe pr create --head topic --fill"),
        Rows(Tree.DirtyWorktree, "PowerShell", Outcome.Refused, "gh pr create `\n  --head topic `\n  --fill"),
        Rows(Tree.BothDirty, "Bash", Outcome.Refused, "gh pr create --head topic --fill && gh pr create --fill"));

    [Theory]
    [MemberData(nameof(PrCallsFromADirtyTree))]
    [MemberData(nameof(NonPrCallsFromADirtyTree))]
    [MemberData(nameof(PrCallsFromACleanTree))]
    [MemberData(nameof(BranchNamedCalls))]
    public async Task PrCall_MeetsItsTreesOutcome(Tree tree, string lane, object command, Outcome outcome)
    {
        await Arrange(tree);

        await AssertOutcome(outcome, () => Attempt(lane, command), $"{tree} {lane}: {command}");
    }

    // A failed reading without a gh word is no PR call: clean or dirty, every try is allowed (DYD-515).
    public static TheoryData<string, object> FailedReadingsWithoutGh => new()
    {
        { "Bash", ArchIdiom }, { "Bash", EchoCase }, { "PowerShell", EscapedApostrophe }, { "cli", EscapedApostrophe },
        { "cli", ArchIdiom }, { "Bash", "bash -c '" + EchoCase + "'" }, { "PowerShell", "pwsh -Command 'v=$(date); echo it\\''s'" },
        { "Bash", new[] { "bash", "-lc", ArchIdiom } }, { "Bash", new[] { "pwsh", "-Command", EscapedApostrophe } },
        { "Bash", new[] { "echo", EchoCase } },
    };

    [Theory]
    [Trait("Job", "trap: DYD-515 an unreadable line without a gh word is not refused")]
    [MemberData(nameof(FailedReadingsWithoutGh))]
    public async Task FailedReading_WithoutAGhWord_IsAllowedFromACleanOrDirtyTree(string lane, object command)
    {
        await InitPushedProject();
        await AssertOutcome(Outcome.Allowed, () => Attempt(lane, command), $"clean {lane}: {command}");
        WriteFile("readme.md", "changed\n");
        await AssertOutcome(Outcome.Allowed, () => Attempt(lane, command), $"dirty {lane}: {command}");
    }

    // A tree is not ready while it has untracked, unstaged or staged changes, or a HEAD its upstream
    // lacks or none at all; the call is refused every try, and the review-block warn neither arms
    // nor spends meanwhile, so it still meets the first try once the tree is ready (DYD-386).
    [Theory]
    [Trait("Job", "contract: DYD-374 tree readiness, DYD-386 warn after every hard block")]
    [InlineData("untracked")]
    [InlineData("unstaged")]
    [InlineData("staged")]
    [InlineData("unpushed")]
    [InlineData("no upstream")]
    public async Task TreeNotReady_RefusesEveryTry_AndTheReadyTreeStillWarnsFirst(string state)
    {
        await InitPushedProject();
        switch (state)
        {
            case "untracked": WriteFile("wip.txt", "w\n"); break;
            case "unstaged": WriteFile(".gitignore", ReadFile(".gitignore") + "\n# changed\n"); break;
            case "staged": WriteFile("wip.txt", "w\n"); Git(TestDir, "add", "wip.txt"); break;
            case "unpushed": WriteFile("more.txt", "m\n"); Commit(TestDir, "more"); break;
            default: Git(TestDir, "checkout", "-b", "local-only"); break;
        }

        for (var attempt = 1; attempt <= 4; attempt++)
            Assert.False(IsAllowed(await Attempt("Bash", "gh pr create --fill")), $"try {attempt}");

        if (state == "no upstream") return;
        if (state == "unpushed") Git(TestDir, "update-ref", "refs/remotes/origin/feature", "HEAD");
        else Git(TestDir, "reset", "--hard", "-q");
        if (File.Exists(Path.Combine(TestDir, "wip.txt"))) File.Delete(Path.Combine(TestDir, "wip.txt"));
        await AssertOutcome(Outcome.Warned, () => Attempt("Bash", "gh pr create --fill"), state);
    }

    // The tree checked is the one the call runs in: tool_input.cwd before Codex's workdir, a
    // relative one against the session cwd (or the process directory without one), a subdirectory's
    // repository, and the project root when the host sends no cwd at all.
    [Theory]
    [Trait("Job", "contract: DYD-391 the call's directory decides its tree")]
    [InlineData("codex workdir", false)]
    [InlineData("codex clean workdir", true)]
    [InlineData("tool cwd before workdir", false)]
    [InlineData("relative tool cwd", false)]
    [InlineData("relative codex workdir", false)]
    [InlineData("relative workdir, no session cwd", true)]
    [InlineData("subdirectory", false)]
    [InlineData("codex without cwd", false)]
    [InlineData("blank cwd", false)]
    [InlineData("blank tool cwd", false)]
    [InlineData("blank workdir", false)]
    public async Task CallDirectory_DecidesTheTreeChecked(string form, bool allowed)
    {
        await InitPushedProject(withWorktree: true);
        WriteFile("wip.txt", "w\n");
        var subdirectory = Path.Combine(TestDir, "src", "app");
        Directory.CreateDirectory(subdirectory);
        var payload = form switch
        {
            "codex workdir" => Payload(PrCreate, cwd: Worktree, codex: true, workdir: TestDir),
            "codex clean workdir" => Payload(PrCreate, codex: true, workdir: Worktree),
            "tool cwd before workdir" => Payload(PrCreate, cwd: Worktree, toolCwd: TestDir, workdir: Worktree),
            "relative tool cwd" => Payload(PrCreate, cwd: Worktree, toolCwd: ".."),
            "relative codex workdir" => Payload(PrCreate, cwd: Worktree, codex: true, workdir: ".."),
            "relative workdir, no session cwd" => Payload(PrCreate, withCwd: false, codex: true, workdir: "wt"),
            "subdirectory" => Payload(PrCreate, cwd: subdirectory),
            "codex without cwd" => Payload(PrCreate, withCwd: false, codex: true),
            "blank cwd" => Payload(PrCreate, cwd: "  "),
            "blank tool cwd" => Payload(PrCreate, cwd: "", toolCwd: ""),
            _ => Payload(PrCreate, cwd: "", workdir: ""),
        };

        await AssertOutcome(allowed ? Outcome.Allowed : Outcome.Refused, () => GuardWithStdinAsync(payload), form);
    }

    // The effective repository is the last -R/--repo, else an inline GH_REPO, else the inherited one;
    // env's unset, ignore-environment and an empty GH_REPO drop the inherited one, in order; a
    // placeholder in an api path resolves against it segment by segment.
    [Theory]
    [Trait("Job", "contract: DYD-391 a call naming another repository is refused")]
    [InlineData("other/x", "gh pr create --fill", true)]
    [InlineData("other/x", "gh pr ready 12", true)]
    [InlineData("other/x", "gh pr create -R acme/lc --fill", false)]
    [InlineData("other/x", "GH_REPO=acme/lc gh pr create --fill", false)]
    [InlineData("acme/lc", "GH_REPO=other/x gh pr create --fill", true)]
    [InlineData("other/x", "env -u GH_REPO gh pr ready 12", false)]
    [InlineData("other/x", "env --unset=GH_REPO gh pr ready 12", false)]
    [InlineData("other/x", "env --unset GH_REPO gh pr ready 12", false)]
    [InlineData("other/x", "env -i gh pr ready 12", false)]
    [InlineData("other/x", "GH_REPO= gh pr ready 12", false)]
    [InlineData("other/x", "env -u GH_REPO GH_REPO=other/y gh pr ready 12", true)]
    [InlineData("other/x", "env -i GH_REPO=other/y gh pr ready 12", true)]
    [InlineData("other/x", "GH_REPO=acme/lc env -u OTHER gh pr ready 12", false)]
    [InlineData("acme/zz", "gh api repos/{owner}/lc/pulls -X POST -f title=t", false)]
    [InlineData("other/x", "gh api repos/{owner}/lc/pulls -X POST -f title=t", true)]
    [InlineData("other/lc", "gh api repos/acme/{repo}/pulls -X POST -f title=t", false)]
    public async Task InheritedGhRepo_DecidesTheForeignRefusal(string inherited, string command, bool refused)
    {
        await InitPushedProject();
        var before = Environment.GetEnvironmentVariable("GH_REPO");
        Environment.SetEnvironmentVariable("GH_REPO", inherited);
        try
        {
            await AssertOutcome(refused ? Outcome.Refused : Outcome.Passes, () => Attempt("Bash", command), command);
        }
        finally
        {
            Environment.SetEnvironmentVariable("GH_REPO", before);
        }
    }

    // Git worktree corners: a worktree whose directory is gone cannot be checked; two worktrees or
    // two paths differing in case holding one branch are each checked; an unpushed worktree is not ready.
    [Theory]
    [Trait("Job", "contract: DYD-391 every worktree holding the named branch is checked")]
    [InlineData("prunable")]
    [InlineData("second worktree dirty")]
    [InlineData("case-differing worktree dirty")]
    [InlineData("unpushed worktree")]
    public async Task BranchHolder_ThatIsNotReady_RefusesTheCall(string holder)
    {
        await InitPushedProject(withWorktree: true);
        switch (holder)
        {
            case "prunable":
                Directory.Delete(Worktree, recursive: true);
                break;
            case "unpushed worktree":
                File.WriteAllText(Path.Combine(Worktree, "more.txt"), "m\n");
                Commit(Worktree, "more");
                break;
            default:
                var second = Path.Combine(TestDir, holder.StartsWith("case") ? "WT" : "wt2");
                if (!Directory.Exists(second))
                {
                    File.AppendAllText(Path.Combine(TestDir, ".gitignore"), $"/{Path.GetFileName(second)}/\n");
                    Git(TestDir, "worktree", "add", "--force", second, "topic");
                }
                File.WriteAllText(Path.Combine(second, "two.txt"), "2\n");
                break;
        }

        await AssertOutcome(Outcome.Refused, () => Attempt("Bash", "gh pr ready topic"), holder);
    }

    // DYD-639: a gh pr create taking its body from a file is cleared by the review block in that file
    // only when the guard may read it: a regular in-tree file, plainly written, named on a line that
    // holds the lone call. Anything else keeps the once-only review-block warning (DYD-736, rulings 6
    // and 7). The fixture holds every body file the rows name; sub/ holds an unreviewed body.md.
    private async Task InitBodyFiles()
    {
        await InitPushedProject();
        foreach (var name in new[] { "body.md", "other.md", "reviewed.md", "pr-body.md", "pr body.md", "reviewed.md$SUFFIX" })
            WriteFile(Path.Combine("dydo", "agents", name), ReviewedBody);
        foreach (var name in new[] { "plain.md", "reviewed.md.plain", Path.Combine("sub", "body.md") })
            WriteFile(Path.Combine("dydo", "agents", name), PlainBody);
        WriteFile(Path.Combine("dydo", "agents", "env.sh"), "cd sub\n");
        WriteFile(Path.Combine("dydo", "_system", ".local", "body.md"), ReviewedBody);
    }

    public static TheoryData<string, bool, string, Outcome> BodyFileCalls()
    {
        var data = new TheoryData<string, bool, string, Outcome>();
        void Add(string lane, bool fromAgents, Outcome outcome, params string[] commands)
        {
            foreach (var command in commands) data.Add(lane, fromAgents, command, outcome);
        }
        Add("Bash", false, Outcome.Allowed,
            "gh pr create --title t --body-file dydo/agents/body.md", "gh pr create --title t --body-file=dydo/agents/body.md",
            "gh pr create --title t -F dydo/agents/body.md", "gh pr create --title t -Fdydo/agents/body.md",
            "gh pr create -F \"dydo/agents/pr body.md\"", "gh pr create --body-file='dydo/agents/pr body.md'",
            "gh pr create --title 'Fix $HOME * {a,b}' -F dydo/agents/reviewed.md",
            "gh pr create --title \"Why? [WIP]\" -F dydo/agents/reviewed.md", "gh pr create -F \"dydo/agents/reviewed.md\"",
            "gh pr create -F dydo/agents/body.md && gh pr create --body '## Independent review'");
        Add("Bash", true, Outcome.Allowed, "gh pr create -F body.md");
        Add("PowerShell", false, Outcome.Allowed,
            "gh pr create --title 'Fix $x @y' -F dydo/agents/reviewed.md", "gh pr create -F \"dydo/agents/reviewed.md\"");
        Add("Bash", false, Outcome.Warned,
            "gh pr create --body-file dydo/agents/plain.md", "gh pr create -F dydo/agents/missing.md", "gh pr create -F dydo",
            "gh pr create -F -", "gh pr create -F dydo/_system/.local/body.md",
            "gh pr create -F dydo/agents/body.md; gh pr create -F dydo/agents/missing.md",
            "gh pr create -F dydo/agents/body.md; gh pr create -F dydo/agents/plain.md",
            "gh pr create -F dydo/agents/body.md; gh pr create --title other",
            "gh pr create -F dydo/agents/plain.md; gh pr create -F dydo/agents/body.md",
            "gh pr create --title other; gh pr create -F dydo/agents/body.md",
            "gh pr create -F dydo/agents/body.md; gh pr create -F dydo/agents/other.md",
            "gh pr create --body '## Independent review'; gh pr create -F dydo/agents/body.md",
            "gh pr create -F \"dydo/agents/reviewed.md$(printf .plain)\"", "gh pr create -F \"dydo/agents/reviewed.md`printf .plain`\"",
            "SUFFIX=.plain; gh pr create -F \"dydo/agents/reviewed.md$SUFFIX\"",
            "gh pr create -F dydo/agents/missing/../body.md", "gh pr create -F dydo/agents/plain.md/../body.md",
            "gh pr create -F dydo/agents/./body.md", "gh pr create -F dydo/agents//body.md", "gh pr create -F dydo/agents/body.md/",
            "gh pr create -F dydo/agents/body.md/.", "gh pr create -F dydo/agents/body.md\\", "gh pr create -F dydo/agents/body\\\r\n.md",
            "gh pr create -F dydo/agents/body.md\r\n", "gh pr create -F dydo/agents/reviewed.md $EXTRA",
            "gh pr create -F dydo/agents/reviewed.md `printf -- '-F dydo/agents/plain.md'`", "gh pr create -F dydo/agents/reviewed.md \"$@\"",
            "gh pr create -F dydo/agents/reviewed.md \"${EXTRA[@]}\"", "gh pr create -F dydo/agents/reviewed.md -? dydo/agents/plain.md",
            "gh pr create -F dydo/agents/reviewed.md {-F,dydo/agents/plain.md}", "gh pr create --title \"$TITLE\" -F dydo/agents/reviewed.md",
            "gh pr create -F dydo/agents/body.md > dydo/agents/body.md", "gh pr create -F dydo/agents/body.md >> dydo/agents/body.md",
            "gh pr create -F dydo/agents/body.md &> dydo/agents/body.md", "gh pr create -F dydo/agents/body.md 2>dydo/agents/body.md",
            "gh pr create -F dydo/agents/body.md >| dydo/agents/body.md", "gh pr create -F dydo/agents/body.md & git status",
            "gh pr create -F dydo/agents/body.md | tee dydo/agents/body.md",
            "dydo pr-ready --body dydo/agents/body.md && gh pr create --base feature/x --title \"DYD-1 fix: thing\" --body-file dydo/agents/body.md",
            "dydo pr-ready --body dydo/agents/body.md && gh pr create --title t -F dydo/agents/body.md",
            "git status && gh pr create -F dydo/agents/body.md", "echo x; gh pr create -F dydo/agents/body.md",
            "gh pr create -F dydo/agents/body.md 2>&1 | cat", "gh pr create -F dydo/agents/body.md < /dev/null",
            "gh pr create -F dydo/agents/body.md && echo done", "gh pr create -F dydo/agents/body.md; true",
            "echo \"$(git diff $DIFF_OPTIONS)\" && gh pr create --title \"DYD-1 fix: thing\" -F dydo/agents/body.md",
            "git diff \"$DIFF_OPTION\" && gh pr create --title t -F dydo/agents/body.md",
            "FOO=1 gh pr create -F dydo/agents/body.md", "GH_PROMPT_DISABLED=1 GH_REPO=acme/lc gh pr create --body-file dydo/agents/body.md",
            "env gh pr create -F dydo/agents/body.md", "env -i gh pr create -F dydo/agents/body.md",
            "env -u GH_TOKEN -- FOO=1 gh pr create -F dydo/agents/body.md");
        Add("Bash", true, Outcome.Warned,
            "git status --short && cd sub && gh pr create -F body.md", "git status --short && pushd sub && gh pr create -F body.md",
            "git status --short; source env.sh; gh pr create -F body.md", "git status --short; . ./env.sh; gh pr create -F body.md",
            "echo ${ cd sub; } && gh pr create -F body.md", "(git status) && gh pr create -F body.md",
            "{ git status; } && gh pr create -F body.md", "git switch plain && gh pr create -F pr-body.md",
            "git checkout x && gh pr create -F pr-body.md", "git -C x status && gh pr create -F pr-body.md",
            "git stash pop && gh pr create -F pr-body.md");
        Add("PowerShell", true, Outcome.Warned, "git status; Set-Location sub; gh pr create -F body.md");
        Add("PowerShell", false, Outcome.Warned,
            "gh pr create -F dydo/agents/reviewed.md $extra", "gh pr create -F dydo/agents/reviewed.md @extra",
            "gh pr create -F dydo/agents/body.md > dydo/agents/body.md", "gh pr create -F dydo/agents/body.md *> dydo/agents/body.md",
            "git status; gh pr create -F dydo/agents/body.md", "git status --short; gh pr create -F dydo/agents/body.md 2>&1 | cat",
            "gh pr create -F \"dydo/agents/reviewed.md$(printf .plain)\"", "SUFFIX=.plain; gh pr create -F \"dydo/agents/reviewed.md$SUFFIX\"");
        Add("PowerShell", false, Outcome.Refused, "gh pr create -F \"dydo/agents/reviewed.md`printf .plain`\"");
        Add("cli", false, Outcome.Refused, "gh pr create -F \"dydo/agents/reviewed.md`printf .plain`\"");
        Add("cli", false, Outcome.Allowed, "gh pr create -F \"dydo/agents/reviewed.md\"");
        return data;
    }

    [Theory]
    [Trait("Job", "contract: DYD-639 body file read for the review block, DYD-736, rulings 6 and 7")]
    [MemberData(nameof(BodyFileCalls))]
    public async Task BodyFileCall_MeetsItsOutcome(string lane, bool fromAgents, string command, Outcome outcome)
    {
        await InitBodyFiles();
        var toolCwd = fromAgents ? Path.Combine(DydoDir, "agents") : null;

        await AssertOutcome(outcome, () => Attempt(lane, command, toolCwd), $"{lane}: {command}");
    }

    // A body file the guard may not follow keeps the warning: outside the work tree, through an
    // in-tree link to one, over the 256 KiB cap, outside git, or with GIT_DIR and GIT_WORK_TREE
    // exported (an in-tree one is still read then); a Codex argv call reads it like a line.
    [Theory]
    [Trait("Job", "trap: DYD-639 a body file outside the tree, linked out of it, or over the cap is not read")]
    [InlineData("outside, absolute")]
    [InlineData("outside, relative")]
    [InlineData("in-tree file link")]
    [InlineData("in-tree directory link")]
    [InlineData("over the cap")]
    [InlineData("outside git")]
    [InlineData("exported GIT_DIR, outside")]
    public async Task BodyFileTheGuardMayNotFollow_Warns(string form)
    {
        if (form == "outside git") await InitProjectAsync("none");
        else await InitPushedProject();
        var outside = Path.Combine(Path.GetTempPath(), "dydo-outside-" + Guid.NewGuid().ToString("N")[..8]);
        Directory.CreateDirectory(outside);
        File.WriteAllText(Path.Combine(outside, "body.md"), ReviewedBody);
        Directory.CreateDirectory(Path.Combine(DydoDir, "agents"));
        var saved = new[] { "GIT_DIR", "GIT_WORK_TREE" }.ToDictionary(name => name, Environment.GetEnvironmentVariable);
        try
        {
            var command = "gh pr create -F dydo/agents/body.md";
            switch (form)
            {
                case "outside, absolute": command = $"gh pr create -F '{Path.Combine(outside, "body.md")}'"; break;
                case "outside, relative": command = $"gh pr create -F ../{Path.GetFileName(outside)}/body.md"; break;
                case "in-tree file link": File.CreateSymbolicLink(Path.Combine(DydoDir, "agents", "body.md"), Path.Combine(outside, "body.md")); break;
                case "in-tree directory link":
                    DirectoryLinks.Create(Path.Combine(DydoDir, "agents", "linked"), outside);
                    command = "gh pr create -F dydo/agents/linked/body.md";
                    break;
                case "over the cap": WriteFile("dydo/agents/body.md", ReviewedBody + new string('x', 256 * 1024)); break;
                case "outside git": WriteFile("dydo/agents/body.md", ReviewedBody); break;
                default:
                    WriteFile("dydo/agents/body.md", ReviewedBody);
                    Environment.SetEnvironmentVariable("GIT_DIR", Path.Combine(TestDir, ".git"));
                    Environment.SetEnvironmentVariable("GIT_WORK_TREE", TestDir);
                    await AssertOutcome(Outcome.Allowed, () => Attempt("Bash", "gh pr create -F dydo/agents/body.md"), "in-tree");
                    command = $"gh pr create -F '{Path.Combine(outside, "body.md")}'";
                    break;
            }

            await AssertOutcome(Outcome.Warned, () => Attempt("Bash", command), form);
        }
        finally
        {
            foreach (var (name, value) in saved)
                Environment.SetEnvironmentVariable(name, value);
            Directory.Delete(outside, recursive: true);
        }
    }

    // The file's own bytes decide too: an empty file, invalid UTF-8 or one byte past the 256 KiB cap
    // is not read, a UTF-8 body at the cap is; an in-tree link is followed through plain names only,
    // to a file the guard may read (DYD-639).
    [Theory]
    [Trait("Job", "edge: DYD-639 body-file bytes, cap and in-tree links")]
    [InlineData("empty", Outcome.Warned)]
    [InlineData("invalid UTF-8", Outcome.Warned)]
    [InlineData("one byte over the cap", Outcome.Warned)]
    [InlineData("at the cap", Outcome.Allowed)]
    [InlineData("UTF-8", Outcome.Allowed)]
    [InlineData("off-limits", Outcome.Warned)]
    [InlineData("link in the tree", Outcome.Allowed)]
    [InlineData("link to an off-limits file", Outcome.Warned)]
    [InlineData("link loop", Outcome.Warned)]
    [InlineData("link through ..", Outcome.Warned)]
    [InlineData("link through .", Outcome.Warned)]
    public async Task BodyFileBytesAndLinks_DecideWhetherItIsRead(string form, Outcome outcome)
    {
        const int cap = 256 * 1024;
        await InitPushedProject();
        var agents = Path.Combine(DydoDir, "agents");
        Directory.CreateDirectory(agents);
        WriteFile("dydo/agents/reviewed.md", ReviewedBody);
        WriteFile("dydo/agents/secrets.json", ReviewedBody);
        var body = Path.Combine(agents, "body.md");
        switch (form)
        {
            case "empty": File.WriteAllText(body, ""); break;
            case "invalid UTF-8": File.WriteAllBytes(body, [.. System.Text.Encoding.UTF8.GetBytes(ReviewedBody), 0xC3, 0x28]); break;
            case "one byte over the cap": File.WriteAllText(body, ReviewedBody.PadRight(cap + 1, 'x')); break;
            case "at the cap": File.WriteAllText(body, ReviewedBody.PadRight(cap, 'x')); break;
            case "UTF-8": File.WriteAllText(body, "DYD-1\n\n## Independent review\n\nPASS – rubric/type: code\n"); break;
            case "off-limits": body = Path.Combine(agents, "secrets.json"); break;
            case "link in the tree": File.CreateSymbolicLink(body, "reviewed.md"); break;
            case "link to an off-limits file": File.CreateSymbolicLink(body, "secrets.json"); break;
            case "link loop":
                File.CreateSymbolicLink(body, "loop.md");
                File.CreateSymbolicLink(Path.Combine(agents, "loop.md"), "body.md");
                break;
            case "link through ..": File.CreateSymbolicLink(body, "../agents/reviewed.md"); break;
            default: File.CreateSymbolicLink(body, "./reviewed.md"); break;
        }
        var command = "gh pr create -F dydo/agents/" + Path.GetFileName(body);

        await AssertOutcome(outcome, () => Attempt("Bash", command), form);
    }

    // A native absolute body file is read for a lone call and kept warned after any earlier command;
    // a Codex argv call with an in-tree body file is read like the line.
    [Fact]
    [Trait("Job", "contract: DYD-639 ruling 7, an absolute body file is read only for a lone call")]
    public async Task AbsoluteOrArgvBodyFile_IsReadOnlyForALoneCall()
    {
        await InitBodyFiles();
        var absolute = Path.Combine(TestDir, "dydo", "agents", "body.md");
        var lane = OperatingSystem.IsWindows() ? "PowerShell" : "Bash";
        Task<CommandResult> Run(string command) => GuardWithStdinAsync(Payload(command, toolName: lane), "--host", "claude");

        foreach (var command in new[] { $"gh pr create -F '{absolute}'", $"gh pr create -F {absolute}" })
            await AssertOutcome(Outcome.Allowed, () => Run(command), command);
        foreach (var command in new[]
        {
            $"printf 'Summary only' > {absolute} && gh pr create -F {absolute}", $"git switch plain && gh pr create -F {absolute}",
            $"git status --short && cd dydo && gh pr create -F {absolute}", $"git status --short && gh pr create -F {absolute}",
        })
            await AssertOutcome(Outcome.Warned, () => Run(command), command);
        await AssertOutcome(Outcome.Allowed,
            () => GuardWithStdinAsync(Payload(new[] { "gh", "pr", "create", "-F", "dydo/agents/body.md" }, codex: true)), "argv");
    }

    // gh placeholders are not plain body-file names, even quoted and naming a reviewed file.
    [Theory]
    [Trait("Job", "trap: DYD-639 R3-S1 a gh placeholder in the body-file operand")]
    [InlineData(false)]
    [InlineData(true)]
    public async Task BodyFileOperandWithAGhPlaceholder_Warns(bool reviewed)
    {
        await InitPushedProject();
        if (!OperatingSystem.IsWindows())
            WriteFile("dydo/agents/reviewed.md:repo", ReviewedBody);
        WriteFile("dydo/agents/reviewed.md{repo}", reviewed ? ReviewedBody : PlainBody);
        WriteFile("dydo/agents/reviewed.md", ReviewedBody);

        await AssertOutcome(Outcome.Warned, () => Attempt("Bash", "gh pr create -F 'dydo/agents/reviewed.md{repo}'"), "placeholder");
        await AssertOutcome(Outcome.Allowed, () => Attempt("Bash", "gh pr create -F 'dydo/agents/reviewed.md'"), "plain");
    }

    // A dydo.json copy of the review-block nudge, current or pre-DYD-736, has its body file read too,
    // and a prefixed call never does; a custom block nudge on gh pr create reads the command alone.
    [Theory]
    [Trait("Job", "contract: DYD-736 the review-block nudge's body file, behind assignments or env")]
    [InlineData(ConfigFactory.ReviewBlockPattern, "gh pr create -F dydo/agents/body.md", Outcome.Allowed)]
    [InlineData(@"(?:^|[;&|]\s*)gh\s+pr\s+create\b(?![\s\S]*Independent review)", "gh pr create -F dydo/agents/body.md", Outcome.Allowed)]
    [InlineData(@"(?:^|[;&|]\s*)gh\s+pr\s+create\b(?![\s\S]*Independent review)", "FOO=1 gh pr create -F dydo/agents/body.md", Outcome.Warned)]
    [InlineData(@"(?:^|[;&|]\s*)gh\s+pr\s+create\b(?![\s\S]*Independent review)", "env -i gh pr create -F dydo/agents/body.md", Outcome.Warned)]
    [InlineData(@"gh\s+pr\s+create\b(?![\s\S]*Independent review)", "gh pr create -F dydo/agents/body.md", Outcome.Refused)]
    public async Task DydoJsonCopyOfTheReviewBlockNudge_ReadsTheBodyFileAsShipped(string pattern, string command, Outcome outcome)
    {
        await InitPushedProject();
        var shipped = ConfigFactory.DefaultNudges.Single(n => n.Pattern == ConfigFactory.ReviewBlockPattern);
        var config = JsonNode.Parse(ReadFile("dydo.json"))!.AsObject();
        config["nudges"] = new JsonArray(new JsonObject
        {
            ["pattern"] = pattern,
            ["message"] = shipped.Message,
            ["severity"] = outcome == Outcome.Refused ? "block" : shipped.Severity,
        });
        WriteFile("dydo.json", config.ToJsonString());
        // The tree check reads the local remote-tracking ref, so moving it stands in for a push.
        Commit(TestDir, "nudge");
        Git(TestDir, "update-ref", "refs/remotes/origin/feature", "HEAD");
        WriteFile("dydo/agents/body.md", ReviewedBody);

        await AssertOutcome(outcome, () => Attempt("Bash", command), command);
    }
}
