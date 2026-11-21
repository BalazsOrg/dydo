namespace DynaDocs.Tests.SkillBuild;

using DynaDocs.SkillBuild;

/// <summary>
/// DYD-1372 phase 1, gap hunt 4 (X16, fix g4): the gaps a walk of one feature through every holder
/// found, each as the rule that closes it, read from the production model through
/// <see cref="SkillBuilder.Build"/>. A rule is stated by claim id, path and line order, by a link, by a
/// tool's or a command's own name, or by a word that is Linear's or the brief's, never by a sentence of
/// the skill (DR 067). Claims are named by id, so this file compiles against the model as it was
/// before the fix, where every test here is red.
/// </summary>
public class TicketCaptainGap4Tests
{
    private const string Folder = "skills/roles/officers/ticket-captain/";
    private const string Spine = Folder + "SKILL.md";
    private const string Resources = Folder + "resources/";

    private static readonly Lazy<BuiltTree> Built = new(() => SkillBuilder.Build(ProductionModel.Model));

    [Fact]
    [Trait("Job", "contract: DYD-1372 g4 G1 R21 K14 a Merge step exists only for the holder its brief gives the merge to: the line that cuts the steps, the line that ends a wake and the lead of the merging resource each tie the Merge to the brief")]
    public void MergeStep_ExistsOnlyWhereTheBriefGivesTheMerge()
    {
        var cut = LineOf("CaptainSplit.MergeIfMine");
        var wake = LineOf("CaptainWaking.NoMerge");
        var lead = LineOf("CaptainMerging.Mine");

        Assert.Equal(LineOf("CaptainSplit.Steps"), cut);
        Assert.Equal(LineOf("CaptainWaking.MergeNext"), wake);
        Assert.Equal(Resources + "merging.md", lead.Path);
        Assert.True(lead.Line < LineOf("CaptainMerging.Begin").Line, "the Merge is started above the line that says whose it is");
        Assert.All(new[] { cut, wake, lead }, line => Assert.Contains("brief", line.Text));
        // The finish itself waits with the dispatches: a holder whose officer merges returns only after the unneeded step is cancelled.
        Assert.True(LineOf("CaptainWaking.Unneeded").Line < wake.Line, "a wake ends before the step it made unnecessary is cancelled");
    }

    [Fact]
    [Trait("Job", "contract: DYD-1372 g4 G2 S6 H12 a rewritten contract that cannot be delivered leaves its ticket to the officer above: the route stands between the send and the cut of what is left, and links the step that sends to the officer's thread")]
    public void UndeliveredContract_GoesToTheOfficerAbove()
    {
        var undelivered = LineOf("CaptainReCut.Undelivered");

        Assert.Equal(Resources + "too-big.md", undelivered.Path);
        Assert.True(LineOf("CaptainTrouble.Handed").Line < undelivered.Line && undelivered.Line < LineOf("CaptainTrouble.ChildReCut").Line,
            "the failed send is routed before the send, or after what is left is cut");
        Assert.Contains("](need-for-the-user.md)", undelivered.Text);
    }

    [Fact]
    [Trait("Job", "contract: DYD-1372 g4 G3 S17 a should fixed inside a Fix is closed by the Fix's holder: the act stands in the Fix part of the implement resource, below the line that bounds what a Fix closes, and the Fix brief names each should by its key")]
    public void ShouldFixedInAFix_IsClosedByTheFixsHolder()
    {
        var closed = LineOf("CaptainRound.ShouldInFix");

        Assert.Equal(Resources + "implement.md", closed.Path);
        Assert.True(LineOf("CaptainImplement.Only").Line < closed.Line && closed.Line < LineOf("CaptainImplement.Fixed").Line, "the should is closed outside the Fix's own part");
        Assert.Contains("Done", closed.Text);
        Assert.Contains("key", LineOf("CaptainBriefs.Change").Text);
    }

    [Fact]
    [Trait("Job", "contract: DYD-1372 g4 G4 R20 S12 a merge made again after a revert has a Merge record of its own: the repair is opened as steps with a new Merge blocked behind them, the new PR is opened before its Review, and the merge runs by the merging resource")]
    public void MergeAfterARevert_HasItsOwnMergeRecord()
    {
        var steps = new[] { "CaptainMergeTrouble.Cancelled", "CaptainMergeTrouble.NewMerge", "CaptainMergeTrouble.Reland", "CaptainMergeTrouble.Scoped" }.Select(LineOf).ToArray();

        Assert.All(steps, step => Assert.Equal(Resources + "red-gate.md", step.Path));
        Assert.Equal(steps.Select(step => step.Line).Order().Distinct(), steps.Select(step => step.Line));
        Assert.Equal(LineOf("CaptainMergeTrouble.Repair"), steps[1]);
        Assert.All(new[] { "`save_issue`", "Merge" }, word => Assert.Contains(word, steps[1].Text));
        Assert.Contains("PR", steps[2].Text);
        Assert.Contains("](merging.md)", steps[3].Text);
    }

    [Fact]
    [Trait("Job", "contract: DYD-1372 g4 G5 R17 R23 a head that is to be merged is reviewed in its PR: the review brief's PR line says the PR comes first and when a candidate has none, and the merging resource says a PASS outside the PR merges nothing")]
    public void HeadToBeMerged_IsReviewedInItsPr()
    {
        var first = LineOf("CaptainBriefs.PrFirst");
        var outside = LineOf("CaptainMerging.NoPass");

        Assert.Equal(LineOf("CaptainBriefs.Pr"), first);
        Assert.Contains("`dydo merge`", first.Text);
        Assert.Equal(Resources + "merging.md", outside.Path);
        Assert.True(outside.Line > LineOf("CaptainMerging.Merge").Line, "the refusal is explained above the step that meets it");
        Assert.Equal(Spine, LineOf("CaptainReview.Pr").Path);
        Assert.True(LineOf("CaptainReview.Pr").Line < LineOf("CaptainReview.Whole").Line, "the spine dispatches the Review above the line that opens the PR");
    }

    [Fact]
    [Trait("Job", "contract: DYD-1372 g4 G6 Y9 R22 a flake after a merge is noted in one line on the Merge step, or on the holder's own ticket where there is none: the note stands between the flake's line and the revert")]
    public void Flake_IsNotedOnTheMergeStep()
    {
        var note = LineOf("CaptainMergeTrouble.FlakeNote");

        Assert.Equal(Resources + "red-gate.md", note.Path);
        Assert.True(LineOf("CaptainMergeTrouble.Flake").Line <= note.Line && note.Line < LineOf("CaptainMergeTrouble.Revert").Line, "the flake is noted after the revert, or above the line that names it");
        Assert.All(new[] { "Merge sub-ticket", "own ticket" }, record => Assert.Contains(record, note.Text));
    }

    // The built line of the folder that states a claim; a table's is its first.
    private static MapLine LineOf(string claim)
    {
        var stated = Built.Value.Map.Where(line => line.Path.StartsWith(Folder, StringComparison.Ordinal) && line.Claims.Contains(claim)).MinBy(line => line.Line);
        Assert.True(stated is not null, $"no line of the skill states claim:{claim}");
        return stated!;
    }
}
