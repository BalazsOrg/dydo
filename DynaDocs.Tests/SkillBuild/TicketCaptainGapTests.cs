namespace DynaDocs.Tests.SkillBuild;

using System.Text.Json;
using DynaDocs.SkillBuild;

/// <summary>
/// DYD-1372 phase 1, gap hunt 1 (X16, fix g1a): the gaps a walk of the skill found, each as the rule
/// that closes it, read from the production model through <see cref="SkillBuilder.Build"/>. A rule is
/// stated by claim id, path and line order, or over the built machine's arrows, never by a sentence
/// of the skill (DR 067). Claims are named by id, so this file compiles against the model as it was
/// before the fix, where every test here is red.
/// </summary>
public class TicketCaptainGapTests
{
    private const string Folder = "skills/roles/officers/ticket-captain/";
    private const string Spine = Folder + "SKILL.md";
    private const string Resources = Folder + "resources/";
    private const string OnlyAChange = "kind = \"change\"";
    private const string OnlyAVerdict = "kind = \"verdict\"";

    private static readonly string[] ReviewLoop = ["InReview", "Clear", "Fixing", "Checkpoint", "Repairing"];

    private static readonly Lazy<BuiltTree> Built = new(() => SkillBuilder.Build(ProductionModel.Model));

    private static readonly Lazy<(string Claim, string From, string To, string? Guard, string? Effect)[]> Moves = new(() =>
    [
        .. Machine.GetProperty("transitions").EnumerateArray().Select(move => (move.GetProperty("claim").GetString()!, move.GetProperty("from").GetString()!,
            move.GetProperty("to").GetString()!, move.GetProperty("guard").GetString(), move.GetProperty("effect").GetString()))
    ]);

    private static JsonElement Machine => JsonDocument.Parse(Built.Value.Files["claims/statechart/captain/transitions.json"]).RootElement;

    [Fact]
    [Trait("Job", "contract: DYD-1372 g1a G1 H23 a Question is sorted at intake by its brief: the line that holds job tickets holds every Question not sent to research, and whoever opens one marks how it is settled")]
    public void Question_IsSortedByItsBriefAtIntake()
    {
        var stage = LineOf("Captain.Stage");
        var research = Stated(Spine).Single(line => line.Text.Contains("](resources/research.md)", StringComparison.Ordinal));

        Assert.Equal(LineOf("Captain.Jobs"), stage);
        Assert.True(stage.Line > research.Line, "the other Questions are sorted above the row for the one to research");
        Assert.Contains("brief", research.Text, StringComparison.OrdinalIgnoreCase);
        Assert.Equal(Resources + "sub-tickets.md", LineOf("CaptainSplit.Asks").Path);
    }

    [Fact]
    [Trait("Job", "contract: DYD-1372 g1a G2 R21 the Merge step is its captain's own: the line that orders the dispatch excepts it, no kind row takes one, and merging opens by starting it")]
    public void MergeStep_IsItsCaptainsOwnAndNeverDispatched()
    {
        var begin = LineOf("CaptainMerging.Begin");

        Assert.Equal(LineOf("CaptainDispatch.Send"), LineOf("CaptainDispatch.MergeOwn"));
        Assert.Equal(Resources + "merging.md", begin.Path);
        Assert.True(begin.Line < LineOf("CaptainMerging.Gates").Line, "the merge runs before its sub-ticket is started");
        Assert.DoesNotContain(Built.Value.Map, line => line.Claims.Contains("Captain.Kinds") && line.Text.Contains("Merge", StringComparison.Ordinal));
    }

    [Fact]
    [Trait("Job", "contract: DYD-1372 g1a G3 R33 R34 a round has one owner, its Review ticket's holder: the round is sized and seated in the review resource's holder part, and only a verdict's holder makes the verdict move")]
    public void Round_IsSizedSeatedAndRecordedByTheReviewsHolder()
    {
        var scale = LineOf("CaptainDispatch.Scale");
        var verdict = Moves.Value.Single(move => move.Claim == "claim:CaptainReviewSeats.Verdict");

        Assert.Equal(Resources + "review.md", scale.Path);
        Assert.Equal(scale, LineOf("CaptainReviewSeats.Prefer"));
        Assert.True(scale.Line > LineOf("CaptainReviewing.Matrix").Line, "a seat reads how its round is sized");
        Assert.Equal(OnlyAVerdict, verdict.Guard);
        Assert.Contains("](round.md)", LineOf("CaptainReviewSeats.Verdict").Text);
        // It can prefer another family only when its brief names the writer's, beside the candidate.
        Assert.Equal(LineOf("CaptainBriefs.Candidate"), LineOf("CaptainBriefs.Author"));
    }

    [Fact]
    [Trait("Job", "contract: DYD-1372 g1a G4 R17 S4 a PASS goes into a PR only where the candidate is in one: the same step says where a round with no PR is recorded, and a review brief names the PR or its absence")]
    public void Pass_GoesIntoAPrOnlyWhereTheCandidateIsInOne()
    {
        var pr = LineOf("CaptainBriefs.Pr");

        Assert.Equal(LineOf("CaptainRound.PrLine"), LineOf("CaptainRound.NoPr"));
        Assert.Equal(Resources + "briefs.md", pr.Path);
        Assert.True(pr.Line > LineOf("CaptainBriefs.Candidate").Line, "the PR is named above its candidate");
    }

