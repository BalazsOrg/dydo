namespace DynaDocs.Tests.SkillBuild.Fixture.Kinds;

using DynaDocs.SkillBuild;
using A = DynaDocs.Tests.SkillBuild.Fixture.FixtureActor;
using F = DynaDocs.Tests.SkillBuild.Fixture.FixtureSection;
using S = DynaDocs.Tests.SkillBuild.Fixture.FixtureSkill;

/// <summary>Migration fixture: the inventory's <c>worktree</c> claims, sub-topics audit to reviewer.</summary>
public static class WorktreeAuditToReviewer
{
    public static readonly Claim AuditCleanupNeverDeletesRetainedRefs = new(
        "Audit cleanup never deletes the retained Bug refs; they remain input to each Bug's normal chain.",
        [A.TicketCaptain],
        [
            new(S.WorkingTreeStandard, "Audit cleanup never deletes those refs; they remain input to each Bug's normal chain.", new(F.WorkingTreeStandardRetainingAnInquisitionSRecordAndProofs, 7))
        ]);

    public static readonly Claim BaseShaAncestorOfHead = new(
        "Before the first edit, the posted base SHA is an ancestor of HEAD.",
        [A.CodeWriter, A.DocsWriter],
        [
            new(S.CodeWriter, "The posted base SHA is an ancestor of `HEAD`.", new(F.CodeWriterBeforeTheFirstEdit, 4)),
            new(S.WorkingTreeStandard, "The posted base SHA is an ancestor of `HEAD`.", new(F.WorkingTreeStandardBeforeTheFirstEdit, 4))
        ]);

    public static readonly Claim CaptainPostsHopSha = new(
        "The Ticket Captain posts each crew hop's commit SHA on the record.",
        [A.TicketCaptain],
        [
            new(S.TicketCaptain, "Set `Implementing` on each spawn and post each hop's commit SHA on the record.", new(F.TicketCaptainMethod, 20)),
            new(S.WorkingTreeStandard, "Every crew **hop** ends on a commit: the return names its SHA, the Ticket Captain posts it on the record, and the ticket branch keeps its hops unsquashed and unrewritten.", new(F.WorkingTreeStandardInvariants, 7), () => [WorktreeAuditToReviewer.HopReturnNamesCommitSha!, WorktreeAuditToReviewer.HopShasPreserved!])
        ]);

    public static readonly Claim CleanBeforeFirstEdit = new(
        "Before the first edit, the worktree is clean.",
        [A.CodeWriter, A.DocsWriter, A.Inquisitor],
        [
            new(S.CodeWriter, "The worktree is clean.", new(F.CodeWriterBeforeTheFirstEdit, 5)),
            new(S.Inquisitor, "The worktree is clean.", new(F.InquisitorBreakBeforeTheFirstEdit, 3)),
            new(S.WorkingTreeStandard, "The worktree is clean.", new(F.WorkingTreeStandardBeforeTheFirstEdit, 5))
        ]);

    public static readonly Claim CleanOrphanedWorktreesAll = new(
        "Clear orphaned worktrees with dydo clean --dry-run --all, then dydo clean --all, from outside every tree and only after confirming no session is live in them.",
        [A.ChiefOfStaff],
        [
            new(S.ChiefOfStaff, "Clear orphaned worktrees yourself with `dydo clean --dry-run --all`, then `dydo clean --all`, run from outside every tree and only after confirming no session is live in them (it deletes each worktree's own branch once merged); clear merged local branches with no worktree by `git branch -d`; list for the user any remote branch still standing after its merge or its ticket's `Done`, since the guard blocks remote deletion.", new(F.ChiefOfStaffMethod, 14), () => [WorktreeAuditToReviewer.DeleteMergedLocalBranchesNoWorktree!, WorktreeAuditToReviewer.NameRemoteBranchesDueForUser!])
        ]);

    public static readonly Claim CleanOwnWorktrees = new(
        "The captain cleans every worktree it or its crew made with dydo clean --dry-run <worktree> then dydo clean <worktree>, from outside that tree.",
        [A.TicketCaptain], []);

