namespace DynaDocs.Tests.SkillBuild.Fixture.Kinds;

using DynaDocs.SkillBuild;
using A = DynaDocs.Tests.SkillBuild.Fixture.FixtureActor;
using F = DynaDocs.Tests.SkillBuild.Fixture.FixtureSection;
using S = DynaDocs.Tests.SkillBuild.Fixture.FixtureSkill;

/// <summary>Migration fixture: the inventory's <c>review</c> claims, sub-topics read to wrong.</summary>
public static class ReviewReadToWrong
{
    public static readonly Claim ReadCandidateNotStory = new(
        "The reviewer reads the candidate itself rather than the story told about it; independence is independence of context.",
        [A.Reviewer],
        [
            new(S.Reviewer, "Your independence is independence of *context*: read the candidate itself rather than the story told about it.", new(F.ReviewerBoundary, 1))
        ]);

    public static readonly Claim ReadExactlyOneRubric = new(
        "The reviewer reads exactly the one rubric the invoker named (code, docs, project-plan, spec or merge).",
        [A.Reviewer],
        [
            new(S.Reviewer, "[code](resources/code.md) · [docs](resources/docs.md) · [project-plan](resources/project-plan.md) · [spec](resources/spec.md) · [merge](resources/merge.md), exactly the one the invoker named.", new(F.ReviewerMethod, 1))
        ]);

    public static readonly Claim RereviewOnScopeChange = new(
        "Changes to destination, scope, acceptance criteria or governing architecture are re-reviewed and need human approval before affected work resumes.",
        [A.Admiral], []);

    public static readonly Claim ReviewEvidenceStaysOnReviewedWorkItem = new(
        "Review evidence stays on the work item reviewed.",
        [A.Crew], []);

    public static readonly Claim ReviewerPinsContractCandidateBase = new(
        "The reviewer pins the contract as the brief pins it, the candidate at its SHA and the base SHA, so another reviewer could open the same contract and candidate; the block's contract ref and base are the brief's.",
        [A.Reviewer],
        [
            new(S.LinearWorkspaceStandard, "A review block's `contract <ref>` is the contract its brief pinned, for a ticket `<KEY> description as of <Linear updatedAt>`; `base <SHA>` is the brief's base.", new(F.LinearWorkspaceStandardCommunicationAndEvidence, 20)),
            new(S.Reviewer, "The contract as the brief pins it, the candidate at its SHA, the base SHA.", new(F.ReviewerMethod, 3))
        ]);

    public static readonly Claim RubricRestatable = new(
        "The rubric step is done when the reviewer can restate every item it asks.",
        [A.Reviewer],
        [
            new(S.Reviewer, "Done when you can restate every item it asks of you.", new(F.ReviewerMethod, 2))
        ]);

    public static readonly Claim SeverityDefinitions = new(
        "Findings are labelled by intent: blocker when the intended case is wrong (data loss, corruption, security hole, broken journey), should for a real bug callers will hit inside the intent, nit for everything else (another code path, rare case, small cost, style, extra compat), however serious it looks; severities are judged against the source.",
        [A.Reviewer, A.Inquisitor],
        [
            new(S.Inquisitor, "The three review severities, judged against the source, not against how sure you feel:", new(F.InquisitorSeverity, 1)),
            new(S.Reviewer, "Label each finding by the intent, not by how it looks: `blocker`, the intended case is wrong; `should`, a real bug callers will hit, inside the intent; `nit`, everything else.", new(F.ReviewerMethod, 16))
        ]);

    public static readonly Claim ShouldAuthorMayClose = new(
        "A should-ticket's own author may close it with a reason.",
        [A.TicketCaptain], []);