    [Fact]
    [Trait("Job", "contract: DYD-1372 g1a G5 H15 H23 a need that already has its ticket goes up as that ticket: the step that opens a sub-ticket first keeps a Grilling, a Walkthrough or another HITL sub-ticket already cut")]
    public void NeedProcedure_CarriesATicketThatAlreadyExists()
    {
        var carried = LineOf("CaptainNeed.Carried");

        Assert.Equal(LineOf("CaptainNeed.Ask"), carried);
        Assert.All(new[] { "Grilling", "Walkthrough", "HITL" }, job => Assert.Contains(job, carried.Text));
        Assert.Contains("](need-for-the-user.md)", LineOf("CaptainWay.Reach").Text);
        // Such a ticket goes up, so the line that orders the dispatch names the Mode it dispatches.
        Assert.Contains("AFK", LineOf("CaptainDispatch.Send").Text);
    }

    [Fact]
    [Trait("Job", "contract: DYD-1372 g1a G9 R19 R21 a repair made before the merge has its way back: a line below the one that sends it out links the recipe that holds its review and its retry")]
    public void RepairBeforeTheMerge_ReturnsThroughAReview()
    {
        var repaired = LineOf("CaptainMerging.Repaired");

        Assert.Equal(Resources + "merging.md", repaired.Path);
        Assert.True(repaired.Line > LineOf("CaptainMerging.Broken").Line, "the repair returns before it is sent out");
        Assert.Contains("](merge-conflict.md)", repaired.Text);
    }

    [Fact]
    [Trait("Job", "contract: DYD-1372 g1a G10 G11 S7 a holder checks before it starts: the check stands above the start and links the route of each check that fails, a thin brief, a ticket already held and a filler that fails")]
    public void Intake_ChecksBeforeItStarts()
    {
        var check = LineOf("CaptainIntake.Check");
        var routes = new[] { "CaptainIntake.Lacking", "CaptainIntake.Held", "Captain.Filler" }.Select(LineOf).ToArray();

        Assert.Equal(Spine, check.Path);
        Assert.True(check.Line < LineOf("Captain.Start").Line, "the ticket is started before it is checked");
        Assert.All(routes, route => Assert.Contains($"](resources/{Path.GetFileName(route.Path)})", check.Text));
        Assert.Contains("](need-for-the-user.md)", routes[0].Text);
        Assert.Contains(Moves.Value, move => move is { Claim: "claim:CaptainIntake.Held", From: "Dispatched", To: "Returned" });
        Assert.Equal(Resources + "briefs.md", LineOf("CaptainBriefs.TakeOver").Path);
    }

    [Fact]
    [Trait("Job", "contract: DYD-1372 g1a G12 a dispatch refused for another reason than a limit has its line under When things go wrong, below the limit's")]
    public void RefusedDispatch_HasItsLineBelowTheLimits()
    {
        var refused = LineOf("CaptainDispatch.Refused");
        var section = Array.IndexOf(Built.Value.Files[Spine].Split('\n'), "## " + SkillRenderer.WhenThingsGoWrong) + 1;

        Assert.Equal(Spine, refused.Path);
        Assert.True(section > 0 && refused.Line > section, "the refusal stands outside When things go wrong");
        Assert.True(refused.Line > LineOf("CaptainFinish.Limits").Line, "the refusal stands above the limit it is told apart from");
    }

    [Fact]
    [Trait("Job", "contract: DYD-1372 g1a arrow audit a Question whose research does not settle its call goes back from the leaf: the move the research resource states is an arrow")]
    public void UnsettledQuestion_ReturnsFromTheLeaf() =>
        Assert.Contains(Moves.Value, move => move is { Claim: "claim:CaptainResearch.Question", From: "Leaf", To: "Returned" });

    [Fact]
    [Trait("Job", "contract: DYD-1372 g1a M1 R34 a verdict is never reviewed: no path of the machine leads a holder whose result is a verdict or an answer into a review round or a repair, and a change still reaches both (777d396e)")]
    public void Verdict_ReachesNoReviewRound()
    {
        var verdict = Reached(OnlyAChange);
        var change = Reached(OnlyAVerdict);

        Assert.All(ReviewLoop, state => Assert.DoesNotContain(state, verdict));
        Assert.All(ReviewLoop, state => Assert.Contains(state, change));
        Assert.All(new[] { verdict, change }, reached => Assert.Contains("Returned", reached));
    }

    [Fact]
    [Trait("Job", "contract: DYD-1372 g1a M1 a holder learns its kind by the one move that takes the ticket: it leaves the initial state, and no later move changes what the result is")]
    public void Kind_IsLearnedByTheMoveThatTakesTheTicket()
    {
        var learns = Moves.Value.Where(move => move.Effect?.Contains("kind'", StringComparison.Ordinal) == true).ToArray();

        Assert.Equal(["claim:Captain.Start"], learns.Select(move => move.Claim));
        Assert.Equal(Machine.GetProperty("initial").GetString(), learns[0].From);
        Assert.DoesNotContain(Moves.Value, move => move.Guard?.Contains("kind", StringComparison.Ordinal) == true && move.From == learns[0].From);
    }

    // Every state a holder can reach when no move under the excluded guard is open to it; the other guards are left open, so nothing reachable is missed.
    private static HashSet<string> Reached(string excluded)
    {
        var reached = new HashSet<string> { Machine.GetProperty("initial").GetString()! };
        var open = Moves.Value.Where(move => move.Guard != excluded).ToArray();
        var grew = true;
        while (grew) grew = open.Where(move => reached.Contains(move.From)).Count(move => reached.Add(move.To)) > 0;
        return reached;
    }

    private static MapLine[] Stated(string path) => [.. Built.Value.Map.Where(line => line.Path == path)];

    // The built line of the folder that states a claim; a table's is its first.
    private static MapLine LineOf(string claim)
    {
        var stated = Built.Value.Map.Where(line => line.Path.StartsWith(Folder, StringComparison.Ordinal) && line.Claims.Contains(claim)).MinBy(line => line.Line);
        Assert.True(stated is not null, $"no line of the skill states claim:{claim}");
        return stated!;
    }
}
