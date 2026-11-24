---
area: project
type: reference
---

# DYD-1520 Statechart Tag Clauses

Every job tag in the [statechart batch](./statechart.md), split into its clauses. Each clause names the assertion
line that fails if the clause is false, in every case the tag covers; a theory's line runs once per row. Lines are
in `DynaDocs.Tests/Statechart/<Class>.cs` on the candidate. Through a helper, the method's call line comes first,
then the helper's lines. A clause no line proved was dropped from its tag. Assertions no clause names stay, unclaimed.

## Archive, Arguments, Assets

- `LockedZip…`: exact member bytes L19, L20.
- `GzipTarRetainsMacJreLayout…`: layout L46; hardlink content L47; modes outside Windows L49.
- `UnsafeNamesCannotWriteOutsideOwnedStaging`: refused L68.
- `DuplicateCaseCollisionAndFileAncestorReject`: refused L82.
- `ExpandedBytesAndEntryCountAreStreamedBounds`: bound error L95, L97; next member not written L98.
- `EscapingTarLinksRejectBeforeLinkCreation`: refused L115; before the link exists L116.
- `ComposedLinksThatLeaveTheRoot…`: refused L134; before any link exists L135, L136; outside file unchanged L137.
- `LinksThatLoopThroughEachOtherReject`: refused L149; before any link exists L150.
- `ALinkThroughAnotherLinkThatStaysInsideExtracts`: extracts L163.
- `ForeignStagingAndCancellation…`: existing staging refused L177; its content kept L178; cancelled run creates no staging L182, L183.
- `NativeArgumentVectorsRejectTruncatingNul…`: a NUL is refused L12.
- `NativeArgumentVectorsPreserveUtf8…`: non-ASCII and empty arguments as UTF-8 L20, L21; NULL end L22; pointer cleared L24.
- `DamagedAssetsAreExtractedAgainOffline…`: the offline run still reaches its outcome L32; extracted again from the payload L34 (`Verify`), L35, L36.
- `ADirectoryWithoutThisPayloadsReceipt…`: refused (the run fails) L54; foreign directory retained L56.
- `AnExtractionACrashLeftBehind…`: swept L73.

## Checker

- `OwnedCopyRetainsOfficialBytes…`: exact copy L59; alone in its directory L61.
- `AnInputOtherThanThePinnedJar…`: refused L79; named L80; before any copy L81.
- `CompanionIsRejected…`: refused by discovery L107, L108; refused by preparation L110; nothing copied L111.
- `AliasedFileAndDirectoryInputs…`: resolves to the actual jar L130; companion beside it refuses L133.
- `EveryLaunchRechecks…`: refused at the next launch L161.
- `ExistingCheckerDirectoryIsRetained…`: never adopted L176; content kept L177.
- `BootstrapResolutionUsesSealedIdentity…`: through the matching bootstrap with the owned jar L183 → L210.
- `AnUnsealedBootstrapIsRefused…`: refused, no raw TLC fallback L188 → L209.
- `ALinkedBootstrapBuildLockIsRefused…`: refused before it is read, no fallback L225 (exact message).

## Command, Downloads, Inventory

- `InstalledDoctorExtractsPayload…` (Windows): §1 no readiness claimed L37; §3 nothing installed L39.
- `InstalledDoctorNamesMissingRuntimes…` (Windows): §1 no readiness claimed L53.
- `DoctorDeclinesOutsideWindows…`: exits 2 L67; nothing under its roots changed L68.
- `InstallSkillPlacesAValidSkill…`: §3 placed and validates L89.
- `InstallSkillWithoutNode…`: §5 non-success L100; §3 placed skill kept and validates L102.
- `ASubcommandWithoutItsRequiredOption…`: exits 2 L113; prints nothing L114; changes nothing L115.
- `LosingTheNpmLifetimeCancels…`: §5 non-success L137; no request saved L139.
- `SelectUsesTheCommissionedLock…`: the platform's lock row L14 to L17; unknown RID refused L18.
- `EveryPlatformPinsManagedNode26`: official archive and checksum L32, L33, L34.
- `StreamingBoundRejectsExcess…`: refused L43; nothing written L44.
- `ADownloadWritesItsBoundedBody…`: body written L55; verifies L56 (`Verify`); never overwrites L58, L59.
- `ABodyOverItsBoundIsRefused…`: refused L72; nothing written L73, L74.
- `AFailedResponseIsAnError…`: error L84; no archive L85.
- `IntegrityRejectsChanged…`: fails verification L129; left in place L132.
- `AnInstalledLinkWhoseFollowedPathLeavesTheRoot…`: rejected L24.
- `InstalledLinksThatLoopOrStayInside…`: inside link recorded L40; loop rejected L44.

