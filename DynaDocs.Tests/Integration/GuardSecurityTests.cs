namespace DynaDocs.Tests.Integration;

using DynaDocs.Models;
using DynaDocs.Services;

/// <summary>
/// Guard bypasses found by the guard inquisition and later retros, each driven through the hook entry:
/// the call is refused or allowed, and a protected canary keeps its bytes. Rows that only the analyzer
/// once pinned live in GuardShellCorpusTests.
/// </summary>
[Collection("Integration")]
public class GuardSecurityTests : IntegrationTestBase
{
    [Trait("Job", "edge: DYD-723 a dangerous-rule match timeout fails closed; no command times a rule out cheaply")]
    [Theory]
    [InlineData("Bash")]
    [InlineData("PowerShell")]
    [InlineData("")]
    public async Task PythonFallbackTimeout_HookRefusesOnEveryAttempt(string toolName)
    {
        await SetupClaimedAgent();
        var patterns = ((Func<string, bool> IsMatch, string Reason)[])typeof(BashCommandAnalyzer)
            .GetField("DangerousPatterns", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static)!
            .GetValue(null)!;
        var index = Array.FindIndex(patterns, pattern => pattern.IsMatch.Method.Name == "RunsPythonFromStdin");
        Assert.True(index >= 0);
        var original = patterns[index];
        try
        {
            patterns[index] = (_ => throw new System.Text.RegularExpressions.RegexMatchTimeoutException(), original.Reason);
            for (var attempt = 0; attempt < 2; attempt++)
            {
                var result = toolName == "" ? await UnknownShellGuardAsync("echo ready")
                    : await GuardWithStdinAsync(ShellJson(toolName, "echo ready"));
                AssertRefused(toolName, result);
            }
        }
        finally
        {
            patterns[index] = original;
        }
    }

    // A benign command padded to exactly 1 MiB is read and allowed; one character more is refused
    // on every attempt, whatever it says, so no input is too long to bound (DYD-723).
    [Trait("Job", "trap: DYD-723 command input is bounded at 1 MiB")]
    [Theory]
    [InlineData("Bash", 1_048_576, false)]
    [InlineData("PowerShell", 1_048_576, false)]
    [InlineData("", 1_048_576, false)]
    [InlineData("Bash", 1_048_577, true)]
    [InlineData("PowerShell", 1_048_577, true)]
    [InlineData("", 1_048_577, true)]
    public async Task LengthBoundary_OnlyACommandPastOneMebibyteIsRefused(string toolName, int length, bool refused)
    {
        await SetupClaimedAgent();
        var command = "echo ready".PadRight(length, ' ');

        for (var attempt = 0; attempt < 2; attempt++)
        {
            var result = toolName == "" ? await UnknownShellGuardAsync(command)
                : await GuardWithStdinAsync(ShellJson(toolName, command));
            if (refused) AssertRefused(toolName, result);
            else if (toolName == "") result.AssertSuccess();
            else result.AssertPreToolUseAllowed();
        }
    }

    [Trait("Job", "trap: DYD-723 group-word origins keep the poll-loop nudge's verdict")]
    [Theory]
    [InlineData("Write-Output @(Write-Output @(<# tail -f #> x))")]
    [InlineData("Write-Output @(<# outer #> Write-Output @(<# tail -f #> x))")]
    [InlineData("Write-Output @(Write-Output @(# tail -f\nx))")]
    public async Task NestedGroupComment_DoesNotConsumePollingNudgeRetry(string command)
    {
        await SetupClaimedAgent();
        for (var attempt = 0; attempt < 2; attempt++)
        {
            var result = await GuardWithStdinAsync(ShellJson("PowerShell", command));
            result.AssertPreToolUseAllowed();
        }
    }

    [Trait("Job", "trap: DYD-723 group-word origins keep the poll-loop nudge's verdict")]
    [Theory]
    [InlineData("Write-Output @(Write-Output @(rg 'tail -f' notes))", false)]
    [InlineData("echo @(cat <<E\n$(rg -n 'tail -f' docs)\nE\n)", true)]
    [InlineData("echo @(cat <<E\r\n$(rg -n 'tail -f' docs)\r\nE\r\n)", true)]
    public async Task NestedGroupData_KeepsWarningOrderAndRetry(string command, bool hardBlock)
    {
        await SetupClaimedAgent();
        var first = await GuardWithStdinAsync(ShellJson("PowerShell", command));
        first.AssertPreToolUseDenied();
        var retry = await GuardWithStdinAsync(ShellJson("PowerShell", command));
        if (hardBlock) retry.AssertPreToolUseDenied();
        else retry.AssertPreToolUseAllowed();
    }

    [Trait("Job", "trap: DYD-610 a nudge match that times out is refused, not waved through")]
    [Theory]
    [InlineData("block", "command")]
    [InlineData("warn", "command")]
    [InlineData("notice", "command")]
    [InlineData("warn", "new-file")]
    public async Task NudgeMatchTimeout_IsRefusedOnEveryAttempt(string severity, string match)
    {
        await InitProjectAsync("none");
        var path = Path.Combine(TestDir, ConfigService.ConfigFileName);
        var config = System.Text.Json.Nodes.JsonNode.Parse(File.ReadAllText(path))!;
        config["nudges"] = System.Text.Json.JsonSerializer.SerializeToNode(new[]
        {
            new NudgeConfig { Pattern = "^(a+)+$", Message = "Slow configured nudge", Severity = severity, Match = match }
        });
        File.WriteAllText(path, config.ToJsonString());
        var command = new string('a', 26) + "!";
        var payload = match == "command" ? BashJson(command) : System.Text.Json.JsonSerializer.Serialize(new
        {
            session_id = TestSessionId, cwd = TestDir, tool_name = "Write",
            tool_input = new { file_path = command, content = "x" }
        });
        for (var attempt = 0; attempt < 2; attempt++)
        {
            var result = await GuardWithStdinAsync(payload);
            result.AssertPreToolUseDenied();
        }
    }

    private async Task SetupClaimedAgent()
    {
        await InitProjectAsync("none");
    }

    private string BashJson(string command) =>
        "{\"session_id\":\"" + TestSessionId + "\",\"tool_name\":\"Bash\",\"tool_input\":{\"command\":\"" +
        command.Replace("\\", "\\\\").Replace("\"", "\\\"") + "\"}}";

    // ================================================================
    // Finding 3: Command substitution - tainted writes blocked
    // ================================================================

    [Trait("Job", "trap: guard inquisition finding 3, a write behind an expansion is unverifiable")]
    [Fact]
    public async Task Finding3_TaintedWriteOperation_Blocked()
    {
        await SetupClaimedAgent();

        var result = await GuardWithStdinAsync(
            BashJson("echo $PAYLOAD > src/output.txt"));
        result.AssertPreToolUseDenied();
    }

    [Trait("Job", "trap: guard inquisition finding 3, a write behind an expansion is unverifiable")]
    [Fact]
    public async Task Finding3_TaintedReadOperation_Allowed()
    {
        await SetupClaimedAgent();

        var result = await GuardWithStdinAsync(BashJson("cat $FILENAME"));
        result.AssertPreToolUseAllowed();
    }

    // ================================================================
    // Finding 5: Command substitution hiding entire write operations
    // ================================================================

    [Trait("Job", "trap: guard inquisition finding 5, a write hidden in a substitution")]
    [Fact]
    public async Task Finding5_WriteHiddenInSubstitution_Blocked()
    {
        await SetupClaimedAgent();

        var result = await GuardWithStdinAsync(
            BashJson("echo $(cp secret.txt /tmp/leak.txt)"));
        result.AssertPreToolUseDenied();
    }

    [Trait("Job", "trap: guard inquisition finding 5, a write hidden in a substitution")]
    [Fact]
    public async Task Finding5_ReadOnlyInSubstitution_Allowed()
    {
        await SetupClaimedAgent();

        var result = await GuardWithStdinAsync(
            BashJson("echo $(cat readme.txt)"));
        result.AssertPreToolUseAllowed();
    }

    // ================================================================
    // Finding 4: nudges fire for every shell command, dydo included (DR-041 Part C),
    // so prefixing a dydo call cannot bypass a block nudge.
    // ================================================================

    [Trait("Job", "contract: DR 041 Part C, nudges and dangerous rules apply to dydo command lines too")]
    [Fact]
    public async Task Finding4_DydoMatchingPattern_NowFires()
    {
        await SetupClaimedAgent();

        var result = await GuardWithStdinAsync(
            BashJson("dydo task list && tail -f build.log"));

        result.AssertPreToolUseDenied();
    }

    [Trait("Job", "contract: DR 041 Part C, nudges and dangerous rules apply to dydo command lines too")]
    [Fact]
    public async Task Finding4_DangerousPattern_CheckedBeforeNudges()
    {
        await SetupClaimedAgent();

        var result = await GuardWithStdinAsync(BashJson("rm -rf /"));
        result.AssertPreToolUseDenied();
    }

    // ================================================================
    // Issue #155: dydo command chains must retain full bash security
    // ================================================================

    [Trait("Job", "trap: issue #155, a dydo command chain keeps full shell analysis")]
    [Fact]
    public async Task Issue155_DydoChain_OffLimitsRead_Blocked()
    {
        await SetupClaimedAgent();

        var result = await GuardWithStdinAsync(
            BashJson("dydo whoami && cat config/secrets.json"));

        result.AssertPreToolUseDenied();
    }

    [Trait("Job", "trap: issue #155, a dydo command chain keeps full shell analysis")]
    [Fact]
    public async Task Issue155_DydoChain_OffLimitsWrite_Blocked()
    {
        await SetupClaimedAgent();

        var result = await GuardWithStdinAsync(
            BashJson("dydo whoami && tee config/secrets.json"));

        result.AssertPreToolUseDenied();
    }

    [Trait("Job", "trap: issue #155, a dydo command chain keeps full shell analysis")]
    [Fact]
    public async Task Issue155_DydoChain_DangerousPattern_Blocked()
    {
        await SetupClaimedAgent();

        var result = await GuardWithStdinAsync(BashJson("dydo whoami && rm -rf /"));

        result.AssertPreToolUseDenied();
    }

