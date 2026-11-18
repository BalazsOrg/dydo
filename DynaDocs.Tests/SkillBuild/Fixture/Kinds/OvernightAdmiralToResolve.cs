namespace DynaDocs.Tests.SkillBuild.Fixture;

using DynaDocs.SkillBuild;
using A = DynaDocs.Tests.SkillBuild.Fixture.FixtureActor;
using F = DynaDocs.Tests.SkillBuild.Fixture.FixtureSection;
using S = DynaDocs.Tests.SkillBuild.Fixture.FixtureSkill;

/// <summary>Migration fixture: the inventory's <c>overnight</c> claims, sub-topics admiral to resolve.</summary>
public static class OvernightAdmiralToResolve
{
    public static readonly Claim AdmiralConfirmsRunnable = new(
        "Activation proceeds only once every included admiral confirms its agreed work is runnable under the night orders, every gap has the user's explicit disposition, and the agreed work and stop line are confirmed.",
        [A.ChiefOfStaff],
        [
            new(S.ChiefOfStaff, "[ ] Obtain every included admiral's confirmation that its agreed work is runnable under those orders.", new(F.ChiefOfStaffMusterLead, 12)),
            new(S.OvernightMode, "Proceed once every included admiral confirms its agreed work is runnable and every gap has the user's explicit disposition.", new(F.OvernightModeMuster, 5))
        ]);

    public static readonly Claim AdmiralReportForm = new(
        "Each admiral's muster report has the form: Project and thread link; Intent as read (verbatim read-back); Work; Stop line; Your calls; Unblock now; Permissions.",
        [A.Admiral],
        [
            new(S.ChiefOfStaff, "```text\n<Project> — <admiral thread link>\nIntent, as the admiral reads it: <verbatim read-back>\nWork: <ordered tasks, first runnable item>\nStop line: <boundary and decisions reserved for you>\nYour calls: <question; context; options; recommendation>\nUnblock now: <obstacle; proposed fix; verification evidence>\nPermissions: <exact action; safe exercise; observed result or gap>\n```", new(F.ChiefOfStaffMusterPresentToTheUser, 2))
        ]);

    public static readonly Claim AdmiralsSendNoStatus = new(
        "Admirals send the chief of staff no status overnight, and the chief of staff never asks for routine STATE since board and threads carry state.",
        [A.Admiral, A.ChiefOfStaff],
        [
            new(S.OvernightMode, "Send the chief of staff no status.", new(F.OvernightModeStandingOrders, 5))
        ]);

    public static readonly Claim AlreadyMatched = new(
        "If the mode value already matched before execution, report it as already on/off rather than claiming a new transition.",
        [A.ChiefOfStaff],
        [
            new(S.OvernightMode, "If the value already matched before execution, report it as already on/off rather than claiming a new transition.", new(F.OvernightModeTransitionPreflightAtEitherTransition, 3))
        ]);

    public static readonly Claim AnnounceStart = new(
        "Announce the verified start to every included admiral.",
        [A.ChiefOfStaff],
        [
            new(S.OvernightMode, "Announce the verified start to every included admiral.", new(F.OvernightModeMuster, 12))
        ]);

    public static readonly Claim AnswerNudgeOneLine = new(
        "Answer a chief-of-staff nudge in one line.",
        [A.AnyAgent],
        [
            new(S.OvernightMode, "Answer a nudge in one line.", new(F.OvernightModeStandingOrders, 8))
        ]);

    public static readonly Claim CaptureSeparately = new(
        "Capture the action, the result and the fresh readback separately.",
        [A.ChiefOfStaff],
        [
            new(S.OvernightMode, "Capture the action, result and fresh readback separately.", new(F.OvernightModeTransitionPreflightAtEitherTransition, 2))
        ]);

    public static readonly Claim CompleteNoActionEndsTheTick = new(
        "A complete `NO_ACTION` ends the tick.",
        [A.ChiefOfStaff],
        [
            new(S.ChiefOfStaff, "Resolve `INCOMPLETE` inspection gaps; a complete `NO_ACTION` ends the tick.", new(F.ChiefOfStaffOvernightDuties, 9), () => [ProjectMapToWatch.SurveyIncomplete])
        ]);

    public static readonly Claim ConnSplit = new(
        "Between muster and morning brief each admiral keeps the conn of its Project and the chief of staff keeps only the night watch.",
        [A.Admiral, A.ChiefOfStaff],
        [
            new(S.OvernightMode, "In between, **each admiral keeps the conn** of its Project and the chief of staff keeps only the **night watch**.", new(HeaderPart.Role, 3))
        ]);

