namespace DynaDocs.Tests.SkillBuild.Fixture;

using DynaDocs.SkillBuild;
using A = DynaDocs.Tests.SkillBuild.Fixture.FixtureActor;
using F = DynaDocs.Tests.SkillBuild.Fixture.FixtureSection;
using S = DynaDocs.Tests.SkillBuild.Fixture.FixtureSkill;

/// <summary>Migration fixture: the inventory's <c>contract</c> claims, sub-topics add to map.</summary>
public static class ContractAddToMap
{
    public static readonly Claim AddRemoveScenariosOnlyWhenAsked = new(
        "Add or remove scenarios only when asked.",
        [A.CodeWriter],
        [
            new(S.CodeWriter, "Add or remove scenarios only when asked.", new(F.CodeWriterBuild, 8))
        ]);

    public static readonly Claim AdjacentOutcomeNewTicket = new(
        "An adjacent outcome becomes another ticket, taken only on the user's word.",
        [A.TicketCaptain],
        [
            new(S.TicketCaptain, "An adjacent outcome becomes another ticket, which you take only on the user's word; the current ticket bounds intent and paths.", new(F.TicketCaptainBoundary, 22), () => [ContractMoveToWriter.TicketBoundsIntentAndPaths])
        ]);

    public static readonly Claim BodyBugTicket = new(
        "A Bug ticket body has the five fields, then Observed, Expected and Reproduction (scenario at the boundary, else the red test; prose Bug quotes both passages at file:line; unused member quotes definition and reference search), with default Sub-tickets reproduce-or-identify and fix.",
        [A.TicketCaptain],
        [
            new(S.LinearWorkspaceStandard, "| `Bug` | the five fields, then `## Observed`, `## Expected`, `## Reproduction` (a scenario at the boundary, else the red test; a prose Bug quotes both contradicting passages at `file:line`; an unused member, export or field quotes its definition at `file:line` and a reference search showing no caller outside the tests); default Sub-tickets: *reproduce or identify*, *fix*. |", new(F.LinearWorkspaceStandardTicketTemplates, 4))
        ]);

    public static readonly Claim BodyEnablementTicket = new(
        "An Enablement ticket body has Condition, Steps only the user can do, and Evidence when done.",
        [A.TicketCaptain],
        [
            new(S.LinearWorkspaceStandard, "| `Enablement` | `## Condition` that must become true, `## Steps only the user can do`, `## Evidence` when done. |", new(F.LinearWorkspaceStandardTicketTemplates, 6))
        ]);

    public static readonly Claim BodyFeatureTicket = new(
        "A Feature ticket body has Outcome, Owned paths, Blockers, Exact gates, Base branch and optional Spec; Exact gates names the command for each test, suite and static gate.",
        [A.TicketCaptain],
        [
            new(S.LinearWorkspaceStandard, "| `Feature` | `## Outcome` · `## Owned paths` · `## Blockers` · `## Exact gates` · `## Base branch`, plus optional `## Spec` linking relevant feature-spec sections: the captain's bounded acceptance contract; `## Exact gates` names the command for each test, suite and static gate. |", new(F.LinearWorkspaceStandardTicketTemplates, 3))
        ]);

    public static readonly Claim BodyGrillingTicket = new(
        "A Grilling ticket body has Subject, Tree of choices with answers and reasoning, and Records linked.",
        [A.MapHolder],
        [
            new(S.LinearWorkspaceStandard, "| `Grilling` | `## Subject` (the plan, decision or idea), `## Tree` of choices with their answers and reasoning, `## Records` linked. |", new(F.LinearWorkspaceStandardTicketTemplates, 10))
        ]);

    public static readonly Claim BodyInquisitionTicket = new(
        "An Inquisition ticket body has Scope (commission inputs), Area map drawn fresh each round, Findings and Record path.",
        [A.TicketCaptain],
        [
            new(S.LinearWorkspaceStandard, "| `Inquisition` | `## Scope`: the commission inputs - the audit SHA, the integrated feature commit and the landing base, the governing intent, a provider and model per phase and the swarm width, the Codex cap, the crew context limit, and the verification skill or why nothing runs; `## Area map`, drawn fresh each round; `## Findings`, the register's link and each finding's disposition: Bugs, patches, the nit ticket, Questions; `## Record` path. The round's nit ticket is a `Feature`, Mode `AFK`, `Backlog`, held by the map holder and linked to its Inquisition; it is listed at the Project's final walkthrough, where the user decides each nit, and closes `Done` once every nit is fixed or accepted as is. |", new(F.LinearWorkspaceStandardTicketTemplates, 7), () => [InquisitionGroupToRead.NitTicketClosesWhenEveryNitDecided, InquisitionGroupToRead.NitTicketFinalWalkthrough, InquisitionGroupToRead.NitTicketOpening])
        ]);

