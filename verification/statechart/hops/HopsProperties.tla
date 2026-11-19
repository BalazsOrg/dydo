---- MODULE HopsProperties ----
EXTENDS Hops

\* Hand-written from the spec; never generated from the claim model (K3).
\* seen is a passive history observer. Every model step has exactly one extension.
VARIABLE seen
PropertyInit == Init /\ seen = {state}
PropertyNext == Next /\ seen' = seen \cup {state'}

\* S3: The reviewed implementation exists before its Review hop.
S3_ImplementBeforeReview == state = "ReviewHop" => "ImplementHop" \in seen

\* S9: Fix rN follows the review that found its blockers.
S9_ReviewBeforeFix == state = "FixHop" => "ReviewHop" \in seen

\* R8, S3: A fix is followed by a fresh Review hop.
R8_FixReturnsToReview == [][((state = "FixHop" /\ state' # state) => state' = "ReviewHop")]_<<vars, seen>>

\* S4, R6: A leaf evidence fix follows its fresh evidence check.
S4_LeafEvidenceFirst == state = "CheckFix" => "EvidenceCheck" \in seen

====
