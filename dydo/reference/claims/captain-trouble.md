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

`claim:CaptainTrouble.SplitRest`: A leaf that proves too big is split: what is left becomes sub-tickets.

Spec: H13, H14

Transition in A captain and its ticket (`claims/statechart/captain/Captain.tla`): Leaf → Split, "the leaf proves too big: what is left is cut".

Evidence (code): `AGENTS.md`

Layer: AGENTS.md lets a holder split; that a leaf already begun may still be split is said only here.

Reason: The alternative is an agent finishing badly in a context that is nearly spent.

**ticket-captain** (TooBigSteps; read by TicketCaptain):

> Cut what is left into sub-tickets that start from that SHA, and dispatch them.

## ReCut

`claim:CaptainTrouble.ReCut`: A captain that cannot hold even the split returns too big, re-cut, naming what is done and what is left.

Spec: H2

Transition in A captain and its ticket (`claims/statechart/captain/Captain.tla`): Leaf → Returned, "`too big, re-cut`: neither the work nor its split fits".

Evidence (run): This skill's writer was briefed (DYD-1372 hop 2): "return `too big, re-cut` naming what is done and what is left".

Layer: T3 returns whatever a child says last, and a child that simply stops says nothing its officer can act on.

Reason: The officer can cut again from a known point without reading the whole thread.

**ticket-captain** (TooBigSteps; read by TicketCaptain):

> If you cannot hold the split either, return `too big, re-cut` to your officer, naming what is done and what is left.

## ChildReCut

`claim:CaptainTrouble.ChildReCut`: An officer whose child returns too big, re-cut cuts what the child left.

Spec: H2, H14

Evidence (tool): delegate_task: a child's final result returns to the thread that started it.

Layer: The return lands in the officer's thread, and nothing re-opens the work.

Reason: The officer knows how the part fits the whole, which the child did not.

**ticket-captain** (TooBigChild; read by TicketCaptain):

> When a child returns `too big, re-cut`, the cut is yours: open sub-tickets for what it left, starting from the SHA it returned.

## FixOwner

`claim:CaptainTrouble.FixOwner`: A Fix goes to its writer's thread while that thread has room, and to a fresh agent otherwise.

Spec: S9

Evidence (run): This skill's own Fix came so: its captain sent fix r0 to the thread that wrote hop 2. t3_thread_send: "A send to a finished child thread starts a fresh task and returns its `taskId`."

Layer: T3 will resume any thread, and cannot tell a useful context from a spent one.

Reason: The writer already understands the code, until its context is what stands in the way.

**ticket-captain** (FixRules; read by TicketCaptain):

> Give a Fix to its writer's thread, with `t3_thread_send`, while that thread is alive and has room in its context; dispatch a fresh agent for it when the context is heavy or a blocker says the approach was wrong.

## FixName

`claim:CaptainTrouble.FixName`: A Fix ticket is opened at verdict time and titled after its round: Fix rN.

Spec: S9

Evidence (run): Linear, DYD-1281's sub-tickets read 2026-11-21: Fix r1, Fix r2 and Fix r3, each after the Review of its number, such as "Fix r2: close review r2's 2 blockers and 5 shoulds". Fix r1, DYD-1327, was created at 04:06 on 2026-11-20, a minute before Review r1, DYD-1294, closed with its verdict.

Layer: Linear has no Type label for a Fix, so only its title says which round it closes.

Reason: Anyone reading the board finds the round a Fix belongs to without opening it.

**ticket-captain** (FixRules; read by TicketCaptain):

> Open a Fix when its round's verdict is posted, titled after the round: Fix rN.

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

> Dispatch the next round once every blocker of the Fix is Done and its proof ran on the head: a fresh reviewer, every lens, and after a security hole the security lens first.

## NextRound

`claim:CaptainTrouble.NextRound`: The next round starts once every blocker of the Fix is Done and its proof ran on the head: a fresh look through every lens.

Spec: R8, S9

Transition in A captain and its ticket (`claims/statechart/captain/Captain.tla`): Fixing → InReview, "the Fix is Done: a fresh round".

Evidence (run): Linear, DYD-1281's sub-tickets read 2026-11-20: Fix r1, r2 and r3, each followed by a Review titled a fresh review; r4 is titled the checkpoint round.

Layer: `dydo merge` asks for a PASS on the head and cannot tell a fresh look from a glance at the diff.

Reason: A fix can break what the last round passed, so the next one looks at everything again.

**ticket-captain** (FixRules; read by TicketCaptain):

> Dispatch the next round once every blocker of the Fix is Done and its proof ran on the head: a fresh reviewer, every lens, and after a security hole the security lens first.

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

## Sources

`claim:CaptainTrouble.Sources`: What research can answer never reaches the user.

Spec: H16

Evidence (run): Linear, team Dydo, labels read 2026-11-20: the Type group holds Question.

