namespace DynaDocs.SkillBuild;

/// <summary>
/// The ticket-captain prototype's waking resource (DYD-1372 gap hunt 2: B2, S3; gap hunt 3: N1, N5,
/// N7): where a holder woken in its own thread goes on from. Phase 1 is for a dispatch, so a woken
/// holder is sorted here first: its own ticket is its own, it re-reads the record, and each return asks
/// the same acts in the same order. The return is taken in before the holder goes on from its phase:
/// a commit made in a worktree of its own is merged into the whole first, so the next round judges a
/// head that holds it. The phase then routes and opens, and sends nothing out: first a step opened
/// ahead and no longer needed is cancelled and a Fix or Review opened after the Merge is made to block
/// it; only then is everything ready dispatched, the Merge never, because that step is its captain's
/// own and comes last. The posting of a returned SHA and the
/// route of an out-of-scope report are <see cref="CaptainDispatch"/>'s claims, stated here because a
/// return is when they are done.
/// </summary>
public static class CaptainWaking
{
    private const string Woke = "This fix's writer, 2026-11-21: woken in its own thread by its captain's `t3_thread_send` for fix g2, on DYD-1372, In Progress since 2026-11-20, " +
                                "with its fix g1a and that fix's return above the message in the same thread.";

    private const string Merges = "Linear, DYD-1290 read with its relations 2026-11-21: a Merge sub-ticket of DYD-1281, Done under the labels Merge and AFK, " +
                                  "`blockedBy` the part it merged, DYD-1287, and nothing else.";

    private static readonly Enum[] Me = [Actor.TicketCaptain];

    public static readonly Claim Mine = new("A holder tells its own ticket from another's by its own thread: the ticket it set In Progress there is its own, whatever its status says since.", Me,
    [
        At(Actor.TicketCaptain, CaptainSlot.WakeLead, 1,
            "A ticket you set In Progress yourself, earlier in this thread, is yours: its status is no conflict, and you need no take-over line.")
    ])
    {
        Spec = ["S7", "S6"],
        Truth = new(Evidence.Run(Woke + " Linear, DYD-1372 read the same day: the ticket shows that it was started and names no holder."),
            Layer: "Linear shows that a ticket was started and nothing of who started it, so the thread's own history is the one record of that.",
            Reason: "Without the test a holder sends back, as an intruder's, the very work it was woken to go on with.")
    };

    public static readonly Claim Reread = new("A woken holder re-reads its ticket, its sub-tickets' statuses and the last SHA posted on each before it goes on.", Me,
    [
        At(Actor.TicketCaptain, CaptainSlot.WakeSteps, 1,
            "Re-read your ticket, your sub-tickets' statuses and the last SHA posted on each: the record, never your memory of it, says where things stand.")
    ])
    {
        Spec = ["H3", "S6"],
        Truth = new(Evidence.Run(Woke + " Its own last commit was `d36ba509`, and the message that woke it said the branch had moved while it waited: " +
                                 "\"Head is `952536c3` (your commit, then the properties)\"."),
            Layer: "A thread keeps what its holder knew when it stopped, and nothing in it changes when another agent commits or moves a ticket.",
            Reason: "Two hours on, a sibling has merged or an officer has re-planned, and work resumed from a stale picture is built on a head that is gone.")
    };

    public static readonly Claim Part = new("A commit returned by a child that works in a worktree of its own, a part or a fix, is merged into its officer's branch by its SHA when the merge is the officer's, " +
                                            "and a conflict there goes to an Implement ticket; the whole is reviewed at one SHA that holds every part.", Me,
    [
        At(Actor.TicketCaptain, CaptainSlot.WakeSteps, 3,
            "When a child that works in its own worktree returns a commit that is yours to merge, merge it into your branch with `git merge <SHA>`: " +
            "the whole is reviewed at one SHA, your branch's head once it holds every part. A conflict there goes to an Implement ticket on your branch.")
    ])
    {
        Spec = ["R2", "H3", "K14"],
        Truth = new(Evidence.Run("`git log` on DYD-1372's branch, read 2026-11-21: `eaeca3f0` is a two-parent merge its captain made in its own checkout, on the ticket's branch, of `fcd71737`, " +
                                 "a commit a child made in a T3 worktree of its own, on another branch. `git rev-parse --git-common-dir`, run in a T3 worktree the same day, names one repository " +
                                 "that all 253 worktrees share, so a child's commit is reachable by its SHA. `git merge` ran unrefused on that ticket branch: " +
                                 "`Commands/GuardCommand.MergeRoutes.cs` refuses a PR merged around `dydo merge` and a push that updates the default branch, and holds no rule on a local merge. " +
                                 "The Implement kind takes a merge conflict: its row of the kind table, claim Captain.Kinds."),
            Layer: "T3 gives a child a checkout of its own and brings none of its commits back: a returned SHA stays on the child's branch until someone merges it.",
            Reason: "A reviewer handed a head that lacks something passes a tree nobody assembled, and the missing piece first meets the others after the verdict.")
    };

