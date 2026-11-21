---- MODULE CaptainProperties ----
EXTENDS Captain

\* Hand-written from the rulings and spec; never generated from the claim model (K3).
\* seen is passive. Every model step has exactly one observer extension.
VARIABLE seen
PropertyInit == Init /\ seen = {state}
PropertyNext == Next /\ seen' = seen \cup {state'}

\* DYD-1295 7b3d2e3e: "It's done when it's contract is met. Period."
\* This checks the work and Finishing phases before Done. Contract satisfaction is unobservable.
WorkPhaseBeforeDone ==
    [][(state' = "Done" /\ state # "Done" =>
        state = "Finishing" /\ seen \cap {"Leaf", "Waiting"} # {})]_<<vars, seen>>

\* H3, DYD-1295 df6a785e: The first move after a split dispatches children
\* and waits. The model has no child status or return receipts.
H3_WaitAfterSplit ==
    [][(state = "Split" /\ state' # state => state' = "Waiting")]_<<vars, seen>>

\* H16, S10, DYD-1295 519126ab. The leaf waits on an answer and resumes
\* in Leaf. Passing up an unsettled Question leaves the leaf waiting.
\* The property checks phase order. The answer itself is unobservable.
H16_ResumeLeafAfterQuestion ==
    [][(state = "Question" /\ state' # state => state' = "Leaf")]_<<vars, seen>>

\* R8: Once review has started, every entry to finishing
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

\* R19 requires fresh review of conflict hunks. R21 and R22 route a real
\* red gate through a Fix and fix-scoped review before re-landing. R8
\* requires a clear verdict. K9 and H12 route escalation to the officer.
R19_ReviewAfterMergeRepair ==
    [][ /\ (state = "Repairing" /\ state' # "Repairing" =>
            state' \in {"InReview", "Returned"})
        /\ ("Repairing" \in seen /\ state # "Finishing" /\ state' = "Finishing" =>
            state = "Clear" /\ "InReview" \in seen)
    ]_<<vars, seen>>

\* H1 and AGENTS.md (Sort yourself) put return at the end of the job.
\* H23 pairs the sub-captain's return line with a closed-thread signal.
\* H12 routes escalations to the officer above.
H1_ReturnedIsTerminal ==
    [][(state = "Returned" => UNCHANGED vars)]_<<vars, seen>>

\* H10, DYD-1372 07c86aad: "one manageable part at a time".
\* Visibility and manageability have no data in this phase model.
H10_WayfindingLeavesByCut ==
    [][(state = "Wayfinding" /\ state' # state => state' = "Split")]_<<vars, seen>>

\* H10, DYD-1372 777d396e, 07c86aad: fog must not skip the part's work.
H10_PartBeforeFogFinish ==
    [][("Wayfinding" \in seen /\ state' \in {"Finishing", "Done"} =>
        {"Split", "Waiting"} \subseteq seen)]_<<vars, seen>>

\* H10: after a returned part, find the next part or proceed with the whole.
H10_ReturnedPartContinues ==
    [][(state = "Waiting" /\ state' # state =>
        state' \in {"Wayfinding", "InReview", "Finishing"})]_<<vars, seen>>

\* R34, DYD-1372 777d396e: "Reviewing the reviews is probably not a good idea."
\* Includes answers: a result with no change has no review/fix/repair loop.
R34_VerdictNeverReviewed ==
    kind = "verdict" =>
        state \notin {"InReview", "Clear", "Checkpoint", "Fixing", "Repairing"}

\* K30, S6: the kind resource is loaded before the holder does its work.
K30_KindKnownBeforeWork ==
    state \notin {"Dispatched", "Returned"} => kind \in {"change", "verdict"}

\* K30's kind belongs to this ticket; S7 and AGENTS.md: one holder, one ticket.
K30_KindStaysKnown ==
    [][(kind # "unread" => kind' = kind)]_<<vars, seen>>

\* S7, S6, AGENTS.md: a refused dispatch returns without starting work.
\* No other holder or ticket identity is observable here.
S7_UnstartedReturnHasNoWork ==
    kind = "unread" => seen \subseteq {"Dispatched", "Returned"}

====
