namespace DynaDocs.Tests.SkillBuild.Fixture;

using DynaDocs.SkillBuild;
using A = DynaDocs.Tests.SkillBuild.Fixture.FixtureActor;
using F = DynaDocs.Tests.SkillBuild.Fixture.FixtureSection;
using S = DynaDocs.Tests.SkillBuild.Fixture.FixtureSkill;

/// <summary>Migration fixture: the inventory's <c>merge</c> claims, sub-topics answer to wake.</summary>
public static class MergeAnswerToWake
{
    public static readonly Claim AnswerDismissalWithReason = new(
        "Answer each dismissal with a written reason.",
        [A.TicketCaptain],
        [
            new(S.TicketCaptain, "A wake is news, not a merge decision: act only on checks and comments newer than the latest push, verify each bot finding against the source and grade it blocker, should or nit as in Review, refresh the body's `**Deferred:**` line, tell repo failures from infra flakes, answer each dismissal with a written reason, and stay silent when nothing changed.", new(F.TicketCaptainMethod, 57), () => [MergeAnswerToWake.RefreshDeferredLine!, MergeAnswerToWake.RepoFailureVsInfraFlake!, MergeAnswerToWake.StaySilentWhenUnchanged!, MergeAnswerToWake.VerifyBotFinding!, MergeAnswerToWake.WakeIsNewsNotDecision!])
        ]);

    public static readonly Claim AtomicTicketNoMergeSubticket = new(
        "An atomic ticket has no Merge Sub-ticket: its PR into main carries the reviewer PASS, the captain confirms CI green and sets Ready to Merge, the user clicks the merge, then the captain closes it Done, cleans up and resumes from the record.",
        [A.TicketCaptain, A.User],
        [
            new(S.TicketCaptain, "An atomic ticket has no Merge Sub-ticket: the user clicks its PR, then you resume from the record.", new(F.TicketCaptainMethod, 64)),
            new(S.WorkingTreeStandard, "| Atomic ticket | `main` | `DYD-123-<slug>` | `main` | user, clicking its PR once it carries a reviewer PASS and CI is green; no Merge Sub-ticket |", new(F.WorkingTreeStandardBranchesAndTargets, 3))
        ]);

    public static readonly Claim BaseMovedRefusalMeansRepinAndReturn = new(
        "A refusal that the base moved means re-pin and return to the captain.",
        [A.CodeWriter],
        [
            new(S.CodeWriter, "Pin the target tip from the branch ref (`git ls-remote origin refs/heads/<base>` or `gh api repos/<owner>/<repo>/branches/<base>`), never from the PR's `baseRefOid` or `base.sha`; a refusal that the base moved means re-pin and return to the captain.", new(F.CodeWriterMergeLead, 4), () => [MergeAnswerToWake.PinTargetTipFromBranchRef!])
        ]);

    public static readonly Claim CaptainDirectsMergeCodeWriter = new(
        "The captain directs each passed lane's or branch's Merge Sub-ticket with one code-writer who maps conflicts and combined gates and performs the merge, then verifies the integrated result.",
        [A.TicketCaptain, A.CodeWriter],
        [
            new(S.TicketCaptain, "Direct each passed lane's Merge Sub-ticket with one `code-writer`: it maps conflicts and combined gates and performs the merge; a resolution that refactored leaves its code no worse than either side.", new(F.TicketCaptainMethod, 48), () => [MergeAnswerToWake.RefactorResolutionNoWorse!])
        ]);

    public static readonly Claim CaptainSetsReadyToMerge = new(
        "The captain sets Ready to Merge, which means a reviewed PR is offered.",
        [A.TicketCaptain, A.Admiral],
        [
            new(S.Admiral, "A captain's `done <key>: PR ready` leaves its ticket `Ready to Merge` with a reviewed PR; its captain rechecked the gate before setting it.", new(F.AdmiralMethod, 33), () => [MergeAnswerToWake.RecheckGateBeforeReadyToMerge!]),
            new(S.TicketCaptain, "Then set `Ready to Merge`, and return `done <key>: PR ready`.", new(F.TicketCaptainMethod, 60), () => [ReturnAdmiralToNo.DonePrReady])
        ]);

