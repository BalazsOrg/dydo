namespace DynaDocs.Tests.Integration;

using System.Text.Json;
using DynaDocs.Models;
using DynaDocs.Services;

/// <summary>
/// New-file nudges through the real hook entry point (stdin payloads), main and worker lanes,
/// including Codex's real apply_patch shape: the whole patch in tool_input.command, no file_path.
/// </summary>
[Collection("Integration")]
[Trait("Job", "contract: dydo-commands-core.md, a new-file nudge matches only a file a tool is about to create")]
public class GuardNewFileNudgeIntegrationTests : IntegrationTestBase
{
    private const string ToolsPattern = "^tools/(?!one-off/)";
    private const string ToolsMessage = "New tools go under tools/one-off/.";

    private async Task InitWithNudges(params NudgeConfig[] nudges)
    {
        await InitProjectAsync("none");
        var configService = new ConfigService();
        var configPath = configService.FindConfigFile(TestDir)!;
        var config = configService.LoadConfig(TestDir)!;
        config.Nudges.AddRange(nudges);
        configService.SaveConfig(config, configPath);
    }

    private static NudgeConfig ToolsNudge(string severity) =>
        new() { Pattern = ToolsPattern, Message = ToolsMessage, Severity = severity, Match = "new-file" };

    // Codex's apply_patch payload carries a turn_id; Claude's tool payloads do not.
    private string Payload(string toolName, object toolInput, string? agentId = null)
    {
        var payload = new Dictionary<string, object?>
        {
            ["session_id"] = TestSessionId,
            ["agent_id"] = agentId,
            ["tool_name"] = toolName,
            ["tool_input"] = toolInput,
            ["cwd"] = TestDir,
        };
        if (toolName == "apply_patch")
            payload["turn_id"] = "turn-1";
        return JsonSerializer.Serialize(payload);
    }

    private string ApplyPatch(params string[] body) =>
        Payload("apply_patch", new { command = string.Join("\n", ["*** Begin Patch", .. body, "*** End Patch"]) });

    [Fact]
    public async Task CodexApplyPatch_AddFileUnderTools_Blocked()
    {
        await InitWithNudges(ToolsNudge("block"));

        var result = await GuardWithStdinAsync(ApplyPatch("*** Add File: tools/x.py", "+print(1)"));

        result.AssertPreToolUseDenied();
        result.AssertStderrContains(ToolsMessage);
    }

    [Fact]
    public async Task CodexApplyPatch_AddFileUnderOneOff_Allowed()
    {
        await InitWithNudges(ToolsNudge("block"));

        var result = await GuardWithStdinAsync(ApplyPatch("*** Add File: tools/one-off/x.py", "+print(1)"));

        result.AssertPreToolUseAllowed();
    }

    [Fact]
    public async Task CodexApplyPatch_UpdateExistingTool_Allowed()
    {
        await InitWithNudges(ToolsNudge("block"));
        WriteFile("tools/existing.py", "print(0)");

        var result = await GuardWithStdinAsync(ApplyPatch("*** Update File: tools/existing.py", "@@", "-print(0)", "+print(1)"));

        result.AssertPreToolUseAllowed();
    }

    // New-file by tier, not by disk: Update File and Delete File never create, so the nudge stays
    // silent even when the path does not exist.
    [Theory]
    [InlineData("*** Update File: tools/missing.py", null)]
    [InlineData("*** Delete File: tools/missing.py", null)]
    [InlineData("*** Update File: tools/missing.py", "wkr-1")]
    public async Task CodexApplyPatch_UpdateOrDeleteMissingTool_NotNudged(string header, string? agentId)
    {
        await InitWithNudges(ToolsNudge("block"));

        var patch = string.Join("\r\n", "*** Begin Patch", header, "@@", "-a", "+b", "*** End Patch");
        var result = await GuardWithStdinAsync(Payload("apply_patch", new { command = patch }, agentId));

        result.AssertPreToolUseAllowed();
    }

    [Fact]
    public async Task CodexApplyPatch_MoveToNewToolPath_Blocked()
    {
        await InitWithNudges(ToolsNudge("block"));
        WriteFile("scripts/a.py", "print(0)");

        var result = await GuardWithStdinAsync(ApplyPatch("*** Update File: scripts/a.py", "*** Move to: tools/a.py"));

        result.AssertPreToolUseDenied();
    }

    [Fact]
    public async Task ClaudeWrite_NewTool_WarnBlocksOnceThenPassesOnRetry()
    {
        await InitWithNudges(ToolsNudge("warn"));
        var payload = Payload("Write", new { file_path = Path.Combine(TestDir, "tools", "new.py"), content = "x" });

        var first = await GuardWithStdinAsync(payload);
        var retry = await GuardWithStdinAsync(payload);

        first.AssertPreToolUseDenied();
        first.AssertStderrContains(ToolsMessage);
        retry.AssertPreToolUseAllowed();
    }

