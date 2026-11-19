---
area: reference
type: reference
---
<!-- built from the model; edit there: tools/SkillBuild -->

# Claims: FeatureMerge

## Start

`claim:FeatureMerge.Start`: The Merge hop's blocker clears, blocked in step order.

Spec: S3, R21

Transition in The Merge hop into the feature branch (`claims/statechart/feature-merge/FeatureMerge.tla`): MergeTodo → PreGate, "the Merge hop's blocker clears". A system transition: no skill carries it.

## PreGreen

`claim:FeatureMerge.PreGreen`: dydo test gate leaves a green receipt for the combined tree; the merge runs.

Spec: Y4, R23

Transition in The Merge hop into the feature branch (`claims/statechart/feature-merge/FeatureMerge.tla`): PreGate → Running, "a green receipt for the combined tree". A system transition: no skill carries it.

## PreRed

`claim:FeatureMerge.PreRed`: A real red gate goes to a code-writer.

Spec: R21

Transition in The Merge hop into the feature branch (`claims/statechart/feature-merge/FeatureMerge.tla`): PreGate → PreFix, "a real red gate before the merge".

**ticket-captain** (PreGate, exit):

> On a real red gate, send a code-writer to fix it on the ticket branch.

## PreFixed

`claim:FeatureMerge.PreFixed`: A fix moves the head, so a fresh round must pass the new head.

Spec: R8, R23

Transition in The Merge hop into the feature branch (`claims/statechart/feature-merge/FeatureMerge.tla`): PreFix → Rereview, "the fix moves the head: a fresh round". A system transition: no skill carries it.

## RereviewPass

`claim:FeatureMerge.RereviewPass`: A PASS on the new head lets the gate and the merge run again.

Spec: R17, R23

Transition in The Merge hop into the feature branch (`claims/statechart/feature-merge/FeatureMerge.tla`): Rereview → PreGate, "PASS on the new head". A system transition: no skill carries it.

## NoPass

`claim:FeatureMerge.NoPass`: dydo merge refuses a PR with no PASS on its head; a review round follows.

Spec: R17, R23, R24

Transition in The Merge hop into the feature branch (`claims/statechart/feature-merge/FeatureMerge.tla`): Running → Rereview, "refused: no PASS on the head or a clean-sync ancestor". A system transition: no skill carries it.

## NoReceipt

`claim:FeatureMerge.NoReceipt`: dydo merge refuses without a green receipt; the captain runs dydo test gate.

Spec: R21, R23, Y4

Transition in The Merge hop into the feature branch (`claims/statechart/feature-merge/FeatureMerge.tla`): Running → PreGate, "refused: no green receipt for the combined tree". A system transition: no skill carries it.

## HandShaped

`claim:FeatureMerge.HandShaped`: dydo merge refuses a hand-shaped tree; it is resolved on the ticket branch with a hunk review.

Spec: R19

Transition in The Merge hop into the feature branch (`claims/statechart/feature-merge/FeatureMerge.tla`): Running → Resolving, "refused: a hand-shaped tree". A system transition: no skill carries it.

## Conflict

`claim:FeatureMerge.Conflict`: A CONFLICT goes to a code-writer, resolved only on the ticket branch.

Spec: R21, R19

Transition in The Merge hop into the feature branch (`claims/statechart/feature-merge/FeatureMerge.tla`): Running → Resolving, "dydo merge reports CONFLICT".

**ticket-captain** (Running, exit):

> On `CONFLICT`, send a code-writer to resolve it on the ticket branch.

**code-writer** (Resolving, arrival):

> Resolve a merge conflict on the ticket branch only, then commit it as `<KEY> merge: <what>`.

**working-tree-standard** (TreeMerging):

> A conflict is resolved only on the ticket branch, under a fresh review of its hunks.

## Resolved

`claim:FeatureMerge.Resolved`: A conflict's resolution gets a fresh review of its hunks.

Spec: R19

Transition in The Merge hop into the feature branch (`claims/statechart/feature-merge/FeatureMerge.tla`): Resolving → HunkReview, "the resolution is committed".

**ticket-captain** (Resolving, exit):

> Send the resolution's hunks to a fresh reviewer.

**reviewer** (HunkReview, arrival):

> Judge the conflict resolution's hunks, the only new work on the branch.

## HunksPass

`claim:FeatureMerge.HunksPass`: A PASS on the hunks names the new head; the merge runs again.

Spec: R19, R17

Transition in The Merge hop into the feature branch (`claims/statechart/feature-merge/FeatureMerge.tla`): HunkReview → PreGate, "PASS on the hunks; the gate and the merge run again".

**reviewer** (HunkReview, exit):

> Post the hunks' verdict as for any round; its PASS names the new head.

## Merge

`claim:FeatureMerge.Merge`: dydo merge writes the merge commit.

Spec: R23, R32

Transition in The Merge hop into the feature branch (`claims/statechart/feature-merge/FeatureMerge.tla`): Running → Gating, "dydo merge writes the merge commit". A system transition: no skill carries it.

## Green

`claim:FeatureMerge.Green`: A Merge hop is Done on DYTEST's green receipt on the merge tree and green hosted CI on the merge SHA.

Spec: R18, R20

Transition in The Merge hop into the feature branch (`claims/statechart/feature-merge/FeatureMerge.tla`): Gating → MergeDone, "DYTEST green on the merge tree, CI green on the merge SHA".

**ticket-captain** (Gating, exit):

