namespace DynaDocs.Tests.SkillBuild.Fixture;

using DynaDocs.SkillBuild;
using A = DynaDocs.Tests.SkillBuild.Fixture.FixtureActor;
using F = DynaDocs.Tests.SkillBuild.Fixture.FixtureSection;
using S = DynaDocs.Tests.SkillBuild.Fixture.FixtureSkill;

/// <summary>Migration fixture: the inventory's <c>linear</c> claims, sub-topics parent to three.</summary>
public static class LinearParentToThree
{
    public static readonly Claim ParentStaysReadyToMerge = new(
        "The parent stays Ready to Merge while its own Merge Sub-ticket runs.",
        [A.TicketCaptain],
        [
            new(S.LinearWorkspaceStandard, "| `Ready to Merge` | started | The PR carries its PASS block and waits for its merge turn. The record stays here while its own Merge Sub-ticket runs; the landing, one Project at a time, and an atomic ticket wait here for the user's click. A merge review FAIL that reverts sends it back to `Implementing`. |", new(F.LinearWorkspaceStandardTicketStatuses, 8), () => [LinearParentToThree.ReadyToMergeMeaning!, MergeAnswerToWake.LandingUserClick, ReviewFixToPrototype.MergeFailRouting])
        ]);

    public static readonly Claim PriorityHigh = new(
        "Priority High is set when clearing the ticket lets a top-level ticket, or a Project's next step, run AFK.",
        [A.MapHolder],
        [
            new(S.LinearWorkspaceStandard, "| `High` | Clearing it lets a top-level ticket, or a Project's next step, run AFK. |", new(F.LinearWorkspaceStandardPriority, 3))
        ]);

    public static readonly Claim PriorityLow = new(
        "Priority Low is set when nothing runs AFK on clearing but the user must clear it anyway.",
        [A.MapHolder],
        [
            new(S.LinearWorkspaceStandard, "| `Low` | Nothing runs AFK when it clears; the user must clear it anyway. |", new(F.LinearWorkspaceStandardPriority, 5))
        ]);

    public static readonly Claim PriorityMedium = new(
        "Priority Medium is set when clearing the ticket lets a lane run AFK.",
        [A.MapHolder],
        [
            new(S.LinearWorkspaceStandard, "| `Medium` | Clearing it lets a lane run AFK. |", new(F.LinearWorkspaceStandardPriority, 4))
        ]);

    public static readonly Claim PriorityNeverSeverity = new(
        "Priority keeps its meaning and never carries severity.",
        [A.AnyAgent],
        [
            new(S.LinearWorkspaceStandard, "Priority keeps its meaning and never carries severity.", new(F.LinearWorkspaceStandardSeverity, 6))
        ]);

    public static readonly Claim PriorityUrgent = new(
        "Priority Urgent means an emergency and is never set by default.",
        [A.MapHolder],
        [
            new(S.LinearWorkspaceStandard, "| `Urgent` | An emergency; never set by default. |", new(F.LinearWorkspaceStandardPriority, 2))
        ]);

    public static readonly Claim ProjectStatusBacklog = new(
        "Project status Backlog means a possible Project retained for later that no one has started charting.",
        [A.MapHolder],
        [
            new(S.LinearWorkspaceStandard, "| `Backlog` | A possible Project retained for later; no one has started charting it. |", new(F.LinearWorkspaceStandardProjectStatuses, 1))
        ]);

    public static readonly Claim ProjectStatusInProgress = new(
        "Project status In Progress means the map holder is working the map through Ticket Captains toward the destination.",
        [A.MapHolder],
        [
            new(S.LinearWorkspaceStandard, "| `In Progress` | The map holder is working the map through Ticket Captains toward the destination. |", new(F.LinearWorkspaceStandardProjectStatuses, 4))
        ]);

    public static readonly Claim ProjectStatusPlanned = new(
        "Project status Planned means the user approved the charted route and any repository plan passed independent review.",
        [A.MapHolder],
        [
            new(S.LinearWorkspaceStandard, "| `Planned` | The user approved the charted route, and any repository plan passed independent review; the map holder commissions the first Ticket Captain to open the feature. |", new(F.LinearWorkspaceStandardProjectStatuses, 3), () => [ProjectAdmiralToLedger.FirstCaptainOpensFeatureBranch])
        ]);

    public static readonly Claim ProjectStatusPlanning = new(
        "Project status Planning means the map holder is charting the first low-resolution map with wayfinder and any repository plan runs its review loop.",
        [A.MapHolder],
        [
            new(S.LinearWorkspaceStandard, "| `Planning` | The map holder is charting the first low-resolution map with `wayfinder`; a repository plan, when one is written, runs its review loop. |", new(F.LinearWorkspaceStandardProjectStatuses, 2))
        ]);

