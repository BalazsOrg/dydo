---- MODULE AdopterProperties ----
EXTENDS Adopter

\* Hand-written from the spec; never generated from the claim model (K3).
\* seen is a passive history observer. Every model step has exactly one extension.
VARIABLE seen
PropertyInit == Init /\ seen = {state}
PropertyNext == Next /\ seen' = seen \cup {state'}

\* K18, N8: Current runtime assets follow the ordinary update.
K18_UpdateBeforeCurrent == state = "Current" => "Updating" \in seen

\* N11: A tailored-file route consults before it rejoins the update.
N11_ConsultTailored == [][((state = "Tailored" /\ state' # state) => state' = "Consulted")]_<<vars, seen>>

====

