using System.Text;
using System.Text.Json.Nodes;
using DynaDocs.Commands;
using DynaDocs.Services;

namespace DynaDocs.Tests.Integration;

/// <summary>
/// init --join and adopt against a real downstream project's committed config: LC origin/master
/// 8ab5a88b, copied byte-exact (BOMs, LF, a PowerShell-indented hooks.json, hand-written node
/// hooks). Each command may change only dydo's own lines.
/// </summary>
[Collection("Integration")]
public class DownstreamConfigTests : IntegrationTestBase
{
    private const string Downstream = "trap: DYD-392 (init --join and adopt on a real downstream project's committed config): ";

    private static readonly string Fixture = Path.Combine(AppContext.BaseDirectory, "TestData", "downstream-lc");

    private const string ClaudeSettings = ".claude/settings.json";
    private const string ClaudeLocalSettings = ".claude/settings.local.json";
    private const string CodexHooks = ".codex/hooks.json";
    private const string CodexConfig = ".codex/config.toml";

    private const string OldClaudeMatcher =
        "Edit|Write|Read|Bash|Glob|Grep|Agent|EnterPlanMode|ExitPlanMode|PowerShell|NotebookEdit|AskUserQuestion";
    private const string ClaudeMatcher =
        "Edit|Write|Read|Bash|Glob|Grep|Agent|EnterPlanMode|ExitPlanMode|PowerShell|NotebookEdit";
    private const string OldCodexMatcher = OldClaudeMatcher + "|apply_patch|shell_command|exec|local_shell|unified_exec";
    private const string CodexMatcher = "Bash|apply_patch|Edit|Write|Agent|shell_command|exec|local_shell|unified_exec";

    public DownstreamConfigTests()
    {
        Directory.CreateDirectory(Path.Combine(TestDir, ".claude"));
        Directory.CreateDirectory(Path.Combine(TestDir, ".codex"));
        File.Copy(Path.Combine(Fixture, "dydo.json"), At(ConfigService.ConfigFileName));
        File.Copy(Path.Combine(Fixture, "claude-settings.json"), At(ClaudeSettings));
        File.Copy(Path.Combine(Fixture, "codex-hooks.json"), At(CodexHooks));
        File.Copy(Path.Combine(Fixture, "codex-config.toml"), At(CodexConfig));
    }

    [Fact]
    [Trait("Job", Downstream + "init --join claude changes only the guard's matcher and host marker and drops the Stop block in .claude/settings.json, keeping its UTF-8 BOM and both node hook lines, with one marked and no bare guard")]
    public async Task JoinClaude_RewritesOnlyDydosOwnLinesInTheCommittedSettings()
    {
        var original = Text(ClaudeSettings);

        (await JoinProjectAsync("claude")).AssertSuccess();

        var result = Text(ClaudeSettings);
        var expected = WithoutStopBlock(original
            .Replace($"\"{OldClaudeMatcher}\"", $"\"{ClaudeMatcher}\"")
            .Replace("\"command\": \"dydo guard\"", "\"command\": \"dydo guard --host claude\""));
        Assert.Equal(expected, result);
        Assert.True(PreservedText.HasBom(At(ClaudeSettings)));
        AssertForeignHookLinesIdentical(original, result);

        var hooks = Assert.IsType<JsonObject>(Parse(result)["hooks"]);
        var preToolUse = Assert.IsType<JsonArray>(hooks["PreToolUse"]);
        Assert.Equal(1, HookTestAssertions.Count(preToolUse, "dydo guard --host claude"));
        Assert.Equal(0, HookTestAssertions.Count(preToolUse, "dydo guard"));
        Assert.DoesNotContain("--stop", result);
    }

    [Fact]
    [Trait("Job", "contract: DR 056 (the allow list stays machine-local): init --join claude on a real downstream project writes no hooks into .claude/settings.local.json and puts Bash(dydo:*) in its allow list")]
    public async Task JoinClaude_LeavesNoDydoHookInTheLocalSettings()
    {
        (await JoinProjectAsync("claude")).AssertSuccess();

        var local = Parse(Text(ClaudeLocalSettings));
        Assert.Null(local["hooks"]);
        var allow = Assert.IsType<JsonArray>(local["permissions"]?["allow"]);
        Assert.Contains(allow, entry => entry?.GetValue<string>() == "Bash(dydo:*)");
    }

