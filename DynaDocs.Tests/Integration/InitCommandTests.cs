using System.Text.Json.Nodes;
using DynaDocs.Services;
using DynaDocs.Utils;

namespace DynaDocs.Tests.Integration;

using DynaDocs.Commands;

/// <summary>
/// `dydo init` and `--join` observed as the files they leave: each host's wiring, the bytes of
/// files dydo did not write, and nothing at all when a preflight refuses.
/// </summary>
[Collection("Integration")]
[Trait("Job", "contract: dydo-commands-core.md dydo init and --join; DR 056 (committed guard); trap: DYD-392 (init and join keep foreign hook, settings and TOML bytes; a refused init writes nothing)")]
public class InitCommandTests : IntegrationTestBase
{
    private const string CustomHooks = """
        {
          "hooks": {
            "PreToolUse": [{
              "matcher": "CustomTool",
              "hooks": [{ "type": "command", "command": "echo custom" }]
            }],
            "PostToolUse": [{
              "matcher": "AnyTool",
              "hooks": [{ "type": "command", "command": "echo post" }]
            }]
          },
          "otherSetting": true
        }
        """;

    [Fact]
    public async Task Join_PostWorkFailure_PreservesOriginalConfigBytes()
    {
        (await InitProjectAsync("none")).AssertSuccess();
        var configPath = Path.Combine(TestDir, "dydo.json");
        AddLegacyModels(configPath);
        var original = File.ReadAllBytes(configPath);

        var (exitCode, _, stderr) = ConsoleCapture.All(() =>
            InitCommand.ExecuteJoin("codex", () => throw new IOException("injected post-work failure")));

        Assert.Equal(ExitCodes.ToolError, exitCode);
        Assert.Contains("injected post-work failure", stderr);
        // Hook wiring and the integration report precede the commit seam.
        AssertFileExists(".codex/hooks.json");
        Assert.Equal(original, File.ReadAllBytes(configPath));
        Assert.Contains("\"models\"", File.ReadAllText(configPath));
        Assert.Empty(Directory.GetFiles(TestDir, "dydo.json.*.tmp"));
    }

    [Fact]
    public async Task Join_AlreadyRecordedIntegration_LeavesConfigBytesUntouched()
    {
        (await InitProjectAsync("codex")).AssertSuccess();
        var configPath = Path.Combine(TestDir, "dydo.json");
        AddLegacyModels(configPath);
        var original = File.ReadAllBytes(configPath);

        var result = await JoinProjectAsync("codex");

        result.AssertSuccess();
        Assert.Equal(original, File.ReadAllBytes(configPath));
        Assert.Contains("\"models\"", File.ReadAllText(configPath));
        Assert.Empty(Directory.GetFiles(TestDir, "dydo.json.*.tmp"));
    }

    #region Adopted dydo version

    [Fact]
    public async Task Init_Fresh_RecordsRunningCliVersion()
    {
        (await InitProjectAsync("none")).AssertSuccess();

        Assert.Equal(DydoVersion.Current, ReadDydoVersion());
        Assert.Matches(@"^\d+\.\d+\.\d+", DydoVersion.Current);
    }

    [Fact]
    public async Task Join_ExistingDydoVersion_StaysUntouched()
    {
        (await InitProjectAsync("codex")).AssertSuccess();
        SetDydoVersion("1.2.3");

        (await JoinProjectAsync("codex")).AssertSuccess();

        Assert.Equal("1.2.3", ReadDydoVersion());
    }

    [Fact]
    public async Task Join_RecordingNewIntegration_PreservesDydoVersion()
    {
        (await InitProjectAsync("none")).AssertSuccess();
        SetDydoVersion("1.2.3");

        var result = await JoinProjectAsync("codex");

        result.AssertSuccess();
        Assert.Equal("1.2.3", ReadDydoVersion());
    }

    [Fact]
    public async Task Join_WithoutDydoVersion_WritesNone()
    {
        (await InitProjectAsync("none")).AssertSuccess();
        SetDydoVersion(null);

        var result = await JoinProjectAsync("codex");

        result.AssertSuccess();
        Assert.False(ReadConfigJson().ContainsKey("dydoVersion"));
    }

    private JsonObject ReadConfigJson() =>
        JsonNode.Parse(File.ReadAllText(Path.Combine(TestDir, "dydo.json")))!.AsObject();

    private string? ReadDydoVersion() => ReadConfigJson()["dydoVersion"]?.GetValue<string>();

    private void SetDydoVersion(string? version)
    {
        var raw = ReadConfigJson();
        raw.Remove("dydoVersion");
        if (version != null)
            raw["dydoVersion"] = version;
        File.WriteAllText(Path.Combine(TestDir, "dydo.json"), raw.ToJsonString());
    }

    #endregion

    private static void AddLegacyModels(string configPath)
    {
        var raw = JsonNode.Parse(File.ReadAllText(configPath))!.AsObject();
        raw["models"] = JsonNode.Parse("""{"agents":{"reviewer":"strong"},"tiers":{"openai":{"strong":"legacy"}}}""");
        raw["unrelated"] = JsonNode.Parse("""{"sentinel":[3,1,4]}""");
        File.WriteAllText(configPath, raw.ToJsonString());
    }

    #region Host wiring

    [Fact]
    public async Task Init_All_WritesHostAgentSettings()
    {
        var result = await InitProjectAsync("all");

        result.AssertSuccess();
        var claude = Assert.IsType<JsonObject>(JsonNode.Parse(ReadFile(".claude/settings.json")));
        var env = Assert.IsType<JsonObject>(claude["env"]);
        Assert.Equal("3", env["CLAUDE_CODE_MAX_SUBAGENT_SPAWN_DEPTH"]?.GetValue<string>());
        Assert.False(claude["autoMemoryEnabled"]?.GetValue<bool>());
        Assert.Equal("[agents]\nmax_depth = 3\nmax_concurrent_threads_per_session = 16\n\n[features]\nmemories = false\n",
            ReadFile(".codex/config.toml"));
    }

    [Fact]
    public async Task Join_All_TurnsNativeMemoryOffOverTrueAndSaysSo()
    {
        (await InitProjectAsync("none")).AssertSuccess();
        Directory.CreateDirectory(Path.Combine(TestDir, ".claude"));
        Directory.CreateDirectory(Path.Combine(TestDir, ".codex"));
        WriteFile(".claude/settings.json", "{ \"other\": \"keep\", \"autoMemoryEnabled\": true }");
        WriteFile(".codex/config.toml",
            "# keep\n[features]\nother = true\nmemories = true  # was on\n\n[agents]\nmax_depth = 3\nmax_concurrent_threads_per_session = 16\n");

        var result = await JoinProjectAsync("all");

        result.AssertSuccess();
        var claude = Assert.IsType<JsonObject>(JsonNode.Parse(ReadFile(".claude/settings.json")));
        Assert.False(claude["autoMemoryEnabled"]?.GetValue<bool>());
        Assert.Equal("keep", claude["other"]?.GetValue<string>());
        Assert.Equal(
            "# keep\n[features]\nother = true\nmemories = false  # was on\n\n[agents]\nmax_depth = 3\nmax_concurrent_threads_per_session = 16\n",
            ReadFile(".codex/config.toml"));
    }

    [Fact]
    public async Task Join_Codex_AddsMemoriesIntoExistingFeaturesTable()
    {
        (await InitProjectAsync("none")).AssertSuccess();
        Directory.CreateDirectory(Path.Combine(TestDir, ".codex"));
        WriteFile(".codex/config.toml", "[features]\nother = true\n\n[agents]\nmax_depth = 3\nmax_concurrent_threads_per_session = 16\n");

        var result = await JoinProjectAsync("codex");

        result.AssertSuccess();
        Assert.Equal(
            "[features]\nother = true\nmemories = false\n\n[agents]\nmax_depth = 3\nmax_concurrent_threads_per_session = 16\n",
            ReadFile(".codex/config.toml"));
    }

