---
area: project
type: reference
---

# DYD-1520 Audit: Docs Batch Tag Clauses

The clause-to-line map of the [docs batch](./docs.md) audit (review lesson 10). Each tag split into clauses; for each clause, the assertion line that fails, in every case the tag
covers, if the clause were false. Lines are in `DynaDocs.Tests/` at `3cace6b40`; `CDV` is
`Commands/CheckDocValidatorTests.cs`, `FCI` `Integration/FixCommandIntegrationTests.cs`.

| Tag at | Covers | Clause → failing line |
|---|---|---|
| `Commands/CheckCommandTests.cs:8` | `RootOption…`, 13 rows | exit code matches the outcome → `:55` with `:59` (rows 1-6, 8-12) or `:57` (rows 0, 7) |
| `Commands/HelpCommandTests.cs:7` | `Help_ListsEveryRegisteredCommand` | names every registered command → `:25` (`:24` non-empty); not starting with `_` → the filter at `:15`, then `:25` |
| `Commands/ValidateCommandTests.cs:34` | `Validate_ReportsTheFindingAndExitCode`, 30 rows | reports the outcome → `:82` (finding rows) or `:80` (passing rows); exits with its code → `:78` |
| `ValidateCommandTests.cs:86` | `Validate_InvalidDydoJson_…` | deserialization error reported → `:94`; exit 1 → `:93` |
| `ValidateCommandTests.cs:99` | `Validate_UnreadableClaudeSettings_…` | warning to make them readable → `:111`; exit 0 → `:110` |
| `CDV:23` | `Check_ReportsTheDocumentsFinding`, 37 rows | exit code → `:74`; the file's error named → `:78` (finding rows); the file not named → `:76` (other rows) |
| `CDV:82` | `Check_ScanExcludedFolder_IsNotReported`, 2 rows | nothing reported under an excluded path, listed (row 0) or required (row 1) → `:93`, `:94` |
| `CDV:98` | `Check_TypeVocabulary_…`, 6 rows | a type accepted exactly when the baseline or the file's readable strings hold it → `:114`, with `:116` (accepted rows) or `:118` (rejected rows) |
| `CDV:122` | `Check_LinkFindings_…`, 22 rows | exactly the document's link findings → `:166`; exit 1 with any, 0 with none → `:167` |
| `CDV:171` | `Check_AnchorOnlyLink_…` | exactly `Broken link: #missing` → `:180`; exit 1 → `:181` |
| `CDV:185` | `Check_LegacyHubMarker_…`, 3 rows | link checks skipped exactly on the marked page → `:198` (row 0 lists no broken link, rows 1-2 list it) |
| `CDV:207` | `Check_ScopedToAFolder_…` | the `<path>` scopes the report → `:220`; exit 1 on its errors → `:217`, `:218` |
| `Commands/FixFileHandlerTests.cs:14` | `Fix_ConvertsResolvableWikilinks_…` | resolvable becomes relative → `:25`; unresolvable stays → `:26` |
| `FixFileHandlerTests.cs:31` | `Fix_RenamesNonKebabFiles_…` | a rename onto an existing file loses nothing → `:41`, `:42`; filename normalization → `:43`, `:44` |
| `FixFileHandlerTests.cs:48` | `Fix_ValidDocument_KeepsItsBytes`, 2 rows | keeps its bytes → `:60`; exit 0 → `:59` |
| `Rules/FutureFeatureRuleTests.cs:17` | `Check_ReportsTheFutureFeatureFinding`, 13 rows | check enforces the legacy shape → `:40`, with `:42` (3 passing rows) or `:44` (10 finding rows) |
| `FutureFeatureRuleTests.cs:48` | `Check_DocumentOutsideTheFutureFeaturesFolder_…`, 2 rows | a document in another folder is not held to it → `:58`, `:59` |
| `Rules/OffLimitsRuleTests.cs:14` | `Check_ReportsTheOffLimitsFinding`, 5 rows | validates the off-limits file → `:30`, with `:34` (rows 0-3) or `:32` (row 4) |
| `OffLimitsRuleTests.cs:38` | `Check_ScopedBelowTheDocsRoot_…` | scoped, so the root's off-limits file is not reported → `:48`; exit → `:47` |
| `FCI:10` | `Fix_ValidationErrors…`, `Fix_PostWorkFailure…`, `Fix_NoConfigChange…` | no successful configuration change → `:27`; `:47`; `:68`, `:69`. Bytes kept → `:29`; `:52`; `:70` |
| `FCI:11` | `Fix_StaleAuditScanExcludeEntry…`, `Fix_ConfigWithoutAuditScanExcludeEntry…` | required scan exclusions restored → `:89`, `:92`; `:107` |
| `FCI:130` | `Fix_ExplicitStandaloneFile_Succeeds` | named in the output → `:139`; exit 0 → `:138` |
| `FCI:145` | `Fix_ExplicitFile_OnlyFixesSelectedFileAfterRename` | name repaired → `:162`, `:163`, `:171`; wikilinks repaired → `:165`, `:166`; no other document's → `:167`, `:169`, `:170`, `:172` |
| `FCI:177` | `Fix_ExplicitFileBesideProjectConfig_…` | filename normalization → `:187`-`:189` |
| `FCI:194` | `Fix_ExplicitFileInLegacyDocs_…` | filename normalization → `:207`, `:208`; wikilink conversion → `:210`-`:212` |
| `FCI:221` | `Fix_ExplicitDirectory_ResolvesSiblingLinks…` | a document in it repaired → `:235`-`:237`; one outside not → `:238`-`:240` |
| `FCI:244` | `Fix_ExplicitDirectory_HonorsRootRelativeScanExclude` | an excluded path not repaired → `:259`, `:260` |
| `FCI:264` | `Fix_ExplicitDirectoryOutsideDocsTree_…` | exit 2 → `:273`; named in the error → `:274` |
| `FCI:282` | `Fix_IgnoresObsidianFolder` | reports nothing on it → `:296`; writes no hub there → `:297` |
| `FCI:301` | `Check_IgnoresObsidianFolder` | reports nothing on them → `:317` (exit `:316`) |
| `FCI:323` | `Fix_PreservesHandwrittenNavigationFiles` | `_index.md` text kept → `:335`; `_custom.md` → `:336`; the document stays → `:337` |
| `Integration/ExplainsCommandTests.cs:40` | `Check_MalformedDeclarationIsAnError`, 9 rows | an invalid declaration is a check error → `:56`-`:58` |
| `ExplainsCommandTests.cs:62` | `Check_EveryGlobMustMatchAFile`, 2 rows | a check error → `:71`, `:73`; naming the glob → `:72` |
| `ExplainsCommandTests.cs:77` | `Check_ValidDeclarationsKeepNormalSuccessOutput`, 5 rows | passes check → `:89`-`:91`; fails once nothing matches → `:97`, `:98` |
| `ExplainsCommandTests.cs:102` | `PrReady_UnchangedDocWarns…` | warns → `:113`, `:117`; without blocking → `:111`, `:112`; the doc → `:114`; every path → `:115`, `:116` |
| `ExplainsCommandTests.cs:121` | `PrReady_UpdatedDocAndUnrelatedCode…` | clears once the doc changes → `:129` then `:136`; none outside the paths → `:144` |
| `ExplainsCommandTests.cs:148` | `PrReady_ExplicitFeatureBase…` | renamed flagged → `:161`; deleted flagged → `:162`; the doc named → `:160` |
| `ExplainsCommandTests.cs:166` | `Check_DiffWarningIsAdvisory…` | a warning → `:173`-`:175`; exit 0 → `:172`; updating the doc clears it → `:180` |
| `ExplainsCommandTests.cs:184` | `PrReady_MissingBaseWarnsWithoutBlocking` | warns → `:192`; stays ready → `:190`, `:191` |
| `ExplainsCommandTests.cs:196` | `Check_MissingBaseWarnsWithoutBlocking` | warns → `:202`, `:203`; exit 0 → `:201` |
| `Integration/DocumentationTests.cs:16` | `Fix_OnAnInitialisedProject_…` | no file gained and no byte changed, navigation pages and the manual-fix document included → `:30` (a snapshot of every file's name and bytes); exit 0 → `:29` |
| `DocumentationTests.cs:34` | `Index_RegeneratesTheRoot_…` | regenerates → `:43`, `:45`; from the configured structure → `:46`; check passes it → `:47` |
| `DocumentationTests.cs:51` | `Index_SectionWithoutItsOptionalNavigationPage_…` | optional → `:59`; a missing one not linked → `:60` |
| `DocumentationTests.cs:77` | `GraphStats_Top_…` | ranks at most N → `:85` |
| `DocumentationTests.cs:89` | `NoDocsFolder_IsAToolError`, 3 rows | exit 2 → `:104` |
| `DocumentationTests.cs:108` | `Graph_UnknownFile_IsAToolError` | exit 2 → `:115` |
| `Integration/ChangelogEvidenceTests.cs:25` | `CommittedReceipt_RefusedByBothCommands_…`, 8 rows | check refuses → `:48`-`:50`; pr-ready refuses → `:48`, `:51`, `:52` |
| `ChangelogEvidenceTests.cs:56` | `SymlinkedProjectPath_…`, 2 rows | check refuses → `:73`, `:74`; pr-ready → `:73`, `:75`; both rows behind the symlink |
| `ChangelogEvidenceTests.cs:79` | `OwnDocsLinkedIntoAnotherRepository_…` | fails closed → `:93`, `:94` |
| `ChangelogEvidenceTests.cs:98` | `NestedRepository_…`, 2 rows | reads its own changelog, not the outer project's → `:118`, `:120` (row `false`); `:119` (row `true`) |
| `ChangelogEvidenceTests.cs:124` | `HumanMarkdownAndIgnoredLocalOutput_AreAllowed` | check passes → `:140`, `:141`; pr-ready passes → `:142` |
| `Services/DocGraphTests.cs:24` | `Graph_ShowsTheLinkedDocuments`, 7 rows | shows what the docs link → `:37`; nothing else → `:38` (exit `:36`) |
| `DocGraphTests.cs:42` | `Graph_UnknownDocument_IsAToolError` | exit 2 → `:47`; named → `:48` |
| `Services/LinkResolverTests.cs:21`, `:38` | `Resolve_HandlesEveryRelativeShape…` 6, `Resolve_AgreesOnAnchored…` 2 | check and graph agree on the target → check exit 0 `:67` (called `:33`, `:47`); graph's one target `:75`, `:76` (called `:34`, `:48`) |
| `LinkResolverTests.cs:52` | `Resolve_DisagreesIntentionallyOnAnchorOnlyLinks` | passes check, exit 0 → `:67` (called `:58`); no graph edge → `:60`, `:61` |
| `Services/PathUtilsTests.cs:14` | `CollapseRelativeSegments_ResolvesDotDot`, 9 rows | the public overload's exact result on the running host → `:26` |
| `PathUtilsTests.cs:70` | `CollapseRelativeSegments_BothReadings_OnAnyHost`, 31 rows | exact Windows reading → `:74`; exact other-host reading → `:75`; on any host: neither line is platform-conditional |
| `Serialization/JsonTextTests.cs:9` | the four `Render_*` layout cases | only what changed, the layout kept → each case's one exact-equality line, `:19`, `:30`, `:41`, `:53` |
| `JsonTextTests.cs:10` | `Merge_AppliesOnlyTheChange…`, `Merge_AppendedItem…` | into the file's own value → `:86`; `:108`. Only the change → `:87`; `:109` |
| `JsonTextTests.cs:57` | `Render_EmptyObjectGainingAMember…` | opens on its own lines → `:63` |
| `JsonTextTests.cs:67` | `Render_NoParseableOriginal…`, 3 rows | a fresh LF document → `:73` |
| `JsonTextTests.cs:92` | `Merge_ArrayTheModelReshaped…` | takes the update → `:97` |
| `JsonTextTests.cs:113` | `Write_MalformedUtf16File…` | encoding, BOM and unparseable body → `:124` |
| `JsonTextTests.cs:133` | `Encode_NewFile_IsUtf8WithoutBom` | UTF-8 without BOM → `:136` |
| `Features/optional-summaries.feature:1` | 9 cases | the result DYD-98's Outcome ("summary absence is valid, title and broken-link checks remain; fix does not demand a summary") gives the document → check rows 0-3 (summary absence valid) `Steps/OptionalSummarySteps.cs:56`, `:57`; check rows 4-5 (title and broken-link checks remain) `:63`, `:66`, `:68`; fix rows `:40`, `:72`; manual repair `:40`, `:72`, then `:63`, `:66`, `:68` |
| `coverage/tests/test_control_dydo.py:52` | `test_cleanup_deletes_only_scratch…` | deletes RUN/scratch → `:56`; only → `:57`; repeatable → `:53`, `:54` |
| `test_control_dydo.py:61` | `test_cleanup_refuses_a_run_it_does_not_own…` | the exact run → `:91`, `:92`; its tree → `:91`, `:92`, `:95`; before deleting → `:94`; each subtest |
| `test_control_dydo.py:98` | `test_a_busy_run_belongs_to_its_live_driver` | → `:101`, `:102` (the error), `:104` (lock kept), `:105` (scratch kept) |
| `test_control_dydo.py:108` | `test_doctor_on_a_changed_candidate…` | refused → `:123`, `:124`, `:129`; before the candidate runs → `:127` |
| `coverage/tests/test_row_deadlines.py:75` | `test_windows_python_coverage_can_finish…` | finishes beyond 1800 s → `:79`, `:80` |
| `test_row_deadlines.py:85` | `test_windows_python_coverage_hang…` | bounds hangs → `:89`-`:93`; halts dispatch → `:94` |
| `test_row_deadlines.py:97` | `test_other_rows_keep_the_original_deadline` | another row keeps 1800 s → `:106`, all six combinations, both runners |
| `coverage/tests/test_runner.py:75` | `test_the_test_runs_exit_code…` | the runner exits with the run's code → `:80`, codes 0, 1, 7 |
| `test_runner.py:83` | `test_the_run_sees_the_uncommitted_working_state` | reaches the run → `:88`; outside the caller's checkout → `:87` |
| `test_runner.py:91` | `test_the_run_writes_no_shell_profile` | never reaches the profile → `:98`, `:99`, bash and zsh |
| `test_runner.py:102` | `test_the_worktree_is_gone…` | removed → `:108`; unregistered → `:109`; exit codes 0 and 1 |
| `coverage/tests/test_statechart_java_build.py:129` | `test_a_verified_build…` | builds a jar → `:132`; exactly the locked classes → `:134`; receipt names its hash → `:136`; verified → `:137` |
| `test_statechart_java_build.py:140` | `test_a_tampered_archive…` | never executed → `:124` via `assert_refused` (`:123`, `:125`, `:126`) |
| `test_statechart_java_build.py:148` | `test_an_extracted_jdk_unlike_its_archive…` | each of the three refused before javac → `:164` (`:123` refused, `:124` javac not run) |
| `test_statechart_java_build.py:168` | `test_a_foreign_release…` | refused before javac → `:171` (`:123`, `:124`) |
| `coverage/tests/setup-skills.test.cjs:48` | `both host roots link every skill flat…` | every skill flat → `:59`; linked → `:61`; both roots → the loop at `:57` |
| `setup-skills.test.cjs:66` | `a rerun exits zero…` | safe to rerun → `:76`, `:77` |
| `setup-skills.test.cjs:81` | the three collision tests | refused → `:106`; every entry as it was → `:107`; each test |

## Related

- [DYD-1520 Audit: Docs Batch](./docs.md) - The batch's totals, input audit, mutation and red proofs
- [Test rubric](./rubric.md) - §2 row 2: a tag is true only when it is true of every case it covers
