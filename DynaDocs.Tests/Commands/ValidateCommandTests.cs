namespace DynaDocs.Tests.Commands;

using System.Runtime.Versioning;

/// <summary>
/// `dydo validate` at the command line: a broken `dydo.json` or nudge exits 1 with a finding; a
/// recorded Claude integration whose committed settings do not run the guard warns and exits 0.
/// `--root` is covered with check and graph in <see cref="CheckCommandTests"/>.
/// </summary>
[Collection("Integration")]
public sealed class ValidateCommandTests : IDisposable
{
    private const string Base = "\"version\":1,\"structure\":{\"root\":\"dydo\"}";
    private const string Claude = "{" + Base + ""","integrations":{"claude":true}}""";
    private const string Wired = """
        {"hooks":{"PreToolUse":[
          {"matcher":"Bash","hooks":[{"type":"command","command":"echo before"}]},
          {"matcher":"Anything","hooks":[{"type":"command","command":"echo mixed"},{"type":"command","command":"dydo guard --host claude"}]}
        ]}}
        """;
    private const string NoGuard = "no PreToolUse hook running 'dydo guard --host claude'";
    private const string FixJson = "Fix the JSON, then run 'dydo init claude --join'";

    private readonly string _dir = Path.Combine(Path.GetTempPath(), "dydo-validate-" + Guid.NewGuid().ToString("N")[..8]);
    private readonly string _original = Environment.CurrentDirectory;

    public ValidateCommandTests()
    {
        Directory.CreateDirectory(Path.Combine(_dir, "dydo", "_system"));
        Environment.CurrentDirectory = _dir;
    }

    [Theory]
    [Trait("Job", "contract: dydo-commands-utilities.md dydo validate (it reports a configuration's outcome, and exits with the code for that outcome)")]
    // dydo.json itself
    [InlineData(1, "not found", null)]
    [InlineData(1, "Invalid JSON", "{not valid json}")]
    [InlineData(1, "root must be an object", "null")]
    [InlineData(1, "testing.runner", """{"testing":{"runner":[]}}""")]
    [InlineData(0, null, """{"testing":{"runner":["runner",""]}}""")]
    // nudges
    [InlineData(1, "invalid regex", "{" + Base + ""","nudges":[{"pattern":"[invalid(regex","message":"m","severity":"block"}]}""")]
    [InlineData(1, "invalid regex", "{" + Base + ""","nudges":[{"pattern":"^tools/(","message":"m","severity":"block","match":"new-file"}]}""")]
    [InlineData(1, "empty pattern", "{" + Base + ""","nudges":[{"pattern":"","message":"m","severity":"block"}]}""")]
    [InlineData(1, "empty message", "{" + Base + ""","nudges":[{"pattern":"x","message":"","severity":"block"}]}""")]
    [InlineData(1, "invalid severity", "{" + Base + ""","nudges":[{"pattern":"x","message":"m","severity":"invalid"}]}""")]
    [InlineData(1, "invalid audience", "{" + Base + ""","nudges":[{"pattern":"x","message":"m","severity":"notice","audience":"team"}]}""")]
    [InlineData(1, "invalid audience", "{" + Base + ""","nudges":[{"pattern":"x","message":"m","severity":"notice","audience":null}]}""")]
    [InlineData(1, "invalid match 'path'", "{" + Base + ""","nudges":[{"pattern":"x","message":"m","severity":"warn","match":"path"}]}""")]
    [InlineData(1, "invalid match ''", "{" + Base + ""","nudges":[{"pattern":"x","message":"m","severity":"warn","match":""}]}""")]
    [InlineData(0, null, "{" + Base + ""","nudges":[{"pattern":"^tools/(?!one-off/)","message":"m","severity":"warn","match":"NEW-FILE"},{"pattern":"dotnet test","message":"m","severity":"warn","match":"command"},{"pattern":"^src/","message":"m","severity":"notice","audience":"WORKER"}]}""")]
    // Claude guard wiring: a warning, exit 0
    [InlineData(0, null, Claude, Wired)]
    [InlineData(0, null, Claude, "﻿" + Wired)]
    [InlineData(0, "the file is missing", Claude)]
    [InlineData(0, "the file is missing", Claude, null, Wired)]
    [InlineData(0, NoGuard, Claude, "{}")]
    [InlineData(0, NoGuard, Claude, """{"hooks":{"PreToolUse":[{"matcher":"Bash","hooks":[{"type":"command","command":"dydo guard"}]}]}}""")]
    [InlineData(0, NoGuard, Claude, """{"hooks":{"PostToolUse":[{"matcher":"Bash","hooks":[{"type":"command","command":"dydo guard --host claude"}]}]}}""")]
    [InlineData(0, NoGuard, Claude, """{"hooks":{"PreToolUse":[{"matcher":"Bash","hooks":["dydo guard --host claude"]}]}}""")]
    [InlineData(0, FixJson, Claude, "{not valid json")]
    [InlineData(0, FixJson, Claude, """{"hooks":{},"hooks":{}}""")]
    [InlineData(0, FixJson, Claude, "[]")]
    [InlineData(0, FixJson, Claude, """{"hooks":[]}""")]
    [InlineData(0, FixJson, Claude, """{"hooks":{"PreToolUse":{"matcher":"Bash"}}}""")]
    [InlineData(0, null, "{" + Base + ""","integrations":{"claude":false,"codex":true}}""")]
    [InlineData(0, null, "{" + Base + "}")]
    public void Validate_ReportsTheFindingAndExitCode(
        int expectedExit, string? expectedFinding, string? dydoJson, string? settings = null, string? localSettings = null)
    {
        if (dydoJson != null)
            File.WriteAllText(Path.Combine(_dir, "dydo.json"), dydoJson);
        WriteClaudeSettings("settings.json", settings);
        WriteClaudeSettings("settings.local.json", localSettings);

        var (code, output) = DydoCli.RunCombined("validate");

        Assert.True(expectedExit == code, $"exit {code}, expected {expectedExit}:\n{output}");
        if (expectedFinding is null)
            Assert.Contains("Validation passed", output);
        else
            Assert.Contains(expectedFinding, output);
    }

