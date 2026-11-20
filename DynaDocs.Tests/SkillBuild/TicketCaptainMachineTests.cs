namespace DynaDocs.Tests.SkillBuild;

using DynaDocs.SkillBuild;

/// <summary>
/// DYD-1372 A8 (DYD-1295 519126ab): the captain's machine as the production model builds it, its
/// arrows the ticket-captain prototype's own claims. Each test pins the moves one ruling or spec rule
/// allows out of a state, by claim id and state, never by an arrow's words.
/// </summary>
public class TicketCaptainMachineTests
{
    private static readonly Lazy<BuiltTree> Built = new(() => SkillBuilder.Build(ProductionModel.Model));

    private static string Arrows => Built.Value.Files["claims/statechart/captain/transitions.json"];

    private static string Module => Built.Value.Files["claims/statechart/captain/Captain.tla"];

    [Fact]
    [Trait("Job", "contract: DYD-1372 A8 W5 the captain's states are one machine whose arrows are the prototype's own claims")]
    public void CaptainMachine_IsBuiltFromThePrototypesClaims()
    {
        var claims = Arrows.Split('\n').Where(line => line.Contains("\"claim\": ", StringComparison.Ordinal)).ToArray();

        Assert.NotEmpty(claims);
        Assert.All(claims, claim => Assert.Contains("\"claim:Captain", claim));
        Assert.All(new[] { "Taken", "Leaf", "Split", "Waiting", "InReview", "Clear", "Fixing", "Checkpoint", "Repairing", "Question", "Finishing", "Done", "Returned" },
            state => Assert.Contains($"\"{state}\"", Arrows));
        Assert.Contains("VARIABLES state, round", Module);
    }

    [Fact]
    [Trait("Job", "contract: DYD-1372 r0 11 12 R10 S10 a third round that still has a blocker leaves only through the checkpoint, to one more round or to the officer")]
    public void CaptainMachine_SendsAThirdBlockedRoundThroughTheCheckpoint()
    {
        Assert.Contains(Arrow("CaptainTrouble.Third", "InReview", "Checkpoint"), Arrows);
        Assert.Contains(Arrow("CaptainTrouble.Checkpoint", "Checkpoint", "Fixing"), Arrows);
        Assert.Contains(Arrow("CaptainTrouble.Escalate", "Checkpoint", "Returned"), Arrows);
        Assert.Contains("CaptainTrouble_Third ==\n    /\\ state = \"InReview\"\n    /\\ round = 3\n", Module);
        Assert.Contains("CaptainReview_Fix ==\n    /\\ state = \"InReview\"\n    /\\ round # 3\n", Module);
        Assert.Contains("CaptainReview_Replan ==\n    /\\ state = \"InReview\"\n    /\\ round # 3\n", Module);
    }

    [Fact]
    [Trait("Job", "contract: DYD-1372 r0 13 r0b 3 R19 R21 a conflict or red-gate repair has a state of its own: it goes on only through a review round, or back to the officer when it cannot be finished")]
    public void CaptainMachine_SendsARepairBackThroughAReview()
    {
        Assert.Contains(Arrow("CaptainMerging.Broken", "Finishing", "Repairing"), Arrows);
        Assert.Contains(Arrow("CaptainMergeTrouble.Hunks", "Repairing", "InReview"), Arrows);
        Assert.Contains(Arrow("CaptainMergeTrouble.Stuck", "Repairing", "Returned"), Arrows);
        Assert.Contains(Arrow("CaptainMergeTrouble.Lost", "Repairing", "Returned"), Arrows);
        Assert.Equal(3, Leaving("Repairing"));
        Assert.DoesNotContain("\"from\": \"Finishing\",\n      \"to\": \"Waiting\"", Arrows);
        Assert.Contains(Arrow("CaptainReview.Groups", "Waiting", "Finishing"), Arrows);
    }

    [Fact]
    [Trait("Job", "contract: DYD-1372 r0b 1 2 R8 R10 a clear round is a state of its own: the review rounds end from it, one more round is ordered only from it and only once, and a blocked round reaches the next only through a Fix")]
    public void CaptainMachine_OrdersOneMoreRoundOnlyAfterAClearOne()
    {
        Assert.Contains(Arrow("CaptainReview.Cleared", "InReview", "Clear"), Arrows);
        Assert.Contains(Arrow("CaptainReview.Clean", "Clear", "Finishing"), Arrows);
        Assert.Contains(Arrow("CaptainReview.OneMore", "Clear", "InReview"), Arrows);
        Assert.DoesNotContain("\"from\": \"InReview\",\n      \"to\": \"InReview\"", Arrows);
        Assert.DoesNotContain("\"from\": \"InReview\",\n      \"to\": \"Finishing\"", Arrows);
        Assert.Contains("CaptainReview_OneMore ==\n    /\\ state = \"Clear\"\n    /\\ ~extra\n    /\\ state' = \"InReview\"\n    /\\ round' = round + 1 /\\ extra' = TRUE\n", Module);
        Assert.Contains("CaptainTrouble_NextRound ==\n    /\\ state = \"Fixing\"\n    /\\ state' = \"InReview\"\n    /\\ round' = round + 1\n", Module);
        Assert.Equal(2, Module.Split("round' = round + 1").Length - 1);
    }

    [Fact]
    [Trait("Job", "contract: DYD-1372 r1b P2 H11 a filler ticket that fails its check goes back from Taken, before anything else is done with it")]
    public void CaptainMachine_ReturnsAFailedFillerFromTaken()
    {
        Assert.Contains(Arrow("Captain.FillerBack", "Taken", "Returned"), Arrows);
        Assert.Equal(1, Arrows.Split("\"from\": \"Taken\",\n      \"to\": \"Returned\"").Length - 1);
    }

    [Fact]
    [Trait("Job", "contract: DYD-1372 r1b P6 r0 3 H16 a leaf's Question leaves two ways: answered, back to the leaf, or unsettled, to the officer; the research resource never routes one to the user")]
    public void CaptainMachine_LetsAQuestionLeaveAnsweredOrUnsettled()
    {
        Assert.Contains(Arrow("CaptainTrouble.Answered", "Question", "Leaf"), Arrows);
        Assert.Contains(Arrow("CaptainTrouble.Up", "Question", "Returned"), Arrows);
        Assert.Equal(2, Leaving("Question"));
        Assert.DoesNotContain("user", Built.Value.Files["skills/roles/officers/ticket-captain/resources/research.md"]);
    }

    private static int Leaving(string state) => Arrows.Split($"\"from\": \"{state}\"").Length - 1;

    private static string Arrow(string claim, string from, string to) => $"\"claim\": \"claim:{claim}\",\n      \"from\": \"{from}\",\n      \"to\": \"{to}\"";
}
