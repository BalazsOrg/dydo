---- MODULE MutationProperties ----
EXTENDS Mutation

\* Hand-written from the spec; never generated from the claim model (K3).
\* seen records this queued campaign; entering Queued starts a new exact-tree scope.
VARIABLE seen
PropertyInit == Init /\ seen = {state}
PropertyNext == Next /\ seen' = IF state' = "Queued" /\ state # state' THEN {state'} ELSE seen \cup {state'}

\* T12: Landing mutation work is queued before execution.
T12_QueueBeforeRun == [][(state' = "Running" /\ state # state') => state = "Queued"]_<<vars, seen>>

\* T12, N15: A clean result follows execution on the landing tree.
T12_RunBeforeClean == state = "Clean" => "Running" \in seen

\* T12: A disputed equivalence first has a claim.
T12_ClaimBeforeDispute == state = "Dispute" => "Claimed" \in seen

====
