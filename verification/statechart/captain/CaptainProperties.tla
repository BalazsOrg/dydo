---- MODULE CaptainProperties ----
EXTENDS Captain

\* Hand-written from his recorded words, the spec and the core page (dydo/core.md), never from the
\* model (K3). A property says what its source requires; when the model breaks one, the model is wrong.
\* The model is the comprehensive one (DYD-1492). A property observes the event its rule names through
\* the record the model keeps of it: the ticket's result and stage, the Question the work waits on, the
\* Review hop's verdict and its blockers, the proof on the head, the PR's PASS, the gates and CI, the
\* checkpoint line, the landing. It never infers an event from the state a step lands in.
\* seen is passive: the states of interest this captain has been in. Every model step has exactly one
\* observer extension, no step is disabled and no fairness is added.
VARIABLE seen
Remembered == {"Leaf", "Cutting", "Waiting", "InReview"}
PropertyInit == Init /\ seen = {state} \cap Remembered
PropertyNext == Next /\ seen' = seen \cup ({state'} \cap Remembered)

\* Where a review round runs or the head under it changes, and where the merge runs.
ReviewPhase == {"InReview", "Fixing", "Mending", "Repairing"}
MergePhase == {"Merging", "AwaitingTurn", "Held", "AwaitingClick", "Landed", "Rerunning"}
\* Where the head a review judged is changed: by a Fix, a mend of a cheap should, or a repair.
HeadChanges == {"Fixing", "Mending", "Repairing"}
QuestionStages == {"research", "council", "his-call"}
ToMaster == {"two-way", "one-way"}

