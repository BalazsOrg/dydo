---
area: reference
type: reference
---
<!-- built from the model; edit there: tools/SkillBuild -->

# Claims: CaptainPassedUp

## Own

`claim:CaptainPassedUp.Own`: The officer that receives a Question a child sends up unsettled opens a Question of its own for it, which names the child's by its key, carries what was found and blocks the child's; its fresh captain is dispatched onto that ticket, never onto the child's.

Spec: H16, S10, S7

Evidence (tool): save_issue, read 2026-11-21: `parentId`, "Parent issue ID or identifier", and `blocks`, "Issue IDs/identifiers this blocks. Append-only; existing relations are never removed", which takes any ticket's key, a child's sub-ticket included. delegate_task: "Start one child agent with only `task`. No parent history.", so the new captain has only what its ticket carries. A captain sent onto a ticket another agent started returns the dispatch as a conflict (claim CaptainIntake.Held).

Layer: `delegate_task` starts a child on any ticket its task names, a held one included, and Linear shows the child's Question In Progress with no word on who may go on with it.

Reason: A second researcher put on a card somebody else began sends the dispatch back as a conflict, or repeats searches the first one already wrote down.

**ticket-captain** (NeedSteps; read by TicketCaptain):

> When the need is an AFK Question a child sent up unsettled, your sub-ticket is a Question of your own: put the child's Question's key and what was found on it, for its captain to start from, and have it block the child's. The next step dispatches onto yours, never onto the child's, which its own captain started and left open.

## Closes

`claim:CaptainPassedUp.Closes`: A Question its captain returned unsettled is closed by that captain once the answer has come: the officer that dispatched it sends the answer to its thread under the Question's key, and it closes the ticket Done on that answer and returns.

Spec: S6, H16, H12

Evidence (tool): t3_thread_send, read 2026-11-21: "A send to a finished child thread starts a fresh task and returns its `taskId`." Its `message` is all it carries, so the key in it tells the woken captain which ticket the answer is for. save_issue: `state`, "State type, name, or ID".

Layer: Linear leaves the Question In Progress once its captain's turn has ended, whoever writes an answer on it, and T3 starts a finished thread only on a send.

Reason: Left In Progress, an answered card keeps every sub-ticket behind it out of the next dispatch; moved by its officer, it reads as a step ended by its starter, which nobody did.

**ticket-captain** (NeedSteps; read by TicketCaptain):

> A Question of yours whose captain returned it unsettled is still open when its answer comes: send the answer to that captain's thread, under the Question's key, with `t3_thread_send`. That captain started the ticket, so it closes it Done on the answer and returns. If the send does not get through, for another reason than a limit, note the failed delivery on the Question in one line and take its closure to your officer, by the step below.

## Lost

`claim:CaptainPassedUp.Lost`: When the answer cannot be delivered to the captain that started the Question, for a reason other than a limit, its officer notes the failed delivery on the Question and takes its closure to the officer above.

Spec: S6, H12

Evidence (tool): t3_thread_send, read 2026-11-21: "The target cannot run with broader modes than yours.", one refusal no retry cures. t3_thread_list: a thread's `statuses` include `failed`, `cancelled` and `interrupted`, so a thread can be past waking.

Layer: T3 reports a send that failed to its sender alone, and nothing in Linear tells an open Question from one whose starter can no longer be woken.

Reason: Unrecorded, the card waits for good on an agent that will never hear of it, and what it holds back is never sent out.

**ticket-captain** (NeedSteps; read by TicketCaptain, on claim:CaptainPassedUp.Closes):

> A Question of yours whose captain returned it unsettled is still open when its answer comes: send the answer to that captain's thread, under the Question's key, with `t3_thread_send`. That captain started the ticket, so it closes it Done on the answer and returns. If the send does not get through, for another reason than a limit, note the failed delivery on the Question in one line and take its closure to your officer, by the step below.

## Woken

`claim:CaptainPassedUp.Woken`: A Question's captain that returned its ticket unsettled is woken with the answer once one is found above: it closes its ticket Done on that answer and returns.

Spec: S6, H16

Evidence (tool): t3_thread_send, read 2026-11-21: "A send to a finished child thread starts a fresh task and returns its `taskId`." Its `freshContext`, "Start a new provider session without the thread's earlier context", is a flag the sender has to set, so the thread that returned unsettled starts again with its research above the message.

Layer: A woken thread holds its old return, and nothing in it says that the ticket it left open is now its own to end.

Reason: Told at the return what the later wake asks, a researcher ends its step in one move instead of studying an answered question again.

**ticket-captain** (ResearchRoutes; read by TicketCaptain, on claim:CaptainResearch.Question):

> They leave anything else open: lay the call out on your ticket, and return it to your officer as unsettled, the Question still open. Your officer sends the answer to your thread once there is one: woken with it, close your ticket Done on that answer and return.
