---- MODULE MapProperties ----
EXTENDS Map

\* Hand-written from the spec; never generated from the claim model (K3).
\* seen is a passive history observer. Every model step has exactly one extension.
VARIABLE seen
PropertyInit == Init /\ seen = {state}
PropertyNext == Next /\ seen' = seen \cup {state'}

\* S26: The displayed current map follows a fresh Linear read.
S26_ReadBeforeFreshDisplay == state = "Shown" => "Fresh" \in seen

\* S26: A revisit snapshot does not become current without a fresh read.
S26_SnapshotNeedsFreshRead == [][((state = "Snapshot" /\ state' # state) => state' = "Fresh")]_<<vars, seen>>

\* S19, S26: Expanded current data follows a fresh read.
S19_FreshBeforeExpand == state = "Expanded" => "Fresh" \in seen

====