    public static readonly Claim CommitsTouchOwnedPathsOnly = new(
        "Commits touch owned paths only.",
        [A.CodeWriter, A.DocsWriter, A.AnyAgent, A.Crew],
        [
            new(S.CodeWriter, "Commits touch owned paths only.", new(F.CodeWriterBeforeTheFirstEdit, 8)),
            new(S.WorkingTreeStandard, "Commits touch owned paths only.", new(F.WorkingTreeStandardInvariants, 6))
        ]);

    public static readonly Claim DeleteInquisitionAuditBranches = new(
        "Delete the audit branch (and as parent every group's audit branch) locally.",
        [A.TicketCaptain],
        [
            new(S.TicketCaptain, "Delete the audit branch, and as parent every group's audit branch, locally, and name their remotes, and any proof branch no open Bug needs, on the record for the user.", new(F.TicketCaptainInquisitionN12Close, 2))
        ]);

    public static readonly Claim DeleteIntegratedTicketBranch = new(
        "The ticket-captain deletes a ticket branch once it is integrated (lane after review and integration into the parent branch; Project ticket after the merge; atomic ticket after the user clicks its PR into main).",
        [A.TicketCaptain],
        [
            new(S.WorkingTreeStandard, "| Integrated lane Sub-ticket branch | `ticket-captain` | The branch is deleted after it passes review and is integrated into the parent ticket branch. |", new(F.WorkingTreeStandardCleanup, 2))
        ]);

    public static readonly Claim DeleteMergedLocalBranchesNoWorktree = new(
        "Delete merged local branches that have no worktree with git branch -d.",
        [A.TicketCaptain, A.ChiefOfStaff], []);

    public static readonly Claim DeletingABranchMeansLocal = new(
        "Deleting a branch in the working-tree standard means the local branch.",
        [A.AnyAgent],
        [
            new(S.WorkingTreeStandard, "Deleting a branch here means the local branch.", new(F.WorkingTreeStandardCleanup, 6))
        ]);

    public static readonly Claim DydoCleanBranchDeletionLimit = new(
        "dydo clean deletes a local branch only once origin's default branch holds it.",
        [A.AnyAgent],
        [
            new(S.WorkingTreeStandard, "`dydo clean` deletes a local branch only once origin's default branch holds it.", new(F.WorkingTreeStandardCleanup, 5))
        ]);

    public static readonly Claim EveryParentTicketIsolatedWorktree = new(
        "Every parent ticket has an isolated integration worktree.",
        [A.TicketCaptain],
        [
            new(S.WorkingTreeStandard, "Every parent ticket has an isolated integration worktree.", new(F.WorkingTreeStandardInvariants, 3))
        ]);

    public static readonly Claim HeadOnTicketBranch = new(
        "Before the first edit, HEAD is on the relevant ticket or Sub-ticket branch.",
        [A.CodeWriter, A.DocsWriter],
        [
            new(S.CodeWriter, "`HEAD` is on the relevant ticket or Sub-ticket branch.", new(F.CodeWriterBeforeTheFirstEdit, 2)),
            new(S.WorkingTreeStandard, "`HEAD` is on the relevant ticket or Sub-ticket branch.", new(F.WorkingTreeStandardBeforeTheFirstEdit, 2))
        ]);

    public static readonly Claim HopEndsOnOneNamedCommit = new(
        "Each hop ends on one commit named \"<KEY> <hop>: <what>\" (hop is implement, fix after a FAIL, or merge; proof as \"<KEY> proof: <hypothesis>\").",
        [A.CodeWriter, A.Inquisitor, A.Crew],
        [
            new(S.CodeWriter, "Each hop ends on one commit named `<KEY> <hop>: <what>`, where the hop is `implement`, `fix` after a FAIL, or `merge`; proof-only commits `<KEY> proof: <hypothesis>`.", new(F.CodeWriterBeforeTheFirstEdit, 9)),
            new(S.Inquisitor, "Commit the test alone as `<KEY> proof: <hypothesis>` and note the SHA.", new(F.InquisitorBreakWork, 2))
        ]);

