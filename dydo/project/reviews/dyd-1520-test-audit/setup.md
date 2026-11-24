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
| keep | 405 |
| keep `unsure` | 3 |
| rewrite | 7 |
| remove | 14 |
| **total** | **429** |

After the batch, 408 cases run (429 less the 14 removed and the 7 rewritten rows, whose assertions
now live in a kept case). Almost all of the batch already met the rubric: every class but
`CompletionProviderTests` carries a true class-level job tag, each case drives `dydo init`,
`dydo clean`, `dydo adopt`, `dydo mode`, the guard hook or `dydo _complete`, or an edge those cannot
stage (injected save failures, a fake `/proc`, a stranded probe), and the large theories are corpora
whose rows differ in an input feature the code under test reads (a TOML form, an encoding, a
command form), so none collapse.

In CI the batch runs on Linux only: `dotnet-coverage`'s Windows leg runs just
`DynaDocs.Tests/coverage/dotnet-platforms.json`'s `windows_tests`, and `guard-windows` names none of
these classes. No case of the batch is removed, rewritten or collapsed whose assertions run only on
another platform.

## What changed

- **Removed as redundant (13).** `InitCheckIntegrationTests.FreshInit_PassesCheck[0]`
  (fresh-installation.feature's first row), both rows of `VerificationModeTests` (machine-mode.feature's
  warning-and-retry outline), `ModeStateTests.Show_MissingOrUnusableStore_ReadsTheDefault[0]` and
  `Set_PersistsForTheNextReader[0]` (machine-mode.feature's show and set scenarios),
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
  machine-mode step `the mode shows {flag} as {value}` asserts the whole output equals the one line,
  where it only looked for the line.
- Two single-row theories that lost a row became Facts under the same method name:
  `InitCheckIntegrationTests.FreshInit_PassesCheck` (row 1) and `ModeStateTests.Set_PersistsForTheNextReader`
  (row 1). `VerificationModeTests.cs` is deleted. No helper or fixture was orphaned. No CI or
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
names that test. `fresh-installation.feature` runs dydo as a child process, which mutation cannot
see, so its coverage of `FreshInit_PassesCheck[0]` rests on PB4.

| Run | Cases | Summed test duration | Command |
|---|---|---|---|
| base `07642b1f3` | 429 passed | 15.06 s | batch filter, `--logger trx`, `MaxCpuCount=2` |
| candidate `8d6424f04` | 408 passed | 16.28 s | the same |

The cases removed here were cheap ones, so the summed time does not drop measurably; the
difference is the shared host's load.

## Red proofs and planted bugs

Two runs of all 408 kept cases on the candidate's tests: PB1-PB4 together, then PB2' and PB5-PB7
together. Each planted bug touches a different observable, so each failure is attributed by its
assertion.

| Bug | Production edit | Kept cases red | What it proves |
|---|---|---|---|
| PB1 | `ModeCommand.Print` writes a second line | machine-mode.feature: Show lists every flag, Set changes a flag, Set refuses [0] and [1], Warning and retry [0] and [1]; ModeStateTests rows; ModeCommandTests.Set_PrintsEveryFlagAfterTheChange | red proof of the exact show step; covers the removed exact-output assertions of ModeStateTests [0] rows and VerificationModeTests |
| PB2 | a fresh Claude guard entry (no earlier guard) also carries `dydo guard` | Init_WritesEachHostsFiles [1] and [3], Init_HostAgentSettings_PreserveSatisfyingValuesAndRepeatBytes | red at the repeat-join byte check, not at the new count line, so PB2' below |
| PB2' | every Claude guard entry also carries `dydo guard` | Init_WritesEachHostsFiles [1] and [3] red at the new `Count(CodexGuard) == 0` line (`InitCommandTests.cs:936`, expected 0, actual 1), and six other Claude wiring cases | red proof of the strengthened guard counts; covers both removed host-marker cases |
| PB3 | `TopLevelCommands` drops index, adopt, guard, version, help, map and survey | Complete_OffersTheWordsValues[0]: `Assert.All` 7 of 14 not found, exactly those seven | red proof of the seven rewritten rows |
| PB4 | `dydo check` exits 1 when `dydo/agents/` holds no file | fresh-installation.feature [0]-[3]; the kept scratch-file `FreshInit_PassesCheck` stays green | keyed to the removed row's input (no scratch file): the kept scenario covers it |
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
  retired nudge, and the class tag (DYD-736, issue 0177) does not cover it. Unresolved: no record
  says whether a git nudge returns.

## Deviations

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
