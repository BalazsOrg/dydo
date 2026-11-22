---- MODULE CaptainProperties ----
EXTENDS Captain

\* Hand-written from his recorded words, the spec and the core page (dydo/core.md), never from the
\* model. A property says what a source requires; when the model breaks one, the model is wrong.
\* The model is the comprehensive one (DYD-1492); the properties PR 385 dropped are back.
\* seen is passive: the set of states this captain has been in. Every model step has exactly one
\* observer extension, no step is disabled and no fairness is added.
VARIABLE seen
PropertyInit == Init /\ seen = {state}
PropertyNext == Next /\ seen' = seen \cup {state'}

\* Where a review round runs or its verdict is acted on, where the merge runs, and where a captain
\* splits.
ReviewPhase == {"InReview", "Clear", "Checkpoint", "Fixing", "Mending", "Repairing"}
MergePhase == {"Merging", "AwaitingTurn", "Rerunning", "AwaitingClick"}
SplitPhase == {"Wayfinding", "Cutting", "Waiting"}

\* ---- The ticket ----

\* core "A ticket": "it is done when its contract is met". DYD-1295 7b3d2e3e.
\* Checked as order only: Done comes after the work, its own or its parts'.
WorkPhaseBeforeDone ==
    [][(state' = "Done" /\ state # "Done" => seen \cap {"Leaf", "Waiting"} # {})]_<<vars, seen>>

\* K30, S6: what the ticket's result is, is known before the holder works.
K30_KindKnownBeforeWork ==
    state \notin {"Dispatched", "HandedBack"} => kind # "unread"

\* K30, S7 and AGENTS.md: one holder, one ticket. Its kind does not change under it.
K30_KindStaysKnown ==
    [][(kind # "unread" => kind' = kind)]_<<vars, seen>>

\* S7, S6: a ticket that cannot be taken goes back with no work done on it.
S7_UnstartedReturnHasNoWork ==
    kind = "unread" => seen \subseteq {"Dispatched", "HandedBack"}

\* H11: a filler is "a Bug or small Feature", so a filler that is taken is a change.
H11_FillerIsAChange ==
    [][(filler /\ state = "Dispatched" /\ state' = "Taken" => kind' = "change")]_<<vars, seen>>

\* AGENTS.md "Sort yourself": crew does its one job and returns. H12: what goes up goes to the
\* officer above. Both ends are final: nothing follows Done or a hand-back. A structural check of
\* the two ends; the sources say a captain returns, not what it may do after.
H1_EndsAreFinal ==
    [][(state \in {"Done", "HandedBack"} => UNCHANGED vars)]_<<vars, seen>>

\* ---- Splitting and the way to the spec ----

\* core "A ticket": a captain "either does the work itself, with native helpers, or splits the
\* ticket and only orchestrates". Once it has cut, or begun to find the way, it never goes back to
\* doing the work itself.
Core_SplitOnlyOrchestrates ==
    seen \cap {"Wayfinding", "Cutting"} # {} => state \notin {"Leaf", "Asking"}

\* H3, DYD-1295 df6a785e: the first move after a cut dispatches the parts and waits. H2: a slice
\* that cannot be divided coherently under its own ticket goes back to the officer above.
H3_WaitAfterSplit ==
    [][(state = "Cutting" /\ state' # state => state' \in {"Waiting", "HandedBack"})]_<<vars, seen>>

\* H10: "the route is found one manageable part at a time": the parts in sight are cut, and the
\* fog is left uncut. Finding the way leads only to a cut.
H10_WayfindingLeavesByCut ==
    [][(state = "Wayfinding" /\ state' # state => state' = "Cutting")]_<<vars, seen>>

\* H2, H10, R2: after a part returns the captain cuts the next parts, finds the way again where fog
\* remains, sends the whole to review, or finishes; H2 sends a change to its contract through the
\* officer above. It never returns to an untaken ticket or to doing the work itself.
H10_ReturnedPartContinues ==
    [][(state = "Waiting" /\ state' # state =>
        state' \in {"Cutting", "Wayfinding", "InReview", "Done", "HandedBack"})]_<<vars, seen>>

\* R34: a seat from another family is a leaf. "They're not meant do delegate into further sub
\* agents for sure." A seat never cuts, finds a way, or opens a Question.
R34_SeatNeverDelegates ==
    kind = "findings" => state \notin SplitPhase \cup {"Asking"}

\* ---- Questions ----

\* core "Who decides what": a call a captain may not decide becomes a Question for a fresh agent.
\* H16, DYD-1295 519126ab: the work waits on the answer and then goes on; it never ends from the wait.
H16_ResumeLeafAfterQuestion ==
    [][(state = "Asking" /\ state' # state => state' = "Leaf")]_<<vars, seen>>

\* H23: "Every Question carries a stage label, set by whoever opens it." A captain that waits on a
\* Question knows its stage, and a captain that waits on none has none.
H23_AQuestionHasAStage ==
    (state = "Asking") <=> (stage # "none")

\* L18: "every elevated call is a Question ticket, and every Question starts with research". H23:
\* "A research captain opens a council or his-call child only when its research does not resolve
\* the call." So only a Question's own captain opens a council or a his-call Question.
L18_EveryQuestionStartsWithResearch ==
    stage \in {"council", "his-call"} => kind = "answer"

\* ---- The review loop ----

\* core "Review severities": "A verdict is not reviewed again." R34, DYD-1372 777d396e. A verdict's
\* holder, and a seat's, is never in the review, fix and merge loop.
R34_VerdictNeverReviewed ==
    kind \in {"verdict", "findings"} => state \notin ReviewPhase \cup MergePhase

\* positions.md: only a captain that merges a change gets the review, fix and merge part. This
\* checks that table against the model; it is the table's rule, not a rule of the spec.
Sees_OnlyAMergerSeesReviewFixMerge ==
    state \in ReviewPhase \cup MergePhase => kind = "change" /\ merges

\* core: "It never judges its own work: the verdict is another agent's." R2, R23: the holder that
\* merges a change closes only after a review, from its merge.
R2_MergingHolderHasReview ==
    [][(merges /\ state' = "Done" /\ state # "Done" =>
        "InReview" \in seen /\ state \in MergePhase)]_<<vars, seen>>

\* R8, R23: a merge needs a PASS. The merge starts only after a round whose verdict is PASS, and a
\* reviewed ticket closes only from its merge.
R8_PassBeforeMerge ==
    [][ /\ (state' \in MergePhase /\ state \notin MergePhase => state = "Clear")
        /\ (state' = "Clear" /\ state # "Clear" => state = "InReview")
        /\ (state' = "Done" /\ state # "Done" /\ "InReview" \in seen => state \in MergePhase)
    ]_<<vars, seen>>

\* core "Review severities": a blocker "needs fixing now". R8: "a fix between every two rounds",
\* "the phase ends on the first round with no new blocker", and the officer "may still order one
\* more". A later round comes from a Fix, a repair, a mend, or the one more round after a PASS; a Fix
\* leads to a round or back to the officer, never straight to the merge.
R8_FixBeforeNextRound ==
    [][ /\ (state' = "InReview" /\ state # "InReview" /\ "InReview" \in seen =>
            state \in {"Fixing", "Repairing", "Mending", "Clear"})
        /\ (state = "Fixing" /\ state' # state => state' \in {"InReview", "HandedBack"})
    ]_<<vars, seen>>

\* R8: "the phase ends on the first round with no new blocker", and "the officer may still order
\* one more". S17: a cheap should is fixed in place before the parent merges. After a PASS comes the
\* merge, the one more round, or the mend; never a hand-back or a Fix.
R8_PassEndsTheRounds ==
    [][(state = "Clear" /\ state' # state =>
        state' \in {"Merging", "AwaitingTurn", "InReview", "Mending"})]_<<vars, seen>>

\* R8: "a round with a blocker is always followed by a full-lens round". R9: after a security hole,
\* the next round has the security lens first.
R8_FullLensAfterBlocker ==
    [][(state = "Fixing" /\ state' = "InReview" => lens' \in {"full", "security first"})]_<<vars, seen>>

\* R8: "the officer may still order one more". One more round after a PASS is ordered once.
R8_ExtraRoundOnlyOnce ==
    [][ /\ (state = "Clear" /\ state' = "InReview" => ~extra /\ extra')
        /\ (extra => extra')
    ]_<<vars, seen>>

\* R10: "After round 3 comes a checkpoint, not a stop." A third round that has a blocker goes to the
\* checkpoint, and only a third round does.
R10_CheckpointAfterThirdFail ==
    [][ /\ (state = "InReview" /\ round = 3 /\ state' # state => state' \in {"Clear", "Checkpoint"})
        /\ (state' = "Checkpoint" /\ state # "Checkpoint" => state = "InReview" /\ round = 3)
    ]_<<vars, seen>>

\* R10: "by default one more round, escalating only for a fundamental break, a new requirement, or
\* something that needs him." S10: the checkpoint line says which. Its two ways out are the Fix and
\* the officer.
R10_CheckpointDecides ==
    [][(state = "Checkpoint" /\ state' # state => state' \in {"Fixing", "HandedBack"})]_<<vars, seen>>

\* R10, S10: at round 3, escalation goes through the checkpoint. A fourth round that follows a third
\* FAIL comes after the checkpoint.
S10_CheckpointBeforeFourthRound ==
    [][(state = "Fixing" /\ state' = "InReview" /\ round = 3 => "Checkpoint" \in seen)]_<<vars, seen>>

\* S17: a cheap should is "fixed in place before the parent merges". R23: a merge needs a PASS on
\* its head, so a mend comes only after a PASS and is followed by a round on the new head.
R23_ReviewMendedHead ==
    [][ /\ (state' = "Mending" /\ state # "Mending" => state = "Clear")
        /\ (state = "Mending" /\ state' # state => state' = "InReview")
    ]_<<vars, seen>>

\* ---- The merge ----

\* R19: a conflict is resolved "under a fresh review of its hunks". R22: a real red gate, "then a
\* fix-scoped round, then a re-land". Nothing but a close, a wait on his click, a rerun or a repair
\* follows a merge, and a repair leads to a fix-scoped round or back to the officer.
R19_ReviewAfterMergeRepair ==
    [][ /\ (state = "Merging" /\ state' # state =>
            state' \in {"Done", "AwaitingClick", "Rerunning", "Repairing"})
        /\ (state = "Repairing" /\ state' # state => state' \in {"InReview", "HandedBack"})
        /\ (state = "Repairing" /\ state' = "InReview" => lens' = "fix-scoped")
    ]_<<vars, seen>>

\* R22: "A red gate after a merge: DYTEST reruns once." A flake ends the hop; a real red is repaired.
R22_RerunOnce ==
    [][ /\ (state' = "Rerunning" /\ state # "Rerunning" => state = "Merging")
        /\ (state = "Rerunning" /\ state' # state => state' \in {"Done", "AwaitingClick", "Repairing"})
    ]_<<vars, seen>>

\* core "Who decides what": Balazs decides "anything one-way (a merge that a revert cannot undo)".
\* R27: a one-way PR needs his click. A one-way merge closes only from the wait on his click.
Core_OneWayWaitsForHisClick ==
    [][(target = "main one-way" /\ state' = "Done" /\ state # "Done" => state = "AwaitingClick")]_<<vars, seen>>

\* R31: "Before his click, the captain watches its own CI and opens the HITL Merge only when it is
\* green." The wait on his click starts from the merge run, never straight from the review.
R31_ClickOnlyAfterGreen ==
    [][(state' = "AwaitingClick" /\ state # "AwaitingClick" => state \in {"Merging", "Rerunning"})]_<<vars, seen>>

\* R29: main-bound merges are paced; the coordinator gives the turn. A two-way PR into master starts
\* its merge only from the wait on its turn.
R29_TwoWayMainWaitsForItsTurn ==
    [][(target' = "main two-way" /\ state' = "Merging" /\ state # "Merging" => state = "AwaitingTurn")]_<<vars, seen>>

\* R27, R29: the wait on his click is for a one-way PR only, and the wait on a turn for a two-way PR
\* into master only.
R27_EachWaitHasItsTarget ==
    /\ state = "AwaitingClick" => target = "main one-way"
    /\ state = "AwaitingTurn" => target = "main two-way"
====
