namespace DynaDocs.Tests.Integration;

using DynaDocs.Commands;

/// <summary>
/// Integration tests for the guard command. The guard is identity-free (DR-041), so tests
/// need no claim, role or must-read setup.
/// </summary>
[Collection("Integration")]
[Trait("Job", "contract: files-off-limits.md tiers, DR 045 §10, issue #64 indirect dydo, the cd-chain coaching")]
public class GuardIntegrationTests : IntegrationTestBase
{
    #region Off-Limits

    [Fact]
    public async Task Guard_OffLimitsPath_Blocks()
    {
        await InitProjectAsync("none");

        var result = await GuardAsync("edit", ".env");

        result.AssertExitCode(2);
    }

    [Fact]
    public async Task Guard_DydoSystemFile_Blocks()
    {
        await InitProjectAsync("none");

        var result = await GuardAsync("edit", "dydo/index.md");

        result.AssertExitCode(2);
    }

    [Fact]
    public async Task Guard_DydoSystemFile_ReadAllows()
    {
        await InitProjectAsync("none");

        // …and read, because every entry prompt orders agents to read it (DR 045 §10).
        var result = await GuardAsync("read", "dydo/index.md");

        result.AssertSuccess();
    }

    [Fact]
    public async Task Guard_StdinHook_DirectReadOffLimits_Blocks()
    {
        await InitProjectAsync("none");

        // Off-limits binds on every direct file op, reads included, not just writes.
        var json = "{\"session_id\":\"" + TestSessionId
            + "\",\"tool_name\":\"Read\",\"tool_input\":{\"file_path\":\"config/secrets.json\"}}";
        var result = await GuardWithStdinAsync(json);

        result.AssertPreToolUseDenied();
    }


    [Fact]
    public async Task Guard_AllowedPath_Passes()
    {
        await InitProjectAsync("none");

        var result = await GuardAsync("edit", "src/file.cs");

        result.AssertSuccess();
    }

    #endregion

    #region Protected Tier - readable by every tool, writable by none (DR 045 §10)

    [Theory]
    [InlineData("dydo/index.md")]
    [InlineData("dydo/files-off-limits.md")]
    [InlineData("dydo.json")]
    public async Task Guard_ProtectedPath_ReadAllowed(string path)
    {
        await InitProjectAsync("none");

        var argMode = await GuardAsync("read", path);
        var hookMode = await GuardWithStdinAsync(
            $"{{\"session_id\":\"{TestSessionId}\",\"tool_name\":\"Read\",\"tool_input\":{{\"file_path\":\"{path}\"}}}}");

        argMode.AssertSuccess();
        hookMode.AssertPreToolUseAllowed();
    }

    [Theory]
    [InlineData("Edit", "dydo/index.md")]
    [InlineData("Write", "dydo/files-off-limits.md")]
    [InlineData("NotebookEdit", "dydo.json")]
    // Codex's apply_patch maps to no action, so the tier must recognize it by tool name or it
    // binds on Claude's lane only.
    [InlineData("apply_patch", "dydo/index.md")]
    public async Task Guard_ProtectedPath_DirectWriteToolBlocked(string toolName, string path)
    {
        await InitProjectAsync("none");

        var result = await GuardWithStdinAsync(
            $"{{\"session_id\":\"{TestSessionId}\",\"tool_name\":\"{toolName}\",\"tool_input\":{{\"file_path\":\"{path}\"}}}}");

        result.AssertPreToolUseDenied();
    }

    [Theory]
    [InlineData("sed -i 's/a/b/' dydo/index.md")]
    [InlineData("echo broken > dydo/files-off-limits.md")]
    [InlineData("rm dydo.json")]
    // A move takes the protected file away or lands on top of it; both directions mutate.
    [InlineData("mv dydo/index.md gone.md")]
    [InlineData("mv other.md dydo/index.md")]
    // A copy can overwrite a protected file. The analyzer cannot tell a copy's source from
    // its destination, so copying *out of* a protected path is blocked too, by design -
    // the content stays readable through Read, cat and head.
    [InlineData("cp evil.json dydo.json")]
    [InlineData("cp permissive.md dydo/files-off-limits.md")]
    [InlineData("cp dydo/index.md backup.md")]
    // A permission change is not a content write, but it is the classic self-escalation lever
    // against the guard's own config.
    [InlineData("chmod 777 dydo.json")]
    public async Task Guard_ProtectedPath_BashMutatingOperationBlocked(string command)
    {
        await InitProjectAsync("none");

        var result = await GuardWithStdinAsync(
            $"{{\"session_id\":\"{TestSessionId}\",\"tool_name\":\"Bash\",\"tool_input\":{{\"command\":\"{command}\"}}}}");

        // "protected", not "off-limits": the protected tier is what stops this.
        result.AssertPreToolUseDenied();
    }