    public static readonly Claim ProjectsCarryNoLabels = new(
        "A Project carries no canonical labels; its status and ticket graph carry the information.",
        [A.AnyAgent],
        [
            new(S.LinearWorkspaceStandard, "There are no canonical Project labels: a Project's status and ticket graph carry the information.", new(F.LinearWorkspaceStandardProjectStatuses, 7))
        ]);

    public static readonly Claim QuestionInTodoIsUserTurn = new(
        "A Question in Todo carries judgment the user must supply and is the user's turn.",
        [A.TicketCaptain, A.User],
        [
            new(S.TicketCaptain, "A spawned captain returns to its spawner; a Question in `Todo` carries judgment the user must supply.", new(F.TicketCaptainBoundary, 28), () => [ReturnAdmiralToNo.CaptainReturnsOneLineToSpawner])
        ]);

    public static readonly Claim RaiseConflictFoundInRecord = new(
        "Raise a conflict you find in the record.",
        [A.TicketCaptain, A.AnyAgent], []);

    public static readonly Claim ReadCommentsOnlyForNamedFact = new(
        "Read comments only for a named missing fact or binding review; comments have no assumed order.",
        [A.AnyAgent],
        [
            new(S.LinearWorkspaceStandard, "Read comments only for a named missing fact or binding review; comments have no assumed order.", new(F.LinearWorkspaceStandardCommunicationAndEvidence, 3))
        ]);

    public static readonly Claim ReadMetadataFirst = new(
        "Use metadata first: read only the current active contract and named evidence.",
        [A.AnyAgent, A.Admiral],
        [
            new(S.Admiral, "The Linear Project's metadata-first map, each current active contract, and named evidence only.", new(F.AdmiralMustReads, 1)),
            new(S.LinearWorkspaceStandard, "Use metadata first: the current active contract and named evidence only.", new(F.LinearWorkspaceStandardCommunicationAndEvidence, 1))
        ]);

    public static readonly Claim ReadyToMergeMeaning = new(
        "Ready to Merge means the PR carries its PASS block and waits for its merge turn.",
        [A.TicketCaptain], []);

    public static readonly Claim ReadyToMergeScope = new(
        "Ready to Merge holds for a lane into its parent, a primary into the feature branch, and an atomic ticket or the landing into main.",
        [A.TicketCaptain], []);

    public static readonly Claim RecordIsLiveTruth = new(
        "The Linear record is the default truth for live work, and its latest word wins.",
        [A.AnyAgent],
        [
            new(S.Admiral, "**Truth:** the Linear record is the default truth for live work, and its latest word wins.", new(F.AdmiralBoundary, 24)),
            new(S.TicketCaptain, "**Truth:** the Linear record is the default truth for live work, and its latest word wins.", new(F.TicketCaptainBoundary, 30))
        ]);

    public static readonly Claim ReviewBlockRecordedOnJudgedWork = new(
        "The invoker records the review block on the work judged: a Project update for a project-plan review, the Merge Ticket for a merge review, otherwise the ticket.",
        [A.TicketCaptain, A.Admiral, A.Reviewer],
        [
            new(S.Admiral, "The block is a Project update.", new(F.AdmiralMethod, 16)),
            new(S.Reviewer, "The invoker records it on the work judged: a Project update for a project-plan review, the Merge Ticket for merge review, otherwise the ticket; the PR body carries it under `## Independent review` when a PR exists.", new(F.ReviewerReturn, 2), () => [MergeAnswerToWake.OpenPrWithPassBlock])
        ]);

    public static readonly Claim ReviewerBlockIsSignature = new(
        "The reviewer field in a reviewer block is its signature; no second or conflicting one is added.",
        [A.Reviewer],
        [
            new(S.LinearWorkspaceStandard, "The reviewer field in a reviewer block is its signature, so do not add a second or conflicting one.", new(F.LinearWorkspaceStandardTruthfulCommentSignatures, 6))
        ]);

    public static readonly Claim SameTypesStatusesChainBothLevels = new(
        "The same Types, statuses and chain hold for Project-level and delivery-level contracts.",
        [A.MapHolder, A.TicketCaptain],
        [
            new(S.Wayfinder, "The same Types, statuses and chain hold.", new(F.WayfinderTickets, 8))
        ]);

