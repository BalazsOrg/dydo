---
area: reference
type: reference
explains: ["verification/statechart/captain/**"]
---

# Captain prototype obligations

## Original check (DYD-1372 hop 3)

These obligations were written from the DYD-1295 walkthrough rulings, the approved
4.0.0 spec and the accepted `AGENTS.md` before reading the generated captain
machine at `278b89da`. The observer runs over the generated `Init`, `Next` and
`round <= 4` bound. The model declares no fairness, so these checks are safety
properties.

| Source | Obligation and result |
|---|---|
| 7b3d2e3e | A holder closes Done only after meeting the contract. **Phase order holds** (then `S6_WorkBeforeDone`). Taken → Leaf → Finishing → Done carries no contract, evidence or acceptance predicate. |
| R8 | **First finish after entering review holds** (`R8_ClearBeforeFirstFinish`). InReview → Finishing stands for a clear round. |
| R2, R6, R12, R16 | Review the integrated whole after wiring. Group-only parents and sufficiently small, low-risk leaves can skip. **Untestable.** Taken → Split → Waiting → Finishing → Done carries no review eligibility, wiring, own versus parent merge or proof tied to a revision. |
| R4 | A plan, spec or contract has no review loop. A spec may get one risk-based scout pass that returns Questions. **Untestable.** Taken → Leaf → InReview carries neither ticket kind nor a separate spec pass or its answers. |
| R8, R9, R11, S9, 41a553f1 | A fixable blocker requires a fix before the next full-lens round. **Numbered round order holds** (`R8_FixBeforeNextRound`). InReview → Fixing → InReview gives no evidence of blocker severity, completeness, repair proof or a fresh reviewer and lenses. InReview → Returned cannot distinguish a fundamental issue, a new requirement and a human need. Shoulds that may wait have no status. The optional extra round after a clear round has no explicit route. |
| R10, S10 | Record the third-round checkpoint before starting round four or escalating. **Both checks fail.** `R10_CheckpointBeforeFourthRound` — Taken → Leaf → InReview(1) → Fixing(1) → InReview(2) → Fixing(2) → InReview(3) → Fixing(3) → InReview(4). `S10_CheckpointBeforeThirdRoundEscalation` — the same prefix through InReview(3), then Returned(3). The chart has no checkpoint observation, note or escalation criterion. Its round bound also prevents TLC from testing continuation beyond a blocked fourth round. |
| R19, R21 | A merge-conflict repair gets a fresh review before retrying. **Fails** (`R19_ReviewAfterMergeRepair`), Taken → Leaf → Finishing → Waiting → Finishing. The single edge into Waiting covers both a conflict and a real red gate, and the grouping-only exit bypasses review in either case. |
| 2119cd65, df6a785e, 32cc5599, R16, R33, S8 | An orchestrating captain dispatches or records reviews; the reviewer judges and never authored or fixed the work. **Chart inspection only.** No arrow assigns judging to the orchestrating captain. Taken → Leaf → InReview → Finishing carries no writer, reviewer or fixer identity, lone or multiple seats, verdict origin or authorship, so TLC cannot verify independence or fresh-writer routing. The later ruling allows a sub-captain to author a small leaf. |
| 519126ab, H16, S10 | Only obvious, reversible calls are decided locally. A non-obvious call goes to a fresh Question captain; the dependent work resumes after the answer. **Leaf wait/resume order holds** (`H16_ResumeLeafAfterQuestion`). Leaf → Question → Leaf carries no call classification, answer, Question holder or blocked scope. This path exists only from Leaf, leaving the split captain's non-obvious calls, re-plan questions and passed-up human needs unrepresented. |
| df6a785e, 0f340abd, 3fb363e3, AGENTS.md, S7, H2, H3 | A split dispatches its children and waits for their required returns. **Wait phase holds** (`H3_WaitAfterSplit`). Taken → Split → Waiting → InReview carries no child identities, completion or receipt evidence, context size, cohesion, native/delegated distinction or cut authority. Leaf → Finishing is always enabled, so TLC cannot establish that oversized work is cut or returned instead of finished. |
| H1, H23, AGENTS.md (Sort yourself) | A return ends this holder's run. **Holds** (then `H2_ReturnedIsTerminal`). Returned only stutters. |

