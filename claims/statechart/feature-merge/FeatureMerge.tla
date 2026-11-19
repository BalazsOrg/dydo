\* built from the model; edit there: tools/SkillBuild
---- MODULE FeatureMerge ----

VARIABLES state

vars == <<state>>

Finals == {"MergeDone"}

Init ==
    /\ state = "MergeTodo"

\* claim:FeatureMerge.Conflict
FeatureMerge_Conflict ==
    /\ state = "Running"
    /\ state' = "Resolving"

\* claim:FeatureMerge.FixAfterRevert
FeatureMerge_FixAfterRevert ==
    /\ state = "Reverted"
    /\ state' = "FixRound"

\* claim:FeatureMerge.Flake
FeatureMerge_Flake ==
    /\ state = "Rerun"
    /\ state' = "MergeDone"

\* claim:FeatureMerge.Green
FeatureMerge_Green ==
    /\ state = "Gating"
    /\ state' = "MergeDone"

\* claim:FeatureMerge.HandShaped
FeatureMerge_HandShaped ==
    /\ state = "Running"
    /\ state' = "Resolving"

\* claim:FeatureMerge.HunksPass
FeatureMerge_HunksPass ==
    /\ state = "HunkReview"
    /\ state' = "PreGate"

\* claim:FeatureMerge.Merge
FeatureMerge_Merge ==
    /\ state = "Running"
    /\ state' = "Gating"

\* claim:FeatureMerge.NoPass
FeatureMerge_NoPass ==
    /\ state = "Running"
    /\ state' = "Rereview"

\* claim:FeatureMerge.NoReceipt
FeatureMerge_NoReceipt ==
    /\ state = "Running"
    /\ state' = "PreGate"

\* claim:FeatureMerge.PreFixed
FeatureMerge_PreFixed ==
    /\ state = "PreFix"
    /\ state' = "Rereview"

\* claim:FeatureMerge.PreGreen
FeatureMerge_PreGreen ==
    /\ state = "PreGate"
    /\ state' = "Running"

\* claim:FeatureMerge.PreRed
FeatureMerge_PreRed ==
    /\ state = "PreGate"
    /\ state' = "PreFix"

\* claim:FeatureMerge.RealRed
FeatureMerge_RealRed ==
    /\ state = "Rerun"
    /\ state' = "Reverted"

\* claim:FeatureMerge.Red
FeatureMerge_Red ==
    /\ state = "Gating"
    /\ state' = "Rerun"

\* claim:FeatureMerge.Reland
FeatureMerge_Reland ==
    /\ state = "FixRound"
    /\ state' = "PreGate"

\* claim:FeatureMerge.RereviewPass
FeatureMerge_RereviewPass ==
    /\ state = "Rereview"
    /\ state' = "PreGate"

\* claim:FeatureMerge.Resolved
FeatureMerge_Resolved ==
    /\ state = "Resolving"
    /\ state' = "HunkReview"

\* claim:FeatureMerge.Start
FeatureMerge_Start ==
    /\ state = "MergeTodo"
    /\ state' = "PreGate"

Done == state \in Finals /\ UNCHANGED vars

Next ==
    \/ FeatureMerge_Conflict
    \/ FeatureMerge_FixAfterRevert
    \/ FeatureMerge_Flake
    \/ FeatureMerge_Green
    \/ FeatureMerge_HandShaped
    \/ FeatureMerge_HunksPass
    \/ FeatureMerge_Merge
    \/ FeatureMerge_NoPass
    \/ FeatureMerge_NoReceipt
    \/ FeatureMerge_PreFixed
    \/ FeatureMerge_PreGreen
    \/ FeatureMerge_PreRed
    \/ FeatureMerge_RealRed
    \/ FeatureMerge_Red
    \/ FeatureMerge_Reland
    \/ FeatureMerge_RereviewPass
    \/ FeatureMerge_Resolved
    \/ FeatureMerge_Start
    \/ Done

Fairness == TRUE

Spec == Init /\ [][Next]_vars /\ Fairness
====
