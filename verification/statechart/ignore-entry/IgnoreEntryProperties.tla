---- MODULE IgnoreEntryProperties ----
EXTENDS IgnoreEntry

\* Hand-written from the spec; never generated from the claim model (K3).
\* seen is a passive history observer. Every model step has exactly one extension.
VARIABLE seen
PropertyInit == Init /\ seen = {state}
PropertyNext == Next /\ seen' = seen \cup {state'}

\* T18: Authored exceptions go through their own ticket first.
T18_TicketBeforeCouncil == state = "CouncilB" => "OwnTicket" \in seen

\* T17, T18: An ignore entry follows generated-path review or the authored council route.
T17_ReviewedPathBeforeIgnore == state = "Ignored" => ("PathA" \in seen \/ "CouncilB" \in seen)

\* T17: A stale entry was previously added.
T17_EntryBeforeStaleness == state = "Stale" => "Ignored" \in seen

====