Two bad-trace controls use the same observer entry-point injection. Taken → Done
must break `S6_WorkBeforeDone`; Question → Finishing must break
`H16_ResumeLeafAfterQuestion`. Both use round 1.

### Recheck with checkpoint and repair phases (DYD-1372 hop 3b)

The table above records hop 3 on the original machine. This recheck uses the
machine with `Checkpoint` and `Repairing` and retains `round <= 4`.

| Source | Obligation and current result |
|---|---|
| 7b3d2e3e | **Holds at phase level** (then `S6_WorkBeforeDone`). Done follows work and Finishing. Taken → Leaf → Finishing → Done cannot show that the contract was met. |
| H3, df6a785e | **Holds at phase level** (`H3_WaitAfterSplit`). Split → Waiting still carries no child completion or receipts. |
| H16, S10, 519126ab | **Holds at phase level** (`H16_ResumeLeafAfterQuestion`). Leaf → Question → Leaf carries no answer or blocked scope. A split captain's Question still has no separate route. |
| R8 | **Holds at phase level** (`R8_ClearBeforeFirstFinish`). InReview → Finishing represents a clear result. |
| R8, R9, R11, S9, 41a553f1 | **Fails** (`R8_FixBeforeNextRound`). Taken → Leaf → InReview(1) → InReview(2) advances without a fix. R8 permits one extra round after a clear verdict. The self-loop tests only the review phase, so the same advance is available without evidence of a clear verdict. It can repeat immediately from InReview(2) to InReview(3). InReview → Fixing → InReview still carries no proof, reviewer, lenses or blocker data. |
| R10, S10 | **Fails** (`R10_CheckpointBeforeFourthRound`). Taken → Leaf → InReview(1) → InReview(2) → InReview(3) → InReview(4) starts round four with no checkpoint or clear verdict. The explicit blocked-round route InReview(3) → Checkpoint(3) → Fixing(3) → InReview(4) records the checkpoint before the fix and advance. Continuation after a blocked fourth round remains outside the explored bound. |
| R10, S10 | **Holds at phase level** (`S10_CheckpointBeforeThirdRoundEscalation`, `R10_EnterCheckpointFromThirdRound`). InReview(3) → Checkpoint(3) → Returned(3) takes the checkpoint before escalation. Entry to the checkpoint comes only from InReview(3) and preserves the round. This trace cannot show the blocker, the written note or the reason for escalation. |
| R19, R21, R22, K9, H12 | **Holds at phase level** (`R19_ReviewAfterMergeRepair`). Taken → Leaf → Finishing → Repairing → InReview → Finishing requires review before a retry. The trace cannot distinguish a conflict from a real red gate, show the repair branch or SHA, verify its hunks and reviewer, or show that a real red was rerun, recorded and reverted. Repairing had no transition for the K9 and H12 escalation to the officer. |
| H1, H23, AGENTS.md (Sort yourself) | **Holds** (then `H2_ReturnedIsTerminal`). Returned only stutters. |

The hop 3b controls use Repairing(1) → Finishing(1) for
`R19_ReviewAfterMergeRepair` and InReview(3) → Returned(3) for
`S10_CheckpointBeforeThirdRoundEscalation`. TLC rejects both shortcuts.

## Recheck with clear verdicts (DYD-1372 hop 3c)

The model at `6661548e` represents a verdict with no new blocker as `Clear`,
and records use of the optional extra round in `extra`. The observer checks the
same `Init`, `Next` and bounds with these phases exposed.

R8 says “By default a round with no new blocker ends the phase, and the officer
may still order one more.” R9 sends a fixable blocker to “Fix, then the next
round,” and S9 requires its blockers Done and proof on the head first. A numbered
advance therefore follows a Fix or is the one extra round after a clear verdict.

