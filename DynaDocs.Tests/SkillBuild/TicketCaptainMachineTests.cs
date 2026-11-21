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
        Assert.All(new[] { "Taken", "Leaf", "Wayfinding", "Split", "Waiting", "InReview", "Clear", "Fixing", "Checkpoint", "Repairing", "Question", "Finishing", "Done", "Returned" },
            state => Assert.Contains($"\"{state}\"", Arrows));
        Assert.Contains("VARIABLES state, round", Module);
    }

    [Fact]
    [Trait("Job", "contract: DYD-1372 X2 a captain that cannot see the route finds the way before it cuts: finding the way is left only by cutting the parts in sight, and entered again when a part returns and fog remains")]
    public void CaptainMachine_FindsTheWayBeforeItCuts()
    {
        Assert.Contains(Arrow("CaptainWay.Fog", "Taken", "Wayfinding"), Arrows);
        Assert.Contains(Arrow("CaptainWay.Visible", "Wayfinding", "Split"), Arrows);
        Assert.Contains(Arrow("CaptainWay.Next", "Waiting", "Wayfinding"), Arrows);
        Assert.Equal(1, Leaving("Wayfinding"));
        Assert.Contains(Arrow("CaptainSplit.Cut", "Taken", "Split"), Arrows);
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
        Assert.Equal(1, Between("Taken", "Returned"));
    }

    [Fact]
    [Trait("Job", "contract: DYD-1372 r2b B3 H16 S10 a leaf's Question is left only to resume the leaf: passed up unsettled, it stays a wait; the research resource never routes one to the user")]
    public void CaptainMachine_LeavesAQuestionOnlyToResumeTheLeaf()
    {
        Assert.Contains(Arrow("CaptainNeed.Answered", "Question", "Leaf"), Arrows);
        Assert.Contains(Arrow("CaptainNeed.Up", "Question", "Question"), Arrows);
        Assert.Equal(Leaving("Question"), Between("Question", "Leaf") + Between("Question", "Question"));
        Assert.DoesNotContain("user", Built.Value.Files["skills/roles/officers/ticket-captain/resources/research.md"]);
    }

    private static int Between(string from, string to) => Arrows.Split($"\"from\": \"{from}\",\n      \"to\": \"{to}\"").Length - 1;

    private static int Leaving(string state) => Arrows.Split($"\"from\": \"{state}\"").Length - 1;

    private static string Arrow(string claim, string from, string to) => $"\"claim\": \"claim:{claim}\",\n      \"from\": \"{from}\",\n      \"to\": \"{to}\"";
}
