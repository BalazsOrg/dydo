---- MODULE MasterRoute ----
EXTENDS Naturals

VARIABLES state, resyncs, oneWay

vars == <<state, resyncs, oneWay>>

Finals == {"Closed"}

Init ==
    /\ state = "Offered"
    /\ resyncs = 0
    /\ oneWay = FALSE

\* claim:MasterRoute.AfkDone
MasterRoute_AfkDone ==
    /\ state = "AfkMerge"
    /\ state' = "Merged"
    /\ UNCHANGED <<resyncs, oneWay>>

\* claim:MasterRoute.Again
MasterRoute_Again ==
    /\ state = "Resync"
    /\ resyncs < 2
    /\ state' = "Paced"
    /\ resyncs' = resyncs + 1
    /\ UNCHANGED <<oneWay>>

\* claim:MasterRoute.AwaitClick
MasterRoute_AwaitClick ==
    /\ state = "Resync"
    /\ oneWay
    /\ state' = "CiWatch"
    /\ UNCHANGED <<resyncs, oneWay>>

\* claim:MasterRoute.CiGreen
MasterRoute_CiGreen ==
    /\ state = "CiWatch"
    /\ state' = "HitlMerge"
    /\ UNCHANGED <<resyncs, oneWay>>

\* claim:MasterRoute.Click
MasterRoute_Click ==
    /\ state = "HitlMerge"
    /\ state' = "Merged"
    /\ UNCHANGED <<resyncs, oneWay>>

\* claim:MasterRoute.Hold
MasterRoute_Hold ==
    /\ state = "Resync"
    /\ resyncs = 2
    /\ state' = "Held"
    /\ resyncs' = 3
    /\ UNCHANGED <<oneWay>>

\* claim:MasterRoute.Merge
MasterRoute_Merge ==
    /\ state = "Resync"
    /\ ~oneWay
    /\ state' = "AfkMerge"
    /\ UNCHANGED <<resyncs, oneWay>>

\* claim:MasterRoute.OneWay
MasterRoute_OneWay ==
    /\ state = "Offered"
    /\ state' = "Paced"
    /\ oneWay' = TRUE
    /\ UNCHANGED <<resyncs>>

\* claim:MasterRoute.RealRed
MasterRoute_RealRed ==
    /\ state = "Merged"
    /\ state' = "Reverted"
    /\ UNCHANGED <<resyncs, oneWay>>

\* claim:MasterRoute.Reland
MasterRoute_Reland ==
    /\ state = "Reverted"
    /\ state' = "Offered"
    /\ UNCHANGED <<resyncs, oneWay>>

\* claim:MasterRoute.Sequenced
MasterRoute_Sequenced ==
    /\ state = "Held"
    /\ state' = "Paced"
    /\ resyncs' = 0
    /\ UNCHANGED <<oneWay>>

\* claim:MasterRoute.Turn
MasterRoute_Turn ==
    /\ state = "Paced"
    /\ state' = "Resync"
    /\ UNCHANGED <<resyncs, oneWay>>

\* claim:MasterRoute.TwoWay
MasterRoute_TwoWay ==
    /\ state = "Offered"
    /\ state' = "Paced"
    /\ oneWay' = FALSE
    /\ UNCHANGED <<resyncs>>

\* claim:MasterRoute.Woken
MasterRoute_Woken ==
    /\ state = "Merged"
    /\ state' = "Closed"
    /\ UNCHANGED <<resyncs, oneWay>>

Done == state \in Finals /\ UNCHANGED vars

Next ==
    \/ MasterRoute_AfkDone
    \/ MasterRoute_Again
    \/ MasterRoute_AwaitClick
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
    \/ Done

Bounds == resyncs <= 3 /\ oneWay \in BOOLEAN

Fairness == TRUE

Spec == Init /\ [][Next]_vars /\ Fairness
====
