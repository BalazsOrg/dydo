---
area: project
type: reference
---

# DYD-1520 Setup Audit

The setup batch of the [DYD-1520](https://linear.app/bodnar-balazs/issue/DYD-1520) test audit:
`dydo init`, `dydo clean`, `dydo adopt`, dydo.json, `dydo mode` and shell completion, 429 executed
cases at `07642b1f3`. Each case's verdict is one line of `setup.tsv` beside this page
([on the feature branch](https://github.com/BalazsOrg/dydo/blob/feature/dydo-4-0-0/dydo/project/reviews/dyd-1520-test-audit/setup.tsv);
`dydo check` accepts only `.md` relative links), judged by the [rubric](./rubric.md).

## Totals

| Verdict | Cases |
|---|---|
| keep | 406 |
| keep `unsure` | 4 |
| rewrite | 7 |
| remove | 12 |
| **total** | **429** |

After the batch, 410 cases run (429 less the 12 removed and the 7 rewritten rows, whose assertions
now live in a kept case). Almost all of the batch already met the rubric: every case but the
`unsure` ones carries a job tag (narrowed in review round 1, below), each case drives `dydo init`,
`dydo clean`, `dydo adopt`, `dydo mode`, the guard hook or `dydo _complete`, or an edge those cannot
stage (injected save failures, a fake `/proc`, a stranded probe), and the large theories are corpora
whose rows differ in an input feature the code under test reads (a TOML form, an encoding, a
command form), so none collapse.

In CI the batch runs on Linux only: `dotnet-coverage`'s Windows leg runs just
`DynaDocs.Tests/coverage/dotnet-platforms.json`'s `windows_tests`, and `guard-windows` names none of
these classes. No case of the batch is removed, rewritten or collapsed whose assertions run only on
another platform.

## What changed

- **Removed as redundant (11).** Both rows of `VerificationModeTests` (machine-mode.feature's
  warning-and-retry outline), `ModeStateTests.Set_PersistsForTheNextReader[0]` (machine-mode.feature's
  set scenario),
  `InitCommandTests.Init_Claude_GuardHookCarriesTheHostMarker` and
  `Init_All_OnlyTheClaudeHookCarriesTheHostMarker` (`Init_WritesEachHostsFiles_AndARepeatJoinChangesNoByte`
  rows 1 and 3), and six `CompletionProviderTests.TopLevelCommands_ContainsCommand` rows (the words
  `CompletionsCommandTests.Complete_OffersTheWordsValues[0]` already asserts).
- **Removed as unreachable (1).** `ScaffoldTreeTests.WriteDydoTree_WithoutEmbeddedScaffold_Throws`
  drives an internal overload with an empty resource set no entry point passes.
- **Rewritten (7).** Seven `TopLevelCommands_ContainsCommand` rows (`index`, `adopt`, `guard`,
  `version`, `help`, `map`, `survey`) move to the `dydo _complete` boundary in
  `Complete_OffersTheWordsValues[0]`.
- **Assertions made stricter in kept tests, so the removed ones are covered.**
  `Init_WritesEachHostsFiles_AndARepeatJoinChangesNoByte` counts exactly one marked Claude guard and no
  bare one in the committed `PreToolUse`, where it only searched the file for the marked text. The
  machine-mode step `the mode shows {flag} as {value}` asserts the whole output is the one line and
  its newline, byte for byte, where it only looked for the line among the output's lines.
- **Kept after review round 1.** `InitCheckIntegrationTests.FreshInit_PassesCheck[0]` and
  `ModeStateTests.Show_MissingOrUnusableStore_ReadsTheDefault[0]` stay. The named replacements miss
  what they guard: fresh-installation.feature accepts any warning on a fresh install (PB10), and
  machine-mode.feature's show scenario discards its first `show` (PB12).
- **Tags narrowed in review round 1.** Each tag now claims only what every case it covers asserts.
  A class tag that claimed more than some of its methods show became method tags: in
  `WorktreeProcessProbeTests`, the idle-folder and missing-path cases test the Windows rename probe,
  which `dydo clean` calls only on Windows, so they no longer carry the held-folder tag. The same
  split was made in `AdoptCommandTests` (a recorded release under an exit-2 tag),
  `InstallNudgeTests` (the never-nudged commands under "installing needs permission"),
  `DownstreamConfigTests` (the local-settings case and the cases that assert no BOM),
  `ConfigServiceTests` (the strict-load and helper cases under the failed-save tag),
  `ShellCompletionInstallerTests` and `CompletionsCommandTests`. The tags of `ConfigFactoryTests`,
  `InitCheckIntegrationTests`, `ModeCommandTests`, machine-mode.feature and
  `CleanCommandTests.BusyProbeThatCannotMoveTheTreeBack_FailsThatTree_LocksIt_AndTheRunGoesOn` were
  reworded to what their cases assert. `CleanCommandTests.KeepMineMergeDriverFromTheEnvironment_DoesNotHideAnUnmergedChange`
  [0] and [2], the dry-run rows, could not fail on the trap their tag names (a dry run deletes no
  branch), so they now assert the printed verdict `branch feature would be kept` (red on PB13).
  `CleanCommandTests.Archive_ListsEntriesNewestFirst` is now kept `unsure` (below).
- A single-row theory that lost a row became a Fact under the same method name:
  `ModeStateTests.Set_PersistsForTheNextReader` (row 1). `VerificationModeTests.cs` is deleted. No helper or fixture was orphaned. No CI or
  tooling line names a removed test or class; the names that remain in
  `dydo/project/changelog/` and `dydo/project/inquisitions/` are history and stay.

## Mutation

Targeted two-phase Stryker.NET (`/var/tmp/dyd1520/tools/stryker-targeted.sh`). Phase 1 ran on a
clean detached checkout of the base `07642b1f3` over every method that loses a case or is rewritten
(35 executed cases, including the kept rows of those methods): 700 mutants Killed, 7 Timeout, wall
1067 s. Phase 2 ran on a clean detached checkout of the candidate `8d6424f04` (production code
identical), mutating the 239 line spans of those 700 kills and running all 408 kept cases of the
batch: 750 Killed, wall 774 s, `compare exit=1` with two mutants not killed after:

| Mutant | Where | After | Justification |
|---|---|---|---|
| #12522 statement removal | `Services/ScaffoldTree.cs:36`, the empty-set `throw` | NoCoverage | unreachable: only the removed `WriteDydoTree_WithoutEmbeddedScaffold_Throws` passes an empty resource set; the one production caller passes the compiled manifest, which `EmbeddedScaffoldResources_AreExactlyTheTreeOnDisk` pins to the non-empty `Scaffold/` tree |
| #12523 string mutation | the same `throw`'s message | NoCoverage | the same |

Every other phase-1 kill is Killed on the candidate. Stryker stops a mutant's run at its first
failing test, so a report's `killedBy` names only some killers; the status comparison is exact, and
each removal's named kept test is backed by a planted bug or by a mutant whose phase-2 `killedBy`
names that test.

| Run | Cases | Summed test duration | Command |
|---|---|---|---|
| base `07642b1f3` | 429 passed | 15.06 s | batch filter, `--logger trx`, `MaxCpuCount=2` |
| candidate `8d6424f04` | 408 passed | 16.28 s | the same |

The cases removed here were cheap ones, so the summed time does not drop measurably; the
difference is the shared host's load.

## Red proofs and planted bugs

Two runs of all 408 kept cases on the round-0 candidate: PB1-PB4 together, then PB2' and PB5-PB7
together. Review round 1 ran four more, each over all 410 kept cases of the batch: PB8, PB9 and
PB10 together; PB11 and PB12 together; PB1 alone on the final step; PB13 alone. Each planted bug
in a run touches a different observable, so each failure is attributed by its assertion and line.
The logs are in `/var/tmp/dyd1520-fix/` (`runA` to `runD`).

| Bug | Production edit | Kept cases red | What it proves |
|---|---|---|---|
| PB1 | `ModeCommand.Print` writes a second line | machine-mode.feature: Show lists every flag, Set changes a flag, Set refuses [0] and [1], Warning and retry [0] and [1]; ModeStateTests rows; ModeCommandTests.Set_PrintsEveryFlagAfterTheChange | red proof of the exact show step; covers the removed exact-output assertions of ModeStateTests.Set_PersistsForTheNextReader[0] and VerificationModeTests |
| PB2 | a fresh Claude guard entry (no earlier guard) also carries `dydo guard` | Init_WritesEachHostsFiles [1] and [3], Init_HostAgentSettings_PreserveSatisfyingValuesAndRepeatBytes | red at the repeat-join byte check, not at the new count line, so PB2' below |
| PB2' | every Claude guard entry also carries `dydo guard` | Init_WritesEachHostsFiles [1] and [3] red at the new `Count(CodexGuard) == 0` line (`InitCommandTests.cs:936`, expected 0, actual 1), and six other Claude wiring cases | red proof of the strengthened guard counts; covers both removed host-marker cases |
| PB3 | `TopLevelCommands` drops index, adopt, guard, version, help, map and survey | Complete_OffersTheWordsValues[0]: `Assert.All` 7 of 14 not found, exactly those seven | red proof of the seven rewritten rows |
| PB4 | `dydo check` exits 1 when `dydo/agents/` holds no file | fresh-installation.feature [0]-[3]; the kept scratch-file `FreshInit_PassesCheck` stays green | superseded: review round 1 kept `FreshInit_PassesCheck[0]` (PB10) |
| PB8 | `ModeCommand.Print` writes `overnight: off ` (a trailing space) | machine-mode.feature: Show lists every flag, Set changes a flag, Set refuses [0] and [1], Warning and retry [0] and [1], all at `MachineModeSteps.cs:72` (expected `overnight: off\n`, actual `overnight: off \n`) | red proof of the byte-exact show step, which the round-1 `Trim()` step accepted |
| PB1 again | as PB1, on the final step | the same six scenarios at `MachineModeSteps.cs:72`, and ModeStateTests' Show rows, Set_PersistsForTheNextReader, ModeCommandTests.Set_PrintsEveryFlagAfterTheChange | the byte-exact step still rejects a second line |
| PB9 | `dydo init`'s Claude guard is written unmarked (`dydo guard`) | Init_WritesEachHostsFiles [1] and [3] at `InitCommandTests.cs:935` (expected 1, actual 0), and eight other Claude wiring cases | breaks the removed `Init_All_OnlyTheClaudeHookCarriesTheHostMarker`'s Contains of the marked command; row [3] covers it |
| PB10 | `dydo check` also prints `WARNINGS:` and `dydo/agents/sample/bad-doc.md: stale scratch warning`, exit unchanged | FreshInit_PassesCheck [0] and [1] at `InitCheckIntegrationTests.cs:23`; fresh-installation.feature's warnings-only step passes it (its rows fail only after customising, at `OptionalSummarySteps.cs:48`) | the reviewer's counterexample: row [0] stays |
| PB11 | the Codex guard is written as `dydo guard --host claude` | Init_WritesEachHostsFiles [2] and [3] at `InitCommandTests.cs:950` (`--host` found in .codex/hooks.json), and Codex wiring cases | breaks the removed `Init_All_OnlyTheClaudeHookCarriesTheHostMarker`'s DoesNotContain(`--host`); row [3] covers it |
| PB12 | the first `dydo mode show` over a missing store creates it and prints `created the mode store` | Show_MissingOrUnusableStore_ReadsTheDefault[0] at `ModeStateTests.cs:27`; machine-mode.feature's Show lists every flag stays green (its first show is discarded); Set refuses [0] and [1] and two mode-store-failure rows red where a later show meets a missing store | the reviewer's counterexample: row [0] stays |
| PB13 | the isolated git run keeps every inherited `GIT_*` variable | KeepMineMergeDriverFromTheEnvironment [1] and [3] at `CleanCommandTests.cs:355` (branch deleted), [0] and [2] at `:358` (no `branch feature would be kept`), and ScratchGitEnvironment_ShutsOutSystemAttributesAndConfig_WhateverIsInherited | red proof of the dry-run rows' new verdict assertion |
| PB5 | `TopLevelCommands` also lists `task` | only TopLevelCommands_ExcludesRetiredWorkCommands | the `unsure` case's `only` |
| PB6 | `TopLevelCommands` drops `complete` | only TopLevelCommands_ContainsCommand (`complete`) | the `unsure` case's `only` |
| PB7 | a default warn nudge `git\s+worktree\s+add` | only Guard_DefaultNudges_WarnOnTheCommand (`digit worktree add foo`) | the `unsure` case's `only` |

Afterwards `git checkout -- Commands Services` restored production code and `git status` was clean.

## Kept `unsure`

- `CompletionProviderTests.TopLevelCommands_ContainsCommand[10]` (`complete`). Protection: the first
  word `complete` is offered. It pins a defect: the command is `_complete`
  (`Commands/CompleteCommand.cs:21`), so `dydo complete` is a parse error. Unresolved: whether
  completion should offer it; fixing `CompletionProvider` is a product change outside this ticket,
  after which this case goes.
- `CompletionProviderTests.TopLevelCommands_ExcludesRetiredWorkCommands`. Protection: `task`,
  `issue` and `review` are not offered. It checks a deleted feature is gone, with no traced
  contract. Unresolved: a boundary property "every first-word candidate is a registered command"
  would cover it, but that property is red today on `complete`, and completion also misses `mode`
  and `statechart` (reported on DYD-1199).
- `ConfigFactoryTests.Guard_DefaultNudges_WarnOnTheCommand[10]` (`digit worktree add foo`).
  Protection: no default nudge fires on a word that merely contains `git`. No default nudge names
  `git` or `worktree` any more (`Services/ConfigFactory.cs`), so the row asserts the absence of a
  retired nudge, and the method's tag (DYD-736, issue 0177) does not cover it. Unresolved: no record
  says whether a git nudge returns.
- `CleanCommandTests.Archive_ListsEntriesNewestFirst`. Protection: `dydo clean --archive` lists the
  newer entry before the older one. dydo-commands-worktrees.md names no order; only the
  `Services/CleanArchive.cs:84` comment says newest first. Unresolved: whether the order is a
  contract. No mutant or planted bug is recorded yet that only this case fails on.

## Deviations

- The review-round-1 tag sweep narrowed every tag of the batch except `InitCommandTests`'. A read of
  that class found false or over-broad tags whose fixes need calls this hop does not make (a cited
  spec line the case contradicts, refused-init cases that check only some files, a row whose
  data shows no merge). Those findings went to the DYD-1520 captain, and the class's tags stand as
  audited until they are settled.

- A strengthened assertion in a kept test (the guard counts, the exact show step) is recorded as
  keep with the absorbed assertion named in its Reason, not as a rewrite: its job and boundary are
  unchanged, and its red proof is recorded here.
- `ScaffoldTree.WriteDydoTree(string, IEnumerable<string>)` is internal and its comment says it
  exists so a test can witness the empty-set throw; with that test gone it is a seam with no test
  purpose. Production code is out of scope here, so it stays; a follow-up can fold it into the
  public overload.

## Related

- [DYD-1520 test audit](./_dyd-1520-test-audit.md) - The audit files of every batch
- [DYD-1520 test rubric](./rubric.md) - What each verdict here applies
