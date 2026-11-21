---
area: reference
type: reference
explains: ["verification/statechart/captain/**"]
---

# Captain prototype obligations

## Phase 1 properties 4 (DYD-1372)

Hand-written from the approved spec, his DYD-1295/DYD-1372 rulings and
AGENTS.md (K3), never from the built skill. Historical checks remain in Git.
The machine at 25bf7d11 starts at Dispatched; seen starts there through Init.
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
| H10_ReturnedPartContinues | H2, H10, R2 | Waiting exits to Wayfinding, Split, review or finishing; never intake. |
| R34_VerdictNeverReviewed | R34; 777d396e | A verdict holder never occupies InReview, Clear, Checkpoint, Fixing or Repairing. |
| K30_KindKnownBeforeWork | K30; 0f340abd, 07c86aad | Every phase except Dispatched and Returned has a known kind. |
| K30_KindStaysKnown | K30, S7; AGENTS.md | Once set, kind never changes during this ticket's run. |
| S7_UnstartedReturnHasNoWork | S6, S7; AGENTS.md | An unread-kind run visits only Dispatched and Returned. |

## New obligations and remaining limits

R34: "Reviewing the reviews is probably not a good idea." The invariant now
covers verdict paths, including leaves and split reviews, replacing
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
Dispatched → Taken(answer) → Leaf → Returned is an unsettled research return,
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

## What is not checked about answers

Whether a research answer may be reviewed has no source: no spec rule and no
word of his speaks of it. R4 covers a plan, spec or contract; R6 gives a leaf
an optional fresh evidence-check seat. No property is written about reviewing
an answer either way. Observation, not obligation: today no move guarded on
`kind = "change"` or `kind = "verdict"` is open to an answer's holder, so the
machine shows no round on an answer.

K30_KindKnownBeforeWork excludes unread in work states, without prescribing a
closed list of known kinds. K30_KindStaysKnown still preserves any set kind;
S7_UnstartedReturnHasNoWork still restricts only unread runs. All three retain
their source meaning with change, verdict and answer distinct.

## Controls and results

H2 says a holder that "cannot divide its slice coherently under its own ticket"
returns it "to the officer above for a re-cut"; a contract or sibling change
also goes through that officer. H10's "Finding the way towards the spec, one
manageable part at a time" (07c86aad) does not forbid cutting again. The old
Waiting exit set was stricter than these sources. Allowing Split corrects the
property, not the machine: the waiting officer continues its own ticket, never
returns to Dispatched or Taken. The child's return is a different run.

H2's re-cut continues through H3's dispatch-and-wait order: H3_WaitAfterSplit
also applies after Waiting → Split. K30's stable kind and R34's ban on reviewing
a verdict still apply through repeated cuts. Witness for all three known kinds:
Dispatched → Taken → Split → Waiting → Split → Waiting → Finishing → Done.
These are safety checks, not eventual progress: stuttering remains possible.
The machine cannot show that the child's ticket was settled before new parts
were cut, or by whom. It has no child ticket, settlement, officer identity,
coherence or changed-contract data; H2's authority and settlement remain owed.

Registered scratch trace: Question(change) → Finishing, round 1, extra FALSE,
kind unchanged; H16_ResumeLeafAfterQuestion rejects it. Historical manual controls
use the same observer injection, each a one-line scratch model change,
preserving round 1 and extra FALSE:

- Replace observer Init/Next with Waiting(verdict) → InReview(verdict): R34_VerdictNeverReviewed must reject it.
- Replace observer Init/Next with InReview(change) → Finishing(change): R8_ClearBeforeFinish must reject it.
- Replace observer Init/Next with Wayfinding(change) → Finishing(change): H10_WayfindingLeavesByCut and H10_PartBeforeFogFinish must reject it.

Phase 3 manual control: in a scratch copy of Captain.tla, change only
CaptainTrouble_ChildReCut's destination line from `state' = "Split"` to
`state' = "Taken"`. H10_ReturnedPartContinues rejects it, with trace
Dispatched → Taken → Split → Waiting → Taken. The managed checker exits 1;
the control harness exits 0. No committed machine is changed.

Phase 4 manual control: in a scratch copy of the current Captain.tla, change
only CaptainReview_Whole's guard from `kind = "change"` to `kind = "verdict"`.
R34_VerdictNeverReviewed rejects Dispatched → Taken(verdict) → Split → Waiting
→ InReview. This is one model line, not an observer replacement. The managed
checker exits 1 and the control harness exits 0. The registered Question →
Finishing control is also rejected by H16_ResumeLeafAfterQuestion (checker 1,
harness 0). No committed machine or registered control is changed.

