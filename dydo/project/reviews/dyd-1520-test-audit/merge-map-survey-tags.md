---
area: project
type: reference
---

# DYD-1520 Merge, Map and Survey Tag Clauses

Every job tag in the [merge-map-survey batch](./merge-map-survey.md), one per test method or feature
file. No class carries a tag, so each tag covers only the cases of its own method. The text in
parentheses is split into clauses here; each clause names the assertion line that fails if it is
false, in every case the tag covers. A theory's line runs once per row, and a "by row" clause holds
row by row. Lines are in `DynaDocs.Tests/<Area>/<Class>.cs` at the fix commit; `→` follows a call
into a helper. The cite before the parentheses is the source each clause restates. Assertions no
clause names stay, unclaimed. `CliEndToEndTests.Check_WithoutInit_ShowsError` is kept `unsure` and
carries no tag.

## Merge/CommitChecksTests

The refusal is `Refusal()`'s string; an exact `Assert.Equal` against a non-null string fails when
the checks pass, and one against null fails when they refuse.

- `GreenSuiteAndRun…`: no refusal L31.
- `ARunThatHasNotCompleted…` (6 rows): the run in the row's status refuses L46.
- `ASuiteWithRunsThatHasNotCompleted…` (6 rows): refuses L61; naming that status L61.
- `AGreenConclusion_Passes` (3 rows): no refusal L74.
- `AnyOtherConclusion…` (7 rows): refuses L90.
- `ACompletedRunWithoutAConclusion…`: refuses L99.
- `EveryProblemIsNamed`: the failed and the unfinished run refuse beside a green one L108.
- `AFailureBeyondPageOne_IsRead`: page two is read and its failed run refuses L121.
- `ALegacyStatusThatIsNotSuccess…` (4 rows): refuses beside a success status L134.
- `ARequiredCheckMissing…`: the missing check refuses L146; the two provided are not reported L146 (exact).
- `RequiredChecksOfProtectionThatIsOff…`: no refusal L155.
- `ARulesetsRequiredCheckMissing…`: the missing check refuses L169; the provided one is not reported L169 (exact).
- `ARepositoryWhosePlanHasNoRulesets…` (trap): no refusal L179.
- `RulesetsGitHubWillNotShow…`: a tool error L188; exit code 2 L190; GitHub's message L191.
- `ZeroSuites_IsNoChecks`: `no checks reported` L201.
- `AnInertAppsKnownUnfinishedSuite…` (5 rows): ignored, no refusal L216.
- `OnlyAnInertAppsQueuedSuite…`: `no checks reported` L226.
- `AnInertAppsSuiteWithARun…`: refuses L235; naming slug and status L235.
- `AnotherAppsUnfinishedSuite…` (3 rows): refuses L247; naming slug and the row's status L247.
- `ACompletedSuiteThatHoldsNoRun…`: refuses L257.
- `AnInertAppsCompletedEmptySuite…` (3 rows): the success row passes, the others refuse L269.
- `UnreadableChecks…`: a tool error L278; exit code 2 L280; an `error:` line L281.

## Merge/PullRequestMergeTests

`AssertLandedOnce` is L38 (one write) and L39 (a POST /merges naming the base and the SHA).

