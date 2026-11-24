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
| keep | 66 |
| rewrite | 243 |
| remove | 17 |
| keep `unsure` | 2 |
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
- **Rewritten (243): 242 tag corrections with the assertions unchanged, and 1 body rewrite.** Each tag
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
    and `UnlinkAll`.
  - The other 23 `SafeDeleteInputsTests` cases. Their class clause "refusal output complete" is not exhaustive,
    and DYD-1172 never states it. The class tag is now DYD-1133's inputs plus DYD-1029's refusal rule. Its
    red proof is `archive-exit0`, `restore-exit0` or `restore-noop`, per row.
  - The two `LinkSafeDeleteTests` link cases. "never its target" is not exhaustive, so the tag now says
    "keeps its target". Its red proof is `delete-follows`.
  - All 126 `SafeDeleteGitHubTests` rows. The class tag's one clause, "remote cleanup only on proved
    closed-world state", needs an exhaustive assertion for "only". DYD-1029 also makes public remotes
    eligible by the PR route, not by closed-world proof. In 71 rows (14 methods) no line proves the clause. The eligible cases show a proved state
    passing, not the refusal that "only" claims. The pagination rows show both verdicts. The
    post-deletion and initially-absent cases show local targets kept, with no remote ref left to clean
    up. Each method now carries the narrower tag mapped below. Its red proof is `gh-refuse-all`,
    `gh-accept-all` or `recheck-off`, as each row's Evidence names. The other 55 refusal rows carry the
    reworded class tag, "an unproved or changed remote eligibility state refuses remote cleanup". Their
    red proof is `gh-accept-all` or `recheck-off`.

  Where a class tag was true of only some methods, it moves to those methods unchanged, and they stay
  `keep`. The body rewrite is `SafeDeleteBoundaryTests.NestedGitDirectory_ValidatesFileKindsBeforeGitDiscovery`.
  It called `SafeDeleteFiles.Capture` directly, and two of its assertions could never fail (§2 rows 3
  and 5). It now drives `dydo archive worktree`. It asserts the hard-link refusal, the refused path, no
  nested-repository refusal, no archive, and the branch and tree bytes kept.
- **Candidates kept after §5 refused them:**
  - The 18 `Raw destructive Git…` rows. `SafeDeleteGuardTests` runs the same commands through the hook,
    but the step reads them in the Unknown shell dialect (both readings), and the hook rows read Posix or
    PowerShell, so a bug keyed to the dialect would turn no kept case red.
  - `LinkSafeDeleteTests.DeleteTree_MissingRoot_IsANoOp` and `DeleteTree_ReadOnlyFilesAndLongPaths…`, kept
    `unsure` (below). `missing-throws` turns no other batch case red.

No CI or `dotnet-platforms.json` line names a removed or renamed case. Every class that `safe-delete-windows`
requires still has cases. The removals leave no step definition unused: the 6 steps that only the removed
scenarios used are deleted. The helpers they had shared (`SafeDeleteGitHubTests.Journey`, `PrepareInitiallyAbsent`,
`SafeDeleteInputsTests.Prepare`, `SafeDeleteBoundaryTests.AssertNestedRepositoryRefused`) still have callers
in their own classes, and are now private.

## Tag clauses and the lines that prove them

Every tag in the batch, kept cases included, is split into clauses. Each clause is mapped to the assertion
line that fails if it were false, in [the tag map](./safe-delete-tag-map.md).

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
  `:203` (`||` to `&&`) and `:204` (the throw removed). Each disables the Linux file-kind refusal. Which kept case hung was
  not isolated; Stryker reports a Timeout without naming the test. At base the unit form of `NestedGitDirectory…`
  killed them quickly. A timeout is not a kill (review lesson 7). So each mutant was planted exactly
  (`DYD1520_PLANT=m100`, `m197`, `m203`, `m204`, on a scratch copy of `8d2bf92f`). The rewritten,
  kept `NestedGitDirectory…` case fails on each in about 0.1 s, on
  `Assert.Contains("Special files and hard links", …)`, and passes with none. Its output is
  "Initialized nested repository" instead. Results: `/var/tmp/dyd1520/sd/mut-timeouts/result.txt`.

Every mutant phase 1 killed is still killed by a kept case.

## Mutation method (coordinator ruling 1)

> The targeted two-phase run complies with §5 when both conditions hold: phase 1 covers every method that
> was removed or whose body changed; the diff changes no shared helper or fixture that an untouched test
> uses. […] If either condition fails, the batch re-runs phase 2 over every test that uses the changed
> helper, including untouched ones.

**Condition 1.** `git diff 07642b1f 271cd889 -- DynaDocs.Tests` removes rows from six scenario outlines and
changes one test method body. All of them are in phase 1:

