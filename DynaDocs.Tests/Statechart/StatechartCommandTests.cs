namespace DynaDocs.Tests.Statechart;

using System.Runtime.Versioning;
using System.Text.Json;
using DynaDocs.Services.Statechart;

/// <summary>
/// <c>dydo statechart doctor</c> and <c>install-skill</c> as a caller sees them: the exit code and what is left on disk.
/// </summary>
public class StatechartCommandTests
{
    [SkippableFact]
    [Trait("Job", "contract: dydo-commands-utilities.md dydo statechart doctor (on fresh roots without TLA2TOOLS, doctor exits 2, reports ready false, and creates no dependency root)")]
    public async Task InstalledDoctorExtractsPayloadAndNeverClaimsChartReadinessOrInstallsPrerequisites()
    {
        using var scope = new StatechartTestDirectory();
        Skip.IfNot(OperatingSystem.IsWindows(), "statechart doctor is Windows-only.");
        // The caller's own PATH stays, so doctor sees whatever runtimes this host has and still installs nothing.
        var start = StatechartTestBinary.Start();
        start.UseShellExecute = false;
        start.CreateNoWindow = true;
        start.RedirectStandardOutput = true;
        start.RedirectStandardError = true;
        start.ArgumentList.Add("statechart");
        start.ArgumentList.Add("doctor");
        foreach (var name in start.Environment.Keys.Where(name => name.StartsWith("DYDO_", StringComparison.Ordinal)).ToArray())
            start.Environment.Remove(name);
        start.Environment["DYDO_STATECHART_ASSET_ROOT"] = Path.Combine(scope.Path, "assets");
        start.Environment["DYDO_STATECHART_DEPENDENCY_ROOT"] = Path.Combine(scope.Path, "dependencies");
        start.Environment["DYDO_STATECHART_STATE_ROOT"] = Path.Combine(scope.Path, "state");
        start.Environment.Remove("JAVA");
        start.Environment.Remove("TLA2TOOLS");
        var result = await StatechartOfflineProcess.Run(start);
        Assert.Equal(2, result.Exit);
        using var json = JsonDocument.Parse(result.Output);
        Assert.False(json.RootElement.GetProperty("ready").GetBoolean());
        Assert.True(Directory.Exists(json.RootElement.GetProperty("payload").GetString()));
        Assert.False(Directory.Exists(Path.Combine(scope.Path, "dependencies")));
        Assert.Empty(StatechartLeftovers.Left(Path.Combine(scope.Path, "state")));
    }

    // With no runtime on PATH the Windows diagnosis still completes, and reports each runtime as null.
    [SkippableFact]
    [Trait("Job", "contract: dydo-commands-utilities.md dydo statechart doctor (with no runtime on PATH, doctor exits 2, reports ready false, and reports node and java as null)")]
    public async Task InstalledDoctorNamesMissingRuntimesWithoutProbingThem()
    {
        Skip.IfNot(OperatingSystem.IsWindows(), "statechart doctor is Windows-only.");
        using var scope = new StatechartTestDirectory();
        var result = await Run(scope.Path, "", "statechart", "doctor");
        Assert.Equal(2, result.Exit);
        using var json = JsonDocument.Parse(result.Output);
        Assert.False(json.RootElement.GetProperty("ready").GetBoolean());
        foreach (var runtime in new[] { "node", "java" })
            Assert.Equal(JsonValueKind.Null, json.RootElement.GetProperty(runtime).ValueKind);
        Assert.Empty(StatechartLeftovers.Left(Path.Combine(scope.Path, "state")));
    }

    // Native diagnostics still need platform validation outside Windows, so doctor declines there.
    [SkippableFact]
    [Trait("Job", "trap: outside Windows, doctor exits 2 and leaves the directory holding its roots empty")]
    public async Task DoctorDeclinesOutsideWindowsBeforeExtractingOrProbing()
    {
        Skip.If(OperatingSystem.IsWindows(), "Windows runs the native diagnosis.");
        using var scope = new StatechartTestDirectory();
        var result = await Run(scope.Path, "", "statechart", "doctor");
        Assert.Equal(2, result.Exit);
        Assert.Empty(Directory.EnumerateFileSystemEntries(scope.Path));
    }

