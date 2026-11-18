namespace DynaDocs.Tests.SkillBuild.Fixture;

using DynaDocs.SkillBuild;
using A = DynaDocs.Tests.SkillBuild.Fixture.FixtureActor;
using F = DynaDocs.Tests.SkillBuild.Fixture.FixtureSection;
using S = DynaDocs.Tests.SkillBuild.Fixture.FixtureSkill;

/// <summary>Migration fixture: the inventory's <c>project</c> claims, sub-topics map to watch.</summary>
public static class ProjectMapToWatch
{
    public static readonly Claim MapAndContractPlacement = new(
        "The map goes in the Project description and each contract's base branch and blockers on its ticket.",
        [A.Admiral], []);

    public static readonly Claim MapComplete = new(
        "The map is complete at the destination, with no open Tasks or unresolved fog.",
        [A.MapHolder],
        [
            new(S.Wayfinder, "The map is complete at the destination, with no open Tasks or unresolved fog.", new(F.WayfinderChartAsYouGo, 3))
        ]);

    public static readonly Claim MapDescriptionForm = new(
        "At Project scale the Linear Project description is the whole map at low resolution: it indexes, each Wayfinding Ticket stores its own context and resolution, in the form Map holder line, Destination, Notes, Resolutions so far, Not yet specified, Out of scope.",
        [A.MapHolder],
        [
            new(S.Wayfinder, "At Project scale, the Linear Project description is the whole map at low resolution.", new(F.WayfinderTheMap, 1))
        ]);

    public static readonly Claim MapHolderCommissionsSpecRetirement = new(
        "The map holder commissions spec retirement, by the feature-spec convention's Retirement section, when a Project completes or is canceled.",
        [A.MapHolder, A.Admiral], []);

    public static readonly Claim MilestoneShape = new(
        "The map's Milestones are placed in Linear while charting: few, each a meaningful chunk where a drift from the user's intent would show, the walking skeleton first and the landing last.",
        [A.Admiral, A.MapHolder],
        [
            new(S.Admiral, "Place the map's Milestones in Linear.", new(F.AdmiralMethod, 12)),
            new(S.LinearWorkspaceStandard, "A Project's map holder places its Milestones in Linear while charting: few, each a meaningful chunk of work where a drift from the user's intent would show, the walking skeleton first and the landing last.", new(F.LinearWorkspaceStandardWhoHoldsWhat, 8)),
            new(S.Wayfinder, "At Project scale, group the Tasks into the Project's Milestones in Linear: few and meaningful, the walking skeleton first, the landing last.", new(F.WayfinderChartTheMap, 10))
        ]);

    public static readonly Claim NoInventedWork = new(
        "Dependency-blocked work waits for its named prerequisite; no work is invented to fill idle capacity.",
        [A.Admiral],
        [
            new(S.Admiral, "Dependency-blocked work waits for its named prerequisite; invent no work to fill idle capacity.", new(F.AdmiralReturn, 4))
        ]);

    public static readonly Claim PlanIsProjectAndTickets = new(
        "The Project and its tickets are the plan.",
        [A.Admiral],
        [
            new(S.Admiral, "The Project and its tickets are the plan.", new(F.AdmiralMethod, 11))
        ]);

    public static readonly Claim PrepareDestination = new(
        "Prepare each destination so the user can act without rebuilding the context.",
        [A.ChiefOfStaff],
        [
            new(S.ChiefOfStaff, "Prepare each destination so he can act without rebuilding the context.", new(F.ChiefOfStaffMethod, 2))
        ]);

    public static readonly Claim ReadStateAtArrival = new(
        "The admiral reads the Project at whatever stage it reaches the admiral, then acts from that state.",
        [A.Admiral],
        [
            new(S.Admiral, "Read the Project at whatever stage it reaches you.", new(HeaderPart.Role, 1))
        ]);

    public static readonly Claim ReconcileDone = new(
        "Reconcile is done when the ledger matches the latest evidence and every Merge whose turn came is known.",
        [A.Admiral],
        [
            new(S.Admiral, "**Done:** the ledger matches the latest evidence, and every Merge whose turn came is known.", new(F.AdmiralMethod, 7))
        ]);