    [Fact]
    [Trait("Job", "contract: dydo-commands-utilities.md dydo validate (dydo.json deserialization, exit 1 on errors)")]
    public void Validate_InvalidDydoJson_ReportsItWithoutCheckingClaudeSettings()
    {
        File.WriteAllText(Path.Combine(_dir, "dydo.json"), """{"integrations":{"claude":true},"testing":{"runner":[]}}""");

        var (code, output) = DydoCli.RunCombined("validate");

        Assert.Equal(1, code);
        Assert.Contains("testing.runner", output);
        Assert.DoesNotContain(".claude/settings.json", output);
    }

    [SkippableFact]
    [Trait("Job", "contract: dydo-commands-utilities.md dydo validate (unreadable Claude settings: a warning to make them readable, exit 0)")]
    [UnsupportedOSPlatform("windows")]
    public void Validate_UnreadableClaudeSettings_WarnsToMakeThemReadable()
    {
        Skip.If(OperatingSystem.IsWindows(), "POSIX modes decide whether the file can be read.");
        File.WriteAllText(Path.Combine(_dir, "dydo.json"), Claude);
        WriteClaudeSettings("settings.json", Wired);
        File.SetUnixFileMode(Path.Combine(_dir, ".claude", "settings.json"), UnixFileMode.None);

        var (code, output) = DydoCli.RunCombined("validate");

        Assert.Equal(0, code);
        Assert.Contains("Make it readable", output);
    }

    private void WriteClaudeSettings(string fileName, string? content)
    {
        if (content == null)
            return;
        Directory.CreateDirectory(Path.Combine(_dir, ".claude"));
        File.WriteAllText(Path.Combine(_dir, ".claude", fileName), content);
    }

    public void Dispose()
    {
        Environment.CurrentDirectory = _original;
        var settings = Path.Combine(_dir, ".claude", "settings.json");
        if (!OperatingSystem.IsWindows() && File.Exists(settings))
            File.SetUnixFileMode(settings, UnixFileMode.UserRead | UnixFileMode.UserWrite);
        TestDirectory.Delete(_dir);
    }
}
