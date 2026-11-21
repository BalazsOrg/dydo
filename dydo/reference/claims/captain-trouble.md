---
area: reference
type: reference
---
<!-- built from the model; edit there: tools/SkillBuild -->

# Claims: CaptainTrouble

## Save

`claim:CaptainTrouble.Save`: A captain whose leaf proves too big first commits what is whole and posts its SHA.

Spec: K9, H3

Evidence (run): This skill's writer was briefed (DYD-1372 hop 2): "commit each part when it is whole and green".

Layer: An agent near the end of its context can stop mid-file, and nothing saves the work for it.

Reason: Whoever goes on, the captain or a fresh agent, starts from proven work.

**ticket-captain** (TooBigSteps; read by TicketCaptain):

> Commit what is whole, and post its SHA on your ticket.

## SplitRest

`claim:CaptainTrouble.SplitRest`: A holder whose slice proves too big divides it itself, keeping coherence: what is left becomes sub-tickets.

Spec: H2, K30

Transition in A captain and its ticket (`claims/statechart/captain/Captain.tla`): Leaf → Split, "the leaf proves too big: what is left is cut".

Evidence (code): `AGENTS.md`

Layer: AGENTS.md lets a holder split; that a leaf already begun may still be split is said only here.

Reason: The alternative is an agent finishing badly in a context that is nearly spent.

**ticket-captain** (TooBigSteps; read by TicketCaptain):

> Divide what is left yourself, into sub-tickets that each hold together and start from that SHA, and dispatch them.

## ReCut

`claim:CaptainTrouble.ReCut`: A holder that cannot divide its slice coherently under its own ticket returns it to its officer for a re-cut, naming what is done and what is left.

Spec: H2

Transition in A captain and its ticket (`claims/statechart/captain/Captain.tla`): Leaf → Returned, "`too big, re-cut`: what is left cannot be divided coherently under the ticket".

Evidence (run): This skill's writer was briefed (DYD-1372 hop 2): "return `too big, re-cut` naming what is done and what is left".

Layer: T3 returns whatever a child says last, and a child that simply stops says nothing its officer can act on.

Reason: The officer can cut again from a known point without reading the whole thread.

**ticket-captain** (TooBigSteps; read by TicketCaptain):

> If what is left cannot be divided coherently under your ticket, for example because part of it belongs with a sibling's work, return `too big, re-cut` to your officer, naming what is done and what is left. Your ticket then stays open as it stands. Your officer rewrites its contract down to what you finished and sends it to your thread: woken with it, close your ticket Done on that contract and return.

## Left

`claim:CaptainTrouble.Left`: A ticket returned too big, re-cut stays open as it stands: its contract is its officer's to rewrite, and its holder, woken with the rewritten contract, closes it Done and returns.

Spec: H2, S6

Evidence (tool): t3_thread_send: "A send to a finished child thread starts a fresh task and returns its `taskId`." delegate_task: a child's final result returns to the thread that started it.

Layer: Linear leaves the ticket In Progress after its holder's turn has ended, and says nothing of who moves it next.

Reason: The child cannot end a step whose promise it has not met, and narrowing that promise was never its own to do; once narrowed, the step is still the child's to end, since the child began it.

**ticket-captain** (TooBigSteps; read by TicketCaptain, on claim:CaptainTrouble.ReCut):

> If what is left cannot be divided coherently under your ticket, for example because part of it belongs with a sibling's work, return `too big, re-cut` to your officer, naming what is done and what is left. Your ticket then stays open as it stands. Your officer rewrites its contract down to what you finished and sends it to your thread: woken with it, close your ticket Done on that contract and return.

## Yours

`claim:CaptainTrouble.Yours`: The contract of a ticket a child returned too big, re-cut is its officer's to rewrite, before the officer cuts what is left; the ticket stays the child's to close.

Spec: H2, S15, S6

Evidence (run): DYD-1372's contract, read 2026-11-21: its sections G and H were written from above, by the coordinator, and no writer below edited them. Spec H2: "a change to a unit's contract, or to its siblings', goes through the officer above".

Layer: Linear lets any agent edit any ticket, and nothing says whose a returned ticket's contract is once its holder's turn has ended.

Reason: Whoever sized a part and wrote down its outcome is the one agent that knows what the narrowed part must still promise.

**ticket-captain** (TooBigChild; read by TicketCaptain):

> When a child returns `too big, re-cut`, the cut is yours, and so is the child's contract, in this order:

## Narrow

