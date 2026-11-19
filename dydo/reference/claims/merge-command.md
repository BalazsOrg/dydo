---
area: reference
type: reference
---
<!-- built from the model; edit there: tools/SkillBuild -->

# Claims: MergeCommand

## NoPass

`claim:MergeCommand.NoPass`: dydo merge refuses, with a stated reason: no PASS on the head, nor on an ancestor followed only by clean syncs.

Spec: R17, R23, R24

Transition in dydo merge and its refusals (`claims/statechart/merge-command/MergeCommand.tla`): Invoked → Refused, "refused: no PASS on the head, nor on an ancestor followed only by clean syncs". A system transition: no skill carries it.

## ReviewOpen

`claim:MergeCommand.ReviewOpen`: dydo merge refuses, with a stated reason: the Review is not Done.

Spec: R17

Transition in dydo merge and its refusals (`claims/statechart/merge-command/MergeCommand.tla`): Invoked → Refused, "refused: the Review is not Done". A system transition: no skill carries it.

## NoReceipt

`claim:MergeCommand.NoReceipt`: dydo merge refuses, with a stated reason: DYTEST holds no green receipt for the combined tree.

Spec: R23, Y4

Transition in dydo merge and its refusals (`claims/statechart/merge-command/MergeCommand.tla`): Invoked → Refused, "refused: DYTEST holds no green receipt for the combined tree". A system transition: no skill carries it.

## HandShaped

`claim:MergeCommand.HandShaped`: dydo merge refuses, with a stated reason: a hand-shaped tree, not git merge-tree --write-tree p1 p2.

Spec: R19

Transition in dydo merge and its refusals (`claims/statechart/merge-command/MergeCommand.tla`): Invoked → Refused, "refused: a hand-shaped tree, not git merge-tree --write-tree p1 p2". A system transition: no skill carries it.

## TipMoved

`claim:MergeCommand.TipMoved`: dydo merge refuses, with a stated reason: the target's tip moved from the pin.

Spec: N12 DR 061 §2

Transition in dydo merge and its refusals (`claims/statechart/merge-command/MergeCommand.tla`): Invoked → Refused, "refused: the target's tip moved from the pin". A system transition: no skill carries it.

## DraftOrFork

`claim:MergeCommand.DraftOrFork`: dydo merge refuses, with a stated reason: a draft or fork PR, or a closed PR not yet in its base.

Spec: N12 DR 061 §2

Transition in dydo merge and its refusals (`claims/statechart/merge-command/MergeCommand.tla`): Invoked → Refused, "refused: a draft or fork PR, or a closed PR not yet in its base". A system transition: no skill carries it.

## OneWay

`claim:MergeCommand.OneWay`: dydo merge refuses, with a stated reason: a PR into master that the dydo.json path list flags one-way.

Spec: R27

Transition in dydo merge and its refusals (`claims/statechart/merge-command/MergeCommand.tla`): Invoked → Refused, "refused: a PR into master that the dydo.json path list flags one-way". A system transition: no skill carries it.

## NotTwoWay

`claim:MergeCommand.NotTwoWay`: dydo merge refuses, with a stated reason: a PR into master whose body does not declare it two-way, or whose CI is not green.

Spec: R26

Transition in dydo merge and its refusals (`claims/statechart/merge-command/MergeCommand.tla`): Invoked → Refused, "refused: a PR into master whose body does not declare it two-way, or whose CI is not green". A system transition: no skill carries it.

## Conflict

`claim:MergeCommand.Conflict`: dydo merge reports CONFLICT when the merge with the tip is not clean.

Spec: R21, R23

Transition in dydo merge and its refusals (`claims/statechart/merge-command/MergeCommand.tla`): Invoked → Conflict, "the merge with the tip is not clean". A system transition: no skill carries it.

## AlreadyIn

`claim:MergeCommand.AlreadyIn`: A closed PR whose SHA is already in its base exits 0, already integrated.

Spec: N12 DR 061 §2

Transition in dydo merge and its refusals (`claims/statechart/merge-command/MergeCommand.tla`): Invoked → AlreadyIn, "a closed PR already in its base". A system transition: no skill carries it.

## Merge

`claim:MergeCommand.Merge`: dydo merge merges only when every check holds, as a merge commit.

Spec: R23, R24, R29, R32, N12 DR 061 §3

Transition in dydo merge and its refusals (`claims/statechart/merge-command/MergeCommand.tla`): Invoked → Merged, "every check holds: a merge commit; a re-land reverts the revert".

**working-tree-standard** (TreeMerging):

> `dydo merge` merges only when the Review is Done with a PASS on the head, or on an ancestor whose later commits are all clean syncs with their second parent already in the target, the merge with the tip is clean, and DYTEST holds a green receipt keyed by the combined tree; it refuses a hand-shaped merge, and there is no `--verify`.
