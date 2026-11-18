namespace DynaDocs.Tests.SkillBuild.Fixture.Kinds;

using DynaDocs.SkillBuild;
using A = DynaDocs.Tests.SkillBuild.Fixture.FixtureActor;
using F = DynaDocs.Tests.SkillBuild.Fixture.FixtureSection;
using S = DynaDocs.Tests.SkillBuild.Fixture.FixtureSkill;

/// <summary>Migration fixture: the inventory's <c>escalation</c> claims, sub-topics afk to user.</summary>
public static class Escalation
{
    public static readonly Claim AfkUnexpectedChoiceBecomesQuestion = new(
        "On an AFK ticket an unexpected choice for the user becomes a blocking Question, and the parent stays AFK.",
        [A.TicketCaptain],
        [
            new(S.LinearWorkspaceStandard, "| `AFK` | The captain reaches reviewed completion without a live conversation. An unexpected choice for the user becomes a blocking `Question`; the parent stays AFK. | `#30A46C` |", new(F.LinearWorkspaceStandardMode, 1), () => [LinearAdmiralToOpen.ModeAfk])
        ]);

    public static readonly Claim AnswerDeterminesTicketResolveFirst = new(
        "If a Question's answer determines which implementation ticket should exist, resolve the Question first.",
        [A.MapHolder],
        [
            new(S.LinearWorkspaceStandard, "If the answer determines which implementation ticket should exist, resolve the Question first.", new(F.LinearWorkspaceStandardQuestionTickets, 4))
        ]);

    public static readonly Claim CrewQuestionsGoToCaptain = new(
        "Crew questions go back to the captain with what was searched and never directly to the user.",
        [A.CodeWriter, A.DocsWriter, A.Inquisitor, A.Crew],
        [
            new(S.CodeWriter, "Missing scope, a hard unknown or an open crossroads returns to it with what you searched, never to the user directly.", new(F.CodeWriterBoundary, 2)),
            new(S.DocsWriter, "Your questions go to it, never to the user directly.", new(F.DocsWriterBoundary, 3)),
            new(S.Inquisitor, "A question goes back to the captain with what you searched.", new(F.InquisitorBoundary, 4)),
            new(S.WorkingTreeStandard, "Crew hand their questions up to their captain and never ask the user directly.", new(F.WorkingTreeStandardDelegation, 21))
        ]);

    public static readonly Claim CrossTicketAnswersToMapHolder = new(
        "Anything that could affect other tickets, a shared contract, or the Project's destination, scope, acceptance criteria, architecture or map goes to the Project's map holder; for a Question the captain prepares the packet.",
        [A.TicketCaptain],
        [
            new(S.LinearWorkspaceStandard, "When the answer can change other tickets, a shared contract, or the Project's destination, scope, acceptance criteria or governing architecture, the captain prepares the packet and the Project's map holder creates and wires the Project-level ticket.", new(F.LinearWorkspaceStandardWhoHoldsWhat, 13), () => [Escalation.MapHolderCreatesProjectLevelTicket!]),
            new(S.TicketCaptain, "For a reported inherited or infrastructure failure, verify attribution and record its owner, repair/escalation route and effect on owed gates; raise cross-ticket scope to the map holder.", new(F.TicketCaptainMethod, 31), () => [EvidenceBugToPassing.InheritedFailureAttribution]),
            new(S.Wayfinder, "Anything that could affect another ticket, a shared contract, or the Project map is escalated to the Project's map holder instead.", new(F.WayfinderTheMap, 5))
        ]);

    public static readonly Claim DiscoveryBeforeQuestion = new(
        "Discovery comes before a Question: search the authoritative sources first (governing records, plan, specifications, documentation, standards, code, tests, prior answers), use Research, Prototype, Grilling or Enablement where they can clear it, and create a Question only when the user's judgment is still needed; if the sources settle it, record the answer where the work lives and continue, and no Question exists.",
        [A.TicketCaptain, A.Research, A.AnyAgent, A.MapHolder, A.Officer],
        [
            new(S.LinearWorkspaceStandard, "A Question is the last step of discovery, not its substitute.", new(F.LinearWorkspaceStandardQuestionTickets, 1)),
            new(S.Research, "A co-thinker, admiral, or Ticket Captain sends you after one fact a choice waits on, before anyone creates a human-facing Question Ticket; if the facts settle it, no Question Ticket exists.", new(F.ResearchBoundary, 1)),
            new(S.TicketCaptain, "Prefer `Research` when facts can settle it; use human-facing tickets only when necessary.", new(F.TicketCaptainBoundary, 34)),
            new(S.Wayfinder, "Use when authoritative evidence, inside or outside the repository, can settle the fact.", new(F.WayfinderTicketTypes, 4)),
            new(S.WhatWouldBalazsDo, "Research first: a call the facts settle is no Question.", new(F.WhatWouldBalazsDoWhen, 1))
        ]);