    public static readonly Claim BodyMergeTicket = new(
        "A Merge ticket body has Source and Target at their SHAs, Plan order, Combined gates and Conflicts expected.",
        [A.TicketCaptain],
        [
            new(S.LinearWorkspaceStandard, "| `Merge` | `## Source` and `## Target` at their SHAs, `## Plan order` (the Merge Sub-ticket this one is blocked by), `## Combined gates`, `## Conflicts expected`. |", new(F.LinearWorkspaceStandardTicketTemplates, 5))
        ]);

    public static readonly Claim BodyPrototypeTicket = new(
        "A Prototype ticket body has Question, Variants, Verdict and the winning commit on prototype/<name>.",
        [A.TicketCaptain],
        [
            new(S.LinearWorkspaceStandard, "| `Prototype` | `## Question` the sketch must settle, `## Variants`, `## Verdict` and the winning commit on `prototype/<name>`. |", new(F.LinearWorkspaceStandardTicketTemplates, 8))
        ]);

    public static readonly Claim BodyResearchTicket = new(
        "A Research ticket body has Question and Destination for the cited report.",
        [A.MapHolder],
        [
            new(S.LinearWorkspaceStandard, "| `Research` | `## Question` and `## Destination` for the cited report. |", new(F.LinearWorkspaceStandardTicketTemplates, 9))
        ]);

    public static readonly Claim BodyWalkthroughTicket = new(
        "A Walkthrough ticket body has Checkpoint, the four-part tour, Verdict and Findings as linked tickets.",
        [A.MapHolder],
        [
            new(S.LinearWorkspaceStandard, "| `Walkthrough` | `## Checkpoint` (the Milestone or the landing, its branch and SHA), the four-part tour, `## Verdict` (his words, or the skip), `## Findings` as linked tickets. |", new(F.LinearWorkspaceStandardTicketTemplates, 11))
        ]);

    public static readonly Claim BugReproduceOrIdentifyThenFix = new(
        "A Bug normally reproduces or identifies, then fixes.",
        [A.TicketCaptain],
        [
            new(S.TicketCaptain, "A Bug normally reproduces or identifies, then fixes; adopt an inquisition's red-test SHA when one exists, and its prepared patch branch when one is attached: verify and review it rather than rebuilding it.", new(F.TicketCaptainKindsAndFailurePaths, 2), () => [EvidenceBugToPassing.ExistingRedTestIsTheReproduction, InquisitionAreaToFollow.BugCaptainAdoptsPreparedPatch])
        ]);

    public static readonly Claim BugSharedPathsTransferAfterReproduction = new(
        "Shared paths transfer to the fix Sub-ticket only after reproduction closes with its evidence recorded.",
        [A.TicketCaptain],
        [
            new(S.LinearWorkspaceStandard, "The Bug Type template is a narrow exception to parallel delivery lanes: its captain may retain ordered reproduce-or-identify and fix Sub-tickets, with fix natively blocked by reproduction and shared paths transferred only after reproduction closes with its evidence recorded.", new(F.LinearWorkspaceStandardWhoHoldsWhat, 15), () => [ContractAddToMap.BugStagesReproduceThenFix!]),
            new(S.TicketCaptain, "Under the workspace standard's Bug exception, you may retain reproduce-or-identify and fix as direct Bug Sub-tickets: fix is natively blocked by reproduction, both carry the parent's Mode, and any shared paths transfer only after reproduction closes and its evidence is recorded.", new(F.TicketCaptainKindsAndFailurePaths, 4), () => [ContractAddToMap.BugStagesReproduceThenFix!]),
            new(S.WorkingTreeStandard, "Fix is natively blocked by reproduction; shared paths transfer only after the reproduction closes with its evidence recorded.", new(F.WorkingTreeStandardDelegation, 30))
        ]);

    public static readonly Claim BugStageNoOverlappingLanes = new(
        "The Bug stage exception creates no overlapping parallel ownership of paths.",
        [A.TicketCaptain],
        [
            new(S.LinearWorkspaceStandard, "Joining acceptance and final review stay on the parent; the exception creates no overlapping parallel ownership.", new(F.LinearWorkspaceStandardWhoHoldsWhat, 18), () => [ContractMoveToWriter.ParentKeepsSequentialWorkAndFinalReview]),
            new(S.WorkingTreeStandard, "This exception does not permit overlapping parallel lanes.", new(F.WorkingTreeStandardDelegation, 33))
        ]);