Layer: The host's question tool asks the user anything an agent cares to send.

Reason: The user's attention is the one thing the swarm cannot make more of.

**ticket-captain** (NeedLead; read by TicketCaptain):

> Most calls never reach the user: whatever the sources can answer is a Question sub-ticket.

## Ask

`claim:CaptainTrouble.Ask`: A call that is not obvious and reversible becomes a Question sub-ticket that blocks only what waits on it.

Spec: H16, S10

Transition in A captain and its ticket (`claims/statechart/captain/Captain.tla`): Leaf → Question, "a call that is not obvious: a Question sub-ticket".

Evidence (run): Linear, DYD-1372 read 2026-11-20: a ticket carries `blocks` and `blockedBy` relations beside its children.

Layer: Linear draws a block once it is set, and stops nobody from working on.

Reason: One open question holds up one part, never the whole ticket.

**ticket-captain** (NeedRules; read by TicketCaptain):

> Open the Question sub-ticket, block only what waits on it, and go on with the rest.

## Answered

`claim:CaptainTrouble.Answered`: The blocked work resumes when the answer is on the Question ticket.

Spec: H16

Transition in A captain and its ticket (`claims/statechart/captain/Captain.tla`): Question → Leaf, "the answer is on the Question ticket".

Evidence (run): Linear, team Dydo, labels read 2026-11-20: Question is described "Resolution: the answer, posted on close".

Layer: T3 wakes the captain when the Question's captain returns; the answer itself is on the ticket.

Reason: The answer outlives both threads, for whoever meets the same call next.

**ticket-captain** (NeedRules; read by TicketCaptain):

> Resume what waited once the answer is on the Question ticket.

## Up

`claim:CaptainTrouble.Up`: A Question that comes back unsettled goes to the officer's thread with what was found and stays open: its holder still waits on the answer.

Spec: H16, H12, S10

Transition in A captain and its ticket (`claims/statechart/captain/Captain.tla`): Question → Question, "the Question comes back unsettled: it goes to the officer and stays open, and the work still waits on its answer".

Evidence (run): T3, read 2026-11-21 from the thread of this skill's fixer: `t3_thread_list` gives its `parentThreadId`, the thread of the captain that dispatched it, and the header of its brief names the same thread. t3_thread_send: "Send a message to any thread"; its `mode:'auto'` "starts an idle thread, steers a running turn, or queues". So a holder reaches its officer's thread without ending its own run.

Layer: A captain has no tool that opens a thread with the user, and its officer is the only one above it.

Reason: Whether a call is worth the user's time is weighed once, above, instead of by every captain that meets one.

**ticket-captain** (NeedRules; read by TicketCaptain):

> When a Question comes back unsettled, send it and what was found to your officer's thread with `t3_thread_send`, and leave it open: what waited still waits on its answer.

## Known

`claim:CaptainTrouble.Known`: A need already known to be one only the user can meet gets no research: its Question sub-ticket goes to the officer at once and blocks only what waits on it.

Spec: R9, S10, L18

Evidence (run): T3, read 2026-11-21 from the thread of this skill's fixer: `t3_thread_list` gives its `parentThreadId`, the thread of the captain that dispatched it, and the header of its brief names the same thread. t3_thread_send: "Send a message to any thread"; its `mode:'auto'` "starts an idle thread, steers a running turn, or queues". So a holder reaches its officer's thread without ending its own run.

Layer: Nothing above a captain learns of a need until the captain sends it, and no tool of its own reaches the user's queue.

Reason: A captain set to study what no source holds costs a day, and the user's answer is the slowest of all to arrive.

**ticket-captain** (NeedRules; read by TicketCaptain):

> When you already know that only the user can meet a need, skip the research: open its Question sub-ticket, block only what waits on it, and send it to your officer's thread at once with `t3_thread_send`. A call that only wants the user's approval is no such need.

## InThread

`claim:CaptainTrouble.InThread`: When the user is already in an officer's thread, it may ask a one-line question there.

Spec: H22, L18

Evidence (tool): AskUserQuestion: "Use this tool only when you are blocked on a decision that is genuinely the user's to make".

Layer: The tool asks whenever it is called; whether the user is there to answer, only the thread shows.

Reason: A ticket and a wait for something the user can answer in a word would be overkill.

**ticket-captain** (NeedRules; read by TicketCaptain):

> When the user is already in your thread, you may ask one line there, with the host's question tool: never what the sources can answer, and never a call that only wants the user's approval.

## Cite

`claim:CaptainTrouble.Cite`: A question put to the user names what each decision record says, never only its number.

Spec: H24

Evidence (code): `dydo/project/decisions`

Layer: A record's number resolves to a file for an agent, and to nothing for a person reading a question.

Reason: The user answers the question in front of him without looking anything up.

**ticket-captain** (NeedRules; read by TicketCaptain):

> Say what a decision record decides whenever you cite one to the user, never only its number.
