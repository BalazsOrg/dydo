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

Targeted two-phase Stryker (`/var/tmp/dyd1520/tools/stryker-targeted.sh`). Phase 1 runs on the base
over every method that loses a case or is rewritten; phase 2 runs on the candidate over the whole
batch filter. Pending at this commit; the result lands with the removals.

The Python and Node tooling tests: mutation unavailable. None is removed, so no protection needs a
planted bug there.

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

Each names a bug the dropped assertion fails on and the kept test that turns red on it. They are
recorded with their runs when the removals land.

| Bug | Kept test that must fail |
|---|---|
| P1: no `Missing frontmatter` violation | feature: A document without frontmatter is left for a manual repair |
| P2: the title finding's message changes | feature: Checking needs a title, row 4 |
| P3: a title must be followed by a paragraph (DYD-98's bug) | feature: Checking needs a title, rows 1 and 2 |
| P4: the broken-link finding's message changes | feature: Checking needs a title, row 5 |
| P5: fix appends a summary after the title | feature: Fixing leaves a document's bytes alone, rows 0 and 1 |
| P6: fix rewrites `_*.md` navigation pages | `DocumentationTests.Fix_OnAnInitialisedProject_WritesNoFileAndChangesNoByte` |
| P7: an unknown graph document exits 0 | `DocGraphTests.Graph_UnknownDocument_IsAToolError` |

## Deviations

- `viewer/dist` was copied from `/var/tmp/dyd1520/mut-setup`, a checkout whose `viewer/` tree is the
  same object (`36d5f73`), instead of rebuilt; the gate lock's three slots were held by other batches'
  Stryker runs.

## Related

- [Test rubric](./rubric.md) - The verdict order, the evidence a removal needs, the record line
- [DYD-1520 test audit](./_dyd-1520-test-audit.md) - Every batch's audit
