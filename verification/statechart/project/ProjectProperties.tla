---- MODULE ProjectProperties ----
EXTENDS Project

\* Hand-written from the spec; never generated from the claim model (K3).
\* seen is a passive history observer. Every model step has exactly one extension.
VARIABLE seen
PropertyInit == Init /\ seen = {state}
PropertyNext == Next /\ seen' = seen \cup {state'}

\* H5: Phase check of the coordinator's watch before replanning; contracts are unmodeled.
H5_WatchBeforeReplan == state = "Replanning" => "Watching" \in seen

\* H5: Phase check of the watch before landing handoff; delivery completion is unmodeled.
H5_WatchBeforeLanding == state = "Landing" => "Watching" \in seen

\* H4 (DYD-1082 555a8300 gaps 1-2): a Project with no live coordinator, not started or idle, moves only
\* when he charts it (HITL, Q21) or when he or the chief of staff starts or wakes its coordinator into the
\* watch. Paused status and running agents, which make it idle, are unmodeled.
H4_UnheldOnlyChartedOrWoken == [][((state = "Unheld" /\ state' # state) => state' \in {"Charting", "Watching"})]_<<vars, seen>>

====