    public static readonly Claim ConflictFreeMergeUsesDydoMerge = new(
        "A conflict-free merge of the reviewed PR into its feature branch is performed by the Merge code-writer with dydo merge; any other merge, a conflicted one included, stays local with the resolutions applied.",
        [A.CodeWriter],
        [
            new(S.CodeWriter, "Merge with a merge commit that keeps both parents and every hop SHA: a conflict-free merge of the reviewed PR into its feature branch is `dydo merge <pr> --match-head-commit <reviewed SHA> --match-base-commit <target tip>`; any other merge, a conflicted one included, stays local, with the resolutions applied.", new(F.CodeWriterMergeLead, 3), () => [WorktreeAuditToReviewer.HopShasPreserved]),
            new(S.TicketCaptain, "When the final Merge Sub-ticket's blocker clears, resume from the record and direct its chain as above into the contract's target; its code-writer merges a conflict-free reviewed PR into its feature branch with `dydo merge`.", new(F.TicketCaptainMethod, 61), () => [MergeAnswerToWake.ResumeWhenBlockerClears!])
        ]);

    public static readonly Claim DydoMergeNeverDefaultBranch = new(
        "Merges go into the feature branch and never into master, main or the default branch; dydo merge and the push guard refuse the default branch, which is the user's, while a conflicted merge's push to its feature branch passes.",
        [A.CodeWriter, A.Admiral, A.Officer, A.AnyAgent],
        [
            new(S.Admiral, "When its Merge Sub-ticket's blocker clears, resume that captain with `merge`, or commission a fresh one from the record; its Merge code-writer runs `dydo merge`, which refuses master, main and the default branch.", new(F.AdmiralMethod, 35), () => [MergeAnswerToWake.ResumeWhenBlockerClears!]),
            new(S.OvernightMode, "**Merge into the feature branch** through your captain's Merge chain, which runs `dydo merge`; never into master or main.", new(F.OvernightModeStandingOrders, 4)),
            new(S.WorkingTreeStandard, "A conflicted merge's push to its feature branch passes the guard, while a push to the default branch is blocked: the default branch is the user's.", new(F.WorkingTreeStandardWhoDoesWhat, 7))
        ]);

    public static readonly Claim EveryIntegrationHasMergeSubticket = new(
        "Every actual merge operation (lane into parent, ticket into feature, landing) has its own Merge Sub-ticket, wired in order, except an atomic ticket's own merge into main.",
        [A.TicketCaptain, A.Admiral],
        [
            new(S.Admiral, "Give every merging delivery ticket a final Merge Sub-ticket, blocked by the previous merge in plan order.", new(F.AdmiralMethod, 22)),
            new(S.Reviewer, "Every operation has its own Merge Ticket: a lane into its parent, a ticket into the feature, or the landing.", new(F.ReviewerMergeLead, 3)),
            new(S.TicketCaptain, "Contract each lane and give each merge into the parent its own Merge Sub-ticket, wired in order.", new(F.TicketCaptainMethod, 15))
        ]);

    public static readonly Claim HandleExistingChecksThenWatch = new(
        "Handle the PR's existing checks and comments, then watch it.",
        [A.TicketCaptain],
        [
            new(S.TicketCaptain, "Handle its existing checks and comments, then **watch** it: one watcher per PR, `watch_pull_request` in T3, elsewhere one long-running watcher (Monitor in Claude Code), never a sleep or polling loop.", new(F.TicketCaptainMethod, 54), () => [MergeAnswerToWake.OneWatcherPerPr!])
        ]);

    public static readonly Claim LandingPrerequisites = new(
        "The landing Merge Ticket is filed blocked by its unresolved prerequisites (delivery, each confirmed Inquisition round, required fixes, every should-ticket a review or inquisition filed) and excludes the landing itself and later or deferred work.",
        [A.Admiral],
        [
            new(S.Admiral, "File the landing Merge Ticket, blocked by its unresolved prerequisites: delivery, each confirmed Inquisition round, required fixes, and every should-ticket a review or an inquisition filed, wired now where it is not yet.", new(F.AdmiralMethod, 62))
        ]);

    public static readonly Claim LandingScope = new(
        "The landing's scope is main into feature, combined gates, merge review, then a PR into main.",
        [A.Admiral],
        [
            new(S.Admiral, "Its scope is main into feature, combined gates, merge review, then a PR into main.", new(F.AdmiralMethod, 66))
        ]);

    public static readonly Claim LandingUserClick = new(
        "The landing (one Project at a time) and an atomic ticket wait in Ready to Merge for the user's merge-commit click; the user alone lands the feature on the default branch.",
        [A.TicketCaptain, A.User, A.MapHolder],
        [
            new(S.Admiral, "Its `Ready to Merge` is the user's click, one Project at a time, as a merge commit.", new(F.AdmiralMethod, 67)),
            new(S.TicketCaptain, "A landing Merge instead offers its reviewed PR and waits for the user's merge-commit click.", new(F.TicketCaptainMethod, 63)),
            new(S.WorkingTreeStandard, "Project tickets integrate serially through their feature branch; the user alone lands the feature on `main`/`master` (the repository's default branch, whatever it is named).", new(F.WorkingTreeStandardInvariants, 9), () => [MergeAnswerToWake.ProjectTicketsIntegrateSeriallyThroughFeatureBranch!])
        ]);

