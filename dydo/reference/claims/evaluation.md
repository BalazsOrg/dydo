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

## RejectRewrite

`claim:Evaluation.RejectRewrite`: In the role evaluation, a rewrite he rejects at his walkthrough is a change for new captains on new tickets (DYD-1195 8661b8f4).

Spec: K22, K16

Transition in The nine role skills evaluated (`claims/statechart/role-evaluation/RoleEvaluation.tla`): WrittenFresh → Changed, "he rejects the rewrite: a change for new captains on new tickets". A system transition: no skill carries it.

## RejectFold

`claim:Evaluation.RejectFold`: In the role evaluation, a fold he rejects at his walkthrough is a change for new captains on new tickets (DYD-1195 8661b8f4).

Spec: K22, K16

Transition in The nine role skills evaluated (`claims/statechart/role-evaluation/RoleEvaluation.tla`): Folded → Changed, "he rejects the fold: a change for new captains on new tickets". A system transition: no skill carries it.

## RejectRemoval

`claim:Evaluation.RejectRemoval`: In the role evaluation, a removal he rejects at his walkthrough is a change for new captains on new tickets (DYD-1195 8661b8f4).

Spec: K22, K16

Transition in The nine role skills evaluated (`claims/statechart/role-evaluation/RoleEvaluation.tla`): Removed → Changed, "he rejects the removal: a change for new captains on new tickets". A system transition: no skill carries it.

## Rebuild

`claim:Evaluation.Rebuild`: In the role evaluation, new captains on new tickets take a rejected change: the role is questioned again with his verdict, and the candidate is rebuilt (DYD-1195 8661b8f4).

Spec: K22, K16

Transition in The nine role skills evaluated (`claims/statechart/role-evaluation/RoleEvaluation.tla`): Changed → Questioned, "new captains on new tickets question it again with his verdict, and rebuild". A system transition: no skill carries it.