- `ARefusal_ExitsOne…` (54 rows): exit 1 L224; a `refused:` line L225; nothing written L226.
- `GitHubRefusingThePost…` (2 rows): exit 1 L259.
- `AnUnexpectedMergeAnswer…` (3 rows): exit 2 L275.
- `GhThatCannotReachGitHub…` (2 rows): exit 2 L287; nothing written L288.
- `ARefMoveThatFailsOnAnUnmovedTip…` (10 rows): exit 2 L308; the base on the pin L309.
- `AMergeCommitGitHubDoesNotName…` (trap, 3 rows): exit 2 L323; no ref move sent L324; the base on the pin L325.
- `ARepositoryWithoutANodeId…`: exit 2 L335.
- `AMergeWhoseFirstParentIsNotProven…` (3 rows): exit 2 L352.
- `AReviewedGreenPr…` (2 rows): exit 0 L367; origin's host L368; a POST /merges naming the base and the SHA L369 → L39.
- `APassNamingNoBase_StillLands`: exit 0 L380; a POST /merges naming the base and the SHA L381 → L39.
- `ADryRun…` (2 rows): exit 0 or 1 by row L393; nothing written L394.
- `BaseTipAncestorOfTheSha…`: exit 0 L405; the base moves onto the commit L406; one commit written L407; parents the pin then the SHA L408.
- `ATargetThatMovesBeforeTheRefMove…` (2 rows): exit 1 L423; the base where the other merge put it L424.
- `ABaseGoneBeforeTheRefMove…`: exit 1 L435.
- `ARefMoveWhoseAnswerWasLost…`: exit 0 L449.
- `AfterALanding…` (8 rows): exit 0 L484; a POST row's only write L486 → L38, L39; a two-parent row's base on the merge commit L488.
- `AClosedPrAlreadyInItsFeatureBase…` (2 rows): exit 0 L507; nothing written L508.
- `AMergeGitHubAnswers204…`: exit 0 L519.
- `ARerunAfterALostMergeAnswer…` (2 rows): the lost answer exits 2 L542; the rerun exits 0 L543.
- `AStaleBaseShaWithThePin…`: exit 0 L560; a POST /merges naming the base and the SHA L561 → L39.
- `ThePinOnlyRefuses…` (8 rows): exit 0 on the tip, 1 off it L583; writes only on a real run on the tip L584; each write's base, SHA, first parent and ref move L585 to L591.

## Merge/IndependentReviewTests

`Only` is L14 (exactly one verdict).

- `CanonicalAndColonFields…` (4 rows): one verdict L26 → L14; equal to the canonical line's L28; candidate L29; base L30.
- `ColonFields_MergeWith…`: exit 0 L42.
- `AnUnfencedLine_Counts`: one verdict L49 → L14; PASS L51; rubric L52; candidate L53; base L54; the line L55.
- `AFencedLine_CountsAlike`: one verdict L62 → L14; candidate L64.
- `AnyDashAfterPass…` (5 rows): one verdict L76 → L14; rubric code L78; candidate L79.
- `ABacktickedCandidate`: one verdict and its candidate L86 → L14.
- `MissingContractAndBaseFields`: one verdict L93 → L14; candidate L95; no base L96.
- `TrailingDeferredFindings`: one verdict L103 → L14; candidate L105; base L106.
- `AnUpperCaseSha_ReadsLowerCase`: one verdict and its candidate L113 → L14.
- `NoCandidateField…` (3 rows): one verdict with no candidate L123 → L14.
- `AFailLine_IsAVerdictToo`: one verdict L130 → L14; not a pass L132; candidate L133.
- `OnlyTheIndependentReviewSectionCounts`: the one verdict is the section's L154 → L14.
- `ProseThatMentionsAPass…`: no verdict L161.
- `ARealBodyWithTwoAbbreviatedPasses` (trap): exactly two verdicts, in order, with rubric, candidate and base L174.
- `AFullShaMustEqualTheHead`: exit 1 L189; a `refused:` line L190.
- `ASevenHexPrefix…`: exit 0 L203.
- `AnAmbiguousPrefix…`: exit 1 L216; a `refused:` line L217; nothing written L218.
- `AFailWhosePrefixResolves…`: exit 1 L231; a `refused:` line L232.
- `ACodeDocsOrMergePass_Merges` (3 rows): exit 0 L247.
- `APassWithAnAllowedRubric…`: exit 0 L259; the Review trailer L260.

## Merge: FeatureBranch, MergeCheckout, MergeCommand

