---
area: reference
type: reference
---
<!-- built from the model; edit there: tools/SkillBuild -->

# Claims: CaptainReview

## Risk

`claim:CaptainReview.Risk`: Persistence, migrations, permissions, guard rules and uncertain native interfaces are always reviewed.

Spec: R16

Evidence (code): `Services/Merge/PullRequestMerge.cs`

Layer: `dydo merge` asks every PR for a PASS and cannot tell these five apart from any other change inside one.

Reason: A mistake here is costly or impossible to undo once it has run.

**ticket-captain** (Reviewed; read by TicketCaptain, on claim:CaptainReview.Other):

> Every change is reviewed by an agent other than its writer, unless too small to need it; persistence, migrations, permissions, guard rules and uncertain native interfaces always are.

## Other

`claim:CaptainReview.Other`: Every change is reviewed by an agent other than its writer, unless it is too small to need it.

Spec: R33, R12

Transition in A captain and its ticket (`claims/statechart/captain/Captain.tla`): Leaf → InReview, "the leaf's change is whole: another agent reviews it".

Evidence (run): Linear, DYD-1281's sub-tickets read 2026-11-20: Review r1 to r4, each after the first titled a fresh review that follows Fix r1, r2 or r3.

Layer: `dydo merge` refuses a PR with no PASS line and cannot tell who wrote the line.

Reason: A writer's own context hides from it what a fresh one sees at once.

**ticket-captain** (Reviewed; read by TicketCaptain):

> Every change is reviewed by an agent other than its writer, unless too small to need it; persistence, migrations, permissions, guard rules and uncertain native interfaces always are.

## Whose

`claim:CaptainReview.Whose`: Whoever merges a piece of work gets it reviewed first; a part is reviewed with the parent it joins.

Spec: R2

Evidence (code): `Services/Merge/PullRequestMerge.cs`

Layer: `dydo merge` refuses a head with no PASS, so the merger finds out at the merge; nothing tells it before.

Reason: A part reviewed alone and again inside its parent is reviewed twice for one answer.

**ticket-captain** (Reviewed; read by TicketCaptain):

> What you merge, you get reviewed first; a part your officer merges is reviewed with its whole.

## NoLoop

`claim:CaptainReview.NoLoop`: A plan, spec or contract gets no review loop; a spec gets at most one Questions pass by a native helper, chosen by risk.

Spec: R4

Evidence (tool): Agent: a native sub-agent, awaited within the turn, returns its report to its caller alone.

Layer: Nothing limits how often a document is sent for review.

Reason: First make it work, then make it good: rounds on a plan polish what building will change.

**ticket-captain** (Reviewed; read by TicketCaptain):

> A plan, spec or contract gets no review rounds; a risky spec gets at most one pass by a native helper, which returns questions.

## Severity

`claim:CaptainReview.Severity`: A blocker is fixed now; a should needs fixing, but the fix can wait; a nit is everything else.

Spec: R11

Evidence (run): Linear, team Dydo, labels read 2026-11-20: the Severity group holds blocker, should and nit.

Layer: Linear holds the three labels and says nothing of what a captain does with each.

Reason: The three words decide what happens next, so they are defined before the steps that use them.

**ticket-captain** (Reviewed; read by TicketCaptain):

> A reviewer gives each finding a severity. A blocker is fixed now; a should needs fixing, but the fix can wait; a nit is everything else.

## Groups

`claim:CaptainReview.Groups`: A ticket that only groups parts gets no review of its own.

Spec: R6

Transition in A captain and its ticket (`claims/statechart/captain/Captain.tla`): Waiting → Finishing, "the ticket only groups parts: no review".

Evidence (run): Linear, DYD-1281's sub-tickets read 2026-11-20: Review r1 to r4, each after the first titled a fresh review that follows Fix r1, r2 or r3.

Layer: Linear nests tickets for any reason; whether a parent integrates anything is known to its captain.

Reason: A grouping adds no behaviour for a reviewer to find wrong.

**ticket-captain** (Rounds; read by TicketCaptain, on claim:CaptainReview.Whole):

> Once the parts are wired, dispatch a Review ticket for the whole: each part, the parts together, their harmony. A ticket that only groups parts needs none.

## Whole

`claim:CaptainReview.Whole`: Once its parts are wired, a parent is reviewed once: each part, the parts together, their harmony.

Spec: R2, R1

Transition in A captain and its ticket (`claims/statechart/captain/Captain.tla`): Waiting → InReview, "the parts are wired: a Review of the whole is dispatched".