    public static readonly Claim CrossProjectContact = new(
        "Admirals contact the chief of staff for cross-Project trouble: collisions, shared-resource problems, and heads-ups or permission before actions that affect other agents.",
        [A.Admiral],
        [
            new(S.OvernightMode, "Contact it for cross-Project trouble: collisions, shared-resource problems, and heads-ups or permission before actions that affect other agents.", new(F.OvernightModeStandingOrders, 6))
        ]);

    public static readonly Claim DeleteWatchSchedule = new(
        "Delete the watch schedule using the complete preserved ID, then list schedules and confirm that exact ID is absent before claiming the watch stopped; report a deletion or verification gap.",
        [A.ChiefOfStaff],
        [
            new(S.OvernightMode, "Delete the watch schedule using the complete preserved ID.", new(F.OvernightModeMorningBrief, 2))
        ]);

    public static readonly Claim ExcludeOrDeferDependentWork = new(
        "Obtain explicit user agreement to exclude or defer dependent work and settle a runnable alternative; revise the work list.",
        [A.ChiefOfStaff],
        [
            new(S.ChiefOfStaff, "Obtain explicit user agreement to exclude or defer dependent work and settle a runnable alternative; revise the work list.", new(F.ChiefOfStaffMusterLead, 10))
        ]);

    public static readonly Claim ExercisePermissions = new(
        "Exercise needed permissions through safe, authorized operations where supported (admiral contact, each session's Linear writes, feature-branch merge capability) and record actual results.",
        [A.ChiefOfStaff],
        [
            new(S.ChiefOfStaff, "[ ] Exercise needed permissions through safe, authorized operations where supported: admiral contact, each session's Linear writes and feature-branch merge capability.", new(F.ChiefOfStaffMusterLead, 6))
        ]);

    public static readonly Claim FailedTransitionPreserveState = new(
        "On a failed transition, preserve the existing state and evidence.",
        [A.ChiefOfStaff],
        [
            new(S.OvernightMode, "Preserve the existing state and evidence.", new(F.OvernightModeTransitionPreflightAtEitherTransition, 4))
        ]);

    public static readonly Claim FailedTransitionReport = new(
        "On refusal, execution failure, missing readback or mismatched value, report the transition incomplete with the actual result and observed state (unknown if unreadable) and the exact recovery action.",
        [A.ChiefOfStaff],
        [
            new(S.ChiefOfStaff, "If blocked, report shutdown incomplete with the observed mode and exact human-terminal action; a stopped watch does not establish mode off.", new(F.ChiefOfStaffMorningBriefLead, 2)),
            new(S.OvernightMode, "On refusal, failure or missing readback, report activation incomplete with the observed state (unknown if unreadable) and exact recovery action; keep the user present and do not announce activation or say goodnight.", new(F.OvernightModeMuster, 8))
        ]);

    public static readonly Claim FailedTransitionResolveAndReturn = new(
        "Return the blocker to the responsible captain or user with the exact action needed, and resolve it before proceeding.",
        [A.ChiefOfStaff],
        [
            new(S.OvernightMode, "Return the blocker to the responsible captain or user with the exact action needed.", new(F.OvernightModeTransitionPreflightAtEitherTransition, 5))
        ]);

    public static readonly Claim FillMorningBrief = new(
        "After verified shutdown, fill the `chief-of-staff` morning-brief template in the conversation, writing nothing to the repo.",
        [A.ChiefOfStaff],
        [
            new(S.ChiefOfStaff, "**Return:** after `overnight-mode` shutdown, fill [the morning brief](resources/morning-brief.md).", new(F.ChiefOfStaffOvernightDuties, 10)),
            new(S.OvernightMode, "Fill `chief-of-staff`'s `resources/morning-brief.md` in the conversation, writing nothing to the repo.", new(F.OvernightModeMorningBrief, 4))
        ]);

    public static readonly Claim GuardWarningRetryOnce = new(
        "Follow the preflight's warning/retry for the mode setter: if the guard's reminder applies, retry the same command once.",
        [A.ChiefOfStaff, A.AnyAgent],
        [
            new(S.OvernightMode, "Follow the preflight's warning/retry for `dydo mode set overnight on`.", new(F.OvernightModeMuster, 6))
        ]);

    public static readonly Claim HandleIncomingMessages = new(
        "Handle incoming messages when received.",
        [A.ChiefOfStaff],
        [
            new(S.ChiefOfStaff, "Handle incoming messages when received.", new(F.ChiefOfStaffOvernightDuties, 8))
        ]);

