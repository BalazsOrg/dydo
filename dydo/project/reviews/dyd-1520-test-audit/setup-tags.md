---
area: project
type: reference
---

# DYD-1520 Setup Tag Clauses

The clause-to-line map of every job tag in the [setup batch](./setup.md) (review lessons 10 and 11),
on the candidate's files. A tag reads `<kind>: <cite> (<topic>): <observation>`. The topic in
parentheses names the cited rule, and the cite states it; every clause of the observation is
listed below with the assertion line that fails when the clause is false, in every case the tag
covers. A row with `[n]` maps one theory row; a line in another file names that file. `exit` is
the exit-code assertion, `ok` an `AssertSuccess` (exit 0).

A feature file has one `# Job:` line for all its scenarios (rubric §2 row 2), so its clauses are a
list: each names the scenarios that prove it, every scenario proves at least one, and none is
false of a scenario it names.

Eight kept `unsure` rows carry no tag, which the rubric allows: the two `CompletionProviderTests`
cases and the six this round moved to `unsure` because no source states their claim (listed in
[setup.md](./setup.md)). The other `unsure` rows keep a tag, mapped here like every other.

## InitCommandTests

| Case | Clause: line |
|---|---|
| Join_PostWorkFailure_PreservesOriginalConfigBytes | exit 2: 44; names the failure: 45; hooks.json stays: 47; bytes, unknown keys: 48-49; no tmp sibling: 50 |
| Join_AlreadyRecordedIntegration_LeavesConfigBytesUntouched | ok: 64; bytes, unknown keys: 65-66; no tmp sibling: 67 |
| Init_Fresh_RecordsRunningCliVersion | ok: 76; dydoVersion is the running version: 78 |
| Join_ExistingDydoVersion_StaysUntouched / Join_RecordingNewIntegration_PreservesDydoVersion | ok: 89 / 103; dydoVersion kept: 91 / 104 |
| Join_WithoutDydoVersion_WritesNone | ok: 116; no dydoVersion: 117 |
| Init_All_WritesHostAgentSettings | ok: 152; depth "3": 155; autoMemoryEnabled false: 156; config.toml exact: 157 |
| Join_All_TurnsNativeMemoryOffOverTrueAndSaysSo | ok: 174; both off: 176, 178; other value: 177; rest of the text: 178 |
| Join_Codex_AddsMemoriesIntoExistingFeaturesTable | ok: 193; inside the table, rest kept: 194 |
| Init_Codex_RejectsMemoriesFormItCannotEditBeforeAnySideEffect | exit 2: 212; names config.toml: 213; bytes: 214; no dydo.json: 215 |
| Init_HostAgentSettings_PreserveSatisfyingValuesAndRepeatBytes | init ok: 229; lines in order: 232; one marked guard: 234; config bytes: 235; join ok: 238; neither file changes: 239-240 |
| Join_Codex_BomAgentsTable_IsReadAndTheBomKept / ...BomFeaturesTable... | ok (reads it): 252 / 266; BOM and exact text: 253 / 267 |
| Init_ClaudeHostSetting_RejectsNoncanonicalOrIncompatibleValue | exit 2: 286; file, key: 287-288; bytes: 289; no dydo.json, local: 290-291 |
| Init_CodexHostSetting_RejectsConflictWithoutPartialInit | exit 2: 311; config.toml, agents: 312-313; bytes: 314; no dydo.json, hooks.json, AGENTS.md: 315-317 |
| Init_CodexHostSetting_RejectsAmbiguousTomlBeforePartialInit | exit 2: 338; config.toml: 339; bytes: 340; no dydo.json, hooks.json, AGENTS.md: 341-343 |
| Init_ClaudeHostSetting_RejectsNullManagedMembersBeforePartialInit | exit 2: 358; settings.json: 359; bytes: 360; no dydo.json, local: 361-362 |
| Init_All_PreflightsBothHostsBeforeAnyProjectMutation | exit 2: 384; both files' bytes: 385-386; none of six files: 387-392 |
| Join_All_PreflightFailurePreservesExistingProjectAndBothHosts | exit 2: 413; dydo.json, both hosts: 414-416; AGENTS.md, CLAUDE.md bytes: 417; no local, hooks.json: 418-419 |
| Init_All_MalformedCodexToml... / Join_All_MalformedCodexToml... | exit 2: 438 / 468; config.toml, malformed: 439-440 / 469-470; kept bytes: 441-442 / 471; not written: 443-448 / 472-474 |
| Init_All_RejectsSemanticDottedAgents... / Join_All_... | exit 2: 492 / 520; config.toml: 493 / 521; kept bytes: 494-495 / 522; not written: 496-501 / 523-525 |
| Init_Codex_PreservesUnrelatedAgentsHistoryArrayAndRepeatBytes | ok: 535, 538; repeat changes no byte: 539; header kept (539 makes it the init's): 540 |
| Init_Codex_RetainsIntegerAgentCapacityValuesAndRepeatBytes | ok: 556, 558; bytes through init and join: 557, 559 |
| Init_Codex_HooksContainPreToolUseDydoGuard | one entry: 575; matcher with shell tools: 581; one hook `dydo guard`: 584-587; no Stop: 589 |
| Init_Codex_PreservesExistingHooks | ok: 601; foreign text: 603-607; dydo guard: 608; no --stop: 609 |
| Init_Codex_PreservesCustomHooksWhenRemovingDydoHooks | keeps one guard: 694; drops other guard entries: 691-693; drops --stop: 699; foreign hooks: 686-691, 697-698 |
| Init_Claude_PreservesExistingHooks | ok: 712; local lines, no guard: 714-715; guard and matcher: 721-722; foreign text: 718-719, 724-727 |
| Join_Claude_MovesDydosGuardOutOfTheLocalSettings_KeepingForeignHooks | ok: 747; no Stop: 751; echo mine the one entry: 753; one marked guard committed: 757 |
| Join_Claude_LeavesExactlyOneDydoGuardInTheCommittedSettings | no Stop: 782; two entries: 784; one guard, full matcher, its one command: 785-788; no bare guard: 789; foreign bytes: 790 |
| Join_MixedGuardEntry_KeepsTheForeignHookBytes | ok: 813; rest of the entry's bytes: 815; one guard entry: 816, 799-800 |
| Join_GuardDuplicatesAroundForeignEntries_KeepTheForeignEntriesBytes | foreign bytes: 831-832; one guard where the first stood: 834-835, 799-800 |
| Join_AlreadyEmptyForeignEntry_StaysBeforeTheGuard | bytes: 849; before the one guard: 851-853, 799-800 |
| Join_Claude_KeepsAnAlreadyEmptyForeignEntryInTheLocalSettings | bytes: 866; guard leaves the file: 867 |
| Join_TrailingRootInAHooksFile_StartsItFresh | ok: 880; one parsable object: 882 |
| Join_Claude_TrailingRootInTheCommittedSettings_IsRefusedAndLeftUntouched | nonzero: 894; malformed: 895; text kept: 896 |
| Init_Claude_GuardMatcherNamesEveryGuardedTool | ok: 905; one entry: 907; nine tools: 909-910 |
| Init_Claude_ReplacesAnUnmarkedGuardHook | no bare: 933; one marked: 934 |
| Init_WritesEachHostsFiles_AndARepeatJoinChangesNoByte | init, join ok: 954-955; no file changes: 956; .gitignore: 958-960; AGENTS.md, no CLAUDE.md: 961-962; integrations: 964-965; Claude files: 967, 972-974, 976-977, 981; Codex files: 984-985, 987 |
| Init_Claude_MergesTheAllowListIntoExistingLocalSettings | ok: 1004; existing list kept [0]: 1007; each entry once: 1008 |
| Join_SecondHost_RecordsBothAndWiresTheNewOne | recorded: 1024-1025; hook files: 1026-1027; .gitignore: 1028 |
| Init_Codex_WritesTheExecPolicyRuleForDydoAndDydoExe | ok: 1057; merge rule: 1060; clean rule: 1061; allow: 1062 |
| Join_Codex_RestoresTheDydoCleanRuleIdempotently | both joins ok: 1073-1074; exactly dydo's rules after them: 1076 |
| Join_Codex_LeavesACrlfRuleWithTheSameContentByteForByte | bytes: 1090 |
| Join_Codex_RewritesAStaleCrlfRuleWithCrlf | ok: 1101; exact rules in CRLF: 1103 |
| Join_Claude_RemovesClaudeMdIdenticalToAgentsMd / ...MatchingTheTemplate... | ok: 1120 / 1135; CLAUDE.md gone: 1121 / 1136; AGENTS.md text: 1122 / 1137 |
| Join_Claude_KeepsUserWrittenClaudeMdAndWarns | ok: 1150; bytes: 1151; hides warning: 1152; @AGENTS.md: 1153 |
| Join_Claude_RemovesClaudeMdAnOlderReleaseGenerated | ok: 1177; gone, each row's release: 1178 |
| ...KeepsCustomisedCopyOfAnOlderReleaseAndWarns / ...KeepsReleasedBody... / Init_Claude_KeepsUserWritten... | ok: 1191 / 1206 / 1220; bytes: 1192 / 1207 / 1221; warning: 1193 / 1208 / 1222 |
| Join_Claude_WarnsAboutOtherFilesThatHideAgentsMd | ok: 1237; bytes: 1238; its own warning: 1239 |
| Join_Claude_DoesNotWarnAboutAnEntryFileThatImportsAgentsMdFirst | ok: 1256; bytes: 1257; neither warning: 1258-1259 |
| Join_Claude_WarnsAboutAnEntryFileWithoutTheLeadingImport | ok: 1275; bytes: 1276; its warning: 1277 |
| Join_Codex_LeavesClaudeMdAlone | ok: 1290; bytes: 1291; stdout names no CLAUDE.md: 1292 |
| Join_Claude_MigratesLegacyClaudeOnlyProject | ok: 1303; AGENTS.md is the template: 1304-1305; CLAUDE.md gone: 1306 |
| Join_All_FailedPreflightKeepsLegacyClaudeMdAndCreatesNoAgentsMd | exit 2: 1320; CLAUDE.md bytes: 1321; no AGENTS.md: 1322 |

## CleanCommandTests

The class tag is the bare cite `dydo-commands-worktrees.md dydo clean`; each class-tagged case
pins the bullet named here.

| Case | Clause: line |
|---|---|
| Merged_... (Branches, Removal) | exit 0: 81; tree, branch, listing gone: 82-84 |
| PushedButUnmerged_... (remote branches never touched) | exit 0: 97; tree gone: 98; branch, remote branch kept: 99-100 |
| NoOriginRemote / FailedFetch / OriginWithoutADefaultBranch (kept, line says why) | exit 0: 111 / 123 / 141; branch kept: 112 / 124 / 142 |
| DefaultBranchChangedOnOrigin_... (stale origin/HEAD never decides) | exit 0: 158; kept: 159; origin/HEAD still stale: 160 |
| SquashMerged_MultiCommitBranch / _InASha256Repository / RebaseMerged / UndefinedMergeDriver... / ...SemicolonAndASpace | exit 0: 213 / 249 / 288 / 409 / 451; deleted: 214 / 250 / 289 / 410 / 452 |
| BranchFallback_DeletesOnlyTheVerifiedTip_AMovedRefIsKept | returns false: 179; at its new tip: 180 |
| UnpushedCommits / SquashMergedThenAnUnmergedCommit / ..._InASha256Repository_KeepsTheBranch | exit 0: 194 / 231 / 268; at its tip: 195 / 232 / 269 |
| ConfiguredKeepMineMergeDriver_DoesNotHideAnUnmergedChange | exit 0: 316; kept: 317 |
| KeepMineMergeDriverFromTheEnvironment_DoesNotHideAnUnmergedChange | exit 0: 354; kept [1], [3]: 355; would be kept [0], [2]: 358 |
| ScratchGitEnvironment_ShutsOutSystemAttributesAndConfig_WhateverIsInherited | exactly five GIT_* values: 383; XDG_CONFIG_HOME: 391 |
| SquashedThenRevertedOnOrigin_KeepsTheBranch | exit 0: 426; kept: 427 |
| DryRun_JudgingASquash_WritesNoObject_AndMovesNoRef | exit 0: 477; no object: 478; no ref: 479 |
| DirtyTrackedChanges_AreArchivedAsAPatchThatReapplies | exit 0: 494; one entry: 495; tree gone: 496; reapplies: 500-502; manifest: 505-507 |
| UntrackedFile_SurvivesCleanInTheArchive | exit 0: 520; tree gone: 521; one entry: 522; bytes: 523 |
| Archive_ListsEntriesNewestFirst (unsure) | both listed: 535-536; newer first: 537 |
| Retention_PrunesEntriesOlderThanNinetyDays / ...PastTheSizeCap (Retention) | exit 0: 549 / 564; pruned: 550 / 565; kept: 551 / 566-567 |
| InvalidSizeCap_IsRefused (configuration.md clean) | exit 2: 579; tree kept: 580 |
| LinkInsideTheTree_IsUnlinked_ItsTargetSurvives | exit 0: 594; removed: 595; target's text: 596 |
| UntrackedLink_IsNotFollowed | exit 0: 610; one entry: 611; no copy: 612; outside text: 613 |
| UntrackedLink_IsRecordedWithItsTarget_... (links record) | exit 0: 626; path, kind, target: 630-632 |
| PathLongerThan260Characters_IsRemoved (Removal) | exit 0: 646; tree gone: 647; unlisted: 648 |
| MainCheckout_IsRefused / TreeHoldingTheCurrentDirectory_IsRefused | exit 2: 657 / 673; kept: 658 / 674 |
| All_SkipsTheMainCheckout_LockedTrees_AndT3Trees | exit 0: 690; locked, T3 kept: 691-692; plain removed: 693 |
| RunningProcessInside_IsSkipped (Windows or Linux) | exit 2: 716; kept: 717 |
| DryRun_ChangesNothing | exit 0: 738; file, branch kept: 739-740; archive only the old entry: 741 |
| DryRun_RunsNoBusyCheck (--dry-run) | exit 0: 753; never probed: 754; tree kept: 755 |
| BusyProbeThatCannotMoveTheTreeBack_... | exit 2: 776; one probe folder: 777; other removed: 778; locked: 780; files in it: 781 |
| SubmoduleHoldingWork_KeepsTheTree | exit 2: 803; submodule, branch kept: 804-805; no entry: 806 |
| CleanPushedSubmodule_GoesWithTheTree (Submodules) | exit 0: 817; removed: 818 |
| SubmoduleWithUnpushedWorkOffHead_KeepsTheTree | exit 2: 850; kept: 851 |
| DryRun_LeavesTheIndexOfTheTreeAndItsSubmoduleUnchanged | exit 0: 874; bytes, write time: 877-878 |
| ArchiveRootBehindALink_IsNeitherPrunedNorWritten | exit 2: 901; old entry's text: 902; only entry: 903; tree's file: 904 |
| SkippedTrees_KeepTheirBranch | exit 2: 918; branch: 919; both trees: 920 |
| ArchiveThatCannotBeWritten_FailsClosed_AndRemovesNothing | exit 2: 941; file, link, target: 942-944; branch: 945; listed, unpruned: 947-948 |
| NeitherOrBothTargetForms_IsAUsageError / UnknownTarget_IsRefused | exit 2: 958 / 966 |

## The other classes

| Case | Clause: line |
|---|---|
| WorktreeProcessProbeTests RenameProbe_IdleFolder_... | not in use: 27; file back at its path: 28; no sibling: 29 (Windows only: `WorktreeProcessProbe.cs:19-20`) |
| ...RenameProbe_FolderThatCannotBeRenamed_ReportsInUse | in-use reason, cannot be renamed: 36 |
| ...RenameProbe_RetriesAMoveBackThatFailsOnce | retried, so not in use (one failure strands it otherwise): 52; back, no sibling: 53 |
| ...RenameProbe_MoveBackThatKeepsFailing_NamesWhereTheFolderIs | throws: 68; probe exists: 70; beside the tree, named: 71; message names it, cause: 72-73 |
| ...ProcScan_FindsAProcessWhoseWorkingDirectoryIsInside | reported by pid: 88 |
| ...ProcScan_IgnoresUnreadableAndUnrelatedProcesses | sibling prefix and no-cwd folders not reported: 103 |
| ConfigServiceTests LoadConfigStrict_DistinguishesMalformed... | throws: 43; dydo.json: 46; Invalid JSON: 47 |
| ...SaveConfig_UnchangedConfig_WritesNothing | no temporary sibling (its creation throws): 60 |
| ...LoadConfigStrict_RejectsInvalidTestingRunner [0]-[5] | refused: 76; names the key: 78 (forms: 65-70) |
| ...LoadConfigStrict_PropagatesAnUnreadableConfiguration | IOException: 88 |
| ...SaveConfig_Success_... | replaced: 119; no sibling: 120 |
| ...PartialTemporaryWrite / Flush / Close / Replacement failure | own error: 132+138 / 152+155 / 169+175 / 189+192; bytes: 139 / 156 / 176 / 193; no sibling: 140 / 157 / 177 / 194 |
| ...SaveConfig_TemporaryNameCollision_PreservesBothFiles | throws: 208; already exists: 210; both files: 211-212 |
| ...TemporarySiblingPath / CreateNewSibling_Refuses / ..._OpensWriteOnly | folder, name, differs: 226-228 / throws, bytes: 238, 240 / write, no read, reader refused: 251-253 |
| ConfigServiceWorktreeIsolationTests | the worktree's root, not the main one: 55-57 |
| InstallNudgeTests Install_... [0]-[53] | denied first (PB15 turns every row red here): 54; retry allowed: 55 |
| ...RestoreOrOtherCommand_NeverMeetsTheNudge [0]-[34] | allowed first: 65 |
| ConfigFactoryTests Guard_DefaultNudges_WarnOnTheCommand | named warning [0]-[6]: 60; pass [7]-[10]: 58 (both through 37) |
| ...Guard_LongEnvOptionChain_IsReadWithinTheMatchBudget | plain word allowed [0]: 58; warning [1]: 60 (a timeout refuses instead, `GuardCommand.cs:262`) |
| AdoptCommandTests Release / NotARelease / NoUsableConfig | exit: 29 / 47 / 62; recorded: 30; bytes: 48; as it was: 64 [0], 66 [1] |
| DownstreamConfigTests JoinClaude_RewritesOnlyDydosOwnLines... | ok: 48; only those changes: 54; BOM: 55; node lines: 56; one marked, no bare, no --stop: 60-62 |
| ...JoinClaude_LeavesNoDydoHookInTheLocalSettings | ok: 69; no hooks: 72; Bash(dydo:*): 74 |
| ...JoinCodex_ReadsTheBomConfig... | ok: 85; appended, BOM: 87-88; only matcher and Stop: 91; node lines: 92; one guard: 94; dydo.json bytes: 97 |
| ...Adopt_AddsOnlyTheVersionAndTheReleaseNudge | ok: 106; lines, no CR, escapes: 109-111; version: 114; one nudge, nothing else: 116-118 |
| ...RepeatedJoinAndAdopt_... | runs ok: 125-131; five files' bytes: 134 |
| ...Adopt_NothingToChange_... [0]-[4] | ok: 148; bytes: 150 |
| ...Adopt_Upgrade_... [0]-[4] | ok: 161; encoding, BOM: 163 (Decode, 213); order, version, unknown key: 164; CRLF: 165; nudges: 167; nothing else: 169 |
| ...JoinAll_Utf16LeHookFiles_... | ok: 184; BOM and exact text: 186-191 (Decode, 213); foreign key first: 193; allow entry: 194 |
| InitCheckIntegrationTests FreshInit_PassesCheck [0]-[1] | exit 0: 22; bad-doc.md not named: 23 |
| ShellCompletionInstallerTests NewProfile / ExistingProfile / Legacy / Customized / UnknownShell | exact bytes, second install: 32-33 / 53 / 71 / 89 / no profile: 100 |
| ScaffoldTreeTests Embedded... / EveryScaffoldFile... | non-empty, same names: 17, 25 / each reads back (a missing one throws, `ScaffoldTree.cs:63`): 35 |
| ModeStateTests Show [0]-[3] / Set_PersistsForTheNextReader | exit 0: 26 / 40; trimmed output: 27 / 41 |
| ModeCommandTests Prints / CaseSensitive / Denied / WithoutAValue | exit: 25 / 38 / 71 / 83; output: 26-27 / 39-40 / 72-73; no store or kept text: 41 / 74 |
| CompletionsCommandTests KnownShell / Complete_OffersTheWordsValues | exit 0: 20 / 66; registration (bash, zsh end in dydo): 21; _complete: 22 / each value: 48 |

## Features

| Feature: clause | Scenarios (step line) |
|---|---|
| machine-mode: show prints exactly overnight's line | Show lists, Set changes, Set refuses, Warning and retry (MachineModeSteps.cs:72) |
| ...set changes the flag | Set changes (57, 72) |
| ...refuses an unknown flag or value with exit 2 naming it, overnight off | Set refuses [0]-[1] (62-63, 72) |
| ...the guard allows mode show for an agent and a sub-agent | Any agent [0]-[1] (119) |
| ...a mode set denied naming dydo mode set | An agent's first, sub-agent, Opposite, Warning and retry (116) |
| ...its identical retry allowed | sub-agent, Opposite, Warning and retry (119) |
| ...each direction and full command warned on its own | Opposite (116, three times) |
| ...stored mode unchanged until the command runs | Warning and retry (45-46 twice, then 57, 72) |
| ...quoted, pathed or indirect denied; a mention allowed | the two corpora (116; 119) |
| fresh-installation: init ok, integrations recorded | [0]-[3] (FreshInstallationSteps.cs:19, 27) |
| ...check: warnings, no errors; customized: neither | [0]-[3] (35-37; OptionalSummarySteps.cs:47-49) |
| mode-store-failure: one error; state kept | [0]-[3] (ModeStoreFailureSteps.cs:58-60; 67-68) |

## The install-nudge rows, traced to DYD-499

DYD-499 names the pattern's commands (`pnpm|npm|yarn|bun add/dlx/create/update/upgrade/link` and
`install <pkg>`, `npx -y/--yes/-p`, `uv add/remove/pip install/tool install`, `uvx`, `pipx`,
`pip/pip3 install`, `python -m pip install`, `dotnet add`, `dotnet tool install/update`,
`dotnet workload install`, `dotnet new install`, `apt/apt-get install`, `snap install`,
`flatpak install`, `cargo/go/gem install`, each optionally behind `sudo`); its policy covers any
new library or tool "in a project or on the machine"; and it ships the interim pattern unchanged.