R10's checkpoint is applied to blocked rounds: S10 cites “Round 3 with a new
blocker” (DR 070 §6). R8 still permits Clear(3) → InReview(4). The broader
reading that every fourth round requires a checkpoint fails on Taken → Leaf →
InReview(1) → Fixing(1) → InReview(2) → Fixing(2) → InReview(3) → Clear(3) →
InReview(4).

| Property | Source | TLC result and observable promise |
|---|---|---|
| `S6_WorkBeforeDone` | DYD-1295 7b3d2e3e | Holds at phase level. Done follows a work phase and Finishing. |
| `H3_WaitAfterSplit` | H3, DYD-1295 df6a785e | Holds. The next phase after Split is Waiting. |
| `H16_ResumeLeafAfterQuestion` | H16, S10, DYD-1295 519126ab | Holds. A leaf's Question resumes in Leaf. |
| `R8_ClearBeforeFinish` | R8 | Holds. A review produces Clear, and once review begins, every entry to Finishing comes from Clear. |
| `R8_FixBeforeNextRound` | R8, R9, S9, DYD-1295 41a553f1 | Holds. A round advances to a fresh review from Fixing or Clear. Fixing can resume review in the next numbered round or return to the officer. |
| `R8_ExtraRoundOnlyOnceAfterClear` | R8 | Holds. Clear may finish or advance once, consuming the unused extra round. The marker starts unused and stays consumed after use. |
| `R10_CheckpointBeforeFourthRound` | R10, S10, DR 070 §6 | Holds. A third-round Fix and the fourth round following it require the checkpoint. |
| `S10_CheckpointBeforeThirdRoundEscalation` | R10, S10 | Holds. A blocked third review round returns to its officer via Checkpoint. |
| `R10_EnterCheckpointFromThirdRound` | R10, S10, DR 070 §6 | Holds. Checkpoint starts from InReview in round three and preserves the round number. |
| `R19_ReviewAfterMergeRepair` | R8, R19, R21, R22, K9, H12 | Holds. Repairing exits to a fresh review or to the officer. Retrying after a repair requires Clear. |
| `H2_ReturnedIsTerminal` | H1, H12, H23, AGENTS.md (Sort yourself) | Holds. Returned ends the holder’s run. |

TLC again rejected the repair and checkpoint shortcuts from hop 3b.

## Recheck with the Question wait restored (DYD-1372 hop 3e)

