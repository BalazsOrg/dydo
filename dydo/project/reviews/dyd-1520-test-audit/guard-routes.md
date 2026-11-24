---
area: project
type: reference
---

# Guard-Routes Test Audit

The [DYD-1520](https://linear.app/bodnar-balazs/issue/DYD-1520) audit of the guard-routes batch
against the [test rubric](./rubric.md): the guard's PR-ready, merge-route, mode-set, worker-lane and
worktree-removal routes at the hook entry, and `dydo pr-ready`. One line per case is in
`guard-routes.tsv` beside this page (`dydo check` takes no link to a non-Markdown file), audited at `3f248ab5` (`DYD-1520-batch-base` before the
rubric merged).

## Scope

872 executed cases in eight classes: `GuardPrReadyTests` (436), `GuardModeSetTests` (105),
`GuardWorkerLaneTests` (100), `GuardWorktreeRemovalTests` (98), `GuardMergeRouteTests` (76),
`PrReadyCommandTests` (28), `WorkerLaneClassificationTests` (25) and `GuardWorkerLaneMergeTests` (4).
The base run passed all 872 in 57 s on Linux (`--filter` on the eight classes, `MaxCpuCount=2`).

## Totals

| Verdict | Cases |
|---|---|
| keep | 851 |
| rewrite | 2 |
| remove | 19 |
| keep `unsure` | 0 |

Most of the batch is corpus tables: each row is a distinct shell spelling, lane, tree state or
payload form that the guard reads. Under §5 two rows collapse only when they differ in no input
feature the code reads, so a row is removed here only where a kept case runs the same input in every
field the guard reads, asserts its observable at least as strictly, or is shown red by a planted bug
the row fails on. No row in any table repeats another row of its own table.

The 19 removals:

- **11 worker-lane rows** (`GuardWorkerLaneTests.Worker_GateDydoCommand_Allowed` [0, 3, 6, 8, 9, 11,
  12, 14, 17] and `Worker_NonGateDydoCommand_IsRefused` [2, 6]) run `dydo <word>` alone, exactly as
  the classification table's row for that word does. The payloads differ only in `agent_type` and the
  `agent_id` value, which the guard never reads (it checks only that `agent_id` is present,
  `Commands/GuardCommand.cs:291`).
- **6 merge-route rows** (`GuardMergeRouteTests.RouteReading_IsRefusedOrAllowedAtTheHook` [13, 14,
  19, 26, 30, 33]) run the same tool and command as a kept fact (`WrappedRoute`,
  `RouteInACommandSubstitution`, `BacktickCrlfInAWord`, `PushToADefaultName`). Those facts add
  `hook_event_name: PreToolUse`, and four of them assert a refusal only as "not allowed", so planted
  bugs P1 and P2 cover the deny shape and the absent event.
- **`GuardPrReadyTests.PrCall_MeetsItsTreesOutcome[1]`** (dirty tree, Bash, `gh pr create --fill`)
  is `TreeNotReady[0]`'s first two tries of the same input.
- **`GuardModeSetTests.EveryLane_MeetsTheReminderOnceThenPasses[0]`** ("no permission mode") differs
  from `ModeSets[0]` only in `permission_mode`, which `Models/HookInput.cs` does not read.

The rewrite: `GuardMergeRouteTests.MergeRoute_WithAMatchingWarn_RefusesEveryTryAndLeavesTheWarnMarker`
(both rows) asserts that the warn's marker file is absent, which is a step (§2 row 4). DYD-386's outcome
is that a hard block never spends the warn, so the same command still meets the warn first once it is
allowed. The rewrite drives that outcome with a push that is refused on `main` and allowed on
`feature`. For `gh pr merge 5` the marker is unobservable, because that call is refused for every
caller in every tree.

## Windows CI

`GuardPrReadyTests` and `GuardMergeRouteTests` run in the `guard-windows` job. In the last
`feature/dydo-4-0-0` run (45064255456, 2026-11-22) they took 534.1 s and 45.0 s, at about 1.17 s
per `PrCall` row and 0.30 s per `RouteReading` row. This batch drops one `PrCall` row and six
`RouteReading` rows, about 3 s of the job's 730 s of tests. Fewer rows are not the lever here.
Each `GuardPrReadyTests` row costs about 1.2 s on Windows because it copies a git template and spawns
the guard twice, and almost every row is a distinct input the guard reads.

## Mutation

Targeted two-phase mutation (`/var/tmp/dyd1520/tools/stryker-targeted.sh`). Phase 1 ran on a
clean checkout at `3f248ab5`, over every method that loses a case or is rewritten, plus the kept
cases named as survivors:

- **Phase 1:** 141 tests and 3,128 mutants tested in 3,467 s. **802 killed**, 29 timed out,
  2,297 survived. The rest were skipped: 10,795 had no coverage, 1,689 were compile errors and
  1,620 were ignored.
- **`redundancy.py --remove`** on the 19 removed and 2 rewritten cases exits 0 and lists 12 mutants
  as killed only by those cases: `CommandPosition.cs` 28, 104, 190, 199 (twice);
  `GuardCommand.MergeRoutes.cs` 18 and 30; `PrCallScanner.MergeRoutes.cs` 73 (twice);
  `ShellReader.cs` 233 and 299; `ConfigFileLocator.cs` 16 (the rewrite's marker assertion). That
  list is not a loss. Stryker 5.1.0 bails by default: it stops a mutant's run at the first failing
  test, so `killedBy` names only the first killer. Each of the 11 non-rewrite mutants is covered by
  the dropped row's named survivor (`WrappedRoute`, `BacktickCrlfInAWord`,
  `RouteInACommandSubstitution` or `PushToADefaultName`), and the survivor runs the same input.
  `CommandPosition.cs:28` is a static initializer that Stryker cannot attribute to a test.
- **Phase 2** waits for the apply step. It mutates these spans on the candidate, runs every kept
  case, and is the check that each of the 802 kills is still a kill.

The tool's filter cannot select a single theory row (the `dotnet test` filter rejects the quote and
comma a row's display name holds), so `PrCall[1]` itself is not in phase 1. Its survivor
`TreeNotReady` is: that case runs the same input, and its first two tries are the dropped row's two.
Planted bug P3 shows both red.

## Planted bugs

Each was planted in a scratch checkout at `3f248ab5`, built, and run against the named tests, then
reverted. None was committed.

| Bug | Change | Dropped rows | Named kept tests |
|---|---|---|---|
| P1 | `BlockIfMergeRoute` returns `-1 + RefusePr(...)`, so a merge-route refusal exits 1 instead of the deny object | `RouteReading` [13, 14, 19, 26, 30, 33] red | `RouteReading[0]`, `[8]`, `RouteInACommandSubstitution` and `BacktickCrlfInAWord` red; `WrappedRoute` and `PushToADefaultName` green (they assert only "not allowed"); 36 of 76 red |
| P2 | `IsPreToolUseHook` reads only `hook_event_name == "PreToolUse"`, so a payload naming no event is not the hook | the same six red | `RouteReading[0]` and `[8]` red; every fact that sends `PreToolUse` green; 34 of 76 red |
| P3 | `PrReadyCheck.StatusProblems` drops untracked files | `PrCall[1]` red | `TreeNotReady[0]` red; 96 of 279 red |

Applying the rewrite needs one more red proof: plant a bug that silently arms the warn marker after
a hard block, and watch the rewritten case fail.

## Unsure cases

None. Every kept case meets §2: each class carries a true `Job` tag (`GuardPrReadyTests` and
`GuardModeSetTests` also tag some methods), drives the guard hook entry or `dydo pr-ready`, and
asserts the decision or output an agent sees.

## Deviations

- **Audited at `3f248ab5`, before the rubric merged**, as the brief's audit-only rule required. The
  rubric's merge (`352b1e57`) changed no rule the verdicts use.
- **Apply step not done.** The captain lifted the audit-only rule mid-hop. Then the host's
  permission classifier denied the edits that delete test rows, so no test is removed or rewritten
  yet. The removal list is this page and the `.tsv`; phase 2 waits for the apply step.
- **Survivors in the phase-1 filter.** Adding them is a superset, which the brief allows; it only
  adds mutants to check.

## Related

- [DYD-1520 test audit](./_dyd-1520-test-audit.md) - The batch audits and the rubric
- [Test rubric](./rubric.md) - The verdicts and the evidence a removal needs
