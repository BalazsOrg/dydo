---- MODULE CaptainProperties ----
EXTENDS Captain

\* Hand-written from his recorded words, the spec and the core page (dydo/core.md), never from the
\* model (K3). A property says what its source requires; when the model breaks one, the model is wrong.
\* The model is the comprehensive one (DYD-1492). A property reads the model's variables as the
\* records its source names: the ticket's result and stage, the verdict's blockers, the PASS, the
\* gates and CI, the checkpoint line.
\* seen is passive: the states of interest this captain has been in. Every model step has exactly one
\* observer extension, no step is disabled and no fairness is added.
VARIABLE seen
Remembered == {"Leaf", "Cutting", "Waiting", "InReview"}
PropertyInit == Init /\ seen = {state} \cap Remembered
PropertyNext == Next /\ seen' = seen \cup ({state'} \cap Remembered)

\* Where a review round runs or the head under it changes, and where the merge runs.
ReviewPhase == {"InReview", "Fixing", "Mending", "Repairing"}
MergePhase == {"Merging", "AwaitingTurn", "Held", "AwaitingClick", "Landed", "Rerunning"}
QuestionStages == {"research", "council", "his-call"}
ToMaster == {"two-way", "one-way"}

\* A step that lands the merge; a step that starts a review round; the number of the round after n,
\* where 4 stands for four or more (the rules name round 3 and nothing later, R10).
Lands == landed' = "in" /\ landed # "in"
StartsARound == state' = "InReview" /\ (state # "InReview" \/ round' # round)
After(n) == IF n < 4 THEN n + 1 ELSE 4
\* A FAIL verdict: the round's blockers go to a Fix or to the officer above.
Fails == state = "InReview" /\ state' \in {"Fixing", "HandedBack"}
\* The PASS a merge rests on stands: on its head, or on an ancestor whose later commits are clean syncs.
Stands == pass \in {"head", "ancestor"}

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
    kind = "unread" => state \in {"Dispatched", "HandedBack"} /\ seen = {}

\* H11: a filler is "a Bug or small Feature", so a filler that is taken is a change.
H11_FillerIsAChange ==
    [][(filler /\ state = "Dispatched" /\ state' = "Taken" => kind' = "change")]_<<vars, seen>>

\* AGENTS.md "Sort yourself": crew does its one job and returns. H12: what goes up goes to the
\* officer above. Both ends are final: nothing follows Done or a hand-back. A structural check of
\* the two ends; the sources say a captain returns, not what it may do after.
H1_EndsAreFinal ==
    [][(state \in {"Done", "HandedBack"} => UNCHANGED vars)]_<<vars, seen>>

\* H23: "Every Question carries a stage label, set by whoever opens it: research, council or
\* his-call"; the stage, not the parent's Type, picks the job. Only a Question has one.
H23_EveryQuestionHasAStage ==
    /\ kind = "answer" => stage \in QuestionStages
    /\ kind \notin {"answer", "unread"} => stage = "none"

\* ---- Splitting and the way to the spec ----

\* core "A ticket": a captain "either does the work itself, with native helpers, or splits the
\* ticket and only orchestrates". Once it has cut, it never goes back to doing the work itself.
Core_SplitOnlyOrchestrates ==
    "Cutting" \in seen => state \notin {"Leaf", "Asking"}