`claim:CaptainTrouble.Narrow`: The officer rewrites the returned ticket's contract down to what the child finished at the SHA it returned, and notes the re-cut on the ticket.

Spec: S15, H2

Evidence (tool): save_issue: `description`, the ticket's body, where spec S15 puts its contract; DYD-1372's description, read 2026-11-21, holds its own.

Layer: Linear keeps the old contract on the ticket until someone rewrites it.

Reason: A ticket closed against a contract it never met tells whoever reads it later that the whole part was built.

**ticket-captain** (TooBigOfficer; read by TicketCaptain):

> Rewrite the child's contract, in its ticket's description, down to what it finished at the SHA it returned, and note the re-cut on the ticket in one line. If it finished nothing whole, cancel the ticket instead, with a one-line reason, and skip the next step.

## Settle

`claim:CaptainTrouble.Settle`: The officer never closes the returned ticket: it cancels it, with a one-line reason, only when the child finished nothing whole.

Spec: S10, S6

Evidence (tool): save_issue: `state`, "State type, name, or ID", moves a ticket for whoever calls it. Linear, DYD-1290 read 2026-11-21, is Done, of the type completed; team Dydo's statuses hold Canceled beside it (claim CaptainWaking.Unneeded).

Layer: Linear lets any agent move any ticket to Done, the officer as readily as the child that started it.

Reason: A dead card left In Progress reads as work under way, and one its officer closed Done reads as a step its starter ended, which nobody did.

**ticket-captain** (TooBigOfficer; read by TicketCaptain, on claim:CaptainTrouble.Narrow):

> Rewrite the child's contract, in its ticket's description, down to what it finished at the SHA it returned, and note the re-cut on the ticket in one line. If it finished nothing whole, cancel the ticket instead, with a one-line reason, and skip the next step.

## Handed

`claim:CaptainTrouble.Handed`: The officer sends the rewritten contract to the child's thread under the ticket's key, and the child, which started the ticket, closes it Done on that contract and returns.

Spec: S6, H2

Evidence (tool): t3_thread_send: "A send to a finished child thread starts a fresh task and returns its `taskId`." Its `message` is all it carries, so the key in it is what tells the child which ticket the send is about.

Layer: A rewritten description wakes nobody: Linear tells no thread that its ticket changed, and T3 starts a finished thread only on a send.

Reason: Left unsent, the narrowed card sits In Progress with a holder that believes itself finished, and no agent remains that may end it.

**ticket-captain** (TooBigOfficer; read by TicketCaptain):

> Send the rewritten contract to the child's thread with `t3_thread_send`, under the ticket's key: the child started the ticket, so the child closes it Done on that contract and returns.

## ChildReCut

`claim:CaptainTrouble.ChildReCut`: An officer whose child returns too big, re-cut cuts what the child left.

Spec: H2, H14

Transition in A captain and its ticket (`claims/statechart/captain/Captain.tla`): Waiting → Split, "a child returns `too big, re-cut`: its contract is rewritten down to what it finished, and what it left is cut".

Evidence (tool): delegate_task: a child's final result returns to the thread that started it.

Layer: The return lands in the officer's thread, and nothing re-opens the work.

Reason: The officer knows how the part fits the whole, which the child did not.

**ticket-captain** (TooBigOfficer; read by TicketCaptain):

> Open sub-tickets for what the child left, starting from that SHA.

## Contracts

`claim:CaptainTrouble.Contracts`: A change to a unit's contract, or to a sibling's, goes through the officer above.

Spec: H2, S15

Evidence (run): DYD-1372's contract, read 2026-11-21: its section G, the phase-1 changes, was written from above, by the coordinator, and no writer below it edited the contract.

Layer: Linear lets any agent edit any ticket's description, its own or a sibling's.

Reason: A contract its holder may rewrite stops being a promise to the officer that cut it.

**ticket-captain** (TooBigEnd; read by TicketCaptain):

> A change to your contract, or to a sibling's, is never part of your cut: take it to your officer.

## FixOwner

`claim:CaptainTrouble.FixOwner`: A Fix goes to the thread of the last child that wrote on the change it repairs while that thread has room, and to a fresh writer otherwise.

Spec: S9

Evidence (run): This skill's own Fix came so: its captain sent fix r0 to the thread that wrote hop 2. t3_thread_send: "A send to a finished child thread starts a fresh task and returns its `taskId`."

Layer: T3 will resume any thread, and cannot tell a useful context from a spent one.

Reason: The writer already understands the code, until its context is what stands in the way.

**ticket-captain** (FixRules; read by TicketCaptain):

