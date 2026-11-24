---
area: project
type: reference
---

# DYD-1520 Setup Audit

The setup batch of the [DYD-1520](https://linear.app/bodnar-balazs/issue/DYD-1520) test audit:
`dydo init`, `dydo clean`, `dydo adopt`, dydo.json, `dydo mode` and shell completion, 429 executed
cases at `07642b1f3`. Each case's verdict is one line of `setup.tsv` beside this page
([on the feature branch](https://github.com/BalazsOrg/dydo/blob/feature/dydo-4-0-0/dydo/project/reviews/dyd-1520-test-audit/setup.tsv);
`dydo check` accepts only `.md` relative links), judged by the [rubric](./rubric.md). Every tag
clause is mapped to the assertion line that proves it in [setup-tags.md](./setup-tags.md), a page
of its own so each stays under the 20 KB file limit.

## Totals

| Verdict | Cases |
|---|---|
| keep | 394 |
| keep `unsure` | 16 |
| rewrite | 7 |
| remove | 12 |
| **total** | **429** |

After the batch, 410 cases run (429 less the 12 removed and the 7 rewritten rows, whose assertions
now live in a kept case). Each case drives `dydo init`, `dydo clean`, `dydo adopt`, `dydo mode`, the
guard hook or `dydo _complete`, or an edge those cannot stage (injected save failures, an injected
mover, a simulated `/proc`), and the large theories are corpora whose rows differ in an input
feature the code under test reads, so none collapse. In CI the batch runs on Linux only
(`dotnet-platforms.json`'s `windows_tests` and `guard-windows` name none of these classes); no case
is removed, rewritten or collapsed whose assertions run only on another platform, and none runs
only on Windows (coordinator ruling 2 lists none here).

## What changed

- **Removed as redundant (11).** Both rows of `VerificationModeTests` (machine-mode.feature's
  warning-and-retry outline), `ModeStateTests.Set_PersistsForTheNextReader[0]` (machine-mode.feature's
  set scenario), `Init_Claude_GuardHookCarriesTheHostMarker` and
  `Init_All_OnlyTheClaudeHookCarriesTheHostMarker` (`Init_WritesEachHostsFiles_AndARepeatJoinChangesNoByte`
  rows 1 and 3), and six `CompletionProviderTests.TopLevelCommands_ContainsCommand` rows.
- **Removed as unreachable (1).** `ScaffoldTreeTests.WriteDydoTree_WithoutEmbeddedScaffold_Throws`.
- **Rewritten (7).** Seven `TopLevelCommands_ContainsCommand` rows move to `dydo _complete` in
  `Complete_OffersTheWordsValues[0]`.
- **Stricter kept assertions.** The Claude guard counts in `Init_WritesEachHostsFiles_...`, and the
  machine-mode step `the mode shows {flag} as {value}`, now byte-exact.
- **Review round 1.** `InitCheckIntegrationTests.FreshInit_PassesCheck[0]` and
  `ModeStateTests.Show_MissingOrUnusableStore_ReadsTheDefault[0]` stay (PB10, PB12); tags were
  narrowed and class tags split; the dry-run rows of `KeepMineMergeDriverFromTheEnvironment` assert
  their printed verdict (PB13).
- **Review round 2.** Each finding closed as asked:
  1. `WorktreeProcessProbeTests`: the held-folder tag is gone. The move-back cases are tagged as an
     injected mover, the scans as a simulated `/proc` tree, and the own-process exclusion, which
     the fixture's cwd-less own-PID folder cannot exercise, is no longer claimed.
  2. machine-mode.feature's show scenario: its Reason records the narrower observation (a first
     show runs unchecked, the Then step's second show is exact); the first show's own result is
     `Show_MissingOrUnusableStore_ReadsTheDefault[0]`'s (PB12).
  3. `LoadConfigStrict_RejectsInvalidTestingRunner`: the tag and each row name the invalid form,
     and the NUL row cites DYD-117 (`2f06421af`), since configuration.md does not forbid NUL.
  4. `InstallNudgeTests`: the tag names the classes the rows cover, each row is traced to DYD-499
     in [setup-tags.md](./setup-tags.md#the-install-nudge-rows-traced-to-dyd-499), and PB15 shows
     the denial on line 54 is the install nudge's for every row.
  5. The PowerShell completions row, the downstream local-settings case and the stale-rule case
     claim only what they assert.
  6. The should: `Archive_ListsEntriesNewestFirst` asserts both entries are listed before their
     order (red on PB14).
- **The lesson-10 and lesson-11 pass over every tag.** All 159 methods and the three feature
  files: each clause now names what an assertion checks (`setup-tags.md`). Class tags some methods
  did not prove became method tags (`ModeStateTests`, `ModeCommandTests`, `DownstreamConfigTests`);
  `CleanCommandTests`' class tag is now the bare cite, its method tags say what each case checks.
  Cites were checked against their sources: traps first fixed under DYD-86 (host settings) and
  DYD-134 (atomic save, late failures) now cite them, not DYD-392, and DYD-368 for the failed
  preflight. Three methods (six cases) whose claim no source states moved to keep `unsure`, their
  tags dropped (below). Reason cells that claimed more than the case checks were narrowed.
- `ModeStateTests.Set_PersistsForTheNextReader` became a Fact (row 1). `VerificationModeTests.cs`
  is deleted. No helper or fixture was orphaned, and no CI or tooling line names a removed test.

## Mutation

Targeted two-phase Stryker.NET (`/var/tmp/dyd1520/tools/stryker-targeted.sh`) on identical
production code. Phase 1 on the base `07642b1f3` over every method that loses a case or is
rewritten: 35 cases, 700 Killed, 7 Timeout. Phase 2 mutates those 239 spans and runs every kept case:

| Phase 2 | Cases | Killed | Wall | Compare |
|---|---|---|---|---|
| `8d6424f04` (round 0) | 408 | 750 | 774 s | exit 1, the two below |
| `74f105028` (round 1) | 410 | 750 | 932 s | exit 1, the two below |
| `3d73677f2` (round 2) | 410 | 751 | 732 s | exit 1, the two below |

| Mutant | Where | After | Justification |
|---|---|---|---|
| #12522 statement removal | `Services/ScaffoldTree.cs:36`, the empty-set `throw` | NoCoverage | unreachable: only the removed `WriteDydoTree_WithoutEmbeddedScaffold_Throws` passes an empty set; the one caller passes the compiled manifest, which `EmbeddedScaffoldResources_AreExactlyTheTreeOnDisk` pins to the non-empty `Scaffold/` tree |
| #12523 string mutation | the same `throw`'s message | NoCoverage | the same |

The batch filter's summed test duration: base 15.06 s (429 cases), round 1 15.98 s, round 2
15.55 s (410); the cases removed were cheap, so the difference is the shared host's load.

## Mutation method (coordinator ruling 1)

> **1. Option A.** The targeted two-phase run complies with §5 when both conditions hold:
> - phase 1 covers every method that was removed or whose body changed;
> - the diff changes no shared helper or fixture that an untouched test uses.
>
> Each batch PR states both conditions, with the diff evidence for the second. If either condition
> fails, the batch re-runs phase 2 over every test that uses the changed helper, including
> untouched ones.

**Condition 1.** `git diff 07642b1f3 3d73677f2 -- DynaDocs.Tests` changes the bodies of
these methods: `VerificationModeTests.ModeSet_...`
(deleted), `ModeStateTests.Set_PersistsForTheNextReader`, `InitCommandTests.Init_Claude_GuardHookCarriesTheHostMarker`
and `Init_All_OnlyTheClaudeHookCarriesTheHostMarker` (deleted), `Init_WritesEachHostsFiles_AndARepeatJoinChangesNoByte`,
`CompletionProviderTests.TopLevelCommands_ContainsCommand` (rows removed),
`CompletionsCommandTests.Complete_OffersTheWordsValues` (row widened),
`ScaffoldTreeTests.WriteDydoTree_WithoutEmbeddedScaffold_Throws` (deleted), and
`CleanCommandTests.KeepMineMergeDriverFromTheEnvironment_DoesNotHideAnUnmergedChange` (round 1)
and `Archive_ListsEntriesNewestFirst` (round 2). Every other change is a `Trait`, a comment, a
class-level tag or a feature file's `# Job:` line. The round-0 phase 1 covered the first eight
(with `FreshInit_PassesCheck` and `Show_MissingOrUnusableStore_ReadsTheDefault`), not the last
two, so a supplementary phase 1b on the base ran over them: with the six scenarios below, 11 cases, 420
Killed, 5 Timeout, wall 1263 s.

**Condition 2.** The one changed non-test-method file is `DynaDocs.Tests/Steps/MachineModeSteps.cs`
(the `the mode shows` step, now byte-exact). Its untouched users are machine-mode.feature's Show
lists every flag, Set changes a flag, Set refuses [0]-[1] and Warning and retry [0]-[1] (that
scenario also through `the owned mode state is unchanged`); no other feature uses the step.
`CompletionProviderTests.cs` changed only a header comment, and no fixture or data file changed.
Phase 1b covered those six scenarios too, and its phase 2 over the whole batch on `3d73677f2`
keeps every one of its 420 kills Killed (471 Killed in its 153 spans, compare exit 0, wall 578 s,
none only a Timeout), so the changed step loses no protection. The candidate's test tree differs
from `3d73677f2` only in `Trait` and comment lines.

**Review lesson 11 (a removal's replacement kills its mutants itself).** Stryker stops a mutant at
its first failures, so `killedBy` undercounts. Phase 1 reran over the removed and rewritten methods
alone with `--disable-bail` (20 cases, 342 Killed, wall 843 s), and phase 2 over the named
replacements alone, also without bail, on `3d73677f2` (11 cases: Warning and retry [0]-[1], Set
changes a flag, `Init_WritesEachHostsFiles` [0]-[3], `Complete_OffersTheWordsValues` [0]-[3];
wall 168 s). For each removal or rewrite, every mutant it killed is killed by its own named
replacement: `VerificationModeTests` [0] 231 of 231 and [1] 235 of 235 (Warning and retry [0], [1]),
`Set_PersistsForTheNextReader[0]` 26 of 26 (Set changes a flag), `Init_Claude_GuardHookCarriesTheHostMarker`
48 of 48 (row [1]), `Init_All_OnlyTheClaudeHookCarriesTheHostMarker` 90 of 90 (row [3]), and each of
the 13 completion rows 4 of 4 (`Complete_OffersTheWordsValues[0]`). The one p1r kill
the replacements miss is `CompletionProvider.cs:10`'s `complete` string, killed only by the kept
`unsure` row [10].

## Red proofs and planted bugs

Each planted bug was built into this worktree, the batch's kept cases run, and production code
restored (`git status` clean). Runs with several bugs touch different observables, so each
failure is attributed by its assertion line. Logs: `/var/tmp/dyd1520-fix/` (rounds 0-1) and
`/var/tmp/dyd1520-fix2/` (round 2).

| Bug | Production edit | Kept cases red | Proves |
|---|---|---|---|
| PB1 | `ModeCommand.Print` writes a second line | machine-mode Show, Set changes, Set refuses [0]-[1], Warning and retry [0]-[1] (`MachineModeSteps.cs:72`); ModeStateTests; `ModeCommandTests.Set_PrintsEveryFlagAfterTheChange` | the exact step; covers the removed exact-output assertions |
| PB2' | every Claude guard entry also carries `dydo guard` | `Init_WritesEachHostsFiles` [1], [3] at the bare-guard count (expected 0, actual 1) | the strengthened counts; both removed host-marker cases |
| PB3 | `TopLevelCommands` drops the seven rewritten words | `Complete_OffersTheWordsValues[0]`, exactly those seven | the seven rewrites |
| PB8 | `overnight: off ` (trailing space) | the same six scenarios at `MachineModeSteps.cs:72` | the byte-exact step |
| PB9 | the Claude guard written unmarked | `Init_WritesEachHostsFiles` [1], [3] at the marked count | `Init_All_OnlyTheClaudeHookCarriesTheHostMarker`'s Contains |
| PB10 | check also prints a `bad-doc.md` warning | `FreshInit_PassesCheck` [0], [1]; fresh-installation.feature stays green | row [0] stays |
| PB11 | the Codex guard gets `--host claude` | `Init_WritesEachHostsFiles` [2], [3] | the removed DoesNotContain(`--host`) |
| PB12 | the first show over a missing store creates it and prints a line | `Show_MissingOrUnusableStore_ReadsTheDefault[0]`; machine-mode Show stays green | row [0] stays |
| PB13 | the isolated git run keeps inherited `GIT_*` | `KeepMineMergeDriverFromTheEnvironment` [0]-[3], `ScratchGitEnvironment_...` | the dry-run verdict |
| PB14 | `CleanArchive.List` leaves out the newest entry | only `Archive_ListsEntriesNewestFirst`, at the new presence check (`CleanCommandTests.cs:535`); the old order check alone passes it (index -1) | the should's assertions; the `unsure` case's `only` |
| PB15 | `ConfigFactory.DefaultNudges` drops `InstallNudge` | all 54 `Install_MeetsTheNudgeOnce_...` rows at `InstallNudgeTests.cs:54`; the 35 others stay green | the denial is the install nudge's |
| PB5, PB6, PB7 | completion offers `task`; drops `complete`; a `git worktree add` nudge | only the matching `unsure` case | their `only` |

## Kept `unsure`

- `CompletionProviderTests.TopLevelCommands_ContainsCommand[10]` (`complete`): pins a defect, the
  command is `_complete`; `only` PB6. Unresolved: whether completion should offer it.
- `CompletionProviderTests.TopLevelCommands_ExcludesRetiredWorkCommands`: a deleted feature is
  gone, untraced; `only` PB5. Unresolved: a "every candidate is registered" property, red today.
- `ConfigFactoryTests.Guard_DefaultNudges_WarnOnTheCommand[10]` (`digit worktree add foo`): the
  absence of a retired git nudge; `only` PB7. Unresolved: whether a git nudge returns.
- `CleanCommandTests.Archive_ListsEntriesNewestFirst`: newest first is only a code comment
  (`CleanArchive.cs:84`); `only` PB14. Unresolved: whether the order is a contract.
- Round 2, no source states the claim (lesson 11), tags dropped:
  `InitCommandTests.Init_Refused_ExitsTwoAndOverwritesNothing` [0]-[2] (init's refusals and their
  exit 2), `CompletionsCommandTests.Completions_UnknownShell_IsAToolError` (fish exits 2) and
  `Complete_UnknownInput_OffersNothingAndExitsZero` [0]-[1]. Each records its observable; missing
  is a spec line, or a mutant or planted bug only it fails on.
- Six `InitCommandTests` cases pin behaviour that conflicts with dydo-commands-core.md, each a
  product question with its assertions unchanged: `Init_Codex_WritesTheExecPolicyRuleForDydoAndDydoExe`
  [0]-[1] (clean under `dydo` only; nothing of archive), `Join_All_TurnsNativeMemoryOffOverTrueAndSaysSo`
  (`--join` overrides a managed value the spec says stops init), `Join_Codex_LeavesACrlfRuleWithTheSameContentByteForByte`
  (the spec rewrites `dydo.rules` every time) and `Join_Claude_DoesNotWarnAboutAnEntryFileThatImportsAgentsMdFirst`
  [2]-[3] (the spec warns for every `CLAUDE.local.md` and `.claude/CLAUDE.md`).

## Deviations

- Lesson 9: every moved case keeps its base input. The seven completion rows run
  `dydo _complete 1 dydo` (PB3); the host-marker cases' inits are rows [1] and [3]'s own, before a
  byte-identical join; the mode removals match their scenarios' inputs. Round 2 changed no input:
  the archive case gained two assertions, everything else is tags.
- A strengthened assertion in a kept test is recorded as keep, the absorbed assertion named in its
  Reason.
- The clause map is its own page, `setup-tags.md`, so both pages stay under 20 KB.
- `ScaffoldTree.WriteDydoTree(string, IEnumerable<string>)` exists for the removed test; production
  code is out of scope, so it stays for a follow-up.

## Related

- [DYD-1520 test audit](./_dyd-1520-test-audit.md) - The audit files of every batch
- [DYD-1520 test rubric](./rubric.md) - What each verdict here applies
- [DYD-1520 setup tag clauses](./setup-tags.md) - Each tag clause and the line that proves it