    [Trait("Job", "trap: issue #155, a dydo command chain keeps full shell analysis")]
    [Fact]
    public async Task Issue155_DydoNotFirst_DangerousPattern_Blocked()
    {
        await SetupClaimedAgent();

        var result = await GuardWithStdinAsync(BashJson("rm -rf / && dydo whoami"));

        result.AssertPreToolUseDenied();
    }

    [Trait("Job", "trap: issue #155, a dydo command chain keeps full shell analysis")]
    [Fact]
    public async Task Issue155_BareDydoCommand_Succeeds()
    {
        await SetupClaimedAgent();

        var result = await GuardWithStdinAsync(BashJson("dydo whoami"));

        result.AssertPreToolUseAllowed();
    }

    private string PowerShellJson(string command) =>
        "{\"session_id\":\"" + TestSessionId + "\",\"tool_name\":\"PowerShell\",\"tool_input\":{\"command\":\"" +
        command.Replace("\\", "\\\\").Replace("\"", "\\\"") + "\"}}";

    [Trait("Job", "trap: bug B, the PowerShell tool bypassed the shell guard")]
    [Fact]
    public async Task BugB_PowerShell_DangerousPattern_IsBlocked()
    {
        // PowerShell must route through HandleBashCommand like Bash; unrouted, it reached
        // HandleWriteOperation with a null filePath and passed - a full guard bypass.
        await SetupClaimedAgent();

        var result = await GuardWithStdinAsync(PowerShellJson("rm -rf /"));
        result.AssertPreToolUseDenied();
    }

    // ================================================================
    // Issue #295: codex shell tool names must route through the shell guard
    // ================================================================
    //
    // Codex names its shell tool shell_command (also exec/local_shell/unified_exec by mode),
    // not Bash/PowerShell. Unrouted, these reach HandleWriteOperation with a null filePath and
    // pass, so every codex shell command runs unguarded.

    private string CodexShellJson(string toolName, string command) =>
        "{\"session_id\":\"" + TestSessionId + "\",\"tool_name\":\"" + toolName + "\",\"tool_input\":{\"command\":\"" +
        command.Replace("\\", "\\\\").Replace("\"", "\\\"") + "\"}}";

    [Trait("Job", "trap: issue #295, Codex shell tool names bypassed the shell guard")]
    [Theory]
    [InlineData("shell_command")]
    [InlineData("exec")]
    [InlineData("local_shell")]
    [InlineData("unified_exec")]
    public async Task Issue295_CodexShellTool_OffLimitsRead_IsBlocked(string toolName)
    {
        // The reported repro: a codex worker read an off-limits agent state file and got the
        // contents back, exit 0, no block.
        await SetupClaimedAgent();

        var result = await GuardWithStdinAsync(
            CodexShellJson(toolName, "cat config/secrets.json"));
        result.AssertPreToolUseDenied();
    }

    [Trait("Job", "trap: issue #295, Codex shell tool names bypassed the shell guard")]
    [Fact]
    public async Task Issue295_CodexShellTool_OffLimitsWrite_IsBlocked()
    {
        await SetupClaimedAgent();

        var result = await GuardWithStdinAsync(CodexShellJson("shell_command", "tee config/secrets.json"));
        result.AssertPreToolUseDenied();
    }

    [Trait("Job", "trap: issue #295, Codex shell tool names bypassed the shell guard")]
    [Fact]
    public async Task Issue295_CodexShellTool_DangerousPattern_IsBlocked()
    {
        await SetupClaimedAgent();

        var result = await GuardWithStdinAsync(CodexShellJson("shell_command", "rm -rf /"));
        result.AssertPreToolUseDenied();
    }

    [Trait("Job", "trap: issue #295, Codex shell tool names bypassed the shell guard")]
    [Fact]
    public async Task Issue295_CodexShellTool_BenignCommand_IsAllowed()
    {
        await SetupClaimedAgent();

        var result = await GuardWithStdinAsync(CodexShellJson("shell_command", "git status --short"));
        result.AssertPreToolUseAllowed();
    }

    // ================================================================
    // Issue #85: bash -c / sh -c inner command analysis
    // ================================================================

    [Trait("Job", "trap: issue #85, bash -c runs its string as a command line")]
    [Fact]
    public async Task Issue85_BashC_InnerOffLimitsWrite_Blocked()
    {
        await SetupClaimedAgent();

        // Real quotes around -c argument - BashJson handles JSON escaping
        var result = await GuardWithStdinAsync(
            BashJson("bash -c \"tee config/secrets.json\""));
        result.AssertPreToolUseDenied();
    }

    // ================================================================
    // Issue #86: Variable hiding command name
    // ================================================================

    [Trait("Job", "trap: issue #86, a variable hiding the command name")]
    [Fact]
    public async Task Issue86_VariableHidingWriteCommand_Blocked()
    {
        await SetupClaimedAgent();

        var result = await GuardWithStdinAsync(BashJson("$CMD dydo/files-off-limits.md"));
        result.AssertPreToolUseDenied();
    }

    // ================================================================
    // Issue #64: Block-severity nudges can't be removed from config
    // ================================================================

    [Trait("Job", "trap: issue #64, block-severity system nudges cannot be removed or downgraded in dydo.json")]
    [Theory]
    [InlineData("removed")]
    [InlineData("downgraded")]
    public async Task Issue64_IndirectDydo_IsBlockedWhateverDydoJsonSaysOfTheNudge(string edit)
    {
        await InitProjectAsync("none");
        var path = Path.Combine(TestDir, ConfigService.ConfigFileName);
        var config = System.Text.Json.Nodes.JsonNode.Parse(File.ReadAllText(path))!;
        if (edit == "removed")
            config["nudges"] = new System.Text.Json.Nodes.JsonArray();
        else
            foreach (var nudge in config["nudges"]!.AsArray())
                nudge!["severity"] = "warn";
        File.WriteAllText(path, config.ToJsonString());

        for (var attempt = 0; attempt < 2; attempt++)
            (await GuardWithStdinAsync(BashJson("npx dydo agent claim auto"))).AssertPreToolUseDenied();
    }

    [Trait("Job", "trap: issue #60, shell reads and writes honour the off-limits tier")]
    [Fact]
    public async Task BashReadOperation_AllowedForClaimedAgent()
    {
        await SetupClaimedAgent();

        var result = await GuardWithStdinAsync(BashJson("cat README.md"));
        result.AssertPreToolUseAllowed();
    }

    [Trait("Job", "trap: issue #60, shell reads and writes honour the off-limits tier")]
    [Fact]
    public async Task BashWriteToAllowedPath_Succeeds()
    {
        await SetupClaimedAgent();

        var result = await GuardWithStdinAsync(
            BashJson("tee scratch/notes.md"));
        result.AssertPreToolUseAllowed();
    }

    [Trait("Job", "trap: issue #60, shell reads and writes honour the off-limits tier")]
    [Fact]
    public async Task BashDeleteOperation_OffLimitsBlocked()
    {
        await SetupClaimedAgent();

        var result = await GuardWithStdinAsync(
            BashJson("rm config/secrets.json"));
        result.AssertPreToolUseDenied();
    }

    [Trait("Job", "trap: issue #60, shell reads and writes honour the off-limits tier")]
    [Fact]
    public async Task Issue60_BashReadOffLimitsNonMode_Blocked()
    {
        await SetupClaimedAgent();

        var result = await GuardWithStdinAsync(
            BashJson("cat config/secrets.json"));
        result.AssertPreToolUseDenied();
    }

    // ================================================================
    // DYD-406: the 3.6.0 session retro's guard misreadings, through the hook
    // ================================================================

    private static string ShellJson(string toolName, string command) =>
        System.Text.Json.JsonSerializer.Serialize(new Dictionary<string, object>
        {
            ["session_id"] = TestSessionId,
            ["tool_name"] = toolName,
            ["tool_input"] = new Dictionary<string, string> { ["command"] = command },
        });

    // GS-02: literal data written to a literal destination is no bypass.
    [Trait("Job", "trap: DYD-406 GS-02 and DYD-422, literal data is no bypass; a guarded or dynamic write still is")]
    [Theory]
    [InlineData("PowerShell", GuardRetroCommands.Gs02SetContentEvidence)]
    [InlineData("PowerShell", GuardRetroCommands.Gs02SetContentVerdict)]
    [InlineData("Bash", GuardRetroCommands.Gs02CatSlotScript)]
    [InlineData("Bash", GuardRetroCommands.Gs02CatAppendEvidence)]
    [InlineData("Bash", "cat > notes.md <<'EOF'\nplain words\nEOF")]
    public async Task Gs02_LiteralDataWrite_ToALiteralDestination_IsAllowed(string toolName, string command)
    {
        await SetupClaimedAgent();

        var result = await GuardWithStdinAsync(ShellJson(toolName, command));

        result.AssertPreToolUseAllowed();
    }

    // The CLI lane reads its command as an unknown shell, both ways.
    private Task<CommandResult> UnknownShellGuardAsync(string command) => GuardWithStdinAsync("", "--command", command);

    // DYD-422: one sed, printf or echo command writing single-quoted literal data to a literal
    // destination is no bypass, through Claude's Bash and PowerShell tools and in an unknown shell.
    [Trait("Job", "trap: DYD-406 GS-02 and DYD-422, literal data is no bypass; a guarded or dynamic write still is")]
    [Theory]
    [InlineData("sed -i 's/`x`/y/' f")]
    [InlineData("printf '%s' 'first line\n$HOME is literal' > notes.md")]
    [InlineData("echo 'cost: $5 and `date`' > notes.md")]
    public async Task Gs02_SingleQuotedLiteralDataWrite_IsAllowed(string command)
    {
        await SetupClaimedAgent();

        var bash = await GuardWithStdinAsync(ShellJson("Bash", command));
        var powerShell = await GuardWithStdinAsync(ShellJson("PowerShell", command));
        var unknown = await UnknownShellGuardAsync(command);

        bash.AssertPreToolUseAllowed();
        powerShell.AssertPreToolUseAllowed();
        unknown.AssertSuccess();
    }

