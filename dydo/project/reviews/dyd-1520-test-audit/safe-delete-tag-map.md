---
area: project
type: reference
---

# DYD-1520 Safe-Delete Tag Map

One line per job tag in the [safe-delete audit](./safe-delete.md): the tag's one behaviour, and the assertion line that
proves it in every case the tag covers (review lesson 10).

Every tag this batch touched has the minimal form: its kind, one behaviour, and its cite. A contract states its rule in
the cite's own words. An edge names the input or condition its case reaches, here races and probe failures injected at
a checkpoint, which driving can't reach. A tag claims nothing beyond its one behaviour, and the assertions show what else
each case checks. Each rewritten case's red check, in its `.tsv` Evidence cell, breaks that one behaviour: a refusal's
exit, a restore, a recheck, or the rule the tag quotes. Paths are under `DynaDocs.Tests/`, and line numbers are at the
candidate.

## Feature file

`Features/safe-delete.feature` has one `# Job:` line: `contract: DYD-1029, DYD-1133 - each scenario row gets the outcome
these tickets set for its input`. Its rows reach different outcomes, so the one behaviour they share is that each row's
outcome is the one its ticket sets. The table names each row's outcome in the ticket's words, and the line that asserts it.
Lines are in `Integration/SafeDeleteTests.cs` unless named.

| Rows | Outcome the ticket sets | Proving line |
|---|---|---|
| exclude and attributes | DYD-1133: "preserve ordinary Git behavior during restore" | `Steps/SafeDeleteSteps.cs:44` |
| unpushed worktree `[0]`, `[1]` | DYD-1029: a proved closed-world absence passes; restore is local by default | `Steps/SafeDeleteSteps.cs:73` |
| recover history `[1]`, `[5]`–`[8]` | "A restore command brings a branch or worktree back from the archive" | `:133`; `[8]`: `:87` |
| refuse `[0]`–`[11]`, `[14]`–`[17]`, `[19]`–`[24]`, `[26]`, `[27]` | "nonzero exit with the original target retained" | `:309`; current worktree `:221` |
| policy `[0]`–`[6]`, `[17]` | restore failures "fail without overwriting user work" | `:395`; `[5]`: `:386` |
| policy `[7]` | "If a later step fails, retain the archive … and return nonzero" | `:361` |
| policy `[8]` | "retain for 30 calendar days" | `:446` |
| policy `[9]` | "list, restore, help and dry-run never purge" | `:450` |
| policy `[10]`–`[13]` | the reserve rule refuses | `:414` |
| policy `[14]`, `[15]` | "A dry run lists intent but neither fetches, creates an archive, deletes nor purges" | `:432` |
| policy `[16]` | "Do not follow symlinks/junctions in archive ancestors" | `:368` |
| policy `[18]` | symlink targets "round-trip correctly or cause a pre-delete refusal" | `:354`; on Windows `:348` |
| policy `[19]` | "a differently named upstream is reported as kept" | `:342` |
| raw destructive Git `[0]`–`[17]` | the raw-command table's verdict | `Steps/SafeDeleteSteps.cs:105` |

## C# tags

