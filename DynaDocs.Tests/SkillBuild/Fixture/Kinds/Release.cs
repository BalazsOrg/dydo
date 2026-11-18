namespace DynaDocs.Tests.SkillBuild.Fixture;

using DynaDocs.SkillBuild;
using A = DynaDocs.Tests.SkillBuild.Fixture.FixtureActor;
using F = DynaDocs.Tests.SkillBuild.Fixture.FixtureSection;
using S = DynaDocs.Tests.SkillBuild.Fixture.FixtureSkill;

/// <summary>Migration fixture: the inventory's <c>release</c> claims, sub-topics blocker to takeover.</summary>
public static class Release
{
    public static readonly Claim BlockerTakeoverDyingSession = new(
        "On a blocker the captain cannot clear or a user takeover (or dying session), push the branch, post the resume SHA, set the parent Todo, unassign, wire any blocker, and return released <key>: <reason>.",
        [A.TicketCaptain],
        [
            new(S.TicketCaptain, "A blocker you cannot clear, the user's takeover, or a dying session releases the ticket: push the branch, post the resume SHA, clean the worktrees you or your crew made (lanes, merge checkouts, temp checkouts) with `dydo clean --dry-run <worktree>`, then `dydo clean <worktree>`, run from outside that tree, set the parent `Todo`, unassign and wire any blocker.", new(F.TicketCaptainRelease, 5)),
            new(S.WorkingTreeStandard, "For an uncleared blocker or user takeover: push, post the resume SHA, clean the worktrees the captain or its crew made (lanes, merge checkouts, temp checkouts) with `dydo clean --dry-run <worktree>`, then `dydo clean <worktree>`, run from outside that tree, set the parent `Todo`, unassign and wire any blocker; return `released <key>: <reason>`.", new(F.WorkingTreeStandardReturnAndRelease, 5), () => [WorktreeAuditToReviewer.CleanOwnWorktrees])
        ]);

    public static readonly Claim DeadSessionTreatedAsRelease = new(
        "A dead session is treated as a release from its last recorded hop, without assuming a final push.",
        [A.MapHolder, A.Admiral, A.AnyAgent],
        [
            new(S.Admiral, "Treat a dead captain as a release without a final push; preserve the last recorded hop.", new(F.AdmiralMethod, 73)),
            new(S.TicketCaptain, "After a dead session the map holder uses the last posted hop, without assuming a final push.", new(F.TicketCaptainRelease, 7)),
            new(S.WorkingTreeStandard, "A dead session is treated as release from its last recorded hop without a final push.", new(F.WorkingTreeStandardReturnAndRelease, 7))
        ]);

    public static readonly Claim FreshCommissionIsPortableFloor = new(
        "A fresh commission from the record is the portable floor; a host that can resume the same captain may do so.",
        [A.AnyAgent, A.Admiral],
        [
            new(S.Admiral, "Fresh commission from the record is the portable floor; one-word resume and transcript steering are host conveniences.", new(F.AdmiralMethod, 75)),
            new(S.TicketCaptain, "Fresh commission is the portable floor; a host that can resume the same captain may do so.", new(F.TicketCaptainRelease, 8)),
            new(S.WorkingTreeStandard, "Fresh commission from the record works on both hosts.", new(F.WorkingTreeStandardReturnAndRelease, 8))
        ]);

    public static readonly Claim InquisitionTwoReleaseCases = new(
        "An Inquisition releases in two cases only: released <key>: verification skill first when a project with a runnable surface has no verification skill, and released <key>: groups <keys> when its area map is cut into group Sub-tickets, resuming once the skill exists or once every group closes.",
        [A.TicketCaptain],
        [
            new(S.TicketCaptain, "Release with `released <key>: verification skill first`.", new(F.TicketCaptainInquisitionN1CommissionInputs, 5)),
            new(S.WorkingTreeStandard, "It releases in two cases only, by [Return and release](#return-and-release): `released <key>: verification skill first` when a project with a runnable surface has no verification skill, and `released <key>: groups <keys>` when its area map is cut into group Sub-tickets, resuming as parent for the cross-group triage once every group closes.", new(F.WorkingTreeStandardRetainingAnInquisitionSRecordAndProofs, 1))
        ]);

    public static readonly Claim LifecycleHandlingThenReturnOrRelease = new(
        "Use established lifecycle handling, then return or release the concrete limitation when captain-owned work cannot run.",
        [A.TicketCaptain],
        [
            new(S.TicketCaptain, "Use established lifecycle handling, then return or release the concrete limitation when captain-owned work cannot run.", new(F.TicketCaptainBoundary, 6)),
            new(S.WorkingTreeStandard, "Use established lifecycle handling, then return or release the concrete limitation when captain-owned work cannot run.", new(F.WorkingTreeStandardDelegation, 23))
        ]);

    public static readonly Claim NextCaptainResumesFromRecord = new(
        "The next captain reads the record and resumes at the stage the record proves, from the branch.",
        [A.TicketCaptain, A.Admiral],
        [
            new(S.Admiral, "Resume at the stage the record proves.", new(F.AdmiralMethod, 4)),
            new(S.TicketCaptain, "The next captain reads the record and resumes from the branch.", new(F.TicketCaptainRelease, 6))
        ]);

    public static readonly Claim ReleasedLinePointsToRecord = new(
        "A released <key>: <reason> points to the record's resume SHA, blocker and prepared packet.",
        [A.Admiral],
        [
            new(S.Admiral, "A `released <key>: <reason>` points to the record's resume SHA, blocker and prepared packet.", new(F.AdmiralMethod, 72))
        ]);

    public static readonly Claim TakeoverGoesThroughRelease = new(
        "A takeover always goes through release before a top-level captain resumes.",
        [A.AnyAgent, A.Admiral],
        [
            new(S.Admiral, "A human takeover always releases before a top-level captain resumes.", new(F.AdmiralMethod, 74)),
            new(S.TicketCaptain, "Takeover always goes through release.", new(F.TicketCaptainRelease, 9))
        ]);
}