    public static readonly Claim ShouldFiledAsOwnTicket = new(
        "Each review finding labelled should is filed as its own ticket by the captain (or admiral).",
        [A.TicketCaptain, A.Admiral],
        [
            new(S.LinearWorkspaceStandard, "The captain files each should as its own ticket and records each nit as one line on the record.", new(F.LinearWorkspaceStandardCommunicationAndEvidence, 22)),
            new(S.TicketCaptain, "A review's `should` is the one exception: you file its ticket yourself (Review below).", new(F.TicketCaptainBoundary, 23))
        ]);

    public static readonly Claim ShouldStaysInProject = new(
        "A review's should stays in its Project, as filler unless it blocks; only the user moves it out.",
        [A.Admiral],
        [
            new(S.Admiral, "A review's `should` stays in this Project, as filler unless it blocks; only the user moves it out.", new(F.AdmiralMethod, 25))
        ]);

    public static readonly Claim SpecCheckFog = new(
        "The spec reviewer checks the fog: any unanswered question records homework and blocks the work through the Captain.",
        [A.Reviewer],
        [
            new(S.Reviewer, "**Check the fog.** A missing answer is either found in the searched ground or returned through the Captain as a prepared Question packet naming its waiters. **Done:** no assumption bridges an unknown route.", new(F.ReviewerSpecMethod, 6))
        ]);

    public static readonly Claim SpecCheckProof = new(
        "Scenarios and exact gates can prove the outcome; only stage-applicable gates were run.",
        [A.Reviewer],
        [
            new(S.Reviewer, "**Check the proof.** Acceptance lines, scenarios and gates are exact and sufficient for the ticket outcome. Run those applicable before code; mark implementation gates `not run - pre-code` after verifying their commands and pass conditions. **Done:** the eventual result can fail as well as pass.", new(F.ReviewerSpecMethod, 5))
        ]);

    public static readonly Claim SpecEdgeCasesCovered = new(
        "Edge cases, failures, migration, compatibility and rollback are covered when consequential.",
        [A.Reviewer],
        [
            new(S.Reviewer, "[ ] Edge cases, failures, migration, compatibility, and rollback are covered when consequential", new(F.ReviewerSpecChecklist, 1))
        ]);

    public static readonly Claim SpecHuntHiddenDecisions = new(
        "The acceptance lines account for every outcome without hidden design choices.",
        [A.Reviewer],
        [
            new(S.Reviewer, "**Hunt hidden decisions.** Walk the contract as the delegated writer through behavior, files, seams, edge and failure handling, migration, compatibility, and proof. **Done:** no material choice is silently delegated to production.", new(F.ReviewerSpecMethod, 4))
        ]);

    public static readonly Claim SpecJudgeScenarios = new(
        "Every scenario stands at the boundary, refines a parent criterion and has no idle example column.",
        [A.Reviewer],
        [
            new(S.Reviewer, "**Judge the scenarios.** Where the contract calls for Gherkin, each scenario stands at the product's boundary, in glossary words, deterministic, refining one criterion its parent carries; every example column changes an outcome. **Done:** the scenarios say what the ticket proves and nothing the parent did not ask for.", new(F.ReviewerSpecMethod, 2))
        ]);

    public static readonly Claim SpecPinGround = new(
        "The spec reviewer pins the ground: exact Linear record, base SHA, branch, worktree, clean state and owned paths match.",
        [A.Reviewer],
        [
            new(S.Reviewer, "**Pin the ground.** Match the contract to the target ticket or direct lane Sub-ticket, its five contract fields, base SHA, branch, worktree, clean state and owned paths. **Done:** it binds the exact tree and nothing outside its authority.", new(F.ReviewerSpecMethod, 1))
        ]);

    public static readonly Claim SpecReviewExcludesWording = new(
        "Wording, formatting preference and an equally valid route are outside the spec rubric and become neither findings nor notes.",
        [A.Reviewer],
        [
            new(S.Reviewer, "Wording, formatting preference, and an equally valid route are outside this rubric: do not turn them into findings or notes.", new(F.ReviewerSpecBoundary, 2))
        ]);