    public static readonly Claim RelayRuleToAllLanes = new(
        "When relaying a new user rule, send it to every lane owner in the ledger, not only those it seems to affect.",
        [A.Admiral],
        [
            new(S.Admiral, "[ ] You relay a new rule from the user: send it to every lane owner in the ledger, not only those you think it affects.", new(F.AdmiralCheckInTickTheAdmiral, 2))
        ]);

    public static readonly Claim RepositoryPlanAmendments = new(
        "Where a repository plan exists, amendments are committed as dated `## Amendment - <YYYY-MM-DD>` sections.",
        [A.Admiral], []);

    public static readonly Claim RepositoryPlanOnlyIfCrossCutting = new(
        "Only a cross-cutting architecture plan earns a repository plan: written at `dydo/project/plans/<kebab-case>.md` in the `project-plan` rubric shape, committed, and sent to a fresh `reviewer(project-plan)` with rubric, Contract at the plan SHA, Candidate SHA and Base SHA.",
        [A.Admiral],
        [
            new(S.Admiral, "Only a cross-cutting architecture plan earns a repository plan: write it at `dydo/project/plans/<kebab-case>.md` in the shape the `project-plan` rubric checks, commit it, and send it to a fresh `reviewer(project-plan)` with rubric, Contract at the plan SHA, Candidate SHA and Base SHA.", new(F.AdmiralMethod, 15))
        ]);

    public static readonly Claim RouteDone = new(
        "Route is done when everything the chief of staff did not close has a named owner.",
        [A.ChiefOfStaff],
        [
            new(S.ChiefOfStaff, "Done when everything you did not close has a named owner.", new(F.ChiefOfStaffMethod, 18))
        ]);

    public static readonly Claim RoutePutToUser = new(
        "The charted route, with any PASS, is put to the user in the admiral session.",
        [A.Admiral],
        [
            new(S.Admiral, "Put the charted route, with any PASS, to the user in this session.", new(F.AdmiralMethod, 19))
        ]);

    public static readonly Claim RunSurvey = new(
        "Run `dydo survey <Project ID>`.",
        [A.Crew],
        [
            new(S.Admiral, "[ ] Run `dydo survey <Project ID>`.", new(F.AdmiralCheckInSurveyTheSurveyor, 4))
        ]);

    public static readonly Claim SequenceShouldTickets = new(
        "The admiral sequences the should-tickets.",
        [A.Admiral], []);

    public static readonly Claim SettleIntentThenCreate = new(
        "The admiral settles the intent with the user (co-think, grilling, `to-project`), then creates the Linear Project or takes the user's, naming the admiral its map holder.",
        [A.Admiral],
        [
            new(S.Admiral, "[ ] Settle the intent with the user (co-think, grilling, `to-project`), then create the Linear Project, or take the user's, naming you its map holder.", new(F.AdmiralScaffoldProjectLead, 1))
        ]);

    public static readonly Claim SortArrivals = new(
        "Sort arrivals: live work on the board, durable knowledge and evidence in Git and dydo, an unscheduled strategic possibility in `FutureFeature` until the user moves it on; sort is done when nothing waiting on the user is unclassified.",
        [A.ChiefOfStaff],
        [
            new(S.ChiefOfStaff, "Live work belongs on the board, durable knowledge and evidence in Git and dydo, and an unscheduled strategic possibility worth preserving stays in `FutureFeature` until the user moves it on.", new(F.ChiefOfStaffMethod, 1))
        ]);

    public static readonly Claim StalledFirstCaptain = new(
        "When the check-in finds a stalled or released first captain, resume or replace it from the draft.",
        [A.Admiral],
        [
            new(S.Admiral, "The check-in finds a stalled or released first captain; resume or replace it from the draft.", new(F.AdmiralScaffoldProjectLead, 7))
        ]);

