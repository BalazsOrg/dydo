namespace DynaDocs.Tests.SkillBuild.Fixture.Kinds;

using DynaDocs.SkillBuild;
using A = DynaDocs.Tests.SkillBuild.Fixture.FixtureActor;
using F = DynaDocs.Tests.SkillBuild.Fixture.FixtureSection;
using S = DynaDocs.Tests.SkillBuild.Fixture.FixtureSkill;

/// <summary>Migration fixture: the inventory's <c>identity</c> claims, sub-topics admiral to user.</summary>
public static class Identity
{
    public static readonly Claim AdmiralAccountableFor = new(
        "The admiral is accountable for Project-plan delivery, the feature branch, ticket sequencing, captain assignments, the integrated state, plan amendments, Linear evidence and the final return.",
        [A.Admiral],
        [
            new(S.Admiral, "**Accountable for:** Project-plan delivery, the feature branch, ticket sequencing, captain assignments, the integrated state, plan amendments, Linear evidence, and the final return.", new(F.AdmiralBoundary, 1))
        ]);

    public static readonly Claim AdmiralCommissionsOtherGitOperations = new(
        "The admiral commissions every other Git operation.",
        [A.Admiral],
        [
            new(S.Admiral, "Your Git authorship covers plans and feature specs; commission other operations.", new(F.AdmiralBoundary, 22), () => [Identity.AdmiralGitAuthorship!])
        ]);

    public static readonly Claim AdmiralGitAuthorship = new(
        "The admiral's Git authorship covers plans and feature specs.",
        [A.Admiral], []);

    public static readonly Claim AdmiralHoldsMap = new(
        "An invoked admiral holds the Project map until the user takes it back, owns the map and how its tickets move, integrate and finish together, and the frontline stays the user's.",
        [A.Admiral],
        [
            new(S.Admiral, "The user invoked you as a right hand for throughput, such as AFK work overnight: you hold the map until he takes it back, and the frontline stays his.", new(HeaderPart.Role, 2))
        ]);

    public static readonly Claim AdmiralProjectAltitude = new(
        "The admiral works at Project altitude: it coordinates captains, not their crew, and working inside one ticket means it has dropped the map.",
        [A.Admiral],
        [
            new(S.Admiral, "Captains own their tickets and direct their crews; you coordinate the captains rather than their crew.", new(F.AdmiralBoundary, 3))
        ]);

    public static readonly Claim BreakTouchesTestPathsOnly = new(
        "Break writes tests on its proof branch and touches test paths only.",
        [A.Inquisitor],
        [
            new(S.Inquisitor, "Break writes tests on its proof branch, and touches test paths only.", new(F.InquisitorBoundary, 3))
        ]);

    public static readonly Claim CaptainAccountableForTicket = new(
        "The ticket-captain stays accountable for the ticket, its evidence and every change, including crew edits and the complete integrated diff, though the crew does the work.",
        [A.TicketCaptain],
        [
            new(S.TicketCaptain, "Your crew works; you remain accountable for every change.", new(HeaderPart.Role, 2)),
            new(S.WorkingTreeStandard, "One Ticket Captain owns the parent ticket and its integrated outcome end to end.", new(F.WorkingTreeStandardInvariants, 2))
        ]);

    public static readonly Claim CaptainDirectsCrew = new(
        "The ticket-captain briefs, sequences, tracks, corrects and directs integration of the crew.",
        [A.TicketCaptain],
        [
            new(S.TicketCaptain, "Brief, sequence, track, correct, and direct integration.", new(F.TicketCaptainBoundary, 2))
        ]);

    public static readonly Claim CaptainOwnsStatusRecordsIntegration = new(
        "The ticket-captain owns status, records, integration and follow-up work; the writer on the change does not.",
        [A.CodeWriter, A.DocsWriter],
        [
            new(S.CodeWriter, "The Ticket Captain owns status, records and integration.", new(F.CodeWriterBoundary, 1)),
            new(S.DocsWriter, "The Ticket Captain that spawned you owns review, integration, status and follow-up work; you own the words.", new(F.DocsWriterBoundary, 2))
        ]);

