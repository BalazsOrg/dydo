namespace DynaDocs.Tests.Integration;

/// <summary>
/// Guard pipeline integration tests for the worker lane (Decision 024). Worker calls carry
/// agent_id/agent_type in the hook payload; only off-limits, dangerous-bash, and nudges apply
/// to them.
/// </summary>
[Collection("Integration")]
[Trait("Job", "contract: Decision 024 worker lane, DR 055 host memory")]
public class GuardWorkerLaneTests : IntegrationTestBase
{
    private string WorkerJson(string toolName, string inputJson) =>
        $"{{\"session_id\":\"{TestSessionId}\",\"agent_id\":\"wkr-test-1\",\"agent_type\":\"reviewer\","
        + $"\"tool_name\":\"{toolName}\",\"tool_input\":{inputJson}}}";

    #region Worker Lane - Universal Layers Only

    [Fact]
    public async Task Worker_NoClaim_CanReadSourceFile()
    {
        await InitProjectAsync();

        var result = await GuardWithStdinAsync(WorkerJson("Read", "{\"file_path\":\"src/Foo.cs\"}"));

        result.AssertPreToolUseAllowed();
    }

    [Fact]
    public async Task Worker_NoClaim_CanWriteSourceFile()
    {
        await InitProjectAsync();

        var result = await GuardWithStdinAsync(WorkerJson("Write", "{\"file_path\":\"src/Foo.cs\"}"));

        result.AssertPreToolUseAllowed();
    }

    [Fact]
    public async Task Worker_NoClaim_CanBashReadSourceFile()
    {
        await InitProjectAsync();

        var result = await GuardWithStdinAsync(WorkerJson("Bash", "{\"command\":\"cat src/Foo.cs\"}"));

        result.AssertPreToolUseAllowed();
    }

    [Fact]
    public async Task Worker_OffLimitsPath_Blocked()
    {
        await InitProjectAsync();

        var result = await GuardWithStdinAsync(WorkerJson("Write", "{\"file_path\":\".env\"}"));

        result.AssertPreToolUseDenied();
    }

    [Fact]
    public async Task Worker_BashOffLimitsPath_Blocked()
    {
        await InitProjectAsync();

        var result = await GuardWithStdinAsync(WorkerJson("Bash", "{\"command\":\"cat .env\"}"));

        result.AssertPreToolUseDenied();
    }

    [Fact]
    public async Task Worker_DangerousBash_Blocked()
    {
        await InitProjectAsync();

        var result = await GuardWithStdinAsync(WorkerJson("Bash", "{\"command\":\"rm -rf /\"}"));

        result.AssertPreToolUseDenied();
    }

    [Fact]
    public async Task Worker_BlockSeverityNudge_Applies()
    {
        await InitProjectAsync();

        // H19: indirect dydo invocation is a block-severity default nudge
        var result = await GuardWithStdinAsync(WorkerJson("Bash", "{\"command\":\"npx dydo agent claim auto\"}"));

        result.AssertPreToolUseDenied();
    }

    [Fact]
    public async Task Worker_BashWriteWithBypassAttempt_Blocked()
    {
        await InitProjectAsync();

        // Command substitution + write op: analysis is unreliable, so writes are blocked
        var result = await GuardWithStdinAsync(WorkerJson("Bash", "{\"command\":\"echo $(whoami) > out.txt\"}"));

        result.AssertPreToolUseDenied();
    }

    private async Task<CommandResult> WorkerShellAsync(string tool, string command)
    {
        await InitProjectAsync();
        var input = $"{{\"command\":{System.Text.Json.JsonSerializer.Serialize(command)}}}";
        return await GuardWithStdinAsync(WorkerJson(tool, input));
    }

    // Rule 1: a command the dydo regex does not match is not a dydo call for the worker lane,
    // so directory arguments, continued lines and prose stay allowed.
    [Theory]
    [InlineData("Bash", "find dydo -name '*.md'")]
    [InlineData("Bash", "grep -rn foo dydo src")]
    [InlineData("Bash", "git diff -- dydo src")]
    [InlineData("Bash", "find \\\n  dydo -name '*.md'")]
    [InlineData("Bash", "git diff --stat \\\n  dydo src")]
    [InlineData("Bash", "rg -n \"pattern\" \\\n  dydo src tests")]
    [InlineData("PowerShell", "Get-ChildItem dydo -Recurse")]
    [InlineData("PowerShell", "Get-ChildItem `\n  dydo -Recurse")]
    [InlineData("Bash", "echo dydo sync")]
    [InlineData("Bash", "git commit -m \"fix (dydo init blocked)\"")]
    [InlineData("Bash", "rg -n \"dydo init\" Commands")]
    public async Task Worker_NotBaseDydoCommand_Allowed(string tool, string command)
    {
        var result = await WorkerShellAsync(tool, command);

        result.AssertPreToolUseAllowed();
    }

