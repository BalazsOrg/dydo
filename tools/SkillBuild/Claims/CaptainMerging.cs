namespace DynaDocs.SkillBuild;

/// <summary>
/// The ticket-captain prototype's merging resource (DYD-1372): the steps of a captain that merges its
/// ticket's PR into a feature branch. Every command is checked against the CLI's own command tree, and
/// what <c>dydo merge</c> does is read from its code, never from the decision that asked for it.
/// </summary>
public static class CaptainMerging
{
    private const string MergeCode = "Services/Merge/PullRequestMerge.cs";

    private static readonly Enum[] Me = [Actor.TicketCaptain];

    public static readonly Claim Begin = new("The merging captain sets its Merge sub-ticket In Progress when it begins the merge, and closes it itself.", Me,
    [
        At(Actor.TicketCaptain, CaptainSlot.MergeSteps, 1,
            "Set your Merge sub-ticket In Progress: the merge is your own step, never dispatched, and you close it yourself. Where your work was not cut into steps, your own ticket is the record instead.")
    ])
    {
        Spec = ["R20", "S6"],
        Truth = new(Evidence.Run("Linear, DYD-1281's Merge sub-tickets read 2026-11-21: DYD-1290 to DYD-1293, one a merge, each Done under the labels Merge and AFK."),
            Layer: "Linear leaves a Merge sub-ticket in Todo until someone moves it, and `dydo merge` touches no ticket.",
            Reason: "The board then shows a merge under way, and a card left in Todo invites a second captain to begin the same one.")
    };

    public static readonly Claim Gates = new("The captain runs the project's gates on the combined tree before it merges.", Me,
        [At(Actor.TicketCaptain, CaptainSlot.MergeSteps, 2, "Run the project's gates, as your brief names them, on the combined tree: your branch merged with the target's current tip.")])
    {
        Spec = ["R21", "R18"],
        Truth = new(Evidence.Code(".github/workflows/ci.yml"),
            Layer: "Hosted CI runs when a PR is pushed and does not run again when its target moves.",
            Reason: "Two branches that are each green can still break each other once they meet.")
    };

    public static readonly Claim Refusal = new("dydo merge states why it refuses.", Me, [])
    {
        Spec = ["R23"],
        Truth = new(Evidence.Code(MergeCode),
            Layer: "The command prints its reason; nothing tells the captain that the reason is the next thing to act on.",
            Reason: "A refusal read as a failure gets retried; read as an answer, it gets fixed.")
    };

    public static readonly Claim Merge = new("A captain merges into a feature branch with dydo merge, naming the reviewed head and the target tip it gated against.", Me,
    [
        At(Actor.TicketCaptain, CaptainSlot.MergeSteps, 3,
            "Merge with `dydo merge <pr-number> --match-head-commit <sha> --match-base-commit <sha>`: the reviewed head, and the tip you ran the gates against. When it refuses, its message says why.",
            () => [Refusal])
    ])
    {
        Spec = ["R23", "R21"],
        Truth = new(Evidence.Command("dydo merge <pr-number> --match-head-commit <sha> --match-base-commit <sha> --dry-run"),
            Layer: "The guard blocks every other way to merge a PR and names this command; the two SHAs are the captain's to supply.",
            Reason: "The pins tie the merge to what was reviewed and gated, so nothing pushed since can ride along.")
    };

    public static readonly Claim Ci = new("A merge is done once hosted CI is green on the merge commit.", Me,
        [At(Actor.TicketCaptain, CaptainSlot.MergeSteps, 4, "Close the Merge sub-ticket once hosted CI is green on the merge commit.")])
    {
        Spec = ["R20", "R18"],
        Truth = new(Evidence.Code(".github/workflows/ci.yml"),
            Layer: "CI runs on every push to `feature/**` and reports to GitHub, where no ticket sees it.",
            Reason: "The merge commit is the first time the combined tree runs on the host everyone trusts.")
    };

    public static readonly Claim Receipt = new("A merge is done only once DYTEST also holds a green receipt on the merge tree.", Me,
        [At(Actor.TicketCaptain, CaptainSlot.MergeSteps, 5, "Before you close it, wait for DYTEST's green receipt on the merge tree as well.")])
    {
        Spec = ["R20"],
        Truth = new(Evidence.Open("DYTEST is not built: no command of the CLI is DYTEST (`Commands/DydoRootCommand.cs` registers none), so nothing issues a receipt to wait for."),
            Layer: "Hosted CI runs its own jobs on the merge commit and keeps no record of the gates the captain ran on the combined tree.",
            Reason: "A proof keyed by the tree shows the gates passed on exactly what was merged, with nobody's word taken for it.")
    };