    [Fact]
    public async Task ClaudeEdit_ExistingTool_Allowed()
    {
        await InitWithNudges(ToolsNudge("block"));
        WriteFile("tools/existing.py", "print(0)");

        var result = await GuardWithStdinAsync(Payload("Edit", new { file_path = "tools/existing.py", old_string = "0", new_string = "1" }));

        result.AssertPreToolUseAllowed();
    }

    [Fact]
    public async Task Worker_ClaudeWrite_NewTool_Blocked()
    {
        await InitWithNudges(ToolsNudge("block"));

        var result = await GuardWithStdinAsync(Payload("Write", new { file_path = "tools/new.py", content = "x" }, agentId: "wkr-1"));

        result.AssertPreToolUseDenied();
        result.AssertStderrContains(ToolsMessage);
    }

    [Fact]
    public async Task Worker_CodexApplyPatch_AddFileUnderTools_Blocked()
    {
        await InitWithNudges(ToolsNudge("block"));

        var payload = Payload("apply_patch", new { command = "*** Begin Patch\n*** Add File: tools/x.py\n+x\n*** End Patch" }, agentId: "wkr-1");
        var result = await GuardWithStdinAsync(payload);

        result.AssertPreToolUseDenied();
    }

    private const string AnyPathMessage = "Nudge fired for any new file.";

    private static NudgeConfig AnyNewFileBlock() =>
        new() { Pattern = ".", Message = AnyPathMessage, Severity = "block", Match = "new-file" };

    // A patch the grammar cannot read names no target, and its file_path is never one: nothing
    // reaches the any-new-file block.
    [Theory]
    [Trait("Job", "contract: DYD-396 a patch line is a target only by Codex's patch grammar")]
    [InlineData(null)]
    [InlineData("not a patch")]
    [InlineData("*** Add File:")]
    [InlineData("*** Update File:   \r\n*** Delete File:")]
    public async Task UnparseablePatch_NamesNoTarget(string? patch)
    {
        await InitWithNudges(AnyNewFileBlock());

        var result = await GuardWithStdinAsync(Payload("apply_patch", new { file_path = "tools/x.py", command = patch }));

        result.AssertPreToolUseAllowed();
        Assert.DoesNotContain(AnyPathMessage, result.Stderr);
    }

    [Theory]
    [Trait("Job", "contract: LC-759 a path tier's refusal wins over a new-file nudge on the same write")]
    [InlineData(null, ".env", "off-limits")]
    [InlineData("wkr-1", ".env", "off-limits")]
    [InlineData(null, "dydo/index.md", "protected")]
    [InlineData("wkr-1", "dydo/index.md", "protected")]
    public async Task NewFileNudge_RunsAfterOffLimitsAndProtected_BlockTextWins(string? agentId, string path, string tier)
    {
        await InitWithNudges(AnyNewFileBlock());
        var existing = Path.Combine(TestDir, path);
        if (File.Exists(existing)) File.Delete(existing);

        var result = await GuardWithStdinAsync(Payload("Write", new { file_path = path, content = "x" }, agentId));

        result.AssertPreToolUseDenied();
        result.AssertStderrContains("BLOCKED:");
        result.AssertStderrContains(tier);
        Assert.DoesNotContain(AnyPathMessage, result.Stderr);
    }

    [Theory]
    [InlineData("../tools/new.py", true)]
    [InlineData("tools/new.py", false)]
    public async Task NewFileNudge_WithoutPayloadCwd_ResolvesAgainstProcessDirectory(string target, bool nudged)
    {
        await InitWithNudges(ToolsNudge("block"));
        var processDir = Path.Combine(TestDir, "src");
        Directory.CreateDirectory(processDir);
        var payload = JsonSerializer.Serialize(new Dictionary<string, object?>
        {
            ["session_id"] = TestSessionId,
            ["tool_name"] = "Write",
            ["tool_input"] = new { file_path = target, content = "x" },
        });

        var original = Environment.CurrentDirectory;
        Environment.CurrentDirectory = processDir;
        try
        {
            var result = await GuardWithStdinAsync(payload);

            if (nudged)
            {
                result.AssertPreToolUseDenied();
                result.AssertStderrContains(ToolsMessage);
            }
            else
            {
                result.AssertPreToolUseAllowed();
                Assert.DoesNotContain(ToolsMessage, result.Stderr);
            }
        }
        finally
        {
            Environment.CurrentDirectory = original;
        }
    }

    [Fact]
    public async Task ShellCreatedFile_IsCommandLaneOnly()
    {
        await InitWithNudges(ToolsNudge("block"));

        var result = await GuardWithStdinAsync(Payload("Bash", new { command = "touch tools/x.py" }));

        result.AssertPreToolUseAllowed();
    }

