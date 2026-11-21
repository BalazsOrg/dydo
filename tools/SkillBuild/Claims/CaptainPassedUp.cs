namespace DynaDocs.SkillBuild;

/// <summary>
/// The ticket-captain prototype's Question that a child sends up unsettled (DYD-1372 gap hunt 6, J1;
/// spec H16, S6). Where it arrives it is a need with no ticket at that level: the receiving officer
/// opens a Question of its own, and its fresh captain is dispatched onto that ticket, never onto the
/// child's, which its own captain started and left open. On the way down, each Question still open is
/// closed by the captain that started it, woken by its officer with the answer, as a ticket returned
/// too big is closed (<see cref="CaptainTrouble.Handed"/>). The steps these lines stand among are
/// <see cref="CaptainNeed"/>'s.
/// </summary>
public static class CaptainPassedUp
{
    private const string Restart = "t3_thread_send, read 2026-11-21: \"A send to a finished child thread starts a fresh task and returns its `taskId`.\"";

    private static readonly Enum[] Me = [Actor.TicketCaptain];

    public static readonly Claim Own = new("The officer that receives a Question a child sends up unsettled opens a Question of its own for it, which names the child's by its key, carries what was found " +
                                           "and blocks the child's; its fresh captain is dispatched onto that ticket, never onto the child's.", Me,
    [
        At(Actor.TicketCaptain, CaptainSlot.NeedSteps, 3,
            "When the need is an AFK Question a child sent up unsettled, your sub-ticket is a Question of your own: put the child's Question's key and what was found on it, for its captain to start from, " +
            "and have it block the child's. The next step dispatches onto yours, never onto the child's, which its own captain started and left open.")
    ])
    {
        Spec = ["H16", "S10", "S7"],
        Truth = new(Evidence.Tool("save_issue, read 2026-11-21: `parentId`, \"Parent issue ID or identifier\", and `blocks`, \"Issue IDs/identifiers this blocks. Append-only; existing relations are never removed\", " +
                                  "which takes any ticket's key, a child's sub-ticket included. delegate_task: \"Start one child agent with only `task`. No parent history.\", " +
                                  "so the new captain has only what its ticket carries. A captain sent onto a ticket another agent started returns the dispatch as a conflict (claim CaptainIntake.Held)."),
            Layer: "`delegate_task` starts a child on any ticket its task names, a held one included, and Linear shows the child's Question In Progress with no word on who may go on with it.",
            Reason: "A second researcher put on a card somebody else began sends the dispatch back as a conflict, or repeats searches the first one already wrote down.")
    };

    public static readonly Claim Closes = new("A Question its captain returned unsettled is closed by that captain once the answer has come: the officer that dispatched it sends the answer to its thread " +
                                              "under the Question's key, and it closes the ticket Done on that answer and returns.", Me,
    [
        At(Actor.TicketCaptain, CaptainSlot.NeedSteps, 6,
            "A Question of yours whose captain returned it unsettled is still open when its answer comes: send the answer to that captain's thread, under the Question's key, with `t3_thread_send`. " +
            "That captain started the ticket, so it closes it Done on the answer and returns. If the send does not get through, for another reason than a limit, " +
            "note the failed delivery on the Question in one line and take its closure to your officer, by the step below.",
            () => [Lost!])
    ])
    {
        Spec = ["S6", "H16", "H12"],
        Truth = new(Evidence.Tool(Restart + " Its `message` is all it carries, so the key in it tells the woken captain which ticket the answer is for. save_issue: `state`, \"State type, name, or ID\"."),
            Layer: "Linear leaves the Question In Progress once its captain's turn has ended, whoever writes an answer on it, and T3 starts a finished thread only on a send.",
            Reason: "Left In Progress, an answered card keeps every sub-ticket behind it out of the next dispatch; moved by its officer, it reads as a step ended by its starter, which nobody did.")
    };

    public static readonly Claim Lost = new("When the answer cannot be delivered to the captain that started the Question, for a reason other than a limit, its officer notes the failed delivery " +
                                            "on the Question and takes its closure to the officer above.", Me, [])
    {
        Spec = ["S6", "H12"],
        Truth = new(Evidence.Tool("t3_thread_send, read 2026-11-21: \"The target cannot run with broader modes than yours.\", one refusal no retry cures. t3_thread_list: a thread's `statuses` include " +
                                  "`failed`, `cancelled` and `interrupted`, so a thread can be past waking."),
            Layer: "T3 reports a send that failed to its sender alone, and nothing in Linear tells an open Question from one whose starter can no longer be woken.",
            Reason: "Unrecorded, the card waits for good on an agent that will never hear of it, and what it holds back is never sent out.")
    };

    public static readonly Claim Woken = new("A Question's captain that returned its ticket unsettled is woken with the answer once one is found above: it closes its ticket Done on that answer and returns.", Me, [])
    {
        Spec = ["S6", "H16"],
        Truth = new(Evidence.Tool(Restart + " Its `freshContext`, \"Start a new provider session without the thread's earlier context\", is a flag the sender has to set, " +
                                  "so the thread that returned unsettled starts again with its research above the message."),
            Layer: "A woken thread holds its old return, and nothing in it says that the ticket it left open is now its own to end.",
            Reason: "Told at the return what the later wake asks, a researcher ends its step in one move instead of studying an answered question again.")
    };

    private static Line At(Actor reader, Enum slot, int order, string text, Func<Claim[]>? also = null) =>
        new(CaptainSkill.TicketCaptain, reader, text, new(slot, order), also);
}
