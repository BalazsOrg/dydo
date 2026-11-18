namespace DynaDocs.Tests.SkillBuild.Fixture;

using DynaDocs.SkillBuild;
using A = DynaDocs.Tests.SkillBuild.Fixture.FixtureActor;
using F = DynaDocs.Tests.SkillBuild.Fixture.FixtureSection;
using S = DynaDocs.Tests.SkillBuild.Fixture.FixtureSkill;

/// <summary>Migration fixture: the inventory's <c>decision</c> claims, sub-topics away to precedent.</summary>
public static class DecisionAwayToPrecedent
{
    public static readonly Claim AwayDefinition = new(
        "Away means dydo mode show reports overnight on; off or unreadable, the user is present.",
        [A.Officer],
        [
            new(S.WhatWouldBalazsDo, "Away means `dydo mode show` reports `overnight` `on`; off or unreadable, he is present.", new(F.WhatWouldBalazsDoWhen, 2))
        ]);

    public static readonly Claim BallotNamesOneExactAction = new(
        "Each tribunal member's ballot names one exact, written action, picked verbatim from the ballot, which lists every option still standing as one exact action quoted from the final positions, and the ballot is tallied by the verdict table.",
        [A.Officer],
        [
            new(S.WhatWouldBalazsDo, "Each member's ballot names one exact, written action.", new(F.WhatWouldBalazsDoVerdict, 1))
        ]);

    public static readonly Claim BelowDrAwayTribunal = new(
        "A call above the officer's say but below DR-grade, while the user is away, is decided by the tribunal; where his intent is unclear, the most reversible option, flagged.",
        [A.Officer],
        [
            new(S.WhatWouldBalazsDo, "| Above your say, below DR-grade | a plain Question; no skill | the tribunal decides; where his intent is unclear, the most reversible option, flagged |", new(F.WhatWouldBalazsDoWhen, 4), () => [DecisionAwayToPrecedent.BelowDrPresentPlainQuestion!])
        ]);

    public static readonly Claim BelowDrPresentPlainQuestion = new(
        "A call above the officer's say but below DR-grade, while the user is present, is a plain Question without the skill.",
        [A.Officer], []);

    public static readonly Claim BelowDrVoteMajority = new(
        "For a call below DR-grade a decision needs two members naming the same text; with three different actions, the most reversible option, flagged.",
        [A.Officer],
        [
            new(S.WhatWouldBalazsDo, "| below DR-grade | two naming the same text | three different actions: the most reversible option, flagged |", new(F.WhatWouldBalazsDoVerdict, 3))
        ]);

    public static readonly Claim CaseFile = new(
        "Research the case law (sending research sparingly) and write the case file in the given form; the phase is done when it holds every option, unranked.",
        [A.Officer],
        [
            new(S.WhatWouldBalazsDo, "Research the case law, sending `research` sparingly where a source is out of reach.", new(F.WhatWouldBalazsDoTribunalPhases, 3))
        ]);

    public static readonly Claim ChiefOfStaffNeverRunsTheSkill = new(
        "The chief of staff never runs the skill.",
        [A.ChiefOfStaff],
        [
            new(S.WhatWouldBalazsDo, "Crew returns its call to its captain; the chief of staff never runs it.", new(F.WhatWouldBalazsDoAuthority, 3), () => [DecisionAwayToPrecedent.CrewReturnsCallToCaptain!])
        ]);

    public static readonly Claim CitationQuotesWords = new(
        "A citation quotes the user's words with the reason behind them.",
        [A.Officer],
        [
            new(S.WhatWouldBalazsDo, "A citation quotes his words with the reason behind them.", new(F.WhatWouldBalazsDoCaseLaw, 7))
        ]);

    public static readonly Claim ClerkIsMechanical = new(
        "The clerk is mechanical: it forwards text verbatim, tracks turns and claims, and tallies ballots; its view enters only through its own turns.",
        [A.Officer],
        [
            new(S.WhatWouldBalazsDo, "The clerk is mechanical: it forwards text verbatim, tracks turns and claims, and tallies ballots; its view enters only through its own turns.", new(F.WhatWouldBalazsDoTribunalLead, 3))
        ]);

    public static readonly Claim ConcessionQuotes = new(
        "A concession quotes the evidence or restates the argument it yields to.",
        [A.Officer],
        [
            new(S.WhatWouldBalazsDo, "A concession quotes the evidence or restates the argument it yields to.", new(F.WhatWouldBalazsDoTribunalPhases, 11))
        ]);

