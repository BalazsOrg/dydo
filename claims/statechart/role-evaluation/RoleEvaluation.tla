\* built from the model; edit there: tools/SkillBuild
---- MODULE RoleEvaluation ----

VARIABLES state

vars == <<state>>

Finals == {"Approved"}

Init ==
    /\ state = "Questioned"

\* claim:Evaluation.Approve
Evaluation_Approve ==
    /\ state = "WrittenFresh"
    /\ state' = "Approved"

\* claim:Evaluation.ApproveFold
Evaluation_ApproveFold ==
    /\ state = "Folded"
    /\ state' = "Approved"

\* claim:Evaluation.ApproveRemoval
Evaluation_ApproveRemoval ==
    /\ state = "Removed"
    /\ state' = "Approved"

\* claim:Evaluation.Fold
Evaluation_Fold ==
    /\ state = "Questioned"
    /\ state' = "Folded"

\* claim:Evaluation.Keep
Evaluation_Keep ==
    /\ state = "Questioned"
    /\ state' = "Kept"

\* claim:Evaluation.Rebuild
Evaluation_Rebuild ==
    /\ state = "Changed"
    /\ state' = "Questioned"

\* claim:Evaluation.RejectFold
Evaluation_RejectFold ==
    /\ state = "Folded"
    /\ state' = "Changed"

\* claim:Evaluation.RejectRemoval
Evaluation_RejectRemoval ==
    /\ state = "Removed"
    /\ state' = "Changed"

\* claim:Evaluation.RejectRewrite
Evaluation_RejectRewrite ==
    /\ state = "WrittenFresh"
    /\ state' = "Changed"

\* claim:Evaluation.Remove
Evaluation_Remove ==
    /\ state = "Questioned"
    /\ state' = "Removed"

\* claim:Evaluation.Rewrite
Evaluation_Rewrite ==
    /\ state = "Kept"
    /\ state' = "WrittenFresh"

Done == state \in Finals /\ UNCHANGED vars

Next ==
    \/ Evaluation_Approve
    \/ Evaluation_ApproveFold
    \/ Evaluation_ApproveRemoval
    \/ Evaluation_Fold
    \/ Evaluation_Keep
    \/ Evaluation_Rebuild
    \/ Evaluation_RejectFold
    \/ Evaluation_RejectRemoval
    \/ Evaluation_RejectRewrite
    \/ Evaluation_Remove
    \/ Evaluation_Rewrite
    \/ Done

Fairness == TRUE

Spec == Init /\ [][Next]_vars /\ Fairness
====
