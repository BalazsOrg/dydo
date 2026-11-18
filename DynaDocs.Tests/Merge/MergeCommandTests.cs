namespace DynaDocs.Tests.Merge;

using DynaDocs.Commands;
using static FakeGitHub;
using static MergeScenario;

/// <summary>The <c>dydo merge</c> command line: both pins are full SHAs, checked before any GitHub call (DR 061 §1-§2).</summary>
[Collection("Integration")]
[Trait("Job", "contract: DR 061 §1-§2; DYD-523")]
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
    public void WithoutPins_TheReleasedCommandRefuses()
    {
        var (exit, _, stderr) = ConsoleCapture.All(() => MergeCommand.Create().Parse(["7"]).Invoke());

        Assert.Equal(1, exit);
        Assert.StartsWith("refused: ", stderr, StringComparison.Ordinal);
    }
}