    public static readonly Claim MergeCommandLineIsReceipt = new(
        "The merge command's own merged or already integrated line is the merge receipt.",
        [A.CodeWriter],
        [
            new(S.CodeWriter, "The command's own `merged` or `already integrated` line is the merge receipt.", new(F.CodeWriterMergeLead, 5))
        ]);

    public static readonly Claim MergeDoneCondition = new(
        "Merge is done when merge review passes, or the user's click has landed the atomic ticket.",
        [A.TicketCaptain], []);

    public static readonly Claim MergeShaIsCandidateForCombinedGatesAndReview = new(
        "The dydo merge SHA is the candidate for the combined gates and the merge review.",
        [A.CodeWriter, A.Reviewer], []);

    public static readonly Claim OfficerUnwatchesOnDoneOrRelease = new(
        "The owning officer unwatches the PR when its captain returns done <key>: PR ready or is released.",
        [A.Admiral],
        [
            new(S.Admiral, "On `PR open <key>, checks pending: <url>, head <SHA>`, watch that PR (`watch_pull_request` in T3) and wake its captain on news; unwatch when it returns `done <key>: PR ready` or is released.", new(F.AdmiralMethod, 34), () => [MergeAnswerToWake.OfficerWatchesCaptainPr!])
        ]);

    public static readonly Claim OfficerWatchesCaptainPr = new(
        "The owning officer (the top-level thread in T3) watches a delegated captain's PR and wakes the captain on news.",
        [A.TicketCaptain, A.Admiral, A.Officer],
        [
            new(S.TicketCaptain, "The PR's owning thread watches; in T3 that is the top-level officer, so a delegated captain links the PR, returns `PR open <key>, checks pending: <url>, head <SHA>` and ends its turn until its officer wakes it.", new(F.TicketCaptainMethod, 55), () => [ReturnOrdinaryToWriter.PrOpenChecksPending]),
            new(S.WorkingTreeStandard, "A delegated captain whose PR still awaits checks returns `PR open <key>, checks pending: <url>, head <SHA>`; its officer, the top-level thread that owns the PR in T3, watches it and wakes the captain, which rechecks the gate before `Ready to Merge`.", new(F.WorkingTreeStandardReturnAndRelease, 4), () => [MergeAnswerToWake.RecheckGateBeforeReadyToMerge!, ReturnOrdinaryToWriter.PrOpenChecksPending])
        ]);

    public static readonly Claim OneWatcherPerPr = new(
        "Use one watcher per PR (watch_pull_request in T3, elsewhere one long-running watcher such as Monitor), never a sleep or polling loop.",
        [A.TicketCaptain], []);

    public static readonly Claim OpenPrWithPassBlock = new(
        "The PR is opened with its PASS block on the record and in the body (under ## Independent review), using a lone gh pr create --body-file, nothing chained, piped or redirected.",
        [A.TicketCaptain],
        [
            new(S.TicketCaptain, "**Done:** push the branch, write the PR body with `pr` to `dydo/agents/workspace/pr-body-<KEY>.md`, listing the should-tickets filed and the nits recorded, run `dydo pr-ready --body <that file>` as its own command, then open the PR, its PASS block on the record and in the body, with a lone `gh pr create --body-file <that file>`, nothing chained, piped or redirected.", new(F.TicketCaptainMethod, 53), () => [MergeAnswerToWake.PrBodyViaPrSkill!, MergeAnswerToWake.PrReadyOwnCommand!, MergeAnswerToWake.PushBranch!])
        ]);

    public static readonly Claim OrderMergesDone = new(
        "Ordering merges is done when every merge has its own captain-directed chain and fresh merge review, the record shows the order that ran, and done <key>: merged wakes the next work.",
        [A.Admiral],
        [
            new(S.Admiral, "**Done:** every merge has its own captain-directed chain and fresh merge review; the record shows the order that ran and `done <key>: merged` wakes the next work.", new(F.AdmiralMethod, 37))
        ]);

    public static readonly Claim PinTargetTipFromBranchRef = new(
        "The target tip is pinned from the branch ref (git ls-remote or gh api), never from the PR's baseRefOid or base.sha.",
        [A.CodeWriter], []);

    public static readonly Claim PrBodyViaPrSkill = new(
        "Write the PR body with the pr skill to dydo/agents/workspace/pr-body-<KEY>.md, listing the should-tickets filed and the nits recorded.",
        [A.TicketCaptain], []);

