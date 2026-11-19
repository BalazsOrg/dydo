---- MODULE FeatureMergeProperties ----
EXTENDS FeatureMerge

\* Hand-written from the spec; never generated from the claim model (K3).
\* seen records the current merge attempt; entering PreGate resets the scope.
VARIABLE seen
PropertyInit == Init /\ seen = {state}
PropertyNext == Next /\ seen' = IF state' = "PreGate" /\ state # state' THEN {state'} ELSE seen \cup {state'}

\* R20: Merge Done follows merge-tree gates and CI checking.
R20_GatesBeforeDone == [][(state' = "MergeDone" /\ state # state') => state \in {"Gating", "Rerun"} /\ "Gating" \in seen]_<<vars, seen>>

\* R19: A conflict resolution is freshly reviewed before merge resumes.
R19_ReviewHunks == [][((state = "Resolving" /\ state' # state) => state' = "HunkReview")]_<<vars, seen>>

\* R22: A real red is rerun once before the immediate revert.
R22_RerunBeforeRevert == [][(state' = "Reverted" /\ state # state') => state = "Rerun"]_<<vars, seen>>

\* R22: A reverted merge goes to fix and fix-scoped review.
R22_FixAfterRevert == [][((state = "Reverted" /\ state' # state) => state' = "FixRound")]_<<vars, seen>>

\* R18, R20: Structural guard against introducing named merge-review phases; no review data is modeled.
Structural_R18_NoMergeReview == state \notin {"MergeReview", "MergeConfidence"}

====
