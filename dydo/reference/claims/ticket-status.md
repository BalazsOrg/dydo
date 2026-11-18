---
area: reference
type: reference
---
<!-- built from the model; edit there: tools/SkillBuild -->

# Claims: TicketStatus

## Open

`claim:TicketStatus.Open`: A hop may be opened ahead in Todo.

Spec: S3

Transition in [A ticket's status](../../../claims/statechart/ticket-status/TicketStatus.tla): New → Todo, "opened in Todo, ahead of its turn if a hop". A system transition: no skill carries it.

## Contract

`claim:TicketStatus.Contract`: The admiral contracts an uncontracted Backlog ticket to Todo.

Spec: N12 DR 063 §4

Transition in [A ticket's status](../../../claims/statechart/ticket-status/TicketStatus.tla): Backlog → Todo, "the admiral contracts an uncontracted ticket". A system transition: no skill carries it.

## Start

`claim:TicketStatus.Start`: Whoever starts a step sets it In Progress with the holder's thread link.

Spec: S6, S13, S3

Transition in [A ticket's status](../../../claims/statechart/ticket-status/TicketStatus.tla): Todo → InProgress, "whoever starts the step sets it In Progress". A system transition: no skill carries it.

## Close

`claim:TicketStatus.Close`: Whoever started a step closes it.

Spec: S6

Transition in [A ticket's status](../../../claims/statechart/ticket-status/TicketStatus.tla): InProgress → Done, "whoever started it closes it". A system transition: no skill carries it.

## Stopped

`claim:TicketStatus.Stopped`: A stopped hop goes back to Todo.

Spec: S24

Transition in [A ticket's status](../../../claims/statechart/ticket-status/TicketStatus.tla): InProgress → Todo, "liveness finds it stopped". A system transition: no skill carries it.

## CancelDead

`claim:TicketStatus.CancelDead`: A re-plan cancels a dead hop with a one-line reason.

Spec: S10

Transition in [A ticket's status](../../../claims/statechart/ticket-status/TicketStatus.tla): Todo → Canceled, "a re-plan cancels a dead hop". A system transition: no skill carries it.

## CancelRunning

`claim:TicketStatus.CancelRunning`: A re-plan or a revert cancels a running hop with its reason.

Spec: S10, S12

Transition in [A ticket's status](../../../claims/statechart/ticket-status/TicketStatus.tla): InProgress → Canceled, "a re-plan, or a reverted Merge". A system transition: no skill carries it.

## MarkDuplicate

`claim:TicketStatus.MarkDuplicate`: A duplicate should is marked Duplicate.

Spec: S17

Transition in [A ticket's status](../../../claims/statechart/ticket-status/TicketStatus.tla): Todo → Duplicate, "a duplicate should". A system transition: no skill carries it.
