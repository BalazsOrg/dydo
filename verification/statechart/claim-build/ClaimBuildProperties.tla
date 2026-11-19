---- MODULE ClaimBuildProperties ----
EXTENDS ClaimBuild

\* Hand-written from the spec; never generated from the claim model (K3).
\* seen is a passive history observer. Every model step has exactly one extension.
VARIABLE seen
PropertyInit == Init /\ seen = {state}
PropertyNext == Next /\ seen' = seen \cup {state'}

\* K3, K20: Claims cannot be approved before successful property checking.
K20_TlcBeforeApproval == state \in {"Approved", "Released"} => "TlcPassed" \in seen

\* K16: Release follows his claims approval.
K16_ApprovalBeforeRelease == state = "Released" => "Approved" \in seen

\* K3, K20: A failed check returns to work rather than approval.
K20_FailureDoesNotFinish == [][((state = "TlcFailed" /\ state' # state) => state' \notin {"InReview", "Approved", "Released"})]_<<vars, seen>>

\* K7, K10, K24: The checked build precedes the final property check.
K7_ChecksBeforeTlc == state = "TlcPassed" => "Checked" \in seen

====

