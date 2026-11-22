---
area: reference
type: reference
explains: ["verification/statechart/captain/**"]
---

# Captain chart obligations

Hand-written from his recorded words, the approved spec and the core page (`dydo/core.md`), never from the model (K3). The model they judge is the comprehensive captain model in [statechart/captain/](../../../statechart/captain/README.md) (DYD-1492): 18 states, 85 actions and 30 variables. A property observes the event its rule names through the record the model keeps of it: the ticket's result and stage, the Question the work waits on, the Review hop's verdict and its blockers, the proof on the head, the PR's PASS, the gates and CI, the checkpoint line, the landing. It never infers an event from the state a step lands in. The simple chart Balazs approved is a view of that model, and `check-chart.mjs` proves the view faithful; these properties judge the model.

`seen` is the passive set of the states `Leaf`, `Cutting`, `Waiting` and `InReview` this captain has been in. No step is disabled, the model needs no bound (the round count stops at four, which stands for four or more), and no fairness is added.

## Properties

`CaptainProperties.cfg` registers 62 properties; the property run also checks `TypeOK`, from `Captain.cfg`, and the deadlock check.

`ReviewPhase` is `InReview`, `Fixing`, `Mending` and `Repairing`. `MergePhase` is `Merging`, `AwaitingTurn`, `Held`, `AwaitingClick`, `Landed` and `Rerunning`. Together they are the merger's states. `HeadChanges` is `Fixing`, `Mending` and `Repairing`, where the head a review judged is changed. The events, each read from its record: a round starts when a fresh reviewer is dispatched onto a new Review hop (`verdict` becomes due); the verdict is written when `verdict` becomes PASS or FAIL; the captain acts on a verdict when, in `InReview` with that verdict, it leaves the round or orders another; the merge lands when `landed` becomes `in`.

| Property | Source | What it checks |
|---|---|---|
| WorkPhaseBeforeDone | core "A ticket"; 7b3d2e3e | Done comes after `Leaf` or `Waiting`. |
| K30_KindKnownBeforeWork | K30, S6 | Every state but `Dispatched` and `HandedBack` has a known kind. |
| K30_KindStaysKnown | K30, S7; AGENTS.md | Once set, the kind never changes. |
| S7_UnstartedReturnHasNoWork | S6, S7 | A run that never read its kind stays in `Dispatched` and `HandedBack`. |
| H11_FillerIsAChange | H11 | A filler, when its kind is read, is a change. |
| H1_EndsAreFinal | H12; AGENTS.md | `Done` and `HandedBack` only stand still. A structural check of the two ends. |
| H23_EveryQuestionHasAStage | H23 | A Question has a stage, research, council or his-call, and no other ticket has one. |
| Core_SplitOnlyOrchestrates | core "A ticket" | After `Cutting`, never `Leaf` or `Asking` again. |
| H3_DispatchAndWaitAfterSplit | H3; H2; df6a785e | `Cutting` exits only to `Waiting`, with the part dispatched, or to `HandedBack`. |
| H10_ReturnedPartContinues | H2, H10, R2 | `Waiting` exits to `Cutting`, `InReview`, `Done` or `HandedBack`. |
| H10_APartIsBackBeforeTheNextCut | H10, H2 | The next cut, a hand-back of the whole, and the fog lifting come only with a part back or handed back. |
| H10_FinishWhenTheContractIsMet | H10, R2, S8 | The whole closes or goes to review only with its parts back, no fog, and neither its work nor its part waiting on a Question; to review only once wired; the wiring starts only from parts that are back. |
| R6_EvidenceCheckBesideAPart | R6, S4, S17, S9 | A part's evidence check starts only once it is back, and ends with the part standing, mending a cheap should, or fixing its blockers. |
| S24_StoppedHopRecovery | S24 | A stopped hop is set back to Todo, then resumed; nothing moves until it is live again: not the state, the part, the verdict or the proof. |
| R34_SeatNeverDelegates | R34 | A seat is never in `Cutting` or `Waiting`. |
| H16_ResumeLeafAfterQuestion | core "Who decides what"; H16; 519126ab | `Asking` exits only to `Leaf`, and only with the answer on the Question. |
| H16_WaitsOnAnOpenQuestion | H16, H23 | A captain in `Asking` waits on an open Question with a stage. |
| H16_TheReceivingOfficerOpensIt | H16, L18, R9, S10 | A research Question opens for this captain's work only for a need passed up, opened by the officer above, or by this captain as the receiving officer of a reviewer's need for Balazs in a FAIL it acts on, whatever else that verdict routes. |
| H16_ThePartWaitsOnItsAnswer | H16, S10 | A part whose need this captain opened as a Question comes back only with the answer; only a part that is out passes a need up. |
| H16_NothingGoesOnPastTheWait | H16, S9 | Nothing closes and no round starts while the work, or its part, waits on an answer. |
| L18_ChildOnlyAfterResearch | L18, H23, L20 | Only a research Question's captain opens a council or his-call child, and only after its research left the call to that child. |
| L18_AnswerAfterResearch | L18, L20, H16 | A research Question's call is settled only by its research or its child's answer; a council child that decided settles it, one that prepared it sends it to him; it closes only with its call settled. |
| Core_HisCallIsHis | core "Who decides what"; H23 | A his-call Question's call is his from the take; only his answer settles it, as a step of its own; the ticket closes only on it. |
| L18_CouncilSteps | L18, L23 | A council runs sealed positions, then one exchange, then the judges agree or split, and closes decided or prepared. |
| L18_CouncilExits | L18, L21 | A council decides only when its judges agree on a two-way call, or on the overnight DR-grade case; it prepares when they split or the call is DR-grade or one-way; agreed on a two-way call, it decides. |
| L18_ACouncilIsNeverOneAgent | L18 | A council's chair is never in `Leaf` or `Asking`. |
| R34_VerdictNeverReviewed | core "Review severities"; R34; 777d396e | A verdict's holder and a seat are never in a merger's state. |
| Sees_OnlyAMergerSeesReviewFixMerge | `statechart/captain/positions.md` | Only a captain that merges a change is in a merger's state. |
| R4_NoReviewLoopOnACut | R4 | A cut is never in a merger's state and gets no verdict. |
| R4_OneScoutPass | R4; K30 | A cut's spec gets one scout pass, never a second. |
| R5_ProofBeforeJudgement | R5, S9; core "Who decides what" | A round starts only on the author's proof of the head it judges; a leaf's change closes as done only proved; a Fix, a mend or a repair starts with its proof owed. |
| R2_MergingHolderHasReview | core "Who decides what"; R2, R23 | A captain that merges closes only from `MergePhase`, with `InReview` behind it. |
| R33_TheVerdictIsTheReviewHops | R33, S3, S8; core | A merger's verdict comes only from the Review hop dispatched for the round, and writing it moves no work: not the state, the proof or the round. Only a merger dispatches a Review hop. |
| R17_VerdictAndPrLineTogether | R17; core "Who decides what" | A PASS verdict and the PR's PASS line are written together, at verdict time, and a FAIL writes no PASS; no PASS on a head comes any other way; a verdict ticket closes only with its verdict written. |
| S8_OneVerdictOnceTheSeatsAreBack | S8 | A reviewer writes its verdict alone, or once its seats are back. |
| R11_VerdictHoldsItsBlockers | core "Review severities"; R11, R23 | A PASS holds no blocker, a FAIL at least one, and no merge state has a blocker on record. |
| R8_PassBeforeMerge | R8, R23 | `MergePhase` starts only on a PASS verdict, and a reviewed ticket closes only from `MergePhase`. |
| R8_RoundsCountUp | R8, R10, S3 | Each round's number is one more than the last, from 1, four standing for four or more; only a new round changes it, and a round at four is still a new round. |
| R8_FixBeforeNextRound | core "Review severities"; R8, S9 | A round starts only from a Fix, a repair, a mend, the one more round on a PASS, or the work's first round; `Fixing` exits to a new round or to `HandedBack`. |
| R8_PassEndsTheRounds | R8, R9, S17 | The captain leaves a round only on its written verdict; a PASS never sends the work to a Fix or back; the merge starts with no cheap should outstanding. |
| R8_ExtraRoundOnlyOnce | R8 | One more round on a PASS is ordered at most once, at round four as at any other. |
| R9_BlockersRoutedByKind | R9, R11, S10, H16 | Acting on a FAIL leaves the round; it goes to the officer above exactly when a blocker is fundamental; a need for Balazs opens a Question, whether the work goes to a Fix or back. |
| R9_LensAfterAFix | R8, R9 | The round after a Fix looks with the full lenses, with the security lens first exactly when a blocker was a security hole. |
| R10_CheckpointAtRoundThree | R10, S10 | Acting on a third round's FAIL writes the checkpoint line, nothing else does, and the round stays 3. |
| R10_EscalateOnlyForFundamentalOrHim | R10, S10 | At the checkpoint it escalates exactly when a blocker is fundamental or needs him; otherwise one more round. |
| S10_CheckpointBeforeFourthRound | R10, S10 | A fourth round after a Fix of round 3 comes after the checkpoint line. |
| S17_CheapShouldMendedBeforeMerge | S17 | No merge state has a cheap should outstanding. |
| R23_ReviewMendedHead | S17, R23 | A mend starts only on a PASS with a cheap should outstanding, and ends only in a new round, with the should gone. |
| R23_PassStandsOnItsHead | R23, R24, R17 | A PASS moves to an ancestor only from the head, and each re-sync moves it there; none stands on a head under a Fix, a mend or a repair, or at a new round's start. |
| R23_MergeRestsOnItsEvidence | R23, R24, R26, S17 | A merge lands only on a standing PASS, green gates, no cheap should, and green CI into master. |
| R19_ReviewAfterMergeRepair | R19, R21, R22 | `Repairing` starts only from `MergePhase` and exits to a new round or `HandedBack`; a repair's hunks stay unreviewed until a verdict covers them; the round that starts with them unreviewed is scoped, and no other; nothing lands with them unreviewed. |
| R21_RepairOnlyForAConflictOrARed | R21, R22 | A repair starts only with a CONFLICT or a red on record. |
| R22_RevertOnlyAfterTheMerge | R21, R22 | A revert comes only after a landing, its red and its rerun, and a Fix follows it at once; nothing is reverted before a landing. |
| R22_RerunOnce | R22 | A red after the landing goes to its one rerun, never straight to an end; `Rerunning` starts only on a landed merge's red, and ends green as a flake or with the merge reverted. |
| R20_DoneOnlyOnceLanded | R20 | A captain that merges closes only with its merge in, DYTEST green on the merge tree and CI green on the merge SHA. |
| Core_OneWayWaitsForHisClick | core "Who decides what"; R27 | A one-way merge lands only from `AwaitingClick`. |
| R31_ClickOnlyAfterGreen | R31, R23 | `AwaitingClick` starts only with CI green, gates green and the PASS standing. |
| R26_TwoWayEvidenceBeforeTheTurn | R26 | `AwaitingTurn` starts only with CI green, gates green and the PASS standing. |
| R29_TwoWayMainWaitsForItsTurn | R26, R29 | A two-way PR into master lands only from `AwaitingTurn`. |
| R29_RecheckJustBeforeLanding | R29 | A PR into master lands only while master has not moved past it; when it has, the re-check re-syncs it or records the CONFLICT, and lands nothing. |
| R29_HeldOnTheThirdResync | R29 | A re-sync happens only at the re-check, after master moved, one at a time; the third is held, and only it; the count goes back only after the hold. |
| R27_EachWaitHasItsTarget | R27, R29 | Only a one-way PR is in `AwaitingClick`, only a two-way PR into master in `AwaitingTurn`, and only a PR into master in `Held`. |

