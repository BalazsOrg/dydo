\* built from the model; edit there: tools/SkillBuild
---- MODULE RoleEvaluation ----

VARIABLES state

vars == <<state>>

Finals == {"Approved"}

Open == {"GAP 1"}

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

\* claim:Evaluation.Remove
Evaluation_Remove ==
    /\ state = "Questioned"
    /\ state' = "Removed"

\* claim:Evaluation.Rewrite
Evaluation_Rewrite ==
    /\ state = "Kept"
    /\ state' = "WrittenFresh"

\* GAP: K16 and K22 give his approval at the final candidate, not the route when he rejects a fold or a rewrite
Gap1 ==
    /\ state = "WrittenFresh"
    /\ state' = "GAP 1"

Done == state \in Finals \cup Open /\ UNCHANGED vars

Next ==
    \/ Evaluation_Approve
    \/ Evaluation_ApproveFold
    \/ Evaluation_ApproveRemoval
    \/ Evaluation_Fold
    \/ Evaluation_Keep
    \/ Evaluation_Remove
    \/ Evaluation_Rewrite
    \/ Gap1
    \/ Done

Spec == Init /\ [][Next]_vars
====
