namespace DynaDocs.Tests.SkillBuild.Fixture;

using DynaDocs.SkillBuild;
using A = DynaDocs.Tests.SkillBuild.Fixture.FixtureActor;
using F = DynaDocs.Tests.SkillBuild.Fixture.FixtureSection;
using S = DynaDocs.Tests.SkillBuild.Fixture.FixtureSkill;

/// <summary>Migration fixture: the inventory's <c>project</c> claims, sub-topics admiral to ledger.</summary>
public static class ProjectAdmiralToLedger
{
    public static readonly Claim AdmiralConfirmsLandingCleanup = new(
        "The admiral confirms the landing artifact cleanup.",
        [A.Admiral],
        [
            new(S.Admiral, "**Done:** an empty walkthrough closes the Project `Completed`; commission spec retirement under `dydo/reference/feature-spec-convention.md`, and confirm landing artifact cleanup.", new(F.AdmiralMethod, 71), () => [ProjectAdmiralToLedger.EmptyLandingWalkthroughCompletes!, ProjectMapToWatch.MapHolderCommissionsSpecRetirement])
        ]);

    public static readonly Claim AdmiralRereadsBoardAfterUserTells = new(
        "After the user tells the admiral, the admiral reads the board again.",
        [A.Admiral],
        [
            new(S.Admiral, "After the user answers a Question, confirms an Inquisition, finishes HITL work or clicks a landing, he tells you; read the board again.", new(F.AdmiralReturn, 5), () => [ProjectMapToWatch.UserTellsAdmiral])
        ]);

    public static readonly Claim AdmiralWakeEvents = new(
        "An invoked admiral wakes on a captain return, the user wake or its scheduled check-in, and rereads the board (including released and blocker-cleared tickets) or runs the check-in resource.",
        [A.Admiral],
        [
            new(S.Admiral, "On each user wake, check-in tick or `done <key>: merged`, run [the check-in](resources/check-in.md); on any other captain return, carry out the authorized continuation or fallback through its captain, or record the specific hold, and update the ledger.", new(F.AdmiralMethod, 1), () => [ProjectAdmiralToLedger.CaptainReturnContinuation!]),
            new(S.WorkingTreeStandard, "An invoked admiral wakes on a captain's return, the user's word or its scheduled check-in, and rereads the board, including released and blocker-cleared tickets.", new(F.WorkingTreeStandardReturnAndRelease, 9))
        ]);

    public static readonly Claim AuthorityPersists = new(
        "A stage or turn ending does not revoke existing authority.",
        [A.Admiral],
        [
            new(S.Admiral, "A stage or turn ending does not revoke existing authority.", new(F.AdmiralMethod, 32))
        ]);

    public static readonly Claim CanceledRecordsWhy = new(
        "Project status Canceled means the destination was consciously abandoned; the map holder records why.",
        [A.MapHolder],
        [
            new(S.LinearWorkspaceStandard, "| `Canceled` | The destination was consciously abandoned; record why and commission [spec retirement](./feature-spec-convention.md#retirement). |", new(F.LinearWorkspaceStandardProjectStatuses, 6))
        ]);

    public static readonly Claim CaptainReturnContinuation = new(
        "On any other captain return, the admiral carries out the authorized continuation or fallback through its captain, or records the specific hold, and updates the ledger.",
        [A.Admiral], []);

    public static readonly Claim ChartAndApproveDone = new(
        "Chart and approve is done when approval is recorded, any plan is `reviewed`, and the Project is `Planned`.",
        [A.Admiral],
        [
            new(S.Admiral, "**Done:** approval is recorded, any plan is `reviewed`, and the Project `Planned`.", new(F.AdmiralMethod, 20))
        ]);

    public static readonly Claim ChartUnknownRoute = new(
        "When the route is unknown, the admiral sets the Project `Planning` and charts it with `wayfinder`: map in the Project description naming the admiral its holder, the first pickable tickets, and each Project-level Question in `Todo` wired to every waiter.",
        [A.Admiral],
        [
            new(S.Admiral, "When the route is unknown, set the Project `Planning` and chart it with `wayfinder`: the map in the Project description, naming you its holder, the first pickable tickets, and each Project-level Question in `Todo`, wired to every waiter.", new(F.AdmiralMethod, 8))
        ]);