    // Literal data keeps its path checks: a guarded destination, or a guarded path a substitution
    // in it names, is refused for that path.
    [Trait("Job", "trap: DYD-406 GS-02 and DYD-422, literal data is no bypass; a guarded or dynamic write still is")]
    [Theory]
    [InlineData("printf '%s' 'first line\n$HOME is literal' > dydo/index.md")]
    [InlineData("sed -i 's/`x`/y/' dydo/files-off-limits.md")]
    [InlineData("sed -i 's#`rm dydo/index.md`#y#' notes.md")]
    [InlineData("printf '%s' '$HOME' > .env")]
    public async Task Gs02_SingleQuotedLiteralDataWrite_ToAGuardedPath_IsStillDenied(string command)
    {
        await SetupClaimedAgent();

        var bash = await GuardWithStdinAsync(ShellJson("Bash", command));
        var unknown = await UnknownShellGuardAsync(command);

        bash.AssertPreToolUseDenied();
        unknown.AssertExitCode(2);
    }

    // Single-quoted text no reading certifies as literal data is refused as on master.
    [Trait("Job", "trap: DYD-406 GS-02 and DYD-422, literal data is no bypass; a guarded or dynamic write still is")]
    [Fact]
    public async Task Gs02_SingleQuotedSetContentWrite_IsDeniedAsOnMaster()
    {
        await SetupClaimedAgent();

        var result = await GuardWithStdinAsync(ShellJson("PowerShell", "Set-Content notes.md 'first line\n$HOME is literal'"));

        result.AssertPreToolUseDenied();
    }

    // DYD-422: where a program on the line may run the single-quoted text, a reading may not be the
    // shell's, or the text sits in a comment or a body, the call is refused as on master and the
    // protected canary keeps its bytes.
    [Trait("Job", "trap: DYD-406 GS-02 and DYD-422, literal data is no bypass; a guarded or dynamic write still is")]
    [Theory]
    [InlineData("eval 'echo x' '> $OUT'")]
    [InlineData("iex 'echo x' '> $OUT'")]
    [InlineData("cmd /c echo 'x > $OUT'")]
    [InlineData("env -S 'echo x' '> $OUT'")]
    [InlineData("pwsh -Command 'echo x' '> $OUT'")]
    [InlineData("ssh host 'echo x' '> $OUT'")]
    [InlineData("watch -n 1 'echo x > $OUT'")]
    [InlineData("echo don\\'t $(rm dydo/index.md) won\\'t")]
    [MemberData(nameof(GuardRetroCommands.Gs02SingleQuotedLiteralLines), MemberType = typeof(GuardRetroCommands))]
    [MemberData(nameof(GuardRetroCommands.Gs02StateChangingLines), MemberType = typeof(GuardRetroCommands))]
    public async Task Gs02_SingleQuotedTextAProgramMayRun_IsDeniedAndTheCanaryUnchanged(string command)
    {
        await SetupClaimedAgent();
        var canary = Path.Combine(DydoDir, "index.md");
        var before = File.ReadAllBytes(canary);

        var bash = await GuardWithStdinAsync(ShellJson("Bash", command));
        var unknown = await UnknownShellGuardAsync(command);

        bash.AssertPreToolUseDenied();
        unknown.AssertExitCode(2);
        Assert.Equal(before, File.ReadAllBytes(canary));
    }

    // DYD-422 review: every route the reviews found (a second command, a file the line writes and
    // then runs, an option off sed's list, a word the shell may expand, text only PowerShell runs)
    // is refused through Claude's Bash and PowerShell tools and an unknown shell, as on master.
    [Trait("Job", "trap: DYD-406 GS-02 and DYD-422, literal data is no bypass; a guarded or dynamic write still is")]
    [Theory]
    [MemberData(nameof(GuardRetroCommands.Gs02WriteThenRunLines), MemberType = typeof(GuardRetroCommands))]
    [MemberData(nameof(GuardRetroCommands.Gs02OptionLines), MemberType = typeof(GuardRetroCommands))]
    [MemberData(nameof(GuardRetroCommands.Gs02OptionValueLines), MemberType = typeof(GuardRetroCommands))]
    [MemberData(nameof(GuardRetroCommands.Gs02NarrowedLines), MemberType = typeof(GuardRetroCommands))]
    [MemberData(nameof(GuardRetroCommands.Gs02ExpandingWordLines), MemberType = typeof(GuardRetroCommands))]
    [MemberData(nameof(GuardRetroCommands.Gs02PowerShellReadingLines), MemberType = typeof(GuardRetroCommands))]
    [MemberData(nameof(GuardRetroCommands.Gs02BreakCharacterLines), MemberType = typeof(GuardRetroCommands))]
    public async Task Gs02_WriteThenRun_IsDeniedInEveryShell(string command)
    {
        await SetupClaimedAgent();

        var bash = await GuardWithStdinAsync(ShellJson("Bash", command));
        var powerShell = await GuardWithStdinAsync(ShellJson("PowerShell", command));
        var unknown = await UnknownShellGuardAsync(command);

        bash.AssertPreToolUseDenied();
        powerShell.AssertPreToolUseDenied();
        unknown.AssertExitCode(2);
    }

    // Split seat D: a Codex pwsh argv envelope runs its script in PowerShell, from a caller read as
    // bash; relief needs PowerShell's reading too, so the text it runs is refused as on master.
    [Trait("Job", "trap: DYD-406 GS-02 and DYD-422, literal data is no bypass; a guarded or dynamic write still is")]
    [Theory]
    [MemberData(nameof(GuardRetroCommands.Gs02PowerShellReadingLines), MemberType = typeof(GuardRetroCommands))]
    [MemberData(nameof(GuardRetroCommands.Gs02BreakCharacterLines), MemberType = typeof(GuardRetroCommands))]
    public async Task Gs02_CodexPwshEnvelope_IsDenied(string script)
    {
        await SetupClaimedAgent();
        var payload = System.Text.Json.JsonSerializer.Serialize(new Dictionary<string, object>
        {
            ["session_id"] = TestSessionId,
            ["turn_id"] = "t1",
            ["tool_name"] = "shell_command",
            ["cwd"] = TestDir,
            ["tool_input"] = new Dictionary<string, object> { ["command"] = new[] { "pwsh", "-Command", script }, ["workdir"] = TestDir },
        });

        var result = await GuardWithStdinAsync(payload);

        result.AssertPreToolUseDenied();
    }

    // DYD-641: pwsh 7.6.9 ends a statement and a comment at a bare carriage return, and a here-string
    // at a bare carriage return or a typographic quote before @ (measured in /tmp/dyd641-evidence);
    // the text after each runs, so Claude's PowerShell tool, a Codex pwsh argv envelope and an
    // unknown shell refuse it, and the protected canary keeps its bytes.
    [Trait("Job", "trap: DYD-641 pwsh and bash read a bare carriage return differently")]
    [Theory]
    [InlineData("echo @'\nx\n’@; iex '$(echo x > $OUT)'\n'@ # '")]
    [InlineData("echo @'\nx\r'@; iex '$(echo x > $OUT)'\n'@ # '")]
    [InlineData("echo x\riex @'\n$(echo x > $OUT)\n'@")]
    [InlineData("echo hello\rrm dydo/index.md")]
    [InlineData("echo hello # note\rRemove-Item dydo/index.md")]
    [InlineData("# x\riex (\n@'\n$(echo x > dydo/index.md)\n'@\n)")]
    [InlineData("# x\riex `\n@'\n$(echo x > dydo/index.md)\n'@")]
    [InlineData("echo a > notes.md # x\"\rRemove-Item dydo/index.md\r# y\"")]
    public async Task Dyd641_StatementPwshRuns_IsDeniedWhereverPwshMayRunIt(string command)
    {
        await SetupClaimedAgent();
        var canary = Path.Combine(DydoDir, "index.md");
        var before = File.ReadAllBytes(canary);

        await AssertPwshToolsDenyAsync(command);
        var unknown = await UnknownShellGuardAsync(command);

        unknown.AssertExitCode(2);
        Assert.Equal(before, File.ReadAllBytes(canary));
    }

    // A line starting with @ reaches the CLI lane's argv parser as a response file, so the unknown
    // shell's reading of this one is proved in the analyzer's tests.
    [Trait("Job", "trap: DYD-641 pwsh and bash read a bare carriage return differently")]
    [Fact]
    public async Task Dyd641_PipeAfterABareCarriageReturn_IsDeniedWherePwshRunsIt()
    {
        await SetupClaimedAgent();
        var canary = Path.Combine(DydoDir, "index.md");
        var before = File.ReadAllBytes(canary);

        await AssertPwshToolsDenyAsync("@'\n$(echo x > dydo/index.md)\n'@\n# c\r| iex");

        Assert.Equal(before, File.ReadAllBytes(canary));
    }

    // bash 5.3.9 keeps a bare carriage return in a word, so the words after it are still the command's
    // arguments: it deletes, truncates, edits or chmods the canary (measured in /tmp/dyd641-evidence/fix2).
    // Claude's Bash tool and an unknown shell refuse it, as before DYD-641.
    [Trait("Job", "trap: DYD-641 pwsh and bash read a bare carriage return differently")]
    [Theory]
    [InlineData("rm \r dydo/index.md")]
    [InlineData("rm -- \r dydo/index.md")]
    [InlineData("tee \r dydo/index.md")]
    [InlineData("sed -i\r s/a/b/ dydo/index.md")]
    [InlineData("truncate -s 0 \r dydo/index.md")]
    [InlineData("chmod 000 \r dydo/index.md")]
    [InlineData("echo $(rm \r dydo/index.md)")]
    public async Task Dyd641_ArgumentsAfterABareCarriageReturn_AreDeniedWhereverBashMayRunThem(string command)
    {
        await SetupClaimedAgent();
        var canary = Path.Combine(DydoDir, "index.md");
        var before = File.ReadAllBytes(canary);

        var bash = await GuardWithStdinAsync(ShellJson("Bash", command));
        var unknown = await UnknownShellGuardAsync(command);

        bash.AssertPreToolUseDenied();
        unknown.AssertExitCode(2);
        Assert.Equal(before, File.ReadAllBytes(canary));
    }

