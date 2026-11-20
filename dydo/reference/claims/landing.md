---
area: reference
type: reference
---
<!-- built from the model; edit there: tools/SkillBuild -->

# Claims: Landing

## Staff

`claim:Landing.Staff`: The coordinator opens the Inquisition ticket; its root captain staffs the round from the roster and the quotas and tells him as an FYI on the HITL queue (DYD-1195 6deea8c0).

Spec: N5, H5, R13, R14

Transition in The landing (`claims/statechart/landing/Landing.tla`): Integrated → Mapped, "the Inquisition ticket opens; its root captain staffs a round and draws a fresh feature map".

**coordinator** (Integrated, exit):

> Once every delivery ticket is merged, open the Inquisition ticket for a fresh root inquisition captain.

**ticket-captain** (Mapped, arrival):

> As the root, staff the round yourself: the provider, the models and the swarm width, from the model roster and `dydo usage`; post the staffing on the coordinator's HITL queue as an FYI for him.

## MapDrawn

`claim:Landing.MapDrawn`: Each slice's lenses start with Read, its own sub-agent.

Spec: N5, R34

Transition in The landing (`claims/statechart/landing/Landing.tla`): Mapped → ReadPhase, "a slice's Read lens starts". A system transition: no skill carries it.

## ReadDone

`claim:Landing.ReadDone`: Read is followed by Break, run by a separate sub-agent (DYD-1082 8239d5b6).

Spec: N5, R34

Transition in The landing (`claims/statechart/landing/Landing.tla`): ReadPhase → BreakPhase, "Read done; Break, a separate sub-agent". A system transition: no skill carries it.

## BreakDone

`claim:Landing.BreakDone`: Break is followed by Drive, run by a separate sub-agent (DYD-1082 8239d5b6).

Spec: N5, R34

Transition in The landing (`claims/statechart/landing/Landing.tla`): BreakPhase → DrivePhase, "Break done; Drive, a separate sub-agent". A system transition: no skill carries it.

## NextSlice

`claim:Landing.NextSlice`: Every slice runs its lenses.

Spec: N5

Transition in The landing (`claims/statechart/landing/Landing.tla`): DrivePhase → ReadPhase, "Drive done; the next slice's Read lens". A system transition: no skill carries it.

## AllSlices

`claim:Landing.AllSlices`: Once every slice has run its lenses, the round has its verdict.

Spec: N5, S8

Transition in The landing (`claims/statechart/landing/Landing.tla`): DrivePhase → Inquisition, "every slice through its lenses: the round's verdict". A system transition: no skill carries it.

## RoundFound

`claim:Landing.RoundFound`: The root inquisition captain holds the round: one verdict stitched from the groups, blockers into Fix rN, shoulds registered, nits on the round's nit ticket.

Spec: N5, S8, S9, S11, S17

Transition in The landing (`claims/statechart/landing/Landing.tla`): Inquisition → RoundFixes, "the round finds a blocker or a should".

**ticket-captain** (Inquisition, exit):

> Hold the round: post one verdict, stitched from the groups' results, naming each seat and its findings; turn its blockers into Fix rN, register its shoulds and put its nits on the round's nit ticket.

## NextRound

`claim:Landing.NextRound`: After the fixes, the next round starts from zero.

Spec: N5

Transition in The landing (`claims/statechart/landing/Landing.tla`): RoundFixes → Mapped, "the fixes are merged; the next round from zero, a fresh feature map". A system transition: no skill carries it.

## NextMergeRound

`claim:Landing.NextMergeRound`: A landing merge review with a blocker goes Fix, then a full-lens merge-review round, the security lens first after a security hole.

Spec: R8, R9

Transition in The landing (`claims/statechart/landing/Landing.tla`): RoundFixes → MergeReview, "the fixes are merged; a full-lens merge-review round, the security lens first after a security hole". A system transition: no skill carries it.

## CleanRound

`claim:Landing.CleanRound`: The Project lands only after a round with no blocker and no should.

Spec: N5

Transition in The landing (`claims/statechart/landing/Landing.tla`): Inquisition → Prerequisites, "a round finds no blocker and no should".

**coordinator** (Inquisition, exit):

> The Project lands only after an inquisition round that finds no blocker and no should.

## PrerequisiteMet

`claim:Landing.PrerequisiteMet`: Each landing prerequisite is met in turn.

Spec: N7, S18, T12, N15, T8, N14, Y2, Y6, L6, L12, N8, N9

Transition in The landing (`claims/statechart/landing/Landing.tla`): Prerequisites → Prerequisites, "a prerequisite met: shoulds closed, mutation clean, purge done, evidence rewritten on his click, DYTEST wired in, principle descriptions written, backfill page passed, law shipped, Upgrade note and re-measure, statuses Enablement". A system transition: no skill carries it.

## Ready

`claim:Landing.Ready`: The landing merge review waits for every landing prerequisite.

Spec: N7, S18, T12, N15

Transition in The landing (`claims/statechart/landing/Landing.tla`): Prerequisites → MergeReview, "every landing prerequisite met".

