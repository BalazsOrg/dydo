namespace DynaDocs.Tests.Integration;

/// <summary>
/// LC-1021: Codex runs hooks through `powershell -NoProfile -Command`, which turns a native
/// exit 2 into exit 1, and Codex lets the tool run on exit 1. A PreToolUse refusal therefore
/// exits 0 with one deny-decision object on stdout, which Codex and Claude Code both honour.
/// </summary>
[Collection("Integration")]
[Trait("Job", "contract: LC-1021 the hook's answer: a PreToolUse refusal is exit 0 with one ASCII deny object, an allow is exit 0 with empty stdout, another event keeps exit 2")]
public class GuardRefusalContractTests : IntegrationTestBase
{
    private static string Hook(string tool, string toolInput, string? eventName = "PreToolUse") =>
        "{\"session_id\":\"" + TestSessionId + "\""
        + (eventName == null ? "" : ",\"hook_event_name\":\"" + eventName + "\"")
        + ",\"tool_name\":\"" + tool + "\",\"tool_input\":" + toolInput + "}";

    // Every refusal lane answers the same way: a cd chain, an unverifiable write, an off-limits
    // file-tool path, a payload without session_id, and one without hook_event_name (PreToolUse).
    [Theory]
    [InlineData("{\"session_id\":\"s\",\"hook_event_name\":\"PreToolUse\",\"tool_name\":\"Bash\",\"tool_input\":{\"command\":\"cd tools && echo hi\"}}")]
    [InlineData("{\"session_id\":\"s\",\"hook_event_name\":\"PreToolUse\",\"tool_name\":\"Bash\",\"tool_input\":{\"command\":\"echo x > $OUT\"}}")]
    [InlineData("{\"session_id\":\"s\",\"hook_event_name\":\"PreToolUse\",\"tool_name\":\"Edit\",\"tool_input\":{\"file_path\":\".env\"}}")]
    [InlineData("{\"hook_event_name\":\"PreToolUse\",\"tool_name\":\"Bash\",\"tool_input\":{\"command\":\"ls\"}}")]
    [InlineData("{\"session_id\":\"s\",\"tool_name\":\"Bash\",\"tool_input\":{\"command\":\"cd tools && echo hi\"}}")]
    public async Task Refusal_IsExitZeroWithOneDenyObject(string payload)
    {
        await InitProjectAsync("none");

        var result = await GuardWithStdinAsync(payload);

        result.AssertPreToolUseDenied();
    }

    [Fact]
    public async Task NonAsciiReason_IsEscapedToAsciiJson()
    {
        await InitProjectAsync("none");

        var result = await GuardWithStdinAsync(Hook("Read", "{\"file_path\":\"config/café/secrets.json\"}"));

        result.AssertPreToolUseDenied();
        Assert.All(result.Stdout, c => Assert.True(c < 128, $"Non-ASCII char U+{(int)c:X4} on stdout"));
    }

    // The deny object carries the same text the guard writes to stderr for a human.
    [Fact]
    public async Task Refusal_StillWritesTheHumanTextToStderr()
    {
        await InitProjectAsync("none");

        var result = await GuardWithStdinAsync(Hook("Bash", "{\"command\":\"cd tools && echo hi\"}"));

        Assert.Equal(result.AssertPreToolUseDenied(), result.Stderr.Trim());
    }

    [Fact]
    public async Task AllowedCall_ExitsZeroWithEmptyStdout()
    {
        await InitProjectAsync("none");

        var result = await GuardWithStdinAsync(Hook("Bash", "{\"command\":\"echo hi\"}"));

        result.AssertSuccess();
        Assert.Equal("", result.Stdout);
    }

    [Fact]
    public async Task OtherHookEvent_KeepsExitTwoAndEmptyStdout()
    {
        await InitProjectAsync("none");

        var result = await GuardWithStdinAsync(Hook("Bash", "{\"command\":\"cd tools && echo hi\"}", "PostToolUse"));

        result.AssertExitCode(2);
        Assert.Equal("", result.Stdout);
    }
}