    public static readonly Claim CheckinDelete = new(
        "When the admiral waits on the user alone, the user takes the map back, or the Project closes, delete the check-in schedule, list schedules and confirm the ID is gone.",
        [A.Admiral],
        [
            new(S.Admiral, "[ ] You wait on the user's word alone, he takes the map back, or the Project closes: delete the schedule, then list schedules and confirm the ID is gone.", new(F.AdmiralCheckInSchedule, 3))
        ]);

    public static readonly Claim CheckinIdRecorded = new(
        "The check-in schedule's full ID is recorded on the Project.",
        [A.Admiral],
        [
            new(S.Admiral, "[ ] Project goes `In Progress`, or you take over an active one: schedule the check-in every 20 minutes, bound to this thread (T3 `schedule_task`, interval), with the prompt `Admiral check-in for <Project>: read <absolute path to this file> and run its Tick section.` Record its full ID on the Project.", new(F.AdmiralCheckInSchedule, 1))
        ]);

    public static readonly Claim CheckinReschedule = new(
        "If the recorded check-in schedule is missing or disabled on a user wake, or new work becomes pickable after it was deleted, schedule the check-in again.",
        [A.Admiral],
        [
            new(S.Admiral, "[ ] On each user wake: list schedules; the recorded ID missing or disabled, schedule it again.", new(F.AdmiralCheckInSchedule, 2))
        ]);

    public static readonly Claim CheckinScheduleStart = new(
        "When the Project goes `In Progress` or the admiral takes over an active one, the admiral starts the check-in schedule (every 20 minutes, bound to this thread, with the prescribed prompt) before other opening work.",
        [A.Admiral],
        [
            new(S.Admiral, "Set the Project `In Progress` and start the check-in schedule.", new(F.AdmiralMethod, 23))
        ]);

    public static readonly Claim DeliveryTicketOneMilestone = new(
        "Every delivery ticket belongs to one Milestone.",
        [A.Admiral, A.MapHolder],
        [
            new(S.Admiral, "Every delivery ticket belongs to one.", new(F.AdmiralMethod, 14)),
            new(S.LinearWorkspaceStandard, "Every delivery ticket belongs to one, and each Milestone has one Walkthrough Ticket.", new(F.LinearWorkspaceStandardWhoHoldsWhat, 9))
        ]);

    public static readonly Claim DeliveryToMapHolder = new(
        "Delivery belongs to the Project's map holder (an invoked admiral, or the user when none is invoked): stage what the Project needs on the Project and tell the user, whose word wakes an admiral only when one is invoked.",
        [A.ChiefOfStaff],
        [
            new(S.ChiefOfStaff, "Delivery belongs to the Project's map holder - an invoked admiral, or the user doing it directly when none is invoked: stage what its Project needs on the Project, and tell the user, whose word wakes an admiral only when one is invoked.", new(F.ChiefOfStaffMethod, 16))
        ]);

    public static readonly Claim DispatchBeforeYield = new(
        "Before yielding, each pickable item is dispatched or has a specific blocker, owner and clearance route.",
        [A.Admiral],
        [
            new(S.Admiral, "**Done before yielding:** each pickable item is dispatched or has a specific blocker, owner and clearance route.", new(F.AdmiralMethod, 31))
        ]);

    public static readonly Claim EmptyLandingWalkthroughCompletes = new(
        "An empty landing walkthrough lets the Project close Completed.",
        [A.Admiral, A.MapHolder],
        [
            new(S.LinearWorkspaceStandard, "| `Completed` | The destination landed and the landing's walkthrough found nothing more; commission [spec retirement](./feature-spec-convention.md#retirement). |", new(F.LinearWorkspaceStandardProjectStatuses, 5), () => [ProjectMapToWatch.MapHolderCommissionsSpecRetirement]),
            new(S.Walkthrough, "An empty landing walkthrough lets the Project close; the map holder commissions spec retirement following `dydo/reference/feature-spec-convention.md`, read from the repository root.", new(SpinePart.Job, 19), () => [ProjectMapToWatch.MapHolderCommissionsSpecRetirement])
        ]);