    // A bash -c string is bash's whichever tool hands it on, so every lane refuses it.
    [Trait("Job", "trap: DYD-641 pwsh and bash read a bare carriage return differently")]
    [Fact]
    public async Task Dyd641_BashCommandStringWithABareCarriageReturn_IsDeniedInEveryLane()
    {
        await SetupClaimedAgent();
        const string command = "bash -c 'rm \r dydo/index.md'";

        foreach (var json in new[] { ShellJson("Bash", command), ShellJson("PowerShell", command), CodexPwshJson(command) })
            (await GuardWithStdinAsync(json)).AssertPreToolUseDenied();
        (await UnknownShellGuardAsync(command)).AssertExitCode(2);
    }

    // Each carriage return here is half of a CRLF, so the line holds none of the three characters:
    // the Windows-authored here-string write is allowed as before DYD-641 (pwsh 7.6.9 runs it harmlessly).
    // A line starting with @ reaches the CLI lane's argv parser as a response file (DYD-675).
    [Trait("Job", "trap: DYD-641 pwsh and bash read a bare carriage return differently")]
    [Theory]
    [InlineData("@'\r\nhello\r\n'@ | Set-Content notes.md\r\necho done")]
    [InlineData("@'\r\na\r\n'@ | Set-Content notes.md\r\n@'\r\nb\r\n'@ | Add-Content notes.md")]
    [InlineData("@'\nhello\n'@ | Set-Content notes.md\r\n")]
    [InlineData("cat @'\r\nhello\r\n'@ > notes.md\r\n")]
    public async Task Dyd641_CrlfAfterAHereStringWrite_IsAllowed(string command)
    {
        await SetupClaimedAgent();

        foreach (var json in new[] { ShellJson("Bash", command), ShellJson("PowerShell", command), CodexPwshJson(command) })
            (await GuardWithStdinAsync(json)).AssertPreToolUseAllowed();
    }

    [Trait("Job", "trap: DYD-641 pwsh and bash read a bare carriage return differently")]
    [Fact]
    public async Task Dyd641_CrlfAfterAHereStringWrite_IsAllowedInAnUnknownShell()
    {
        await SetupClaimedAgent();

        (await UnknownShellGuardAsync("cat @'\r\nhello\r\n'@ > notes.md\r\n")).AssertExitCode(0);
    }

    // Claude's PowerShell tool and a Codex pwsh argv envelope both refuse the command as a bypass.
    private async Task AssertPwshToolsDenyAsync(string command)
    {
        var powerShell = await GuardWithStdinAsync(ShellJson("PowerShell", command));
        var envelope = await GuardWithStdinAsync(CodexPwshJson(command));

        powerShell.AssertPreToolUseDenied();
        envelope.AssertPreToolUseDenied();
    }

    // A Codex shell call that runs the command through pwsh -Command.
    private string CodexPwshJson(string command) =>
        System.Text.Json.JsonSerializer.Serialize(new Dictionary<string, object>
        {
            ["session_id"] = TestSessionId,
            ["turn_id"] = "t1",
            ["tool_name"] = "shell_command",
            ["cwd"] = TestDir,
            ["tool_input"] = new Dictionary<string, object> { ["command"] = new[] { "pwsh", "-Command", command }, ["workdir"] = TestDir },
        });

    // The round-5 apostrophes pair only in PowerShell; bash's reading is required for every caller,
    // so Claude's Bash and PowerShell tools and an unknown shell all refuse them, as on master.
    [Trait("Job", "trap: DYD-406 GS-02 and DYD-422, literal data is no bypass; a guarded or dynamic write still is")]
    [Theory]
    [InlineData("echo don\\'t $(rm dydo/index.md) won\\'t")]
    [InlineData("echo don\\'t $(echo x > $OUT) won\\'t")]
    [InlineData("echo don\\'t $(rm dydo/index.md) won\\'t # it's")]
    [InlineData("# don't\necho don\\'t $(rm dydo/index.md) won\\'t")]
    [InlineData("cat > notes.md <<'EOF'\nplain words\nEOF\necho don\\'t $(rm dydo/index.md) won\\'t")]
    [InlineData("cat <<EOF\necho don\\'t $(rm dydo/index.md) won\\'t\nEOF")]
    public async Task Gs02_RoundFiveApostrophes_AreDeniedInEveryShellAndTheCanaryUnchanged(string command)
    {
        await SetupClaimedAgent();
        var canary = Path.Combine(DydoDir, "index.md");
        var before = File.ReadAllBytes(canary);

        var bash = await GuardWithStdinAsync(ShellJson("Bash", command));
        var powerShell = await GuardWithStdinAsync(ShellJson("PowerShell", command));
        var unknown = await UnknownShellGuardAsync(command);

        bash.AssertPreToolUseDenied();
        powerShell.AssertPreToolUseDenied();
        unknown.AssertExitCode(2);
        Assert.Equal(before, File.ReadAllBytes(canary));
    }

    // Where PowerShell may run the line: a typographic quote closes its string, a caret escapes
    // nothing, --% stops parsing, and a group word, an alias, an assignment or a call operator may
    // run the text, so it is code.
    [Trait("Job", "trap: DYD-406 GS-02 and DYD-422, literal data is no bypass; a guarded or dynamic write still is")]
    [Theory]
    [InlineData("echo 'a’ $(echo x > $OUT) ’b'")]
    [InlineData("echo “a '$(echo x > $OUT)' b”")]
    [InlineData("echo ^\"'$(echo x > $OUT)'^\"")]
    [InlineData("echo '$HOME' (iex 'rm dydo/index.md') > notes.md")]
    [InlineData("echo --% '$(echo x > $OUT)' > notes.md")]
    [InlineData("echo '$(echo x > $OUT)' -OutVariable a > notes.md")]
    [InlineData("Set-Alias echo Invoke-Expression; echo 'echo x > $OUT'")]
    [InlineData("New-Alias printf Invoke-Expression; printf 'echo x > $OUT'")]
    [InlineData("sal echo iex; echo 'echo x > $OUT'")]
    [InlineData("Set-Item alias:echo Invoke-Expression; echo 'echo x > $OUT'")]
    [InlineData("$x = 1; echo '$(echo x > $OUT)' > notes.md")]
    [InlineData("& echo '$(echo x > $OUT)' > notes.md")]
    public async Task Gs02_TextPowerShellMayRun_IsDenied(string command)
    {
        await SetupClaimedAgent();

        var powerShell = await GuardWithStdinAsync(ShellJson("PowerShell", command));
        var unknown = await UnknownShellGuardAsync(command);

        powerShell.AssertPreToolUseDenied();
        unknown.AssertExitCode(2);
    }

    // DYD-421 DECIDED: apostrophes around a substitution or a variable, on a command line or in an
    // expanding heredoc body, hide nothing; the protected canary keeps its bytes.
    [Trait("Job", "contract: DYD-421 DECIDED, apostrophes around an expansion hide nothing")]
    [Theory]
    [InlineData("echo don\\'t $(rm dydo/index.md) won\\'t")]
    [InlineData("cat > notes.md <<EOF\nDon't $(rm dydo/index.md) it's\nEOF")]
    [InlineData("cat > notes.md <<EOF\nDon't $(echo y > dydo/index.md) it's\nEOF")]
    [InlineData("cat > notes.md <<EOF\nthe user's file: $(cat dydo/index.md > /dev/null; rm dydo/index.md) isn't kept\nEOF")]
    [InlineData("cat > notes.md <<EOF\nDon't run $(date) or it's late\nEOF")]
    [InlineData("cat > notes.md <<EOF\nDon't touch $HOME, it's mine\nEOF")]
    public async Task Gs02_ExpansionBetweenApostrophes_IsDeniedAndTheCanaryUnchanged(string command)
    {
        await SetupClaimedAgent();
        var canary = Path.Combine(DydoDir, "index.md");
        var before = File.ReadAllBytes(canary);

        var result = await GuardWithStdinAsync(ShellJson("Bash", command));

        result.AssertPreToolUseDenied();
        Assert.Equal(before, File.ReadAllBytes(canary));
    }

    [Trait("Job", "trap: DYD-406 GS-02 and DYD-422, literal data is no bypass; a guarded or dynamic write still is")]
    [Theory]
    [InlineData("Bash", "cat > $OUT <<'EOF'\nplain words\nEOF")]
    [InlineData("Bash", "cat > notes.md <<EOF\n$(rm -rf build)\nEOF")]
    [InlineData("Bash", "cat > notes.md <<EOF\nprice: $PRICE\nEOF")]
    [InlineData("Bash", "cat > notes.md <<'EOF'\nplain words\nEOF\nrm $TARGET")]
    [InlineData("Bash", "bash <<'EOF'\necho x > $OUT\nEOF")]
    [InlineData("Bash", "bash -c 'echo x > $OUT'")]
    [InlineData("Bash", "bash -c 'cd build\necho x > $OUT'")]
    [InlineData("Bash", "echo hi # don't\necho x > $OUT\necho 'ok'")]
    [InlineData("PowerShell", "Set-Content -LiteralPath $p -Value @'\nplain words\n'@")]
    [InlineData("PowerShell", "@\"\n$(Remove-Item build -Recurse)\n\"@ | Set-Content notes.md")]
    public async Task Gs02_DynamicWrite_NextToLiteralData_IsStillDenied(string toolName, string command)
    {
        await SetupClaimedAgent();

        var result = await GuardWithStdinAsync(ShellJson(toolName, command));

        result.AssertPreToolUseDenied();
    }

    [Trait("Job", "trap: DYD-406 GS-02 and DYD-422, literal data is no bypass; a guarded or dynamic write still is")]
    [Theory]
    [InlineData("Bash", "cat > dydo/files-off-limits.md <<'EOF'\nplain words\nEOF")]
    [InlineData("Bash", "cat > notes.md <<'EOF'\nplain words\nEOF\nrm dydo/index.md")]
    [InlineData("PowerShell", "Set-Content -LiteralPath 'dydo/index.md' -Value @'\nplain words\n'@")]
    [InlineData("Bash", "cat > .env <<'EOF'\nplain words\nEOF")]
    public async Task Gs02_LiteralDataWrite_ToAGuardedPath_IsStillDenied(string toolName, string command)
    {
        await SetupClaimedAgent();

        var result = await GuardWithStdinAsync(ShellJson(toolName, command));

        result.AssertPreToolUseDenied();
    }

