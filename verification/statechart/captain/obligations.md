---
area: reference
type: reference
explains: ["verification/statechart/captain/**"]
---

# Captain chart obligations

Hand-written from the core page (`dydo/core.md`), the approved spec and his rulings, never from the model (K3). The model they judge is the hand-written captain chart in [statechart/captain/](../../../statechart/captain/README.md) (DYD-1437): 11 states, 21 arrows, and the variables `state`, `kind` and `merges`. Where the core page and the spec disagree, the core page wins.

The properties of the generated chart that came before, and their receipts, are in Git up to `7466727a`.

`seen` is the passive set of states this captain has been in. No step is disabled, the model needs no bound, and no fairness is added.

## Properties

| Property | Source | What it checks |
|---|---|---|
| WorkPhaseBeforeDone | core "A ticket"; 7b3d2e3e | Done comes after `Leaf` or `Waiting`. |
| H3_WaitAfterSplit | H3; df6a785e | `Cutting` exits only to `Waiting`. |
| H16_ResumeLeafAfterQuestion | core "Who decides what"; H16; 519126ab | `Asking` exits only to `Leaf`. |
| Core_SplitOnlyOrchestrates | core "A ticket" | After `Cutting`, never `Leaf` or `Asking` again. |
| H10_ReturnedPartContinues | H2, H10, R2 | `Waiting` exits to `Cutting`, `InReview` or `Done`. |
| R8_PassBeforeMerge | R8, R23 | `Merging` starts only from `InReview`, and a reviewed ticket closes only from `Merging`. |
| R8_FixBeforeNextRound | core "Review severities"; R8, S9 | A second round starts only from `Fixing`, and `Fixing` exits to `InReview` or `HandedBack`. |
| R19_ReviewAfterMergeRepair | R19, R21, R22 | `Merging` exits only to `Done` or `Fixing`. |
| R2_MergingHolderHasReview | core "Who decides what"; R2, R23 | A captain that merges closes only from `Merging`, with `InReview` behind it. |
| R34_VerdictNeverReviewed | core "Review severities"; R34; 777d396e | A verdict's holder is never in `InReview`, `Fixing` or `Merging`. |
| Sees_OnlyAMergerSeesReviewFixMerge | `statechart/captain/positions.md` | Only a captain that merges a change is in `InReview`, `Fixing` or `Merging`. |
| K30_KindKnownBeforeWork | K30, S6 | Every state but `Dispatched` and `HandedBack` has a known kind. |
| K30_KindStaysKnown | K30, S7; AGENTS.md | Once set, the kind never changes. |
| S7_UnstartedReturnHasNoWork | S6, S7 | A run that never read its kind visits only `Dispatched` and `HandedBack`. |
| H1_EndsAreFinal | H12; AGENTS.md | `Done` and `HandedBack` only stand still. |

`Sees_OnlyAMergerSeesReviewFixMerge` is not a rule of the spec. It checks the states column of the who-sees-what table against the model, so the table cannot say a Question's captain never needs the review, fix and merge text while the model lets it in.

## Changed and dropped at DYD-1437

The chart was simplified, so each property below changed with the state it named. Old wording is in Git at `7466727a`.

Changed:

| Property | What changed | Why |
|---|---|---|
| WorkPhaseBeforeDone | No longer requires `Finishing` before Done. | `Finishing` is gone: a ticket closes from `Leaf`, `Waiting` or `Merging`. The work-before-Done order stays. |
| H3_WaitAfterSplit | `Split` is now `Cutting`. | Renamed state. |
| H16_ResumeLeafAfterQuestion | `Question` is now `Asking`. | Renamed state. |
| H10_ReturnedPartContinues | Exits are `Cutting`, `InReview`, `Done`. `Wayfinding`, `Finishing` and `Leaf` for an answer are gone. | `Wayfinding` merged into `Cutting`. The core page says a captain that splits only orchestrates, so a research holder no longer goes back to `Leaf`. |
| R8_ClearBeforeFinish, now R8_PassBeforeMerge | `Clear` and `Finishing` are now the PASS arrow and `Merging`. The exception for a mended part is gone. | `Clear` and `Mending` are gone. |
| R8_FixBeforeNextRound | Written on order, not on a round number. `Clear` and `Mending` are no longer ways into a new round. | The model no longer counts rounds, and the extra round and the mend are gone. |
| R19_ReviewAfterMergeRepair | `Repairing` is now `Fixing`. | A broken merge goes to a Fix like any blocker. With R8_FixBeforeNextRound and R8_PassBeforeMerge, a repair still cannot reach Done without a review. |
| R2_MergingHolderHasReview | `merger = "holder"` is now `merges`, and the close is from `Merging`. | The model keeps one fact, whether this captain merges. |
| R34_VerdictNeverReviewed | The states are `InReview`, `Fixing`, `Merging`. | `Clear`, `Checkpoint`, `Repairing` and `Mending` are gone. |
| K30_KindKnownBeforeWork, S7_UnstartedReturnHasNoWork | `Returned` is now `HandedBack`. | Renamed state. |
| H1_ReturnedIsTerminal, now H1_EndsAreFinal | Both ends are final. | `Done` no longer steps to `Returned`: Done is an end, and the line back to the officer is part of it. |

