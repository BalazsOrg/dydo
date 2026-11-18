namespace DynaDocs.Tests.SkillBuild.Fixture;

using DynaDocs.SkillBuild;
using A = DynaDocs.Tests.SkillBuild.Fixture.FixtureActor;
using F = DynaDocs.Tests.SkillBuild.Fixture.FixtureSection;
using S = DynaDocs.Tests.SkillBuild.Fixture.FixtureSkill;

/// <summary>Migration fixture: the inventory's <c>dispatch</c> claims, sub-topics captain to no.</summary>
public static class DispatchCaptainToNo
{
    public static readonly Claim CaptainSpecifiesParentBeforeLanesMergeSubtickets = new(
        "The captain specifies its parent before naming disjoint parallel lanes, and each merging lane or ticket gets its own Merge Sub-ticket in order, except an atomic ticket's own merge into main.",
        [A.TicketCaptain],
        [
            new(S.Wayfinder, "The captain specifies its parent before naming disjoint parallel lanes, and each merging lane or ticket gets its own Merge Sub-ticket in order, except an atomic ticket's own merge into main.", new(F.WayfinderTickets, 9))
        ]);

    public static readonly Claim ChannelFollowsWhereResultAwaited = new(
        "Under T3, the dispatch channel follows where the child's result is awaited.",
        [A.AnyAgent],
        [
            new(S.WorkingTreeStandard, "Under T3, the channel follows where the child's result is awaited.", new(F.WorkingTreeStandardDelegation, 4))
        ]);

    public static readonly Claim CheckInSurveyBriefFixed = new(
        "The fixed check-in surveyor dispatch brief has: the absolute path to check-in.md and its Survey section only, the Project ID, the pasted ledger, and the parent return address with return through the task result only.",
        [A.Admiral],
        [
            new(S.Admiral, "```text\nRead <absolute path to admiral/resources/check-in.md>; perform only its Survey section.\nProject: <Project ID>\nLedger: <pasted from the Project description>\nParent return address: <Admiral thread link>; return through this task's result only.\n```", new(F.AdmiralCheckInFixedDispatch, 1))
        ]);

    public static readonly Claim ChooseCrewCapability = new(
        "Choose each commission's or crew member's capability yourself: the strongest supported model for real engineering challenges, the smallest adequate one otherwise, and the effort where the host exposes one, weighed from the task's difficulty, uncertainty, consequence of error, required independence, context size and likely retries.",
        [A.Admiral, A.TicketCaptain],
        [
            new(S.Admiral, "Choose each commission's capability yourself: the strongest supported model for real engineering challenges, the smallest adequate one otherwise, and the effort where the host exposes one, weighed from that ticket's difficulty, uncertainty, consequence of error, required independence, context size and likely retries.", new(F.AdmiralMethod, 26)),
            new(S.TicketCaptain, "Choose each crew member's capability as you brief it: the strongest supported model for real engineering challenges, the smallest adequate one otherwise, and the effort where the host exposes one, weighed from that task's difficulty, uncertainty, consequence of error, required independence, context size and likely retries.", new(F.TicketCaptainMethod, 24))
        ]);

    public static readonly Claim ConsumeRunningSurveyNoSecond = new(
        "If a survey is already running or pending, consume it and dispatch no second one.",
        [A.Admiral, A.ChiefOfStaff],
        [
            new(S.Admiral, "[ ] A survey is still running: consume it; dispatch no second one.", new(F.AdmiralCheckInTickTheAdmiral, 3)),
            new(S.ChiefOfStaff, "If a survey is pending, consume that child instead of dispatching another.", new(F.ChiefOfStaffOvernightDuties, 5))
        ]);

    public static readonly Claim DispatchFreshSurveyor = new(
        "The admiral dispatches one fresh surveyor on the smallest adequate model with the fixed dispatch brief to read the board and return actions.",
        [A.Admiral],
        [
            new(S.Admiral, "A cheap surveyor reads the board and returns actions; the Admiral acts on them.", new(F.AdmiralCheckInLead, 1), () => [ProjectMapToWatch.SurveyFindingToAction])
        ]);

