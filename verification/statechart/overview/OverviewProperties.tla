---- MODULE OverviewProperties ----
EXTENDS Overview

\* Hand-written from the spec; never generated from the claim model (K3).
\* seen is a passive history observer. Every model step has exactly one extension.
VARIABLE seen
PropertyInit == Init /\ seen = {state}
PropertyNext == Next /\ seen' = seen \cup {state'}

\* R2: The nominal merge path follows feature review.
R2_ReviewBeforeMerge == state = "MergeArea" => "ReviewArea" \in seen

\* N5, N6: The nominal master route follows landing validation.
N5_LandingBeforeMaster == state = "MasterArea" => "LandingArea" \in seen

\* K18, N1: The ordinary release update follows tagging.
K18_TagBeforeAdopters == state = "AdopterArea" => "Tagged" \in seen

====