- `FeatureBranchTests.AFeatureBranch_Passes` (3 rows): valid L24. `AnythingElse_IsRefused` (14 rows): not valid L45.
- `MergeCheckoutTests.OriginUrls…` (9 rows): the row's host, acme and widgets L32. `OtherOrigins…` (6 rows): no repository L45. `AnSshHostWithoutADot…` (trap, 4 rows): alias or not, by row L56. `OriginsInsteadOfRewrite…`: the rewritten origin L68. `TheLocalOriginHead…`: `trunk` L78. `ANestedRepository…`: exit 1 L92; a `refused:` line L93. `ASubdirectoryOfTheCheckout…`: exit 0 L106. `AnSshAlias_IsAToolError`: exit 2 L117; an `error:` line L118. `GhRepoGhHostAndGitDir…`: exit 0 L140; origin's host L141; origin's repository L142; `gh api` with `--hostname` L143; no GH_REPO L144; no GH_HOST L145.
- `MergeCommandTests.TheCommand_MergesThePr…`: exit 0 L30; PR #7 read L31; nothing written L32. `AnythingButAFullBasePin…` (6 rows): exit 1 L48; a `refused:` line L49; no GitHub call L50. `AnUpperCaseBasePin…`: exit 0 L64; nothing written L65. `WithoutPins…`: exit 1 L81; a `refused:` line L82.

## Map

- `LinearGraphQLTests.TheKeyIsSentBare` (trap): the bare key L22. `ALinearFailure_MapsToOneError` (10 rows): named, by the row's code and status L45, L47; never the key L48. `UnreachableOrSlowLinear…` (2 rows): the one `linear_error` failure L61, L63.
- `LinearReaderTests.Teams_AreReadAcrossEveryPage`: every team on both pages L34 → L22. `Projects_OfOneTeam…`: status L52; target date, completedAt and canceledAt L53; each null when unset L55. `AnUnknownTeamOrProject…`: `not_found` and 404 for the team L64, L67 and the Project L65, L68. `Graph_ReadsEveryIssuePage`: all 43 issues L86 → L22. `Graph_CarriesEachIssuesFields`: the Project L101; each field of issue 1 L103; its label L106; issue 2's state type and parent L107; no labels for 2, 3 and 4 L109. `Graph_KeepsBlocksAndRelatedOnce…`: exactly two relations L126; blocks and related L127 → L22; no external issue L129. `Graph_FarEnds…`: only the Project's own issue L145; both far ends external L147 → L22; with their Project L148 or none L149; their relations kept L150 → L22. `Graph_DropsArchivedIssues…`: the archived member dropped L170; the archived far end dropped L171; both relations dropped L172. `Graph_AResolvedBlocker…`: the one blocks relation L185. `Graph_RelationOverflow…` (edge): every relation L213 → L22; every far end L214 → L22. `Graph_OverflowOnAnIssue…`: a failure L225; `not_found` L227. `ReadProject_CarriesTheBody…`: the Project L242; its body L243; each priority, the far end's included L244; the one parent identifier L245. `ReadProject_WithoutABody…` (2 rows): the empty body L259 or `not_found` L261, by row.
- `MapCommandTests.MissingKey_PrintsTheKeyHelp…` (3 rows): exit 2 L21; the help L22. `MissingKey_ThroughTheCommandLine…`: exit 2 L38. `WithAKey_ServesOnLocalhost…` (2 rows): exit 0 L60; the localhost URL L61; not the key L62; the browser row opens one localhost URL L64.
- `MapServerTests.Start_BindsLocalhost…`: localhost L51; a picked port L52. `Start_RetriesWithAFreshListener…` (edge): the next probed port L68. `Saved_FirstVisit…`: 200 L86; a null snapshot L87; Linear not called L88. `Graph_AnswersTheReadGraph…`: 200 L100; the issue L102; the far end L103; the blocks relation L105; not the key L107. `Graph_IsNotCached…`: Todo L121, then In Progress L122. `MissingParameter…` (3 rows): 400 `missing_parameter` L134. `LinearFailures…` (6 rows): the row's status and code L158; not the key L159. `AnUnknownPathOrANonGetMethod…` (5 rows): 404 `not_found` L173.
- `Features/map-memory.feature` (`Map/MapMemorySteps.cs`), the amendment's rules by scenario: a fresh read L46, L51 and its saved map L57, L65 to L68; the saved map after a restart L82, L65 to L68, read without Linear L72; kept on a failed fetch L90, L66, replaced on success L66; missing for another Project, key or endpoint L101; a miss when unusable L101, then a fresh fetch L51; one complete map from simultaneous writers L131 to L133; another Project's saved map while a fetch is blocked L66, the fetch ending with the server L153, L155.

