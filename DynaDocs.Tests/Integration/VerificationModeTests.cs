namespace DynaDocs.Tests.Integration;

using System.Text.Json;
using DynaDocs.Commands;
using DynaDocs.Services;

/// <summary>
/// A `dydo mode set` reaching the guard is warned once and passes on retry, and the guard itself
/// never changes the mode: only the command's own run does.
/// </summary>
[Collection("Integration")]
[Trait("Job", "contract: machine-mode.feature (an executing mode set warns once, then passes the same command's retry); DYD-351")]
public sealed class VerificationModeTests : IntegrationTestBase
{
    [Theory]
    [InlineData("off", "on")]
    [InlineData("on", "off")]
    public async Task ModeSet_ThroughTheGuard_WarnsOncePassesTheRetry_AndOnlyTheCommandChangesTheMode(string initial, string target)
    {
        var root = Path.Combine(TestDir, "machine-state");
        await RunAsync(ModeCommand.Create(new ModeState(root)), "set", "overnight", initial);
        await InitProjectAsync("none");
        var payload = JsonSerializer.Serialize(new
        {
            session_id = TestSessionId,
            hook_event_name = "PreToolUse",
            tool_name = "shell_command",
            permission_mode = "default",
            tool_input = new { command = $"dydo mode set overnight {target}" }
        });

        var first = await GuardWithStdinAsync(payload);
        var retry = await GuardWithStdinAsync(payload);
        var afterGuard = await RunAsync(ModeCommand.Create(new ModeState(root)), "show");
        await RunAsync(ModeCommand.Create(new ModeState(root)), "set", "overnight", target);
        var afterSet = await RunAsync(ModeCommand.Create(new ModeState(root)), "show");

        first.AssertPreToolUseDenied();
        retry.AssertPreToolUseAllowed();
        Assert.Equal($"overnight: {initial}", afterGuard.Stdout.Trim());
        Assert.Equal($"overnight: {target}", afterSet.Stdout.Trim());
    }
}
