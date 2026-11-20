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

\* R2, R8, R12, R16: Once review has started, every entry to finishing
\* follows a clear round. Only a review round can produce that result.
R8_ClearBeforeFinish ==
    [][ /\ (state' = "Clear" /\ state # "Clear" => state = "InReview")
        /\ (state' = "Finishing" /\ state # "Finishing" /\ "InReview" \in seen =>
            state = "Clear")
    ]_<<vars, seen>>

\* R8, R9, S9, DYD-1295 41a553f1: A blocked round requires a fix before
\* the next round if review continues. R10 governs entry to that Fix in
\* round three. Only a clear verdict permits advancing without a Fix.
R8_FixBeforeNextRound ==
    [][ /\ (round' > round =>
            state \in {"Fixing", "Clear"} /\ state' = "InReview" /\ round' = round + 1)
        /\ (state = "Fixing" /\ state' # "Fixing" =>
            (state' = "InReview" /\ round' = round + 1) \/ state' = "Returned")
    ]_<<vars, seen>>

\* R8: A clear round ends the phase by default. The officer may order
\* one further round after a clear verdict, once in this holder's run.
R8_ExtraRoundOnlyOnceAfterClear ==
    /\ ~extra
    /\ [][ /\ (extra => extra')
           /\ ((~extra /\ extra') => state = "Clear" /\ state' = "InReview")
           /\ (state = "Clear" /\ state' # "Clear" =>
                state' = "Finishing" \/ (state' = "InReview" /\ round' = round + 1 /\ ~extra /\ extra'))
       ]_<<vars, seen>>

\* R10, S10: A blocked third round requires a checkpoint before round four.
\* DR 070 §6 says "Round 3 with a new blocker". R8's extra round after
\* a clear third verdict is separate from the checkpoint's Fix route.
R10_CheckpointBeforeFourthRound ==
    [][ /\ (state # "Fixing" /\ state' = "Fixing" /\ (round = 3 \/ round' = 3) =>
            state = "Checkpoint" /\ round' = round)
        /\ (state' = "InReview" /\ round = 3 /\ round' = 4 /\ state # "Clear" =>
            state = "Fixing" /\ "Checkpoint" \in seen)
    ]_<<vars, seen>>

\* R10, S10: A third-round escalation also records the checkpoint.
S10_CheckpointBeforeThirdRoundEscalation ==
    [][(state \in {"InReview", "Fixing", "Checkpoint"} /\ round = 3 /\ state' = "Returned" =>
        state = "Checkpoint")]_<<vars, seen>>

\* R10, S10: The checkpoint belongs to a blocked third review round.
\* This checks the source phase and number. The blocker itself has no data.
R10_EnterCheckpointFromThirdRound ==
    [][(state # "Checkpoint" /\ state' = "Checkpoint" =>
        state = "InReview" /\ round = 3 /\ round' = round)]_<<vars, seen>>

\* R8, R19, R21, R22: A successful repair needs a fresh review and a clear
\* verdict before retry. The merge-conflict and red-gate resources allow
\* return to the officer for an unresolved repair.
R19_ReviewAfterMergeRepair ==
    [][ /\ (state = "Repairing" /\ state' # "Repairing" =>
            state' \in {"InReview", "Returned"})
        /\ ("Repairing" \in seen /\ state # "Finishing" /\ state' = "Finishing" =>
            state = "Clear" /\ "InReview" \in seen)
    ]_<<vars, seen>>

\* H2, DYD-1295 7b3d2e3e: Returning ends this holder's run.
H2_ReturnedIsTerminal ==
    [][(state = "Returned" => UNCHANGED vars)]_<<vars, seen>>

====