    [Theory]
    [InlineData("features = { memories = true }\n")]
    [InlineData("features.memories = true\n")]
    [InlineData("[features]\n\"memories\" = true\n")]
    public async Task Init_Codex_RejectsMemoriesFormItCannotEditBeforeAnySideEffect(string toml)
    {
        Directory.CreateDirectory(Path.Combine(TestDir, ".codex"));
        WriteFile(".codex/config.toml", toml);
        var original = File.ReadAllBytes(Path.Combine(TestDir, ".codex/config.toml"));

        var result = await InitProjectAsync("codex");

        result.AssertExitCode(2);
        result.AssertStderrContains(".codex/config.toml");
        Assert.Equal(original, File.ReadAllBytes(Path.Combine(TestDir, ".codex/config.toml")));
        AssertFileNotExists("dydo.json");
    }

    [Fact]
    public async Task Init_HostAgentSettings_PreserveSatisfyingValuesAndRepeatBytes()
    {
        Directory.CreateDirectory(Path.Combine(TestDir, ".claude"));
        Directory.CreateDirectory(Path.Combine(TestDir, ".codex"));
        WriteFile(".claude/settings.json", "{\n  \"other\": true,\n  \"env\": { \"CLAUDE_CODE_MAX_SUBAGENT_SPAWN_DEPTH\": \"4\" },\n  \"autoMemoryEnabled\": false\n}\n");
        WriteFile(".codex/config.toml", "# retain this\ncustom = true\n\n[agents]\nmax_depth = +5\nmax_concurrent_threads_per_session = +20\n\n[features]\nmemories = false\n");
        var claudeOriginal = ReadFile(".claude/settings.json");
        var codex = File.ReadAllBytes(Path.Combine(TestDir, ".codex/config.toml"));

        (await InitProjectAsync("all")).AssertSuccess();
        // The satisfying values stay as written; the only addition is the committed guard hook.
        var claudeAfterInit = ReadFile(".claude/settings.json");
        PreservedText.AssertLinesKeptInOrder(claudeOriginal, claudeAfterInit);
        var preToolUse = Assert.IsType<JsonArray>(JsonNode.Parse(claudeAfterInit)?["hooks"]?["PreToolUse"]);
        Assert.Equal(1, HookTestAssertions.Count(preToolUse, "dydo guard --host claude"));
        Assert.Equal(codex, File.ReadAllBytes(Path.Combine(TestDir, ".codex/config.toml")));
        var claude = File.ReadAllBytes(Path.Combine(TestDir, ".claude/settings.json"));

        (await JoinProjectAsync("all")).AssertSuccess();
        Assert.Equal(claude, File.ReadAllBytes(Path.Combine(TestDir, ".claude/settings.json")));
        Assert.Equal(codex, File.ReadAllBytes(Path.Combine(TestDir, ".codex/config.toml")));
    }

    [Fact]
    public async Task Join_Codex_BomAgentsTable_IsReadAndTheBomKept()
    {
        (await InitProjectAsync("none")).AssertSuccess();
        WriteFile(".codex/config.toml", "﻿[agents]\nmax_depth = 3\nmax_concurrent_threads_per_session = 16\n");

        var result = await JoinProjectAsync("codex");

        result.AssertSuccess();
        Assert.Equal("﻿[agents]\nmax_depth = 3\nmax_concurrent_threads_per_session = 16\n\n[features]\nmemories = false\n",
            PreservedText.Read(Path.Combine(TestDir, ".codex/config.toml")));
    }

    [Fact]
    public async Task Join_Codex_BomFeaturesTable_GetsMemoriesInsideItsOneTable()
    {
        (await InitProjectAsync("none")).AssertSuccess();
        WriteFile(".codex/config.toml", "﻿[features]\nother = true\n");

        var result = await JoinProjectAsync("codex");

        result.AssertSuccess();
        Assert.Equal("﻿[features]\nother = true\nmemories = false\n\n[agents]\nmax_depth = 3\nmax_concurrent_threads_per_session = 16\n",
            PreservedText.Read(Path.Combine(TestDir, ".codex/config.toml")));
    }

    [Theory]
    [InlineData("\"03\"")]
    [InlineData("\"+3\"")]
    [InlineData("\" 3\"")]
    [InlineData("\"2\"")]
    [InlineData("3")]
    public async Task Init_ClaudeHostSetting_RejectsNoncanonicalOrIncompatibleValue(string value)
    {
        Directory.CreateDirectory(Path.Combine(TestDir, ".claude"));
        WriteFile(".claude/settings.json", $$"""{ "env": { "CLAUDE_CODE_MAX_SUBAGENT_SPAWN_DEPTH": {{value}} } }""");
        var original = File.ReadAllBytes(Path.Combine(TestDir, ".claude/settings.json"));

        var result = await InitProjectAsync("claude");

        result.AssertExitCode(2);
        result.AssertStderrContains(".claude/settings.json");
        result.AssertStderrContains("CLAUDE_CODE_MAX_SUBAGENT_SPAWN_DEPTH");
        Assert.Equal(original, File.ReadAllBytes(Path.Combine(TestDir, ".claude/settings.json")));
        AssertFileNotExists("dydo.json");
        AssertFileNotExists(".claude/settings.local.json");
    }

    [Theory]
    [InlineData("max_depth = 2\nmax_concurrent_threads_per_session = 16")]
    [InlineData("max_depth = 3\nmax_concurrent_threads_per_session = 15")]
    [InlineData("enabled = false\nmax_depth = 3\nmax_concurrent_threads_per_session = 16")]
    [InlineData("enabled = \"true\"\nmax_depth = 3\nmax_concurrent_threads_per_session = 16")]
    [InlineData("max_depth = \"3\"\nmax_concurrent_threads_per_session = 16")]
    [InlineData("max_depth = 3.0\nmax_concurrent_threads_per_session = 16")]
    [InlineData("max_depth = true\nmax_concurrent_threads_per_session = 16")]
    public async Task Init_CodexHostSetting_RejectsConflictWithoutPartialInit(string agents)
    {
        Directory.CreateDirectory(Path.Combine(TestDir, ".codex"));
        WriteFile(".codex/config.toml", "[agents]\n" + agents + "\n");
        var original = File.ReadAllBytes(Path.Combine(TestDir, ".codex/config.toml"));

        var result = await InitProjectAsync("codex");

        result.AssertExitCode(2);
        result.AssertStderrContains(".codex/config.toml");
        result.AssertStderrContains("agents");
        Assert.Equal(original, File.ReadAllBytes(Path.Combine(TestDir, ".codex/config.toml")));
        AssertFileNotExists("dydo.json");
        AssertFileNotExists(".codex/hooks.json");
        AssertFileNotExists("AGENTS.md");
    }

    [Theory]
    [InlineData("[agents]\nmax_depth = 3\nmax_depth = 3\nmax_concurrent_threads_per_session = 16\n")]
    [InlineData("agents = { max_depth = 3, max_concurrent_threads_per_session = 16 }\n")]
    [InlineData("agents = 3\n")]
    [InlineData("agents = []\n")]
    [InlineData("[[agents]]\nmax_depth = 3\n")]
    [InlineData("agents.max_depth = 3\n")]
    [InlineData("[ agents ]\nmax_depth = 3\nmax_concurrent_threads_per_session = 16\n")]
    [InlineData("[\"agents\"]\nmax_depth = 3\nmax_concurrent_threads_per_session = 16\n")]
    public async Task Init_CodexHostSetting_RejectsAmbiguousTomlBeforePartialInit(string toml)
    {
        Directory.CreateDirectory(Path.Combine(TestDir, ".codex"));
        WriteFile(".codex/config.toml", toml);
        var original = File.ReadAllBytes(Path.Combine(TestDir, ".codex/config.toml"));

        var result = await InitProjectAsync("codex");

        result.AssertExitCode(2);
        result.AssertStderrContains(".codex/config.toml");
        Assert.Equal(original, File.ReadAllBytes(Path.Combine(TestDir, ".codex/config.toml")));
        AssertFileNotExists("dydo.json");
        AssertFileNotExists(".codex/hooks.json");
        AssertFileNotExists("AGENTS.md");
    }

