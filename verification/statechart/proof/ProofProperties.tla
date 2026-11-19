---- MODULE ProofProperties ----
EXTENDS Proof

\* Hand-written from the spec; never generated from the claim model (K3).
\* seen is a passive history observer. Every model step has exactly one extension.
VARIABLE seen
PropertyInit == Init /\ seen = {state}
PropertyNext == Next /\ seen' = seen \cup {state'}

\* T3, T5: A newly created graduating test demonstrates failure without the implementation.
\* T8: a kept test is measured, not newly graduated; rewritten old tests still qualify here.
T3_RedBeforeGraduation == state = "Graduated" /\ ("Written" \in seen \/ "Conformance" \in seen) => "Red" \in seen

\* T5, T23: A newly created graduating suite test has passing proof.
T23_GreenBeforeGraduation == state = "Graduated" /\ ("Written" \in seen \/ "Conformance" \in seen) => "Receipt" \in seen

\* T23, Y4: The green receipt follows checking.
T23_GateBeforeReceipt == [][(state' = "Receipt" /\ state # state') => state = "Gated"]_<<vars, seen>>

\* T12, Y6: Structural guard against introducing named mutation-gate phases; no mutation data is modeled.
Structural_T12_NoMutationGate == state \notin {"MutationGate", "MutationPending"}

====
