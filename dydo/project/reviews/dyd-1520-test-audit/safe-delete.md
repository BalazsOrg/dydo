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
| keep | 146 |
| rewrite | 165 |
| remove | 17 |
| keep `unsure` | 0 |
| **all** | **328** before, **311** after (all executed here; the Windows-only cases return early on Linux) |

- **Removed (17), all from `safe-delete.feature`; each one duplicates a kept C# case:**
  - The 6 `Recover history…` rows `[0]`–`[5]`. Each row's step calls `SafeDeleteTests.Journey` with the
    same name that one of the six `SafeDeleteTests` Facts uses.
  - The `Recover files governed by repository-local exclude and attributes` scenario. It has the same
    fixture and assertions as `SafeDeleteInputsTests.CommonInputs_ArchiveAndRestoreOrdinaryGitBehavior[2]`,
    which also reads status with `--untracked-files=all`.
  - Both `Archive an unpushed worktree…` rows. They match
    `SafeDeleteGitHubTests.InitiallyAbsentClosedWorld_ArchivesWithoutPushAndRestoresIndependently[0]` and `[2]`.
  - Both `Authoritative remote eligibility…` rows. Their When step is the kept Facts' `Journey` call, and
    their Then step asserts only a flag.
  - Refusal `[18]` (shared bare nested repository). It runs the same helper call as
    `SafeDeleteBoundaryTests.InitializedNestedGitDirectory_RefusesBeforePublicationOrDeletion[0]`.
  - Refusal `[12]` and `[13]` (PR base, offline GitHub), collapsed into `[11]` (PR head), and refusal
    `[25]` and `[28]`, collapsed into `[24]` (live T3 owner). Their injected refusal text is opaque to the
    code, which null-checks it and throws it as the message. Planted bugs `gh-msg` and `t3-msg` replace
    that text, and each turns the kept row red.
  - Policy `[20]` (init archive prefix rules). `SafeDeleteGuardTests.InitGeneratedCodexRules_AdmitAllArchiveForms`
    asserts the same rule text, more strictly, in the file `dydo init` writes.
- **Rewritten (165): 164 tag corrections with the assertions unchanged, and 1 body rewrite.** Each tag
  correction replaces a class or feature tag that claimed more than its case shows (§2 row 2; review
  lessons 4 and 6). They are:
  - 61 feature rows. The old feature tag ("safe delete archives unique work before removal") is untrue
    of refusals, restore policies, retention, dry runs and guard verdicts. The new `# Job:` line names
    one clause per behaviour, and each row shows one clause.
  - The 6 `SafeDeleteTests` Facts. The old tag claimed a purge that none of them shows. The new tag
    claims only the restore after source gc: `UnfetchedRemote` asserts no deletion, because its local
    ref was already absent.
  - The 9 `SafeDeletePolicyTests` cases (T3 settings, T3 history paths, redaction). The old tag said
    "unproven PR heads", and none of them involves a PR.
  - The 10 `UnsafePaths` rows. The old tag said "colon filenames", and these rows are traversal and
    separator refusals.
  - `LegacyBranchArchive`, `InitGeneratedCodexRules`, both `DefaultAllocationVolume_Identifies…` rows,
    and the `LinkSafeDeleteTests` cases for read-only files, a missing root and `UnlinkAll`.
  - 71 `SafeDeleteGitHubTests` rows in 14 methods. The class tag's one clause, "remote cleanup only on
    proved closed-world state", has no proving line in them. The eligible cases show a proved state
    passing, not the refusal that "only" claims. The pagination rows show both verdicts. The
    post-deletion and initially-absent cases show local targets kept, with no remote ref left to clean
    up. Each method now carries the narrower tag mapped below. Its red proof is `gh-refuse-all`,
    `gh-accept-all` or `recheck-off`, as each row's Evidence names. The 55 refusal rows that keep the
    remote ref keep the class tag.

  Where a class tag was true of only some methods, it moves to those methods unchanged, and they stay
  `keep`. The body rewrite is `SafeDeleteBoundaryTests.NestedGitDirectory_ValidatesFileKindsBeforeGitDiscovery`.
  It called `SafeDeleteFiles.Capture` directly, and two of its assertions could never fail (§2 rows 3
  and 5). It now drives `dydo archive worktree`. It asserts the hard-link refusal, the refused path, no
  nested-repository refusal, no archive, and the branch and tree bytes kept.
