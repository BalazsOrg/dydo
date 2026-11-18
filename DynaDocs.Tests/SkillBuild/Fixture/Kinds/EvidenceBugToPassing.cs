namespace DynaDocs.Tests.SkillBuild.Fixture.Kinds;

using DynaDocs.SkillBuild;
using A = DynaDocs.Tests.SkillBuild.Fixture.FixtureActor;
using F = DynaDocs.Tests.SkillBuild.Fixture.FixtureSection;
using S = DynaDocs.Tests.SkillBuild.Fixture.FixtureSkill;

/// <summary>Migration fixture: the inventory's <c>evidence</c> claims, sub-topics bug to passing.</summary>
public static class EvidenceBugToPassing
{
    public static readonly Claim BugNameExpectedObserved = new(
        "For a Bug, name the expected and observed behaviour at the highest stable seam, and the behaviour the fix must not change.",
        [A.CodeWriter],
        [
            new(S.CodeWriter, "Name the expected and observed behaviour at the highest stable seam, and the behaviour the fix must not change.", new(F.CodeWriterBugLead, 1))
        ]);

    public static readonly Claim BugReproduceRedAtSeam = new(
        "Reproduce a Bug red: a scenario when the defect shows at the product's boundary, else a test at the seam.",
        [A.CodeWriter],
        [
            new(S.CodeWriter, "Reproduce it red: a scenario when the defect shows at the product's boundary, else a test at the seam.", new(F.CodeWriterBugLead, 2))
        ]);

    public static readonly Claim BugRevertFixCycle = new(
        "A Bug gets a red reproduction, then the fix; revert the fix, watch the reproduction fail, restore it.",
        [A.CodeWriter],
        [
            new(S.CodeWriter, "**Bug** ([bug](resources/bug.md)): a red reproduction, then the fix; revert it, watch the reproduction fail, restore.", new(F.CodeWriterBugOrFeature, 1))
        ]);

    public static readonly Claim CaptainRerunIsProofOfRecord = new(
        "Before offering, the captain reruns each writer's proof recipe itself; that rerun, posted on the record, is the proof of record, and the admiral neither observes nor reruns it.",
        [A.TicketCaptain, A.Admiral],
        [
            new(S.Admiral, "A captain's own rerun of its crew's proof recipe is the proof of record; you neither observe nor rerun it.", new(F.AdmiralMethod, 38)),
            new(S.TicketCaptain, "Before offering, rerun each writer's proof recipe yourself; that rerun, posted on the record, is the proof of record.", new(F.TicketCaptainMethod, 52))
        ]);

    public static readonly Claim CheapProofBeforeExpensiveTests = new(
        "Before expensive tests, record cheap proof of repository or snapshot, intended selection and nonzero discovery; a quiet healthy test continues.",
        [A.AnyAgent],
        [
            new(S.LinearWorkspaceStandard, "Before expensive tests, record cheap proof of repository or snapshot, intended selection and nonzero discovery; a quiet healthy test continues.", new(F.LinearWorkspaceStandardCommunicationAndEvidence, 9))
        ]);

    public static readonly Claim CheckSuccessorBeforeBroadGates = new(
        "Before expensive broad gates, check the exact successor against the previous reviewer's strongest counterexample and the accepted invariant at the implicated boundary; return failed or inconclusive evidence to the captain.",
        [A.CodeWriter],
        [
            new(S.CodeWriter, "Before expensive broad gates, check the exact successor against the previous reviewer's strongest concrete counterexample and the accepted invariant at the implicated boundary; return failed or inconclusive evidence to the captain.", new(F.CodeWriterWhenYouAreTheFixHop, 6))
        ]);

    public static readonly Claim CloseEveryGateThisStageOwes = new(
        "Close every gate this stage owes under the pinned contract and testing strategy; each has its contract-named command and result on this candidate.",
        [A.Reviewer],
        [
            new(S.Reviewer, "Close every gate this stage owes under the pinned contract and testing strategy.", new(F.ReviewerMethod, 18))
        ]);

    public static readonly Claim CutWhatTheContractDoesNotNeed = new(
        "Cut what the contract does not need.",
        [A.CodeWriter],
        [
            new(S.CodeWriter, "Fix at the root: a gap gets its test first; cut what the contract does not need; split or flatten what is complex; hide what leaks across a seam; a surviving mutant sharpens its test or deletes its code; a surviving example value gets its step wired.", new(F.CodeWriterWhenYouAreTheFixHop, 5), () => [EvidenceBugToPassing.GapGetsItsTestFirst!, EvidenceBugToPassing.HideWhatLeaksAcrossASeam!, EvidencePinToValidate.SplitOrFlattenComplexity, EvidencePinToValidate.SurvivingExampleValueGetsItsStepWired, EvidencePinToValidate.SurvivingMutantSharpensTestOrDeletesCode])
        ]);