    [Theory]
    [InlineData("{ \"env\": null }")]
    [InlineData("{ \"env\": { \"CLAUDE_CODE_MAX_SUBAGENT_SPAWN_DEPTH\": null } }")]
    public async Task Init_ClaudeHostSetting_RejectsNullManagedMembersBeforePartialInit(string json)
    {
        Directory.CreateDirectory(Path.Combine(TestDir, ".claude"));
        WriteFile(".claude/settings.json", json);
        var original = File.ReadAllBytes(Path.Combine(TestDir, ".claude/settings.json"));

        var result = await InitProjectAsync("claude");

        result.AssertExitCode(2);
        result.AssertStderrContains(".claude/settings.json");
        Assert.Equal(original, File.ReadAllBytes(Path.Combine(TestDir, ".claude/settings.json")));
        AssertFileNotExists("dydo.json");
        AssertFileNotExists(".claude/settings.local.json");
    }

    [Theory]
    [InlineData("claude")]
    [InlineData("codex")]
    public async Task Init_All_PreflightsBothHostsBeforeAnyProjectMutation(string invalidHost)
    {
        Directory.CreateDirectory(Path.Combine(TestDir, ".claude"));
        Directory.CreateDirectory(Path.Combine(TestDir, ".codex"));
        WriteFile(".claude/settings.json", invalidHost == "claude"
            ? "{ \"env\": { \"CLAUDE_CODE_MAX_SUBAGENT_SPAWN_DEPTH\": \"02\" } }"
            : "{ \"env\": { \"custom\": \"preserve\" } }");
        WriteFile(".codex/config.toml", invalidHost == "codex"
            ? "[agents\nmax_depth = 3\n"
            : "# existing\n[agents]\nmax_depth = 3\nmax_concurrent_threads_per_session = 16\n");
        var claude = File.ReadAllBytes(Path.Combine(TestDir, ".claude/settings.json"));
        var codex = File.ReadAllBytes(Path.Combine(TestDir, ".codex/config.toml"));

        var result = await InitProjectAsync("all");

        result.AssertExitCode(2);
        Assert.Equal(claude, File.ReadAllBytes(Path.Combine(TestDir, ".claude/settings.json")));
        Assert.Equal(codex, File.ReadAllBytes(Path.Combine(TestDir, ".codex/config.toml")));
        AssertFileNotExists("dydo.json");
        AssertFileNotExists("CLAUDE.md");
        AssertFileNotExists("AGENTS.md");
        AssertFileNotExists(".claude/settings.local.json");
        AssertFileNotExists(".codex/hooks.json");
        AssertFileNotExists(".gitignore");
    }

    [Fact]
    public async Task Join_All_PreflightFailurePreservesExistingProjectAndBothHosts()
    {
        (await InitProjectAsync("none")).AssertSuccess();
        Directory.CreateDirectory(Path.Combine(TestDir, ".claude"));
        Directory.CreateDirectory(Path.Combine(TestDir, ".codex"));
        WriteFile(".claude/settings.json", "{ \"env\": { \"custom\": \"preserve\" } }");
        WriteFile(".codex/config.toml", "[agents]\nmax_depth = 2\nmax_concurrent_threads_per_session = 16\n");
        // A generated CLAUDE.md that a successful join would remove must survive the failed one.
        WriteFile("CLAUDE.md", ReadFile("AGENTS.md"));
        var config = File.ReadAllBytes(Path.Combine(TestDir, "dydo.json"));
        var claude = File.ReadAllBytes(Path.Combine(TestDir, ".claude/settings.json"));
        var codex = File.ReadAllBytes(Path.Combine(TestDir, ".codex/config.toml"));
        var preserved = Snapshot("AGENTS.md", "CLAUDE.md");

        var result = await JoinProjectAsync("all");

        result.AssertExitCode(2);
        Assert.Equal(config, File.ReadAllBytes(Path.Combine(TestDir, "dydo.json")));
        Assert.Equal(claude, File.ReadAllBytes(Path.Combine(TestDir, ".claude/settings.json")));
        Assert.Equal(codex, File.ReadAllBytes(Path.Combine(TestDir, ".codex/config.toml")));
        AssertUnchanged(preserved);
        AssertFileNotExists(".claude/settings.local.json");
        AssertFileNotExists(".codex/hooks.json");
    }

    [Theory]
    [InlineData("unrelated = true\nunrelated = false\n")]
    [InlineData("unrelated = \"unterminated\n")]
    [InlineData("[agents]\nmax_depth = 9223372036854775808\n")]
    public async Task Init_All_MalformedCodexTomlPreservesBothHostsAndAllSideEffects(string toml)
    {
        Directory.CreateDirectory(Path.Combine(TestDir, ".claude"));
        Directory.CreateDirectory(Path.Combine(TestDir, ".codex"));
        WriteFile(".claude/settings.json", "{ \"env\": { \"custom\": \"preserve\" } }");
        WriteFile(".codex/config.toml", toml);
        var claude = File.ReadAllBytes(Path.Combine(TestDir, ".claude/settings.json"));
        var codex = File.ReadAllBytes(Path.Combine(TestDir, ".codex/config.toml"));

        var result = await InitProjectAsync("all");

        result.AssertExitCode(2);
        result.AssertStderrContains(".codex/config.toml");
        result.AssertStderrContains("malformed");
        Assert.Equal(claude, File.ReadAllBytes(Path.Combine(TestDir, ".claude/settings.json")));
        Assert.Equal(codex, File.ReadAllBytes(Path.Combine(TestDir, ".codex/config.toml")));
        AssertFileNotExists("dydo.json");
        AssertFileNotExists("CLAUDE.md");
        AssertFileNotExists("AGENTS.md");
        AssertFileNotExists(".claude/settings.local.json");
        AssertFileNotExists(".codex/hooks.json");
        AssertFileNotExists(".gitignore");
    }

    [Theory]
    [InlineData("unrelated = true\nunrelated = false\n")]
    [InlineData("unrelated = \"unterminated\n")]
    [InlineData("[agents]\nmax_depth = 9223372036854775808\n")]
    public async Task Join_All_MalformedCodexTomlPreservesBothHostsAndAllSideEffects(string toml)
    {
        (await InitProjectAsync("none")).AssertSuccess();
        Directory.CreateDirectory(Path.Combine(TestDir, ".claude"));
        Directory.CreateDirectory(Path.Combine(TestDir, ".codex"));
        WriteFile(".claude/settings.json", "{ \"env\": { \"custom\": \"preserve\" } }");
        WriteFile(".codex/config.toml", toml);
        var preserved = Snapshot("dydo.json", ".gitignore", "AGENTS.md",
                                 ".claude/settings.json", ".codex/config.toml");

        var result = await JoinProjectAsync("all");

        result.AssertExitCode(2);
        result.AssertStderrContains(".codex/config.toml");
        result.AssertStderrContains("malformed");
        AssertUnchanged(preserved);
        AssertFileNotExists("CLAUDE.md");
        AssertFileNotExists(".claude/settings.local.json");
        AssertFileNotExists(".codex/hooks.json");
    }

    [Theory]
    [InlineData("agents . max_depth = 3\n")]
    [InlineData("\"agents\".max_depth = 3\n")]
    public async Task Init_All_RejectsSemanticDottedAgentsBeforeAnySideEffect(string toml)
    {
        Directory.CreateDirectory(Path.Combine(TestDir, ".claude"));
        Directory.CreateDirectory(Path.Combine(TestDir, ".codex"));
        WriteFile(".claude/settings.json", "{ \"env\": { \"custom\": \"preserve\" } }");
        WriteFile(".codex/config.toml", toml);
        var claude = File.ReadAllBytes(Path.Combine(TestDir, ".claude/settings.json"));
        var codex = File.ReadAllBytes(Path.Combine(TestDir, ".codex/config.toml"));

        var result = await InitProjectAsync("all");

        result.AssertExitCode(2);
        result.AssertStderrContains(".codex/config.toml");
        Assert.Equal(claude, File.ReadAllBytes(Path.Combine(TestDir, ".claude/settings.json")));
        Assert.Equal(codex, File.ReadAllBytes(Path.Combine(TestDir, ".codex/config.toml")));
        AssertFileNotExists("dydo.json");
        AssertFileNotExists("CLAUDE.md");
        AssertFileNotExists("AGENTS.md");
        AssertFileNotExists(".claude/settings.local.json");
        AssertFileNotExists(".codex/hooks.json");
        AssertFileNotExists(".gitignore");
    }

