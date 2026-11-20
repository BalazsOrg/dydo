---
area: reference
type: reference
---
<!-- built from the model; edit there: tools/SkillBuild -->

# Claims: Mutation

## LandingCheck

`claim:Mutation.LandingCheck`: The landing check queues mutation work missing or stale for the exact tree it judges.

Spec: T12, T13

Transition in The landing's mutation check (`claims/statechart/mutation/Mutation.tla`): Background → Queued, "the landing check judges the Project's diff". A system transition: no skill carries it.

## Schedule

`claim:Mutation.Schedule`: Landing mutation work is scheduled exactly like a test run, on local machines and peers.

Spec: T12

Transition in The landing's mutation check (`claims/statechart/mutation/Mutation.tla`): Queued → Running, "scheduled like a test run, at normal priority". A system transition: no skill carries it.

## Cloud

`claim:Mutation.Cloud`: Under a large queue it spills to the cloud only if that route is not much more work than the test-run one.

Spec: T12

Transition in The landing's mutation check (`claims/statechart/mutation/Mutation.tla`): Queued → Running, "a large queue spills to the cloud". A system transition: no skill carries it.

## Survive

`claim:Mutation.Survive`: A mutant survives in the Project's changed code.

Spec: T12

Transition in The landing's mutation check (`claims/statechart/mutation/Mutation.tla`): Running → Survivors, "a mutant survives". A system transition: no skill carries it.

## AllKilled

`claim:Mutation.AllKilled`: No survivor is left without a fix or an approved equivalent.

Spec: T12, N15

Transition in The landing's mutation check (`claims/statechart/mutation/Mutation.tla`): Running → Clean, "no survivor without a fix or an approved equivalent". A system transition: no skill carries it.

## CutFix

`claim:Mutation.CutFix`: The coordinator holds the landing and opens each survivor's fix ticket for a captain.

Spec: N6, H5, T12

Transition in The landing's mutation check (`claims/statechart/mutation/Mutation.tla`): Survivors → Ticketed, "the coordinator opens the survivor's fix ticket".

**coordinator** (Survivors, exit):

> Open a fix ticket for each landing survivor and put a captain on it.

## Fix

`claim:Mutation.Fix`: A survivor is fixed, and the changed tree's mutation work is queued again.

Spec: T12

Transition in The landing's mutation check (`claims/statechart/mutation/Mutation.tla`): Ticketed → Queued, "fixed; the changed tree's work is queued again".

**implementer** (Ticketed, exit):

> Fix a surviving mutant: sharpen the test that missed it, or delete the code it shows nothing needs.

## ClaimEquivalent

`claim:Mutation.ClaimEquivalent`: The author claims a survivor equivalent, with a reason, and never approves its own claim.

Spec: T12

Transition in The landing's mutation check (`claims/statechart/mutation/Mutation.tla`): Ticketed → Claimed, "claimed equivalent, with a reason".

**implementer** (Ticketed, exit):

> Or claim it equivalent, with a reason; a fresh reviewer decides the claim, never you.

## Approve

`claim:Mutation.Approve`: A fresh independent reviewer approves an equivalent one at a time, bound to the mutant and its code.

Spec: T12

Transition in The landing's mutation check (`claims/statechart/mutation/Mutation.tla`): Claimed → Clean, "a fresh independent reviewer approves it".

**reviewer** (Claimed, exit):

> Approve an equivalent mutant one at a time, with a reason, only if you wrote neither the code nor its tests; your approval binds that mutant and its surrounding code.

## Rejected

`claim:Mutation.Rejected`: A rejected claim the author accepts leaves the survivor to fix.

Spec: T12

Transition in The landing's mutation check (`claims/statechart/mutation/Mutation.tla`): Claimed → Ticketed, "rejected, and the author accepts: back to fixing". A system transition: no skill carries it.

## Reject

`claim:Mutation.Reject`: A claim disputed between author and reviewer goes to a council.

Spec: T12, L16

Transition in The landing's mutation check (`claims/statechart/mutation/Mutation.tla`): Claimed → Dispute, "rejected, and the author disputes it: a council".

**reviewer** (Claimed, exit):

> A rejected claim the author disputes goes to a council.

## Ruled

`claim:Mutation.Ruled`: The council rules the mutant equivalent.

Spec: T12, L14

Transition in The landing's mutation check (`claims/statechart/mutation/Mutation.tla`): Dispute → Clean, "the council rules it equivalent". A system transition: no skill carries it.

## RuledNot

`claim:Mutation.RuledNot`: The council rules the mutant a real survivor; it gets a fix.

Spec: T12, L14

Transition in The landing's mutation check (`claims/statechart/mutation/Mutation.tla`): Dispute → Ticketed, "the council rules it a real survivor". A system transition: no skill carries it.

## Lapse

`claim:Mutation.Lapse`: An approval lapses when the mutant or its surrounding code changes.

Spec: T12

Transition in The landing's mutation check (`claims/statechart/mutation/Mutation.tla`): Clean → Queued, "the mutant or its code changes: the approval lapses". A system transition: no skill carries it.

## Totals

`claim:Mutation.Totals`: He sees the totals of approved equivalents per feature, through the coordinator's envoy, the one channel to him.

Spec: T12, H6, H12

**coordinator** (Prerequisites, nominal):

> Post the per-feature totals of approved equivalent mutants on your HITL queue as an FYI for him, riding in the next envoy.