    public static readonly Claim GoOn = new("A woken holder goes on from the phase it had reached once the return is taken in, its SHA posted and its commit merged; " +
                                            "whatever that phase has it dispatch, and its own Merge, wait until the unneeded step is cancelled and each later step blocks the Merge.", Me,
    [
        At(Actor.TicketCaptain, CaptainSlot.WakeSteps, 5,
            "Go on from the phase you had reached; any sub-ticket it has you send out, and your own Merge, wait for the steps below.")
    ])
    {
        Spec = ["S6", "R8", "S3"],
        Truth = new(Evidence.Tool("delegate_task: `mode`, default 'async', wakes a dispatcher as each child returns; `isolation`, 'worktree', gives the child a \"new prepared checkout from the parent commit\", " +
                                  "so what it commits lies outside its dispatcher's checkout until that dispatcher merges it."),
            Layer: "A phase says what its holder does next, and nothing of what a return needs done to it first.",
            Reason: "A next round sent out ahead of the merge judges a head the repair never reached, and a ticket closed before the cancel leaves a dead card behind it.")
    };

    public static readonly Claim Unneeded = new("A sub-ticket opened ahead that its predecessor's outcome made unnecessary is cancelled with a one-line reason, and a cancelled step blocks nothing.", Me,
    [
        At(Actor.TicketCaptain, CaptainSlot.WakeSteps, 6,
            "Cancel a sub-ticket you opened ahead that the return made unnecessary, such as the Fix of a round that passed, with a one-line reason: a Canceled step blocks nothing.")
    ])
    {
        Spec = ["S10", "S3"],
        Truth = new(Evidence.Run("Linear, team Dydo, statuses read 2026-11-21: Canceled is a status, of the type canceled. Linear's documentation, Issue relations, read the same day: " +
                                 "\"Once the blocking issue has been resolved, the relationship moves under Related.\""),
            Layer: "Linear leaves a step opened ahead in Todo for as long as nobody moves it, and goes on listing the next step as blocked by it.",
            Reason: "A Fix nobody will ever start holds the merge behind a card that reads as work still owed.")
    };

    public static readonly Claim Wired = new("A Fix or Review opened after its Merge sub-ticket is made to block that Merge before anything newly ready is dispatched.", Me,
    [
        At(Actor.TicketCaptain, CaptainSlot.WakeSteps, 7,
            "A Fix or Review you opened after your Merge sub-ticket blocks nothing until you set it: make it block the Merge, with Linear's `save_issue` (`blocks`).")
    ])
    {
        Spec = ["S3", "R2", "R8"],
        Truth = new(Evidence.Tool("save_issue: `blocks`, \"Issue IDs/identifiers this blocks. Append-only; existing relations are never removed\", beside `blockedBy`. " +
                                  Merges + " So a relation stands only where a call set it."),
            Layer: "Linear draws a block only where one was set, and a Merge opened ahead was blocked by the steps that existed that day.",
            Reason: "Without it a second round runs beside a card that reads as free to go, and the board invites the very merge that round was ordered to hold.")
    };

    public static readonly Claim Ready = new("At each return a captain dispatches the sub-tickets it unblocked, never its Merge: those still unstarted and not sent out, whose every blocker is Done or Canceled.", Me,
    [
        At(Actor.TicketCaptain, CaptainSlot.WakeSteps, 8,
            "Dispatch each AFK sub-ticket, the Merge excepted, that is still in Todo, that you have not sent out already and that nothing blocks any more: " +
            "every sub-ticket that blocked it is Done or Canceled.")
    ])
    {
        Spec = ["S3", "R2"],
        Truth = new(Evidence.Run("Linear, DYD-1372 read with its relations 2026-11-21: a ticket lists `blocks`, `blockedBy` and `relatedTo`, each with the other ticket's key. " +
                                 "delegate_task takes any `task`, and reads no relation and no status of the ticket the task names. Team Dydo's statuses, read the same day: Todo is the one unstarted status. " +
                                 Merges + " So its Mode alone keeps no Merge out of a dispatch."),
            Layer: "`delegate_task` starts a child on a blocked ticket, or on one a child already holds, as readily as on a free one, and the child's own intake reads its status and its brief, never its blockers.",
            Reason: "A Review sent out ahead of its Implement has no candidate to look at, a Fix ahead of its round has no finding to close, and a second child on a running step overwrites the first.")
    };

    public static readonly Claim MergeNext = new("A captain's Merge sub-ticket is never dispatched at a wake: once its review rounds have ended and nothing blocks the Merge, the captain runs it itself, as its own next step.", Me,
    [
        At(Actor.TicketCaptain, CaptainSlot.WakeSteps, 9,
            "The Merge is your own step: once the review rounds have ended and nothing blocks it, it is your next one, by [Merging](merging.md).")
    ])
    {
        Spec = ["R21", "S6", "R8"],
        Truth = new(Evidence.Run(Merges + " This skill holds no kind resource for a merge: `tools/SkillBuild/Model4/Carriers/CaptainCarrier.cs` gives its first phase Implement, Review and Research, " +
                                 "so a child sent onto a Merge would find no recipe to load."),
            Layer: "Linear lists an unblocked Merge among the tickets free to go and cannot say that this one has no holder but its captain.",
            Reason: "Left as one more card to hand out, it goes to a child with nothing to load, or waits for a taker that will never come.")
    };

    private static Line At(Actor reader, Enum slot, int order, string text) =>
        new(CaptainSkill.TicketCaptain, reader, text, new(slot, order));
}
