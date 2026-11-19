---
area: reference
type: reference
---
<!-- built from the model; edit there: tools/SkillBuild -->

# Claims: Evaluation

## Keep

`claim:Evaluation.Keep`: In the role evaluation, questioned, it should exist: a survivor.

Spec: K22

Transition in The nine role skills evaluated (`claims/statechart/role-evaluation/RoleEvaluation.tla`): Questioned → Kept, "questioned, it should exist: a survivor". A system transition: no skill carries it.

## Fold

`claim:Evaluation.Fold`: In the role evaluation, questioned, it folds into a survivor, which carries its responsibilities.

Spec: K22

Transition in The nine role skills evaluated (`claims/statechart/role-evaluation/RoleEvaluation.tla`): Questioned → Folded, "questioned, it folds into a survivor, which carries its responsibilities". A system transition: no skill carries it.

## Remove

`claim:Evaluation.Remove`: In the role evaluation, questioned, it should not exist: removed.

Spec: K22

Transition in The nine role skills evaluated (`claims/statechart/role-evaluation/RoleEvaluation.tla`): Questioned → Removed, "questioned, it should not exist: removed". A system transition: no skill carries it.

## Rewrite

`claim:Evaluation.Rewrite`: In the role evaluation, only a survivor is written fresh from the 4.0.0 decisions and claims, simpler and streamlined; its current file is a checklist of what not to lose, never a source.

Spec: K22, K19

Transition in The nine role skills evaluated (`claims/statechart/role-evaluation/RoleEvaluation.tla`): Kept → WrittenFresh, "only a survivor is written fresh from the 4.0.0 decisions and claims, simpler and streamlined; its current file is a checklist of what not to lose, never a source". A system transition: no skill carries it.

## Approve

`claim:Evaluation.Approve`: In the role evaluation, he approves the rewrite with the other claims, at the final candidate.

Spec: K16, K22

Transition in The nine role skills evaluated (`claims/statechart/role-evaluation/RoleEvaluation.tla`): WrittenFresh → Approved, "he approves the rewrite with the other claims, at the final candidate". A system transition: no skill carries it.

## ApproveFold

`claim:Evaluation.ApproveFold`: In the role evaluation, he approves the fold with the other claims, at the final candidate.

Spec: K16, K22

Transition in The nine role skills evaluated (`claims/statechart/role-evaluation/RoleEvaluation.tla`): Folded → Approved, "he approves the fold with the other claims, at the final candidate". A system transition: no skill carries it.

## ApproveRemoval

`claim:Evaluation.ApproveRemoval`: In the role evaluation, he approves the removal with the other claims, at the final candidate.

Spec: K16, K22

Transition in The nine role skills evaluated (`claims/statechart/role-evaluation/RoleEvaluation.tla`): Removed → Approved, "he approves the removal with the other claims, at the final candidate". A system transition: no skill carries it.