\* H3, DYD-1295 df6a785e: the first move after a cut dispatches the parts by delegate_task and waits
\* on them. H2: a slice that cannot be divided coherently under its own ticket goes back to the
\* officer above.
H3_DispatchAndWaitAfterSplit ==
    [][ /\ (state = "Cutting" /\ state' # state => state' \in {"Waiting", "HandedBack"})
        /\ (state = "Cutting" /\ state' = "Waiting" => part' = "out" /\ hop' = "live")
    ]_<<vars, seen>>

\* H2, H10, R2: after a part returns the captain dispatches a ready part, cuts the next parts, sends
\* the whole to review, or finishes; H2 sends a change to its contract through the officer above.
\* It never returns to an untaken ticket or to doing the work itself.
H10_ReturnedPartContinues ==
    [][(state = "Waiting" /\ state' # state => state' \in {"Cutting", "InReview", "Done", "HandedBack"})]_<<vars, seen>>

\* H10: the next cut comes when a part is back and the contract is not met; H2: a part handed back
\* is cut again, or shows the contract cannot be met inside the ticket.
H10_APartIsBackBeforeTheNextCut ==
    [][(state = "Waiting" /\ state' \in {"Cutting", "HandedBack"} => part \in {"back", "returned"})]_<<vars, seen>>

\* H10: "if the contract is met, it finishes its ticket": the parts are back, no route is left in
\* fog, and nothing waits on an answer. R2: a whole that merges is reviewed "after wiring and before
\* its Merge hop". S8: a review's verdict comes when its seats are back.
H10_FinishWhenTheContractIsMet ==
    [][ /\ (state = "Waiting" /\ state' \in {"Done", "InReview"} =>
            part \in {"back", "wired"} /\ ~fog /\ asked = "none")
        /\ (state = "Waiting" /\ state' = "InReview" => part = "wired")
        /\ (part' = "wiring" /\ part # "wiring" => part = "back" /\ ~fog /\ asked = "none")
    ]_<<vars, seen>>

\* R6: "A leaf or lane gets no full review, only an optional fresh evidence-check seat." S4: it is
\* a Review hop beside the leaf, run once the part is back. Its verdict: the part stands, a cheap
\* should is fixed in place before the parent merges (S17), or its blockers are fixed now (core,
\* S9).
R6_EvidenceCheckBesideAPart ==
    [][ /\ (part' = "checking" /\ part # "checking" => part = "back")
        /\ (part = "checking" /\ part' # "checking" => part' \in {"back", "mending", "fixing"})
    ]_<<vars, seen>>

\* S24: a hop found stopped is set back to Todo, "then resumes the thread or dispatches a fresh
\* agent". Nothing goes on from it until then.
S24_StoppedHopRecovery ==
    [][ /\ (hop' = "stopped" /\ hop # "stopped" => hop = "live")
        /\ (hop = "stopped" /\ hop' # "stopped" => hop' = "todo")
        /\ (hop = "todo" /\ hop' # "todo" => hop' = "live")
        /\ (hop # "live" => state' = state /\ part' = part)
    ]_<<vars, seen>>

\* R34: a seat from another family is a leaf. "They're not meant do delegate into further sub
\* agents for sure." A seat never cuts or waits on parts.
R34_SeatNeverDelegates ==
    kind = "findings" => state \notin {"Cutting", "Waiting"}

\* ---- Questions ----

\* core "Who decides what": a call a captain may not decide becomes a Question for a fresh agent.
\* H16, DYD-1295 519126ab: the work waits on the answer and then goes on; it never ends from the
\* wait. It leaves the wait only with the answer on the Question.
H16_ResumeLeafAfterQuestion ==
    [][(state = "Asking" /\ state' # state => state' = "Leaf" /\ asked # "none" /\ asked' = "none")]_<<vars, seen>>

\* H16, H23: the captain waits on an open Question, and a Question has a stage.
H16_WaitsOnAnOpenQuestion ==
    state = "Asking" => asked \in QuestionStages

\* H16: "A need passed up becomes a Question ticket held by a fresh captain, at every level: the
\* receiving officer opens it [...] and waits on it." A captain's own need is opened by its officer;
\* a captain opens one only as the receiving officer, for a part's need or a reviewer's need for
\* Balazs (R9, S10). L18: it opens at stage research.
H16_TheReceivingOfficerOpensIt ==
    [][(asked' = "research" /\ asked # "research" =>
          \/ asked = "passed up"
          \/ state = "Waiting" /\ part = "out"
          \/ state = "InReview" /\ "him" \in blockers')]_<<vars, seen>>

\* H16: the work waits on the Question; S9: a Fix is Done only when its blockers are. Nothing closes
\* and no round starts while the work waits on an answer.
H16_NothingGoesOnPastTheWait ==
    [][(state' \in {"Done", "InReview"} /\ state' # state => asked = "none")]_<<vars, seen>>

\* L18: "every Question starts with research". H23: "A research captain opens a council or his-call
\* child only when its research does not resolve the call." L18: to him only when it is worth his
\* time, and otherwise to a council; L20: a call a council prepared goes to him.
L18_ChildOnlyAfterResearch ==
    [][(asked' \in {"council", "his-call"} /\ asked' # asked =>
          /\ kind = "answer" /\ stage = "research"
          /\ asked' = "council" => call = "council"
          /\ asked' = "his-call" => call = "his")]_<<vars, seen>>

\* L18, H16: a research Question's answer comes from its research, or from the council or his-call
\* child it opened; it closes only with its call settled.
L18_AnswerAfterResearch ==
    [][ /\ (call' = "settled" /\ call # "settled" =>
            call = "open" \/ (asked \in {"council", "his-call"} /\ asked' = "none"))
        /\ (stage = "research" /\ state' = "Done" /\ state # "Done" => call = "settled")
    ]_<<vars, seen>>

\* L18: two judges from different families take sealed positions, then "one exchange follows", then
\* the exits. L23: a council's outcome is an answer: decided, or prepared for him.
L18_CouncilSteps ==
    [][ /\ (tribunal' = "sealed" /\ tribunal # "sealed" => stage = "council" /\ state' = "Waiting")
        /\ (tribunal' = "exchange" /\ tribunal # "exchange" => tribunal = "sealed" /\ part = "back")
        /\ (tribunal' \in {"agreed", "split"} /\ tribunal' # tribunal => tribunal = "exchange" /\ part = "out")
        /\ (stage = "council" /\ state' = "Done" /\ state # "Done" => tribunal' \in {"decided", "prepared"})
    ]_<<vars, seen>>

\* L18 exits: "the judges agree on a two-way hard call → decided"; "they disagree, or the precedent
\* points both ways → prepared for him"; "one-way or DR-grade → always prepared, except the rare
\* overnight case in L21". L21: a two-way, seriously blocking DR-grade call may be decided overnight;
\* "Irreversible calls are never decided."
L18_CouncilExits ==
    [][ /\ (tribunal' = "decided" /\ tribunal # "decided" =>
            tribunal = "agreed" /\ door \in {"two-way", "dr-grade overnight"})
        /\ (tribunal' = "prepared" /\ tribunal # "prepared" =>
            tribunal = "split" \/ door \in {"dr-grade", "one-way"})
        /\ (tribunal = "agreed" /\ door = "two-way" /\ state = "Waiting" /\ state' = "Done" => tribunal' = "decided")
    ]_<<vars, seen>>

\* L18: a council is two judges and its chair; it is never one agent's work.
L18_ACouncilIsNeverOneAgent ==
    stage = "council" => state \notin {"Leaf", "Asking"}

\* ---- The review loop ----

\* core "Review severities": "A verdict is not reviewed again." R34, DYD-1372 777d396e. A verdict's
\* holder, and a seat's, is never in the review, fix and merge loop.
R34_VerdictNeverReviewed ==
    kind \in {"verdict", "findings"} => state \notin ReviewPhase \cup MergePhase

\* positions.md: only a captain that merges a change gets the review, fix and merge part. This
\* checks that table against the model; it is the table's rule, not a rule of the spec.
Sees_OnlyAMergerSeesReviewFixMerge ==
    state \in ReviewPhase \cup MergePhase => kind = "change" /\ merges

\* R4: "No review loop on a plan, spec or contract." A cut is never reviewed or merged.
R4_NoReviewLoopOnACut ==
    kind = "cut" => state \notin ReviewPhase \cup MergePhase

\* R5: "The author proves with receipts, and a fresh reviewer judges through lenses." A change goes
\* to review, or back to its officer as done, only with its proof.
R5_ProofBeforeJudgement ==
    [][ /\ (state' = "InReview" /\ state \in {"Leaf", "Waiting"} => proved)
        /\ (state = "Leaf" /\ state' = "Done" /\ kind = "change" => proved)
    ]_<<vars, seen>>

\* core: "It never judges its own work: the verdict is another agent's." R2, R23: the holder that
\* merges a change closes only after a review, from its merge.
R2_MergingHolderHasReview ==
    [][(merges /\ state' = "Done" /\ state # "Done" =>
        "InReview" \in seen /\ state \in MergePhase)]_<<vars, seen>>

\* R8, R23: a merge needs a PASS. The merge starts only from a round whose verdict is PASS, and a
\* reviewed ticket closes only from its merge.
R8_PassBeforeMerge ==
    [][ /\ (state' \in MergePhase /\ state \notin MergePhase => state = "InReview")
        /\ (state' = "Done" /\ state # "Done" /\ "InReview" \in seen => state \in MergePhase)
    ]_<<vars, seen>>

\* R8: "a fix between every two rounds"; S3: one Review per round; R10 names round 3. Each round's
\* number is one more than the last, from 1, and only a new round changes it.
R8_RoundsCountUp ==
    [][ /\ (StartsARound => round' = After(round))
        /\ (round' # round => StartsARound)
    ]_<<vars, seen>>

\* core "Review severities": a blocker "needs fixing now". R8: "a fix between every two rounds", and
\* the officer "may still order one more". A later round comes from a Fix, a repair, a mend, or the
\* one more round after a PASS; the first from the work, its own or its parts'. A Fix leads to a
\* round or back to the officer, never straight to the merge.
R8_FixBeforeNextRound ==
    [][ /\ (StartsARound =>
            \/ state \in {"Fixing", "Repairing", "Mending"}
            \/ state \in {"Leaf", "Waiting"} /\ round = 0
            \/ state = "InReview" /\ ~extra /\ extra')
        /\ (state = "Fixing" /\ state' # state => state' \in {"InReview", "HandedBack"})
    ]_<<vars, seen>>

\* R8: "the phase ends on the first round with no new blocker". R9: a FAIL goes to a Fix or to the
\* officer above. S17: a cheap should is fixed in place before the parent merges, so the merge
\* starts with none outstanding.
R8_PassEndsTheRounds ==
    [][ /\ (state = "InReview" /\ state' # state => state' \in {"Fixing", "HandedBack", "Merging", "Mending"})
        /\ (state = "InReview" /\ state' = "Merging" => ~cheap)
    ]_<<vars, seen>>

\* R8: "the officer may still order one more". One more round after a PASS is ordered at most once.
R8_ExtraRoundOnlyOnce ==
    [][ /\ (extra => extra')
        /\ (extra' /\ ~extra => state = "InReview" /\ state' = "InReview")
    ]_<<vars, seen>>

\* R9: blockers are routed by kind: "broken or against the spec → Fix, then the next round"; "a
\* security hole → Fix, then the next round with the security lens first"; "fundamental, or a new
\* requirement → the officer above re-plans"; "needs Balazs → up the chain at once". S10: a need for
\* Balazs "becomes a Question ticket that blocks only what waits on it". A FAIL has a blocker (R11).
R9_BlockersRoutedByKind ==
    [][(Fails =>
          /\ blockers' # {}
          /\ (state' = "HandedBack") = ("fundamental" \in blockers')
          /\ (state' = "Fixing" /\ "him" \in blockers' => asked' = "research"))]_<<vars, seen>>

\* R8: "a round with a blocker is always followed by a full-lens round". R9: after a security hole,
\* the next round has the security lens first.
R9_LensAfterAFix ==
    [][(state = "Fixing" /\ state' = "InReview" =>
          /\ lens' \in {"full", "security first"}
          /\ (lens' = "security first") = ("security" \in blockers))]_<<vars, seen>>

\* R10: "After round 3 comes a checkpoint, not a stop." S10: "The round-3 checkpoint is one line on
\* the Review". A third round with a blocker writes the line, and only it does; the round stays 3.
R10_CheckpointAtRoundThree ==
    [][ /\ (Fails /\ round = 3 => checkpoint' \in {"one more round", "escalated"})
        /\ (checkpoint' # checkpoint => Fails /\ round = 3 /\ round' = 3)
    ]_<<vars, seen>>

\* R10: "by default one more round, escalating only for a fundamental break, a new requirement, or
\* something that needs him." S10: "checkpoint: one more round, because ..." or "escalated: ...".
R10_EscalateOnlyForFundamentalOrHim ==
    [][(Fails /\ round = 3 =>
          (checkpoint' = "escalated") = (blockers' \cap {"fundamental", "him"} # {}))]_<<vars, seen>>

\* R10, S10: at round 3, escalation goes through the checkpoint. A fourth round that follows a third
\* FAIL comes after the checkpoint.
S10_CheckpointBeforeFourthRound ==
    [][(state = "Fixing" /\ state' = "InReview" /\ round = 3 => checkpoint # "none")]_<<vars, seen>>

\* S17: a should is "fixed in place before the parent merges if cheap". No merge runs with one
\* outstanding.
S17_CheapShouldMendedBeforeMerge ==
    state \in MergePhase => ~cheap

\* S17, R23: a merge needs a PASS on its head, so a mend comes only after a PASS and is followed by a
\* round on the new head, with the should gone.
R23_ReviewMendedHead ==
    [][ /\ (state' = "Mending" /\ state # "Mending" => state = "InReview")
        /\ (state = "Mending" /\ state' # state => state' = "InReview" /\ ~cheap')
    ]_<<vars, seen>>

\* R23: the PASS names the head; R24: or an ancestor whose later commits are clean syncs. A PASS
\* comes only from a round's verdict, a clean sync keeps one that stands, and no PASS stands on a head
\* under review, under a Fix, a mend or a repair.
R23_PassComesFromAVerdict ==
    [][ /\ (pass' = "head" /\ pass # "head" => state = "InReview")
        /\ (pass' = "ancestor" /\ pass # "ancestor" => pass = "head")
        /\ (state' \in ReviewPhase => pass' = "none")
    ]_<<vars, seen>>

\* R23: dydo merge refuses "unless the PASS names the head, or an ancestor as R24 allows, the merge
\* with the tip is clean and DYTEST holds a green receipt keyed by the combined tree". R26: a PR into
\* master also needs CI green. S17: no cheap should is outstanding.
R23_MergeRestsOnItsEvidence ==
    [][(Lands => Stands /\ gates /\ ~cheap /\ (target \in ToMaster => ci))]_<<vars, seen>>

\* ---- The merge ----

\* R19: a conflict is resolved "under a fresh review of its hunks". R22: a real red gate, "then a
\* fix-scoped round, then a re-land". A repair starts only from the merge, and leads to a round
\* scoped to it or back to the officer; its head lands only on a later PASS (R23_PassComesFromAVerdict).
R19_ReviewAfterMergeRepair ==
    [][ /\ (state' = "Repairing" /\ state # "Repairing" => state \in MergePhase)
        /\ (state = "Repairing" /\ state' # state => state' \in {"InReview", "HandedBack"})
        /\ (state = "Repairing" /\ state' = "InReview" => lens' = "scoped")
    ]_<<vars, seen>>

\* R22: "A red gate after a merge: DYTEST reruns once. [...] A real red means the captain reverts at
\* once". R21: before the merge, a red gate or a CONFLICT only sends an implementer. A revert comes
\* only after a landing and its rerun, and a repair before the landing reverts nothing.
R22_RevertOnlyAfterTheMerge ==
    [][ /\ (landed' = "reverted" /\ landed # "reverted" =>
            landed = "in" /\ state = "Rerunning" /\ state' = "Repairing")
        /\ (state' = "Repairing" /\ state # "Repairing" /\ landed # "in" => landed' = landed)
    ]_<<vars, seen>>

\* R22: "A red gate after a merge: DYTEST reruns once." A flake ends the hop; a real red is reverted
\* at once and repaired.
R22_RerunOnce ==
    [][ /\ (state' = "Rerunning" /\ state # "Rerunning" => state = "Landed" /\ landed = "in")
        /\ (state = "Rerunning" /\ state' # state => state' \in {"Done", "Repairing"})
        /\ (state = "Rerunning" /\ state' = "Repairing" => landed' = "reverted")
    ]_<<vars, seen>>

\* R20: "Done means DYTEST holds a green receipt on the merge tree and hosted CI is green on the merge
\* SHA." A captain that merges closes only once its merge is in, from the merge's own checks.
R20_DoneOnlyOnceLanded ==
    [][(merges /\ state' = "Done" /\ state # "Done" => landed = "in" /\ state \in {"Landed", "Rerunning"})]_<<vars, seen>>

\* core "Who decides what": Balazs decides "anything one-way (a merge that a revert cannot undo)".
\* R27: a one-way PR needs his click. A one-way merge lands only from the wait on his click.
Core_OneWayWaitsForHisClick ==
    [][(target = "one-way" /\ Lands => state = "AwaitingClick")]_<<vars, seen>>

\* R31: "Before his click, the captain watches its own CI and opens the HITL Merge only when it is
\* green." R23: and the rest of its evidence stands.
R31_ClickOnlyAfterGreen ==
    [][(state' = "AwaitingClick" /\ state # "AwaitingClick" => ci /\ gates /\ Stands)]_<<vars, seen>>

\* R26: "A two-way PR into master merges AFK, merged by its captain, when its evidence is complete":
\* a PASS on the head or an ancestor, gates green on the combined tree with current master, CI green.
R26_TwoWayEvidenceBeforeTheTurn ==
    [][(state' = "AwaitingTurn" /\ state # "AwaitingTurn" => ci /\ gates /\ Stands)]_<<vars, seen>>

\* R29: main-bound merges are paced; the coordinator gives the turn, and the captain merges. A two-way
\* PR into master lands only from the wait on its turn.
R29_TwoWayMainWaitsForItsTurn ==
    [][(target = "two-way" /\ Lands => state = "AwaitingTurn")]_<<vars, seen>>

\* R29: "Each re-checks against current master just before landing [...]. A PR forced to re-sync a
\* third time is held while the coordinator sequences the colliding work."
R29_HeldOnTheThirdResync ==
    [][ /\ (resyncs' # resyncs => resyncs' = resyncs + 1 /\ state = "AwaitingTurn")
        /\ (resyncs' = 3 /\ resyncs = 2 => state' = "Held")
        /\ (state' = "Held" /\ state # "Held" => resyncs = 2 /\ resyncs' = 3)
        /\ (state = "Held" => resyncs = 3)
    ]_<<vars, seen>>

\* R27, R29: the wait on his click is for a one-way PR only, and the wait on a turn for a two-way PR
\* into master only.
R27_EachWaitHasItsTarget ==
    /\ state = "AwaitingClick" => target = "one-way"
    /\ state \in {"AwaitingTurn", "Held"} => target = "two-way"
====
