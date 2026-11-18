namespace DynaDocs.Tests.SkillBuild.Fixture;

using DynaDocs.SkillBuild;
using A = DynaDocs.Tests.SkillBuild.Fixture.FixtureActor;
using F = DynaDocs.Tests.SkillBuild.Fixture.FixtureSection;
using S = DynaDocs.Tests.SkillBuild.Fixture.FixtureSkill;

/// <summary>Migration fixture: the inventory's <c>dispatch</c> claims, sub-topics one to without.</summary>
public static class DispatchOneToWithout
{
    public static readonly Claim OneAgentPerAreaPerPhase = new(
        "Dispatch one agent per area per phase.",
        [A.TicketCaptain],
        [
            new(S.TicketCaptain, "Dispatch one agent per area per phase, and set no quota of hypotheses or journeys.", new(F.TicketCaptainInquisitionN6ScheduleThePipeline, 3), () => [InquisitionGroupToRead.NoHypothesisQuota])
        ]);

    public static readonly Claim OneAreaPerDispatch = new(
        "A dispatch never carries more than one area.",
        [A.TicketCaptain],
        [
            new(S.TicketCaptain, "Merge tiny areas into one area here; a dispatch never carries more than one area.", new(F.TicketCaptainInquisitionN3DrawTheAreaMap, 4))
        ]);

    public static readonly Claim OneCaptainPerPickableTicket = new(
        "Give each pickable ticket (and each Inquisition group) to one ticket-captain.",
        [A.Admiral, A.Officer],
        [
            new(S.Admiral, "**Command:** give each pickable ticket to one `ticket-captain`.", new(F.AdmiralBoundary, 2)),
            new(S.TicketCaptain, "Wire the parent blocked by every group, open this round's nit ticket (step 8), then release the parent by the Release section with `released <key>: groups <keys>`; your officer commissions one captain per group.", new(F.TicketCaptainInquisitionN4SplitAboveAbout10Areas, 4), () => [InquisitionGroupToRead.GroupReleaseWiring])
        ]);

    public static readonly Claim OneCodeWriterPerParentOrLane = new(
        "Send one code-writer per parent or lane, which builds and proves the contract in one hop; a captain-held ticket normally uses one author and review at the Delivery scale.",
        [A.TicketCaptain],
        [
            new(S.LinearWorkspaceStandard, "A captain-held ticket normally uses one author - `code-writer`, or `docs-writer` for a documentation change - and review at the working-tree standard's [Delivery scale](../guides/working-tree-standard.md#delivery-scale-and-evidence).", new(F.LinearWorkspaceStandardType, 11)),
            new(S.TicketCaptain, "Send one `code-writer` per parent or lane, which builds and proves the contract in one hop.", new(F.TicketCaptainMethod, 18)),
            new(S.WorkingTreeStandard, "The default crew is one author - `code-writer`, or `docs-writer` for a documentation change - and fresh independent whole-change review, whose first round on a candidate seats two reviewers in parallel, one Claude and one Codex, their findings merged into one block that FAILs only on a blocker.", new(F.WorkingTreeStandardDeliveryScaleAndEvidence, 1), () => [ReviewReadToWrong.TwoReviewersFirstRound])
        ]);

    public static readonly Claim OnlyCaptainCommissionsCrew = new(
        "Only the ticket-captain commissions exact-scope delivery crew.",
        [A.TicketCaptain],
        [
            new(S.TicketCaptain, "**Native capacity:** You alone commission exact-scope crew.", new(F.TicketCaptainBoundary, 3)),
            new(S.WorkingTreeStandard, "The captain alone commissions exact-scope delivery crew, budgets that capacity and preserves the fresh reviewer obligation; `research` still sends its own scouts.", new(F.WorkingTreeStandardDelegation, 2), () => [DispatchOneToWithout.PreserveFreshReviewerObligation!, DispatchOneToWithout.ResearchOneScoutPerFamily!])
        ]);

    public static readonly Claim OtherVendorModelCandidates = new(
        "Candidate models include other vendors' wherever the host can dispatch them.",
        [A.Admiral, A.TicketCaptain],
        [
            new(S.Admiral, "Candidates include other vendors' models wherever the host can dispatch them.", new(F.AdmiralMethod, 30)),
            new(S.TicketCaptain, "Candidates include other vendors' models wherever the host can dispatch them; weigh them on the same terms as your own.", new(F.TicketCaptainMethod, 28))
        ]);

