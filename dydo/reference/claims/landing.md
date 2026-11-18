---
area: reference
type: reference
---
<!-- built from the model; edit there: tools/SkillBuild -->

# Claims: Landing

## Staff

`claim:Landing.Staff`: The admiral staffs each inquisition round from the roster and dydo usage.

Spec: N5, R13, R14

Transition in The landing (`claims/statechart/landing/Landing.tla`): Integrated → Inquisition, "the admiral staffs a round".

**admiral** (Integrated, exit):

> Staff each inquisition round from the model roster and `dydo usage`, and tell him in your next envoy bundle.

## RoundFound

`claim:Landing.RoundFound`: The admiral holds the round it staffs: one verdict, blockers into Fix rN, shoulds registered.

Spec: N5, S8, S9, S17

Transition in The landing (`claims/statechart/landing/Landing.tla`): Inquisition → RoundFixes, "the round finds a blocker or a should".

**admiral** (Inquisition, exit):

> Hold each round you staff: post one verdict naming each seat and its findings, turn its blockers into Fix rN and register its shoulds.

## NextRound

`claim:Landing.NextRound`: After the fixes, the next round starts from zero.

Spec: N5

Transition in The landing (`claims/statechart/landing/Landing.tla`): RoundFixes → Inquisition, "the fixes are merged; the next round starts from zero". A system transition: no skill carries it.

## NextMergeRound

`claim:Landing.NextMergeRound`: A landing merge review with a blocker goes Fix, then a full-lens merge-review round.

Spec: R8, R9

Transition in The landing (`claims/statechart/landing/Landing.tla`): RoundFixes → MergeReview, "the fixes are merged; a full-lens merge-review round". A system transition: no skill carries it.

## CleanRound

`claim:Landing.CleanRound`: The Project lands only after a round with no blocker and no should.

Spec: N5

Transition in The landing (`claims/statechart/landing/Landing.tla`): Inquisition → Prerequisites, "a round finds no blocker and no should".

**admiral** (Inquisition, exit):

> The Project lands only after an inquisition round that finds no blocker and no should.

## PrerequisiteMet

`claim:Landing.PrerequisiteMet`: Each landing prerequisite is met in turn.

Spec: N7, S18, T12, N15, T8, N14, Y2, Y6, L6, L12, N8, N9

Transition in The landing (`claims/statechart/landing/Landing.tla`): Prerequisites → Prerequisites, "a prerequisite met: shoulds closed, mutation clean, purge done, evidence rewritten on his click, DYTEST wired in, principle descriptions written, backfill page passed, law shipped, Upgrade note and re-measure, statuses Enablement". A system transition: no skill carries it.

## Ready

`claim:Landing.Ready`: The landing merge review waits for every landing prerequisite.

Spec: N7, S18, T12, N15

Transition in The landing (`claims/statechart/landing/Landing.tla`): Prerequisites → MergeReview, "every landing prerequisite met".

**admiral** (Prerequisites, exit):

> Open the landing merge review only once every should is Done, Canceled with a reason or moved by his call, and the mutation check on the Project's diff is clean.

## Confident

`claim:Landing.Confident`: On the Merge-confidence PASS, the landing candidate goes to his walkthrough.

Spec: N6, L16, R20, H22

Transition in The landing (`claims/statechart/landing/Landing.tla`): MergeReview → Walkthrough, "the Merge-confidence council passes the landing candidate".

**admiral** (MergeReview, exit):

> On the council's PASS, send the walkthrough of the landing candidate, the feature branch with master merged in, to a fresh envoy.

## ReviewFail

`claim:Landing.ReviewFail`: A blocker from the landing merge review goes to fixes.

Spec: R9, R11

Transition in The landing (`claims/statechart/landing/Landing.tla`): MergeReview → RoundFixes, "the landing merge review finds a blocker".

**admiral** (MergeReview, exit):

> A blocker from the landing merge review goes to fixes like any round's.

## Accepted

`claim:Landing.Accepted`: He accepts the landing candidate, then it merges.

Spec: N6

Transition in The landing (`claims/statechart/landing/Landing.tla`): Walkthrough → Merging, "Balazs accepts the landing candidate".

**admiral** (Walkthrough, exit):

> When he accepts the landing candidate, it merges: AFK when two-way, on his click when one-way.

## Merged

`claim:Landing.Merged`: The landing PR merges into master.

Spec: N6, R26

Transition in The landing (`claims/statechart/landing/Landing.tla`): Merging → Landed, "the landing PR is in master". A system transition: no skill carries it.

## Hardened

`claim:Landing.Hardened`: The tag waits for the Guard hardening Project to land, every family.

Spec: N2

Transition in The landing (`claims/statechart/landing/Landing.tla`): Landed → Landed, "the Guard hardening Project lands". A system transition: no skill carries it.

## Tag

`claim:Landing.Tag`: He pushes the release tag once guard hardening has landed and mutation is clean.

Spec: R28, N2, N15

Transition in The landing (`claims/statechart/landing/Landing.tla`): Landed → Tagged, "Balazs pushes the release tag". A system transition: no skill carries it.

## SpecGone

`claim:Landing.SpecGone`: At acceptance, the spec's lasting parts move and the spec and plan files are deleted.

Spec: S14, D11, N7

**admiral** (Merging, nominal):

> Once he accepts, move the spec's lasting parts to DRs, the glossary, docs and the verification map, then delete the spec and the plan files.

## TagIsHis

`claim:Landing.TagIsHis`: Release tags are his.

Spec: R28, N2

**admiral** (Landed, nominal):

> The release tag is his to push.
