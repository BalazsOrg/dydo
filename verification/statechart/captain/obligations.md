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
| S6, 7b3d2e3e | A holder closes Done only after meeting the contract. **Phase order holds** (`S6_WorkBeforeDone`). The contract, evidence and acceptance predicate are unobservable. The trace Taken → Leaf → Finishing → Done looks identical whether or not the contract was met. |
| R2, R8, R12, R16 | A change that needs review clears it before finish or merge; review the integrated whole after wiring. Group-only parents and sufficiently small, low-risk leaves can skip. **First finish after entering review holds** (`R8_ClearBeforeFirstFinish`); InReview → Finishing stands for a clear round. Review eligibility, wiring, own vs. parent merge and proof tied to a revision are unobservable. Taken → Split → Waiting → Finishing → Done cannot distinguish a grouping ticket from a review-required feature. |
| R4 | A plan, spec or contract has no review loop. A spec may get one risk-based scout pass that returns Questions. **Untestable.** Taken → Leaf → InReview carries neither ticket kind nor a separate spec pass or its answers. |
| R8, R9, R11, S9, 41a553f1 | A fixable blocker requires a fix before the next full-lens round. **Numbered round order holds** (`R8_FixBeforeNextRound`). InReview → Fixing → InReview gives no evidence of blocker severity, completeness, repair proof or a fresh reviewer and lenses. InReview → Returned cannot distinguish a fundamental issue, a new requirement and a human need. Shoulds that may wait have no status. The optional extra round after a clear round has no explicit route. |
| R10, S10 | Record the third-round checkpoint before starting round four or escalating. **Both checks fail.** `R10_CheckpointBeforeFourthRound` — Taken → Leaf → InReview(1) → Fixing(1) → InReview(2) → Fixing(2) → InReview(3) → Fixing(3) → InReview(4). `S10_CheckpointBeforeThirdRoundEscalation` — the same prefix through InReview(3), then Returned(3). The chart has no checkpoint observation, note or escalation criterion. Its round bound also prevents TLC from testing continuation beyond a blocked fourth round. |
| R19, R21 | A merge-conflict repair gets a fresh review before retrying. **Fails** (`R19_ReviewAfterMergeRepair`), Taken → Leaf → Finishing → Waiting → Finishing. The single edge into Waiting covers both a conflict and a real red gate, and the grouping-only exit bypasses review in either case. |
| 2119cd65, df6a785e, 32cc5599, R16, R33, S8 | An orchestrating captain dispatches or records reviews; the reviewer judges and never authored or fixed the work. **Chart inspection only.** No arrow assigns judging to the orchestrating captain. Taken → Leaf → InReview → Finishing carries no writer, reviewer or fixer identity, lone or multiple seats, verdict origin or authorship, so TLC cannot verify independence or fresh-writer routing. The later ruling allows a sub-captain to author a small leaf. |
| 519126ab, H16, S10 | Only obvious, reversible calls are decided locally. A non-obvious call goes to a fresh Question captain; the dependent work resumes after the answer. **Leaf wait/resume order holds** (`H16_ResumeLeafAfterQuestion`). Leaf → Question → Leaf carries no call classification, answer, Question holder or blocked scope. This path exists only from Leaf, leaving the split captain's non-obvious calls, re-plan questions and passed-up human needs unrepresented. |
| df6a785e, 0f340abd, 3fb363e3, AGENTS.md, S7, H2, H3 | A split dispatches its children and waits for their required returns. **Wait phase holds** (`H3_WaitAfterSplit`). Taken → Split → Waiting → InReview carries no child identities, completion or receipt evidence, context size, cohesion, native/delegated distinction or cut authority. Leaf → Finishing is always enabled, so TLC cannot establish that oversized work is cut or returned instead of finished. |
| H2, 7b3d2e3e | A return ends this holder's run. **Holds** (`H2_ReturnedIsTerminal`); Returned only stutters. |

Two bad-trace controls use the same observer entry-point injection. Taken → Done
must break `S6_WorkBeforeDone`; Question → Finishing must break
`H16_ResumeLeafAfterQuestion`. Both use round 1.

### Recheck with checkpoint and repair phases (DYD-1372 hop 3b)