Evidence (run): Linear, DYD-1281's sub-tickets read 2026-11-20: Review r1 to r4, each after the first titled a fresh review that follows Fix r1, r2 or r3.

Layer: Nothing dispatches a review when the last part returns.

Reason: Early enough that nothing is built on a fault, late enough that the whole can be seen working.

**ticket-captain** (Rounds; read by TicketCaptain):

> Once the parts are wired, dispatch a Review ticket for the whole: each part, the parts together, their harmony. A ticket that only groups parts needs none.

## Record

`claim:CaptainReview.Record`: A lone reviewer records its own round; a round of several seats is recorded by the officer that holds it.

Spec: S8

Evidence (run): Linear, DYD-1281's sub-tickets read 2026-11-20: Review r1 to r4, each after the first titled a fresh review that follows Fix r1, r2 or r3.

Layer: Each seat returns its findings to its dispatcher alone, so nobody else can put them together.

Reason: One verdict per round, however many seats looked.

**ticket-captain** (Rounds; read by TicketCaptain):

> A lone reviewer records its own round; a round of several seats is yours to record, by A round's record (`resources/round.md`).

## Fix

`claim:CaptainReview.Fix`: A blocker that is broken, against the spec or a security hole goes to a Fix ticket holding one sub-ticket per blocker.

Spec: R9, S9

Transition in A captain and its ticket (`claims/statechart/captain/Captain.tla`): InReview → Fixing, "a round returns a blocker: a Fix ticket".

Evidence (run): Linear, DYD-1281's sub-tickets read 2026-11-20: Review r1 to r4, each after the first titled a fresh review that follows Fix r1, r2 or r3. Fix r3, DYD-1339, holds no sub-ticket, so one per blocker is the spec's rule and not yet the practice.

Layer: A FAIL stops `dydo merge`; it opens no ticket for the fix.

Reason: Each blocker can be closed, and seen closed, on its own.

**ticket-captain** (Rounds; read by TicketCaptain):

> Send a blocker that is broken, against the spec or a security hole to a Fix ticket, one sub-ticket per blocker inside it. (open: his call)

## Replan

`claim:CaptainReview.Replan`: A fundamental blocker or a new requirement goes to the officer above for a re-plan.

Spec: R9

Transition in A captain and its ticket (`claims/statechart/captain/Captain.tla`): InReview → Returned, "a blocker is fundamental or a new requirement: back to the officer".

Evidence (tool): delegate_task: a child's final result returns to the thread that started it.

Layer: A Fix can only repair what the plan asked for; nothing below the officer can change the plan.

Reason: Fixing toward a plan that is itself wrong builds more of the wrong thing.

**ticket-captain** (Rounds; read by TicketCaptain):

> Send a fundamental blocker or a new requirement to your officer, for a re-plan.

## OneMore

`claim:CaptainReview.OneMore`: The officer running the rounds may order one more after a round with no new blocker.

Spec: R8

Evidence (run): Linear, DYD-1281's sub-tickets read 2026-11-20: Review r1 to r4, each after the first titled a fresh review that follows Fix r1, r2 or r3.

Layer: Nothing counts rounds or closes the loop.

Reason: A round that barely passed can be looked at again without anyone inventing a blocker.

**ticket-captain** (Rounds; read by TicketCaptain, on claim:CaptainReview.Clean):

> Repeat until a round finds no new blocker; you may order one more.

## Clean

`claim:CaptainReview.Clean`: The first round with no new blocker ends the review rounds.

Spec: R8

Transition in A captain and its ticket (`claims/statechart/captain/Captain.tla`): InReview → Finishing, "a round finds no new blocker".

Evidence (run): Linear, DYD-1281's sub-tickets read 2026-11-20: Review r1 to r4, each after the first titled a fresh review that follows Fix r1, r2 or r3.

Layer: `dydo merge` needs one PASS on the head and knows nothing of earlier rounds.

Reason: The loop ends on evidence, with no cap that would let a known blocker through.

**ticket-captain** (Rounds; read by TicketCaptain):

> Repeat until a round finds no new blocker; you may order one more.

## Mine

`claim:CaptainReview.Mine`: A blocker found in a change a captain wrote goes to a fresh writer.

Spec: R16, R33

Evidence (tool): delegate_task: "Start one child agent with only `task`. No parent history."

Layer: Nothing keeps an agent from repairing its own work and calling it fixed.

Reason: Whoever wrote the fault tends to defend the thinking that produced it.

**ticket-captain** (Mine; read by TicketCaptain):

> A blocker found in a change you wrote goes to a fresh writer.
