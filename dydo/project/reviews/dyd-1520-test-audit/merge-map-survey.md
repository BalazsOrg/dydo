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
`dydo check` accepts only `.md` relative links), judged by the [rubric](./rubric.md).

## Totals

| Verdict | Cases |
|---|---|
| keep | 363 |
| keep `unsure` | 1 |
| rewrite | 0 |
| remove | 31 |
| **total** | **395** |

After the batch, 364 cases run. The batch had already been through the DYD-1199 purge and rewrite:
every case drives `dydo merge` through `PullRequestMerge.Run` against a fake GitHub, the map server
over real HTTP, the survey command, the reader against a fake Linear, the committed CI guard under
pwsh, `dotnet pack` on the real packaging configuration, or the built dll; each asserts exit codes,
what was written, or exact output. So no case needed a rewrite, and the removals are rows that add
no input feature the code under test reads.

**The collapse rule this batch applied.** A theory row is removed only when its varying value
reaches the code through one predicate (an equality, a set membership) and is otherwise only echoed
into the message, and the spec names neither the value nor its class, so the kept row of that class
covers it exactly (rubric §5, keep-first by ordinal). Rows stay wherever the value is a JSON kind, a
length, a case, a regex anchor, a separator, a path shape or a status code the spec names: those are
corpora (the origin URLs, ref names, SHA pins, GraphQL answers, inert-app counts, Project references,
the spec's 403/422/ruleset answers, DR 061 §5's five inert statuses, DR 063 §4's "backlog and
triage included", and every "unknown state" row).

In CI the batch runs on Linux only: neither `guard-windows` nor
`DynaDocs.Tests/coverage/dotnet-platforms.json`'s `windows_tests` names these classes. The ten
`NodeTestGuardTests` cases skip on this host (no pwsh) and run on the Linux CI runner; none is
removed.

## What changed

- **Removed as one equivalence class with a kept row (24).** `CommitChecksTests`: four unfinished
  run statuses and four unfinished suite statuses (`in_progress`, `pending`, `requested`, `waiting`;
  `queued` and the unknown `mystery` stay), five non-green conclusions (`failure` and the unknown
  `exploded` stay), two legacy states (`pending` and `mystery` stay), and the other app's `requested`
  (`queued` and `mystery` stay). `PullRequestMergeTests.ARefusal_ExitsOne_AndWritesNothing`: rubric
  `project-plan` (`spec` stays) and compare status `behind` (`diverged` stays);
  `ThePinOnlyRefuses_...` rows 6 and 7 (a `Moved` pin is a `Stale` pin to `PinnedBranch`).
  `SurveyRulesTests.Unblocked_...` rows `unstarted` and `started` (`triage` and `backlog`, which
  DR 063 §4 names, stay).
- **Removed as redundant with a stricter kept case (7).** `AfterALanding_...` rows 2, 4 and 6 (the
  rereads of rows 1, 3 and 5 on the two-parent path, whose landing
  `BaseTipAncestorOfTheSha_LandsATwoParentCommit_OnThePin` asserts); `IndependentReviewTests`
  `CanonicalAndColonFields_ParseIdentically` rows 0 (the unchanged line, `AnUnfencedLine_Counts`) and
  3 (both colons, rows 1 and 2 each), `ColonFields_MergeWithTheSamePinsAndBasePolicy`
  (review-pass-grammar's colon rows plus the landing) and `ACodeDocsOrMergePass_Merges` row 0
  (`AReviewedGreenPr_LandsOnceAsAMergeIntoItsFeatureBase` row 0, the same input);
  `SurveyCommandTests.MissingKey_PrintsMapsKeyHelp_BeforeAnyRequest` row 0 (survey.feature's no-key
  scenario); `ViewerPackTests.Pack_WithoutViewer_FailsWithBuildInstructions` row 1 (an empty
  `viewer/dist` is the same input as none, about 3 s of the batch's 23 s).
- **An assertion moved into a kept case.** survey.feature's step `the error is dydo map's
  missing-key help` now also asserts an empty stdout, the one assertion the removed
  `MissingKey_...[0]` held beyond it (red proof PB3).
- **Shapes.** `AfterALanding_...` loses its `twoParent` parameter. Two theories left with one row
  became Facts under the same name: `SurveyCommandTests.MissingKey_PrintsMapsKeyHelp_BeforeAnyRequest`
  (the whitespace key), `ViewerPackTests.Pack_WithoutViewer_FailsWithBuildInstructions` (no
  `viewer/dist`). No helper or fixture was orphaned (`MergeScenario.Moved`, `FakeGitHub`,
  `FakeLinear`, `FakeLinearServer` and every step class keep callers). No CI or tooling line names a
  removed test; no class was deleted or renamed.
- **Tags (§2 row 2, swept over every kept case).** Six class tags claimed less than their cases pin
  and now cite the command reference that states it: `PullRequestMergeTests`, `CommitChecksTests`,
  `IndependentReviewTests`, `MergeCheckoutTests` (`dydo-commands-delivery.md` dydo merge),
  `LinearReaderTests`, `MapServerTests` and `SurveyCommandTests` (`dydo-commands-linear.md`).
  `MapServerTests.Start_BindsLocalhostOnAFreePort` claimed the server "must bind localhost only"
  while it checks the URL it returns; it now says that. `NodeTestGuardTests` names what the guard
  proves. `CliEndToEndTests`' class tag claimed `dydo-commands-core.md` exit codes for every case;
  it keeps only the subprocess edge, and seven cases gained the method tag their assertions pin,
  including the #0185-#0188 trap on `Check_Subfolder_...` (red proof PB5).

## Mutation

Targeted two-phase Stryker.NET (`/var/tmp/dyd1520/tools/stryker-targeted.sh`). Phase 1 ran on
clean detached checkouts of the base `07642b1f3` over every method that loses a case (114 executed
cases, the kept rows of those methods included), in three chunks:

| Chunk | Methods | Cases | Killed | Timeout | Wall |
|---|---|---|---|---|---|
| A | `ARefusal_...`, `ThePinOnlyRefuses_...`, `AfterALanding_...` | 74 | 243 | 0 | 707 s |
| B | the five `CommitChecksTests` theories, `CanonicalAndColonFields_...`, `ColonFields_...`, `ACodeDocsOrMergePass_Merges` | 34 | 190 | 0 | 572 s |
| C | `Unblocked_...`, `MissingKey_PrintsMapsKeyHelp_...` | 6 | 15 | 3 | 428 s |

GeneratedRegex patterns and MSBuild targets are not mutated, so PB1, PB2 and PB4 carry those
protections. Chunk C's three Timeouts (#3001, #3003, #3004, the survey's missing-key test) are not
kills, so phase 2 does not recheck them; PB6 shows the kept no-key scenario fails on #3001.

Phase 2 runs on the removals commit, over every kept case of the batch; this page records its
comparison when it has run.

## Red proofs and planted bugs

No case is rewritten, so the red proofs are the moved survey assertion (PB3) and the new trap tag on
`Check_Subfolder_...` (PB5). PB1-PB5 ran together over the candidate's 364 batch cases in a scratch
checkout, base production code plus the bug: 10 red, each attributed by its assertion. PB6 and PB7
each ran alone. Afterwards `git checkout -- Services Commands ViewerPackaging.targets` restored
production code and `git status` showed only the candidate's test files.

| Bug | Production edit | Kept cases red | What it proves |
|---|---|---|---|
| PB1 | `CandidateRegex` without the optional colon (`candidate:?` to `candidate`) | `CanonicalAndColonFields_ParseIdentically` candidate-colon row (candidate null); review-pass-grammar `Canonical and colon fields...` rows 1 and 3 (exit 1, expected 0) | regex patterns are not mutated; covers the removed colon/colon row's candidate half and `ColonFields_MergeWithTheSamePinsAndBasePolicy` |
| PB2 | `BaseRegex` without the optional colon | `CanonicalAndColonFields_ParseIdentically` base-colon row (base null) | covers the removed colon/colon row's base half |
| PB3 | the survey's missing-key path also writes `PB3` to stdout | survey.feature `Without LINEAR_API_KEY...` at the moved `Assert.Empty(stdout)`; `SurveyCommandTests.MissingKey_PrintsMapsKeyHelp_BeforeAnyRequest` | red proof of the moved assertion; covers the removed null-key row |
| PB4 | `ValidateViewerForPack` disabled (`Condition="false"`) | `Pack_WithoutViewer_FailsWithBuildInstructions` (exit 0, expected not 0), both `Pack_NoBuild_...` cases | the kept no-dist case fails when a viewerless package is written, as the removed empty-dist row did; MSBuild has no mutation tool |
| PB5 | `dydo check <path>` validates from the path instead of the docs root (#0185's bug) | only `Check_Subfolder_AcceptsLinksToTargetsOutsideSubfolder` (`Broken link:` found) | red proof of its new trap tag |
| PB6 | `dydo survey`'s key test replaced by `apiKey != null`, base mutant #3001's replacement (a Timeout at phase 1, not a kill) | survey.feature `Without LINEAR_API_KEY...` (exit 0, expected 2), and 15 keyed survey cases that now print the help | the kept no-key scenario covers the null-key row's mutant that mutation only timed out |
| PB7 | `dydo check` outside any project exits 2 | over the full suite (4,599 cases), only `Check_WithoutInit_ShowsError` | the `unsure` case's `only` |

## Kept `unsure`

- `CliEndToEndTests.Check_WithoutInit_ShowsError`. Protection: `dydo check .` outside any project
  does not exit 2 and prints something. The assertion is loose (a crash exiting 134 passes it) and
  untraced: `dydo-commands-core.md` does not say what `dydo check` does outside a project. Today it
  prints `No docs folder found.` and `All checks passed.` and exits 0, while the same page makes a
  `--root` in no dydo project a tool error (exit 2). Unresolved: which outcome is right is a product
  decision; then this case is rewritten to that exact outcome. PB7 shows nothing else in the suite
  guards it.

## Deviations

- An assertion moved into a kept case (the survey no-key step) is recorded as keep with the
  absorbed assertion named in its Reason, not as a rewrite, as the setup batch did: its job and
  boundary are unchanged, and its red proof is PB3.
- Phase 1 ran in three chunks by class, each from its own clean detached base checkout, so the
  runs never shared a build output: A (`PullRequestMergeTests`), B (`CommitChecksTests`,
  `IndependentReviewTests`), C (`SurveyRulesTests`, `SurveyCommandTests`). `ViewerPackTests` is
  in none: it drives `dotnet pack`, which mutation cannot see, so PB4 carries its evidence.
- The base's ten skips are the `NodeTestGuardTests` cases (no pwsh on this host); the candidate
  keeps all ten.

## Related

- [DYD-1520 test audit](./_dyd-1520-test-audit.md) - The audit files of every batch
- [DYD-1520 test rubric](./rubric.md) - What each verdict here applies