    public static readonly Claim FirstCaptainOpensFeatureBranch = new(
        "When a Project is opened, the map holder commissions the first Ticket Captain to open `feature/<project-slug>` from the approved main SHA and commit the Project's working spec on it before the ticket branch or claim.",
        [A.TicketCaptain, A.Admiral, A.MapHolder],
        [
            new(S.Admiral, "Commission the first Ticket Captain to open `feature/<project-slug>` from the approved main SHA before claiming its ticket; put the map in the Project description and each contract's base branch and blockers on its ticket.", new(F.AdmiralMethod, 21), () => [ProjectMapToWatch.MapAndContractPlacement]),
            new(S.TicketCaptain, "When the Project's map holder commissions Project setup, open the feature branch from its named approved main SHA, commit the spec draft its brief names, unchanged, as that branch's first commit and push it, before creating the ticket branch.", new(F.TicketCaptainMethod, 2)),
            new(S.WorkingTreeStandard, "| Open the Project | the map holder, commissioning the first Ticket Captain | The first Captain opens the feature branch from the approved main SHA, commits the Project's working spec on it when the map holder hands one over, and reports both; the Project map is in Linear; every ticket carries outcome, owned paths, blockers, exact gates and base branch. Only then is a ticket pickable. |", new(F.WorkingTreeStandardWhoDoesWhat, 1), () => [ProjectAdmiralToLedger.FirstCaptainReportsBranchAndSpec!, ProjectMapToWatch.TicketPickableOnlyWithMapAndContractFields])
        ]);

    public static readonly Claim FirstCaptainReportsBranchAndSpec = new(
        "The first Ticket Captain reports the opened feature branch and the committed spec.",
        [A.TicketCaptain], []);

    public static readonly Claim FirstTicketBlocksFirstLanes = new(
        "Before any commission, the first ticket is wired as a native Linear blocker of every other first lane, so survey and check-in Tick see those lanes blocked until it is `Done`.",
        [A.Admiral],
        [
            new(S.Admiral, "Before any commission, wire the first ticket as a native Linear blocker of every other first lane: `dydo survey` and the check-in Tick then see those lanes blocked until it is `Done`.", new(F.AdmiralScaffoldProjectLead, 6))
        ]);

    public static readonly Claim FlagStalledLane = new(
        "Flag a lane as stalled when its owner or crew ended with work left and no return delivered, or nothing happened since the ledger's time while the owner neither runs, waits on its crew, nor waits on its recorded prerequisite.",
        [A.Crew],
        [
            new(S.Admiral, "[ ] Flag a lane as stalled when its owner or crew ended with work left and no return delivered, or when nothing happened since the ledger's time while the owner neither runs, waits on its crew, nor waits on its recorded prerequisite.", new(F.AdmiralCheckInSurveyTheSurveyor, 7))
        ]);

    public static readonly Claim FlagUnnamedNeedsUser = new(
        "Flag each `needs the user` lane that names no Question or recorded gate (approval, landing click, walkthrough).",
        [A.Crew],
        [
            new(S.Admiral, "[ ] Flag each `needs the user` lane that names no Question or recorded gate (approval, landing click, walkthrough).", new(F.AdmiralCheckInSurveyTheSurveyor, 8))
        ]);

    public static readonly Claim FollowScaffoldRecipe = new(
        "When creating a Project, or first charting one without a working spec, the admiral follows the scaffold recipe, which commits the Project spec.",
        [A.Admiral, A.MapHolder],
        [
            new(S.Admiral, "Scaffolding a new Project: follow [the recipe](resources/scaffold-project.md).", new(F.AdmiralMethod, 9)),
            new(S.Wayfinder, "In a Project an admiral scaffolds, its scaffold recipe commits the Project's spec; this step covers feature specs inside it.", new(F.WayfinderChartTheMap, 5))
        ]);

    public static readonly Claim FourLanes = new(
        "The admiral strives to keep four lanes occupied: core tickets first, `Filler` when the next core ticket is blocked, and more or fewer lanes when the work calls for it.",
        [A.Admiral],
        [
            new(S.Admiral, "[ ] Strive to keep four lanes occupied: core tickets first, `Filler` when the next core ticket is blocked, and more or fewer lanes when the work calls for it.", new(F.AdmiralCheckInTickTheAdmiral, 5))
        ]);

