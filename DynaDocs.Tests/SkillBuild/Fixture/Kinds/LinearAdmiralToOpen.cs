namespace DynaDocs.Tests.SkillBuild.Fixture;

using DynaDocs.SkillBuild;
using A = DynaDocs.Tests.SkillBuild.Fixture.FixtureActor;
using F = DynaDocs.Tests.SkillBuild.Fixture.FixtureSection;
using S = DynaDocs.Tests.SkillBuild.Fixture.FixtureSkill;

/// <summary>Migration fixture: the inventory's <c>linear</c> claims, sub-topics admiral to open.</summary>
public static class LinearAdmiralToOpen
{
    public static readonly Claim AdmiralOwnsBoardStatuses = new(
        "The admiral owns Project and map-holder-held ticket statuses; delivery captains own theirs.",
        [A.Admiral],
        [
            new(S.Admiral, "**Board discipline:** own Project and map-holder-held ticket statuses; delivery captains own theirs.", new(F.AdmiralBoundary, 18))
        ]);

    public static readonly Claim AfkTicketsCarryNoPriority = new(
        "AFK tickets carry no priority; the plan order and native blockers carry their sequence.",
        [A.AnyAgent, A.Admiral],
        [
            new(S.Admiral, "Set and revisit priority on every human waiter by the standard's guide; AFK order remains in the map and blockers.", new(F.AdmiralMethod, 55), () => [LinearTicketToWriter.WaiterPrioritySetByStandard]),
            new(S.LinearWorkspaceStandard, "AFK tickets carry no priority: the plan order and the native blockers carry their sequence.", new(F.LinearWorkspaceStandardPriority, 6))
        ]);

    public static readonly Claim AssignmentIsTheClaim = new(
        "Linear assignment is the claim on a ticket: assigned means taken; unassigned, Todo and unblocked means pickable.",
        [A.TicketCaptain, A.AnyAgent],
        [
            new(S.LinearWorkspaceStandard, "Assignment is the claim: assigned means taken; unassigned, `Todo` and unblocked means pickable.", new(F.LinearWorkspaceStandardTicketStatuses, 14)),
            new(S.Wayfinder, "Assignment is the claim: assign it first, before any work, so concurrent sessions skip it.", new(F.WayfinderTickets, 3)),
            new(S.WorkingTreeStandard, "Linear assignment is the **claim**.", new(F.WorkingTreeStandardInvariants, 1))
        ]);

    public static readonly Claim BoardInboxAndResumePoint = new(
        "The Linear board is the captain's inbox and each hop's SHA its resume point.",
        [A.TicketCaptain],
        [
            new(S.TicketCaptain, "The board is your inbox and each hop's SHA its resume point.", new(F.TicketCaptainBoundary, 26))
        ]);

    public static readonly Claim CaptainRelayKeepsIdentity = new(
        "A captain relay preserves the original author or reviewer identity and distinguishes the captain's posting identity.",
        [A.TicketCaptain],
        [
            new(S.LinearWorkspaceStandard, "A captain relay preserves the original author or reviewer identity and distinguishes the captain's posting identity.", new(F.LinearWorkspaceStandardTruthfulCommentSignatures, 5))
        ]);

    public static readonly Claim CaptainTicketCarriesTypeAndMode = new(
        "Every captain-held ticket carries exactly one Type and one Mode (AFK or HITL); new implementation tickets get one Type and one Mode.",
        [A.TicketCaptain, A.Admiral, A.AnyAgent],
        [
            new(S.LinearWorkspaceStandard, "Every ticket carries exactly one Type.", new(F.LinearWorkspaceStandardTicketLabels, 2)),
            new(S.TicketCaptain, "**Record:** every captain-held ticket carries one Type and one Mode (`AFK` or `HITL`).", new(F.TicketCaptainBoundary, 24)),
            new(S.Wayfinder, "Every captain-held ticket carries Mode **HITL** (human in the loop, worked _with_ a human who speaks for themselves) or **AFK**, driven by the agent alone.", new(F.WayfinderTicketTypes, 1))
        ]);

    public static readonly Claim ClaimDoneCriteria = new(
        "A claim is done when the ticket is assigned, its branch and isolated worktree exist, and branch, base SHA and worktree path are recorded on the ticket.",
        [A.TicketCaptain],
        [
            new(S.TicketCaptain, "**Done:** the parent is assigned and records its Type, Mode, branch, base SHA, isolated worktree, clean state, and owned paths.", new(F.TicketCaptainMethod, 4)),
            new(S.WorkingTreeStandard, "Branch, base SHA and worktree path are posted on the relevant ticket or Sub-ticket before the first edit.", new(F.WorkingTreeStandardInvariants, 8))
        ]);