    public static readonly Claim SetImplementingOnAuthorSpawn = new(
        "The captain sets Implementing when the author of the change (code-writer or docs-writer), including a fix hop after a FAIL, is spawned, and after a spec review of either verdict.",
        [A.TicketCaptain],
        [
            new(S.LinearWorkspaceStandard, "| `Implementing` | started | The author of the change's kind is spawned for one hop - a `code-writer`, or a `docs-writer` for a documentation change - a fix hop after a FAIL included. |", new(F.LinearWorkspaceStandardTicketStatuses, 6)),
            new(S.TicketCaptain, "When one risk buys a spec review, record that risk in the contract, brief a fresh `reviewer(spec)` on the contract text and set `In Review`; either verdict sets `Implementing`.", new(F.TicketCaptainMethod, 7), () => [LinearParentToThree.SetInReviewOnReviewerSpawn!, ReviewReadToWrong.SpecReviewOnlyForRecordedRisk, ReviewReadToWrong.SpecReviewerBriefedOnContractText]),
            new(S.WorkingTreeStandard, "| Claim the ticket | `ticket-captain` | Ticket is assigned; its branch and isolated worktree exist; branch, base SHA and worktree path are on the ticket. The captain sets `Implementing` when it spawns the author, a code-writer or a docs-writer. |", new(F.WorkingTreeStandardWhoDoesWhat, 2))
        ]);

    public static readonly Claim SetInReviewOnReviewerSpawn = new(
        "In Review is set when any reviewer is spawned, and In Review on the board always means a reviewer is running.",
        [A.TicketCaptain, A.AnyAgent],
        [
            new(S.LinearWorkspaceStandard, "| `In Review` | started | Any reviewer is spawned, the optional spec review of the contract before any code included. A FAIL, one or more blockers, returns the record to `Implementing`; a spec review returns it there on either verdict. |", new(F.LinearWorkspaceStandardTicketStatuses, 7), () => [ReviewAnswerToFinding.FailBlockersBindingToImplementing])
        ]);

    public static readonly Claim SeverityBlockerCarriers = new(
        "Severity blocker (the intended case is wrong) is carried by each blocker Bug an inquisition files.",
        [A.AnyAgent],
        [
            new(S.LinearWorkspaceStandard, "| `blocker` | The intended case is wrong. | each blocker Bug an inquisition files | `#E5484D` |", new(F.LinearWorkspaceStandardSeverity, 2))
        ]);

    public static readonly Claim SeverityNitCarriers = new(
        "Severity nit (everything else) is carried by each inquisition round's nit ticket.",
        [A.AnyAgent],
        [
            new(S.LinearWorkspaceStandard, "| `nit` | Everything else. | each inquisition round's nit ticket | `#8B8D98` |", new(F.LinearWorkspaceStandardSeverity, 4))
        ]);

    public static readonly Claim SeverityOnlyOnFindingTickets = new(
        "Severity sits only on a ticket born from a finding; most tickets carry none.",
        [A.AnyAgent],
        [
            new(S.LinearWorkspaceStandard, "Severity sits only on a ticket born from a finding.", new(F.LinearWorkspaceStandardTicketLabels, 3))
        ]);

    public static readonly Claim SeverityShouldCarriers = new(
        "Severity should (a real bug callers will hit, inside the intent) is carried by each should ticket filed from a review and each should Bug an inquisition files.",
        [A.AnyAgent],
        [
            new(S.LinearWorkspaceStandard, "| `should` | A real bug callers will hit, inside the intent. | each should ticket filed from a review; each should Bug an inquisition files | `#FFB224` |", new(F.LinearWorkspaceStandardSeverity, 3))
        ]);

    public static readonly Claim SmallWorkStaysOnTicket = new(
        "Small lookups, conversations and setup stay on the ticket that needs them; a map-holder-held ticket is created only when the work needs independent tracking or blocks other work.",
        [A.AnyAgent, A.MapHolder, A.Admiral],
        [
            new(S.Admiral, "Do small discovery inline; create a Wayfinding Ticket when the investigation needs its own status, owner, blocker, or evidence.", new(F.AdmiralMethod, 48)),
            new(S.LinearWorkspaceStandard, "Small lookups, conversations and setup stay on the ticket that needs them.", new(F.LinearWorkspaceStandardType, 17))
        ]);

    public static readonly Claim StateEffortOnlyWhenKnown = new(
        "State effort only when it is known and useful.",
        [A.AnyAgent],
        [
            new(S.LinearWorkspaceStandard, "State effort only when it is known and useful.", new(F.LinearWorkspaceStandardTruthfulCommentSignatures, 4))
        ]);

    public static readonly Claim StatusBacklogMeaning = new(
        "Status Backlog holds a ticket retained with a Type, unscheduled and waiting to become Todo.",
        [A.AnyAgent],
        [
            new(S.LinearWorkspaceStandard, "| `Backlog` | backlog | Retained with a Type, unscheduled, waiting to become a Todo: no contract yet, or one awaiting the user's go, as an Inquisition's. |", new(F.LinearWorkspaceStandardTicketStatuses, 3))
        ]);

