namespace DynaDocs.Tests.SkillBuild.Fixture.Kinds;

using DynaDocs.SkillBuild;
using A = DynaDocs.Tests.SkillBuild.Fixture.FixtureActor;
using F = DynaDocs.Tests.SkillBuild.Fixture.FixtureSection;
using S = DynaDocs.Tests.SkillBuild.Fixture.FixtureSkill;

/// <summary>Migration fixture: the inventory's <c>review</c> claims, sub-topics fix to prototype.</summary>
public static class ReviewFixToPrototype
{
    public static readonly Claim FixAtNewCommitAndFreshReview = new(
        "Each fix of a FAIL's blockers lands at its own new commit and is reviewed afresh.",
        [A.TicketCaptain, A.Admiral],
        [
            new(S.Admiral, "Correct a FAIL's blockers at a new commit and review afresh; the second FAIL goes to the user as the choice.", new(F.AdmiralMethod, 17), () => [ReviewFixToPrototype.PlanSecondFailToUser!])
        ]);

    public static readonly Claim FixHopChecksSeverityAgainstSource = new(
        "On a fix hop check each finding's severity against the source, not the reviewer's label.",
        [A.CodeWriter, A.DocsWriter],
        [
            new(S.CodeWriter, "Check each finding's severity against the source, not the reviewer's label; a label you change goes in your `IMPLEMENTED` behaviour with its evidence (file:line, test, output).", new(F.CodeWriterWhenYouAreTheFixHop, 2), () => [ReviewFixToPrototype.FixHopLabelChangesRecorded!])
        ]);

    public static readonly Claim FixHopFilesNoShouldsOrNits = new(
        "Shoulds and nits are the captain's to file, not the fix hop's.",
        [A.TicketCaptain],
        [
            new(S.CodeWriter, "Shoulds and nits are the captain's to file.", new(F.CodeWriterWhenYouAreTheFixHop, 3))
        ]);

    public static readonly Claim FixHopLabelChangesRecorded = new(
        "When the fix-hop author changes a finding's severity label after checking it against the source, the change and its reason or evidence is reported and recorded.",
        [A.TicketCaptain, A.CodeWriter, A.DocsWriter],
        [
            new(S.TicketCaptain, "Record every label the fix-hop author changed, with its reason; a should-ticket's own author may close it with a reason.", new(F.TicketCaptainMethod, 38), () => [ReviewReadToWrong.ShouldAuthorMayClose])
        ]);

    public static readonly Claim FixHopPrefersNarrowing = new(
        "A fix hop prefers the change that deletes or narrows code; review never grows the PR.",
        [A.CodeWriter, A.DocsWriter],
        [
            new(S.CodeWriter, "Prefer the change that deletes or narrows code; review never grows the PR.", new(F.CodeWriterWhenYouAreTheFixHop, 4)),
            new(S.DocsWriter, "Prefer the edit that deletes or narrows; review never grows the change.", new(F.DocsWriterBoundary, 5))
        ]);

    public static readonly Claim FullStrengthWhateverCost = new(
        "Reviews and gates keep full strength whatever the work below or the review costs.",
        [A.TicketCaptain, A.Admiral],
        [
            new(S.TicketCaptain, "Reviews and gates keep the full strength the consequence of error deserves, whatever the work below cost.", new(F.TicketCaptainMethod, 40))
        ]);

    public static readonly Claim GateSetOnExactCandidate = new(
        "At a gate (final gates, a merge, the landing) the whole gate set applies on that exact candidate, read from its gate record or run by the reviewer.",
        [A.Reviewer],
        [
            new(S.Reviewer, "At a gate - the ticket's final gates, a merge, the landing - the whole set below on that exact candidate, read from its gate record or run by you: The ticket's final-stage commands: declared own-feature Heavy, measuring every Heavy-only source owed to it or failing; coverage producer/consumer at the scope Feature proof and gate placement in `dydo/guides/testing-strategy.md` sets for that gate (ticket-scoped at final gates and a Merge Sub-ticket, whole-stack at the landing and a default-branch sync, whole-stack per ticket where the facade records no feature selection), a changed line it misses failing; any mandatory integration/release suites; unresolved required failures remain failures, including inherited ones Coverage, HCRAP and the one-level static policy in the project's testing guide Mutation on the changed files, no survivor; one example value changed per scenario, none left green `dydo check` when the change touches documentation or validation surfaces", new(F.ReviewerCodeGates, 5))
        ]);

