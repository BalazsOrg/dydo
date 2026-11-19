---- MODULE MergeCommandProperties ----
EXTENDS MergeCommand

\* Hand-written from the spec; never generated from the claim model (K3).
\* seen is a passive history observer. Every model step has exactly one extension.
VARIABLE seen
PropertyInit == Init /\ seen = {state}
PropertyNext == Next /\ seen' = seen \cup {state'}

\* R23: A refused invocation cannot subsequently create a merge.
R23_RefusalIsTerminal == [][((state = "Refused") => state' = "Refused")]_<<vars, seen>>

\* R19, R21: This invocation returns conflict; resolution uses the ticket branch.
R21_ConflictIsTerminal == [][((state = "Conflict") => state' = "Conflict")]_<<vars, seen>>

\* R23, R32: A completed merge invocation retains its result.
R32_MergeResultStable == [][((state = "Merged") => state' = "Merged")]_<<vars, seen>>

====