**coordinator** (Prerequisites, exit):

> Open the landing merge review only once every should is closed and the mutation check on the Project's diff is clean.

## Confident

`claim:Landing.Confident`: On the Merge-confidence PASS, the landing candidate goes to his walkthrough.

Spec: N6, L16, R20, H22

Transition in The landing (`claims/statechart/landing/Landing.tla`): MergeReview → Walkthrough, "the Merge-confidence council passes the landing candidate".

**coordinator** (MergeReview, exit):

> On the council's PASS, send the walkthrough of the landing candidate, the feature branch with master merged in, to a fresh envoy.

## ReviewFail

`claim:Landing.ReviewFail`: A broken or against-spec blocker from the landing merge review goes to Fix, then the next round.

Spec: R9, R11

Transition in The landing (`claims/statechart/landing/Landing.tla`): MergeReview → RoundFixes, "a blocker is broken or against the spec".

**coordinator** (MergeReview, exit):

> Route a landing merge-review blocker by kind: broken or against the spec to Fix, then the next round; a security hole to Fix, then the next round with the security lens first; fundamental or a new requirement to the officer above to re-plan; one that needs him up the chain at once.

## ReviewSecurity

`claim:Landing.ReviewSecurity`: A security hole from the landing merge review goes to Fix, then a round with the security lens first.

Spec: R9

Transition in The landing (`claims/statechart/landing/Landing.tla`): MergeReview → RoundFixes, "a blocker is a security hole".

**coordinator** (MergeReview, exit, on claim:Landing.ReviewFail):

> Route a landing merge-review blocker by kind: broken or against the spec to Fix, then the next round; a security hole to Fix, then the next round with the security lens first; fundamental or a new requirement to the officer above to re-plan; one that needs him up the chain at once.

## ReviewFundamental

`claim:Landing.ReviewFundamental`: A fundamental landing merge-review blocker, or a new requirement, goes to the officer above to re-plan.

Spec: R9

Transition in The landing (`claims/statechart/landing/Landing.tla`): MergeReview → Replanned, "a blocker is fundamental, or a new requirement".

**coordinator** (MergeReview, exit, on claim:Landing.ReviewFail):

> Route a landing merge-review blocker by kind: broken or against the spec to Fix, then the next round; a security hole to Fix, then the next round with the security lens first; fundamental or a new requirement to the officer above to re-plan; one that needs him up the chain at once.

## ReviewNeedsHim

`claim:Landing.ReviewNeedsHim`: A landing merge-review blocker that needs Balazs goes up the chain at once.

Spec: R9

Transition in The landing (`claims/statechart/landing/Landing.tla`): MergeReview → Escalated, "a blocker needs Balazs".

**coordinator** (MergeReview, exit, on claim:Landing.ReviewFail):

> Route a landing merge-review blocker by kind: broken or against the spec to Fix, then the next round; a security hole to Fix, then the next round with the security lens first; fundamental or a new requirement to the officer above to re-plan; one that needs him up the chain at once.

## Accepted

`claim:Landing.Accepted`: He accepts the landing candidate, then it merges.

Spec: N6

Transition in The landing (`claims/statechart/landing/Landing.tla`): Walkthrough → Merging, "Balazs accepts the landing candidate".

**coordinator** (Walkthrough, exit):

> When he accepts the landing candidate, it merges: AFK when two-way, on his click when one-way.

## NotAccepted

`claim:Landing.NotAccepted`: A landing walkthrough he does not accept files its findings as tickets in the same Project (DYD-1082 555a8300 gap 5).

Spec: N6

Transition in The landing (`claims/statechart/landing/Landing.tla`): Walkthrough → NotAccepted, "he does not accept: his findings become tickets in the same Project".

**ticket-captain** (Walkthrough, exit):

> When he does not accept the landing walkthrough, file each of his findings as a ticket in the same Project.

## ReCut

`claim:Landing.ReCut`: After a landing walkthrough not accepted, a fix captain re-cuts the branch and another landing follows (DYD-1082 555a8300 gap 5).

Spec: N6

Transition in The landing (`claims/statechart/landing/Landing.tla`): NotAccepted → Integrated, "a fix captain re-cuts the branch; another landing follows".

**ticket-captain** (NotAccepted, exit):

> As its fix captain, re-cut the landing branch once the findings' tickets are merged; another landing follows.

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

`claim:Landing.SpecGone`: At acceptance, the Wayfinding captain moves the spec's lasting parts and deletes the spec and plan files; the coordinator only opens its ticket.

Spec: S14, D11, N7, H4, H5

**coordinator** (Merging, nominal):

> Once he accepts, open a Wayfinding ticket to move the spec's lasting parts and delete the spec and the plan files.

**ticket-captain** (Wayfinding):

> At the landing, once he accepts, move the spec's lasting parts to DRs, the glossary, docs and the verification map, then delete the spec and the plan files.

## TagIsHis

`claim:Landing.TagIsHis`: Release tags are his.

Spec: R28, N2

**coordinator** (Landed, nominal):

> The release tag is his to push.
