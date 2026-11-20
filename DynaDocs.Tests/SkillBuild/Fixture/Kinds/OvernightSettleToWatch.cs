namespace DynaDocs.Tests.SkillBuild.Fixture;

using DynaDocs.SkillBuild;
using A = DynaDocs.Tests.SkillBuild.Fixture.FixtureActor;
using F = DynaDocs.Tests.SkillBuild.Fixture.FixtureSection;
using S = DynaDocs.Tests.SkillBuild.Fixture.FixtureSkill;

/// <summary>Migration fixture: the inventory's <c>overnight</c> claims, sub-topics settle to watch.</summary>
public static class OvernightSettleToWatch
{
    public static readonly Claim SettleCallsBeforeProceeding = new(
        "Settle the calls before proceeding.",
        [A.ChiefOfStaff], []);

    public static readonly Claim SettlePendingSurvey = new(
        "Settle any pending survey through its host lifecycle so no tick remains in flight.",
        [A.ChiefOfStaff],
        [
            new(S.OvernightMode, "Settle any pending survey through its host lifecycle so no tick remains in flight.", new(F.OvernightModeMorningBrief, 3))
        ]);

    public static readonly Claim ShutdownDone = new(
        "Shutdown is done when the schedule is confirmed absent, pending surveys are settled, mode is off, admirals have the announcement and the user has the brief.",
        [A.ChiefOfStaff],
        [
            new(S.OvernightMode, "Done when the schedule is confirmed absent, pending surveys are settled, mode is off, admirals have the announcement and the user has the brief.", new(F.OvernightModeMorningBrief, 5))
        ]);

    public static readonly Claim ShutdownInCosConversation = new(
        "When the user says he's back, shutdown runs in the chief of staff's conversation.",
        [A.ChiefOfStaff],
        [
            new(S.OvernightMode, "When the user says he's back, in the chief of staff's conversation:", new(F.OvernightModeMorningBrief, 1))
        ]);

    public static readonly Claim SkeletonWalkthroughBlocksNothing = new(
        "In overnight mode the walking skeleton's Walkthrough Ticket blocks nothing; post `DECIDED` on it, carry on, and treat a wrong direction as a revert.",
        [A.Admiral, A.MapHolder],
        [
            new(S.Admiral, "In overnight mode the skeleton doesn't wait either: its Walkthrough Ticket blocks nothing; post `DECIDED` on it, carry on, and treat a wrong direction as a revert.", new(F.AdmiralMethod, 42)),
            new(S.LinearWorkspaceStandard, "In overnight mode it blocks nothing, and later Milestones' Walkthrough Tickets never block.", new(F.LinearWorkspaceStandardWhoHoldsWhat, 11), () => [Walkthrough.LaterTicketsNeverBlock]),
            new(S.OvernightMode, "The walking skeleton does not wait either: its Walkthrough Ticket blocks nothing; post `DECIDED` on it, carry on, and treat a wrong direction as a revert.", new(F.OvernightModeStandingOrders, 10))
        ]);

    public static readonly Claim SkillResourcesSelfContained = new(
        "The already-invoked chief of staff uses its own overnight resources, resolved inside the installed `chief-of-staff` skill folder, without reloading its skill or invoking another officer role.",
        [A.ChiefOfStaff],
        [
            new(S.OvernightMode, "The already-invoked chief of staff uses its own resources below.", new(F.OvernightModeMuster, 1))
        ]);

    public static readonly Claim StartupDone = new(
        "Startup is done when readiness is confirmed, the schedule ID is preserved, mode is on and every included admiral has the announcement.",
        [A.ChiefOfStaff],
        [
            new(S.OvernightMode, "Done when readiness is confirmed, the schedule ID is preserved, mode is on and every included admiral has the announcement.", new(F.OvernightModeMuster, 13))
        ]);

    public static readonly Claim StopOnlyAtStopLine = new(
        "One blocker never stops the night: take the next pickable work; stop only at the night orders' stop line or when nothing is pickable.",
        [A.Admiral],
        [
            new(S.OvernightMode, "One blocker never stops the night: take the next pickable work.", new(F.OvernightModeStandingOrders, 9))
        ]);

    public static readonly Claim SurveyCheckNudges = new(
        "Check the supplied outstanding nudge evidence and any response.",
        [A.Crew],
        [
            new(S.ChiefOfStaff, "Check the supplied outstanding nudge evidence and any response.", new(F.ChiefOfStaffNightSurveySurvey, 3))
        ]);

    public static readonly Claim SurveyIdentifyProblems = new(
        "Identify idle officers with runnable work, unanswered nudges, blockers needing intervention, conflicting work or departures from night orders.",
        [A.Crew],
        [
            new(S.ChiefOfStaff, "Identify idle officers with runnable work, unanswered nudges, blockers needing intervention, conflicting work or departures from night orders.", new(F.ChiefOfStaffNightSurveySurvey, 4))
        ]);

    public static readonly Claim SurveyInspectThreads = new(
        "Where progress or readiness is unclear, inspect the admiral's thread, following captain threads only as needed.",
        [A.Crew],
        [
            new(S.ChiefOfStaff, "Where progress or readiness is unclear, inspect the admiral's thread, following relevant captain threads only as needed.", new(F.ChiefOfStaffNightSurveySurvey, 2))
        ]);

    public static readonly Claim SurveyNoShadowBoard = new(
        "Previous observations are disposable comparison data; maintain no shadow board.",
        [A.Crew],
        [
            new(S.ChiefOfStaff, "Previous observations are disposable comparison data; maintain no shadow board.", new(F.ChiefOfStaffNightSurveySurvey, 8))
        ]);