    public static readonly Claim CosAnswerNeededGrilling = new(
        "On request, the chief of staff takes the Answer needed list one ticket at a time with grilling, pressing until both the answer and its reasoning are sharp and recording both on the ticket, which closes Done; grilling is done when every question is answered on its ticket or parked there in the user's words.",
        [A.ChiefOfStaff],
        [
            new(S.ChiefOfStaff, "On request, take the *Answer needed* list one ticket at a time and reach for `grilling`: press until both the answer and the reasoning behind it are sharp, then record both on the ticket, which closes `Done` on the answer.", new(F.ChiefOfStaffMethod, 10))
        ]);

    public static readonly Claim CosMediation = new(
        "When two workstreams contradict, the chief of staff establishes the facts, names the trade-off and proposes the smallest resolution that frees both; mediation is done when the resolution is on the ticket, or the one call above its authority is a Question on the Answer needed list with a recommendation.",
        [A.ChiefOfStaff],
        [
            new(S.ChiefOfStaff, "When two workstreams contradict each other, establish the facts, name the trade-off, and propose the smallest resolution that frees both.", new(F.ChiefOfStaffMethod, 11))
        ]);

    public static readonly Claim CrewReturnsCallToCaptain = new(
        "Crew returns its call to its captain.",
        [A.Crew], []);

    public static readonly Claim DecideAsBalazsWould = new(
        "Make the call the way the user would, or prepare it with the answer he would give, tested by a tribunal of three against his own words; a judge answers as he would, in the spirit of his words, citing why he decided.",
        [A.Officer, A.AnyAgent],
        [
            new(S.WhatWouldBalazsDo, "Make the call the way he would, or prepare it with the answer he would give.", new(HeaderPart.Role, 2))
        ]);

    public static readonly Claim DecideInHisSpirit = new(
        "Case law is evidence of intent, not a rulebook: read why he decided (DRs, core docs, project purpose, common sense) and decide in that spirit.",
        [A.Officer],
        [
            new(S.WhatWouldBalazsDo, "Case law is evidence of his intent, not a rulebook.", new(F.WhatWouldBalazsDoCaseLaw, 5))
        ]);

    public static readonly Claim DecidedBlock = new(
        "When the tribunal decides, post the DECIDED block (by what-would-balazs-do tribunal, unratified; result, vote, log, undo, next).",
        [A.Officer, A.AnyAgent],
        [
            new(S.LinearWorkspaceStandard, "`DECIDED - by: what-would-balazs-do tribunal, unratified; result: <action>; vote: <n of 3, or split; most reversible>; log: <link>; undo: <steps>; next: <action>.`", new(F.LinearWorkspaceStandardCommunicationAndEvidence, 18)),
            new(S.WhatWouldBalazsDo, "**Decided:** post `DECIDED - by: what-would-balazs-do tribunal, unratified; result: <action>; vote: <n of 3, or split; most reversible>; log: <link>; undo: <steps>; next: <action>.` Leave the Question unwired; work resumes, and any review the work requires still runs.", new(F.WhatWouldBalazsDoRecord, 2), () => [DecisionAwayToPrecedent.DecidedQuestionStaysInTodoUnwired!, DecisionPreparedToWork.WorkResumesAfterDecision])
        ]);

    public static readonly Claim DecidedQuestionStaysInTodoUnwired = new(
        "A Question a what-would-balazs-do tribunal decided, while the user was away or on his own invocation, stays in Todo unwired.",
        [A.AnyAgent, A.Officer],
        [
            new(S.LinearWorkspaceStandard, "A Question a `what-would-balazs-do` tribunal decided while the user was away, or on his own invocation, stays in `Todo` unwired, and work proceeds; he answers it with `RATIFIED` or `OVERTURNED`.", new(F.LinearWorkspaceStandardQuestionTickets, 5), () => [DecisionPreparedToWork.RatifyOrOverturn, DecisionPreparedToWork.WorkResumesAfterDecision])
        ]);

    public static readonly Claim DispatchDone = new(
        "The dispatch phase is done when both judges return sealed positions and their results show the model and effort asked for.",
        [A.Officer],
        [
            new(S.WhatWouldBalazsDo, "Done when both return sealed positions and their results show the model and effort asked for.", new(F.WhatWouldBalazsDoTribunalPhases, 7))
        ]);

    public static readonly Claim DrGradeAwayUnanimousDecides = new(
        "A DR-grade call while the user is away is decided by the tribunal if unanimous, which writes the DR; that verdict is the user's approval.",
        [A.Admiral, A.Officer],
        [
            new(S.Admiral, "Where a repository plan exists, commit dated `## Amendment - <YYYY-MM-DD>` sections to it; give every new implementation ticket one Type, one Mode, and `Todo`; re-review changes to destination, scope, acceptance criteria, or governing architecture and obtain human approval before affected work resumes; a unanimous away `what-would-balazs-do` verdict is that approval, and only an irreversible change waits for his word.", new(F.AdmiralMethod, 45), () => [DecisionAwayToPrecedent.IrreversibleCallIsUsers!, LinearAdmiralToOpen.CaptainTicketCarriesTypeAndMode, ProjectMapToWatch.RepositoryPlanAmendments, ReviewReadToWrong.RereviewOnScopeChange]),
            new(S.WhatWouldBalazsDo, "| DR-grade | the tribunal prepares the Question | the tribunal decides if unanimous, and writes the DR; that verdict is his approval |", new(F.WhatWouldBalazsDoWhen, 5), () => [DecisionAwayToPrecedent.DrGradePresentPreparedQuestion!])
        ]);

