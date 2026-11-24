---
area: project
type: reference
---

# DYD-1520 Statechart Tag Clauses

Every job tag in the [statechart batch](./statechart.md), split into its clauses. Each clause names the assertion
line that fails if the clause is false, in every case the tag covers; a theory's line runs once per row. Lines are
in `DynaDocs.Tests/Statechart/<Class>.cs` on the candidate; through a helper, the method's line comes first, then
the helper's (`→`). Assertions no clause names stay, unclaimed. For a rewrite, `red` names the bug or mutant that
breaks each clause and turns the case red (its record is the `.tsv` Evidence cell). A keep needs none; a keep
`unsure` owes one.

## How the tags read

- **A clause claims the observation, not more.** "No link is left at its path", not "before the link is created";
  "writes nothing to stdout", not "prints nothing"; "a host whose file name is dotnet", not "the same host".
  Exception types and messages are named as asserted.
- **A clause names the call the case makes:** `Verify`, `TlcArguments`, `Place`, `Validate`, `InstallAsync`, `Read`,
  `Private` or `CompleteAsync`, not the launch or command that call feeds.
- **A contract cites a source that states its rule**, and its parenthetical is the case's instance of that rule. A
  tag whose rule no source states is a trap or an edge, with no cite. The DYD-548 and DYD-415 trap cites were
  dropped: DYD-548 is a coverage ticket, and no record here shows DYD-415 describing the sweep or re-stamp trap.

## Sources

`runtime.md` is `skills/engineering/statechart/references/runtime.md`; `utilities` is `dydo-commands-utilities.md`.

| Cite | What it states, verbatim |
|---|---|
| DR 062 §1 | "A ready invocation runs offline with verified compatible dependencies and no repeated installation or consent. Explicit invalid overrides are errors." |
| DR 062 §2 | "Missing dependencies produce a precise locked plan, structured event and saved original request, exit3." |
| DR 062 §4 | "An authorized continuation verifies immutable request/payload/lock identities, installs missing locked assets into owned roots, proves readiness, publishes its receipt atomically and continues the saved inputs." |
| DR 062 §5 | "Denial, unavailable authorization, installation failure and cancellation remain non-success and preserve the request." |
| DR 062 §6 | "Checking runs under native statechart ownership, with registered scratch and startup gates. Only authoritative drain authorizes registered-owned cleanup; uncertainty retains scratch and a durable reason. A notification alone does not establish drain." |
| DR 062 Consequences | "An update that keeps the lock re-verifies the existing install"; "Content that fails verification yields an approval-required repair plan ...; nothing managed is replaced without it."; "`install-skill` replaces an installed skill unchanged since its install"; "extracted assets that fail verification are extracted again offline from the embedded payload" |
| utilities `dydo statechart` | "Missing prerequisites produce a locked JSON plan and saved request with exit `3`; the initial invocation starts no installer." |
| utilities `resume` | "Failures preserve the pending request; changed inputs require a fresh plan. Completed requests return the recorded verdict only while their output still matches." |
| utilities `install-skill` | "Installs this executable's embedded statechart skill into the explicitly named project"; "a skill whose files still match that record is replaced with this release's. A receipt without that record is replaced only when its files are already this release's. Foreign skills, local edits ... stay untouched, and the refusal names the changed files"; "`--skill DIR` ... verifies the installed skill's exact release, protocol, payload and file identities ...; an unchanged skill from an earlier release is refused" |
| utilities `doctor` | "Diagnostic JSON distinguishes candidate compatibility from verified checking readiness. Exit `2` means checking readiness is not established"; "This operation changes no PATH, home settings or managed prerequisites." |
| runtime.md Prerequisites | "It accepts an existing Node 22.18–22.x or 23.6+ and Java 11+"; "Path-valued overrides resolve from the caller's directory" |
| runtime.md Checking transport | "TLC runs through the loopback RMI bootstrap in `scripts/java/`, whose sealed identity is `scripts/java/build-lock.json`." |

## Archive, Arguments, Assets