The table above records hop 3 on the original machine. This recheck uses the
machine with `Checkpoint` and `Repairing` and retains `round <= 4`.

| Source | Obligation and current result |
|---|---|
| S6, 7b3d2e3e | **Holds at phase level** (`S6_WorkBeforeDone`). Done follows work and Finishing. Taken → Leaf → Finishing → Done cannot show that the contract was met. |
| H3, df6a785e | **Holds at phase level** (`H3_WaitAfterSplit`). Split → Waiting still carries no child completion or receipts. |
| H16, S10, 519126ab | **Holds at phase level** (`H16_ResumeLeafAfterQuestion`). Leaf → Question → Leaf carries no answer or blocked scope. A split captain's Question still has no separate route. |
| R2, R8, R12, R16 | **Holds at phase level** (`R8_ClearBeforeFirstFinish`). InReview → Finishing represents a clear result. Taken → Split → Waiting → Finishing → Done still cannot show if that ticket required review. |
| R8, R9, R11, S9, 41a553f1 | **Fails** (`R8_FixBeforeNextRound`). Taken → Leaf → InReview(1) → InReview(2) advances without a fix. R8 permits one extra round after a clear verdict. The self-loop tests only the review phase, so the same advance is available without evidence of a clear verdict. It can repeat immediately from InReview(2) to InReview(3). InReview → Fixing → InReview still carries no proof, reviewer, lenses or blocker data. |
| R10, S10 | **Fails** (`R10_CheckpointBeforeFourthRound`). Taken → Leaf → InReview(1) → InReview(2) → InReview(3) → InReview(4) starts round four with no checkpoint or clear verdict. The explicit blocked-round route InReview(3) → Checkpoint(3) → Fixing(3) → InReview(4) records the checkpoint before the fix and advance. Continuation after a blocked fourth round remains outside the explored bound. |
| R10, S10 | **Holds at phase level** (`S10_CheckpointBeforeThirdRoundEscalation`, `R10_EnterCheckpointFromThirdRound`). InReview(3) → Checkpoint(3) → Returned(3) takes the checkpoint before escalation. Entry to the checkpoint comes only from InReview(3) and preserves the round. This trace cannot show the blocker, the written note or the reason for escalation. |
| R19, R21, R22 | **Holds at phase level** (`R19_ReviewAfterMergeRepair`). Taken → Leaf → Finishing → Repairing → InReview → Finishing requires review before a retry. The trace cannot distinguish a conflict from a real red gate, show the repair branch or SHA, verify its hunks and reviewer, or show that a real red was rerun, recorded and reverted. The skill's return to the officer for a repair that cannot be resolved has no transition out of Repairing. |
| H2, 7b3d2e3e | **Holds** (`H2_ReturnedIsTerminal`). Returned only stutters. |

The original R8 numbered-round test stays in place. The model needs an
observable basis for the R8 clear-round exception before the check can
distinguish it from a bypass. The R10 checks now attach the checkpoint to
the third-round fix and escalation. The R19 check now uses the dedicated repair
phase and requires a clear review before each retry. An unresolved repair may
return to the officer under the merge-conflict and red-gate resources.
The other observability findings in the original table still apply.

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