> Close the Merge hop Done once DYTEST holds a green receipt on the merge tree and hosted CI is green on the merge SHA.

**working-tree-standard** (TreeMerging):

> A Merge hop is Done when DYTEST holds a green receipt on the merge tree and hosted CI is green on the merge SHA.

## Red

`claim:FeatureMerge.Red`: A red gate after a merge reruns once.

Spec: R22

Transition in The Merge hop into the feature branch (`claims/statechart/feature-merge/FeatureMerge.tla`): Gating → Rerun, "a gate goes red after the merge".

**ticket-captain** (Gating, exit):

> A gate red after the merge reruns once through DYTEST.

**working-tree-standard** (TreeMerging, on claim:FeatureMerge.RealRed):

> A gate red after a merge reruns once; green is a flake and the hop is Done; red again is real: the captain reverts at once and files a CI-findings ticket, a Fix follows on the ticket branch, then a fix-scoped round, then a re-land where `dydo merge` reverts the revert.

## Flake

`claim:FeatureMerge.Flake`: Green on the rerun is a flake: the hop is Done and DYTEST counts it.

Spec: R22, Y9

Transition in The Merge hop into the feature branch (`claims/statechart/feature-merge/FeatureMerge.tla`): Rerun → MergeDone, "green on the rerun: a flake".

**ticket-captain** (Rerun, exit):

> Green on the rerun is a flake: the hop is Done, and DYTEST counts the flake; it becomes a ticket only when it repeats.

**working-tree-standard** (TreeMerging, on claim:FeatureMerge.RealRed):

> A gate red after a merge reruns once; green is a flake and the hop is Done; red again is real: the captain reverts at once and files a CI-findings ticket, a Fix follows on the ticket branch, then a fix-scoped round, then a re-land where `dydo merge` reverts the revert.

## RealRed

`claim:FeatureMerge.RealRed`: A real red is reverted at once and becomes a CI-findings ticket.

Spec: R22, Y9, S12

Transition in The Merge hop into the feature branch (`claims/statechart/feature-merge/FeatureMerge.tla`): Rerun → Reverted, "red again: a real red".

**ticket-captain** (Rerun, exit):

> Red again, after any merge, is real: revert at once, cancel the Merge hop with `reverted: <gate> red on <merge SHA>`, and file a CI-findings ticket.

**linear-workspace-standard** (LinearStatuses):

> A reverted Merge goes Canceled with `reverted: <gate> red on <merge SHA>`.

**working-tree-standard** (TreeMerging):

> A gate red after a merge reruns once; green is a flake and the hop is Done; red again is real: the captain reverts at once and files a CI-findings ticket, a Fix follows on the ticket branch, then a fix-scoped round, then a re-land where `dydo merge` reverts the revert.

## FixAfterRevert

`claim:FeatureMerge.FixAfterRevert`: After a revert, a Fix on the ticket branch and a fix-scoped round.

Spec: R22, R21

Transition in The Merge hop into the feature branch (`claims/statechart/feature-merge/FeatureMerge.tla`): Reverted → FixRound, "a Fix follows on the ticket branch".

**working-tree-standard** (TreeMerging, on claim:FeatureMerge.RealRed):

> A gate red after a merge reruns once; green is a flake and the hop is Done; red again is real: the captain reverts at once and files a CI-findings ticket, a Fix follows on the ticket branch, then a fix-scoped round, then a re-land where `dydo merge` reverts the revert.

**ticket-captain** (Reverted, exit):

> Send a code-writer to fix it on the ticket branch, then hold a fix-scoped round.

## Reland

`claim:FeatureMerge.Reland`: The re-land runs dydo merge, which reverts the revert.

Spec: R22

Transition in The Merge hop into the feature branch (`claims/statechart/feature-merge/FeatureMerge.tla`): FixRound → PreGate, "the fix-scoped round passes; re-land".

**working-tree-standard** (TreeMerging, on claim:FeatureMerge.RealRed):

> A gate red after a merge reruns once; green is a flake and the hop is Done; red again is real: the captain reverts at once and files a CI-findings ticket, a Fix follows on the ticket branch, then a fix-scoped round, then a re-land where `dydo merge` reverts the revert.

**ticket-captain** (FixRound, exit):

> Re-land with `dydo merge`, which reverts the revert.

## GateFirst

`claim:FeatureMerge.GateFirst`: The captain runs dydo test gate itself before dydo merge.

Spec: R21, Y4

**ticket-captain** (PreGate, nominal):

> Run `dydo test gate` on the combined tree yourself, then `dydo merge`.

## OneEntry

`claim:FeatureMerge.OneEntry`: dydo merge is the one entry point and states why it refuses.

Spec: R23

**ticket-captain** (Running, nominal):

> Merge only through `dydo merge`; when it refuses, its stated reason says why.

## NoMergeReview

`claim:FeatureMerge.NoMergeReview`: A conflict-free merge into the feature branch gets no merge review.

Spec: R18

**ticket-captain** (Running, nominal):

> A conflict-free merge into the feature branch gets no merge review: the combined gates on the merge SHA and hosted CI on the `feature/**` push prove it.

**working-tree-standard** (TreeMerging):

> A conflict-free merge into the feature branch is proved by the combined gates on the merge SHA plus hosted CI on the `feature/**` push; there is no merge review.

## NoMutationWait

`claim:FeatureMerge.NoMutationWait`: No ticket or merge waits on mutation.

Spec: T12, T14

**ticket-captain** (Running, nominal):

> No ticket or merge waits on mutation; it is checked at the landing.
