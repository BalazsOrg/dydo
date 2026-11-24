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
| keep | 200 |
| rewrite | 22 |
| remove | 3 |
| keep `unsure` | 0 |
| **all** | **225** (110 methods; 198 ran here, 27 skip on this Linux host) |

The 27 local skips are the six Windows-only cases and the 21 rows that read the pinned `tla2tools.jar`
from `TLA2TOOLS` (no Java runs). Both kinds run in CI (`statechart-windows`, `statechart-coverage`).

- **Removed (3):** `StatechartDownloadsTests.StreamingBoundRejectsExcessWithoutWritingIt`,
  `StatechartSkillTests.OnlyThisReleasesUneditedSkillValidates[0]` (the valid row) and
  `StatechartPosixChannelTests.AnExistingSocketDirectoryIsUsedOnlyWhenNoOneElseCanEnterIt[0]` (mode 700).
- **Rewritten (22), all as tag corrections with the assertions unchanged:** each case carried a class-level
  job tag that was untrue of it (§2 row 2). These are the three positive `StatechartArchiveTests` cases,
  the UTF-8 `StatechartArgumentsTests` case, all seven `StatechartAssetsTests` cases, the missing-directory
  `StatechartLockTests` case and the ten remaining `StatechartSkillTests` cases, whose class tag cited
  DR 062 §3 (the skill submits the continuation), which none of them asserts. Each replacement tag claims
  only what its case asserts, and the case's red proof breaks that exact behaviour. The Skill cases get one
  method tag each (validation, replacement, local edits, no-inventory receipts, foreign skills). Where a
  class tag was true only of some methods, it moves to those methods unchanged; they stay `keep`.
- **Kept, not removed, after a candidate failed §5 (2):** `StatechartPrerequisitesTests.RuntimeCompatibilityPreservesExistingBoundaries[5]`
  (v26.13.0) and `StatechartManagedTests.AMissingStoreIsMissingEverything_AndIsNotCreated`. Planted bugs
  turn kept cases red, but only cases that run outside Windows, and both candidates run on Windows too.

The area was already tagged and built around DR 062's clauses, and most theory rows each set a different
input the code reads (a mode bit, a version boundary, a platform's checksum, a link shape). So the rubric
supports few collapses. Nothing here tests behaviour 4.0.0 drops, since the purge ruling keeps statechart.

## Mutation

Targeted two-phase mutation (`/var/tmp/dyd1520/tools/stryker-targeted.sh`, Stryker 5.1.0):

- **Phase 1** on a clean `07642b1f` checkout, over the 17 methods that lose a case or are rewritten
  (40 executed cases): 321 killed, 984 survived, 10 timeout, 12,608 no coverage, 1,689 compile error.
  Phase 1 mutates the whole product, so most no-coverage mutants are outside statechart code. The rest
  sit in code that only the dydo child process runs, which Stryker cannot see. The planted bugs below
  cover the dropped protections there.
- **Phase 2 on the candidate** (`f2f854c9`, the batch's 222 cases): 347 killed, and of the 321 phase-1
  kills only `PosixStatechartArguments.cs:22` (statement removal) is not Killed. It times out, so it is
  still detected. Exit 1 lists just that one. The later tag-only commit changes no assertion.
- **Phase 2 on the base tree, first, while the apply step waited:** the batch filter minus the three
  methods that lose a case, each excluded whole. That is stricter than the candidate, which drops one row
  of two of them. Result: 321 killed before, 340 after; 8 killed before and not after:
  - `PosixStatechartArguments.cs:22` statement removal: Timeout after, so it is still detected.
  - Seven `StatechartSkill.cs` mutants (lines 16, 18, 124 ×3, 151, 152): lost only because the exclusion
    dropped the five kept rows of `OnlyThisReleasesUneditedSkillValidates` too. Each was planted by hand
    (below), and each turns a kept row of that method red, never only the dropped valid row.

  Both runs agree: every mutant phase 1 killed is still detected by a case the batch keeps.

## Planted bugs

Each bug was gated by `DYD1520_PLANT` in a scratch checkout at `07642b1f`. After each build, the cases
under test ran once per bug and once with none (all passed with none). There were three builds: the first
nine bugs and the m-* bugs each ran the whole batch, and the last three ran the Arguments and Lock
classes. The checkout was reverted afterwards and never committed.

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
| no-zero | disposing the vector keeps its pointer | red proof: the same case fails its pointer assertion, then its second dispose crashes the test host (double free) |
| held-missing | a missing lock directory counts as a held lock | red proof: `StatechartLockTests.AMissingDirectoryIsAnErrorNotAWait`, after it waits out its 1-minute bound |

Each rewrite's red proof breaks the behaviour its tag names, not a neighbouring one. Where a tag names
several behaviours, each has its own planted bug or phase-1 mutant (for example the JRE case: member
write 14248, mode 14369, hardlink copy 14394). The table names them per case.

## Deviations

- **The audit commit came before the apply step, which then waited.** A permission question (test-row
  deletions on this host) held the apply step until the captain cleared it. The removals and tag
  rewrites then landed in their own commit, as audited. No CI or `dotnet-platforms.json` line names a
  case this audit removes or renames, and no helper is left without a caller.
- **Phase 2 first ran on the base tree with an exclusion filter** (see Mutation), because the apply step
  was waiting. It was then repeated on the candidate.
- **The 21 pinned-jar rows had no local evidence** (no `TLA2TOOLS` provisioned here). All of them are kept.
- **Red proofs for tag-only rewrites** are a mutant or planted bug the unchanged case fails on, observed at
  base. That is the same break-and-watch-red check, automated, and the test bodies do not change.

## Related

- [Test rubric](./rubric.md) - The verdicts and evidence this audit applies
- [DYD-1520 test audit](./_dyd-1520-test-audit.md) - The other batches