    public static readonly Claim LaterRoundsReviewerChoice = new(
        "Later review rounds take the model and vendor the captain chooses, cross-vendor preferred.",
        [A.TicketCaptain],
        [
            new(S.TicketCaptain, "A candidate's first round seats two in parallel, one Claude and one Codex, their findings merged into one deduplicated block that FAILs if either found a blocker; later rounds take the model and vendor you choose, cross-vendor preferred.", new(F.TicketCaptainMethod, 39), () => [ReviewReadToWrong.TwoReviewersFirstRound])
        ]);

    public static readonly Claim MergeFailRouting = new(
        "A merge review FAIL is owned: an integration defect is fixed inside the Merge Ticket then re-reviewed; a source-work defect is reverted inside Merge, the Merge ticket closed Canceled with the reason, and the source ticket returned to Implementing.",
        [A.TicketCaptain],
        [
            new(S.TicketCaptain, "Merge review FAIL has an owner: fix an integration defect inside the Merge Ticket, then re-review.", new(F.TicketCaptainKindsAndFailurePaths, 14)),
            new(S.WorkingTreeStandard, "Merge FAIL, a blocker, stays owned: fix integration defects inside Merge; revert a source defect there and close Merge `Canceled`, returning the source to `Implementing`.", new(F.WorkingTreeStandardReturnAndRelease, 10))
        ]);

    public static readonly Claim MergeReviewAfterMerge = new(
        "Only after the merge is made does a fresh reviewer(merge) judge the integrated candidate.",
        [A.TicketCaptain, A.Reviewer],
        [
            new(S.TicketCaptain, "Only then does a fresh `reviewer(merge)` judge the integrated parent.", new(F.TicketCaptainMethod, 49))
        ]);

    public static readonly Claim MergeReviewArtifactSweep = new(
        "The merge reviewer sweeps for merge artifacts: conflict markers, duplicated or vanished hunks, a neighbour's change reverted, build products or local files committed.",
        [A.Reviewer],
        [
            new(S.Reviewer, "**Sweep for merge artifacts.** Conflict markers, hunks that arrived twice, hunks that disappeared in resolution, a neighbour's change reverted by the resolution, build products or local files committed by accident. Grep for the markers yourself; a clean build hides all of this. Done when each has been searched for.", new(F.ReviewerMergeMethod, 2))
        ]);

    public static readonly Claim MergeReviewFixTheUnit = new(
        "The merge reviewer names the merge commit, both parents and the tickets that landed in it.",
        [A.Reviewer],
        [
            new(S.Reviewer, "**Fix the unit.** Name the merge commit, both parents and the tickets that landed in it; diff the merged tree against the base the branch grew from, since each ticket's own review already covered its branch diff. Done when every landed ticket is named.", new(F.ReviewerMergeMethod, 1))
        ]);

    public static readonly Claim MergeReviewGatesOnMergeCommit = new(
        "Each landed ticket's exact gate commands have run once on this exact merge commit, verified from a gate record naming it or run in the merged tree.",
        [A.CodeWriter, A.Reviewer],
        [
            new(S.CodeWriter, "Run the combined gates on the merge SHA, the merge review's candidate.", new(F.CodeWriterMergeLead, 6)),
            new(S.Reviewer, "**Put the gates on the integrated state.** Each landed ticket's exact gate commands have run once on this exact merge commit: where a gate record names that commit you verify it, and where none does you run them yourself in the merged tree. A green from any other tree - an isolated worktree, either parent - proves nothing here. Done when every gate has its command and result recorded.", new(F.ReviewerMergeMethod, 4))
        ]);

    public static readonly Claim MergeReviewNotSubstitutable = new(
        "No review at another level substitutes for the merge review.",
        [A.Reviewer], []);

    public static readonly Claim MergeReviewReadScale = new(
        "The merge reviewer reads the integrated state at the scale of what landed.",
        [A.Reviewer],
        [
            new(S.Reviewer, "Read the integrated state at the scale of what landed: a two-file merge is minutes of mechanical checking, a whole feature is not.", new(F.ReviewerMergeLead, 1))
        ]);