    [Fact]
    [Trait("Job", Downstream + "init --join codex appends [features] memories = false to the BOM-led .codex/config.toml, keeping its BOM, changes only the matcher and drops the Stop block in .codex/hooks.json, keeping both node hook lines and one dydo guard, and leaves dydo.json byte for byte")]
    public async Task JoinCodex_ReadsTheBomConfigAndRewritesOnlyDydosOwnHookLines()
    {
        var hooks = Text(CodexHooks);
        var config = Text(CodexConfig);
        var dydoJson = File.ReadAllBytes(At(ConfigService.ConfigFileName));

        (await JoinProjectAsync("codex")).AssertSuccess();

        Assert.Equal(config + "\n[features]\nmemories = false\n", Text(CodexConfig));
        Assert.True(PreservedText.HasBom(At(CodexConfig)));

        var result = Text(CodexHooks);
        Assert.Equal(WithoutStopBlock(hooks.Replace($"\"{OldCodexMatcher}\"", $"\"{CodexMatcher}\"")), result);
        AssertForeignHookLinesIdentical(hooks, result);
        var preToolUse = Assert.IsType<JsonArray>(Parse(result)["hooks"]?["PreToolUse"]);
        Assert.Equal(1, HookTestAssertions.Count(preToolUse, "dydo guard"));
        Assert.DoesNotContain("--stop", result);

        Assert.Equal(dydoJson, File.ReadAllBytes(At(ConfigService.ConfigFileName)));
    }

    [Fact]
    [Trait("Job", Downstream + "dydo adopt 3.5.0 keeps every line of dydo.json in order, adds no CR, keeps its count of unicode escapes, sets dydoVersion to 3.5.0 and appends one nudge, changing nothing else")]
    public async Task Adopt_AddsOnlyTheVersionAndTheReleaseNudge()
    {
        var original = Text(ConfigService.ConfigFileName);

        (await RunAsync(AdoptCommand.Create(), "3.5.0")).AssertSuccess();

        var result = Text(ConfigService.ConfigFileName);
        PreservedText.AssertLinesKeptInOrder(original, result);
        Assert.DoesNotContain("\r", result);
        Assert.Equal(CountEscapes(original), CountEscapes(result));

        var adopted = Parse(result);
        Assert.Equal("3.5.0", adopted["dydoVersion"]?.GetValue<string>());
        adopted.Remove("dydoVersion");
        var nudges = Assert.IsType<JsonArray>(adopted["nudges"]);
        nudges.RemoveAt(nudges.Count - 1);
        Assert.True(JsonNode.DeepEquals(Parse(original), adopted), "adopt changed more than dydoVersion and its nudge");
    }

    [Fact]
    [Trait("Job", Downstream + "a second init --join all and dydo adopt 3.5.0 change no byte of .claude/settings.json, .claude/settings.local.json, .codex/hooks.json, .codex/config.toml or dydo.json")]
    public async Task RepeatedJoinAndAdopt_LeaveEveryFileByteIdentical()
    {
        (await JoinProjectAsync("all")).AssertSuccess();
        (await RunAsync(AdoptCommand.Create(), "3.5.0")).AssertSuccess();
        var files = new[] { ClaudeSettings, ClaudeLocalSettings, CodexHooks, CodexConfig, ConfigService.ConfigFileName };
        var first = files.Select(file => File.ReadAllBytes(At(file))).ToList();

        (await JoinProjectAsync("all")).AssertSuccess();
        (await RunAsync(AdoptCommand.Create(), "3.5.0")).AssertSuccess();

        for (var i = 0; i < files.Length; i++)
            Assert.True(first[i].SequenceEqual(File.ReadAllBytes(At(files[i]))), $"{files[i]} changed on the repeat");
    }

    // Windows PowerShell 5.1 `'...' > dydo.json` writes UTF-16LE with a BOM; other editors pick the rest.
    public static TheoryData<string> BomEncodings => ["utf-16le", "utf-8", "utf-16be", "utf-32le", "utf-32be"];

    [Theory]
    [Trait("Job", "trap: DYD-392 (a file with nothing to change keeps its bytes): dydo adopt 3.5.0 of a dydo.json already at 3.5.0, BOM-encoded in UTF-16LE, UTF-8, UTF-16BE, UTF-32LE or UTF-32BE, leaves its bytes")]
    [MemberData(nameof(BomEncodings))]
    public async Task Adopt_NothingToChange_LeavesABomEncodedConfigByteIdentical(string encoding)
    {
        var bytes = Encode(encoding, "{\"version\":1,\"dydoVersion\":\"3.5.0\",\"projectTag\":\"keep\"}\r\n");
        File.WriteAllBytes(At(ConfigService.ConfigFileName), bytes);

        (await RunAsync(AdoptCommand.Create(), "3.5.0")).AssertSuccess();

        Assert.Equal(bytes, File.ReadAllBytes(At(ConfigService.ConfigFileName)));
    }

