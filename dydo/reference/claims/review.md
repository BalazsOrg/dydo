---
area: reference
type: reference
---
<!-- built from the model; edit there: tools/SkillBuild -->

# Claims: Review

## Open

`claim:Review.Open`: The parent is reviewed once, after wiring and before its Merge hop, against the governing spec.

Spec: R2, R3, R6

Transition in [The review loop](../../../claims/statechart/review/Review.tla): Wired → Round, "the parts are wired; Review r1 opens".

**ticket-captain** (Wired, exit):

> Once the parts are wired and before the Merge hop, review the parent once: each part, the parts together, and their harmony.

**reviewer** (Round, arrival):

> Judge the change, and the behaviour it inherits unchanged, against the governing spec's problem statement and goals, not only the contract.

## Skip

`claim:Review.Skip`: A parent that only groups, or a small leaf, may get no review.

Spec: R6

Transition in [The review loop](../../../claims/statechart/review/Review.tla): Wired → Ended, "the parent only groups its parts".

**ticket-captain** (Wired, exit):

> Skip the parent's review when it only groups parts.

## Post

`claim:Review.Post`: The round's holder posts the verdict on the Review hop and the PR line together.

Spec: R11, R17, S8

Transition in [The review loop](../../../claims/statechart/review/Review.tla): Round → Verdict, "the verdict and the PR line are posted".

**reviewer** (Round, exit):

> Holding the round alone, post its verdict on your Review hop (PASS or FAIL, lenses, seats, shoulds) and, at the same moment, the one-line PASS naming the Review ticket in the PR body's `## Independent review`.

**ticket-captain** (Round, exit):

> Holding a round with several seats, post one verdict naming each seat's model and provider and its blockers, and write the PR line at the same moment.

**linear-workspace-standard** (LinearEvidence):

> The verdict lives on the Review sub-ticket: PASS or FAIL, lenses, seats and shoulds. The PR body's `## Independent review` carries only a one-line PASS naming its Review ticket, written with the verdict; the parent gets no copy, and `dydo map` and `dydo survey board` show the latest verdict on its card.

## Broken

`claim:Review.Broken`: A broken or against-spec blocker goes to Fix, then the next round.

Spec: R9, S9

Transition in [The review loop](../../../claims/statechart/review/Review.tla): Verdict → Fixing, "a blocker is broken or against the spec".

**ticket-captain** (Verdict, exit):

> A broken or against-spec blocker is fixed, then the next round runs.

## Security

`claim:Review.Security`: A security hole goes to Fix, then a round with the security lens first.

Spec: R9

Transition in [The review loop](../../../claims/statechart/review/Review.tla): Verdict → Fixing, "a blocker is a security hole".

**ticket-captain** (Verdict, exit):

> A security hole is fixed, and the next round runs with the security lens first.

## Fundamental

`claim:Review.Fundamental`: A fundamental blocker or a new requirement goes to the officer above to re-plan.

Spec: R9

Transition in [The review loop](../../../claims/statechart/review/Review.tla): Verdict → Replan, "a blocker is fundamental, or a new requirement".

**ticket-captain** (Verdict, exit):

> Send a fundamental blocker or a new requirement to the officer above to re-plan.

## NeedsBalazs

`claim:Review.NeedsBalazs`: A blocker that needs Balazs goes up the chain at once.

Spec: R9

Transition in [The review loop](../../../claims/statechart/review/Review.tla): Verdict → Escalated, "a blocker needs Balazs".

**ticket-captain** (Verdict, exit):

> Pass a blocker that needs Balazs up the chain at once.

## Clean

`claim:Review.Clean`: By default the first round with no new blocker ends the phase.

Spec: R8

Transition in [The review loop](../../../claims/statechart/review/Review.tla): Verdict → Ended, "no new blocker this round".

**ticket-captain** (Verdict, exit):

> By default, the first round with no new blocker ends the phase.

## OneMore

`claim:Review.OneMore`: The officer running the rounds may order one more round.

Spec: R8

Transition in [The review loop](../../../claims/statechart/review/Review.tla): Verdict → Round, "no new blocker, but the officer orders one more round".

**ticket-captain** (Verdict, exit):

> You may still order one more round after a round with no new blocker.

## FixDone

`claim:Review.FixDone`: Fix rN is Done when its blockers are Done and proof ran on the head; a full-lens round follows.

Spec: S9, R8

Transition in [The review loop](../../../claims/statechart/review/Review.tla): Fixing → Round, "Fix rN is Done: its blockers Done, proof run on the head".

**ticket-captain** (Fixing, exit):

> Start the next round, a fresh full-lens look, once Fix rN's blockers are Done and its proof ran on the head.

## ToCheckpoint

