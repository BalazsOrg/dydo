---- MODULE FeatureMergeProperties ----
EXTENDS FeatureMerge

\* Hand-written from the spec; never generated from the claim model (K3).
\* seen is a passive history observer. Every model step has exactly one extension.
VARIABLE seen
PropertyInit == Init /\ seen = {state}
PropertyNext == Next /\ seen' = seen \cup {state'}

\* R20: Merge Done follows merge-tree gates and CI checking.
R20_GatesBeforeDone == state = "MergeDone" => "Gating" \in seen

\* R19: A conflict resolution is freshly reviewed before merge resumes.
R19_ReviewHunks == [][((state = "Resolving" /\ state' # state) => state' = "HunkReview")]_<<vars, seen>>

\* R22: A real red is rerun once before the immediate revert.
R22_RerunBeforeRevert == state = "Reverted" => "Rerun" \in seen

\* R22: A reverted merge goes to fix and fix-scoped review.
R22_FixAfterRevert == [][((state = "Reverted" /\ state' # state) => state' = "FixRound")]_<<vars, seen>>

\* R18, R20: Ordinary feature merges have no landing merge-confidence phase.
R18_NoMergeReview == state \notin {"MergeReview", "MergeConfidence"}

====