    public static readonly Claim CommentSignsRoleAndModel = new(
        "Every agent-authored Linear comment identifies its acting role and truthful model identity.",
        [A.Crew, A.AnyAgent],
        [
            new(S.CodeWriter, "Sign every comment and return `code-writer/model: <truthful identity>`.", new(F.CodeWriterReturn, 2)),
            new(S.LinearWorkspaceStandard, "Every agent-authored Linear comment identifies its acting role and truthful model identity.", new(F.LinearWorkspaceStandardTruthfulCommentSignatures, 1))
        ]);

    public static readonly Claim ContractsAreTicketsOrSubtickets = new(
        "For the Project's map holder, contracts are tickets under the Project; for a captain they are Sub-tickets under its ticket.",
        [A.MapHolder, A.TicketCaptain],
        [
            new(S.Wayfinder, "For the Project's map holder, contracts are tickets under the Project; for a captain they are Sub-tickets under its ticket.", new(F.WayfinderTickets, 7))
        ]);

    public static readonly Claim FilterToolOutput = new(
        "Filter tool output before it enters model context.",
        [A.AnyAgent],
        [
            new(S.LinearWorkspaceStandard, "Filter tool output before it enters model context.", new(F.LinearWorkspaceStandardCommunicationAndEvidence, 4))
        ]);

    public static readonly Claim FindingTicketCarriesFindingSeverity = new(
        "A ticket born from a finding carries that finding's severity, by DR 058's names.",
        [A.AnyAgent],
        [
            new(S.LinearWorkspaceStandard, "A ticket born from a finding carries that finding's severity, by DR 058's names; whoever files the ticket sets it.", new(F.LinearWorkspaceStandardSeverity, 1), () => [LinearAdmiralToOpen.FindingTicketFilerSetsSeverity!])
        ]);

    public static readonly Claim FindingTicketFilerSetsSeverity = new(
        "Whoever files a ticket born from a finding sets its Severity.",
        [A.AnyAgent], []);

    public static readonly Claim FuturefeatureOnlyUserPromotes = new(
        "Only the user promotes a FutureFeature.",
        [A.User],
        [
            new(S.LinearWorkspaceStandard, "| `FutureFeature` | backlog | An unscheduled strategic possibility with no Type yet. Only the user promotes it. |", new(F.LinearWorkspaceStandardTicketStatuses, 2), () => [LinearParentToThree.StatusFuturefeatureMeaning])
        ]);

    public static readonly Claim FuturefeatureProjectGrainPromotion = new(
        "At Project or Initiative grain, promoting a FutureFeature creates and links that record and marks the FutureFeature Done.",
        [A.User],
        [
            new(S.LinearWorkspaceStandard, "A `FutureFeature` is promoted by the user alone: at ticket grain it gains a Type and moves to `Backlog` or `Todo` on the same record; at Project or Initiative grain that record is created and linked and the FutureFeature is marked `Done`.", new(F.LinearWorkspaceStandardTicketStatuses, 15), () => [LinearAdmiralToOpen.FuturefeatureTicketGrainPromotion!])
        ]);

    public static readonly Claim FuturefeatureRejectedCanceled = new(
        "A rejected FutureFeature becomes Canceled with the reason recorded.",
        [A.User],
        [
            new(S.LinearWorkspaceStandard, "A rejected one becomes `Canceled` with the reason recorded.", new(F.LinearWorkspaceStandardTicketStatuses, 16))
        ]);

    public static readonly Claim FuturefeatureTicketGrainPromotion = new(
        "At ticket grain, promoting a FutureFeature gives it a Type and moves it to Backlog or Todo on the same record.",
        [A.User], []);

    public static readonly Claim HistoricalCommentsUntouched = new(
        "Historical comments stay untouched.",
        [A.AnyAgent],
        [
            new(S.LinearWorkspaceStandard, "Historical comments stay untouched.", new(F.LinearWorkspaceStandardTruthfulCommentSignatures, 7))
        ]);

    public static readonly Claim InProgressStatusMeaning = new(
        "In Progress is the status of a record not running the chain itself, such as a parent while its lanes run, a wayfinding ticket or an Inquisition sweep.",
        [A.TicketCaptain, A.AnyAgent],
        [
            new(S.LinearWorkspaceStandard, "| `In Progress` | started | A record not running the chain itself: a parent while its lanes run, a wayfinding ticket, an Inquisition's sweep (its map, phases, triage and patch lane) until its record enters the chain. |", new(F.LinearWorkspaceStandardTicketStatuses, 5)),
            new(S.TicketCaptain, "**Done:** the parent is `In Progress` while lanes run; each has its own chain and evidence.", new(F.TicketCaptainMethod, 16))
        ]);