    public static readonly Claim DrGradePresentPreparedQuestion = new(
        "A DR-grade call while the user is present, and any call while he is present without invoking the skill, ends as a tribunal-prepared Question carrying the predicted answer and any dissent.",
        [A.Officer], []);

    public static readonly Claim DrGradeVoteUnanimous = new(
        "For a DR-grade call a decision needs all three members naming the same text; otherwise no decision and a prepared Question.",
        [A.Officer],
        [
            new(S.WhatWouldBalazsDo, "| DR-grade | all three naming the same text | no decision: a prepared Question |", new(F.WhatWouldBalazsDoVerdict, 2))
        ]);

    public static readonly Claim DrGraduationCriteria = new(
        "An answer graduates to a Decision Record only when it is hard to reverse, surprising later and the result of a real trade-off.",
        [A.MapHolder],
        [
            new(S.Wayfinder, "An answer graduates to a Decision Record only when it is hard to reverse, surprising later, and the result of a real trade-off.", new(F.WayfinderDiscoveryBeforeQuestion, 4))
        ]);

    public static readonly Claim DrLifecycle = new(
        "A DR-grade decision writes the repo DR with status proposed and ratification pending, linking the log; ratified it becomes accepted, overturned it becomes deprecated or superseded and the affected work reopens, and either way ratification is dropped.",
        [A.Officer],
        [
            new(S.WhatWouldBalazsDo, "A DR-grade decision also writes the repo DR `status: proposed`, `ratification: pending`, linking the log.", new(F.WhatWouldBalazsDoRecord, 3))
        ]);

    public static readonly Claim FactualClaimCheck = new(
        "A factual claim names its check.",
        [A.Officer],
        [
            new(S.WhatWouldBalazsDo, "A factual claim names its check; the other two members run it independently and report confirmed, refuted or inconclusive.", new(F.WhatWouldBalazsDoTribunalPhases, 12), () => [DecisionAwayToPrecedent.OtherMembersRunFactualCheckIndependently!])
        ]);

    public static readonly Claim IncompleteTribunalNoDecision = new(
        "An incomplete tribunal decides nothing: the call becomes a prepared Question.",
        [A.Officer],
        [
            new(S.WhatWouldBalazsDo, "An incomplete tribunal decides nothing; the call becomes a prepared Question.", new(F.WhatWouldBalazsDoVerdict, 4))
        ]);

    public static readonly Claim InvokedFollowsAwayColumn = new(
        "The user may invoke the skill on any call; when invoked it follows the away column: reversible calls are decided by the Verdict rules, and only an irreversible call or one the tribunal cannot decide waits for his word.",
        [A.User, A.Officer],
        [
            new(S.WhatWouldBalazsDo, "He may invoke this skill on any call.", new(F.WhatWouldBalazsDoWhen, 7))
        ]);

    public static readonly Claim IrreversibleCallIsUsers = new(
        "An irreversible call is the user's alone, present or away, and the tribunal prepares the Question; overnight irreversible calls are always a Question.",
        [A.Admiral, A.Officer],
        [
            new(S.OvernightMode, "Other calls: `what-would-balazs-do`, whose trigger map decides it or prepares the Question; irreversible calls are always a Question.", new(F.OvernightModeStandingOrders, 3), () => [DecisionPreparedToWork.UseWwbdTrigger]),
            new(S.WhatWouldBalazsDo, "| Irreversible | his alone; the tribunal prepares the Question | the same |", new(F.WhatWouldBalazsDoWhen, 6))
        ]);

    public static readonly Claim JudgeAnswersEachQueuedRoundWithOneTurn = new(
        "Later rounds arrive to a judge as queued messages; it answers each with one turn.",
        [A.AnyAgent],
        [
            new(S.WhatWouldBalazsDo, "Later rounds arrive as queued messages; answer each with one turn.", new(F.WhatWouldBalazsDoTribunalJudgeBrief, 4))
        ]);

    public static readonly Claim JudgeBrief = new(
        "Send each judge the judge brief with the case file; judges get the brief and nothing else from the raising officer's session.",
        [A.Officer],
        [
            new(S.WhatWouldBalazsDo, "Send each the judge brief with the case file.", new(F.WhatWouldBalazsDoTribunalPhases, 6))
        ]);

