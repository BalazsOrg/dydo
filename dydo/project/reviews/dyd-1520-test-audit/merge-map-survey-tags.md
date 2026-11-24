---
area: project
type: reference
---

# DYD-1520 Merge, Map and Survey Tag Clauses

Every job tag in the [merge-map-survey batch](./merge-map-survey.md), one per test method or feature
file, in the minimal form: the kind, one behaviour, and its cite. No class carries a tag, so each tag
covers only its own method's cases. Each line below gives the tag's one behaviour and the assertion
line, in `DynaDocs.Tests/<Area>/<Class>.cs`, that every case of the method runs and that fails when
the behaviour is broken. The other assertions stay, unclaimed. `AssertLandedOnce` is
`PullRequestMergeTests.cs` L38 to L39. "The visual Linear project map plan" is
[the plan at b995a19](https://github.com/BalazsOrg/dydo/blob/b995a1927c9ad29a78f35fc9ee1d5f82ee3b358c/dydo/project/plans/visual-linear-project-map.md),
whose §3 HTTP API contract DYD-264's Outcome serves exactly. "DYD-417 design e6a6affe" is the design
comment DYD-424, which built `dydo merge`, names as its governing contract.

## Merge/PullRequestMergeTests

- `ARefusal_ExitsOne_AndWritesNothing` (contract): each refusal exits 1, L224.
- `GitHubRefusingThePost_ExitsOne_AndNothingElseIsWritten` (contract): a conflict (409) or a base gone (404) is refused, L259.
- `AnUnexpectedMergeAnswer_IsAToolError` (contract): an unexpected GitHub answer such as a 403, 422 or ruleset rejection is a tool error, L275.
- `GhThatCannotReachGitHub_IsAToolError_WithNothingWritten` (contract): the network is a tool error, L287.
- `ARefMoveThatFailsOnAnUnmovedTip_IsAToolError_AndTheTargetStaysOnThePin` (contract): a failed ref move with the base still at the pin is a tool error, L308.
- `AMergeCommitGitHubDoesNotNameAsACommit_IsAToolError_BeforeTheRefMoves` (trap): a ref move to an all-zero afterOid deletes the ref (6f702ccb5, DYD-523), L324.
- `ARepositoryWithoutANodeId_IsAToolError_BeforeAnyWrite` (contract): an unexpected GitHub answer is a tool error, L335.
- `AMergeWhoseFirstParentIsNotProvenThePin_ExitsTwo` (contract): afterwards it checks that the merge commit's first parent is the pin, L352.
- `AReviewedGreenPr_LandsOnceAsAMergeIntoItsFeatureBase` (contract): POST repos/<owner>/<repo>/merges names the validated base and the SHA, L369.
- `APassNamingNoBase_StillLands` (contract): when the PASS names no base, the base check is skipped, L381.
- `ADryRun_ChecksLikeAMerge_AndWritesNothing` (contract): --dry-run makes every read and check and merges nothing, L394.
- `BaseTipAncestorOfTheSha_LandsATwoParentCommit_OnThePin` (contract): when the pin is an ancestor of the SHA, it writes a two-parent commit, parents the pin and the SHA, L408.
- `ATargetThatMovesBeforeTheRefMove_RefusesAndNothingMerges` (contract): a base that moved meanwhile, even to a commit in the SHA's history, fails that move, L424.
- `ABaseGoneBeforeTheRefMove_RefusesAndNothingMerges` (contract): a base gone is refused, L435.
- `ARefMoveWhoseAnswerWasLost_ButLanded_ExitsZero` (contract): exit 0 for a merge, L449.
- `AfterALanding_WhateverTheRereadsSay_ItExitsZero` (contract): exit stays 0 for yes, pending and unverified, L484.
- `AClosedPrAlreadyInItsFeatureBase_ExitsZero_WithoutAWrite_WhateverThePin` (contract): a closed PR whose SHA is already in its feature base exits 0 with any valid pin, L507.
- `AMergeGitHubAnswers204_IsAlreadyIntegrated_AndExitsZero` (contract): a 204, already merged, is integrated: exit 0, L519.
- `ARerunAfterALostMergeAnswer_ExitsZero_WithoutASecondMerge` (contract): a rerun after a lost answer is safe, L543.
- `AStaleBaseShaWithThePinOnTheActualRef_LandsOntoTheRef` (contract): the base's tip is read from its branch ref, never from the PR's base.sha, L561.
- `ThePinOnlyRefuses_EveryWriteTargetsTheBaseRefAndAPinOffTheTipWritesNothing` (contract): the pin can only refuse, L584.

## Merge/CommitChecksTests

- `GreenSuiteAndRunWithNoLegacyStatuses_IsGreen` (contract): the combined status counts only when its total_count is above 0, L31.
- `ARunThatHasNotCompleted_IsNotGreen` (contract): every latest check run must finish green, L46.
- `ASuiteWithRunsThatHasNotCompleted_IsNotGreen` (contract): any unfinished suite then refuses, whatever its run count, L61.
- `AGreenConclusion_Passes` (contract): suites and runs must finish with success, neutral or skipped, L74.
- `AnyOtherConclusion_IsNotGreen` (contract): suites and runs must finish with success, neutral or skipped, L90.
- `ACompletedRunWithoutAConclusion_IsNotGreen` (contract): suites and runs must finish with success, neutral or skipped, L99.
- `EveryProblemIsNamed` (contract): every latest check run must finish green, L108.
- `AFailureBeyondPageOne_IsRead` (contract): every latest check run is read to the last page, L121.
- `ALegacyStatusThatIsNotSuccess_IsNotGreen` (contract): legacy statuses must be success, L134.
- `ARequiredCheckMissing_IsNotGreen` (contract): every check the target's branch protection requires must be present, L146.
- `RequiredChecksOfProtectionThatIsOff_AreNotRequired` (edge): branch protection that is off while it still lists a required context, L155.
- `ARulesetsRequiredCheckMissing_IsNotGreen` (contract): every check the target's rulesets require must be present, L169.
- `ARepositoryWhosePlanHasNoRulesets_RequiresNoRulesetChecks` (trap): a private repository on GitHub's free plan answers rulesets with a 403 (bodnarbalazs/LC, DYD-424), L179.
- `RulesetsGitHubWillNotShow_AreAToolError` (contract): an unexpected GitHub answer such as a 403 is a tool error, with GitHub's message, L191.
- `ZeroSuites_IsNoChecks` (contract): with no suite left, the command refuses no checks reported, L201.
- `AnInertAppsKnownUnfinishedSuiteThatHoldsNoRun_IsIgnored` (contract): the inert app's suite is ignored only when its status is queued, in_progress, pending, requested or waiting and latest_check_runs_count is a valid integer zero, L216.
- `OnlyAnInertAppsQueuedSuiteThatHoldsNoRun_IsNoChecks` (contract): with no suite left after the exception, the command refuses no checks reported, L226.
- `AnInertAppsSuiteWithARun_IsJudged` (contract): every non-exempt suite is judged normally, L235.
- `AnotherAppsUnfinishedSuiteThatHoldsNoRun_IsNotGreen` (contract): an unlisted app's unfinished suite refuses even with zero runs, L247.
- `ACompletedSuiteThatHoldsNoRun_StillCounts` (contract): every non-exempt suite is judged normally, L257.
- `AnInertAppsCompletedEmptySuite_IsJudged` (contract): every non-exempt suite is judged normally, L269.
- `UnreadableChecks_AreAToolError` (contract): the network is a tool error, L280.

## Merge/IndependentReviewTests

- `CanonicalAndColonFields_ParseIdentically` (contract): the candidate and base field names accept an optional colon, L28.
- `ColonFields_MergeWithTheSamePinsAndBasePolicy` (contract): the candidate and base field names accept an optional colon, L42.
- `AnUnfencedLine_Counts` (contract): a PASS line counts fenced or not, L51.
- `AFencedLine_CountsAlike` (contract): a PASS line counts fenced or not, L64.
- `AnyDashAfterPass_AndLooseSpacing` (contract): a hyphen, en dash or em dash after PASS, L79.
- `ABacktickedCandidate` (contract): the SHA possibly backticked, L86.
- `MissingContractAndBaseFields` (contract): base is optional, L96.
- `TrailingDeferredFindings` (contract): trailing Deferred findings are fine, L106.
- `AnUpperCaseSha_ReadsLowerCase` (edge): a PASS candidate in upper-case hex, L113.
- `NoCandidateField_NamesNoCandidate` (contract): a missing or invalid candidate field gives no candidate, L123.
- `AFailLine_IsAVerdictToo` (contract): a PASS for the SHA and no FAIL for it, L132.
- `OnlyTheIndependentReviewSectionCounts` (contract): the ## Independent review section, up to the next level-one or level-two heading, L154.
- `ProseThatMentionsAPass_IsNoVerdict` (contract): prose-only PASS lines are refused, L161.
- `ARealBodyWithTwoAbbreviatedPasses` (trap): a real body's two abbreviated PASSes, the merge one naming two parents, misread (bodnarbalazs/dydo#205), L174.
- `AFullShaMustEqualTheHead` (contract): a PASS whose candidate is the SHA, L190.
- `ASevenHexPrefixGitHubResolvesToTheSha_Counts` (contract): an abbreviation counts only when GitHub resolves it to exactly that SHA, L203.
- `AnAmbiguousPrefix_DoesNotCount` (contract): an abbreviation counts only when GitHub resolves it to exactly that SHA, L217.
- `AFailWhosePrefixResolvesToTheSha_Refuses` (contract): a FAIL line for the same candidate refuses, L232.
- `ACodeDocsOrMergePass_Merges` (contract): a code, docs or merge PASS, L247.
- `APassWithAnAllowedRubric_WinsOverAnEarlierSpecPass` (contract): the Review trailer is the PASS line, first 200 characters, L260.

## Merge/FeatureBranchTests

- `AFeatureBranch_Passes` (contract): it must be feature/ and one or more /-separated segments, each starting with a letter or digit, and pass git check-ref-format --branch, L24.
- `AnythingElse_IsRefused` (contract): it must be feature/ and one or more /-separated segments, each starting with a letter or digit, and pass git check-ref-format --branch, L45.

## Merge/MergeCheckoutTests

- `OriginUrls_NameHostOwnerAndRepository` (contract): the repository is the one origin names, as HOST/OWNER/REPO, L32.
- `OtherOrigins_NameNoRepository` (contract): an origin that does not name HOST/OWNER/REPO names no repository, L45.
- `AnSshHostWithoutADot_IsAnAlias` (trap): an SSH config alias host, which gh cannot resolve, taken for a real host (DYD-417 design e6a6affe §2), L56.
- `OriginsInsteadOfRewrite_IsHonoured` (contract): remote get-url origin (honours insteadOf) is parsed as HOST/OWNER/REPO, L68.
- `TheLocalOriginHead_IsReadWithoutItsRemoteName` (contract): the local origin/HEAD name a base is refused for being, L78.
- `ANestedRepository_IsNotTheProjectsCheckout` (contract): the checkout's git top level must be the dydo.json folder, L93.
- `ASubdirectoryOfTheCheckout_FindsTheProject` (contract): commands find the project by walking up to the nearest dydo.json, L106.
- `AnSshAlias_IsAToolError` (contract): an origin on an SSH alias host is a tool error, L117.
- `GhRepoGhHostAndGitDirPointingElsewhere_ChangeNothing` (contract): gh runs as gh api with --hostname set to origin's host and GH_REPO and GH_HOST removed, L143.

## Merge/MergeCommandTests

- `TheCommand_MergesThePrItParsesFromTheCurrentDirectory` (contract): --dry-run makes every read and check and merges nothing, L32.
- `AnythingButAFullBasePin_IsRefusedBeforeAnyCall` (contract): both pins are required and validated before any network call, L50.
- `AnUpperCaseBasePin_IsTheSameTip` (edge): a base pin in upper-case hex, L64.
- `WithoutPins_TheReleasedCommandRefuses` (contract): both are required, L81.

## Map/LinearGraphQLTests

- `TheKeyIsSentBare` (contract): Authorization: <LINEAR_API_KEY> (no Bearer), L22.
- `ALinearFailure_MapsToOneError` (contract): each Linear failure is one of its error codes, with that code's status, L47.
- `UnreachableOrSlowLinear_IsALinearError` (edge): Linear unreachable, or too slow to answer, L63.

## Map/LinearReaderTests

- `Teams_AreReadAcrossEveryPage` (edge): a workspace whose teams span two of Linear's pages, L34.
- `Projects_OfOneTeam_CarryTheirStatusAndDates_OrNullWhenUnset` (contract): Projects group by status, each sorted by target date with undated last, and completed Projects list newest-first, L53.
- `AnUnknownTeamOrProject_IsNotFound` (contract): the not_found (404) error, L67.
- `Graph_ReadsEveryIssuePage` (contract): the Project's non-archived issues, L86.
- `Graph_CarriesEachIssuesFields` (contract): the Graph's project and an Issue's fields, L103.
- `Graph_KeepsBlocksAndRelatedOnce_DropsDuplicateAndSimilar` (contract): dedupe relations by id; keep blocks and related only, L126.
- `Graph_FarEndsOutsideTheProject_AreExternal_WithOrWithoutAProject` (contract): external far-end issues go in external, L147.
- `Graph_DropsArchivedIssues_AndEveryRelationTouchingOne` (contract): archived issues are excluded, together with every relation touching one, L172.
- `Graph_AResolvedBlocker_StaysABlocksRelation` (contract): a resolved blocker keeps its blocks edge, L185.
- `Graph_RelationOverflow_IsReadUntilBothListsEnd` (contract): an IssueRelationsPage follow-up when nested hasNextPage is true, L213.
- `Graph_OverflowOnAnIssueLinearNoLongerHas_IsNotFound` (edge): an issue Linear no longer has when its relation overflow is read, L227.
- `ReadProject_CarriesTheBody_EveryIssuesPriority_AndEachParentIdentifier` (contract): the survey adds the Project body, each issue's priority, and each Project issue's parent identifier, L244.
- `ReadProject_WithoutABody_IsEmpty_AndWithoutAProject_IsNotFound` (edge): Linear answering null for the Project, or for its body, L259.

## Map/MapCommandTests

- `MissingKey_PrintsTheKeyHelp_AndExits2BeforeAnyServer` (contract): without a key it prints how to create one and exits 2, L22.
- `MissingKey_ThroughTheCommandLine_Exits2` (contract): when the key is missing, the command exits 2, L38.
- `WithAKey_ServesOnLocalhost_OpensTheBrowserUnlessToldNot_AndStopsOnCancel` (contract): it prints its http://localhost URL, L61.

## Map/MapServerTests

- `Start_BindsLocalhostOnAFreePort` (contract): http://localhost:<free port>/, L51.
- `Start_RetriesWithAFreshListener_WhenTheFirstProbedPortIsTaken` (edge): the probed free port taken before the listener binds it, L68.
- `Saved_FirstVisitIsAnExplicitMiss_WithoutReadingLinear` (contract): /api/saved reads only the cache, L88.
- `Graph_AnswersTheReadGraph_WithoutTheKey` (contract): GET /api/graph?project=<projId> answers 200 with the Graph, L102.
- `Graph_IsNotCached_EachRequestReadsLinearAgain` (contract): /api/graph always reads Linear, L122.
- `MissingParameter_Is400_WithoutCallingLinear` (contract): the missing_parameter (400) error, L134.
- `LinearFailures_AnswerTheErrorEnvelope_WithoutTheKey` (contract): each failure answers the error envelope with its code and that code's status, L158.
- `AnUnknownPathOrANonGetMethod_Is404_AndNeverReachesLinear` (contract): the server answers GET only, and the JSON API exactly as plan §3 specifies it, L173.

## Survey/SurveyCommandTests

- `NotExactlyOneProject_IsAUsageError_BeforeAnyRequest` (contract): an argument that is not one Project UUID, slugId or Linear Project URL is a usage error and exits 2, L35.
- `MissingKey_PrintsMapsKeyHelp_BeforeAnyRequest` (contract): without a key it prints the missing-key help it shares with dydo map and exits 2 before any request, L51.
- `ARead_ExitsZero_WithAUtcHeader_AndOneLinePerFinding` (contract): the first line starts dydo survey <Project name> <UTC ISO time>, L67.
- `AnArchivedBlocker_IsNoBlocker` (contract): archived issues are excluded, together with every relation touching one, L83.
- `ATicketMissingMidRead_IsNamed_NotCalledAnUnknownProject` (contract): a ticket that vanishes mid-read is named as that ticket, not as an unknown Project, L100.
- `AnUnexpectedAnswer_IsAToolError_WithNoDigest` (contract): an API failure exits 2 with a message, L117.
- `ACanceledRun_IsNotReportedAsLinearsAnswer` (edge): a run canceled before it reads, L125.

## Survey/SurveyRulesTests

- `Unblocked_AnOpenUnassignedIssue_WhoseEveryBlockerIsClosed_IsListedOnce` (contract): unblocked: an open, unassigned issue with at least one blocks relation into it, where every blocker is closed, L58.
- `Unblocked_IncludesAnIssueLinkedFromTheProject` (contract): unblocked: an open, unassigned issue in the Project or linked from it, where every blocker is closed, L68.
- `Unblocked_NotWhileABlockerIsOpen_NorWhenAssigned_Closed_OrOnlyRelated` (contract): unblocked: an open, unassigned issue with at least one blocks relation into it, where every blocker is closed, L79.
- `Pickable_IsTheViewersRule_MinusIssuesAlreadyUnblocked` (contract): pickable: the map viewer's rule, minus issues already listed as unblocked, L91.
- `Uncontracted_AnUnassignedBacklogProjectIssue_WithNoOpenBlocker_IsNeverPickable` (contract): uncontracted: an unassigned Project issue in a backlog-type state with no open blocker, never pickable, L105.
- `QuestionNoPriority_AnOpenQuestionWithNoPriority` (contract): question-no-priority: an open Project issue labelled Question with no priority, L120.
- `Findings_AreSortedByKind_ThenByKeyNumber` (contract): sorted by kind and then key, L130.
- `AQuietProject_HasNoFindings` (contract): a finding only where its kind's rule holds, L138.

## Workflow/NodeTestGuardTests

- `Guard_AcceptsAReportInWhichEveryTestRanAndPassed` (trap): node exits 0 when tests skip, so skipped tests pass a CI leg (DYD-622), L24.
- `Guard_RejectsAMissingReport` (trap): node exits 0 when tests skip, so skipped tests pass a CI leg (DYD-622), L34.
- `Guard_RejectsAReportWithoutEverySummaryCounter` (trap): node exits 0 when tests skip, so skipped tests pass a CI leg (DYD-622), L46.
- `Guard_RejectsAReportThatProvesLessThanEveryRequiredTestPassing` (trap): node exits 0 when tests skip, so skipped tests pass a CI leg (DYD-622), L63.

## Workflow/ViewerPackTests

- `Pack_WithoutViewer_FailsWithBuildInstructions` (trap): a from-source pack silently ships dydo without the map viewer (DYD-687), L51.
- `Pack_NoBuild_RejectsAViewerlessAssemblyEvenWhenDistNowExists` (trap): a from-source pack silently ships dydo without the map viewer (DYD-687), L66.
- `Pack_NoBuild_RejectsAViewerlessPublishOutputEvenWhenBuildOutputHasTheViewer` (trap): a from-source pack silently ships dydo without the map viewer (DYD-687), L84.
- `Pack_WithViewer_EmbedsTheBundleInTheTool` (trap): a from-source pack silently ships dydo without the map viewer (DYD-687), L107.

## EndToEnd/CliEndToEndTests

- `Help_DoesNotCrash` (edge): the built dydo.dll started as a subprocess, L58.
- `Subcommand_Help_DoesNotCrash` (edge): the built dydo.dll started as a subprocess, L76.
- `Init_CreatesProject` (contract): every mode writes dydo.json and scaffolds the dydo/ folders, L94.
- `Init_RecordsTheVersionTheCliReports` (contract): a fresh init records the running dydo version in dydo.json as dydoVersion, L109.
- `Init_ThenCheck_Passes` (edge): the built dydo.dll's init none, then its check, L121.
- `Init_All_WritesAndRetainsHostAgentSettingsThroughJoin` (contract): repeated-run identity of the host settings init writes, L138.
- `Check_Subfolder_AcceptsLinksToTargetsOutsideSubfolder` (trap): dydo check on a subfolder reported every cross-folder link broken (#0185, fixed in 578386730), L162.
- `Check_NonExistentPath_ReturnsToolError` (contract): exit 2 means a tool error, such as a <path> outside the docs tree, L174.
- `Check_PathOutsideDocsTree_ReturnsToolError` (contract): exit 2 means a tool error, such as a <path> outside the docs tree, L190.
- `Check_WithoutInit_ShowsError`: kept `unsure`, no tag.

## Feature files

One `# Job:` comment heads each file and covers all its scenarios; each scenario proves the behaviour
through its own step line.

- `Features/map-memory.feature` (contract, `Map/MapMemorySteps.cs`), unchanged from base: disposable
  saved maps, by scenario L66 (the saved title), L101 (a miss), L131 to L133 (one complete map) and
  L153 to L155 (the blocked fetch ends with the server).
- `Features/survey.feature` (contract, `Survey/SurveySteps.cs`): dydo survey at its command boundary,
  by scenario L100 (the digest lines of the four kept kinds, in order), L95 with L118 or L120 (the
  argument's exit and the Project asked for), L107 (the unknown Project and each Linear failure
  named), L112 (the shared key help) and L126 to L131 (every request a query).
- `Features/review-pass-grammar.feature` (contract, `Merge/ReviewPassGrammarSteps.cs`): one shared
  grammar, both commands exiting alike, L45 to L46 (both accept) and L52 to L53 (both refuse).

## Related

- [DYD-1520 merge, map and survey audit](./merge-map-survey.md) - The batch's totals and evidence
- [Test rubric](./rubric.md) - §2 row 2, a true job tag
