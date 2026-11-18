namespace DynaDocs.Tests.SkillBuild.Fixture.Kinds;

using DynaDocs.SkillBuild;
using A = DynaDocs.Tests.SkillBuild.Fixture.FixtureActor;
using F = DynaDocs.Tests.SkillBuild.Fixture.FixtureSection;
using S = DynaDocs.Tests.SkillBuild.Fixture.FixtureSkill;

/// <summary>Migration fixture: the inventory's <c>worktree</c> claims, sub-topics root to t3.</summary>
public static class WorktreeRootToT3
{
    public static readonly Claim RootIsIsolatedWorktree = new(
        "Before the first edit, the repository root is the isolated worktree, not the main checkout.",
        [A.CodeWriter, A.DocsWriter],
        [
            new(S.CodeWriter, "The repository root is the isolated worktree, not the main checkout.", new(F.CodeWriterBeforeTheFirstEdit, 3)),
            new(S.WorkingTreeStandard, "The repository root is the isolated worktree, not the main checkout.", new(F.WorkingTreeStandardBeforeTheFirstEdit, 3))
        ]);

    public static readonly Claim SatisfyStandardBeforeSpawning = new(
        "Satisfy the working-tree standard before spawning.",
        [A.TicketCaptain], []);

    public static readonly Claim SpawnedCaptainPushesParentBranchAndOpensPr = new(
        "A spawned Ticket Captain first pushes the parent branch and opens its PR so the work survives its return.",
        [A.TicketCaptain],
        [
            new(S.WorkingTreeStandard, "| Parent ticket and lane worktrees | `ticket-captain` | Every worktree it or its crew made (lanes, merge checkouts, temp checkouts) is cleaned with `dydo clean --dry-run <worktree>`, then `dydo clean <worktree>`, run from outside that tree. The worktree its thread runs in belongs to T3: leave it. Any worktree under `~/.t3/worktrees` was made by T3, a crew member's `delegate_task` checkout included, and `dydo clean` skips it. A spawned Ticket Captain first pushes the parent branch and opens its PR so the work survives its return. |", new(F.WorkingTreeStandardCleanup, 1), () => [WorktreeRootToT3.T3MadeWorktreesSkippedByDydoClean!])
        ]);

    public static readonly Claim StageOwnedPathsByName = new(
        "Each writer stages owned paths by name, never a whole-tree add.",
        [A.Crew],
        [
            new(S.WorkingTreeStandard, "Each writer stages owned paths by name; a whole-tree add can capture another writer's work.", new(F.WorkingTreeStandardDelegation, 34))
        ]);

    public static readonly Claim SweepOrphans = new(
        "The chief of staff audits failures of the working-tree standard, not routine cleanup: it compares git worktree list and merged branches with Linear and sweeps orphans (worktrees and branches a merge should have retired, inquisition/<slug> branches past their ticket's Done, prototype/<name> past the delivery ticket's Done), treating each finding as a standard failure to clear or route with its owner named.",
        [A.ChiefOfStaff],
        [
            new(S.ChiefOfStaff, "Sweep orphans too: the worktrees and branches a merge should have retired, and an `inquisition/<slug>` past its ticket's `Done` (a group's `inquisition/<slug>-<group>` only once its parent Inquisition is `Done`), or a `prototype/<name>` past its delivery ticket's `Done`.", new(F.ChiefOfStaffMethod, 13)),
            new(S.WorkingTreeStandard, "The `chief-of-staff` audits failures of this standard, not routine cleanup.", new(F.WorkingTreeStandardLead, 1))
        ]);

    public static readonly Claim T3MadeWorktreesSkippedByDydoClean = new(
        "Worktrees under ~/.t3/worktrees (delegate_task checkouts included) are T3-made, and dydo clean skips them.",
        [A.TicketCaptain], []);
}
