namespace DynaDocs.Tests.SkillBuild.Fixture;

using DynaDocs.SkillBuild;
using A = DynaDocs.Tests.SkillBuild.Fixture.FixtureActor;
using F = DynaDocs.Tests.SkillBuild.Fixture.FixtureSection;
using S = DynaDocs.Tests.SkillBuild.Fixture.FixtureSkill;

/// <summary>Migration fixture: the inventory's <c>contract</c> claims, sub-topics move to writer.</summary>
public static class ContractMoveToWriter
{
    public static readonly Claim MoveOrRenameOwnsBreakage = new(
        "A move or rename also owns every file it breaks: links, tests, notices.",
        [A.AnyAgent],
        [
            new(S.TicketCaptain, "A move or rename also owns every file it breaks: links, tests, notices.", new(F.TicketCaptainMethod, 3))
        ]);

    public static readonly Claim NoRetrospectiveGateCancel = new(
        "No gate is cancelled retrospectively by a new default.",
        [A.TicketCaptain], []);

    public static readonly Claim OneCompactAcceptanceContract = new(
        "Keep one compact acceptance contract (on the parent) and point to its evidence instead of repeating it.",
        [A.TicketCaptain],
        [
            new(S.LinearWorkspaceStandard, "The captain records one compact acceptance contract and points to its evidence.", new(F.LinearWorkspaceStandardType, 14)),
            new(S.TicketCaptain, "Keep one compact acceptance contract on the parent and link its evidence instead of repeating it.", new(F.TicketCaptainMethod, 5)),
            new(S.WorkingTreeStandard, "Keep one compact acceptance contract and point to evidence rather than copying it.", new(F.WorkingTreeStandardDeliveryScaleAndEvidence, 3))
        ]);

    public static readonly Claim ParallelCrewDisjointPaths = new(
        "Parallel crew write only the disjoint paths their Ticket Captain grants; fan-out is safe only across disjoint paths.",
        [A.Crew, A.TicketCaptain],
        [
            new(S.WorkingTreeStandard, "Parallel crew write only the disjoint paths their Ticket Captain grants; the Bug Type-map exception below transfers paths serially.", new(F.WorkingTreeStandardInvariants, 5))
        ]);

    public static readonly Claim ParallelLanesAsSubtickets = new(
        "Independently trackable parallel lanes become direct Sub-tickets, each contracted with the parent's Type and Mode, bounded outcome, a disjoint owned-path subset, exact gates, a child-key branch off the parent branch and an isolated worktree.",
        [A.TicketCaptain],
        [
            new(S.TicketCaptain, "Where the writer's comment names disjoint parallel lanes, open them in `Todo`, with the parent's Type and Mode, bounded outcome, paths, gates and isolated branch/worktree off the parent.", new(F.TicketCaptainMethod, 14)),
            new(S.WorkingTreeStandard, "Independently trackable parallel lanes may become direct Sub-tickets, each with an isolated branch and worktree.", new(F.WorkingTreeStandardInvariants, 4))
        ]);

    public static readonly Claim ParentKeepsSequentialWorkAndFinalReview = new(
        "Ordinary sequential work, joining scenarios, the joined acceptance and the whole-result review stay on the parent; the Bug Type-map exception is separate.",
        [A.TicketCaptain],
        [
            new(S.TicketCaptain, "Keep ordinary sequential work, joining scenarios and the whole-result review on the parent; the Bug Type-map exception is below.", new(F.TicketCaptainMethod, 13))
        ]);

    public static readonly Claim PrototypeTicketClosesOnVerdict = new(
        "A Prototype uses the prototype skill and closes on the user's verdict with its winning branch linked, never submitted.",
        [A.TicketCaptain],
        [
            new(S.TicketCaptain, "A Prototype uses `prototype` and closes on the user's verdict with its winning branch linked, never submitted.", new(F.TicketCaptainKindsAndFailurePaths, 7))
        ]);