    public static readonly Claim HopReturnNamesCommitSha = new(
        "Every crew hop ends on a commit whose SHA the return names.",
        [A.Crew], []);

    public static readonly Claim HopShasPreserved = new(
        "Hop SHAs are preserved: the ticket branch keeps its hops unsquashed and unrewritten.",
        [A.TicketCaptain, A.CodeWriter, A.Reviewer],
        [
            new(S.TicketCaptain, "Every operation preserves the hop SHAs.", new(F.TicketCaptainKindsAndFailurePaths, 16))
        ]);

    public static readonly Claim InquisitionAuditBranch = new(
        "An Inquisition audit branch is cut from the audit SHA as inquisition/<slug> (a group Sub-ticket uses inquisition/<slug>-<group>) and has no PR target.",
        [A.TicketCaptain, A.Inquisitor], []);

    public static readonly Claim InquisitionFollowsRetentionRules = new(
        "An Inquisition follows the working-tree standard on retaining its record and proofs.",
        [A.TicketCaptain], []);

    public static readonly Claim InquisitionPatchBranch = new(
        "An Inquisition patch branches from the audit SHA as its Bug's own <BUG-KEY>-<slug> and targets and merges as its Bug does.",
        [A.TicketCaptain],
        [
            new(S.WorkingTreeStandard, "| Inquisition patch | the audit SHA | its Bug's own `<BUG-KEY>-<slug>` | as its Bug | as its Bug, through the Bug's own chain |", new(F.WorkingTreeStandardBranchesAndTargets, 7))
        ]);

    public static readonly Claim InquisitionProofBranch = new(
        "An Inquisition proof branches from the audit SHA as proof/<KEY>-<area>-<n>, one per Break dispatch (follow-ups and hand-backs counted), has no PR target and never merges.",
        [A.Inquisitor],
        [
            new(S.WorkingTreeStandard, "| Inquisition proof | the audit SHA | `proof/<KEY>-<area>-<n>`, one per Break dispatch, `<n>` counting that area's dispatches, a follow-up or hand-back included | none; never merges | - |", new(F.WorkingTreeStandardBranchesAndTargets, 6))
        ]);

    public static readonly Claim KeyInBranchNames = new(
        "Use the ticket's key in branch names so Linear attaches the branch and PR.",
        [A.AnyAgent],
        [
            new(S.WorkingTreeStandard, "`DYD-123` is an example: use the ticket's key so Linear attaches the branch and PR.", new(F.WorkingTreeStandardBranchesAndTargets, 11))
        ]);

    public static readonly Claim LaneSubticketBranch = new(
        "A lane Sub-ticket branches from its parent ticket branch as <KEY>-<slug>, its PR targets the parent ticket branch, and its captain-directed Merge Sub-ticket merges it.",
        [A.TicketCaptain],
        [
            new(S.WorkingTreeStandard, "| Lane Sub-ticket | its parent ticket branch | `DYD-124-<slug>` | its parent ticket branch | its captain-directed Merge Sub-ticket |", new(F.WorkingTreeStandardBranchesAndTargets, 4))
        ]);

    public static readonly Claim LeaveThreadWorktreeToT3 = new(
        "The worktree the captain's thread runs in belongs to T3: leave it.",
        [A.TicketCaptain],
        [
            new(S.TicketCaptain, "The worktree your thread runs in belongs to T3: leave it.", new(F.TicketCaptainMethod, 66)),
            new(S.WorkingTreeStandard, "The worktree its thread runs in belongs to T3: leave it.", new(F.WorkingTreeStandardReturnAndRelease, 6))
        ]);