    [Theory]
    [InlineData("cat dydo/index.md")]
    [InlineData("head -n 5 dydo.json")]
    public async Task Guard_ProtectedPath_BashReadAllowed(string command)
    {
        await InitProjectAsync("none");

        var result = await GuardWithStdinAsync(
            $"{{\"session_id\":\"{TestSessionId}\",\"tool_name\":\"Bash\",\"tool_input\":{{\"command\":\"{command}\"}}}}");

        result.AssertPreToolUseAllowed();
    }

    [Fact]
    public async Task Guard_UnprotectedPath_WriteToolsStillPass()
    {
        await InitProjectAsync("none");

        // The tier is three files wide: ordinary source stays writable on both lanes.
        var edit = await GuardWithStdinAsync(
            $"{{\"session_id\":\"{TestSessionId}\",\"tool_name\":\"Edit\",\"tool_input\":{{\"file_path\":\"src/Foo.cs\"}}}}");
        var patch = await GuardWithStdinAsync(
            $"{{\"session_id\":\"{TestSessionId}\",\"tool_name\":\"apply_patch\",\"tool_input\":{{\"file_path\":\"src/Foo.cs\"}}}}");

        edit.AssertPreToolUseAllowed();
        patch.AssertPreToolUseAllowed();
    }

    #endregion

    #region Reads allowed unless off-limits

    [Fact]
    public async Task Guard_ReadRootFile_Allows()
    {
        await InitProjectAsync("none");

        var result = await GuardAsync("read", "CLAUDE.md");

        result.AssertSuccess();
    }


    [Fact]
    public async Task Guard_ReadSourceFile_Allows()
    {
        await InitProjectAsync("none");

        var result = await GuardAsync("read", "src/code.cs");

        result.AssertSuccess();
    }


    [Fact]
    public async Task Guard_ReadNonAgentWorkflow_Allows()
    {
        await InitProjectAsync("none");

        var result = await GuardAsync("read", "docs/workflow.md");

        result.AssertSuccess();
    }

    #endregion

    #region Stdin Hook Mode

    [Fact]
    public async Task Guard_StdinHook_ReadBootstrapFile_Allows()
    {
        await InitProjectAsync("none");

        var json = "{\"session_id\":\"" + TestSessionId + "\",\"tool_name\":\"Read\",\"tool_input\":{\"file_path\":\"CLAUDE.md\"}}";
        var result = await GuardWithStdinAsync(json);

        result.AssertPreToolUseAllowed();
    }

    [Fact]
    public async Task Guard_StdinHook_Write_Allows()
    {
        await InitProjectAsync("none");

        var json = "{\"session_id\":\"" + TestSessionId + "\",\"tool_name\":\"Write\",\"tool_input\":{\"file_path\":\"src/test.cs\"}}";
        var result = await GuardWithStdinAsync(json);

        result.AssertPreToolUseAllowed();
    }

    #endregion

    #region Bash Commands (Hook Mode)

    [Fact]
    public async Task Guard_CommandOption_Parses()
    {
        await InitProjectAsync("none");

        var cmd = GuardCommand.Create();
        var result = await RunAsync(cmd, "--command", "dotnet build");

        result.AssertSuccess();
    }

    [Fact]
    public async Task Guard_CommandOption_AppliesShellAnalysis()
    {
        // Issue 0302: arg mode carries no tool name, yet a CLI --command is a shell command by
        // definition and must reach the shell analyzer.
        await InitProjectAsync("none");

        var cmd = GuardCommand.Create();
        var result = await RunAsync(cmd, "--command", "npx dydo check");

        result.AssertExitCode(2);
    }

