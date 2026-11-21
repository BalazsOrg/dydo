namespace DynaDocs.SkillBuild;

/// <summary>
/// The ticket-captain prototype's waking resource (DYD-1372 gap hunt 2: B2, S3): where a holder woken
/// in its own thread goes on from. Phase 1 is for a dispatch, so a woken holder is sorted here first:
/// its own ticket is its own, it re-reads the record, and each return asks the same acts in the same
/// order, the cancel of a step opened ahead and the dispatch of whatever the return unblocked among
/// them. The posting of a returned SHA and the route of an out-of-scope report are
/// <see cref="CaptainDispatch"/>'s claims, stated here because a return is when they are done.
/// </summary>
public static class CaptainWaking
{
    private const string Woke = "This fix's writer, 2026-11-21: woken in its own thread by its captain's `t3_thread_send` for fix g2, on DYD-1372, In Progress since 2026-11-20, " +
                                "with its fix g1a and that fix's return above the message in the same thread.";

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
            "Re-read your ticket, your sub-tickets' statuses and the last SHA posted on each, and go on from the phase you had reached: the record, never your memory of it, says where things stand.")
    ])
    {
        Spec = ["H3", "S6"],
        Truth = new(Evidence.Run(Woke + " Its own last commit was `d36ba509`, and the message that woke it said the branch had moved while it waited: " +
                                 "\"Head is `952536c3` (your commit, then the properties)\"."),
            Layer: "A thread keeps what its holder knew when it stopped, and nothing in it changes when another agent commits or moves a ticket.",
            Reason: "Two hours on, a sibling has merged or an officer has re-planned, and work resumed from a stale picture is built on a head that is gone.")
    };

    public static readonly Claim Unneeded = new("A sub-ticket opened ahead that its predecessor's outcome made unnecessary is cancelled with a one-line reason, and a cancelled step blocks nothing.", Me,
    [
        At(Actor.TicketCaptain, CaptainSlot.WakeSteps, 4,
            "Cancel a sub-ticket you opened ahead that the return made unnecessary, such as the Fix of a round that passed, with a one-line reason: a Canceled step blocks nothing.")
    ])
    {
        Spec = ["S10", "S3"],
        Truth = new(Evidence.Run("Linear, team Dydo, statuses read 2026-11-21: Canceled is a status, of the type canceled. Linear's documentation, Issue relations, read the same day: " +
                                 "\"Once the blocking issue has been resolved, the relationship moves under Related.\""),
            Layer: "Linear leaves a step opened ahead in Todo for as long as nobody moves it, and goes on listing the next step as blocked by it.",
            Reason: "A Fix nobody will ever start holds the merge behind a card that reads as work still owed.")
    };

    public static readonly Claim Ready = new("At each return a captain dispatches the sub-tickets it unblocked: those whose every blocker is Done or Canceled.", Me,
    [
        At(Actor.TicketCaptain, CaptainSlot.WakeSteps, 5,
            "Dispatch each AFK sub-ticket that nothing blocks any more: every sub-ticket that blocked it is Done or Canceled.")
    ])
    {
        Spec = ["S3", "R2"],
        Truth = new(Evidence.Run("Linear, DYD-1372 read with its relations 2026-11-21: a ticket lists `blocks`, `blockedBy` and `relatedTo`, each with the other ticket's key. " +
                                 "delegate_task takes any `task`, and reads no relation of the ticket the task names."),
            Layer: "`delegate_task` starts a child on a blocked ticket as readily as on a free one, and the child's own intake reads its status and its brief, never its blockers.",
            Reason: "A Review sent out ahead of its Implement has no candidate to look at, and a Fix ahead of its round has no finding to close.")
    };

    private static Line At(Actor reader, Enum slot, int order, string text) =>
        new(CaptainSkill.TicketCaptain, reader, text, new(slot, order));
}
