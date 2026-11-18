namespace DynaDocs.Tests.SkillBuild.Fixture;

using DynaDocs.SkillBuild;
using A = DynaDocs.Tests.SkillBuild.Fixture.FixtureActor;
using F = DynaDocs.Tests.SkillBuild.Fixture.FixtureSection;
using S = DynaDocs.Tests.SkillBuild.Fixture.FixtureSkill;

/// <summary>Migration fixture: the inventory's <c>inquisition</c> claims, sub-topics area to follow.</summary>
public static class InquisitionAreaToFollow
{
    public static readonly Claim AreaMapDone = new(
        "The map is done when every changed path sits in exactly one area, every seam is an area, and every cell holds yes or a one-line reason.",
        [A.TicketCaptain],
        [
            new(S.TicketCaptain, "Done when every changed path sits in exactly one area, every seam is an area, and every cell holds `yes` or a one-line reason.", new(F.TicketCaptainInquisitionN3DrawTheAreaMap, 8))
        ]);

    public static readonly Claim AreaMapFromDiff = new(
        "The area map is drawn from git diff --stat <landing base>..<audit SHA> and the file layout, never from the feature list or an earlier round map.",
        [A.TicketCaptain],
        [
            new(S.TicketCaptain, "Draw it from `git diff --stat <landing base>..<audit SHA>` and the file layout, never from the feature list or an earlier round's map.", new(F.TicketCaptainInquisitionN3DrawTheAreaMap, 1))
        ]);

    public static readonly Claim AreaMapPostedAsTable = new(
        "The area map is posted on the ticket as a table.",
        [A.TicketCaptain],
        [
            new(S.TicketCaptain, "Post it on the ticket as a table: | A-01 | the files or folder | one line | yes | yes, or why not in one line | yes, or why not in one line |", new(F.TicketCaptainInquisitionN3DrawTheAreaMap, 2))
        ]);

    public static readonly Claim AreasSmall = new(
        "Areas are kept small: one file can be an area and tiny areas merge at map time.",
        [A.TicketCaptain],
        [
            new(S.TicketCaptain, "**Small on purpose:** a big area gets superficial coverage.", new(F.TicketCaptainInquisitionN3DrawTheAreaMap, 3))
        ]);

    public static readonly Claim AuditWithoutProject = new(
        "An audit with no Project files a team ticket, base main, with no landing relation.",
        [A.TicketCaptain],
        [
            new(S.TicketCaptain, "An audit with no Project files a team ticket, base `main`, with no landing relation.", new(F.TicketCaptainInquisitionN8TriageEachReturn, 4))
        ]);

    public static readonly Claim BranchStepDone = new(
        "The branch step is done when branch, audit SHA and worktree path are on the ticket and the register first commit is pushed.",
        [A.TicketCaptain],
        [
            new(S.TicketCaptain, "Done when branch, audit SHA and worktree path are on the ticket and the register's first commit is pushed.", new(F.TicketCaptainInquisitionN2OpenTheAuditBranch, 2))
        ]);

    public static readonly Claim BranchesNeverMerge = new(
        "The inquisition, audit, group and proof branches never merge; the Inquisition merges nothing but its own record.",
        [A.TicketCaptain, A.Inquisitor],
        [
            new(S.TicketCaptain, "An Inquisition gets `inquisition/<slug>` from the integrated feature SHA, never merged.", new(F.TicketCaptainKindsAndFailurePaths, 9), () => [WorktreeAuditToReviewer.InquisitionAuditBranch]),
            new(S.WorkingTreeStandard, "| Inquisition audit | the audit SHA, the integrated feature commit | `inquisition/<slug>`; a group Sub-ticket's `inquisition/<slug>-<group>` | none; never merges | - |", new(F.WorkingTreeStandardBranchesAndTargets, 5), () => [WorktreeAuditToReviewer.InquisitionAuditBranch])
        ]);

    public static readonly Claim BreakAndDriveApplyWhereverBehaviourChanged = new(
        "Break and Drive apply wherever the landing changed behaviour.",
        [A.TicketCaptain],
        [
            new(S.TicketCaptain, "**Behaviour:** Break and Drive apply wherever the landing changed behaviour; Drive says `nothing to run` on a project without a runnable surface.", new(F.TicketCaptainInquisitionN3DrawTheAreaMap, 7), () => [InquisitionAreaToFollow.DriveNotApplicableWithoutRunnableSurface!])
        ]);