- `LockedZip…` edge: members at their paths with their text L19, L20; red 14237.
- `GzipTar…` edge: file at its path with its text L46; hardlink's text L47; mode outside Windows L49; red 14248, 14394, 14369 in that order.
- `UnsafeNames…` trap: refused L68; red `names` (rows 1-8; row 0 is a keep).
- `DuplicateCase…` trap: refused L82; red `collisions`.
- `ExpandedBytes…` trap: refused L95; second member absent afterwards L98; red `bounds` (no bound: no refusal, and the second member is written).
- `EscapingTarLinks…` trap (keep): refused L115; no link at its path L116.
- `ComposedLinks…` trap (keep): refused L134; neither link at its path L135, L136. L137 reads `outside`, which only row 0 reaches, so no clause claims it.
- `LinksThatLoop…` trap: refused L149; no link at the first one's path L150; red `loops` (the cycle resolves, so no refusal and both links are created).
- `ALinkThroughAnother…` edge: extracts L162 (no throw); reads the inside file's text L163; red 14401, 14383.
- `ForeignStaging…` trap: existing staging refused L177, red `adopt-staging`; cancelled run leaves no directory at its staging path L183, red `no-cancel-extract`.
- `NativeArgumentVectorsRejectTruncatingNul…` trap (keep): building the vector refuses a NUL argument L12.
- `NativeArgumentVectorsPreserveUtf8…` edge: non-ASCII as UTF-8 L20, red `utf16`; empty argument L21, red `no-argv-write` (host crash on read); NULL end L22, red 13580; 0 once disposed L24, red `no-zero` (double-free abort).
- `DamagedAssets…` contract, DR 062 Consequences: in the offline run (exit 3 L31), extracted again: `Verify` passes L33, the helper's and setup script's bytes are back L34, L35; red `no-repair` (L33, all rows).
- `ADirectoryWithoutThisPayloadsReceipt…` trap: the run exits 2 L53; the file in the root keeps its text L55; red `adopt-foreign` (the root is taken for a damaged extraction and extracted over).
- `AnExtractionACrashLeftBehind…` trap: the crash-left staging is gone after the next run L72; red `no-sweep`.

## Checker

- `OwnedCopy…` edge: after preparation `Verify` returns a copy with the input's exact bytes L57 → L58, L59; red `checker-copy` (preparation's own `Verify` refuses the empty copy at L57).
- `AnInputOther…` trap: preparation refuses L79; no `checker` directory afterwards L81; red `checker-digest` (no refusal, and preparation goes on to create it).
- `Companion…` trap: refused by explicit discovery L107 and by preparation L110; no `checker` directory afterwards L111; red `checker-companion` (both calls share the check; the copy is then made).
- `Aliased…` trap: each link resolves to the jar's own path L130, red `checker-noresolve`; refused once a companion is beside it L133, red `checker-companion`.
- `EveryLaunch…` trap: the next `Verify` refuses after each row's change L161; red `checker-noverify` (all sixteen rows).
- `ExistingChecker…` trap: preparation refuses with a `Win32Exception` L176; red `checker-adopt` (an `IOException` instead).
- `Bootstrap…` contract, runtime.md Checking transport: `TlcArguments` is the helper and owned copy on the classpath with `LoopbackTlc` L183 → L210; red `bootstrap-raw`.
- `AnUnsealedBootstrap…` trap: `TlcArguments` throws for a missing, changed or unbuilt helper L188 → L209; red `bootstrap-fallback`.
- `ALinkedBootstrapBuildLock…` trap: `TlcArguments` refuses a linked build lock with its exact message L225; red `checker-linked-lock`.

## Command, Downloads, Inventory