- **Candidates kept after §5 refused them:**
  - The 18 `Raw destructive Git…` rows. `SafeDeleteGuardTests` runs the same commands through the hook,
    but the step reads them in the Unknown shell dialect (both readings), and the hook rows read Posix or
    PowerShell, so a bug keyed to the dialect would turn no kept case red.
  - `LinkSafeDeleteTests.DeleteTree_MissingRoot_IsANoOp`. Planted bug `missing-throws` turns no other
    batch case red.

No CI or `dotnet-platforms.json` line names a removed or renamed case. Every class that `safe-delete-windows`
requires still has cases. The removals leave no step definition unused: the 6 steps that only the removed
scenarios used are deleted. The helpers they had shared (`SafeDeleteGitHubTests.Journey`, `PrepareInitiallyAbsent`,
`SafeDeleteInputsTests.Prepare`, `SafeDeleteBoundaryTests.AssertNestedRepositoryRefused`) still have callers
in their own classes, and are now private.

## Tag clauses and the lines that prove them

Every job tag in the batch, kept cases included, split into its clauses. Each clause names the assertion
line, at the candidate, that fails in each case it covers if that clause were false. A tag that lists
behaviours separated by commas or semicolons (the feature `# Job:` line, `SafeDeleteBoundaryTests`,
`SafeDeleteInputsTests`) claims for each case only the clause mapped to it below. Each of the other tags
is one clause, and every case under it proves that clause. Clauses that no line proved were dropped
before the map was written:

- `UnlinkAll`: "removes each link" (its link assertion had no planted bug of its own).
- `SafeDeleteTests`: "is deleted" (`UnfetchedRemote` asserts no deletion).
- `SafeDeleteGitHubTests`: "only" on the eligible cases. Those cases now carry tags of their own.

Paths are under `DynaDocs.Tests/`.