    public static readonly Claim BreakDiscardUnconfirmedTests = new(
        "For a not-reproduced or inconclusive hypothesis the test is discarded and the observation (the deciding one when inconclusive) is noted.",
        [A.Inquisitor],
        [
            new(S.Inquisitor, "Discard the test; note the observation.", new(F.InquisitorBreakWork, 3))
        ]);

    public static readonly Claim BreakHypothesisList = new(
        "Break lists its own hypotheses plus every register entry tagged testable; each names the suspected failure, the seam, the input or state, the expected observation and the test that would refute it.",
        [A.Inquisitor],
        [
            new(S.Inquisitor, "**List the hypotheses.** Yours, plus every register entry for the area tagged `testable`. Done when every tagged entry and every hypothesis of yours is on the list.", new(F.InquisitorBreakDiscovery, 2))
        ]);

    public static readonly Claim BreakOwnTests = new(
        "Break forms hypotheses about how the area fails and decides each with one test it writes itself on its proof branch, in the project own test framework, through the public seam where one exists, running only that test.",
        [A.Inquisitor],
        [
            new(S.Inquisitor, "Break forms hypotheses about how the area fails and decides each with a test it writes itself, on its proof branch.", new(F.InquisitorBreakLead, 1))
        ]);

    public static readonly Claim BreakReadForFailure = new(
        "Break reads the area for how it fails: inputs, states, error paths, boundaries, concurrency and seams it calls through.",
        [A.Inquisitor],
        [
            new(S.Inquisitor, "**Read the area for how it fails.** Its inputs, states, error paths, boundaries, concurrency and the seams it calls through. Done when you can name where the area's behaviour can go wrong.", new(F.InquisitorBreakDiscovery, 1))
        ]);

    public static readonly Claim BreakVerdictMapping = new(
        "A test red for the stated reason means confirmed, a green test means not reproduced, and a test red for another reason or undecidable means inconclusive.",
        [A.Inquisitor],
        [
            new(S.Inquisitor, "**Red, failing for the stated reason:** confirmed.", new(F.InquisitorBreakWork, 1))
        ]);

    public static readonly Claim BreakVerdictStepDone = new(
        "The verdict step is done when every hypothesis has one verdict and every confirmed one has its commit.",
        [A.Inquisitor],
        [
            new(S.Inquisitor, "Done when every hypothesis has one verdict and every confirmed one has its commit.", new(F.InquisitorBreakWork, 5))
        ]);

    public static readonly Claim BugCaptainAdoptsPreparedPatch = new(
        "A Bug's captain adopts an inquisition's prepared patch branch when one is attached: it verifies and reviews it rather than rebuilding it.",
        [A.TicketCaptain], []);

    public static readonly Claim BugFilingFields = new(
        "A proven blocker or should is filed as a Bug: Mode AFK, Severity matching the finding, in the Project, base the feature branch, ## Reproduction holding its proof, linked to the Inquisition.",
        [A.TicketCaptain],
        [
            new(S.TicketCaptain, "**Proven blocker or should:** file a Bug: Mode `AFK`, Severity `blocker` or `should` matching the finding, in the Project, base the feature branch, `## Reproduction` holding its proof (the red-test SHA and test, the quoted passages, the definition and reference search, or the rerunnable drive steps), linked to this Inquisition.", new(F.TicketCaptainInquisitionN8TriageEachReturn, 2))
        ]);

    public static readonly Claim BugFixMayMergeMidRound = new(
        "A Bug fix may merge into the feature mid-round; the audit is pinned to its SHA.",
        [A.TicketCaptain],
        [
            new(S.TicketCaptain, "Its fix may merge into the feature mid-round: the audit is pinned to its SHA.", new(F.TicketCaptainInquisitionN8TriageEachReturn, 6))
        ]);

    public static readonly Claim BugStatusAndLandingBlock = new(
        "A blocker Bug goes to Todo, a should Bug to Backlog, natively blocking the Project landing Merge Ticket when it exists.",
        [A.TicketCaptain],
        [
            new(S.TicketCaptain, "A blocker goes to `Todo`, a should to `Backlog`, natively blocking the Project's landing Merge Ticket when it exists.", new(F.TicketCaptainInquisitionN8TriageEachReturn, 3))
        ]);

    public static readonly Claim CaptainNeverHunts = new(
        "The captain never hunts for bugs itself; reading code for bugs is the crew work.",
        [A.TicketCaptain],
        [
            new(S.TicketCaptain, "You never hunt yourself: reading code for bugs is the crew's.", new(F.TicketCaptainInquisitionLead, 2))
        ]);