H16 says the receiving officer “opens it … and waits on it.” The Question captain
“researches first, then answers the need, sends it to a council … or passes it up
toward his HITL queue.” S10 says a Question “blocks only what waits on it.” In
519126ab Balazs says, “For more complex one it creates a sub captain to
investigate and answer the thing.” The investigation can go up the officer
chain under H12, and the dependent leaf still waits for an answer. The model
represents this as `Question → Question`. `Question → Leaf` is the only exit.
[4.0.0 spec](https://linear.app/bodnar-balazs/document/spec-dydo-400-08326afbb349)

Hop 3d allowed `Question → Returned` in the property. The restored check rejects
that exit because it ends the waiting holder's run while its work still depends
on the answer.

TLC held all 11 properties and `NoDeadlock` within `round <= 4`. It generated
1,163 states, found 733 distinct states and emptied the queue. The runner
exited 0.

| Property | Source | Observable promise |
|---|---|---|
| `WorkPhaseBeforeDone` | DYD-1295 7b3d2e3e, AGENTS.md (How work happens here) | Done follows Finishing and a prior Leaf or Waiting phase. |
| `H3_WaitAfterSplit` | H3, DYD-1295 df6a785e | The next phase after Split is Waiting. |
| `H16_ResumeLeafAfterQuestion` | H16, S10, DYD-1295 519126ab | Question may stutter. Any exit goes to Leaf. |
| `R8_ClearBeforeFinish` | R8 | Clear comes from InReview, and finishing after review comes from Clear. |
| `R8_FixBeforeNextRound` | R8, R9, S9, DYD-1295 41a553f1 | An advance goes from Fixing or Clear to the next numbered InReview. Fixing can also return to the officer. |
| `R8_ExtraRoundOnlyOnceAfterClear` | R8 | Clear can finish or consume the one extra round. The marker begins unused and stays consumed after use. |
| `R10_CheckpointBeforeFourthRound` | R10, S10, DR 070 §6 | A third-round Fix and the fourth round following it require the checkpoint. |
| `S10_CheckpointBeforeThirdRoundEscalation` | R10, S10 | A blocked third-round escalation goes via Checkpoint. |
| `R10_EnterCheckpointFromThirdRound` | R10, S10, DR 070 §6 | Checkpoint starts from InReview in round three and preserves the round number. |
| `R19_ReviewAfterMergeRepair` | R8, R19, R21, R22, K9, H12 | Repairing goes to fresh review or returns to the officer. Each retry after repair requires Clear. |
| `H1_ReturnedIsTerminal` | H1, H12, H23, AGENTS.md (Sort yourself) | “Crew does its one job and returns.” H23 pairs the sub-captain's return line with a closed-thread signal. Returned allows only stuttering. |

### Control traces

Each control injects one shortcut in the scratch observer, starting at its
source phase with the indicated round and `extra = FALSE`. The manual controls
use the same `check_machine` entry point as the registered control.

| Shortcut | Action property TLC reported violated | Run |
|---|---|---|
| Question(1) → Finishing(1) | `H16_ResumeLeafAfterQuestion` | Registered control |
| Repairing(1) → Finishing(1) | `R19_ReviewAfterMergeRepair` | Manual control |
| InReview(3) → Returned(3) | `S10_CheckpointBeforeThirdRoundEscalation` | Manual control |

For each shortcut, TLC reported “Action property … is violated” and printed
the two-state trace. The managed checker exited 1 and the control runner
exited 0 after catching the expected violation.

### What remains unobservable

The model has no fairness assumption. The checks establish safety on observed
moves, and a wait may continue indefinitely.

| Source | Trace and missing evidence |
|---|---|
| DYD-1295 7b3d2e3e, AGENTS.md (How work happens here), S6 | Taken → Leaf → Finishing → Done carries no contract predicate, acceptance, proof receipt or identity tying start and close to the same holder. |
| R2, R4, R12, R16 | Taken → Split → Waiting → Finishing → Done carries no wiring or review eligibility. Taken → Leaf → InReview carries no ticket kind, own versus parent merge, revision-bound proof or separate Questions-only spec pass. |
| H2, H3, S7, df6a785e, 0f340abd, 3fb363e3 | Taken → Split → Waiting → InReview carries no child identity, completion, receipt, context size, cohesion, cut authority or dispatch kind. Taken → Leaf → Finishing cannot show whether oversized work should have been cut or returned. |
| H16, H12, S10, DYD-1295 519126ab | Question → Leaf carries no answer. The Question self-loop is indistinguishable from waiting in place. Ticket identity, classification, research, fresh holder, escalation recipient, council, and the scope of blocked work are unobservable. Split → Waiting carries no distinct Question or re-plan route. |
| H11 | Taken → Returned carries no filler classification or eligibility result, assignment state, one-line reason, Wayfinding blocking relation or release. |
| H1, H12, H23, AGENTS.md (Sort yourself) | Finishing → Done → Returned carries no return-line contents, receipts, recipient or closed-thread signal. A new holder's run is outside the model. |
| R8, R9, R11, R16, R33, S8, S9, 2119cd65, df6a785e, 32cc5599, 41a553f1 | InReview → Fixing → InReview has no blocker items or severity, shoulds, completed Fix and proof on the head, reviewer/fixer identities, seats, verdict origin, lenses or security-first ordering. InReview → Clear reports the phase but carries no verdict evidence. Clear → InReview consumes the extra round but carries no officer decision or identity. |
| R9, R10, S10 | InReview(3) → Checkpoint(3) → Returned(3) carries no blocker, checkpoint note, reason or officer decision. InReview → Returned outside round three also carries no reason. InReview(4) → Fixing(4) is reachable, with the next review at round five beyond `round <= 4`. |
| R19, R21, R22 | Finishing → Repairing → InReview → Clear → Finishing carries no conflict/red distinction, repair branch/SHA, reviewed hunks, reviewer, rerun, flake, revert, CI record or re-land evidence. Repairing → Returned carries no evidence that the repair was unresolved. |

## Phase 1: wayfinding and review seats (DYD-1372)

Sources: approved spec H10, H16, S10, R34 and DYD-1372 DECIDED comments
777d396e and 07c86aad. No built skill supplies an obligation. The eleven
existing properties name phases, not the renamed Ask/Answered/Up actions;
they already bind to the unchanged Question arrows now named CaptainNeed.

| Added property | Source and obligation |
|---|---|
| `H10_WayfindingLeavesByCut` | H10; 07c86aad: “Finding the way towards the spec, one manageable part at a time.” Any exit from Wayfinding goes to Split. |
| `H10_PartBeforeFogFinish` | H10; 777d396e's concern about cutting before seeing the route; 07c86aad's words above. After fog, finishing or Done requires prior Split and Waiting. This history check is cumulative; together with the exit and split-wait checks it requires a cut and wait on every wayfinding cycle. |
| `H10_ReturnedPartContinues` | H10; 07c86aad: “removing the fog gradually”. Waiting finds the way again or proceeds to review/finishing, never restarts at Taken. |
| `R34_VerdictGoesToFinishing` | R34; 777d396e: “Reviewing the reviews is probably not a good idea.” The designated verdict arrow goes Waiting → Finishing and preserves round and extra. |

H16 and S10 still require a Question to be left only to resume dependent work.
`H16_ResumeLeafAfterQuestion` stays unchanged: passing the need up stutters,
and the only exit resumes Leaf. An answer's existence remains unobservable.

### Limits and witnesses

H10's route visibility, manageable size and timing of human input are
**untestable**. Taken → Split → Waiting → Finishing → Done is enabled without
a route predicate: it cannot tell whether the initial cut was premature.
Taken → Wayfinding → Split → Waiting → Wayfinding → Split → Waiting → Finishing
shows gradual phase order, but no evidence that either part was in sight,
that its work returned, or that the remaining fog was left uncut. Without
fairness, Wayfinding may stutter indefinitely.

R34's full “A verdict gets no review” obligation is **untestable** for a review
holder. Taken → Split (`CaptainReviewSeats.Ticket`) → Waiting
(`CaptainDispatch.Send`) → InReview (`CaptainReview.Whole`) is enabled.
The model cannot distinguish this review-seat dispatch from an implementation
split. Even after the verdict, Taken → Split → Waiting → Finishing
(`CaptainReviewSeats.Verdict`) → Repairing → InReview is enabled. No ticket
kind or verdict identity can forbid these paths only for reviews. The arrow
check does not prove the full obligation. R34's family preference, separate
seat tickets and one posted verdict, S1's cut and S8's verdict contents are
also unobservable on Taken → Split → Waiting → Finishing. No kind is invented.

### New manual control

One-line scratch change: replace observer Init/Next with round 1, extra FALSE
and the edge Wayfinding → Finishing. `H10_WayfindingLeavesByCut` must reject
it; the fog-history check rejects it too. The registered Question → Finishing
control stays unchanged. Controls use the existing `check_machine` entry point
and change only scratch observer entry points. Both were rejected: Question by
`H16_ResumeLeafAfterQuestion`; Wayfinding by both H10 checks named above.
Each managed checker exited 1 and each control runner exited 0.

TLC held all eleven existing and four added properties plus NoDeadlock:
1,872 generated states, 1,176 distinct, queue empty, depth 23, round <= 4.
The ordinary runner exited 0. No tested obligation failed; the semantic gaps
and their witnesses above remain unresolved.
Size gate exited 0 (this file 19,815 bytes before this result line); build
exited 0; dydo check exited 0: 0 errors, 0 warnings in 1,020 files.

## Related

- [Statechart property runner](../README.md)