| Tag (where) | Clause | Cases and proving lines |
|---|---|---|
| feature `# Job:` (`Features/safe-delete.feature`) | an archived branch or worktree restores its unique state after deletion | journeys `[6]`, `[7]`: `Integration/SafeDeleteTests.cs:126` (deleted), `:133` (restore exit 0), `:149` (tip back), `:159` (remote absent or present as asked); `[8]`: `:86`, `:87`, `:90`–`:92` |
| | a refusal exits 2 and keeps the target | refusal rows: `SafeDeleteTests.cs:309` (exit 2), `:311`–`:318` (refs, tree, bytes); current worktree `:221`–`:224`; reserve rows `:414`; archive symlink `:368` |
| | a failure after the remote deletion exits 2 and keeps the complete archive and the local targets | policy `[7]`: `:361`, `:362` |
| | restore refuses a damaged or colliding archive and keeps it | policy `[0]`–`[4]`, `[6]`, `[17]`: `:395` (exit 2, entry kept), `:396` (no ref); `[5]`: `:386` |
| | a dry run prints its estimates and neither archives nor deletes | policy `[14]`, `[15]`: `:431`–`:434` |
| | archives expire only when a later archive purges them | policy `[8]`: `:446` (kept at 30 days), `:458` (purged after); `[9]`: `:450`–`:453` |
| | a differently named upstream is reported and kept | policy `[19]`: `:342`, `:343` |
| | links round-trip (Windows: refused) | policy `[18]`: `:353`, `:354`; on Windows `:348` |
| | the guard refuses raw destructive Git and admits dydo archive | guard rows `[0]`–`[17]`: `Steps/SafeDeleteSteps.cs:39` |
| `SafeDeleteTests` class | an archived branch or worktree restores its unique history and files after source gc | `SafeDeleteTests.cs:133` (restore exit 0) with `:142`–`:149` and `:154`–`:156` per journey; stash `:185`, `:188`–`:193` |
| `SafeDeleteBoundaryTests` (per method) | archive before delete: the archived state restores | `RepositoryFragments` `:143` (create-only restore, so it also proves the deletion), `:144`; `AdministrativeNameCasing` `:166`–`:169`; `WorktreeFilemode` `:190`–`:194`; `BranchNameCasing` `:224`, `:225`; `PerWorktreeRefTip` `:250`–`:252`; `ExplicitRemoteRestore` `:306`–`:309`; `IndexConflictStages` `:436`–`:438`; `EveryResolveUndoStage` `:470`–`:474` |
| | refuse unproven state | `InitializedNested…` `:45`–`:56`; `NestedGitDirectory…` `:116`–`:122`; `RestoreWithChangedSharedSettings` `:208`–`:211`; `AllocationVolumeUnavailable` `:261`, `:262`; `MalformedManifestFields` `:278`; `LinkedIncompleteEntry` `:289`, `:290`; `CorruptManifestGitConfiguration` `:319`; `RestorePathCollision` `:331`, `:332`; `FinalBranchRecheck` `:341`, `:342`; `FinalSnapshotRecheck` `:365`, `:366`; `NativeBusyProbe` `:381`–`:383`; `CompareAndSwap` `:402`–`:406`; `PostFetchReserveShortfall` `:416`–`:419`; `ResolveUndoBlobMissing…` `:494`–`:497`; `ResolveUndoListing…` `:511`–`:513`; `DefaultAllocationVolume_RefusesAnUnknownVolume` `:538`; `WindowsStreamMetadata` `:553`; `InvalidReserveConfiguration` `:565`, `:566`; `CorruptProofPayload` `:578`, `:579` |
| `DefaultAllocationVolume_Identifies…` | finds the containing volume of a missing target, on the root volume and under the temp path | `[0]`, `[1]`: `SafeDeleteBoundaryTests.cs:530` |
| `SafeDeleteGitHubTests` refusal methods (class tag, unchanged) | remote cleanup only on proved closed-world state | `Refusal` helper `:659` (exit 2), `:670` (remote tip kept); `FinalPrePushChange` `:139`, `:140`→`:670`; `FinalMetadataRead` `:157`, `:159`→`:670`; `OwnDeletionReceipt` `:284`; `ProcessFailures…` `:571`–`:577` |
| `PublicExactRef…`, `InitiallyAbsentClosedWorld_CompleteOwn…` | a proved closed-world state passes the remote eligibility check | `:19`; `:295` |
| `PrivateClosedWorld_RealCli…`, `PublicFork_Leased…` | on a proved closed-world state the remote ref is deleted with the archived worktree | `Journey` `:36` (exit 0), `:39` (remote ref gone) |
| `InitiallyAbsentClosedWorld_ArchivesWithoutPush…` | a proved closed-world absence of the remote ref lets the local targets be deleted | `:310`–`:312` |
| `OwnPullPagination…` | own-PR pagination proves eligibility only when it is terminal, unique and complete | every row `:546` |
| `AfterOwnDeletion_RecheckFailure…`, `EveryRemainingDestructiveStep…`, `AfterOwnDeletion_ApiRefSurvives…` | after the remote step, a failed recheck keeps the remaining local targets | `:195`; `:218`, `:219`; `:260`, `:261` |
| the five `InitiallyAbsent…` refusal methods | with the remote ref initially absent, an unproved or changed closed-world state keeps every local target | `:371`; `:389`; `:420`, `:421`; `:465`, `:466`; `:506` (`AssertInitiallyAbsentLocalIntact` asserts at `:473`–`:481`) |
| `SafeDeleteInputsTests` (per method) | repository exclude and attribute inputs survive | `ChangedCommonInput` `:28`, `:30`; `CorruptInputPayload` `:85`; `ChangedRestoreInputs` `:109`; `CommonInputs` `:187`, `:192`–`:197` |
| | refusal output complete (names its cause) | `ChangedCommonInput` `:29`; `StatCleanEntry…` `:57`, `:58`; `ChangedRestoreInputs` `:105`; `UnsupportedInputManifest` `:140` |
| `LegacyBranchArchive_RemainsReadable` | a version-1 branch archive still restores | `:165`, `:166` |
| `RawDeletionTable…` | safe-delete through the guard | `SafeDeleteGuardTests.cs:31` (denied, names dydo archive), `:34` (allowed), `:36` (the compound command denied) |
| `InitGeneratedCodexRules…` | dydo init's Codex rules admit every dydo archive form | `:65`, `:67` |
| `UnsafePaths…` | an archive entry path never resolves outside its root | `SafeDeletePathsTests.cs:23` |
| `ColonPaths…`, `LinuxIgnoredTimestamp…` | native Linux colon filenames | `:41`; `:57`–`:66` |
| `AuthoritativeT3Settings…` | no or an empty worktree root refuses nothing | `[0]`, `[1]`: `SafeDeletePolicyTests.cs:23` |
| | invalid or unreadable settings refuse an ordinary tree | `[2]`–`[6]`: `:23`, `:24` |
| `T3HistoryPathRecognition…` | a `.t3/worktrees` path is refused as T3-owned with no settings file | `:33` |
| `Redaction…` | userinfo, query and fragment are dropped from a printed URL | `:41` |
| `LinkSafeDeleteTests` link cases | deleting a tree removes a link, never its target | `DeleteTree_LinkInside` `Services/LinkSafeDeleteTests.cs:19`, `:20`; `RootThatIsALink` `:65`, `:66` |
| `UnlinkAll_RemovesOnlyLinks` | never touches a link's target | `:34` |
| | never touches a real file | `:33` |
| `DeleteTree_ReadOnlyFilesAndLongPaths…` | removes read-only files, deep in a long path | `:50` |
| `DeleteTree_MissingRoot_IsANoOp` | deleting an absent tree is a no-op, not an error | the call at `:73` (a throw fails the case), `:75` |

