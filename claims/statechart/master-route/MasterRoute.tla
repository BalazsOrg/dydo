\* built from the model; edit there: tools/SkillBuild
---- MODULE MasterRoute ----
EXTENDS Naturals

VARIABLES state, resyncs

vars == <<state, resyncs>>

Finals == {"Closed"}

Init ==
    /\ state = "Offered"
    /\ resyncs = 0

\* claim:MasterRoute.AfkDone
MasterRoute_AfkDone ==
    /\ state = "AfkMerge"
    /\ state' = "Merged"
    /\ UNCHANGED <<resyncs>>

\* claim:MasterRoute.Again
MasterRoute_Again ==
    /\ state = "Resync"
    /\ resyncs < 2
    /\ state' = "Paced"
    /\ resyncs' = resyncs + 1

\* claim:MasterRoute.CiGreen
MasterRoute_CiGreen ==
    /\ state = "CiWatch"
    /\ state' = "HitlMerge"
    /\ UNCHANGED <<resyncs>>

\* claim:MasterRoute.Click
MasterRoute_Click ==
    /\ state = "HitlMerge"
    /\ state' = "Merged"
    /\ UNCHANGED <<resyncs>>

\* claim:MasterRoute.Hold
MasterRoute_Hold ==
    /\ state = "Resync"
    /\ resyncs = 2
    /\ state' = "Held"
    /\ resyncs' = 3

\* claim:MasterRoute.Merge
MasterRoute_Merge ==
    /\ state = "Resync"
    /\ state' = "AfkMerge"
    /\ UNCHANGED <<resyncs>>

\* claim:MasterRoute.OneWay
MasterRoute_OneWay ==
    /\ state = "Offered"
    /\ state' = "CiWatch"
    /\ UNCHANGED <<resyncs>>

\* claim:MasterRoute.RealRed
MasterRoute_RealRed ==
    /\ state = "Merged"
    /\ state' = "Reverted"
    /\ UNCHANGED <<resyncs>>

\* claim:MasterRoute.Reland
MasterRoute_Reland ==
    /\ state = "Reverted"
    /\ state' = "Offered"
    /\ UNCHANGED <<resyncs>>

\* claim:MasterRoute.Sequenced
MasterRoute_Sequenced ==
    /\ state = "Held"
    /\ state' = "Paced"
    /\ resyncs' = 0

\* claim:MasterRoute.Turn
MasterRoute_Turn ==
    /\ state = "Paced"
    /\ state' = "Resync"
    /\ UNCHANGED <<resyncs>>

\* claim:MasterRoute.TwoWay
MasterRoute_TwoWay ==
    /\ state = "Offered"
    /\ state' = "Paced"
    /\ UNCHANGED <<resyncs>>

\* claim:MasterRoute.Woken
MasterRoute_Woken ==
    /\ state = "Merged"
    /\ state' = "Closed"
    /\ UNCHANGED <<resyncs>>

\* GAP: R29 says the admiral lands PRs one at a time, not how a turn is signalled
Gap1 ==
    /\ state = "Paced"
    /\ state' = "Resync"
    /\ UNCHANGED <<resyncs>>

Done == state \in Finals /\ UNCHANGED vars

Next ==
    \/ MasterRoute_AfkDone
    \/ MasterRoute_Again
    \/ MasterRoute_CiGreen
    \/ MasterRoute_Click
    \/ MasterRoute_Hold
    \/ MasterRoute_Merge
    \/ MasterRoute_OneWay
    \/ MasterRoute_RealRed
    \/ MasterRoute_Reland
    \/ MasterRoute_Sequenced
    \/ MasterRoute_Turn
    \/ MasterRoute_TwoWay
    \/ MasterRoute_Woken
    \/ Gap1
    \/ Done

Bounds == resyncs <= 3

Spec == Init /\ [][Next]_vars
====