    [Theory]
    [InlineData("agents . max_depth = 3\n")]
    [InlineData("\"agents\".max_depth = 3\n")]
    public async Task Join_All_RejectsSemanticDottedAgentsBeforeAnySideEffect(string toml)
    {
        (await InitProjectAsync("none")).AssertSuccess();
        Directory.CreateDirectory(Path.Combine(TestDir, ".claude"));
        Directory.CreateDirectory(Path.Combine(TestDir, ".codex"));
        WriteFile(".claude/settings.json", "{ \"env\": { \"custom\": \"preserve\" } }");
        WriteFile(".codex/config.toml", toml);
        var preserved = Snapshot("dydo.json", ".gitignore", "AGENTS.md",
                                 ".claude/settings.json", ".codex/config.toml");

        var result = await JoinProjectAsync("all");

        result.AssertExitCode(2);
        result.AssertStderrContains(".codex/config.toml");
        AssertUnchanged(preserved);
        AssertFileNotExists("CLAUDE.md");
        AssertFileNotExists(".claude/settings.local.json");
        AssertFileNotExists(".codex/hooks.json");
    }

    [Fact]
    public async Task Init_Codex_PreservesUnrelatedAgentsHistoryArrayAndRepeatBytes()
    {
        Directory.CreateDirectory(Path.Combine(TestDir, ".codex"));
        WriteFile(".codex/config.toml", "[[agents_history]]\nname = \"retain\"\n");

        (await InitProjectAsync("codex")).AssertSuccess();
        var config = File.ReadAllBytes(Path.Combine(TestDir, ".codex/config.toml"));

        (await JoinProjectAsync("codex")).AssertSuccess();
        Assert.Equal(config, File.ReadAllBytes(Path.Combine(TestDir, ".codex/config.toml")));
        Assert.Contains("[[agents_history]]", ReadFile(".codex/config.toml"));
    }

    [Theory]
    [InlineData("3", "16")]
    [InlineData("+3", "+16")]
    [InlineData("0x3", "0x10")]
    [InlineData("0o3", "0o20")]
    [InlineData("0b11", "0b10000")]
    public async Task Init_Codex_RetainsIntegerAgentCapacityValuesAndRepeatBytes(string depth, string concurrency)
    {
        Directory.CreateDirectory(Path.Combine(TestDir, ".codex"));
        WriteFile(".codex/config.toml", $"[agents]\nmax_depth = {depth}\nmax_concurrent_threads_per_session = {concurrency}\n\n[features]\nmemories = false\n");
        var original = File.ReadAllBytes(Path.Combine(TestDir, ".codex/config.toml"));

        (await InitProjectAsync("codex")).AssertSuccess();
        Assert.Equal(original, File.ReadAllBytes(Path.Combine(TestDir, ".codex/config.toml")));
        (await JoinProjectAsync("codex")).AssertSuccess();
        Assert.Equal(original, File.ReadAllBytes(Path.Combine(TestDir, ".codex/config.toml")));
    }

    [Fact]
    public async Task Init_Codex_HooksContainPreToolUseDydoGuard()
    {
        var result = await InitProjectAsync("codex");

        result.AssertSuccess();

        var settings = Assert.IsType<JsonObject>(JsonNode.Parse(ReadFile(".codex/hooks.json")));
        Assert.Null(settings["PreToolUse"]);

        var hooks = Assert.IsType<JsonObject>(settings["hooks"]);
        var preToolUse = Assert.IsType<JsonArray>(hooks["PreToolUse"]);
        var preToolUseEntry = Assert.Single(preToolUse);
        Assert.NotNull(preToolUseEntry);
        var matcher = preToolUseEntry["matcher"]?.GetValue<string>();
        // The documented Codex tool names first, then the shell names from issue 0295 (without
        // them the guard never sees Codex shell commands, so off-limits and dangerous-bash don't
        // bind on that lane). Claude-only names are left out: Codex never emits them.
        Assert.Equal("Bash|apply_patch|Edit|Write|Agent|shell_command|exec|local_shell|unified_exec", matcher);

        var guardHooks = Assert.IsType<JsonArray>(preToolUseEntry["hooks"]);
        var guardHook = Assert.Single(guardHooks);
        Assert.NotNull(guardHook);
        Assert.Equal("command", guardHook["type"]?.GetValue<string>());
        Assert.Equal("dydo guard", guardHook["command"]?.GetValue<string>());

        Assert.False(hooks.ContainsKey("Stop"));
    }

    [Fact]
    public async Task Init_Codex_PreservesExistingHooks()
    {
        Directory.CreateDirectory(Path.Combine(TestDir, ".codex"));
        WriteFile(".codex/hooks.json", CustomHooks);

        var result = await InitProjectAsync("codex");

        result.AssertSuccess();
        var content = ReadFile(".codex/hooks.json");
        Assert.Contains("CustomTool", content);
        Assert.Contains("echo custom", content);
        Assert.Contains("PostToolUse", content);
        Assert.Contains("echo post", content);
        Assert.Contains("otherSetting", content);
        Assert.Contains("dydo guard", content);
        Assert.DoesNotContain("dydo guard --stop", content);
    }

    [Fact]
    public async Task Init_Codex_PreservesCustomHooksWhenRemovingDydoHooks()
    {
        Directory.CreateDirectory(Path.Combine(TestDir, ".codex"));
        WriteFile(".codex/hooks.json", """
            {
              "hooks": {
                "PreToolUse": [
                  {
                    "matcher": "CustomSubstring",
                    "hooks": [
                      {
                        "type": "command",
                        "command": "echo before dydo guard after"
                      }
                    ]
                  },
                  {
                    "matcher": "Mixed",
                    "hooks": [
                      {
                        "type": "command",
                        "command": "dydo guard"
                      },
                      {
                        "type": "command",
                        "command": "echo custom"
                      }
                    ]
                  },
                  {
                    "matcher": "ManagedOnly",
                    "hooks": [
                      {
                        "type": "command",
                        "command": "dydo guard"
                      }
                    ]
                  }
                ],
                "Stop": [
                  {
                    "hooks": [
                      {
                        "type": "command",
                        "command": "dydo guard --stop"
                      },
                      {
                        "type": "command",
                        "command": "echo stop custom"
                      }
                    ]
                  },
                  {
                    "hooks": [
                      {
                        "type": "command",
                        "command": "echo before dydo guard --stop after"
                      }
                    ]
                  }
                ]
              }
            }
            """);

        var result = await InitProjectAsync("codex");

        result.AssertSuccess();

        var settings = Assert.IsType<JsonObject>(JsonNode.Parse(ReadFile(".codex/hooks.json")));
        var hooks = Assert.IsType<JsonObject>(settings["hooks"]);
        var preToolUse = Assert.IsType<JsonArray>(hooks["PreToolUse"]);
        Assert.Contains(preToolUse, entry =>
            entry?["matcher"]?.GetValue<string>() == "CustomSubstring" &&
            HookTestAssertions.Commands(entry).Contains("echo before dydo guard after"));
        Assert.Contains(preToolUse, entry =>
            entry?["matcher"]?.GetValue<string>() == "Mixed" &&
            HookTestAssertions.Commands(entry).SequenceEqual(["echo custom"]));
        Assert.DoesNotContain(preToolUse, entry =>
            entry?["matcher"]?.GetValue<string>() == "ManagedOnly");
        Assert.Equal(1, HookTestAssertions.Count(preToolUse, "dydo guard"));

        var stop = Assert.IsType<JsonArray>(hooks["Stop"]);
        Assert.Contains(stop, entry => HookTestAssertions.Commands(entry).SequenceEqual(["echo stop custom"]));
        Assert.Contains(stop, entry => HookTestAssertions.Commands(entry).Contains("echo before dydo guard --stop after"));
        Assert.Equal(0, HookTestAssertions.Count(stop, "dydo guard --stop"));
    }