    // GS-03 (review round 6): a launch a runner makes, or the command string ssh or watch runs, keeps
    // master's poll-loop nudge. Unquoted and uninventoried quoted arguments still count;
    // only the finite data operands and proven inert bodies receive relief.
    [Trait("Job", "trap: DYD-406 GS-03, the poll-loop nudge fires on a launch, not on quoted or literal text")]
    [Theory]
    [InlineData("Bash", "stdbuf -oL tail -f app.log")]
    [InlineData("Bash", "stdbuf -o L tail -f app.log")]
    [InlineData("Bash", "stdbuf --output=L tail -f app.log")]
    [InlineData("Bash", "ionice -c 3 tail -f app.log")]
    [InlineData("Bash", "setsid tail -f app.log")]
    [InlineData("Bash", "unbuffer tail -f app.log")]
    [InlineData("Bash", "caffeinate -t 60 tail -f app.log")]
    [InlineData("Bash", "wsl -d Ubuntu -- tail -f app.log")]
    [InlineData("PowerShell", "wsl bash -c 'tail -f app.log'")]
    [InlineData("Bash", "ssh host 'tail -f app.log'")]
    [InlineData("Bash", "watch -n 1 'tail -f app.log'")]
    public async Task Gs03_RunnerAndRemoteLaunch_KeepsThePollLoopNudge(string toolName, string command)
    {
        await SetupClaimedAgent();

        var result = await GuardWithStdinAsync(ShellJson(toolName, command));

        result.AssertPreToolUseDenied();
    }

    [Trait("Job", "trap: DYD-406 GS-03, the poll-loop nudge fires on a launch, not on quoted or literal text")]
    [Theory]
    [InlineData("echo stdbuf -oL tail -f app.log")]
    [InlineData("grep 'ssh host tail -f' notes.md")]
    [InlineData("docker exec web tail -f app.log")]
    [InlineData("kubectl exec pod -- tail -f app.log")]
    [InlineData("flock /tmp/lock tail -f app.log")]
    [InlineData("script -q -c 'tail -f app.log' /dev/null")]
    [InlineData("su -c 'tail -f app.log'")]
    [InlineData("tmux new -d 'tail -f app.log'")]
    [InlineData("unknown-wrapper 'tail -f app.log'")]
    [InlineData("rg tail --follow docs")]
    public async Task Gs03_UninventoriedArguments_KeepTheWarning(string command)
    {
        await SetupClaimedAgent();

        var result = await GuardWithStdinAsync(ShellJson("Bash", command));

        result.AssertPreToolUseDenied();
    }

    [Trait("Job", "trap: DYD-406 GS-03, the poll-loop nudge fires on a launch, not on quoted or literal text")]
    [Theory]
    [InlineData("Bash", "cat <<'EOF' | bash\ntail -f app.log\nEOF")]
    [InlineData("Bash", "cat <<'EOF' | unknown-wrapper\ntail -f app.log\nEOF")]
    [InlineData("PowerShell", "Write-Output @'\ntail -f app.log\n'@ | Invoke-Expression")]
    [InlineData("PowerShell", "Write-Output @'\ntail -f app.log\n'@ | pwsh -Command -")]
    [InlineData("Bash", "echo @'\nx'; tail -f app.log; echo '\n'@")]
    [InlineData("Bash", "bash <<'OUT' | bash\ncat <<'IN'\ntail -f app.log\nIN\nOUT")]
    [InlineData("Bash", "bash <<'OUT' | unknown-wrapper\ncat <<'IN'\ntail -f app.log\nIN\nOUT")]
    [InlineData("Bash", "bash -c \"cat <<'IN'\ntail -f app.log\nIN\" | bash")]
    [InlineData("Bash", "bash -c \"cat <<'IN'\ntail -f app.log\nIN\" | unknown-wrapper")]
    public async Task Gs03_ContextBody_HookDenialAndRetry(string toolName, string command)
    {
        await SetupClaimedAgent();

        (await GuardWithStdinAsync(ShellJson(toolName, command))).AssertPreToolUseDenied();
        (await GuardWithStdinAsync(ShellJson(toolName, command))).AssertPreToolUseAllowed();
    }

    [Trait("Job", "trap: DYD-406 GS-03, the poll-loop nudge fires on a launch, not on quoted or literal text")]
    [Theory]
    [InlineData("Bash", "cat <<'EOF'\ntail -f app.log\nEOF")]
    [InlineData("PowerShell", "Write-Output @'\ntail -f app.log\n'@")]
    [InlineData("Bash", "echo @'\n$(tail -f app.log)\n'@")]
    [InlineData("Bash", "bash <<'OUT'\ncat <<'IN'\ntail -f app.log\nIN\nOUT")]
    [InlineData("Bash", "bash -c \"cat <<'IN'\ntail -f app.log\nIN\"")]
    public async Task Gs03_ContextBody_HookLiteralControl(string toolName, string command)
    {
        await SetupClaimedAgent();
        var result = await GuardWithStdinAsync(ShellJson(toolName, command));
        result.AssertPreToolUseAllowed();
    }

    // A here-string terminator with text glued to it is no terminator, so the poll loop after it
    // runs: it warns once and the identical retry passes, as on master.
    [Trait("Job", "trap: DYD-406 GS-02/GS-03 a glued here-string terminator ends no body")]
    [Fact]
    public async Task Gs02_HereStringBoundary_GluedPollLoopWarnsOnceThenPasses()
    {
        await SetupClaimedAgent();
        const string command = "Write-Output @'\ntext\n'@tail -f app.log";

        (await GuardWithStdinAsync(ShellJson("PowerShell", command))).AssertPreToolUseDenied();
        (await GuardWithStdinAsync(ShellJson("PowerShell", command))).AssertPreToolUseAllowed();
    }

    [Trait("Job", "trap: DYD-406 GS-03, the poll-loop nudge fires on a launch, not on quoted or literal text")]
    [Theory]
    [InlineData("H01-posix", "Bash", "echo @\"\n\" $(cat <<'IN'\ntail -f app.log\nIN\n) | bash; echo \"\n\"@", true)]
    [InlineData("H03-data", "Bash", "echo @\"\n$(cat <<'IN'\ntail -f app.log\nIN\n)\n\"@", false)]
    [InlineData("H04-child-quotes", "Bash", "echo @\"\n$(cat <<'IN'\nIt's \"ready\"; tail -f app.log\nIN\n)\n\"@", false)]
    [InlineData("H05-code", "Bash", "echo @\"\n$(cat <<'IN'\ntail -f app.log\nIN\n)\n\"@ | bash", true)]
    [InlineData("H06-unknown-consumer", "Bash", "echo @\"\n$(cat <<'IN'\ntail -f app.log\nIN\n)\n\"@ | unknown-wrapper", true)]
    [InlineData("H07-malformed", "PowerShell", "Write-Output @\"\n$(Write-Output @'\ntail -f app.log\n'@\n)\n\"@suffix", true)]
    [InlineData("H08-data", "PowerShell", "Write-Output @\"\n$(Write-Output @'\ntail -f app.log\n'@\n)\n\"@", false)]
    [InlineData("H09-child-quotes", "PowerShell", "Write-Output @\"\n$(Write-Output @'\nIt's \"ready\"; tail -f app.log\n'@\n)\n\"@", false)]
    [InlineData("H10-code", "PowerShell", "Write-Output @\"\n$(Write-Output @'\ntail -f app.log\n'@\n)\n\"@ | Invoke-Expression", true)]
    [InlineData("H11-unknown-consumer", "PowerShell", "Write-Output @\"\n$(Write-Output @'\ntail -f app.log\n'@\n)\n\"@ | unknown-wrapper", true)]
    [InlineData("H12-dash-suffix", "PowerShell", "Write-Output @\"\n$(Write-Output @'\ntail -f app.log\n'@\n)\n\"@-suffix", true)]
    [InlineData("H13-space", "PowerShell", "Write-Output @\"\n$(Write-Output @'\ntail -f app.log\n'@\n)\n\"@ ", false)]
    [InlineData("H13-tab", "PowerShell", "Write-Output @\"\n$(Write-Output @'\ntail -f app.log\n'@\n)\n\"@\t", false)]
    [InlineData("H14-later-launch", "PowerShell", "Write-Output @\"\n$(Write-Output @'\ntail -f app.log\n'@\n)\n\"@; tail -f real.log", true)]
    [InlineData("H15-backtick-data", "Bash", "echo @\"\n`cat <<'IN'\ntail -f app.log\nIN\n`\n\"@", false)]
    [InlineData("H15-backtick-code", "Bash", "echo @\"\n`cat <<'IN'\ntail -f app.log\nIN\n`\n\"@ | bash", true)]
    [InlineData("H16-identical-children", "Bash", "echo @\"\n$(cat <<'IN'\ntail -f app.log\nIN\n)\n$(cat <<'IN'\ntail -f app.log\nIN\n)\n\"@", false)]
    [InlineData("H17-sibling-quotes-unknown", "Bash", "echo @\"\n$(cat <<'IN'\nIt's \"ready\"; tail -f app.log\nIN\n)\n$(cat <<'IN'\ntail -f app.log\nIN\n)\n\"@ | unknown-wrapper", true)]
    [InlineData("H18-stdin-data", "Bash", "bash <<'OUT'\necho @\"\n$(cat <<'IN'\ntail -f app.log\nIN\n)\n\"@\nOUT", false)]
    [InlineData("H18-stdin-code", "Bash", "bash <<'OUT' | bash\necho @\"\n$(cat <<'IN'\ntail -f app.log\nIN\n)\n\"@\nOUT", true)]
    [InlineData("H18-stdin-unknown", "Bash", "bash <<'OUT' | unknown-wrapper\necho @\"\n$(cat <<'IN'\ntail -f app.log\nIN\n)\n\"@\nOUT", true)]
    [InlineData("H19-script-data", "Bash", "bash -c \"echo @\\\"\n\\$(cat <<'IN'\ntail -f app.log\nIN\n)\n\\\"@\"", false)]
    [InlineData("H19-script-code", "Bash", "bash -c \"echo @\\\"\n\\$(cat <<'IN'\ntail -f app.log\nIN\n)\n\\\"@\" | bash", true)]
    [InlineData("H20-script-ps-data", "Bash", "pwsh -Command \"Write-Output @\\\"\n\\$(Write-Output @'\ntail -f app.log\n'@\n)\n\\\"@\"", false)]
    [InlineData("H20-script-ps-unknown", "Bash", "pwsh -Command \"Write-Output @\\\"\n\\$(Write-Output @'\ntail -f app.log\n'@\n)\n\\\"@\" | unknown-wrapper", true)]
    [InlineData("H21-data-then-unsafe", "Bash", "echo @\"\n$(cat <<'IN'\ntail -f app.log\nIN\n)\n\"@\necho @\"\n\" $(cat <<'IN'\ntail -f app.log\nIN\n) | bash; echo \"\n\"@", true)]
    [InlineData("H21-unsafe-then-data", "Bash", "echo @\"\n\" $(cat <<'IN'\ntail -f app.log\nIN\n) | bash; echo \"\n\"@\necho @\"\n$(cat <<'IN'\ntail -f app.log\nIN\n)\n\"@", true)]
    [InlineData("H22-child-launch", "Bash", "echo @\"\n$(cat <<'IN'\ntail -f app.log\nIN\ntail -f real.log\n)\n\"@", true)]
    [InlineData("H23-child-launch-code", "Bash", "echo @\"\n$(cat <<'IN'\ntail -f app.log\nIN\ntail -f real.log\n)\n\"@ | bash", true)]
    [InlineData("H24-crlf-posix", "Bash", "echo @\"\r\n$(cat <<'IN'\r\ntail -f app.log\r\nIN\r\n)\r\n\"@", false)]
    [InlineData("H24-crlf-powershell", "PowerShell", "Write-Output @\"\r\n$(Write-Output @'\r\ntail -f app.log\r\n'@\r\n)\r\n\"@", false)]
    [InlineData("H25-capture-stdin-data", "Bash", "echo @\"\n$(bash <<'MID'\ncat <<'IN'\ntail -f app.log\nIN\nMID\n)\n\"@", false)]
    [InlineData("H25-capture-stdin-code", "Bash", "echo @\"\n$(bash <<'MID'\ncat <<'IN'\ntail -f app.log\nIN\nMID\n)\n\"@ | bash", true)]
    [InlineData("H26-hereword-data", "Bash", "cat <<< \"$(cat <<'IN'\nIt's \"ready\"; tail -f app.log\nIN\n)\"", false)]
    [InlineData("H26-hereword-code", "Bash", "cat <<< \"$(cat <<'IN'\nIt's \"ready\"; tail -f app.log\nIN\n)\" | bash", true)]
    [InlineData("H27-two-captures-data", "Bash", "cat <<OUTER\n$(cat <<MIDDLE\n$(cat <<'INNER'\ntail -f app.log\nINNER\n)\nMIDDLE\n)\nOUTER", false)]
    [InlineData("H27-two-captures-code", "Bash", "cat <<OUTER | bash\n$(cat <<MIDDLE\n$(cat <<'INNER'\ntail -f app.log\nINNER\n)\nMIDDLE\n)\nOUTER", true)]
    public async Task Gs03_HostEdge_HookFirstAndRetry(string id, string toolName, string command, bool deny)
    {
        await SetupClaimedAgent();
        var canary = Path.Combine(DydoDir, "index.md");
        var before = File.ReadAllBytes(canary);
        var payload = ShellJson(toolName, command);
        var first = await GuardWithStdinAsync(payload, "--host", "claude");
        var retry = await GuardWithStdinAsync(payload, "--host", "claude");
        Assert.Equal(before, File.ReadAllBytes(canary));
        retry.AssertPreToolUseAllowed();
        Assert.True(deny == !first.IsPreToolUseAllowed, id);
        if (deny) first.AssertPreToolUseDenied();
    }

