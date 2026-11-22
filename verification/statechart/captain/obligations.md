---
area: reference
type: reference
explains: ["verification/statechart/captain/**"]
---

# Captain chart obligations

Hand-written from his recorded words, the approved spec and the core page (`dydo/core.md`), never from the model (K3). The model they judge is the comprehensive captain model in [statechart/captain/](../../../statechart/captain/README.md) (DYD-1492): 18 states, 75 actions and 26 variables. A property reads the model's variables as the records its source names: the ticket's result and stage, the Question the work waits on, the verdict's blockers, the checkpoint line, the PASS, the gates and CI, the landing. The simple chart Balazs approved is a view of that model, and `check-chart.mjs` proves the view faithful; these properties judge the model.

`seen` is the passive set of the states `Leaf`, `Cutting`, `Waiting` and `InReview` this captain has been in. No step is disabled, the model needs no bound (the round count stops at four, which stands for four or more), and no fairness is added.

## Properties

`ReviewPhase` is `InReview`, `Fixing`, `Mending` and `Repairing`. `MergePhase` is `Merging`, `AwaitingTurn`, `Held`, `AwaitingClick`, `Landed` and `Rerunning`. Together they are the merger's states. A FAIL is a step out of `InReview` to `Fixing` or `HandedBack`; a step lands the merge when `landed` becomes `in`; a round starts when the captain enters `InReview` or the round number changes there.