    // Rule 2 and 3: a matched command is allowed when every dydo token followed by a word
    // names a gate; a bare dydo prints help.
    [Theory]
    [InlineData("dydo check")]
    [InlineData("dydo check && dydo fix dydo/guides")]
    [InlineData("dydo check dydo/guides")]
    [InlineData("dydo --help")]
    [InlineData("dydo")]
    [InlineData("./dydo fix dydo/guides/coding-standards.md")]
    [InlineData("dydo gap-check")]
    [InlineData("dydo graph stats")]
    [InlineData("dydo validate")]
    [InlineData("dydo version")]
    [InlineData("dydo pr-ready --body pr-body.md")]
    [InlineData("dydo -h")]
    [InlineData("dydo --version")]
    [InlineData("dydo check \\\n  dydo/understand")]
    [InlineData("dydo survey")]
    [InlineData("dydo survey P-DYD-46")]
    [InlineData("dydo survey \"Project ID\" --json")]
    [InlineData("dydo statechart")]
    [InlineData("dydo statechart SPEC.tla --skill skills/engineering/statechart --out /tmp/chart")]
    [InlineData("dydo statechart install-skill --root /work/project")]
    [InlineData("dydo statechart resume --request /tmp/request.json")]
    [InlineData("dydo statechart doctor")]
    [InlineData("./dydo statechart SPEC.tla && dydo survey P-DYD-46")]
    public async Task Worker_GateDydoCommand_Allowed(string command)
    {
        var result = await WorkerShellAsync("Bash", command);

        result.AssertPreToolUseAllowed();
    }

    // A matched line naming any word off the worker list is refused: the top-level-only commands, a
    // retired or unknown subcommand, a word after a substitution, a continuation or any Unicode space.
    [Theory]
    [InlineData("Bash", "dydo init claude")]
    [InlineData("Bash", "dydo adopt 3.7.0")]
    [InlineData("Bash", "dydo index")]
    [InlineData("Bash", "dydo guard --host claude")]
    [InlineData("Bash", "dydo map --no-browser")]
    [InlineData("Bash", "dydo completions bash")]
    [InlineData("Bash", "dydo _complete")]
    [InlineData("Bash", "dydo survey P-DYD-46 && dydo init")]
    [InlineData("Bash", "dydo agent claim auto")]
    [InlineData("Bash", "dydo sync")]
    [InlineData("Bash", "dydo check && dydo sync")]
    [InlineData("Bash", "dydo check $(dydo init)")]
    [InlineData("Bash", "dydo check \"$(dydo index)\"")]
    [InlineData("Bash", "dydo check `dydo init`")]
    [InlineData("Bash", "dydo check <(dydo init)")]
    [InlineData("PowerShell", "dydo check (dydo init claude)")]
    [InlineData("Bash", "dydo check && dy\\\ndo init")]
    [InlineData("Bash", "dydo \\\ninit")]
    [InlineData("PowerShell", "dydo `\ninit claude")]
    [InlineData("PowerShell", "dydo `\r\ninit claude")]
    [InlineData("Bash", "dydo check; DYDO SYNC")]
    [InlineData("Bash", "dydo check; ./dydo.exe init")]
    [InlineData("Bash", "dydo check dydo src")]
    [InlineData("PowerShell", "dydo\finit")]
    [InlineData("PowerShell", "dydo\vinit")]
    [InlineData("PowerShell", "dydo\u00A0init")]
    [InlineData("PowerShell", "dydo\u2003init")]
    [InlineData("PowerShell", "dydo\u0085init")]
    [InlineData("PowerShell", "dydo check; dydo init")]
    public async Task Worker_NonGateDydoCommand_IsRefused(string tool, string command)
    {
        var result = await WorkerShellAsync(tool, command);

        result.AssertPreToolUseDenied();
    }