    // GS-01: a PowerShell variable assigned from a literal Get-Content is a read.
    [Trait("Job", "trap: DYD-406 GS-01, a PowerShell assignment from Get-Content is a read")]
    [Theory]
    [InlineData(GuardRetroCommands.Gs01SlicedRead)]
    [InlineData(GuardRetroCommands.Gs01SelectStringRead)]
    [InlineData("$p = Get-Content dydo/index.md; $p[0..5]")]
    public async Task Gs01_LiteralReadAssignment_IsAllowed(string command)
    {
        await SetupClaimedAgent();

        var result = await GuardWithStdinAsync(ShellJson("PowerShell", command));

        result.AssertPreToolUseAllowed();
    }

    [Trait("Job", "trap: DYD-406 GS-01, a PowerShell assignment from Get-Content is a read")]
    [Theory]
    [InlineData("$p = Get-Content notes.md; Set-Content notes.md $p")]
    [InlineData("$x = Remove-Item notes.md")]
    [InlineData("$p=Get-Content notes.md | Set-Content dydo/index.md")]
    [InlineData("$CMD dydo/files-off-limits.md")]
    [InlineData("$p = Get-Content .env")]
    public async Task Gs01_AssignmentNextToAMutationOrAGuardedRead_IsStillDenied(string command)
    {
        await SetupClaimedAgent();

        var result = await GuardWithStdinAsync(ShellJson("PowerShell", command));

        result.AssertPreToolUseDenied();
    }

    [Trait("Job", "trap: DYD-406 case arms keep their file operations and nudges")]
    [Fact]
    public async Task CaseArm_MatchingExecutableArm_IsDeniedByHook()
    {
        await SetupClaimedAgent();
        const string command = "cat <<EOF\n$(case x in x) tail -f app.log;; esac)\nEOF";

        var result = await GuardWithStdinAsync(ShellJson("Bash", command));

        // The restored conservative file analysis refuses before command nudges run.
        Assert.Contains("Command contains bypass patterns", result.AssertPreToolUseDenied());
    }

    [Trait("Job", "trap: DYD-406 case arms keep their file operations and nudges")]
    [Theory]
    [InlineData("x", "rm dydo/files-off-limits.md")]
    [InlineData("x", "echo changed > dydo/index.md")]
    [InlineData("x|y", "rm dydo/files-off-limits.md")]
    [InlineData("x|y", "echo changed > dydo/index.md")]
    public async Task CaseArm_ProtectedMutation_IsDeniedOnFirstAndRetry(string pattern, string mutation)
    {
        await SetupClaimedAgent();
        var paths = new[] { Path.Combine(DydoDir, "files-off-limits.md"), Path.Combine(DydoDir, "index.md") };
        var before = paths.Select(File.ReadAllBytes).ToArray();
        var command = "cat <<EOF\n$(case x in " + pattern + ") " + mutation + ";; esac)\nEOF";

        var first = await GuardWithStdinAsync(ShellJson("Bash", command));
        var retry = await GuardWithStdinAsync(ShellJson("Bash", command));

        first.AssertPreToolUseDenied();
        retry.AssertPreToolUseDenied();
        for (var i = 0; i < paths.Length; i++)
            Assert.Equal(before[i], File.ReadAllBytes(paths[i]));
    }

    // GS-05: a Python program on stdin is refused before it runs, so the protected canary keeps its bytes.
    [Trait("Job", "trap: DYD-406 GS-05, a Python program on stdin hides its file operations")]
    [Theory]
    [InlineData("Bash", "python - <<'EOF'\nopen('dydo/files-off-limits.md','w').write('changed')\nEOF")]
    [InlineData("Bash", "printf \"open('dydo/files-off-limits.md','w').write('changed')\" | python3")]
    [InlineData("PowerShell", "@'\nopen('dydo/files-off-limits.md','w').write('changed')\n'@ | py -")]
    [InlineData("Bash", GuardRetroCommands.Gs05ProtectedRewrite)]
    [InlineData("Bash", "python3 < tools/rewrite.py")]
    public async Task Gs05_PythonOnStdin_WritingTheProtectedCanary_IsDeniedAndTheCanaryUnchanged(string toolName, string command)
    {
        await SetupClaimedAgent();
        var canary = Path.Combine(DydoDir, "files-off-limits.md");
        var before = File.ReadAllBytes(canary);

        var result = await GuardWithStdinAsync(ShellJson(toolName, command));

        var reason = result.AssertPreToolUseDenied();
        Assert.Contains("Python program on stdin", reason);
        Assert.Equal(before, File.ReadAllBytes(canary));
    }

    // A script file runs and its own file operations are not followed: the documented limit.
    [Trait("Job", "trap: DYD-406 GS-05, a Python program on stdin hides its file operations")]
    [Fact]
    public async Task Gs05_PythonScriptFile_IsAllowed()
    {
        await SetupClaimedAgent();

        var result = await GuardWithStdinAsync(ShellJson("Bash", "python tools/rewrite.py"));

        result.AssertPreToolUseAllowed();
    }

    // Nothing feeds stdin, so no program is read from it: a lookup, xargs operands, a prompt.
    [Trait("Job", "trap: DYD-406 GS-05, a Python program on stdin hides its file operations")]
    [Theory]
    [InlineData("command -v python3")]
    [InlineData("command -V python3")]
    [InlineData("command -v python3 || command -v python")]
    [InlineData("ls *.py | xargs python3")]
    [InlineData("python")]
    public async Task Gs05_PythonWithNothingFedToIt_IsAllowed(string command)
    {
        await SetupClaimedAgent();

        var result = await GuardWithStdinAsync(ShellJson("Bash", command));

        result.AssertPreToolUseAllowed();
    }

    // GS-02: every word eval, ssh or watch joins into the code it runs is code, and so is a line a
    // shell reads from a pipe, so a dynamic write in any of them is still denied.
    [Trait("Job", "trap: DYD-406 GS-02 and DYD-422, literal data is no bypass; a guarded or dynamic write still is")]
    [Theory]
    [InlineData("eval 'echo x' '> $OUT'")]
    [InlineData("eval echo 'x > $OUT'")]
    [InlineData("ssh host 'echo x' '> $OUT'")]
    [InlineData("watch -n 1 'echo x > $OUT'")]
    [InlineData("printf 'echo x > $OUT' | bash")]
    public async Task Gs02_DynamicWrite_InAnyWordACommandRuns_IsDenied(string command)
    {
        await SetupClaimedAgent();

        var result = await GuardWithStdinAsync(ShellJson("Bash", command));

        result.AssertPreToolUseDenied();
    }

