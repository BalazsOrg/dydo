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
- **Tags (§2 row 2, every kept case).** Seven class tags now name, clause by clause, what their
  cases pin, citing `dydo-commands-delivery.md` (the four merge classes) or
  `dydo-commands-linear.md` (`LinearReaderTests`, `MapServerTests`, `SurveyCommandTests`).
  `CliEndToEndTests`' class tag keeps only the subprocess edge; seven of its cases gained method
  tags, including the #0185-#0188 trap on `Check_Subfolder_...` (red proof PB5). Clauses no line
  proves were dropped: "must bind localhost only" (`Start_Binds...` checks the URL); DR 052 §1 and
  "every page" of a team's Projects (`LinearReaderTests`); "never the key"
  (`UnreachableOrSlowLinear_...`); "before any request" (`MapCommandTests`); "the hosted Windows
  leg relies on" (`NodeTestGuardTests`); "must not print a digest" (`ACanceledRun_...` asserts the
  throw); "the agent settings" (`Init_All_...` reads one key each). Clauses cases proved but no tag
  named were added: the rubric, no checks reported, the dry run, missing parameters, error envelopes.

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

Phase 2 ran on clean detached checkouts of the removals commit `9f2b34276` (production code
identical), once per chunk, each mutating that chunk's killed spans and running all 364 kept cases:

| Chunk | Killed before | Killed after | Not killed after | Wall |
|---|---|---|---|---|
| A | 243 | 285 | 0 | 154 s |
| B | 190 | 234 | 0 | 152 s |
| C | 15 | 19 | 0 | 103 s |

`compare exit=0` for each: every mutant phase 1 killed is Killed on the candidate, and no mutant
is a Timeout only. Stryker's `killedBy` lists are partial (a mutant's run stops early), so each
mutant the `.tsv` names against a kept case was checked one of two ways: the kept case is in
`killedBy`, or the mutant was planted by hand in the scratch checkout and only the named kept
cases were run (red, then restored):

| Mutant | Kept case red | How |
|---|---|---|
| #8854 rubric `First` | `ARefusal_...`[30] (`spec`) | phase 2 `killedBy` |
| #8893 `Contains` negated | `ARefusal_...`[32] (`diverged`) | planted (as `is not "ahead"`, the same predicate; the literal form does not compile under warnings as errors) |
| #8881 `tip != pin` | `ThePinOnly...`[3], [4] | planted |
| #8763 two-parent path never taken | `BaseTipAncestorOfTheSha_...` | planted |
| #9019 reread path emptied | `AfterALanding_...` rows 1, 3, 5 | phase 1 `killedBy` at base, where those rows are byte-identical; #9017 (`attempt--`) also turns row 3 red, planted |
| #8907 POST merges path emptied | `AReviewedGreenPr_...`[0] | phase 2 `killedBy` |
| #8730 `"code"` emptied | `AReviewedGreenPr_...`[0], [1] | planted |
| #8536 `status == "completed"` | `ARunThatHasNotCompleted_...`[0], `ASuiteWithRuns...`[0] | planted |
| #8490 suite named by slug only | `ASuiteWithRuns...`[0] | phase 2 `killedBy` |
| #8489 suite named by app name | `AnotherApps...`[0] | planted |
| #8544 green test `\|\|` | `AnyOtherConclusion_...`[0] | phase 2 `killedBy` |
| #8496 `state == "success"` | `ALegacyStatus...`[0] | phase 2 `killedBy` |
| #8675 every field null | `AnUnfencedLine_Counts` | planted (as `field.Success && false ?`, the same value) |
| #16447 `IsOpen` as `IsClosed` | `Unblocked_...`[0] (`triage`) | phase 2 `killedBy` |
| #3006 key help not written | survey.feature `Without LINEAR_API_KEY...` | phase 2 `killedBy` |

| Run | Cases | Summed test duration | Command |
|---|---|---|---|
| base `07642b1f3` | 395 (385 passed, 10 skipped) | 23.52 s | batch filter, `--logger trx`, `MaxCpuCount=2` |
| candidate `9f2b34276` | 364 (354 passed, 10 skipped) | 24.61 s | the same |

The removed cases were cheap but for the empty-dist pack row (about 3 s), and the shared host's load
moves the sum by more than that, so the summed time does not drop measurably.

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

## Tag clauses

Every tag in the batch, split into its clauses, each with the assertion lines that fail when it is
false (`file:line` in `DynaDocs.Tests/` at the final commit). A class tag's clause holds for the
cases listed against it; every case sits under at least one clause, or under its own method tag.
Clauses no line proved were dropped in this batch (listed under What changed).

- **Merge/PullRequestMergeTests** (class). Refusal exits 1: `ARefusal_...` :222-223,
  `GitHubRefusingThePost_...` :256, `ATargetThatMoves...` :409, `ABaseGone...` :420,
  `ADryRun_...`[1] :381, `ThePinOnly...` off-tip rows :552. Nothing written before the merge call:
  :224, :283, :328, :382, :483, :553. Tool error exits 2: :271, :282, :302, :316, :327, :343, :515.
  Exits 0 and lands once (merge commit or two-parent on the pin): :357-359, :369-370, :392-395,
  :433 (exit 0 only), :463-464, :532-533, :552-554. Dry run writes nothing: :382, :553. Already
  integrated exits 0: :482, :493, :516-517. DYD-523 (pin): :303, :318, :410, :552-554. DYD-535:
  :463-464.