    public static readonly Claim IntentReadbackToUser = new(
        "Show each intent read-back verbatim, separately from work, and obtain the user's corrections.",
        [A.ChiefOfStaff],
        [
            new(S.ChiefOfStaff, "[ ] Show each intent read-back verbatim, separately from work; obtain the user's corrections.", new(F.ChiefOfStaffMusterLead, 3))
        ]);

    public static readonly Claim KeepGaps = new(
        "Keep unsupported exercises and unresolved items as gaps.",
        [A.ChiefOfStaff],
        [
            new(S.ChiefOfStaff, "[ ] Keep unsupported exercises and unresolved items as gaps.", new(F.ChiefOfStaffMusterLead, 9))
        ]);

    public static readonly Claim LoadTriggers = new(
        "Overnight mode is loaded only on the chief of staff's muster or morning-brief announcement, or when the user calls it; between muster and morning brief the admiral loads it.",
        [A.Admiral, A.ChiefOfStaff, A.AnyAgent],
        [
            new(S.Admiral, "**Overnight mode:** starts only when the chief of staff announces it at muster and ends at its morning-brief announcement; in between, load `overnight-mode` and obey standing plus night orders.", new(F.AdmiralBoundary, 29), () => [OvernightAdmiralToResolve.ObeyStandingAndNightOrders!]),
            new(S.ChiefOfStaff, "When the user calls overnight mode, load `overnight-mode`.", new(HeaderPart.Role, 2)),
            new(S.OvernightMode, "Load only on the chief of staff's muster or morning-brief announcement, or when the user calls overnight mode.", new(HeaderPart.Role, 1))
        ]);

    public static readonly Claim MediateCrossProject = new(
        "The chief of staff mediates cross-Project trouble by Method step 4, Mediate a collision, relaying heads-ups down the chain.",
        [A.ChiefOfStaff],
        [
            new(S.OvernightMode, "Mediate cross-Project trouble by chief-of-staff Method step 4, Mediate a collision, relaying heads-ups down the chain.", new(F.OvernightModeNightWatch, 5))
        ]);

    public static readonly Claim MusterChecklistComplete = new(
        "The chief of staff completes the muster checklist (`resources/muster.md`) with the user before activation.",
        [A.ChiefOfStaff],
        [
            new(S.ChiefOfStaff, "The Chief of Staff completes this checklist with the user before activation.", new(F.ChiefOfStaffMusterLead, 1)),
            new(S.OvernightMode, "Complete `chief-of-staff`'s `resources/muster.md`.", new(F.OvernightModeMuster, 3))
        ]);

    public static readonly Claim MusterReadyThenActivate = new(
        "Muster is ready only when all checklist boxes are complete; then return to `overnight-mode` for activation.",
        [A.ChiefOfStaff],
        [
            new(S.ChiefOfStaff, "Ready means all boxes are complete.", new(F.ChiefOfStaffMusterLead, 13))
        ]);

    public static readonly Claim MusterTools = new(
        "Decisions at muster use `grilling`; terminal steps, including morning-brief unblockers and transition blockers, use `wizard`, and only for terminal steps.",
        [A.ChiefOfStaff],
        [
            new(S.OvernightMode, "Use `grilling` for decisions and `wizard` for terminal steps only.", new(F.OvernightModeMuster, 4))
        ]);

    public static readonly Claim NightOrdersExpireAndWin = new(
        "Night orders expire at the morning brief and win over standing orders where the two conflict.",
        [A.Admiral],
        [
            new(S.OvernightMode, "Night orders expire at the morning brief and win where the two conflict.", new(HeaderPart.Role, 6))
        ]);

    public static readonly Claim NightOrdersPostedInForm = new(
        "The chief of staff posts each Project's night orders in the template form: date, expiry at morning brief, override of conflicting standing orders; Intent (read-back with corrections); Work; Answered; Unblocked; Permissions; Deferred; Stop line.",
        [A.ChiefOfStaff],
        [
            new(S.ChiefOfStaff, "[ ] Post each Project's night orders using the template below.", new(F.ChiefOfStaffMusterLead, 11))
        ]);

    public static readonly Claim NoDummyMerge = new(
        "Never perform a dummy merge.",
        [A.ChiefOfStaff],
        [
            new(S.ChiefOfStaff, "Never perform a dummy merge.", new(F.ChiefOfStaffMusterLead, 8))
        ]);

    public static readonly Claim NudgeIdleAdmiral = new(
        "For an idle admiral, or one waiting with nothing in flight and runnable work, send one specific nudge naming the evidence.",
        [A.ChiefOfStaff],
        [
            new(S.OvernightMode, "Idle, or waiting with nothing in flight and runnable work: send one specific nudge naming the evidence.", new(F.OvernightModeNightWatch, 3))
        ]);

