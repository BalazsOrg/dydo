---
area: project
type: reference
---

# DYD-1520 Statechart Audit

The statechart batch of the [DYD-1520](https://linear.app/bodnar-balazs/issue/DYD-1520) audit: every case
under `FullyQualifiedName~DynaDocs.Tests.Statechart.` at `07642b1f`, judged against the
[rubric](./rubric.md). The record is `statechart.tsv` in this folder, one line per case in §6's format.

## Totals

| Verdict | Cases |
|---|---|
| keep | 84 |
| rewrite | 133 |
| remove | 2 |
| keep `unsure` | 6 |
| **all** | **225** at base (110 methods); the candidate runs 223 cases in 117 methods |

Before, at `07642b1f`: 225 cases, 198 ran and 27 skipped, 14.4 s summed. After, on the candidate: 223 cases, 217
ran and 6 skipped (the Windows-only cases), 13.1 s summed. The after run sets `TLA2TOOLS` to a local copy of the
pinned `tla2tools.jar` (SHA-256 `936a2620…0e88`), so the 21 rows that read it run too. No Java runs.

- **Removed (2):** `StatechartSkillTests.OnlyThisReleasesUneditedSkillValidates[0]` (the valid row) and
  `StatechartPosixChannelTests.AnExistingSocketDirectoryIsUsedOnlyWhenNoOneElseCanEnterIt[0]` (mode 700).
- **Rewritten (133), every one a tag rewrite:** no assertion and no input changed. Each base tag claimed something
  its case does not show (§2 row 2). Each new tag keeps only the clauses its lines prove, and each clause has a red
  proof that breaks it. Seven theories whose rows reach opposite outcomes are split by outcome, so one tag can be true
  of every row it covers.
- **Kept `unsure` (6):** the six Windows-only cases, per coordinator ruling 2 (see Open).
- **Kept after a removal candidate failed §5 (2):** `StatechartPrerequisitesTests.RuntimeCompatibilityPreservesExistingBoundaries[5]`
  (v26.13.0) and `StatechartManagedTests.AMissingStoreIsMissingEverything_AndIsNotCreated`. Planted bugs turn kept
  cases red, but only cases that run outside Windows, and both run on Windows too.

Nothing here tests behaviour 4.0.0 drops, since the purge ruling keeps statechart.

## Review round 2

Round 2 FAILed tags and map clauses wider than the assertions that check them. This hop narrowed every tag in the
batch, not only the ones named. Test bodies and inputs are unchanged; only `Trait` attributes, comments and these
records changed.

- **Every clause is the observation:** "no link is left at its path" for "before the link is created", "writes
  nothing to stdout" for "prints nothing", "a host whose file name is dotnet" for "the same host". A clause names the
  call the case makes (`Verify`, `TlcArguments`, `Place`, `InstallAsync`), not the launch or command it feeds.
- **Every contract cite states its clause.** A contract cites DR 062, `dydo-commands-utilities.md` or the shipped
  `skills/engineering/statechart/references/runtime.md`. A rule no source states is a trap or an edge, with no cite.
  The DYD-548 and DYD-415 trap cites were dropped: DYD-548 is a coverage ticket, and no record here shows DYD-415
  describing the sweep or re-stamp trap.
- **Every rewrite clause has its own red proof.** Clauses no recorded bug breaks were dropped from the tag, and
  their assertions stay. One red proof was replaced: round 1's `skill-no-replace` failed in setup, at the first
  install, not at the replacement.
- **Base tags re-judged:** eight keeps whose base tag no assertion proves became rewrites with a red proof:
  `UnsafeNames…[1]` to `[4]` (the outside-path assertion reads only where `../escape` lands), `InstallSkillPlaces…`,
  `InstallSkillWithoutNode…`, `CallerRelativeJava…` and `AFrameworkDependentLaunch…`. The six Windows-only cases,
  three keeps and three rewrites, are now keep `unsure`.

The clause-to-line map, each contract's source sentence and each rewrite clause's red proof are in
[statechart tag clauses](./statechart-tags.md).

## Base inputs

Each split or moved case runs its base body unchanged, with its base row values. Only the method signature or
the data attribute moved. Bootstrap rows come from `Both("valid")` and `Both("missing", "changed", "unbuilt")`,
the same rows in the same pinned-then-stand-in order as base `Both("valid", "missing", "changed", "unbuilt")`.
The Skill inventory-less theory's two branches are unrolled verbatim. No input moved, so none needed a keyed
plant or a restore.

## Mutation method (coordinator ruling 1)

> **1. Option A.** The targeted two-phase run complies with §5 when both conditions hold:
> - phase 1 covers every method that was removed or whose body changed;
> - the diff changes no shared helper or fixture that an untouched test uses.
>
> Each batch PR states both conditions, with the diff evidence for the second. If either condition fails, the
> batch re-runs phase 2 over every test that uses the changed helper, including untouched ones.

Targeted two-phase mutation (`/var/tmp/dyd1520/tools/stryker-targeted.sh`, Stryker 5.1.0), phase 1 on a clean
`07642b1f` checkout:

- **Chunk 1, round 0 (17 methods, 40 cases, no `TLA2TOOLS`):** `StatechartDownloadsTests.StreamingBound…`,
  `StatechartManagedTests.AMissingStore…`, `StatechartSkillTests.OnlyThisReleasesUneditedSkillValidates`,
  `…AnUnchangedSkill…`, `…LocalEdits…`, `…AnEarlierReceiptWithoutAnInventoryIsReplacedOnlyWhenItsFilesAreThisRelease`,
  `…ForeignCanonicalSkill…`, `StatechartPrerequisitesTests.RuntimeCompatibility…`,
  `StatechartPosixChannelTests.AnExistingSocketDirectoryIsUsedOnlyWhenNoOneElseCanEnterIt`,
  `StatechartArgumentsTests.NativeArgumentVectorsPreserveUtf8…`, `StatechartArchiveTests.LockedZip…`, `…GzipTar…`,
  `…ALinkThroughAnother…`, the three `StatechartAssetsTests` methods and `StatechartLockTests.AMissingDirectory…`.
  321 killed.
- **Chunk 2, this hop (5 methods, 21 cases, `TLA2TOOLS` set, so the pinned rows ran):** the methods round 1 split
  after chunk 1 ran: `StatechartCheckerTests.BootstrapResolutionUsesSealedIdentityAndNeverFallsBackToRawTlc`,
  `StatechartLifecycleTests.OwnerLossBeforeOrDuringRegistration_NeverDeletesAmbiguousDirectories`,
  `StatechartPosixChannelTests.TheCoreFxPipePathStaysWhileItFits`,
  `StatechartPublicFlowTests.OnlyACompleteVerdictTheCheckerExitAgreesWithIsPublished` and
  `…PublicationReplacesOnlyAnEmptyPlainDestination_AndRetainsAnythingForeign`. 120 killed.

**Condition 1 holds.** Comparing every member of each changed file between `07642b1f` and the candidate (`Trait`
lines, comments and blank lines ignored) finds these base members removed or changed: the five chunk-2 methods,
chunk 1's `OnlyThisReleasesUneditedSkillValidates`, `AnExistingSocketDirectoryIsUsedOnlyWhenNoOneElseCanEnterIt`
and `AnEarlierReceiptWithoutAnInventoryIsReplacedOnlyWhenItsFilesAreThisRelease`, and the data member
`StatechartCheckerTests.Bootstraps`, used only by the bootstrap method. Every one is in chunk 1 or 2.

**Condition 2 holds.** The diff under `DynaDocs.Tests/` changes 16 files, all `*Tests.cs` test classes. No helper,
fixture or data file changes: `StatechartLeftovers`, `StatechartOfflineProcess`, `StatechartStandIns`,
`StatechartTestBinary`, `StatechartTestDirectory` and `dotnet-platforms.json` are unchanged. Inside the classes, the
new private helpers `Bootstrap`, `OwnerLoss`, `CoreFxPipePath`, `ExistingSocketDirectory`, `Publication` and
`Destination` and the data members `Sealed` and `Unsealed` have no caller but the split methods. Unchanged helpers
(the Skill class's `Installed`, `FromAnEarlierRelease` and `Restamp`, the Checker's `CopyInput` and `Both`) did not
change. No untouched test uses a changed helper.

**Phase 2** mutates phase 1's killed spans on the candidate and runs the batch's kept cases:

- Chunk 1 (round 1, on `ecefb2d7`, no `TLA2TOOLS`): 347 killed. Of the 321 phase-1 kills, only
  `PosixStatechartArguments.cs:22` (statement removal) is not Killed. It times out, which is no kill. Planted by hand
  as `no-argv-write`, it crashes the test host in the kept `NativeArgumentVectorsPreserveUtf8AndTerminateWithNull`.
- Chunk 2 (this hop, on `e3ab8651`, `TLA2TOOLS` set, 217 cases run and 6 skipped): 127 killed. Of the 120 phase-1
  kills, three are not Killed. They are `PosixStatechartSocket.cs:15` (the `CoreFxPipe_` prefix emptied) and `:16`
  (`<` turned to `>`, and the comparison negated), and all three time out. Planted by hand as `m-sun-name`,
  `m-sun-gt` and `m-sun-negate`, each turns kept cases red when the PosixChannel class runs alone (12 cases, all
  passing with no plant). `TheCoreFxPipePathStaysWhileItFits` fails on all three, and
  `TheCoreFxPipePathIsReplacedOneBytePastSunPath` also fails on `m-sun-negate`.

Both phase-2 runs ran on production code and test bodies identical to the final commit: `ecefb2d7` and `e3ab8651`
differ from it only in `Trait` attributes, comments, a `#region` label and these records.

## Removal replacements (lesson 11)

Each removed case ran alone under phase 1 at base, from a scratch copy of the base class holding only its row. Phase 2
then ran only the replacements named in its `.tsv` line:

- `OnlyThisReleasesUneditedSkillValidates[0]` killed 89 alone (65 in `StatechartAssets.cs`, 16 in
  `StatechartSkill.cs`, 8 in `StatechartPaths.cs`). `AnUnchangedSkill…`,
  `AnEarlierReceiptWithoutAnInventoryIsReplaced…` and `InstallSkillWithoutNode…` kill all 89 (compare exit 0).
- `AnExistingSocketDirectoryIsUsedOnlyWhenNoOneElseCanEnterIt[0]` killed 10 alone (8 in `PosixStatechartSocket.cs`,
  1 each in `PosixStatechartFiles.cs` and `PosixStatechartNative.cs`). `ALongTempDirFallsBack…`,
  `AMissingSocketDirectoryIsCreatedPrivate` and `AChildInheritingALongTempDir…` kill all 10 (compare exit 0).

## Other lesson 11 checks

- **Attribution:** no tag ties a finding to one document; plans and diagnostics are parsed as JSON, not searched.
- **"Only" and "nothing else":** no tag uses them. "Exactly" is claimed only where the assertion is exact equality.
- **Fixtures that cannot tell a clause from its nearest wrong behaviour drop the clause:** the second download
  writes the same bytes (L59), and an adopted staging, checker directory or store root would leave the foreign file
  as it was (Archive L178, Checker L177, Managed L92). The outside sentinel is unreachable from the second composed
  link (L137).
- **Timeouts:** every child-process case asserts its exit code exactly after an unbounded wait, so a timeout cannot
  pass as a success or failure. The lock cases name their exception exactly, so the bound's `IOException` fails them.
- **Failure paths:** a tag names the exception its case asserts (`Win32Exception` for a failed exec or exclusive
  create, `InvalidDataException` for a protocol refusal, `DirectoryNotFoundException`).

## Planted bugs

Round 2 planted these in a scratch checkout of `e3ab8651`, each gated by `DYD1520_PLANT`, built with analyzers off
(the `m-sun-*` results are under Mutation above):

| Bug | Change | Result |
|---|---|---|
| skill-keep-earlier | `Place` returns an unchanged earlier-release skill instead of replacing it | `AnUnchangedSkill…` and `AnEarlierReceiptWithoutAnInventoryIsReplaced…` fail at their `Validate` assertions; the other 8 Skill cases pass, as all 10 do with no plant |
| continuation-other-host | the continuation names `other-host` beside dotnet | `AFrameworkDependentLaunch…` fails at its host-name assertion; it passes with no plant |
| continuation-no-assembly | the continuation's assembly argument dropped | `AFrameworkDependentLaunch…` fails at its assembly assertion |
| m-sun-name, m-sun-gt, m-sun-negate | phase 2's three Timeout mutants in `PosixStatechartSocket.For` | kept channel cases fail on each, the class run alone |

Round 1 planted every other bug in two scratch checkouts of `ecefb2d7`, with the same gating. The whole batch ran once
per bug and once with none, which passed 217 and skipped 6. `utf16`, `no-zero` and `no-argv-write` ran Arguments
alone, and `lock-ignore-cancel` ran Lock alone, because those bugs hang other classes' child processes or locks.
Each `.tsv` Evidence cell names the bug that turns its case red and the assertion it fails at.

| Bugs | Change (one per name) |
|---|---|
| names, collisions, bounds, loops | entry names unchecked; duplicates, case collisions and file ancestors pass, a later entry overwriting; no entry or byte bound; a link cycle resolves |
| adopt-staging, no-cancel-extract | an existing staging path is extracted into; cancellation is checked only after staging exists |
| utf16, no-zero, no-argv-write | the vector stores UTF-16; Dispose keeps its pointer (the host aborts on the double free); the per-argument pointer write is removed (the host crashes) |
| lock-no-wait, lock-message, lock-ignore-cancel, held-missing | a held lock fails at once; the bound error omits the path; the wait ignores its token; a missing directory counts as held |
| write-first, overwrite, no-declared, no-status, file-before-status, no-integrity, delete-on-mismatch | a chunk is written before its bound check; the archive opens with `FileMode.Create`; the `Content-Length` check is skipped; the status check is skipped; the archive is created before the status check; the digest comparison is skipped; a failing archive is deleted |
| sun-overflow, sun-chars, sun-strict, sun-lenient | the CoreFxPipe path is kept at any length; sun_path counted in characters; the limit one byte short; one byte long |
| socket700, socket-500-refused, socket-mode-lenient, socket-owner, socket-link, socket-file | `Private` refuses the owner write bit; refuses its absence; ignores group, other and special bits; ignores the owner; accepts a link; accepts a file |
| checker-copy, checker-digest, checker-companion, checker-noresolve, checker-noverify, checker-adopt, checker-linked-lock, bootstrap-raw, bootstrap-fallback | an empty copy; no digest check; no companion check; links unresolved; `Verify` unchecked; the checker directory reused; a linked build lock read; raw TLC arguments; raw TLC when the helper does not match |
| cleanup-reserve-early, cleanup-exit0, cleanup-keep, cleanup-bad-nonce-exit0, cleanup-bad-nonce-litter | scratch reserved before the reserve frame; owner loss exits 0; the drain retains scratch; an invalid channel exits 0; an invalid channel creates a directory |
| publish-never, publish-always, publish-incomplete, publish-ignore-cancel, publish-refuse-empty, publish-clobber | nothing publishes; the verdict checks are skipped; an incomplete stage publishes; cancellation is ignored; an empty destination is refused; a foreign destination is deleted |
| skill-missing-current, skill-current-refused, m-16, m-18, foreign-overwrite, skill-bytes-unchecked, skill-edits-ignored | a receiptless skill validates; this release's skill is refused; `Validate` skips `Verify`; an unchanged earlier release is not refused; a foreign SKILL.md is overwritten before the refusal; inventory-less bytes go unchecked; recorded changes are ignored |
| no-repair, adopt-foreign, no-sweep, no-install-sweep, node26, missing-root-created, missing-root-ready | damaged assets returned unrepaired; the asset receipt check skipped; crash-left `.extract-*` kept; the install sweep skipped; Node 26 rejected; a missing store is created; a missing store reports `tlc` ready |
| doctor-exit0, doctor-extract-first, options-exit0, options-print, options-litter, java-process-cwd | doctor's decline exits 0; doctor extracts before its platform check; a missing required option exits 0, writes stdout, or creates a directory; a relative JAVA resolves against the process directory |

Round 0's removal evidence, unchanged: `write-first` also turns the restored `StreamingBound…` red;
`skill-current-refused` turns `AnUnchangedSkill…`, `AnEarlierReceiptWithoutAnInventoryIsReplaced…` and
`InstallSkillWithoutNode…` red (the removed valid row's protection); `socket700` turns `ALongTempDirFallsBack…`,
`AMissingSocketDirectoryIsCreatedPrivate` and `AChildInheritingALongTempDir…` red (the removed 0700 row's); `node26`
and `missing-root-ready` turn red only cases that skip on Windows, so `[5]` and `AMissingStore…` stay.

## Open

**Six Windows-only cases are kept `unsure`, "Windows-only; tag narrowed; red check owed to a Windows run"**
(coordinator ruling 2). Each tag was narrowed, and the break-and-watch-red check needs Windows: this host is Linux.
On CI the Windows leg of `dotnet-coverage` runs them, selected by `DynaDocs.Tests/coverage/dotnet-platforms.json`
`windows_tests`; `statechart-windows` runs the skill's TypeScript Windows tests, not these.

- `StatechartCommandTests.InstalledDoctorExtractsPayloadAndNeverClaimsChartReadinessOrInstallsPrerequisites`: on
  fresh roots, doctor exits 2, reports ready false and creates no dependency root.
- `StatechartCommandTests.InstalledDoctorNamesMissingRuntimesWithoutProbingThem`: with no runtime on PATH, doctor
  exits 2, reports ready false, and reports node and java as null.
- `StatechartLifecycleTests.IndependentCleanupDeletesRegisteredScratchOnlyAfterNativeOwnerSealsAndDrains`: after
  `CompleteAsync` returns true, the stage and temp directories are gone.
- `StatechartLifecycleTests.OwnerEofRetainsRegisteredScratchWithDurableUncertaintyReason`: after the owner's EOF with
  no drain, the stage and a scratch journal remain.
- `StatechartLifecycleTests.AHelperThatDoesNotSpeakTheCleanupProtocolIsReleasedAndRejected`: a non-protocol helper
  gives `InvalidDataException`, an unstartable one `Win32Exception`, and the roots' directory stays empty.
- `StatechartLifecycleTests.ALifetimeOwnsAttachedChecksInOrderAndReportsALostHelper`: the attached wait returns with
  the check exited, completion cleans the stage, and a later unattached wait throws `IOException`.

## Deviations

- **Round 1's audit and apply steps were separate commits,** because a permission question held the apply step.
  No CI or `dotnet-platforms.json` line names a case this audit removes, renames or splits.
- **The `.tsv` is generated:** `audit2.py` reads each tag from the candidate's source, with red proofs from a JSON file
  written from the runs (both under `/var/tmp/dyd1520/scfix2/`). Its output was copied into place byte for byte.
- **Red proofs for tag-only rewrites** are a mutant or planted bug that the unchanged case fails on: the same
  break-and-watch-red check, automated, with the test bodies unchanged.

## Related

- [Test rubric](./rubric.md) - The verdicts and evidence this audit applies
- [Statechart tag clauses](./statechart-tags.md) - Each tag's clauses, sources and red proofs
- [DYD-1520 test audit](./_dyd-1520-test-audit.md) - The other batches
