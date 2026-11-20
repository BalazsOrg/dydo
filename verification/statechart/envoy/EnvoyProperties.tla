---- MODULE EnvoyProperties ----
EXTENDS Envoy

\* Hand-written from the spec; never generated from the claim model (K3).
\* seen is a passive history observer. Every model step has exactly one extension.
VARIABLE seen
PropertyInit == Init /\ seen = {state}
PropertyNext == Next /\ seen' = seen \cup {state'}

\* H16, L18: Every Question starts with research, so no envoy goes out before a research Question
\* (DYD-1195 317eb49f, 3a854e37; supersedes "the officer above does the homework first").
H16_ResearchBeforeEnvoy == state = "Active" => "Research" \in seen

\* H16: Only the coordinator dispatches envoys, from its HITL queue.
H16_OnlyCoordinatorSends == state = "Active" => level = "coordinator"

\* H23, L18: A council or his-call child is opened only after its research leaves the call unresolved.
H23_ChildAfterResearch == state \in {"Council", "Queued"} => "Research" \in seen

\* H16: An obvious, reversible call is decided where it arises and never becomes a ticket.
H16_ObviousNeverTicketed == state = "Decided" => seen \cap {"Research", "Council", "Queued", "Active", "Reported", "Closed"} = {}

\* H5, H16: The coordinator decides no call itself; an obvious one is decided by the captain where it arises.
H5_CoordinatorDecidesNone == state = "Decided" => level = "captain"

\* H18: The active-phase counter is one; actual envoy identities/cardinality are unmodeled.
H18_ActiveCounter == state = "Active" => envoys = 1

\* H18: Only his closing a reported envoy's thread frees the queue for the next envoy; there is
\* no "moved on" exit (DYD-1195 893e1f5b: "I will close it.").
H18_OnlyHisCloseFrees == [][(envoys = 1 /\ envoys' = 0) => (state = "Reported" /\ state' = "Closed")]_<<vars, seen>>

\* H19: The envoy reports before calling the thread complete.
H19_ReportBeforeClose == state = "Closed" => "Reported" \in seen

\* H15, H19: Reporting follows an active interaction.
H19_WorkBeforeReport == state = "Reported" => "Active" \in seen

====