    public static readonly Claim OutlastingWorkByDelegateTask = new(
        "Work that cannot finish inside the turn goes through delegate_task instead.",
        [A.AnyAgent, A.TicketCaptain],
        [
            new(S.TicketCaptain, "Stay in your turn while you wait on a native child or a result you own; work that outlasts the turn goes by `delegate_task`.", new(F.TicketCaptainBoundary, 10), () => [DispatchOneToWithout.StayInTurnWhileWaiting!]),
            new(S.WorkingTreeStandard, "Work that cannot finish inside the turn goes through `delegate_task` instead.", new(F.WorkingTreeStandardDelegation, 16))
        ]);

    public static readonly Claim PeerOfOtherVendorByT3Messaging = new(
        "A peer of another vendor is reached by T3 messaging.",
        [A.AnyAgent],
        [
            new(S.WorkingTreeStandard, "A peer of another vendor is reached by T3 messaging.", new(F.WorkingTreeStandardDelegation, 9))
        ]);

    public static readonly Claim PreserveFreshReviewerObligation = new(
        "Preserve the fresh reviewer obligation.",
        [A.TicketCaptain], []);

    public static readonly Claim ProofOnlyBriefNames = new(
        "A proof-only brief names the commissioning captain, the pinned source SHA and the test path.",
        [A.TicketCaptain, A.Officer],
        [
            new(S.CodeWriter, "**Proof-only**: the brief names the commissioning captain, the pinned source SHA and the test path.", new(F.CodeWriterBugOrFeature, 9)),
            new(S.WorkingTreeStandard, "A proof-only brief names the commissioning captain, the pinned source SHA and the test path.", new(F.WorkingTreeStandardBeforeTheFirstEdit, 8))
        ]);

    public static readonly Claim ReconciliationCommissionsAfkTickets = new(
        "On each reconciliation the admiral commissions each authorized pickable AFK ticket, including blocker-cleared and released ones, from its record.",
        [A.Admiral], []);

    public static readonly Claim RequestedValueEffectiveIdentityByTelemetry = new(
        "Claim an effective model identity only where trustworthy runtime information or telemetry shows it, and otherwise qualify a family or a requested/configured value.",
        [A.TicketCaptain, A.AnyAgent],
        [
            new(S.LinearWorkspaceStandard, "State the effective identity when trustworthy runtime information exposes it.", new(F.LinearWorkspaceStandardTruthfulCommentSignatures, 2))
        ]);

    public static readonly Claim ResearchOneScoutPerFamily = new(
        "Research sends one scout per family in play, in parallel, each carrying the question, its family and the scout brief; when only one family is in play, read it yourself.",
        [A.Research],
        [
            new(S.Research, "One scout per family in play, in parallel, each carrying the question, its family, and the brief in the `scout` agent; when one family alone is in play, read it yourself.", new(F.ResearchMethod, 4))
        ]);

    public static readonly Claim ReturnComesBackToCaptain = new(
        "Every crew return comes back to the ticket-captain.",
        [A.TicketCaptain, A.Crew],
        [
            new(S.TicketCaptain, "Every crew return comes back to you.", new(F.TicketCaptainBoundary, 7))
        ]);

    public static readonly Claim ReturnOnceOnDispatchChannel = new(
        "A return comes back once, on the channel it was dispatched on.",
        [A.AnyAgent],
        [
            new(S.WorkingTreeStandard, "A return comes back once, on the channel it was dispatched on.", new(F.WorkingTreeStandardDelegation, 11))
        ]);

    public static readonly Claim RouteDocsToDocsWriter = new(
        "Route documentation to docs-writer.",
        [A.TicketCaptain], []);

    public static readonly Claim RunDisjointLanesConcurrently = new(
        "Run disjoint lanes concurrently.",
        [A.TicketCaptain],
        [
            new(S.TicketCaptain, "Run disjoint lanes concurrently and keep every attempt on its existing record.", new(F.TicketCaptainMethod, 23), () => [LinearAdmiralToOpen.KeepAttemptsOnExistingRecord])
        ]);

    public static readonly Claim SavedBriefNotAdmiralAuthority = new(
        "A saved brief is not Admiral-to-crew authority.",
        [A.Admiral, A.Crew],
        [
            new(S.Admiral, "A saved brief is not Admiral-to-crew authority.", new(F.AdmiralBoundary, 6)),
            new(S.WorkingTreeStandard, "A saved brief is not Admiral-to-crew authority.", new(F.WorkingTreeStandardDelegation, 3))
        ]);