    public static readonly Claim EscalateOnlyConcreteMissing = new(
        "Escalate concrete missing authority, access or scope, not routine work.",
        [A.Admiral],
        [
            new(S.Admiral, "Escalate concrete missing authority, access or scope, not routine work.", new(F.AdmiralMethod, 53))
        ]);

    public static readonly Claim FifthConsecutiveFail = new(
        "A fifth consecutive review FAIL on one review loop escalates; record it on the ticket and wire a prepared Question as blocker.",
        [A.TicketCaptain, A.Admiral],
        [
            new(S.Admiral, "A fifth consecutive review FAIL on one review loop also escalates; record it on the ticket and wire a prepared Question as blocker.", new(F.AdmiralBoundary, 28)),
            new(S.TicketCaptain, "**Done:** each fix has its own commit and fresh review; the fifth consecutive FAIL stops the loop, records the findings and wires a prepared Question through the scope rule below.", new(F.TicketCaptainMethod, 45), () => [ReviewFixToPrototype.FixAtNewCommitAndFreshReview])
        ]);

    public static readonly Claim JudgmentCallIsAQuestion = new(
        "A judgment call is filed as a Question for the user, prepared by the standard's Question Tickets rules.",
        [A.TicketCaptain],
        [
            new(S.TicketCaptain, "**Judgment call:** a Question for the user, prepared by the standard's Question Tickets rules.", new(F.TicketCaptainInquisitionN8TriageEachReturn, 13))
        ]);

    public static readonly Claim LocalQuestionInTodoWired = new(
        "A Question is created in Todo, wired as a blocker of everything that waits, with its homework recorded.",
        [A.TicketCaptain, A.MapHolder],
        [
            new(S.LinearWorkspaceStandard, "Create the Question only when the user's judgment is still needed, in `Todo`, wired as a blocker of everything that waits.", new(F.LinearWorkspaceStandardQuestionTickets, 2)),
            new(S.Reviewer, "A sharp blocker left after authoritative homework and needing human judgment is a `Question` ticket in `Todo`, created by the map holder, recording its homework and blocking every ticket that waits on it, with priority by that standard.", new(F.ReviewerProjectPlanMethod, 7)),
            new(S.TicketCaptain, "File any other local Question Sub-ticket in `Todo`, wired to its waiters; send a prepared packet to the Project's map holder when its answer reaches other tickets or the Project's destination.", new(F.TicketCaptainRelease, 2))
        ]);

    public static readonly Claim MapHolderCreatesProjectLevelTicket = new(
        "The Project's map holder creates and wires the Project-level ticket from the captain's packet.",
        [A.MapHolder], []);

    public static readonly Claim MergeProductChoiceToCaptain = new(
        "A new product choice in a merge goes back to the captain.",
        [A.CodeWriter],
        [
            new(S.CodeWriter, "State each resolution's intended behaviour; a new product choice goes back to the captain.", new(F.CodeWriterMergeLead, 2), () => [MergeWriter.WriterStatesResolutionBehaviour])
        ]);

    public static readonly Claim ParentNotActiveHandDispatchUp = new(
        "On parent_not_active, hand the dispatch up to your officer.",
        [A.AnyAgent],
        [
            new(S.WorkingTreeStandard, "On `parent_not_active`, hand the dispatch up to your officer.", new(F.WorkingTreeStandardDelegation, 14))
        ]);

    public static readonly Claim PathCrewCaptainAdmiralUser = new(
        "Escalation runs crew, then Ticket Captain, then admiral, then user.",
        [A.AnyAgent],
        [
            new(S.Admiral, "**Escalation:** crew → Ticket Captain → admiral → user.", new(F.AdmiralBoundary, 26)),
            new(S.TicketCaptain, "**Escalation:** crew → Ticket Captain → `admiral`, when one is invoked → user.", new(F.TicketCaptainBoundary, 35))
        ]);

