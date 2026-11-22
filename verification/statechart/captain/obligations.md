---
area: reference
type: reference
explains: ["verification/statechart/captain/**"]
---

# Captain chart obligations

Hand-written from his recorded words, the approved spec and the core page (`dydo/core.md`), never from the model (K3). The model they judge is the comprehensive captain model in [statechart/captain/](../../../statechart/captain/README.md) (DYD-1492): 19 states, 46 actions, and the variables `state`, `kind`, `merges`, `target`, `filler`, `round`, `extra`, `lens` and `stage`. The simple chart Balazs approved is a view of that model, and `check-chart.mjs` proves the view faithful; these properties judge the model.

`seen` is the passive set of states this captain has been in. No step is disabled, the model needs no bound (the round count stops at four, which stands for four or more), and no fairness is added.

## Properties

`ReviewPhase` is `InReview`, `Clear`, `Checkpoint`, `Fixing`, `Mending` and `Repairing`. `MergePhase` is `Merging`, `AwaitingTurn`, `Rerunning` and `AwaitingClick`. Together they are the merger's states.

| Property | Source | What it checks |
|---|---|---|
| WorkPhaseBeforeDone | core "A ticket"; 7b3d2e3e | Done comes after `Leaf` or `Waiting`. |
| K30_KindKnownBeforeWork | K30, S6 | Every state but `Dispatched` and `HandedBack` has a known kind. |
| K30_KindStaysKnown | K30, S7; AGENTS.md | Once set, the kind never changes. |
| S7_UnstartedReturnHasNoWork | S6, S7 | A run that never read its kind visits only `Dispatched` and `HandedBack`. |
| H11_FillerIsAChange | H11 | A filler that is taken is a change. |
| H1_EndsAreFinal | H12; AGENTS.md | `Done` and `HandedBack` only stand still. A structural check of the two ends. |
| Core_SplitOnlyOrchestrates | core "A ticket" | After `Wayfinding` or `Cutting`, never `Leaf` or `Asking` again. |
| H3_WaitAfterSplit | H3; H2; df6a785e | `Cutting` exits only to `Waiting`, or to `HandedBack` when the cut cannot be made. |
| H10_WayfindingLeavesByCut | core "A ticket"; H10 | `Wayfinding` exits only to `Cutting`. |
| H10_ReturnedPartContinues | H2, H10, R2 | `Waiting` exits to `Cutting`, `Wayfinding`, `InReview`, `Done` or `HandedBack`. |
| R34_SeatNeverDelegates | R34 | A seat is never in `Asking`, `Wayfinding`, `Cutting` or `Waiting`. |
| H16_ResumeLeafAfterQuestion | core "Who decides what"; H16; 519126ab | `Asking` exits only to `Leaf`. |
| H23_AQuestionHasAStage | H23 | A captain is in `Asking` exactly when the Question it waits on has a stage. |
| L18_EveryQuestionStartsWithResearch | L18, H23 | Only a Question's captain waits on a `council` or `his-call` Question. |
| R34_VerdictNeverReviewed | core "Review severities"; R34; 777d396e | A verdict's holder and a seat are never in a merger's state. |
| Sees_OnlyAMergerSeesReviewFixMerge | `statechart/captain/positions.md` | Only a captain that merges a change is in a merger's state. |
| R2_MergingHolderHasReview | core "Who decides what"; R2, R23 | A captain that merges closes only from `MergePhase`, with `InReview` behind it. |
| R8_PassBeforeMerge | R8, R23 | `MergePhase` starts only from `Clear`, `Clear` only from `InReview`, and a reviewed ticket closes only from `MergePhase`. |
| R8_FixBeforeNextRound | core "Review severities"; R8, S9 | A later round starts only from `Fixing`, `Repairing`, `Mending` or `Clear`, and `Fixing` exits to `InReview` or `HandedBack`. |
| R8_PassEndsTheRounds | R8, S17 | `Clear` exits only to `Merging`, `AwaitingTurn`, `InReview` or `Mending`. |
| R8_FullLensAfterBlocker | R8, R9 | A round after a Fix looks with the full lenses, or with the security lens first. |
| R8_ExtraRoundOnlyOnce | R8 | One more round after a PASS is ordered at most once. |
| R10_CheckpointAfterThirdFail | R10 | A third round exits only to `Clear` or `Checkpoint`, and only a third round enters `Checkpoint`. |
| R10_CheckpointDecides | R10, S10 | `Checkpoint` exits only to `Fixing` (one more round) or `HandedBack` (escalated). |
| S10_CheckpointBeforeFourthRound | R10, S10 | A fourth round after a third FAIL comes after `Checkpoint`. |
| R23_ReviewMendedHead | S17, R23 | `Mending` starts only from `Clear` and exits only to `InReview`. |
| R19_ReviewAfterMergeRepair | R19, R21, R22 | `Merging` exits only to `Done`, `AwaitingClick`, `Rerunning` or `Repairing`; `Repairing` exits to `InReview` with a fix-scoped round, or to `HandedBack`. |
| R22_RerunOnce | R22 | `Rerunning` starts only from `Merging` and exits only to `Done`, `AwaitingClick` or `Repairing`. |
| Core_OneWayWaitsForHisClick | core "Who decides what"; R27 | A one-way merge closes only from `AwaitingClick`. |
| R31_ClickOnlyAfterGreen | R31 | `AwaitingClick` starts only from `Merging` or `Rerunning`. |
| R29_TwoWayMainWaitsForItsTurn | R26, R29 | A two-way PR into master starts `Merging` only from `AwaitingTurn`. |
| R27_EachWaitHasItsTarget | R27, R29 | Only a one-way PR is in `AwaitingClick`, and only a two-way PR into master in `AwaitingTurn`. |