    public static readonly Claim DydoCompilesNoCapabilityTable = new(
        "dydo compiles no model, effort or standing capability table.",
        [A.TicketCaptain, A.AnyAgent],
        [
            new(S.Admiral, "State the requested value in your signature and claim an effective identity only where telemetry shows it. dydo compiles no model, no effort and no standing capability table.", new(F.AdmiralMethod, 29), () => [DispatchOneToWithout.RequestedValueEffectiveIdentityByTelemetry, DispatchOneToWithout.StateRequestedValueSoSignatureIsTruthful]),
            new(S.TicketCaptain, "Brief the requested value so the crew member's signature is truthful, and claim an effective identity only where telemetry shows it. dydo compiles no model, no effort and no standing capability table.", new(F.TicketCaptainMethod, 27), () => [DispatchOneToWithout.RequestedValueEffectiveIdentityByTelemetry, DispatchOneToWithout.StateRequestedValueSoSignatureIsTruthful])
        ]);

    public static readonly Claim FailedSurveyNotRunning = new(
        "A failed or lost survey is not running.",
        [A.Admiral],
        [
            new(S.Admiral, "A failed or lost survey is not running.", new(F.AdmiralCheckInTickTheAdmiral, 4))
        ]);

    public static readonly Claim FreshAgentForRestOfArea = new(
        "The captain sends a fresh agent for the rest of an area when an agent hands work items back or stops past its context limit.",
        [A.TicketCaptain],
        [
            new(S.Inquisitor, "The captain sends a fresh agent for the rest of the area.", new(F.InquisitorContextSizeRunItAfterEachWorkItem, 3)),
            new(S.TicketCaptain, "A return with work items handed back gets a fresh agent for the rest of the area, briefed with those items; a hand-back or follow-up Break agent gets the next `<n>`.", new(F.TicketCaptainInquisitionN6ScheduleThePipeline, 6), () => [DispatchCaptainToNo.NextNForFollowUpBreakAgent!])
        ]);

    public static readonly Claim HitlOwnTopLevelThread = new(
        "HITL work, including an HITL captain, runs in its own new top-level thread.",
        [A.Admiral, A.TicketCaptain],
        [
            new(S.Admiral, "On each reconciliation commission each authorized pickable AFK ticket, including blocker-cleared and released ones, from its record; open each HITL captain in a new top-level thread.", new(F.AdmiralMethod, 24), () => [DispatchOneToWithout.ReconciliationCommissionsAfkTickets]),
            new(S.TicketCaptain, "**Human loop:** HITL runs in its own top-level thread.", new(F.TicketCaptainBoundary, 27))
        ]);

    public static readonly Claim InquisitionAgentBriefContents = new(
        "Brief each inquisition agent with its phase, area with map row and register entries, audit SHA and landing base, governing intent, context limit, worktree, and for Break the proof branch, for Drive the verification skill and the API base.",
        [A.TicketCaptain],
        [
            new(S.Inquisitor, "The brief: your phase, your area with its map row and register entries, the audit SHA and landing base, the governing intent, your worktree, your context limit, and for Break the proof branch, for Drive the verification skill and the API base.", new(F.InquisitorMustReads, 1)),
            new(S.TicketCaptain, "Brief each agent with its phase, its area's map row and register entries, the audit SHA and landing base, the governing intent, its context limit, and its worktree: one at the audit SHA for Read and Drive; for Break, one on its own proof branch `proof/<KEY>-<area>-<n>` cut from the audit SHA, with the test paths it may touch, `<n>` counting the Break dispatches for that area; for Drive, the verification skill and the API base.", new(F.TicketCaptainInquisitionN6ScheduleThePipeline, 5))
        ]);

    public static readonly Claim InquisitionCrewOwnSessionDelegateTask = new(
        "Dispatch each Inquisition crew agent as its own session through T3 delegate_task (DR 057), because crew outlive the captain's turn and the context check needs its own session.",
        [A.TicketCaptain],
        [
            new(S.Inquisitor, "Inquisition crew run as their own sessions, dispatched through T3 `delegate_task` (DR 057), because they outlive the captain's turn.", new(F.InquisitorContextSizeOwnSessionOnly, 1)),
            new(S.TicketCaptain, "Dispatch each as its own session through T3 `delegate_task` (DR 057): crew outlive your turn, and the context check needs its own session.", new(F.TicketCaptainInquisitionN6ScheduleThePipeline, 4))
        ]);