\* The events the rules name, each read from its record:
\* a fresh reviewer is dispatched onto a new round's Review hop (S3: one Review per round);
StartsARound == verdict' = "due" /\ verdict # "due"
\* the round's verdict is written on its Review sub-ticket (R17);
Judges == verdict' \in {"pass", "fail"} /\ verdict' # verdict
\* the captain running a round acts on its written verdict v: it leaves the round, or orders another;
ActsOn(v) == state = "InReview" /\ verdict = v /\ <<state, verdict>>' # <<state, verdict>>
\* the merge lands.
Lands == landed' = "in" /\ landed # "in"
\* The number of the round after n, where 4 stands for four or more (the rules name round 3 and
\* nothing later, R10).
After(n) == IF n < 4 THEN n + 1 ELSE 4
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
    [][(filler /\ kind = "unread" /\ kind' # "unread" => kind' = "change")]_<<vars, seen>>

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
\* is cut again, or shows the contract cannot be met inside the ticket. H10: the route comes into
\* sight "one manageable part at a time", so the fog lifts only when a part is back.
H10_APartIsBackBeforeTheNextCut ==
    [][ /\ (state = "Waiting" /\ state' \in {"Cutting", "HandedBack"} => part \in {"back", "returned"})
        /\ (fog /\ ~fog' => part \in {"back", "returned"})
    ]_<<vars, seen>>

\* H10: "if the contract is met, it finishes its ticket": the parts are back, no route is left in
\* fog, and nothing waits on an answer ("if what is left all waits on Questions or his calls, it
\* waits on them"). R2: a whole that merges is reviewed "after wiring and before its Merge hop".
\* S8: a review's verdict comes when its seats are back.
H10_FinishWhenTheContractIsMet ==
    [][ /\ (state = "Waiting" /\ state' \in {"Done", "InReview"} =>
            part \in {"back", "wired"} /\ ~fog /\ asked = "none" /\ ~partwaits)
        /\ (state = "Waiting" /\ state' = "InReview" => part = "wired")
        /\ (part' = "wiring" /\ part # "wiring" => part = "back" /\ ~fog /\ asked = "none" /\ ~partwaits)
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
        /\ (hop # "live" => state' = state /\ part' = part /\ verdict' = verdict /\ proved' = proved)
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
\* a captain opens one only as the receiving officer, for a reviewer's need for Balazs in a FAIL it
\* acts on (R9: "up the chain at once"; S10), whatever else the verdict routes. L18: it opens at
\* stage research.
H16_TheReceivingOfficerOpensIt ==
    [][(asked' = "research" /\ asked # "research" =>
          \/ asked = "passed up"
          \/ ActsOn("fail") /\ "him" \in blockers)]_<<vars, seen>>

\* H16: a part's need passed up to this captain becomes a Question it opens, as the receiving
\* officer, and waits on; S10: it "blocks only what waits on it", the part. The part comes back only
\* with the answer, and only a part that is out passes a need up.
H16_ThePartWaitsOnItsAnswer ==
    [][ /\ (part' = "back" /\ part # "back" => ~partwaits)
        /\ (partwaits' /\ ~partwaits => part = "out")
    ]_<<vars, seen>>

\* H16: the work waits on the Question; S9: a Fix is Done only when its blockers are. Nothing closes
\* and no round starts while the work waits on an answer.
H16_NothingGoesOnPastTheWait ==
    [][((state' = "Done" /\ state # "Done") \/ StartsARound => asked = "none" /\ ~partwaits)]_<<vars, seen>>

\* L18: "every Question starts with research". H23: "A research captain opens a council or his-call
\* child only when its research does not resolve the call." L18: to him only when it is worth his
\* time, and otherwise to a council; L20: a call a council prepared goes to him.
L18_ChildOnlyAfterResearch ==
    [][(asked' \in {"council", "his-call"} /\ asked' # asked =>
          /\ kind = "answer" /\ stage = "research"
          /\ asked' = "council" => call = "council"
          /\ asked' = "his-call" => call = "his")]_<<vars, seen>>

\* L18, H16: a research Question's answer comes from its research, or from the council or his-call
\* child it opened; it closes only with its call settled. L18: a council child decides the call or
\* prepares it for him; L20: "by day the tribunal prepares and he decides", so a prepared call goes
\* to him, and only a decided one settles it.
L18_AnswerAfterResearch ==
    [][ /\ (stage = "research" /\ call' = "settled" /\ call # "settled" =>
            call = "open" \/ (asked \in {"council", "his-call"} /\ asked' = "none"))
        /\ (stage = "research" /\ asked = "council" /\ asked' # "council" =>
            \/ tribunal' = "decided" /\ call' = "settled"
            \/ tribunal' = "prepared" /\ call' = "his")
        /\ (stage = "research" /\ state' = "Done" /\ state # "Done" => call = "settled")
    ]_<<vars, seen>>

\* core "Who decides what": "Balazs decides [...] every call that is his to make." H23: a his-call
\* Question is the envoy ticket itself, and the envoy brings him the call. The call is his from the
\* take; only his answer settles it, as a step of its own; the ticket closes only on it.
Core_HisCallIsHis ==
    [][ /\ (kind = "unread" /\ stage' = "his-call" => call' = "his")
        /\ (stage = "his-call" /\ call' # call => call = "his" /\ call' = "settled" /\ state' = state)
        /\ (stage = "his-call" /\ state' = "Done" /\ state # "Done" => call = "settled")
    ]_<<vars, seen>>

\* L18: two judges from different families take sealed positions, then "one exchange follows", then
\* the exits. L23: a council's outcome is an answer: decided, or prepared for him.
L18_CouncilSteps ==
    [][ /\ (tribunal' = "sealed" /\ tribunal # "sealed" => stage = "council" /\ part' = "out")
        /\ (tribunal' = "exchange" /\ tribunal # "exchange" => tribunal = "sealed" /\ part = "back")
        /\ (tribunal' \in {"agreed", "split"} /\ tribunal' # tribunal => tribunal = "exchange" /\ part = "out")
        /\ (stage = "council" /\ state' = "Done" /\ state # "Done" => tribunal' \in {"decided", "prepared"})
    ]_<<vars, seen>>

\* L18 exits: "the judges agree on a two-way hard call → decided"; "they disagree, or the precedent
\* points both ways → prepared for him"; "one-way or DR-grade → always prepared, except the rare
\* overnight case in L21". L21: a two-way, seriously blocking DR-grade call may be decided overnight;
\* "Irreversible calls are never decided."
L18_CouncilExits ==
    [][ /\ (stage = "council" /\ tribunal' = "decided" /\ tribunal # "decided" =>
            tribunal = "agreed" /\ door \in {"two-way", "dr-grade overnight"})
        /\ (stage = "council" /\ tribunal' = "prepared" /\ tribunal # "prepared" =>
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

\* R4: "No review loop on a plan, spec or contract." A cut is never reviewed or merged, and gets no
\* verdict.
R4_NoReviewLoopOnACut ==
    kind = "cut" => state \notin ReviewPhase \cup MergePhase /\ verdict = "none"

\* R4: "A spec gets one scout pass that returns Questions, never PASS or FAIL, and is never
\* repeated." K30: the scout is a Wayfinding captain's native helper.
R4_OneScoutPass ==
    [][(scouts' # scouts => kind = "cut" /\ scouts = 0 /\ scouts' = 1 /\ verdict' = verdict)]_<<vars, seen>>

\* R5: "The author proves with receipts, and a fresh reviewer judges through lenses." S9: "Fix rN is
\* Done when its blockers are Done and proof ran on the head." A round starts only with the author's
\* proof of the head it judges, and a leaf's change closes as done only proved. A Fix, a mend or a
\* repair changes the head, so its proof is owed again when it starts.
R5_ProofBeforeJudgement ==
    [][ /\ (StartsARound => proved)
        /\ (state = "Leaf" /\ state' = "Done" /\ kind = "change" => proved)
        /\ (state' \in HeadChanges /\ state \notin HeadChanges => ~proved')
    ]_<<vars, seen>>

\* core: "It never judges its own work: the verdict is another agent's." R2, R23: the holder that
\* merges a change closes only after a review, from its merge.
R2_MergingHolderHasReview ==
    [][(merges /\ state' = "Done" /\ state # "Done" =>
        "InReview" \in seen /\ state \in MergePhase)]_<<vars, seen>>

\* R33: "No agent judges its own step. The Review hop's holder is never the Implement or Fix
\* holder." S3: one Review per round; S8: a lone reviewer holds its round's Review hop. A merger's
\* verdict comes only from the Review hop dispatched for the round, and only a merger dispatches one;
\* writing the verdict moves no work: not the captain's state, not the author's proof, not the round.
R33_TheVerdictIsTheReviewHops ==
    [][ /\ (Judges /\ kind # "verdict" => verdict = "due" /\ state' = state /\ proved' = proved /\ round' = round)
        /\ (StartsARound => merges)
    ]_<<vars, seen>>

\* R17: the verdict lives on the Review sub-ticket; "The round's holder writes the verdict and the PR
\* line together, at verdict time", and the PR body carries only a one-line PASS. core "Who decides
\* what": a reviewer decides its verdict, so a verdict ticket is done only with it written.
R17_VerdictAndPrLineTogether ==
    [][ /\ (Judges => (verdict' = "pass") = (pass' = "head" /\ pass # "head"))
        /\ (pass' = "head" /\ pass # "head" => Judges)
        /\ (kind = "verdict" /\ state' = "Done" /\ state # "Done" => verdict \in {"pass", "fail"})
    ]_<<vars, seen>>

\* S8: "A lone reviewer is handed its round whole and holds its Review hop. A round with several
\* seats is held by its officer, which posts one verdict naming each seat (model and provider) and
\* its blockers." A reviewer that cut its review into seats writes the verdict once they are back.
S8_OneVerdictOnceTheSeatsAreBack ==
    [][(Judges /\ kind = "verdict" => state = "Leaf" \/ (state = "Waiting" /\ part = "back"))]_<<vars, seen>>

\* core "Review severities", R11: "FAIL means at least one blocker"; PASS means none. A merge rests on
\* a PASS (R23) and a blocker is fixed now (core), so no merge runs with a blocker on record.
R11_VerdictHoldsItsBlockers ==
    /\ verdict = "pass" => blockers = {}
    /\ verdict = "fail" => blockers # {}
    /\ state \in MergePhase => blockers = {}

\* R8, R23: a merge needs a PASS. The merge starts only on a round's PASS verdict, and a reviewed
\* ticket closes only from its merge.
R8_PassBeforeMerge ==
    [][ /\ (state' \in MergePhase /\ state \notin MergePhase => verdict = "pass")
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
\* one more round on a PASS; the first from the work, its own or its parts'. A Fix leads to a round
\* or back to the officer, never straight to the merge.
R8_FixBeforeNextRound ==
    [][ /\ (StartsARound =>
            \/ state \in HeadChanges
            \/ state \in {"Leaf", "Waiting"} /\ round = 0
            \/ ActsOn("pass") /\ ~extra /\ extra')
        /\ (state = "Fixing" /\ state' # state => StartsARound \/ state' = "HandedBack")
    ]_<<vars, seen>>

\* R8: "the phase ends on the first round with no new blocker". R9: a FAIL goes to a Fix or to the
\* officer above. The captain leaves a round only on its written verdict; a PASS never sends the
\* work to a Fix or back; and S17: the merge starts with no cheap should outstanding.
R8_PassEndsTheRounds ==
    [][ /\ (state = "InReview" /\ state' # state => verdict \in {"pass", "fail"})
        /\ (ActsOn("pass") => state' \notin {"Fixing", "HandedBack"})
        /\ (state' \in MergePhase /\ state \notin MergePhase => ~cheap)
    ]_<<vars, seen>>

\* R8: "the officer may still order one more". One more round on a PASS is ordered at most once.
R8_ExtraRoundOnlyOnce ==
    [][ /\ (extra => extra')
        /\ (ActsOn("pass") /\ StartsARound => ~extra /\ extra')
        /\ (extra' /\ ~extra => ActsOn("pass") /\ StartsARound)
    ]_<<vars, seen>>

\* R9: blockers are routed by kind: "broken or against the spec → Fix, then the next round"; "a
\* security hole → Fix, then the next round with the security lens first"; "fundamental, or a new
\* requirement → the officer above re-plans"; "needs Balazs → up the chain at once". S10: a need for
\* Balazs "becomes a Question ticket that blocks only what waits on it", whatever else the verdict
\* holds. A FAIL never ends in the round it judged.
R9_BlockersRoutedByKind ==
    [][(ActsOn("fail") =>
          /\ state' # state
          /\ (state' = "HandedBack") = ("fundamental" \in blockers)
          /\ ("him" \in blockers => asked' = "research"))]_<<vars, seen>>

\* R8: "a round with a blocker is always followed by a full-lens round". R9: after a security hole,
\* the next round has the security lens first.
R9_LensAfterAFix ==
    [][(state = "Fixing" /\ StartsARound =>
          /\ lens' \in {"full", "security first"}
          /\ (lens' = "security first") = ("security" \in blockers))]_<<vars, seen>>

\* R10: "After round 3 comes a checkpoint, not a stop." S10: "The round-3 checkpoint is one line on
\* the Review". Acting on a third round's FAIL writes the line, and only it does; the round stays 3.
R10_CheckpointAtRoundThree ==
    [][ /\ (ActsOn("fail") /\ round = 3 => checkpoint' \in {"one more round", "escalated"})
        /\ (checkpoint' # checkpoint => ActsOn("fail") /\ round = 3 /\ round' = 3)
    ]_<<vars, seen>>

\* R10: "by default one more round, escalating only for a fundamental break, a new requirement, or
\* something that needs him." S10: "checkpoint: one more round, because ..." or "escalated: ...".
R10_EscalateOnlyForFundamentalOrHim ==
    [][(ActsOn("fail") /\ round = 3 =>
          (checkpoint' = "escalated") = (blockers \cap {"fundamental", "him"} # {}))]_<<vars, seen>>

\* R10, S10: at round 3, escalation goes through the checkpoint. A fourth round that follows a third
\* FAIL comes after the checkpoint.
S10_CheckpointBeforeFourthRound ==
    [][(StartsARound /\ state = "Fixing" /\ round = 3 => checkpoint # "none")]_<<vars, seen>>

\* S17: a should is "fixed in place before the parent merges if cheap". No merge runs with one
\* outstanding.
S17_CheapShouldMendedBeforeMerge ==
    state \in MergePhase => ~cheap

\* S17, R23: a merge needs a PASS on its head, so a mend comes only on a PASS with a cheap should
\* outstanding, and is followed by a round on the new head, with the should gone.
R23_ReviewMendedHead ==
    [][ /\ (state' = "Mending" /\ state # "Mending" => ActsOn("pass") /\ cheap)
        /\ (state = "Mending" /\ state' # state => StartsARound /\ ~cheap')
    ]_<<vars, seen>>

\* R23: the PASS names the head; R24: or an ancestor whose later commits are clean syncs, so a clean
\* sync keeps one that stands, now on an ancestor. No PASS stands on a head under a Fix, a mend or a
\* repair, and a new round's PASS is its own (R17 writes it).
R23_PassStandsOnItsHead ==
    [][ /\ (pass' = "ancestor" /\ pass # "ancestor" => pass = "head")
        /\ (resyncs' > resyncs => pass' = "ancestor")
        /\ (state' \in HeadChanges => pass' = "none")
        /\ (StartsARound => pass' = "none")
    ]_<<vars, seen>>

\* R23: dydo merge refuses "unless the PASS names the head, or an ancestor as R24 allows, the merge
\* with the tip is clean and DYTEST holds a green receipt keyed by the combined tree". R26: a PR into
\* master also needs CI green. S17: no cheap should is outstanding.
R23_MergeRestsOnItsEvidence ==
    [][(Lands => Stands /\ gates = "green" /\ ~cheap /\ (target \in ToMaster => ci = "green"))]_<<vars, seen>>

\* ---- The merge ----

\* R19: a conflict is resolved "under a fresh review of its hunks". R22: a real red gate, "then a
\* fix-scoped round, then a re-land". A repair starts only from the merge and leads to a round or
\* back to the officer. Its hunks stay unreviewed until a round's verdict covers them; the round that
\* starts with them unreviewed is scoped to them, and no other is; nothing lands before a verdict
\* has covered them.
R19_ReviewAfterMergeRepair ==
    [][ /\ (state' = "Repairing" /\ state # "Repairing" => state \in MergePhase)
        /\ (state = "Repairing" /\ state' # state => StartsARound \/ state' = "HandedBack")
        /\ (hunks' /\ ~hunks => state = "Repairing")
        /\ (hunks /\ ~hunks' => Judges)
        /\ (StartsARound => (lens' = "scoped") = hunks)
        /\ (Lands => ~hunks)
    ]_<<vars, seen>>

\* R21: the captain "sends an implementer only for a CONFLICT or a real red gate", and R22 after a
\* merge only for a real red, once rerun. A repair starts only with a CONFLICT or a red on record.
R21_RepairOnlyForAConflictOrARed ==
    [][(state' = "Repairing" /\ state # "Repairing" => gates \in {"red", "conflict"} \/ ci = "red")]_<<vars, seen>>

\* R22: "A red gate after a merge: DYTEST reruns once. [...] A real red means the captain reverts at
\* once [...], a Fix follows on the ticket branch". R21: before the merge, a red gate or a CONFLICT
\* only sends an implementer. A revert comes only after a landing, its red and its rerun; the Fix
\* follows it at once; nothing is reverted before a landing.
R22_RevertOnlyAfterTheMerge ==
    [][ /\ (landed' = "reverted" /\ landed # "reverted" =>
            landed = "in" /\ gates = "red" /\ state = "Rerunning" /\ state' = "Repairing")
        /\ (landed = "no" => landed' \in {"no", "in"})
    ]_<<vars, seen>>

\* R22: "A red gate after a merge: DYTEST reruns once. A flake means the hop is Done [...]. A real red
\* means the captain reverts at once". A red after the landing is rerun, never ends the hop itself;
\* the rerun ends it, green, as a flake, or reverts the merge.
R22_RerunOnce ==
    [][ /\ (landed = "in" /\ gates' = "red" /\ gates # "red" => state' = "Rerunning")
        /\ (state' = "Rerunning" /\ state # "Rerunning" => landed = "in" /\ gates' = "red")
        /\ (state = "Rerunning" /\ state' # state => (state' = "Done" /\ gates' = "green") \/ landed' = "reverted")
    ]_<<vars, seen>>

\* R20: "Done means DYTEST holds a green receipt on the merge tree and hosted CI is green on the merge
\* SHA." A captain that merges closes only with its merge in and both green.
R20_DoneOnlyOnceLanded ==
    [][(merges /\ state' = "Done" /\ state # "Done" => landed' = "in" /\ gates' = "green" /\ ci' = "green")]_<<vars, seen>>

\* core "Who decides what": Balazs decides "anything one-way (a merge that a revert cannot undo)".
\* R27: a one-way PR needs his click. A one-way merge lands only from the wait on his click.
Core_OneWayWaitsForHisClick ==
    [][(target = "one-way" /\ Lands => state = "AwaitingClick")]_<<vars, seen>>

\* R31: "Before his click, the captain watches its own CI and opens the HITL Merge only when it is
\* green." R23: and the rest of its evidence stands.
R31_ClickOnlyAfterGreen ==
    [][(state' = "AwaitingClick" /\ state # "AwaitingClick" => ci = "green" /\ gates = "green" /\ Stands)]_<<vars, seen>>

\* R26: "A two-way PR into master merges AFK, merged by its captain, when its evidence is complete":
\* a PASS on the head or an ancestor, gates green on the combined tree with current master, CI green.
R26_TwoWayEvidenceBeforeTheTurn ==
    [][(state' = "AwaitingTurn" /\ state # "AwaitingTurn" => ci = "green" /\ gates = "green" /\ Stands)]_<<vars, seen>>

\* R29: main-bound merges are paced; the coordinator gives the turn, and the captain merges. A two-way
\* PR into master lands only from the wait on its turn.
R29_TwoWayMainWaitsForItsTurn ==
    [][(target = "two-way" /\ Lands => state = "AwaitingTurn")]_<<vars, seen>>

\* R29: "Each re-checks against current master just before landing". A PR into master lands only
\* current with master; when master has moved, the re-check re-syncs it or finds the CONFLICT, and
\* never lands it.
R29_RecheckJustBeforeLanding ==
    [][ /\ (Lands /\ target \in ToMaster => ~behind)
        /\ (behind /\ ~behind' => ~Lands /\ (resyncs' = resyncs + 1 \/ gates' = "conflict"))
    ]_<<vars, seen>>

\* R29: "A PR forced to re-sync a third time is held while the coordinator sequences the colliding
\* work." Each forced re-sync counts one, at the re-check; the third is held, and only it; the count
\* goes back only once the coordinator has sequenced the colliding work.
R29_HeldOnTheThirdResync ==
    [][ /\ (resyncs' > resyncs => resyncs' = resyncs + 1 /\ behind /\ state \in {"AwaitingTurn", "AwaitingClick"})
        /\ (resyncs' < resyncs => state = "Held")
        /\ (resyncs' = 3 /\ resyncs = 2 => state' = "Held")
        /\ (state' = "Held" /\ state # "Held" => resyncs = 2 /\ resyncs' = 3)
        /\ (state = "Held" => resyncs = 3)
    ]_<<vars, seen>>

\* R27, R29: the wait on his click is for a one-way PR only, the wait on a turn for a two-way PR into
\* master only, and the hold for a PR into master.
R27_EachWaitHasItsTarget ==
    /\ state = "AwaitingClick" => target = "one-way"
    /\ state = "AwaitingTurn" => target = "two-way"
    /\ state = "Held" => target \in ToMaster
====