- `InstalledDoctorExtracts…` (keep `unsure`, Windows) contract, utilities doctor: exits 2 L34; ready false L36; no dependency root L38.
- `InstalledDoctorNames…` (keep `unsure`, Windows) contract, utilities doctor: exits 2 L50; ready false L52; node and java null L54.
- `DoctorDeclines…` trap: exits 2 L66, red `doctor-exit0`; the directory holding its roots is empty L67, red `doctor-extract-first`.
- `InstallSkillPlaces…` contract, utilities install-skill: the placed skill validates L88; red `skill-current-refused` (after exit 0 at L85).
- `InstallSkillWithoutNode…` trap: the placed skill validates L101 after the exit-2 failure L99; red `skill-current-refused`.
- `ASubcommand…` trap: exits 2 L112, red `options-exit0`; nothing on stdout L113, red `options-print`; working directory empty L114, red `options-litter`.
- `LosingTheNpmLifetimeCancels…` contract (keep), DR 062 §5: non-success, exit 130 L136.
- `Select…` edge: names L14; limits L15, L16; pele `sha512-` L17; unknown RID refused L18; red 14714 (the row match), 14717 (the refusal).
- `EveryPlatformPinsManagedNode26` edge: URL L33; SHA-256 L34; red 14714.
- `StreamingBound…` edge: refused L43, red 14763, 14764; output empty L44, red `write-first`.
- `ADownloadWrites…` edge: the body's bytes L55 and `Verify` passes L56, red 14758 (an empty archive); a download to an existing archive path refused L58, red `overwrite`. L59 reads bytes the second fetch would also write, so no clause claims it.
- `ABodyOverItsBound…` edge: none of the body at the archive path L73, L74; red `no-declared` (row 0), `write-first` (row 1).
- `AFailedResponse…` edge: `HttpRequestException` L84, red `no-status`; no file at the archive path L85, red `file-before-status`.
- `Integrity…` trap: fails verification L129, red `no-integrity`; a file still at its path L132, red `delete-on-mismatch`.
- `AnInstalledLinkWhoseFollowedPath…` trap (keep): rejected L24.
- `InstalledLinksThatLoopOrStayInside…` edge: the inside link recorded with its target L40, red 14969; the loop rejected L44, red `loops`.

## Lifecycle and Lock

- `IndependentCleanup…` (keep `unsure`, Windows) contract, DR 062 §6: `CompleteAsync` true L51; stage and temp gone L52, L53.
- `OwnerEof…` (keep `unsure`, Windows) contract, DR 062 §6: the stage exists L70; a scratch journal L71.
- `AHelperThatDoesNotSpeak…` (keep `unsure`, Windows) trap: `InvalidDataException` L84; `Win32Exception` L86; the directory holding its roots empty after both L88.
- `ALifetimeOwns…` (keep `unsure`, Windows) trap: the wait returns L104 with the check exited L105; `CompleteAsync` true L107; stage gone L108; after completion, the unattached wait throws `IOException` L111.
- `OwnerLossBeforeAnyReservation…` trap: exits 2 L124 → L143, red `cleanup-exit0`; no scratch directory L144, red `cleanup-reserve-early`.
- `OwnerLossBeforeOrDuringRegistration…` contract (keep), DR 062 §6: two scratch directories left L128 → L144.
- `ADrainedSuccess…` contract, DR 062 §6: no scratch directory L161; red `cleanup-keep`.
- `AForgedDrainOrALostOwner…` contract (keep), DR 062 §6: stage and temp exist L178, L179.
- `AReplacedScratchPath…` contract (keep), DR 062 §6: on Linux, the replacement's file L202 and the moved stage L203. The Windows branch's lines are unclaimed.
- `AHelperWithoutAValidChannelNonce…` trap: exits 2 L234, red `cleanup-bad-nonce-exit0`; directory empty L235, red `cleanup-bad-nonce-litter`.
- `AWaiter…` edge: not completed while held L15; acquires once released L17; red `lock-no-wait` (it faults at once).
- `AHolderPastTheBound…` edge: the `IOException` names its path L29; red `lock-message`.
- `CancellationEndsTheWait` edge: a cancellation L42; red `lock-ignore-cancel`.
- `AMissingDirectory…` edge: `DirectoryNotFoundException` L50; red `held-missing`.

## Managed

All keeps but the sweep.

