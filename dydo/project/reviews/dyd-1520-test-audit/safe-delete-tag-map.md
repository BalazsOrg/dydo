---
area: project
type: reference
---

# DYD-1520 Safe-Delete Tag Map

The clause-to-line map for every job tag in the [safe-delete audit](./safe-delete.md) (review lesson 10).

Every job tag in the batch, kept cases included, split into its clauses. Each clause names the assertion
line, at the candidate, that fails in each case it covers if that clause were false. A tag that lists
behaviours separated by commas or semicolons (the feature `# Job:` line, `SafeDeleteBoundaryTests`,
`SafeDeleteInputsTests`) claims for each case only the clause mapped to it below. Each of the other tags
is one clause, and every case under it proves that clause. Clauses that no line proved were dropped
before the map was written:

- `UnlinkAll`: "removes each link" (no planted bug of its own) and "or a real file" (no source states it).
- `SafeDeleteTests`: "is deleted" (`UnfetchedRemote` asserts no deletion).
- `SafeDeleteGitHubTests`: "only" (no assertion is exhaustive), so every method carries a narrower tag.
- Every "only", "never" and "nothing" wording, with no exhaustive assertion behind it, is reworded to what
  the cases assert. This covers retention, `UnsafePaths`, `UnlinkAll`, the link cases, T3 settings and pagination.
- `Redaction`: "never reach archive output" (the case calls `Redact`, not the archive), and "fragment" (no
  source promises it).
- `DefaultAllocationVolume_Identifies…`: "finds the containing volume" (its fixture cannot tell the right volume
  from a wrong one).
- `SafeDeleteInputsTests`: "refusal output complete" (a substring is not exhaustive, and its cite DYD-1172 is a
  test-diagnostic change).

Paths are under `DynaDocs.Tests/`.

