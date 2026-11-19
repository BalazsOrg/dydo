---- MODULE SafeDeleteProperties ----
EXTENDS SafeDelete

\* Hand-written from the spec; never generated from the claim model (K3).
\* seen is a passive history observer. Every model step has exactly one extension.
VARIABLE seen
PropertyInit == Init /\ seen = {state}
PropertyNext == Next /\ seen' = seen \cup {state'}

\* N10: Deletion purges a previously archived branch/worktree.
N10_ArchiveBeforePurge == state = "Purged" => "Archived" \in seen

\* N10: Structural guard for the purged terminal phase; retention and purge safety remain unmodeled.
Structural_N10_PurgeIsFinal == [][((state = "Purged") => state' = "Purged")]_<<vars, seen>>

====
