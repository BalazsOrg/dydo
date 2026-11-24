---
area: project
type: reference
---

# DYD-1520 Safe-Delete Audit

The safe-delete batch of the [DYD-1520](https://linear.app/bodnar-balazs/issue/DYD-1520) audit: every case
under `FullyQualifiedName~DynaDocs.Tests.Integration.SafeDelete`, the `safe-delete.feature` scenarios and
`LinkSafeDeleteTests` at `07642b1f`, judged against the [rubric](./rubric.md). The record is
`safe-delete.tsv` in this folder, one line per case in §6's format.

## Totals

| Verdict | Cases |
|---|---|
| keep | 18 |
| rewrite | 274 |
| remove | 12 |
| keep `unsure` | 24 |
| **all** | **328** before, **316** after (all executed here; the Windows-only cases return early on Linux) |

- **Removed (12), all from `safe-delete.feature`, each duplicating a kept C# case:**
  - `Recover history…` `[0]`, `[2]`, `[3]`, `[4]`: each step calls `SafeDeleteTests.Journey` with a kept Fact's name.
  - Both `Authoritative remote eligibility…` rows: the kept Facts' `Journey` call, then only a flag.
  - Refusal `[18]` (shared bare nested repository): the same helper call as `InitializedNestedGitDirectory…[0]`.
  - Refusal `[12]`, `[13]` into `[11]`, and `[25]`, `[28]` into `[24]`. Their injected refusal text is opaque to the code,
    which throws it as the message. Planted bugs `gh-msg` and `t3-msg` replace it and turn the kept row red.
  - Policy `[20]` (init rules): `InitGeneratedCodexRules_AdmitAllArchiveForms` asserts the same rule text in the file
    `dydo init` writes.
- **Restored at fix hop 1 (5), byte for byte from base, with their steps,** because their replacement's kill of a mutant
  was a race or never observed ([lesson 11](#each-removed-cases-mutants-killed-by-its-replacement-lesson-11)): the
  exclude/attributes scenario, both unpushed rows, `Recover history…[1]` and `[5]`. Four are `keep`; `[1]` is a rewrite,
  because its local ref is absent from the start, so it never showed the removal the old feature tag claimed.
- **Rewritten (274): 273 tag corrections with the assertions unchanged, and 1 body rewrite.** Each old tag claimed more
  than its case shows (§2 row 2; lessons 4, 6, 10). Each new tag names one behaviour that every case it covers asserts:
  - **43 feature rows** (24 refusal, 18 raw-command, journey `[1]`). The old feature tag ("safe delete archives unique
    work before removal") is untrue of refusals and guard verdicts. Each scenario now has its own `# Job:` line
    ([below](#gherkin-tags-captains-ruling)).
  - **The 6 `SafeDeleteTests` Facts.** The old tag claimed a purge none of them shows.
  - **45 `SafeDeleteBoundaryTests` cases.** In 43 the class tag had moved to the method unchanged; each method has its own
    tag now. The casing methods carry DYD-1029's rule for both platforms (a refusal on Windows, a round trip elsewhere).
    The other two are `DefaultAllocationVolume_Identifies…` and the body rewrite below.
  - **The 24 `SafeDeleteInputsTests` cases and all 126 `SafeDeleteGitHubTests` rows**, one tag per method.
  - **The 9 `SafeDeletePolicyTests` cases, 10 `UnsafePaths` rows and 7 `ColonPaths` rows.** The `ColonPaths` trap named
    only native Linux colon filenames, but on Windows its rows assert their refusal; it carries DYD-1134's rule now.
  - **`InitGeneratedCodexRules`**, a trap now, not DYD-1029's "admit" contract: it reads the rule text, so a `decision`
    changed from `allow` to `prompt` would stay green.
  - **The three `LinkSafeDeleteTests` link cases**, under DYD-362's "the target must survive".

  The body rewrite: `NestedGitDirectory_ValidatesFileKindsBeforeGitDiscovery` called `SafeDeleteFiles.Capture`, and two
  assertions could never fail (§2 rows 3, 5). It now drives `dydo archive worktree` and asserts the hard-link refusal, the
  refused path, no nested-repository refusal, no archive, and the branch and tree bytes kept.
- **Kept after §5 refused them:** the 18 `Raw destructive Git…` rows. Their step reads each command in the Unknown shell
  dialect and the hook rows read Posix or PowerShell, so a bug keyed to the dialect would turn no kept case red.

No CI or `dotnet-platforms.json` line names a removed or renamed case, and every class `safe-delete-windows` requires
still has cases. No step definition is left unused: the 2 steps and 1 field only the `Authoritative…` rows used are
deleted, and the `Refusal` step loses the shared-bare branch only refusal `[18]` reached. The shared helpers keep their
base access.

## Gherkin tags (captain's ruling)

The DYD-1520 captain's ruling, fix hop 2: a feature may carry a `# Job:` line directly above each Scenario or Scenario
Outline instead of one file-level header. It is the Gherkin equivalent of a C# method tag where a class tag doesn't fit
every case. The rubric's §2 row 2 applies to each scenario-level tag: true of every row of that scenario. Keep a file-level
header only if it names a rule that every scenario asserts. Otherwise drop it.

`safe-delete.feature` has no file-level header now, because no rule holds for all six scenarios. Five scenarios carry a
`# Job:` line. `Validate archive policy through the command` carries none. Its 20 rows pin different DYD-1029 rules:
restore refusals, retention, the reserve, dry runs, links and the upstream. No one tag is true of every row, so each row is
kept `unsure` (below). A true tag for each needs the outline split by rule, and that adds scenarios.

## Tags and the lines that prove them

[The tag map](./safe-delete-tag-map.md) gives each tag the assertion line that fails in each case it covers if that
behaviour were false.

## Mutation

Targeted two-phase mutation (`/var/tmp/dyd1520/tools/stryker-targeted.sh`, Stryker 5.1.0). Phase 1 ran on a
clean `07642b1f` checkout in two chunks:

- **feature**: the six scenarios and outlines that lose a row or a tag (64 cases). 1,097 killed, 1,405 survived,
  8 timeout, 2,850 s.
- **rest**: the six `SafeDeleteTests` Facts and the other C# methods rewritten or retagged (30 cases).
  878 killed, 1,557 survived, 2 timeout, 1,657 s.

Two exclusions, both deviations below: the tag-only `Raw destructive Git…` outline, and
`DefaultAllocationVolume_Identifies…`, which Stryker cannot run.

Phase 2 ran on the candidate `1c19e068` over the batch's 309 kept cases, without the random-row
`DefaultAllocationVolume_Identifies…`. Later commits change only `Job` traits, `# Job:` comments, the audit files and the
byte-for-byte restores, which can only add kills. No later commit changes a test body, so phase 2 is not owed again.

- **feature**: exit 0. All 1,097 phase-1 kills are still killed (1,233 killed after). Every removed case
  is in this chunk.
- **rest**: 878 phase-1 kills, 874 still killed, 997 killed after. Four mutants were Timeout after, not
  Killed: `SafeDeleteFiles.cs:100`, `:197`, `:203` and `:204`, each disabling the Linux file-kind refusal. A timeout is
  not a kill (review lesson 7), so each was planted exactly (`m100`, `m197`, `m203`, `m204`, on a scratch copy of
  `8d2bf92f`). The kept `NestedGitDirectory…` case fails on each, on `Assert.Contains("Special files and hard links", …)`,
  and passes with none (`/var/tmp/dyd1520/sd/mut-timeouts/result.txt`).

Every mutant phase 1 killed is still killed by a kept case.

## Each removed case's mutants, killed by its replacement (lesson 11)

Stryker stops at the first failing test, so phase 1's `killedBy` lists only some killers. Two further runs
used `--disable-bail` over the feature chunk's 1,097 killed spans: **A**, at base, the six outlines (1,744 s); **B**, on
`271cd889`, only the named replacements (1,416 s). For each removed case, every mutant it killed in A was checked against
B, and B had to show the named replacement itself killing it (`/var/tmp/dyd1520/sd/replacement_check.py`). Three mutants
failed that check, all from removed cases now restored:

| Mutant | Removed case | Named replacement | Evidence | Now |
|---|---|---|---|---|
| `SafeDeleteHistory.cs:152` (a `WaitForExit` removed: a race) | journey `[1]` | `UnfetchedRemote` | exact plant `h152`: 0 of 6 | restored |
| | unpushed `[0]` (public) | `InitiallyAbsent…[0]` | 3 of 6 | restored |
| | unpushed `[1]` (private) | `InitiallyAbsent…[1]`, the private worktree row | 0 of 6 | restored |
| `SafeDeleteGit.cs:55` (a `WaitForExit` removed: a race) | exclude/attributes | `CommonInputs…[2]` | not killed in B; exact plant `g55`: 3 of 3 | restored |
| `SafeDeleteFiles.cs:108` (the nested-`.git` refusal removed) | journey `[5]` | `DetachedHead` | not killed in B; the removed row's own kill did not reproduce (`f108`, 0 of 3) | restored |

Plant runs: `/var/tmp/dyd1520/sd/parity` and `/var/tmp/dyd1520/sd/race` (three at base, three on the candidate). For
every other removed case, B shows its named replacement killing each mutant it killed in A. Three notes:

- **Policy `[20]`**: 14 mutants in `GitRunner`, `GitResult` and `LinkSafeDelete` that the row reaches only through its
  fixture. `InitializedNested…[0]` runs the same fixture and kills all 14 in B; `codex-rules` covers the assertions.
- **`GitRunner.cs:37`** (a `WaitForExit` removal): each named replacement kills it in B, through the same `Journey`,
  `Refusal` or helper call.
- **`SafeDeleteFiles.cs:108`, from refusal `[18]`**: `InitializedNested…[0]` makes the same helper call and kills it in B.

## Mutation method (coordinator ruling 1)

> The targeted two-phase run complies with §5 when both conditions hold: phase 1 covers every method that
> was removed or whose body changed; the diff changes no shared helper or fixture that an untouched test
> uses. […] If either condition fails, the batch re-runs phase 2 over every test that uses the changed
> helper, including untouched ones.

**Condition 1.** `git diff 07642b1f -- DynaDocs.Tests` removes rows from three scenario outlines, removes the
`Authoritative…` outline, and changes one test method body. All of them are in phase 1:

| Removed or body-changed | Phase 1 chunk |
|---|---|
| `Authoritative remote eligibility…`, `Recover history…`, `Refuse without losing the original target`, `Validate archive policy through the command` | feature (the six scenarios and outlines) |
| `SafeDeleteBoundaryTests.NestedGitDirectory_ValidatesFileKindsBeforeGitDiscovery` | rest |

The rest chunk also covered the `SafeDeleteTests` Facts, `LegacyBranchArchive`, `InitGeneratedCodexRules`,
`UnsafePaths`, the `SafeDeletePolicyTests` methods and two `DeleteTree…` cases. Every other changed method changes only
its `Job` trait.

**Condition 2.** These are the changed non-test-method files and members under `DynaDocs.Tests/`:

| Changed | Change | Untouched tests that use it |
|---|---|---|
| `Integration/SafeDeleteTests.cs` `Refusal` | case labels for the removed rows `[12]`, `[13]`, `[25]`, `[28]` dropped | none: only the `Refuse…` outline calls it (phase 1) |
| `Integration/SafeDeleteTests.cs` `Policy` | the removed row `[20]`'s case dropped | none: only the `Validate archive policy…` outline (phase 1) |
| `Steps/SafeDeleteSteps.cs` | the `Authoritative…`-only steps and field deleted; the `Refusal` step's shared-bare branch dropped | the `Refuse…` rows and the restored scenarios (phase 1). `Raw destructive Git…` (18 rows) uses only unchanged steps, but the changed file, so phase 2 was rerun over it |
| `Features/safe-delete.feature` | the removed rows, and `# Job:` comment lines | comments only, for the kept rows |
| helper access modifiers (`Journey`, `PrepareInitiallyAbsent`, `Prepare`, `AssertNestedRepositoryRefused`) | narrowed, then restored to base in `0dff36ab` | none: identical to base |

**Rerun for `Raw destructive Git…`.** Phase 1 ran at base with the filter of that outline: 18 cases,
140 killed, 499 s. Phase 2 ran on `0dff36ab` with the batch filter: exit 0, all 140 still killed (145
killed after).

## Kept `unsure`

| Case | Protection | Unresolved |
|---|---|---|
| `LinkSafeDeleteTests.DeleteTree_MissingRoot_IsANoOp` | deleting an absent tree is a no-op; only this case fails on `missing-throws` | No spec, DR or ticket states it (DYD-362 covers links only). It needs a citable source, or a kept case at a boundary that deletes an absent tree |
| `LinkSafeDeleteTests.DeleteTree_ReadOnlyFilesAndLongPaths_AreDeleted` | a read-only file past 260 characters is deleted with its tree; `readonly-skip` fails it, and 290 more only in cleanup | No citable source. It needs one, or a kept assertion on the read-only long-path form |
| `SafeDeleteBoundaryTests.DefaultAllocationVolume_RefusesAnUnknownVolume` | on Windows, the default space probe throws for a target on an unknown UNC share (`:538`) | Windows-only; tag narrowed; red check owed to a Windows run |
| `SafeDeleteBoundaryTests.WindowsStreamMetadata_AcceptsDefaultStreamsAndRefusesAlternateStreams` | on Windows, metadata reads accept default streams and refuse an alternate data stream (`:549`–`:553`) | Windows-only; tag narrowed; red check owed to a Windows run |
| the 20 `Validate archive policy through the command` rows, below | each row's observable, with its red bug on the candidate | no true scenario-level tag ([Gherkin tags](#gherkin-tags-captains-ruling)). Policy `[18]`'s Windows refusal also owes its red check to a Windows run |

The Windows-only pair follows ruling 2. The policy rows' observables, in `Integration/SafeDeleteTests.cs`:

| Policy rows | Observable | Red bug |
|---|---|---|
| `[0]`–`[4]`, `[6]`, `[17]` (corrupt or incomplete archives, collisions, restore failure, traversal) | restore exits 2 with the archive kept (`:395`) | `restore-exit0` |
| `[5]` (remote restore collision) | `restore --remote` exits 2 with the archive kept (`:386`) | `restore-exit0` |
| `[7]` (partial deletion failure) | a worktree removal failing after the remote step exits 2 (`:361`) | `archive-exit0` |
| `[8]` (expiry boundary) | an archive run after the 30-day boundary purges the expired entry (`:458`) | `no-purge` |
| `[9]` (read-only commands) | list, dry run and restore keep an expired entry (`:450`–`:453`) | `list-purges` |
| `[10]`–`[13]` (the reserve rows) | an archive below the reserve, or with a negative `reserveBytes`, exits 2 (`:414`) | `archive-exit0` |
| `[14]`, `[15]` (dry runs) | `--dry-run` leaves no archive and keeps the local ref (`:432`) | `dryrun-archives` |
| `[16]` (archive symlink) | an archive under a symlinked archive root exits 2 (`:368`) | `archive-exit0` |
| `[18]` (link round trip or refusal) | a worktree symlink is refused before delete on Windows (`:348`) and round-trips its target elsewhere (`:354`) | `restore-noop`, Linux only |
| `[19]` (differently named upstream) | archiving a branch keeps its differently named upstream's remote ref (`:342`) | `upstream-deleted` |

Three C# methods also run on both platforms with different outcomes: `AdministrativeNameCasing`, `BranchNameCasing` and
`ColonPaths`. Each carries its ticket's rule for both platforms. Their red checks ran on Linux, and the Windows refusal
branch's red check is owed to a Windows run, recorded in each Evidence cell. `UnsafePaths` and the two Windows-listed
Facts run on every platform with the same assertions, so their red checks ran here.

## Cites

Every cite was checked against its ticket; the result is in [the tag map](./safe-delete-tag-map.md#cites).

## Planted bugs

Each bug is gated by `DYD1520_PLANT` in a scratch checkout of the production code, built once with analyzers off and never
committed. The scratch checkout was at `07642b1f`, then at each candidate, with the same production code. The batch
filter ran once per bug and once with none. With no bug it ran 328 of 328 green at base
(`/var/tmp/dyd1520/sd/plants-base`) and 316 of 316 on the fix hop 2 test tree `beef8ad6`
(`/var/tmp/dyd1520/sd/plants-fix2`, which reruns `list-links`, `colon-refused`, `restore-noop`, `archive-exit0` and
`restore-exit0`).

| Bug | Change | Red cases this audit names |
|---|---|---|
| gh-msg, t3-msg | the GitHub or T3 refusal text replaced | refusal `[11]` and removed `[12]`, `[13]` (and 48 more); `[24]`, `[26]`, `[27]` and removed `[25]`, `[28]` |
| codex-rules | `CodexRules` loses the `dydo.exe` prefix and the `list` form | `InitGeneratedCodexRules…` and removed policy `[20]` |
| missing-throws | deleting a missing tree throws | only `DeleteTree_MissingRoot_IsANoOp` |
| archive-exit0 | an archive refusal exits 0 | the 24 refusal rows (`:309`, `:221`), policy `[7]`, `[10]`–`[13]`, `[16]`, and the Boundary, Inputs and GitHub archive refusals (158 cases) |
| restore-exit0 | a restore refusal exits 0 | policy `[0]`–`[6]`, `[17]`; the Boundary and Inputs restore refusals |
| restore-noop | restore does nothing and exits 0 | the 6 Facts, journeys `[1]`, `[5]`–`[8]`, the exclude and unpushed rows, policy `[18]`, the Boundary and Inputs round trips and manifest refusals |
| no-purge, list-purges | purge never deletes; `list` purges | policy `[8]`; `[9]` |
| dryrun-archives | `--dry-run` archives and deletes | policy `[14]`, `[15]` (and `[9]`) |
| upstream-unreported, upstream-deleted | no upstream report; remote deletion also deletes the upstream | policy `[19]` (`:343`; `:342`) |
| guard-git-allowed, guard-all-dangerous | no `git` command is dangerous; every command is | guard rows `[0]`–`[11]`, `[17]`; `[12]`–`[16]` (and every `RawDeletionTable` row) |
| v1-refused | a version-1 archive is unsupported | `LegacyBranchArchive_RemainsReadable` |
| child-open | `SafeDeletePaths.Child` validates nothing | `UnsafePaths…[0]`–`[9]` |
| colon-refused | `Child` rejects `:` on every platform (the DYD-1134 bug) | `ColonPaths…[0]`–`[6]` (`:41`), `LinuxIgnoredTimestamp…` (`:57`) |
| list-links | `list` skips the link check on each entry | only `LinkedIncompleteEntry…`, at its `list` exit (`:289`) |
| t3-strict, t3-invalid-null | no non-empty `worktreesDirectory` is invalid; invalid settings refuse nothing | `AuthoritativeT3Settings…[0]`, `[1]`; `[2]`–`[6]` |
| t3-history-off, redact-off | no `.t3/worktrees` recognition; `Redact` returns its input | `T3HistoryPathRecognition…`; `Redaction…` |
| readonly-skip | tree deletion skips read-only files | `DeleteTree_ReadOnlyFilesAndLongPaths…` (and 290 more, only in fixture cleanup) |
| delete-follows | tree deletion follows a link into its target | the two `LinkSafeDeleteTests` link cases. `LinkedIncompleteEntry…` fails on it only in fixture disposal, so `list-links` is its red check |
| unlink-real, unlink-target | `UnlinkAll` deletes real files; it empties a link's target | `UnlinkAll_RemovesOnlyLinks` |
| gh-refuse-all, gh-accept-all | eligibility refuses everything; accepts everything | the eligible GitHub rows and complete pagination rows; the refused pagination rows and the initially-absent refusals it reaches |
| recheck-off | the final rechecks before worktree and local deletion are skipped | the post-deletion recheck rows, `FinalSnapshotRecheck…`, the initially-absent final-step rows `gh-accept-all` misses |
| volume-missing | the default space probe throws for a missing target | `DefaultAllocationVolume_Identifies…[0]`, `[1]` |
| hardlink-unchecked | the Linux file-kind check accepts a hard link | `NestedGitDirectory…` |

Each case's red bug is in its `.tsv` Evidence cell, from `/var/tmp/dyd1520/sd/redmap-cand-all.tsv`, `plants-fix2` and
`/var/tmp/dyd1520/sd/redmap-base.tsv`.

## Deviations

- **Phase 1 left out the tag-only `Raw destructive Git…` outline.** It keeps all 18 rows and every assertion, and its rows
  would have added the whole guard analyzer's mutants. `UnlinkAll_RemovesOnlyLinks`, retagged after phase 1, is tag-only.
- **`DefaultAllocationVolume_Identifies…` was judged by hand**, as the brief directs; its red proof is `volume-missing`.
- **Tag-only rewrites are proved red by planted bugs**, each breaking its tag's one behaviour on its proving line.
- **The policy outline's 20 rows are keep `unsure`, not rewrites.** No one Job line is true of all of them, and splitting
  the outline adds scenarios, which this batch was not asked to do.
- **The planted-bug builds ran with analyzers off** (the plants exceed the cognitive complexity limit), only in the
  scratch checkout.

## Related

- [Test rubric](./rubric.md) - The verdicts and evidence this audit applies
- [Safe-delete tag map](./safe-delete-tag-map.md) - Each tag and the line that proves it
- [DYD-1520 test audit](./_dyd-1520-test-audit.md) - The other batches
