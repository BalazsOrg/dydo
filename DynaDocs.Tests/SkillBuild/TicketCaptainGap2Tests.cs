namespace DynaDocs.Tests.SkillBuild;

using DynaDocs.SkillBuild;

/// <summary>
/// DYD-1372 phase 1, gap hunt 2 (X16, fix g2): the gaps a second walk of the skill found, each as the
/// rule that closes it, read from the production model through <see cref="SkillBuilder.Build"/>. A
/// rule is stated by claim id, path and line order, by a link, or by a word that is Linear's or the
/// machine's own, never by a sentence of the skill (DR 067). Claims are named by id, so this file
/// compiles against the model as it was before the fix, where every test here is red.
/// </summary>
public class TicketCaptainGap2Tests
{
    private const string Folder = "skills/roles/officers/ticket-captain/";
    private const string Spine = Folder + "SKILL.md";
    private const string Resources = Folder + "resources/";

    private static readonly Lazy<BuiltTree> Built = new(() => SkillBuilder.Build(ProductionModel.Model));

    [Fact]
    [Trait("Job", "contract: DYD-1372 g2 B1 H23 a captain briefs a Question only to have it researched: the brief line for one sends a HITL Question up by the need procedure, where only an AFK sub-ticket is dispatched")]
    public void QuestionBrief_IsOnlyForAQuestionSentToResearch()
    {
        var asked = LineOf("CaptainBriefs.Asked");

        Assert.Contains("HITL", asked.Text);
        Assert.Contains("](need-for-the-user.md)", asked.Text);
        Assert.Contains("AFK", LineOf("CaptainDispatch.Send").Text);
    }

    [Fact]
    [Trait("Job", "contract: DYD-1372 g2 B2 S3 S10 a sub-ticket goes out only once nothing blocks it: at each return a step opened ahead and no longer needed is cancelled before what the return unblocked is dispatched")]
    public void SubTicket_GoesOutOnlyOnceNothingBlocksIt()
    {
        var unneeded = LineOf("CaptainWaking.Unneeded");
        var ready = LineOf("CaptainWaking.Ready");

        Assert.Equal(Resources + "waking.md", ready.Path);
        Assert.Equal(ready.Path, unneeded.Path);
        Assert.True(unneeded.Line < ready.Line, "what a return unblocked is dispatched before the step it made unnecessary is cancelled");
        Assert.All(new[] { ready, LineOf("CaptainDispatch.Send") }, line => Assert.Contains("block", line.Text));
        Assert.Contains("Canceled", unneeded.Text);
    }

    [Fact]
    [Trait("Job", "contract: DYD-1372 g2 B3 H2 S6 a ticket returned too big, re-cut is settled by its officer: its contract is narrowed, it is closed or cancelled, and only then is what it left cut, the move the machine draws from Waiting")]
    public void ReturnedTicket_IsSettledBeforeWhatItLeftIsCut()
    {
        var steps = new[] { "CaptainTrouble.Narrow", "CaptainTrouble.Settle", "CaptainTrouble.ChildReCut" }.Select(LineOf).ToArray();

        Assert.All(steps, step => Assert.Equal(Resources + "too-big.md", step.Path));
        Assert.Equal(steps.Select(step => step.Line).Order(), steps.Select(step => step.Line));
        Assert.Equal(LineOf("CaptainTrouble.ReCut"), LineOf("CaptainTrouble.Left"));
        Assert.Contains("\"claim\": \"claim:CaptainTrouble.ChildReCut\",\n      \"from\": \"Waiting\",\n      \"to\": \"Split\"", Built.Value.Unwritten["claims/statechart/captain/transitions.json"]);
    }

    [Theory]
    [Trait("Job", "contract: DYD-1372 g2 S1 S4 work not cut into steps has no Merge sub-ticket: each line that names that sub-ticket as a record names the holder's own ticket for such work")]
    [InlineData("CaptainMerging.Begin")]
    [InlineData("CaptainMergeTrouble.Saved")]
    [InlineData("CaptainMergeTrouble.Cancelled")]
    public void MergeRecord_IsTheHoldersOwnTicketWhereThereAreNoSteps(string claim)
    {
        var line = LineOf(claim).Text;

        Assert.Contains("Merge sub-ticket", line);
        Assert.Contains("own ticket", line);
    }

    [Fact]
    [Trait("Job", "contract: DYD-1372 g2 S2 S17 a batch of shoulds closes the shoulds it fixes: the act stands in the batch's part of the implement resource, below the move of each should under the batch")]
    public void ShouldsBatch_ClosesEachShouldItFixes()
    {
        var closed = LineOf("CaptainImplement.Fixed");

        Assert.Equal(Resources + "implement.md", closed.Path);
        Assert.True(closed.Line > LineOf("CaptainImplement.Reparent").Line, "a should is closed before it is moved under the batch");
        Assert.Contains("Done", closed.Text);
    }

    [Fact]
    [Trait("Job", "contract: DYD-1372 g2 S3 S7 a holder woken in its own thread is sorted before the intake check: the first line of phase 1 sends it to the waking resource, which tells its own ticket apart and has it re-read the record first")]
    public void WokenHolder_IsSortedBeforeTheIntakeCheck()
    {
        var woken = LineOf("CaptainIntake.Woken");
        var mine = LineOf("CaptainWaking.Mine");
        var acts = new[] { "CaptainWaking.Reread", "CaptainDispatch.Resume", "CaptainDispatch.Route", "CaptainWaking.Ready" }.Select(LineOf).ToArray();

        Assert.Equal(Spine, woken.Path);
        Assert.True(woken.Line < LineOf("CaptainIntake.Check").Line, "a woken holder meets the intake check before it is sorted");
        Assert.Contains($"](resources/{Path.GetFileName(mine.Path)})", woken.Text);
        Assert.All(acts, act => Assert.Equal(mine.Path, act.Path));
        Assert.True(mine.Line < acts[0].Line && acts[0].Line < acts.Skip(1).Min(act => act.Line), "a woken holder acts before it re-reads the record");
    }

    [Fact]
    [Trait("Job", "contract: DYD-1372 g2 S4 R6 a review's level follows the candidate, never the size of the ticket briefed: the level line says so, and a seat is briefed at its review's own level (aa5b445e)")]
    public void ReviewLevel_FollowsTheCandidate()
    {
        Assert.Contains("candidate", LineOf("CaptainBriefs.Level").Text);
        Assert.Contains("level", LineOf("CaptainReviewSeats.SeatBrief").Text);
    }

    // The built line of the folder that states a claim; a table's is its first.
    private static MapLine LineOf(string claim)
    {
        var stated = Built.Value.Map.Where(line => line.Path.StartsWith(Folder, StringComparison.Ordinal) && line.Claims.Contains(claim)).MinBy(line => line.Line);
        Assert.True(stated is not null, $"no line of the skill states claim:{claim}");
        return stated!;
    }
}