`Sees_OnlyAMergerSeesReviewFixMerge` is not a rule of the spec. It checks the states column of the who-sees-what table against the model.

## Restored and changed at DYD-1492

PR 385 made the twin simple by taking logic out of the model. Balazs said that was a mistake: the simple chart is the view, and the model holds all the logic (DYD-1440). The model is comprehensive again, so the properties PR 385 dropped are back, rewritten for the new state names. The table above has them all; the old ones are in Git up to `8077c474`.

| PR 385 dropped | Now |
|---|---|
| R8_ExtraRoundOnlyOnceAfterClear | R8_ExtraRoundOnlyOnce, on `Clear` and `extra`. |
| R10_CheckpointBeforeFourthRound | S10_CheckpointBeforeFourthRound. |
| S10_CheckpointBeforeThirdRoundEscalation | Part of R10_CheckpointAfterThirdFail: at round 3 a FAIL goes to `Checkpoint`, never straight to the officer. |
| R10_EnterCheckpointFromThirdRound | Part of R10_CheckpointAfterThirdFail. |
| H10_WayfindingLeavesByCut | Restored as it was, on `Wayfinding`. |
| H10_PartBeforeFogFinish | Not restored. Its meaning, that after the fog no Done comes without the parts, holds through H10_WayfindingLeavesByCut, H3_WaitAfterSplit and Core_SplitOnlyOrchestrates. |
| R23_ReviewMendedHead | Restored on `Mending`. The mend whose part the officer merges is gone: in this model only a captain that merges is reviewed, so only it mends. |

Changed: R8_PassBeforeMerge, R8_FixBeforeNextRound, R19_ReviewAfterMergeRepair and R2_MergingHolderHasReview name the merge and review phases instead of one state each; H10_ReturnedPartContinues and Core_SplitOnlyOrchestrates take in `Wayfinding`; R34_VerdictNeverReviewed takes in seats.

New: H11_FillerIsAChange, R34_SeatNeverDelegates, H23_AQuestionHasAStage, L18_EveryQuestionStartsWithResearch, R8_PassEndsTheRounds, R8_FullLensAfterBlocker, R10_CheckpointDecides, R22_RerunOnce, Core_OneWayWaitsForHisClick, R31_ClickOnlyAfterGreen, R29_TwoWayMainWaitsForItsTurn and R27_EachWaitHasItsTarget.

## What the model cannot show

The properties check order only. The model has no data for any of these, so none is checked:

- Whether the contract is met, and whether a ticket really fits one agent.
- Who an agent is: that the reviewer is not the author, comes from another model family, or is fresh; who holds the Fix.
- Which head a PASS names, whether a merge is clean, and whether a gate is green.
- What a verdict holds: its blockers, shoulds and nits, and that a should was filed or a nit ruled on.
- Which skill an agent loaded and what its brief said. `positions.md` is checked by reading.
- That a sub-ticket was settled before the next parts were cut, and how many parts there are.
- Whether a call was one the captain may decide, and whether a his-call is worth his time.
- The words of the checkpoint line (S10). The model has the checkpoint, not what its line says.
- That the rounds go on past the fifth FAIL (R10): the round count stops at four, which stands for four or more, and nothing in the model stops at five.
- That anything ends. Without fairness a captain may wait forever; these are safety checks.

Whether an answer may be reviewed has no source. The model gives an answer's holder no review, because only a change is merged; that is a choice of the model, checked only as part of the who-sees-what table.

## Controls

The registered control in [controls.json](../controls.json) is the step `Asking` to `Done` for a change that is not merged. H16_ResumeLeafAfterQuestion must reject it.

The hand-run controls are one changed line each in a scratch copy of `Captain.tla`, with the committed model untouched. Each must be rejected by the property its row names; the others it breaks are listed too.

