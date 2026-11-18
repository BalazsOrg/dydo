namespace DynaDocs.Tests.SkillBuild.Fixture.Kinds;

using DynaDocs.SkillBuild;
using A = DynaDocs.Tests.SkillBuild.Fixture.FixtureActor;
using F = DynaDocs.Tests.SkillBuild.Fixture.FixtureSection;
using S = DynaDocs.Tests.SkillBuild.Fixture.FixtureSkill;

/// <summary>Migration fixture: the inventory's <c>evidence</c> claims, sub-topics pin to validate.</summary>
public static class EvidencePinToValidate
{
    public static readonly Claim PinExactGatesFromTestingStrategy = new(
        "The captain pins project-owned selection, commands and stages in the contract's Exact gates by the testing strategy, binding it to real project commands.",
        [A.TicketCaptain],
        [
            new(S.LinearWorkspaceStandard, "`## Exact gates` binds the [testing strategy](../guides/testing-strategy.md#feature-proof-and-gate-placement) to real project commands: selected own/reached features and parent-shared tests, Light/Heavy levels, prerequisites and stage, applicable static/mutation assurance, final coverage producer/consumer, and permitted evidence reuse.", new(F.LinearWorkspaceStandardTicketTemplates, 12)),
            new(S.TicketCaptain, "Pin project-owned selection, commands and stages by the testing strategy.", new(F.TicketCaptainMethod, 10)),
            new(S.WorkingTreeStandard, "The captain pins those project-owned commands in Exact gates; the writer proves that hop's actual reach and the reviewer judges its record.", new(F.WorkingTreeStandardDeliveryScaleAndEvidence, 6), () => [EvidencePinToValidate.ReachIncludesEveryTestChangeReaches!])
        ]);

    public static readonly Claim PlatformSpecificCodeProvedOnCiPlatform = new(
        "Platform-specific code (processes, paths, signals) is proved on CI's platform too, not only the local one.",
        [A.CodeWriter],
        [
            new(S.CodeWriter, "Platform-specific code (processes, paths, signals) is proved on CI's platform too, not only on yours.", new(F.CodeWriterBugOrFeature, 16))
        ]);

    public static readonly Claim ProofBehaviourInCodeRedTest = new(
        "Behaviour in code is proved by a red test committed on the proof branch: its SHA and test name (Break).",
        [A.Inquisitor],
        [
            new(S.Inquisitor, "| Behaviour in code | a red test committed on the proof branch: its SHA and test name | Break |", new(F.InquisitorProof, 2))
        ]);

    public static readonly Claim ProofBehaviourThroughAppDrive = new(
        "Behaviour through the app is proved by rerunnable drive steps with observed and expected output (Drive).",
        [A.Inquisitor],
        [
            new(S.Inquisitor, "| Behaviour through the app | rerunnable drive steps with the observed and the expected output | Drive |", new(F.InquisitorProof, 3))
        ]);

    public static readonly Claim ProofOnlyRunsOneDecidingTest = new(
        "A proof-only hop runs only the one test deciding the hypothesis.",
        [A.CodeWriter],
        [
            new(S.CodeWriter, "Run only the one test deciding the hypothesis; return to that captain `confirmed` with its red-test SHA, `not reproduced`, or `inconclusive` with the deciding observation.", new(F.CodeWriterBugOrFeature, 10), () => [ReturnOrdinaryToWriter.ProofOnlyVerdict])
        ]);

    public static readonly Claim ProofProseQuotedPassages = new(
        "Missed intent, a broken DR or standard, a wrong comment or a prose contradiction is proved by both passages quoted at file:line (Read).",
        [A.Inquisitor],
        [
            new(S.Inquisitor, "| Missed intent, a DR or standard broken, a wrong comment, a prose contradiction | both passages quoted at `file:line` | Read |", new(F.InquisitorProof, 4))
        ]);

    public static readonly Claim ProseOnlyReviewMayHaveNoRuntimeGate = new(
        "A bounded prose-only review may have no runtime gate when its contract names none.",
        [A.Reviewer],
        [
            new(S.Reviewer, "A bounded prose-only review may have no runtime gate when its contract names none.", new(F.ReviewerMethod, 19))
        ]);

    public static readonly Claim QuoteBothPassagesForProseFinding = new(
        "Quote both passages at file:line for an intent, decision, standard, comment or prose finding.",
        [A.Inquisitor],
        [
            new(S.Inquisitor, "Quote both passages at `file:line` for an intent, decision, standard, comment or prose finding.", new(F.InquisitorReadWork, 4))
        ]);

    public static readonly Claim RawProofOnceInDurableArtifact = new(
        "Put new full raw proof and native gate logs once in a durable, linked Git or document artifact.",
        [A.AnyAgent], []);