- **Merge/CommitChecksTests** (class). Only finished green pass: :31, :63, :130, :152, :186, :233;
  every problem named by slug or name and state: :41, :51, :73, :81, :89, :111, :203, :213, :222,
  :233. Required check present: :122, :143 (absent when off or plan-less: :130, :152). Every page:
  :101. Inert-app exception: :186, :195, :203, :233. No suite left: :172, :195. Unreadable list a
  tool error: :162-163, :243-244.
- **Merge/IndependentReviewTests** (class). Which lines count: :36, :48, :61, :109, :130, :136.
  Candidate and base fields: :26-28, :38-40, :68, :76-77, :85-86, :92, :101, :148. Abbreviation bound
  to the SHA: :162-163, :175, :187-189, :201-202. Rubric code, docs or merge: :215, :226-227.
- **Merge/FeatureBranchTests** (class). Pattern and git's rule: :24, :44 (rows `feature/x.lock`,
  `feature/a..b` pass the pattern and fail only git's rule).
- **Merge/MergeCheckoutTests** (class). Origin names the repository: :32, :44, :65. Checkout
  rooted at dydo.json: :87-89, :100. Local origin/HEAD: :66, :74. SSH alias a tool error: :54,
  :110-112. GH_REPO, GH_HOST, GIT_DIR ignored: :132-137.
- **Merge/MergeCommandTests** (class). DR 061 §1 one path with both pins: :30-32, :78-79. §2 and
  DYD-523, the base pin only refuses, before any call: :47-49, :62-63.
- **Map/LinearGraphQLTests**. `TheKeyIsSentBare` trap: :22. `ALinearFailure_...` named, never the
  key: :47, :48. `UnreachableOrSlowLinear_...` named failure: :63.
- **Map/LinearReaderTests** (class). Every page of teams and of issues and relations: :23 (via
  `AssertSet` in `Teams_...`, `Graph_ReadsEveryIssuePage`, `Graph_RelationOverflow_...`). Projects
  with status and dates: :51-54. Each issue's fields: :97-105. Archived dropped with relations:
  :163-165. Only blocks and related: :121-124; far ends external: :139-143; resolved blocker kept:
  :177. Priorities, parents, body: :232-235, :248. Not found: :65-66, :218, :250. Method tag
  (overflow edge): `Graph_RelationOverflow_...` through :23.
- **Map/MapCommandTests** (class). §2 missing key exits 2, nothing on stdout, no browser: :21-24,
  :37-38. §3 localhost URL, key never printed: :58-60, :62.
- **Map/MapServerTests** (class). GET only: :168. /api/graph afresh: :119-120. /api/saved null
  without Linear: :86-88. Missing parameter 400 without Linear: :131-132. Failure status and code:
  :154. Key never in the browser: :106, :155. DR 052 §1 (Linear read only when asked): :88, :132,
  :169. Method tags: `Start_Binds...` :52-53; `Start_Retries...` :69.
- **Survey/SurveyCommandTests** (class). §1 key help, exit 2, before any request: :47-50. §2 the
  shared reader: :78, :93-96. §4 one line per finding: :64. §5 exit codes: :35-38, :62, :93,
  :109-111. UTC header: :63. Method tag (`ACanceledRun_...`): :118.
- **Survey/SurveyRulesTests** (class). DR 063 §4: :56, :65, :75, :86-87, :99-102, :113, :122, :129.
- **Workflow/NodeTestGuardTests** (class). Passes a full report: :24-25. Fails and names why: :33-34,
  :44-45, :60-61.
- **Workflow/ViewerPackTests** (class). No package without the viewer: :43-46, :57-60, :75-77. The
  package embeds it: :99-100.
- **EndToEnd/CliEndToEndTests**. Class (the process exit code is real): :58, :75, :91, :104, :116,
  :120, :129, :136, :159, :173, :189, :204. Method tags: `Init_CreatesProject` :93, :95;
  `Init_RecordsThe...` :108; `Init_ThenCheck_Passes` :120; `Init_All_...` :130, :133, :137;
  `Check_Subfolder_...` :161 (PB5); `Check_NonExistentPath_...` :173-174;
  `Check_PathOutsideDocsTree_...` :189-190.
- **Features/review-pass-grammar.feature**. One grammar for pr-ready and merge:
  `Merge/ReviewPassGrammarSteps.cs` :45-46, :52-53.
- **Features/map-memory.feature**. Disposable saved maps: `Map/MapMemorySteps.cs` :51, :65-68, :72,
  :82, :90, :101, :131-133, :153-155.
- **Features/survey.feature**. §1 read-only and key help: `Survey/SurveySteps.cs` :114-115, :122,
  :130-135. §4 findings: :100-101. §5 exit codes and messages: :95, :107-108, :124.

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
- Inputs of changed cases (review lesson 9). No kept case had its input changed: the kept
  `AfterALanding_...` rows lost only the `twoParent` parameter their base rows set false, the
  whitespace-key and no-dist Facts are their base rows' inputs as literals, and the survey step
  gained one assertion. Each removed case whose input differs from its covering case carries a
  planted bug or planted mutant keyed to that difference (PB1, PB2, PB4, #8763, #8907, #9019).
- The base's ten skips are the `NodeTestGuardTests` cases (no pwsh on this host); the candidate
  keeps all ten.

## Related

- [DYD-1520 test audit](./_dyd-1520-test-audit.md) - The audit files of every batch
- [DYD-1520 test rubric](./rubric.md) - What each verdict here applies
