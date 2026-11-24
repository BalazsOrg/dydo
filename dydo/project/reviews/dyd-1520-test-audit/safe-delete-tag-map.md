---
area: project
type: reference
---

# DYD-1520 Safe-Delete Tag Map

One line per job tag in the [safe-delete audit](./safe-delete.md): the tag's one behaviour, and the assertion line that
fails in every case the tag covers if that behaviour were false (review lesson 10).

Every tag has one behaviour and its cite. A contract quotes or closely paraphrases its cite's rule. A trap names its
pitfall and the ticket that records it. An edge names the input its case reaches. The assertions check more than the tag
claims; the tag claims only its one behaviour. Each rewritten case's red check, in its `.tsv` Evidence cell, breaks that
behaviour and fails on the line given here. Paths are under `DynaDocs.Tests/`, and line numbers are at the candidate.

## Feature file

`Features/safe-delete.feature` has no file-level header, because no rule holds for every scenario. A `# Job:` line sits
above each scenario whose rows all assert one rule (the captain's ruling, recorded in the audit). Lines are in
`Integration/SafeDeleteTests.cs` unless named.

| Scenario (rows) | Job line | Proving line |
|---|---|---|
| Recover files governed by repository-local exclude and attributes | contract: DYD-1133 - preserve ordinary Git behavior during restore | `Steps/SafeDeleteSteps.cs:44` (ignored status after restore) |
| Archive an unpushed worktree with proved remote absence `[0]`, `[1]` | contract: DYD-1029 - a restore command brings a branch or worktree back from the archive | `Steps/SafeDeleteSteps.cs:74` |
| Recover history and every worktree version independently `[1]`, `[5]`–`[8]` | contract: DYD-1029 - a restore command brings a branch or worktree back from the archive | `:149` for `[1]`, `[6]`, `[7]`; `:142` for `[5]`; `:90` for `[8]` |
| Refuse without losing the original target (24 rows) | trap: a refused safe delete that exits 0 (DYD-1029) | `:309`; current worktree `:221` |
| Validate archive policy through the command (20 rows) | none | keep `unsure`: its rows pin different DYD-1029 rules. Each row's protection is in the audit's unsure table |
| Raw destructive Git requires the archive command (18 rows) | contract: DYD-1029 - implement and prove the explicit raw-command refusal table | `Steps/SafeDeleteSteps.cs:105` |

## C# tags

| Tag (where) | Proving line |
|---|---|
| `SafeDeleteTests` (6 Facts): contract: DYD-1029 - a restore command brings a branch or worktree back from the archive | `Integration/SafeDeleteTests.cs:149` (local branch, unfetched remote, divergent); `:142` (partially staged, detached HEAD); `:188` (stash) |
| `InitializedNestedGitDirectory…`: contract: DYD-1029 - initialized nested repositories without complete independent preservation are refused | `Integration/SafeDeleteBoundaryTests.cs:45` |
| `NestedGitDirectory_ValidatesFileKinds…` (Linux): contract: DYD-1029 - unsupported filesystem objects are refused before any delete | `:116` |
| `RepositoryFragments…`: contract: DYD-1029 - restore recreates the working files | `:144` |
| `IndexConflictStages…`, `EveryResolveUndoStage…`: contract: DYD-1029 - restore recreates the index | `:437`; `:471` |
| `AdministrativeNameCasing…`, `BranchNameCasing…`: contract: DYD-1029 - case collisions must either round-trip correctly or cause a pre-delete refusal | Windows refusal `:161`, `:229`; round trip elsewhere `:169`, `:225`. The red check ran on the round trip; the Windows one is owed to a Windows run |
| `WorktreeFilemodeOverride…` (not Windows): contract: DYD-1029 - Git administrative metadata is preserved | `:192` |
| `RestoreWithChangedSharedSettings…`: contract: DYD-1029 - restore fails on unsupported metadata without overwriting user work | `:208`; the user's config bytes `:210` |
| `MalformedManifestFields…`, `CorruptManifestGitConfiguration…`, `UnsupportedInputManifest…`: contract: DYD-1029 - restore refuses an invalid manifest before it reconstructs into a new path/ref | `:278`; `:319`; `Integration/SafeDeleteInputsTests.cs:132`–`:133` |
| `PerWorktreeRefTip…`: contract: DYD-1029 - a tip named only by a per-worktree ref is recreated under refs/dydo-restored | `:251` |
| `AllocationVolumeUnavailable…`: edge: an allocation volume whose free space cannot be read | `:261` |
| `ResolveUndoListingDisagreeing…`: contract: DYD-1029 - restore fails on corrupt data | `:511` |
| `LinkedIncompleteEntry…`: contract: DYD-1029 - refuse unsafe paths | `:289`, the `list` exit |
| `ExplicitRemoteRestore…`: contract: DYD-1029 - the original same-name remote tip is the sole source for explicit --remote republishing | `:308` |
| `RestorePathCollision…`: contract: DYD-1029 - restore fails on existing paths without overwriting user work | `:332` |
| `FinalBranchRecheck…`: contract: DYD-1029 - branches checked out elsewhere are refused | `:341` |
| `FinalSnapshotRecheck…`: contract: DYD-1029 - a change detected in the source snapshot before destructive steps aborts | `:365` |
| `NativeBusyProbe…`: contract: DYD-1029 - in-use worktrees are refused | `:381` |
| `CompareAndSwap…`: edge: a ref that moves after the final query | `:402` |
| `PostFetchReserveShortfall…`: edge: a reserve shortfall that appears only after the fetch | `:416` |
| `ResolveUndoBlobMissing…`: contract: DYD-1029 - failed verification means nonzero exit with the original target retained | `:494`; `:495` |
| `DefaultAllocationVolume_Identifies…`: edge: the default space probe on a target that does not exist yet (6b46ce6b) | `:530` |
| `DefaultAllocationVolume_RefusesAnUnknownVolume` (Windows only): edge: the default space probe on an unknown Windows UNC share | `:538` |
| `WindowsStreamMetadata…` (Windows only): edge: a Windows file with an alternate data stream | `:553` |
| `InvalidReserveConfiguration…`: contract: DYD-1029 - invalid configuration refuses | `:565` |
| `CorruptProofPayload…`: contract: DYD-1029 - an empty bundle means nonzero exit with the original target retained | `:578` |
| `ChangedCommonInput…`: edge: a proof input that changes during the archive | `Integration/SafeDeleteInputsTests.cs:25` |
| `StatCleanEntryStale…`: edge: a stat-clean index entry the proof cannot reproduce | `:56` |
| `CorruptInputPayload…`: edge: an archived proof input corrupted during the archive | `:82` |
| `ChangedRestoreInputs…`: edge: proof inputs that change between the archive and its restore | `:104` |
| `CommonInputs…`: contract: DYD-1133 - preserve ordinary Git behavior during restore | `:192` |
| `LegacyBranchArchive…`: contract: DYD-1142 - v1 branch archives stay readable | `:165` |
| `OwnPullPagination…`: contract: DYD-1029 - eligibility follows the exact, paginated own-repository head/base associations | `Integration/SafeDeleteGitHubTests.cs:546` |
| `InitGeneratedCodexRules…`: trap: dydo init writing Codex rules without the archive prefix pattern (DYD-1029) | `Integration/SafeDeleteGuardTests.cs:65` |
| `UnsafePaths…`: contract: DYD-1029 - refuse unsafe paths | `Integration/SafeDeletePathsTests.cs:23` |
| `ColonPaths…`: contract: DYD-1134 - allow valid native Linux names while rejecting Windows drive/ADS syntax where unsafe | Linux `:41`; Windows `:39`. The red check ran on Linux; the Windows one is owed to a Windows run |
| `AuthoritativeT3Settings…`: contract: DYD-1029 - T3 settings are validated authoritatively before a target is assumed ordinary | `Integration/SafeDeletePolicyTests.cs:23` |
| `DeleteTree_LinkInside…`, `DeleteTree_RootThatIsALink…`, `UnlinkAll…`: contract: DYD-362 - the target must survive | `Services/LinkSafeDeleteTests.cs:20`, `:65`, `:34` |

These tags keep the form an earlier hop gave them, mapped the same way:

| Tag (where) | Proving line |
|---|---|
| `SafeDeleteGitHubTests` refusal methods: an unproved or changed remote eligibility state refuses remote cleanup | `Refusal` `Integration/SafeDeleteGitHubTests.cs:659`; `FinalPrePushChange` `:139`; `FinalMetadataRead` `:157`; `OwnDeletionReceipt` `:284`; `ProcessFailures…` `:571` |
| `PublicExactRef…`: a proved eligible state passes the remote eligibility check | `:19` |
| `InitiallyAbsentClosedWorld_CompleteOwn…`: a proved closed-world state passes the remote eligibility check | `:295` |
| `PrivateClosedWorld_RealCli…`, `PublicFork_Leased…`: on a proved eligible state the remote ref is deleted with the archived worktree | `Journey` `:39` |
| `InitiallyAbsentClosedWorld_ArchivesWithoutPush…`: a proved closed-world absence of the remote ref lets the local targets be deleted | `:311` |
| `AfterOwnDeletion_RecheckFailure…`, `EveryRemainingDestructiveStep…`, `AfterOwnDeletion_ApiRefSurvives…`: after the remote step, a failed recheck keeps the remaining local targets | `:195`; `:218`; `:260` |
| the five `InitiallyAbsent…` refusal methods: with the remote ref initially absent, an unproved or changed closed-world state keeps every local target | `:371`; `:389`; `:420`; `:465`; `:506` |
| `RawDeletionTable…`: safe-delete through the guard | `Integration/SafeDeleteGuardTests.cs:31` |
| `LinuxIgnoredTimestamp…` (Linux): trap: DYD-1134 (native Linux colon filenames) | `Integration/SafeDeletePathsTests.cs:57`; `:65` |
| `T3HistoryPathRecognition…`: a tree under a .t3/worktrees path is refused as T3-owned with no settings file | `Integration/SafeDeletePolicyTests.cs:33` |
| `Redaction…`: Redact drops a remote URL's userinfo and query | `:41` |

## Cites

Each contract's rule is quoted or closely paraphrased from its cite:

- **DYD-1029's description** states every DYD-1029 rule above, and its 2026-11-17 ruling states the pagination rule. Four
  rules are narrowed from longer sentences:
  - "restore recreates the working files" and "restore recreates the index" come from "recreates the working files and
    index".
  - "restore refuses an invalid manifest before it reconstructs into a new path/ref" comes from "Restore validates the
    manifest and hashes first, then reconstructs into a new path/ref". Several rows change only a field or the version,
    so the tag does not claim the hash check.
  - "restore fails on corrupt data" comes from "corrupt data … fail without overwriting user work". That case has no user
    work at the target.
  - "case collisions must either round-trip correctly or cause a pre-delete refusal" is its Windows and Linux rule.
- **DYD-1029's three other phrases:**
  - Its trap cites the "nonzero exit" it requires of failures.
  - The raw-command line quotes "Implement and prove the explicit raw-command refusal table below".
  - `LinkedIncompleteEntry` quotes "Do not follow symlinks/junctions in archive ancestors or entries; refuse unsafe paths".
    Its case pins the refusal: `list` exits 2 on the linked entry.
- **The other tickets and the commit:**
  - DYD-1133 says "preserve ordinary Git behavior during restore".
  - DYD-1134 says "allow valid native Linux names while retaining path traversal protection and rejecting Windows
    drive/ADS syntax where unsafe".
  - DYD-1142's record says "v1 branch archives stay readable".
  - DYD-362 says "the target must survive".
  - Commit `6b46ce6b` ("match root volumes") made the space probe handle a target that does not exist yet.

DYD-1029 asks for Codex rules that "admit the actual archive forms". `InitGeneratedCodexRules` reads the rule text, and
does not check whether a rule admits a form, so its tag is a trap, not that contract. No cite states the no-op or read-only
long-path deletion behaviour of `LinkSafeDelete`, so `DeleteTree_MissingRoot` and `DeleteTree_ReadOnly…` carry no tag and
are `unsure`.

## Related

- [Safe-delete audit](./safe-delete.md) - The batch this map belongs to
- [Test rubric](./rubric.md) - §2 row 2, the tag rule this map checks
