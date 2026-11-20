---
area: reference
type: reference
---
<!-- built from the model; edit there: tools/SkillBuild -->

# Claims: CaptainSplit

## Leaf

`claim:CaptainSplit.Leaf`: A ticket that fits one agent is a leaf, done by its holder with native helpers.

Spec: S2, S7

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

## OffRecord

`claim:CaptainSplit.OffRecord`: A helper, a lookup, a command and one seat among several stay off the record.

Spec: S1

Evidence (run): Linear, DYD-1281's sub-tickets read 2026-11-20: twenty, titled Review r1 to r4, Fix r1 to r3, Merge … and by part, each a child of the ticket.

Layer: Linear takes any ticket anyone files; only the cutter knows who will use what the work leaves.

Reason: A record nobody else reads is bookkeeping that slows the work it describes.

**ticket-captain** (Split; read by TicketCaptain, on claim:CaptainSplit.Cut):

> Cut a bigger ticket into sub-tickets, one per part another captain will hold; a helper, a lookup, a command and one seat among several get none, and several steps for one agent are one ticket.

## OneTicket

`claim:CaptainSplit.OneTicket`: Several steps given to one agent are one ticket.

Spec: S7

Evidence (code): `AGENTS.md`

Layer: AGENTS.md says one agent holds one ticket; the cutter is who keeps a second from being opened.

Reason: An agent that understands something once uses it across every step it was given.

**ticket-captain** (Split; read by TicketCaptain, on claim:CaptainSplit.Cut):

> Cut a bigger ticket into sub-tickets, one per part another captain will hold; a helper, a lookup, a command and one seat among several get none, and several steps for one agent are one ticket.

## Cut

`claim:CaptainSplit.Cut`: A ticket bigger than one agent is cut into sub-tickets, one per part another captain holds.

Spec: H13, H14

Transition in A captain and its ticket (`claims/statechart/captain/Captain.tla`): Taken → Split, "the ticket is bigger than one agent: it is cut".

Evidence (run): Linear, DYD-1281's sub-tickets read 2026-11-20: twenty, titled Review r1 to r4, Fix r1 to r3, Merge … and by part, each a child of the ticket.

Layer: AGENTS.md says to cut along what one agent understands together; opening the tickets is the holder's act.

Reason: Each part then has one agent that answers for it.

**ticket-captain** (Split; read by TicketCaptain):

> Cut a bigger ticket into sub-tickets, one per part another captain will hold; a helper, a lookup, a command and one seat among several get none, and several steps for one agent are one ticket.

## Contract

`claim:CaptainSplit.Contract`: A sub-ticket's description is its contract: its outcome, how it fits the ticket above, and the spec sections it serves, quoted and linked.

Spec: S15

Evidence (run): DYD-1372's contract, read 2026-11-20, is written this way and was the whole of what its writer was given to build to.

Layer: A child starts from its ticket and its brief; what the ticket omits, the child never learns.

Reason: A quoted section still says what was meant after the spec has moved on.

**ticket-captain** (Split; read by TicketCaptain):

> Write each sub-ticket's contract in its description: its outcome, how it fits your ticket, and the spec sections it serves, quoted and linked.

## Mode

`claim:CaptainSplit.Mode`: A sub-ticket's Mode label is AFK, or HITL when the user must take part.

Spec: S6, S12

Evidence (run): Linear, team Dydo, labels read 2026-11-20: the Mode group holds AFK, "an agent can produce the work", and HITL, "active human participation is required".

Layer: Linear holds the label; only the cutter knows whether a part needs the user.

Reason: The label tells everyone who the part waits on.

**ticket-captain** (Split; read by TicketCaptain):

> Label its Mode: AFK, or HITL when the user must take part.

## Steps

`claim:CaptainSplit.Steps`: Work with its own review rounds is cut into Implement, Review, Fix and Merge sub-tickets blocked in order; a small leaf is one ticket.

Spec: S3, S4

Evidence (run): Linear, DYD-1281's sub-tickets read 2026-11-20: twenty, titled Review r1 to r4, Fix r1 to r3, Merge … and by part, each a child of the ticket. Linear has no Type label for Implement, Review or Fix, so the title carries the step.

Layer: Linear draws the blocking order once it is set; setting it is the cutter's.

Reason: The board then shows which step a piece of work has reached without anyone asking its captain.

**ticket-captain** (Split; read by TicketCaptain):

> Work with its own review rounds becomes Implement, Review (one per round), Fix and Merge sub-tickets, blocked in that order and opened ahead in Todo if you like; a small leaf stays one ticket. (open: his call)

## Small

`claim:CaptainSplit.Small`: A captain that has split writes only a change cheaper to do than to brief and too small to need review.

Spec: R16

Evidence (code): `AGENTS.md`

Layer: AGENTS.md says a splitting captain only orchestrates; this is the one exception, and no layer states it.

Reason: A captain that wrote the work can no longer be impartial about the findings on it.

**ticket-captain** (Split; read by TicketCaptain):

> Once you split, write a change yourself only when that is cheaper than briefing it and it is too small to need review.
