namespace DynaDocs.Tests.SkillBuild.Fixture;

using DynaDocs.SkillBuild;
using A = DynaDocs.Tests.SkillBuild.Fixture.FixtureActor;
using F = DynaDocs.Tests.SkillBuild.Fixture.FixtureSection;
using S = DynaDocs.Tests.SkillBuild.Fixture.FixtureSkill;

/// <summary>Migration fixture: the inventory's <c>return</c> claims, sub-topics admiral to no.</summary>
public static class ReturnAdmiralToNo
{
    public static readonly Claim AdmiralBoardIsReturn = new(
        "The board is the admiral's return: current map, contracts, blockers, reviews and human gates.",
        [A.Admiral],
        [
            new(S.Admiral, "The board is the return: current map, contracts, blockers, reviews and human gates.", new(F.AdmiralReturn, 1))
        ]);

    public static readonly Claim AdmiralOwedDecisionOwnDebt = new(
        "An owed admiral decision or dispatch is reported as the admiral's own debt, not a human wait.",
        [A.Admiral],
        [
            new(S.Admiral, "Report an owed admiral decision or dispatch as your debt, not a human wait.", new(F.AdmiralReturn, 2))
        ]);

    public static readonly Claim AttentionActionVerbs = new(
        "Surveyor action verbs are commission, contract, fill, resume, message, handle, rewire, prioritize, fix ledger, ask the user.",
        [A.Crew],
        [
            new(S.Admiral, "Verbs: `commission` a captain for a ticket, `contract` a Backlog ticket, `fill` a free slot, `resume` a captain from its hop SHA, `message` a captain's thread with the question, `handle` a captain's return you saw, `rewire`, `prioritize`, `fix ledger`, `ask the user`.", new(F.AdmiralCheckInSurveyTheSurveyor, 13))
        ]);

    public static readonly Claim AttentionListsFindingsWithEvidence = new(
        "ATTENTION lists each finding or action with its evidence link and UTC observation time, most urgent first.",
        [A.Crew],
        [
            new(S.Admiral, "`ATTENTION`: one line per action, most urgent first: `<verb> <who> on <what> — <evidence link, UTC time>`.", new(F.AdmiralCheckInSurveyTheSurveyor, 12)),
            new(S.ChiefOfStaff, "**ATTENTION** — for each finding: owner; observed problem; evidence link and observation time; recommended Chief of Staff action.", new(F.ChiefOfStaffNightSurveyReturn, 3))
        ]);

    public static readonly Claim BriefActionSlot = new(
        "Morning brief slot: the concrete action or decision, with context and why it needs the user.",
        [A.ChiefOfStaff],
        [
            new(S.ChiefOfStaff, "<concrete action or decision; context and why it needs you>", new(F.ChiefOfStaffMorningBriefYourHelpNeeded, 1))
        ]);

    public static readonly Claim BriefDecidedEntryForm = new(
        "Morning brief Decided entries read `<title> (<ticket>): <context>; decided <what>, because <why>`.",
        [A.ChiefOfStaff],
        [
            new(S.ChiefOfStaff, "**Decided:** <title> (<ticket>): <context>; decided <what>, because <why>.", new(F.ChiefOfStaffMorningBriefQuestionsDecidedFirst, 1))
        ]);

    public static readonly Claim BriefDecidedEntryRatifyPrompt = new(
        "Decided entries end with the prompt to ratify or reverse.",
        [A.ChiefOfStaff],
        [
            new(S.ChiefOfStaff, "Ratify or reverse.", new(F.ChiefOfStaffMorningBriefQuestionsDecidedFirst, 2))
        ]);

    public static readonly Claim BriefFinishedItemForm = new(
        "Morning brief line form for each finished item: `<title> (<PR link>): <result>.`",
        [A.ChiefOfStaff],
        [
            new(S.ChiefOfStaff, "<title> (<PR link>): <result>.", new(F.ChiefOfStaffMorningBriefPRsIntoMasterAwaitingYourClick, 1))
        ]);

    public static readonly Claim BriefNextActionSlot = new(
        "Morning brief slot: the next action and its owner.",
        [A.ChiefOfStaff],
        [
            new(S.ChiefOfStaff, "<next action and owner>", new(F.ChiefOfStaffMorningBriefNext, 1))
        ]);

    public static readonly Claim BriefOpenEntryForm = new(
        "Morning brief Open entries read `<title> (<ticket>): <context, options and consequences>`.",
        [A.ChiefOfStaff],
        [
            new(S.ChiefOfStaff, "**Open:** <title> (<ticket>): <context, options and consequences>.", new(F.ChiefOfStaffMorningBriefQuestionsDecidedFirst, 3))
        ]);

