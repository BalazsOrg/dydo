namespace DynaDocs.Tests.SkillBuild;

using DynaDocs.SkillBuild;
using DynaDocs.Tests.SkillBuild.Truths;

/// <summary>
/// DYD-1372 A6 and A7 (DYD-1295 2119cd65): a truth-checked skill replaces a legacy one through its
/// ledger. The legacy lines stay in the model and are checked, but no file states them; the ledger
/// accounts for every line of the legacy spine and every resource it links, and the build emits it.
/// Through <see cref="SkillBuilder.Build"/> on the truth fixture.
/// </summary>
public class LedgerTests
{
    private const string Folder = "skills/roles/crew/keeper/";
    private const string Review = "dydo/reference/claim-review/";

    private static readonly BuiltTree Built = SkillBuilder.Build(TruthModel.Replacing(TruthModel.Entries));

    [Fact]
    [Trait("Job", "contract: DYD-1372 A6 the replacing skill takes the legacy skill's folder and name, and the legacy files are rendered but never built or mapped")]
    public void ReplacedSkill_IsCheckedButNotBuilt()
    {
        Assert.Equal(SkillBuilder.Build(TruthModel.With()).Files[Folder + "SKILL.md"], Built.Files[Folder + "SKILL.md"]);
        Assert.Equal([Folder + "SKILL.md", Folder + "resources/guide.md", Folder + "resources/stuck.md"],
            Built.Files.Keys.Where(path => path.StartsWith(Folder, StringComparison.Ordinal)));
        Assert.Equal([Folder + "SKILL.md", Folder + "resources/manual.md", Folder + "resources/stale.md"], Built.Unbuilt.Keys);
        Assert.Contains("\n1. Cap the runs at eight.\n", Built.Unbuilt[Folder + "SKILL.md"]);
        Assert.DoesNotContain(Built.Map, line => line.Claims.Any(id => id.StartsWith("OldMoves.", StringComparison.Ordinal) && id != "OldMoves.Desk"));
    }

    [Fact]
    [Trait("Job", "trap: DYD-1372 A6 without a ledger two skills of one name would write one folder, so the build fails")]
    public void TwoSkillsOfOneNameWithNoLedger_FailsTheBuild() =>
        AssertRejected(new([typeof(KeeperMoves), typeof(OldMoves)], [TruthModel.OldKeeper, TruthModel.Keeper, TruthModel.Clerk]) { Machines = [TruthModel.Watch] },
            Folder + "SKILL.md is built twice; two skills share a folder only when one replaces the other through its ledger");

    [Fact]
    [Trait("Job", "contract: DYD-1372 A7 the build emits the ledger, one file per section of the legacy spine: each line and linked resource with its fate")]
    public void Ledger_IsBuiltBySectionOfTheLegacySpine()
    {
        Assert.Equal([Review + "keeper-ledger-1.md", Review + "keeper-ledger-2.md", Review + "keeper-ledger-3.md", Review + "keeper-open.md", Review + "legacy-commands.md"],
            Built.Files.Keys.Where(path => path.StartsWith(Review, StringComparison.Ordinal)));
        Assert.EndsWith("# Ledger: keeper, the job paragraph\n\nWhat became of each line of the legacy `keeper` spine in this section, and of each resource it links.\n\n" +
                        "`claim:OldMoves.Owns`, kept in `claim:KeeperMoves.Owns`\n\n> You keep one thing.\n", Built.Files[Review + "keeper-ledger-1.md"]);
        Assert.Equal(StatechartFilesTests.Text("""
            ---
            area: reference
            type: reference
            ---
            <!-- built from the model; edit there: tools/SkillBuild -->

            # Ledger: keeper, 1. Do it

            What became of each line of the legacy `keeper` spine in this section, and of each resource it links.

            `claim:OldMoves.Cap`, dropped, it fails Handled: The proxy already limits the runs.

            > Cap the runs at eight.

            `claim:OldMoves.Survey`, deferred: Its manual is not rebuilt here.

            > Read `dydo survey board` first, by the manual (`resources/manual.md`).

            Resource `resources/manual.md` (Manual), kept as `resources/guide.md`
            """), Built.Files[Review + "keeper-ledger-2.md"]);
        Assert.EndsWith("# Ledger: keeper, When things go wrong\n\nWhat became of each line of the legacy `keeper` spine in this section, and of each resource it links.\n\n" +
                        "Resource `resources/stale.md` (Stale), deferred: Not rebuilt here.\n", Built.Files[Review + "keeper-ledger-3.md"]);
    }

    [Fact]
    [Trait("Job", "contract: DYD-1372 A7 a legacy spine line or resource with no ledger entry fails the build, naming each one")]
    public void LedgerMissingAnEntry_FailsTheBuildNamingIt()
    {
        AssertRejected(TruthModel.Replacing([.. TruthModel.Entries.Where(fate => fate.Line != OldMoves.Cap && fate.Line != OldMoves.Survey)]),
            "keeper: the ledger has no entry for 2 line(s) of the legacy spine: claim:OldMoves.Cap, claim:OldMoves.Survey; each is kept, dropped or deferred");
        AssertRejected(TruthModel.Replacing([.. TruthModel.Entries.Where(fate => !Equals(fate.Resource, OldSlot.Stale))]),
            "keeper: the ledger has no entry for 1 resource(s) of the legacy spine: OldSlot.Stale");
    }

