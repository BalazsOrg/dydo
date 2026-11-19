---- MODULE BackfillProperties ----
EXTENDS Backfill

\* Hand-written from the spec; never generated from the claim model (K3).
\* seen is a passive history observer. Every model step has exactly one extension.
VARIABLE seen
PropertyInit == Init /\ seen = {state}
PropertyNext == Next /\ seen' = seen \cup {state'}

\* L12: The review page follows citation checking.
L12_CitationsBeforePage == [][(state' = "OnPage" /\ state # state') => state = "Checked"]_<<vars, seen>>

\* L12: A row can merge only through his approval page.
L12_PageBeforeMerge == [][(state' = "Merged" /\ state # state') => state = "OnPage"]_<<vars, seen>>

\* L12: A rejected row does not merge in this batch.
L12_RejectedCannotMerge == state = "Merged" => "Rejected" \notin seen

====