    public static readonly Claim ReachIncludesEveryTestChangeReaches = new(
        "A hop's proof accounts for every own or reached feature and shared test (consumers and parent-shared tests included); omitted relevant proof is a finding.",
        [A.CodeWriter, A.Reviewer],
        [
            new(S.CodeWriter, "Account for every own/reached feature and shared test, preflight its selection, and return the hop's affected-feature Light and applicable static evidence.", new(F.CodeWriterProve, 2)),
            new(S.Reviewer, "Check actual reach, including consumers and parent-shared tests: omitted relevant proof is a finding.", new(F.ReviewerMethod, 12))
        ]);

    public static readonly Claim ReadExactCandidateEvidence = new(
        "Read this exact candidate's command, selection, environment, exit and artifacts.",
        [A.Reviewer],
        [
            new(S.Reviewer, "Read this exact candidate's command, selection, environment, exit and artifacts; a rerun needs a concrete doubt you state.", new(F.ReviewerMethod, 13))
        ]);

    public static readonly Claim ReadTestingStrategySections = new(
        "Read the named sections of dydo/guides/testing-strategy.md.",
        [A.TicketCaptain, A.CodeWriter, A.DocsWriter, A.Reviewer, A.Admiral],
        [
            new(S.Admiral, "When a captain reports inherited or infrastructure failures, read Evidence and inherited failures in `dydo/guides/testing-strategy.md` from the repository root.", new(F.AdmiralMustReads, 6)),
            new(S.CodeWriter, "From the repository root, read Feature proof and gate placement, Runner prerequisites and shared hosts, and Evidence and inherited failures in `dydo/guides/testing-strategy.md`.", new(F.CodeWriterMustReads, 5)),
            new(S.DocsWriter, "From the repository root, read Feature proof and gate placement in `dydo/guides/testing-strategy.md`.", new(F.DocsWriterMustReads, 5)),
            new(S.Reviewer, "From the repository root, read Feature proof and gate placement and Evidence and inherited failures in `dydo/guides/testing-strategy.md`.", new(F.ReviewerMustReads, 5)),
            new(S.TicketCaptain, "From the repository root, read Feature proof and gate placement, Runner prerequisites and shared hosts, and Evidence and inherited failures in `dydo/guides/testing-strategy.md`.", new(F.TicketCaptainMustReads, 7))
        ]);

    public static readonly Claim ReadingFindingCarriesProofOrIsDropped = new(
        "Each finding made by reading carries its proof or is dropped as refuted.",
        [A.Inquisitor],
        [
            new(S.Inquisitor, "**Prove what reading proves.** Done when each such finding carries its proof or is dropped as refuted.", new(F.InquisitorReadWork, 3))
        ]);

    public static readonly Claim RecordGateResultFields = new(
        "Each gate result names its candidate, command, environment or session, exit and result location.",
        [A.TicketCaptain, A.AnyAgent],
        [
            new(S.LinearWorkspaceStandard, "Each gate result names its candidate, command, environment or session, exit and result location.", new(F.LinearWorkspaceStandardCommunicationAndEvidence, 8)),
            new(S.TicketCaptain, "Record each gate result with its candidate, command, environment or session, exit and result location.", new(F.TicketCaptainMethod, 46))
        ]);

    public static readonly Claim RecordOnlyObservedNativeEvidence = new(
        "Record only observed native evidence.",
        [A.AnyAgent],
        [
            new(S.WorkingTreeStandard, "Record only observed native evidence.", new(F.WorkingTreeStandardDelegation, 24))
        ]);

    public static readonly Claim ReproductionRefutable = new(
        "A reproduction is done when one observation could refute it.",
        [A.CodeWriter],
        [
            new(S.CodeWriter, "Done when one observation could refute the reproduction.", new(F.CodeWriterBugLead, 3))
        ]);

    public static readonly Claim RerunOnlyOnConcern = new(
        "Reuse exact-candidate evidence when the gate and environment permit; rerun only after a relevant change or a stated concern, or at a distinct mandatory integration or release boundary; judge a hop on the proof it ran.",
        [A.TicketCaptain, A.Reviewer, A.AnyAgent],
        [
            new(S.LinearWorkspaceStandard, "Reuse exact-candidate evidence only when the gate and environment permit it, rerunning after relevant changes or new concerns and at mandatory integration or release boundaries.", new(F.LinearWorkspaceStandardCommunicationAndEvidence, 10)),
            new(S.Reviewer, "Where a writer produced the candidate, check its `IMPLEMENTED` line against them and its proof against `maintain-verification-skill`'s Proof; rerun its proof only on a doubt you state.", new(F.ReviewerMethod, 4), () => [ReviewAnswerToFinding.CheckImplementedLineAndProofAgainstPins]),
            new(S.TicketCaptain, "Reuse exact-candidate evidence when that gate and environment permit; rerun only after relevant change or concern, or at a distinct mandatory integration or release boundary.", new(F.TicketCaptainMethod, 47))
        ]);