    [Fact]
    public async Task Init_Claude_PreservesExistingHooks()
    {
        Directory.CreateDirectory(Path.Combine(TestDir, ".claude"));
        WriteFile(".claude/settings.local.json", CustomHooks);
        WriteFile(".claude/settings.json", CustomHooks);

        var result = await InitProjectAsync("claude");

        result.AssertSuccess();
        var local = ReadFile(".claude/settings.local.json");
        PreservedText.AssertLinesKeptInOrder(CustomHooks, local);
        Assert.DoesNotContain("dydo guard", local);

        var content = ReadFile(".claude/settings.json");
        Assert.Contains("CustomTool", content);
        Assert.Contains("echo custom", content);

        Assert.Contains("dydo guard", content);
        Assert.Contains("Edit|Write|Read|Bash", content);

        Assert.Contains("PostToolUse", content);
        Assert.Contains("echo post", content);

        Assert.Contains("otherSetting", content);
    }

    [Fact]
    public async Task Join_Claude_MovesDydosGuardOutOfTheLocalSettings_KeepingForeignHooks()
    {
        (await InitProjectAsync("none")).AssertSuccess();
        WriteFile(".claude/settings.local.json", """
            {
              "hooks": {
                "PreToolUse": [
                  { "matcher": "Edit|AskUserQuestion", "hooks": [ { "type": "command", "command": "dydo guard" } ] },
                  { "matcher": "Bash", "hooks": [ { "type": "command", "command": "dydo guard --host claude" }, { "type": "command", "command": "echo mine" } ] }
                ],
                "Stop": [ { "hooks": [ { "type": "command", "command": "dydo guard --stop" } ] } ]
              }
            }
            """);

        (await JoinProjectAsync("claude")).AssertSuccess();

        var local = Assert.IsType<JsonObject>(JsonNode.Parse(ReadFile(".claude/settings.local.json")));
        var localHooks = Assert.IsType<JsonObject>(local["hooks"]);
        Assert.False(localHooks.ContainsKey("Stop"));
        var localPreToolUse = Assert.IsType<JsonArray>(localHooks["PreToolUse"]);
        Assert.Equal(["echo mine"], HookTestAssertions.Commands(Assert.Single(localPreToolUse)));
        Assert.Contains("Bash(dydo:*)", ReadFile(".claude/settings.local.json"));

        var committed = Assert.IsType<JsonArray>(JsonNode.Parse(ReadFile(".claude/settings.json"))?["hooks"]?["PreToolUse"]);
        Assert.Equal(1, HookTestAssertions.Count(committed, "dydo guard --host claude"));
    }

    [Fact]
    public async Task Join_Claude_LeavesExactlyOneDydoGuardInTheCommittedSettings()
    {
        (await InitProjectAsync("none")).AssertSuccess();
        WriteFile(".claude/settings.json", """
            {
              "hooks": {
                "PreToolUse": [
                  { "matcher": "Edit|AskUserQuestion", "hooks": [ { "type": "command", "command": "dydo guard" } ] },
                  { "matcher": "Bash", "hooks": [ { "type": "command", "command": "echo mine" } ] },
                  { "matcher": "Edit", "hooks": [ { "type": "command", "command": "dydo guard --host claude" } ] }
                ],
                "Stop": [ { "hooks": [ { "type": "command", "command": "dydo guard --stop" } ] } ]
              }
            }
            """);

        (await JoinProjectAsync("claude")).AssertSuccess();

        var content = ReadFile(".claude/settings.json");
        var hooks = Assert.IsType<JsonObject>(JsonNode.Parse(content)?["hooks"]);
        Assert.False(hooks.ContainsKey("Stop"));
        var preToolUse = Assert.IsType<JsonArray>(hooks["PreToolUse"]);
        Assert.Equal(2, preToolUse.Count);
        var guard = Assert.Single(preToolUse, entry => HookTestAssertions.Commands(entry).Contains("dydo guard --host claude"));
        Assert.Equal("Edit|Write|Read|Bash|Glob|Grep|Agent|EnterPlanMode|ExitPlanMode|PowerShell|NotebookEdit",
            guard?["matcher"]?.GetValue<string>());
        Assert.Equal(["dydo guard --host claude"], HookTestAssertions.Commands(guard));
        Assert.Equal(0, HookTestAssertions.Count(preToolUse, "dydo guard"));
        Assert.Contains("{ \"matcher\": \"Bash\", \"hooks\": [ { \"type\": \"command\", \"command\": \"echo mine\" } ] }", content);
    }

    private const string ClaudeGuard = "dydo guard --host claude";
    private const string CodexGuard = "dydo guard";

    private JsonArray JoinedPreToolUse(string file, string guard)
    {
        var preToolUse = Assert.IsType<JsonArray>(JsonNode.Parse(ReadFile(file))?["hooks"]?["PreToolUse"]);
        Assert.Equal(1, HookTestAssertions.Count(preToolUse, guard));
        Assert.Equal(1, HookTestAssertions.Count(preToolUse, CodexGuard) + HookTestAssertions.Count(preToolUse, ClaudeGuard));
        return preToolUse;
    }

    [Theory]
    [InlineData("claude", ".claude/settings.json", ClaudeGuard)]
    [InlineData("codex", ".codex/hooks.json", CodexGuard)]
    public async Task Join_MixedGuardEntry_KeepsTheForeignHookBytes(string host, string file, string guard)
    {
        (await InitProjectAsync("none")).AssertSuccess();
        WriteFile(file, """{"hooks":{"PreToolUse":[{"matcher":"Bash","hooks":[{"type":"command","command":"dydo guard"},{"type":"command","command":"echo it's"}]}]}}""");

        (await JoinProjectAsync(host)).AssertSuccess();

        Assert.Contains("""{"matcher":"Bash","hooks":[{"type":"command","command":"echo it's"}]}""", ReadFile(file));
        Assert.Equal(2, JoinedPreToolUse(file, guard).Count);
    }

    [Theory]
    [InlineData("claude", ".claude/settings.json", ClaudeGuard)]
    [InlineData("codex", ".codex/hooks.json", CodexGuard)]
    public async Task Join_GuardDuplicatesAroundForeignEntries_KeepTheForeignEntriesBytes(string host, string file, string guard)
    {
        (await InitProjectAsync("none")).AssertSuccess();
        WriteFile(file, """{"hooks":{"PreToolUse":[{"matcher":"Edit","hooks":[{"type":"command","command":"dydo guard"}]}, {"matcher":"Bash", "hooks":[ {"type":"command","command":"echo it's"} ]},{"matcher":"Mixed","hooks":[{"type":"command","command":"dydo guard --host claude"},{"type":"command","command":"echo  two"}]},{"matcher":"Edit","hooks":[{"type":"command","command":"dydo guard"}]}]}}""");

        (await JoinProjectAsync(host)).AssertSuccess();

        var content = ReadFile(file);
        Assert.Contains("""{"matcher":"Bash", "hooks":[ {"type":"command","command":"echo it's"} ]}""", content);
        Assert.Contains("""{"matcher":"Mixed","hooks":[{"type":"command","command":"echo  two"}]}""", content);
        var preToolUse = JoinedPreToolUse(file, guard);
        Assert.Equal(3, preToolUse.Count);
        Assert.Equal([guard], HookTestAssertions.Commands(preToolUse[0]));
    }