- `AnApprovedInstall…` contract, DR 062 §4: every component ready L34, L35; the checker is the served jar's bytes L36.
- `AServedArchive…` contract, DR 062 §4 §5: the install throws L61; every component still missing L63.
- `AMissingStore…` contract, DR 062 §4: every component missing L73.
- `AForeignMarker…` contract, DR 062 §4: `Read` refuses L91.
- `ContentThatFailsVerification…` contract, DR 062 §4 and Consequences: not ready L112; the jar keeps its bytes L113.
- `AComponentWithoutAReadinessReceipt…` contract, DR 062 §4 and Consequences: not ready L126; the file keeps its text L127.
- `VerifiedContent…` contract, DR 062 Consequences: ready at its path L136.
- `AManagedRuntimeThatCannotExecute…` contract, DR 062 §4: null or its path, by row L152.
- `CancellingAWaitingInstall…` contract, DR 062 §5: a cancellation L169.
- `CrashLeftStages…` trap: stage and receipt temp file gone L194, L195; red `no-install-sweep` (both rows).
- `ARestamp…` trap: returns false L219; the holder's receipt as written L220.

## POSIX channel and lifecycle

- `AChildInheritingALongTempDir…` trap: the child connects L32 (accept), L34 (its anchor frame); red `sun-overflow` (no socket to connect to).
- `ASocketPathFitsSunPath…` trap: fits in bytes L49; red `sun-chars`.
- `TheCoreFxPipePathStays…` edge: kept at 108 bytes L55 → L68; red `sun-strict`.
- `TheCoreFxPipePathIsReplaced…` edge: replaced at 109 bytes L59 → L68; red `sun-lenient`.
- `ALongTempDirFallsBack…` trap: the path under /tmp named for the effective user L79; red `sun-overflow`.
- `AMissingSocketDirectory…` edge: `Private` accepts it L92; red `socket700`.
- `AnExistingSocketDirectoryOnlyItsOwnerCanEnter…` edge: used L99 → L114; red `socket-500-refused`.
- `AnExistingSocketDirectoryIsUsedOnlyWhen…` trap: refused L106 → L115 → L146; red `socket-mode-lenient`.
- `ASocketDirectoryOwnedBySomeoneElse…` trap: refused L126 → L146; red `socket-owner`.
- `ALinkOrFile…` trap: link refused L137 → L146, red `socket-link`; file refused L141 → L146, red `socket-file`.
- `ARegisteredLifetime…` contract (keep), DR 062 §6: `CompleteAsync` true L26; no scratch directory L27.
- `AConnectedChannel…` trap (keep): no file at the CoreFxPipe path L42.
- `ALostParent…` trap (keep): no stage L64 or temp L65 after disposal.
- `AFailedHandoff…` contract (keep), DR 062 §6: after the `Win32Exception` L80, `CompleteAsync` true L81; no stage or temp L82, L83.
- `AContinuedHandoff…` contract (keep), DR 062 §5 §6: no verdict L106; stage gone L107. Trap: a child started after the handoff holds the descriptor L104.
- `AContinuationOutside…` contract (keep), DR 062 §4 §6: `InvalidDataException` L123; stage and temp gone L125, L126.
- `AHandedOff…` trap (keep): no verdict after the drain L168; the verdict once the result frame arrives L170; exits 0 L172; no stage or temp L173, L174.
- `AnUnproven…` trap (keep): no verdict L199; exits 2 L201; stage gone L202.
- `OnlyACheckingChild…` trap (keep): exit 137 L223.
- `AMalformed…` trap (keep): exits 2 L243; stage gone L244.
- `LosingTheNpmLifetimeStops…` contract (keep), DR 062 §5 §6: exit 2 L256; no stage or temp L257, L258.
- `APrivateMode…` trap (keep): exits 2 L274; nothing on stdout L276; directory empty L277.

## Prerequisites, public flow, Ready, Skill