    public static readonly Claim BriefResultLines = new(
        "Morning brief slot: the result, at most three result lines total.",
        [A.ChiefOfStaff],
        [
            new(S.ChiefOfStaff, "<result; at most three result lines total>", new(F.ChiefOfStaffMorningBriefTheNight, 2))
        ]);

    public static readonly Claim BriefVerdictAndCi = new(
        "Morning brief states the review's current verdict and CI state on the current head for each item.",
        [A.ChiefOfStaff],
        [
            new(S.ChiefOfStaff, "Review <current verdict>; CI <state on current head>.", new(F.ChiefOfStaffMorningBriefPRsIntoMasterAwaitingYourClick, 2))
        ]);

    public static readonly Claim BriefVisual = new(
        "Morning brief slot: one real visual of the work touched.",
        [A.ChiefOfStaff],
        [
            new(S.ChiefOfStaff, "<one real visual of the work touched>", new(F.ChiefOfStaffMorningBriefTheNight, 1))
        ]);

    public static readonly Claim CaptainReturnsOneLineToSpawner = new(
        "A spawned captain returns one line to its spawner (done <key>: PR ready, done <key>: merged, released <key>: <reason>, or done <key> for a non-merging Type); everything else lives on the record.",
        [A.TicketCaptain], []);

    public static readonly Claim CosAnswerNeededList = new(
        "Answer needed lists the open Question tickets in Todo, ordered by priority, a released ticket's blocker among them.",
        [A.ChiefOfStaff],
        [
            new(S.ChiefOfStaff, "*Answer needed*: the open `Question` tickets in `Todo`, ordered by priority, a released ticket's blocker among them.", new(F.ChiefOfStaffMethod, 4))
        ]);

    public static readonly Claim CosApprovalNeededList = new(
        "Approval needed lists a charted route awaiting approval and an Inquisition filed in Backlog awaiting the move to Todo.",
        [A.ChiefOfStaff],
        [
            new(S.ChiefOfStaff, "*Approval needed*: a charted route awaiting approval, with its passing commit where a repository plan exists, waiting on the user's word in the map holder's session - the user's own when no admiral is invoked, an invoked admiral's otherwise - and an Inquisition filed in `Backlog`, waiting on the move to `Todo`.", new(F.ChiefOfStaffMethod, 5))
        ]);

    public static readonly Claim CosLandingList = new(
        "Landing lists the landing Merge Ticket, each atomic ticket in Ready to Merge awaiting the user's click, and each prepared Walkthrough Ticket.",
        [A.ChiefOfStaff],
        [
            new(S.ChiefOfStaff, "*Landing*: the landing Merge Ticket and each atomic ticket in `Ready to Merge`, waiting on the user's click, and each prepared Walkthrough Ticket, a Milestone's or the landing's, the walking skeleton's first since work waits on it.", new(F.ChiefOfStaffMethod, 6))
        ]);

    public static readonly Claim CosLeadWithMeaningRecommendOutcome = new(
        "Lead with meaning (key, SHA or filename paired with a plain-language title at first use) and recommend an outcome or answer for every item.",
        [A.ChiefOfStaff],
        [
            new(S.ChiefOfStaff, "Lead with meaning: keep a ticket key, SHA or filename for traceability, paired at first use with its title in plain language, and recommend an outcome for every item.", new(F.ChiefOfStaffMethod, 8))
        ]);

    public static readonly Claim CosReportDoneCondition = new(
        "The Chief of Staff report is done when each list is empty or one line the user can act on.",
        [A.ChiefOfStaff],
        [
            new(S.ChiefOfStaff, "Done when each list is empty or one line the user can act on.", new(F.ChiefOfStaffMethod, 9))
        ]);

    public static readonly Claim CosSayWhatWasFixedRouted = new(
        "After the lists, say what was fixed, what was routed and to whom, and what was left alone on purpose.",
        [A.ChiefOfStaff],
        [
            new(S.ChiefOfStaff, "Then say what you fixed, what you routed and to whom, and what you left alone on purpose.", new(F.ChiefOfStaffReturn, 1))
        ]);

    public static readonly Claim CosThreeListsOrder = new(
        "The Chief of Staff reports the three lists in order Answer needed, Approval needed, Landing, one line each.",
        [A.ChiefOfStaff],
        [
            new(S.ChiefOfStaff, "**Report the three lists, in this order.**", new(F.ChiefOfStaffMethod, 3))
        ]);

    public static readonly Claim CrewReturnGoesToTicketCaptain = new(
        "Every crew return comes back to the Ticket Captain.",
        [A.Crew], []);

    public static readonly Claim DecidedBlockForm = new(
        "A DECIDED block reads `DECIDED - result: <result>; immutable contract: <contract>; next: <action>.`",
        [A.TicketCaptain],
        [
            new(S.LinearWorkspaceStandard, "`DECIDED - result: <result>; immutable contract: <contract>; next: <action>.`", new(F.LinearWorkspaceStandardCommunicationAndEvidence, 17))
        ]);