    public static readonly Claim BugStageOwnContractChainBranchWorktree = new(
        "Each Bug stage keeps the parent's Type and Mode and has its own contract, chain, branch and worktree.",
        [A.TicketCaptain],
        [
            new(S.LinearWorkspaceStandard, "Each stage keeps the parent's Type and Mode and has its own contract, chain, branch and worktree; every actual integration has a Merge Sub-ticket.", new(F.LinearWorkspaceStandardWhoHoldsWhat, 16)),
            new(S.TicketCaptain, "Give each stage its own contract, chain, branch and worktree; create a Merge Sub-ticket for each actual integration.", new(F.TicketCaptainKindsAndFailurePaths, 5)),
            new(S.WorkingTreeStandard, "Each stage has a contract, branch and worktree; every actual integration has a Merge Sub-ticket.", new(F.WorkingTreeStandardDelegation, 31), () => [MergeAnswerToWake.EveryIntegrationHasMergeSubticket])
        ]);

    public static readonly Claim BugStagesReproduceThenFix = new(
        "Under the Bug exception the captain may retain ordered reproduce-or-identify and fix Sub-tickets, fix natively blocked by reproduction, both carrying the parent's Mode.",
        [A.TicketCaptain],
        [
            new(S.WorkingTreeStandard, "A Bug may retain its Type template's ordered reproduce-or-identify and fix Sub-tickets.", new(F.WorkingTreeStandardDelegation, 29))
        ]);

    public static readonly Claim CollapseSimpleBugPlaceholders = new(
        "Collapse simple Bug template placeholders into parent hops, record why and close the unused records Canceled.",
        [A.TicketCaptain],
        [
            new(S.LinearWorkspaceStandard, "For a simple Bug, collapse the staged placeholders into parent hops and close the unused records `Canceled` with the reason.", new(F.LinearWorkspaceStandardWhoHoldsWhat, 17)),
            new(S.TicketCaptain, "Collapse simple Bug template placeholders into parent hops, recording why and closing the unused records `Canceled`.", new(F.TicketCaptainKindsAndFailurePaths, 3)),
            new(S.WorkingTreeStandard, "A simple Bug collapses the placeholders into parent hops and closes the unused records `Canceled` with the reason.", new(F.WorkingTreeStandardDelegation, 32))
        ]);

    public static readonly Claim ContractEachTicketFromSpecSections = new(
        "Contract each ticket from the spec sections it serves.",
        [A.Admiral],
        [
            new(S.Admiral, "Contract each ticket from the spec sections it serves.", new(F.AdmiralScaffoldProjectLead, 5))
        ]);

    public static readonly Claim ContractNamesLanesOrNone = new(
        "The captain's compact acceptance contract names the lanes, or none.",
        [A.TicketCaptain],
        [
            new(S.WorkingTreeStandard, "| Resolve the work | `ticket-captain` | The captain's compact acceptance contract names the lanes, or none; when the work splits beyond it, the writer's one pre-code comment names them. Parallel crew receive disjoint paths, the ticket's feature files among them, and exact gates; independently trackable parallel lanes become direct Sub-tickets. |", new(F.WorkingTreeStandardWhoDoesWhat, 3), () => [ContractMoveToWriter.WriterPreCodeCommentNamesFurtherLanes])
        ]);

    public static readonly Claim CrewInheritContractAndCommitOwnHop = new(
        "Crew inherit the relevant ticket or Sub-ticket contract, owned paths and gates, and commit their own hop.",
        [A.Crew],
        [
            new(S.WorkingTreeStandard, "Crew inherit the relevant ticket or Sub-ticket contract, owned paths and gates, and commit their own hop.", new(F.WorkingTreeStandardDelegation, 25))
        ]);

    public static readonly Claim DoneDefinition = new(
        "Contract is done when the compact acceptance contract bounds the work exactly enough to build.",
        [A.TicketCaptain],
        [
            new(S.TicketCaptain, "**Done:** the compact acceptance contract bounds the work exactly enough to build.", new(F.TicketCaptainMethod, 12))
        ]);

    public static readonly Claim EnablementUsesWizard = new(
        "Enablement uses wizard for the steps only the user can perform.",
        [A.TicketCaptain, A.AnyAgent],
        [
            new(S.LinearWorkspaceStandard, "| `Enablement` | captain | any | Access, environment, credentials or material other work needs; `wizard` guides the steps only the user can do. | the condition true, with evidence | `#26B5CE` |", new(F.LinearWorkspaceStandardType, 4), () => [LinearTicketToWriter.TypeEnablement]),
            new(S.TicketCaptain, "Enablement uses `wizard` for the steps only the user can perform.", new(F.TicketCaptainKindsAndFailurePaths, 8))
        ]);