    public static readonly Claim LoadCodebaseDesign = new(
        "Load codebase-design when designing a seam or interface.",
        [A.CodeWriter],
        [
            new(S.CodeWriter, "`codebase-design`: you design a seam or interface.", new(F.CodeWriterBuild, 3))
        ]);

    public static readonly Claim LoadDiagnosingBugs = new(
        "The code-writer loads diagnosing-bugs when a Bug other than an unused member lacks a red reproduction.",
        [A.CodeWriter],
        [
            new(S.CodeWriter, "`diagnosing-bugs`: a Bug other than an unused member lacks a red reproduction.", new(F.CodeWriterBuild, 2)),
            new(S.TicketCaptain, "Route docs to `docs-writer`; the code-writer uses `diagnosing-bugs` when a Bug other than an unused member lacks a red reproduction.", new(F.TicketCaptainMethod, 19), () => [DispatchOneToWithout.RouteDocsToDocsWriter])
        ]);

    public static readonly Claim LoadDomainModeling = new(
        "Load domain-modeling when touching a glossary term or Decision Record.",
        [A.CodeWriter],
        [
            new(S.CodeWriter, "`domain-modeling`: you touch a glossary term or Decision Record.", new(F.CodeWriterBuild, 4))
        ]);

    public static readonly Claim LoadPrototype = new(
        "Load prototype when a design question is open; stop and return the question.",
        [A.CodeWriter],
        [
            new(S.CodeWriter, "`prototype`: a design question is open; stop and return it.", new(F.CodeWriterBuild, 6))
        ]);

    public static readonly Claim LoadWritingForAgents = new(
        "Load writing-for-agents when editing a skill or entry file.",
        [A.CodeWriter],
        [
            new(S.CodeWriter, "`writing-for-agents`: you edit a skill or entry file.", new(F.CodeWriterBuild, 5))
        ]);

    public static readonly Claim NativeCaptainNoCodexReviewer = new(
        "A native captain cannot send its own Codex reviewer; a delegate_task captain can.",
        [A.TicketCaptain],
        [
            new(S.WorkingTreeStandard, "A native captain therefore cannot send its own Codex reviewer; a `delegate_task` captain can.", new(F.WorkingTreeStandardDelegation, 13))
        ]);

    public static readonly Claim NativeChildCannotDelegateTask = new(
        "A native child lives only inside its dispatcher's turn and cannot call delegate_task (parent_not_active); when this arises the work is dispatched by someone who can.",
        [A.ChiefOfStaff, A.TicketCaptain, A.AnyAgent],
        [
            new(S.Admiral, "A native child lives only inside its dispatcher's turn and cannot `delegate_task` (`parent_not_active`): when a captain reports it, dispatch its crew for it.", new(F.AdmiralBoundary, 10)),
            new(S.ChiefOfStaff, "A native child lives only inside its dispatcher's turn and cannot `delegate_task` (`parent_not_active`); on it, ask the user to dispatch it.", new(F.ChiefOfStaffBoundary, 7)),
            new(S.TicketCaptain, "A native child lives only inside its dispatcher's turn and cannot `delegate_task`: on `parent_not_active`, ask your officer (the Admiral, or the user when none is invoked) to dispatch it.", new(F.TicketCaptainBoundary, 11)),
            new(S.WorkingTreeStandard, "A native child lives only inside its dispatcher's turn and cannot itself call `delegate_task`; T3 refuses with `parent_not_active` (\"Delegated tasks require an active run owned by this MCP provider session\").", new(F.WorkingTreeStandardDelegation, 12))
        ]);