    [Fact]
    public async Task Guard_CommandOption_BlocksOffLimitsReference()
    {
        await InitProjectAsync("none");

        var cmd = GuardCommand.Create();
        var result = await RunAsync(cmd, "--command", "cat .env");

        result.AssertExitCode(2);
    }

    #endregion

    #region Indirect Dydo Invocation

    [Theory]
    [InlineData("npx dydo check")]
    [InlineData("npx -q dydo check")]
    [InlineData("npx --yes dydo check")]
    [InlineData("dotnet dydo check")]
    [InlineData("dotnet tool run dydo check")]
    [InlineData("dotnet run -- guard --action read --path foo.cs")]
    [InlineData("dotnet run -- validate")]
    [InlineData("bash dydo check")]
    [InlineData("sh dydo check")]
    [InlineData("bash -c \\\"dydo check\\\"")]
    [InlineData("sh -c 'dydo check'")]
    [InlineData("python dydo check")]
    [InlineData("python3 dydo check")]
    [InlineData("py dydo check")]
    public async Task Guard_IndirectDydo_IsBlocked(string command)
    {
        await InitProjectAsync("none");

        var json = "{\"session_id\":\"" + TestSessionId + "\",\"tool_name\":\"Bash\",\"tool_input\":{\"command\":\"" + command + "\"}}";
        var result = await GuardWithStdinAsync(json);

        result.AssertPreToolUseDenied();
    }

    [Theory]
    [InlineData("npx prettier --write .")]
    [InlineData("dotnet build")]
    [InlineData("dotnet run")]
    [InlineData("dotnet run --project SomeApp")]
    [InlineData("dotnet run -- --help")]
    [InlineData("dotnet run -- serve --port 8080")]
    [InlineData("dotnet run -- myarg1 myarg2")]
    [InlineData("dotnet tool run other-tool --flag")]
    [InlineData("bash script.sh")]
    [InlineData("python script.py")]
    [InlineData("python3 -m pytest")]
    [InlineData("py -3 script.py")]
    public async Task Guard_IndirectDydo_FalsePositiveSafety(string command)
    {
        await InitProjectAsync("none");

        var json = "{\"session_id\":\"" + TestSessionId + "\",\"tool_name\":\"Bash\",\"tool_input\":{\"command\":\"" + command + "\"}}";
        var result = await GuardWithStdinAsync(json);

        result.AssertPreToolUseAllowed();
    }

    [Fact]
    public async Task Guard_DirectDydo_StillWorks()
    {
        await InitProjectAsync("none");

        var json = "{\"session_id\":\"" + TestSessionId + "\",\"tool_name\":\"Bash\",\"tool_input\":{\"command\":\"dydo check\"}}";
        var result = await GuardWithStdinAsync(json);

        result.AssertPreToolUseAllowed();
    }

    [Fact]
    public async Task Guard_ChainedIndirectDydo_StillCaught()
    {
        await InitProjectAsync("none");

        var json = "{\"session_id\":\"" + TestSessionId + "\",\"tool_name\":\"Bash\",\"tool_input\":{\"command\":\"cd /tmp && npx dydo check\"}}";
        var result = await GuardWithStdinAsync(json);

        result.AssertPreToolUseDenied();
    }

    #endregion

    #region Coaching: cd Compound

    [Fact]
    public async Task Guard_CdGitCompound_BlocksWithCoachingMessage()
    {
        await InitProjectAsync("none");

        var json = "{\"session_id\":\"" + TestSessionId + "\",\"tool_name\":\"Bash\",\"tool_input\":{\"command\":\"cd /c/Users/User/Desktop/Projects && git diff --name-only\"}}";
        var result = await GuardWithStdinAsync(json);

        result.AssertPreToolUseDenied();
    }

    [Fact]
    public async Task Guard_CdNonGitCompound_Blocked()
    {
        await InitProjectAsync("none");

        var json = "{\"session_id\":\"" + TestSessionId + "\",\"tool_name\":\"Bash\",\"tool_input\":{\"command\":\"cd /tmp && ls\"}}";
        var result = await GuardWithStdinAsync(json);

        result.AssertPreToolUseDenied();
    }

    #endregion

    #region Search Tools (Glob/Grep)

