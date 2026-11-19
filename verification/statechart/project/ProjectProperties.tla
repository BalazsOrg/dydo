---- MODULE ProjectProperties ----
EXTENDS Project

\* Hand-written from the spec; never generated from the claim model (K3).
\* seen is a passive history observer. Every model step has exactly one extension.
VARIABLE seen
PropertyInit == Init /\ seen = {state}
PropertyNext == Next /\ seen' = seen \cup {state'}

\* R9: Replanning follows a working Project.
R9_ReplanFromWorkingProject == state = "Replanning" => "Watching" \in seen

\* N5: The Project reaches landing after its delivery watch.
N5_DeliveryBeforeLanding == state = "Landing" => "Watching" \in seen

====