    public static readonly Claim NativeForegroundOnlyWhenAwaited = new(
        "Use a native foreground subagent only for a job the dispatcher waits on within its current turn.",
        [A.TicketCaptain, A.AnyAgent, A.Admiral, A.ChiefOfStaff],
        [
            new(S.Admiral, "**Dispatch:** under T3, captains and anything else that returns to you go by `delegate_task` from your own turn, a child of you; a native foreground subagent only for a job you wait on within this turn, and no native background subagents.", new(F.AdmiralBoundary, 8), () => [DispatchCaptainToNo.NoNativeBackgroundSubagents!, DispatchOneToWithout.T3DelegateTaskForReturningChildren]),
            new(S.ChiefOfStaff, "**Dispatch:** under T3, anything that returns to you goes by `delegate_task`, a child of you; a native foreground subagent only for a job you wait on within this turn, and no native background subagents.", new(F.ChiefOfStaffBoundary, 4), () => [DispatchCaptainToNo.NoNativeBackgroundSubagents!, DispatchOneToWithout.T3DelegateTaskForReturningChildren]),
            new(S.TicketCaptain, "**Dispatch:** under T3, crew and reviewers go by `delegate_task`, a child of you; a native foreground subagent only for a job you wait on within this turn, and no native background subagents.", new(F.TicketCaptainBoundary, 8), () => [DispatchCaptainToNo.NoNativeBackgroundSubagents!, DispatchOneToWithout.T3DelegateTaskForReturningChildren]),
            new(S.WorkingTreeStandard, "A native foreground subagent only when the dispatcher waits for it within its own turn.", new(F.WorkingTreeStandardDelegation, 5))
        ]);

    public static readonly Claim NextNForFollowUpBreakAgent = new(
        "A hand-back or follow-up Break agent gets the next <n> for its proof branch.",
        [A.TicketCaptain], []);

    public static readonly Claim NightSurveyBriefFixed = new(
        "The fixed night-survey brief has: the absolute path to night-survey.md and its survey only, the Project IDs with admiral thread links, exact night orders per Project with update links, the previous observation, the outstanding nudges and the parent return address with return through the task result only.",
        [A.ChiefOfStaff],
        [
            new(S.ChiefOfStaff, "```text\nRead <absolute path to chief-of-staff/resources/night-survey.md>; perform only its survey.\nProjects: <Project IDs and admiral thread links>\nNight orders: <exact orders per Project and Project-update links>\nPrevious observation: <UTC time and optional comparison data, or none>\nOutstanding nudges: <evidence links and times, or none>\nParent return address: <Chief of Staff thread link>; return through this task's result only.\n```", new(F.ChiefOfStaffNightSurveyFixedDispatchChiefOfStaffFillsAndSends, 1))
        ]);

    public static readonly Claim NoNativeBackgroundSubagents = new(
        "Under T3, never use native background subagents.",
        [A.TicketCaptain, A.AnyAgent, A.Admiral, A.ChiefOfStaff],
        [
            new(S.WorkingTreeStandard, "No native background subagents.", new(F.WorkingTreeStandardDelegation, 8))
        ]);

    public static readonly Claim NoPollingDelegates = new(
        "Never poll running delegates; T3 background completion is event-driven, and assignment or AFK labels alone prove no activity.",
        [A.Admiral, A.ChiefOfStaff],
        [
            new(S.Admiral, "Never poll running delegates; assignment and AFK labels alone prove no activity.", new(F.AdmiralMethod, 3)),
            new(S.ChiefOfStaff, "Follow the dispatch lifecycle above: T3 background completion is event-driven, without polling; wait for native foreground work within this turn.", new(F.ChiefOfStaffOvernightDuties, 6))
        ]);

    public static readonly Claim NoTopLevelThreadsForCrew = new(
        "Never use t3_thread_launch or create_threads for crew (or AFK captains): top-level threads are the user's, only for HITL work the user takes over or when the user asks.",
        [A.TicketCaptain, A.Officer, A.AnyAgent],
        [
            new(S.Admiral, "Never `t3_thread_launch` or `create_threads` for AFK captains or crew: top-level threads are the user's, only for HITL work or when the user asks.", new(F.AdmiralBoundary, 13)),
            new(S.ChiefOfStaff, "Never `t3_thread_launch` or `create_threads` for crew: top-level threads are the user's, only for HITL work or when the user asks.", new(F.ChiefOfStaffBoundary, 8)),
            new(S.TicketCaptain, "Never `t3_thread_launch` or `create_threads` for crew: top-level threads are the user's, only for HITL work or when the user asks.", new(F.TicketCaptainBoundary, 18)),
            new(S.WorkingTreeStandard, "A new top-level T3 thread only for HITL work the user takes over, or when the user asks for one.", new(F.WorkingTreeStandardDelegation, 7))
        ]);
}
