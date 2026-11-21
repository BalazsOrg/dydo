---
area: reference
type: reference
explains: ["verification/statechart/captain/**"]
---

# Captain prototype obligations

## Phase 1 properties 2 (DYD-1372)

Hand-written from the approved spec, his DYD-1295/DYD-1372 rulings and
AGENTS.md (K3), never from the built skill. Historical checks remain in Git.
The machine at d36ba509 starts at Dispatched; seen starts there through Init.
No move is disabled, round <= 4 is preserved and no fairness is added.

| Property | Source | Observable obligation |
|---|---|---|
| WorkPhaseBeforeDone | 7b3d2e3e; AGENTS.md | Done follows Finishing and Leaf or Waiting. |
| H3_WaitAfterSplit | H3; df6a785e | Split exits to Waiting. |
| H16_ResumeLeafAfterQuestion | H16, S10; 519126ab | A dependent leaf's Question exits only to Leaf. |
| R8_ClearBeforeFinish | R8 | Clear comes from review; finishing after review follows Clear. |
| R8_FixBeforeNextRound | R8, R9, S9; 41a553f1 | Advance follows Fixing or Clear; Fixing resumes next round or returns. |
| R8_ExtraRoundOnlyOnceAfterClear | R8 | Clear finishes or consumes the single persistent extra-round marker. |
| R10_CheckpointBeforeFourthRound | R10, S10, DR 070 §6 | A blocked third-round fix and subsequent fourth round require Checkpoint. |
| S10_CheckpointBeforeThirdRoundEscalation | R10, S10 | Blocked third-round return passes Checkpoint. |
| R10_EnterCheckpointFromThirdRound | R10, S10, DR 070 §6 | Checkpoint comes from InReview(3), preserving round. |
| R19_ReviewAfterMergeRepair | R8, R19, R21, R22, K9, H12 | Repairing exits to review or return; retry requires Clear. |
| H1_ReturnedIsTerminal | H1, H12, H23; AGENTS.md | Returned only stutters. |
| H10_WayfindingLeavesByCut | H10; 07c86aad | Wayfinding exits to Split. |
| H10_PartBeforeFogFinish | H10; 777d396e, 07c86aad | Wayfinding history requires Split and Waiting before finishing. |
| H10_ReturnedPartContinues | H10 | Waiting exits to Wayfinding, review or finishing. |
| R34_VerdictNeverReviewed | R34; 777d396e | Verdict/answer never occupies InReview, Clear, Checkpoint, Fixing or Repairing. |
| K30_KindKnownBeforeWork | K30; 0f340abd, 07c86aad | Every phase except Dispatched and Returned has a known kind. |
| K30_KindStaysKnown | K30, S7; AGENTS.md | Once set, kind never changes during this ticket's run. |
| S7_UnstartedReturnHasNoWork | S6, S7; AGENTS.md | An unread-kind run visits only Dispatched and Returned. |

## New obligations and remaining limits

R34: "Reviewing the reviews is probably not a good idea." The invariant now
covers every verdict/answer path, including leaves and split reviews, replacing
the narrow R34_VerdictGoesToFinishing arrow check. K30's one captain base and
kind resources were accepted as "I like A" (0f340abd), kind-specific phases as
"I like a" (07c86aad). Known-before-work and stable kind are now checkable;
actually loading the right resource remains unobservable.

S7 says "One agent holds one ticket"; AGENTS.md says "one agent holds one
ticket" and owns its outcome. Dispatched(unread) → Returned(unread) checks
the no-work footprint and terminal return. It cannot prove exclusive ownership,
already-held status or authorized takeover: holder/ticket identity and intake
reason are absent. That trace is identical for filler refusal and held-ticket
conflict. Dispatched → Taken(change) is enabled without an other-holder premise.
Full mutual exclusion remains unexpressible.

R2 requires review after wiring and before merge; R8 a clear round; R33 forbids
judging one's own step. Existing review-order properties apply to every run,
therefore also to change runs, preserving the stronger checks. Non-vacuity
witness: Dispatched → Taken(change) → Leaf → InReview(1) → Clear(1) → Finishing
→ Done. Fixes, Checkpoint(3), extra clear rounds and Repairing are reachable
for changes too. The change-holder shortcut control exercises finishing's
antecedent. R8 permits an extra round after Clear(3) without Checkpoint;
DR 070 §6 specifies "Round 3 with a new blocker" for the checkpoint route.

But "a change that needs review" still lacks its needs-review predicate.
Dispatched → Taken(change) → Leaf → Finishing → Done is enabled. Without risk,
size, grouping, wiring or parent-review data it cannot be classified as an
allowed handover or forbidden bypass. R5/R6/R16 permit skips; imposing review
on every change would contradict the source. Reviewer/author/fixer identity,
fresh reviewers, revision-bound proof, blocker data, checkpoint notes, reverts
and repair hunks remain unobservable (R2, R8–R10, R16, R19, R21–R22, R33,
S8–S10). Continuation beyond a blocked fourth round is outside the bound.

H16 says the Question captain "researches first, then answers the need" or
"passes it up"; the receiving officer "waits on it". These are different runs.
Dispatched → Taken(verdict) → Leaf → Returned is an unsettled research return,
without Finishing or Done; Returned remains terminal. The dependent holder's
Leaf → Question → Question → Leaf still waits. Question cannot exit to
Returned. Neither the Question property nor wayfinding checks forbids the new
Leaf → Returned move. Question-open status, answers and child identities are
absent, so the open unsettled return remains only phase-level.

H10: "Finding the way towards the spec, one manageable part at a time."
Dispatched → Taken → Split → Waiting → Finishing lacks a route predicate to
show whether wayfinding was required. Taken → Wayfinding → Split → Waiting
→ Wayfinding → Split → Waiting → Finishing shows gradual order but no size,
route visibility, child receipt or well-timed human point. Without fairness,
waiting may last forever. S1's cut, R34's family preference and S8's one posted
verdict are unobservable on Taken(verdict) → Split → Waiting → Finishing.
Contract satisfaction is absent: "It's done when it's contract is met. Period."
(7b3d2e3e) is checked only as phase order.

## Controls and results

Registered scratch trace: Question(change) → Finishing, round 1, extra FALSE,
kind unchanged; H16_ResumeLeafAfterQuestion must reject it. Manual controls
use the same observer injection, each a one-line scratch model change,
preserving round 1 and extra FALSE:

- Replace observer Init/Next with Waiting(verdict) → InReview(verdict): R34_VerdictNeverReviewed must reject it.
- Replace observer Init/Next with InReview(change) → Finishing(change): R8_ClearBeforeFinish must reject it.
- Replace observer Init/Next with Wayfinding(change) → Finishing(change): H10_WayfindingLeavesByCut and H10_PartBeforeFogFinish must reject it.

All 18 properties above and NoDeadlock hold: 1,941 generated states, 1,224
distinct, queue empty, depth 24. The ordinary runner exited 0. No tested
obligation failed; the semantic limits and witness traces above remain open.
The registered control was rejected by H16_ResumeLeafAfterQuestion; the three
manual controls were rejected by exactly the properties named above. Every
managed control checker exited 1, and each control harness exited 0.
No committed machine is changed. The size gate exited 0; build exited 0 with
0 warnings and 0 errors. Dydo check exited 0: 0 errors, 0 warnings in 1,021 files.

## Related

- [Statechart property runner](../README.md)
