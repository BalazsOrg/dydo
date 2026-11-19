---- MODULE DocsProperties ----
EXTENDS Docs

\* Hand-written from the spec; never generated from the claim model (K3).
\* seen is a passive history observer. Every model step has exactly one extension.
VARIABLE seen
PropertyInit == Init /\ seen = {state}
PropertyNext == Next /\ seen' = seen \cup {state'}

\* D3, D5: A new doc satisfies admission before declaring explained code.
D3_AdmitBeforeDeclare == state = "Declared" => ("Admitted" \in seen \/ "Grandfathered" \in seen)

\* D4: A new doc is invited before admission.
D4_InviteNewDoc == state = "Admitted" => ("Invited" \in seen \/ "Grandfathered" \in seen)

\* D5: Change warnings operate on declared explained paths.
D5_DeclareBeforeFlag == state = "Flagged" => "Declared" \in seen

\* D6, D7: A found problem returns to repair or is cut.
D7_DriftFixedOrCut == [][((state = "Finding" /\ state' # state) => state' \in {"Declared", "Admitted", "CutDoc"})]_<<vars, seen>>

====