    [Theory]
    [Trait("Job", "trap: DYD-392 (a file dydo edits keeps its encoding and text): dydo adopt 3.5.0 over 3.4.0 of a BOM-encoded dydo.json, in each of the five encodings, keeps that encoding and BOM, the key order with the unknown key and the CRLF ending, and changes only dydoVersion and the added nudges")]
    [MemberData(nameof(BomEncodings))]
    public async Task Adopt_Upgrade_KeepsTheEncodingBomAndUnknownKeysOfTheConfig(string encoding)
    {
        const string original = "{\"version\":1,\"dydoVersion\":\"3.4.0\",\"projectTag\":\"keep\"}\r\n";
        File.WriteAllBytes(At(ConfigService.ConfigFileName), Encode(encoding, original));

        (await RunAsync(AdoptCommand.Create(), "3.5.0")).AssertSuccess();

        var result = Decode(encoding, File.ReadAllBytes(At(ConfigService.ConfigFileName)));
        Assert.StartsWith("{\"version\":1,\"dydoVersion\":\"3.5.0\",\"projectTag\":\"keep\",", result);
        Assert.EndsWith("}\r\n", result);
        var adopted = Parse(result);
        Assert.NotNull(adopted["nudges"]);
        adopted.Remove("nudges");
        Assert.True(JsonNode.DeepEquals(Parse(original.Replace("3.4.0", "3.5.0")), adopted),
            "adopt changed more than dydoVersion and its nudge");
    }

    [Fact]
    [Trait("Job", Downstream + "init --join all over UTF-16LE .claude/settings.json, .codex/hooks.json and .claude/settings.local.json keeps each file's UTF-16LE BOM, changes only dydo's lines in the two hook files, and keeps the local file's foreign key first while adding Bash(dydo:*)")]
    public async Task JoinAll_Utf16LeHookFiles_KeepTheirEncodingBomAndForeignContent()
    {
        var settings = Text(ClaudeSettings).TrimStart('﻿');
        var hooks = Text(CodexHooks);
        const string local = "{\r\n  \"foreign\": \"keep\"\r\n}\r\n";
        File.WriteAllBytes(At(ClaudeSettings), Encode("utf-16le", settings));
        File.WriteAllBytes(At(CodexHooks), Encode("utf-16le", hooks));
        File.WriteAllBytes(At(ClaudeLocalSettings), Encode("utf-16le", local));

        (await JoinProjectAsync("all")).AssertSuccess();

        Assert.Equal(WithoutStopBlock(settings
            .Replace($"\"{OldClaudeMatcher}\"", $"\"{ClaudeMatcher}\"")
            .Replace("\"command\": \"dydo guard\"", "\"command\": \"dydo guard --host claude\"")),
            Decode("utf-16le", File.ReadAllBytes(At(ClaudeSettings))));
        Assert.Equal(WithoutStopBlock(hooks.Replace($"\"{OldCodexMatcher}\"", $"\"{CodexMatcher}\"")),
            Decode("utf-16le", File.ReadAllBytes(At(CodexHooks))));
        var localResult = Decode("utf-16le", File.ReadAllBytes(At(ClaudeLocalSettings)));
        Assert.StartsWith("{\r\n  \"foreign\": \"keep\",", localResult);
        Assert.Contains("Bash(dydo:*)", localResult);
    }

    private static Encoding Named(string encoding) => encoding switch
    {
        "utf-8" => new UTF8Encoding(false),
        "utf-16le" => new UnicodeEncoding(bigEndian: false, byteOrderMark: false),
        "utf-16be" => new UnicodeEncoding(bigEndian: true, byteOrderMark: false),
        "utf-32le" => new UTF32Encoding(bigEndian: false, byteOrderMark: false),
        _ => new UTF32Encoding(bigEndian: true, byteOrderMark: false),
    };

    // The BOM in front, as the encoding's own bytes of U+FEFF.
    private static byte[] Encode(string encoding, string text) => Named(encoding).GetBytes("﻿" + text);

    // Asserts the encoding's BOM still leads the bytes and returns the text after it.
    private static string Decode(string encoding, byte[] bytes)
    {
        var bom = Named(encoding).GetBytes("﻿");
        Assert.True(bytes.AsSpan().StartsWith(bom), $"the {encoding} BOM is gone");
        return Named(encoding).GetString(bytes, bom.Length, bytes.Length - bom.Length);
    }

    private string At(string relative) => System.IO.Path.Combine(TestDir, relative);

    private string Text(string relative) => PreservedText.Read(At(relative));

    private static JsonObject Parse(string text) => JsonNode.Parse(text.TrimStart('﻿'))!.AsObject();

    private static int CountEscapes(string text) => text.Split("\\u").Length - 1;

    // LC's own node hooks: each command line must come through byte for byte.
    private static void AssertForeignHookLinesIdentical(string original, string result)
    {
        var foreign = original.Split('\n').Where(line => line.Contains("node -e")).ToList();
        Assert.Equal(2, foreign.Count);
        var lines = result.Split('\n');
        foreach (var line in foreign)
            Assert.Contains(line, lines);
    }

    // The Stop member, the comma before it and its whole array: what dropping dydo's only Stop entry removes.
    private static string WithoutStopBlock(string text)
    {
        var key = text.IndexOf("\"Stop\"", StringComparison.Ordinal);
        var start = text.LastIndexOf(',', key);
        var depth = 0;
        var end = text.IndexOf('[', key);
        do
        {
            depth += text[end] switch { '[' => 1, ']' => -1, _ => 0 };
            end++;
        } while (depth > 0);
        return text[..start] + text[end..];
    }
}
