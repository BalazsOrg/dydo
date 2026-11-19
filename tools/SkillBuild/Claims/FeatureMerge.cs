namespace DynaDocs.SkillBuild;

/// <summary>The Merge hop into the feature branch (M7): the captain's own merge, a conflict on the ticket branch, the gates, a flake, a real red, the revert and the re-land.</summary>
public static class FeatureMerge
{
    // The captain's lines are Dispatch.Start (it starts the hop) and GateFirst (it runs the gate and the merge), so this arrow has none (K4).
    public static readonly Claim Start = new("The Merge hop's blocker clears, blocked in step order.", [Actor.Linear], [])
    { Spec = ["S3", "R21"], Transition = new(FeatureMergeState.MergeTodo, FeatureMergeState.PreGate, "the Merge hop's blocker clears") };

    public static readonly Claim PreGreen = new("dydo test gate leaves a green receipt for the combined tree; the merge runs.", [Actor.Dytest], [])
    { Spec = ["Y4", "R23"], Transition = new(FeatureMergeState.PreGate, FeatureMergeState.Running, "a green receipt for the combined tree") };

    public static readonly Claim PreRed = new("A real red gate goes to a code-writer.", [Actor.TicketCaptain],
        [new(Skill.TicketCaptain, "On a real red gate, send a code-writer to fix it on the ticket branch.", new(FeatureMergeState.PreGate, 620, MoveKind.Exit))])
    { Spec = ["R21"], Transition = new(FeatureMergeState.PreGate, FeatureMergeState.PreFix, "a real red gate before the merge") };

    public static readonly Claim PreFixed = new("A fix moves the head, so a fresh round must pass the new head.", [Actor.TicketCaptain], [])
    { Spec = ["R8", "R23"], Transition = new(FeatureMergeState.PreFix, FeatureMergeState.Rereview, "the fix moves the head: a fresh round") };

    public static readonly Claim RereviewPass = new("A PASS on the new head lets the gate and the merge run again.", [Actor.Reviewer], [])
    { Spec = ["R17", "R23"], Transition = new(FeatureMergeState.Rereview, FeatureMergeState.PreGate, "PASS on the new head") };

    public static readonly Claim NoPass = new("dydo merge refuses a PR with no PASS on its head; a review round follows.", [Actor.DydoMerge], [])
    { Spec = ["R17", "R23", "R24"], Transition = new(FeatureMergeState.Running, FeatureMergeState.Rereview, "refused: no PASS on the head or a clean-sync ancestor") };

    public static readonly Claim NoReceipt = new("dydo merge refuses without a green receipt; the captain runs dydo test gate.", [Actor.DydoMerge], [])
    { Spec = ["R21", "R23", "Y4"], Transition = new(FeatureMergeState.Running, FeatureMergeState.PreGate, "refused: no green receipt for the combined tree") };

    public static readonly Claim HandShaped = new("dydo merge refuses a hand-shaped tree; it is resolved on the ticket branch with a hunk review.", [Actor.DydoMerge], [])
    { Spec = ["R19"], Transition = new(FeatureMergeState.Running, FeatureMergeState.Resolving, "refused: a hand-shaped tree") };

    public static readonly Claim Conflict = new("A CONFLICT goes to a code-writer, resolved only on the ticket branch.", [Actor.TicketCaptain, Actor.CodeWriter],
    [
        new(Skill.TicketCaptain, "On `CONFLICT`, send a code-writer to resolve it on the ticket branch.", new(FeatureMergeState.Running, 602, MoveKind.Exit)),
        new(Skill.CodeWriter, "Resolve a merge conflict on the ticket branch only, then commit it as `<KEY> merge: <what>`.", new(FeatureMergeState.Resolving, 602, MoveKind.Arrival)),
        new(Skill.WorkingTreeStandard, "A conflict is resolved only on the ticket branch, under a fresh review of its hunks.", new(Section.TreeMerging, 3))
    ])
    { Spec = ["R21", "R19"], Transition = new(FeatureMergeState.Running, FeatureMergeState.Resolving, "dydo merge reports CONFLICT") };

    public static readonly Claim Resolved = new("A conflict's resolution gets a fresh review of its hunks.", [Actor.TicketCaptain, Actor.Reviewer],
    [
        new(Skill.TicketCaptain, "Send the resolution's hunks to a fresh reviewer.", new(FeatureMergeState.Resolving, 603, MoveKind.Exit)),
        new(Skill.Reviewer, "Judge the conflict resolution's hunks, the only new work on the branch.", new(FeatureMergeState.HunkReview, 603, MoveKind.Arrival))
    ])
    { Spec = ["R19"], Transition = new(FeatureMergeState.Resolving, FeatureMergeState.HunkReview, "the resolution is committed") };

    public static readonly Claim HunksPass = new("A PASS on the hunks names the new head; the merge runs again.", [Actor.Reviewer, Actor.TicketCaptain],
        [new(Skill.Reviewer, "Post the hunks' verdict as for any round; its PASS names the new head.", new(FeatureMergeState.HunkReview, 604, MoveKind.Exit))])
    { Spec = ["R19", "R17"], Transition = new(FeatureMergeState.HunkReview, FeatureMergeState.PreGate, "PASS on the hunks; the gate and the merge run again") };

    public static readonly Claim Merge = new("dydo merge writes the merge commit.", [Actor.DydoMerge], [])
    { Spec = ["R23", "R32"], Transition = new(FeatureMergeState.Running, FeatureMergeState.Gating, "dydo merge writes the merge commit") };