    [Fact]
    public async Task NewFileEntryWithH19Pattern_LeavesH19BlockingBash()
    {
        var h19 = ConfigFactory.DefaultNudges.First(n => n.Severity == "block" && n.Pattern.Contains("npx"));
        await InitProjectAsync("none");
        var configService = new ConfigService();
        var config = configService.LoadConfig(TestDir)!;
        config.Nudges = [new NudgeConfig { Pattern = h19.Pattern, Message = "file lane", Severity = "notice", Match = "new-file" }];
        configService.SaveConfig(config, configService.FindConfigFile(TestDir)!);

        var result = await GuardWithStdinAsync(Payload("Bash", new { command = "npx dydo agent claim auto" }));

        result.AssertPreToolUseDenied();
        result.AssertStderrContains("BLOCKED");
        Assert.DoesNotContain("file lane", result.Stderr);
        Assert.Contains("\"match\": \"new-file\"", File.ReadAllText(configService.FindConfigFile(TestDir)!));
    }

    private const string DocsNotice = "NOTICE: Docs usually live under `dydo/`. If this is a pointer README, or it belongs here, carry on.";

    [Theory]
    [InlineData("src/notes.md")]
    [InlineData("src/README.md")]
    public async Task DefaultProject_NewMdOutsideDydo_NoticesAndAllows(string path)
    {
        await InitProjectAsync("none");

        var result = await GuardWithStdinAsync(Payload("Write", new { file_path = path, content = "# x" }));

        result.AssertPreToolUseAllowed();
        result.AssertStderrContains(DocsNotice);
    }

    [Theory]
    [InlineData("skills/a/SKILL.md")]
    [InlineData(".github/x.md")]
    public async Task DefaultProject_ExceptedMdPath_NoNotice(string path)
    {
        await InitProjectAsync("none");

        var result = await GuardWithStdinAsync(Payload("Write", new { file_path = path, content = "# x" }));

        result.AssertPreToolUseAllowed();
        Assert.DoesNotContain("NOTICE: Docs usually", result.Stderr);
    }

    [Fact]
    public async Task DefaultProject_EditToExistingMd_NoNotice()
    {
        await InitProjectAsync("none");
        WriteFile("src/notes.md", "# old");

        var result = await GuardWithStdinAsync(Payload("Edit", new { file_path = "src/notes.md", old_string = "old", new_string = "new" }));

        result.AssertPreToolUseAllowed();
        Assert.DoesNotContain("NOTICE: Docs usually", result.Stderr);
    }
    // DYD-386: a block listed after a warn on the same new path refuses every try, and the warn
    // neither arms nor spends its pass meanwhile: once the block is gone the warn still meets the
    // first try, and only then does the retry pass.
    [Fact]
    [Trait("Job", "contract: DYD-386 a new-file block refuses every try and leaves a new-file warn on the same path unspent")]
    public async Task BlockAfterWarnInConfigOrder_RefusesEveryTry_AndLeavesTheWarnUnspent()
    {
        await InitWithNudges(ToolsNudge("warn"),
            new NudgeConfig { Pattern = "x[.]py", Message = "file block", Severity = "block", Match = "new-file" });
        var write = Payload("Write", new { file_path = Path.Combine(TestDir, "tools", "x.py"), content = "x" });

        for (var attempt = 1; attempt <= 3; attempt++)
            (await GuardWithStdinAsync(write)).AssertPreToolUseDenied();

        var configService = new ConfigService();
        var config = configService.LoadConfig(TestDir)!;
        config.Nudges.RemoveAll(nudge => nudge.Pattern == "x[.]py");
        configService.SaveConfig(config, configService.FindConfigFile(TestDir)!);

        (await GuardWithStdinAsync(write)).AssertPreToolUseDenied();
        (await GuardWithStdinAsync(write)).AssertPreToolUseAllowed();
    }

    // A command warn and a new-file warn on the same pattern keep separate passes: the command's
    // pass never lets the first new-file write through, and the command still has its own.
    [Fact]
    [Trait("Job", "trap: a command warn's pass let a new-file warn of the same pattern through")]
    public async Task CommandAndNewFileWarnsOfOnePattern_KeepSeparatePasses()
    {
        await InitWithNudges(new NudgeConfig { Pattern = ToolsPattern, Message = "command lane", Severity = "warn" }, ToolsNudge("warn"));
        var command = Payload("Bash", new { command = "tools/run.sh" });
        var write = Payload("Write", new { file_path = Path.Combine(TestDir, "tools", "x.py"), content = "x" });

        (await GuardWithStdinAsync(command)).AssertPreToolUseDenied();
        (await GuardWithStdinAsync(write)).AssertPreToolUseDenied();
        (await GuardWithStdinAsync(command)).AssertPreToolUseAllowed();
    }
}
