---- MODULE MasterRouteProperties ----
EXTENDS MasterRoute

\* Hand-written from the spec; never generated from the claim model (K3).
\* seen is a passive history observer. Every model step has exactly one extension.
VARIABLE seen
PropertyInit == Init /\ seen = {state}
PropertyNext == Next /\ seen' = seen \cup {state'}

\* R26, R27: An irreversible PR cannot take the AFK route.
R26_TwoWayAfk == state = "AfkMerge" => ~oneWay

\* R27, R31: The one-way path is the HITL path.
R31_OneWayHitl == state = "HitlMerge" => oneWay

\* R31: A HITL Merge opens only after the captain's CI watch.
R31_CiBeforeClick == state = "HitlMerge" => "CiWatch" \in seen

\* R29: Landing either door follows current-master rechecking.
R29_RecheckBeforeMerge == state \in {"AfkMerge", "CiWatch", "HitlMerge", "Merged", "Closed"} => "Resync" \in seen

\* R29: The third forced resync is held for admiral sequencing.
R29_HoldThirdResync == [][((resyncs' = 3 /\ resyncs < 3) => state' = "Held")]_<<vars, seen>>

\* R20, R31: A completed Master Merge follows an actual merge.
R20_MergeBeforeClose == state = "Closed" => "Merged" \in seen

====

