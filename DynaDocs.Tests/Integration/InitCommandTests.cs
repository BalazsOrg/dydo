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
    [Trait("Job", "trap: DYD-134 (a late failure keeps dydo.json's input bytes): a --join whose post-work fails (injected) exits 2 naming the failure, keeps dydo.json byte for byte with its unknown keys and leaves no dydo.json.*.tmp sibling, and the .codex/hooks.json written before that point stays")]
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
    [Trait("Job", "trap: DYD-134 (a join saves dydo.json only when its integration changes): a --join of an integration dydo.json already records exits 0 and leaves dydo.json byte for byte, unknown keys included, with no dydo.json.*.tmp sibling")]
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
    [Trait("Job", "contract: dydo-commands-core.md dydo init (a fresh init records the running dydo version in dydo.json as dydoVersion)")]
    public async Task Init_Fresh_RecordsRunningCliVersion()
    {
        (await InitProjectAsync("none")).AssertSuccess();

        Assert.Equal(DydoVersion.Current, ReadDydoVersion());
        Assert.Matches(@"^\d+\.\d+\.\d+", DydoVersion.Current);
    }

    [Fact]
    [Trait("Job", "contract: dydo-commands-core.md dydo init --join (leaves dydoVersion alone)")]
    public async Task Join_ExistingDydoVersion_StaysUntouched()
    {
        (await InitProjectAsync("codex")).AssertSuccess();
        SetDydoVersion("1.2.3");

        (await JoinProjectAsync("codex")).AssertSuccess();

        Assert.Equal("1.2.3", ReadDydoVersion());
    }

    [Fact]
    [Trait("Job", "contract: dydo-commands-core.md dydo init --join (leaves dydoVersion alone when it joins a new integration)")]
    public async Task Join_RecordingNewIntegration_PreservesDydoVersion()
    {
        (await InitProjectAsync("none")).AssertSuccess();
        SetDydoVersion("1.2.3");

        var result = await JoinProjectAsync("codex");

        result.AssertSuccess();
        Assert.Equal("1.2.3", ReadDydoVersion());
    }

    [Fact]
    [Trait("Job", "contract: dydo-commands-core.md dydo init --join (dydoVersion): a --join exits 0 and adds no dydoVersion to a dydo.json without one")]
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
    [Trait("Job", "contract: dydo-commands-core.md dydo init host table (settings.json env CLAUDE_CODE_MAX_SUBAGENT_SPAWN_DEPTH = 3 and autoMemoryEnabled false; config.toml [agents] max_depth = 3 and max_concurrent_threads_per_session = 16); DR 055 (memories = false under [features])")]
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
    [Trait("Job", "contract: DR 055 (native memory off): --join all over autoMemoryEnabled true and memories = true exits 0 and sets both off, keeping settings.json's other value and the rest of config.toml's text")]
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
    [Trait("Job", "contract: DR 055 (memories = false under [features] in .codex/config.toml): --join adds it inside an existing [features] table, keeping the rest of the text")]
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
    [Trait("Job", "contract: dydo-commands-core.md dydo init (a conflicting value for a key init manages): a memories = true in a form init cannot edit exits 2 naming .codex/config.toml, keeps that file's bytes and writes no dydo.json")]
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
    [Trait("Job", "trap: DYD-86 (unrelated host settings kept, a repeat init idempotent); DYD-392 (the committed guard): over satisfying values, init all exits 0 and keeps settings.json's lines in order beside one marked guard and config.toml's bytes, and a repeat --join exits 0 and changes neither file's bytes")]
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
    [Trait("Job", "trap: DYD-392 (--join stopped on a config.toml led by a BOM): it reads it, keeps the BOM and the [agents] table's text, and adds [features] memories = false")]
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
    [Trait("Job", "trap: DYD-392 (--join stopped on a config.toml led by a BOM): it keeps the BOM, sets memories = false inside the one [features] table and adds [agents]")]
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
    [Trait("Job", "contract: dydo-commands-core.md dydo init (a conflicting value for a key init manages): a CLAUDE_CODE_MAX_SUBAGENT_SPAWN_DEPTH other than the string 3 exits 2 naming the file and the key, keeps settings.json's bytes and writes no dydo.json or settings.local.json")]
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
    [Trait("Job", "contract: dydo-commands-core.md dydo init (a conflicting value for a key init manages): a conflicting [agents] table exits 2 naming .codex/config.toml and agents, keeps its bytes and writes no dydo.json, .codex/hooks.json or AGENTS.md")]
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
    [Trait("Job", "trap: DYD-86 (malformed or conflicting host settings): an [agents] form init cannot edit safely exits 2 naming .codex/config.toml, keeps its bytes and writes no dydo.json, .codex/hooks.json or AGENTS.md")]
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
    [Trait("Job", "contract: dydo-commands-core.md dydo init (a conflicting value for a key init manages): a null env or a null CLAUDE_CODE_MAX_SUBAGENT_SPAWN_DEPTH exits 2 naming .claude/settings.json, keeps its bytes and writes no dydo.json or settings.local.json")]
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
    [Trait("Job", "trap: DYD-86 (malformed or conflicting host settings): init all with one host file it refuses exits 2, keeps both host files' bytes and writes no dydo.json, CLAUDE.md, AGENTS.md, settings.local.json, .codex/hooks.json or .gitignore")]
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
    [Trait("Job", "contract: dydo-commands-core.md dydo init (a conflicting value for a key init manages): --join all over a conflicting [agents] exits 2, keeps dydo.json, both host files, AGENTS.md and a generated CLAUDE.md byte for byte and writes no settings.local.json or .codex/hooks.json")]
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
    [Trait("Job", "trap: DYD-86 (malformed or conflicting host settings): init all over a malformed config.toml exits 2 with .codex/config.toml and malformed in its error, keeps both host files' bytes and writes no dydo.json, CLAUDE.md, AGENTS.md, settings.local.json, .codex/hooks.json or .gitignore")]
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
    [Trait("Job", "trap: DYD-86 (malformed or conflicting host settings): --join all over a malformed config.toml exits 2 with .codex/config.toml and malformed in its error, keeps dydo.json, .gitignore, AGENTS.md and both host files byte for byte and writes no CLAUDE.md, settings.local.json or .codex/hooks.json")]
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
    [Trait("Job", "trap: DYD-86 (malformed or conflicting host settings): init all over a dotted agents key init cannot edit exits 2 naming .codex/config.toml, keeps both host files' bytes and writes no dydo.json, CLAUDE.md, AGENTS.md, settings.local.json, .codex/hooks.json or .gitignore")]
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
    [Trait("Job", "trap: DYD-86 (malformed or conflicting host settings): --join all over a dotted agents key init cannot edit exits 2 naming .codex/config.toml, keeps dydo.json, .gitignore, AGENTS.md and both host files byte for byte and writes no CLAUDE.md, settings.local.json or .codex/hooks.json")]
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
    [Trait("Job", "trap: DYD-86 (unrelated TOML keys kept, a repeat init idempotent): init codex over an unrelated [[agents_history]] array exits 0 and keeps its header, and a repeat --join changes no byte of config.toml")]
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
    [Trait("Job", "trap: DYD-86 (existing settings kept, a repeat init idempotent): [agents] integers that already satisfy init, in five TOML integer notations, keep config.toml's bytes through init and --join, each exiting 0")]
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
    [Trait("Job", "contract: dydo-commands-core.md host table codex row (.codex/hooks.json: one PreToolUse entry running dydo guard, and no Stop entry); trap: issue 0295 (the matcher names Codex's shell tools)")]
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
    [Trait("Job", "contract: dydo-commands-core.md dydo init (hooks it did not write): init codex exits 0, .codex/hooks.json still holds the foreign hooks' matcher and commands, PostToolUse and otherSetting beside dydo guard, and gains no dydo guard --stop")]
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
    [Trait("Job", "contract: dydo-commands-core.md dydo init (keeps one dydo guard entry, removes other dydo guard entries and Stop entries running dydo guard --stop, leaves hooks it did not write alone): a foreign command that merely contains dydo guard, or shares an entry with it, survives")]
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
    [Trait("Job", "contract: dydo-commands-core.md dydo init (hooks it did not write); DR 056 (the guard rides the committed settings.json): init claude exits 0, settings.local.json keeps its lines and gets no dydo guard, and settings.json holds dydo guard with its matcher beside the foreign hooks' text, PostToolUse and otherSetting")]
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
    [Trait("Job", "contract: dydo-commands-core.md dydo init (dydo hooks leave settings.local.json, hooks it did not write stay); DR 056 (one marked guard in the committed settings.json): --join claude removes the bare, marked and --stop dydo guard hooks from settings.local.json, leaving echo mine as its one PreToolUse entry, and the committed settings.json holds one marked guard")]
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
    [Trait("Job", "contract: dydo-commands-core.md dydo init (keeps one dydo guard entry, removes other dydo guard entries and the Stop entry running dydo guard --stop, leaves hooks it did not write alone): one marked guard with the full matcher, and the foreign entry's bytes")]
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
    [Trait("Job", "trap: DYD-392 (never touch foreign hooks; a1f3a044a): a foreign command that shared an entry with dydo guard keeps the rest of its entry's bytes, beside one dydo guard entry")]
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
    [Trait("Job", "trap: DYD-392 (never touch foreign hooks; a1f3a044a): duplicate dydo guard entries around foreign ones become one guard entry where the first stood, and the foreign entries keep their bytes")]
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
    [Trait("Job", "trap: DYD-392 (never touch foreign hooks; a1f3a044a): an already-empty foreign entry keeps its bytes and stays before the one guard entry")]
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
    [Trait("Job", "trap: DYD-392 (never touch foreign hooks; a1f3a044a): an already-empty foreign entry in settings.local.json keeps its bytes while dydo's guard leaves that file")]
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
    [Trait("Job", "trap: DYD-392 (a1f3a044a, a trailing JSON root in a hooks file): --join on a settings.local.json or .codex/hooks.json holding {} {} succeeds and leaves one parsable JSON object")]
    public async Task Join_TrailingRootInAHooksFile_StartsItFresh(string host, string file)
    {
        (await InitProjectAsync("none")).AssertSuccess();
        WriteFile(file, "{} {}");

        (await JoinProjectAsync(host)).AssertSuccess();

        Assert.IsType<JsonObject>(JsonNode.Parse(ReadFile(file)));
    }

    [Fact]
    [Trait("Job", "trap: DYD-392 (a1f3a044a, a trailing JSON root in a hooks file): --join claude over a committed settings.json holding {} {} exits nonzero with malformed in its error and keeps its text")]
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
    [Trait("Job", "trap: a tool the Claude matcher does not name bypasses every guard layer (PowerShell, NotebookEdit, Agent): init claude exits 0 and its one PreToolUse entry's matcher names Edit, Write, Read, Bash, Glob, Grep, Agent, PowerShell and NotebookEdit")]
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
    [Trait("Job", "contract: dydo-commands-core.md host table claude row (the committed PreToolUse runs dydo guard --host claude); DR 056: an unmarked dydo guard is replaced by exactly one marked guard")]
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

    // dydo-commands-core.md, dydo init's host table: what each integration writes, and a repeat join
    // that changes no byte of any of it.
    [Theory]
    [Trait("Job", "contract: dydo-commands-core.md dydo init host table: for each integration, init exits 0 and writes dydo.json's integrations, AGENTS.md and no CLAUDE.md, and the .gitignore lines; with claude, settings.json with one marked and no bare guard and settings.local.json with no dydo guard and each dydo allow entry once; with codex, hooks.json without --host and dydo.rules; without claude no Claude settings file, without codex no hooks.json or dydo.rules; a repeat --join exits 0 and changes no file")]
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
            // Only the Claude hook carries the host marker, exactly once.
            var preToolUse = Assert.IsType<JsonArray>(JsonNode.Parse(ReadFile(".claude/settings.json"))?["hooks"]?["PreToolUse"]);
            Assert.Equal(1, HookTestAssertions.Count(preToolUse, ClaudeGuard));
            Assert.Equal(0, HookTestAssertions.Count(preToolUse, CodexGuard));
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
    [Trait("Job", "contract: dydo-commands-core.md host table claude row (settings.local.json carries each dydo allow entry once, also when the file already exists); trap: an existing allow list is merged, not replaced (the row that has one)")]
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
    [Trait("Job", "trap: issue 0300 (--join did not record the joined integration in dydo.json): both hosts end recorded and both hosts' hook files exist; trap: issue 0303 (the Claude local settings rode a commit): .gitignore names .claude/settings.local.json")]
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

    // Kept `unsure` by the DYD-1520 setup audit: no spec line gives init's refusals or their exit code.
    [Theory]
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
    [Trait("Job", "contract: dydo-commands-core.md host table codex row (.codex/rules/dydo.rules): init exits 0 and the file holds a merge rule under dydo or dydo.exe, a clean rule under dydo and an allow decision")]
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
    [Trait("Job", "contract: dydo-commands-core.md host table codex row (.codex/rules/dydo.rules on --join): after a stale file and two more --join codex runs, each exiting 0, the file holds exactly dydo's rules")]
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
    [Trait("Job", "contract: DYD-403 (dydo.rules keeps its bytes and line endings): --join leaves a CRLF dydo.rules that holds dydo's rules byte for byte")]
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
    [Trait("Job", "contract: dydo-commands-core.md host table codex row (.codex/rules/dydo.rules on --join); DYD-403 (in the file's own line endings): --join codex exits 0 and rewrites a stale CRLF file to exactly dydo's rules with CRLF")]
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
    [Trait("Job", "contract: dydo-commands-core.md dydo init (with claude, --join removes a CLAUDE.md equal to AGENTS.md): one equal but for line endings and trailing blank lines goes, --join exits 0, and AGENTS.md keeps its text")]
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
    [Trait("Job", "contract: dydo-commands-core.md dydo init (with claude, --join removes a CLAUDE.md equal to the template): it goes even when AGENTS.md differs, and AGENTS.md keeps its text")]
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
    [Trait("Job", "contract: dydo-commands-core.md dydo init (a CLAUDE.md you wrote is kept, with a warning): --join claude exits 0, keeps it byte for byte, prints that it hides AGENTS.md from Claude Code, and names @AGENTS.md")]
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
    [Trait("Job", "contract: dydo-commands-core.md dydo init (with claude, --join removes a CLAUDE.md that is dydo's own generated copy); DYD-372 (a copy a released dydo generated counts): --join claude exits 0 and removes a copy generated by each release range the rows name, from v1.1.0 to the 3.5 base tree, LF or CRLF")]
    public async Task Join_Claude_RemovesClaudeMdAnOlderReleaseGenerated(string release, string claudeMd)
    {
        (await InitProjectAsync("none")).AssertSuccess();
        WriteFile("CLAUDE.md", claudeMd);

        var result = await JoinProjectAsync("claude");

        result.AssertSuccess();
        Assert.False(File.Exists(Path.Combine(TestDir, "CLAUDE.md")), $"CLAUDE.md from {release} was kept");
    }

    [Fact]
    [Trait("Job", "contract: dydo-commands-core.md dydo init (a CLAUDE.md you wrote is kept, with a warning that it hides AGENTS.md); DYD-372: an older release's copy with text added counts as written")]
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
    [Trait("Job", "contract: dydo-commands-core.md dydo init (a CLAUDE.md you wrote is kept, with a warning that it hides AGENTS.md); DYD-372: a released body under a first line of your own counts as written")]
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
    [Trait("Job", "contract: dydo-commands-core.md dydo init (init claude keeps a CLAUDE.md you wrote, with a warning that it hides AGENTS.md)")]
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
    [Trait("Job", "contract: dydo-commands-core.md dydo init (CLAUDE.local.md and .claude/CLAUDE.md that hide AGENTS.md): --join claude exits 0, keeps each byte for byte and prints its own warning")]
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
    [Trait("Job", "contract: dydo-commands-core.md dydo init (the AGENTS.md import at an entry file's top); DYD-403 (no warning on a leading AGENTS.md import): --join claude exits 0, and an entry file that opens with its AGENTS.md import is kept byte for byte with neither the hides nor the stops-reading warning")]
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
    [Trait("Job", "contract: dydo-commands-core.md dydo init (a CLAUDE.md you wrote, CLAUDE.local.md and .claude/CLAUDE.md are kept, each with its own warning); DYD-403: a file whose import is not first, or names the wrong path, is kept byte for byte and gets its warning")]
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
    [Trait("Job", "contract: dydo-commands-core.md dydo init (with claude or all, a generated CLAUDE.md is removed): --join codex exits 0, keeps a generated CLAUDE.md byte for byte, and its stdout names no CLAUDE.md")]
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
    [Trait("Job", "contract: dydo-commands-core.md dydo init (--join writes the entry point when it is missing; with claude it removes a generated CLAUDE.md): a CLAUDE.md-only project gets AGENTS.md from the template and loses the generated CLAUDE.md")]
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
    [Trait("Job", "trap: DYD-368 (a failed preflight leaves no side effect): --join all over a malformed config.toml exits 2, keeps a legacy generated CLAUDE.md byte for byte and writes no AGENTS.md")]
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
