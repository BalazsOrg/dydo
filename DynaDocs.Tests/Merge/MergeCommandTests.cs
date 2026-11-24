namespace DynaDocs.Tests.Merge;

using DynaDocs.Commands;
using static FakeGitHub;
using static MergeScenario;

/// <summary>The <c>dydo merge</c> command line: both pins are full SHAs, checked before any GitHub call (DR 061 §1-§2).</summary>
[Collection("Integration")]
public sealed class MergeCommandTests : IDisposable
{
    private readonly MergeScenario _s = new();
    private readonly string _originalDirectory = Environment.CurrentDirectory;

    public void Dispose()
    {
        Environment.CurrentDirectory = _originalDirectory;
        _s.Dispose();
    }

    [Fact]
    [Trait("Job", "contract: DR 061 §1; dydo-commands-delivery.md dydo merge (the command line `<pr> --match-head-commit <sha> --match-base-commit <tip> --dry-run`, run in the project's checkout, merges nothing: exit 0, PR #7 read, nothing written)")]
    public void TheCommand_MergesThePrItParsesFromTheCurrentDirectory()
    {
        Environment.CurrentDirectory = _s.Root;
        var command = MergeCommand.Create(_ => _s.GitHub);

        var (exit, _, stderr) = ConsoleCapture.All(() =>
            command.Parse(["7", "--match-head-commit", Sha, "--match-base-commit", Tip, "--dry-run"]).Invoke());

        Assert.True(exit == 0, stderr);
        Assert.Contains(_s.GitHub.Calls, call => call.Path == Pulls);
        Assert.Empty(_s.GitHub.Writes);
    }

    // Proof (e): the base pin is validated like the head pin, before any GitHub call.
    [Theory]
    [Trait("Job", "contract: DR 061 2026-11-04 amendment; dydo-commands-delivery.md dydo merge (a base pin that is not a full 40-character SHA, by row missing, short, long, non-hex or a branch name, refuses before any network call: exit 1, a `refused:` line, no GitHub call)")]
    [InlineData(null)]
    [InlineData("1111111")]
    [InlineData("111111111111111111111111111111111111111")]
    [InlineData("11111111111111111111111111111111111111111")]
    [InlineData("g111111111111111111111111111111111111111")]
    [InlineData("feature/x")]
    public void AnythingButAFullBasePin_IsRefusedBeforeAnyCall(string? pin)
    {
        var (exit, _, stderr) = Invoke(["7", "--match-head-commit", Sha, .. pin == null ? Array.Empty<string>() : ["--match-base-commit", pin]]);

        Assert.Equal(1, exit);
        Assert.StartsWith("refused: ", stderr, StringComparison.Ordinal);
        Assert.Empty(_s.GitHub.Calls);
    }

    // As with the head pin, upper-case hex names the same commit.
    [Fact]
    [Trait("Job", "contract: dydo-commands-delivery.md dydo merge (--match-base-commit takes the full SHA of the tip: the tip's SHA in upper-case hex is that tip, so a dry run exits 0 and writes nothing)")]
    public void AnUpperCaseBasePin_IsTheSameTip()
    {
        const string tip = "abcdef1111111111111111111111111111111111";
        _s.GitHub.On("GET", Branch, Reply(200, BranchAt(tip)));
        _s.GitHub.On("GET", Compare(ReviewBase, tip), Reply(200, """{ "status": "ahead" }"""));

        var (exit, _, stderr) = Invoke(["7", "--match-head-commit", Sha, "--match-base-commit", tip.ToUpperInvariant(), "--dry-run"]);

        Assert.True(exit == 0, stderr);
        Assert.Empty(_s.GitHub.Writes);
    }

    private (int Exit, string Stdout, string Stderr) Invoke(string[] args)
    {
        Environment.CurrentDirectory = _s.Root;
        return ConsoleCapture.All(() => MergeCommand.Create(_ => _s.GitHub).Parse(args).Invoke());
    }

    // The released command, with the real gh behind it: no pins refuses before gh is ever asked.
    [Fact]
    [Trait("Job", "contract: dydo-commands-delivery.md dydo merge (both pins are required: the released command given neither refuses, exit 1, a `refused:` line)")]
    public void WithoutPins_TheReleasedCommandRefuses()
    {
        var (exit, _, stderr) = ConsoleCapture.All(() => MergeCommand.Create().Parse(["7"]).Invoke());

        Assert.Equal(1, exit);
        Assert.StartsWith("refused: ", stderr, StringComparison.Ordinal);
    }
}