    public static readonly Claim DeleteTestOnlyWithRequiredCodeRemoval = new(
        "Delete a test only together with code whose removal the contract or a FAIL requires (dead code with its test-only callers, or obsolete internal tests replaced by interface tests).",
        [A.CodeWriter],
        [
            new(S.CodeWriter, "Delete a test only with code whose removal the contract or a FAIL requires: dead code with its test-only callers, or obsolete internal tests replaced by interface tests that still prove their behaviour.", new(F.CodeWriterBugOrFeature, 14))
        ]);

    public static readonly Claim DirectDoneDefinition = new(
        "Direct is done when each candidate accounts for its paths, passes its gates, ends on a posted commit and carries no unresolved choice.",
        [A.TicketCaptain],
        [
            new(S.TicketCaptain, "**Done:** each candidate accounts for its paths, passes its gates, ends on a posted commit, and carries no unresolved choice.", new(F.TicketCaptainMethod, 33))
        ]);

    public static readonly Claim DocsHopDone = new(
        "The docs hop is done when the bounded proof has run and the work is committed.",
        [A.DocsWriter],
        [
            new(S.DocsWriter, "Done when the bounded proof has run and the work is committed.", new(F.DocsWriterMethod, 13))
        ]);

    public static readonly Claim DocsHopProof = new(
        "Run dydo fix after a document is added, moved or renamed, then the hop's proof until clean (dydo check, cheap checks the ticket names, tests the change reaches, using the documentation-only scope).",
        [A.DocsWriter],
        [
            new(S.DocsWriter, "`dydo fix` after a document is added, moved or renamed, then this hop's proof until clean: `dydo check`, the other cheap checks the ticket names, and the tests your change reaches, using the testing strategy's documentation-only scope.", new(F.DocsWriterMethod, 11))
        ]);

    public static readonly Claim DontAssertCallCountsOrOrder = new(
        "Do not assert on call counts or order.",
        [A.CodeWriter],
        [
            new(S.CodeWriter, "Asserting on call counts/order", new(F.CodeWriterTestsBadTests, 3))
        ]);

    public static readonly Claim DontMockInternalCollaborators = new(
        "Do not mock internal collaborators; your own modules stay real in tests.",
        [A.CodeWriter],
        [
            new(S.CodeWriter, "Mocking internal collaborators", new(F.CodeWriterTestsBadTests, 1))
        ]);

    public static readonly Claim DontTestPrivateMethods = new(
        "Do not test private methods.",
        [A.CodeWriter],
        [
            new(S.CodeWriter, "Testing private methods", new(F.CodeWriterTestsBadTests, 2))
        ]);

    public static readonly Claim ExactGatesPassForHop = new(
        "A hop's exact gates pass.",
        [A.Crew],
        [
            new(S.WorkingTreeStandard, "| Build and prove | crew | Changes stay inside owned paths; exact gates pass; each hop ends on a commit `<KEY> <hop>: <what>`, the hop being `implement`, `fix` after a FAIL, `merge`, or `proof`, which commits as `<KEY> proof: <hypothesis>`; review evidence stays on the work item reviewed; every return comes back to the Ticket Captain. |", new(F.WorkingTreeStandardWhoDoesWhat, 4), () => [ReturnAdmiralToNo.CrewReturnGoesToTicketCaptain, ReviewReadToWrong.ReviewEvidenceStaysOnReviewedWorkItem, WorktreeAuditToReviewer.HopEndsOnOneNamedCommit])
        ]);

    public static readonly Claim ExactGatesSeparateHopFromFinal = new(
        "Exact gates lists hop proof separately from final, combined-merge and release gates.",
        [A.TicketCaptain],
        [
            new(S.LinearWorkspaceStandard, "List hop proof separately from final, combined-merge and release gates.", new(F.LinearWorkspaceStandardTicketTemplates, 13))
        ]);

    public static readonly Claim ExistingRedTestIsTheReproduction = new(
        "An existing red test, an inquisition's included, is the reproduction: adopt it.",
        [A.TicketCaptain, A.CodeWriter],
        [
            new(S.CodeWriter, "An existing failing test is the red.", new(F.CodeWriterBugOrFeature, 13))
        ]);

