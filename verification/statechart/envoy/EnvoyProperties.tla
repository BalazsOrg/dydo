---- MODULE EnvoyProperties ----
EXTENDS Envoy

\* Hand-written from the spec; never generated from the claim model (K3).
\* seen is a passive history observer. Every model step has exactly one extension.
VARIABLE seen
PropertyInit == Init /\ seen = {state}
PropertyNext == Next /\ seen' = seen \cup {state'}

\* H16: The admiral does homework before sending an envoy.
H16_HomeworkBeforeEnvoy == state = "Active" => "Homework" \in seen

\* H16: Only an admiral dispatches the active envoy.
H16_OnlyAdmiralSends == state = "Active" => level = "admiral"

\* H18: An active envoy is counted.
H18_ActiveCount == state = "Active" => envoys = 1

\* H19: The envoy reports before calling the thread complete.
H19_ReportBeforeClose == state = "Closed" => "Reported" \in seen

\* H15, H19: Reporting follows an active interaction.
H19_WorkBeforeReport == state = "Reported" => "Active" \in seen

====
