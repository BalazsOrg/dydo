namespace DynaDocs.Tests.Commands;

using System.Text.Json;
using DynaDocs.Commands;
using DynaDocs.Models;
using DynaDocs.Services;
using DynaDocs.Tests.Integration;
using DynaDocs.Utils;

/// <summary>
/// Command nudges at the hook entry: a block refuses every try, a warn refuses once and lets the
/// identical command through, a notice never refuses; the retry is keyed by the command, whitespace
/// aside. The guard leaves no dydo state outside a project's dydo root (DYD-39). One edge stays below
/// the hook: a patch target on a network-share checkout, which no test machine can mount.
/// </summary>
[Collection("Integration")]
public class GuardCommandTests : GuardHookTestBase
{
    private void WriteNudges(params (string Pattern, string Message, string Severity)[] nudges)
    {
        var json = string.Join(",", nudges.Select(n =>
            $$"""{"pattern": {{JsonSerializer.Serialize(n.Pattern)}}, "message": {{JsonSerializer.Serialize(n.Message)}}, "severity": "{{n.Severity}}"}"""));
        WriteFile("dydo.json", $$"""{ "version": 1, "structure": { "root": "dydo" }, "nudges": [{{json}}] }""");
    }

    [Theory]
    [Trait("Job", "contract: DR 041 nudge severities, block refuses, warn refuses once, notice never")]
    [InlineData("block", true, true)]
    [InlineData("warn", true, false)]
    [InlineData("notice", false, false)]
    public async Task NudgeSeverity_DecidesEveryTry(string severity, bool firstRefused, bool retryRefused)
    {
        await InitProjectAsync("none");
        WriteNudges(("risky-command", "Mind the risk.", severity));

        var first = await ShellAsync("Bash", "risky-command --now");
        var retry = await ShellAsync("Bash", "risky-command --now");
        var third = await ShellAsync("Bash", "risky-command --now");

        Assert.Equal(firstRefused, IsRefused("Bash", first));
        Assert.Equal(retryRefused, IsRefused("Bash", retry));
        Assert.Equal(firstRefused, IsRefused("Bash", third));
    }

    // DYD-503/DYD-499: a warn's pass is the command's own. Another command meets the warn afresh and
    // leaves the first's pass alone; the first passes when retried the same up to whitespace only.
    [Theory]
    [Trait("Job", "contract: DYD-499 retry this exact command, the warn is keyed by the command")]
    [InlineData("pnpm  add   left-pad", true)]
    [InlineData("pnpm add\tleft-pad\n", true)]
    [InlineData("  pnpm add left-pad\r\n", true)]
    [InlineData("pnpm add Left-Pad", false)]
    [InlineData("PNPM add left-pad", false)]
    [InlineData("pnpm add left-pad2", false)]
    public async Task WarnRetry_PassesOnlyForTheSameCommandUpToWhitespace(string retry, bool passes)
    {
        await InitProjectAsync("none");

        AssertRefused("Bash", await ShellAsync("Bash", "pnpm add left-pad"));
        AssertRefused("Bash", await ShellAsync("Bash", "pnpm add other"));
        Assert.Equal(!passes, IsRefused("Bash", await ShellAsync("Bash", retry)));
        AssertAllowed("Bash", await ShellAsync("Bash", "pnpm add other"));
    }

    // A call meeting two warns passes only once both are armed, and the pass spends both (DYD-386).
    [Fact]
    [Trait("Job", "contract: DYD-386 every warn on a call is met before the call passes")]
    public async Task TwoWarnsOnOneCall_PassTogether_AndSpendTogether()
    {
        await InitProjectAsync("none");
        WriteNudges(("risky-alpha", "Warning A", "warn"), ("risky-beta", "Warning B", "warn"));
        const string command = "risky-alpha risky-beta";

        Assert.True(IsRefused("Bash", await ShellAsync("Bash", command)));
        AssertAllowed("Bash", await ShellAsync("Bash", command));
        Assert.True(IsRefused("Bash", await ShellAsync("Bash", command)));
    }

