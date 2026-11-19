---- MODULE ShouldProperties ----
EXTENDS Should

\* Hand-written from the spec; never generated from the claim model (K3).
\* seen is a passive history observer. Every model step has exactly one extension.
VARIABLE seen
PropertyInit == Init /\ seen = {state}
PropertyNext == Next /\ seen' = seen \cup {state'}

\* S17, S18: A new should is registered before its disposition.
S17_RegisterBeforeDisposition == state \in {"Done", "Duplicate", "Canceled", "Moved"} => "Open" \in seen

\* S17: An already-open duplicate does not create another should here.
S17_KnownNotRegisteredAgain == [][((state = "Known") => state' = "Known")]_<<vars, seen>>

====