| Tag (where) | Proving line |
|---|---|
| `SafeDeleteTests`: contract: DYD-1029 - a restore command brings a branch or worktree back from the archive | `Integration/SafeDeleteTests.cs:133`; stash `:185` |
| `InitializedNestedGitDirectory…`: contract: DYD-1029 - initialized nested repositories without complete independent preservation are refused | `Integration/SafeDeleteBoundaryTests.cs:45` |
| `NestedGitDirectory_ValidatesFileKinds…` (Linux): contract: DYD-1029 - unsupported filesystem objects are refused before any delete | `:116` |
| `RepositoryFragments…`, `IndexConflictStages…`, `EveryResolveUndoStage…`: contract: DYD-1029 - restore recreates the working files and index | `:144`; `:437`; `:471` |
| `AdministrativeNameCasing…`, `BranchNameCasing…`: contract: DYD-1029 - case collisions round-trip correctly or cause a pre-delete refusal | `:169`, `:225`; on Windows `:161`, `:229` |
| `WorktreeFilemodeOverride…` (not Windows): contract: DYD-1029 - Git administrative metadata is preserved | `:192` |
| `RestoreWithChangedSharedSettings…`, `CorruptManifestGitConfiguration…`: contract: DYD-1029 - restore fails on unsupported metadata without overwriting user work | `:208`; `:319` |
| `PerWorktreeRefTip…`: contract: DYD-1029 - a tip named only by a per-worktree ref is recreated under refs/dydo-restored | `:251` |
| `AllocationVolumeUnavailable…`: edge: an allocation volume whose free space cannot be read | `:261` |
| `MalformedManifestFields…`, `UnsupportedInputManifest…`: contract: DYD-1029 - restore validates the manifest and hashes first | `:278`; `Integration/SafeDeleteInputsTests.cs:132` |
| `ResolveUndoListingDisagreeing…`: contract: DYD-1029 - restore fails on corrupt data without overwriting user work | `:511` |
| `LinkedIncompleteEntry…`: contract: DYD-1029 - archive entries are traversed without following links | `:289` (the sentinel) |
| `ExplicitRemoteRestore…`: contract: DYD-1029 - the original same-name remote tip is the sole source for explicit --remote republishing | `:308` |
| `RestorePathCollision…`: contract: DYD-1029 - restore fails on existing paths without overwriting user work | `:332` |
| `FinalBranchRecheck…`: contract: DYD-1029 - branches checked out elsewhere are refused | `:341` |
| `FinalSnapshotRecheck…`: contract: DYD-1029 - a change detected in the source snapshot before destructive steps aborts | `:365` |
| `NativeBusyProbe…`: contract: DYD-1029 - in-use worktrees are refused | `:381` |
| `CompareAndSwap…`: edge: a ref that moves after the final query | `:402` |
| `PostFetchReserveShortfall…`: edge: a reserve shortfall that appears only after the fetch | `:416` |
| `ResolveUndoBlobMissing…`: contract: DYD-1029 - failed verification means nonzero exit with the original target retained | `:494` |
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
| `AuthoritativeT3Settings…`: contract: DYD-1029 - T3 settings are validated authoritatively before a target is assumed ordinary | `Integration/SafeDeletePolicyTests.cs:23` |
| `DeleteTree_LinkInside…`, `DeleteTree_RootThatIsALink…`, `UnlinkAll…`: contract: DYD-362 - the target must survive | `Services/LinkSafeDeleteTests.cs:20`, `:65`, `:33` |

The tags this hop did not touch keep their earlier form, mapped the same way:

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
| `ColonPaths…`, `LinuxIgnoredTimestamp…`: native Linux colon filenames | `Integration/SafeDeletePathsTests.cs:41`; `:65` |
| `T3HistoryPathRecognition…`: a tree under a .t3/worktrees path is refused as T3-owned with no settings file | `Integration/SafeDeletePolicyTests.cs:33` |
| `Redaction…`: Redact drops a remote URL's userinfo and query | `:41` |

**Windows lines.** The case-collision tags and policy `[18]` are proved on Windows by a line only Windows runs. Their red
checks here cover the other platforms' line, and the Windows line's red check is owed to a Windows run, as for the two
Windows-only methods (coordinator ruling 2).

## Cites

Each contract's rule is quoted or closely paraphrased from its cite. DYD-1029's description states every DYD-1029 rule
above, and its 2026-11-17 ruling states the pagination rule. DYD-1133 says "preserve ordinary Git behavior during restore"
and "never delete on a failed reconstruction". DYD-1142 says "v1 branch archives stay readable". DYD-362 says "the target
must survive". Commit `6b46ce6b` ("match root volumes") made the space probe handle a target that does not exist yet.

DYD-1029 asks for Codex rules that "admit the actual archive forms". `InitGeneratedCodexRules` reads the rule text, and
does not check whether a rule admits a form, so its tag is a trap, not that contract. No cite states the no-op or read-only
long-path deletion behaviour of `LinkSafeDelete`, so `DeleteTree_MissingRoot` and `DeleteTree_ReadOnly…` carry no tag and
are `unsure`.

## Related

- [Safe-delete audit](./safe-delete.md) - The batch this map belongs to
- [Test rubric](./rubric.md) - §2 row 2, the tag rule this map checks
