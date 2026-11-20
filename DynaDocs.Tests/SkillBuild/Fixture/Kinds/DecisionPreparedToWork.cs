namespace DynaDocs.Tests.SkillBuild.Fixture;

using DynaDocs.SkillBuild;
using A = DynaDocs.Tests.SkillBuild.Fixture.FixtureActor;
using F = DynaDocs.Tests.SkillBuild.Fixture.FixtureSection;
using S = DynaDocs.Tests.SkillBuild.Fixture.FixtureSkill;

/// <summary>Migration fixture: the inventory's <c>decision</c> claims, sub-topics prepared to work.</summary>
public static class DecisionPreparedToWork
{
    public static readonly Claim PreparedQuestion = new(
        "When a call is prepared or not decided, post no DECIDED; the Question's body carries the prediction, any dissent, the stop reason and the log link, and it is wired as a blocker of every waiter.",
        [A.Officer],
        [
            new(S.WhatWouldBalazsDo, "**Prepared or not decided:** no `DECIDED` post.", new(F.WhatWouldBalazsDoRecord, 4))
        ]);

    public static readonly Claim ProofOnlyExperiment = new(
        "An experiment goes to a proof-only code-writer in an isolated worktree with effects contained; tribunal members otherwise only read.",
        [A.Officer],
        [
            new(S.WhatWouldBalazsDo, "An experiment goes to a proof-only `code-writer` in an isolated worktree, its effects contained; its brief names the commissioning captain, the pinned source SHA and the test path.", new(F.WhatWouldBalazsDoTribunalPhases, 18), () => [DispatchOneToWithout.ProofOnlyBriefNames])
        ]);

    public static readonly Claim RaisingOfficerIsMemberAndClerk = new(
        "The raising officer is a full tribunal member and also the clerk.",
        [A.Officer],
        [
            new(S.WhatWouldBalazsDo, "The raising officer is a full member and also the clerk.", new(F.WhatWouldBalazsDoTribunalLead, 2))
        ]);

    public static readonly Claim RatifyOrOverturn = new(
        "The user answers a tribunal-decided Question with RATIFIED or OVERTURNED; the officer posts that block quoting his words verbatim and moves the Question to Done.",
        [A.Officer, A.User, A.AnyAgent],
        [
            new(S.WhatWouldBalazsDo, "**His answer:** post `RATIFIED` or `OVERTURNED` in the standard's form, quoting him, and move the Question to `Done`.", new(F.WhatWouldBalazsDoRecord, 5))
        ]);

    public static readonly Claim ReadBeforeDeciding = new(
        "Before deciding, read the Question Tickets and Communication and evidence sections of the Linear workspace standard and dydo/project/decisions/_decisions.md from the repository root.",
        [A.Officer],
        [
            new(S.WhatWouldBalazsDo, "From the repository root, read the Question Tickets and Communication and evidence sections of `dydo/reference/linear-workspace-standard.md`.", new(F.WhatWouldBalazsDoMustReads, 1))
        ]);

    public static readonly Claim RecordedDecisionAgainstHisSpiritIsNamed = new(
        "A recorded decision that goes against his spirit or no longer fits the project loses to it: the position names the decision and why, and the call becomes DR-grade, or at least flagged.",
        [A.Officer],
        [
            new(S.WhatWouldBalazsDo, "A recorded decision that goes against that spirit or no longer fits the project loses to it: the position names the decision and why, and the call becomes DR-grade, or at least flagged.", new(F.WhatWouldBalazsDoCaseLaw, 6))
        ]);

    public static readonly Claim Reframe = new(
        "A reframe states exactly what it changes, and the judges may object.",
        [A.Officer],
        [
            new(S.WhatWouldBalazsDo, "A reframe states exactly what it changes, and the judges may object.", new(F.WhatWouldBalazsDoTribunalPhases, 13))
        ]);

    public static readonly Claim ReversibleCallByOfficer = new(
        "An easily reversible call is decided by the officer without the skill, present or away; overnight it is recorded DECIDED.",
        [A.Admiral, A.Officer],
        [
            new(S.OvernightMode, "Easily reversible: decide and record `DECIDED`.", new(F.OvernightModeStandingOrders, 2)),
            new(S.WhatWouldBalazsDo, "| Easily reversible | you decide; no skill | you decide; no skill |", new(F.WhatWouldBalazsDoWhen, 3))
        ]);

    public static readonly Claim RoundBundle = new(
        "Each round, send every member the same bundle (the sealed positions and the last round's turns), always queued, never steered.",
        [A.Officer],
        [
            new(S.WhatWouldBalazsDo, "Send every member the same bundle: the sealed positions and the last round's turns, always queued, never steered.", new(F.WhatWouldBalazsDoTribunalPhases, 8))
        ]);

    public static readonly Claim RoundStop = new(
        "Stop the rounds when a round changes nothing and no claim is pending, and after at most 4 rounds, evidence rounds and reframes included.",
        [A.Officer],
        [
            new(S.WhatWouldBalazsDo, "Stop when a round changes nothing and no claim is pending, and after 4 rounds at most, evidence rounds and reframes included.", new(F.WhatWouldBalazsDoTribunalPhases, 15))
        ]);

    public static readonly Claim SealedPositionForm = new(
        "A sealed position has the form: the member's own class, the call in its own words, an option the file did not list or why none exists, the premises checked and findings, its answer with any recorded decision it overrides, and what would change its mind.",
        [A.Officer],
        [
            new(S.WhatWouldBalazsDo, "the member's own class for the call the call, in its own words an option the file did not list, or why none exists the premises it checked, and what it found its answer: the action he would take and why, naming any recorded decision it overrides what would change its mind", new(F.WhatWouldBalazsDoTribunalSealedPosition, 1))
        ]);