    public static readonly Claim SurveyQuietNotStuck = new(
        "An unchanged board or quiet running task alone does not imply stuck work.",
        [A.Crew],
        [
            new(S.ChiefOfStaff, "An unchanged board or quiet running task alone does not imply stuck work.", new(F.ChiefOfStaffNightSurveySurvey, 5))
        ]);

    public static readonly Claim SurveyReadAndCompare = new(
        "Read the supplied night orders and every named Project live board, and compare with the previous observation when supplied.",
        [A.Crew],
        [
            new(S.ChiefOfStaff, "Read the supplied night orders and every named Project's live board.", new(F.ChiefOfStaffNightSurveySurvey, 1))
        ]);

    public static readonly Claim SurveyorInspectsCosActs = new(
        "The surveyor inspects; the chief of staff performs the nudge, escalation or intervention itself.",
        [A.ChiefOfStaff],
        [
            new(S.ChiefOfStaff, "Treat the result as inspection: perform the nudge, escalation or intervention yourself.", new(F.ChiefOfStaffOvernightDuties, 7)),
            new(S.OvernightMode, "The surveyor inspects; the chief of staff acts on its findings:", new(F.OvernightModeNightWatch, 2))
        ]);

    public static readonly Claim TransitionCommandOnlyByCos = new(
        "Run the transition command only as the chief of staff readying or ending overnight mode; otherwise stop.",
        [A.ChiefOfStaff],
        [
            new(S.OvernightMode, "Run the applicable command only as the chief of staff readying overnight mode or ending it; otherwise stop.", new(F.OvernightModeTransitionPreflightAtEitherTransition, 1))
        ]);

    public static readonly Claim TransitionCommands = new(
        "The transition commands are `dydo mode set overnight on` to start and `dydo mode set overnight off` to end.",
        [A.ChiefOfStaff],
        [
            new(S.OvernightMode, "```text\nStart: dydo mode set overnight on\nEnd:   dydo mode set overnight off\n```", new(F.OvernightModeTransitionPreflightLead, 1))
        ]);

    public static readonly Claim TransitionPreflight = new(
        "While the user is present, the chief of staff completes the transition preflight for startup and shutdown: read and record the current `dydo mode show` value and prepare the warning, same-command retry and separate later readback.",
        [A.ChiefOfStaff],
        [
            new(S.ChiefOfStaff, "**Muster:** start with `overnight-mode`'s transition preflight while the user is present, then complete [the muster checklist](resources/muster.md).", new(F.ChiefOfStaffOvernightDuties, 1)),
            new(S.OvernightMode, "While the user is present, complete the [transition preflight](resources/transition-preflight.md) for both startup and shutdown.", new(F.OvernightModeMuster, 2))
        ]);

    public static readonly Claim UnansweredNudgeIntervene = new(
        "When a nudge goes unanswered, inspect the relevant captain threads and intervene with the owner.",
        [A.ChiefOfStaff],
        [
            new(S.OvernightMode, "Nudge unanswered: inspect the relevant captain threads and intervene with the owner.", new(F.OvernightModeNightWatch, 4))
        ]);

    public static readonly Claim UnprotectedBranchNoMergeProof = new(
        "An unprotected branch proves no merge permission.",
        [A.ChiefOfStaff],
        [
            new(S.ChiefOfStaff, "An unprotected branch proves no merge permission.", new(F.ChiefOfStaffMusterLead, 7))
        ]);

    public static readonly Claim VerifiedReadbackBeforeAnnounce = new(
        "The chief of staff announces start or end only after the readback shows the requested mode (`overnight: on` at muster, `overnight: off` at the morning brief), and activation awaits that readback.",
        [A.ChiefOfStaff],
        [
            new(S.ChiefOfStaff, "That skill owns transition and readback; announce activation only after its completion criteria pass.", new(F.ChiefOfStaffOvernightDuties, 2)),
            new(S.OvernightMode, "At **muster**, the chief of staff verifies `overnight: on` with `dydo mode show` before announcing the start; at the **morning brief**, it verifies `overnight: off` before announcing the end.", new(SpinePart.Job, 2))
        ]);

    public static readonly Claim WatchScheduleCreated = new(
        "Create the 30-minute watch schedule, returning to this thread.",
        [A.ChiefOfStaff],
        [
            new(S.OvernightMode, "Create the 30-minute watch schedule, returning to this thread.", new(F.OvernightModeMuster, 9))
        ]);

    public static readonly Claim WatchScheduleFailure = new(
        "If scheduling fails, report mode on with watch unavailable and resolve it while the user is present.",
        [A.ChiefOfStaff],
        [
            new(S.OvernightMode, "If scheduling fails, report mode on with watch unavailable and resolve it while the user is present.", new(F.OvernightModeMuster, 11))
        ]);

    public static readonly Claim WatchScheduleIdPreserved = new(
        "Preserve the complete watch schedule ID exactly as the host returns it, including any prefix, for deletion at shutdown.",
        [A.ChiefOfStaff],
        [
            new(S.OvernightMode, "Preserve the complete schedule ID exactly as returned by the host, including any prefix, for deletion at shutdown.", new(F.OvernightModeMuster, 10))
        ]);

    public static readonly Claim WatchTickSurvey = new(
        "The chief of staff runs the watch step every 30 minutes, using one fresh surveyor per scheduled tick with the fixed night-survey brief (`resources/night-survey.md`).",
        [A.ChiefOfStaff],
        [
            new(S.ChiefOfStaff, "**Watch:** use one fresh surveyor per scheduled tick with [the fixed survey brief](resources/night-survey.md).", new(F.ChiefOfStaffOvernightDuties, 3)),
            new(S.OvernightMode, "The chief of staff runs its Overnight duties watch step every 30 minutes, using its `resources/night-survey.md`.", new(F.OvernightModeNightWatch, 1))
        ]);
}