| Tag (where) | Clause | Cases and proving lines |
|---|---|---|
| feature `# Job:` (`Features/safe-delete.feature`) | an archived branch or worktree restores its unique state after deletion | journeys `[6]`, `[7]`: `Integration/SafeDeleteTests.cs:126` (deleted), `:133` (restore exit 0), `:149` (tip back), `:159` (remote absent or present as asked); `[8]`: `:86`, `:87`, `:90`–`:92` |
| | a refusal exits 2 and keeps the target | refusal rows: `SafeDeleteTests.cs:309` (exit 2), `:311`–`:318` (refs, tree, bytes); current worktree `:221`–`:224`; reserve rows `:414`; archive symlink `:368` |
| | a failure after the remote deletion exits 2 and keeps the complete archive and the local targets | policy `[7]`: `:361`, `:362` |
| | restore refuses a damaged or colliding archive and keeps it | policy `[0]`–`[4]`, `[6]`, `[17]`: `:395` (exit 2, entry kept), `:396` (no ref); `[5]`: `:386` |
| | a dry run prints its estimates and neither archives nor deletes | policy `[14]`, `[15]`: `:431`–`:434` |
| | an expired archive is purged by a later archive, not by list, dry-run or restore | policy `[8]`: `:446` (kept at 30 days), `:458` (purged by the next archive); `[9]`: `:450` (list), `:451` (dry-run), `:453` (restore) |
| | a differently named upstream is reported and kept | policy `[19]`: `:342`, `:343` |
| | links round-trip (Windows: refused) | policy `[18]`: `:353`, `:354`; on Windows `:348` |
| | the guard refuses raw destructive Git and admits dydo archive | guard rows `[0]`–`[17]`: `Steps/SafeDeleteSteps.cs:39` |
| `SafeDeleteTests` class | an archived branch or worktree restores its unique history and files after source gc | `SafeDeleteTests.cs:133` (restore exit 0) with `:142`–`:149` and `:154`–`:156` per journey; stash `:185`, `:188`–`:193` |
| `SafeDeleteBoundaryTests` (per method) | archive before delete: the archived state restores | `RepositoryFragments` `:143` (create-only restore, so it also proves the deletion), `:144`; `AdministrativeNameCasing` `:166`–`:169`; `WorktreeFilemode` `:190`–`:194`; `BranchNameCasing` `:224`, `:225`; `PerWorktreeRefTip` `:250`–`:252`; `ExplicitRemoteRestore` `:306`–`:309`; `IndexConflictStages` `:436`–`:438`; `EveryResolveUndoStage` `:470`–`:474` |
| | refuse unproven state | `InitializedNested…` `:45`–`:56`; `NestedGitDirectory…` `:116`–`:122`; `RestoreWithChangedSharedSettings` `:208`–`:211`; `AllocationVolumeUnavailable` `:261`, `:262`; `MalformedManifestFields` `:278`; `LinkedIncompleteEntry` `:289`, `:290`; `CorruptManifestGitConfiguration` `:319`; `RestorePathCollision` `:331`, `:332`; `FinalBranchRecheck` `:341`, `:342`; `FinalSnapshotRecheck` `:365`, `:366`; `NativeBusyProbe` `:381`–`:383`; `CompareAndSwap` `:402`–`:406`; `PostFetchReserveShortfall` `:416`–`:419`; `ResolveUndoBlobMissing…` `:494`–`:497`; `ResolveUndoListing…` `:511`–`:513`; `DefaultAllocationVolume_RefusesAnUnknownVolume` `:538`; `WindowsStreamMetadata` `:553`; `InvalidReserveConfiguration` `:565`, `:566`; `CorruptProofPayload` `:578`, `:579` |
| `DefaultAllocationVolume_Identifies…` (cite: DYD-1029's reserve rule and commit `6b46ce6b`, "match root volumes") | returns figures for a missing target, on the root volume and under the temp path, instead of throwing | `[0]`, `[1]`: `SafeDeleteBoundaryTests.cs:530` |
| `SafeDeleteGitHubTests` refusal methods (class tag) | an unproved or changed remote eligibility state refuses remote cleanup | `Refusal` helper `:659` (exit 2), `:670` (remote tip kept); `FinalPrePushChange` `:139`, `:140`→`:670`; `FinalMetadataRead` `:157`, `:159`→`:670`; `OwnDeletionReceipt` `:284`; `ProcessFailures…` `:571`–`:577` |
| `PublicExactRef…` | a proved eligible state (an exact public ref with no PR head or base, or a closed-world private one) passes the check | `:19` |
| `InitiallyAbsentClosedWorld_CompleteOwn…` | a proved closed-world state passes the remote eligibility check | `:295` |
| `PrivateClosedWorld_RealCli…`, `PublicFork_Leased…` | on a proved eligible state the remote ref is deleted with the archived worktree | `Journey` `:36` (exit 0), `:39` (remote ref gone) |
| `InitiallyAbsentClosedWorld_ArchivesWithoutPush…` | a proved closed-world absence of the remote ref lets the local targets be deleted | `:310`–`:312` |
| `OwnPullPagination…` | own-PR pagination refuses a late head or base, a repeated or missing cursor, a changing count, a duplicate node or the page limit, and accepts a complete listing | each row its own item: `:546` (verdict), `:549` (pages read) |
| `AfterOwnDeletion_RecheckFailure…`, `EveryRemainingDestructiveStep…`, `AfterOwnDeletion_ApiRefSurvives…` | after the remote step, a failed recheck keeps the remaining local targets | `:195`; `:218`, `:219`; `:260`, `:261` |
| the five `InitiallyAbsent…` refusal methods | with the remote ref initially absent, an unproved or changed closed-world state keeps every local target | `:371`; `:389`; `:420`, `:421`; `:465`, `:466`; `:506` (`AssertInitiallyAbsentLocalIntact` asserts at `:473`–`:481`) |
| `SafeDeleteInputsTests` (per method; cite DYD-1133 for the inputs, DYD-1029 for the refusal) | repository exclude and attribute inputs survive an archive and restore | `CommonInputs` `:187`, `:192`–`:197` |
| | a changed or corrupt input refuses with the target kept | `ChangedCommonInput` `:25`–`:30`; `StatCleanEntry…` `:56`, `:59`–`:62`; `CorruptInputPayload` `:82`–`:85`; `ChangedRestoreInputs` `:104`–`:110`; `UnsupportedInputManifest` `:132`–`:135`, `:139` |
| `LegacyBranchArchive_RemainsReadable` (cite DYD-1142: "v1 branch archives stay readable") | a version-1 branch archive still restores | `:165`, `:166` |
| `RawDeletionTable…` | safe-delete through the guard | `SafeDeleteGuardTests.cs:31` (denied, names dydo archive), `:34` (allowed), `:36` (the compound command denied) |
| `InitGeneratedCodexRules…` | dydo init's Codex rules admit every dydo archive form | `:65`, `:67` |
| `UnsafePaths…` (cite DYD-1029 traversal, DYD-1134 drives, DYD-1142 rooted and empty segments) | dot, empty, rooted, drive and backslash archive entry paths are refused | each row its own form: `SafeDeletePathsTests.cs:23` |
| `ColonPaths…`, `LinuxIgnoredTimestamp…` | native Linux colon filenames | `:41`; `:57`–`:66` |
| `AuthoritativeT3Settings…` | settings without a worktree root, or with an empty one, leave an ordinary tree unrefused | `[0]`, `[1]`: `SafeDeletePolicyTests.cs:23` |
| | invalid or unreadable settings refuse an ordinary tree | `[2]`–`[6]`: `:23`, `:24` |
| `T3HistoryPathRecognition…` | a `.t3/worktrees` path is refused as T3-owned with no settings file | `:33` |
| `Redaction…` (DYD-1029: "redact URL userinfo and credential-bearing query values") | `Redact` drops a remote URL's userinfo and query | `:41` |
| `LinkSafeDeleteTests` link cases (DYD-362: "the target must survive") | deleting a tree, or a root that is a link, removes the link and keeps its target | `DeleteTree_LinkInside` `Services/LinkSafeDeleteTests.cs:19`, `:20`; `RootThatIsALink` `:64`, `:65` |
| `UnlinkAll_RemovesOnlyLinks` | unlinking a tree's links leaves each link's target in place | `:34` |

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

## Related

- [Safe-delete audit](./safe-delete.md) - The batch this map belongs to
- [Test rubric](./rubric.md) - §2 row 2, the tag rule this map checks