| Property | Source | What it checks |
|---|---|---|
| WorkPhaseBeforeDone | core "A ticket"; 7b3d2e3e | Done comes after `Leaf` or `Waiting`. |
| K30_KindKnownBeforeWork | K30, S6 | Every state but `Dispatched` and `HandedBack` has a known kind. |
| K30_KindStaysKnown | K30, S7; AGENTS.md | Once set, the kind never changes. |
| S7_UnstartedReturnHasNoWork | S6, S7 | A run that never read its kind stays in `Dispatched` and `HandedBack`. |
| H11_FillerIsAChange | H11 | A filler that is taken is a change. |
| H1_EndsAreFinal | H12; AGENTS.md | `Done` and `HandedBack` only stand still. A structural check of the two ends. |
| H23_EveryQuestionHasAStage | H23 | A Question has a stage, research, council or his-call, and no other ticket has one. |
| Core_SplitOnlyOrchestrates | core "A ticket" | After `Cutting`, never `Leaf` or `Asking` again. |
| H3_DispatchAndWaitAfterSplit | H3; H2; df6a785e | `Cutting` exits only to `Waiting`, with the part dispatched, or to `HandedBack`. |
| H10_ReturnedPartContinues | H2, H10, R2 | `Waiting` exits to `Cutting`, `InReview`, `Done` or `HandedBack`. |
| H10_APartIsBackBeforeTheNextCut | H10, H2 | The next cut, and a hand-back of the whole, come only with a part back or handed back. |
| H10_FinishWhenTheContractIsMet | H10, R2, S8 | The whole closes or goes to review only with its parts back, no fog and no open Question; to review only once wired; the wiring starts only from parts that are back. |
| R6_EvidenceCheckBesideAPart | R6, S4, S17, S9 | A part's evidence check starts only once it is back, and ends with the part standing, mending a cheap should, or fixing its blockers. |
| S24_StoppedHopRecovery | S24 | A stopped hop is set back to Todo, then resumed; nothing moves until it is live again. |
| R34_SeatNeverDelegates | R34 | A seat is never in `Cutting` or `Waiting`. |
| H16_ResumeLeafAfterQuestion | core "Who decides what"; H16; 519126ab | `Asking` exits only to `Leaf`, and only with the answer on the Question. |
| H16_WaitsOnAnOpenQuestion | H16, H23 | A captain in `Asking` waits on an open Question with a stage. |
| H16_TheReceivingOfficerOpensIt | H16, L18, R9, S10 | A research Question opens for a need passed up, opened by the officer above, or by this captain only as the receiving officer: for a part's need or a reviewer's blocker that needs Balazs. |
| H16_NothingGoesOnPastTheWait | H16, S9 | Nothing closes and no round starts while the work waits on an answer. |
| L18_ChildOnlyAfterResearch | L18, H23, L20 | Only a research Question's captain opens a council or his-call child, and only after its research left the call to that child. |
| L18_AnswerAfterResearch | L18, H16 | A research Question's call is settled only by its research or its child's answer, and it closes only with its call settled. |
| L18_CouncilSteps | L18, L23 | A council runs sealed positions, then one exchange, then the judges agree or split, and closes decided or prepared. |
| L18_CouncilExits | L18, L21 | A council decides only when its judges agree on a two-way call, or on the overnight DR-grade case; it prepares when they split or the call is DR-grade or one-way; agreed on a two-way call, it decides. |
| L18_ACouncilIsNeverOneAgent | L18 | A council's chair is never in `Leaf` or `Asking`. |
| R34_VerdictNeverReviewed | core "Review severities"; R34; 777d396e | A verdict's holder and a seat are never in a merger's state. |
| Sees_OnlyAMergerSeesReviewFixMerge | `statechart/captain/positions.md` | Only a captain that merges a change is in a merger's state. |
| R4_NoReviewLoopOnACut | R4 | A cut is never in a merger's state. |
| R5_ProofBeforeJudgement | R5; core "Who decides what" | The work goes to review, and a change closes as done, only proved. |
| R2_MergingHolderHasReview | core "Who decides what"; R2, R23 | A captain that merges closes only from `MergePhase`, with `InReview` behind it. |
| R8_PassBeforeMerge | R8, R23 | `MergePhase` starts only from `InReview`, and a reviewed ticket closes only from `MergePhase`. |
| R8_RoundsCountUp | R8, R10, S3 | Each round's number is one more than the last, from 1, four standing for four or more; only a new round changes it. |
| R8_FixBeforeNextRound | core "Review severities"; R8, S9 | A round starts only from a Fix, a repair, a mend, the one more round after a PASS, or the work's first round; `Fixing` exits to `InReview` or `HandedBack`. |
| R8_PassEndsTheRounds | R8, R9, S17 | `InReview` exits only to `Fixing`, `HandedBack`, `Merging` or `Mending`, and to `Merging` only with no cheap should outstanding. |
| R8_ExtraRoundOnlyOnce | R8 | One more round after a PASS is ordered at most once. |
| R9_BlockersRoutedByKind | R9, R11, S10, H16 | A FAIL holds a blocker; it goes to the officer above exactly when one is fundamental; a need for Balazs opens a Question the Fix waits on. |
| R9_LensAfterAFix | R8, R9 | The round after a Fix looks with the full lenses, with the security lens first exactly when a blocker was a security hole. |
| R10_CheckpointAtRoundThree | R10, S10 | A third round's FAIL writes the checkpoint line, nothing else does, and the round stays 3. |
| R10_EscalateOnlyForFundamentalOrHim | R10, S10 | At the checkpoint it escalates exactly when a blocker is fundamental or needs him; otherwise one more round. |
| S10_CheckpointBeforeFourthRound | R10, S10 | A fourth round after a third FAIL comes after the checkpoint line. |
| S17_CheapShouldMendedBeforeMerge | S17 | No merge state has a cheap should outstanding. |
| R23_ReviewMendedHead | S17, R23 | `Mending` starts only from `InReview` and exits only to `InReview`, with the should gone. |
| R23_PassComesFromAVerdict | R23, R24 | A PASS on the head comes only from a round, one on an ancestor only from a PASS that stands, and none stands in `ReviewPhase`. |
| R23_MergeRestsOnItsEvidence | R23, R24, R26, S17 | A merge lands only on a standing PASS, green gates, no cheap should, and green CI into master. |
| R19_ReviewAfterMergeRepair | R19, R21, R22 | `Repairing` starts only from `MergePhase` and exits to `InReview` with a scoped round, or to `HandedBack`. |
| R22_RevertOnlyAfterTheMerge | R21, R22 | A revert comes only after a landing and its rerun; a repair before the landing reverts nothing. |
| R22_RerunOnce | R22 | `Rerunning` starts only from a landing, exits only to `Done` or `Repairing`, and a real red is reverted. |
| R20_DoneOnlyOnceLanded | R20 | A captain that merges closes only with its merge in, from `Landed` or `Rerunning`. |
| Core_OneWayWaitsForHisClick | core "Who decides what"; R27 | A one-way merge lands only from `AwaitingClick`. |
| R31_ClickOnlyAfterGreen | R31, R23 | `AwaitingClick` starts only with CI green, gates green and the PASS standing. |
| R26_TwoWayEvidenceBeforeTheTurn | R26 | `AwaitingTurn` starts only with CI green, gates green and the PASS standing. |
| R29_TwoWayMainWaitsForItsTurn | R26, R29 | A two-way PR into master lands only from `AwaitingTurn`. |
| R29_HeldOnTheThirdResync | R29 | A re-sync happens only at the re-check on the turn, one at a time; the third is held, and only it. |
| R27_EachWaitHasItsTarget | R27, R29 | Only a one-way PR is in `AwaitingClick`, and only a two-way PR into master in `AwaitingTurn` or `Held`. |

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
| R19_ReviewAfterMergeRepair checked exit and lens | It also keeps the repair to the merge; R23_PassComesFromAVerdict and R23_MergeRestsOnItsEvidence make the repaired head land only on a later PASS. |
| R8_PassEndsTheRounds, R23_ReviewMendedHead on `Clear` | On the PASS step out of `InReview`, with the cheap should carried. |
| R22_RerunOnce, R20 not checked | R22_RerunOnce, R22_RevertOnlyAfterTheMerge, R20_DoneOnlyOnceLanded split the landing from what comes before it. |

