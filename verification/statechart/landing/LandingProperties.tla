---- MODULE LandingProperties ----
EXTENDS Landing

\* Hand-written from the spec; never generated from the claim model (K3).
\* seen is a passive history observer. Every model step has exactly one extension.
VARIABLE seen
PropertyInit == Init /\ seen = {state}
PropertyNext == Next /\ seen' = seen \cup {state'}

\* N5: A slice's Break lens follows Read.
N5_ReadBeforeBreak == [][((state' = "BreakPhase" /\ state # state') => state = "ReadPhase")]_<<vars, seen>>

\* N5: A slice's Drive lens follows Break.
N5_BreakBeforeDrive == [][((state' = "DrivePhase" /\ state # state') => state = "BreakPhase")]_<<vars, seen>>

\* N5: A new inquisition round starts from a fresh feature map.
N5_FreshMapAfterFix == [][((state = "RoundFixes" /\ fixFrom = "inquisition" /\ state' # state) => state' = "Mapped")]_<<vars, seen>>

\* N7, N15, T12, N6: The counted prerequisites are complete from merge review through tagging.
N7_PrerequisitesBeforeLanding == state \in {"MergeReview", "Walkthrough", "Merging", "Landed", "Tagged"} => prereqs = 10

\* N6: His walkthrough follows the landing merge review.
N6_ReviewBeforeWalkthrough == [][(state' = "Walkthrough" /\ state # state') => state = "MergeReview"]_<<vars, seen>>

\* N6: The accepted candidate walkthrough precedes merging.
N6_AcceptanceBeforeMerge == [][((state' = "Merging" /\ state # state') => state = "Walkthrough")]_<<vars, seen>>

\* N2: Release tagging waits for guard hardening.
N2_HardeningBeforeTag == state = "Tagged" => hardened

\* N2, N15, T12: The release tag follows landing and its prerequisites; tagger identity is unmodeled.
N15_LandingBeforeTag == [][(state' = "Tagged" /\ state # state') => state = "Landed"]_<<vars, seen>>

\* N6 (DYD-1082 555a8300 gap 5): a landing walkthrough he does not accept never merges; its findings become
\* tickets, a fix captain re-cuts the branch, and another landing follows from its start. Tickets are unmodeled.
N6_NotAcceptedLandsAgain == [][((state = "NotAccepted" /\ state' # state) => state' = "Integrated")]_<<vars, seen>>

====