    public static readonly Claim MergeReviewReadsRecord = new(
        "The merge reviewer reads the Merge Ticket record's pinned source, target and combined gates.",
        [A.Reviewer],
        [
            new(S.Reviewer, "Read that record's pinned source, target and combined gates.", new(F.ReviewerMergeLead, 4))
        ]);

    public static readonly Claim MergeReviewScope = new(
        "Merge review scope is what this merge created or exposed.",
        [A.Reviewer],
        [
            new(S.Reviewer, "Scope is what this merge created or exposed.", new(F.ReviewerMergeLead, 2))
        ]);

    public static readonly Claim MergeReviewWalkSeams = new(
        "The merge reviewer walks every file, symbol and behaviour two merged tickets both touch.",
        [A.Reviewer],
        [
            new(S.Reviewer, "**Walk the seams.** Every file, symbol and behaviour two merged tickets both touch: a caller left on the old contract, a name that moved under someone else, two implementations of one rule, an assumption one ticket holds and another broke. Done when every shared file, symbol and behaviour is checked.", new(F.ReviewerMergeMethod, 3))
        ]);

    public static readonly Claim NeverSelfReview = new(
        "An author never serves as the independent review of its own candidate or delivery; a successful delivery is not an independent PASS.",
        [A.TicketCaptain, A.DocsWriter, A.AnyAgent, A.Admiral],
        [
            new(S.DocsWriter, "A writer's successful delivery is never an independent `PASS`.", new(F.DocsWriterReturn, 2)),
            new(S.LinearWorkspaceStandard, "A writer's successful delivery is never an independent `PASS`.", new(F.LinearWorkspaceStandardCommunicationAndEvidence, 24))
        ]);

    public static readonly Claim NitOneLineOnRecord = new(
        "Each review nit is recorded as one line on the ticket or Project update record.",
        [A.TicketCaptain, A.Admiral, A.AnyAgent],
        [
            new(S.Admiral, "File each `should` as its own `Backlog` ticket with Severity `should` in the Project and record each `nit` as one line on the Project update.", new(F.AdmiralMethod, 18), () => [ReviewReadToWrong.ShouldFiledAsOwnTicket])
        ]);

    public static readonly Claim NoPerHopCoverageRun = new(
        "Review adds no per-hop coverage run; ordinary hop static checks remain required.",
        [A.Reviewer],
        [
            new(S.Reviewer, "This adds no per-hop coverage run; ordinary hop static checks remain required", new(F.ReviewerCodeGates, 3))
        ]);

    public static readonly Claim ObservationOutOfScope = new(
        "A defect the candidate neither created nor exposed is one line after the block: `Observation (out of scope, non-binding):`.",
        [A.Reviewer],
        [
            new(S.Reviewer, "A defect the candidate neither created nor exposed is one line after the block, `Observation (out of scope, non-binding):`.", new(F.ReviewerReturn, 6))
        ]);

    public static readonly Claim ParallelReviewersSeparateWorktrees = new(
        "Parallel reviewers sit in separate worktrees when either may run probes.",
        [A.TicketCaptain],
        [
            new(S.TicketCaptain, "Seat parallel reviewers in separate worktrees when either may run probes.", new(F.TicketCaptainMethod, 41))
        ]);

    public static readonly Claim PassMeansNoBlockerListsDeferred = new(
        "A PASS carries no blocker and lists its should and nit findings under Deferred.",
        [A.Reviewer],
        [
            new(S.Reviewer, "PASS means no blocker, and binds this candidate under this contract; it lists its should and nit findings in the form's `Deferred` tail.", new(F.ReviewerReturn, 4))
        ]);

    public static readonly Claim PlanCheckShape = new(
        "The plan reviewer checks the shape of the plan.",
        [A.Reviewer],
        [
            new(S.Reviewer, "**Check the shape.** Frontmatter carries `title`, `status`, `area`, `type` and `linear-project`; six numbered sections stand in order: 1 Specification (intent, in scope, out of scope, acceptance criteria, questions and answers), 2 Prior art, 3 Design, 4 Implementation ticket map (First pickable tickets, Later bearings, Exact gates), 5 Ordering and isolation, 6 Watch-outs; any Not yet specified fog closes it; `dydo check` passes. Done when every required element exists or is one finding.", new(F.ReviewerProjectPlanMethod, 1))
        ]);