    public static readonly Claim SurveyFindingToAction = new(
        "Each survey finding becomes its action (`pickable`/`unblocked` to `commission`; `uncontracted` to `contract`; any on a `Filler` ticket to `fill`; `unlaned`/`stale-row` to `fix ledger`; `started-unassigned` to `message` or `resume`; `question-no-priority` to `prioritize`) and the admiral carries out every returned action in order.",
        [A.Admiral, A.Crew], []);

    public static readonly Claim SurveyIncomplete = new(
        "On an `INCOMPLETE` survey, read what it could not or record the gap on the Project.",
        [A.Admiral, A.ChiefOfStaff],
        [
            new(S.Admiral, "[ ] `INCOMPLETE`: read what it could not, or record the gap on the Project.", new(F.AdmiralCheckInTickTheAdmiral, 6))
        ]);

    public static readonly Claim SurveyReadsLaneThreads = new(
        "For each ledger lane, read its owner's thread (`t3_thread_read`) and its ticket's record: last hop SHA, PR state, and crew dispatched and whether each returned.",
        [A.Crew],
        [
            new(S.Admiral, "[ ] For each ledger lane, read its owner's thread (`t3_thread_read`) and its ticket's record: last hop SHA, PR state, and the crew the captain dispatched and whether each returned.", new(F.AdmiralCheckInSurveyTheSurveyor, 5))
        ]);

    public static readonly Claim SweepBoard = new(
        "Sweep the board for stale states (working status with no crew running, `Question` without priority, broken blocking relations, missing evidence links, finished work shown active), fix mechanical drift, surface what needs judgement; done when the board reads true and every orphan is cleared or named with the reason it survives.",
        [A.ChiefOfStaff],
        [
            new(S.ChiefOfStaff, "Hunt stale states: a working status with no crew running, a `Question` without a priority, broken blocking relations, missing evidence links, and finished work still shown as active.", new(F.ChiefOfStaffMethod, 12))
        ]);

    public static readonly Claim TickEnd = new(
        "Rewrite the ledger if anything changed; otherwise end the tick in one line.",
        [A.Admiral],
        [
            new(S.Admiral, "[ ] Rewrite the ledger if anything changed; otherwise end the tick in one line.", new(F.AdmiralCheckInTickTheAdmiral, 7))
        ]);

    public static readonly Claim TickReturnsFirst = new(
        "At each tick the admiral handles every delivered return and receipt first, by Method step 1.",
        [A.Admiral],
        [
            new(S.Admiral, "[ ] Handle every delivered return and receipt first, by Method step 1.", new(F.AdmiralCheckInTickTheAdmiral, 1))
        ]);

    public static readonly Claim TicketPickableOnlyWithMapAndContractFields = new(
        "A ticket is pickable only once the Project map is in Linear and the ticket carries outcome, owned paths, blockers, exact gates and base branch.",
        [A.MapHolder, A.TicketCaptain], []);

    public static readonly Claim TriageUserAttention = new(
        "Triage everything that reaches the user, whose attention is the scarcest resource in the project.",
        [A.ChiefOfStaff],
        [
            new(S.ChiefOfStaff, "The user's attention is the scarcest resource in this project: triage everything that reaches him.", new(HeaderPart.Role, 1))
        ]);

    public static readonly Claim UserTellsAdmiral = new(
        "After the user answers a Question, confirms an Inquisition, finishes HITL work or clicks a landing, he tells the admiral.",
        [A.Admiral, A.User], []);

    public static readonly Claim WaitAfterReconcile = new(
        "Waiting follows reconciliation: wait for the user's word only when nothing is in flight and no authorized work is pickable.",
        [A.Admiral],
        [
            new(S.Admiral, "Waiting follows reconciliation: wait for the user's word only when nothing is in flight and no authorized work is pickable.", new(F.AdmiralReturn, 3))
        ]);

    public static readonly Claim WatchOpenPrs = new(
        "On every wake the admiral lists all open PRs (`gh pr list --limit 1000`) and watches each ticket PR not yet watched.",
        [A.Admiral],
        [
            new(S.Admiral, "On every wake, list all open PRs (`gh pr list --limit 1000`) and watch each ticket PR not yet watched.", new(F.AdmiralMethod, 2))
        ]);
}