    [Theory]
    [InlineData("Glob")]
    [InlineData("Grep")]
    public async Task Guard_SearchTool_WithPath_Allows(string toolName)
    {
        await InitProjectAsync("none");

        var json = $"{{\"session_id\":\"{TestSessionId}\",\"tool_name\":\"{toolName}\",\"tool_input\":{{\"path\":\"src\",\"pattern\":\"*.cs\"}}}}";
        var result = await GuardWithStdinAsync(json);

        result.AssertPreToolUseAllowed();
    }

    [Theory]
    [InlineData("Glob")]
    [InlineData("Grep")]
    public async Task Guard_SearchTool_NoPath_Allows(string toolName)
    {
        await InitProjectAsync("none");

        var json = $"{{\"session_id\":\"{TestSessionId}\",\"tool_name\":\"{toolName}\",\"tool_input\":{{\"pattern\":\"*.cs\"}}}}";
        var result = await GuardWithStdinAsync(json);

        result.AssertPreToolUseAllowed();
    }

    [Theory]
    [InlineData("Glob")]
    [InlineData("Grep")]
    public async Task Guard_SearchTool_OffLimitsPath_Blocks(string toolName)
    {
        await InitProjectAsync("none");

        var json = $"{{\"session_id\":\"{TestSessionId}\",\"tool_name\":\"{toolName}\",\"tool_input\":{{\"path\":\".env\",\"pattern\":\"*\"}}}}";
        var result = await GuardWithStdinAsync(json);

        result.AssertPreToolUseDenied();
    }

    [Theory]
    [InlineData("Read", "file_path")]
    [InlineData("Glob", "path")]
    public async Task Guard_NonWorkerTraversalPath_MatchesDirectOffLimitsBlock(
        string toolName, string pathProperty)
    {
        await InitProjectAsync("none");

        var suffix = toolName == "Glob" ? ",\"pattern\":\"*\"" : "";
        async Task<CommandResult> Run(string path) => await GuardWithStdinAsync(
            $"{{\"session_id\":\"{TestSessionId}\",\"tool_name\":\"{toolName}\","
            + $"\"tool_input\":{{\"{pathProperty}\":\"{path}\"{suffix}}}}}");

        var direct = await Run("dydo/_system/types.json");
        var traversal = await Run("safe/../dydo/_system/types.json");

        direct.AssertPreToolUseDenied();
        traversal.AssertPreToolUseDenied();
    }

    #endregion

    #region Git commands

    [Theory]
    [InlineData("git stash")]
    [InlineData("git stash pop")]
    [InlineData("git stash apply")]
    [InlineData("git merge feature-branch")]
    [InlineData("git merge --no-ff main")]
    public async Task Guard_GitStashAndMerge_NotBlocked(string command)
    {
        await InitProjectAsync("none");

        var json = $"{{\"session_id\":\"{TestSessionId}\",\"tool_name\":\"Bash\",\"tool_input\":{{\"command\":\"{command}\"}}}}";
        var result = await GuardWithStdinAsync(json);

        result.AssertPreToolUseAllowed();
    }

    [Theory]
    [InlineData("git status")]
    [InlineData("git commit -m 'test'")]
    [InlineData("git diff")]
    [InlineData("git log")]
    public async Task Guard_OtherGitCommands_NotBlocked(string command)
    {
        await InitProjectAsync("none");

        var json = $"{{\"session_id\":\"{TestSessionId}\",\"tool_name\":\"Bash\",\"tool_input\":{{\"command\":\"{command}\"}}}}";
        var result = await GuardWithStdinAsync(json);

        result.AssertPreToolUseAllowed();
    }

    #endregion

    #region Blocked Tools

    // Claude Code's built-in plan mode is refused: dydo plans live in the repo or Linear.
    [Theory]
    [InlineData("EnterPlanMode")]
    [InlineData("ExitPlanMode")]
    public async Task Guard_PlanModeTools_AreRefused(string tool)
    {
        await InitProjectAsync("none");

        var json = "{\"session_id\":\"" + TestSessionId + "\",\"tool_name\":\"" + tool + "\",\"tool_input\":{}}";
        var result = await GuardWithStdinAsync(json);

        result.AssertPreToolUseDenied();
    }

    #endregion

}