    public static readonly Claim CaptainRegisterAndTriage = new(
        "The captain keeps the findings register, triages every return into Bugs, one nit ticket and Questions, and prepares well-defined fixes on their Bugs branches.",
        [A.TicketCaptain],
        [
            new(S.TicketCaptain, "You keep the findings register, triage every return into Bugs, one nit ticket and Questions, and prepare well-defined fixes on their Bugs' branches.", new(F.TicketCaptainKindsAndFailurePaths, 11))
        ]);

    public static readonly Claim CleanAreaReportsNothing = new(
        "A clean area reports nothing.",
        [A.Inquisitor],
        [
            new(S.Inquisitor, "**A clean area reports nothing.** A return with only real problems, or none, has succeeded.", new(F.InquisitorCalibration, 4))
        ]);

    public static readonly Claim CloseDoneWhenVerified = new(
        "The Inquisition closes Done when its record is merged and verified on the feature, Bugs filed and their proof refs retained.",
        [A.TicketCaptain],
        [
            new(S.LinearWorkspaceStandard, "| `Inquisition` | captain | primary; its group Sub-tickets above about 10 areas | One round of audit of the integrated feature: an area map, each area run through Read, Break and Drive by `inquisitor` crew, every finding triaged by severity into Bugs, one nit ticket and Questions. Each round starts from zero. | its record merged and verified on the feature, Bugs filed and their proof refs retained; a group Sub-ticket, its register final and pushed | `#5E6AD2` |", new(F.LinearWorkspaceStandardType, 5), () => [InquisitionGroupToRead.GroupCaptainRunAndClose, LinearTicketToWriter.TypeInquisition]),
            new(S.TicketCaptain, "Close the Inquisition `Done` and return `done <key>: merged`.", new(F.TicketCaptainInquisitionN12Close, 3)),
            new(S.WorkingTreeStandard, "Only then does it delete the audit branch and, as parent, every group's audit branch, name their remotes and any proof branch no open Bug needs on the record for the user, close `Done` and return `done <key>: merged`.", new(F.WorkingTreeStandardRetainingAnInquisitionSRecordAndProofs, 9), () => [WorktreeAuditToReviewer.DeleteInquisitionAuditBranches, WorktreeAuditToReviewer.NameRemoteBranchesDueForUser])
        ]);

    public static readonly Claim CommissionInputs = new(
        "The commission inputs (provider and model per phase, swarm width, Codex cap, crew context limit) are put to the user and recorded in the ticket ## Scope; the commission step is done when they are recorded.",
        [A.TicketCaptain, A.Admiral],
        [
            new(S.TicketCaptain, "Done when the ticket's `## Scope` records: the audit SHA, the integrated feature commit, and the landing base it is diffed against; the governing intent: the Project plan, the landed tickets' contracts, the Decision Records; a provider and model for each phase, and the swarm width: the most crew agents running at once; the Codex cap: start about 8 at once, go toward 16 while runs stay clean, back off on a 429; the crew context limit, 110k tokens unless the commission set another; the project's verification skill, or the one-line reason nothing in the project runs.", new(F.TicketCaptainInquisitionN1CommissionInputs, 1))
        ]);

    public static readonly Claim DispatchDone = new(
        "Dispatch is done when every applicable phase of every area has returned.",
        [A.TicketCaptain],
        [
            new(S.TicketCaptain, "Done when every applicable phase of every area has returned.", new(F.TicketCaptainInquisitionN6ScheduleThePipeline, 7))
        ]);

    public static readonly Claim DispositionVocabulary = new(
        "A disposition is a Bug key, patch <SHA> on its Bug, the nit ticket, a Question key, or refuted with the observation.",
        [A.TicketCaptain],
        [
            new(S.TicketCaptain, "A disposition is a Bug key, `patch <SHA>` on its Bug, the nit ticket, a Question key, or `refuted` with the observation.", new(F.TicketCaptainInquisitionN5KeepTheFindingsRegister, 3))
        ]);

    public static readonly Claim DriveEachJourneyViaSkill = new(
        "Drive runs each journey through the verification skill and the API endpoints, following the skill launch model: health-check the instance before trusting it and clean every drive residue.",
        [A.Inquisitor],
        [
            new(S.Inquisitor, "**Drive each journey** through the verification skill and the API endpoints, following the skill's launch model: health-check the instance before trusting it, and clean every drive's residue. Capture what you observe: output, screenshot or snapshot, response body, exit code. Done when each journey has an observed result.", new(F.InquisitorDriveWork, 1))
        ]);