    // A fake Node stands in for the embedded projection installer; the skill placement and owned probe are real.
    [SkippableFact]
    [Trait("Job", "contract: dydo-commands-utilities.md dydo statechart install-skill (the skill install-skill places in the named project validates)")]
    [UnsupportedOSPlatform("windows")]
    public async Task InstallSkillPlacesAValidSkill_RunsTheProjection_AndLeavesNoScratch()
    {
        Skip.IfNot(OperatingSystem.IsLinux(), "POSIX ownership runs on Linux.");
        using var scope = new StatechartTestDirectory();
        var project = Directory.CreateDirectory(Path.Combine(scope.Path, "project")).FullName;
        var bin = Directory.CreateDirectory(Path.Combine(scope.Path, "bin")).FullName;
        var projected = Path.Combine(scope.Path, "projected");
        var node = Path.Combine(bin, "node");
        File.WriteAllText(node, $"#!/bin/sh\n: > {projected}\n");
        File.SetUnixFileMode(node, UnixFileMode.UserRead | UnixFileMode.UserExecute);
        var result = await Run(scope.Path, bin, "statechart", "install-skill", "--root", project);
        Assert.True(result.Exit == 0, result.Error);
        Assert.True(File.Exists(projected));
        var skill = Path.Combine(project, "skills", "engineering", "statechart");
        Assert.Equal(skill, StatechartSkill.Validate(skill, new StatechartAssets()));
        Assert.Empty(StatechartLeftovers.Left(Path.Combine(scope.Path, "state")));
    }

    [Fact]
    [Trait("Job", "trap: when install-skill fails for want of Node, the skill it already placed still validates")]
    public async Task InstallSkillWithoutNode_Fails_AndKeepsThePlacedSkill()
    {
        using var scope = new StatechartTestDirectory();
        var project = Directory.CreateDirectory(Path.Combine(scope.Path, "project")).FullName;
        var result = await Run(scope.Path, "", "statechart", "install-skill", "--root", project);
        Assert.Equal(2, result.Exit);
        var skill = Path.Combine(project, "skills", "engineering", "statechart");
        Assert.Equal(skill, StatechartSkill.Validate(skill, new StatechartAssets()));
    }

    [Theory]
    [Trait("Job", "trap: a statechart subcommand run without its required option exits 2, writes nothing to stdout, and leaves its working directory, which holds its roots, empty")]
    [InlineData("install-skill")]
    [InlineData("resume")]
    public async Task ASubcommandWithoutItsRequiredOptionChangesNothing(string subcommand)
    {
        using var scope = new StatechartTestDirectory();
        var result = await Run(scope.Path, "", "statechart", subcommand);
        Assert.Equal(2, result.Exit);
        Assert.Equal("", result.Output);
        Assert.Empty(Directory.EnumerateFileSystemEntries(scope.Path));
    }

    // The npm lifetime pipe closes while another run holds the payload extraction lock, so the wait ends in cancellation.
    [Fact]
    [Trait("Job", "contract: DR 062 §5 (a run cancelled by its npm lifetime closing while it waits is non-success, exiting 130)")]
    public async Task LosingTheNpmLifetimeCancelsAWaitingRunWithoutAPlan()
    {
        using var scope = new StatechartTestDirectory();
        File.WriteAllText(Path.Combine(scope.Path, "model.tla"), "---- MODULE model ----\n====\n");
        File.WriteAllText(Path.Combine(scope.Path, "model.cfg"), "\n");
        var release = Directory.CreateDirectory(Path.Combine(scope.Path, "assets", new StatechartAssets().Release)).FullName;
        using var held = new FileStream(Path.Combine(release, ".extract.lock"), FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None);
        var start = StatechartOfflineProcess.Start(scope.Path, "", ["statechart", "model.tla"]);
        start.RedirectStandardInput = true;
        start.Environment["DYDO_STATECHART_NPM_LIFETIME"] = "1";
        using var child = System.Diagnostics.Process.Start(start)!;
        var output = child.StandardOutput.ReadToEndAsync();
        var errors = child.StandardError.ReadToEndAsync();
        child.StandardInput.Close();
        await child.WaitForExitAsync();
        await errors;
        Assert.Equal(130, child.ExitCode);
        Assert.Equal("", await output);
        Assert.False(Directory.Exists(Path.Combine(scope.Path, "state", "requests")));
    }

    private static Task<(int Exit, string Output, string Error)> Run(string root, string path, params string[] arguments) =>
        StatechartOfflineProcess.Run(StatechartOfflineProcess.Start(root, path, arguments));
}