    public static readonly Claim PrReadyOwnCommand = new(
        "Run dydo pr-ready --body <that file> as its own command.",
        [A.TicketCaptain], []);

    public static readonly Claim PrTargetsBranchInBranchesTable = new(
        "The PR targets the branch the working-tree standard's Branches table names.",
        [A.TicketCaptain],
        [
            new(S.WorkingTreeStandard, "| Review and offer | `ticket-captain` | Passed lane branches are integrated into the parent ticket branch; combined gates pass; a fresh parent ticket-review PASS block is on the ticket and in the PR; the branch is pushed and the PR targets the branch in the table above. |", new(F.WorkingTreeStandardWhoDoesWhat, 5), () => [MergeAnswerToWake.PushBranch!, ReviewReadToWrong.WholeTicketPassAfterLanes])
        ]);

    public static readonly Claim ProjectTicketIntegrationPath = new(
        "A Project ticket integrates through its final Merge Sub-ticket.",
        [A.TicketCaptain], []);

    public static readonly Claim ProjectTicketsIntegrateSeriallyThroughFeatureBranch = new(
        "Project tickets integrate serially through their feature branch.",
        [A.TicketCaptain], []);

    public static readonly Claim PushBranch = new(
        "Push the branch.",
        [A.TicketCaptain], []);

    public static readonly Claim RecheckGateBeforeReadyToMerge = new(
        "The captain rechecks the merge gate itself before setting Ready to Merge.",
        [A.TicketCaptain, A.Admiral],
        [
            new(S.TicketCaptain, "Recheck the merge gate yourself: [ ] the proof of record covers every change in the diff; [ ] required checks are green on the latest push; [ ] review bots have finished, each finding fixed or answered; [ ] no human review requests changes; [ ] the branch is up to date with its target (not BEHIND; T3 reports only conflicts), and GitHub reports it mergeable.", new(F.TicketCaptainMethod, 59))
        ]);

    public static readonly Claim RefactorResolutionNoWorse = new(
        "A merge resolution that refactored leaves its code no worse than either side.",
        [A.CodeWriter],
        [
            new(S.CodeWriter, "A resolution that refactored leaves its code no worse than either side.", new(F.CodeWriterMergeLead, 7))
        ]);

    public static readonly Claim RefreshDeferredLine = new(
        "Refresh the PR body's **Deferred:** line.",
        [A.TicketCaptain], []);

    public static readonly Claim RepoFailureVsInfraFlake = new(
        "Tell repo failures from infra flakes.",
        [A.TicketCaptain], []);

    public static readonly Claim RestFallbackWhenGraphqlExhausted = new(
        "With GraphQL exhausted, read the PR and check-runs over REST.",
        [A.TicketCaptain],
        [
            new(S.TicketCaptain, "With GraphQL exhausted, read the PR and check-runs over REST (`gh api repos/<o>/<r>/pulls/<n>`, `.../commits/<sha>/check-runs`).", new(F.TicketCaptainMethod, 58))
        ]);

    public static readonly Claim ResumeWhenBlockerClears = new(
        "When a ticket's final Merge Sub-ticket blocker clears, its captain resumes from the record (or a fresh captain takes the record) and directs the chain into the contract's target.",
        [A.TicketCaptain, A.Admiral],
        [
            new(S.WorkingTreeStandard, "It resumes when its Merge Sub-ticket's native blocker clears, or a fresh captain takes the record, and returns `done <key>: merged` after merge PASS and cleanup.", new(F.WorkingTreeStandardReturnAndRelease, 2), () => [ReturnAdmiralToNo.DoneMerged])
        ]);

    public static readonly Claim RewireOrderForIndependentPr = new(
        "The merge order is rewired when a later ready PR is independent of an earlier unready one.",
        [A.Admiral, A.MapHolder],
        [
            new(S.Admiral, "Rewire the order when a later ready PR is independent of an earlier unready one.", new(F.AdmiralMethod, 36))
        ]);

    public static readonly Claim StaySilentWhenUnchanged = new(
        "Stay silent when nothing changed.",
        [A.TicketCaptain], []);

    public static readonly Claim UnwatchBeforeHandoff = new(
        "Unwatch the PR before handing off, asking or releasing.",
        [A.TicketCaptain],
        [
            new(S.TicketCaptain, "Unwatch before handing off, asking or releasing.", new(F.TicketCaptainMethod, 56))
        ]);

    public static readonly Claim VerifyBotFinding = new(
        "Verify each bot finding against the source and grade it blocker, should or nit as in Review.",
        [A.TicketCaptain], []);

    public static readonly Claim WakeIsNewsNotDecision = new(
        "A wake is news, not a merge decision: act only on checks and comments newer than the latest push.",
        [A.TicketCaptain], []);
}
