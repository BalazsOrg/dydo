namespace DynaDocs.Tests.Integration;

using System.Text;
using System.Text.Json.Nodes;

/// <summary>
/// The built-in mode-set reminder at the hook: every way a shell may run <c>dydo mode set</c> meets
/// it once and the identical retry passes, a mention passes untouched, it cannot be configured away,
/// and any hard block still wins over it. The reading is each shell's own (DydoInvocationScanner,
/// DYD-391 r3, DYD-641); where it cannot be sure, a loose dydo ... mode ... set fails closed.
/// </summary>
[Collection("Integration")]
[Trait("Job", "contract: machine-mode spec, dydo mode set meets the reminder once per command")]
public sealed class GuardModeSetTests : IntegrationTestBase
{
    // The hook command dydo init writes for Claude Code.
    private static readonly string[] ClaudeHook = ["--host", "claude"];

    private const string Set = "mode set overnight on";

    private static string Hook(string command, string? permissionMode = "default", string eventName = "PreToolUse",
        string tool = "Bash", bool codex = false)
    {
        var hook = new JsonObject
        {
            ["session_id"] = TestSessionId,
            ["hook_event_name"] = eventName,
            ["tool_name"] = tool,
            ["tool_input"] = new JsonObject { ["command"] = command }
        };
        if (permissionMode != null)
            hook["permission_mode"] = permissionMode;
        // turn_id is a Codex-only extension of the hook payload.
        if (codex)
            hook["turn_id"] = "turn-1";
        return hook.ToJsonString();
    }

    private static string Encoded(string script) => Convert.ToBase64String(Encoding.Unicode.GetBytes(script));