    public static readonly Claim ExactGatesNameUnavailableSupport = new(
        "Exact gates name unavailable support.",
        [A.TicketCaptain],
        [
            new(S.LinearWorkspaceStandard, "Name unavailable support and explicitly reconcile older active obligations; no gate is cancelled retrospectively by a new default.", new(F.LinearWorkspaceStandardTicketTemplates, 14), () => [ContractMoveToWriter.NoRetrospectiveGateCancel, ContractMoveToWriter.ReconcileOlderGateContracts])
        ]);

    public static readonly Claim FixHopBlockersAreTheContract = new(
        "On a fix hop the FAIL block's blockers are the contract: close them and nothing else.",
        [A.CodeWriter, A.DocsWriter],
        [
            new(S.CodeWriter, "The FAIL block's blockers are your contract: close them and nothing else.", new(F.CodeWriterWhenYouAreTheFixHop, 1)),
            new(S.DocsWriter, "On a fix hop the FAIL block's blockers are your contract: check each finding's severity against the source, not the reviewer's label, close only the blockers, and put a label you change in your `IMPLEMENTED` behaviour with its evidence (file:line, check, output).", new(F.DocsWriterBoundary, 4), () => [ReviewFixToPrototype.FixHopChecksSeverityAgainstSource, ReviewFixToPrototype.FixHopLabelChangesRecorded])
        ]);

    public static readonly Claim GherkinMarking = new(
        "The contract says whether the ticket carries Gherkin: user journeys yes, backend processes where workable, some code never.",
        [A.TicketCaptain],
        [
            new(S.TicketCaptain, "Say in it whether the ticket carries Gherkin: user journeys yes, backend processes where workable, some code never.", new(F.TicketCaptainMethod, 6))
        ]);

    public static readonly Claim GuardRuleTicketDisclaimer = new(
        "A guard-rule ticket says in its contract and every reviewer brief: guards stop accidents, not rogue agents.",
        [A.TicketCaptain],
        [
            new(S.TicketCaptain, "A guard-rule ticket says in its contract and every reviewer brief: guards stop accidents, not rogue agents.", new(F.TicketCaptainKindsAndFailurePaths, 6))
        ]);

    public static readonly Claim LaneNeedingSplitBecomesSiblings = new(
        "A captain's Sub-tickets are one level deep: a lane needing another split is replaced by sibling lanes under the parent, and the only other permitted children are the Bug stages, Merge, an Inquisition's group Sub-tickets and map-holder-held Sub-tickets.",
        [A.TicketCaptain],
        [
            new(S.LinearWorkspaceStandard, "A captain creates Sub-tickets one level deep: lanes for separate work that can run at the same time, each carrying its parent's Type and Mode, a Merge Sub-ticket for each merge operation except an atomic ticket's own merge into main, which the user clicks once its PR carries a reviewer PASS and CI is green, and a map-holder-held Sub-ticket for fog that blocks only its parent or a lane and stays inside the approved Project destination and ticket outcome.", new(F.LinearWorkspaceStandardWhoHoldsWhat, 12), () => [MergeAnswerToWake.EveryIntegrationHasMergeSubticket, WayfindingAcceptToResolve.CaptainLocalFogLoop]),
            new(S.TicketCaptain, "A lane needing another split becomes siblings; the Bug stages below, Merge, an Inquisition's group Sub-tickets and map-holder-held Sub-tickets are the other permitted children.", new(F.TicketCaptainMethod, 17)),
            new(S.WorkingTreeStandard, "Lanes have one level; the Bug stages below, Merge, an Inquisition's group Sub-tickets and map-holder-held Sub-tickets are the other permitted children.", new(F.WorkingTreeStandardDelegation, 27))
        ]);

    public static readonly Claim LoweredTargetRestored = new(
        "A target lowered for cost goes back up once new evidence makes it reachable.",
        [A.TicketCaptain],
        [
            new(S.TicketCaptain, "A target lowered for cost goes back up once new evidence makes it reachable.", new(F.TicketCaptainMethod, 8))
        ]);

    public static readonly Claim MapHolderWritesContractsOneLevelDown = new(
        "The map holder writes the contracts one level down.",
        [A.MapHolder],
        [
            new(S.LinearWorkspaceStandard, "The map holder writes the contracts one level down, charts with `wayfinder` before dividing, and runs the map-holder-held tickets that clear its own fog: it dispatches `research`, runs a Grilling or Walkthrough with the user, and files a `Question` only when judgment remains.", new(F.LinearWorkspaceStandardWhoHoldsWhat, 2), () => [Identity.MapHolderRunsFogTickets, WayfindingAcceptToResolve.MapHolderCharts])
        ]);
}
