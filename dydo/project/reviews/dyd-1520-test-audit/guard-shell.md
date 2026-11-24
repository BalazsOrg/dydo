---
area: project
type: reference
---

# Guard-Shell Test Audit

The [DYD-1520](https://linear.app/bodnar-balazs/issue/DYD-1520) audit of the guard's shell-parsing
corpus against the [test rubric](./rubric.md): `GuardShellCorpusTests`, `GuardSecurityTests`,
`DangerousPatternCultureTests` and `GuardOffLimitsTests`, 1,794 executed cases at
`3f248ab5604c69ac116a6e71839ab2389cf8d078`. One line per case is in `guard-shell.tsv`, beside this
page, with the rubric's §6 columns (a code path, since `dydo check` links only `.md` files).

## Totals

| Verdict | Cases |
|---|---|
| keep | 1,084 |
| rewrite | 662 |
| remove | 48 |
| keep `unsure` | 0 |

The batch runs 1,746 cases after the apply step (1,794 less 48).

- **Removed (48), all from `GuardSecurityTests`.** Each one repeats a kept case's exact input, in
  the same lane, with an assertion at most as strict: 42 are rows the corpus runs, usually refused on
  both attempts where the removed case checked only the first; 6 repeat a kept `GuardSecurityTests`
  case that asserts more lanes (`Gs02_RoundFiveApostrophes[0]` and `[5]`,
  `Gs02_SingleQuotedTextAProgramMayRun[0]`, `[5]` and `[6]`). The kept case gets the same hook JSON or
  CLI argv, after the same `InitProjectAsync("none")`, so it fails on every bug the removed case
  fails on. Seven whole methods (28 rows) go, since every row went: `Finding4_DangerousPattern_CheckedBeforeNudges`,
  `Gs02_SingleQuotedLiteralDataWrite_IsAllowed`, `Gs02_SingleQuotedSetContentWrite_IsDeniedAsOnMaster`,
  `Gs02_DynamicWrite_NextToLiteralData_IsStillDenied`, `Gs05_PythonWithNothingFedToIt_IsAllowed`,
  `Gs02_DynamicWrite_InAnyWordACommandRuns_IsDenied` and `Gs05_PythonWithAFeedOnlyInAComment_IsAllowed`.
  The other 20 are rows of methods that stay; two rows of `GuardRetroCommands.Gs02SingleQuotedLiteralLines`
  and two of `Gs02NarrowedLines` leave those member lists, which feed only `GuardSecurityTests`.
- **Rewritten (662): a tag correction, no assertion dropped.** `DangerousCommand_IsRefusedOnEveryAttempt`
  (462 rows) and `LookAlike_IsAllowed` (200 rows) called DYD-188 a `trap`. DYD-188 is a Feature (the
  David Ondrej denylist bake, labelled Feature in Linear), so both tags failed §2 row 2. Its rows pin
  the denylist and its allow probes, recorded in `dydo/understand/guard-system-denylist.md`, so the
  tags now cite that as a `contract` beside the real traps. The brief's
  `GuardShellCorpusTests.cs:121` was right to doubt it; line 175 had the same fault.
- **Kept (1,084).** Corpus rows differ from one another in some input feature the reader or analyzer
  reads (device family, wrapper, quote form, separator, here-string terminator, lane, path tier), so
  a planted bug keyed to one row's feature turns only that row red; none collapse. The other
  `GuardSecurityTests` cases assert a lane, input or observable no kept case asserts: a Codex
  `shell_command` or pwsh envelope, the `--host claude` lane, a refusal reason, a deny-then-allow
  retry, a nudge, or a different path. Every cited issue's kind was checked. Every DYD cite in a `trap`
  tag is a Bug in Linear except DYD-421, a DECIDED Question cited as `contract`. The `issue #N` cites
  are the historical `dydo/project/issues/00NN-*.md` files (gone from the tree, in git history), not
  GitHub numbers.

No case is kept `unsure`: no case was found that shows a §3 sign but lacks the §5 evidence to go.

## Mutation

Targeted two-phase Stryker.NET with `/var/tmp/dyd1520/tools/stryker-targeted.sh` (the batch's
analyzer is 134 KB; a whole-batch baseline would take hours).

- **Phase 1** at `3f248ab5`, the 48 removed cases: 3,221 mutants tested in 2,380 s; 740 killed, 51
  timed out, 2,430 survived (10,702 no coverage, 1,689 compile errors, 1,620 ignored). Every
  removed case kills at least 2 mutants (the per-case counts are in the table's Evidence cells).
- **Phase 2** on the candidate, every kept case of the four classes, against phase 1's 740 kills:
  pending the apply step.
- **Timeouts are not compared.** The tool matches `Killed` only. The 51 timed-out mutants hang the
  guard on a removed case's input; each kept cover runs that same input.

## Planted bugs

Each was planted in a scratch checkout of `3f248ab5`, built, run on the named tests, and reverted;
none is committed.

| Bug | Planted in `Services/BashCommandAnalyzer.cs` | Result |
|---|---|---|
| P1, red proof of the `DangerousCommand_IsRefusedOnEveryAttempt` retag | the DYD-188 force-push rule (`OndrejGitPushRegex`) deleted from `DangerousPatterns` | `DangerousCommand_IsRefusedOnEveryAttempt` 10 of 462 red: `git push -f`, `nice git push --force`, `GIT_SSH=x git push --force`, `env FOO=1 git push --force` and `bash -lc 'git push --force'`, in both lanes (the other force-push rows still meet another rule) |
| P2, red proof of the `LookAlike_IsAllowed` retag | `RunsPasswordStore` refuses on the word `pass` alone, without its real-program check (the DYD-294/295 false refusal) | `LookAlike_IsAllowed` 4 of 200 red: `Gs04ReviewVerdictGrep` and the PR-body heredoc holding `PASS`, in both lanes |
| P3, the removed allow rows' covers | `RunsPythonFromStdin` refuses every line that names python, fed or not | all 8 removed `Gs05_PythonWithNothingFedToIt` and `Gs05_PythonWithAFeedOnlyInAComment` rows red, and so are their covers `LookAlike_IsAllowed[67]` to `[71]` and `[74]` to `[76]` (80 look-alike rows red in all) |

P3 ran the removed rows from the scratch class described under Deviations.

## Canary compares carry no protection

Several removed cases also compare `dydo/index.md` or `dydo/files-off-limits.md` before and after.
That assertion can never fail (rubric §3): `dydo guard` decides and runs nothing, and its only
writes are its warn markers and its validation timestamp (`Commands/GuardCommand.cs:893`, `:899`,
`:1243`). Dropping it drops no protection. The kept cases that make the same compare keep it; it is
harmless there.

## Deviations

- **Phase 1 ran on a scratch class, not on method filters.** A VSTest filter selects methods, not
  theory rows. The removed rows sit in methods of up to 150 rows, and the method-level run measured
  2.6 s a mutant over 3,317 mutants, past the 7,000 s run limit. So phase 1 ran on one uncommitted
  class in the base checkout, `Dyd1520RemovedRows`, holding exactly the 48 removed cases: the same
  bodies, helpers and rows, checked line by line against the base source. Production code was the
  base's.
- **The two retag rewrites are outside phase 1.** They drop no assertion and keep their bodies, so
  §5 asks no evidence of them. The red proofs below show each can fail.
- **No Windows mutation evidence.** No case in the batch is Windows-only. Each removed case's cover
  runs the identical input and also runs in `guard-windows`, so it fails on Windows wherever the
  removed one would.

## Related

- [Test rubric](./rubric.md) - The rubric every verdict here applies
- [DYD-1520 test audit](./_dyd-1520-test-audit.md) - Every batch's audit