    [Theory]
    [InlineData("claude", ".claude/settings.json", ClaudeGuard)]
    [InlineData("codex", ".codex/hooks.json", CodexGuard)]
    public async Task Join_AlreadyEmptyForeignEntry_StaysBeforeTheGuard(string host, string file, string guard)
    {
        (await InitProjectAsync("none")).AssertSuccess();
        WriteFile(file, """{"hooks":{"PreToolUse":[{"matcher":"Bash","hooks":[]},{"matcher":"Edit","hooks":[{"type":"command","command":"dydo guard"}]}]}}""");

        (await JoinProjectAsync(host)).AssertSuccess();

        Assert.Contains("""{"matcher":"Bash","hooks":[]}""", ReadFile(file));
        var preToolUse = JoinedPreToolUse(file, guard);
        Assert.Equal(2, preToolUse.Count);
        Assert.Empty(HookTestAssertions.Commands(preToolUse[0]));
        Assert.Equal([guard], HookTestAssertions.Commands(preToolUse[1]));
    }

    [Fact]
    public async Task Join_Claude_KeepsAnAlreadyEmptyForeignEntryInTheLocalSettings()
    {
        (await InitProjectAsync("none")).AssertSuccess();
        WriteFile(".claude/settings.local.json", """{"hooks":{"PreToolUse":[{"matcher":"Bash","hooks":[]},{"matcher":"Edit","hooks":[{"type":"command","command":"dydo guard"}]}]}}""");

        (await JoinProjectAsync("claude")).AssertSuccess();

        var local = ReadFile(".claude/settings.local.json");
        Assert.Contains("""{"matcher":"Bash","hooks":[]}""", local);
        Assert.DoesNotContain("dydo guard", local);
    }

    // A malformed hooks file starts fresh; one root followed by another is malformed.
    [Theory]
    [InlineData("claude", ".claude/settings.local.json")]
    [InlineData("codex", ".codex/hooks.json")]
    public async Task Join_TrailingRootInAHooksFile_StartsItFresh(string host, string file)
    {
        (await InitProjectAsync("none")).AssertSuccess();
        WriteFile(file, "{} {}");

        (await JoinProjectAsync(host)).AssertSuccess();

        Assert.IsType<JsonObject>(JsonNode.Parse(ReadFile(file)));
    }

    [Fact]
    public async Task Join_Claude_TrailingRootInTheCommittedSettings_IsRefusedAndLeftUntouched()
    {
        (await InitProjectAsync("none")).AssertSuccess();
        WriteFile(".claude/settings.json", "{} {}");

        var result = await JoinProjectAsync("claude");

        Assert.NotEqual(0, result.ExitCode);
        result.AssertStderrContains("malformed");
        Assert.Equal("{} {}", ReadFile(".claude/settings.json"));
    }

    [Fact]
    [Trait("Job", "trap: a tool the Claude matcher does not name bypasses every guard layer (PowerShell, NotebookEdit, Agent)")]
    public async Task Init_Claude_GuardMatcherNamesEveryGuardedTool()
    {
        var result = await InitProjectAsync("claude");

        result.AssertSuccess();
        var settings = Assert.IsType<JsonObject>(JsonNode.Parse(ReadFile(".claude/settings.json")));
        var entry = Assert.Single(Assert.IsType<JsonArray>(settings["hooks"]?["PreToolUse"]));
        var matched = entry?["matcher"]?.GetValue<string>().Split('|') ?? [];
        Assert.All(new[] { "Edit", "Write", "Read", "Bash", "Glob", "Grep", "Agent", "PowerShell", "NotebookEdit" },
            tool => Assert.Contains(tool, matched));
    }

    [Fact]
    public async Task Init_Claude_GuardHookCarriesTheHostMarker()
    {
        var result = await InitProjectAsync("claude");

        result.AssertSuccess();
        var settings = Assert.IsType<JsonObject>(JsonNode.Parse(ReadFile(".claude/settings.json")));
        var preToolUse = Assert.IsType<JsonArray>(settings["hooks"]?["PreToolUse"]);
        Assert.Equal(1, HookTestAssertions.Count(preToolUse, "dydo guard --host claude"));
        Assert.Equal(0, HookTestAssertions.Count(preToolUse, "dydo guard"));
    }

    [Fact]
    public async Task Init_Claude_ReplacesAnUnmarkedGuardHook()
    {
        Directory.CreateDirectory(Path.Combine(TestDir, ".claude"));
        WriteFile(".claude/settings.json", """
            {
              "hooks": {
                "PreToolUse": [
                  { "matcher": "Edit", "hooks": [ { "type": "command", "command": "dydo guard" } ] }
                ]
              }
            }
            """);

        var result = await InitProjectAsync("claude");

        result.AssertSuccess();
        var settings = Assert.IsType<JsonObject>(JsonNode.Parse(ReadFile(".claude/settings.json")));
        var preToolUse = Assert.IsType<JsonArray>(settings["hooks"]?["PreToolUse"]);
        Assert.Equal(0, HookTestAssertions.Count(preToolUse, "dydo guard"));
        Assert.Equal(1, HookTestAssertions.Count(preToolUse, "dydo guard --host claude"));
    }

    [Fact]
    public async Task Init_All_OnlyTheClaudeHookCarriesTheHostMarker()
    {
        var result = await InitProjectAsync("all");

        result.AssertSuccess();
        Assert.Contains("\"dydo guard --host claude\"", ReadFile(".claude/settings.json"));
        Assert.DoesNotContain("--host", ReadFile(".codex/hooks.json"));
    }

    // dydo-commands-core.md, dydo init's host table: what each integration writes, and a repeat join
    // that changes no byte of any of it.
    [Theory]
    [Trait("Job", "contract: dydo-commands-core.md dydo init host table (files per host; --join rewrites nothing it already wrote)")]
    [InlineData("none")]
    [InlineData("claude")]
    [InlineData("codex")]
    [InlineData("all")]
    public async Task Init_WritesEachHostsFiles_AndARepeatJoinChangesNoByte(string integration)
    {
        var claude = integration is "claude" or "all";
        var codex = integration is "codex" or "all";

        var init = await InitProjectAsync(integration);
        var written = AllFiles();
        var join = await JoinProjectAsync(integration);

        Assert.Equal(0, init.ExitCode);
        Assert.Equal(0, join.ExitCode);
        Assert.Equal(written, AllFiles());
        var gitignore = ReadFile(".gitignore").Split('\n').Select(line => line.Trim()).ToList();
        foreach (var entry in new[] { "dydo/agents/", "dydo/_system/.local/", "/.claude/skills/", "/.agents/skills/" })
            Assert.Equal(1, gitignore.Count(line => line == entry));
        Assert.Equal(claude ? 1 : 0, gitignore.Count(line => line == ".claude/settings.local.json"));
        Assert.True(File.Exists(Path.Combine(TestDir, "AGENTS.md")));
        Assert.False(File.Exists(Path.Combine(TestDir, "CLAUDE.md")));
        var integrations = ReadConfigJson()["integrations"]?.AsObject();
        Assert.Equal(claude, integrations?["claude"]?.GetValue<bool>() ?? false);
        Assert.Equal(codex, integrations?["codex"]?.GetValue<bool>() ?? false);

        Assert.Equal(claude, File.Exists(Path.Combine(TestDir, ".claude", "settings.json")));
        if (claude)
        {
            Assert.Contains("dydo guard --host claude", ReadFile(".claude/settings.json"));
            Assert.DoesNotContain("dydo guard", ReadFile(".claude/settings.local.json"));
            var allow = AllowEntries();
            foreach (var entry in DydoAllowEntries)
                Assert.Equal(1, allow.Count(value => value == entry));
        }
        else
        {
            Assert.False(File.Exists(Path.Combine(TestDir, ".claude", "settings.local.json")));
        }

        Assert.Equal(codex, File.Exists(Path.Combine(TestDir, ".codex", "hooks.json")));
        Assert.Equal(codex, File.Exists(Path.Combine(TestDir, ".codex", "rules", "dydo.rules")));
        if (codex)
            Assert.DoesNotContain("--host", ReadFile(".codex/hooks.json"));
    }

    private static readonly string[] DydoAllowEntries =
    [
        "Bash(dydo:*)", "PowerShell(dydo:*)", "Bash(dydo clean:*)", "PowerShell(dydo clean:*)",
        "Bash(dydo merge:*)", "PowerShell(dydo merge:*)"
    ];

