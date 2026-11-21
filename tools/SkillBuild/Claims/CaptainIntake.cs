namespace DynaDocs.SkillBuild;

/// <summary>
/// The ticket-captain prototype's checks before a ticket is started (DYD-1372 gap hunt 1: G10, G11):
/// the one line of the spine every holder reads, and the route of each check that fails, which only
/// the holder it stops reads. A ticket shows its status and nothing of its holder, so the check names
/// only what a holder can read: the status, and its own brief.
/// </summary>
public static class CaptainIntake
{
    private const string Status = "Linear, DYD-1372 read 2026-11-21: `status` In Progress, `statusType` started and `startedAt` set, with no assignee, no delegate and no attachment but its PR. " +
                                  "Team Dydo's statuses, read the same day: Todo is the one unstarted status.";

    private const string OwnBrief = "This fix's writer was briefed (DYD-1372 phase 1, fix g1a): ";

    private static readonly Enum[] Me = [Actor.TicketCaptain];

    public static readonly Claim Woken = new("Phase 1 is read once, at dispatch; a holder woken later in its own thread goes on from the waking resource.", Me,
    [
        At(Actor.TicketCaptain, CaptainSlot.Take, 0,
            "This phase is for your dispatch. Woken later in this thread, by a return or a message, go on by [Waking](resources/waking.md).")
    ])
    {
        Spec = ["S7", "H12"],
        Truth = new(Evidence.Tool("t3_thread_send: \"A send to a finished child thread starts a fresh task and returns its `taskId`.\", and `mode:'auto'` \"starts an idle thread, steers a running turn, or queues\". " +
                                  "delegate_task: `mode`, default 'async', wakes a dispatcher as each child returns."),
            Layer: "T3 wakes a thread with a return or a message and says nothing of where its holder had got to.",
            Reason: "A holder that runs its intake a second time finds its own ticket started and takes itself for an intruder.")
    };

    public static readonly Claim Check = new("Before it starts a ticket, its holder checks that the ticket is unstarted and that its brief and the contract give it what the work needs.", Me,
    [
        At(Actor.TicketCaptain, CaptainSlot.Take, 2,
            "Then check that the ticket is unstarted and that your brief and its contract give you what the work needs; " +
            "if either fails, or your brief calls it filler, go by [Before you start](resources/start.md).")
    ])
    {
        Spec = ["S7", "K14", "S15"],
        Truth = new(Evidence.Run(Status + " So a ticket shows that it was started, and nothing of who holds it."),
            Layer: "Linear sets a started ticket In Progress a second time without a word, and T3 starts a child on whatever brief it is handed.",
            Reason: "One read ahead of the first act is cheap; found later, the same fault has cost two agents' work or a change built on a guess.")
    };

    public static readonly Claim Lacking = new("A holder whose brief or contract lacks what the work needs asks its officer for exactly that, and leaves what depends on it unstarted.", Me,
    [
        At(Actor.TicketCaptain, CaptainSlot.StartChecks, 1,
            "Your brief or the ticket's contract lacks something the work needs, such as who merges, the paths you own or the gates you must pass: " +
            "ask your officer for exactly that, by the step of [A need for the user](need-for-the-user.md) that sends to your officer's thread, and start nothing that depends on it.")
    ])
    {
        Spec = ["K14", "S15", "H12"],
        Truth = new(Evidence.Run(OwnBrief + "it named who merges, \"Do not push; no PR; no merge.\", the owned paths and the gates before return. " +
                                 "`resources/briefs.md` makes all three part of every brief, and no tool reads a brief for them. " +
                                 "t3_thread_send: \"Send a message to any thread\"."),
            Layer: "T3 passes a brief on untouched, whatever it leaves out, and Linear takes a ticket whose description is empty.",
            Reason: "A guess at who merges picks the wrong way to finish, and a guess at the owned paths writes into another writer's files.")
    };

    public static readonly Claim Held = new("A holder dispatched onto a ticket that another agent started or closed, with no word in its brief that it takes the ticket over, " +
                                            "starts nothing and returns the dispatch to its officer as a conflict.", Me,
    [
        At(Actor.TicketCaptain, CaptainSlot.StartChecks, 2,
            "The ticket is already started, or closed, by someone else: you did not set it In Progress in this thread, and your brief does not say that you take it over. " +
            "Start nothing, and return the dispatch to your officer as a conflict, with the ticket's key and its status.")
    ])
    {
        Spec = ["S7", "S6"],
        Truth = new(Evidence.Run(Status + " " + OwnBrief + "DYD-1372 was In Progress when it was dispatched, and its brief said so: " +
                                 "\"You are a fresh writer: the writer who built this part ran out of context and started nothing on this fix.\""),
            Layer: "Linear shows a ticket's status and nothing of its holder, and sets it In Progress again for anyone who asks.",
            Reason: "Two agents on one ticket each overwrite what the other built, and neither learns of it before the merge."),
        Transition = new(CaptainState.Dispatched, CaptainState.Returned, "the ticket is already started or closed and the brief names no take-over: the dispatch goes back as a conflict")
    };

    private static Line At(Actor reader, Enum slot, int order, string text) =>
        new(CaptainSkill.TicketCaptain, reader, text, new(slot, order));
}
