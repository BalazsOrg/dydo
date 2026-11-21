---
area: reference
type: reference
---
<!-- built from the model; edit there: tools/SkillBuild -->

# Claims: CaptainNeed

## Never

`claim:CaptainNeed.Never`: A captain never takes a need to the user itself: a need goes up through its officer, and only the coordinator sends an envoy to the user.

Spec: H16, H18, H23

Evidence (run): T3, 2026-11-21: this skill's fixer sent a note from inside its turn to its captain's thread with `t3_thread_send`, both threads in mode `auto`. The send came back `delivery: started`, and the fixer's turn went on. `t3_thread_list` gives the address, a thread's `parentThreadId`, which the header of its brief names too. `skills/roles/officers/coordinator/SKILL.md`, the coordinator's skill on this branch, not yet truth-checked: "Everything that needs him goes out through your own envoy". The user kept this process when asked how a need should reach him. Spec section 6 records it: "answered: (a), the current process (H16; relayed by the chief of staff; open to reopening on sight)". His words, "Yeah, probably a.", are in the chief of staff's project update 9099daa1 on the dydo 4.0.0 Project, 2026-11-21 (https://linear.app/bodnar-balazs/project/dydo-400-6ab0e0094779/activity#project-update-9099daa1).

Layer: The host's question tool asks the user whatever an agent sends it, and nothing counts how many agents ask at once.

Reason: His attention is the one thing the swarm cannot make more of, so a single queue spends it.

**ticket-captain** (NeedLead; read by TicketCaptain):

> You never take a need to the user yourself: it goes up through your officer, and only the coordinator sends an envoy to the user.

## Level

`claim:CaptainNeed.Level`: A need a child passes up is taken by the officer that receives it through the same steps as a need of its own.

Spec: H16, H12

Evidence (run): `skills/productivity/wayfinder/SKILL.md`, read 2026-11-21: "A need passed up opens at stage `research`, held by a fresh ticket captain who researches it first". The user kept this process: spec section 6, question 1, and project update 9099daa1, quoted in claim CaptainNeed.Never.

Layer: A child's message lands in its officer's thread, and nothing there says what the officer owes it.

Reason: An officer often holds what its child lacked, so the same question is cheaper to settle one level up than at the top.

**ticket-captain** (NeedLead; read by TicketCaptain):

> Your own need, and one a child sends up to you, take these steps in order.

## Here

`claim:CaptainNeed.Here`: A call that is obvious and reversible from where its holder stands is decided there, and gets no ticket.

Spec: H16

Evidence (run): `skills/productivity/wayfinder/SKILL.md`, read 2026-11-21: "Decide an obvious, reversible call where it arises; it never becomes a ticket (H16)." The user kept this process: spec section 6, question 1, and project update 9099daa1, quoted in claim CaptainNeed.Never.

Layer: Linear takes a Question for any call, the plain ones included.

Reason: A ticket and a wait for something its reader can see at a glance stops a child for nothing.

**ticket-captain** (NeedSteps; read by TicketCaptain):

> Decide a call yourself when, from where you stand, it is obvious and reversible; it gets no ticket.

## Enable

`claim:CaptainNeed.Enable`: What only the user can give, a token or an authorisation, is an Enablement sub-ticket.

Spec: H17

Evidence (run): Linear, team Dydo, labels read 2026-11-21: the Type group holds Enablement, "Access, environment, credentials or material other work needs", beside Question. The user kept this process: spec section 6, question 1, and project update 9099daa1, quoted in claim CaptainNeed.Never.

Layer: Linear holds both labels and cannot tell a thing to be given from a call to be made.

Reason: Research can settle a call and can never produce a token, so the two take different routes from the start.

**ticket-captain** (NeedSteps; read by TicketCaptain, on claim:CaptainNeed.Ask):

> Open a sub-ticket for any other need, by Sub-tickets (`sub-tickets.md`), blocking only what waits on it, and go on with the rest: an Enablement for what only the user can give, a token or an authorisation; a Question for a call. A child's need that is already an Enablement, or a call only the user can make, keeps the sub-ticket the child opened.

## Carried

`claim:CaptainNeed.Carried`: A child's Enablement, or its call only the user can make, goes up as the one sub-ticket the child opened.

Spec: H18, R9

Evidence (run): `skills/roles/officers/coordinator/SKILL.md`, the coordinator's skill on this branch, not yet truth-checked, queues the tickets themselves: it parents "its member Questions and Enablements" under one wrapper, each carrying "the blocking relations of the work that waits". The user kept this process: spec section 6, question 1, and project update 9099daa1, quoted in claim CaptainNeed.Never.

Layer: Linear takes a second ticket for the same token at every level it passes.

Reason: One ticket keeps every block on it in a single place, where whoever ranks the user's queue can count them.

**ticket-captain** (NeedSteps; read by TicketCaptain, on claim:CaptainNeed.Ask):

> Open a sub-ticket for any other need, by Sub-tickets (`sub-tickets.md`), blocking only what waits on it, and go on with the rest: an Enablement for what only the user can give, a token or an authorisation; a Question for a call. A child's need that is already an Enablement, or a call only the user can make, keeps the sub-ticket the child opened.

## Ask

`claim:CaptainNeed.Ask`: A call that is not obvious and reversible becomes a Question sub-ticket that blocks only what waits on it.

Spec: H16, S10

Transition in A captain and its ticket (`claims/statechart/captain/Captain.tla`): Leaf → Question, "a call that is not obvious: a Question sub-ticket".

Evidence (run): Linear, DYD-1372 read 2026-11-20: a ticket carries `blocks` and `blockedBy` relations beside its children. The user kept this process: spec section 6, question 1, and project update 9099daa1, quoted in claim CaptainNeed.Never.

Layer: Linear draws a block once it is set, and stops nobody from working on.

Reason: One open question holds up one part, never the whole ticket.

**ticket-captain** (NeedSteps; read by TicketCaptain):

> Open a sub-ticket for any other need, by Sub-tickets (`sub-tickets.md`), blocking only what waits on it, and go on with the rest: an Enablement for what only the user can give, a token or an authorisation; a Question for a call. A child's need that is already an Enablement, or a call only the user can make, keeps the sub-ticket the child opened.

## Skip

`claim:CaptainNeed.Skip`: A need already known to be the user's gets no research.

Spec: H17, R9

Evidence (run): Linear, team Dydo, labels read 2026-11-21: Enablement closes "on the condition true, with evidence", which no source can supply. The user kept this process: spec section 6, question 1, and project update 9099daa1, quoted in claim CaptainNeed.Never.

Layer: Nothing keeps a captain from dispatching a researcher onto a need that no source holds.

Reason: A captain set to study what only he can give costs a day, and his answer is the slowest of all to arrive.

**ticket-captain** (NeedSteps; read by TicketCaptain, on claim:CaptainNeed.Fresh):

> Dispatch a fresh captain onto a Question with `delegate_task`, to research it first; skip this when you already know that only the user can meet the need. A call that only wants the user's approval is never such a need.

## Approval

`claim:CaptainNeed.Approval`: A call that only wants the user's approval is never a need only the user can meet.

Spec: L18

Evidence (tool): AskUserQuestion: "Use this tool only when you are blocked on a decision that is genuinely the user's to make". The user kept this process: spec section 6, question 1, and project update 9099daa1, quoted in claim CaptainNeed.Never.

Layer: Nothing tells a call the user alone can make from one an agent would merely like confirmed.

Reason: A rubber stamp costs him the time to understand the question and returns nothing the sources could not.

**ticket-captain** (NeedSteps; read by TicketCaptain, on claim:CaptainNeed.Fresh):

> Dispatch a fresh captain onto a Question with `delegate_task`, to research it first; skip this when you already know that only the user can meet the need. A call that only wants the user's approval is never such a need.

## Fresh

`claim:CaptainNeed.Fresh`: What the sources can answer never reaches the user: a Question is held by a fresh captain, dispatched with delegate_task, who researches it first.

Spec: H16, L18

Evidence (run): delegate_task: "Start one child agent with only `task`. No parent history." `skills/productivity/wayfinder/SKILL.md`, read 2026-11-21: "A need passed up opens at stage `research`, held by a fresh ticket captain who researches it first". The user kept this process: spec section 6, question 1, and project update 9099daa1, quoted in claim CaptainNeed.Never.

Layer: The host's question tool is one call away, and the sources that may hold the answer are many.

Reason: A context full of its own work answers with a bias; a fresh one reads the sources before it says anything.

**ticket-captain** (NeedSteps; read by TicketCaptain):

> Dispatch a fresh captain onto a Question with `delegate_task`, to research it first; skip this when you already know that only the user can meet the need. A call that only wants the user's approval is never such a need.

## Down

`claim:CaptainNeed.Down`: An answer to a need a child passed up is put on the child's sub-ticket and sent to the child's thread.

Spec: H16, H12

Evidence (run): T3, 2026-11-21: three messages from its captain's thread reached this skill's writer inside its running turn (DYD-1372 phase 1, hop A). t3_thread_send: `mode:'auto'` "starts an idle thread, steers a running turn, or queues", and "A send to a finished child thread starts a fresh task and returns its `taskId`." The user kept this process: spec section 6, question 1, and project update 9099daa1, quoted in claim CaptainNeed.Never.

Layer: T3 wakes a thread only when something is sent to it; a comment on a ticket wakes nobody.

Reason: Without the send, a child that ended its turn on a need would wait on an answer that already exists.

**ticket-captain** (NeedSteps; read by TicketCaptain, on claim:CaptainNeed.Answered):

> When the answer is on the Question ticket, resume what waited; an answer to a child's need you put on the child's sub-ticket and send to the child's thread with `t3_thread_send`.

## Answered

`claim:CaptainNeed.Answered`: The blocked work resumes when the answer is on the Question ticket.

Spec: H16

Transition in A captain and its ticket (`claims/statechart/captain/Captain.tla`): Question → Leaf, "the answer is on the Question ticket".

Evidence (run): Linear, team Dydo, labels read 2026-11-20: Question is described "Resolution: the answer, posted on close". The user kept this process: spec section 6, question 1, and project update 9099daa1, quoted in claim CaptainNeed.Never.

Layer: T3 wakes the captain when the Question's captain returns; the answer itself is on the ticket.

Reason: The answer outlives both threads, for whoever meets the same call next.

**ticket-captain** (NeedSteps; read by TicketCaptain):

> When the answer is on the Question ticket, resume what waited; an answer to a child's need you put on the child's sub-ticket and send to the child's thread with `t3_thread_send`.

## AtOnce

`claim:CaptainNeed.AtOnce`: A need only the user can meet goes to the officer at once.

Spec: R9, S10

Evidence (run): T3, 2026-11-21: this skill's fixer sent a note from inside its turn to its captain's thread with `t3_thread_send`, both threads in mode `auto`. The send came back `delivery: started`, and the fixer's turn went on. `t3_thread_list` gives the address, a thread's `parentThreadId`, which the header of its brief names too. The user kept this process: spec section 6, question 1, and project update 9099daa1, quoted in claim CaptainNeed.Never.

Layer: Nothing above a captain learns of a need until the captain sends it.

Reason: His answer is the slowest route there is, so every hour before it is asked for lengthens the wait.

**ticket-captain** (NeedSteps; read by TicketCaptain, on claim:CaptainNeed.Up):

> Send what stays open to your officer's thread with `t3_thread_send`, by its key and with what was found: at once for a need only the user can meet, and for a Question when it comes back unsettled. Leave the sub-ticket open; what waited still waits on its answer.

## Up

`claim:CaptainNeed.Up`: A Question that comes back unsettled goes to the officer's thread with what was found and stays open: its holder still waits on the answer.

Spec: H16, H12, S10

Transition in A captain and its ticket (`claims/statechart/captain/Captain.tla`): Question → Question, "the Question comes back unsettled: it goes to the officer and stays open, and the work still waits on its answer".

Evidence (run): T3, 2026-11-21: this skill's fixer sent a note from inside its turn to its captain's thread with `t3_thread_send`, both threads in mode `auto`. The send came back `delivery: started`, and the fixer's turn went on. `t3_thread_list` gives the address, a thread's `parentThreadId`, which the header of its brief names too. The user kept this process: spec section 6, question 1, and project update 9099daa1, quoted in claim CaptainNeed.Never.

Layer: A captain has no tool that opens a thread with the user, and its officer is the only one above it.

Reason: Whether a call is worth his time is weighed once, above, instead of by every captain that meets one.

**ticket-captain** (NeedSteps; read by TicketCaptain):

> Send what stays open to your officer's thread with `t3_thread_send`, by its key and with what was found: at once for a need only the user can meet, and for a Question when it comes back unsettled. Leave the sub-ticket open; what waited still waits on its answer.

## Refused

`claim:CaptainNeed.Refused`: A send to the officer's thread is refused when the sender's mode is narrower than the officer's; the holder then ends its turn on the message, and its work stops until the officer sends to its thread again.

Spec: H12, H23

Evidence (run): t3_thread_send: "The target cannot run with broader modes than yours." The way round it ran in this ticket: each return of this skill's fixer, the last message of its turn, reached its captain, whose next `t3_thread_send` to the finished thread started it again with its context, twice by 2026-11-21. No thread here ran in a narrower mode than its officer, so the refusal itself is the schema's word.

Layer: T3 refuses the send and names no other way up; a child's return is the one route that no mode closes.

Reason: Without it a narrower child would sit on a need that nobody above ever hears of.

**ticket-captain** (NeedSteps; read by TicketCaptain):

> If that send is refused, because your thread's mode is narrower than your officer's, end your turn on the message instead; your work then stops until your officer sends to your thread again.

## InThread

`claim:CaptainNeed.InThread`: When the user is already in an officer's thread, it may ask a one-line question there.

Spec: H22, L18

Evidence (tool): AskUserQuestion: "Use this tool only when you are blocked on a decision that is genuinely the user's to make".

Layer: The tool asks whenever it is called; whether the user is there to answer, only the thread shows.

Reason: A ticket and a wait for something the user can answer in a word would be overkill.

**ticket-captain** (NeedRules; read by TicketCaptain):

> When the user is already in your thread, you may ask one line there instead, with the host's question tool: never what the sources can answer, and never a call that only wants the user's approval.

## Cite

`claim:CaptainNeed.Cite`: A question put to the user names what each decision record says, never only its number.

Spec: H24

Evidence (code): `dydo/project/decisions`

Layer: A record's number resolves to a file for an agent, and to nothing for a person reading a question.

Reason: The user answers the question in front of him without looking anything up.

**ticket-captain** (NeedRules; read by TicketCaptain):

> Say what a decision record decides whenever you cite one to the user, never only its number.