    [Theory]
    [Trait("Job", "contract: dydo-commands-core.md dydo init (settings.local.json carries the allow entries); trap: existing local settings are merged, not replaced")]
    [InlineData("""{ "permissions": { "allow": [ "Bash(git:*)", "Read(**)" ] } }""", "Bash(git:*)", "Read(**)")]
    [InlineData("""{ "hooks": {} }""")]
    public async Task Init_Claude_MergesTheAllowListIntoExistingLocalSettings(string existing, params string[] kept)
    {
        WriteFile(".claude/settings.local.json", existing);

        (await InitProjectAsync("claude")).AssertSuccess();

        var allow = AllowEntries();
        Assert.All(kept, entry => Assert.Contains(entry, allow));
        Assert.All(DydoAllowEntries, entry => Assert.Equal(1, allow.Count(value => value == entry)));
    }

    // Issue 0300 and 0303: joining a second host records both and gitignores what the new host needs.
    [Theory]
    [Trait("Job", "trap: issue 0300 (a joined second host replaced the first in dydo.json); trap: issue 0303 (the Claude local settings rode a commit)")]
    [InlineData("claude", "codex")]
    [InlineData("codex", "claude")]
    [InlineData("none", "all")]
    public async Task Join_SecondHost_RecordsBothAndWiresTheNewOne(string first, string second)
    {
        (await InitProjectAsync(first)).AssertSuccess();

        (await JoinProjectAsync(second)).AssertSuccess();

        var integrations = ReadConfigJson()["integrations"]!.AsObject();
        Assert.True(integrations["claude"]!.GetValue<bool>());
        Assert.True(integrations["codex"]!.GetValue<bool>());
        Assert.True(File.Exists(Path.Combine(TestDir, ".codex", "hooks.json")));
        Assert.True(File.Exists(Path.Combine(TestDir, ".claude", "settings.json")));
        Assert.Contains(".claude/settings.local.json", ReadFile(".gitignore"));
    }

    [Theory]
    [Trait("Job", "contract: dydo-commands-core.md dydo init (exit 2: unknown integration, --join outside a project, a project already initialised)")]
    [InlineData(false, "none", "--join")]
    [InlineData(false, "invalid")]
    [InlineData(true, "none")]
    public async Task Init_Refused_ExitsTwoAndOverwritesNothing(bool initialised, params string[] args)
    {
        if (initialised)
        {
            await InitProjectAsync("none");
            WriteFile("dydo/understand/architecture.md", "# Custom Architecture\n");
        }
        var before = AllFiles();

        var result = await RunAsync(InitCommand.Create(), args);

        Assert.Equal(2, result.ExitCode);
        Assert.Equal(before, AllFiles());
    }

    [Theory]
    [Trait("Job", "contract: dydo-commands-core.md dydo init codex row (.codex/rules/dydo.rules allows dydo archive, clean and merge under dydo or dydo.exe)")]
    [InlineData("codex")]
    [InlineData("all")]
    public async Task Init_Codex_WritesTheExecPolicyRuleForDydoAndDydoExe(string integration)
    {
        (await InitProjectAsync(integration)).AssertSuccess();

        var rules = ReadFile(".codex/rules/dydo.rules");
        Assert.Contains("pattern = [[\"dydo\", \"dydo.exe\"], \"merge\"]", rules);
        Assert.Contains("pattern = [\"dydo\", \"clean\"]", rules);
        Assert.Contains("decision = \"allow\"", rules);
    }

    [Fact]
    public async Task Join_Codex_RestoresTheDydoCleanRuleIdempotently()
    {
        (await InitProjectAsync("none")).AssertSuccess();
        (await JoinProjectAsync("codex")).AssertSuccess();
        WriteFile(".codex/rules/dydo.rules", "stale\n");

        (await JoinProjectAsync("codex")).AssertSuccess();
        (await JoinProjectAsync("codex")).AssertSuccess();

        Assert.Equal(InitCommand.CodexRules, ReadFile(".codex/rules/dydo.rules"));
    }

    [Fact]
    public async Task Join_Codex_LeavesACrlfRuleWithTheSameContentByteForByte()
    {
        (await InitProjectAsync("none")).AssertSuccess();
        (await JoinProjectAsync("codex")).AssertSuccess();
        WriteFile(".codex/rules/dydo.rules", InitCommand.CodexRules.ReplaceLineEndings("\r\n"));
        var preserved = Snapshot(".codex/rules/dydo.rules");

        (await JoinProjectAsync("codex")).AssertSuccess();

        AssertUnchanged(preserved);
    }

    [Fact]
    public async Task Join_Codex_RewritesAStaleCrlfRuleWithCrlf()
    {
        (await InitProjectAsync("none")).AssertSuccess();
        (await JoinProjectAsync("codex")).AssertSuccess();
        WriteFile(".codex/rules/dydo.rules", "stale\r\nrule\r\n");

        (await JoinProjectAsync("codex")).AssertSuccess();

        Assert.Equal(InitCommand.CodexRules.ReplaceLineEndings("\r\n"), ReadFile(".codex/rules/dydo.rules"));
    }

    #endregion

    #region Entry file

    [Fact]
    public async Task Join_Claude_RemovesClaudeMdIdenticalToAgentsMd()
    {
        (await InitProjectAsync("none")).AssertSuccess();
        WriteFile("AGENTS.md", "# Custom entry\n\nSee dydo/index.md.\n");
        WriteFile("CLAUDE.md", "# Custom entry\r\n\r\nSee dydo/index.md.\r\n\r\n");

        var result = await JoinProjectAsync("claude");

        result.AssertSuccess();
        AssertFileNotExists("CLAUDE.md");
        Assert.Equal("# Custom entry\n\nSee dydo/index.md.\n", ReadFile("AGENTS.md"));
    }

    [Fact]
    public async Task Join_Claude_RemovesClaudeMdMatchingTheTemplateEvenWhenAgentsMdDiffers()
    {
        (await InitProjectAsync("none")).AssertSuccess();
        WriteFile("AGENTS.md", "# Custom entry\n");
        WriteFile("CLAUDE.md", ScaffoldTree.EntryPoint(Path.GetFileName(TestDir)) + "\n");

        var result = await JoinProjectAsync("claude");

        result.AssertSuccess();
        AssertFileNotExists("CLAUDE.md");
        Assert.Equal("# Custom entry\n", ReadFile("AGENTS.md"));
    }

    [Fact]
    public async Task Join_Claude_KeepsUserWrittenClaudeMdAndWarns()
    {
        (await InitProjectAsync("none")).AssertSuccess();
        WriteFile("CLAUDE.md", "# My own rules\n\nAlways run the tests.\n");
        var preserved = Snapshot("CLAUDE.md");

        var result = await JoinProjectAsync("claude");

        result.AssertSuccess();
        AssertUnchanged(preserved);
        result.AssertStdoutContains("CLAUDE.md hides AGENTS.md from Claude Code");
        result.AssertStdoutContains("@AGENTS.md");
    }

    public static TheoryData<string, string> ReleasedClaudeMds => new()
    {
        { "3.5 base tree, LF", ReleasedEntryTexts.Generated("Old Clone", ReleasedEntryTexts.V3_5Base, "\n") },
        { "3.5 base tree, CRLF", ReleasedEntryTexts.Generated("Old Clone", ReleasedEntryTexts.V3_5Base, "\r\n") },
        { "v3.0.0..v3.4.1", ReleasedEntryTexts.Generated("Old Clone", ReleasedEntryTexts.V3_0, "\n") + "\n" },
        { "v2.1.0..v2.2.3, CRLF", ReleasedEntryTexts.Generated("Old Clone", ReleasedEntryTexts.V2_1, "\r\n") },
        { "v2.2.4..v2.2.5", ReleasedEntryTexts.Generated("Old Clone", ReleasedEntryTexts.V2_2_4, "\n") },
        { "v2.2.6..v2.2.9", ReleasedEntryTexts.Generated("Old Clone", ReleasedEntryTexts.V2_2_6, "\n") },
        { "v1.1.0..v2.0.15", ReleasedEntryTexts.Generated("Old Clone", ReleasedEntryTexts.V1, "\n") },
    };