> Give a Fix to the thread of the last child that wrote on the change it repairs, its implementer or an earlier Fix's writer, with `t3_thread_send`, while that thread is alive and has room in its context. Dispatch a fresh writer for it, with `delegate_task`, when that context is heavy, when a blocker says the approach was wrong, or when no child has written on the change because you wrote it: you never fix a change of your own. Either way the Fix is a new ticket to its holder: brief it by Briefs (`briefs.md`), under the Fix's own key.

## FixBrief

`claim:CaptainTrouble.FixBrief`: A Fix is a new ticket to whoever takes it, its writer's thread included: its brief is written like any dispatch's and names the Fix's own key.

Spec: S9, S6, K14

Evidence (tool): t3_thread_send: "A send to a finished child thread starts a fresh task and returns its `taskId`." Its `message` is all it carries: nothing in a send marks it as a second ticket for the thread that receives it.

Layer: T3 carries a message into a thread as it is, and adds no key and no mark that an assignment is new.

Reason: The key is the one thing in the message that tells a writer its Implement is over and another step begins, to be started and closed as its own.

**ticket-captain** (FixRules; read by TicketCaptain, on claim:CaptainTrouble.FixOwner):

> Give a Fix to the thread of the last child that wrote on the change it repairs, its implementer or an earlier Fix's writer, with `t3_thread_send`, while that thread is alive and has room in its context. Dispatch a fresh writer for it, with `delegate_task`, when that context is heavy, when a blocker says the approach was wrong, or when no child has written on the change because you wrote it: you never fix a change of your own. Either way the Fix is a new ticket to its holder: brief it by Briefs (`briefs.md`), under the Fix's own key.

## FixName

`claim:CaptainTrouble.FixName`: A Fix ticket is opened at verdict time, titled after its round, Fix rN, and lists every blocker it is to close.

Spec: S9

Evidence (run): Linear, DYD-1281's sub-tickets read 2026-11-21: Fix r1, Fix r2 and Fix r3, each after the Review of its number, such as "Fix r2: close review r2's 2 blockers and 5 shoulds". Fix r1, DYD-1327, was created at 04:06 on 2026-11-20, a minute before Review r1, DYD-1294, closed with its verdict, and its description lists that round's blockers by number, B1 to B7.

Layer: Linear has no Type label for a Fix, so only its title says which round it closes.

Reason: Anyone reading the board finds the round a Fix belongs to without opening it.

**ticket-captain** (FixRules; read by TicketCaptain):

> Open one Fix when its round's verdict is posted, unless you opened it ahead, titled after the round, Fix rN, and list on it every blocker it is to close.

## Big

`claim:CaptainTrouble.Big`: A blocker gets a sub-ticket of its own inside the Fix only when it is too big to fix with the rest.

Spec: S9, S7

Evidence (run): Linear, DYD-1281's sub-tickets read 2026-11-21: Fix r1 (DYD-1327), Fix r2 (DYD-1333) and Fix r3 (DYD-1339) are one ticket each and hold no sub-ticket. Fix r1 closed 7 blockers so, and no blocker has needed a ticket of its own yet. Spec S9 as amended: one Fix ticket "listing all of them, with a sub-ticket only for a big one" (DYD-1372 07c86aad, "4. Lgtm").

Layer: Linear takes a ticket for every blocker as readily as for none.

Reason: A card per small repair buries the board, while a large one hidden in a list has no owner to answer for it.

**ticket-captain** (FixRules; read by TicketCaptain):

> A blocker too big to fix with the rest gets a sub-ticket of its own inside the Fix; no other blocker gets one.

## Parallel

`claim:CaptainTrouble.Parallel`: A Fix is split only between parallel writers, as Fix rNa and Fix rNb.

Spec: S9

Evidence (run): Linear, DYD-1281's sub-tickets read 2026-11-20: Fix r1, r2 and r3, each followed by a Review titled a fresh review; r4 is titled the checkpoint round. Each Fix is one ticket. This skill's own round 1 was split between two writers in parallel, Fix r1a on the generator and Fix r1b on the skill: T3 thread titles and `git log`, read 2026-11-21.

Layer: Nothing stops a captain from opening a Fix ticket for every finding.

Reason: One writer closing all of a round's blockers understands the code once.

**ticket-captain** (FixRules; read by TicketCaptain):

> Split a Fix only between writers who work in parallel, as Fix rNa and Fix rNb.

## Security

`claim:CaptainTrouble.Security`: After a security hole, the next round leads with the security lens.

Spec: R9