`Sees_OnlyAMergerSeesReviewFixMerge` is not a rule of the spec. It checks the states column of the who-sees-what table against the model.

## Changed at DYD-1492 r1

The r1 review (DYD-1495) found the model too thin in nine places. The model now carries what those rules decide on, so the properties check the rules themselves, not only the states before and after.

| Before | Now |
|---|---|
| L18_EveryQuestionStartsWithResearch checked `kind = "answer"` | L18_ChildOnlyAfterResearch and L18_AnswerAfterResearch check the held Question's stage, its research and its child; L18_CouncilSteps, L18_CouncilExits and L18_ACouncilIsNeverOneAgent check the council. |
| R31_ClickOnlyAfterGreen checked the state before | It checks CI, gates and the PASS. |
| R8_FullLensAfterBlocker accepted either lens after a security hole | R9_LensAfterAFix: security first exactly after a security hole. |
| No property fixed the round number | R8_RoundsCountUp, and R10_CheckpointAtRoundThree keeps round 3 through the checkpoint. |
| R10_CheckpointAfterThirdFail, R10_CheckpointDecides on a `Checkpoint` state | R10_CheckpointAtRoundThree, R10_EscalateOnlyForFundamentalOrHim, on the round-3 FAIL and its blockers. |
| H3_WaitAfterSplit, H10_WayfindingLeavesByCut checked destinations | H3_DispatchAndWaitAfterSplit observes the dispatch; H10_APartIsBackBeforeTheNextCut and H10_FinishWhenTheContractIsMet observe the part. `Wayfinding` is now a cut in fog. |
| H16_ResumeLeafAfterQuestion checked the destination | It observes the answer. |
| R19_ReviewAfterMergeRepair checked exit and lens | It also keeps the repair to the merge. |
| R8_PassEndsTheRounds, R23_ReviewMendedHead on `Clear` | On the PASS step out of `InReview`, with the cheap should carried. |
| R22_RerunOnce, R20 not checked | R22_RerunOnce, R22_RevertOnlyAfterTheMerge, R20_DoneOnlyOnceLanded split the landing from what comes before it. |

The properties PR 385 dropped stay restored in these forms; the earlier ones are in Git up to `3b1ab539`.

## Changed at DYD-1492 r2

The r2 review (DYD-1502) found properties that still inferred an event from where a step lands, and a view check that trusted a name. The model now keeps a record of each event a rule names, and each property observes that record.