    public static readonly Claim SpecReviewFindingKinds = new(
        "A scenario beyond its parent criterion, a concrete contract conflict, a false pattern, a hidden design choice, a consequential missing case or a gate that cannot prove the outcome is a spec finding.",
        [A.Reviewer],
        [
            new(S.Reviewer, "A scenario beyond its parent criterion, a concrete contract conflict, a false pattern, a hidden design choice, a consequential missing case, or a gate that cannot prove the outcome is a finding.", new(F.ReviewerSpecBoundary, 1))
        ]);

    public static readonly Claim SpecReviewJudgedAgainst = new(
        "The spec is judged against the parent's criteria, the settled working tree, governing Decisions, pinned architecture-plan and feature-spec sections and the code-writer skill.",
        [A.Reviewer],
        [
            new(S.Reviewer, "Judge it against the parent's criteria, the settled working tree, the governing Decisions, pinned architecture-plan and feature-spec sections, and the `code-writer` skill.", new(F.ReviewerSpecLead, 2))
        ]);

    public static readonly Claim SpecReviewOnlyForRecordedRisk = new(
        "A spec review of the contract before any code is bought only for one short concrete risk reason recorded in the contract.",
        [A.TicketCaptain],
        [
            new(S.LinearWorkspaceStandard, "A spec review of the contract before any code needs one short concrete risk reason, recorded in the contract; persistence, migrations, permissions and uncertain native interfaces are examples of such a risk.", new(F.LinearWorkspaceStandardType, 12)),
            new(S.WorkingTreeStandard, "Add a spec review of the contract before any code only when one short, concrete risk reason, recorded in the contract, calls for it; persistence, migrations, permissions and uncertain native interfaces are examples of such a risk.", new(F.WorkingTreeStandardDeliveryScaleAndEvidence, 2))
        ]);

    public static readonly Claim SpecReviewTarget = new(
        "The spec review targets the contract text on the ticket before any code and is invoked when the Ticket Captain has bought one.",
        [A.Reviewer],
        [
            new(S.Reviewer, "Target: the contract text on the ticket as it stands before any code, invoked when the Ticket Captain has bought a spec review.", new(F.ReviewerSpecLead, 1))
        ]);

    public static readonly Claim SpecReviewerBriefedOnContractText = new(
        "A fresh spec reviewer is briefed on the contract text.",
        [A.TicketCaptain], []);

    public static readonly Claim SpecVerifyCitedGround = new(
        "Every Decision, pattern and path the spec cites holds at the base SHA.",
        [A.Reviewer],
        [
            new(S.Reviewer, "**Verify the ground it cites.** Read every cited Decision, specification, pattern, seam, file, and test at the base SHA. **Done:** each citation holds, or its departure is justified.", new(F.ReviewerSpecMethod, 3))
        ]);

    public static readonly Claim TicketPrBindingReviewCompact = new(
        "Ticket-and-PR binding review stays compact but complete.",
        [A.Reviewer],
        [
            new(S.LinearWorkspaceStandard, "Ticket-and-PR binding review stays compact but complete.", new(F.LinearWorkspaceStandardCommunicationAndEvidence, 11))
        ]);

    public static readonly Claim TwoReviewersFirstRound = new(
        "A candidate's first review round seats two reviewers in parallel, one Claude and one Codex, and merges their findings into one block.",
        [A.TicketCaptain], []);

    public static readonly Claim WholeTicketPassAfterLanes = new(
        "Once all lanes are in, the captain verifies the combined proof and obtains a fresh whole-ticket (parent) PASS on the ticket and in the PR.",
        [A.TicketCaptain],
        [
            new(S.TicketCaptain, "Obtain a fresh whole-ticket PASS once all lanes are in.", new(F.TicketCaptainMethod, 50))
        ]);

    public static readonly Claim WrongScenarioAmendedByFixHop = new(
        "A wrong scenario is amended by the fix hop when the FAIL block names it.",
        [A.TicketCaptain], []);
}
