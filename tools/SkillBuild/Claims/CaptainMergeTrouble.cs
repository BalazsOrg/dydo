namespace DynaDocs.SkillBuild;

/// <summary>
/// The ticket-captain prototype's two merge scenarios (DYD-1372): a conflict, and a gate red after the
/// merge. What <c>dydo merge</c> does on each is read from its code: it refuses a conflict, and it
/// undoes no revert.
/// </summary>
public static class CaptainMergeTrouble
{
    private const string MergeCode = "Services/Merge/PullRequestMerge.cs";

    private static readonly Enum[] Me = [Actor.TicketCaptain];

    public static readonly Claim Resolver = new("On a conflict the captain dispatches an Implement ticket to resolve it on the ticket's branch.", Me,
        [At(CaptainSlot.ConflictSteps, 1, "Dispatch an Implement ticket to resolve the conflict on your ticket's branch.")])
    {
        Spec = ["R21", "R19"],
        Truth = new(Evidence.Code(MergeCode),
            Layer: "On a conflict `dydo merge` refuses, merges nothing and resolves nothing.",
            Reason: "A resolution is new code, and the captain about to merge it must not be its writer.")
    };

    public static readonly Claim Saved = new("The resolution's commit SHA is posted on the Merge sub-ticket.", Me,
        [At(CaptainSlot.ConflictSteps, 2, "Post the resolution's commit SHA on the Merge sub-ticket.")])
    {
        Spec = ["K9", "H3"],
        Truth = new(Evidence.Run("Linear, DYD-1281's sub-tickets read 2026-11-20: each merge is its own ticket, such as DYD-1291."),
            Layer: "The resolver returns its SHA to the captain's thread, which can end before the merge does.",
            Reason: "The merge can be resumed from the ticket by whoever holds it next.")
    };

    public static readonly Claim Hunks = new("A conflict's resolution gets a fresh review of its hunks.", Me,
        [At(CaptainSlot.ConflictSteps, 3, "Dispatch a fresh reviewer for the resolution's hunks, the only new work on the branch.")])
    {
        Spec = ["R19"],
        Truth = new(Evidence.Code(MergeCode),
            Layer: "`dydo merge` refuses the new head until a PASS names it, and says nothing of who may give one.",
            Reason: "A resolution is the one part of a merge that somebody wrote by hand."),
        Transition = new(CaptainState.Repairing, CaptainState.InReview, "the repair is in: a fresh review of what it changed")
    };

    public static readonly Claim Again = new("The merge runs again once the hunks pass, on the new head.", Me,
        [At(CaptainSlot.ConflictSteps, 4, "Merge again, on the new head, once that review passes.")])
    {
        Spec = ["R19", "R23"],
        Truth = new(Evidence.Command("dydo merge <pr-number> --match-head-commit <sha> --match-base-commit <sha> --dry-run"),
            Layer: "`dydo merge` merges only the head it is given, and the resolution made a new one.",
            Reason: "The pin on the old head would refuse, and rightly.")
    };

    public static readonly Claim Stuck = new("A conflict the captain cannot get resolved goes to its officer.", Me,
        [At(CaptainSlot.ConflictEnd, 1, "Take a conflict you cannot get resolved to your officer, with both sides named.")])
    {
        Spec = ["K9"],
        Truth = new(Evidence.Tool("delegate_task: a child's final result returns to the thread that started it."),
            Layer: "Two tickets that collide have two captains, and neither can re-order the other's work.",
            Reason: "The officer above both is the first one able to sequence them.")
    };

    public static readonly Claim Rerun = new("A gate red after the merge is rerun once.", Me,
        [At(CaptainSlot.RedLead, 1, "Rerun the red run once.")])
    {
        Spec = ["R22"],
        Truth = new(Evidence.Tool("ci_rerun: \"Re-run a PR's failing checks, or one named check.\""),
            Layer: "Hosted CI reports red and stays red until somebody reruns it.",
            Reason: "One rerun tells a flake from a break at the cost of a few minutes.")
    };