    public static readonly Claim ExpectedValuesNotRestatingImplementation = new(
        "Expected values do not restate the implementation; a test must not pass by construction.",
        [A.CodeWriter],
        [
            new(S.CodeWriter, "**Tautological tests**: Expected value restates the implementation, so the test passes by construction.", new(F.CodeWriterTestsBadTests, 4))
        ]);

    public static readonly Claim FeatureCodeAndTestsTogether = new(
        "On a Feature, write code and tests together.",
        [A.CodeWriter],
        [
            new(S.CodeWriter, "Write code and tests together.", new(F.CodeWriterBugOrFeature, 5))
        ]);

    public static readonly Claim FeatureRedByStash = new(
        "When a Feature is green, stash all but the tests, run those the change reaches, keep one failing line each, restore.",
        [A.CodeWriter],
        [
            new(S.CodeWriter, "When green, stash all but the tests, run those the change reaches, keep one failing line each, restore.", new(F.CodeWriterBugOrFeature, 6))
        ]);

    public static readonly Claim FeatureTestCasesBeforeCode = new(
        "On a Feature, before code, turn each acceptance line into a named test case, empty bodies allowed.",
        [A.CodeWriter],
        [
            new(S.CodeWriter, "**Feature**: before code, turn each acceptance line into a named test case, empty bodies allowed.", new(F.CodeWriterBugOrFeature, 4))
        ]);

    public static readonly Claim FinalAndLandingGatesStay = new(
        "Combined-merge, integration, release and final G/M gates stay mandatory at their boundaries; delivery scale or scaled review never weakens them.",
        [A.TicketCaptain, A.Reviewer, A.AnyAgent],
        [
            new(S.LinearWorkspaceStandard, "Required G/M, integration and release gates remain.", new(F.LinearWorkspaceStandardType, 13)),
            new(S.Reviewer, "Meaningful code changes retain their specified test obligations; combined merge gates and final G/M release obligations remain mandatory at their stages.", new(F.ReviewerMethod, 20), () => [EvidenceBugToPassing.MeaningfulChangesRetainTestObligations!]),
            new(S.TicketCaptain, "Apply the testing strategy's final-candidate gates here and the contract's mandatory combined and landing gates at their boundaries, with exact-candidate reuse only where permitted.", new(F.TicketCaptainMethod, 51), () => [EvidenceBugToPassing.FinalCandidateGatesApplyAtOffer!]),
            new(S.WorkingTreeStandard, "This scale rule does not weaken required G/M, integration or release gates, and skipped native proof is never runtime proof.", new(F.WorkingTreeStandardDeliveryScaleAndEvidence, 4), () => [EvidencePinToValidate.SkippedNativeProofIsNeverRuntimeProof])
        ]);

    public static readonly Claim FinalCandidateGatesApplyAtOffer = new(
        "Final-candidate gates apply at offer.",
        [A.TicketCaptain], []);

    public static readonly Claim FindingCountsWhenProven = new(
        "A finding counts once the repository proves it on the audit SHA, by the proof its type requires.",
        [A.Inquisitor],
        [
            new(S.Inquisitor, "A finding counts once the repository proves it on the audit SHA:", new(F.InquisitorProof, 1))
        ]);

    public static readonly Claim FixHcrapAndComplexity = new(
        "Fix every HCRAP or cognitive-complexity finding in a method you changed.",
        [A.CodeWriter],
        [
            new(S.CodeWriter, "Fix every HCRAP or cognitive-complexity finding in a method you changed.", new(F.CodeWriterProve, 4))
        ]);

    public static readonly Claim GapGetsItsTestFirst = new(
        "Fix at the root: a gap gets its test first.",
        [A.CodeWriter], []);

    public static readonly Claim GherkinScenarios = new(
        "Write each acceptance line the contract marks for Gherkin as a feature-file scenario at the product's boundary, in glossary words, with example tables where values vary and every column changes an outcome, wired through step definitions.",
        [A.CodeWriter],
        [
            new(S.CodeWriter, "Write each acceptance line the contract marks for Gherkin as a feature-file scenario: at the product's boundary, in glossary words, example tables where values vary and every column changes an outcome, wired through step definitions.", new(F.CodeWriterBuild, 7))
        ]);

    public static readonly Claim GreenBeforeCodeIsAFinding = new(
        "A test that is green before any code exists is a finding about the test.",
        [A.CodeWriter],
        [
            new(S.CodeWriter, "A test green before any code exists is a finding about the test.", new(F.CodeWriterBugOrFeature, 12))
        ]);

    public static readonly Claim HideWhatLeaksAcrossASeam = new(
        "Hide what leaks across a seam.",
        [A.CodeWriter], []);

