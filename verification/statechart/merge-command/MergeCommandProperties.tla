---- MODULE MergeCommandProperties ----
EXTENDS MergeCommand

\* Hand-written from the spec; never generated from the claim model (K3).
\* seen is a passive history observer. Every model step has exactly one extension.
VARIABLE seen
PropertyInit == Init /\ seen = {state}
PropertyNext == Next /\ seen' = seen \cup {state'}

\* R23: Structural guard: this refused invocation is terminal; it does not validate refusal conditions.
Structural_R23_RefusalIsTerminal == [][((state = "Refused") => state' = "Refused")]_<<vars, seen>>

\* R19, R21: Structural guard: this conflict invocation ends; resolution is a separate operation.
Structural_R21_ConflictIsTerminal == [][((state = "Conflict") => state' = "Conflict")]_<<vars, seen>>

\* R23: Structural guard: the completed invocation retains its result; merge form is not checked.
Structural_R23_MergeResultStable == [][((state = "Merged") => state' = "Merged")]_<<vars, seen>>

====
