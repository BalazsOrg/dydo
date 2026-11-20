---- MODULE MasterRouteProperties ----
EXTENDS MasterRoute

\* Hand-written from the spec; never generated from the claim model (K3).
\* seen records this offered attempt; each re-offer after a revert resets its scope.
VARIABLE seen
PropertyInit == Init /\ seen = {state}
PropertyNext == Next /\ seen' = IF state' = "Offered" /\ state # state' THEN {state'} ELSE seen \cup {state'}

\* R26, R27: An irreversible PR cannot take the AFK route.
R26_TwoWayAfk == state = "AfkMerge" => ~oneWay

\* R27, R31: The one-way path is the HITL path.
R31_OneWayHitl == state = "HitlMerge" => oneWay

\* R31: A HITL Merge opens only after the captain's CI watch.
R31_CiBeforeClick == [][(state' = "HitlMerge" /\ state # state') => state = "CiWatch"]_<<vars, seen>>

\* R29: Each attempt enters its AFK/CI route from the recheck phase; actual tip/tree freshness is unmodeled.
R29_RecheckBeforeMerge == [][(state' \in {"AfkMerge", "CiWatch"} /\ state # state') => state = "Resync"]_<<vars, seen>>

\* R29: The third forced resync is held for the coordinator's sequencing.
R29_HoldThirdResync == [][((resyncs' = 3 /\ resyncs < 3) => state' = "Held")]_<<vars, seen>>

\* R31: The captain closes its master Merge after observing the merge.
R31_MergeBeforeClose == [][(state' = "Closed" /\ state # state') => state = "Merged"]_<<vars, seen>>

====