    // Each row names the tool whose shell runs the command: a backslash path is a path only where
    // backslashes are literal, so C:\tools\dydo.exe runs under the PowerShell tool.
    public static TheoryData<string, string> ModeSets => new()
    {
        { "Bash", "dydo " + Set },
        { "Bash", "dydo mode set overnight off; echo done" },
        { "Bash", "env -S dydo " + Set },
        { "Bash", "env -S 'dydo' " + Set },
        { "Bash", "env -S 'dydo " + Set + "'" },
        { "Bash", "env -i --split-string='dydo " + Set + "'" },
        { "Bash", "timeout 30 env -Sdydo " + Set },
        { "Bash", "dydo mode \\\nset overnight on" },
        { "Bash", "dydo mode `\r\nset overnight on" },
        { "PowerShell", @"C:\tools\dydo.exe " + Set },
        { "Bash", @"d\ydo " + Set },
        { "Bash", "echo start\ndydo " + Set },
        { "Bash", "echo $(dydo " + Set + ")" },
        { "Bash", "git status || DYDO_X=1 dydo " + Set },
        { "Bash", "\"C:/tools/dydo.exe\" mode 'set' overnight \"off\"" },
        { "Bash", "/usr/local/bin/dydo " + Set },
        { "PowerShell", @".\dydo.ps1 " + Set },
        { "Bash", "DYDO_HOME=/x env -u PATH -i dydo " + Set },
        { "Bash", "timeout -s KILL 30 dydo " + Set },
        { "Bash", "timeout -- 30 dydo " + Set },
        { "Bash", "time -p nice -n 5 dydo " + Set },
        { "Bash", "if true; then dydo " + Set + "; fi" },
        { "Bash", "{ dydo " + Set + "; }" },
        { "Bash", "(cd x && dydo " + Set + ")" },
        { "Bash", "echo `dydo " + Set + "`" },
        { "Bash", "echo \"${HOME}$(dydo " + Set + ")\"" },
        { "Bash", "dydo " + Set + " 2>&1 >/dev/null" },
        { "Bash", "dydo " + Set + " < in.txt" },
        { "Bash", "cmd.exe /C dydo " + Set },
        { "Bash", "powershell -c \"& { dydo " + Set + " }\"" },
        { "Bash", "sh -lc \"cd /tmp; dydo " + Set + "\"" },
        { "Bash", "sudo -u me nohup dydo " + Set },
        { "Bash", "echo on | xargs -n 1 dydo mode set overnight" },
        { "Bash", "saps -FilePath C:/tools/dydo.exe -ArgumentList '" + Set + "' -Wait" },
        { "Bash", "Start-Process cmd -ArgumentList '/c dydo " + Set + "'" },
        { "Bash", "Start-Process dydo -ArgumentList \"mode\", \"set\", \"overnight\", \"off\"" },
        { "Bash", "Start-Process -args 'mode set overnight on' -FilePath dydo" },
        { "Bash", "git commit -m \"x $(dydo " + Set + ")\"" },
        { "Bash", "dotnet exec C:/src/bin/dydo.dll " + Set },
        { "Bash", "dotnet run --project DynaDocs.csproj -- " + Set },
        { "Bash", "dotnet run -p ../src/DynaDocs/ -- " + Set },
        { "Bash", "dotnet run --project=tools/dydo -- " + Set },
        { "Bash", "dotnet run -- " + Set },
        { "Bash", "dotnet --roll-forward Major dydo.dll " + Set },
        { "Bash", "dydo mode set \"overnight on" },
        { "Bash", "eval 'dydo " + Set + "'" },
        { "Bash", "bash <<< 'dydo " + Set + "'" },
        { "Bash", "bash <<'EOF'\ndydo " + Set + "\nEOF" },
        { "Bash", "bash -o pipefail --norc -c 'dydo " + Set + "'" },
        { "Bash", "pwsh -NoLogo -ExecutionPolicy Bypass -c dydo " + Set },
        { "Bash", "start \"\" /b dydo " + Set },
        { "Bash", "cat <<EOF\n$(dydo " + Set + ")\nEOF" },
        { "Bash", "echo dydo " + Set + " | bash" },
        { "Bash", "pwsh -NoProfile -EncodedCommand " + Encoded("dydo " + Set) },
        { "PowerShell", "Start-Process dydo -ArgumentList @('mode', 'set', 'overnight', 'on') -Wait" },
        { "PowerShell", "Start-Process dydo -ArgumentList ('mode', 'set', 'overnight', 'on')" },
        { "PowerShell", "Start-Process -FilePath dydo -ArgumentList @(\"mode\", \"set\") -NoNewWindow" },
        { "PowerShell", "Write-Output (dydo " + Set + ")" },
        { "PowerShell", "<# note\ndydo " + Set },
        { "PowerShell", "if ($x) { dydo " + Set + " }" },
        { "PowerShell", "iex -Command 'dydo " + Set + "'" },
        { "PowerShell", @"echo ""C:\""; dydo " + Set },
        { "PowerShell", "@\"\n$(dydo " + Set + ")\n\"@ | Out-Null" },
        { "PowerShell", "echo 'x'\rdydo " + Set },
        { "PowerShell", "echo x # note\rdydo " + Set },
        { "PowerShell", "echo ‘a'; dydo " + Set + "; ’b'" },
        { "PowerShell", "echo \"a”; dydo " + Set + "; “b\"" },
        { "PowerShell", "echo a^;dydo " + Set },
    };

    [Theory]
    [MemberData(nameof(ModeSets))]
    public async Task ModeSet_MeetsTheReminderOnce_AndTheRetryPasses(string tool, string command)
    {
        await InitProjectAsync("none");
        var hook = Hook(command, tool: tool);

        (await GuardWithStdinAsync(hook, ClaudeHook)).AssertPreToolUseDenied();
        (await GuardWithStdinAsync(hook, ClaudeHook)).AssertPreToolUseAllowed();
    }

    // A mention, quoted or literal text, a comment, another program's arguments, or another
    // project's dotnet run is no mode set, read as the tool's shell reads it.
    public static TheoryData<string, string> Mentions => new()
    {
        { "Bash", "dydo mode show" },
        { "Bash", "echo dydo mode" },
        { "Bash", "git commit -m \"fix: the dydo mode set nudge\"" },
        { "Bash", "grep -rn \"dydo mode set\" Commands" },
        { "Bash", "echo dydo.exe " + Set },
        { "Bash", "echo 'dydo " + Set + "'" },
        { "Bash", "echo \"dydo " + Set + "\" | grep dydo" },
        { "Bash", "cmd /c echo dydo " + Set },
        { "Bash", "bash script.sh dydo " + Set },
        { "Bash", "pwsh -File x.ps1 dydo " + Set },
        { "Bash", "cat <<'EOF'\n$(dydo " + Set + ")\nEOF" },
        { "Bash", "env -S 'echo dydo " + Set + "'" },
        { "Bash", "git commit -m \"fix guard\n\nTests pass; dydo mode set now asks\nand | dydo " + Set + "\"" },
        { "Bash", "@'\nfix guard; dydo mode set must ask\n'@ | git commit -F -" },
        { "Bash", "@\"\nfix \"guard\"; dydo mode set must ask\n\"@ | git commit -F -" },
        { "Bash", "git commit -m \"$(cat <<'EOF'\nfix guard\n\ndydo " + Set + " now asks, it doesn't deny\nEOF\n)\"" },
        { "Bash", "grep -rn 'dydo mode set' Commands | head" },
        { "Bash", "echo 'a;b' && git log --grep=\"dydo mode set\"" },
        { "Bash", "dotnet run --project examples/OtherCli -- mode set dark on" },
        { "Bash", "dotnet run -p OtherCli.csproj -- mode set dark on" },
        { "Bash", "git commit -m \"fix \\\"mode\\\"; dydo mode set now asks\"" },
        { "Bash", "git commit -m \"dir C:\\\\\" -m \"then; dydo mode set now asks\"" },
        { "PowerShell", "git commit -m \"fix `\"mode`\"; dydo mode set now asks\"" },
        { "PowerShell", "gh pr comment 12 --body \"fix `\"mode`\"; dydo mode set now asks\"" },
        { "PowerShell", @"git commit -m ""path C:\"" -m ""note; dydo mode set now asks""" },
        { "PowerShell", @"git commit -m ""fix """"mode""""; dydo mode set now asks""" },
        { "PowerShell", @"git commit -m 'it''s; dydo mode set now asks'" },
        { "PowerShell", "<#\ndydo " + Set + "\n#>\ngit status" },
        { "PowerShell", "git status <# dydo " + Set + " #>; git log" },
        { "PowerShell", "echo \"a `\"b`\"; dydo " + Set + "\"" },
    };

    [Theory]
    [MemberData(nameof(Mentions))]
    public async Task Mention_PassesUntouched(string tool, string command)
    {
        await InitProjectAsync("none");

        (await GuardWithStdinAsync(Hook(command, tool: tool), ClaudeHook)).AssertPreToolUseAllowed();
    }

    // A notice nudge in dydo.json on the same command cannot downgrade the built-in reminder.
    [Fact]
    public async Task ConfigNudgeOnTheSameCommand_CannotDowngradeIt()
    {
        await InitProjectAsync("none");
        var config = JsonNode.Parse(ReadFile("dydo.json"))!.AsObject();
        config["nudges"] = new JsonArray(new JsonObject
        {
            ["pattern"] = @"dydo\s+mode\s+set",
            ["message"] = "fine",
            ["severity"] = "notice"
        });
        WriteFile("dydo.json", config.ToJsonString());

        (await GuardWithStdinAsync(Hook("dydo " + Set), ClaudeHook)).AssertPreToolUseDenied();
    }

    // The reminder holds on every lane a host may send: a payload without a permission mode, another
    // hook event (exit 2, then 0) and the CLI's --command (exit 2, then 0).
    [Theory]
    [InlineData("no permission mode")]
    [InlineData("other hook event")]
    [InlineData("cli")]
    public async Task EveryLane_MeetsTheReminderOnceThenPasses(string lane)
    {
        await InitProjectAsync("none");
        Task<CommandResult> Attempt() => lane switch
        {
            "no permission mode" => GuardWithStdinAsync(Hook("dydo " + Set, permissionMode: null), ClaudeHook),
            "other hook event" => GuardWithStdinAsync(Hook("dydo " + Set, eventName: "PostToolUse"), ClaudeHook),
            _ => RunAsync(DynaDocs.Commands.GuardCommand.Create(), "--command", "dydo " + Set),
        };

        var first = await Attempt();
        var retry = await Attempt();

        if (lane == "no permission mode")
        {
            first.AssertPreToolUseDenied();
            retry.AssertPreToolUseAllowed();
            return;
        }
        first.AssertExitCode(2);
        Assert.Equal("", first.Stdout);
        retry.AssertSuccess();
        Assert.Equal("", retry.Stdout);
    }

    // DYD-386: a hard block on the same command wins over the reminder, on the retry too.
    [Fact]
    [Trait("Job", "contract: DYD-386 warn nudges run after every hard block")]
    public async Task AnotherBlock_WinsOverTheReminder()
    {
        await InitProjectAsync("none");
        var hook = Hook("dydo " + Set + " && cat .env");

        (await GuardWithStdinAsync(hook, ClaudeHook)).AssertPreToolUseDenied();
        (await GuardWithStdinAsync(hook, ClaudeHook)).AssertPreToolUseDenied();
    }

    // Codex sends its shell calls as Bash with no host marker, and runs them in PowerShell on
    // Windows and in the user's POSIX shell elsewhere.
    [Fact]
    public async Task CodexShellCall_IsReadWithTheShellCodexRuns()
    {
        await InitProjectAsync("none");

        var result = await GuardWithStdinAsync(
            Hook("git commit -m \"fix `\"mode`\"; dydo mode set now asks\"", codex: true));

        if (OperatingSystem.IsWindows())
            result.AssertPreToolUseAllowed();
        else
            result.AssertPreToolUseDenied();
    }

    // The retry is keyed by the whole command, whitespace normalised: another mode-set command meets
    // the reminder afresh, and only the same command passes.
    [Fact]
    public async Task ModeSetWarn_KeysTheFullCommandAndNormalizesWhitespace()
    {
        await InitProjectAsync("none");
        var commands = new[]
        {
            "dydo mode set overnight on", "dydo mode set overnight off",
            "dydo mode set overnight on; echo done",
            "  dydo\tmode  set overnight on  ", "dydo mode set overnight off",
            "dydo mode set overnight on; echo done", "dydo mode set overnight on"
        };
        var allowed = new[] { false, false, false, true, true, true, false };
        for (var index = 0; index < commands.Length; index++)
        {
            var result = await GuardWithStdinAsync(Hook(commands[index]));
            if (allowed[index])
                result.AssertPreToolUseAllowed();
            else
                result.AssertPreToolUseDenied();
        }
    }
}
