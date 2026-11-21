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
        Assert.Equal(
            [
                Review + "keeper-ledger-1.md", Review + "keeper-ledger-2.md", Review + "keeper-ledger-3.md", Review + "keeper-open.md", Review + "legacy-commands.md",
                Review + "not-yet-migrated.md"
            ],
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
    [Trait("Job", "contract: DYD-1372 A7 r1 N1 a legacy line kept in an open claim, which no built file states, reads withheld (open), never kept")]
    public void LineKeptInAnOpenClaim_ReadsWithheld()
    {
        var files = SkillBuilder.Build(TruthModel.Replacing(
            [.. TruthModel.Entries.Where(fate => fate.Line != OldMoves.Owns && fate.Line != OldMoves.Cap),
                Fate.Kept(OldMoves.Owns, KeeperMoves.Owns, KeeperOpen.Unseen), Fate.Kept(OldMoves.Cap, KeeperOpen.Unseen)],
            typeof(KeeperOpen))).Files;

        Assert.EndsWith("`claim:OldMoves.Owns`, kept in `claim:KeeperMoves.Owns`; withheld (open) in `claim:KeeperOpen.Unseen`\n\n> You keep one thing.\n",
            files[Review + "keeper-ledger-1.md"]);
        Assert.Contains("`claim:OldMoves.Cap`, withheld (open) in `claim:KeeperOpen.Unseen`\n\n> Cap the runs at eight.\n", files[Review + "keeper-ledger-2.md"]);
    }

    [Fact]
    [Trait("Job", "contract: DYD-1372 g5 N3 a legacy line kept in the placeholder's claim, whose text no built file states yet, reads placeholder, never kept, between what is kept and what is withheld")]
    public void LineKeptInThePlaceholdersClaim_ReadsPlaceholder()
    {
        var files = SkillBuilder.Build(TruthModel.Replacing(
            [.. TruthModel.Entries.Where(fate => fate.Line != OldMoves.Owns && fate.Line != OldMoves.Cap),
                Fate.Kept(OldMoves.Owns, KeeperMoves.Owns, KeeperSecondOpen.Dusk, KeeperPlaceholder.Bells), Fate.Kept(OldMoves.Cap, KeeperPlaceholder.Bells)],
            typeof(KeeperPlaceholder), typeof(KeeperSecondOpen))).Files;

        Assert.EndsWith(
            "`claim:OldMoves.Owns`, kept in `claim:KeeperMoves.Owns`; placeholder in `claim:KeeperPlaceholder.Bells`; withheld (open) in `claim:KeeperSecondOpen.Dusk`\n\n> You keep one thing.\n",
            files[Review + "keeper-ledger-1.md"]);
        Assert.Contains("`claim:OldMoves.Cap`, placeholder in `claim:KeeperPlaceholder.Bells`\n\n> Cap the runs at eight.\n", files[Review + "keeper-ledger-2.md"]);
    }

    [Fact]
    [Trait("Job", "contract: DYD-1372 A7 r2 L1 a kept line says what became of each clause its claims leave out: dropped, deferred or withheld, printed under the line")]
    public void ClausesOfAKeptLine_ArePrintedUnderIt()
    {
        var kept = Fate.Kept(OldMoves.Cap, KeeperMoves.Limit)
            .Dropped("at eight", Question.Reason, "Nobody knows where eight came from.")
            .Deferred("the runs", "Its manual is not rebuilt here.")
            .Withheld("Cap", KeeperOpen.Unseen);

        var ledger = SkillBuilder.Build(TruthModel.Replacing([.. TruthModel.Entries.Where(fate => fate.Line != OldMoves.Cap), kept], typeof(KeeperOpen)))
            .Files[Review + "keeper-ledger-2.md"];

        Assert.Contains(
            "`claim:OldMoves.Cap`, kept in `claim:KeeperMoves.Limit`\n\n> Cap the runs at eight.\n\n" +
            "A clause of it, dropped, it fails Reason: Nobody knows where eight came from.\n\n> at eight\n\n" +
            "A clause of it, deferred: Its manual is not rebuilt here.\n\n> the runs\n\n" +
            "A clause of it, withheld (open) in `claim:KeeperOpen.Unseen`\n\n> Cap\n\n", ledger);
    }

    [Fact]
    [Trait("Job", "trap: DYD-1372 A7 r2 L1 a clause the legacy line does not hold word for word, a part of a word, or a blank one, fails the build")]
    public void ClauseNotQuotedFromItsLine_FailsTheBuild()
    {
        AssertRejected(Swap(OldMoves.Cap, Fate.Kept(OldMoves.Cap, KeeperMoves.Limit).Dropped("a", Question.Reason, "Nobody knows.")),
            "keeper: the clause \"a\" of claim:OldMoves.Cap is not in its legacy line; a clause is quoted from the line, word for word");
        AssertRejected(Swap(OldMoves.Cap, Fate.Kept(OldMoves.Cap, KeeperMoves.Limit).Dropped("at eigh", Question.Reason, "Nobody knows.")),
            "keeper: the clause \"at eigh\" of claim:OldMoves.Cap is not in its legacy line");
        AssertRejected(Swap(OldMoves.Cap, Fate.Kept(OldMoves.Cap, KeeperMoves.Limit).Dropped("at nine", Question.Reason, "Nobody knows.")),
            "keeper: the clause \"at nine\" of claim:OldMoves.Cap is not in its legacy line; a clause is quoted from the line, word for word");
        AssertRejected(Swap(OldMoves.Cap, Fate.Kept(OldMoves.Cap, KeeperMoves.Limit).Deferred(" ", "Not rebuilt here.")),
            "keeper: the clause \" \" of claim:OldMoves.Cap is not in its legacy line");
    }

    [Fact]
    [Trait("Job", "trap: DYD-1372 A7 r2 L1 a dropped or deferred clause with no reason, and a clause withheld in a claim that is not open, fail the build")]
    public void ClauseWithNoAccount_FailsTheBuild()
    {
        AssertRejected(Swap(OldMoves.Cap, Fate.Kept(OldMoves.Cap, KeeperMoves.Limit).Dropped("at eight", Question.Reason, " ")),
            "keeper: the clause \"at eight\" of claim:OldMoves.Cap has no reason; a dropped or deferred clause says why in one line");
        AssertRejected(Swap(OldMoves.Cap, Fate.Kept(OldMoves.Cap, KeeperMoves.Limit).Deferred("at eight", "")),
            "keeper: the clause \"at eight\" of claim:OldMoves.Cap has no reason");
        AssertRejected(Swap(OldMoves.Cap, Fate.Kept(OldMoves.Cap, KeeperMoves.Limit).Withheld("at eight", KeeperMoves.Owns)),
            "keeper: the clause \"at eight\" of claim:OldMoves.Cap is withheld in a claim that is not an open claim the skill carries");
    }

    [Fact]
    [Trait("Job", "trap: DYD-1372 A7 r3 a clause quoted twice on one entry fails the build: one clause has one fate")]
    public void ClauseQuotedTwice_FailsTheBuild() =>
        AssertRejected(
            Swap(OldMoves.Cap, Fate.Kept(OldMoves.Cap, KeeperMoves.Limit).Dropped("at eight", Question.Reason, "Nobody knows.").Deferred("at eight", "Later.")),
            "keeper: the clause \"at eight\" of claim:OldMoves.Cap is quoted twice; one clause has one fate");

    [Fact]
    [Trait("Job", "edge: DYD-1372 A7 r3 a clause that opens or closes on punctuation is matched as it stands")]
    public void ClauseEdgedByPunctuation_IsPrinted() =>
        Assert.Contains("A clause of it, deferred: Later.\n\n> `dydo survey board` first,\n",
            SkillBuilder.Build(Swap(OldMoves.Survey, Fate.Kept(OldMoves.Survey, KeeperMoves.Scale).Deferred("`dydo survey board` first,", "Later.")))
                .Files[Review + "keeper-ledger-2.md"]);

    [Fact]
    [Trait("Job", "trap: DYD-1372 A7 r2 L1 only a kept line has clauses: one on a dropped or deferred entry fails the build")]
    public void ClauseOnAnEntryThatIsNotAKeptLine_FailsTheBuild() =>
        AssertRejected(Swap(OldMoves.Cap, Fate.Dropped(OldMoves.Cap, Question.Handled, "The proxy limits the runs.").Deferred("at eight", "Later.")),
            "keeper: the ledger gives claim:OldMoves.Cap a clause; only a kept line has clauses");

    [Fact]
    [Trait("Job", "contract: DYD-1372 F r2 S4 a line of a skill not yet migrated that names a resource the replacing skill does not build is listed for review, and the build goes on")]
    public void LegacyLineNamingAnUnbuiltResource_IsListedNotFailed() =>
        Assert.Equal(StatechartFilesTests.Text("""
            ---
            area: reference
            type: reference
            ---
            <!-- built from the model; edit there: tools/SkillBuild -->

            # Lines that name a resource no longer built

            Lines of the model's skills and standards not yet truth-checked that name a resource of a legacy skill which the skill that replaced it does not build. A hand-written file is not read here. Each sends its reader to a file that is not there; these are listed, not hidden, until their skill is switched over or the resource is rebuilt.

            ## clerk

            `claim:ClerkStale.Hand` names `resources/stale.md` of `keeper`, which the skill that replaced the legacy `keeper` does not build:

            > Hand a stale watch to `/keeper`, with its `stale` resource.
            """), SkillBuilder.Build(TruthModel.Replacing(TruthModel.Entries, typeof(ClerkStale))).Files[Review + "unbuilt-resources.md"]);

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