    public static readonly Claim DocsInquisitionRecord = new(
        "A docs-writer's inquisition-record return names the exact path, blob or content digest and preserved evidence.",
        [A.DocsWriter], []);

    public static readonly Claim DoneMerged = new(
        "A captain returns done <key>: merged after merge PASS (or the user's click) and cleanup.",
        [A.TicketCaptain], []);

    public static readonly Claim DonePrReady = new(
        "A captain returns done <key>: PR ready when its PR is offered at Ready to Merge (the Inquisition captain when it offers the record's PR).",
        [A.TicketCaptain],
        [
            new(S.WorkingTreeStandard, "The captain offers a PR with its PASS block, sets `Ready to Merge`, and returns `done <key>: PR ready`.", new(F.WorkingTreeStandardReturnAndRelease, 1))
        ]);

    public static readonly Claim FailBlockForm = new(
        "A FAIL block reads `FAIL - rubric/type: <rubric>; contract <ref>; candidate <SHA>; base <SHA>; reviewer/model: <name>; gates <N/N>. 1. Where; severity; wrong; why; required correction; owner. Evidence: <link>.`",
        [A.Reviewer],
        [
            new(S.LinearWorkspaceStandard, "`FAIL - rubric/type: <rubric>; contract <ref>; candidate <SHA>; base <SHA>; reviewer/model: <name>; gates <N/N>. 1. Where: <place>; severity: <blocker|should|nit>; wrong: <fact>; why: <impact>; required correction: <action>; owner: <role>. Evidence: <link>.`", new(F.LinearWorkspaceStandardCommunicationAndEvidence, 15))
        ]);

    public static readonly Claim FindingSeverityAndAxisSlot = new(
        "Each finding in a FAIL or PASS tail carries severity and axis in the wrong: slot as \"wrong: <axis>: <fact>\", axis being contract, standards, gates or security.",
        [A.Reviewer],
        [
            new(S.Reviewer, "Each finding, in a FAIL or a PASS tail, carries its severity and its axis in the `wrong:` slot, as `wrong: <axis>: <fact>`, the axis being `contract`, `standards`, `gates` or `security`.", new(F.ReviewerCodeVerdict, 1))
        ]);

    public static readonly Claim FollowingFixTicketIfDependent = new(
        "If a later merge depends on the defective source work, a following fix ticket is filed instead of reverting.",
        [A.TicketCaptain, A.Reviewer],
        [
            new(S.Reviewer, "Distinguish integration defects from defects in the source work so the Captain can route a fix here, or a revert and source rework; a later dependent merge requires a following fix ticket.", new(F.ReviewerMergeMethod, 5), () => [ReturnAdmiralToNo.MergeReviewDistinguishesDefects!]),
            new(S.TicketCaptain, "If a later merge depends on it, file a following fix ticket instead of reverting.", new(F.TicketCaptainKindsAndFailurePaths, 15)),
            new(S.WorkingTreeStandard, "If a later merge depends on it, use a following fix ticket.", new(F.WorkingTreeStandardReturnAndRelease, 11))
        ]);

    public static readonly Claim ImplementedBlockForm = new(
        "An implementation hop returns to the Ticket Captain in the IMPLEMENTED form `IMPLEMENTED - hop/candidate <SHA>; <behavior>; proof: <evidence>; blocker: <none or named blocker>.`",
        [A.CodeWriter, A.DocsWriter, A.Crew],
        [
            new(S.CodeWriter, "Outside proof-only, return to the Ticket Captain in the standard's form, proof carrying the proof recipe of `maintain-verification-skill`'s Proof: the red lines, or for an unused-member Bug its definition and reference search, and gates run with exits; a Prototype hop returns runnable-demo evidence and its commit: `IMPLEMENTED - hop/candidate <SHA>; <behavior>; proof: <evidence>; blocker: <none or named blocker>.`", new(F.CodeWriterReturn, 3), () => [ReturnOrdinaryToWriter.WriterReturnCarriesRedProof]),
            new(S.DocsWriter, "To the Ticket Captain, use the `IMPLEMENTED` form in the communication protocol, naming the SHA, files, witness, checks, gaps, and evidence.", new(F.DocsWriterReturn, 1)),
            new(S.LinearWorkspaceStandard, "`IMPLEMENTED - hop/candidate <SHA>; <behavior>; proof: <evidence>; blocker: <none or named blocker>.`", new(F.LinearWorkspaceStandardCommunicationAndEvidence, 16))
        ]);