| Rows of Install_... | Class | Traced by |
|---|---|---|
| 0-4, 17-18, 22, 36, 52, 53 | an add (52 in a pipeline, 53 upper case) | the named `add` forms |
| 11-15, 24-25, 28-33, 35, 37, 39-48, 50-51 | an install of a named package | the named `install` forms (31, 35 by path; 33 a versioned python; 43 behind sudo; 50-51 behind env or an assignment) |
| 34 | `py -m pip install` | `python -m pip install`, through the Windows launcher |
| 5-6, 19-20, 27 | a fetch-and-run | `dlx`, `create`, `npx -y/--yes`, `uvx` |
| 21, 26, 49 | a fetch-and-run, or a tool, outside the list | the policy: `pnpx -p` and `uv run --with` fetch a package, `brew install` adds a machine tool |
| 7-9, 16, 38 | an update (8 is `pnpm up`, its alias) | `update/upgrade`, `dotnet tool update` |
| 10 | a link | `link` |
| 23 | a removal | `uv remove` |

Of the 35 `RestoreOrOtherCommand` rows, DYD-499 names the restores in 0-8, 12-13 and 15-16 (bare
`pnpm install` or `npm install` and `pnpm install --frozen-lockfile`, alone or with an option, a
chain, a pipe or a redirect; `npm ci`; `uv sync --frozen/--locked`; `dotnet restore` and
`dotnet tool restore`); the rest restore declared dependencies, run, build or list, or only
mention an install in text, which its goal ("installing or adding") leaves out.

## Related

- [DYD-1520 setup audit](./setup.md) - The batch's verdicts, mutation and red proofs