    [Fact]
    public async Task TopLevelSession_DydoSync_NotBlockedByWorkerRule()
    {
        await InitProjectAsync();

        var json = $"{{\"session_id\":\"{TestSessionId}\",\"tool_name\":\"Bash\","
            + "\"tool_input\":{\"command\":\"dydo check && dydo sync\"}}";
        var result = await GuardWithStdinAsync(json);

        result.AssertPreToolUseAllowed();
    }

    [Fact]
    public async Task Worker_SystemPath_Blocked()
    {
        await InitProjectAsync();

        // dydo/_system/** and dydo.json are agent-untouchable system off-limits.
        // Use ABSOLUTE paths - that's what Claude Code's hook actually delivers.
        var systemAbs = Path.Combine(TestDir, "dydo", "_system", "roles", "x.json").Replace('\\', '/');
        var systemWrite = await GuardWithStdinAsync(WorkerJson("Write", $"{{\"file_path\":\"{systemAbs}\"}}"));
        systemWrite.AssertPreToolUseDenied();

        var configAbs = Path.Combine(TestDir, "dydo.json").Replace('\\', '/');
        var config = await GuardWithStdinAsync(WorkerJson("Write", $"{{\"file_path\":\"{configAbs}\"}}"));
        config.AssertPreToolUseDenied();
    }

    #endregion

    #region Host Memory (DR 055)