    public static readonly Claim Green = new("A Merge hop is Done on DYTEST's green receipt on the merge tree and green hosted CI on the merge SHA.", [Actor.TicketCaptain],
    [
        new(Skill.TicketCaptain, "Close the Merge hop Done once DYTEST holds a green receipt on the merge tree and hosted CI is green on the merge SHA.", new(FeatureMergeState.Gating, 606, MoveKind.Exit)),
        new(Skill.WorkingTreeStandard, "A Merge hop is Done when DYTEST holds a green receipt on the merge tree and hosted CI is green on the merge SHA.", new(Section.TreeMerging, 4))
    ])
    { Spec = ["R18", "R20"], Transition = new(FeatureMergeState.Gating, FeatureMergeState.MergeDone, "DYTEST green on the merge tree, CI green on the merge SHA") };

    public static readonly Claim Red = new("A red gate after a merge reruns once.", [Actor.TicketCaptain],
        [new(Skill.TicketCaptain, "A gate red after the merge reruns once through DYTEST.", new(FeatureMergeState.Gating, 607, MoveKind.Exit))])
    { Spec = ["R22"], Transition = new(FeatureMergeState.Gating, FeatureMergeState.Rerun, "a gate goes red after the merge") };

    public static readonly Claim Flake = new("Green on the rerun is a flake: the hop is Done and DYTEST counts it.", [Actor.TicketCaptain],
        [new(Skill.TicketCaptain, "Green on the rerun is a flake: the hop is Done, and DYTEST counts the flake; it becomes a ticket only when it repeats.", new(FeatureMergeState.Rerun, 608, MoveKind.Exit))])
    { Spec = ["R22", "Y9"], Transition = new(FeatureMergeState.Rerun, FeatureMergeState.MergeDone, "green on the rerun: a flake") };

    public static readonly Claim RealRed = new("A real red is reverted at once and becomes a CI-findings ticket.", [Actor.TicketCaptain],
    [
        new(Skill.TicketCaptain, "Red again, after any merge, is real: revert at once, cancel the Merge hop with `reverted: <gate> red on <merge SHA>`, and file a CI-findings ticket.", new(FeatureMergeState.Rerun, 609, MoveKind.Exit)),
        new(Skill.LinearWorkspaceStandard, "A reverted Merge goes Canceled with `reverted: <gate> red on <merge SHA>`.", new(Section.LinearStatuses, 4)),
        new(Skill.WorkingTreeStandard, "A gate red after a merge reruns once; green is a flake and the hop is Done; red again is real: the captain reverts at once and files a CI-findings ticket, a Fix follows on the ticket branch, then a fix-scoped round, then a re-land where `dydo merge` reverts the revert.",
            new(Section.TreeMerging, 5), () => [Red, Flake, FixAfterRevert!, Reland!])
    ])
    { Spec = ["R22", "Y9", "S12"], Transition = new(FeatureMergeState.Rerun, FeatureMergeState.Reverted, "red again: a real red") };

    public static readonly Claim FixAfterRevert = new("After a revert, a Fix on the ticket branch and a fix-scoped round.", [Actor.TicketCaptain],
        [new(Skill.TicketCaptain, "Send a code-writer to fix it on the ticket branch, then hold a fix-scoped round.", new(FeatureMergeState.Reverted, 610, MoveKind.Exit))])
    { Spec = ["R22", "R21"], Transition = new(FeatureMergeState.Reverted, FeatureMergeState.FixRound, "a Fix follows on the ticket branch") };

    public static readonly Claim Reland = new("The re-land runs dydo merge, which reverts the revert.", [Actor.TicketCaptain],
        [new(Skill.TicketCaptain, "Re-land with `dydo merge`, which reverts the revert.", new(FeatureMergeState.FixRound, 611, MoveKind.Exit))])
    { Spec = ["R22"], Transition = new(FeatureMergeState.FixRound, FeatureMergeState.PreGate, "the fix-scoped round passes; re-land") };

    public static readonly Claim GateFirst = new("The captain runs dydo test gate itself before dydo merge.", [Actor.TicketCaptain],
        [new(Skill.TicketCaptain, "Run `dydo test gate` on the combined tree yourself, then `dydo merge`.", new(FeatureMergeState.PreGate, 612, MoveKind.Nominal))])
    { Spec = ["R21", "Y4"] };

    public static readonly Claim OneEntry = new("dydo merge is the one entry point and states why it refuses.", [Actor.TicketCaptain],
        [new(Skill.TicketCaptain, "Merge only through `dydo merge`; when it refuses, its stated reason says why.", new(FeatureMergeState.Running, 613, MoveKind.Nominal))])
    { Spec = ["R23"] };

    public static readonly Claim NoMergeReview = new("A conflict-free merge into the feature branch gets no merge review.", [Actor.TicketCaptain],
    [
        new(Skill.TicketCaptain, "A conflict-free merge into the feature branch gets no merge review: the combined gates on the merge SHA and hosted CI on the `feature/**` push prove it.", new(FeatureMergeState.Running, 614, MoveKind.Nominal)),
        new(Skill.WorkingTreeStandard, "A conflict-free merge into the feature branch is proved by the combined gates on the merge SHA plus hosted CI on the `feature/**` push; there is no merge review.", new(Section.TreeMerging, 1))
    ])
    { Spec = ["R18"] };

    public static readonly Claim NoMutationWait = new("No ticket or merge waits on mutation.", [Actor.TicketCaptain],
        [new(Skill.TicketCaptain, "No ticket or merge waits on mutation; it is checked at the landing.", new(FeatureMergeState.Running, 615, MoveKind.Nominal))])
    { Spec = ["T12", "T14"] };
}