| Review r2 | Before | Now |
|---|---|---|
| B1 | A his-call Question closed with no answer from him. | `call` is his from the take; `HeDecides` settles it; `CloseLeaf` and `CloseTheWhole` need it settled. Core_HisCallIsHis. `AskACouncil` and `AskHim` now guard on the held stage, research. |
| B2 | A part's Question was the splitter's `asked`, and the part came back with it open; a verdict holding a fundamental blocker and a need for him handed back with no Question. | `partwaits` is the part's Question; `PartBack` needs it answered (H16_ThePartWaitsOnItsAnswer). `Replan` and `CheckpointEscalates` open the Question for a need for him, and R9_BlockersRoutedByKind and H16_TheReceivingOfficerOpensIt observe the verdict acted on, whatever its destination. |
| B3 | Fog lifted only through another cut. | `TheWayIsFound`: once the part that brings the route into sight is back, the contract check lifts the fog, and the captain finishes or cuts. H10_APartIsBackBeforeTheNextCut observes the fog. |
| B4 | The next round after a Fix, a mend or a repair started with no proof on the changed head. | `TheFixIsProved`, `TheMendIsProved`, `TheRepairIsProved`; the proof is owed whenever a head starts to change. R5_ProofBeforeJudgement checks every round's start. |
| B6 | A one-way PR had no re-check before his click; after the hold no clean re-sync was left; a repair started before any red result. | `MasterMoves`, `ResyncOnTheRecheck`, `ConflictOnTheRecheck` on both waits; the count starts again after the hold. `gates` and `ci` record their results, and a repair needs a CONFLICT or a red on record. R29_RecheckJustBeforeLanding, R21_RepairOnlyForAConflictOrARed, R29_HeldOnTheThirdResync. |
| B8 | `check-chart.mjs` accepted false words, skipped every edge named `Ended`, and read any graph given with `--dot`. | It pins the approved words' digest, checks the config, binds the graph to the model's TLC run and its variables, and checks that `Ended` stands still at an end and every other step moves. `--dot` is gone. |
| B9 | A round at four was not seen as a new round, a red after landing could end the hop, a merge could carry a blocker, and a second scout pass left no trace. | A round's start is a new Review hop (`verdict` due), so R8_RoundsCountUp and R8_ExtraRoundOnlyOnce see it at four; R20_DoneOnlyOnceLanded and R22_RerunOnce read the gates; R11_VerdictHoldsItsBlockers; `scouts` counts the passes (R4_OneScoutPass). The routing reads the verdict, not the destination (R8_PassBeforeMerge, R8_PassEndsTheRounds, R9, R10). |
| New blocker | The merging captain wrote the PASS; a verdict ticket closed with no verdict. | The Review hop: `TheReviewPasses` and `TheReviewFails` are the reviewer's steps, writing the verdict and, for a PASS, the PR's PASS line (R17_VerdictAndPrLineTogether, R33_TheVerdictIsTheReviewHops); the captain only acts on them. `WriteTheVerdict` for a verdict ticket, once its seats are back (S8_OneVerdictOnceTheSeatsAreBack). |
| PR 385 nit 4 | R19 checked the repair-review entry and its lens. | `hunks` records a repair's hunks until a verdict covers them; the round that starts with them is scoped, and nothing lands before (R19_ReviewAfterMergeRepair). |
| This fix's own attempts | A council child that prepared the call could pass for one that decided it; a re-sync could keep the PASS on the changed head; a revert needed no Fix after it. | `tribunal` records the council child's outcome for a research Question (L18_AnswerAfterResearch); R23_PassStandsOnItsHead moves the PASS to an ancestor at each re-sync; R22_RevertOnlyAfterTheMerge sends the revert to its Fix. |

R23_PassComesFromAVerdict is renamed R23_PassStandsOnItsHead: the PASS on the head now comes from R17's verdict, in R17_VerdictAndPrLineTogether. New: H16_ThePartWaitsOnItsAnswer, Core_HisCallIsHis, R4_OneScoutPass, R33_TheVerdictIsTheReviewHops, R17_VerdictAndPrLineTogether, S8_OneVerdictOnceTheSeatsAreBack, R11_VerdictHoldsItsBlockers, R21_RepairOnlyForAConflictOrARed and R29_RecheckJustBeforeLanding.

## What the model cannot show

The properties check order only. The model has no data for any of these, so none is checked:

- Whether the contract is met, and whether a ticket really fits one agent.
- Who an agent is: that the reviewer is not the author, comes from another model family, or is fresh; who holds the Fix or the mend. The model has the Review hop as a step of its own, apart from the author's proof and the Fix, and R33_TheVerdictIsTheReviewHops checks that; it cannot see that two hops are held by different agents.
- Whether a gate's red is real or a flake before the merge, and what a recovery plan for a one-way PR holds. A red before the merge is recorded as real; re-running CI before a merge is not ordered by any source, and the model draws no rerun there.
- What a verdict holds beyond the kinds that route it and a cheap should: the shoulds that wait, the nits, and that each was filed or ruled on; a seat's findings.
- Which skill an agent loaded and what its brief said. `positions.md` is checked by reading.
- How many parts there are: one abstract part stands for them all, one Question at a time for the captain's own work and one for its part.
- Whether a call was one the captain may decide, and whether a call is worth his time.
- The words of the checkpoint line (S10). The model has the line's kind, not what it says.
- That the rounds go on past the fifth FAIL (R10): the round count stops at four, which stands for four or more, and nothing in the model stops at five.
- That anything ends. Without fairness a captain may wait forever; these are safety checks.

Whether an answer may be reviewed has no source. The model gives an answer's holder no review, because only a change is merged; that is a choice of the model, checked only as part of the who-sees-what table.

## Controls

The registered control in [controls.json](../controls.json) is the step `Asking` to `Done` for a change that is not merged, waiting on an open research Question, with every other variable of the model set. H16_ResumeLeafAfterQuestion must reject it.

