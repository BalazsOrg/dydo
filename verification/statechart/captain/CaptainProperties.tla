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

\* R8: "the phase ends on the first round with no new blocker".
\* R2 permits a part to return to its officer for whole review. R23 requires
\* the merging holder's PASS to name its head; a mend is not R24's clean sync.
R8_ClearBeforeFinish ==
    [][ /\ (state' = "Clear" /\ state # "Clear" => state = "InReview")
        /\ (state' = "Finishing" /\ state # "Finishing" /\ "InReview" \in seen =>
            state = "Clear" \/ (state = "Mending" /\ merger = "officer"))
    ]_<<vars, seen>>

\* R8, R9, S9, DYD-1295 41a553f1: A blocked round requires a fix before
\* the next round if review continues. R10 governs entry to that Fix in
\* round three. An own-PR cheap mend requires a new-head round (R23/R24).
R8_FixBeforeNextRound ==
    [][ /\ (round' > round =>
            state \in {"Fixing", "Clear", "Mending"} /\ state' = "InReview" /\ round' = round + 1)
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
                state' \in {"Finishing", "Mending"} \/ (state' = "InReview" /\ round' = round + 1 /\ ~extra /\ extra'))
       ]_<<vars, seen>>

\* R10, S10: A blocked third round requires a checkpoint before round four.
\* DR 070 §6 says "Round 3 with a new blocker". R8's extra round after
\* a clear third verdict, including its cheap mend, is separate from that Fix route.
R10_CheckpointBeforeFourthRound ==
    [][ /\ (state # "Fixing" /\ state' = "Fixing" /\ (round = 3 \/ round' = 3) =>
            state = "Checkpoint" /\ round' = round)
        /\ (state' = "InReview" /\ round = 3 /\ round' = 4 /\ state \notin {"Clear", "Mending"} =>
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

\* R19: "under a fresh review of its hunks". R22: "then a fix-scoped
\* round, then a re-land". R8 requires that review to clear before retry.
\* A merge repair belongs to the holder that merges, not a part handoff.
R19_ReviewAfterMergeRepair ==
    [][ /\ (state = "Repairing" /\ state' # "Repairing" =>
            state' \in {"InReview", "Returned"})
        /\ ("Repairing" \in seen /\ state # "Finishing" /\ state' = "Finishing" =>
            merger = "holder" /\ state = "Clear" /\ "InReview" \in seen)
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

\* H2, H10: after a returned part, find the way, cut again, or proceed.
\* H10 allows finish, find the way and cut, or wait; R2 adds whole review.
\* H16 says the Question captain "researches first, then answers the need";
\* K30/H2 keep the escape hatch: research may resume after its parts return.
\* H2 returns an incoherently divisible slice to its officer for a re-cut.
\* Continuing never returns this holder to an untaken or unread ticket.
H10_ReturnedPartContinues ==
    [][(state = "Waiting" /\ state' # state =>
        state' \in {"Wayfinding", "Split", "InReview", "Finishing"} \/
        (state' = "Leaf" /\ kind = "answer"))]_<<vars, seen>>

\* R34, DYD-1372 777d396e: "Reviewing the reviews is probably not a good idea."
\* R34: "A verdict gets no review." A verdict holder has no review/fix/repair loop.
R34_VerdictNeverReviewed ==
    kind = "verdict" =>
        state \notin {"InReview", "Clear", "Checkpoint", "Fixing", "Repairing", "Mending"}

\* K30, S6: the kind resource is loaded before the holder does its work.
K30_KindKnownBeforeWork ==
    state \notin {"Dispatched", "Returned"} => kind # "unread"

\* K30's kind belongs to this ticket; S7 and AGENTS.md: one holder, one ticket.
K30_KindStaysKnown ==
    [][(kind # "unread" => kind' = kind)]_<<vars, seen>>

\* S7, S6, AGENTS.md: a refused dispatch returns without starting work.
\* No other holder or ticket identity is observable here.
S7_UnstartedReturnHasNoWork ==
    kind = "unread" => seen \subseteq {"Dispatched", "Returned"}


\* R23: "unless the PASS names the head", except R24's clean syncs.
\* Mending changes content, so the holder that merges cannot finish on that
\* stale PASS. R8_ClearBeforeFinish requires a cleared review before finishing.
\* R2's officer handoff is not a merge by this holder.
R23_ReviewMendedHead ==
    [][(state = "Mending" /\ merger = "holder" =>
        state' # "Finishing") ]_<<vars, seen>>

\* R2: "after wiring and before its Merge hop"; R23: "the PASS names the
\* head". Parts handed to an officer are reviewed with the whole (R2).
R2_MergingHolderHasReview ==
    [][(kind = "change" /\ merger = "holder" /\ state' = "Finishing" =>
        "InReview" \in seen /\ state = "Clear") ]_<<vars, seen>>
====