    [Theory]
    [MemberData(nameof(ReleasedClaudeMds))]
    public async Task Join_Claude_RemovesClaudeMdAnOlderReleaseGenerated(string release, string claudeMd)
    {
        (await InitProjectAsync("none")).AssertSuccess();
        WriteFile("CLAUDE.md", claudeMd);

        var result = await JoinProjectAsync("claude");

        result.AssertSuccess();
        Assert.False(File.Exists(Path.Combine(TestDir, "CLAUDE.md")), $"CLAUDE.md from {release} was kept");
    }

    [Fact]
    public async Task Join_Claude_KeepsCustomisedCopyOfAnOlderReleaseAndWarns()
    {
        (await InitProjectAsync("none")).AssertSuccess();
        WriteFile("CLAUDE.md", ReleasedEntryTexts.Generated("Old Clone", ReleasedEntryTexts.V3_0 + "\n\nAlways run the tests.", "\n"));
        var preserved = Snapshot("CLAUDE.md");

        var result = await JoinProjectAsync("claude");

        result.AssertSuccess();
        AssertUnchanged(preserved);
        result.AssertStdoutContains("CLAUDE.md hides AGENTS.md from Claude Code");
    }

    [Fact]
    public async Task Join_Claude_KeepsReleasedBodyUnderANonTitleFirstLine()
    {
        (await InitProjectAsync("none")).AssertSuccess();
        WriteFile("CLAUDE.md", ("Notes for Claude\n\n" + ReleasedEntryTexts.V3_0).ReplaceLineEndings("\n"));
        var preserved = Snapshot("CLAUDE.md");

        var result = await JoinProjectAsync("claude");

        result.AssertSuccess();
        AssertUnchanged(preserved);
        result.AssertStdoutContains("CLAUDE.md hides AGENTS.md from Claude Code");
    }

    [Fact]
    public async Task Init_Claude_KeepsUserWrittenClaudeMdAndWarns()
    {
        WriteFile("CLAUDE.md", "# My own rules\n");
        var preserved = Snapshot("CLAUDE.md");

        var result = await InitProjectAsync("claude");

        result.AssertSuccess();
        AssertUnchanged(preserved);
        result.AssertStdoutContains("CLAUDE.md hides AGENTS.md from Claude Code");
    }

    [Theory]
    [InlineData("CLAUDE.local.md", "CLAUDE.local.md stops Claude Code reading AGENTS.md")]
    [InlineData(".claude/CLAUDE.md", ".claude/CLAUDE.md stops Claude Code reading AGENTS.md. Put @../AGENTS.md at its top.")]
    public async Task Join_Claude_WarnsAboutOtherFilesThatHideAgentsMd(string path, string warning)
    {
        (await InitProjectAsync("none")).AssertSuccess();
        WriteFile(path, ReadFile("AGENTS.md"));
        var preserved = Snapshot(path);

        var result = await JoinProjectAsync("claude");

        result.AssertSuccess();
        AssertUnchanged(preserved);
        result.AssertStdoutContains(warning);
    }

    [Theory]
    [InlineData("CLAUDE.md", "\n  @AGENTS.md  \n\n# Notes for Claude\n")]
    [InlineData("CLAUDE.md", "\uFEFF@AGENTS.md\r\n\r\nAlways run the tests.\r\n")]
    [InlineData("CLAUDE.local.md", "@AGENTS.md\n\nMy local notes.\n")]
    [InlineData(".claude/CLAUDE.md", "@../AGENTS.md\n")]
    public async Task Join_Claude_DoesNotWarnAboutAnEntryFileThatImportsAgentsMdFirst(string path, string content)
    {
        (await InitProjectAsync("none")).AssertSuccess();
        WriteFile(path, content);
        var preserved = Snapshot(path);

        var result = await JoinProjectAsync("claude");

        result.AssertSuccess();
        AssertUnchanged(preserved);
        Assert.DoesNotContain("hides AGENTS.md", result.Stdout);
        Assert.DoesNotContain("stops Claude Code reading AGENTS.md", result.Stdout);
    }

    [Theory]
    [InlineData("CLAUDE.md", "# Notes for Claude\n\n@AGENTS.md\n", "CLAUDE.md hides AGENTS.md from Claude Code")]
    [InlineData("CLAUDE.local.md", "@../AGENTS.md\n", "CLAUDE.local.md stops Claude Code reading AGENTS.md")]
    [InlineData(".claude/CLAUDE.md", "@AGENTS.md\n", ".claude/CLAUDE.md stops Claude Code reading AGENTS.md")]
    public async Task Join_Claude_WarnsAboutAnEntryFileWithoutTheLeadingImport(string path, string content, string warning)
    {
        (await InitProjectAsync("none")).AssertSuccess();
        WriteFile(path, content);
        var preserved = Snapshot(path);

        var result = await JoinProjectAsync("claude");

        result.AssertSuccess();
        AssertUnchanged(preserved);
        result.AssertStdoutContains(warning);
    }

    [Fact]
    public async Task Join_Codex_LeavesClaudeMdAlone()
    {
        (await InitProjectAsync("none")).AssertSuccess();
        WriteFile("CLAUDE.md", ReadFile("AGENTS.md"));
        var preserved = Snapshot("CLAUDE.md");

        var result = await JoinProjectAsync("codex");

        result.AssertSuccess();
        AssertUnchanged(preserved);
        Assert.DoesNotContain("CLAUDE.md", result.Stdout);
    }

    [Fact]
    public async Task Join_Claude_MigratesLegacyClaudeOnlyProject()
    {
        await MakeLegacyClaudeOnlyProjectAsync();

        var result = await JoinProjectAsync("claude");

        result.AssertSuccess();
        AssertFileExists("AGENTS.md");
        Assert.Equal(ScaffoldTree.EntryPoint(Path.GetFileName(TestDir)), ReadFile("AGENTS.md"));
        AssertFileNotExists("CLAUDE.md");
    }

    [Fact]
    public async Task Join_All_FailedPreflightKeepsLegacyClaudeMdAndCreatesNoAgentsMd()
    {
        await MakeLegacyClaudeOnlyProjectAsync();
        Directory.CreateDirectory(Path.Combine(TestDir, ".codex"));
        WriteFile(".codex/config.toml", "[agents\nmax_depth = 3\n");
        var preserved = Snapshot("CLAUDE.md");

        var result = await JoinProjectAsync("all");

        result.AssertExitCode(2);
        AssertUnchanged(preserved);
        AssertFileNotExists("AGENTS.md");
    }

    private async Task MakeLegacyClaudeOnlyProjectAsync()
    {
        (await InitProjectAsync("none")).AssertSuccess();
        var generated = ReadFile("AGENTS.md");
        File.Delete(Path.Combine(TestDir, "AGENTS.md"));
        WriteFile("CLAUDE.md", generated);
    }

    #endregion

    private Dictionary<string, byte[]> Snapshot(params string[] paths) =>
        paths.ToDictionary(path => path, path => File.ReadAllBytes(Path.Combine(TestDir, path)));

    private void AssertUnchanged(Dictionary<string, byte[]> snapshot)
    {
        foreach (var (path, expected) in snapshot)
            Assert.Equal(expected, File.ReadAllBytes(Path.Combine(TestDir, path)));
    }

    private SortedDictionary<string, string> AllFiles() => new(
        Directory.GetFiles(TestDir, "*", SearchOption.AllDirectories)
            .ToDictionary(file => Path.GetRelativePath(TestDir, file), file => Convert.ToBase64String(File.ReadAllBytes(file))),
        StringComparer.Ordinal);

    private List<string> AllowEntries() =>
        JsonNode.Parse(ReadFile(".claude/settings.local.json"))!["permissions"]!["allow"]!.AsArray()
            .Select(entry => entry!.GetValue<string>()).ToList();
}