`claim:Review.ToCheckpoint`: After round 3 with a blocker comes a checkpoint, not a stop.

Spec: R10

Transition in [The review loop](../../../claims/statechart/review/Review.tla): Verdict → Checkpoint, "round 3 ends with a blocker".

**ticket-captain** (Verdict, exit):

> After round 3 with a blocker, stop at the checkpoint and decide on purpose.

## CheckpointGoOn

`claim:Review.CheckpointGoOn`: At the checkpoint the default is one more round, written on the Review.

Spec: R10, S10

Transition in [The review loop](../../../claims/statechart/review/Review.tla): Checkpoint → Fixing, "checkpoint: one more round".

**ticket-captain** (Checkpoint, exit):

> By default, write `checkpoint: one more round, because …` on the Review and go on.

**linear-workspace-standard** (LinearEvidence):

> The round-3 checkpoint is one line on the Review: `checkpoint: one more round, because …` or `escalated: …`.

## CheckpointEscalate

`claim:Review.CheckpointEscalate`: At the checkpoint, escalate only for a fundamental break, a new requirement or a need for him.

Spec: R10, S10

Transition in [The review loop](../../../claims/statechart/review/Review.tla): Checkpoint → Escalated, "a fundamental break, a new requirement, or a need for Balazs".

**linear-workspace-standard** (LinearEvidence, on claim:Review.CheckpointGoOn):

> The round-3 checkpoint is one line on the Review: `checkpoint: one more round, because …` or `escalated: …`.

**ticket-captain** (Checkpoint, exit):

> Escalate only for a fundamental break, a new requirement or a need for Balazs, writing `escalated: …` on the Review.

## FixTicket

`claim:Review.FixTicket`: At verdict time blockers become Fix rN with one Blocker ticket each.

Spec: S9

**ticket-captain** (Verdict, nominal):

> At verdict time, turn the blockers into Fix rN with one Blocker ticket per blocker inside it; for parallel writers, split it into Fix rNb.

**linear-workspace-standard** (LinearTickets):

> At verdict time, blockers become a Fix rN ticket with one Blocker ticket per blocker inside it; Fix rN is Done when its blockers are Done and proof ran on the head; for parallel writers it splits into Fix rNb.

## Nit

`claim:Review.Nit`: A nit is one line on its record.

Spec: S11

**ticket-captain** (Verdict, nominal):

> Record each nit as one line on its record.

**reviewer** (Round, nominal):

> Holding the round alone, record each nit as one line on its record.

**linear-workspace-standard** (LinearEvidence):

> A nit is one line on its record; an inquisition round's nits go on that round's nit ticket.

## CaptainMade

`claim:Review.CaptainMade`: A blocker later found in a captain-made change goes to a crew writer.

Spec: R16

**ticket-captain** (Verdict, nominal):

> A blocker a later review finds in a change you made goes to a crew writer.

## Labels

`claim:Review.Labels`: The captain records every severity a fix-hop author changed, with its reason.

Spec: N12 DR 058 §3

**ticket-captain** (Verdict, nominal):

> Record every severity a fix-hop author changed, with its reason.

## FixOwner

`claim:Review.FixOwner`: Who gets the Fix is the officer's call; the next round is a fresh look either way.

Spec: S9

**ticket-captain** (Fixing, nominal):

> Give the Fix to the implementer's thread while it is alive and has room in its context, and to a fresh agent when its context is heavy or the blocker says the approach was wrong; the next round is a fresh look either way.

## Severities

`claim:Review.Severities`: Severities are blocker, should and nit; FAIL means at least one blocker.

Spec: R11, N12 DR 058 §1

**reviewer** (Round, nominal):

> Give every finding one severity: blocker, the intended case is wrong; should, a real bug callers will hit inside the intent; nit, everything else. FAIL means at least one blocker.

**linear-workspace-standard** (LinearLabels):

> Severity is `blocker`, `should` or `nit`: blocker, the intended case is wrong; should, a real bug callers will hit inside the intent; nit, everything else. FAIL means at least one blocker.

## OnePass

`claim:Review.OnePass`: All visible blockers are reported in one pass.

Spec: R8

**reviewer** (Round, nominal):

> Report every visible blocker in one pass.

## NoRerun

`claim:Review.NoRerun`: A fresh reviewer judges through lenses and re-runs nothing the same way.

Spec: R5

**reviewer** (Round, nominal):

> Judge through your lenses and re-run nothing the author ran the same way, unless enough has changed since that its result is in doubt.

## Statechart

`claim:Review.Statechart`: A statechart is used only for a finished subsystem that has a state machine.

Spec: R7

**reviewer** (Round, nominal):

> Use a statechart only for a finished subsystem that has a state machine.