    public static readonly Claim ContractSetsDestination = new(
        "The ticket's contract sets the destination; the code-writer chooses the route.",
        [A.TicketCaptain],
        [
            new(S.TicketCaptain, "The ticket is your ship: its contract sets the destination; the code-writer finds the route.", new(HeaderPart.Role, 1))
        ]);

    public static readonly Claim CosFixMechanicalHandBackJudgement = new(
        "The chief of staff fixes mechanical fields and broken links on sight and hands back every judgement call with a recommendation.",
        [A.ChiefOfStaff],
        [
            new(S.ChiefOfStaff, "Fix mechanical fields and broken links on sight, and hand back every judgement call with a recommendation.", new(F.ChiefOfStaffBoundary, 3))
        ]);

    public static readonly Claim CosNeverCarriesDelivery = new(
        "Delivery belongs to whoever owns it; the chief of staff never carries it.",
        [A.ChiefOfStaff],
        [
            new(S.ChiefOfStaff, "You triage, prepare and report; the user keeps every approval; delivery belongs to whoever owns it, and carrying it yourself costs the independence that makes your triage worth reading.", new(F.ChiefOfStaffBoundary, 2), () => [Identity.UserKeepsEveryApproval!])
        ]);

    public static readonly Claim CosStaffNotLine = new(
        "The chief of staff cuts across every stage of the map and owns none of it (staff, not line): it triages, prepares and reports.",
        [A.ChiefOfStaff],
        [
            new(S.ChiefOfStaff, "You cut across every stage of the map and own none of it: staff, not line.", new(F.ChiefOfStaffBoundary, 1))
        ]);

    public static readonly Claim CrewNoBranchPrMergeSelfReview = new(
        "Ordinary crew do not create the ticket's branch, open its PR, merge it or review their own work.",
        [A.Crew],
        [
            new(S.WorkingTreeStandard, "Ordinary crew do not create its branch, open its PR, merge it or review their own work.", new(F.WorkingTreeStandardDelegation, 26))
        ]);

    public static readonly Claim CrewRolesSplit = new(
        "The code and its proof belong to one code-writer, or to docs-writer for a documentation change; independent judgment belongs to the reviewer.",
        [A.TicketCaptain],
        [
            new(S.TicketCaptain, "**Crew:** the code and its proof belong to one `code-writer`, or to `docs-writer` for a documentation change; independent judgment to `reviewer`.", new(F.TicketCaptainBoundary, 1))
        ]);

    public static readonly Claim DefaultOfficer = new(
        "The ticket-captain is the default officer: every ticket gets a captain whether or not the user invoked an admiral.",
        [A.TicketCaptain],
        [
            new(S.TicketCaptain, "You are the default officer: a ticket gets a captain whether or not the user has invoked an admiral.", new(HeaderPart.Role, 3))
        ]);

    public static readonly Claim DocsWriterWritesOneChange = new(
        "The docs-writer writes or corrects one reviewed documentation change, an inquisition's own record included, for its Ticket Captain, and makes it true.",
        [A.DocsWriter],
        [
            new(S.DocsWriter, "Write or correct one reviewed change, an inquisition's own record included, for its Ticket Captain.", new(HeaderPart.Role, 1))
        ]);

    public static readonly Claim DriveLeavesVerificationSkill = new(
        "Drive leaves the verification skill as it is; the gap ticket fixes it.",
        [A.Inquisitor],
        [
            new(S.Inquisitor, "Leave the verification skill as it is; the gap ticket fixes it.", new(F.InquisitorDriveWork, 3))
        ]);

    public static readonly Claim InquisitionCaptainConducts = new(
        "In an Inquisition the captain conducts and the inquisitor crew hunts.",
        [A.TicketCaptain],
        [
            new(S.TicketCaptain, "You conduct; the crew hunts.", new(F.TicketCaptainInquisitionLead, 1))
        ]);

    public static readonly Claim InquisitionProofsBelongToBreakInquisitor = new(
        "An Inquisition's proofs belong to its Break-phase inquisitor, which writes them on its proof branch.",
        [A.CodeWriter, A.TicketCaptain],
        [
            new(S.CodeWriter, "An Inquisition's proofs are its Break-phase inquisitor's, not yours; its patch lane briefs you as a Bug.", new(F.CodeWriterBugOrFeature, 11), () => [InquisitionGroupToRead.PatchCodeWriterBrief]),
            new(S.TicketCaptain, "Set `In Progress` on the user's confirmation and run it by [inquisition](resources/inquisition.md): you draw the area map and conduct `inquisitor` crew through Read, Break and Drive on each area; Break writes its own proof tests on its proof branch.", new(F.TicketCaptainKindsAndFailurePaths, 10), () => [InquisitionRecordToWork.RunByResourcePipeline, InquisitionRecordToWork.StartOnUserConfirmation])
        ]);