    // GS-05: a < or | in a comment feeds nothing.
    [Trait("Job", "trap: DYD-406 GS-05, a Python program on stdin hides its file operations")]
    [Theory]
    [InlineData("python # <stdin>")]
    [InlineData("python # input < program.py")]
    [InlineData("python # input | python")]
    public async Task Gs05_PythonWithAFeedOnlyInAComment_IsAllowed(string command)
    {
        await SetupClaimedAgent();

        var result = await GuardWithStdinAsync(ShellJson("Bash", command));

        result.AssertPreToolUseAllowed();
    }

    // A real feed is still refused after a comment, into a group, and inside a substitution.
    [Trait("Job", "trap: DYD-406 GS-05, a Python program on stdin hides its file operations")]
    [Theory]
    [InlineData("printf \"open('dydo/files-off-limits.md','w').write('changed')\" | python3 # feeds it")]
    [InlineData("echo \"$(printf \"open('dydo/files-off-limits.md','w').write('changed')\" | python3)\"")]
    [InlineData("printf \"open('dydo/files-off-limits.md','w').write('changed')\" | (python3)")]
    [InlineData("printf \"open('dydo/files-off-limits.md','w').write('changed')\" | { python3; }")]
    public async Task Gs05_PythonFedThroughACommentGroupOrSubstitution_IsDeniedAndTheCanaryUnchanged(string command)
    {
        await SetupClaimedAgent();
        var canary = Path.Combine(DydoDir, "files-off-limits.md");
        var before = File.ReadAllBytes(canary);

        var result = await GuardWithStdinAsync(ShellJson("Bash", command));

        var reason = result.AssertPreToolUseDenied();
        Assert.Contains("Python program on stdin", reason);
        Assert.Equal(before, File.ReadAllBytes(canary));
    }

    // GS-02: a runner named only in a comment or an argument runs no body, and the blank or
    // comment-only lines after a heredoc separate no commands.
    [Trait("Job", "trap: DYD-406 GS-02 and DYD-422, literal data is no bypass; a guarded or dynamic write still is")]
    [Theory]
    [InlineData("Bash", "cat > helper.sh <<'EOF' # bash script\necho hello\nEOF")]
    [InlineData("Bash", "grep -c bash <<'EOF'\nline one\nline two\nEOF")]
    [InlineData("Bash", "cat > /tmp/python <<'EOF'\n$HOME\nEOF")]
    [InlineData("PowerShell", "Set-Content -LiteralPath notes.md -Value @'\n$HOME\n'@ # bash example")]
    [InlineData("Bash", "cat > notes.md <<'EOF'\n$HOME\nEOF\n\n")]
    [InlineData("Bash", "cat > notes.md <<'EOF'\nplain\nEOF\n\n# written\n")]
    public async Task Gs02_LiteralBody_WithARunnerInACommentOrTrailingBlankLines_IsAllowed(string toolName, string command)
    {
        await SetupClaimedAgent();

        var result = await GuardWithStdinAsync(ShellJson(toolName, command));

        result.AssertPreToolUseAllowed();
    }

    [Trait("Job", "trap: DYD-406 GS-02 and DYD-422, literal data is no bypass; a guarded or dynamic write still is")]
    [Theory]
    [InlineData("Bash", "bash <<'EOF' # bash script\necho x > $OUT\nEOF")]
    [InlineData("Bash", "cat <<'EOF' | bash # bash script\necho x > $OUT\nEOF")]
    [InlineData("PowerShell", "@'\necho x > $OUT\n'@ | pwsh -Command - # runs it")]
    [InlineData("Bash", "cat > notes.md <<'EOF'\nplain\nEOF\n\nrm secret.txt")]
    public async Task Gs02_BodyARunnerIsFed_OrACommandAfterABlankLine_IsStillDenied(string toolName, string command)
    {
        await SetupClaimedAgent();

        var result = await GuardWithStdinAsync(ShellJson(toolName, command));

        result.AssertPreToolUseDenied();
    }

    // Round 4: a body is data only when data commands alone read it; a quoted or pathed runner, a
    // stdin source, a group piped on, a later line of code and a comment's apostrophe hide nothing.
    [Trait("Job", "trap: DYD-406 GS-02 and DYD-422, literal data is no bypass; a guarded or dynamic write still is")]
    [Theory]
    [InlineData("Bash", "cat > notes.md <<'EOF' # don't expand\nplain\nEOF\nrm dydo/index.md")]
    [InlineData("Bash", "\"C:/Program Files/Git/bin/bash.exe\" <<'EOF'\necho y > dydo/index.md\nEOF")]
    [InlineData("Bash", "\"bash\" <<'EOF'\necho y > dydo/index.md\nEOF")]
    [InlineData("Bash", ". /dev/stdin <<'EOF'\necho y > dydo/index.md\nEOF")]
    [InlineData("Bash", "fish <<'EOF'\necho y > dydo/index.md\nEOF")]
    [InlineData("Bash", "cat <<'EOF' | \"C:/Program Files/Git/bin/bash.exe\"\necho y > dydo/index.md\nEOF")]
    [InlineData("Bash", "cat <<'EOF' |\necho y > dydo/index.md\nEOF\nbash")]
    [InlineData("Bash", "{\ncat <<'EOF'\necho y > dydo/index.md\nEOF\n} | bash")]
    [InlineData("Bash", "cat <<'EOF' > >(bash)\necho y > dydo/index.md\nEOF")]
    [InlineData("Bash", "echo hi # don't\nrm 'dydo/index.md'; echo x > notes.md")]
    [InlineData("PowerShell", "@'\necho y > dydo/index.md\n'@ | & \"C:\\Program Files\\Git\\bin\\bash.exe\"")]
    [InlineData("PowerShell", "@'\necho y > dydo/index.md\n'@ | wsl bash")]
    [InlineData("PowerShell", "@'\necho y > dydo/index.md\n'@\n| iex")]
    [InlineData("PowerShell", "@'\nplain\n'@ | Set-Content notes.md # don't\nRemove-Item 'dydo/index.md'")]
    public async Task Gs02_ProtectedChangeABodyOrALaterLineHolds_IsDenied(string toolName, string command)
    {
        await SetupClaimedAgent();
        var canary = Path.Combine(DydoDir, "index.md");
        var before = File.ReadAllBytes(canary);

        var result = await GuardWithStdinAsync(ShellJson(toolName, command));

        result.AssertPreToolUseDenied();
        Assert.Equal(before, File.ReadAllBytes(canary));
    }

    [Trait("Job", "trap: DYD-406 GS-02 and DYD-422, literal data is no bypass; a guarded or dynamic write still is")]
    [Theory]
    [InlineData("Bash", "cat > notes.md <<'END-OF-FILE'\nplain\nEND-OF-FILE")]
    [InlineData("Bash", "cat > notes.md <<\"END_OF_FILE\"\nplain\nEND_OF_FILE")]
    [InlineData("Bash", "cat > notes.md <<-'END-OF-FILE'\n\tplain\n\tEND-OF-FILE")]
    [InlineData("Bash", "cat > notes.md <<'EOF' # don't expand\nplain $HOME\nEOF")]
    [InlineData("PowerShell", "@'\nIt's ready.\n'@ | Set-Content notes.md")]
    public async Task Gs02_LiteralBodyOnlyDataCommandsRead_IsAllowed(string toolName, string command)
    {
        await SetupClaimedAgent();

        var result = await GuardWithStdinAsync(ShellJson(toolName, command));

        result.AssertPreToolUseAllowed();
    }

    [Trait("Job", "trap: DYD-406 GS-05, a Python program on stdin hides its file operations")]
    [Theory]
    [InlineData("echo \"$(python3 tools/rewrite.py)\"")]
    [InlineData("printf x | (python3 tools/rewrite.py)")]
    public async Task Gs05_PythonScriptFileInAGroupOrSubstitution_IsAllowed(string command)
    {
        await SetupClaimedAgent();

        var result = await GuardWithStdinAsync(ShellJson("Bash", command));

        result.AssertPreToolUseAllowed();
    }

    // ================================================================
    // DYD-640: an fd-numbered redirection writes its target, checked like plain >
    // ================================================================

    [Trait("Job", "trap: DYD-640 every redirection the shell reads is a write; fd duplication and sinks are not")]
    [Theory]
    [InlineData("printf x > dydo/index.md")]
    [InlineData("printf x 1> dydo/index.md")]
    [InlineData("printf x 2> dydo/index.md")]
    [InlineData("printf x 3> dydo/index.md")]
    [InlineData("printf x 1>> dydo/index.md")]
    [InlineData("printf x 1>| dydo/index.md")]
    [InlineData("printf x >| dydo/index.md")]
    [InlineData("printf x &> dydo/index.md")]
    [InlineData("printf x &>> dydo/index.md")]
    [InlineData("printf x >&dydo/index.md")]
    [InlineData("printf word2>dydo/index.md")]
    [InlineData("printf x 1> .env")]
    [InlineData("printf x 2>> .env")]
    public async Task Dyd640_OutputRedirectionToAGuardedPath_IsDenied(string command)
    {
        await SetupClaimedAgent();

        foreach (var toolName in new[] { "Bash", "PowerShell" })
        {
            var result = await GuardWithStdinAsync(ShellJson(toolName, command));

            result.AssertPreToolUseDenied();
        }
    }

    // fd duplication or close, and an fd-numbered redirection into a sink, beside a $ word stay allowed.
    [Trait("Job", "trap: DYD-640 every redirection the shell reads is a write; fd duplication and sinks are not")]
    [Theory]
    [InlineData("Bash", "echo \"$HOME\" 2>/dev/null")]
    [InlineData("Bash", "ls $(pwd) 2>/dev/null")]
    [InlineData("Bash", "echo $HOME 1>/dev/null")]
    [InlineData("Bash", "echo $HOME 2>/dev/stderr")]
    [InlineData("Bash", "echo $HOME 1>/dev/stdout")]
    [InlineData("Bash", "echo $HOME 3>/dev/fd/1")]
    [InlineData("Bash", "echo $HOME 2>&1")]
    [InlineData("Bash", "echo $HOME 1>&2")]
    [InlineData("Bash", "echo $HOME >&2")]
    [InlineData("Bash", "echo $HOME 2>&-")]
    [InlineData("PowerShell", "Get-ChildItem $HOME 2>$null")]
    [InlineData("PowerShell", "Invoke-Command { Get-ChildItem $HOME 2>$null}")]
    public async Task Dyd640_FdDuplicationOrSinkBesideABypass_IsAllowed(string toolName, string command)
    {
        await SetupClaimedAgent();

        var result = await GuardWithStdinAsync(ShellJson(toolName, command));

        result.AssertPreToolUseAllowed();
    }