    public static readonly Claim InquisitionSweepHoldsInProgress = new(
        "An Inquisition ticket stays In Progress through its sweep (map, phases, proofs, triage, patch lane) until its own record enters the chain.",
        [A.TicketCaptain],
        [
            new(S.TicketCaptain, "You alone set its status at each chain spawn; an Inquisition's sweep holds `In Progress` until its record enters the chain.", new(F.TicketCaptainBoundary, 25), () => [LinearAdmiralToOpen.OnlyCaptainSetsDeliveryStatus!, LinearParentToThree.StatusFlipsOnlyOnChainSpawn])
        ]);

    public static readonly Claim KeepAttemptsOnExistingRecord = new(
        "Every attempt and agent invocation is recorded as comments and evidence on the existing ticket or Sub-ticket, never as another child; successive agents may work through the same record.",
        [A.TicketCaptain, A.AnyAgent],
        [
            new(S.WorkingTreeStandard, "An agent invocation is recorded as comments and evidence on the relevant ticket or Sub-ticket, never as another child.", new(F.WorkingTreeStandardDelegation, 28))
        ]);

    public static readonly Claim KeepBlockersAnswersEvidenceTrue = new(
        "Blockers, answers and evidence are kept true to the work.",
        [A.Admiral],
        [
            new(S.Admiral, "Keep blockers, answers and evidence true to the work.", new(F.AdmiralBoundary, 19))
        ]);

    public static readonly Claim KeepContractInDescription = new(
        "Keep the stable current contract and state in the ticket description.",
        [A.AnyAgent],
        [
            new(S.LinearWorkspaceStandard, "Keep the stable current contract and state in the description.", new(F.LinearWorkspaceStandardCommunicationAndEvidence, 2))
        ]);

    public static readonly Claim LiveInstructionOverridesRecordBack = new(
        "Write a live instruction that overrides the record back to the record.",
        [A.TicketCaptain, A.AnyAgent],
        [
            new(S.Admiral, "Raise a conflict you find; write a live instruction that overrides the record back to it.", new(F.AdmiralBoundary, 25), () => [LinearParentToThree.RaiseConflictFoundInRecord]),
            new(S.TicketCaptain, "Raise a conflict you find; write a live instruction that overrides the record back to it.", new(F.TicketCaptainBoundary, 31), () => [LinearParentToThree.RaiseConflictFoundInRecord])
        ]);

    public static readonly Claim MergeOperationClosesDone = new(
        "The merge operation and its source ticket close Done.",
        [A.TicketCaptain],
        [
            new(S.TicketCaptain, "**Done:** merge review passes, or the user's click has landed the atomic ticket; the operation and source ticket close `Done`, the worktrees you or your crew made (lanes, merge checkouts, temp checkouts) are cleaned (`dydo clean --dry-run <worktree>`, then `dydo clean <worktree>`, run from outside that tree; it deletes the worktree's own branch once merged), merged local branches with no worktree go by `git branch -d`, and you return `done <key>: merged`.", new(F.TicketCaptainMethod, 65), () => [MergeAnswerToWake.MergeDoneCondition, ReturnAdmiralToNo.DoneMerged, WorktreeAuditToReviewer.CleanOwnWorktrees, WorktreeAuditToReviewer.DeleteMergedLocalBranchesNoWorktree]),
            new(S.WorkingTreeStandard, "| Integrate a Project ticket | `ticket-captain` | Its final Merge Sub-ticket runs a code-writer - conflicts and combined gates mapped, the merge performed (a conflict-free merge of the reviewed PR into its feature branch is `dydo merge`, whose merge SHA is the candidate for the combined gates and merge review; a conflicted merge stays the local merge), a resolution that refactored leaving its code no worse than either side - then a fresh merge reviewer, preserving the merge commit and hop SHAs; the map holder wires the order and may advance an independent ready PR. Parent stays `Ready to Merge` until merge PASS, then both close `Done`. |", new(F.WorkingTreeStandardWhoDoesWhat, 6), () => [LinearParentToThree.ParentStaysReadyToMerge, MergeAnswerToWake.CaptainDirectsMergeCodeWriter, MergeAnswerToWake.ConflictFreeMergeUsesDydoMerge, MergeAnswerToWake.MergeShaIsCandidateForCombinedGatesAndReview, MergeAnswerToWake.ProjectTicketIntegrationPath, MergeAnswerToWake.RefactorResolutionNoWorse, MergeAnswerToWake.RewireOrderForIndependentPr, ReviewFixToPrototype.MergeReviewAfterMerge])
        ]);