The hand-run controls are each one change in a scratch copy of the twin, with the committed files untouched: one changed line, except where the table shows two edits (the review's `p-false-verdict`, as the review wrote it, and `v4`, two actions off as before). The model controls run through `run.py --machine captain` and must be rejected by the property their row names; "Also broken" lists the other rules the statechart tool reported broken. Its re-runs after the first break stop at 100,000 distinct states, below the property run's 127,534, so a rule left out of that list was not always checked. `NoDeadlock` stands where the mutant leaves a state with no step. `b*` and `e*` are the earlier controls, adapted where the model's text changed; `p-*` are review r2's probes; `m*` and `c*` are this fix's own attempts to fool each property and the view check.

| For | Mutant | Changed line | Rejected by | Also broken |
|---|---|---|---|---|
| B1 | b1a | `AskACouncil`: `/\ call = "council"` → `/\ kind = "answer"` | L18_ChildOnlyAfterResearch |  |
| B1 | b1b | `TheCouncilDecides`: `/\ door \in {"two-way", "dr-grade overnight"}` → `/\ TRUE` | L18_CouncilExits |  |
| B1 | b1c | `TheJudgesAgree`: `/\ tribunal = "exchange"` → `/\ tribunal = "sealed"` | L18_CouncilSteps |  |
| B1 | b1d | `AskHim`: `/\ call = "his"` → `/\ TRUE` | L18_ChildOnlyAfterResearch |  |
| B2 | b2a | `PassTheNeedUp`: `/\ asked' = "passed up"` → `/\ asked' = "research"` | H16_TheReceivingOfficerOpensIt |  |
| B2 | b2b (adapted: the verdict now holds the blockers) | `Fail`: `/\ asked' = IF "him" \in blockers THEN "research" ELSE asked` → `/\ asked' = asked` | R9_BlockersRoutedByKind |  |
| B2 | b2c (adapted: Replan hands back blockers with no fundamental one) | `Replan`: `/\ "fundamental" \in blockers` → `/\ TRUE` | R9_BlockersRoutedByKind |  |
| B2 | b2d (adapted: the guard moved from ReviewAgain to the Fix's proof) | `TheFixIsProved`: `/\ asked = "none"` → `/\ TRUE` | H16_NothingGoesOnPastTheWait |  |
| B3 | b3a | `CloseTheWhole`: `/\ part = "back"` → `/\ TRUE` | H10_FinishWhenTheContractIsMet | TypeOK, H1_EndsAreFinal, R6_EvidenceCheckBesideAPart, S24_StoppedHopRecovery, H16_NothingGoesOnPastTheWait |
| B3 | b3b | `WireTheParts`: `/\ part = "back"` → `/\ TRUE` | H10_FinishWhenTheContractIsMet | R6_EvidenceCheckBesideAPart, S24_StoppedHopRecovery, H16_NothingGoesOnPastTheWait |
| B3 | b3c | `ResumeTheHop`: `/\ hop = "todo"` → `/\ hop = "stopped"` | S24_StoppedHopRecovery | NoDeadlock |
| B3 | b3d | `PartBack`: `/\ hop = "live"` → `/\ TRUE` | S24_StoppedHopRecovery | TypeOK, H1_EndsAreFinal, H3_DispatchAndWaitAfterSplit |
| B3 | b3e | `Dispatch`: `/\ part' = "out"` → `/\ part' = "back"` | H3_DispatchAndWaitAfterSplit | H10_FinishWhenTheContractIsMet, H16_ThePartWaitsOnItsAnswer, H16_NothingGoesOnPastTheWait, L18_CouncilSteps |
| B4 | b4a | `NoMoreRound`: `/\ ~cheap` → `/\ TRUE` | R8_PassEndsTheRounds | S17_CheapShouldMendedBeforeMerge, R23_MergeRestsOnItsEvidence |
| B4 | b4b | `SendLeafToReview`: `/\ proved` → `/\ TRUE` | R5_ProofBeforeJudgement |  |
| B4 | b4c | `CheckThePart`: `/\ part = "back"` → `/\ part = "out"` | R6_EvidenceCheckBesideAPart | H10_FinishWhenTheContractIsMet, S24_StoppedHopRecovery, H16_ThePartWaitsOnItsAnswer, H16_NothingGoesOnPastTheWait |
| B4 | b4d (adapted: the mend's proof now clears the should) | `TheMendIsProved`: `/\ cheap' = FALSE` → `/\ cheap' = cheap` | R23_ReviewMendedHead |  |
| B4 | b4e | `Take`: `IF kind' = "change" THEN BOOLEAN` → `IF kind' \in {"change", "cut"} THEN BOOLEAN` | R4_NoReviewLoopOnACut | TypeOK, Sees_OnlyAMergerSeesReviewFixMerge |
| B5 | b5a (adapted: one more round takes a need for Balazs) | `CheckpointOneMoreRound`: `/\ blockers \cap {"fundamental", "him"} = {}` → `/\ "fundamental" \notin blockers` | R10_EscalateOnlyForFundamentalOrHim | R9_BlockersRoutedByKind |
| B5 | b5b (adapted: escalates fixable blockers) | `CheckpointEscalates`: `/\ "fundamental" \in blockers` → `/\ TRUE` | R10_EscalateOnlyForFundamentalOrHim | R9_BlockersRoutedByKind |
| B5 | b5c | `ReviewAgain`: `IF "security" \in blockers THEN "security first" ELSE "full"` → `"full"` | R9_LensAfterAFix |  |
| B5 | b5d | `CheckpointOneMoreRound`: `/\ checkpoint' = "one more round"` → `/\ checkpoint' = checkpoint` | R10_CheckpointAtRoundThree | S10_CheckpointBeforeFourthRound |
| B5 | b5e | `Fail`: `/\ round # 3` → `/\ TRUE` | R10_CheckpointAtRoundThree | R10_EscalateOnlyForFundamentalOrHim, S10_CheckpointBeforeFourthRound |
| B6 | b6a | `GateRed`: `/\ state = "Landed"` → `/\ state = "Merging"` | R22_RerunOnce | R22_RevertOnlyAfterTheMerge, R20_DoneOnlyOnceLanded |
| B6 | b6b | `HisClick`: `/\ state' = "Landed"` → `/\ state' = "Done"` | R20_DoneOnlyOnceLanded |  |
| B6 | b6c (adapted: gates are now a result) | `MergeIntoFeature`: `/\ gates = "green"` → `/\ TRUE` | R23_MergeRestsOnItsEvidence |  |
| B6 | b6d (adapted: CI is now a result) | `OpenTheHitlMerge`: `/\ ci = "green"` → `/\ TRUE` | R31_ClickOnlyAfterGreen | R23_MergeRestsOnItsEvidence |
| B6 | b6e (adapted: ResyncOnTheTurn is now ResyncOnTheRecheck) | `ResyncOnTheRecheck`: `IF resyncs' = 3 THEN "Held"` → `IF FALSE THEN "Held"` | R29_HeldOnTheThirdResync | TypeOK |
| B6 | b6f | `RealRed`: `/\ landed' = "reverted"` → `/\ landed' = landed` | R22_RerunOnce |  |
| B6 | b6g (adapted: the UNCHANGED list grew) | `RedBeforeLanding`: `/\ UNCHANGED <<ticket, scouts, split, hop, question, review, target, behind, resyncs, landed>>` → `/\ landed' = "reverted" /\ UNCHANGED <<ticket, scouts, split, hop, question, review, target, behind, resyncs>>` | R22_RevertOnlyAfterTheMerge |  |
| B9 | b9a | `ReviewAgain`: `/\ round' = NextRound` → `/\ round' = round` | R8_RoundsCountUp |  |
| B9 | b9b | `RepairReviewed`: `/\ round' = NextRound` → `/\ round' = 1` | R8_RoundsCountUp |  |
| Earlier | e01 | `Replan`: `/\ round # 3` → `/\ TRUE` | R10_CheckpointAtRoundThree | R10_EscalateOnlyForFundamentalOrHim |
| Earlier | e02 | `OneMoreRound`: `/\ ~extra` → `/\ TRUE` | R8_ExtraRoundOnlyOnce | R8_FixBeforeNextRound |
| Earlier | e03 (its catcher changed: the mended head merges with no PASS verdict) | `ReviewTheMendedHead`: `/\ state' = "InReview"` → `/\ state' = "Merging"` | R8_PassBeforeMerge | NoDeadlock, R8_RoundsCountUp, R19_ReviewAfterMergeRepair |
| Earlier | e04 (adapted: a mend on a FAIL (its old form, a mend from Fixing, now never fires)) | `MendACheapShould`: `/\ verdict = "pass"` → `/\ verdict \in {"pass", "fail"}` | R23_ReviewMendedHead | R9_BlockersRoutedByKind, R10_CheckpointAtRoundThree, R10_EscalateOnlyForFundamentalOrHim |
| Earlier | e05 | `RepairReviewed`: `/\ lens' = "scoped"` → `/\ lens' = "full"` | R19_ReviewAfterMergeRepair |  |
| Earlier | e06 (its catcher changed: the revert keeps its record, and no Fix follows it) | `RealRed`: `/\ state' = "Repairing"` → `/\ state' = "Merging"` | R22_RevertOnlyAfterTheMerge | NoDeadlock |
| Earlier | e07 (its catcher changed: the round has no Review hop, so the model stands still) | `MergeConflict`: `/\ state' = "Repairing"` → `/\ state' = "InReview"` | NoDeadlock |  |
| Earlier | e08 | `MergeIntoFeature`: `/\ target = "feature"` → `/\ TRUE` | Core_OneWayWaitsForHisClick | R23_MergeRestsOnItsEvidence, R29_TwoWayMainWaitsForItsTurn |
| Earlier | e09 (its catcher changed: the merge starts with no PASS verdict) | `ReviewAgain`: `/\ state' = "InReview"` → `/\ state' = "Merging"` | R8_PassBeforeMerge | S17_CheapShouldMendedBeforeMerge, NoDeadlock, R8_RoundsCountUp, R8_PassEndsTheRounds, R19_ReviewAfterMergeRepair |
| Earlier | e10 | `Split`: `/\ kind # "findings"` → `/\ TRUE` | R34_SeatNeverDelegates |  |
| Earlier | e11 | `QuestionOpened`: `/\ asked' = "research"` → `/\ asked' = "council"` | L18_ChildOnlyAfterResearch | Core_HisCallIsHis |
| Earlier | e12 | `Take`: `IF filler THEN {"change"}` → `IF FALSE THEN {"change"}` | H11_FillerIsAChange |  |
| Earlier | e13 | `CutTheNextParts`: `/\ state' = "Cutting"` → `/\ state' = "Leaf"` | Core_SplitOnlyOrchestrates | H10_ReturnedPartContinues, H16_NothingGoesOnPastTheWait |
| Earlier | e14 | `CloseLeaf`: `/\ ~merges` → `/\ TRUE` | R2_MergingHolderHasReview | R20_DoneOnlyOnceLanded |
| Earlier | e15 | `SendLeafToReview`: `/\ merges` → `/\ TRUE` | Sees_OnlyAMergerSeesReviewFixMerge | TypeOK, R33_TheVerdictIsTheReviewHops |
| Earlier | e16 | `Dispatch`: `/\ state' = "Waiting"` → `/\ state' = "Done"` | H3_DispatchAndWaitAfterSplit | WorkPhaseBeforeDone, H16_NothingGoesOnPastTheWait, L18_AnswerAfterResearch, Core_HisCallIsHis, L18_CouncilSteps, R2_MergingHolderHasReview, R17_VerdictAndPrLineTogether, R20_DoneOnlyOnceLanded |
| Earlier | e17 | `WaitOnTheAnswer`: `/\ state' = "Asking"` → `/\ state' = "Done"` | H16_NothingGoesOnPastTheWait | L18_AnswerAfterResearch, Core_HisCallIsHis, R5_ProofBeforeJudgement, R2_MergingHolderHasReview, R17_VerdictAndPrLineTogether, R20_DoneOnlyOnceLanded |
| r2 probe | p-skip-red-rerun | `GateRed`: `/\ state' = "Rerunning"` → `/\ state' \in {"Rerunning", "Done"}` | R20_DoneOnlyOnceLanded | R22_RerunOnce |
| r2 probe | p-repeat-extra-at-four | `OneMoreRound`: `/\ ~extra` → `/\ (~extra \/ round = 4)` | R8_ExtraRoundOnlyOnce | R8_FixBeforeNextRound |
| r2 probe | p-repeat-scout (its guard was ~scouted) | `ScoutTheSpec`: `/\ scouts = 0` → `/\ TRUE` | R4_OneScoutPass | TypeOK |
| r2 probe | p-false-verdict (two lines, as the reviewer wrote it: NoMoreRound records a broken blocker as it merges) | `NoMoreRound`: `/\ verdict' = "none"` → `/\ verdict' = "none" /\ blockers' = {"broken"}`; `NoMoreRound`: `round, extra, blockers, cheap` → `round, extra, cheap` | R11_VerdictHoldsItsBlockers |  |
| r2 probe | p-hiscall-closes (B1 probe: a his-call closes with no answer from him) | `CloseLeaf`: `/\ call \in {"none", "settled"}` → `/\ TRUE` | Core_HisCallIsHis | L18_AnswerAfterResearch |
| r2 probe | p-part-back-open (B2 probe: the part comes back with its Question open) | `PartBack`: `/\ ~partwaits` → `/\ TRUE` | H16_ThePartWaitsOnItsAnswer | H10_FinishWhenTheContractIsMet, H16_NothingGoesOnPastTheWait |
| r2 probe | p-mixed-drops-him (B2 probe: {fundamental, him} at round 3 drops the Question) | `CheckpointEscalates`: `/\ asked' = IF "him" \in blockers THEN "research" ELSE asked` → `/\ asked' = asked` | R9_BlockersRoutedByKind |  |
| r2 probe | p-round-unproved (B4 probe: the round after a Fix starts with no proof) | `ReviewAgain`: `/\ proved` → `/\ TRUE` | R5_ProofBeforeJudgement | H16_NothingGoesOnPastTheWait |
| r2 probe | p-red-unseen (B6 probe: a repair before any red result) | `RedBeforeLanding`: `/\ gates = "red" \/ ci = "red"` → `/\ TRUE` | R21_RepairOnlyForAConflictOrARed |  |
| r2 probe | p-click-behind (B6 probe: his click lands a PR master has moved past) | `HisClick`: `/\ ~behind` → `/\ TRUE` | R29_RecheckJustBeforeLanding |  |
| r2 probe | p-verdictless-close (new blocker probe: a verdict ticket closes with no verdict) | `CloseTheWhole`: `/\ kind = "verdict" => verdict \in {"pass", "fail"}` → `/\ TRUE` | R17_VerdictAndPrLineTogether |  |
| r2 probe | p-merger-writes-pass (new blocker probe: a PASS verdict with no PR line) | `TheReviewPasses`: `/\ pass' = "head"` → `/\ pass' = pass` | R17_VerdictAndPrLineTogether | NoDeadlock |
| This fix | m01 | `Take`: `/\ state' = "Taken"` → `/\ state' \in {"Taken", "Done"}` | WorkPhaseBeforeDone |  |
| This fix | m02 | `Take`: `/\ kind' \in IF filler THEN {"change"} ELSE Kinds` → `/\ kind' \in IF filler THEN {"change"} ELSE Kinds \cup {"unread"}` | K30_KindKnownBeforeWork | S7_UnstartedReturnHasNoWork |
| This fix | m03 | `WorkItself`: `/\ UNCHANGED <<ticket, work, split, hop, question, review, merge>>` → `/\ kind' \in Kinds /\ UNCHANGED <<stage, merges, filler, door, work, split, hop, question, review, merge>>` | K30_KindStaysKnown | TypeOK, H23_EveryQuestionHasAStage, R34_VerdictNeverReviewed, Sees_OnlyAMergerSeesReviewFixMerge, R4_NoReviewLoopOnACut, L18_ChildOnlyAfterResearch, R17_VerdictAndPrLineTogether, S8_OneVerdictOnceTheSeatsAreBack, R23_PassStandsOnItsHead |
| This fix | m04 | `CannotTake`: `/\ state' = "HandedBack"` → `/\ state' \in {"HandedBack", "Leaf"}` | S7_UnstartedReturnHasNoWork | K30_KindKnownBeforeWork |
| This fix | m05 | `HandBackTheWork`: `/\ state = "Leaf"` → `/\ state \in {"Leaf", "Done"}` | H1_EndsAreFinal |  |
| This fix | m06 | `Take`: `/\ stage' \in IF kind' = "answer" THEN Stages ELSE {"none"}` → `/\ stage' \in Stages \cup {"none"}` | H23_EveryQuestionHasAStage | TypeOK, NoDeadlock, R34_SeatNeverDelegates, H10_FinishWhenTheContractIsMet, L18_ChildOnlyAfterResearch, L18_AnswerAfterResearch, Core_HisCallIsHis, L18_CouncilSteps, R2_MergingHolderHasReview, R17_VerdictAndPrLineTogether, R20_DoneOnlyOnceLanded |
| This fix | m07 | `HandBackTheWhole`: `/\ state' = "HandedBack"` → `/\ state' = "Taken"` | H10_ReturnedPartContinues | Core_SplitOnlyOrchestrates, H16_NothingGoesOnPastTheWait |
| This fix | m08 | `CutTheNextParts`: `/\ part \in {"back", "returned"}` → `/\ TRUE` | H10_APartIsBackBeforeTheNextCut | TypeOK, H1_EndsAreFinal, H3_DispatchAndWaitAfterSplit, R6_EvidenceCheckBesideAPart, S24_StoppedHopRecovery |
| This fix | m09 | `TheWayIsFound`: `/\ part = "back"` → `/\ TRUE` | H10_APartIsBackBeforeTheNextCut |  |
| This fix | m10 | `CloseTheWhole`: `/\ ~fog` → `/\ TRUE` | H10_FinishWhenTheContractIsMet |  |
| This fix | m11 | `NeedFromAPart`: `/\ part = "out"` → `/\ part \in {"out", "back"}` | H16_ThePartWaitsOnItsAnswer | H10_FinishWhenTheContractIsMet, H16_NothingGoesOnPastTheWait |
| This fix | m12 | `TheCheckFails`: `/\ part' = "fixing"` → `/\ part' = "wiring"` | R6_EvidenceCheckBesideAPart | H10_FinishWhenTheContractIsMet |
| This fix | m13 | `TheReviewPasses`: `/\ hop = "live"` → `/\ TRUE` | S24_StoppedHopRecovery | TypeOK, H1_EndsAreFinal |
| This fix | m14 | `TheFixIsProved`: `/\ hop = "live"` → `/\ TRUE` | S24_StoppedHopRecovery |  |
| This fix | m15 | `Answered`: `/\ asked' = "none"` → `/\ asked' = asked` | H16_ResumeLeafAfterQuestion | L18_AnswerAfterResearch |
| This fix | m16 | `WaitOnTheAnswer`: `/\ asked \in Stages` → `/\ asked # "none"` | H16_WaitsOnAnOpenQuestion |  |
| This fix | m17 | `Fail`: `/\ asked' = IF "him" \in blockers THEN "research" ELSE asked` → `/\ asked' = "research"` | H16_TheReceivingOfficerOpensIt |  |
| This fix | m18 | `SendTheWholeToReview`: `/\ asked = "none"` → `/\ TRUE` | H16_NothingGoesOnPastTheWait | H10_FinishWhenTheContractIsMet |
| This fix | m19 | `AskHim`: `/\ stage = "research"` → `/\ TRUE` | L18_ChildOnlyAfterResearch | Core_HisCallIsHis |
| This fix | m20 | `CouncilPrepared`: `/\ call' = "his"` → `/\ call' = "settled"` | L18_AnswerAfterResearch |  |
| This fix | m21 | `TheCouncilDecides`: `/\ tribunal = "agreed"` → `/\ tribunal \in {"agreed", "split"}` | L18_CouncilExits |  |
| This fix | m22 | `WorkItself`: `/\ stage # "council"` → `/\ TRUE` | L18_ACouncilIsNeverOneAgent | L18_CouncilSteps |
| This fix | m23 | `Take`: `/\ call' = CASE stage' = "research" -> "open" [] stage' = "his-call" -> "his" [] OTHER -> "none"` → `/\ call' = CASE stage' = "research" -> "open" [] stage' = "his-call" -> "settled" [] OTHER -> "none"` | Core_HisCallIsHis |  |
| This fix | m24 | `WriteTheVerdict`: `/\ kind = "verdict"` → `/\ kind \in {"verdict", "cut"}` | R4_NoReviewLoopOnACut | R33_TheVerdictIsTheReviewHops |
| This fix | m25 | `Fail`: `/\ proved' = FALSE` → `/\ proved' = proved` | R5_ProofBeforeJudgement | H16_NothingGoesOnPastTheWait |
| This fix | m26 | `RepairReviewed`: `/\ proved` → `/\ TRUE` | R5_ProofBeforeJudgement | R19_ReviewAfterMergeRepair |
| This fix | m27 | `ReviewTheMendedHead`: `/\ proved` → `/\ TRUE` | R5_ProofBeforeJudgement | R23_ReviewMendedHead |
| This fix | m28 | `SendLeafToReview`: `/\ verdict' = "due"` → `/\ verdict' = "pass"` | R33_TheVerdictIsTheReviewHops | NoDeadlock, R17_VerdictAndPrLineTogether, R8_RoundsCountUp |
| This fix | m29 | `CloseLeaf`: `/\ kind = "verdict" => verdict \in {"pass", "fail"}` → `/\ TRUE` | R17_VerdictAndPrLineTogether |  |
| This fix | m30 | `TheReviewFails`: `/\ blockers' \in (SUBSET Blockers) \ {{}}` → `/\ blockers' \in SUBSET Blockers` | R11_VerdictHoldsItsBlockers |  |
| This fix | m31 | `TheReviewPasses`: `/\ blockers' = {}` → `/\ blockers' \in SUBSET Blockers` | R11_VerdictHoldsItsBlockers |  |
| This fix | m32 | `NoMoreRound`: `/\ verdict = "pass"` → `/\ verdict \in {"pass", "fail"}` | R8_PassBeforeMerge | R11_VerdictHoldsItsBlockers, NoDeadlock, R9_BlockersRoutedByKind, R10_CheckpointAtRoundThree, R10_EscalateOnlyForFundamentalOrHim |
| This fix | m33 | `OneMoreRound`: `/\ round' = NextRound` → `/\ round' = round` | R8_RoundsCountUp |  |
| This fix | m34 | `HandBackTheFix`: `/\ state' = "HandedBack"` → `/\ state' = "Merging"` | R8_FixBeforeNextRound | R11_VerdictHoldsItsBlockers, S17_CheapShouldMendedBeforeMerge, NoDeadlock, H16_NothingGoesOnPastTheWait, R8_PassBeforeMerge, R8_PassEndsTheRounds |
| This fix | m35 | `Fail`: `/\ verdict = "fail"` → `/\ verdict \in {"due", "fail"}` | R8_PassEndsTheRounds | S24_StoppedHopRecovery, R19_ReviewAfterMergeRepair |
| This fix | m36 | `MendACheapShould`: `/\ state' = "Mending"` → `/\ state' = "Fixing"` | R8_PassEndsTheRounds | S10_CheckpointBeforeFourthRound |
| This fix | m37 | `Fail`: `/\ "fundamental" \notin blockers` → `/\ TRUE` | R9_BlockersRoutedByKind |  |
| This fix | m38 | `Replan`: `/\ asked' = IF "him" \in blockers THEN "research" ELSE asked` → `/\ asked' = asked` | R9_BlockersRoutedByKind |  |
| This fix | m39 | `CheckpointAsksHim`: `/\ checkpoint' = "escalated"` → `/\ checkpoint' = "one more round"` | R10_EscalateOnlyForFundamentalOrHim |  |
| This fix | m40 | `MendACheapShould`: `/\ cheap` → `/\ TRUE` | R23_ReviewMendedHead |  |
| This fix | m41 | `OneMoreRound`: `/\ pass' = "none"` → `/\ pass' = pass` | R23_PassStandsOnItsHead | R17_VerdictAndPrLineTogether |
| This fix | m42 | `RealRed`: `/\ pass' = "none"` → `/\ pass' = pass` | R23_PassStandsOnItsHead | R17_VerdictAndPrLineTogether |
| This fix | m43 | `ResyncOnTheRecheck`: `/\ pass' = "ancestor"` → `/\ pass' = "head"` | R23_PassStandsOnItsHead |  |
| This fix | m44 | `TheRepairIsProved`: `/\ hunks' = TRUE` → `/\ hunks' = hunks` | R19_ReviewAfterMergeRepair |  |
| This fix | m45 | `TheReviewPasses`: `/\ hunks' = FALSE` → `/\ hunks' = hunks` | R19_ReviewAfterMergeRepair |  |
| This fix | m46 | `RepairReviewed`: `/\ state' = "InReview"` → `/\ state' = "Merging"` | R19_ReviewAfterMergeRepair | NoDeadlock, R8_PassBeforeMerge, R8_RoundsCountUp |
| This fix | m47 | `MergeConflict`: `/\ gates = "conflict"` → `/\ TRUE` | R21_RepairOnlyForAConflictOrARed |  |
| This fix | m48 | `Flake`: `/\ gates' = "green"` → `/\ gates' = gates` | R22_RerunOnce | R20_DoneOnlyOnceLanded |
| This fix | m49 | `GreenOnTheMergeSha`: `/\ ci' = "green"` → `/\ ci' = ci` | R20_DoneOnlyOnceLanded |  |
| This fix | m50 | `OpenTheHitlMerge`: `/\ gates = "green"` → `/\ TRUE` | R31_ClickOnlyAfterGreen | R23_MergeRestsOnItsEvidence |
| This fix | m51 | `OfferForTheTurn`: `/\ ci = "green"` → `/\ TRUE` | R26_TwoWayEvidenceBeforeTheTurn | R23_MergeRestsOnItsEvidence |
| This fix | m52 | `MergeIntoFeature`: `/\ target = "feature"` → `/\ target \in {"feature", "two-way"}` | R29_TwoWayMainWaitsForItsTurn | R23_MergeRestsOnItsEvidence |
| This fix | m53 | `LandOnTheTurn`: `/\ ~behind` → `/\ TRUE` | R29_RecheckJustBeforeLanding |  |
| This fix | m54 | `ResyncOnTheRecheck`: `/\ behind` → `/\ TRUE` | R29_HeldOnTheThirdResync |  |
| This fix | m55 | `OfferForTheTurn`: `/\ target = "two-way"` → `/\ target \in Master` | R27_EachWaitHasItsTarget | Core_OneWayWaitsForHisClick |
| This fix | m56 | `HeDecides`: `/\ UNCHANGED <<state, ticket, work, split, hop, asked, review, merge>>` → `/\ state' = "Done" /\ UNCHANGED <<ticket, work, split, hop, asked, review, merge>>` | Core_HisCallIsHis | TypeOK, H1_EndsAreFinal, H10_FinishWhenTheContractIsMet, S24_StoppedHopRecovery, H16_NothingGoesOnPastTheWait |
| This fix | m57 | `ScoutTheSpec`: `/\ kind = "cut"` → `/\ TRUE` | R4_OneScoutPass |  |
| This fix | m58 | `Take`: `IF kind' = "change" THEN BOOLEAN` → `IF kind' \in {"change", "verdict"} THEN BOOLEAN` | R34_VerdictNeverReviewed | TypeOK, Sees_OnlyAMergerSeesReviewFixMerge, R17_VerdictAndPrLineTogether, S8_OneVerdictOnceTheSeatsAreBack, R23_PassStandsOnItsHead |
| This fix | m59 | `TheCheckPasses`: `/\ part' = "back"` → `/\ part' = "wired"` | R6_EvidenceCheckBesideAPart |  |
| This fix | m60 | `HisClick`: `/\ state = "AwaitingClick"` → `/\ state \in {"AwaitingClick", "Merging"}` | R23_MergeRestsOnItsEvidence | Core_OneWayWaitsForHisClick, R29_TwoWayMainWaitsForItsTurn |
| This fix | m61 | `TheReviewFails`: `/\ hunks' = FALSE` → `/\ hunks' = hunks` | R19_ReviewAfterMergeRepair |  |
| This fix | m62 | `ReviewAgain`: `ELSE "full"` → `ELSE "scoped"` | R9_LensAfterAFix | R19_ReviewAfterMergeRepair |
| This fix | m63 | `OneMoreRound`: `/\ extra' = TRUE` → `/\ extra' = extra` | R8_ExtraRoundOnlyOnce | R8_FixBeforeNextRound |
| This fix | m64 | `SendTheWholeToReview`: `/\ round' = 1` → `/\ round' = round` | R8_RoundsCountUp |  |
| This fix | m65 | `CheckpointOneMoreRound`: `/\ checkpoint' = "one more round"` → `/\ checkpoint' = "escalated"` | R10_EscalateOnlyForFundamentalOrHim |  |
| This fix | m66 | `TheReviewPasses`: `/\ verdict = "due"` → `/\ verdict \in {"due", "fail"}` | R33_TheVerdictIsTheReviewHops | R9_BlockersRoutedByKind, R10_CheckpointAtRoundThree, R10_EscalateOnlyForFundamentalOrHim |
| This fix | m67 | `ConflictOnTheRecheck`: `/\ gates' = "conflict"` → `/\ gates' = gates` | R29_RecheckJustBeforeLanding |  |
| This fix | m68 | `Sequenced`: `/\ state' = "Merging"` → `/\ state' = "AwaitingTurn"` | R26_TwoWayEvidenceBeforeTheTurn | R27_EachWaitHasItsTarget, R23_MergeRestsOnItsEvidence, Core_OneWayWaitsForHisClick |
| This fix | m69 | `GateRed`: `/\ gates' = "red"` → `/\ gates' = gates` | R22_RerunOnce | R21_RepairOnlyForAConflictOrARed, R22_RevertOnlyAfterTheMerge |
| This fix | m70 | `RealRed`: `/\ state = "Rerunning"` → `/\ state \in {"Rerunning", "Landed"}` | R22_RevertOnlyAfterTheMerge | R21_RepairOnlyForAConflictOrARed |
| This fix | m71 | `CouncilDecided`: `/\ tribunal' = "decided"` → `/\ tribunal' = "prepared"` | L18_AnswerAfterResearch |  |
| This fix | m72 | `CloseTheWhole`: `/\ stage # "council"` → `/\ TRUE` | L18_CouncilSteps | L18_CouncilExits |
| This fix | m73 | `HisClick`: `/\ landed' = "in"` → `/\ landed' = landed` | R20_DoneOnlyOnceLanded | R22_RevertOnlyAfterTheMerge, R22_RerunOnce |
| This fix | m74 | `WriteTheVerdict`: `\/ state = "Waiting" /\ part = "back"` → `\/ state = "Waiting"` | S8_OneVerdictOnceTheSeatsAreBack | S24_StoppedHopRecovery |
| This fix | m75 | `CloseLeaf`: `/\ asked = "none"` → `/\ TRUE` | H16_NothingGoesOnPastTheWait |  |
| This fix | m76 | `TheJudgesSplit`: `/\ tribunal = "exchange"` → `/\ tribunal \in {"exchange", "sealed"}` | L18_CouncilSteps |  |
| This fix | m77 | `Sequenced`: `/\ state = "Held"` → `/\ state \in {"Held", "AwaitingTurn"}` | R29_HeldOnTheThirdResync |  |

The view controls run `check-chart.mjs` on the scratch copy, which must exit 1 (c4, a changed comment, must exit 0). The column gives the first problem it reports.

| For | Mutant | Changed line | What check-chart.mjs reports |
|---|---|---|---|
| View | v1 | `view-map.mjs`: `Mending: "InReview",` → `Mending: "Fixing",` | exit 1: a model step the map does not place there: MendACheapShould, InReview to Mending, at InReview --> Fixing |
| View | v2 | `view-map.mjs`: `CloseLeaf: ["Leaf --> Done"],` → `CloseLeaf: ["Leaf --> HandedBack"],` | exit 1: a model step the map does not place there: CloseLeaf, Leaf to Done, at Leaf --> Done |
| View | v3 | `view-map.mjs`: `RepairFails: ["Fixing --> HandedBack"],` → (removed) | exit 1: model action the map gives no place: RepairFails |
| View | v4 (two actions off, as before (v4 with v4b)) | `HandBackTheFix`: `/\ state = "Fixing"` → `/\ FALSE`; `RepairFails`: `/\ state = "Repairing"` → `/\ FALSE` | exit 1: model action no reachable step takes: HandBackTheFix |
| View | v5 | `ReviewTheMendedHead`: `/\ state' = "InReview"` → `/\ state' = "Fixing"` | exit 1: a model step the map does not place there: ReviewTheMendedHead, Mending to Fixing, at InReview --> Fixing |
| View | v6 | `chart.mmd`: `[*] --> Dispatched` → `[*] --> Dispatched Taken --> Done` | exit 1: chart.mmd:25: an arrow with no words: Taken --> Done |
| View | v7 | `view-map.mjs`: `OneMoreRound: ["InReview"],` → `OneMoreRound: ["Merging"],` | exit 1: a model step the map does not place there: OneMoreRound, InReview to InReview, at InReview |
| View | v8 | `chart.mmd`: `[*] --> Dispatched` → `[*] --> Dispatched Taken --> Done: it is done` | exit 1: chart.mmd's visible words are not the words Balazs approved (sha256 a85cccc881963fdfc05ae81d3dff749f6f73402438be3a39aab19cc754843a57, approved f5a0b99c11799be14 |
| View | v9 | `view-map.mjs`: `RunTheGates: ["Merging"],` → `RunTheGates: ["Fixing"],` | exit 1: a model step the map does not place there: RunTheGates, Merging to Merging, at Merging |
| View | v10 | `chart.mmd`: `[*] --> Dispatched` → `[*] --> Dispatched state Taken { }` | exit 1: chart.mmd:25: a line the check cannot read: state Taken { |
| r2 probe | p-false-label | `chart.mmd`: `InReview --> Merging: the verdict is PASS` → `InReview --> Merging: the verdict is FAIL` | exit 1: chart.mmd's visible words are not the words Balazs approved (sha256 a64bf2f35be26490583cddf14ae68593a2da04b8105423894716482a5cf57162, approved f5a0b99c11799be14 |
| r2 probe | p-end-hides-step | `Ended`: `state \in Ends /\ UNCHANGED vars` → `(state \in Ends /\ UNCHANGED vars) \/ (state = "Taken" /\ state' = "Done" /\ UNCHANGED <<ticket, work, split, hop, question, review, merge>>)` | exit 1: a step named Ended that is not standing still at an end: Taken to Done, a different state |
| r2 probe | p-stale-model | `WorkItself`: `/\ stage # "council"` → `/\ FALSE` | exit 1: TLC did not complete without error (exit 11): |
| This fix | c1 | `Captain.cfg`: `INVARIANT TypeOK` → `INVARIANT TypeOK CONSTRAINT state # "Merging"` | exit 1: TLC did not complete without error (exit 151): |
| This fix | c2 | `Captain.cfg`: `INVARIANT TypeOK` → (removed) | exit 1: Captain.cfg must say only INIT Init, NEXT Next and INVARIANT TypeOK, so that nothing is pruned; it says: INIT Init; NEXT Next |
| This fix | c3 | `Captain.cfg`: `INVARIANT TypeOK` → `INVARIANT TypeOK CHECK_DEADLOCK FALSE` | exit 1: Captain.cfg must say only INIT Init, NEXT Next and INVARIANT TypeOK, so that nothing is pruned; it says: INIT Init; NEXT Next; INVARIANT TypeOK; CHECK_DEADLOCK  |
| This fix | c4 | `chart.mmd`: `%% Each arrow is one case.` → `%% Each arrow is one case, as he approved it.` | exit 0, as it must: chart.mmd is a faithful view of the model, on every reachable step: its words are the approved words; 18 model states shown as 11 chart states, 85 actions under |
| This fix | c5 | `chart.mmd`: `Merging: Merging its change` → `Merging: Merging its changes` | exit 1: chart.mmd's visible words are not the words Balazs approved (sha256 03dccecd059ce10b1be91a7b680b066f72e68d4ff27d752ce1921256fb195d37, approved f5a0b99c11799be14 |
| This fix | c6 | `view-map.mjs`: `export const standStill = "Ended";` → `export const standStill = "Flake";` | exit 1: model action the map gives no place: Ended |
| This fix | c7 | `Ended`: `state \in Ends /\ UNCHANGED vars` → `UNCHANGED vars` | exit 1: a step named Ended that is not standing still at an end: Dispatched to Dispatched |
| This fix | c8 | `Next`: `\/ Ended` → `\/ Ended \/ state = "Taken" /\ state' = "Done" /\ UNCHANGED <<ticket, work, split, hop, question, review, merge>>` | exit 1: a model step whose action the map does not name: Next, Taken to Done |
| This fix | c9 | `view-map.mjs`: `Taken: "Taken",` → `Taken: "Leaf",` | exit 1: a model step the map does not place there: Take, Dispatched to Taken, at Dispatched --> Leaf |
| This fix | c10 | `view-map.mjs`: `(TheReviewPasses, TheReviewFails)` → `(TheReviewPasses, TheReviewFail)` | exit 1: a listed mismatch names what the model lacks: TheReviewFail |
| This fix | c11 | `ScoutTheSpec`: `/\ scouts' = scouts + 1` → `/\ scouts' = scouts` | exit 1: a step of ScoutTheSpec that changes nothing, at Leaf: every step but the stand-still must change the model's state |

Review r2's graph witnesses (`probe.py` in its scratch folder) were rerun on the fixed model's TLC graph, adapted to the new records: each bad witness finds no match, and each missing outcome is reached (B1 his-call closes on his answer; B3 a wayfinding part back, then the ticket finishes with no further cut; B6 the re-check from his click, and a clean re-sync after the hold). Its stale-graph probe, `check-chart.mjs --dot <graph>`, exits 2: the checker takes no graph.

## Receipts

Each command ran under the gate lock and the agent memory cap, with dydo 3.6.0's managed Java and TLC, on the committed head named in the DYD-1502 return.

- The view check (`node statechart/captain/check-chart.mjs`): exit 0. TLC on `Captain.cfg`: `TypeOK` and `NoDeadlock` hold; 80,275 states generated, 43,324 distinct, depth 46; 72,003 steps checked; the words are the approved words; 18 model states shown as 11 chart states, 85 actions under 23 arrows or inside a state. It lists three cases for Balazs: `Mending` under `InReview`, `Waiting` on the captain's own need, and `InReview` once its verdict is in.
- The statechart tool on the model (`dydo statechart ... --view view.mjs`): exit 0, `TypeOK` and `NoDeadlock` hold; more than 500 states, so no chart is drawn.
- The properties (`run.py --machine captain`): exit 0, result pass. All 62 registered properties, `TypeOK` and `NoDeadlock` hold: 64 rules; 233,998 states generated, 127,534 distinct, queue empty, depth 50.
- The registered control (`run.py --controls --machine captain`): harness exit 0, `control_caught` true. H16_ResumeLeafAfterQuestion rejects `Asking` to `Done`, and so do WorkPhaseBeforeDone and H16_NothingGoesOnPastTheWait.
- The 138 hand-run model controls and the 24 view controls: each as its table says. The graph witnesses: as above.

Output was written under `/tmp/dyd1492/fix2/` and is not kept.

## Related

- [The captain statechart](../../../statechart/captain/README.md)
- [Statechart property runner](../README.md)
