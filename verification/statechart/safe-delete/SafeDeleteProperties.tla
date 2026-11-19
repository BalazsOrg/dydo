---- MODULE SafeDeleteProperties ----
EXTENDS SafeDelete

\* Hand-written from the spec; never generated from the claim model (K3).
\* seen is a passive history observer. Every model step has exactly one extension.
VARIABLE seen
PropertyInit == Init /\ seen = {state}
PropertyNext == Next /\ seen' = seen \cup {state'}

\* N10: Deletion purges a previously archived branch/worktree.
N10_ArchiveBeforePurge == state = "Purged" => "Archived" \in seen

\* N10: Purged data is no longer restorable in this archive lifetime.
N10_PurgeIsFinal == [][((state = "Purged") => state' = "Purged")]_<<vars, seen>>

====