R10’s “After round 3 comes a checkpoint” also admits a reading where every
fourth round requires one, including the optional round after `Clear(3)`. S10
cites the decision that specifies the trigger as “Round 3 with a new blocker.”
R10 replaces the fifth-FAIL stop and its escalation reasons describe blockers.
I applied the checkpoint to the blocked route and kept R8’s clear-round option
at round three. The old unconditional check reported `Taken → Leaf →
InReview(1) → Fixing(1) → InReview(2) → Fixing(2) → InReview(3) → Clear(3) →
InReview(4)`. That remains a witness for the broader reading. [4.0.0 spec](https://linear.app/bodnar-balazs/document/spec-dydo-400-08326afbb349) · [DR 070 §6](https://github.com/BalazsOrg/dydo/blob/1ee7066e/dydo/project/decisions/070-statuses-as-sub-tickets-shoulds-and-the-map.md)

| Property | Source | TLC result and observable promise |
|---|---|---|
| `S6_WorkBeforeDone` | S6, DYD-1295 7b3d2e3e | Holds. Done follows work and Finishing. |
| `H3_WaitAfterSplit` | H3, DYD-1295 df6a785e | Holds. The next phase after Split is Waiting. |
| `H16_ResumeLeafAfterQuestion` | H16, S10, DYD-1295 519126ab | Holds. A leaf's Question resumes in Leaf. |
| `R8_ClearBeforeFinish` | R2, R8, R12, R16 | Holds. A review produces Clear, and once review begins, every entry to Finishing comes from Clear. |
| `R8_FixBeforeNextRound` | R8, R9, S9, DYD-1295 41a553f1 | Holds. A round advances to a fresh review from Fixing or Clear. Fixing can resume review in the next numbered round or return to the officer. |
| `R8_ExtraRoundOnlyOnceAfterClear` | R8, captain phase 4 | Holds. Clear may finish or advance once, consuming the unused extra round. The marker starts unused and stays consumed after use. |
| `R10_CheckpointBeforeFourthRound` | R10, S10, DR 070 §6 | Holds. A third-round Fix and the fourth round following it require the checkpoint. |
| `S10_CheckpointBeforeThirdRoundEscalation` | R10, S10 | Holds. A blocked third review round returns to its officer via Checkpoint. |
| `R10_EnterCheckpointFromThirdRound` | R10, S10, DR 070 §6 | Holds. Checkpoint starts from InReview in round three and preserves the round number. |
| `R19_ReviewAfterMergeRepair` | R8, R19, R21, R22, merge-conflict and red-gate resources | Holds. Repairing exits to a fresh review or to the officer. Retrying after a repair requires Clear. |
| `H2_ReturnedIsTerminal` | H2, DYD-1295 7b3d2e3e | Holds. Returned ends the holder’s run. |

The registered controls use `Repairing(1) → Finishing(1)` for
`R19_ReviewAfterMergeRepair` and `InReview(3) → Returned(3)` for
`S10_CheckpointBeforeThirdRoundEscalation`, both with `extra = FALSE`. TLC rejects
both shortcuts.

### What remains unobservable

The traces below show where the model loses the distinction required by the
rule. The model has no fairness assumption, so the checks establish safety on
observed moves. They do not establish that a holder makes those moves eventually.

| Source | Trace and missing evidence |
|---|---|
| S6, 7b3d2e3e | Taken → Leaf → Finishing → Done carries no contract predicate, acceptance or proof receipt. |
| R2, R4, R12, R16 | Taken → Split → Waiting → Finishing → Done carries no wiring or review eligibility. Taken → Leaf → InReview carries no ticket kind, own versus parent merge, revision-bound proof or separate Questions-only spec pass. |
| H2, H3, S7, df6a785e, 0f340abd, 3fb363e3 | Taken → Split → Waiting → InReview carries no child identity, completion, receipt, context size, cohesion, cut authority or dispatch kind. Taken → Leaf → Finishing cannot show whether oversized work should have been cut or returned. |
| H16, S10, 519126ab | Leaf → Question → Leaf carries no classification, answer, fresh Question holder or blocked scope. Split → Waiting has no distinct route for a split captain's Question, re-plan Question or passed-up human need. |
| R8, R9, R11, R16, R33, S8, S9, 2119cd65, df6a785e, 32cc5599, 41a553f1 | InReview → Fixing → InReview has no blocker items or severity, shoulds, completed Fix and proof on the head, reviewer/fixer identities, seats, verdict origin, lenses or security-first ordering. InReview → Clear reports the phase but carries no verdict evidence. Clear → InReview consumes the extra round but carries no officer decision or identity. |
| R9, R10, S10 | InReview(3) → Checkpoint(3) → Returned(3) carries no blocker, checkpoint note, reason or officer decision. InReview → Returned outside round three also carries no reason. InReview(4) → Fixing(4) is reachable, with the next review at round five beyond `round <= 4`. |
| R19, R21, R22 | Finishing → Repairing → InReview → Clear → Finishing carries no conflict/red distinction, repair branch/SHA, reviewed hunks, reviewer, rerun, flake, revert, CI record or re-land evidence. Repairing → Returned carries no evidence that the repair was unresolved. |
