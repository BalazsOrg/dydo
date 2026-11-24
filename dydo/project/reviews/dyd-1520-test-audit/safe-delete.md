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
| keep | 26 |
| rewrite | 286 |
| remove | 12 |
| keep `unsure` | 4 |
| **all** | **328** before, **316** after (all executed here; the Windows-only cases return early on Linux) |

- **Removed (12), all from `safe-delete.feature`; each one duplicates a kept C# case:**
  - `Recover history…` rows `[0]`, `[2]`, `[3]` and `[4]`. Each row's step calls `SafeDeleteTests.Journey` with the
    same name that one of the `SafeDeleteTests` Facts uses.
  - Both `Authoritative remote eligibility…` rows. Their When step is the kept Facts' `Journey` call, and
    their Then step asserts only a flag.
  - Refusal `[18]` (shared bare nested repository). It runs the same helper call as
    `SafeDeleteBoundaryTests.InitializedNestedGitDirectory_RefusesBeforePublicationOrDeletion[0]`.
  - Refusal `[12]` and `[13]` (PR base, offline GitHub), collapsed into `[11]` (PR head), and refusal
    `[25]` and `[28]`, collapsed into `[24]` (live T3 owner). Their injected refusal text is opaque to the
    code, which null-checks it and throws it as the message. Planted bugs `gh-msg` and `t3-msg` replace
    that text, and each turns the kept row red.
  - Policy `[20]` (init archive prefix rules). `SafeDeleteGuardTests.InitGeneratedCodexRules_AdmitAllArchiveForms`
    asserts the same rule text in the file `dydo init` writes.
