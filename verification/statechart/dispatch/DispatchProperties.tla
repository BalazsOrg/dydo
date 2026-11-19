---- MODULE DispatchProperties ----
EXTENDS Dispatch

\* Hand-written from the spec; never generated from the claim model (K3).
\* seen is a passive history observer. Every model step has exactly one extension.
VARIABLE seen
PropertyInit == Init /\ seen = {state}
PropertyNext == Next /\ seen' = seen \cup {state'}

\* K14, H3: Crew begin after the dispatch brief.
K14_BriefBeforeWork == state = "Working" => "Briefed" \in seen

\* H1, K14: A normal return follows the crew job.
H1_WorkBeforeReturn == state = "Returned" => "Working" \in seen

\* H2: Oversized crew work returns for a fresh cut and brief.
H2_RecutBeforeNewBrief == [][((state = "ReCut" /\ state' # state) => state' \in {"Staffing", "Briefed"})]_<<vars, seen>>

====

