---
area: reference
type: reference
---
<!-- built from the model; edit there: tools/SkillBuild -->

# Claims: CaptainSplit

## Leaf

`claim:CaptainSplit.Leaf`: A ticket that fits one agent is a leaf, done by its holder with native helpers.

Spec: H2, S2

Transition in A captain and its ticket (`claims/statechart/captain/Captain.tla`): Taken → Leaf, "the ticket fits one agent".

Evidence (code): `AGENTS.md`

Layer: AGENTS.md sizes a leaf; what its holder then does is said nowhere below this skill.

Reason: Below one agent's size, a split costs more in hand-offs than it saves.

**ticket-captain** (Leaf; read by TicketCaptain):

> A ticket that fits you is a leaf: do it as your kind's resource says, with native helpers for lookups and side work.

## Helpers

`claim:CaptainSplit.Helpers`: A native helper's work is committed by its holder.

Spec: H3, S1

Evidence (tool): Agent: a native sub-agent runs inside its caller's session and reports only to the caller.

Layer: The host starts the helper; whether its work is recorded and committed is left to the caller.

Reason: A helper's edits are part of the holder's change, so they land under the holder's commit and its review.

**ticket-captain** (Leaf; read by TicketCaptain):

> You commit what a helper changes.

## Uses

`claim:CaptainSplit.Uses`: Work gets a ticket only when someone other than its asker uses what it leaves.

Spec: S1

Evidence (run): Linear, DYD-1281's sub-tickets read 2026-11-20: twenty, titled Review r1 to r4, Fix r1 to r3, Merge … and by part, each a child of the ticket.

Layer: Linear takes any ticket anyone files; only the cutter knows who will use what the work leaves.

Reason: A record nobody else reads is bookkeeping that slows the work it describes.

**ticket-captain** (Tickets; read by TicketCaptain):

> Work gets a ticket only when someone other than its asker uses what it leaves.

## OffRecord

`claim:CaptainSplit.OffRecord`: A helper, a lookup, a command, a file and one native seat among several are always off the record.

Spec: S1

Evidence (run): Linear, DYD-1281's sub-tickets read 2026-11-20: twenty, titled Review r1 to r4, Fix r1 to r3, Merge … and by part, each a child of the ticket. None of the twenty is a helper's errand, a lookup, a command, a file or a single seat of a round. A seat from another model family goes out with `delegate_task` and holds a ticket of its own (S1 as amended, DYD-1372 aa5b445e), so only a native seat stays off the record.

Layer: Linear takes a ticket for a lookup or a file as readily as for a feature, and nothing stops a captain from filing one.

Reason: Each of the five is a piece of some ticket's work, and a card for every piece would bury the board.

**ticket-captain** (Tickets; read by TicketCaptain):

> A helper, a lookup, a command, a file and one native seat among several never get a ticket.

## OneTicket

`claim:CaptainSplit.OneTicket`: Several steps given to one agent are one ticket.

Spec: S7

Evidence (code): `AGENTS.md`

Layer: AGENTS.md says one agent holds one ticket; the cutter is who keeps a second from being opened.

Reason: An agent that understands something once uses it across every step it was given.

**ticket-captain** (Tickets; read by TicketCaptain):

> Several steps for one agent are one ticket.

## Cut

`claim:CaptainSplit.Cut`: A ticket bigger than one agent is cut into sub-tickets, one per part another captain holds.

Spec: H2, H13

Transition in A captain and its ticket (`claims/statechart/captain/Captain.tla`): Taken → Split, "the ticket is bigger than one agent and its route is in sight: it is cut".

Evidence (run): Linear, DYD-1281's sub-tickets read 2026-11-20: twenty, titled Review r1 to r4, Fix r1 to r3, Merge … and by part, each a child of the ticket.

Layer: AGENTS.md says to cut along what one agent understands together; opening the tickets is the holder's act.

Reason: Each part then has a single owner who answers for it.

**ticket-captain** (Split; read by TicketCaptain):

> Cut a bigger ticket into sub-tickets, one per part another captain will hold, by Sub-tickets (`resources/sub-tickets.md`).