    public static readonly Claim InheritedFailureAttribution = new(
        "Attribute a reported inherited or infrastructure failure by the testing strategy's Evidence and inherited failures and record its owner and route.",
        [A.TicketCaptain, A.Admiral, A.CodeWriter, A.Reviewer],
        [
            new(S.Admiral, "For evidenced inherited or infrastructure failures, assign the separate repair owner or escalate the unresolved decision; record blocked gates without converting the failure into a pass.", new(F.AdmiralMethod, 52), () => [EvidencePinToValidate.UnrunGateIsNotAPass]),
            new(S.Reviewer, "Check final Heavy/coverage placement and inherited failure attribution with its owner disposition.", new(F.ReviewerMethod, 14))
        ]);

    public static readonly Claim InjectBoundaryDependencies = new(
        "At a boundary, inject the dependency and give each external operation its own function rather than one generic fetcher, so each mock returns one shape and setup holds no logic.",
        [A.CodeWriter],
        [
            new(S.CodeWriter, "At a boundary, inject the dependency, and give each external operation its own function (`getUser`, `createOrder`) rather than one generic fetcher, so each mock returns one shape and the setup holds no logic.", new(F.CodeWriterTestsWhereAMockBelongs, 2))
        ]);

    public static readonly Claim KeepHistoricalOriginals = new(
        "Keep existing historical originals accessible.",
        [A.AnyAgent],
        [
            new(S.LinearWorkspaceStandard, "Keep existing historical originals accessible.", new(F.LinearWorkspaceStandardCommunicationAndEvidence, 6))
        ]);

    public static readonly Claim MeaningfulChangesRetainTestObligations = new(
        "Meaningful code changes retain their specified test obligations.",
        [A.Reviewer], []);

    public static readonly Claim MeaningfulCodeChangesKeepSpecifiedTests = new(
        "Meaningful code changes passed their specified tests.",
        [A.Reviewer],
        [
            new(S.Reviewer, "[ ] Meaningful code changes passed their specified tests.", new(F.ReviewerMethod, 22))
        ]);

    public static readonly Claim MockOnlyAtSystemBoundaries = new(
        "Mock only at system boundaries: external APIs, time and randomness, and sometimes the database or file system (a test instance is better still).",
        [A.CodeWriter],
        [
            new(S.CodeWriter, "At system boundaries only: external APIs, time and randomness, and sometimes the database or file system, where a test instance is better still.", new(F.CodeWriterTestsWhereAMockBelongs, 1))
        ]);

    public static readonly Claim NeverWeakenTestOfKeptBehaviour = new(
        "Never disable a test of kept behaviour, remove its assertion or weaken its scenario.",
        [A.CodeWriter],
        [
            new(S.CodeWriter, "Disabling a test of kept behaviour, removing its assertion or weakening its scenario is forbidden.", new(F.CodeWriterBugOrFeature, 15))
        ]);

    public static readonly Claim NoRawLogsInPrsOrComments = new(
        "Do not dump full raw proof or gate logs into PRs or comments.",
        [A.AnyAgent],
        [
            new(S.LinearWorkspaceStandard, "Put new full raw proof and native gate logs once in a durable, linked Git or document artifact; do not dump them into PRs or comments.", new(F.LinearWorkspaceStandardCommunicationAndEvidence, 7), () => [EvidencePinToValidate.RawProofOnceInDurableArtifact])
        ]);

    public static readonly Claim NoSleepOrTimeoutInTests = new(
        "A test that needs a sleep or timeout is wrong; force races with fake timers or interleaving, never synthetic CPU load, and keep any load small and cleaned up.",
        [A.Crew, A.CodeWriter, A.AnyAgent],
        [
            new(S.CodeWriter, "Shared machine: force races with fake timers or interleaving, never synthetic CPU load; any load stays small and cleans up.", new(F.CodeWriterBugOrFeature, 3)),
            new(S.Inquisitor, "A test that needs a sleep or a timeout is wrong; force races with fake timers or interleaving.", new(F.InquisitorBreakWork, 4))
        ]);

    public static readonly Claim OneLogicalAssertionPerTest = new(
        "One logical assertion per test.",
        [A.CodeWriter],
        [
            new(S.CodeWriter, "One logical assertion per test", new(F.CodeWriterTestsGoodTests, 4))
        ]);

    public static readonly Claim PassingWitnessNotASubstitute = new(
        "A passing witness does not replace ordinary RED/fix proof, required final gates or independent review.",
        [A.AnyAgent],
        [
            new(S.CodeWriter, "A passing witness does not replace ordinary RED/fix proof, required final gates or independent review.", new(F.CodeWriterWhenYouAreTheFixHop, 7))
        ]);
}
