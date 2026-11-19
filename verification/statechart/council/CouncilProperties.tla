---- MODULE CouncilProperties ----
EXTENDS Council

\* Hand-written from the spec; never generated from the claim model (K3).
\* seen is a passive history observer. Every model step has exactly one extension.
VARIABLE seen
PropertyInit == Init /\ seen = {state}
PropertyNext == Next /\ seen' = seen \cup {state'}

\* L14: Every sealed answer precedes synthesis.
L14_AnswersBeforeSynthesis == state = "Synthesised" => "Answered" \in seen

\* L14, L22: Family waiting cannot skip the sealed answers.
L22_WaitDoesNotDecide == [][((state = "Waiting" /\ state' # state) => state' # "Synthesised")]_<<vars, seen>>

====
