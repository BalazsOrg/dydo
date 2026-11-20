namespace DynaDocs.Tests.SkillBuild;

using DynaDocs.SkillBuild;
using DynaDocs.Tests.SkillBuild.Truths;

/// <summary>
/// DYD-1372 A1 to A4 (DYD-1295 317be5f7): on a truth-checked skill the four truth questions are fields
/// the build checks, each check red through <see cref="SkillBuilder.Build"/> or <see cref="Program.Run"/>
/// on the truth fixture: a line's reader, a claim's evidence, layer and reason, the commands it names,
/// a claim still open, and a code path given as evidence.
/// </summary>
public class TruthFieldTests
{
    private const string Spine = "skills/roles/crew/keeper/SKILL.md";

    [Fact]
    [Trait("Job", "contract: DYD-1372 A3a a line read by an actor its skill does not belong to fails the build (DYD-1295 41a553f1)")]
    public void LineForAnotherReader_FailsTheBuild() =>
        AssertRejected(typeof(KeeperWrongReader),
            "keeper: the line of claim:KeeperWrongReader.Landing is read by KeeperActor.Lander, and this skill belongs to KeeperActor.Keeper; " +
            "a line belongs in the skill of the reader who acts on it");

    [Fact]
    [Trait("Job", "contract: DYD-1372 A2 a truth-checked skill's line that names no reader fails the build")]
    public void LineWithNoReader_FailsTheBuild() =>
        AssertRejected(typeof(KeeperNoReader), "keeper: the line of claim:KeeperNoReader.Anyone names no reader");

    [Fact]
    [Trait("Job", "contract: DYD-1372 A2 a claim a truth-checked skill states with no truth fields fails the build")]
    public void ClaimWithNoTruth_FailsTheBuild() =>
        AssertRejected(typeof(KeeperNoTruth), "claim:KeeperNoTruth.Bare is stated by keeper, a truth-checked skill, and carries no Truth");

    [Theory]
    [Trait("Job", "contract: DYD-1372 A3d an empty evidence, layer or reason fails the build")]
    [InlineData(typeof(KeeperEmptyEvidence), "claim:KeeperEmptyEvidence.Blank has an empty evidence")]
    [InlineData(typeof(KeeperEmptyLayer), "claim:KeeperEmptyLayer.Blank has an empty layer")]
    [InlineData(typeof(KeeperEmptyReason), "claim:KeeperEmptyReason.Blank has an empty reason")]
    public void EmptyTruthField_FailsTheBuild(Type kind, string message) => AssertRejected(kind, message);

    [Theory]
    [Trait("Job", "contract: DYD-1372 A3d a reason that only restates its rule or its spec citation fails the build")]
    [InlineData(typeof(KeeperRestated), nameof(KeeperRestated.Echo))]
    [InlineData(typeof(KeeperSpecOnly), nameof(KeeperSpecOnly.Cited))]
    public void ReasonThatOnlyRestates_FailsTheBuild(Type kind, string member) =>
        AssertRejected(kind, $"claim:{kind.Name}.{member}: its reason only restates its rule or its spec citation");

    [Theory]
    [Trait("Job", "contract: DYD-1372 A3b a command the CLI's command tree does not hold, a literal word where it takes an argument, or an option it lacks fails the build (DYD-1295 71e649c0)")]
    [InlineData(typeof(KeeperMissingCommand), "the line of claim:KeeperMissingCommand.Usage names `dydo usage`: `usage` is no subcommand of `dydo`")]
    [InlineData(typeof(KeeperLiteralArgument),
        "the line of claim:KeeperLiteralArgument.Board names `dydo survey board <project>`: `board` is no subcommand of `dydo survey`; an argument is written `<placeholder>`")]
    [InlineData(typeof(KeeperBadOption), "the line of claim:KeeperBadOption.Flag names `dydo check --no-such-flag`: `--no-such-flag` is no option of `dydo check`")]
    [InlineData(typeof(KeeperCommandEvidence), "claim:KeeperCommandEvidence.Gate: its command evidence names `dydo test gate`: `test` is no subcommand of `dydo`")]
    public void CommandTheCliDoesNotHave_FailsTheBuild(Type kind, string message) => AssertRejected(kind, message);

    [Fact]
    [Trait("Job", "edge: DYD-1372 A3b a real command with its argument as a placeholder and its own option builds, in a line and as evidence")]
    public void RealCommandsWithPlaceholders_Build()
    {
        var spine = SkillBuilder.Build(TruthModel.With()).Files[Spine];

        Assert.Contains("`dydo survey <project>`", spine);
        Assert.Contains("`dydo merge <pr> --dry-run`", spine);
        Assert.Equal(EvidenceKind.Command, KeeperMoves.Scale.Truth!.Evidence.Kind);
    }