    public static readonly Claim DriveJourneysLikeAUser = new(
        "Drive takes the running app through the area journeys and edge cases the way a user would.",
        [A.Inquisitor],
        [
            new(S.Inquisitor, "Drive takes the running app through the area's journeys and edge cases, the way a user would, so that it works as well as it reads.", new(F.InquisitorDriveLead, 1))
        ]);

    public static readonly Claim DriveJudgeJourney = new(
        "Drive judges each journey: the observed result against the expected one from the intent, feature map and register entry.",
        [A.Inquisitor],
        [
            new(S.Inquisitor, "**Judge each journey.** The observed result against the expected one from the intent, the feature map and the register entry. Done when each journey is `works`, a finding, a sighting, or `unreachable` with its concrete prerequisite.", new(F.InquisitorDriveWork, 2))
        ]);

    public static readonly Claim DriveListJourneys = new(
        "Drive lists the journeys: every feature of the area a user can reach through the app or API, each with edge cases (empty, invalid, boundary, repeated, interrupted, concurrent where a user can cause it).",
        [A.Inquisitor],
        [
            new(S.Inquisitor, "**List the journeys.** Every feature of the area a user can reach through the app or its API, each with its edge cases: empty, invalid, boundary, repeated, interrupted, concurrent where a user can cause it. Add every register entry for the area tagged `drivable`. Done when every feature of the area and every tagged entry has at least one journey on the list.", new(F.InquisitorDriveDiscovery, 2))
        ]);

    public static readonly Claim DriveLoadVerificationSkill = new(
        "Drive loads the verification skill the brief names.",
        [A.Inquisitor],
        [
            new(S.Inquisitor, "**Load the verification skill** your brief names. Read its launch and drive sections and the feature-map files that cover the area. Done when you know how to launch the app at the audit SHA and how to drive each feature the area touches.", new(F.InquisitorDriveDiscovery, 1))
        ]);

    public static readonly Claim DriveNotApplicableWithoutRunnableSurface = new(
        "A project with nothing to run records that Drive does not apply.",
        [A.TicketCaptain, A.Admiral, A.Inquisitor],
        [
            new(S.Admiral, "A project with a runnable surface and no verification skill needs one first: raise it with the user, who makes it with `create-verification-skill`; one with nothing to run records that Drive does not apply.", new(F.AdmiralMethod, 57), () => [InquisitionRecordToWork.VerificationSkillFirst]),
            new(S.Inquisitor, "You change nothing in the repository; on a project with nothing to run, Drive does not apply and the area's map row says so.", new(F.InquisitorDriveLead, 2))
        ]);

    public static readonly Claim FileAfterIntegration = new(
        "Once the feature is integrated, an Inquisition is filed in Backlog with its feature SHA and cost.",
        [A.Admiral], []);

    public static readonly Claim FilesOutcomesNoVerdict = new(
        "An Inquisition files outcomes and never PASSes or FAILs.",
        [A.TicketCaptain],
        [
            new(S.TicketCaptain, "It files, never PASSes or FAILs.", new(F.TicketCaptainKindsAndFailurePaths, 13))
        ]);

    public static readonly Claim FindingIds = new(
        "A new finding gets a row with the next ID F-01, F-02 and so on; a group qualifies its IDs with its group, as G2-F-07.",
        [A.TicketCaptain],
        [
            new(S.TicketCaptain, "IDs run `F-01`, `F-02` and so on; a group qualifies its IDs with its group, as `G2-F-07`.", new(F.TicketCaptainInquisitionN5KeepTheFindingsRegister, 1))
        ]);

    public static readonly Claim FindingNewOrPreexisting = new(
        "The inquisitor says whether a finding is new or pre-existing; work that merely exposed an older defect is worth reporting.",
        [A.Inquisitor],
        [
            new(S.Inquisitor, "**New or pre-existing.** Work that merely exposed an older defect is worth reporting; say which.", new(F.InquisitorCalibration, 2))
        ]);

    public static readonly Claim FindingReachableAndConcrete = new(
        "A finding must be reachable and concrete: under X this returns or corrupts Y, a sequence someone hits.",
        [A.Inquisitor],
        [
            new(S.Inquisitor, "**Reachable and concrete.** \"Under X this returns or corrupts Y\": a sequence someone hits.", new(F.InquisitorCalibration, 1))
        ]);

    public static readonly Claim FollowUpBriefItemsOnly = new(
        "On a follow-up or hand-back brief the inquisitor works only the items it names.",
        [A.Inquisitor],
        [
            new(S.Inquisitor, "A follow-up or hand-back brief names your items; work only those.", new(F.InquisitorMethod, 2))
        ]);
}
