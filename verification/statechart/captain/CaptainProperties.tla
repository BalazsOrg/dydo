---- MODULE CaptainProperties ----
EXTENDS Captain

\* Hand-written from the rulings and spec; never generated from the claim model (K3).
\* seen is passive. Every model step has exactly one observer extension.
VARIABLE seen
PropertyInit == Init /\ seen = {state}
PropertyNext == Next /\ seen' = seen \cup {state'}

\* S6, DYD-1295 7b3d2e3e: Closing Done follows work and finishing.
\* The model can test phase order; it has no predicate for meeting the contract.
S6_WorkBeforeDone ==
    [][(state' = "Done" /\ state # "Done" =>
        state = "Finishing" /\ seen \cap {"Leaf", "Waiting"} # {})]_<<vars, seen>>

\* H3, DYD-1295 df6a785e: The first move after a split dispatches children
\* and waits. The model has no child status or return receipts.
H3_WaitAfterSplit ==
    [][(state = "Split" /\ state' # state => state' = "Waiting")]_<<vars, seen>>

\* H16, S10, DYD-1295 519126ab: A leaf suspended on a Question resumes
\* the leaf, with no shortcut out to finishing. Only leaf Questions are modelled.
H16_ResumeLeafAfterQuestion ==
    [][(state = "Question" /\ state' # state => state' = "Leaf")]_<<vars, seen>>

\* R2, R8, R12, R16: The first finish after dispatching review follows a
\* clear round. In this chart InReview -> Finishing represents that result.
R8_ClearBeforeFirstFinish ==
    [][(state' = "Finishing" /\ "InReview" \in seen /\ "Finishing" \notin seen =>
        state = "InReview")]_<<vars, seen>>

\* R8, R9, S9, DYD-1295 41a553f1: The next numbered round follows a fix.
\* Continuing from a fix must go to the fresh round before finishing.
R8_FixBeforeNextRound ==
    [][((round' > round \/ (state = "Fixing" /\ state' \in {"InReview", "Finishing", "Done"})) =>
        state = "Fixing" /\ state' = "InReview" /\ round' = round + 1)]_<<vars, seen>>

\* R10, S10: A blocked third round requires a checkpoint before round four.
\* Checkpoint is the required observation. The generated model has no such phase.
R10_CheckpointBeforeFourthRound ==
    [][(state' = "InReview" /\ round = 3 /\ round' = 4 =>
        "Checkpoint" \in seen)]_<<vars, seen>>

\* R10, S10: A third-round escalation also records the checkpoint.
S10_CheckpointBeforeThirdRoundEscalation ==
    [][(state = "InReview" /\ round = 3 /\ state' = "Returned" =>
        "Checkpoint" \in seen)]_<<vars, seen>>

\* R19, R21: A conflict repair needs fresh review before finishing. The
\* chart puts conflicts and real red gates on the same Finishing -> Waiting
\* edge. That repair wait cannot use the grouping-only shortcut.
R19_ReviewAfterMergeRepair ==
    [][(state = "Waiting" /\ "Finishing" \in seen =>
        state' \notin {"Finishing", "Done"})]_<<vars, seen>>

\* H2, DYD-1295 7b3d2e3e: Returning ends this holder's run.
H2_ReturnedIsTerminal ==
    [][(state = "Returned" => UNCHANGED vars)]_<<vars, seen>>

====