    public static readonly Claim ReadAbout = new(
        "Read dydo/understand/about.md from the repository root.",
        [A.TicketCaptain, A.DocsWriter, A.Research, A.Reviewer, A.Scout, A.Admiral, A.ChiefOfStaff, A.Inquisitor],
        [
            new(S.Admiral, "From the repository root, read `dydo/understand/about.md`.", new(F.AdmiralMustReads, 4)),
            new(S.ChiefOfStaff, "From the repository root, read `dydo/understand/about.md`.", new(F.ChiefOfStaffMustReads, 2)),
            new(S.DocsWriter, "From the repository root, read `dydo/understand/about.md`.", new(F.DocsWriterMustReads, 3)),
            new(S.Inquisitor, "From the repository root, read `dydo/understand/about.md`.", new(F.InquisitorMustReads, 4)),
            new(S.Research, "From the repository root, read `dydo/understand/about.md`.", new(F.ResearchMustReads, 2)),
            new(S.Reviewer, "From the repository root, read `dydo/understand/about.md`.", new(F.ReviewerMustReads, 2)),
            new(S.Scout, "From the repository root, read `dydo/understand/about.md`.", new(F.ScoutMustReads, 2)),
            new(S.TicketCaptain, "From the repository root, read `dydo/understand/about.md`.", new(F.TicketCaptainMustReads, 5))
        ]);

    public static readonly Claim ReadArchitecture = new(
        "Read dydo/understand/architecture.md from the repository root.",
        [A.TicketCaptain, A.Research, A.Reviewer, A.Scout, A.Admiral, A.ChiefOfStaff, A.Inquisitor],
        [
            new(S.Admiral, "From the repository root, read `dydo/understand/architecture.md`.", new(F.AdmiralMustReads, 5)),
            new(S.ChiefOfStaff, "From the repository root, read `dydo/understand/architecture.md`.", new(F.ChiefOfStaffMustReads, 3)),
            new(S.Inquisitor, "From the repository root, read `dydo/understand/architecture.md`.", new(F.InquisitorMustReads, 5)),
            new(S.Research, "From the repository root, read `dydo/understand/architecture.md`.", new(F.ResearchMustReads, 3)),
            new(S.Reviewer, "From the repository root, read `dydo/understand/architecture.md`.", new(F.ReviewerMustReads, 3)),
            new(S.Scout, "From the repository root, read `dydo/understand/architecture.md`.", new(F.ScoutMustReads, 3)),
            new(S.TicketCaptain, "From the repository root, read `dydo/understand/architecture.md`.", new(F.TicketCaptainMustReads, 6))
        ]);

    public static readonly Claim ReadCodingStandards = new(
        "Read dydo/guides/coding-standards.md from the repository root.",
        [A.CodeWriter, A.Reviewer, A.Inquisitor],
        [
            new(S.CodeWriter, "From the repository root, read `dydo/guides/coding-standards.md`.", new(F.CodeWriterMustReads, 3)),
            new(S.Inquisitor, "From the repository root, read `dydo/guides/coding-standards.md`.", new(F.InquisitorMustReads, 6)),
            new(S.Reviewer, "From the repository root, read `dydo/guides/coding-standards.md`.", new(F.ReviewerMustReads, 4))
        ]);

    public static readonly Claim ReadFeatureSpecConvention = new(
        "When a feature spec applies, read dydo/reference/feature-spec-convention.md from the repository root.",
        [A.TicketCaptain],
        [
            new(S.TicketCaptain, "When a feature spec applies, from the repository root read `dydo/reference/feature-spec-convention.md`.", new(F.TicketCaptainMustReads, 3))
        ]);

    public static readonly Claim ReadGlossary = new(
        "Read dydo/glossary.md from the repository root.",
        [A.CodeWriter],
        [
            new(S.CodeWriter, "From the repository root, read `dydo/glossary.md`.", new(F.CodeWriterMustReads, 4))
        ]);

    public static readonly Claim ReadGoverningPlanAndSpec = new(
        "Read the ticket's governing plan and the applicable feature-spec sections at their pinned commits.",
        [A.TicketCaptain, A.CodeWriter, A.DocsWriter],
        [
            new(S.CodeWriter, "The governing plan and applicable feature-spec sections at the brief's commits, and their Decisions.", new(F.CodeWriterMustReads, 2)),
            new(S.DocsWriter, "The owning Linear ticket, its governing plan and applicable feature-spec sections at pinned commits, and on a fix hop the FAIL block that sent you.", new(F.DocsWriterMustReads, 1), () => [ContractMoveToWriter.ReadTicketAndBrief!]),
            new(S.TicketCaptain, "Its governing plan and applicable feature-spec sections at pinned commits, or atomic intent.", new(F.TicketCaptainMustReads, 2))
        ]);