## Lifecycle and Lock

- `IndependentCleanupDeletesRegisteredScratch…` (Windows): deleted once sealed and drained L52, L53, L54.
- `OwnerEofRetainsRegisteredScratch…` (Windows): no drain, no cleanup L71, L72.
- `AHelperThatDoesNotSpeakTheCleanupProtocol…` (Windows): non-protocol helper rejected L85; unstartable helper rejected L87; nothing reserved L89.
- `ALifetimeOwnsAttachedChecks…` (Windows): owned until exit L106; scratch then cleaned L108, L109; unowned wait fails L112.
- `OwnerLossBeforeAnyReservation…`: exits 2 L125 → L144; creates nothing L145.
- `OwnerLossBeforeOrDuringRegistration…`: no drain, no cleanup L129 → L144, L145 (both reserved directories left).
- `ADrainedSuccess_LeavesNoScratch`: exits 0 L161; scratch deleted L162.
- `AForgedDrainOrALostOwner…`: no authoritative drain, no cleanup L178, L179, L180.
- `AReplacedScratchPathIsRetained…`: Linux, no cleanup of what the drain did not register L202, L203, L204; Windows, no deletion before the drain L214, L215, L216.
- `AHelperWithoutAValidChannelNonce…`: exits 2 L235; creates nothing L236.
- `AWaiterTakesTheLock…`: waits while held L15; then exclusive L18.
- `AHolderPastTheBound…`: fails L28; names its path L29.
- `CancellationEndsTheWait`: ends with a cancellation L42.
- `AMissingDirectoryIsAnErrorNotAWait`: `DirectoryNotFoundException` L50.

## Managed

- `AnApprovedInstall_IsReady…`: components placed and ready L35, L36, L37.
- `AServedArchiveThatDiffersFromItsLock…`: §5 the install fails L62; §4 nothing ready L64.
- `AMissingStoreIsMissingEverything…`: no component ready L74.
- `AForeignMarkerOrUnexpectedChild…`: refused L92; content kept L93.
- `ContentThatFailsVerification…`: not ready L113; left in place L114.
- `AComponentWithoutAReadinessReceipt…`: not ready L127; left in place L128.
- `VerifiedContentFromAnEarlierRelease…`: ready L137.
- `AManagedRuntimeThatCannotExecute…`: §4 readiness proved by the runtime's own start, null or its path L153.
- `CancellingAWaitingInstall…`: §5 non-success L170; nothing left L171, L172.
- `CrashLeftStagesAndReceiptTempFiles…`: own RID's stages and receipts swept L195, L196, L198; another RID's kept L197.
- `ARestampThatWaitedOnTheLock…`: rescans and does nothing L220, L221, L222.

## POSIX channel and lifecycle

- `AChildInheritingALongTempDir…`: the child connects L32 (accept), L34 (its anchor frame).
- `ASocketPathFitsSunPathCountedInUtf8Bytes`: the path fits in bytes L49.
- `TheCoreFxPipePathStaysWhileItFits`: stays at 108 bytes L55 → L68.
- `TheCoreFxPipePathIsReplacedOneBytePastSunPath`: replaced at 109 bytes L59 → L68.
- `ALongTempDirFallsBack…`: one private directory per user under /tmp L79, L80.
- `AMissingSocketDirectoryIsCreatedPrivate`: used L92; mode 0700 L93.
- `AnExistingSocketDirectoryOnlyItsOwnerCanEnterIsUsed`: used L99 → L114.
- `AnExistingSocketDirectoryIsUsedOnlyWhenNoOneElseCanEnterIt`: refused L106 → L115 → L146; names the directory L147; names TMPDIR L148.
- `ASocketDirectoryOwnedBySomeoneElseIsRefused`: refused, naming it and TMPDIR L126 → L146, L147, L148.
- `ALinkOrFileInPlaceOfTheSocketDirectory…`: link refused L137 → L146 to L148; file refused L141 → L146 to L148.
- `ARegisteredLifetime_CleansItsScratch…`: on Linux, cleaned after the drain L27, L28.
- `AConnectedChannelLeavesNoSocketFileBehind`: no socket file L43.
- `ALostParent_StillDrainsAndCleans…`: on Linux, drained and cleaned L65, L66.
- `AFailedHandoffLeavesTheOwnerToDrain…`: drained and cleaned L82, L83, L84.
- `AContinuedHandoff…`: lock survives exec L105; §5 no verdict L107; §6 cleaned L106, L108.
- `AContinuationOutsideTheOwnersStore…`: refused L124; cleaned L126, L127.
- `AHandedOffRequestRecordsItsVerdict…`: no verdict until reported L169, L171; cleaned L173, L174, L175.
- `AnUnprovenCompletedRequestFrame…`: §5 no verdict L200, non-success L202; §6 cleaned L203.
- `OnlyACheckingChildInsideTheOwnedGroup…`: the drain stops the attached child L224.
- `AMalformedOrForeignControlFrame…`: exits 2 L244; cleaned L245.
- `LosingTheNpmLifetimeStopsControl…`: §5 non-success L257; §6 cleaned L258, L259.
- `APrivateModeWithoutAValidChannel…`: exits 2 L275; prints nothing L277; creates nothing L278.