    public static readonly Claim PlanFogIsNotAGap = new(
        "Wayfinding fog is not a gap: dimmer uncertainty stays in Not yet specified, and a plan that pretends either is settled FAILs while honest placement passes.",
        [A.Reviewer],
        [
            new(S.Reviewer, "**Wayfinding fog is not a gap.** Dimmer uncertainty stays in `## Not yet specified`. A plan that pretends either is settled FAILs; honest placement passes.", new(F.ReviewerProjectPlanMethod, 6))
        ]);

    public static readonly Claim PlanInspectStartingRoute = new(
        "Each first ticket is self-contained and vertical, carries outcome, owned paths, blockers, gate and base branch, exactly one Type, one Mode and Todo; its gate is copy-pasteable.",
        [A.Reviewer],
        [
            new(S.Reviewer, "**Inspect the starting route.** Each first ticket is self-contained and vertical, carries outcome, owned paths, blockers, gate and base branch, exactly one Type, one Mode and `Todo`; its gate is copy-pasteable. Check status and priority against `dydo/reference/linear-workspace-standard.md`, read from the repository root. Owned paths isolate parallel work, hot paths are serial, and the first merge order is credible. Done when every listed ticket is safe for a Ticket Captain to claim.", new(F.ReviewerProjectPlanMethod, 4))
        ]);

    public static readonly Claim PlanPassMeaning = new(
        "A project-plan PASS means the plan can reach human approval and open the first work; it certifies a starting route, not a complete one.",
        [A.Reviewer],
        [
            new(S.Reviewer, "PASS means the plan can reach human approval and open the first work; it certifies a starting route, not a complete one.", new(F.ReviewerProjectPlanLead, 2))
        ]);

    public static readonly Claim PlanRespectHorizon = new(
        "The plan reviewer respects the horizon: later bearings orient without speculative precision, every in-scope outcome (the durable knowledge and integration the destination needs included) is a first ticket, a later bearing, honest fog or a blocking Question Ticket, and the step is done when the map starts the journey without hiding or inventing what comes later.",
        [A.Reviewer],
        [
            new(S.Reviewer, "**Respect the horizon.** Later bearings orient without speculative precision, and every in-scope outcome, the durable knowledge and the integration the destination needs among them, is a first ticket, a later bearing, honest fog or a blocking Question Ticket. Done when the map starts the journey without hiding or inventing what comes later.", new(F.ReviewerProjectPlanMethod, 5))
        ]);

    public static readonly Claim PlanReviewTarget = new(
        "The project-plan review targets one committed Project plan the map holder wrote, judged in the given shape at the plan's candidate commit.",
        [A.Reviewer],
        [
            new(S.Reviewer, "Target: one committed Project plan the map holder wrote for a cross-cutting architecture plan, proposed for human approval, or an amendment to it, judged in the shape below at the plan's candidate commit.", new(F.ReviewerProjectPlanLead, 1))
        ]);

    public static readonly Claim PlanSecondFailToUser = new(
        "The second FAIL of a plan review goes to the user as a choice, owned by the map holder's review loop.",
        [A.Admiral, A.MapHolder],
        [
            new(S.Reviewer, "The map holder owns the review loop and presents approval to the user; the second FAIL is his choice.", new(F.ReviewerProjectPlanMethod, 8))
        ]);

    public static readonly Claim PlanTestDestination = new(
        "Plan intent, scope, acceptance and governing design agree, and each acceptance criterion names the scenario, command, diff or artifact proving it at the final merge.",
        [A.Reviewer],
        [
            new(S.Reviewer, "**Test the destination.** Intent, scope, acceptance and governing design agree, and each acceptance criterion names the scenario, command, diff or artifact that proves it at the final merge. Done when the user knows what approval fixes.", new(F.ReviewerProjectPlanMethod, 3))
        ]);

    public static readonly Claim PlanVerifyGround = new(
        "The plan reviewer reads every cited Decision, path, pattern and specification at the candidate commit.",
        [A.Reviewer],
        [
            new(S.Reviewer, "**Verify the ground.** Read every cited Decision, path, pattern and specification at the candidate commit. Done when every material claim matches its source or is one finding.", new(F.ReviewerProjectPlanMethod, 2))
        ]);

    public static readonly Claim PrototypeReviewIsUserVerdict = new(
        "The user's verdict is a prototype's review.",
        [A.User],
        [
            new(S.CodeWriter, "The user's verdict is its review.", new(F.CodeWriterBugOrFeature, 8))
        ]);
}