    // DYD-406 GS-03: a configured command nudge fires on a launch the shell runs, never on the same
    // words in text a command reads: the retro's own process listings and greps, a comment, a body.
    // The nudges are LC's dydo.json ones the retro's refusals came from (LC master fe021e2e).
    private void WriteLcNudges() => WriteNudges(
        (@"(?:^|[;&|]\s*)dotnet\s+test\b(.*)", "Use the one test command instead.", "warn"),
        (@"(?:^|[;&|]\s*)cs2ts\b(.*)", "Use the wrapper script instead.", "warn"),
        (@"(?:^|[;&|]\s*)npm\s+(.*)", "Use pnpm, not npm. Try: pnpm $1", "warn"),
        (@"(?:^|[;&|]\s*)(?:&\s+)?(?:""[^""]*(?:LC[.]AppHost|LC[.]Api)[.]exe\b""|'[^']*(?:LC[.]AppHost|LC[.]Api)[.]exe\b'|[^\s;&|]*(?:LC[.]AppHost|LC[.]Api)[.]exe\b)",
            "Don't launch LC.AppHost or LC.Api executables directly.", "block"));

    [Theory]
    [Trait("Job", "trap: DYD-406 GS-03 a nudge fired on launch words in text a command reads")]
    [InlineData("", GuardRetroCommands.Gs03ProcessListing)]
    [InlineData("", GuardRetroCommands.Gs03RgPattern)]
    [InlineData("Bash", GuardRetroCommands.Gs03QuotedProcessListing)]
    [InlineData("PowerShell", GuardRetroCommands.Gs03PowerShellProcessListing)]
    [InlineData("", GuardRetroCommands.Gs03GitGrepPattern)]
    [InlineData("", GuardRetroCommands.Gs03LogGrep)]
    [InlineData("", "echo ready # then; dotnet test")]
    [InlineData("", "cat > notes.md <<'EOF'\nrun it | dotnet test\nEOF")]
    [InlineData("", "Write-Output @'\nthen; npm install\n'@")]
    public async Task LaunchWordsInTextACommandReads_MeetNoNudge(string lane, string command)
    {
        await InitProjectAsync("none");
        WriteLcNudges();

        AssertAllowed(lane, await ShellAsync(lane, command));
    }

    [Theory]
    [Trait("Job", "trap: DYD-406 GS-03 a real launch keeps its nudge's severity")]
    [InlineData("dotnet test --filter Foo", false)]
    [InlineData("git status; dotnet test --no-build", false)]
    [InlineData("& dotnet test", false)]
    [InlineData("rg -l foo | cs2ts --check", false)]
    [InlineData("npm install left-pad", false)]
    [InlineData("git status && npm run lint", false)]
    [InlineData("echo \"unbalanced; dotnet test", false)]
    [InlineData("& \"C:\\Program Files\\LC\\LC.AppHost.exe\" --urls http://x", true)]
    [InlineData("'C:/x y/LC.Api.exe' --urls http://x", true)]
    [InlineData("C:/LC/bin/LC.Api.exe", true)]
    public async Task RealLaunch_MeetsItsNudge(string command, bool block)
    {
        await InitProjectAsync("none");
        WriteLcNudges();

        AssertRefused("", await ShellAsync("", command));
        Assert.Equal(block, IsRefused("", await ShellAsync("", command)));
    }