    public static readonly Claim StatusCanceledMeaning = new(
        "Status Canceled means the ticket will not be completed; the record says why.",
        [A.AnyAgent],
        [
            new(S.LinearWorkspaceStandard, "| `Canceled` | canceled | The ticket will not be completed; the record says why. |", new(F.LinearWorkspaceStandardTicketStatuses, 10))
        ]);

    public static readonly Claim StatusDoneMeaning = new(
        "Status Done means merged, or the outcome the Type names reached, with its evidence.",
        [A.AnyAgent],
        [
            new(S.LinearWorkspaceStandard, "| `Done` | completed | Merged, or the outcome the Type names reached, with its evidence. |", new(F.LinearWorkspaceStandardTicketStatuses, 9))
        ]);

    public static readonly Claim StatusDuplicateMeaning = new(
        "Status Duplicate means another ticket owns the outcome; the record links to it.",
        [A.AnyAgent],
        [
            new(S.LinearWorkspaceStandard, "| `Duplicate` | canceled | Another ticket owns the outcome; the record links to it. |", new(F.LinearWorkspaceStandardTicketStatuses, 11))
        ]);

    public static readonly Claim StatusFlipsOnlyOnChainSpawn = new(
        "A delivery ticket's status flips on every chain spawn of the record that runs the chain, and nothing else flips it.",
        [A.AnyAgent, A.TicketCaptain], []);

    public static readonly Claim StatusFuturefeatureMeaning = new(
        "Status FutureFeature is an unscheduled strategic possibility with no Type yet.",
        [A.AnyAgent], []);

    public static readonly Claim StatusPathCaptainDefault = new(
        "A captain-held ticket's default status path, subject to Type exceptions, is Todo, Implementing, In Review, Ready to Merge, Done, with In Progress while lanes run.",
        [A.TicketCaptain],
        [
            new(S.LinearWorkspaceStandard, "| captain-held default, subject to the Type exceptions | `Todo` → `Implementing` → `In Review` → `Ready to Merge` → `Done`, with `In Progress` while lanes run |", new(F.LinearWorkspaceStandardWhoHoldsWhat, 7))
        ]);

    public static readonly Claim StatusPathGrillingWalkthrough = new(
        "A Grilling or Walkthrough ticket's normal status path is Todo, In Progress, Done.",
        [A.MapHolder],
        [
            new(S.LinearWorkspaceStandard, "| `Grilling`, `Walkthrough` | `Todo` → `In Progress` → `Done` |", new(F.LinearWorkspaceStandardWhoHoldsWhat, 4))
        ]);

    public static readonly Claim StatusPathQuestion = new(
        "A Question ticket's normal status path is Todo, Done.",
        [A.MapHolder],
        [
            new(S.LinearWorkspaceStandard, "| `Question` | `Todo` → `Done` |", new(F.LinearWorkspaceStandardWhoHoldsWhat, 5))
        ]);

    public static readonly Claim StatusPathResearch = new(
        "A Research ticket's normal status path is Todo, In Progress, Done.",
        [A.MapHolder],
        [
            new(S.LinearWorkspaceStandard, "| `Research` | `Todo` → `In Progress` → `Done` |", new(F.LinearWorkspaceStandardWhoHoldsWhat, 3))
        ]);

    public static readonly Claim StatusSetOrder = new(
        "Primary tickets and Sub-tickets share one set of ten statuses, kept in the listed order within Linear's categories.",
        [A.AnyAgent],
        [
            new(S.LinearWorkspaceStandard, "The order of the statuses is part of the standard: Linear draws each status circle from its position in its category.", new(F.LinearWorkspaceStandardLead, 1))
        ]);

    public static readonly Claim StatusTodoMeaning = new(
        "Status Todo is the incoming list of contracted tickets to be started soon.",
        [A.AnyAgent], []);

    public static readonly Claim TaskNamesRoleNotLabel = new(
        "Task names the captain-held ticket's role on a map; it is not a label.",
        [A.AnyAgent],
        [
            new(S.LinearWorkspaceStandard, "`Task` names the captain-held ticket's role on a map; it is not a label.", new(F.LinearWorkspaceStandardType, 16))
        ]);

    public static readonly Claim ThreeLabelGroups = new(
        "Tickets use three label groups, Type, Mode and Severity, one label per group.",
        [A.AnyAgent],
        [
            new(S.LinearWorkspaceStandard, "Three label groups, `Type`, `Mode` and `Severity`, because one label per group is the rule Linear enforces.", new(F.LinearWorkspaceStandardTicketLabels, 1))
        ]);
}