## Mutation

Targeted two-phase mutation (`/var/tmp/dyd1520/tools/stryker-targeted.sh`, Stryker 5.1.0). Phase 1 ran on a
clean `07642b1f` checkout in two chunks:

- **feature**: the six scenario outlines that lose a row or a tag (64 cases). 1,097 killed, 1,405 survived,
  8 timeout, 2,850 s.
- **rest**: the six `SafeDeleteTests` Facts and the other C# methods rewritten or retagged (30 cases).
  878 killed, 1,557 survived, 2 timeout, 1,657 s.

Two exclusions, both deviations below: the `Raw destructive Git…` outline, which is tag-only and loses no
row, and `DefaultAllocationVolume_Identifies…`, which Stryker cannot run.

Phase 2 ran on the candidate `1c19e068` over the batch's 309 kept cases. The random-row
`DefaultAllocationVolume_Identifies…` was excluded, because Stryker refuses to start with it. The later
commits change only `Job` traits and the audit files, not test bodies.

- **feature**: exit 0. All 1,097 phase-1 kills are still killed (1,233 killed after). Every removed case
  is in this chunk.
- **rest**: 878 phase-1 kills, 874 still killed, 997 killed after. Four mutants were Timeout after, not
  Killed: `SafeDeleteFiles.cs:100` (the `ValidateKind` call removed), `:197` (the Linux guard negated),
  `:203` (`||` to `&&`) and `:204` (the throw removed). Each disables the Linux file-kind refusal, so a
  kept FIFO refusal row then blocks reading its pipe. At base the unit form of `NestedGitDirectory…`
  killed them quickly. A timeout is not a kill (review lesson 7). So each mutant was planted exactly
  (`DYD1520_PLANT=m100`, `m197`, `m203`, `m204`, on a scratch copy of `8d2bf92f`). The rewritten,
  kept `NestedGitDirectory…` case fails on each in about 0.1 s, on
  `Assert.Contains("Special files and hard links", …)`, and passes with none. Its output is
  "Initialized nested repository" instead. Results: `/var/tmp/dyd1520/sd/mut-timeouts/result.txt`.

