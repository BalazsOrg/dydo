namespace DynaDocs.Tests.SkillBuild;

using DynaDocs.SkillBuild;
using DynaDocs.Tests.SkillBuild.Truths;

/// <summary>
/// DYD-1372 X3, X11 (07c86aad): an open claim is withheld and listed, with one exception. A placeholder,
/// a text that is someone's to write and is not written, renders its one sentence and its token, stays
/// on the open list marked as one, and is the only such claim a model may hold. Through
/// <see cref="SkillBuilder.Build"/> on the truth fixture.
/// </summary>
public class PlaceholderClaimTests
{
    private const string Spine = "skills/roles/crew/keeper/SKILL.md";
    private const string OpenList = "dydo/reference/claim-review/keeper-open.md";

    [Fact]
    [Trait("Job", "contract: DYD-1372 X3 a placeholder's line is rendered, its token alone on a line, and the build maps it to its claim")]
    public void Placeholder_IsRenderedWithItsTokenAloneOnALine()
    {
        var tree = SkillBuilder.Build(TruthModel.With(typeof(KeeperPlaceholder)));

        Assert.Contains($"\nOnce the list stands below, ring each bell on it.\n\n{KeeperPlaceholder.Token}\n", tree.Files[Spine]);
        Assert.Contains(tree.Map, line => line.Text == KeeperPlaceholder.Token && line.Claims.Contains("KeeperPlaceholder.Bells"));
    }

    [Fact]
    [Trait("Job", "contract: DYD-1372 X3 a placeholder stays on the open list, marked as one, with why its text is not written and where its token stands")]
    public void Placeholder_StaysOnTheOpenListMarkedAsOne()
    {
        var list = SkillBuilder.Build(TruthModel.With(typeof(KeeperPlaceholder), typeof(KeeperSecondOpen))).Files[OpenList];

        Assert.Contains("is withheld until its claim has evidence, but for the one placeholder, whose line stands in the skill with its token unfilled.\n", list);
        Assert.Contains(
            "## KeeperPlaceholder.Bells\n\n`claim:KeeperPlaceholder.Bells`: The keeper rings the bells the harbour master lists.\n\n" +
            "Placeholder: The list is the harbour master's to write, and it is not written.\n\n" +
            $"Stated in Extra, with `{KeeperPlaceholder.Token}` where its text will stand:\n\n> Once the list stands below, ring each bell on it.\n", list);
        Assert.Contains("## KeeperSecondOpen.Dusk\n", list);
        Assert.Contains("Withheld from Extra:\n\n> Light the lamp at dusk.\n", list);
    }

    [Fact]
    [Trait("Job", "contract: DYD-1372 X3 a kind's why doc says a placeholder's claim is open and names the token its line shows")]
    public void WhyDoc_SaysAPlaceholderIsOpenAndNamesItsToken()
    {
        var doc = SkillBuilder.Build(TruthModel.With(typeof(KeeperPlaceholder))).Files["dydo/reference/claims/keeper-placeholder.md"];

        Assert.Contains(
            $"Evidence: open, a placeholder: the skill shows `{KeeperPlaceholder.Token}` where this claim's text will stand. The list is the harbour master's to write, and it is not written.\n", doc);
        Assert.Contains("**keeper** (placeholder; Extra; read by Keeper):\n", doc);
    }

    [Fact]
    [Trait("Job", "trap: DYD-1372 X11 a second placeholder fails the build: the roster's is the only exception to withhold-and-list")]
    public void SecondPlaceholder_FailsTheBuild() =>
        AssertRejected(
            "claim:KeeperPlaceholder.Bells and claim:KeeperSecondPlaceholder.Lamps are placeholders; a model renders one open claim as a placeholder, and withholds every other",
            typeof(KeeperPlaceholder), typeof(KeeperSecondPlaceholder));

    [Theory]
    [Trait("Job", "trap: DYD-1372 X3 a placeholder's line that drafts the text it stands for, below its token or in a second sentence above it, or whose token does not read as a placeholder, fails the build")]
    [InlineData(typeof(KeeperDraftedPlaceholder), "{bell_placeholder}")]
    [InlineData(typeof(KeeperTwoSentencePlaceholder), "{bell_placeholder}")]
    [InlineData(typeof(KeeperPlainPlaceholder), "the bells")]
    public void PlaceholderThatShowsMoreThanItsToken_FailsTheBuild(Type kind, string token) =>
        AssertRejected(
            $"keeper: the line of claim:{kind.Name}.Bells is a placeholder's, and it is not one sentence, a blank line, then `{token}` alone, a token of the form `{{name_placeholder}}`",
            kind);

    [Fact]
    [Trait("Job", "edge: DYD-1372 X11 an open claim with no token is withheld as before, beside a placeholder that is rendered")]
    public void OpenClaimBesideAPlaceholder_IsStillWithheld()
    {
        var spine = SkillBuilder.Build(TruthModel.With(typeof(KeeperPlaceholder), typeof(KeeperSecondOpen))).Files[Spine];

        Assert.Contains(KeeperPlaceholder.Token, spine);
        Assert.DoesNotContain("Light the lamp at dusk.", spine);
    }

    private static void AssertRejected(string message, params Type[] kinds) =>
        Assert.Contains(message, Assert.Throws<InvalidDataException>(() => SkillBuilder.Build(TruthModel.With(kinds))).Message);
}