- `RuntimeCompatibility…` contract (keep), runtime.md Prerequisites: each row's judgement L21.
- `InvalidExplicitOverrides…` contract (keep), DR 062 §1: missing JAVA throws L29; failing TLA2TOOLS throws L32.
- `CallerRelativeJava…` contract, runtime.md Prerequisites: `./java` resolves in the caller's directory L44; red `java-process-cwd`.
- `MissingPrerequisites…` contract (keep), DR 062 §2 and utilities: exit 3 L23; request saved L24; no dependency root L25.
- `ChangedSavedInputs…` contract (keep), utilities resume: fails, exit 2 L39; request file kept L40; no dependency root L41.
- `AnInvalidExplicitJava…` contract (keep), DR 062 §1: exit 2 L64; nothing on stdout L65; no dependency root L66; no requests directory L67.
- `AnIdeJava…` trap (keep): java in the plan's missing list L101.
- `ADiscoveredRuntime…` contract (keep), DR 062 §2 and runtime.md: the exact missing list per row L117.
- `ADamagedManagedComponent…` contract (keep), DR 062 Consequences: tlc in the plan L135; its file keeps its text L136.
- `ResumingACompletedRequest…` contract (keep), utilities resume and DR 062 §1: exits with verdict 1 L152; no dependency root L153.
- `AnApprovedResumeWithoutNetwork…` contract (keep), DR 062 §4 §5: non-success, exit 2 L165; request file kept L166; every component missing L168.
- `AFrameworkDependentLaunch…` trap: host file name `dotnet` L190, red `continuation-other-host`; this dydo's assembly next L191, red `continuation-no-assembly`.
- `AnApprovedResume_InstallsAndContinues…` contract (keep), DR 062 §4 and Consequences: runs the saved request, exit 1 L225, verdict recorded L226; every component ready L227; later runs exit 0 L230 and, after the receipts name an earlier release, L237.
- `OnlyACompleteVerdict…IsPublished` edge: the report at the destination L254 → L274; red `publish-never`.
- `AnIncompleteVerdictIsNotPublished` edge: exits 2 L261 → L272; no report L274; red `publish-always` (it fails at the exit; the same bug publishes the report).
- `AnIncompleteStageOrACancelledRun…` edge: exit 2 L295 or a cancellation L290; no destination directory L297; red `publish-incomplete`, `publish-ignore-cancel` (each also publishes).
- `PublicationReplacesAnEmptyPlainDestination` edge: the report at the destination L302 → L341; red `publish-refuse-empty`.
- `PublicationReplacesOnly…RetainsAnythingForeign` trap: refused L344; the chart's report keeps its text L345; the dangling link stays a link L347, or the foreign file keeps its text L349; red `publish-clobber` (it deletes the foreign entry and moves the chart).
- `TheNativeBootstrapStartsOnlyAfterGo…` contract (keep), DR 062 §6: the stand-in's exit 1 L28, which it gives only after the exact go frame.
- `ABootstrapWithoutTheExactReadyFrame…` trap (keep): `InvalidDataException` L43; no go frame recorded L46.
- `AFailedRuntimeHandoff…` contract (keep), DR 062 §6: after the `Win32Exception` L63, no directory in the caller's directory L67, L68.
- `ACancelledRun…` contract (keep), DR 062 §5 §6: a cancellation L82; no directory in the caller's directory L86, L87.
- `ReadinessProves…` contract (keep), DR 062 §4 §6: passes L103; a journal L104 → L150; no scratch directory L151.
- `ReadinessRefuses…` contract (keep), DR 062 §4 §6: `IOException` L124; a journal L126 → L150; no scratch directory L151.
- `OnlyThisReleasesUneditedSkillValidates` contract, utilities install-skill: `Validate` refuses each row's skill L29; red `m-18` (payload, protocol, earlier), `m-16` (source), `skill-missing-current` (missing).
- `AnUnchangedSkill…` contract, DR 062 Consequences: validates as this release's L42; no earlier wording L43; red `skill-keep-earlier` (L42).
- `LocalEdits…` contract, utilities install-skill: refused L56; SKILL.md ends with the edit L57; receipt bytes kept L58; red `skill-edits-ignored` (replaced: no refusal, no edit, a new receipt).
- `AnEarlierReceiptWithoutAnInventoryIsReplaced…` contract, utilities install-skill: validates after `Place` L69 → L70; red `skill-keep-earlier` (L70).
- `AnEarlierReceiptWithoutAnInventoryIsRefused…` contract, utilities install-skill: refused L80; SKILL.md ends with the earlier wording L81; red `skill-bytes-unchecked` (replaced: no refusal, wording gone).
- `ForeignCanonicalSkill…` contract, utilities install-skill: SKILL.md keeps its text after `InstallAsync` L97; red `foreign-overwrite`.

## Related

- [DYD-1520 statechart audit](./statechart.md) - The batch's totals, mutation result and planted bugs
- [Test rubric](./rubric.md) - §2 row 2, a true job tag
