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
| 299 C# and Gherkin | 236 | 9 | 38 | 16 |
| 15 Python | 15 | 0 | 0 | 0 |
| 5 Node | 5 | 0 | 0 | 0 |
| **319** | **256** | **9** | **38** | **16** |

Most of this batch already drives `dydo` at the command line and carries true tags, so most of it is
kept. What goes is the rule-level and resolver-level units on hand-built `DocFile`s
(`BrokenLinksRuleTests`, `RelativeLinksRuleTests`, `LinkResolverTests`), which fail §2 row 3, plus
exact duplicates of the optional-summaries feature and of other kept tests.

- **Removed (16):** 4 `BrokenLinksRuleTests` and 3 `RelativeLinksRuleTests` cases whose input shape a
  kept `dydo check` or `dydo graph` case already supplies; `CheckDocValidatorTests` rows 1, 27, 28, 29
  and 30 and `FixFileHandlerTests.Fix_ValidDocument_KeepsItsBytes` rows 0 and 1, which the feature
  asserts at least as strictly; and `CollapseRelativeSegments_ResolvesDotDot` rows 7 and 8, which
  `BothReadings` rows 30 and 29 repeat with both readings.
- **Rewritten (38):** the other 28 link-rule cases become rows of one `dydo check` theory,
  `CheckDocValidatorTests.Check_LinkFindings_AreExactlyTheOnesReported`. Each row keeps its case's
  input shape (the source's folder, the target path, the target's anchors, the other links in the
  document) and asserts the whole list of findings, each message once, and the exit. The nine
  `LinkResolverTests` cases keep their names and rows but drive `dydo check` and `dydo graph`, the
  two commands whose agreement is trap #0187. `FixCommandIntegrationTests.Check_IgnoresObsidianFolder`
  is kept verbatim with a method tag naming `dydo check`, which its class tag does not.
- **Restored after review round 1:** `Fix_IgnoresObsidianFolder`, `Check_IgnoresObsidianFolder`,
  `Fix_PreservesHandwrittenNavigationFiles` and `Graph_UnknownFile_IsAToolError` carry inputs no kept
  test supplies: an `.obsidian/` folder of JSON, a sibling document beside handwritten navigation pages
  under an explicit root, and an unknown bare filename, which graph's lookup matches by basename.
- **Tags:** every kept case's tag was swept against §2 row 2 (review lesson 6). A class tag that
  claimed more than some of its methods observe became method tags: `CheckDocValidatorTests`,
  `FixFileHandlerTests`, `ValidateCommandTests`, `OffLimitsRuleTests`, `FixCommandIntegrationTests`,
  `DocumentationTests`, `ChangelogEvidenceTests`, `DocGraphTests`, `LinkResolverTests` (its anchor-only
  case cites #0186, not #0187), `JsonTextTests` and `PathUtilsTests` (only `BothReadings` claims both
  host readings). Narrowed: the help tag (it checks `dydo help`, not the reference), the `explains:`
  tags (rule 4; no test shows rule 6's grandfathering), the feature tag (it now names the title, link
  and frontmatter findings its rows assert), four Python tags and the Node collision tag (each claimed
  a case its assertions never drive). `Graph_AnyDegree_GetsAnAnswer` and the `GetMainProjectRoot`
  rows, kept `unsure`, carry no tag. The `setup-skills.test.cjs` tags cite README "Install the skills",
  its real heading (`README.md:164`); they cited "Adopting the skills". Each kept row's Evidence in
  `docs.tsv` is its tag as the tree now carries it.

## Mutation

Targeted two-phase Stryker 5.1.0 (`/var/tmp/dyd1520/tools/stryker-targeted.sh`).

- **Phase 1**, base `07642b1f3`, the 96 cases of every method that loses a case or is rewritten
  (`BrokenLinksRuleTests`, `RelativeLinksRuleTests`, `LinkResolverTests`, and the methods
  `Check_ReportsTheDocumentsFinding`, `Fix_ValidDocument_KeepsItsBytes`,
  `Fix_PreservesHandwrittenNavigationFiles`, both `.obsidian` tests, `Graph_UnknownFile_IsAToolError`,
  `CollapseRelativeSegments_ResolvesDotDot`): **504 killed** (3 timeouts not counted), 781 s.
- **Phase 2**, fix candidate `6b17ea167`, the 279 kept cases of the batch filter, over phase 1's 130
  killed spans: 542 killed, 341 s; **501 of the 504 still killed**, comparison exit 1 on the same
  three mutants as the apply hop's run on `797d3ec88`:

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
| After, candidate `6b17ea167` | 279 passed | 5.8 s |