    public static readonly Claim IncompleteNamesGaps = new(
        "INCOMPLETE names what could not be read or checked (including a failed dydo survey's error) and keeps the verified actions or findings.",
        [A.Crew],
        [
            new(S.Admiral, "[ ] `dydo survey` failed: return `INCOMPLETE` with its error.", new(F.AdmiralCheckInSurveyTheSurveyor, 9)),
            new(S.ChiefOfStaff, "**INCOMPLETE** — name unreadable sources or missing facts and what could not be checked; retain verified findings, including recommended actions.", new(F.ChiefOfStaffNightSurveyReturn, 5))
        ]);

    public static readonly Claim InquisitorAreaEntriesShape = new(
        "The inquisitor returns to the captain in the given shape, writing the area entries in the return shape; an empty list is written none.",
        [A.Inquisitor],
        [
            new(S.Inquisitor, "To the captain, in this shape; an empty list is written `none`: ```text\nPhase <read|break|drive>; area <ID>; handed back <items or none>; context <tokens> of <limit>; inquisitor/model: <identity>.\nFindings:\n1. <title>; severity <blocker|should|nit>; where `file:line`; wrong: <the wrong outcome in one sentence>; proof: <per the Proof table, or tagged testable|drivable>.\nSightings:\n1. <register ID>; where `file:line`; proof: <per the Proof table>.\nNot reproduced:\n1. <register ID or your hypothesis>; observed: <the deciding observation>.\nUnreachable:\n1. <journey>; prerequisite: <what it needs>; route tried: <what you tried>.\n```", new(F.InquisitorReturn, 1))
        ]);

    public static readonly Claim InquisitorConfirmedIsSighting = new(
        "A confirmed register entry is a sighting with that proof.",
        [A.Inquisitor],
        [
            new(S.Inquisitor, "A confirmed register entry is a sighting with that proof.", new(F.InquisitorBreakDocument, 1))
        ]);

    public static readonly Claim InquisitorNotReproduced = new(
        "Every not-reproduced and inconclusive hypothesis goes under Not reproduced with its observation, marked inconclusive where it is.",
        [A.Inquisitor],
        [
            new(S.Inquisitor, "Every not-reproduced and inconclusive hypothesis goes under `Not reproduced` with its observation, marked `inconclusive` where it is.", new(F.InquisitorBreakDocument, 2))
        ]);

    public static readonly Claim InquisitorReturnCompleteness = new(
        "The inquisitor return is done when every work item is documented or listed as handed back and every hypothesis appears exactly once.",
        [A.Inquisitor],
        [
            new(S.Inquisitor, "Done when every work item is documented or listed as handed back.", new(F.InquisitorMethod, 5))
        ]);

    public static readonly Claim LengthAimsSoft = new(
        "Brevity is soft: ordinary events aim for at most 800 characters, descriptions and state at most 1,500 characters or 12 lines; a necessary finding, gap, blocker or evidence may exceed it, omitting narration and repetition.",
        [A.AnyAgent],
        [
            new(S.LinearWorkspaceStandard, "Brevity is soft: ordinary events aim for at most 800 characters, descriptions and state for at most 1,500 characters or 12 lines.", new(F.LinearWorkspaceStandardCommunicationAndEvidence, 23))
        ]);

    public static readonly Claim LinearPostStartsWithForm = new(
        "Start each Linear post with the applicable form (PASS, FAIL, IMPLEMENTED, DECIDED, RATIFIED, OVERTURNED, STATE).",
        [A.AnyAgent],
        [
            new(S.LinearWorkspaceStandard, "Start each post with the applicable form:", new(F.LinearWorkspaceStandardCommunicationAndEvidence, 13))
        ]);

    public static readonly Claim MergeHopReturnsParentsAndSha = new(
        "A merge hop returns both parents and the merge SHA.",
        [A.CodeWriter],
        [
            new(S.CodeWriter, "Return both parents and the merge SHA.", new(F.CodeWriterMergeLead, 8))
        ]);

    public static readonly Claim MergeReviewDistinguishesDefects = new(
        "The merge reviewer distinguishes integration defects from source-work defects so the Captain can route a fix here or a revert and source rework.",
        [A.Reviewer], []);

    public static readonly Claim NoActionBare = new(
        "When returning NO_ACTION, return only `NO_ACTION`.",
        [A.Crew],
        [
            new(S.ChiefOfStaff, "Return only `NO_ACTION`.", new(F.ChiefOfStaffNightSurveyReturn, 2))
        ]);

    public static readonly Claim NoActionOnlyWhenComplete = new(
        "NO_ACTION means every item was checked completely and nothing is flagged; an incomplete inspection never yields NO_ACTION.",
        [A.Crew],
        [
            new(S.Admiral, "`NO_ACTION`: every item above checked, nothing flagged.", new(F.AdmiralCheckInSurveyTheSurveyor, 11)),
            new(S.ChiefOfStaff, "**NO_ACTION** — only after all named Projects and outstanding nudges are checked completely, with no missing facts or action needed.", new(F.ChiefOfStaffNightSurveyReturn, 1))
        ]);
}