    // DYD-39: the guard's daily validation writes under the project's dydo root, never a dydo folder
    // in the directory it runs from, and outside a project, or with a dydo.json it cannot read, none.
    [Theory]
    [Trait("Job", "trap: DYD-39 daily validation created dydo state outside the dydo root")]
    [InlineData("nested directory of a project")]
    [InlineData("no dydo.json")]
    [InlineData("malformed dydo.json")]
    public async Task Guard_LeavesNoDydoStateOutsideTheDydoRoot(string where)
    {
        var root = TestDir;
        if (where == "nested directory of a project") await InitProjectAsync("none");
        else if (where == "malformed dydo.json") WriteFile("dydo.json", "{not valid json}");
        var nested = Path.Combine(root, "src", "feature");
        Directory.CreateDirectory(nested);
        var dydoBefore = Directory.Exists(Path.Combine(root, "dydo"));

        Environment.CurrentDirectory = nested;
        try
        {
            await ShellAsync("Bash", "git status");
        }
        finally
        {
            Environment.CurrentDirectory = root;
        }

        Assert.False(Directory.Exists(Path.Combine(nested, "dydo")));
        Assert.Equal(dydoBefore, Directory.Exists(Path.Combine(root, "dydo")));
    }

    // A checkout on a network share cannot be mounted here, so the seam is faked: the project root
    // the off-limits service relativizes against is a UNC string, and each target goes through
    // ResolvePatchTargets, the one resolution Decide makes before the main and worker lanes split,
    // then the per-target off-limits and protected checks both lanes run on it (DYD-396).
    private const string UncProjectRoot = "//server/share/repo";

    private sealed class FixedRootConfigService(string root) : IConfigService
    {
        public string? FindConfigFile(string? startPath = null) => null;
        public DydoConfig? LoadConfig(string? startPath = null) => null;
        public void SaveConfig(DydoConfig config, string path) { }
        public string? GetProjectRoot(string? startPath = null) => root;
        public string GetDydoRoot(string? startPath = null) => root + "/dydo";
        public string GetDocsPath(string? startPath = null) => root + "/dydo";
        public string GetChangelogPath(string? startPath = null) => root + "/dydo";
    }

    private static int? JudgePatchTarget(string cwd, string target)
    {
        var offLimits = new OffLimitsService(new FixedRootConfigService(UncProjectRoot));
        var resolved = Assert.Single(GuardCommand.ResolvePatchTargets([new WriteTarget(target, WriteTargetKind.Delete)], cwd));
        var stderr = Console.Error;
        Console.SetError(TextWriter.Null);
        try
        {
            return GuardCommand.BlockIfPathOffLimits(resolved.Path, offLimits) ?? GuardCommand.BlockIfPathProtected(resolved.Path, offLimits);
        }
        finally
        {
            Console.SetError(stderr);
        }
    }

    // Forward-slash UNC forms are absolute on every platform; backslash ones and a share root's '..'
    // are Windows forms, run where they mean a UNC path.
    [SkippableTheory]
    [Trait("Job", "edge: DYD-396 a patch target on a UNC checkout keeps its share root through '..'")]
    [InlineData("//server/share/repo", "dydo.json", true, false)]
    [InlineData("//server/share/repo/src", "../dydo/_system/x.md", true, false)]
    [InlineData(@"\\server\share\repo", "dydo.json", true, true)]
    [InlineData(@"\\server\share\repo\src", "../dydo.json", true, true)]
    [InlineData(@"\\server\share\repo", @"\\server\share\repo\dydo.json", true, true)]
    [InlineData(@"\\elsewhere\x", "//server/share/repo/src/../dydo.json", true, true)]
    [InlineData(@"\\server\share\repo", @"dydo\_system\.local\state.json", true, true)]
    [InlineData(@"\\server\share", "../../repo/dydo.json", true, true)]
    [InlineData(@"\\server\share\repo", "src/Foo.cs", false, true)]
    public void PatchTargetOnAUncCheckout_IsJudgedAgainstTheShare(string cwd, string target, bool refused, bool windowsForm)
    {
        Skip.If(windowsForm && !OperatingSystem.IsWindows(), "A backslash UNC path is a Windows form.");

        Assert.Equal(refused ? ExitCodes.ToolError : null, JudgePatchTarget(cwd, target));
    }
}
