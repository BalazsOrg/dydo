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
| H1, AGENTS.md (Sort yourself), Ticket Captain §5 | A return ends this holder's run. **Holds** (then `H2_ReturnedIsTerminal`). Returned only stutters. |

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
| R19, R21, R22 | **Holds at phase level** (`R19_ReviewAfterMergeRepair`). Taken → Leaf → Finishing → Repairing → InReview → Finishing requires review before a retry. The trace cannot distinguish a conflict from a real red gate, show the repair branch or SHA, verify its hunks and reviewer, or show that a real red was rerun, recorded and reverted. The skill's return to the officer for a repair that cannot be resolved has no transition out of Repairing. |
| H1, AGENTS.md (Sort yourself), Ticket Captain §5 | **Holds** (then `H2_ReturnedIsTerminal`). Returned only stutters. |

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
| `S6_WorkBeforeDone` | DYD-1295 7b3d2e3e | Holds at phase level. Done follows a work phase and Finishing. |
| `H3_WaitAfterSplit` | H3, DYD-1295 df6a785e | Holds. The next phase after Split is Waiting. |
| `H16_ResumeLeafAfterQuestion` | H16, S10, DYD-1295 519126ab | Holds. A leaf's Question resumes in Leaf. |
| `R8_ClearBeforeFinish` | R8 | Holds. A review produces Clear, and once review begins, every entry to Finishing comes from Clear. |
| `R8_FixBeforeNextRound` | R8, R9, S9, DYD-1295 41a553f1 | Holds. A round advances to a fresh review from Fixing or Clear. Fixing can resume review in the next numbered round or return to the officer. |
| `R8_ExtraRoundOnlyOnceAfterClear` | R8, captain phase 4 | Holds. Clear may finish or advance once, consuming the unused extra round. The marker starts unused and stays consumed after use. |
| `R10_CheckpointBeforeFourthRound` | R10, S10, DR 070 §6 | Holds. A third-round Fix and the fourth round following it require the checkpoint. |
| `S10_CheckpointBeforeThirdRoundEscalation` | R10, S10 | Holds. A blocked third review round returns to its officer via Checkpoint. |
| `R10_EnterCheckpointFromThirdRound` | R10, S10, DR 070 §6 | Holds. Checkpoint starts from InReview in round three and preserves the round number. |
| `R19_ReviewAfterMergeRepair` | R8, R19, R21, R22, merge-conflict and red-gate resources | Holds. Repairing exits to a fresh review or to the officer. Retrying after a repair requires Clear. |
| `H2_ReturnedIsTerminal` | H1, AGENTS.md (Sort yourself), Ticket Captain §5 | Holds. Returned ends the holder’s run. |

TLC again rejected the repair and checkpoint shortcuts from hop 3b.

## Recheck with an unsettled Question (DYD-1372 hop 3d)

