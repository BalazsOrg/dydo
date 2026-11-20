---
area: reference
type: reference
---
<!-- built from the model; edit there: tools/SkillBuild -->

# Claims: Hops

## Single

`claim:Hops.Single`: A leaf lane or a batched should is one ticket, with no hops.

Spec: S4

Transition in A step's hops, by Type label (`claims/statechart/hops/Hops.tla`): Cut → OneTicket, "the step has no review loop of its own".

**ticket-captain** (Cut, exit):

> A leaf lane or a batched should stays one ticket, with no hops.

**coordinator** (Cut, exit):

> A leaf lane or a batched should stays one ticket, with no hops.

**linear-workspace-standard** (LinearTickets):

> A leaf lane or a batched should is one ticket, with no hops.

## Chain

`claim:Hops.Chain`: A step with its own review loop gets typed hop sub-tickets, blocked in step order.

Spec: S3, S4

Transition in A step's hops, by Type label (`claims/statechart/hops/Hops.tla`): Cut → ImplementHop, "the step has its own review loop: its Implement hop starts".

**ticket-captain** (Cut, exit):

> A step with its own review loop gets hop sub-tickets typed Implement, Review (one per round), Fix and Merge, blocked in step order; you may open them ahead in Todo.

**linear-workspace-standard** (LinearTickets):

> Hops appear only under a step with its own review loop: Implement, Review (one per round), Fix and Merge, blocked in step order; each goes Todo → In Progress → Done and may be opened ahead in Todo.

## AddCheck

`claim:Hops.AddCheck`: A leaf's evidence check is an optional fresh seat, a Review hop beside it.

Spec: S4, R6

Transition in A step's hops, by Type label (`claims/statechart/hops/Hops.tla`): OneTicket → EvidenceCheck, "the officer adds an evidence-check seat".

**ticket-captain** (OneTicket, exit):

> For a leaf's optional evidence check, add a fresh seat as a Review hop beside it, never a full review.

**linear-workspace-standard** (LinearTickets):

> A leaf's evidence check is a Review hop beside it.

## CheckDone

`claim:Hops.CheckDone`: The evidence check's Review hop closes Done.

Spec: S4, S6

Transition in A step's hops, by Type label (`claims/statechart/hops/Hops.tla`): EvidenceCheck → StepDone, "the evidence check's Review hop is Done". A system transition: no skill carries it.

## CheckFail

`claim:Hops.CheckFail`: A blocker in a leaf's evidence check becomes Fix rN at verdict time.

Spec: S9, R11

Transition in A step's hops, by Type label (`claims/statechart/hops/Hops.tla`): EvidenceCheck → CheckFix, "the evidence check finds a blocker". A system transition: no skill carries it.

## CheckAgain

`claim:Hops.CheckAgain`: After the fix, the next evidence check runs.

Spec: R8, S9

Transition in A step's hops, by Type label (`claims/statechart/hops/Hops.tla`): CheckFix → EvidenceCheck, "Fix rN Done; the next check runs". A system transition: no skill carries it.

## ImplementDone

`claim:Hops.ImplementDone`: The Implement hop is Done before the Review hop starts, in step order.

Spec: S3, R2

Transition in A step's hops, by Type label (`claims/statechart/hops/Hops.tla`): ImplementHop → ReviewHop, "Implement hop Done; Review r1 starts". A system transition: no skill carries it.

## ReviewFails

`claim:Hops.ReviewFails`: A Review hop with a blocker is followed by a Fix hop.

Spec: S3, S9, R11

Transition in A step's hops, by Type label (`claims/statechart/hops/Hops.tla`): ReviewHop → FixHop, "FAIL: a Fix rN hop". A system transition: no skill carries it.

## FixedHop

`claim:Hops.FixedHop`: A Fix hop is followed by the next round's Review hop.

Spec: S3, S9, R8

Transition in A step's hops, by Type label (`claims/statechart/hops/Hops.tla`): FixHop → ReviewHop, "Fix rN Done; the next round's Review hop". A system transition: no skill carries it.

## LeafDone

`claim:Hops.LeafDone`: A leaf's holder closes it Done.

Spec: S6

Transition in A step's hops, by Type label (`claims/statechart/hops/Hops.tla`): OneTicket → StepDone, "its holder closes it Done". A system transition: no skill carries it.

## AfkType

`claim:Hops.AfkType`: An AFK Merge is a feature-branch merge or a two-way main merge.

Spec: S12

Transition in A step's hops, by Type label (`claims/statechart/hops/Hops.tla`): ReviewHop → AfkMerge, "PASS: the Merge hop, AFK into the feature branch or two-way into main".

**ticket-captain** (ReviewHop, exit):

> Type the Merge hop AFK Merge for a feature-branch merge or a two-way main merge.

**linear-workspace-standard** (LinearTickets):

> Merge sub-tickets: an AFK Merge for a feature-branch merge or a two-way main merge, a HITL Merge only for a one-way one.

## HitlType

`claim:Hops.HitlType`: A HITL Merge is only for a one-way main merge.

Spec: S12, R27

Transition in A step's hops, by Type label (`claims/statechart/hops/Hops.tla`): ReviewHop → HitlMerge, "PASS: the Merge hop, HITL for a one-way main merge".

**linear-workspace-standard** (LinearTickets, on claim:Hops.AfkType):

> Merge sub-tickets: an AFK Merge for a feature-branch merge or a two-way main merge, a HITL Merge only for a one-way one.

**ticket-captain** (ReviewHop, exit):

> Type the Merge hop HITL Merge only for a one-way main merge.

## TypeLabels

`claim:Hops.TypeLabels`: Each hop's kind is its Type label.

Spec: S3

**linear-workspace-standard** (LinearLabels):

> Each hop's kind is its Type label: Implement, Review, Fix, Blocker and Wayfinding sit in the Type group next to `Merge`; setting them up is his Enablement.