    public static readonly Claim SearchCurrentProjectForPrecedent = new(
        "Search the current project for precedent unless the user points it at another project's records.",
        [A.Officer],
        [
            new(S.WhatWouldBalazsDo, "Search the current project unless he points it at another project's records.", new(F.WhatWouldBalazsDoCaseLaw, 4))
        ]);

    public static readonly Claim StalledMemberOneRetry = new(
        "A missing or stalled tribunal member gets one retry.",
        [A.Officer],
        [
            new(S.WhatWouldBalazsDo, "A missing or stalled member gets one retry. After that the tribunal is incomplete and ends without a decision; never swap a vendor silently.", new(F.WhatWouldBalazsDoTribunalPhases, 16))
        ]);

    public static readonly Claim StrictestClassStands = new(
        "No class is lowered over an unresolved objection: the strictest class stands.",
        [A.Officer],
        [
            new(S.WhatWouldBalazsDo, "No class is lowered over an unresolved objection: the strictest class stands.", new(F.WhatWouldBalazsDoTribunalPhases, 14))
        ]);

    public static readonly Claim TribunalCallIsQuestionTicket = new(
        "Every call the tribunal takes is a Question ticket in Todo with the tribunal log attached as a Linear Document; a call decided without the skill needs no record here.",
        [A.Officer],
        [
            new(S.WhatWouldBalazsDo, "Every call the tribunal takes is a `Question` ticket in `Todo`, his turn, with the tribunal log attached as a Linear Document.", new(F.WhatWouldBalazsDoRecord, 1))
        ]);

    public static readonly Claim TribunalLog = new(
        "Keep the tribunal log as a Linear Document \"Tribunal log - <KEY>\" attached to the Question, with the sections Case file, Dispatch, Sealed positions, Round n, Claims, Ballot, Verdict; append each finished phase and round, checkpointing Dispatch, so a fresh clerk can resume.",
        [A.Officer],
        [
            new(S.WhatWouldBalazsDo, "Append each finished phase to the log before the next.", new(F.WhatWouldBalazsDoTribunalPhases, 2))
        ]);

    public static readonly Claim TribunalOutputNeverPrecedentUntilRatified = new(
        "A tribunal's output is never precedent until the user ratifies it.",
        [A.Officer],
        [
            new(S.WhatWouldBalazsDo, "A tribunal's output is never precedent until he ratifies it.", new(F.WhatWouldBalazsDoCaseLaw, 3))
        ]);

    public static readonly Claim TribunalPositionStandsOnlyWhileItSurvivesChallenge = new(
        "The tribunal has three members, each position standing only while it survives the strongest challenge.",
        [A.Officer],
        [
            new(S.WhatWouldBalazsDo, "Three members, Popper-style: each position stands only while it survives the strongest challenge.", new(F.WhatWouldBalazsDoTribunalLead, 1))
        ]);

    public static readonly Claim TribunalProcedureFile = new(
        "Run the tribunal by resources/tribunal.md; judges load the skill and follow the same file.",
        [A.Officer, A.AnyAgent],
        [
            new(S.WhatWouldBalazsDo, "Run it by [tribunal](resources/tribunal.md); judges load this skill and follow the same file.", new(F.WhatWouldBalazsDoTribunal, 1))
        ]);

    public static readonly Claim TurnContent = new(
        "Each turn raises the strongest material objection to another member's position, or says why none exists, and answers the strongest outstanding challenge to its own.",
        [A.Officer],
        [
            new(S.WhatWouldBalazsDo, "Each turn raises the strongest material objection to another member's position, or says why none exists, and answers the strongest outstanding challenge to its own.", new(F.WhatWouldBalazsDoTribunalPhases, 10))
        ]);

    public static readonly Claim UseWwbdTrigger = new(
        "A call above the deciding agent's say goes through what-would-balazs-do when it is DR-grade or irreversible, or when the user is away (or asks for it on any call), and the result sets the Question's wiring.",
        [A.TicketCaptain, A.Admiral, A.Officer],
        [
            new(S.Admiral, "A call above your say goes through `what-would-balazs-do` when it is DR-grade or irreversible, or when the user is away; it sets the Question's wiring.", new(F.AdmiralMethod, 49)),
            new(S.TicketCaptain, "A call above your say goes through `what-would-balazs-do` when it is DR-grade or irreversible, or when the user is away; it sets the Question's wiring.", new(F.TicketCaptainRelease, 1)),
            new(S.WhatWouldBalazsDo, "Use when an officer must decide one while he is away, prepare a DR-grade or irreversible one for him, or when he asks for it on any call.", new(SpinePart.Job, 1))
        ]);

    public static readonly Claim VoteNeverWidensRemit = new(
        "A vote never widens an officer's remit.",
        [A.Officer],
        [
            new(S.WhatWouldBalazsDo, "A vote never widens an officer's remit.", new(F.WhatWouldBalazsDoAuthority, 4))
        ]);

    public static readonly Claim WhoRunsTheSkill = new(
        "Only officers run the skill, each inside its own remit (a captain on calls inside its ticket contract, the map holder on scope, destination, architecture and cross-ticket calls).",
        [A.Officer],
        [
            new(S.WhatWouldBalazsDo, "Officers only, each inside its own remit: a captain on calls inside its ticket contract, the map holder on scope, destination, architecture and cross-ticket calls.", new(F.WhatWouldBalazsDoAuthority, 2))
        ]);

    public static readonly Claim WorkResumesAfterDecision = new(
        "After the tribunal decides, work resumes, and any review the work requires still runs.",
        [A.Officer, A.AnyAgent], []);
}