    public static readonly Claim HumanHoldsRemain = new(
        "Explicit human holds remain while independent authorized work proceeds.",
        [A.Admiral],
        [
            new(S.Admiral, "Explicit human holds remain; independent authorized work proceeds.", new(F.AdmiralMethod, 6))
        ]);

    public static readonly Claim LandingBlockersAndShouldsQuery = new(
        "At landing no open `blocker` or `should` ticket remains in the Project unless the user explicitly deferred it; the check is a query of the Project's open tickets by Severity label.",
        [A.Admiral, A.MapHolder],
        [
            new(S.Admiral, "Query the Project for open tickets labelled Severity `blocker` or `should`; only those the user explicitly deferred may remain open at landing.", new(F.AdmiralMethod, 63)),
            new(S.LinearWorkspaceStandard, "The landing check is a query: no open `blocker` or `should` ticket in the Project, unless the user explicitly deferred it.", new(F.LinearWorkspaceStandardSeverity, 7))
        ]);

    public static readonly Claim LandingCleanup = new(
        "On the map holder's landing-cleanup commission, the landing Captain deletes the merged feature branch after the user lands it and reports completion.",
        [A.Admiral, A.MapHolder, A.TicketCaptain],
        [
            new(S.Admiral, "When the user tells you it landed, resume the landing captain to close and clean up the merged feature branch (`dydo clean --dry-run <worktree>`, then `dydo clean <worktree>`, run from outside that tree); open the last Milestone's Walkthrough Ticket, load `walkthrough` yourself, and facilitate it here.", new(F.AdmiralMethod, 68), () => [Walkthrough.AdmiralFacilitatesLast]),
            new(S.TicketCaptain, "On the map holder's landing-cleanup commission, remove the merged feature branch locally and report completion.", new(F.TicketCaptainMethod, 68)),
            new(S.WorkingTreeStandard, "| Merged feature branch | the map holder, commissioning the landing Captain | The landing Captain deletes the branch after the user lands it and reports completion. |", new(F.WorkingTreeStandardCleanup, 3))
        ]);

    public static readonly Claim LandingOnlyAfterCleanInquisitionRound = new(
        "The Project lands only after an inquisition round that found no blocker and no should.",
        [A.Admiral],
        [
            new(S.Admiral, "Sequence the should-tickets; the Project lands only when each is `Done` or closed with a reason, and only after an inquisition round that found no blocker and no should.", new(F.AdmiralMethod, 64), () => [ProjectAdmiralToLedger.LandingRequiresShouldTicketsClosed!, ProjectMapToWatch.SequenceShouldTickets])
        ]);

    public static readonly Claim LandingRequiresShouldTicketsClosed = new(
        "The Project lands only when each should-ticket is `Done` or closed with a reason.",
        [A.Admiral], []);

    public static readonly Claim LedgerLaneStates = new(
        "A lane state is one of `moving`, `waiting on <prerequisite>, cleared by <who>`, `blocker-resolution running: <who>`, `admiral owes <decision or dispatch>`, `needs the user: <Question or gate>`, `deferred by the user`, `done`.",
        [A.Admiral],
        [
            new(S.Admiral, "State is one of: `moving`, `waiting on <prerequisite>, cleared by <who>`, `blocker-resolution running: <who>`, `admiral owes <decision or dispatch>`, `needs the user: <Question or gate>`, `deferred by the user`, `done`.", new(F.AdmiralCheckInLedger, 3))
        ]);

    public static readonly Claim LedgerRowForms = new(
        "The ledger has one table row per lane then the `Filler` row; a lane row holds name, issue keys in order, captain thread link, state and who does what or the event awaited; the Filler row holds issue keys next first, no thread, state `ready`, taken when a lane slot is free.",
        [A.Admiral],
        [
            new(S.Admiral, "One table row per lane, then the `Filler` row:", new(F.AdmiralCheckInLedger, 2))
        ]);

    public static readonly Claim LedgerSection = new(
        "A `## Lane ledger - <UTC time>` section in the Project description is rewritten in place whenever it changes, holds only the current state, and its time is that change.",
        [A.Admiral],
        [
            new(S.Admiral, "The `## Lane ledger — <UTC time>` section of the Project description, rewritten in place whenever it changes; it holds only the current state, and its time is that change.", new(F.AdmiralCheckInLedger, 1))
        ]);
}