    public static readonly Claim Archive = new("After a merge the captain archives the branches and worktrees its dispatches created.", Me,
    [
        At(Actor.TicketCaptain, CaptainSlot.MergeSteps, 6,
            "Archive each branch and worktree your dispatches created, with `dydo archive branch <target>` and `dydo archive worktree <target>`; the archive keeps them 30 days.")
    ])
    {
        Spec = ["N10", "N12 DR 045 §8"],
        Truth = new(Evidence.Open("Dry runs with the built CLI, 2026-11-20, on this ticket's own T3-made worktree and its branch: `dydo archive worktree` refused, " +
                                  "\"T3 ownership/activity unavailable … target retained\", and `dydo archive branch` refused, \"Branch is checked out elsewhere\". " +
                                  "Every worktree a dispatch creates is T3's (`Services/SafeDeleteT3Policy.cs`; spec CA19, DYD-1216)."),
            Layer: "T3 makes a child's worktree and leaves it standing: `git worktree list` on the writer's host showed 237.",
            Reason: "Merged checkouts pile up on the host, and an archive can be undone where a delete cannot.")
    };

    public static readonly Claim Clean = new("A conflict-free merge into the feature branch gets no merge review.", Me,
        [At(Actor.TicketCaptain, CaptainSlot.MergeRules, 1, "A clean merge gets no review.")])
    {
        Spec = ["R18"],
        Truth = new(Evidence.Code(MergeCode),
            Layer: "`dydo merge` refuses a hand-made conflict resolution's head until it has its own PASS, and asks nothing more of a clean one.",
            Reason: "A clean merge adds no line anyone wrote, so the gates and CI prove it.")
    };

    public static readonly Claim Broken = new("A conflict or a real red gate before the merge goes to an Implement ticket on the ticket's branch.", Me,
        [At(Actor.TicketCaptain, CaptainSlot.MergeRules, 2, "A conflict, or a gate that is really red, goes to an Implement ticket on your branch.")])
    {
        Spec = ["R21", "R19"],
        Truth = new(Evidence.Code(MergeCode),
            Layer: "On a conflict `dydo merge` refuses and merges nothing; it repairs neither that nor a red gate.",
            Reason: "A captain that repairs the work it is about to merge has written what nobody reviewed."),
        Transition = new(CaptainState.Finishing, CaptainState.Repairing, "a conflict or a real red gate: an Implement ticket repairs it") { Guard = CaptainMachines.Change }
    };

    public static readonly Claim Repaired = new("A repair made before the merge, of a conflict or of a red gate, returns to a fresh review of what it changed, and the merge is then run again on the new head; " +
                                                "one that cannot be made goes to the officer.", Me,
    [
        At(Actor.TicketCaptain, CaptainSlot.MergeRules, 4,
            "Either repair comes back the way [A merge conflict](merge-conflict.md) lays out: a fresh reviewer judges what it changed, " +
            "and you merge again from step 2, on the new head. A red you cannot get fixed goes to your officer, with the gate.")
    ])
    {
        Spec = ["R19", "R21", "R23"],
        Truth = new(Evidence.Code(MergeCode),
            Layer: "`dydo merge` refuses a head no PASS names, and a repair always makes a new head; it sends the repair to no reviewer.",
            Reason: "A fix written to turn a gate green is new code like any other, and unread it would be the one line nobody judged.")
    };

    public static readonly Claim Mutation = new("No ticket or merge waits on mutation testing.", Me,
        [At(Actor.TicketCaptain, CaptainSlot.MergeRules, 3, "Nothing waits on mutation testing.")])
    {
        Spec = ["T12"],
        Truth = new(Evidence.Open("`dydo/guides/testing-strategy.md` still lists `gate mutation --since BASE` among the project's gates, and DYTEST Mutation, which T12 moves it to, is not built."),
            Layer: "Which gates a merge waits on is whatever the brief names, and the brief may still name this one.",
            Reason: "Mutation runs take hours, and a merge that waits on them holds a lane the whole time.")
    };

    private static Line At(Actor reader, Enum slot, int order, string text, Func<Claim[]>? also = null) =>
        new(CaptainSkill.TicketCaptain, reader, text, new(slot, order), also);
}
