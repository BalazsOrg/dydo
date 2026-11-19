---- MODULE HostSetupProperties ----
EXTENDS HostSetup

\* Hand-written from the spec; never generated from the claim model (K3).
\* seen is a passive history observer. Every model step has exactly one extension.
VARIABLE seen
PropertyInit == Init /\ seen = {state}
PropertyNext == Next /\ seen' = seen \cup {state'}

\* N12 DR 056 §1: Committed host guard setup precedes guarded calls.
N12_InitBeforeGuard == state = "Guarded" => "Initialised" \in seen

\* N12 DR 055 §2: Guard denial follows an intercepted agent call.
N12_GuardBeforeDenial == state = "MemoryDenied" => "Guarded" \in seen

====