    public static readonly Claim InquisitorReadsOwnPhaseChecklist = new(
        "An inquisitor reads the checklist of its own phase: read, break or drive.",
        [A.Inquisitor],
        [
            new(S.Inquisitor, "Your phase's checklist: [read](resources/read.md), [break](resources/break.md) or [drive](resources/drive.md).", new(F.InquisitorMustReads, 2))
        ]);

    public static readonly Claim InquisitorSkillTrigger = new(
        "The inquisitor skill is used when an inquisition captain sends a phase (Read, Break or Drive) and an area.",
        [A.Inquisitor],
        [
            new(S.Inquisitor, "Use when an inquisition's captain sends you a phase and an area.", new(HeaderPart.Role, 1))
        ]);

    public static readonly Claim InquisitorWorkerOneReturn = new(
        "An inquisitor is a worker: one phase, one area, one return to the captain who sent it; the captain files tickets, prepares patches and keeps the register, and the inquisitor outputs only its return.",
        [A.Inquisitor],
        [
            new(S.Inquisitor, "You are a worker: one phase, one area, one return to the captain who sent you.", new(F.InquisitorBoundary, 1))
        ]);

    public static readonly Claim MapHolderDefinition = new(
        "The map holder is the user, or an admiral the user invoked, for a Project, and the captain for a ticket.",
        [A.MapHolder],
        [
            new(S.LinearWorkspaceStandard, "The map holder is the user, or an admiral the user invoked, for a Project, and the captain for a ticket; the Project description names the Project's current holder.", new(F.LinearWorkspaceStandardWhoHoldsWhat, 1), () => [Identity.ProjectDescriptionNamesMapHolder!]),
            new(S.Wayfinder, "A Project's map holder is the user, driving the map in his own session, or an admiral the user invoked, until the user takes the map back; the Project description names the current holder.", new(HeaderPart.Role, 3), () => [Identity.ProjectDescriptionNamesMapHolder!])
        ]);

    public static readonly Claim MapHolderHeldTicketNoCaptainBranchPrReview = new(
        "A map-holder-held ticket receives no captain, branch, PR or review loop.",
        [A.MapHolder],
        [
            new(S.LinearWorkspaceStandard, "A map-holder-held ticket is run directly by the map holder whose map it clears; it receives no captain, branch, PR or review loop.", new(F.LinearWorkspaceStandardType, 15), () => [Identity.MapHolderHeldTicketRunDirectly!])
        ]);

    public static readonly Claim MapHolderHeldTicketRunDirectly = new(
        "A map-holder-held ticket is run directly by the map holder whose map it clears.",
        [A.MapHolder], []);

    public static readonly Claim MapHolderRunsFogTickets = new(
        "The map holder runs the map-holder-held tickets that clear its own fog: it dispatches research, runs a Grilling or Walkthrough with the user, and files a Question only when judgment remains.",
        [A.MapHolder], []);

    public static readonly Claim MutationTestingIsReviewers = new(
        "Mutation testing belongs to the reviewer, not the code-writer.",
        [A.Reviewer],
        [
            new(S.CodeWriter, "Mutation is the reviewer's.", new(F.CodeWriterProve, 5))
        ]);

    public static readonly Claim OfficerAuthorsNoProductionChange = new(
        "An admiral or ticket-captain authors no production change.",
        [A.Admiral, A.TicketCaptain],
        [
            new(S.Admiral, "Neither role authors production changes or reviews its own candidate.", new(F.AdmiralBoundary, 21), () => [ReviewFixToPrototype.NeverSelfReview]),
            new(S.TicketCaptain, "Author no production change and never review your own candidate.", new(F.TicketCaptainBoundary, 21), () => [ReviewFixToPrototype.NeverSelfReview])
        ]);

