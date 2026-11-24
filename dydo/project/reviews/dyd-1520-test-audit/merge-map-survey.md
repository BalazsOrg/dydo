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
one behaviour and the assertion line that proves it are in
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

## Review round 1 and what the first fix hop changed

Round 1 FAILed the removals and the class tags. The first fix hop restored every removed case and replaced
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
  carries one heading comment, written for that method's cases alone.

## Review round 2 and what this hop changed

Round 2 FAILed tags that claimed more than their assertions check, and cites that stated less than
their tags claimed. This hop rewrites all 137 method tags and two of the three feature tags in the
minimal form the DYD-1520 captain set: `contract: <cite> - <the one rule, in the cite's words>`,
`trap: <the bug or pitfall> (<record>)` or `edge: <the input reached>`. Each names one behaviour
that every case of its method asserts. The diff is still Job lines only. The round-2 findings:

- **The origin parser** now pins only "the repository is the one origin names, as HOST/OWNER/REPO";
  no scheme list remains to leave HTTP out.
- **The three merge tags** claim one asserted outcome each. `ARefusal_…` claims "each refusal exits
  1" (L224), with no check order. `ThePinOnlyRefuses_…` claims "the pin can only refuse" (DR 061's
  amendment; L584 ties writes to the pin being on the tip), with no write sequence.
  `ARealBodyWithTwoAbbreviatedPasses` names its #205 trap and claims no `Pass` flag.
- **The reader tags** cite only what their sources state. Team pagination and a null Project or body
  have no source, so `Teams_…`, `ReadProject_WithoutABody_…` and
  `Graph_OverflowOnAnIssueLinearNoLongerHas_…` are edges naming the input they reach. `not_found`
  (404) is the visual map plan's §3 error code, which DYD-264 serves; DR 063 §5 is no longer cited
  for it. `Graph_CarriesEachIssuesFields` claims the Graph's project and "an Issue's fields" (the
  full comparison of issue 1, L103).
- **The localhost command tag** claims only "it prints its http://localhost URL" (L61); nothing
  claims serving. `AnUnknownPathOrANonGetMethod_…` cites DYD-264's "only answers GET" and its exact
  API, with no "each non-GET method".
- **The survey feature tag** names the command boundary only. Its digest scenario is mapped to the
  ordered `kind KEY` entries of the four kept kinds (SurveySteps L100), not to whole lines.
- **Other cites corrected on the way:** the success, neutral or skipped and legacy-status rules cite
  `dydo-commands-delivery.md`, which states them, not DR 061 §5. The insteadOf rule, the 204, 404 and
  409 merge answers and refusals 16 and 17 cite DYD-417's design comment e6a6affe, which DYD-424 names
  as its governing contract. DYD-523's acceptance and DYD-535's acceptance 3 are cited where they
  state the rule. Bearer-less auth cites the plan's §3 instead of a rejection nobody recorded.
- **Uncited behaviour became edges, not contracts:** an upper-case PASS candidate and base pin,
  protection that is off but lists a context, an unreachable or slow Linear, a canceled survey run,
  and the built dll's `init none` then `check`. No new `unsure` case: an edge names a condition the
  case reaches, which needs no cite.

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
| PB5 | `dydo check <path>` validates from the path instead of the docs root (#0185's bug) | `Check_Subfolder_AcceptsLinksToTargetsOutsideSubfolder` (`Broken link:` found), the one case PB5 turned red. PB5 ran alongside PB1 to PB4 in one combined run over the 364 batch cases at round 1, which failed ten cases in all, this expected one among them (`/var/tmp/dyd1520/mms/run-pb-run1/`) | its trap tag (this case's body equals this hop's) |
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

This page cannot name the commit that holds it, so the gate record is outside the tree. Every final
gate ran on a clean detached checkout of the pushed candidate, behind `/var/tmp/dydo-gate-lock.sh`
under `systemd-run` with `MemoryMax=8G`: `dotnet build DynaDocs.sln -m:2 --warnaserror`; the batch
filter and the full suite with `--logger trx` and `MaxCpuCount=2`; and `dydo check`. Their raw output
is in `/var/tmp/dyd1520/mms-fix2/gates-<candidate SHA>/`, whose `head.txt` records the SHA,
`git rev-parse HEAD^{tree}` and the empty `git status --porcelain` of the tested checkout, and
`rc.txt` each gate's exit, count and wall time. The hop's return repeats them. Round 2's gates, on
`2d1cd5a78`, are in `/var/tmp/dyd1520/mms-fix/gates-2d1cd5a78/`.

## Deviations

- The clause map lives in [merge-map-survey tag clauses](./merge-map-survey-tags.md), as the
  statechart and docs batches did. On this page it would pass the size limit.
- The base's ten skips are the `NodeTestGuardTests` cases (no pwsh on this host); the candidate
  keeps all ten.
- The tags follow the DYD-1520 captain's minimal-tags rule, issued during this hop, rather than the
  round-2 FAIL's own wording ("narrow the tags"). Each finding is closed in the narrower form.

## Related

- [DYD-1520 test audit](./_dyd-1520-test-audit.md) - The audit files of every batch
- [DYD-1520 test rubric](./rubric.md) - What each verdict here applies
- [Merge, map and survey tag clauses](./merge-map-survey-tags.md) - Each tag's clauses and the line checking each