New: H23_EveryQuestionHasAStage, R6_EvidenceCheckBesideAPart, S24_StoppedHopRecovery, H16_WaitsOnAnOpenQuestion, H16_TheReceivingOfficerOpensIt, H16_NothingGoesOnPastTheWait, R4_NoReviewLoopOnACut, R5_ProofBeforeJudgement, R9_BlockersRoutedByKind, S17_CheapShouldMendedBeforeMerge, R23_PassComesFromAVerdict, R23_MergeRestsOnItsEvidence, R26_TwoWayEvidenceBeforeTheTurn and R29_HeldOnTheThirdResync. The properties PR 385 dropped stay restored in these forms; the earlier ones are in Git up to `3b1ab539`.

## What the model cannot show

The properties check order only. The model has no data for any of these, so none is checked:

- Whether the contract is met, and whether a ticket really fits one agent.
- Who an agent is: that the reviewer is not the author, comes from another model family, or is fresh; who holds the Fix or the mend.
- Whether a gate's red is real or a flake before the merge, and what a recovery plan for a one-way PR holds.
- What a verdict holds beyond the kinds that route it and a cheap should: the shoulds that wait, the nits, and that each was filed or ruled on.
- Which skill an agent loaded and what its brief said. `positions.md` is checked by reading.
- How many parts there are: one abstract part stands for them all, and one Question at a time for every Question.
- Whether a call was one the captain may decide, and whether a call is worth his time.
- The words of the checkpoint line (S10). The model has the line's kind, not what it says.
- That the scout pass on a spec runs only once: a second pass would change no value the model holds.
- That the rounds go on past the fifth FAIL (R10): the round count stops at four, which stands for four or more, and nothing in the model stops at five.
- That anything ends. Without fairness a captain may wait forever; these are safety checks.

Whether an answer may be reviewed has no source. The model gives an answer's holder no review, because only a change is merged; that is a choice of the model, checked only as part of the who-sees-what table.

## Controls

The registered control in [controls.json](../controls.json) is the step `Asking` to `Done` for a change that is not merged, waiting on an open research Question. H16_ResumeLeafAfterQuestion must reject it.

The hand-run model controls are one changed line each in a scratch copy of `Captain.tla`, with the committed model untouched, each run through `run.py`. Each must be rejected by the property its row names; the others it broke are listed too. Where the runner also reports rules "not checked", the statechart tool's re-runs after the first break stopped at its 100,000-state bound; the named property still broke.

