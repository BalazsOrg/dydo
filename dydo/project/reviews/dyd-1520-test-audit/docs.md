---
area: project
type: reference
---

# DYD-1520 Audit: Docs Batch

The docs batch of the [DYD-1520](https://linear.app/bodnar-balazs/issue/DYD-1520) audit: `dydo check`,
`fix`, `index`, `graph`, `help` and `validate`, the link and off-limits rules, `explains:`, changelog
evidence, path collapsing, JSON text, the optional-summaries feature, and the Python and Node tooling
tests under `DynaDocs.Tests/coverage/tests/`. Each case is sorted against the
[rubric](./rubric.md) at `937a46014`. The record is the table `docs.tsv` beside this page, one §6 line
per case, audited at base `07642b1f3` (`dydo check` accepts no link to a `.tsv`).

## Totals

| Cases | Keep | Keep `unsure` | Rewrite | Remove |
|---|---|---|---|---|
| 299 C# and Gherkin | 233 | 9 | 29 | 28 |
| 15 Python | 15 | 0 | 0 | 0 |
| 5 Node | 5 | 0 | 0 | 0 |
| **319** | **253** | **9** | **29** | **28** |

Most of this batch already drives `dydo` at the command line and carries true tags, so most of it is
kept. What goes is the rule-level and resolver-level units on hand-built `DocFile`s
(`BrokenLinksRuleTests`, `RelativeLinksRuleTests`, `LinkResolverTests`), which fail §2 row 3, plus
exact duplicates of the optional-summaries feature and of other kept tests.

- **Removed (28):** 11 `BrokenLinksRuleTests` and 4 `RelativeLinksRuleTests` cases that kept
  `dydo check` tests already cover; `CheckDocValidatorTests` rows 1, 27, 28, 29 and 30 and
  `FixFileHandlerTests.Fix_ValidDocument_KeepsItsBytes` rows 0 and 1, which the feature asserts at
  least as strictly; `Fix_PreservesHandwrittenNavigationFiles` and `Graph_UnknownFile_IsAToolError`,
  duplicates of kept tests; the two `.obsidian` tests, which can never fail; and
  `CollapseRelativeSegments_ResolvesDotDot` rows 7 and 8, which `BothReadings` rows 30 and 29 repeat
  with both readings.
- **Rewritten (29):** the link-rule cases no kept test covers become `dydo check` cases in
  `CheckDocValidatorTests`: new rows of `Check_ReportsTheDocumentsFinding` (external link, cross-document
  missing anchor, two broken links, legacy generated marker, quoted marker, absolute path, missing
  `.md`), a new `Check_Wikilink_IsReportedAsAWikilinkNotABrokenLink`, and a new
  `Check_LinkedAsset_IsValidOnlyWhenItExists` with one row per asset extension plus a missing asset.
  The nine `LinkResolverTests` cases keep their names and rows but drive `dydo check` and `dydo graph`,
  the two commands whose agreement is trap #0187.
- **Tags:** `PathUtilsTests`'s class tag moves to its two `CollapseRelativeSegments` methods, since
  it is untrue of `GetMainProjectRoot`. The `setup-skills.test.cjs` tags cite README "Install the
  skills", its real heading (`README.md:164`); they cited "Adopting the skills".

## Mutation

Targeted two-phase Stryker 5.1.0 (`/var/tmp/dyd1520/tools/stryker-targeted.sh`).

- **Phase 1**, base `07642b1f3`, the 96 cases of every method that loses a case or is rewritten
  (`BrokenLinksRuleTests`, `RelativeLinksRuleTests`, `LinkResolverTests`, and the methods
  `Check_ReportsTheDocumentsFinding`, `Fix_ValidDocument_KeepsItsBytes`,
  `Fix_PreservesHandwrittenNavigationFiles`, both `.obsidian` tests, `Graph_UnknownFile_IsAToolError`,
  `CollapseRelativeSegments_ResolvesDotDot`): **504 killed** (3 timeouts not counted), 781 s.
- **Phase 2**, candidate `797d3ec88`, the 269 kept cases of the batch filter, over phase 1's 130
  killed spans: 540 killed; **501 of the 504 still killed**, comparison exit 1 on three mutants:

| Mutant | After | Justification |
|---|---|---|
| `Rules/BrokenLinksRule.cs:12:43` String → `""` (`Description`) | NoCoverage | unreachable: no command prints a rule's `Description` (no production read of `.Description` in Commands, Services, Rules or Utils) |
| `Rules/TitleRule.cs:14:43` String → `""` (the title message) | NoCoverage | covered: the feature's Examples row 4 asserts the exact message and fails on P2; it runs `dydo` as a child process, which Stryker's coverage does not see |
| `Rules/TitleRule.cs:14:13` Statement → `;` (the title violation) | NoCoverage | covered: the same row fails on P8, which drops the violation, and is the only batch test that does |

The Python and Node tooling tests: mutation unavailable. None is removed, so no protection needs a
planted bug there.

## Counts

| | Cases | Summed duration |
|---|---|---|
| Before, base `07642b1f3` | 299 passed | 4.8 s |
| After, candidate `797d3ec88` | 269 passed | 5.2 s |

The batch has 30 fewer cases but runs 0.4 s longer: the rewrites trade rule-level units on hand-built
`DocFile`s for whole `dydo check` and `dydo graph` runs.

## Keep `unsure`

- `DocumentationTests.Graph_AnyDegree_GetsAnAnswer` rows 0 to 2 (`--degree` 0, -1, 100).
  Protection: `dydo graph <file> --degree N` exits 0. Unresolved: `dydo-commands-core.md` shows only
  `--degree 2`, so whether an out-of-range degree should answer or be a tool error is undecided, and
  no kept test covers another N.
- `PathUtilsTests.GetMainProjectRoot_*`, all six rows. Protection: `OffLimitsService` maps a cwd
  under `dydo/_system/.local/worktrees/` to the main checkout, so off-limits patterns still match from
  a legacy dydo worktree. Unresolved: 4.0.0 creates no worktree there (`WorktreeCleaner` uses
  `~/.t3/worktrees`), yet the guard still reads it, and no record keeps or drops that layout. The
  rows carry no tag.

## Planted Bugs

Each is a production edit the dropped assertion fails on, planted in a scratch checkout of
`797d3ec88`, built, run over the batch filter and reverted (`plant.py` sets A to F; the tree was clean
after). Every named kept test was red.

| Bug | Set | Kept test red on it |
|---|---|---|
| P1: no `Missing frontmatter` violation | C | feature: A document without frontmatter is left for a manual repair |
| P2: the title message becomes `Missing heading` | A | feature: Checking needs a title, row 4 |
| P3: a title must be followed by a paragraph (DYD-98's bug) | D | feature: Checking needs a title, rows 1 and 2, each naming `document.md` |
| P4: `Broken link:` becomes `Dead link:` | C | feature: Checking needs a title, row 5 |
| P5: fix appends a summary to every document | D | feature: Fixing leaves a document's bytes alone, rows 0 and 1; `Fix_OnAnInitialisedProject_WritesNoFileAndChangesNoByte` |
| P6: fix appends a newline to `_*.md` pages | E | `DocumentationTests.Fix_OnAnInitialisedProject_WritesNoFileAndChangesNoByte` |
| P7: an unknown graph document exits 0 | E | `DocGraphTests.Graph_UnknownDocument_IsAToolError` |
| P8: the missing-title violation is dropped | F | feature: Checking needs a title, row 4, the only red case |

## Red Proofs

Each rewrite went red on a planted break of the behaviour it guards, in the same runs:

| Break | Set | Rewritten case red |
|---|---|---|
| external links are link-checked | A | `Check_ReportsTheDocumentsFinding` external-link row |
| wikilinks are link-checked | A | `Check_Wikilink_IsReportedAsAWikilinkNotABrokenLink` |
| absolute paths are not reported | A | the `/guides/doc.md` row |
| no extension counts as an asset | A | all nine asset rows of `Check_LinkedAsset_IsValidOnlyWhenItExists` (svg in image syntax) |
| a non-markdown target is never checked on disk | B | the missing-asset row |
| only the first broken link is reported | B | the two-broken-links row |
| a link without `.md` is not reported | B | the `./guide` row |
| no document counts as generated | B | the legacy-marker row |
| prose quoting the marker marks the file generated | C | the quoted-marker row |
| a cross-document anchor is never checked | C | the `../index.md#nowhere` row |
| graph resolves no relative link, and an anchor-only link to the hub | E | all nine `LinkResolverTests` cases |

## Deviations

- `viewer/dist` was copied from `/var/tmp/dyd1520/mut-setup`, a checkout whose `viewer/` tree is the
  same object (`36d5f73`), instead of rebuilt; the gate lock's three slots were held by other batches'
  Stryker runs.
- `.static-gates/python` is a link to a prepared CPython 3.14.4 environment whose `pip freeze`
  matches `requirements.lock` exactly, as `coverage-tools-provisioning.md` allows; the checkout had
  none, and the Python static gate is invalid without it.
- The tooling tests run outside the gate lock, under the memory cap: they are not .NET builds, test
  runs or Stryker.

## Related

- [Test rubric](./rubric.md) - The verdict order, the evidence a removal needs, the record line
- [DYD-1520 test audit](./_dyd-1520-test-audit.md) - Every batch's audit
