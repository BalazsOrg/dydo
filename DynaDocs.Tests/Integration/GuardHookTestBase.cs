namespace DynaDocs.Tests.Integration;

using System.Text.Json;
using DynaDocs.Services;

/// <summary>
/// Drives <c>dydo guard</c> at its hook entry. A tool name selects the lane: Claude's Bash or
/// PowerShell tool through hook JSON on stdin, or "" for the CLI's <c>--command</c>, which reads
/// the command as an unknown shell. A hook refusal is exit 0 with one deny object (LC-1021); the
/// CLI lane refuses with exit 2.
/// </summary>
public abstract class GuardHookTestBase : IntegrationTestBase
{
    /// <summary>Every lane a shell command reaches the guard through.</summary>
    protected static readonly string[] EveryLane = ["Bash", "PowerShell", ""];

    /// <summary>The lanes bash may run the command in: Claude's Bash tool and the unknown shell.</summary>
    protected static readonly string[] BashLanes = ["Bash", ""];

    /// <summary>The lanes pwsh may run the command in: the PowerShell tool and the unknown shell.</summary>
    protected static readonly string[] PowerShellLanes = ["PowerShell", ""];

    /// <summary>
    /// Claude's Bash tool as Claude Code's installed hook calls the guard (<c>--host claude</c>):
    /// bash's reading on every platform, Git Bash's on Windows, where an unmarked Bash call reads
    /// as the unknown shell.
    /// </summary>
    protected const string ClaudeBashLane = "Claude Bash";

    protected static string ShellJson(string toolName, string command) =>
        JsonSerializer.Serialize(new Dictionary<string, object>
        {
            ["session_id"] = TestSessionId,
            ["tool_name"] = toolName,
            ["tool_input"] = new Dictionary<string, string> { ["command"] = command },
        });

    protected static string FileToolJson(string toolName, string path, string? agentId = null)
    {
        var payload = new Dictionary<string, object>
        {
            ["session_id"] = TestSessionId,
            ["tool_name"] = toolName,
            ["tool_input"] = new Dictionary<string, string> { ["file_path"] = path, ["content"] = "x" },
        };
        if (agentId != null) payload["agent_id"] = agentId;
        return JsonSerializer.Serialize(payload);
    }

    protected Task<CommandResult> ShellAsync(string lane, string command) => lane switch
    {
        "" => GuardWithStdinAsync("", "--command", command),
        ClaudeBashLane => GuardWithStdinAsync(ShellJson("Bash", command), "--host", ClaudeGuardWiring.Host),
        _ => GuardWithStdinAsync(ShellJson(lane, command)),
    };

    protected static bool IsRefused(string lane, CommandResult result) =>
        lane == "" ? result.ExitCode == 2 : result.ExitCode == 0 && result.Stdout.Contains("\"deny\"");

    protected static void AssertRefused(string lane, CommandResult result)
    {
        if (lane == "") result.AssertExitCode(2);
        else result.AssertPreToolUseDenied();
    }

    protected static void AssertAllowed(string lane, CommandResult result)
    {
        if (lane == "") result.AssertSuccess();
        else result.AssertPreToolUseAllowed();
    }

    /// <summary>A hard refusal: the first try and the identical retry are both refused.</summary>
    protected async Task AssertRefusedOnEveryAttempt(string lane, string command)
    {
        for (var attempt = 0; attempt < 2; attempt++)
            AssertRefused(lane, await ShellAsync(lane, command));
    }

    /// <summary>
    /// Crosses each command with each lane it is read in. A command starting with @ skips the CLI
    /// lane, whose argv parser reads it as a response file (DYD-675, proved in GuardAtCommandTests).
    /// </summary>
    protected static TheoryData<string, string> InLanes(IEnumerable<string> lanes, params string[] commands)
    {
        var data = new TheoryData<string, string>();
        foreach (var lane in lanes)
            foreach (var command in commands)
                if (lane != "" || !command.StartsWith('@'))
                    data.Add(lane, command);
        return data;
    }

    protected static TheoryData<string, string> Join(params TheoryData<string, string>[] tables)
    {
        var data = new TheoryData<string, string>();
        foreach (var table in tables)
            foreach (var row in table)
                data.Add((string)row[0], (string)row[1]);
        return data;
    }
}