The full suite on `6b17ea167`: 4,566 passed and 44 skipped (the base's known skips), 4,610 cases, 248 s.

## Keep `unsure`

- `DocumentationTests.Graph_AnyDegree_GetsAnAnswer` rows 0 to 2 (`--degree` 0, -1, 100).
  Protection: `dydo graph <file> --degree N` exits 0. Unresolved: `dydo-commands-core.md` shows only
  `--degree 2`, so whether an out-of-range degree should answer or be a tool error is undecided, and
  no kept test covers another N. The rows carry no tag.
- `PathUtilsTests.GetMainProjectRoot_WorktreeCwd_ReturnsMainRoot` rows 0 to 2. Protection:
  `OffLimitsService` maps a cwd under `dydo/_system/.local/worktrees/` to the main checkout, so
  off-limits patterns still match from a legacy dydo worktree. Unresolved: 4.0.0 creates no worktree
  there (`WorktreeCleaner` uses `~/.t3/worktrees`), yet the guard still reads it, and no record keeps
  or drops that layout. The rows carry no tag.
- `PathUtilsTests.GetMainProjectRoot_NonWorktreeCwd_ReturnsNull` rows 0 to 2. Protection: an ordinary
  cwd maps to no main checkout, so `OffLimitsService.cs:125` falls back to the cwd's own project root
  and reads that project's off-limits file. Unresolved: no kept guard or off-limits test is shown
  failing when an ordinary cwd maps to another root, and while the legacy layout is undecided, so is
  whether this fallback needs a test of its own. The rows carry no tag.

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
| P8: the missing-title violation is dropped | F | feature: Checking needs a title, row 4, the only red case |

## Red Proofs

Each rewritten or restored case went red on a planted break of the behaviour it guards. `G` sets
(`plant2.py`) ran on scratch checkouts of the fix candidate (`9230903ba` for G1 to G14, `6b17ea167`
for G15); `A` to `C` (`plant.py`) are the
apply hop's breaks, rerun on the same checkout. `Check_LinkFindings_AreExactlyTheOnesReported` is
`LinkFindings` below, its rows by ordinal.

| Break | Set | Red |
|---|---|---|
| every finding is printed twice | G1 | `LinkFindings` 0, 1, 2, 5, 7, 8, 9, 10, 12, 22: every row with a finding, and no other batch test |
| a document's first broken link is dropped when it has more links | G2 | `LinkFindings` 1 only |
| a sibling link's anchor is never checked | G3 | `LinkFindings` 2 only |
| a parent link to the root index is rejected | G4 | `LinkFindings` 3 only |
| a two-level climb into another folder is rejected | G5 | `LinkFindings` 4 (and `LinkResolverTests` row 4); row 3's climb to a root-level file stays green |
| a missing same-page anchor passes when it is the document's only link | G6 | `LinkFindings` 5 and 22; the kept two-link `#nowhere` row stays green |
| an anchor-only finding loses its anchor | G7 | `LinkFindings` 5 and 22 |
| a wikilink is also reported as a broken link | G8 | `LinkFindings` 7 only |
| image syntax is not extracted as a link | G9 | `LinkFindings` 12 only (the missing svg) |
| after an existing asset, a document's later links go unchecked | G10 | `LinkFindings` 10 only (both assets in one document) |
| a missing asset gets another message | G11 | `LinkFindings` 9, 10, 12 |
| the scanner reads `.json` under the docs root | G12 | `Fix_IgnoresObsidianFolder`, `Check_IgnoresObsidianFolder` |
| an unknown bare filename exits 0 from graph | G13 | `Graph_UnknownFile_IsAToolError` only; `DocGraphTests.Graph_UnknownDocument_IsAToolError` stays green |
| a sibling link's existing anchor is rejected | G15 | `LinkFindings` 23 (the root-level sibling), with `LinkResolverTests` anchored row 0 |
| fix appends a newline to `_*.md` pages | G14 | `Fix_PreservesHandwrittenNavigationFiles`, `Fix_OnAnInitialisedProject_WritesNoFileAndChangesNoByte` |
| external links are link-checked; wikilinks are link-checked; absolute paths are not reported; no extension counts as an asset | A | `LinkFindings` 6, 7, 8, 10 to 20 |
| a non-markdown target is never checked on disk; only the first broken link is reported; a link without `.md` is not reported; no document counts as generated | B | `LinkFindings` 1, 9, 10, 12, 21 |
| prose quoting the marker marks the file generated; a cross-document anchor is never checked; `Broken link:` becomes `Dead link:` | C | `LinkFindings` 0, 1, 2, 5, 8, 22 |
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
