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

\* K22, K16 (DYD-1082 555a8300 gap 9; DYD-1195 8661b8f4): a fold or rewrite he rejects returns as a change,
\* and the role is questioned again from the start; the new captains and tickets it goes to are unmodeled.
K22_RejectedQuestionedAgain == [][((state = "Changed" /\ state' # state) => state' = "Questioned")]_<<vars, seen>>

====