| Removed or body-changed | Phase 1 chunk |
|---|---|
| `Recover files governed by repository-local exclude and attributes`, `Archive an unpushed worktree…`, `Authoritative remote eligibility…`, `Recover history…`, `Refuse without losing the original target`, `Validate archive policy through the command` | feature (the six outlines) |
| `SafeDeleteBoundaryTests.NestedGitDirectory_ValidatesFileKindsBeforeGitDiscovery` | rest |

The rest chunk also covered the retagged `SafeDeleteTests` Facts and these methods: `LegacyBranchArchive`,
`InitGeneratedCodexRules`, `UnsafePaths`, the three `SafeDeletePolicyTests` methods, `DeleteTree_ReadOnly…` and
`DeleteTree_MissingRoot…`. Every other changed method changes only its `Job` trait.

**Condition 2.** These are the changed non-test-method files and members under `DynaDocs.Tests/`:

| Changed | Change | Untouched tests that use it |
|---|---|---|
| `Integration/SafeDeleteTests.cs` `Refusal` | case labels for the removed rows `[12]`, `[13]`, `[25]`, `[28]` dropped | none: only the `Refuse…` outline calls it (phase 1) |
| `Integration/SafeDeleteTests.cs` `Policy` | the removed row `[20]`'s case dropped | none: only the `Validate archive policy…` outline (phase 1) |
| `Steps/SafeDeleteSteps.cs` | 6 steps, 6 fields and 2 constructors only the removed scenarios used are deleted; the `Refusal` step loses its shared-bare branch | `Raw destructive Git requires the archive command` (18 rows). The steps it uses (`Given`, `Guard`, `Verdict`, `Dispose`) are unchanged, but it uses the changed file, so phase 2 was rerun over it |
| `Features/safe-delete.feature` | the removed rows and scenarios, and the `# Job:` line | the comment only, for the kept rows |
| helper access modifiers (`Journey`, `PrepareInitiallyAbsent`, `Prepare`, `AssertNestedRepositoryRefused`) | narrowed, then restored to base in `0dff36ab` | none: identical to base |

**Rerun for `Raw destructive Git…`.** Phase 1 ran at base with the filter of that outline: 18 cases,
140 killed, 499 s. Phase 2 ran on `0dff36ab` with the batch filter: exit 0, all 140 still killed (145
killed after).

## Kept `unsure`

| Case | Protection | Unresolved |
|---|---|---|
| `LinkSafeDeleteTests.DeleteTree_MissingRoot_IsANoOp` | deleting an absent tree is a no-op. Planted bug `missing-throws` fails only this case | No spec, DR or ticket states the behaviour, so no tag can be cited. DYD-362 covers links only. It needs a citable source, or a kept case at a boundary that deletes an absent tree |
| `LinkSafeDeleteTests.DeleteTree_ReadOnlyFilesAndLongPaths_AreDeleted` | a read-only file deep in a path past 260 characters is deleted with its tree. `readonly-skip` fails it, and fails 290 other cases only in their fixture cleanup | No citable source. It needs one, or a kept assertion on the read-only long-path form |

Ruling 2 (Windows-only cases whose tag is narrowed become keep `unsure`) matches no case. The two
Windows-only methods, `DefaultAllocationVolume_RefusesAnUnknownVolume` and `WindowsStreamMetadata…`, keep the
class tag unchanged. The two Windows-listed Facts and `UnsafePaths` run on every platform with the same
assertions, so their red checks ran here.

## Cites

A helper read DYD-362, DYD-1029 (description, and its comments by keyword), DYD-1133, DYD-1134, DYD-1142,
DYD-1172 and spec N10 ("Spec: dydo 4.0.0"), and judged every clause against them. Spec N10 sets only the
scope: "archive a branch or worktree for 30 days, then purge", T3 included. DYD-1029 states the
archive, refusal, restore, retention, dry-run, upstream, guard, Codex-rule and T3 clauses in its
description. Six cites were only partly supported and were fixed:

- **Public remotes:** eligible by the PR route, not closed-world proof. The GitHub tags now say
  "eligibility state".
- **Redaction:** DYD-1029 promises userinfo and query, not fragment. The tag drops "fragment".
- **Version-1 archives:** only DYD-1142 says "v1 branch archives stay readable". That cite is added.
- **"refusal output complete":** DYD-1172 is a test-diagnostic change. The clause is replaced by
  DYD-1029's refusal rule.
- **Rooted and backslash paths:** only DYD-1142 states them. `UnsafePaths` adds DYD-1029 and DYD-1142.
- **The space probe:** DYD-1029 has the reserve rule only. The tag adds the root-volume fix commit
  `6b46ce6b`.

DYD-362 states only that a link's target survives, so the two cases above are `unsure`.

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
- [Safe-delete tag map](./safe-delete-tag-map.md) - Each tag clause and the line that proves it
- [DYD-1520 test audit](./_dyd-1520-test-audit.md) - The other batches