Evidence (run): Linear, DYD-1281's sub-tickets read 2026-11-20: Fix r1, r2 and r3, each followed by a Review titled a fresh review; r4 is titled the checkpoint round.

Layer: The next reviewer starts fresh and knows nothing of what the last round found.

Reason: A hole that was missed once is the first thing to look for again.

**ticket-captain** (FixRules; read by TicketCaptain, on claim:CaptainTrouble.NextRound):

> Dispatch the next round once the Fix is Done, each blocker it lists closed and its proof run on the head: a fresh reviewer, every lens, and after a security hole the security lens first.

## NextRound

`claim:CaptainTrouble.NextRound`: The next round starts once every blocker of the Fix is Done and its proof ran on the head: a fresh look through every lens.

Spec: R8, S9

Transition in A captain and its ticket (`claims/statechart/captain/Captain.tla`): Fixing → InReview, "the Fix is Done: a fresh round".

Evidence (run): Linear, DYD-1281's sub-tickets read 2026-11-20: Fix r1, r2 and r3, each followed by a Review titled a fresh review; r4 is titled the checkpoint round.

Layer: `dydo merge` asks for a PASS on the head and cannot tell a fresh look from a glance at the diff.

Reason: A fix can break what the last round passed, so the next one looks at everything again.

**ticket-captain** (FixRules; read by TicketCaptain):

> Dispatch the next round once the Fix is Done, each blocker it lists closed and its proof run on the head: a fresh reviewer, every lens, and after a security hole the security lens first.

## Dead

`claim:CaptainTrouble.Dead`: On a re-plan, dead sub-tickets go Canceled with a one-line reason.

Spec: S10

Evidence (run): Linear, team Dydo, statuses read 2026-11-20: Canceled exists.

Layer: Linear leaves an abandoned ticket open for as long as nobody touches it.

Reason: A dropped ticket left open looks like work someone still owes.

**ticket-captain** (FixRules; read by TicketCaptain):

> When your officer re-plans, cancel each sub-ticket the new plan drops, with a one-line reason.

## Third

`claim:CaptainTrouble.Third`: A third round that still has a blocker goes to the checkpoint before anything else.

Spec: R10

Transition in A captain and its ticket (`claims/statechart/captain/Captain.tla`): InReview → Checkpoint, "a third round still has a blocker: the checkpoint".

Evidence (run): Linear, DYD-1281's sub-tickets read 2026-11-20: Fix r1, r2 and r3, each followed by a Review titled a fresh review; r4 is titled the checkpoint round.

Layer: Nothing counts rounds, so nothing would notice a loop that never ends.

Reason: Three blocked rounds say something other than the code may be wrong, and a fourth should not start unasked.

**ticket-captain** (FixCheckpoint; read by TicketCaptain, on claim:CaptainTrouble.Checkpoint):

> After a third round that still has a blocker, write `checkpoint: one more round, because …` on the Review ticket; its blockers then go to a Fix, as in any round.

## Checkpoint

`claim:CaptainTrouble.Checkpoint`: At the checkpoint the default is one more round, noted on the Review ticket.

Spec: R10, S10

Transition in A captain and its ticket (`claims/statechart/captain/Captain.tla`): Checkpoint → Fixing, "`checkpoint: one more round, because …`: the blockers go to a Fix".

Evidence (run): Linear, DYD-1281's sub-tickets read 2026-11-20: Fix r1, r2 and r3, each followed by a Review titled a fresh review; r4 is titled the checkpoint round.

Layer: The Review ticket takes any comment, and nothing asks for this one.

Reason: A written reason makes the captain look at why three rounds were not enough.

**ticket-captain** (FixCheckpoint; read by TicketCaptain):

> After a third round that still has a blocker, write `checkpoint: one more round, because …` on the Review ticket; its blockers then go to a Fix, as in any round.

## Escalate

`claim:CaptainTrouble.Escalate`: At the checkpoint a captain escalates only for a fundamental break, a new requirement or a need for the user.

Spec: R10

Transition in A captain and its ticket (`claims/statechart/captain/Captain.tla`): Checkpoint → Returned, "`escalated: …`: a fundamental break, a new requirement or a need for the user goes to the officer".

Evidence (tool): delegate_task: a child's final result returns to the thread that started it.

Layer: An officer hears nothing of a loop below it until the captain speaks.

Reason: Most of the time the right answer is one more round, and an escalation stops work that could go on.

**ticket-captain** (FixCheckpoint; read by TicketCaptain):

> Write `escalated: …` instead, and take it to your officer, only for a fundamental break, a new requirement or a need for the user.