    public static readonly Claim NameRemoteBranchesDueForUser = new(
        "Name any remote branch due for deletion on the record for the user, since GitHub deletes only a merged PR's branch and the guard blocks remote deletion.",
        [A.TicketCaptain, A.ChiefOfStaff, A.Officer],
        [
            new(S.TicketCaptain, "GitHub deletes a merged PR's remote branch itself; the guard blocks remote deletion, so name any other remote branch due for deletion, an `inquisition/` or `prototype/` branch included, on the record for the user.", new(F.TicketCaptainMethod, 67))
        ]);

    public static readonly Claim PathsInOwnedPaths = new(
        "Before the first edit, every path the work item will touch is in the ticket's owned paths.",
        [A.CodeWriter, A.DocsWriter],
        [
            new(S.CodeWriter, "Every path the work item will touch is in the ticket's owned paths.", new(F.CodeWriterBeforeTheFirstEdit, 6)),
            new(S.WorkingTreeStandard, "Every path the work item will touch is in the ticket's owned paths.", new(F.WorkingTreeStandardBeforeTheFirstEdit, 6))
        ]);

    public static readonly Claim PlaceTicketWorktree = new(
        "Use the host-provided ticket worktree, else place it beside the repository at ../<repo>.worktrees/<KEY>-<slug>.",
        [A.TicketCaptain],
        [
            new(S.WorkingTreeStandard, "The host may provide the ticket worktree; otherwise place it beside the repository at `../<repo>.worktrees/DYD-123-<slug>`.", new(F.WorkingTreeStandardBranchesAndTargets, 12))
        ]);

    public static readonly Claim ProjectBranch = new(
        "A Project branches from main as feature/<project-slug>, its PR targets main, and the user merges it.",
        [A.TicketCaptain, A.User],
        [
            new(S.WorkingTreeStandard, "| Project | `main` | `feature/<project-slug>` | `main` | user |", new(F.WorkingTreeStandardBranchesAndTargets, 1))
        ]);

    public static readonly Claim ProjectTicketBranch = new(
        "A Project ticket branches from its feature branch as <KEY>-<slug>, its PR targets the feature branch, and its captain-directed Merge Sub-ticket merges it.",
        [A.TicketCaptain],
        [
            new(S.WorkingTreeStandard, "| Project ticket | its feature branch | `DYD-123-<slug>` | its feature branch | its captain-directed Merge Sub-ticket |", new(F.WorkingTreeStandardBranchesAndTargets, 2))
        ]);

    public static readonly Claim ProofBranchLeftCommittedCleanForCaptain = new(
        "The inquisitor leaves the proof branch committed and clean for the captain, who pushes it.",
        [A.Inquisitor],
        [
            new(S.Inquisitor, "**Leave the branch for the captain.** The captain pushes it; you leave it committed and clean. Done when `git status` is clean and `git log` on the branch shows only your proof commits.", new(F.InquisitorBreakWork, 6))
        ]);

    public static readonly Claim ProofOnlyBranch = new(
        "A proof-only proof branch is a child branch named in the brief, cut from the pinned source SHA, with no PR target, and never merges.",
        [A.CodeWriter],
        [
            new(S.WorkingTreeStandard, "| Proof branch | the pinned source SHA its brief names | a child branch named in the proof-only brief | none; never merges | - |", new(F.WorkingTreeStandardBranchesAndTargets, 9))
        ]);

    public static readonly Claim ProofWorkHeadOnProofBranch = new(
        "Before editing, proof work proves HEAD is on the proof branch its brief names, cut from the pinned SHA (the audit SHA for Break).",
        [A.Inquisitor, A.CodeWriter],
        [
            new(S.Inquisitor, "`HEAD` is on the proof branch your brief names, `proof/<KEY>-<area>-<n>`, cut from the audit SHA.", new(F.InquisitorBreakBeforeTheFirstEdit, 2)),
            new(S.WorkingTreeStandard, "Proof work, a proof-only brief or an Inquisition's Break phase, proves three things: `HEAD` is on the proof branch its brief names, cut from that SHA (the audit SHA for Break); the worktree is clean; and every path it touches is a test path.", new(F.WorkingTreeStandardBeforeTheFirstEdit, 9), () => [WorktreeAuditToReviewer.ProofWorkTouchesTestPathsOnly!])
        ]);