All 18 properties above and NoDeadlock hold: 2,033 generated states, 1,280
distinct, queue empty, depth 24. The ordinary runner exited 0. The registered
control was rejected by H16_ResumeLeafAfterQuestion (checker 1, harness 0).
No tested obligation failed; the semantic limits above remain open.
Commands (OUT denotes a fresh scratch output directory; SKILL the installed
statechart skill directory):

```text
/var/tmp/dydo-gate-lock.sh env MSBUILDDISABLENODEREUSE=1 DOTNET_CLI_USE_MSBUILD_SERVER=0 UseSharedCompilation=false systemd-run --user --scope -p MemoryMax=8G -p MemorySwapMax=0 -p OOMPolicy=continue timeout 3600 python3 verification/statechart/run.py --machine captain --skill SKILL --out OUT
```

Add `--controls` for the registered control. Ordinary runner last line:
`captain: pass (exit 0)`. TLC: `Model checking completed. No error has been
found.`; `2033 states generated, 1280 distinct states found, 0 states left on
queue.`; `The depth of the complete state graph search is 24.`

Size gate exited 0 (all changed files at or under 20,000 bytes). Build exited
0 with 0 warnings and 0 errors. Dydo check exited 0: 0 errors, 0 warnings in
1,023 files. Both dotnet commands used the same lock and memory-cap wrapper.

## Related

## Generation 6: cheap mend and research handoff

Machine 33081889 adds Mending and mended (included in vars and the passive
observer). The earlier results are historical. All 18 properties remain;
three are added, without fairness or disabling moves.

Changed R8_FixBeforeNextRound to allow Mending as a predecessor: S17 says
"fixed in place before the parent merges if cheap" and R23 requires that
"the PASS names the head", except R24's clean syncs. Changed
R8_ExtraRoundOnlyOnceAfterClear to allow Mending after Clear: R8 says "By
default" and S17 allows the cheap fix; its mandatory new-head review is
separate from the optional "one more". Changed
R10_CheckpointBeforeFourthRound to exempt Mending from the blocked-third-round
premise: the approved draft says "After round 3 with a blocker". A clear
third round followed by a cheap fix has no such blocker. Extended
R34_VerdictNeverReviewed to Mending: "A verdict gets no review" excludes
this review/fix loop too. No source forbids reviewing an answer.

Added S17_MendAfterClear (clear provenance, same round),
R23_ReviewMendedHead (continuation requires the next review round without
spending extra), and R8_MendOnlyOnce (persistent marker and single entry).
"Once" is the skill writer's reading of R8's default, not the rule's own
word or a separate user ruling. These are safety checks, not eventual exit.
R8_ClearBeforeFinish requires a clear result after new-head review.

WorkPhaseBeforeDone already allows Leaf to finish; H16's Question is the
dependent holder's wait, a different run from research handoff. That wait
still exits only to Leaf. K30 and unread-return checks apply unchanged.

The model cannot show whether the handed-on call is really the user's,
worth his time (L18), or correctly filed as a HITL his-call child with
blocking relations (H16/H23/S10), or meets the research contract. Leaf to
Finishing also represents other handovers. It cannot show whether a should
was cheap, duplicate-checked, filed, fixed by another writer, or in the
holder's own PR. Head identity, PASS receipt, actual merge and clean-sync
ancestry are absent: R23/R24 are checked as phase order on the path modeling
new-head review. Answer exclusion from Mending is reachability, not a
source-backed prohibition; verdict exclusion is source-backed.

Renewed registered bad trace: Question(change) to Finishing, with mended
FALSE, rejected by H16. The properties' author kept it in a local
bad-trace.json; the captain then moved it into the shared controls.json,
which `run.py --controls` reads, and removed the local copy.

Generation 6 receipts: ordinary managed runner at 78683406 exited 0;
all 21 properties and NoDeadlock hold, 3,695 generated states, 2,296 distinct,
queue empty, depth 25. Output: /tmp/dyd-1372/tlc-g6-props-1.
The renewed local control was rejected only by H16_ResumeLeafAfterQuestion
(checker 1, harness 0; /tmp/dyd-1372/tlc-g6-props-3). No machine violation
remains. Both runs used the required gate lock, app-t3code-agents.slice,
8G MemoryMax and zero swap. The first control attempt caught the property
but its harness exited 2 because importing run.py created bytecode in the
checkout; that explicit scratch bytecode was removed and the successful
repeat disabled bytecode. The final TLA edits after these runs change comments
only. No machine, skill, shared runner or shared control file was changed.

- [Statechart property runner](../README.md)