    public static readonly Claim SkippedNativeProofIsNeverRuntimeProof = new(
        "Skipped native proof is never runtime proof.",
        [A.TicketCaptain], []);

    public static readonly Claim SplitOrFlattenComplexity = new(
        "Split or flatten what is complex.",
        [A.CodeWriter], []);

    public static readonly Claim SurvivingExampleValueGetsItsStepWired = new(
        "A surviving example value gets its step wired.",
        [A.CodeWriter], []);

    public static readonly Claim SurvivingMutantSharpensTestOrDeletesCode = new(
        "A surviving mutant sharpens its test or deletes its code.",
        [A.CodeWriter], []);

    public static readonly Claim TestThroughRealInterfaces = new(
        "Test and verify through real, public interfaces, not mocks of internal parts or external means that bypass the interface.",
        [A.CodeWriter],
        [
            new(S.CodeWriter, "**Integration-style**: Test through real interfaces, not mocks of internal parts.", new(F.CodeWriterTestsGoodTests, 1))
        ]);

    public static readonly Claim TestingStrategyGovernsProof = new(
        "Prove and judge each hop by the testing strategy (Feature proof and gate placement, with the ticket's exact commands); the strategy owns selection, stage placement, runner prerequisites, shared-host constraints and exact-candidate evidence reuse.",
        [A.TicketCaptain, A.CodeWriter, A.Reviewer, A.AnyAgent],
        [
            new(S.CodeWriter, "Use the testing strategy's Feature proof and gate placement with the ticket's exact commands.", new(F.CodeWriterProve, 1)),
            new(S.Reviewer, "Apply the testing strategy to the pinned Exact gates.", new(F.ReviewerMethod, 11)),
            new(S.TicketCaptain, "Brief each hop's proof by Feature proof and gate placement, including the prerequisites and shared host constraints.", new(F.TicketCaptainMethod, 30)),
            new(S.WorkingTreeStandard, "The [testing strategy](./testing-strategy.md#feature-proof-and-gate-placement) owns selection, stage placement, runner prerequisites, shared-host constraints and exact-candidate evidence reuse.", new(F.WorkingTreeStandardDeliveryScaleAndEvidence, 5))
        ]);

    public static readonly Claim TestsCoverBehaviourNotImplementation = new(
        "Tests cover behaviour users or callers care about and describe what, not how.",
        [A.CodeWriter],
        [
            new(S.CodeWriter, "Tests behavior users/callers care about", new(F.CodeWriterTestsGoodTests, 2))
        ]);

    public static readonly Claim TestsNotCoupledToInternals = new(
        "Tests are not coupled to internal structure and survive internal refactors; a test that breaks on a behaviour-preserving refactor is a red flag to fix.",
        [A.CodeWriter],
        [
            new(S.CodeWriter, "Survives internal refactors", new(F.CodeWriterTestsGoodTests, 3))
        ]);

    public static readonly Claim UnrunGateIsNotAPass = new(
        "A follow-up ticket, or a required gate that cannot run, is not a passing gate: name the gate and why, never treat it as passed; real output is evidence.",
        [A.TicketCaptain, A.Reviewer, A.Admiral],
        [
            new(S.Reviewer, "Real output is evidence; a required gate that cannot run is named with why, never treated as passed.", new(F.ReviewerMethod, 15)),
            new(S.TicketCaptain, "A follow-up ticket is not a passing gate.", new(F.TicketCaptainMethod, 32))
        ]);

    public static readonly Claim UnusedMemberReproduction = new(
        "An unused member, export or field is proved by its definition at file:line and a reference search showing no caller outside the tests (a test is not a caller); no test is needed and no revert applies.",
        [A.CodeWriter, A.Inquisitor],
        [
            new(S.CodeWriter, "An unused member's definition and reference search are its reproduction, with no revert.", new(F.CodeWriterBugOrFeature, 2)),
            new(S.Inquisitor, "| Unused member, export or field | its definition at `file:line` and a reference search, command and output quoted, finding no caller outside the tests; a test is not a caller | Read |", new(F.InquisitorProof, 5))
        ]);

    public static readonly Claim ValidateNativeShapesEarly = new(
        "Validate uncertain native interface shapes early.",
        [A.AnyAgent, A.TicketCaptain],
        [
            new(S.TicketCaptain, "Validate uncertain native interface shapes early.", new(F.TicketCaptainMethod, 29)),
            new(S.WorkingTreeStandard, "Validate uncertain native interface shapes early.", new(F.WorkingTreeStandardDeliveryScaleAndEvidence, 8))
        ]);
}