    [Fact]
    [Trait("Job", "trap: DYD-1372 A7 two entries for one legacy line fail the build")]
    public void LedgerWithTwoEntriesForOneLine_FailsTheBuild() =>
        AssertRejected(TruthModel.Replacing([.. TruthModel.Entries, Fate.Deferred(OldMoves.Cap, "Again.")]), "keeper: the ledger has two entries for the line claim:OldMoves.Cap");

    [Theory]
    [Trait("Job", "trap: DYD-1372 A7 an entry for a claim or resource the legacy spine does not hold fails the build")]
    [InlineData(true, "keeper: a ledger entry names a line the legacy spine does not hold: claim:OldMoves.Manual")]
    [InlineData(false, "keeper: a ledger entry names a resource the legacy spine does not hold: OldSlot.Desk")]
    public void LedgerEntryForSomethingOffTheSpine_FailsTheBuild(bool line, string message) =>
        AssertRejected(TruthModel.Replacing([.. TruthModel.Entries, line ? Fate.Deferred(OldMoves.Manual, "A resource's line.") : Fate.Deferred(OldSlot.Desk, "No resource.")]), message);

    [Fact]
    [Trait("Job", "trap: DYD-1372 A7 a line kept in no claim, or in a claim the replacing skill does not carry, fails the build")]
    public void LineKeptInAClaimTheSkillDoesNotCarry_FailsTheBuild()
    {
        AssertRejected(Swap(OldMoves.Owns, Fate.Kept(OldMoves.Owns, OldMoves.Desk)), "keeper: the ledger keeps claim:OldMoves.Owns in no claim, or in a claim the skill does not carry");
        AssertRejected(Swap(OldMoves.Owns, Fate.Kept(OldMoves.Owns)), "keeper: the ledger keeps claim:OldMoves.Owns in no claim");
    }

    [Fact]
    [Trait("Job", "trap: DYD-1372 A7 a resource kept as a resource the replacing skill does not declare fails the build")]
    public void ResourceKeptAsAResourceTheSkillDoesNotDeclare_FailsTheBuild() =>
        AssertRejected(TruthModel.Replacing([.. TruthModel.Entries.Where(fate => !Equals(fate.Resource, OldSlot.Manual)), Fate.Kept(OldSlot.Manual, OldSlot.Stale)]),
            "keeper: the ledger keeps OldSlot.Manual as a resource the skill does not declare");

    [Fact]
    [Trait("Job", "contract: DYD-1372 A7 a dropped or deferred entry says why")]
    public void DroppedEntryWithNoReason_FailsTheBuild() =>
        AssertRejected(Swap(OldMoves.Cap, Fate.Dropped(OldMoves.Cap, Question.Handled, " ")), "keeper: the ledger gives claim:OldMoves.Cap no reason");

    [Fact]
    [Trait("Job", "trap: DYD-1372 A7 a ledger is a truth-checked skill's account of a legacy skill the model declares")]
    public void LedgerOverASkillTheModelDoesNotDeclare_FailsTheBuild() =>
        AssertRejected(TruthModel.Of(TruthModel.Keeper with { Ledger = new(OldSkill.Keeper) }), "keeper: its ledger accounts for OldSkill.Keeper, which the model does not declare");

    [Fact]
    [Trait("Job", "contract: DYD-1372 A3b a legacy line naming a missing command is listed for review, a replaced skill's too, and the build goes on")]
    public void LegacyLinesNamingAMissingCommand_AreListedNotFailed() =>
        Assert.Equal(StatechartFilesTests.Text("""
            ---
            area: reference
            type: reference
            ---
            <!-- built from the model; edit there: tools/SkillBuild -->

            # Legacy lines that name a missing command

            Lines of the skills and standards not yet truth-checked that name, in code, a `dydo …` command the CLI's command tree does not hold. A truth-checked skill fails the build on such a line; these are listed, not hidden, until their skill is switched over.

            ## keeper (legacy, not built)

            `claim:OldMoves.Survey` names `dydo survey board`: `board` is no subcommand of `dydo survey`; an argument is written `<placeholder>`

            ## clerk

            `claim:OldMoves.Desk` names `dydo usage`: `usage` is no subcommand of `dydo`; an argument is written `<placeholder>`
            """), Built.Files[Review + "legacy-commands.md"]);

    [Fact]
    [Trait("Job", "edge: DYD-1372 A3b a model whose legacy lines name only real commands builds no list")]
    public void ModelNamingOnlyRealCommands_BuildsNoLegacyList() =>
        Assert.DoesNotContain(Review + "legacy-commands.md", SkillBuilder.Build(TruthModel.With()).Files.Keys);

    [Fact]
    [Trait("Job", "contract: DYD-1372 A6 a why doc marks a replaced skill's line as legacy and not built")]
    public void WhyDoc_MarksAReplacedSkillsLines() =>
        Assert.Contains("**keeper** (legacy, not built; Work):\n\n> Cap the runs at eight.\n", Built.Files["dydo/reference/claims/old-moves.md"]);

    private static Model Swap(Claim line, Fate entry) => TruthModel.Replacing([.. TruthModel.Entries.Where(fate => fate.Line != line), entry]);

    private static void AssertRejected(Model model, string message) =>
        Assert.Contains(message, Assert.Throws<InvalidDataException>(() => SkillBuilder.Build(model)).Message);
}