    // DYD-640 review B1, B3 and nit 2, round 2 B1: a duplication or sink closed by a substitution or
    // subshell, text after it included, continued onto the next line or into /dev/tty stays allowed.
    [Trait("Job", "trap: DYD-640 every redirection the shell reads is a write; fd duplication and sinks are not")]
    [Theory]
    [InlineData("out=$(git status 2>&1)")]
    [InlineData("out=$(git status 2>&1) || true")]
    [InlineData("echo \"$(make 2>&1)\"")]
    [InlineData("x=`ls 2>&1`")]
    [InlineData("echo $(echo hi >&2)")]
    [InlineData("(echo $HOME >&2)")]
    [InlineData("x=$(git rev-parse HEAD 2>/dev/null)")]
    [InlineData("x=\"$(git rev-parse HEAD 2>/dev/null)\"")]
    [InlineData("v=$(command -v jq 2>/dev/null) || true")]
    [InlineData("if [ -n \"$(git status --porcelain 2>/dev/null)\" ]; then echo dirty; fi")]
    [InlineData("echo \"$HOME\" 2>\\\n/dev/null")]
    [InlineData("echo $HOME 2>/dev/tty")]
    [InlineData("tag=\"$(git describe --always 2>/dev/null)-dirty\"")]
    [InlineData("echo \"$(git status 2>&1):done\"")]
    [InlineData("x=$(git status 2>/dev/null)suffix")]
    [InlineData("x=`git status 2>&1`suffix")]
    [InlineData("x=$(echo $(git status 2>&1)x )")]
    public async Task Dyd640_SinkInsideASubstitutionOrContinued_IsAllowed(string command)
    {
        await SetupClaimedAgent();

        foreach (var toolName in new[] { "Bash", "PowerShell" })
        {
            var result = await GuardWithStdinAsync(ShellJson(toolName, command));

            result.AssertPreToolUseAllowed();
        }
    }

    // A guarded target inside a substitution or after a continuation is still refused.
    [Trait("Job", "trap: DYD-640 every redirection the shell reads is a write; fd duplication and sinks are not")]
    [Theory]
    [InlineData("x=$(printf y 2>dydo/index.md)")]
    [InlineData("x=\"$(printf y 1>dydo/index.md)\"")]
    [InlineData("x=`printf y 2>>dydo/index.md`")]
    [InlineData("{ printf y >&dydo/index.md; }")]
    [InlineData("printf y 2>\\\ndydo/index.md")]
    [InlineData("tag=\"$(git describe --always 2>dydo/index.md)-dirty\"")]
    [InlineData("echo \"$(git status 2>dydo/index.md):done\"")]
    [InlineData("x=$(git status 2>dydo/index.md)suffix")]
    [InlineData("x=`git status 2>dydo/index.md`suffix")]
    [InlineData("x=$( $(git status 2>dydo/index.md)x )")]
    [InlineData("x=$(git status 2>.env)suffix")]
    public async Task Dyd640_GuardedTargetInsideASubstitutionOrContinued_IsDenied(string command)
    {
        await SetupClaimedAgent();

        foreach (var toolName in new[] { "Bash", "PowerShell" })
        {
            var result = await GuardWithStdinAsync(ShellJson(toolName, command));

            result.AssertPreToolUseDenied();
        }
    }

    // DYD-640 review B2: $null is a sink only as PowerShell's own unquoted variable.
    [Trait("Job", "trap: DYD-640 every redirection the shell reads is a write; fd duplication and sinks are not")]
    [Theory]
    [InlineData("Bash", "null=dydo/index.md; printf x 1>>$null")]
    [InlineData("Bash", "echo $HOME 2>$null")]
    [InlineData("PowerShell", "Get-ChildItem $HOME 2>>'$null'")]
    [InlineData("PowerShell", "Get-ChildItem $HOME 2>\"$null\"")]
    public async Task Dyd640_NullOutsidePowerShellOrQuoted_IsDenied(string toolName, string command)
    {
        await SetupClaimedAgent();

        var result = await GuardWithStdinAsync(ShellJson(toolName, command));

        result.AssertPreToolUseDenied();
    }

    // DYD-640 round 2 B2 and N1: >& onto a variable or a quoted ) writes the file the word names, and bash
    // ends no word at }, so each is a write beside a $ word.
    [Trait("Job", "trap: DYD-640 every redirection the shell reads is a write; fd duplication and sinks are not")]
    [Theory]
    [InlineData("Bash", "fd=dydo/index.md; printf x >&\"$fd\"")]
    [InlineData("PowerShell", "fd=dydo/index.md; printf x >&\"$fd\"")]
    [InlineData("Bash", "fd=.env; printf x >&$fd")]
    [InlineData("PowerShell", "fd=.env; printf x >&$fd")]
    [InlineData("Bash", "fd=dydo/index.md; printf x >&$fd")]
    [InlineData("PowerShell", "fd=dydo/index.md; printf x >&$fd")]
    [InlineData("Bash", "fd=.env; printf x >&\"$fd\"")]
    [InlineData("PowerShell", "fd=.env; printf x >&\"$fd\"")]
    [InlineData("Bash", "x=$( $(git status 2>&1)x )")]
    [InlineData("Bash", "echo $HOME >&\"1)\"")]
    [InlineData("PowerShell", "echo $HOME >&\"1)\"")]
    [InlineData("Bash", "echo $HOME 2>/dev/null}")]
    public async Task Dyd640_WordTheShellWritesBesideABypass_IsDenied(string toolName, string command)
    {
        await SetupClaimedAgent();

        var result = await GuardWithStdinAsync(ShellJson(toolName, command));

        result.AssertPreToolUseDenied();
    }

    // DYD-723: a command nested past the guard's reading bound is refused as too complex to verify,
    // through Claude's Bash and PowerShell tools and in an unknown shell.
    [Trait("Job", "trap: DYD-723 nesting past the reading bound is refused, promptly, on every path")]
    [Theory]
    [InlineData("Bash")]
    [InlineData("PowerShell")]
    [InlineData("")]
    public async Task DeepNesting_IsRefusedAsTooComplex(string toolName)
    {
        await SetupClaimedAgent();

        var result = await NestingGuardAsync(toolName, 40);

        AssertRefused(toolName, result);
    }

    // 100,000 characters of it are refused as promptly, since reading stops at the bound. Before
    // DYD-723 the reading recursed once per level and overflowed the stack: run on the fix only.
    [Trait("Job", "trap: DYD-723 nesting past the reading bound is refused, promptly, on every path")]
    [Theory]
    [InlineData("Bash")]
    [InlineData("PowerShell")]
    [InlineData("")]
    public async Task LongDeepNesting_IsRefusedPromptly(string toolName)
    {
        await SetupClaimedAgent();
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        var result = await NestingGuardAsync(toolName, 33_333);

        Assert.True(stopwatch.Elapsed < TimeSpan.FromSeconds(10), $"Took {stopwatch.Elapsed}");
        AssertRefused(toolName, result);
    }

    // The bound holds on every path a command runs (DYD-723). bash reads an unquoted $( as a group,
    // which nests as a substitution does, and a command string a shell runs is read eight levels
    // below the command. Past 32 groups, or past eight shells around a substitution nested past the
    // bound, the command is refused in every lane; at 32 groups, and at eight shells around one
    // nested 16 deep, it is allowed as before.
    [Trait("Job", "trap: DYD-723 nesting past the reading bound is refused, promptly, on every path")]
    [Theory]
    [InlineData("Bash", "$(", ")", 33, 0, true)]
    [InlineData("PowerShell", "$(", ")", 33, 0, true)]
    [InlineData("", "$(", ")", 33, 0, true)]
    [InlineData("Bash", "$(", ")", 32, 0, false)]
    [InlineData("PowerShell", "$(", ")", 32, 0, false)]
    [InlineData("", "$(", ")", 32, 0, false)]
    [InlineData("Bash", "\"$(", ")\"", 40, 9, true)]
    [InlineData("PowerShell", "\"$(", ")\"", 40, 9, true)]
    [InlineData("", "\"$(", ")\"", 40, 9, true)]
    [InlineData("Bash", "\"$(", ")\"", 16, 8, false)]
    [InlineData("PowerShell", "\"$(", ")\"", 16, 8, false)]
    [InlineData("", "\"$(", ")\"", 16, 8, false)]
    public async Task NestingOnEveryPath_PastTheBound_IsRefused(string toolName, string opener, string closer, int levels, int shells, bool refused)
    {
        await SetupClaimedAgent();
        var command = "echo " + string.Concat(Enumerable.Repeat(opener, levels)) + "x" + string.Concat(Enumerable.Repeat(closer, levels));
        for (var shell = 0; shell < shells; shell++)
            command = "bash -c '" + command.Replace("'", "'\"'\"'") + "'";

        var result = toolName == "" ? await UnknownShellGuardAsync(command) : await GuardWithStdinAsync(ShellJson(toolName, command));

        if (refused)
        {
            AssertRefused(toolName, result);
        }
        else if (toolName == "")
            result.AssertSuccess();
        else
            result.AssertPreToolUseAllowed();
    }

    // A hook call is refused with a deny decision, the CLI lane with exit 2.
    private static void AssertRefused(string toolName, CommandResult result)
    {
        if (toolName == "")
            result.AssertExitCode(2);
        else
            result.AssertPreToolUseDenied();
    }

    private Task<CommandResult> NestingGuardAsync(string toolName, int levels)
    {
        var command = "echo " + string.Concat(Enumerable.Repeat("\"$(", levels)) + "x" + string.Concat(Enumerable.Repeat(")\"", levels));
        return toolName == "" ? UnknownShellGuardAsync(command) : GuardWithStdinAsync(ShellJson(toolName, command));
    }
}
