---- MODULE ClaimBuildProperties ----
EXTENDS ClaimBuild

\* Hand-written from the spec; never generated from the claim model (K3).
\* seen records the current claim edit; each new Edited phase resets its scope.
VARIABLE seen
PropertyInit == Init /\ seen = {state}
PropertyNext == Next /\ seen' = IF state' = "Edited" /\ state # state' THEN {state'} ELSE seen \cup {state'}

\* K3, K20: Claims cannot be approved before successful property checking.
K20_TlcBeforeApproval == state \in {"Approved", "Released"} => "TlcPassed" \in seen

\* K16: Release follows his claims approval.
K16_ApprovalBeforeRelease == state = "Released" => "Approved" \in seen

\* K3, K20: A failed check returns to work rather than approval.
K20_FailureDoesNotFinish == [][((state = "TlcFailed" /\ state' # state) => state' \notin {"InReview", "Approved", "Released"})]_<<vars, seen>>

\* K7, K10, K24: The checked build precedes the final property check.
K7_ChecksBeforeTlc == state = "TlcPassed" => "Checked" \in seen

\* K16, K22 (DYD-1082 555a8300 gap 9): his review of the candidate ends in his approval or a rejection,
\* and a rejected candidate is rebuilt from an edit; it is never released or checked as it stood.
K16_RejectedRebuilt == [][((state = "InReview" /\ state' # state) => state' \in {"Approved", "Edited"})]_<<vars, seen>>

====
