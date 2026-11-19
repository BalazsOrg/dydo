---- MODULE ProjectProperties ----
EXTENDS Project

\* Hand-written from the spec; never generated from the claim model (K3).
\* seen is a passive history observer. Every model step has exactly one extension.
VARIABLE seen
PropertyInit == Init /\ seen = {state}
PropertyNext == Next /\ seen' = seen \cup {state'}

\* H5: Phase check of the admiral's watch before replanning; contracts are unmodeled.
H5_WatchBeforeReplan == state = "Replanning" => "Watching" \in seen

\* H5: Phase check of the watch before landing handoff; delivery completion is unmodeled.
H5_WatchBeforeLanding == state = "Landing" => "Watching" \in seen

====
