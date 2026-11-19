---- MODULE StatechartRunProperties ----
EXTENDS StatechartRun

\* Hand-written from the spec; never generated from the claim model (K3).
\* seen is a passive history observer. Every model step has exactly one extension.
VARIABLE seen
PropertyInit == Init /\ seen = {state}
PropertyNext == Next /\ seen' = seen \cup {state'}

\* N12 DR 062 §1–5: Checking follows ready dependencies or authorized verified continuation.
N12_VerifyBeforeChecking == state = "Checking" => ("Ready" \in seen \/ "Continuing" \in seen)

\* N12 DR 062 §6, K3: A chart result follows a checker run.
N12_CheckBeforeChart == state = "Charted" => "Checking" \in seen

\* N12 DR 062 §5, K20: Denied, unavailable or failed setup never becomes success.
N12_NonSuccessCannotPass == state = "Charted" => "NonSuccess" \notin seen

\* N12 DR 062 §2–4: Continuation originates in a saved approval-required request.
N12_ApprovalBeforeContinue == state = "Continuing" => "ApprovalRequired" \in seen

\* N12 DR 062 §6: Uncertain checker drain retains its checking scratch.
N12_CheckBeforeRetainingScratch == state = "ScratchKept" => "Checking" \in seen

====