    public static readonly Claim JudgeBriefTellsItIsAJudge = new(
        "A judge brief tells the judge it is a judge on a what-would-balazs-do tribunal.",
        [A.AnyAgent],
        [
            new(S.WhatWouldBalazsDo, "You are a judge on a `what-would-balazs-do` tribunal.", new(F.WhatWouldBalazsDoTribunalJudgeBrief, 1))
        ]);

    public static readonly Claim JudgeGoesStraightToTribunalSection = new(
        "A judge goes straight to the Tribunal section.",
        [A.AnyAgent],
        [
            new(S.WhatWouldBalazsDo, "A judge goes straight to the Tribunal section.", new(F.WhatWouldBalazsDoAuthority, 1))
        ]);

    public static readonly Claim JudgeMaySendResearchSubagents = new(
        "A judge may send research subagents after evidence.",
        [A.AnyAgent],
        [
            new(S.WhatWouldBalazsDo, "You may send `research` subagents after evidence.", new(F.WhatWouldBalazsDoTribunalJudgeBrief, 2))
        ]);

    public static readonly Claim JudgeNeverDispatchesJudges = new(
        "A judge never dispatches judges.",
        [A.AnyAgent],
        [
            new(S.WhatWouldBalazsDo, "Never dispatch judges.", new(F.WhatWouldBalazsDoTribunalJudgeBrief, 6))
        ]);

    public static readonly Claim JudgeReturnsSealedPositionByDeadline = new(
        "A judge returns its sealed position in the skill's form by the brief's deadline.",
        [A.AnyAgent],
        [
            new(S.WhatWouldBalazsDo, "Return your sealed position in the skill's form by <deadline>.", new(F.WhatWouldBalazsDoTribunalJudgeBrief, 3))
        ]);

    public static readonly Claim JudgeUsesSealedFormAndMemberPhases = new(
        "A judge uses the Sealed position form and phases 4-7 as a member; phases 1-3, the clerk duties and writing the ballot list are the clerk's.",
        [A.AnyAgent],
        [
            new(S.WhatWouldBalazsDo, "As a judge, use the Sealed position form and phases 4-7 as a member; phases 1-3, the clerk duties and writing the ballot list are the clerk's.", new(F.WhatWouldBalazsDoTribunalJudgeBrief, 5))
        ]);

    public static readonly Claim LaterWordOutranks = new(
        "A later word of the user outranks an earlier one.",
        [A.Officer],
        [
            new(S.WhatWouldBalazsDo, "A later word outranks an earlier one.", new(F.WhatWouldBalazsDoCaseLaw, 2))
        ]);

    public static readonly Claim NeverSwapVendorSilently = new(
        "Never swap a tribunal vendor silently.",
        [A.Officer],
        [
            new(S.WhatWouldBalazsDo, "After that the tribunal is incomplete and ends without a decision; never swap a vendor silently.", new(F.WhatWouldBalazsDoTribunalPhases, 17))
        ]);

    public static readonly Claim OtherMembersRunFactualCheckIndependently = new(
        "The other two members run a named check independently and report confirmed, refuted or inconclusive.",
        [A.Officer], []);

    public static readonly Claim OwnSealedPositionFirst = new(
        "Write and log your own sealed position before any judge starts.",
        [A.Officer],
        [
            new(S.WhatWouldBalazsDo, "Write and log your own sealed position before any judge starts.", new(F.WhatWouldBalazsDoTribunalPhases, 4))
        ]);

    public static readonly Claim OwnTurnBeforeReading = new(
        "Write your own turn before reading the others'.",
        [A.Officer],
        [
            new(S.WhatWouldBalazsDo, "Write your own turn before reading theirs.", new(F.WhatWouldBalazsDoTribunalPhases, 9))
        ]);

    public static readonly Claim PhaseDeadlines = new(
        "Set every tribunal phase's deadline when opening the case file.",
        [A.Officer],
        [
            new(S.WhatWouldBalazsDo, "Set every phase's deadline when you open the case file.", new(F.WhatWouldBalazsDoTribunalPhases, 1))
        ]);

    public static readonly Claim PrecedentIsHisWords = new(
        "Only the user's own words are precedent (Decision Records, docs, answered Questions and grillings, Walkthrough verdicts, findings and skips, past conversations).",
        [A.Officer],
        [
            new(S.WhatWouldBalazsDo, "Only his own words are his precedent: Decision Records (superseded ones as history), the docs, his answered Questions and grillings, and past conversations where available.", new(F.WhatWouldBalazsDoCaseLaw, 1))
        ]);
}