    public static readonly Claim ReadLinearStandardCommunicationOnly = new(
        "Read only the Communication and evidence section of dydo/reference/linear-workspace-standard.md from the repository root for the communication protocol, and other sections on demand when their fields or status rules are needed; do not preload the whole standard.",
        [A.Crew, A.Admiral, A.ChiefOfStaff, A.TicketCaptain, A.AnyAgent],
        [
            new(S.Admiral, "From the repository root, read only the Communication and evidence section in `dydo/reference/linear-workspace-standard.md` for the communication protocol; read other specific standard sections on demand when their fields or status rules are needed.", new(F.AdmiralMustReads, 7)),
            new(S.ChiefOfStaff, "From the repository root, read only the Communication and evidence section in `dydo/reference/linear-workspace-standard.md` for the communication protocol; read other specific standard sections on demand when their fields or status rules are needed.", new(F.ChiefOfStaffMustReads, 1)),
            new(S.DocsWriter, "From the repository root, read only the Communication and evidence section in `dydo/reference/linear-workspace-standard.md` for the communication protocol; do not preload the whole standard.", new(F.DocsWriterMustReads, 6)),
            new(S.Research, "From the repository root, read only the Communication and evidence section in `dydo/reference/linear-workspace-standard.md` for the communication protocol; do not preload the whole standard.", new(F.ResearchMustReads, 4)),
            new(S.Reviewer, "From the repository root, read only the Communication and evidence section in `dydo/reference/linear-workspace-standard.md` for the communication protocol; do not preload the whole standard.", new(F.ReviewerMustReads, 6)),
            new(S.TicketCaptain, "From the repository root, read only the Communication and evidence section in `dydo/reference/linear-workspace-standard.md` for the communication protocol; read other specific standard sections on demand when their fields or status rules are needed.", new(F.TicketCaptainMustReads, 8)),
            new(S.Wayfinder, "For a communication protocol, from the repository root read only the Communication and evidence section in `dydo/reference/linear-workspace-standard.md`, not the whole workspace standard.", new(SpinePart.Job, 5))
        ]);

    public static readonly Claim ReadQuestionAsStated = new(
        "Read the question as the invoker stated it, with its destination or the one family owned.",
        [A.Research, A.Scout],
        [
            new(S.Research, "The question as the invoker stated it, and the destination it named for the findings.", new(F.ResearchMustReads, 1)),
            new(S.Scout, "The question as the researcher stated it, and the one family you own.", new(F.ScoutMustReads, 1))
        ]);

    public static readonly Claim ReadTicketAndBrief = new(
        "Read the owning ticket (its contract) first, and on a fix hop the FAIL block that sent you.",
        [A.TicketCaptain, A.CodeWriter, A.DocsWriter, A.Reviewer],
        [
            new(S.CodeWriter, "The owning ticket, and on a fix hop the FAIL block that sent you.", new(F.CodeWriterMustReads, 1)),
            new(S.Reviewer, "The contract the candidate must satisfy, as the brief pins it: outcome, scenarios, owned paths, gates, applicable feature-spec commit and sections; the brief's rubric, `Contract` (for a ticket, `<KEY> description as of <Linear updatedAt>`), Candidate SHA, Base SHA, and, where a writer produced the candidate, the writer's `IMPLEMENTED` line.", new(F.ReviewerMustReads, 1)),
            new(S.TicketCaptain, "The Linear ticket: outcome, owned paths, blockers, exact gates, and base branch.", new(F.TicketCaptainMustReads, 1))
        ]);

    public static readonly Claim ReadWritingDocs = new(
        "Read dydo/reference/writing-docs.md from the repository root.",
        [A.DocsWriter],
        [
            new(S.DocsWriter, "From the repository root, read `dydo/reference/writing-docs.md`.", new(F.DocsWriterMustReads, 2))
        ]);

