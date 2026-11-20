namespace DynaDocs.Tests.SkillBuild;

using DynaDocs.SkillBuild;
using DynaDocs.Tests.SkillBuild.Charts;
using DynaDocs.Tests.SkillBuild.Slots;

/// <summary>
/// K13: one why doc per claim kind under <c>dydo/reference/claims/</c>, each claim's rule, spec ids,
/// machine move and every skill's line side by side, built only where nothing ships.
/// </summary>
public class WhyDocTests
{
    [Fact]
    [Trait("Job", "contract: K13 a kind's why doc gives each claim's rule, spec ids, move with its machine link, and every skill's line")]
    public void LeaseMovesWhyDoc_SetsEachClaimsLinesSideBySide() => Assert.Equal(StatechartFilesTests.Text("""
        ---
        area: reference
        type: reference
        ---
        <!-- built from the model; edit there: tools/SkillBuild -->

        # Claims: LeaseMoves

        ## HoldsOneLease

        `claim:LeaseMoves.HoldsOneLease`: The holder holds one lease.

        Spec: K9

        **holder** (Job):

        > You hold one lease at a time.

        ## ReviewsReturns

        `claim:LeaseMoves.ReviewsReturns`: The warden reviews what holders return.

        Spec: R1

        **warden** (Job):

        > You review each returned lease.

        ## Take

        `claim:LeaseMoves.Take`: A holder takes a free lease when dispatched.

        Spec: K9

        Transition in Lease (`claims/statechart/lease/Lease.tla`): Free → Held, "the holder takes the lease".

        **holder** (Held, arrival):

        > Take the lease when dispatched.

        ## KeepProofGreen

        `claim:LeaseMoves.KeepProofGreen`: The proof stays green while the lease is held.

        Spec: T3

        **holder** (Held, nominal):

        > Keep the proof green.

        ## Return

        `claim:LeaseMoves.Return`: A held lease returns for review once its proof is green.

        Spec: R1, K28

        Transition in Lease (`claims/statechart/lease/Lease.tla`): Held → Review, "the holder returns it".

        **holder** (Held, exit):

        > Return for review once the proof is green.

        **warden** (Review, arrival):

        > Review what the holder returns.

        ## Fail

        `claim:LeaseMoves.Fail`: A failed review sends the lease back, at most twice.

        Spec: R3

        Transition in Lease (`claims/statechart/lease/Lease.tla`): Review → Held, "the review fails".

        **warden** (Review, exit):

        > Fail the review with its findings.

        **holder** (Held, arrival):

        > Take the findings back to work.

        ## Land

        `claim:LeaseMoves.Land`: A passed review lands by command.

        Spec: R2, CA19

        Transition in Lease (`claims/statechart/lease/Lease.tla`): Review → Done, "the merge lands". A system transition: no skill carries it.
        """), SkillBuilder.Build(ChartModel.With()).Files["dydo/reference/claims/lease-moves.md"]);

    [Fact]
    [Trait("Job", "contract: K13 a combined line shows under each claim it states, and a multi-line line stays one quote")]
    public void WhyDoc_ShowsCombinedAndMultiLineLines()
    {
        var doc = SkillBuilder.Build(DemoModel.With()).Files["dydo/reference/claims/demo-moves.md"];

        Assert.Contains("## ResumeAfterSaving\n\n`claim:DemoMoves.ResumeAfterSaving`: Resume once the work is saved.\n\n" +
                        "**demo** (StaleLeaseSteps, on claim:DemoMoves.SaveTheWork):\n\n> Save the work, then resume.\n", doc);
        Assert.Contains("**demo** (Commands):\n\n> ```bash\n> dydo check\n> ```\n", doc);
    }

    [Fact]
    [Trait("Job", "trap: K13 a skill line's resource link would break under dydo/reference/claims/, so the why doc names its target instead (dydo check)")]
    public void WhyDoc_NamesALinesLinkTargetWithoutLinkingIt()
    {
        var doc = SkillBuilder.Build(ChartModel.With(typeof(LeaseLinked))).Files["dydo/reference/claims/lease-linked.md"];

        Assert.Contains("> Follow the lease guide (`resources/lease-guide.md`) while you hold it.\n", doc);
        Assert.DoesNotContain("](", doc);
    }

    [Fact]
    [Trait("Job", "contract: K13 why docs and chart files are built only under dydo/ and claims/, which never ship")]
    public void WhyDocsAndChartFiles_AreBuiltWhereNothingShips()
    {
        var unshipped = SkillBuilder.Build(ChartModel.With()).Files.Keys.Where(path => !path.StartsWith("skills/", StringComparison.Ordinal)).ToArray();

        Assert.Equal(
        [
            "claims/statechart/lease/Lease.cfg",
            "claims/statechart/lease/Lease.tla",
            "claims/statechart/lease/transitions.json",
            "claims/statechart/lease/view.mjs",
            "dydo/reference/claims/lease-moves.md"
        ], unshipped);
    }
}
