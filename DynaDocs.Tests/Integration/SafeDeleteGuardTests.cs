namespace DynaDocs.Tests.Integration;

using System.Text.Json;

[Collection("Integration")]
[Trait("Job", "contract: spec N10; DYD-1029 (safe-delete through the guard)")]
public sealed class SafeDeleteGuardTests : IntegrationTestBase
{
    [Theory]
    [InlineData("claude-bash",false)]
    [InlineData("claude-powershell",false)]
    [InlineData("codex-bash",false)]
    [InlineData("codex-pwsh",false)]
    [InlineData("codex-argv",false)]
    [InlineData("claude-bash",true)]
    [InlineData("claude-powershell",true)]
    [InlineData("codex-bash",true)]
    [InlineData("codex-pwsh",true)]
    [InlineData("codex-argv",true)]
    public async Task RawDeletionTable_IsEnforcedForEveryHookAndWorker(string envelope, bool worker)
    {
        await InitProjectAsync("none");
        string[] denied = ["git branch -D topic", "git branch -d -f topic", "git branch --delete --force topic",
            "git update-ref -d refs/heads/topic", "git push origin --delete topic", "git -C repo push origin --delete topic",
            "git -c core.bare=false push origin :refs/heads/topic", "/usr/bin/git --git-dir=repo push origin +:refs/heads/topic",
            "git push --prune origin", "git push --mirror origin", "git worktree remove tree", "git worktree remove --force tree"];
        string[] allowed = ["git branch -d merged-topic", "dydo archive branch topic", "dydo archive worktree tree", "dydo archive list", "dydo archive restore ID"];
        foreach (var command in denied)
        {
            var result = await GuardWithStdinAsync(Payload(command,envelope,worker));
            Assert.Contains("dydo archive",result.AssertPreToolUseDenied());
        }
        foreach (var command in allowed)
            (await GuardWithStdinAsync(Payload(command,envelope,worker))).AssertPreToolUseAllowed();
        if (envelope != "codex-argv")
            Assert.Contains("dydo archive",(await GuardWithStdinAsync(Payload("dydo archive list && git branch -D topic",envelope,worker))).AssertPreToolUseDenied());
    }

    private string Payload(string command, string envelope, bool worker)
    {
        object words = envelope switch
        {
            "codex-bash" => new[] { "bash", "-lc", command },
            "codex-pwsh" => new[] { "pwsh", "-Command", command },
            "codex-argv" => command.Split(' '),
            _ => command
        };
        var value = new Dictionary<string,object?>
        {
            ["session_id"] = TestSessionId, ["hook_event_name"] = "PreToolUse", ["cwd"] = TestDir,
            ["tool_name"] = envelope == "claude-powershell" ? "PowerShell" : "Bash",
            ["tool_input"] = new Dictionary<string,object> { ["command"] = words }
        };
        if (envelope.StartsWith("codex",StringComparison.Ordinal)) value["turn_id"] = "turn-safe-delete";
        if (worker) { value["agent_id"] = "safe-delete-worker"; value["agent_type"] = "code-writer"; }
        return JsonSerializer.Serialize(value);
    }

    [Fact]
    public async Task InitGeneratedCodexRules_AdmitAllArchiveForms()
    {
        await InitProjectAsync("codex");
        var rules = ReadFile(".codex/rules/dydo.rules");
        Assert.Contains("pattern = [[\"dydo\", \"dydo.exe\"], \"archive\"]",rules);
        foreach (var form in new[] { "branch topic", "worktree tree", "list", "restore ID" })
            Assert.Contains("dydo archive " + form,rules);
    }
}