    public static readonly Claim ReconcileAffectedContracts = new(
        "When shared feature intent changes, the spec is updated first and then the affected contracts are reconciled.",
        [A.Admiral],
        [
            new(S.Admiral, "Maintain shared feature intent following `dydo/reference/feature-spec-convention.md`, read from the repository root; reconcile affected contracts, each carrying every earlier ruling the user gave on its artifact as an acceptance line.", new(F.AdmiralMethod, 44), () => [ContractMoveToWriter.RecontractCarriesUserRulings!, Docs.MaintainFeatureIntentSpec])
        ]);

    public static readonly Claim ReconcileOlderGateContracts = new(
        "Reconcile any older active gate contract explicitly before changing its obligations.",
        [A.TicketCaptain, A.Reviewer],
        [
            new(S.Reviewer, "Check explicit reconciliation of older active gate contracts rather than silently dropping an obligation.", new(F.ReviewerMethod, 21)),
            new(S.TicketCaptain, "Reconcile any older active gate contract explicitly before changing its obligations.", new(F.TicketCaptainMethod, 11)),
            new(S.WorkingTreeStandard, "Reconcile existing active gate contracts explicitly before changing obligations.", new(F.WorkingTreeStandardDeliveryScaleAndEvidence, 7))
        ]);

    public static readonly Claim RecontractCarriesUserRulings = new(
        "A re-contract of an artifact carries every earlier ruling the user gave on it as an acceptance line.",
        [A.TicketCaptain, A.Admiral],
        [
            new(S.TicketCaptain, "A re-contract of an artifact carries every earlier ruling the user gave on it as an acceptance line.", new(F.TicketCaptainMethod, 9))
        ]);

    public static readonly Claim ScoutQuestionFixedFamilyOwn = new(
        "The scout's question is fixed and the family is the scout's alone (governing records, the rest of dydo, code and tests, git history, or outside sources).",
        [A.Scout],
        [
            new(S.Scout, "The question is fixed and the family is yours alone: governing records, the rest of dydo, code and tests, git history, or outside sources.", new(F.ScoutBoundary, 1))
        ]);

    public static readonly Claim SourcePrecedence = new(
        "Precedence of sources: user's live instruction, then DR, then reviewed architecture plan, then feature spec (both pinned), then ticket contract, then coding standards, then existing code.",
        [A.AnyAgent],
        [
            new(S.Admiral, "**Precedence:** user's live instruction → DR → reviewed architecture plan → feature spec (both pinned) → ticket contract → coding standards → existing code.", new(F.AdmiralBoundary, 23)),
            new(S.TicketCaptain, "**Precedence:** user's live instruction → DR → reviewed architecture plan → feature spec (both pinned) → ticket contract → coding standards → existing code.", new(F.TicketCaptainBoundary, 29))
        ]);

    public static readonly Claim TicketBoundsIntentAndPaths = new(
        "The current ticket bounds intent and paths.",
        [A.TicketCaptain], []);

    public static readonly Claim TypeShapeFromWorkspaceStandard = new(
        "Start from the Type's shape in the workspace standard; the contract makes the map exact when used.",
        [A.TicketCaptain],
        [
            new(S.TicketCaptain, "Start from the Type's shape in the workspace standard; when used, your contract makes the map exact.", new(F.TicketCaptainKindsAndFailurePaths, 1))
        ]);

    public static readonly Claim VerifyAtClaim = new(
        "At claim, verify reviewed intent, blockers, base branch, owned paths and gates.",
        [A.TicketCaptain],
        [
            new(S.TicketCaptain, "Verify reviewed intent, blockers, base branch, owned paths, and gates; satisfy the working-tree standard before spawning.", new(F.TicketCaptainMethod, 1), () => [WorktreeRootToT3.SatisfyStandardBeforeSpawning])
        ]);

    public static readonly Claim WriterPreCodeCommentNamesFurtherLanes = new(
        "When the work splits beyond the contract, the writer's one pre-code comment names the lanes.",
        [A.CodeWriter], []);
}