    public static readonly Claim ProofWorkTouchesTestPathsOnly = new(
        "Before editing, proof work proves every path it will touch is a test path (a test project, folder or file); a proof-only hop touches only the brief's test path.",
        [A.Inquisitor, A.CodeWriter],
        [
            new(S.Inquisitor, "Every path you will touch is a test path: a test project, a test folder or a test file.", new(F.InquisitorBreakBeforeTheFirstEdit, 4))
        ]);

    public static readonly Claim PrototypeBranch = new(
        "A Prototype Ticket branches from its feature branch (else main) as prototype/<name>, has no PR target and never merges.",
        [A.TicketCaptain],
        [
            new(S.WorkingTreeStandard, "| Prototype Ticket | its feature branch, else `main` | `prototype/<name>` | none; never merges | - |", new(F.WorkingTreeStandardBranchesAndTargets, 10))
        ]);

    public static readonly Claim PrototypeBranchLifecycle = new(
        "A prototype branch is the ticket-captain's, tracked by the map holder: keep the winning code linked as delivery-spec input; delete when the delivery ticket is Done or with feature cleanup.",
        [A.TicketCaptain, A.MapHolder],
        [
            new(S.WorkingTreeStandard, "| Prototype branch | `ticket-captain`, tracked by the map holder | Keep the winning code linked as delivery-spec input; delete when that delivery ticket is Done or with feature cleanup. |", new(F.WorkingTreeStandardCleanup, 4))
        ]);

    public static readonly Claim ProveChecksBeforeFirstEdit = new(
        "Before the first edit of an implement or fix hop, the writer proves the working-tree checks and returns a failure to its Ticket Captain instead of working around it (an Inquisition's Break inquisitor returns a failed check the same way).",
        [A.CodeWriter, A.DocsWriter, A.Inquisitor],
        [
            new(S.CodeWriter, "On an `implement` or `fix` hop, prove the working-tree standard's checks; return a failure to the captain and stop:", new(F.CodeWriterBeforeTheFirstEdit, 1)),
            new(S.Inquisitor, "Prove three things and return a failure to the captain instead of working around it:", new(F.InquisitorBreakBeforeTheFirstEdit, 1)),
            new(S.WorkingTreeStandard, "Before the first edit of an `implement` or `fix` hop in a parent ticket or lane, its assigned writer proves all five checks and returns a failure to its Ticket Captain instead of working around it: `HEAD` is on the relevant ticket or Sub-ticket branch.", new(F.WorkingTreeStandardBeforeTheFirstEdit, 1))
        ]);

    public static readonly Claim ReadWorkingTreeStandard = new(
        "Read dydo/guides/working-tree-standard.md from the repository root.",
        [A.TicketCaptain, A.DocsWriter, A.Admiral, A.ChiefOfStaff],
        [
            new(S.Admiral, "From the repository root, read `dydo/guides/working-tree-standard.md`.", new(F.AdmiralMustReads, 3)),
            new(S.ChiefOfStaff, "From the repository root, read `dydo/guides/working-tree-standard.md`.", new(F.ChiefOfStaffMustReads, 4)),
            new(S.DocsWriter, "From the repository root, read `dydo/guides/working-tree-standard.md`.", new(F.DocsWriterMustReads, 4)),
            new(S.TicketCaptain, "From the repository root, read `dydo/guides/working-tree-standard.md`.", new(F.TicketCaptainMustReads, 4))
        ]);

    public static readonly Claim ReviewerProbeDetachedWorktree = new(
        "A probe that edits files runs in the reviewer's own detached worktree at the candidate SHA, removed after.",
        [A.Reviewer],
        [
            new(S.Reviewer, "A probe that edits files runs in your own detached worktree at the candidate SHA, removed after.", new(F.ReviewerMethod, 5))
        ]);
}