    public static readonly Claim QuestionPacketAndBody = new(
        "A Question carries a packet of the one question, the named work it blocks, the sources searched and facts found, and the credible options with a recommendation when evidence supports one, all on the record.",
        [A.TicketCaptain, A.MapHolder],
        [
            new(S.LinearWorkspaceStandard, "Its body carries: the one question; the named work it blocks and why; the sources searched and facts found; the credible options, trade-offs, and the recommendation when evidence supports one.", new(F.LinearWorkspaceStandardQuestionTickets, 3)),
            new(S.TicketCaptain, "Put the packet and all evidence on the record.", new(F.TicketCaptainRelease, 3)),
            new(S.Wayfinder, "The ticket carries the homework, credible options, trade-offs, and recommendation.", new(F.WayfinderTicketTypes, 7))
        ]);

    public static readonly Claim ReachUserOnlyFor = new(
        "Reach the user only for a DR conflict, live state the agents cannot coordinate, or missing authority.",
        [A.AnyAgent, A.Admiral],
        [
            new(S.Admiral, "Reach the user only for a DR conflict, live state the agents cannot coordinate, or missing authority.", new(F.AdmiralBoundary, 27)),
            new(S.TicketCaptain, "Reach the user only for a DR conflict, live state the agents cannot coordinate, or missing authority.", new(F.TicketCaptainBoundary, 36))
        ]);

    public static readonly Claim RemainingGatesToCaptain = new(
        "A docs-writer hands any remaining project-declared gates to the captain.",
        [A.DocsWriter],
        [
            new(S.DocsWriter, "Then commit in the owned paths; hand any remaining project-declared gates to the Captain.", new(F.DocsWriterMethod, 12), () => [WorktreeAuditToReviewer.CommitsTouchOwnedPathsOnly])
        ]);

    public static readonly Claim StopsPreservedNotRetried = new(
        "Safety, provenance and review stops and bounded refusals are preserved with their record, candidate, hop SHA and brief, and are not retried blindly or in loops.",
        [A.Admiral, A.AnyAgent, A.TicketCaptain],
        [
            new(S.Admiral, "On a bounded refusal, preserve the state and avoid blind retries; use established lifecycle handling, then escalate or release the concrete limitation when captain-owned work cannot run.", new(F.AdmiralBoundary, 7)),
            new(S.TicketCaptain, "On a bounded refusal, preserve the record, candidate, hop SHA and brief; do not broaden it or retry blindly.", new(F.TicketCaptainBoundary, 5)),
            new(S.WorkingTreeStandard, "On a bounded refusal, preserve the record, candidate, hop SHA and exact brief; do not broaden it or retry blindly.", new(F.WorkingTreeStandardDelegation, 22))
        ]);

    public static readonly Claim UnmeetableTargetNotLowered = new(
        "A stated target the design cannot meet is redesigned or returned to the captain, never lowered.",
        [A.CodeWriter],
        [
            new(S.CodeWriter, "So does a stated target your design cannot meet: redesign or return it, never lower it.", new(F.CodeWriterBoundary, 3))
        ]);

    public static readonly Claim UnresolvedAttributionToCaptain = new(
        "Unresolved failure attribution or missing support goes to the captain.",
        [A.CodeWriter],
        [
            new(S.CodeWriter, "Attribute failures by Evidence and inherited failures; send unresolved attribution or missing support to the captain.", new(F.CodeWriterProve, 3), () => [EvidenceBugToPassing.InheritedFailureAttribution])
        ]);

    public static readonly Claim UserAskedThroughHostQuestionTool = new(
        "An officer that needs the user while work runs asks through the host's question tool, and the Linear Question ticket stays the record.",
        [A.TicketCaptain, A.Officer],
        [
            new(S.Admiral, "Needing the user while work runs, ask through the host's question tool; the Linear Question ticket stays the record.", new(F.AdmiralBoundary, 14)),
            new(S.ChiefOfStaff, "Needing the user while work runs, ask through the host's question tool; the Linear Question ticket stays the record.", new(F.ChiefOfStaffBoundary, 9)),
            new(S.TicketCaptain, "Needing the user while work runs, ask through the host's question tool; the Linear Question ticket stays the record.", new(F.TicketCaptainBoundary, 19)),
            new(S.WorkingTreeStandard, "An officer that needs the user while its work runs asks through the host's question tool; T3 then lifts the thread into the user's queue.", new(F.WorkingTreeStandardDelegation, 20))
        ]);
}
