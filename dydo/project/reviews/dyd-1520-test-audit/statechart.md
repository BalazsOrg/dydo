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
| keep | 95 |
| rewrite | 128 |
| remove | 2 |
| keep `unsure` | 0 |
| **all** | **225** at base (110 methods); the candidate runs 223 cases in 117 methods |

Before, at `07642b1f`: 225 cases, 198 ran and 27 skipped, 14.4 s summed. After, on the candidate: 223 cases, 217
ran and 6 skipped (the Windows-only cases), 12.7 s summed. The 21 rows that read the pinned `tla2tools.jar` ran
here too: `TLA2TOOLS` named a local copy of it (SHA-256 `936a2620…0e88`, the payload lock's pin). No Java runs.

- **Removed (2):** `StatechartSkillTests.OnlyThisReleasesUneditedSkillValidates[0]` (the valid row) and
  `StatechartPosixChannelTests.AnExistingSocketDirectoryIsUsedOnlyWhenNoOneElseCanEnterIt[0]` (mode 700).
- **Restored after review round 1:** `StatechartDownloadsTests.StreamingBoundRejectsExcessWithoutWritingIt`,
  byte-identical to base. Its 32-byte first read against a 16-byte limit is an input no kept case drives (the
  HTTP case sends five bytes against four, which may arrive in short reads).
- **Rewritten (128), every one a tag rewrite:** no assertion and no input changed. Each case's base job tag
  claimed something its own assertions do not show (§2 row 2). Each new tag keeps only the clauses its lines
  prove (see the tag map below), and each case has a red proof that breaks the behaviour its tag names.
  Three Windows-only rewrites still owe that red check (see Open). Seven theories whose rows reach opposite
  outcomes are split by outcome into a fact or theory each, so one tag can be true of every row they cover.
- **Kept, not removed, after a candidate failed §5 (2):** `StatechartPrerequisitesTests.RuntimeCompatibilityPreservesExistingBoundaries[5]`
  (v26.13.0) and `StatechartManagedTests.AMissingStoreIsMissingEverything_AndIsNotCreated`. Planted bugs
  turn kept cases red, but only cases that run outside Windows, and both candidates run on Windows too.

Nothing here tests behaviour 4.0.0 drops, since the purge ruling keeps statechart.

## Reading the tags

Review round 1 FAILed tags that claimed more than their cases show. The fix reads every tag the same way:

- **A clause counts only when an assertion line of every case the tag covers fails if the clause is false.**
  A listed item is a clause of its own, and a theory's method tag must hold for each of its rows. Clauses no
  line proves are dropped; no assertion was added.
- **A bare section cite at base** (`contract: DR 062 §4 §5`) was read as true of a case that pins a sentence of
  a cited section, as round 1 passed them. Such a case stays `keep`, and its method tag now names only the
  sections its lines prove. A case that pins no cited section is a rewrite (doctor's decline outside Windows,
  a subcommand without its required option).
- **No class tag remains.** A class tag true of every case moved to each method unchanged.
- **Split theories:** the Skill inventory-less receipt, the 108/109-byte sun_path boundary, the existing
  socket directory (0500 used; 0750, 0702 and 01700 refused), the Checker bootstrap (sealed; unsealed), the
  Lifecycle owner loss (before a reservation; after it), the publication verdict (complete; not) and the
  publication destination (empty; foreign).

## Base inputs

Each split or moved case runs its base body unchanged, with its base row values. Only the method signature or
the data attribute moved. Bootstrap rows come from `Both("valid")` and `Both("missing", "changed", "unbuilt")`,
the same rows in the same pinned-then-stand-in order as base `Both("valid", "missing", "changed", "unbuilt")`.
The Skill inventory-less theory's two branches are unrolled verbatim. No input moved, so none needed a keyed
plant or a restore.

## Mutation

Targeted two-phase mutation (`/var/tmp/dyd1520/tools/stryker-targeted.sh`, Stryker 5.1.0):

- **Phase 1** on a clean `07642b1f` checkout, over the 17 methods that lose a case or are rewritten
  (40 executed cases): 321 killed, 984 survived, 10 timeout, 12,608 no coverage, 1,689 compile error.
  Phase 1 mutates the whole product, so most no-coverage mutants are outside statechart code. The rest
  sit in code that only the dydo child process runs, which Stryker cannot see. The planted bugs below
  cover the dropped protections there.
- **Phase 2 on the candidate** (`ecefb2d7`, the batch's 223 cases, run without `TLA2TOOLS` like phase 1):
  347 killed. Of the 321 phase-1 kills, only `PosixStatechartArguments.cs:22` (statement removal) is not Killed,
  and the comparison exits 1 on it. It times out: the lifecycle cases that spawn a process with the unwritten
  vector hang first. A timeout is not a kill, so the mutant was planted by hand as `no-argv-write` and the kept
  `StatechartArgumentsTests` run alone. The test host crashes with an `AccessViolationException` when
  `NativeArgumentVectorsPreserveUtf8AndTerminateWithNull` reads the vector, its only reader in that class.
  Phase 1's killer was that same case.
- The final commit after `ecefb2d7` changes only `Trait` attributes and the audit files. No assertion, input or
  production line changes, so phase 2 ran the final assertions against the final production code.
- Phase 1 and phase 2 cover only the 17 phase-1 methods' spans. The 128 tag rewrites get their red proofs from
  mutants of those runs where one breaks the tagged behaviour, and from planted bugs otherwise.

## Planted bugs

Round 2 planted every bug in two scratch checkouts of the candidate `ecefb2d7`, each gated by `DYD1520_PLANT`
and built with analyzers off, since only the scratch code trips them. The whole batch ran once per bug and once
with none; with none, 217 passed and 6 skipped. Four bugs ran one class alone, because their bug hangs other
classes' child processes or locks: `utf16`, `no-zero` and `no-argv-write` ran Arguments, and
`lock-ignore-cancel` ran Lock. The cases each bug turned red are in the `.tsv` Evidence cells. Each bug is
keyed to one tagged behaviour, and where a tag names several behaviours each has its own bug or mutant.

| Bugs | Change (one per name) |
|---|---|
| names, collisions, bounds, loops | entry names unchecked; duplicates, case collisions and file ancestors pass, a later entry overwriting; no entry or byte bound; a link loop resolves |
| adopt-staging, no-cancel-extract | an existing staging path is extracted into; cancellation is checked only after staging exists |
| utf16, no-zero, no-argv-write | the argument vector stores UTF-16; Dispose keeps its pointer (the host aborts on the double free, the observed result); the per-argument pointer write is removed (phase 2's timeout mutant; the host crashes) |
| lock-no-wait, lock-message, lock-ignore-cancel, held-missing | a held lock fails at once; the bound error omits the path; the wait ignores its token; a missing directory counts as held |
| write-first, overwrite, no-declared, no-status, file-before-status, no-integrity, delete-on-mismatch | a chunk is written before its bound check; the archive opens with `FileMode.Create`; the `Content-Length` check is skipped; the status check is skipped; the archive is created before the status check; the digest comparison is skipped; a failing archive is deleted |
| sun-overflow, sun-chars, sun-strict, sun-lenient | the CoreFxPipe path is kept at any length; sun_path counted in characters; the limit one byte short; one byte long |
| socket700, socket-500-refused, socket-mode-lenient, socket-owner, socket-link, socket-file | `Private` refuses the owner write bit; refuses its absence; ignores group, other and special bits; ignores the owner; accepts a link; accepts a file |
| checker-copy, checker-digest, checker-companion, checker-noresolve, checker-noverify, checker-adopt, checker-linked-lock, bootstrap-raw, bootstrap-fallback | an empty copy; no digest check; no companion check; links unresolved; `Verify` unchecked; the checker directory reused; a linked build lock read; raw TLC arguments; raw TLC when the helper does not match |
| cleanup-reserve-early, cleanup-exit0, cleanup-keep, cleanup-bad-nonce-exit0, cleanup-bad-nonce-litter | scratch reserved before the reserve frame; owner loss exits 0; the drain retains scratch; an invalid channel exits 0; an invalid channel creates a directory |
| publish-never, publish-always, publish-incomplete, publish-ignore-cancel, publish-refuse-empty, publish-clobber | nothing publishes; the verdict checks are skipped; an incomplete stage publishes; cancellation is ignored; an empty destination is refused; a foreign destination is deleted |
| skill-missing-current, skill-current-refused, m-16, m-18, foreign-overwrite, skill-no-replace, skill-bytes-unchecked, skill-edits-ignored | a receiptless skill validates; this release's skill is refused; `Validate` skips `Verify`; an unchanged earlier release is not refused; a foreign SKILL.md is overwritten before the refusal; `Place` keeps the old skill; inventory-less bytes go unchecked; recorded changes are ignored |
| no-repair, adopt-foreign, no-sweep, no-install-sweep, node26, missing-root-created, missing-root-ready | damaged assets returned unrepaired; the asset receipt check skipped; crash-left `.extract-*` kept; the install sweep skipped; Node 26 rejected; a missing store is created; a missing store reports `tlc` ready |
| doctor-exit0, doctor-extract-first, options-exit0, options-print, options-litter, java-process-cwd | doctor's decline exits 0; doctor extracts before its platform check; a missing required option exits 0, writes stdout, or creates a directory; a relative JAVA resolves against the process directory |

Round 1's bugs, as recorded at base. Round 2 re-ran all of them except m-124a/b/c, m-151 and m-152, which no
round 2 red proof relies on:

| Bug | Change | Kept cases red (and the dropped one) |
|---|---|---|
| write-first | `CopyBoundedAsync` writes a chunk before its bound check | `StatechartDownloadsTests.ABodyOverItsBoundIsRefusedWhetherDeclaredOrStreamed[1]` (dropped `StreamingBound…` also red) |
| skill-current-refused | `Validate` throws for this release's unedited skill | `StatechartSkillTests.AnUnchangedSkill…`, `…AnEarlierReceiptWithoutAnInventory…[0]`, `StatechartCommandTests.InstallSkillWithoutNode…`, `…InstallSkillPlacesAValidSkill…` (dropped `[0]` also red) |
| socket700 | `PosixStatechartSocket.Private` refuses a directory with the owner write bit | `StatechartPosixChannelTests.ALongTempDirFallsBack…`, `…AMissingSocketDirectoryIsCreatedPrivate`, `…AChildInheritingALongTempDir…`, `…ASocketPathFitsSunPath…`, `…TheCoreFxPipePathStaysWhileItFits[1]` (dropped `[0]` also red) |
| node26 | `Compatible` rejects Node major 26 | `StatechartPublicFlowTests.ADiscoveredRuntime…[0]`, `StatechartReadyTests.ReadinessProves…[0]` `[1]`, `StatechartManagedTests.AnApprovedInstall…`, `StatechartPublicFlowTests.AnApprovedResume_InstallsAndContinues…`: none runs on Windows, so row `[5]` stays |
| missing-root-created | reading a missing managed store creates its root | `StatechartPublicFlowTests.MissingPrerequisites…` (every platform) and 13 more |
| missing-root-ready | a missing managed store reports `tlc` ready | `StatechartPublicFlowTests.ADiscoveredRuntime…[0]` `[1]`, `…AnApprovedResume…`, `StatechartManagedTests.AnApprovedInstall…`: none runs on Windows, so `AMissingStore…` stays |
| no-repair | asset extraction returns a damaged target unrepaired | red proof: `StatechartAssetsTests.DamagedAssets…[0]`–`[3]` |
| adopt-foreign | asset extraction skips the receipt check and extracts over a foreign directory | red proof: `StatechartAssetsTests.ADirectoryWithoutThisPayloadsReceipt…[0]` `[1]` |
| no-sweep | asset extraction leaves crash-left `.extract-*` staging | red proof: `StatechartAssetsTests.AnExtractionACrashLeftBehind…` |
| m-16 | `Validate` skips `Verify` for a current receipt | `StatechartSkillTests.OnlyThisReleasesUneditedSkillValidates[3]` (source) |
| m-18 | `Validate` no longer refuses an unchanged earlier release | `…OnlyThisReleasesUneditedSkillValidates[1]` `[2]` `[5]` |
| m-124a/b/c | one `&&` of `Current` becomes `\|\|` | `…[2]` (a); `…[1]` `[2]` (b, c) |
| m-151, m-152 | `Verify` skips `VerifySkill`, or swallows its failure | `…[3]` (source) |
| utf16 | the argument vector stores UTF-16, not UTF-8 | red proof: `StatechartArgumentsTests.NativeArgumentVectorsPreserveUtf8AndTerminateWithNull` |
| no-zero | disposing the vector keeps its pointer | red proof: the test host aborts with a double free (`free(): double free detected in tcache 2`); that abort is the observed result |
| held-missing | a missing lock directory counts as a held lock | red proof: `StatechartLockTests.AMissingDirectoryIsAnErrorNotAWait`, after it waits out its 1-minute bound |

## Tag clauses

The clause-to-line map, every tag split into its clauses with the assertion line that proves each, is in
[statechart tag clauses](./statechart-tags.md); it would push this page past its size limit.

## Open

- **Three Windows-only rewrites owe their red check:**
  `StatechartLifecycleTests.IndependentCleanupDeletesRegisteredScratchOnlyAfterNativeOwnerSealsAndDrains`,
  `…AHelperThatDoesNotSpeakTheCleanupProtocolIsReleasedAndRejected` and
  `…ALifetimeOwnsAttachedChecksInOrderAndReportsALostHelper`. Their base tag (only authoritative drain
  authorizes cleanup) is a restriction their positive or empty cases do not show, so the tag was narrowed. The
  break-and-watch-red check needs Windows (`statechart-windows`), and this host is Linux.

## Deviations

- **Round 1's audit and apply steps were separate commits,** because a permission question held the apply step.
  No CI or `dotnet-platforms.json` line names a case this audit removes, renames or splits. The renamed and split
  methods are Linux or all-platform cases, and no helper is left without a caller.
- **The `.tsv` is generated:** `audit.py` reads each tag from the candidate's source, with red proofs from a JSON
  file written from the runs (both under `/var/tmp/dyd1520/scfix/`). Its output was copied into place byte for
  byte, not retyped.
- **Red proofs for tag-only rewrites** are a mutant or planted bug that the unchanged case fails on. That is the
  same break-and-watch-red check, automated, and the test bodies do not change.

## Related

- [Test rubric](./rubric.md) - The verdicts and evidence this audit applies
- [Statechart tag clauses](./statechart-tags.md) - Each tag's clauses and the assertion line proving each
- [DYD-1520 test audit](./_dyd-1520-test-audit.md) - The other batches