    public static readonly Claim OfficersDirectCrewProduces = new(
        "Admirals and captains direct the work; the crew produces it.",
        [A.Admiral, A.TicketCaptain, A.Officer],
        [
            new(S.Admiral, "**Guardrail:** admirals and captains direct the work; the crew produces it.", new(F.AdmiralBoundary, 20)),
            new(S.TicketCaptain, "**Guardrail:** admirals and captains direct the work; the crew produces it.", new(F.TicketCaptainBoundary, 20))
        ]);

    public static readonly Claim ProjectDescriptionNamesMapHolder = new(
        "The Project description names the Project's current map holder.",
        [A.MapHolder], []);

    public static readonly Claim ReadAndDriveChangeNothing = new(
        "Read and Drive change nothing in the repository.",
        [A.Inquisitor],
        [
            new(S.Inquisitor, "Read and Drive change nothing in the repository.", new(F.InquisitorBoundary, 2))
        ]);

    public static readonly Claim ResearchFactsNotChoice = new(
        "Facts are the researcher's job; the choice they inform stays with the invoker and the user.",
        [A.Research],
        [
            new(S.Research, "Facts are your job; the choice they inform stays with the invoker and the user.", new(F.ResearchBoundary, 2))
        ]);

    public static readonly Claim ReviewerJudgesOneCandidate = new(
        "The reviewer judges one candidate against one rubric and lets no blocker past; corrections and status are the invoker's.",
        [A.Reviewer],
        [
            new(S.Reviewer, "Gandalf at the bridge: judge one candidate against one rubric, and let no blocker past.", new(HeaderPart.Role, 1))
        ]);

    public static readonly Claim ScoutCollectsResearcherConcludes = new(
        "The scout collects evidence from one source family for one question and concludes nothing; the researcher verifies, pools and concludes.",
        [A.Research, A.Scout],
        [
            new(S.Scout, "Collect evidence from one source family for one question; you conclude nothing.", new(HeaderPart.Role, 1))
        ]);

    public static readonly Claim SurveyorReadOnlyNoOfficerRole = new(
        "A surveyor is its officer's read-only surveyor: it reads its own section or resource alone and assumes no officer role.",
        [A.Crew],
        [
            new(S.Admiral, "You are the Admiral's read-only surveyor.", new(F.AdmiralCheckInSurveyTheSurveyor, 1)),
            new(S.ChiefOfStaff, "You are the Chief of Staff's read-only surveyor.", new(F.ChiefOfStaffNightSurveyLead, 1))
        ]);

    public static readonly Claim SurveyorSendsEditsStartsNothing = new(
        "A surveyor sends no messages, edits nothing and starts no work, subagents or schedules.",
        [A.Crew],
        [
            new(S.Admiral, "Send no messages; edit nothing; start no work, subagents or schedules.", new(F.AdmiralCheckInSurveyTheSurveyor, 2)),
            new(S.ChiefOfStaff, "Send no messages or nudges; edit no boards, records or files; make no decisions; start no work, subagents or schedules.", new(F.ChiefOfStaffNightSurveyLead, 3))
        ]);

    public static readonly Claim TaskOwnedByCaptainAndCrew = new(
        "A Task is owned end to end by a Ticket Captain, who with the crew carries it through its contract, production, review and Merge Sub-ticket.",
        [A.TicketCaptain, A.Crew],
        [
            new(S.Wayfinder, "Tasks are the route someone builds; a Ticket Captain owns each one end to end.", new(F.WayfinderTickets, 1))
        ]);

    public static readonly Claim UserKeepsEveryApproval = new(
        "The user keeps every approval.",
        [A.ChiefOfStaff], []);

    public static readonly Claim UserQueueThreeLists = new(
        "The user's queue is the three named lists and never the assignee filter.",
        [A.ChiefOfStaff],
        [
            new(S.ChiefOfStaff, "The user's queue is these and never the assignee filter.", new(F.ChiefOfStaffMethod, 7))
        ]);

    public static readonly Claim UserWorkReachesTicketViaCaptain = new(
        "Anything the user asks the admiral to get done reaches a ticket through a captain; for hands-on work the admiral declines and offers to commission a captain or have the user open a captain session.",
        [A.Admiral],
        [
            new(S.Admiral, "Anything the user asks you to get done reaches a ticket through a captain.", new(F.AdmiralBoundary, 4))
        ]);
}
