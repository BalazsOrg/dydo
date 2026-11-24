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
1067 s. Phase 2 is pending at the audit commit.

## Red proofs and planted bugs

Each ran against the candidate's tests with one local production edit, then the edit was restored
(`git diff --stat` empty on production code afterwards).

Pending at the audit commit. Planned: PB1 `dydo mode show` prints a second line; PB2 a fresh Claude
guard entry also carries the bare `dydo guard`; PB3 completion drops the seven rewritten words; PB4
`dydo check` fails when no file exists under `dydo/agents/`; and, for the `unsure` cases, PB5
completion also offers `task`, PB6 completion drops `complete`, PB7 a default nudge on
`git worktree add` without a word boundary.

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