    public static readonly Claim MergeSubticketNeverReadyToMerge = new(
        "A Merge Sub-ticket never enters Ready to Merge.",
        [A.TicketCaptain],
        [
            new(S.LinearWorkspaceStandard, "`Ready to Merge` holds for a lane into its parent, a primary into the feature branch, and an atomic ticket or the landing into main; a Merge Sub-ticket never enters it, since it runs the chain and closes.", new(F.LinearWorkspaceStandardTicketStatuses, 12), () => [LinearParentToThree.ReadyToMergeScope]),
            new(S.TicketCaptain, "The parent stays `Ready to Merge` while the Sub-ticket runs; a Merge Sub-ticket never enters that status.", new(F.TicketCaptainMethod, 62), () => [LinearParentToThree.ParentStaysReadyToMerge]),
            new(S.WorkingTreeStandard, "A Merge Sub-ticket never enters `Ready to Merge`.", new(F.WorkingTreeStandardReturnAndRelease, 3))
        ]);

    public static readonly Claim ModeAfk = new(
        "Mode AFK: the captain reaches reviewed completion without a live conversation.",
        [A.AnyAgent], []);

    public static readonly Claim ModeHitl = new(
        "Mode HITL: the user and the captain work the ticket together in a session the user opened.",
        [A.AnyAgent],
        [
            new(S.LinearWorkspaceStandard, "| `HITL` | The user and the captain work the ticket together in a session the user opened. | `#F76B15` |", new(F.LinearWorkspaceStandardMode, 2))
        ]);

    public static readonly Claim NameProjectAndTicketByTitleKeyUrl = new(
        "Every Project and ticket is named by its title with its Linear key and URL inside that name.",
        [A.AnyAgent],
        [
            new(S.Wayfinder, "In everything the user reads, name every Project and ticket by its title, with its Linear key and URL inside that name.", new(F.WayfinderReferByName, 1))
        ]);

    public static readonly Claim NativeBlockerRelations = new(
        "Blocking uses Linear's native dependency relationship, connecting every waiting record to what it waits on.",
        [A.AnyAgent],
        [
            new(S.LinearWorkspaceStandard, "Native blocker relations connect every waiting record to what it waits on.", new(F.LinearWorkspaceStandardWhoHoldsWhat, 14)),
            new(S.Wayfinder, "Blocking uses Linear's native dependency relationship: essential because it renders the frontier visually in Linear, so the user sees what is takeable without opening the map.", new(F.WayfinderTickets, 4))
        ]);

    public static readonly Claim NeverInferAliasParentEffort = new(
        "Never infer an alias resolution, parent model or effort.",
        [A.AnyAgent],
        [
            new(S.LinearWorkspaceStandard, "Otherwise qualify a family or a requested/configured value; never infer an alias resolution, parent model, or effort.", new(F.LinearWorkspaceStandardTruthfulCommentSignatures, 3))
        ]);

    public static readonly Claim NoAtMentionUnlessDelegating = new(
        "Name an agent without its @ unless delegating, because an @mention starts that agent's session.",
        [A.AnyAgent],
        [
            new(S.LinearWorkspaceStandard, "Name an agent without its `@` unless you mean to delegate: an `@mention` starts that agent's session.", new(F.LinearWorkspaceStandardCommunicationAndEvidence, 5))
        ]);

    public static readonly Claim NoBlockedStatus = new(
        "Do not add a Blocked status; it can drift from Linear's dependency graph.",
        [A.AnyAgent],
        [
            new(S.LinearWorkspaceStandard, "A ticket with an open native blocker is blocked in any status; do not add a `Blocked` status that can drift from Linear's dependency graph.", new(F.LinearWorkspaceStandardTicketStatuses, 13))
        ]);

    public static readonly Claim NoSecondStatusBoard = new(
        "Linear stays the live truth, so the repository holds no second status board.",
        [A.ChiefOfStaff],
        [
            new(S.ChiefOfStaff, "Linear stays the live truth, so keep the repository free of a second status board.", new(F.ChiefOfStaffMethod, 15))
        ]);

    public static readonly Claim OnlyCaptainSetsDeliveryStatus = new(
        "Only the ticket-captain sets a delivery ticket's status.",
        [A.TicketCaptain],
        [
            new(S.LinearWorkspaceStandard, "The captain alone sets a delivery ticket's status; the record that runs the chain flips on every chain spawn, and nothing else flips it.", new(F.LinearWorkspaceStandardTicketStatuses, 1), () => [LinearParentToThree.StatusFlipsOnlyOnChainSpawn])
        ]);

    public static readonly Claim OpenBlockerMeansBlocked = new(
        "A ticket with an open native blocker is blocked in any status and cannot be picked up.",
        [A.AnyAgent],
        [
            new(S.LinearWorkspaceStandard, "| `Todo` | unstarted | The incoming list: contracted and to be started soon. An open native blocker still prevents pickup. A `Question` in `Todo` is the user's turn. |", new(F.LinearWorkspaceStandardTicketStatuses, 4), () => [LinearParentToThree.QuestionInTodoIsUserTurn, LinearParentToThree.StatusTodoMeaning])
        ]);
}