    public static readonly Claim ObeyStandingAndNightOrders = new(
        "Between muster and morning brief the admiral obeys standing plus night orders.",
        [A.Admiral], []);

    public static readonly Claim ObeyStandingPlusNightOrders = new(
        "Admirals obey the standing orders plus the night orders: one Project update per Project, set at muster.",
        [A.Admiral],
        [
            new(S.OvernightMode, "Admirals obey the **standing orders** below plus the **night orders**: one Project update per Project, set at muster.", new(HeaderPart.Role, 5))
        ]);

    public static readonly Claim OnlyReadbackProvesMode = new(
        "Intent, a watch schedule, message delivery, a warning, a retry, a successful setter exit or a deleted schedule establishes no mode transition; only the verified `dydo mode show` value does.",
        [A.AnyAgent, A.ChiefOfStaff],
        [
            new(S.OvernightMode, "Intent, a watch schedule and message delivery do not establish a mode transition.", new(HeaderPart.Role, 4))
        ]);

    public static readonly Claim PresentQuestions = new(
        "Present current and foreseeable questions with context, options and recommendations; record the user's answers and why.",
        [A.ChiefOfStaff],
        [
            new(S.ChiefOfStaff, "[ ] Present current and foreseeable questions with context, options and recommendations; record the user's answers and why.", new(F.ChiefOfStaffMusterLead, 4))
        ]);

    public static readonly Claim ProjectDecisionsAdmiral = new(
        "Project decisions are the admiral's overnight.",
        [A.Admiral],
        [
            new(S.OvernightMode, "Project decisions are the admiral's.", new(F.OvernightModeStandingOrders, 1))
        ]);

    public static readonly Claim ReadbackInSeparateCall = new(
        "Wait for the mode setter to return its result, then run `dydo mode show` in a separate later tool call, never in parallel, and require the requested value.",
        [A.ChiefOfStaff],
        [
            new(S.ChiefOfStaff, "Complete `overnight-mode` shutdown first: wait for `dydo mode set overnight off` to return its result, then run `dydo mode show` in a separate later tool call, never in parallel, and require `overnight: off`.", new(F.ChiefOfStaffMorningBriefLead, 1)),
            new(S.OvernightMode, "Wait for the setter to return its result, then run `dydo mode show` in a separate later tool call, never in parallel, and require `overnight: on`.", new(F.OvernightModeMuster, 7))
        ]);

    public static readonly Claim RebuildAfterWatchGap = new(
        "After any gap in the watch, rebuild from Linear and sweep everyone by Method step 5, Sweep the board.",
        [A.ChiefOfStaff],
        [
            new(S.OvernightMode, "After any gap in the watch, rebuild from Linear and sweep everyone by Method step 5, Sweep the board.", new(F.OvernightModeNightWatch, 6))
        ]);

    public static readonly Claim RecoveryNeedsFreshReadback = new(
        "Every recovery still needs a fresh readback.",
        [A.ChiefOfStaff],
        [
            new(S.OvernightMode, "Resolve the blocker before proceeding; every recovery still needs fresh readback.", new(F.OvernightModeTransitionPreflightAtEitherTransition, 6))
        ]);

    public static readonly Claim RelayDownTheChain = new(
        "The chief of staff relays down the chain admiral to captain to crew.",
        [A.ChiefOfStaff],
        [
            new(S.OvernightMode, "It relays down admiral → captain → crew.", new(F.OvernightModeStandingOrders, 7))
        ]);

    public static readonly Claim ReportBlockPerProject = new(
        "Copy the admiral report block once per Project.",
        [A.ChiefOfStaff],
        [
            new(S.ChiefOfStaff, "Copy once per Project; settle the calls before proceeding.", new(F.ChiefOfStaffMusterPresentToTheUser, 1), () => [OvernightSettleToWatch.SettleCallsBeforeProceeding])
        ]);

    public static readonly Claim RequestAdmiralReports = new(
        "Request each admiral's report using the muster request block, asking for Intent, Work, Questions, Blockers and Permissions.",
        [A.ChiefOfStaff],
        [
            new(S.ChiefOfStaff, "[ ] Request each admiral's report using the block below.", new(F.ChiefOfStaffMusterLead, 2))
        ]);

    public static readonly Claim ResolveBlockersPresent = new(
        "Resolve operational blockers while the user is present and verify each fix with evidence.",
        [A.ChiefOfStaff],
        [
            new(S.ChiefOfStaff, "[ ] Resolve operational blockers while the user is present; verify each fix with evidence.", new(F.ChiefOfStaffMusterLead, 5))
        ]);
}
