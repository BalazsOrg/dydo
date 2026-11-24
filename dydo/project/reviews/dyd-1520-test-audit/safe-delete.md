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
| keep | 217 |
| rewrite | 94 |
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
- **Rewritten (94): 93 tag corrections with the assertions unchanged, and 1 body rewrite.** Each tag
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

## Mutation

Targeted two-phase mutation (`/var/tmp/dyd1520/tools/stryker-targeted.sh`, Stryker 5.1.0). Phase 1 ran on a
clean `07642b1f` checkout in two chunks:

- **feature**: the six scenario outlines that lose a row or a tag (64 cases). 1,097 killed, 1,405 survived,
  8 timeout, 2,850 s.
- **rest**: the six `SafeDeleteTests` Facts and the other C# methods rewritten or retagged (30 cases).
  878 killed, 1,557 survived, 2 timeout, 1,657 s.

Two exclusions, both deviations below: the `Raw destructive Git…` outline, which is tag-only and loses no
row, and `DefaultAllocationVolume_Identifies…`, which Stryker cannot run.

Phase 2 runs on the candidate once the removals and rewrites are committed; its result is recorded here.

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
