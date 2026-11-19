---- MODULE ProofProperties ----
EXTENDS Proof

\* Hand-written from the spec; never generated from the claim model (K3).
\* seen is a passive history observer. Every model step has exactly one extension.
VARIABLE seen
PropertyInit == Init /\ seen = {state}
PropertyNext == Next /\ seen' = seen \cup {state'}

\* T3, T5: A newly created graduating test demonstrates failure without the implementation.
\* T8's kept old tests may have pre-existing red/green evidence, absent from this machine.
T3_RedBeforeGraduation == state = "Graduated" /\ "Old" \notin seen => "Red" \in seen

\* T5, T23: A newly created graduating suite test has passing proof.
T23_GreenBeforeGraduation == state = "Graduated" /\ "Old" \notin seen => "Receipt" \in seen

\* T23, Y4: The green receipt follows checking.
T23_GateBeforeReceipt == state = "Receipt" => "Gated" \in seen

\* T12, Y6: Ordinary proof does not wait on a mutation stage.
T12_NoMutationGate == state \notin {"MutationGate", "MutationPending"}

====