H16 says the receiving officer “opens it … and waits on it” and that the
Question captain “researches first, then answers the need, sends it to a council
… or passes it up toward his HITL queue.” In 519126ab Balazs says, “For more
complex one it creates a sub captain to investigate and answer the thing.” The
leaf resumes only after the answer. If the Question comes back unsettled, the
holder passes it up and leaves it open. The waiting leaf cannot go straight to
review or finish. The `Question → Returned` route ends this holder's run while
the Question stays open with the officer. TLC checks the routing and the end of
the run. [Spec and rulings](https://linear.app/bodnar-balazs/document/spec-dydo-400-08326afbb349) · [A need for the user](../../../skills/roles/officers/ticket-captain/resources/need-for-the-user.md)

| Property | Source | TLC result and observable promise |
|---|---|---|
| `WorkPhaseBeforeDone` | DYD-1295 7b3d2e3e and AGENTS.md (How work happens here) | Holds. “It's done when it's contract is met. Period.” TLC checks a prior Leaf or Waiting phase and entry to Done from Finishing. The contract's satisfaction is unobservable. |
| `H3_WaitAfterSplit` | H3, DYD-1295 df6a785e | Holds. The next phase after Split is Waiting. |
| `H16_AnswerOrPassUpBeforeContinuing` | H16, DYD-1295 519126ab | Holds at phase level. The leaf waits in Question, resumes work via Leaf on the answer route, or leaves via Returned on the unsettled route. Review and Finishing require leaving the wait via Leaf first. |
| `R8_ClearBeforeFinish` | R8 | Holds. R8 ends the phase on a round without a new blocker, with one optional further round. Clear comes from InReview, and finishing after review comes from Clear. |
| `R8_FixBeforeNextRound` | R8, R9, S9, DYD-1295 41a553f1 | Holds. An advance goes from Fixing or Clear to the next numbered InReview. Fixing can also return to the officer. |
| `R8_ExtraRoundOnlyOnceAfterClear` | R8 | Holds. Clear can finish or consume the one extra round. The marker begins unused and stays consumed after use. |
| `R10_CheckpointBeforeFourthRound` | R10, S10, DR 070 §6 | Holds. A third-round Fix and the fourth round following it require the checkpoint. |
| `S10_CheckpointBeforeThirdRoundEscalation` | R10, S10 | Holds. A blocked third-round escalation goes via Checkpoint. |
| `R10_EnterCheckpointFromThirdRound` | R10, S10, DR 070 §6 | Holds. Checkpoint starts from InReview in round three and preserves the round number. |
| `R19_ReviewAfterMergeRepair` | R8, R19, R21, R22, merge-conflict and red-gate resources | Holds. Repairing goes to fresh review or returns to the officer. Each retry after repair requires Clear. |
| `H1_ReturnedIsTerminal` | H1, AGENTS.md (Sort yourself), Ticket Captain §5 | Holds. “Crew does its one job and returns” and “end on a commit, and return its SHA” put the return at the end of the holder's run. `Returned` allows only stuttering. |

H11 says a filler ticket that fails the captain's check “goes back unassigned
with a one-line reason.” The `Taken → Returned` arrow now belongs to
`Captain.FillerBack`. The checks use phase predicates, so the action rename
requires no property binding. [Ticket Captain](../../../skills/roles/officers/ticket-captain/SKILL.md)

### Rejected control traces

All three traces start with `extra = FALSE`. TLC exited 1 for each with the
named property broken. The commands exited 0 after detecting the expected
violation. The manual controls call the runner's `check_machine` directly.

| Trace | Property that rejected it | Run |
|---|---|---|
| Question(1) → Finishing(1) | `H16_AnswerOrPassUpBeforeContinuing` | Registered control |
| Repairing(1) → Finishing(1) | `R19_ReviewAfterMergeRepair` | Manual control |
| InReview(3) → Returned(3) | `S10_CheckpointBeforeThirdRoundEscalation` | Manual control |

From the checkout root, run the registered control with a fresh output directory.

```sh
/var/tmp/dydo-gate-lock.sh env MSBUILDDISABLENODEREUSE=1 DOTNET_CLI_USE_MSBUILD_SERVER=0 UseSharedCompilation=false \
    systemd-run --user --scope -p MemoryMax=8G -p MemorySwapMax=0 -p OOMPolicy=continue timeout 3600 \
    python3 verification/statechart/run.py --controls --machine captain \
    --skill /tmp/dyd-1372/sc-root/skills/engineering/statechart --out /tmp/dyd-1372-hop3d/registered
```

Save this script as `/tmp/dyd-1372-hop3d/manual-controls.py`. It runs the two
manual controls through the same managed TLC path and writes a record for each.

```python
from argparse import Namespace
import os
from pathlib import Path
import sys

root = Path.cwd()
sys.path.insert(0, str(root / 'verification/statechart'))
import run

source_head = run.checkout_head(root)
if run.checkout_status(root):
    raise SystemExit('checkout is dirty')

model = root / 'claims/statechart/captain/Captain.tla'
environment = dict(os.environ, JAVA_TOOL_OPTIONS='-Xmx512m -XX:+UseParallelGC')
skill = Path('/tmp/dyd-1372/sc-root/skills/engineering/statechart')
controls = [
    ('repair', 'Repairing', 'Finishing', '1', 'R19_ReviewAfterMergeRepair'),
    ('checkpoint', 'InReview', 'Returned', '3', 'S10_CheckpointBeforeThirdRoundEscalation'),
]
for name, start, end, round_number, property_name in controls:
    out = Path('/tmp/dyd-1372-hop3d') / name
    out.mkdir(exist_ok=False)
    args = Namespace(out=out, skill=skill, baseline=False, controls=True)
    control = {'from': start, 'to': end, 'property': property_name,
               'variables': {'round': round_number, 'extra': 'FALSE'}}
    record = run.check_machine(model, root, args, environment, source_head, control)
    run.save_records(out, [record])
    result = run.finish(root, source_head, [record], args)
    print(f'{name}: {record["result"]} (exit {record["exit"]}); broken: {record.get("broken")}')
    if result:
        raise SystemExit(result)
```

```sh
/var/tmp/dydo-gate-lock.sh env MSBUILDDISABLENODEREUSE=1 DOTNET_CLI_USE_MSBUILD_SERVER=0 UseSharedCompilation=false \
    systemd-run --user --scope -p MemoryMax=8G -p MemorySwapMax=0 -p OOMPolicy=continue timeout 3600 \
    python3 -B /tmp/dyd-1372-hop3d/manual-controls.py
```

### What remains unobservable

The traces below show where the model loses the distinction required by the
rule. The model has no fairness assumption, so the checks establish safety on
observed moves. They do not establish that a holder makes those moves eventually.

| Source | Trace and missing evidence |
|---|---|
| DYD-1295 7b3d2e3e, AGENTS.md (How work happens here), S6 | Taken → Leaf → Finishing → Done carries no contract predicate, acceptance, proof receipt or identity tying start and close to the same holder. |
| R2, R4, R12, R16 | Taken → Split → Waiting → Finishing → Done carries no wiring or review eligibility. Taken → Leaf → InReview carries no ticket kind, own versus parent merge, revision-bound proof or separate Questions-only spec pass. |
| H2, H3, S7, df6a785e, 0f340abd, 3fb363e3 | Taken → Split → Waiting → InReview carries no child identity, completion, receipt, context size, cohesion, cut authority or dispatch kind. Taken → Leaf → Finishing cannot show whether oversized work should have been cut or returned. |
| H16, S10, DYD-1295 519126ab | Leaf → Question → Leaf carries no classification, research, answer, fresh Question holder or blocked scope. Leaf → Question → Returned carries no open Question ticket, officer identity, research returned, council or onward escalation. Split → Waiting has no distinct route for a split captain's Question, re-plan Question or passed-up human need. |
| H11 | Taken → Returned carries no filler classification or eligibility result, assignment state, one-line reason, Wayfinding blocking relation or release. |
| H1, Ticket Captain §5 | Finishing → Done → Returned carries no returned SHA, receipts, recipient or closed-thread signal. Returned stutters in this machine; a new holder's run is outside its scope. |
| R8, R9, R11, R16, R33, S8, S9, 2119cd65, df6a785e, 32cc5599, 41a553f1 | InReview → Fixing → InReview has no blocker items or severity, shoulds, completed Fix and proof on the head, reviewer/fixer identities, seats, verdict origin, lenses or security-first ordering. InReview → Clear reports the phase but carries no verdict evidence. Clear → InReview consumes the extra round but carries no officer decision or identity. |
| R9, R10, S10 | InReview(3) → Checkpoint(3) → Returned(3) carries no blocker, checkpoint note, reason or officer decision. InReview → Returned outside round three also carries no reason. InReview(4) → Fixing(4) is reachable, with the next review at round five beyond `round <= 4`. |
| R19, R21, R22 | Finishing → Repairing → InReview → Clear → Finishing carries no conflict/red distinction, repair branch/SHA, reviewed hunks, reviewer, rerun, flake, revert, CI record or re-land evidence. Repairing → Returned carries no evidence that the repair was unresolved. |

## Related

- [Statechart property runner](../README.md)