## Prerequisites, public flow, Ready, Skill

- `RuntimeCompatibilityPreservesExistingBoundaries`: compatible or not at its boundary L21.
- `InvalidExplicitOverridesCannotFallBack…`: invalid overrides are errors L29, L32; no override, no checker L33.
- `CallerRelativeJavaAndBareJava…`: a JAVA path resolves against the caller's directory L44.
- `MissingPrerequisites_Exit3…`: §2 exit 3 and a saved request L24, L25; §3 nothing installed L26.
- `ChangedSavedInputs_RefuseTheResume…`: §4 refused L40; §5 request kept L41, nothing installed L42.
- `AnInvalidExplicitJava…`: error L65; no plan L66; no saved request L68.
- `AnIdeJavaIsNeverLookedFor…`: not taken for Java L101, L102.
- `ADiscoveredRuntime_IsUsed…`: §1 §2 the plan lists exactly what no compatible runtime provides L118.
- `ADamagedManagedComponent…`: §2 in the plan L136; Consequences, left in place L137.
- `ResumingACompletedRequest…`: recorded verdict L153; nothing installed L154.
- `AnApprovedResumeWithoutNetwork…`: non-success L166; request kept L167; nothing installed L169.
- `AFrameworkDependentLaunch…`: same host L191; same assembly L192.
- `AnApprovedResume_InstallsAndContinues…`: continues the saved request L226, L227; installed L228; later runs reuse the store L231, L238.
- `OnlyACompleteVerdict…IsPublished`: publishes L273 (exit), L275 (chart present).
- `AnIncompleteVerdictIsNotPublished`: exits 2 L273; publishes nothing L275.
- `AnIncompleteStageOrACancelledRun…`: no destination L298.
- `PublicationReplacesAnEmptyPlainDestination`: replaced with the chart L303 → L342.
- `PublicationReplacesOnlyAnEmptyPlainDestination_AndRetainsAnythingForeign`: refused L345; chart kept L346; foreign kept L348 (link), L350 (file or directory).
- `TheNativeBootstrapStartsOnlyAfterGo…`: released only by the exact go frame, its exit the verdict L30.
- `ABootstrapWithoutTheExactReadyFrame…`: never released L48.
- `AFailedRuntimeHandoffSurfaces…`: error surfaces L65; no scratch or output L69, L70.
- `ACancelledRunNeverHandsOff…`: §5 non-success L84; §6 no scratch or output L88, L89.
- `ReadinessProvesTheSelectedRuntimes…`: under registered ownership and cleaned L106 → L152, L153.
- `ReadinessRefusesAnyComponent…`: under registered ownership and cleaned L128 → L152, L153.
- `OnlyThisReleasesUneditedSkillValidates`: does not validate L31.
- `AnUnchangedSkillFromAnEarlierRelease…`: replaced L43, L44, L45.
- `LocalEditsToAnEarlierRelease…`: edits untouched L59, L60.
- `AnEarlierReceiptWithoutAnInventoryIsReplaced…`: replaced L72.
- `AnEarlierReceiptWithoutAnInventoryIsRefusedUntouched…`: refused L82; untouched L83.
- `ForeignCanonicalSkillIsPreserved…`: refused L97; its file as it was L99.

## Related

- [DYD-1520 statechart audit](./statechart.md) - The batch's totals, mutation result and planted bugs
- [Test rubric](./rubric.md) - §2 row 2, a true job tag
