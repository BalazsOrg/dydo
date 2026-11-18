namespace DynaDocs.Tests.Integration;

/// <summary>
/// DYD-362: a worktree is removed with <c>dydo clean</c> or <c>dydo archive</c>, which keep its work.
/// The guard lets those through for the main session and for sub-agents (a Ticket Captain cleans its
/// own worktree), lets deletes inside a tree through, and refuses raw worktree removal on every try,
/// in every spelling bash, PowerShell or cmd gives it.
/// </summary>
[Collection("Integration")]
[Trait("Job", "trap: DYD-362 a raw worktree removal lost uncommitted work")]
public class GuardWorktreeRemovalTests : GuardHookTestBase
{
    private async Task<CommandResult> SubAgentShellAsync(string tool, string command)
    {
        var json = $"{{\"session_id\":\"{TestSessionId}\",\"agent_id\":\"wkr-1\",\"agent_type\":\"general-purpose\","
            + $"\"hook_event_name\":\"PreToolUse\",\"tool_name\":\"{tool}\",\"tool_input\":{{\"command\":{System.Text.Json.JsonSerializer.Serialize(command)}}}}}";
        return await GuardWithStdinAsync(json);
    }

    [Theory]
    [InlineData("Bash", "dydo clean --all")]
    [InlineData("PowerShell", "dydo clean .claude/worktrees/dyd-1")]
    [InlineData("Bash", "rm -rf .claude/worktrees/dyd-1/bin")]
    [InlineData("Bash", "rm -rf \"C:/repo/.claude/worktrees/my tree/bin\"")]
    public async Task SubAgent_DydoCleanAndDeletesInsideATree_AreAllowed(string tool, string command)
    {
        await InitProjectAsync("none");

        (await SubAgentShellAsync(tool, command)).AssertPreToolUseAllowed();
    }

    public static TheoryData<string, string> SafeForms => InLanes(BashLanes,
        "dydo clean --dry-run", "dydo clean --all", "dydo clean .claude/worktrees/dyd-1", "git worktree prune",
        "git worktree list --porcelain", "rm -f .claude/worktrees/x/notes.txt", "rm -rf bin/obj", "rm -rf /tmp/my-worktrees-backup",
        "rm -rf C:/Users/me/repo/.claude/worktrees/dyd-1/bin", "rm -rf /home/me/.t3/worktrees/DynaDocs/t3-1/node_modules",
        "rm -rf \"C:/repo/.claude/worktrees/my tree/bin\"", "Remove-Item -Force .claude\\worktrees\\x\\notes.txt",
        "Remove-Item -Recurse C:\\repo\\.claude\\worktrees\\dyd-1\\DynaDocs.Tests\\obj",
        "Remove-Item -Recurse \"C:\\repo\\.claude\\worktrees\\my tree\\DynaDocs.Tests\\obj\"",
        "Remove-Item -Recurse 'C:\\Users\\me\\.t3\\worktrees\\Dyna Docs\\my thread\\bin'");

    [Theory]
    [MemberData(nameof(SafeForms))]
    public async Task DydoCleanAndDeletesInsideATree_AreAllowed(string lane, string command)
    {
        await InitProjectAsync("none");

        AssertAllowed(lane, await ShellAsync(lane, command));
    }

    public static TheoryData<string, string> RawRemovals => Join(
        InLanes(BashLanes,
            "git worktree remove --force .claude/worktrees/dyd-1", "git worktree remove -f ../wt",
            "git -C C:/repo worktree remove C:/repo/.claude/worktrees/x --force", "git status && git worktree remove -f x",
            "git worktree remove -ff .claude/worktrees/dyd-1", "git worktree remove -f -f .claude/worktrees/dyd-1",
            "rm -rf .claude/worktrees/dyd-1", "rm -fr /home/me/.t3/worktrees/DynaDocs/t3-1",
            "rm -r -f \"C:/Users/me/repo/.claude/worktrees/x\"", "rm --recursive worktrees/x", "rm .claude/worktrees/x -rf",
            "rm -rf .claude/worktrees", "rm -rf C:/Users/me/repo/.claude/worktrees/dyd-1/", "rm -rf /home/me/.t3/worktrees/DynaDocs",
            "git -C 'C:/My Repo' worktree remove -f x", "rm -rf \"C:/repo/.claude/worktrees/my tree\"",
            "rm -rf 'C:/repo/.claude/worktrees/my tree/'", "bash -c \"rm -rf '/home/me/repo/.claude/worktrees/my tree'\"",
            "git worktree remove tree", "git -C repo worktree remove tree"),
        InLanes(EveryLane,
            "Remove-Item -Recurse -Force C:\\repo\\.claude\\worktrees\\x", "Remove-Item 'C:\\repo\\.claude\\worktrees\\x' -Recurse",
            "ri -r .claude\\worktrees\\x", "rd /s /q C:\\repo\\.claude\\worktrees\\x",
            "git -C \"C:\\My Repo\" worktree remove --force \"C:\\My Repo\\.claude\\worktrees\\x\"",
            "git -c core.longpaths=true -C \"C:\\My Repo\" worktree remove x --force",
            "Remove-Item -Recurse \"C:\\repo\\.claude\\worktrees\\my tree\"",
            "Remove-Item -Recurse -Force 'C:\\Users\\me\\.t3\\worktrees\\Dyna Docs\\my thread'"));

    [Theory]
    [MemberData(nameof(RawRemovals))]
    public async Task RawWorktreeRemoval_IsRefusedOnEveryAttempt(string lane, string command)
    {
        await InitProjectAsync("none");

        await AssertRefusedOnEveryAttempt(lane, command);
    }
}