    [Fact]
    [Trait("Job", "contract: DYD-1372 A3c an open claim's line is not rendered, mapped or lost: the build lists it for review")]
    public void OpenClaim_IsWithheldFromTheSkillAndListed()
    {
        var tree = SkillBuilder.Build(TruthModel.With(typeof(KeeperOpen)));

        Assert.DoesNotContain("Ring the bell", string.Concat(tree.Files.Where(file => file.Key.StartsWith("skills/", StringComparison.Ordinal)).Select(file => file.Value)));
        Assert.DoesNotContain(tree.Map, line => line.Claims.Contains("KeeperOpen.Unseen"));
        Assert.Equal(SkillBuilder.Build(TruthModel.With()).Files[Spine], tree.Files[Spine]);
        Assert.Equal(StatechartFilesTests.Text("""
            ---
            area: reference
            type: reference
            ---
            <!-- built from the model; edit there: tools/SkillBuild -->

            # Open claims: keeper

            Claims of `keeper` that nobody has verified yet. The skill does not state them: each line below is withheld until its claim has evidence.

            ## KeeperOpen.Unseen

            `claim:KeeperOpen.Unseen`: The keeper rings the bell at noon.

            Open: Nobody has watched a noon yet.

            Withheld from Extra:

            > Ring the bell at noon.
            """), tree.Files["dydo/reference/claim-review/keeper-open.md"]);
    }

    [Fact]
    [Trait("Job", "edge: DYD-1372 A3c a truth-checked skill with no open claim says so in its list")]
    public void SkillWithNoOpenClaim_ListsNone() =>
        Assert.EndsWith("is withheld until its claim has evidence.\n\nNone.\n", SkillBuilder.Build(TruthModel.With()).Files["dydo/reference/claim-review/keeper-open.md"]);

    [Fact]
    [Trait("Job", "trap: DYD-1372 A3c an open claim on a line that also states a verified one could not be withheld alone, so it fails the build")]
    public void OpenClaimOnACombinedLine_FailsTheBuild() =>
        AssertRejected(typeof(KeeperOpenCombined), "the line of claim:KeeperOpenCombined.Unseen also states claim:KeeperOpenCombined.Seen, and one of them is open");

    [Fact]
    [Trait("Job", "contract: DYD-1372 A4 a kind's why doc shows each truth-checked claim's evidence, layer and reason, and each line's reader")]
    public void WhyDoc_ShowsEachClaimsAnswersAndItsLinesReader()
    {
        var doc = SkillBuilder.Build(TruthModel.With(typeof(KeeperOpen))).Files;

        Assert.Contains(
            "## Scale\n\n`claim:KeeperMoves.Scale`: Seats scale with the work.\n\nEvidence (command): `dydo survey <project>`\n\n" +
            "Layer: Nothing below the skill covers it.\n\nReason: Holds the fixture on its path.\n\n**keeper** (Seats; read by Keeper):\n\n" +
            "> Scale the seats to the work `dydo survey <project>` shows.\n", doc["dydo/reference/claims/keeper-moves.md"]);
        Assert.Contains("Evidence (run): A fixture run showed it.\n", doc["dydo/reference/claims/keeper-moves.md"]);
        Assert.Contains("Evidence: open, so no skill states this claim yet. Nobody has watched a noon yet.\n", doc["dydo/reference/claims/keeper-open.md"]);
    }

    [Fact]
    [Trait("Job", "contract: DYD-1372 A3e a code evidence naming a path the repository does not hold fails the run, which writes nothing")]
    public void CodeEvidenceNamingNoPath_FailsTheRun()
    {
        using var root = new TempRoot();

        var (exit, _, error) = root.Run(TruthModel.With(typeof(KeeperCodePath)));

        Assert.Equal(1, exit);
        Assert.Equal("SkillBuild failed: claim:KeeperCodePath.Filed: its code evidence, docs/keeper.md, is no path in the repository\n", error.ReplaceLineEndings("\n"));
        Assert.Empty(root.Files());
    }

    [Fact]
    [Trait("Job", "edge: DYD-1372 A3e a code evidence naming a path the repository holds lets the run go on")]
    public void CodeEvidenceNamingAPath_TheRunGoesOn()
    {
        using var root = new TempRoot();
        root.Write("docs/keeper.md", "# Keeper\n");

        var (exit, _, error) = root.Run(TruthModel.With(typeof(KeeperCodePath)));

        Assert.True(exit == 0, error);
        Assert.Contains("File your notes in the docs.", root.Files()[Spine]);
    }

    private static void AssertRejected(Type kind, string message) =>
        Assert.Contains(message, Assert.Throws<InvalidDataException>(() => SkillBuilder.Build(TruthModel.With(kind))).Message);
}