    public static readonly Claim Flake = new("Green on the rerun is a flake, and the merge is done.", Me,
        [At(CaptainSlot.RedLead, 2, "Green on the rerun is a flake: the merge is done.")])
    {
        Spec = ["R22"],
        Truth = new(Evidence.Code(".github/workflows/ci.yml"),
            Layer: "CI keeps no count of which runs turned green on a second try.",
            Reason: "A merge that is sound is not reverted for a test that wobbled.")
    };

    public static readonly Claim Revert = new("A real red after a merge is reverted on the feature branch at once, by the captain.", Me,
        [At(CaptainSlot.RedSteps, 1, "Revert the merge on the feature branch at once: `git revert -m 1 <merge SHA>`, pushed.")])
    {
        Spec = ["R22"],
        Truth = new(Evidence.Run("Each layer read 2026-11-20, no revert yet made: GitHub reports `feature/dydo-4-0-0` unprotected and under no ruleset; the guard's own corpus " +
                                 "lets `git push --force-with-lease origin feature/x` through (`GuardShellCorpusTests`), its merge-route block naming only the default branch; " +
                                 "the feature branch's history holds no revert."),
            Layer: "The guard stops a push to the default branch only, and nothing reverts a red merge by itself.",
            Reason: "Everyone else builds on the feature branch, and every hour it stays red is theirs too.")
    };

    public static readonly Claim Cancelled = new("A reverted Merge is cancelled with its reason, and what broke becomes a CI-findings ticket.", Me,
        [At(CaptainSlot.RedSteps, 2, "Cancel the Merge sub-ticket with `reverted: <gate> red on <merge SHA>`, and file what broke as a ticket in the team's CI findings.")])
    {
        Spec = ["R22", "S12"],
        Truth = new(Evidence.Run("Linear, team Dydo, read 2026-11-20: the status Canceled exists, and so does the Project \"CI findings\"."),
            Layer: "Linear leaves the Merge sub-ticket open after a revert, as if the merge still stood.",
            Reason: "The record says why the merge is gone, and the break has a ticket of its own.")
    };

    public static readonly Claim Repair = new("After a revert, an Implement ticket fixes it on the ticket's branch, then a review round scoped to the fix.", Me,
        [At(CaptainSlot.RedSteps, 3, "Dispatch an Implement ticket to fix it on your ticket's branch, then a review round scoped to the fix.")])
    {
        Spec = ["R22"],
        Truth = new(Evidence.Code(MergeCode),
            Layer: "`dydo merge` merges only a head with a PASS, and the fix makes a new head.",
            Reason: "The rest of the branch already passed, so the round looks at what changed.")
    };

    public static readonly Claim Reland = new("The fixed branch is merged again through a PR that itself restores the reverted change.", Me,
    [
        At(CaptainSlot.RedSteps, 4,
            "Merge the fixed branch again through a new PR that brings the reverted change back itself: `dydo merge` undoes no revert, and calls the old PR already integrated.")
    ])
    {
        Spec = ["R22", "R23"],
        Truth = new(Evidence.Code(MergeCode),
            Layer: "The spec has `dydo merge` revert the revert; the command as built does not, so the branch must.",
            Reason: "A re-merge of the old commits would land nothing and still report success.")
    };

    public static readonly Claim Lost = new("A red gate the captain cannot get fixed goes to its officer.", Me,
        [At(CaptainSlot.RedEnd, 1, "Take a red you cannot get fixed to your officer, with the merge SHA and the gate.")])
    {
        Spec = ["K9"],
        Truth = new(Evidence.Tool("delegate_task: a child's final result returns to the thread that started it."),
            Layer: "A reverted merge leaves the ticket open with nobody but its captain watching it.",
            Reason: "The officer can re-plan around a part that will not go in.")
    };

    private static Line At(Enum slot, int order, string text) =>
        new(CaptainSkill.TicketCaptain, Actor.TicketCaptain, text, new(slot, order));
}
