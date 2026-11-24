---
area: project
type: reference
---

# DYD-1520 Merge, Map and Survey Audit

The merge-map-survey batch of the [DYD-1520](https://linear.app/bodnar-balazs/issue/DYD-1520) test
audit: `dydo merge` (`Merge.*` and `review-pass-grammar.feature`), `dydo map` (`Map.*` and
`map-memory.feature`), `dydo survey` (`Survey.*` and `survey.feature`), the CI workflow and viewer
packaging (`Workflow.*`) and the built-CLI tests (`EndToEnd.CliEndToEndTests`), 395 executed cases at
`07642b1f3`. Each case's verdict is one line of `merge-map-survey.tsv` beside this page
([on the feature branch](https://github.com/BalazsOrg/dydo/blob/feature/dydo-4-0-0/dydo/project/reviews/dyd-1520-test-audit/merge-map-survey.tsv);
`dydo check` accepts only `.md` relative links), judged by the [rubric](./rubric.md). Each tag's
clauses and the assertion lines that check them are in
[merge-map-survey tag clauses](./merge-map-survey-tags.md).

## Totals

| Verdict | Cases |
|---|---|
| keep | 394 |
| keep `unsure` | 1 |
| rewrite | 0 |
| remove | 0 |
| **total** | **395** |

All 395 cases still run. The batch had already been through the DYD-1199 purge: every case drives
`dydo merge` through `PullRequestMerge.Run` against a fake GitHub, the map server over real HTTP,
the survey command, the reader against a fake Linear, the committed CI guard under pwsh,
`dotnet pack` on the real packaging configuration, or the built dll, and asserts exit codes, what
was written, or exact output. No case needed a rewrite. Its work is the job tags.

## Review round 1 and what this hop changed

Round 1 FAILed the removals and the class tags. This hop restores every removed case and replaces
every class tag with method tags:

- **Every removal is restored, byte-identical to base (31 cases).** The 16 `CommitChecksTests` rows
  (four unfinished run and four unfinished suite statuses, five conclusions, two legacy states and the
  other app's `requested`) each assert a refusal that echoes their own value. `SurveyRulesTests.Unblocked_…`
  rows `unstarted` and `started` are back, as are `AfterALanding_…` rows 2, 4 and 6 (the
  `twoParent` parameter is back) and `ColonFields_MergeWithTheSamePinsAndBasePolicy`. The re-check of
  the remaining removals restored the rest. Some inputs exist in no kept case: rubric `project-plan`,
  compare status `behind`, both `Moved` pin rows, the colon/colon parse row, the null-key survey row and
  the empty `viewer/dist` row. Two removed rows had inputs identical to a kept case, but the kept case
  was not shown to kill their own phase-1 mutants (124 for `ACodeDocsOrMergePass_Merges[0]`, 5 for
  `CanonicalAndColonFields_ParseIdentically[0]`). Both are restored too. The survey step that took over
  the null-key row's stdout assertion is back to base, since the row itself is back.
- **Subtotals (round-1 nit).** No removal groups remain. The round-1 groups held 22 same-class
  cases and 9 other redundancies, all restored.
- **Tags.** No class carries a Job tag now. Each test method carries one, and each feature file
  carries one heading comment, written for that method's cases alone. A tag's parenthesis claims only what the method's
  assertions check, and its cite (DR 061, DR 052, DR 063, `dydo-commands-delivery.md`,
  `dydo-commands-linear.md`, `dydo-commands-core.md`, DYD-264's Outcome, DYD-86, the 3.1.1
  changelog, or the trap's own bug) states each clause. Claims a cite does not state were dropped,
  and their assertions stay unclaimed. Examples: the exact text of a run, status or required-check
  refusal, where only a suite refusal's slug and status are specified; a key never printed on stderr;
  every list's pagination. The per-case Reason and Evidence cells were regenerated from the source tags,
  so each Evidence cell is that case's own tag.

## Mutation method (coordinator ruling 1)

> **1. Option A.** The targeted two-phase run complies with §5 when both conditions hold:
> - phase 1 covers every method that was removed or whose body changed;
> - the diff changes no shared helper or fixture that an untouched test uses.
>
> Each batch PR states both conditions, with the diff evidence for the second. If either condition
> fails, the batch re-runs phase 2 over every test that uses the changed helper, including untouched
> ones. Quote this ruling in each PR body so the reviewer judges the conditions, not the method.

**Phase 1's methods.** Phase 1 ran at base `07642b1f3` for round 1's removals, in three chunks:
A, `PullRequestMergeTests.ARefusal_ExitsOne_AndWritesNothing`, `ThePinOnlyRefuses_…` and
`AfterALanding_…` (243 killed). B, the five `CommitChecksTests` theories with removed rows,
`IndependentReviewTests.CanonicalAndColonFields_ParseIdentically`,
`ColonFields_MergeWithTheSamePinsAndBasePolicy` and `ACodeDocsOrMergePass_Merges` (190 killed).
C, `SurveyRulesTests.Unblocked_…` and `SurveyCommandTests.MissingKey_PrintsMapsKeyHelp_BeforeAnyRequest`
(15 killed).

**Condition 1 holds: no method is removed and no body changed.**
`git diff 07642b1f3 <candidate> -- DynaDocs.Tests` touches 17 files: the 15 test classes of the
batch and two feature files. Every changed line is a `[Trait("Job", …)]` attribute or a
`# Job:` comment. These commands print nothing:

```bash
git diff -U0 07642b1f3 <candidate> -- DynaDocs.Tests | grep -E '^[+-]' | grep -vE '^(\+\+\+|---) ' \
  | grep -vE '^[+-]\s*\[Trait\("Job", ' | grep -vE '^[+-]# Job: '
```

An attribute line is not a method body, and a Gherkin comment is not a step. So the set of removed
or body-changed methods is empty, and phase 1 covers it trivially. With no assertion or body changed,
phase 2 is not rerun. Every phase-1 kill comes from a case that is still present, byte-identical.

**Condition 2 holds: no helper, fixture, step definition or data file changed.** The changed files
under `DynaDocs.Tests/` that are not test methods: none. `MergeScenario`, `FakeGitHub`,
`TargetBranch`, `FakeLinear`, `FakeLinearServer`, `LinearJson`, `KeptFindings`, the step classes
`ReviewPassGrammarSteps`, `MapMemorySteps` and `SurveySteps`, and `TestData/` are identical to base.
The two feature files differ only in their `# Job:` comment line, which no step reads. So no
untouched test uses a changed helper, and no phase-2 rerun is owed.

**Ruling 2 (Windows-only cases).** None in this batch. No case has a platform condition. Neither
`guard-windows` in `.github/workflows/ci.yml` nor `DynaDocs.Tests/coverage/dotnet-platforms.json`'s
`windows_tests` names these classes. The ten `NodeTestGuardTests` cases need pwsh. They skip on this
host and run on the Linux CI runner, where a missing pwsh fails them.

## Red proofs and planted bugs

No case is removed or rewritten, so no removal needs a planted bug. Round 1's PB1 to PB4 and PB6
supported removals this hop restored, and they carry no verdict now. Two planted bugs still back
records. Each ran in a scratch checkout with base production code plus the bug, then was restored:

| Bug | Production edit | Kept cases red | What it proves |
|---|---|---|---|
| PB5 | `dydo check <path>` validates from the path instead of the docs root (#0185's bug) | only `Check_Subfolder_AcceptsLinksToTargetsOutsideSubfolder` (`Broken link:` found) | its trap tag (run at round 1, where this case's body equals this hop's) |
| PB7 | `dydo check` with no docs folder returns a tool error (`CheckCommand.cs`, the `No docs folder found.` branch returns `DocsOutcome(true, …)`) | over the full suite at `e3ac39919` (4,630 cases, the same test bodies as the final commit), only `Check_WithoutInit_ShowsError` (1 failed, 4,585 passed, 44 skipped) | the `unsure` case's `only` |

## Kept `unsure`

- `CliEndToEndTests.Check_WithoutInit_ShowsError`. Protection: `dydo check .` outside any project
  does not exit 2 and prints something. The assertion is loose (a crash exiting 134 passes it) and
  untraced: `dydo-commands-core.md` does not say what `dydo check` does outside a project. Today it
  prints `No docs folder found.` and `All checks passed.` and exits 0, while the same page makes a
  `--root` in no dydo project a tool error (exit 2). Unresolved: which outcome is right is a product
  decision; then this case is rewritten to that exact outcome. PB7 shows nothing else in the suite
  guards it. The case carries no tag.

## Gates

Each ran behind `/var/tmp/dydo-gate-lock.sh` under `systemd-run` with `MemoryMax=8G`, on
`2d1cd5a78`, whose code and tests the page-only commit after it leaves unchanged. Output is in
`/var/tmp/dyd1520/mms-fix/gates-2d1cd5a78/`.

| Gate | Result |
|---|---|
| `dotnet build DynaDocs.sln -m:2 --warnaserror` | exit 0, 0 warnings, 0 errors |
| batch filter, `--logger trx`, `MaxCpuCount=2` | exit 0: 395 cases, 385 passed, 10 skipped; summed duration 22.51 s (base 23.52 s, the same 395 cases) |
| full suite, `--logger trx`, `MaxCpuCount=2` | exit 0: 4,630 cases, 4,586 passed, 44 skipped (the base's skips), 0 failed; 3 m 33 s, 217 s wall with the lock wait |
| `dydo check` | exit 0, 0 errors, 0 warnings in 943 files |

## Deviations

- The clause map lives in [merge-map-survey tag clauses](./merge-map-survey-tags.md), as the
  statechart and docs batches did. On this page it would pass the size limit.
- The base's ten skips are the `NodeTestGuardTests` cases (no pwsh on this host); the candidate
  keeps all ten.

## Related

- [DYD-1520 test audit](./_dyd-1520-test-audit.md) - The audit files of every batch
- [DYD-1520 test rubric](./rubric.md) - What each verdict here applies
- [Merge, map and survey tag clauses](./merge-map-survey-tags.md) - Each tag's clauses and the line checking each