Dropped:

| Property | Why |
|---|---|
| R8_ExtraRoundOnlyOnceAfterClear | The arrow "one more round is ordered" is gone. R8 lets an officer order it; the chart leaves it out, because a PASS ends the review and the captain need not decide more. |
| R10_CheckpointBeforeFourthRound, S10_CheckpointBeforeThirdRoundEscalation, R10_EnterCheckpointFromThirdRound | The `Checkpoint` state is gone. Its two exits are the Fix and the way to the officer, which every FAIL already has. R10's line on the Review is a record on those arrows, and the model no longer counts rounds, so none of the three can be written. |
| H10_WayfindingLeavesByCut | `Wayfinding` merged into `Cutting`, and H3_WaitAfterSplit already says `Cutting` exits only to `Waiting`. |
| H10_PartBeforeFogFinish | The same merge. H3_WaitAfterSplit and Core_SplitOnlyOrchestrates cover it: after a cut, the only way on is through `Waiting`. |
| R23_ReviewMendedHead | The `Mending` state is gone: a should becomes a ticket that waits. |

Added: Core_SplitOnlyOrchestrates and Sees_OnlyAMergerSeesReviewFixMerge.

## What the model cannot show

The properties check order only. The model has no data for any of these, so none is checked:

- Whether the contract is met, and whether a ticket really fits one agent.
- Who an agent is: that the reviewer is not the author, comes from another model family, or is fresh.
- Which head a PASS names, whether a merge is clean, and whether a gate is green.
- What a verdict holds: its blockers, shoulds and nits, and that a should was filed or a nit ruled on.
- Which skill an agent loaded and what its brief said. `positions.md` is checked by reading.
- That a sub-ticket was settled before the next parts were cut, and how many parts there are.
- Whether a call was one the captain may decide.
- R10's checkpoint line after a third FAIL.
- That anything ends. Without fairness a captain may wait forever; these are safety checks.

Whether an answer may be reviewed has no source. The model gives an answer's holder no review, because only a change is merged; that is a choice of the model, checked only as part of the who-sees-what table.

## Controls

The registered control in [controls.json](../controls.json) is the step `Asking` to `Done` for a change that is not merged. H16_ResumeLeafAfterQuestion must reject it.

Five more controls were run by hand at DYD-1437, each one changed line in a scratch copy of `Captain.tla`, with the committed model untouched:

| Changed line | Property that must reject it |
|---|---|
| `SendTheWholeToReview` loses its `merges` guard | Sees_OnlyAMergerSeesReviewFixMerge, R34_VerdictNeverReviewed |
| `CloseLeaf` loses its `~merges` guard | R2_MergingHolderHasReview |
| `CutTheNextParts` goes to `Leaf` | Core_SplitOnlyOrchestrates, H10_ReturnedPartContinues |
| `ReviewAgain` goes to `Merging` | R8_FixBeforeNextRound, R8_PassBeforeMerge |
| `MergeBroke` goes to `InReview` | R19_ReviewAfterMergeRepair, R8_FixBeforeNextRound |

## Receipts

All at `e5c282d9864107456a54ae878596500c4c29c0e4`, each run under the gate lock, with dydo 3.6.0's managed Java and TLC:

- The model alone (`run.py --baseline --machine captain`): exit 0. `TypeOK` and `NoDeadlock` hold; 33 states, 61 steps.
- `check-chart.mjs` on the chart that run drew: exit 0. `chart.mmd` and the model agree on 11 states and 21 arrows, and no arrow joins two cases with "or".
- The properties (`run.py --machine captain`): exit 0. All 15 properties, `TypeOK` and `NoDeadlock` hold; 232 states generated, 137 distinct, queue empty, depth 12.
- The registered control (`run.py --controls --machine captain`): checker 1, harness 0, `control_caught` true. H16_ResumeLeafAfterQuestion rejects `Asking` to `Done`, and so does WorkPhaseBeforeDone.
- The five hand-run controls: checker 1 on each, and each is rejected by exactly the two properties, or the one, that the table above names.

No control was run against H3_WaitAfterSplit, K30_KindKnownBeforeWork, K30_KindStaysKnown, S7_UnstartedReturnHasNoWork or H1_EndsAreFinal. Their meaning did not change at DYD-1437; only state names did.

Output was written to `/tmp/dyd-1437/` and is not kept. Only this page and a comment's place in `chart.mmd` changed after these runs.

## Related

- [The captain statechart](../../../statechart/captain/README.md)
- [Statechart property runner](../README.md)
