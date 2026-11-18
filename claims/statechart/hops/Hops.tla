\* built from the model; edit there: tools/SkillBuild
---- MODULE Hops ----

VARIABLES state

vars == <<state>>

Finals == {"StepDone", "AfkMerge", "HitlMerge"}

Init ==
    /\ state = "Cut"

\* claim:Hops.AddCheck
Hops_AddCheck ==
    /\ state = "OneTicket"
    /\ state' = "EvidenceCheck"

\* claim:Hops.AfkType
Hops_AfkType ==
    /\ state = "Chain"
    /\ state' = "AfkMerge"

\* claim:Hops.Chain
Hops_Chain ==
    /\ state = "Cut"
    /\ state' = "Chain"

\* claim:Hops.CheckAgain
Hops_CheckAgain ==
    /\ state = "CheckFix"
    /\ state' = "EvidenceCheck"

\* claim:Hops.CheckDone
Hops_CheckDone ==
    /\ state = "EvidenceCheck"
    /\ state' = "StepDone"

\* claim:Hops.CheckFail
Hops_CheckFail ==
    /\ state = "EvidenceCheck"
    /\ state' = "CheckFix"

\* claim:Hops.HitlType
Hops_HitlType ==
    /\ state = "Chain"
    /\ state' = "HitlMerge"

\* claim:Hops.LeafDone
Hops_LeafDone ==
    /\ state = "OneTicket"
    /\ state' = "StepDone"

\* claim:Hops.Single
Hops_Single ==
    /\ state = "Cut"
    /\ state' = "OneTicket"

Done == state \in Finals /\ UNCHANGED vars

Next ==
    \/ Hops_AddCheck
    \/ Hops_AfkType
    \/ Hops_Chain
    \/ Hops_CheckAgain
    \/ Hops_CheckDone
    \/ Hops_CheckFail
    \/ Hops_HitlType
    \/ Hops_LeafDone
    \/ Hops_Single
    \/ Done

Spec == Init /\ [][Next]_vars
====