## Contract

`claim:CaptainSplit.Contract`: A sub-ticket's description is its contract: its outcome, how it fits the ticket above, and the spec sections it serves, quoted and linked.

Spec: S15

Evidence (run): DYD-1372's contract, read 2026-11-20, is written this way and was the whole of what its writer was given to build to.

Layer: A child starts from its ticket and its brief; what the ticket omits, the child never learns.

Reason: A quoted section still says what was meant after the spec has moved on.

**ticket-captain** (Tickets; read by TicketCaptain):

> Write each sub-ticket's contract in its description: its outcome, how it fits your ticket, and the spec sections it serves, quoted and linked.

## Mode

`claim:CaptainSplit.Mode`: A sub-ticket's Mode label is AFK, or HITL when the user must take part.

Spec: S6, S12

Evidence (run): Linear, team Dydo, labels read 2026-11-20: the Mode group holds AFK, "an agent can produce the work", and HITL, "active human participation is required".

Layer: Linear holds the label; only the cutter knows whether a part needs the user.

Reason: The label tells everyone who the part waits on.

**ticket-captain** (Tickets; read by TicketCaptain):

> Label each sub-ticket's Mode: AFK, or HITL when the user must take part.

## Asks

`claim:CaptainSplit.Asks`: A Question sub-ticket's Mode says how it is settled: AFK, by a captain sent to research it; HITL, by the user, when its opener already knows the call is the user's alone.

Spec: H23, S3

Evidence (run): Linear, team Dydo, labels read 2026-11-21: the Mode group holds AFK, "an agent can produce the work without live human participation once unblocked", and HITL, "active human participation is required to produce or decide the work"; no label names a Question's stage. Spec H23 ties the two: "The stage sets its Mode, AFK for research and council and HITL for his-call".

Layer: Linear has no label for a Question's stage yet, so the Mode is the only mark on the ticket that whoever dispatches its holder can read.

Reason: Whoever staffs the Question reads its Mode before anything else, and a wrong one seats a researcher on what only he can say.

**ticket-captain** (Tickets; read by TicketCaptain):

> On a Question sub-ticket the Mode also says how it is settled: AFK, by a captain you send to research it; HITL, by the user, when you already know that the call is the user's alone.

## Steps

`claim:CaptainSplit.Steps`: Work with its own review rounds is cut into Implement, Review, Fix and Merge sub-tickets blocked in order, the Merge its cutter's own step; a small leaf and a batched should are each one ticket.

Spec: S3, S4

Evidence (run): Linear, DYD-1281's sub-tickets read 2026-11-20: twenty, titled Review r1 to r4, Fix r1 to r3, Merge … and by part, each a child of the ticket. Its four Merge sub-tickets, DYD-1290 to DYD-1293, read 2026-11-21, carry the labels Merge and AFK. Linear has no Type label for Implement, Review or Fix, so the title carries the step. No Shoulds batch has been cut in team Dydo's Linear yet, read 2026-11-21, so the batched should rests on spec S4, which the user kept (DYD-1372 07c86aad, "4. Lgtm").

Layer: Linear draws the blocking order once it is set; setting it is the cutter's.

Reason: The board then shows which step a piece of work has reached without anyone asking its captain.

**ticket-captain** (Tickets; read by TicketCaptain):

> Work with its own review rounds gets a sub-ticket per step, blocked in this order: Implement, Review (one per round), Fix, Merge, the last your own step. You may open them ahead, in Todo. A small leaf or a batched should is one ticket, with no steps.

## Small

`claim:CaptainSplit.Small`: A captain that has split writes only a change cheaper to do than to brief and too small to need review.

Spec: R16, K29

Evidence (code): `AGENTS.md`

Layer: AGENTS.md says a splitting captain only orchestrates; this is the one exception, and no layer states it.

Reason: A captain that wrote the work can no longer be impartial about the findings on it.

**ticket-captain** (Tickets; read by TicketCaptain):

> A change cheaper to make than to brief, and too small to need review, gets no ticket either: once you have split, it is the only change you write yourself.
