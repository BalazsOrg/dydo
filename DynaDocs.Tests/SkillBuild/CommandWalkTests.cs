namespace DynaDocs.Tests.SkillBuild;

using DynaDocs.SkillBuild;
using DynaDocs.Tests.SkillBuild.Truths;

/// <summary>
/// DYD-1372 A3b, review round 1 (G1, G5, G6): the walk over the <c>dydo …</c> commands a truth-checked
/// line names reads every invocation in its code, every word of each, and the prose around the code.
/// Each shape that once built green is red through <see cref="SkillBuilder.Build"/> on the truth fixture.
/// </summary>
public class CommandWalkTests
{
    private const string Spine = "skills/roles/crew/keeper/SKILL.md";

    [Theory]
    [Trait("Job", "trap: DYD-1372 A3b r1 G1 a missing command after &&, ||, a pipe or a semicolon, in a code span or on a fenced line, fails the build")]
    [InlineData(typeof(KeeperChainedAnd), "the line of claim:KeeperChainedAnd.Usage names `dydo usage`: `usage` is no subcommand of `dydo`")]
    [InlineData(typeof(KeeperChainedOr), "the line of claim:KeeperChainedOr.Usage names `dydo usage`: `usage` is no subcommand of `dydo`")]
    [InlineData(typeof(KeeperChainedPipe), "the line of claim:KeeperChainedPipe.Usage names `dydo usage`: `usage` is no subcommand of `dydo`")]
    [InlineData(typeof(KeeperChainedSemicolon), "the line of claim:KeeperChainedSemicolon.Gate names `dydo test gate`: `test` is no subcommand of `dydo`")]
    public void MissingCommandAfterASeparator_FailsTheBuild(Type kind, string message) => AssertRejected(kind, message);

    [Theory]
    [Trait("Job", "trap: DYD-1372 A3b r1 G6 an option only one word of a|b has, a literal word with glued punctuation or a capital, and an option in brackets are each read, and fail the build")]
    [InlineData(typeof(KeeperAlternationOption),
        "the line of claim:KeeperAlternationOption.Flag names `dydo archive restore|branch <id> --path <dir>`: `--path` is no option of `dydo archive branch`")]
    [InlineData(typeof(KeeperGluedLiteral),
        "the line of claim:KeeperGluedLiteral.Board names `dydo survey board…`: `board…` is no subcommand of `dydo survey`; an argument is written `<placeholder>`")]
    [InlineData(typeof(KeeperCapitalLiteral),
        "the line of claim:KeeperCapitalLiteral.Board names `dydo survey Board`: `Board` is no subcommand of `dydo survey`; an argument is written `<placeholder>`")]
    [InlineData(typeof(KeeperBracketOption),
        "the line of claim:KeeperBracketOption.Flag names `dydo check [--no-such-flag]`: `[--no-such-flag]` is no subcommand of `dydo check`; an argument is written `<placeholder>`")]
    public void WordTheWalkOnceSkipped_FailsTheBuild(Type kind, string message) => AssertRejected(kind, message);

    [Fact]
    [Trait("Job", "edge: DYD-1372 A3b r1 G6 an option's value typed in lowercase is its value, and an option both of a|b have holds for each: the line builds")]
    public void OptionValueAndSharedOption_Build()
    {
        var spine = SkillBuilder.Build(TruthModel.With(typeof(KeeperTypedCommands))).Files[Spine];

        Assert.Contains("- Run `dydo merge <pr> --match-head-commit abc123 --dry-run`.\n", spine);
        Assert.Contains("- Run `dydo archive branch|worktree <target> --dry-run`.\n", spine);
    }

    [Fact]
    [Trait("Job", "trap: DYD-1372 A3b r1 G6 a flag takes no value, so a literal word after it is still read as a subcommand and fails the build")]
    public void LiteralAfterAFlag_FailsTheBuild() =>
        AssertRejected(typeof(KeeperFlagValue),
            "the line of claim:KeeperFlagValue.Dry names `dydo merge <pr> --dry-run nonsense`: `nonsense` is no subcommand of `dydo merge`; an argument is written `<placeholder>`");

    [Fact]
    [Trait("Job", "contract: DYD-1372 A3b r1 G5 a command named in plain prose, where no check reads it, fails the build and says to put it in code")]
    public void CommandInPlainProse_FailsTheBuild() =>
        AssertRejected(typeof(KeeperProseCommand),
            "keeper: the line of claim:KeeperProseCommand.Board names `dydo survey` outside code; a command is written in code, where the build checks it against the CLI");

    private static void AssertRejected(Type kind, string message) =>
        Assert.Contains(message, Assert.Throws<InvalidDataException>(() => SkillBuilder.Build(TruthModel.With(kind))).Message);
}