    public static readonly Claim ScoutReadsItselfNoCrew = new(
        "A scout reads its family itself, in its own thread, and has no crew.",
        [A.Scout],
        [
            new(S.Scout, "Read it yourself, in this thread; a scout has no crew.", new(F.ScoutBoundary, 2))
        ]);

    public static readonly Claim SelectModelAndEffortTogether = new(
        "Select model and effort together where the host takes both; where it takes only a model, leave effort host-owned and record that limit.",
        [A.Admiral, A.TicketCaptain],
        [
            new(S.Admiral, "Select model and effort together where the host takes both; where it takes only a model, leave effort host-owned and record that limit.", new(F.AdmiralMethod, 28)),
            new(S.TicketCaptain, "Select model and effort together where the host takes both; where it takes only a model, leave effort host-owned and record that limit.", new(F.TicketCaptainMethod, 26))
        ]);

    public static readonly Claim StartAdequateEscalate = new(
        "Start with an adequate model and effort and escalate on evidence.",
        [A.Admiral, A.TicketCaptain],
        [
            new(S.Admiral, "Start adequate and escalate on evidence; keep every review and gate at full strength whatever it costs.", new(F.AdmiralMethod, 27), () => [ReviewFixToPrototype.FullStrengthWhateverCost]),
            new(S.TicketCaptain, "Start adequate and escalate on evidence.", new(F.TicketCaptainMethod, 25))
        ]);

    public static readonly Claim StateRequestedValueSoSignatureIsTruthful = new(
        "State the requested model/effort value so the crew member's or agent's signature is truthful.",
        [A.TicketCaptain, A.AnyAgent], []);

    public static readonly Claim StayInTurnWhileWaiting = new(
        "A dispatcher waiting on a native child, or a result it owns, stays in its turn until the result arrives.",
        [A.TicketCaptain, A.AnyAgent],
        [
            new(S.Admiral, "Stay in your turn while you wait on a native child.", new(F.AdmiralBoundary, 11)),
            new(S.ChiefOfStaff, "Stay in your turn while you wait on a native child.", new(F.ChiefOfStaffBoundary, 6)),
            new(S.WorkingTreeStandard, "A dispatcher waiting on a native child, or a child awaiting a result it owns (a reviewer, a suite), stays in its turn until the result arrives.", new(F.WorkingTreeStandardDelegation, 15))
        ]);

    public static readonly Claim SurveyorGetsResourceAlone = new(
        "Give the surveyor child the night-survey resource alone, not the chief-of-staff officer role.",
        [A.ChiefOfStaff],
        [
            new(S.ChiefOfStaff, "Give the child that resource alone, not this officer role.", new(F.ChiefOfStaffOvernightDuties, 4))
        ]);

    public static readonly Claim T3DelegateTaskForReturningChildren = new(
        "Under T3, send any child that returns to its dispatcher by delegate_task, as a child of the dispatcher's own turn.",
        [A.Admiral, A.ChiefOfStaff, A.TicketCaptain, A.AnyAgent],
        [
            new(S.WorkingTreeStandard, "`delegate_task` for every other child that returns to its dispatcher: captains, crew, reviewers, and any agent of another vendor.", new(F.WorkingTreeStandardDelegation, 6))
        ]);

    public static readonly Claim TwoJudgesClaudeAndCodex = new(
        "Start two fresh judges as T3 delegate_task children, one Claude and one Codex, each with the strongest model and effort set explicitly.",
        [A.Officer],
        [
            new(S.WhatWouldBalazsDo, "Start two fresh judges as T3 `delegate_task` children, one Claude and one Codex, each with the strongest model and effort set explicitly.", new(F.WhatWouldBalazsDoTribunalPhases, 5))
        ]);

    public static readonly Claim WithoutT3NativeSubagents = new(
        "Without T3, use the host's native subagents.",
        [A.TicketCaptain, A.AnyAgent],
        [
            new(S.Admiral, "Without T3, the host's native subagents.", new(F.AdmiralBoundary, 9)),
            new(S.ChiefOfStaff, "Without T3, the host's native subagents.", new(F.ChiefOfStaffBoundary, 5)),
            new(S.TicketCaptain, "Without T3, the host's native subagents.", new(F.TicketCaptainBoundary, 9)),
            new(S.WorkingTreeStandard, "Without T3, use the host's native subagents and messaging, or none.", new(F.WorkingTreeStandardDelegation, 10))
        ]);
}