- **Restored at fix hop 1 (5), byte for byte from base.** Each had a mutant whose kill by its named replacement was a race,
  or was never observed:
  - `Recover files governed by repository-local exclude and attributes`, for `SafeDeleteGit.cs:55`.
  - Both `Archive an unpushed worktree…` rows, for `SafeDeleteHistory.cs:152`.
  - `Recover history…[1]` (unfetched remote), for `SafeDeleteHistory.cs:152`.
  - `Recover history…[5]` (detached HEAD), for `SafeDeleteFiles.cs:108`.

  Their steps are restored too. See [lesson 11](#each-removed-cases-mutants-killed-by-its-replacement-lesson-11). Four are
  `keep`. `[1]` is a rewrite: its local ref is absent from the start, so it never showed the removal the old feature tag
  claimed.
- **Rewritten (286): 285 tag corrections with the assertions unchanged, and 1 body rewrite.** Each tag
  correction replaces a tag that claimed more than its case shows (§2 row 2; review lessons 4, 6 and 10). Every tag this
  batch touched now has the minimal form: its kind, one behaviour, and its cite, with a contract's rule in the cite's own
  words. They are:
  - **62 feature rows.** The old feature tag ("safe delete archives unique work before removal") is untrue of refusals,
    restore policies, retention, dry runs and guard verdicts. The rows reach different outcomes, so the one `# Job:` line
    names what they share: each row gets the outcome DYD-1029, or DYD-1133 for the repository-local inputs, sets for its
    input. The tag map names each row's outcome in the ticket's words.
  - **The 6 `SafeDeleteTests` Facts.** The old tag claimed a purge that none of them shows.
  - **45 `SafeDeleteBoundaryTests` cases.** In 43 the class tag ("archive before delete, refuse unproven state") had moved to
    the method unchanged. Each method shows only a refusal or only a round trip, and `LinkedIncompleteEntry` shows `list`.
    Each now has its own tag. The other two are `DefaultAllocationVolume_Identifies…`, and the body rewrite below.
  - **The 24 `SafeDeleteInputsTests` cases.** Each method now has its own tag: an edge for each injected input change,
    DYD-1029's manifest-validation rule for the unsupported manifest, DYD-1133's for `CommonInputs`, and DYD-1142's for
    `LegacyBranchArchive`.
  - **All 126 `SafeDeleteGitHubTests` rows.** The class tag's "only" needed an exhaustive assertion, and public remotes are
    eligible by the PR route. Each method has a narrower tag. `OwnPullPagination` now names the rule all its rows share.
  - **The 9 `SafeDeletePolicyTests` cases, and the 10 `UnsafePaths` rows.** The T3 settings tag no longer claims
    unreadable settings, because every row writes a readable file.
  - **`InitGeneratedCodexRules`.** It is now a trap, not DYD-1029's "admit" contract. The test reads the rule text, so a
    `decision` changed from `allow` to `prompt` would stay green.
  - **The three `LinkSafeDeleteTests` link cases.** Each carries DYD-362's rule, "the target must survive".

  The body rewrite is `SafeDeleteBoundaryTests.NestedGitDirectory_ValidatesFileKindsBeforeGitDiscovery`.
  It called `SafeDeleteFiles.Capture` directly, and two of its assertions could never fail (§2 rows 3
  and 5). It now drives `dydo archive worktree`. It asserts the hard-link refusal, the refused path, no
  nested-repository refusal, no archive, and the branch and tree bytes kept.
- **Candidates kept after §5 refused them:**
  - The 18 `Raw destructive Git…` rows. Their step reads each command in the Unknown shell dialect, and the hook rows
    read Posix or PowerShell, so a bug keyed to the dialect would turn no kept case red.
  - The two `LinkSafeDeleteTests` cases kept `unsure` (below).

No CI or `dotnet-platforms.json` line names a removed or renamed case. Every class that `safe-delete-windows`
requires still has cases. The removals leave no step definition unused. The 2 steps and 1 field that only the
`Authoritative…` rows used are deleted. The `Refusal` step loses its shared-bare branch, which only refusal `[18]` reached.
The shared helpers (`SafeDeleteGitHubTests.Journey`, `PrepareInitiallyAbsent`, `SafeDeleteInputsTests.Prepare`,
`SafeDeleteBoundaryTests.AssertNestedRepositoryRefused`) keep their base access, and the diff does not touch them.

## Tags and the lines that prove them

Every tag in the batch, kept cases included, names one behaviour. [The tag map](./safe-delete-tag-map.md) gives each tag
the assertion line that fails in each case it covers if that behaviour were false.

## Mutation

Targeted two-phase mutation (`/var/tmp/dyd1520/tools/stryker-targeted.sh`, Stryker 5.1.0). Phase 1 ran on a
clean `07642b1f` checkout in two chunks:

- **feature**: the six scenarios and outlines that lose a row or a tag (64 cases). 1,097 killed, 1,405 survived,
  8 timeout, 2,850 s.
- **rest**: the six `SafeDeleteTests` Facts and the other C# methods rewritten or retagged (30 cases).
  878 killed, 1,557 survived, 2 timeout, 1,657 s.

Two exclusions, both deviations below: the `Raw destructive Git…` outline, which is tag-only and loses no
row, and `DefaultAllocationVolume_Identifies…`, which Stryker cannot run.

Phase 2 ran on the candidate `1c19e068` over the batch's 309 kept cases. The random-row
`DefaultAllocationVolume_Identifies…` was excluded, because Stryker refuses to start with it. The later
commits change only `Job` traits, the audit files, and the restores, which are byte for byte from base. A restore adds a
kept case, so it can only add kills.

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
used `--disable-bail` over the feature chunk's 1,097 killed spans:

- **A:** at base, the six outlines. 1,744 s.
- **B:** on `271cd889`, only the named replacements. 1,416 s.

For each removed case, every mutant it killed in A was checked against B, and B had to show the named replacement
itself killing it (`/var/tmp/dyd1520/sd/replacement_check.py`). Three mutants failed that check, all from removed cases now
restored:

| Mutant | Removed case | Named replacement | Evidence | Now |
|---|---|---|---|---|
| `SafeDeleteHistory.cs:152` (a `WaitForExit` removed: a race) | journey `[1]` | `UnfetchedRemote` | exact plant `h152`: 0 of 6 | restored |
| | unpushed `[0]` (public) | `InitiallyAbsent…[0]` | 3 of 6 | restored |
| | unpushed `[1]` (private) | `InitiallyAbsent…[1]`, the private worktree row | 0 of 6 | restored |
| `SafeDeleteGit.cs:55` (a `WaitForExit` removed: a race) | exclude/attributes | `CommonInputs…[2]` | not killed in B; exact plant `g55`: 3 of 3 | restored |
| `SafeDeleteFiles.cs:108` (the nested-`.git` refusal removed) | journey `[5]` | `DetachedHead` | not killed in B; the removed row's own kill did not reproduce (`f108`, 0 of 3) | restored |

Plant runs: `/var/tmp/dyd1520/sd/parity` and `/var/tmp/dyd1520/sd/race`. The six runs per pair are three at base and three
on the candidate. In every case B shows the named replacement killing each mutant the removed case killed in A, except
policy `[20]`:

- **Policy `[20]`** (init rules): 14 mutants not killed by `InitGeneratedCodexRules`. All 14 are in
  `GitRunner`, `GitResult` and `LinkSafeDelete`. The row hits them only through its `SafeDeleteTests` fixture
  (constructor, `Dispose`), not through its rule assertions. `InitializedNested…[0]` runs the same fixture and kills
  all 14 in B. Planted bug `codex-rules` covers the assertions.
- **The other process-wait mutant.** The remaining removed cases kill one more `WaitForExit` removal in A,
  `GitRunner.cs:37`. Each named replacement kills it in B. Each runs the same Git calls as its removed case: the same
  `Journey`, `Refusal` or helper call. Policy `[20]`'s replacement for it, `InitializedNested…[0]`, runs the same fixture
  constructor and more Git calls.
- **`SafeDeleteFiles.cs:108`, from refusal `[18]`.** Its replacement, `InitializedNested…[0]`, makes the same helper call
  on a fixture that holds a nested repository, and kills it in B.

## Mutation method (coordinator ruling 1)

> The targeted two-phase run complies with §5 when both conditions hold: phase 1 covers every method that
> was removed or whose body changed; the diff changes no shared helper or fixture that an untouched test
> uses. […] If either condition fails, the batch re-runs phase 2 over every test that uses the changed
> helper, including untouched ones.

**Condition 1.** At the fix-hop candidate, `git diff 07642b1f -- DynaDocs.Tests` removes rows from three scenario outlines,
removes the `Authoritative…` outline, and changes one test method body. All of them are in phase 1:

| Removed or body-changed | Phase 1 chunk |
|---|---|
| `Authoritative remote eligibility…`, `Recover history…`, `Refuse without losing the original target`, `Validate archive policy through the command` | feature (the six scenarios and outlines) |
| `SafeDeleteBoundaryTests.NestedGitDirectory_ValidatesFileKindsBeforeGitDiscovery` | rest |

The rest chunk also covered the retagged `SafeDeleteTests` Facts and these methods: `LegacyBranchArchive`,
`InitGeneratedCodexRules`, `UnsafePaths`, the three `SafeDeletePolicyTests` methods, `DeleteTree_ReadOnly…` and
`DeleteTree_MissingRoot…`. Every other changed method changes only its `Job` trait.

**Condition 2.** These are the changed non-test-method files and members under `DynaDocs.Tests/`:

| Changed | Change | Untouched tests that use it |
|---|---|---|
| `Integration/SafeDeleteTests.cs` `Refusal` | case labels for the removed rows `[12]`, `[13]`, `[25]`, `[28]` dropped | none: only the `Refuse…` outline calls it (phase 1) |
| `Integration/SafeDeleteTests.cs` `Policy` | the removed row `[20]`'s case dropped | none: only the `Validate archive policy…` outline (phase 1) |
| `Steps/SafeDeleteSteps.cs` | the 2 steps and 1 field only the `Authoritative…` rows used are deleted; the `Refusal` step loses its shared-bare branch | the `Refuse…` outline's kept rows and the restored scenarios (phase 1). `Raw destructive Git requires the archive command` (18 rows) uses the unchanged `Given`, `Guard`, `Verdict` and `Dispose` steps, but uses the changed file, so phase 2 was rerun over it |
| `Features/safe-delete.feature` | the removed rows and the `# Job:` line | the comment only, for the kept rows |
| helper access modifiers (`Journey`, `PrepareInitiallyAbsent`, `Prepare`, `AssertNestedRepositoryRefused`) | narrowed, then restored to base in `0dff36ab` | none: identical to base |

**Rerun for `Raw destructive Git…`.** Phase 1 ran at base with the filter of that outline: 18 cases,
140 killed, 499 s. Phase 2 ran on `0dff36ab` with the batch filter: exit 0, all 140 still killed (145
killed after).

## Kept `unsure`

| Case | Protection | Unresolved |
|---|---|---|
| `LinkSafeDeleteTests.DeleteTree_MissingRoot_IsANoOp` | deleting an absent tree is a no-op. Planted bug `missing-throws` fails only this case | No spec, DR or ticket states the behaviour, so no tag can be cited. DYD-362 covers links only. It needs a citable source, or a kept case at a boundary that deletes an absent tree |
| `LinkSafeDeleteTests.DeleteTree_ReadOnlyFilesAndLongPaths_AreDeleted` | a read-only file deep in a path past 260 characters is deleted with its tree. `readonly-skip` fails it, and fails 290 other cases only in their fixture cleanup | No citable source. It needs one, or a kept assertion on the read-only long-path form |
| `SafeDeleteBoundaryTests.DefaultAllocationVolume_RefusesAnUnknownVolume` | on Windows, the default space probe throws for a target on an unknown UNC share (`:538`) | Windows-only; tag narrowed; red check owed to a Windows run |
| `SafeDeleteBoundaryTests.WindowsStreamMetadata_AcceptsDefaultStreamsAndRefusesAlternateStreams` | on Windows, metadata reads accept default streams and refuse an alternate data stream (`:549`–`:553`) | Windows-only; tag narrowed; red check owed to a Windows run |

The last two follow coordinator ruling 2. Three more cases are proved on Windows by a line only Windows runs:
`AdministrativeNameCasing`, `BranchNameCasing` and policy `[18]`. Their red checks here cover the other platforms' line,
and the Windows line's red check is owed to a Windows run too. `UnsafePaths` and the two Windows-listed Facts run on every platform
with the same assertions, so their red checks ran here.

## Cites

Every cite was checked against its ticket; the result is in [the tag map](./safe-delete-tag-map.md#cites).

## Planted bugs

Each bug is gated by `DYD1520_PLANT` in a scratch checkout of the production code. The scratch checkout
was at `07642b1f`, and again at the candidate with the same production code. It was built once with
analyzers off, and never committed. The batch filter ran once per bug and once with none. With no bug
it ran 328 of 328 green at base. `/var/tmp/dyd1520/sd/plants-base` holds the TRX of every run.

| Bug | Change | Red cases that this audit names |
|---|---|---|
| gh-msg | the GitHub eligibility refusal's text replaced | refusal `[11]` (kept) and `[12]`, `[13]` (removed), and 48 more |
| t3-msg | the T3 refusal's text replaced | refusal `[24]`, `[26]`, `[27]` (kept) and `[25]`, `[28]` (removed) |
| codex-rules | `InitCommand.CodexRules` loses the `dydo.exe` prefix and the `list` form | `InitGeneratedCodexRules…` (kept) and policy `[20]` (removed) |
| missing-throws | deleting a missing tree throws | only `DeleteTree_MissingRoot_IsANoOp`: kept |
| archive-exit0 | an archive refusal exits 0 | every kept archive refusal: the refusal rows, policy `[7]`, `[10]`–`[13]`, `[16]`, and the Boundary, Inputs and GitHub archive refusals (162 cases in all) |
| restore-exit0 | a restore refusal exits 0 | policy `[0]`–`[6]`, `[17]`, and the Boundary and Inputs restore refusals |
| restore-noop | restore does nothing and exits 0 | the six `SafeDeleteTests` Facts, journeys `[1]`, `[5]`–`[8]`, the restored exclude and unpushed rows (at base), policy `[18]`, and the Boundary round trips |
| no-purge, list-purges | purge never deletes; `list` purges expired archives | policy `[8]`; policy `[9]` |
| dryrun-archives | `--dry-run` archives and deletes | policy `[14]`, `[15]` (and `[9]`) |
| upstream-unreported, upstream-deleted | the upstream report is dropped; remote deletion also deletes the differently named upstream | policy `[19]` |
| guard-git-allowed, guard-all-dangerous | no command containing `git` is dangerous; every command is | guard rows `[0]`–`[11]`, `[17]`; `[12]`–`[16]` (and every `RawDeletionTable` row) |
| v1-refused | a version-1 archive is unsupported | `LegacyBranchArchive_RemainsReadable` |
| child-open | `SafeDeletePaths.Child` validates nothing | `UnsafePaths…[0]`–`[9]` |
| t3-strict, t3-invalid-null | settings without a non-empty `worktreesDirectory` are invalid; invalid settings refuse nothing | `AuthoritativeT3Settings…[0]`, `[1]`; `[2]`–`[6]` |
| t3-history-off, redact-off | no `.t3/worktrees` path recognition; `Redact` returns its input | `T3HistoryPathRecognition…`; `Redaction…` |
| readonly-skip | tree deletion skips read-only files | `DeleteTree_ReadOnlyFilesAndLongPaths…` (and 290 more, whose fixture cleanup deletes Git's read-only objects) |
| delete-follows | tree deletion follows a link and deletes its target's files | the two `LinkSafeDeleteTests` link cases, `LinkedIncompleteEntry…` |
| unlink-real, unlink-target | `UnlinkAll` also deletes real files; it empties a link's target first | `UnlinkAll_RemovesOnlyLinks` (candidate run) |
| gh-refuse-all, gh-accept-all | the remote eligibility check refuses everything; it accepts everything | the eligible GitHub rows and pagination's complete rows; the refused pagination rows and the initially-absent refusals it reaches |
| recheck-off | the final remote and source rechecks before the worktree and local deletion are skipped | the post-deletion recheck rows, `FinalSnapshotRecheck…`, and the initially-absent final-step rows that `gh-accept-all` does not reach |
| volume-missing | the default space probe throws for a missing target | `DefaultAllocationVolume_Identifies…[0]`, `[1]` |
| hardlink-unchecked | the Linux file-kind check accepts a hard link | `NestedGitDirectory…` at base and, rewritten, on the candidate |

Each case's own red bug is in its `.tsv` Evidence cell, from `/var/tmp/dyd1520/sd/redmap-cand-all.tsv` (the candidate)
and, for the restored rows, `/var/tmp/dyd1520/sd/redmap-base.tsv`.

## Deviations

- **Phase 1 left out the tag-only `Raw destructive Git…` outline.** It keeps all 18 rows and every
  assertion, so it drops no protection, and its 18 rows would have added the whole guard analyzer's
  mutants to an already long run. `UnlinkAll_RemovesOnlyLinks` was retagged after phase 1, and it is
  tag-only too.
- **`DefaultAllocationVolume_Identifies…` was judged by hand**, as the brief directs: its random row names
  defeat Stryker. It is kept and retagged, and its red proof is the planted bug `volume-missing`.
- **The red proofs for tag-only rewrites are planted bugs, observed at base and again on the candidate.**
  Each case's bug breaks the one behaviour its tag names, and fails that case. The restored row `[1]` was observed at base
  only, because the earlier candidate did not hold it; the candidate keeps base's production code.
- **The planted-bug builds ran with analyzers off**, because the planted branches exceed the cognitive
  complexity limit. Only the scratch checkout was built this way.

## Related

- [Test rubric](./rubric.md) - The verdicts and evidence this audit applies
- [Safe-delete tag map](./safe-delete-tag-map.md) - Each tag clause and the line that proves it
- [DYD-1520 test audit](./_dyd-1520-test-audit.md) - The other batches