| For | Changed line | Rejected by | Also broken |
|---|---|---|---|
| B1 | `AskACouncil` guards on `kind = "answer"` instead of `call = "council"` | L18_ChildOnlyAfterResearch | H10_FinishWhenTheContractIsMet, H16_NothingGoesOnPastTheWait |
| B1 | `TheCouncilDecides` loses its door guard | L18_CouncilExits | |
| B1 | `TheJudgesAgree` leaves from the sealed positions, skipping the exchange | L18_CouncilSteps | |
| B1 | `AskHim` loses its `call = "his"` guard | L18_ChildOnlyAfterResearch | H10_FinishWhenTheContractIsMet, H16_NothingGoesOnPastTheWait |
| B2 | `PassTheNeedUp` opens the Question itself, at stage research | H16_TheReceivingOfficerOpensIt | |
| B2 | `Fail` opens no Question for a need for Balazs | R9_BlockersRoutedByKind | |
| B2 | `Replan` hands back blockers with no fundamental one | R9_BlockersRoutedByKind | |
| B2 | `ReviewAgain` loses its `asked = "none"` guard | H16_NothingGoesOnPastTheWait | |
| B3 | `CloseTheWhole` loses its `part = "back"` guard | H10_FinishWhenTheContractIsMet | TypeOK, H1_EndsAreFinal, R6_EvidenceCheckBesideAPart, S24_StoppedHopRecovery |
| B3 | `WireTheParts` loses its `part = "back"` guard | H10_FinishWhenTheContractIsMet | R6_EvidenceCheckBesideAPart, S24_StoppedHopRecovery |
| B3 | `ResumeTheHop` resumes a stopped hop without Todo | S24_StoppedHopRecovery | NoDeadlock |
| B3 | `PartBack` loses its `hop = "live"` guard | S24_StoppedHopRecovery | TypeOK, H1_EndsAreFinal, H3_DispatchAndWaitAfterSplit |
| B3 | `Dispatch` sends no part out | H3_DispatchAndWaitAfterSplit | |
| B4 | `NoMoreRound` loses its `~cheap` guard | R8_PassEndsTheRounds | S17_CheapShouldMendedBeforeMerge, R23_MergeRestsOnItsEvidence |
| B4 | `SendLeafToReview` loses its `proved` guard | R5_ProofBeforeJudgement | |
| B4 | `CheckThePart` checks a part that is still out | R6_EvidenceCheckBesideAPart | S24_StoppedHopRecovery |
| B4 | `ReviewTheMendedHead` keeps the cheap should | R23_ReviewMendedHead | |
| B4 | `Take` lets a cut merge | R4_NoReviewLoopOnACut | TypeOK, Sees_OnlyAMergerSeesReviewFixMerge |
| B5 | `CheckpointOneMoreRound` takes a need for Balazs | R10_EscalateOnlyForFundamentalOrHim | R9_BlockersRoutedByKind |
| B5 | `CheckpointEscalates` escalates fixable blockers | R10_EscalateOnlyForFundamentalOrHim | R9_BlockersRoutedByKind |
| B5 | `ReviewAgain` always sets the full lens | R9_LensAfterAFix | |
| B5 | `CheckpointOneMoreRound` writes no line | R10_CheckpointAtRoundThree | S10_CheckpointBeforeFourthRound |
| B5 | `Fail` loses its `round # 3` guard | R10_CheckpointAtRoundThree | R10_EscalateOnlyForFundamentalOrHim, S10_CheckpointBeforeFourthRound |
| B6 | `GateRed` fires before the landing | R22_RerunOnce | R22_RevertOnlyAfterTheMerge, R20_DoneOnlyOnceLanded (others not checked) |
| B6 | `HisClick` goes straight to `Done` | R20_DoneOnlyOnceLanded | |
| B6 | `MergeIntoFeature` loses its `gates` guard | R23_MergeRestsOnItsEvidence | |
| B6 | `OpenTheHitlMerge` loses its `ci` guard | R31_ClickOnlyAfterGreen | R23_MergeRestsOnItsEvidence |
| B6 | `ResyncOnTheTurn` never holds | R29_HeldOnTheThirdResync | |
| B6 | `RealRed` reverts nothing | R22_RerunOnce | |
| B6 | `RedBeforeLanding` reverts | R22_RevertOnlyAfterTheMerge | |
| B9 | `ReviewAgain` keeps `round' = round` (the r1 reviewer's) | R8_RoundsCountUp | |
| B9 | `RepairReviewed` starts round 1 again | R8_RoundsCountUp | (others not checked) |
| Earlier | `Replan` loses its `round # 3` guard | R10_CheckpointAtRoundThree | R10_EscalateOnlyForFundamentalOrHim |
| Earlier | `OneMoreRound` loses its `~extra` guard | R8_FixBeforeNextRound | |
| Earlier | `ReviewTheMendedHead` goes to `Merging` | R23_ReviewMendedHead | R8_PassBeforeMerge, R8_RoundsCountUp |
| Earlier | `MendACheapShould` leaves from `Fixing` | R23_ReviewMendedHead | H16_NothingGoesOnPastTheWait, R8_FixBeforeNextRound (others not checked) |
| Earlier | `RepairReviewed` sets the full lens | R19_ReviewAfterMergeRepair | |
| Earlier | `RealRed` goes back to `Merging` | R22_RerunOnce | R22_RevertOnlyAfterTheMerge |
| Earlier | `MergeConflict` goes to `InReview` | R8_FixBeforeNextRound | R8_RoundsCountUp |
| Earlier | `MergeIntoFeature` loses its feature guard | Core_OneWayWaitsForHisClick | R23_MergeRestsOnItsEvidence, R29_TwoWayMainWaitsForItsTurn |
| Earlier | `ReviewAgain` goes to `Merging` | R8_FixBeforeNextRound | S17_CheapShouldMendedBeforeMerge, R8_PassBeforeMerge, R8_RoundsCountUp |
| Earlier | `Split` loses its seat guard | R34_SeatNeverDelegates | |
| Earlier | `QuestionOpened` opens at stage council | L18_ChildOnlyAfterResearch | (others not checked) |
| Earlier | `Take` lets a filler be any kind | H11_FillerIsAChange | |
| Earlier | `CutTheNextParts` goes to `Leaf` | Core_SplitOnlyOrchestrates | H10_ReturnedPartContinues |
| Earlier | `CloseLeaf` loses its `~merges` guard | R2_MergingHolderHasReview | R20_DoneOnlyOnceLanded |
| Earlier | `SendLeafToReview` loses its `merges` guard | Sees_OnlyAMergerSeesReviewFixMerge | TypeOK |
| Earlier | `Dispatch` goes to `Done` | H3_DispatchAndWaitAfterSplit | WorkPhaseBeforeDone, H16_NothingGoesOnPastTheWait, L18_AnswerAfterResearch, L18_CouncilSteps, R2_MergingHolderHasReview, R20_DoneOnlyOnceLanded |
| Earlier | `WaitOnTheAnswer` goes to `Done` | H16_NothingGoesOnPastTheWait | L18_AnswerAfterResearch, R5_ProofBeforeJudgement, R2_MergingHolderHasReview, R20_DoneOnlyOnceLanded |

The view check has controls of its own, one change each in a scratch copy of `statechart/captain/`. `check-chart.mjs` exits 1 on each:

| For | Changed line | What it reports |
|---|---|---|
| B8 | `chart.mmd` gains an arrow `Taken --> Done` with no words (the r1 reviewer's) | An arrow with no words |
| B8 | `view-map.mjs` puts `OneMoreRound` under `Merging` (the r1 reviewer's) | A step inside one chart state off the map |
| B8 | `chart.mmd` gains `Taken --> Done: it is done` | A chart arrow no step is under |
| B8 | `view-map.mjs` puts `RunTheGates` inside `Fixing` | A step inside one chart state off the map |
| B8 | `chart.mmd` gains a composite `state Taken { }` | Lines the check cannot read |
| B7 | `view-map.mjs` shows `Mending` as `Fixing` | Two steps off the map: a mend is not a FAIL |
| Earlier | `view-map.mjs` puts `CloseLeaf` under `Leaf --> HandedBack` | A step off the map, and the chart arrow `Leaf --> Done` with no step |
| Earlier | `view-map.mjs` drops `RepairFails` | An action with no place, and a step it does not name |
| Earlier | `HandBackTheFix` and `RepairFails` never fire | The chart arrow "the Fix comes back undone" has no model step |
| Earlier | `ReviewTheMendedHead` goes to `Fixing` | A step off the map |

## Receipts

Each command ran under the gate lock and the agent memory cap, with dydo 3.6.0's managed Java and TLC, on the committed head named in the DYD-1495 return.

- The view check (`node statechart/captain/check-chart.mjs`): exit 0. TLC on `Captain.cfg`: `TypeOK` and `NoDeadlock` hold; 38,215 states generated, 16,577 distinct, depth 35; 33,355 steps checked; 18 model states shown as 11 chart states, 75 actions under 23 arrows or inside a state. It lists two cases for Balazs: `Mending` under `InReview`, and `Waiting` on the captain's own need.
- The statechart tool on the model (`dydo statechart ... --view view.mjs`): exit 0, `TypeOK` and `NoDeadlock` hold; more than 500 states, so no chart is drawn.
- The properties (`run.py --machine captain`): exit 0. All 54 properties, `TypeOK` and `NoDeadlock` hold; 112,384 states generated, 48,859 distinct, queue empty, depth 39.
- The registered control (`run.py --controls --machine captain`): harness exit 0, `control_caught` true. H16_ResumeLeafAfterQuestion rejects `Asking` to `Done`, and so do WorkPhaseBeforeDone and H16_NothingGoesOnPastTheWait.
- The 49 hand-run model controls and the ten view controls: each rejected as its table says.

Output was written under `/tmp/dyd1492/fix/` and is not kept.

## Related

- [The captain statechart](../../../statechart/captain/README.md)
- [Statechart property runner](../README.md)