| Changed line | Rejected by |
|---|---|
| `Fail` loses its `round # 3` guard | R10_CheckpointAfterThirdFail, S10_CheckpointBeforeFourthRound |
| `Replan` loses its `round # 3` guard | R10_CheckpointAfterThirdFail |
| `ThirdFail` fires at round 2 | R10_CheckpointAfterThirdFail |
| `CheckpointEscalates` leaves from `Clear` | R8_PassEndsTheRounds |
| `OneMoreRound` loses its `~extra` guard | R8_ExtraRoundOnlyOnce |
| `ReviewTheMendedHead` goes to `Merging` | R23_ReviewMendedHead, R8_PassBeforeMerge, R29_TwoWayMainWaitsForItsTurn |
| `MendACheapShould` leaves from `InReview` | R23_ReviewMendedHead, R10_CheckpointAfterThirdFail |
| `CheckpointOneMoreRound` sets a fix-scoped lens | R8_FullLensAfterBlocker |
| `RepairReviewed` sets the full lens | R19_ReviewAfterMergeRepair |
| `RealRed` goes back to `Merging` | R22_RerunOnce, R29_TwoWayMainWaitsForItsTurn |
| `MergeConflict` goes to `InReview` | R19_ReviewAfterMergeRepair, R8_FixBeforeNextRound |
| `GreenForHisClick` goes to `Done` | Core_OneWayWaitsForHisClick |
| `Flake` always goes to `Done` | Core_OneWayWaitsForHisClick |
| `NoMoreRound` loses its `main two-way` guard | R29_TwoWayMainWaitsForItsTurn |
| `ReviewAgain` goes to `Merging` | R8_PassBeforeMerge, R8_FixBeforeNextRound, R29_TwoWayMainWaitsForItsTurn |
| `Split` loses its seat guard | R34_SeatNeverDelegates |
| `AskHim` loses its `answer` guard | L18_EveryQuestionStartsWithResearch, R34_SeatNeverDelegates |
| `Ask` opens at stage `council` | L18_EveryQuestionStartsWithResearch |
| `Take` lets a filler be any kind | H11_FillerIsAChange |
| `PartsInSight` goes to `Leaf` | H10_WayfindingLeavesByCut, Core_SplitOnlyOrchestrates |
| `CutTheNextParts` goes to `Leaf` | Core_SplitOnlyOrchestrates, H10_ReturnedPartContinues |
| `CloseLeaf` loses its `~merges` guard | R2_MergingHolderHasReview |
| `SendTheWholeToReview` loses its `merges` guard | Sees_OnlyAMergerSeesReviewFixMerge, R34_VerdictNeverReviewed, TypeOK |
| `Dispatch` goes to `Done` | H3_WaitAfterSplit, WorkPhaseBeforeDone, R2_MergingHolderHasReview |

The view check has controls of its own, one change each in a scratch copy of `statechart/captain/`. `check-chart.mjs` exits 1 on each:

| Changed line | What it reports |
|---|---|
| `view-map.mjs` shows `Mending` as `Fixing` | A step off the map, and the FAIL and Fix arrows drawn joined with it |
| `view-map.mjs` puts `CloseLeaf` under `Leaf --> HandedBack` | The chart arrow `Leaf --> Done` has no model action, and a step off the map |
| `view-map.mjs` drops `RepairFails` | A model action with no place, and a step off the map |
| `HandBackTheFix` and `RepairFails` never fire | The chart arrow "the Fix comes back undone" has no model step |
| `ReviewTheMendedHead` goes to `Fixing` | A step off the map: a mend is not a FAIL |

## Receipts

Each command ran under the gate lock with dydo 3.6.0's managed Java and TLC. The property runs used scratch git copies of `statechart/captain/` and `verification/statechart/` at the candidate's content, because the runner refuses a dirty checkout; the PR body has the runs on the committed head.

- The model through the simple view: `TypeOK` and `NoDeadlock` hold; 373 states, 739 steps. `check-chart.mjs` exit 0: 19 model states shown as 11 chart states, 46 actions under 23 arrows or inside a state.
- The properties (`run.py --machine captain`): exit 0. All 32 properties, `TypeOK` and `NoDeadlock` hold; 29,542 states generated, 15,256 distinct, queue empty, depth 32.
- The registered control (`run.py --controls --machine captain`): harness exit 0, `control_caught` true. H16_ResumeLeafAfterQuestion rejects `Asking` to `Done`, and so do WorkPhaseBeforeDone and H23_AQuestionHasAStage.
- The 24 hand-run model controls and the five view controls: each rejected as its table says.

Output was written under `/tmp/dyd1492/` and is not kept.

## Related

- [The captain statechart](../../../statechart/captain/README.md)
- [Statechart property runner](../README.md)