## Survey

- `SurveyCommandTests.NotExactlyOneProject…` (3 rows): exit 2 L35. `MissingKey_PrintsMapsKeyHelp…` (2 rows): exit 2 L50; exactly the help L51; Linear not called L53. `ARead_ExitsZero…`: exit 0 L66; the UTC header L67; the one kept finding on its own line L68. `AnArchivedBlocker_IsNoBlocker`: exactly `pickable DYD-1` L83. `ATicketMissingMidRead…`: exit 2 L99; the ticket named L100; not the unknown-Project message L101. `AnUnexpectedAnswer…` (2 rows): exit 2 L116; a message on stderr L117. `ACanceledRun…` (edge): the throw L125.
- `SurveyRulesTests.Unblocked_AnOpenUnassignedIssue…` (4 rows): exactly DYD-1 L58. `Unblocked_IncludesAnIssueLinked…`: exactly DYD-9 L68. `Unblocked_NotWhile…`: no unblocked issue L79. `Pickable_IsTheViewersRule…`: pickable exactly DYD-1 L91; unblocked exactly DYD-2 L92. `Uncontracted…`: exactly DYD-1 and DYD-5 L105; the reason L106; pickable exactly DYD-7 L108. `QuestionNoPriority…`: exactly DYD-1 L120. `Findings_AreSorted…`: the exact order L130. `AQuietProject…`: no kept finding L138.
- `Features/survey.feature` (`Survey/SurveySteps.cs`), by scenario: the findings in order L100, nothing on stderr L101, exit 0 L95; each argument's exit L95 and the Project asked for or none L118, L120; the unknown Project's exit L95 and message L107; a Linear failure's exit 2 L95, named L107, no digest L108; the shared key help L112, exit 2 L95, no request L118; every request a query L126 to L131.

## Workflow, EndToEnd and review-pass-grammar

- `NodeTestGuardTests.Guard_Accepts…`: exit 0 L24; every counter L25. `Guard_RejectsAMissingReport`: non-zero L34; no TAP report L35. `Guard_RejectsAReportWithoutEverySummaryCounter` (2 rows): non-zero L46; the missing counters L47. `Guard_RejectsAReportThatProves…` (6 rows): non-zero L63; the row's reason with every counter L64. Each waits for pwsh to exit, so a hang fails the run rather than passing.
- `ViewerPackTests.Pack_WithoutViewer…` (2 rows): fails L48; both pnpm lines L49, L50; no package L51. `Pack_NoBuild_RejectsAViewerlessAssembly…`: fails L63; rebuild L64; without --no-build L65; no package L66. `Pack_NoBuild_RejectsAViewerlessPublishOutput…`: fails L82; DYDO002 L83; no package L84. `Pack_WithViewer…` (2 rows): pack succeeds L98 → L122; one package L100; index.html L107; the asset L108.
- `CliEndToEndTests.Help_DoesNotCrash` (edge): exit 0 L58; text naming dydo L60. `Subcommand_Help…` (edge, 7 rows): exit 0 L76. `Init_CreatesProject` (2 rows): exit 0 L92; dydo.json L94; dydo/ L96. `Init_RecordsTheVersion…`: init exit 0 L105; dydoVersion is `dydo version`'s L109. `Init_ThenCheck_Passes`: init exit 0 L117; check exit 0 L121. `Init_All…`: init exit 0 L130; the spawn depth L131; the thread limit L134; join exit 0 L137; config.toml byte for byte L138. `Check_Subfolder…` (trap): exit 0 or 1 L160; no broken link L162. `Check_NonExistentPath…`: exit 2 L174. `Check_PathOutsideDocsTree…`: exit 2 L190.
- `Features/review-pass-grammar.feature` (`Merge/ReviewPassGrammarSteps.cs`): both accept, pr-ready exit 0 L45 and the merge dry run exit 0 L46; both refuse, exit 1 L52, L53.

## Related

- [DYD-1520 merge, map and survey audit](./merge-map-survey.md) - The batch's totals and evidence
- [Test rubric](./rubric.md) - §2 row 2, a true job tag