    private static string MemoryPath(params string[] tail)
    {
        var home = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile).Replace('\\', '/');
        return $"{home}/.claude/projects/proj/memory/{string.Join('/', tail)}";
    }

    private string MainJson(string toolName, string inputJson) =>
        $"{{\"session_id\":\"{TestSessionId}\",\"tool_name\":\"{toolName}\",\"tool_input\":{inputJson}}}";

    [Fact]
    public async Task Memory_NoIdentity_ReadAllowed()
    {
        await InitProjectAsync();

        var result = await GuardWithStdinAsync(MainJson("Read", $"{{\"file_path\":\"{MemoryPath("debugging.md")}\"}}"));

        result.AssertPreToolUseAllowed();
    }

    [Theory]
    [InlineData("Write")]
    [InlineData("Edit")]
    public async Task Memory_NoIdentity_WriteBlocked(string tool)
    {
        await InitProjectAsync();

        var result = await GuardWithStdinAsync(MainJson(tool, $"{{\"file_path\":\"{MemoryPath("MEMORY.md")}\"}}"));

        result.AssertPreToolUseDenied();
    }

    [Fact]
    public async Task Memory_TraversalIntoMemory_WriteBlocked()
    {
        await InitProjectAsync();

        var sneaky = MemoryPath("..", "..", "other", "..", "proj", "memory", "MEMORY.md");
        var result = await GuardWithStdinAsync(MainJson("Write", $"{{\"file_path\":\"{sneaky}\"}}"));

        result.AssertPreToolUseDenied();
    }

    [Fact]
    public async Task Memory_ApplyPatchIntoCodexMemories_Blocked()
    {
        await InitProjectAsync();
        var home = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile).Replace('\\', '/');
        var codexHome = Environment.GetEnvironmentVariable("CODEX_HOME")?.Replace('\\', '/') ?? $"{home}/.codex";

        var patch = $"*** Begin Patch\\n*** Add File: {codexHome}/memories/note.md\\n+x\\n*** End Patch";
        var result = await GuardWithStdinAsync(MainJson("apply_patch", $"{{\"command\":\"{patch}\"}}"));

        result.AssertPreToolUseDenied();
    }

    [Fact]
    public async Task Memory_ClaudeConfigDirProfile_WriteBlocked()
    {
        await InitProjectAsync();
        var profile = Path.Combine(TestDir, "profile").Replace('\\', '/');
        var original = Environment.GetEnvironmentVariable("CLAUDE_CONFIG_DIR");
        Environment.SetEnvironmentVariable("CLAUDE_CONFIG_DIR", profile);
        try
        {
            var result = await GuardWithStdinAsync(
                MainJson("Write", $"{{\"file_path\":\"{profile}/projects/p/memory/MEMORY.md\"}}"));

            result.AssertPreToolUseDenied();
        }
        finally
        {
            Environment.SetEnvironmentVariable("CLAUDE_CONFIG_DIR", original);
        }
    }

    [Fact]
    public async Task Worker_MemoryWrite_Blocked_ReadAllowed()
    {
        await InitProjectAsync();

        var write = await GuardWithStdinAsync(WorkerJson("Write", $"{{\"file_path\":\"{MemoryPath("MEMORY.md")}\"}}"));
        write.AssertPreToolUseDenied();

        var read = await GuardWithStdinAsync(WorkerJson("Read", $"{{\"file_path\":\"{MemoryPath("MEMORY.md")}\"}}"));
        read.AssertPreToolUseAllowed();
    }

    [Theory]
    [InlineData("echo note > ~/.claude/projects/proj/memory/MEMORY.md")]
    [InlineData("echo note | tee ~/.claude/projects/proj/memory/MEMORY.md")]
    [InlineData("cp notes.md ~/.claude/projects/proj/memory/notes.md")]
    [InlineData("mv notes.md ~/.claude/agent-memory/reviewer/notes.md")]
    [InlineData("rm .claude/agent-memory/reviewer/MEMORY.md")]
    [InlineData("rm -r ~/.codex/memories")]
    [InlineData("cp notes.md ~/.claude/projects/proj/memory/")]
    [InlineData("cp -t ~/.claude/projects/proj/memory notes.md")]
    [InlineData("cp ~/.claude/projects/proj/memory/a.md ~/.claude/projects/proj/memory/b.md")]
    [InlineData("mv ~/.claude/projects/proj/memory/x.md notes.md")]
    public async Task Memory_ShellWrite_Blocked(string command)
    {
        await InitProjectAsync();

        if (command == "rm -r ~/.codex/memories")
        {
            var codexHome = Environment.GetEnvironmentVariable("CODEX_HOME");
            if (string.IsNullOrWhiteSpace(codexHome))
                codexHome = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".codex");
            var memoryPath = Path.GetFullPath(Path.Combine(codexHome, "memories")).Replace('\\', '/');
            command = $"rm -r '{memoryPath.Replace("'", "'\\''")}'";
        }
        var input = System.Text.Json.JsonSerializer.Serialize(new { command });

        var main = await GuardWithStdinAsync(MainJson("Bash", input));
        main.AssertPreToolUseDenied();

        var worker = await GuardWithStdinAsync(WorkerJson("Bash", input));
        worker.AssertPreToolUseDenied();
    }

    [Theory]
    [InlineData("codex home")]
    [InlineData(null)]
    [InlineData(" \t ")]
    public async Task Memory_CodexShellWrite_EffectiveHome_Blocked(string? codexHome)
    {
        var original = Environment.GetEnvironmentVariable("CODEX_HOME");
        try
        {
            Environment.SetEnvironmentVariable("CODEX_HOME", string.IsNullOrWhiteSpace(codexHome)
                ? codexHome : Path.Combine(TestDir, codexHome));
            await Memory_ShellWrite_Blocked("rm -r ~/.codex/memories");
        }
        finally
        {
            Environment.SetEnvironmentVariable("CODEX_HOME", original);
        }
    }

    [Theory]
    [InlineData("cat ~/.claude/projects/proj/memory/MEMORY.md")]
    [InlineData("ls ~/.claude/projects/proj/memory")]
    [InlineData("grep -r lesson ~/.codex/memories")]
    [InlineData("echo note > notes/memory/MEMORY.md")]
    public async Task Memory_ShellReadOrOrdinaryWrite_Allowed(string command)
    {
        await InitProjectAsync();

        var result = await GuardWithStdinAsync(MainJson("Bash", $"{{\"command\":\"{command}\"}}"));

        result.AssertPreToolUseAllowed();
    }

    [Theory]
    [InlineData("cp ~/.claude/projects/proj/memory/MEMORY.md notes.md")]
    [InlineData("cp -r ~/.claude/projects/proj/memory backup")]
    [InlineData("cp -t backup ~/.claude/projects/proj/memory/MEMORY.md")]
    public async Task Memory_ShellCopyOut_Allowed(string command)
    {
        await InitProjectAsync();

        var main = await GuardWithStdinAsync(MainJson("Bash", $"{{\"command\":\"{command}\"}}"));
        main.AssertPreToolUseAllowed();

        var worker = await GuardWithStdinAsync(WorkerJson("Bash", $"{{\"command\":\"{command}\"}}"));
        worker.AssertPreToolUseAllowed();
    }

    #endregion

    #region Tier-1 Writes (identity-free)

    #endregion
}