Every mutant phase 1 killed is still killed by a kept case.

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
| archive-exit0 | an archive refusal exits 0 | every kept refusal row and policy `[7]`, `[10]`–`[13]`, `[16]` (162 cases in all) |
| restore-exit0 | a restore refusal exits 0 | policy `[0]`–`[6]`, `[17]` |
| restore-noop | restore does nothing and exits 0 | the six `SafeDeleteTests` Facts, journeys `[6]`–`[8]`, policy `[18]` (70 cases in all) |
| no-purge, list-purges | purge never deletes; `list` purges expired archives | policy `[8]`; policy `[9]` |
| dryrun-archives | `--dry-run` archives and deletes | policy `[14]`, `[15]` (and `[9]`) |
| upstream-unreported, upstream-deleted | the upstream report is dropped; remote deletion also deletes the differently named upstream | policy `[19]` |
| guard-git-allowed, guard-all-dangerous | no command containing `git` is dangerous; every command is | guard rows `[0]`–`[11]`, `[17]`; `[12]`–`[16]` (and every `RawDeletionTable` row) |
| v1-refused | a version-1 archive is unsupported | `LegacyBranchArchive_RemainsReadable` |
| child-open | `SafeDeletePaths.Child` validates nothing | `UnsafePaths…[0]`–`[9]` |
| t3-strict, t3-invalid-null | settings without a non-empty `worktreesDirectory` are invalid; invalid settings refuse nothing | `AuthoritativeT3Settings…[0]`, `[1]`; `[2]`–`[6]` |
| t3-history-off, redact-off | no `.t3/worktrees` path recognition; `Redact` returns its input | `T3HistoryPathRecognition…`; `Redaction…` |
| readonly-skip | tree deletion skips read-only files | `DeleteTree_ReadOnlyFilesAndLongPaths…` (and 290 more, whose fixture cleanup deletes Git's read-only objects) |
| unlink-real, unlink-target | `UnlinkAll` also deletes real files; it empties a link's target first | `UnlinkAll_RemovesOnlyLinks` (candidate run) |
| gh-refuse-all, gh-accept-all | the remote eligibility check refuses everything; it accepts everything | the eligible GitHub rows and pagination's complete rows; the refused pagination rows and the initially-absent refusals it reaches |
| recheck-off | the final remote and source rechecks before the worktree and local deletion are skipped | the post-deletion recheck rows and the initially-absent final-step rows that `gh-accept-all` does not reach |
| volume-missing | the default space probe throws for a missing target | `DefaultAllocationVolume_Identifies…[0]`, `[1]` |
| hardlink-unchecked | the Linux file-kind check accepts a hard link | `NestedGitDirectory…` at base and, rewritten, on the candidate |

## Deviations

- **Phase 1 left out the tag-only `Raw destructive Git…` outline.** It keeps all 18 rows and every
  assertion, so it drops no protection, and its 18 rows would have added the whole guard analyzer's
  mutants to an already long run. `UnlinkAll_RemovesOnlyLinks` was retagged after phase 1, and it is
  tag-only too.
- **`DefaultAllocationVolume_Identifies…` was judged by hand**, as the brief directs: its random row names
  defeat Stryker. It is kept and retagged, and its red proof is the planted bug `volume-missing`.
- **The red proofs for tag-only rewrites are planted bugs, observed at base and again on the candidate.**
  Each bug breaks the behaviour the new tag names. Where a tag names two behaviours, each has its own bug.
- **The planted-bug builds ran with analyzers off**, because the planted branches exceed the cognitive
  complexity limit. Only the scratch checkout was built this way.

## Related

- [Test rubric](./rubric.md) - The verdicts and evidence this audit applies
- [DYD-1520 test audit](./_dyd-1520-test-audit.md) - The other batches
