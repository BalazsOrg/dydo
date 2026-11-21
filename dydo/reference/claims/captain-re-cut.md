---
area: reference
type: reference
---
<!-- built from the model; edit there: tools/SkillBuild -->

# Claims: CaptainReCut

## Undelivered

`claim:CaptainReCut.Undelivered`: When the rewritten contract cannot be delivered to the child for a reason other than a limit, its officer notes the failed delivery on the ticket and takes the ticket's closure to the officer above; it never counts on the child to close.

Spec: S6, H12, H2

Evidence (tool): t3_thread_send: "The target cannot run with broader modes than yours.", one refusal no retry cures. t3_thread_list: a thread's `statuses` include `failed`, `cancelled` and `interrupted`, so a child's thread can be past waking. save_issue: `description` keeps the rewritten contract on the ticket whoever reads it next.

Layer: T3 reports a send that failed to its sender and to nobody else, and Linear goes on showing the ticket In Progress under a holder that will never hear of the new contract.

Reason: Unrecorded, the reduced card waits for a close that cannot come, and whoever is handed it later finds neither why it stalled nor what was already built.

**ticket-captain** (TooBigOfficer; read by TicketCaptain):

> If that send does not get through, for another reason than a limit, note the failed delivery on the ticket in one line, beside the rewritten contract and the SHA, and take the ticket's closure to your officer, by the step of A need for the user (`need-for-the-user.md`) that sends to your officer's thread: never count on the child to close it.
