---- MODULE RoleEvaluationProperties ----
EXTENDS RoleEvaluation

\* Hand-written from the spec; never generated from the claim model (K3).
\* seen is a passive history observer. Every model step has exactly one extension.
VARIABLE seen
PropertyInit == Init /\ seen = {state}
PropertyNext == Next /\ seen' = seen \cup {state'}

\* K22: Only a kept survivor is rewritten fresh.
K22_SurvivorBeforeRewrite == state = "WrittenFresh" => "Kept" \in seen

\* K16, K22: Approval concerns a fresh survivor, fold or removal.
K22_OutcomeBeforeApproval == state = "Approved" => ("WrittenFresh" \in seen \/ "Folded" \in seen \/ "Removed" \in seen)

====
