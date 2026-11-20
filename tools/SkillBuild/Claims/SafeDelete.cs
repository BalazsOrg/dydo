namespace DynaDocs.SkillBuild;

/// <summary>Safe delete (M19): a branch or worktree archived for 30 days, then purged; T3 worktrees included.</summary>
public static class SafeDelete
{
    public static readonly Claim Archive = new("After a merge, the captain safe-deletes the branches and worktrees it made.", [Actor.TicketCaptain],
    [
        new(Skill.TicketCaptain, "After a merge, safe-delete the branches and worktrees you made with `dydo archive`; it keeps them 30 days, then purges them.", new(SafeDeleteState.Live, 1701, MoveKind.Exit)),
        new(Skill.WorkingTreeStandard, "After a merge, its captain safe-deletes the branches and worktrees it made; the archive keeps them 30 days, then purges.", new(Section.TreeCleanup, 1))
    ])
    { Spec = ["N10", "N12 DR 045 §8"], Transition = new(SafeDeleteState.Live, SafeDeleteState.Archived, "after its merge, its captain safe-deletes it") };

    public static readonly Claim ArchiveT3 = new("Safe delete covers T3 worktrees without breaking T3.", [Actor.TicketCaptain], [])
    { Spec = ["N10"], Transition = new(SafeDeleteState.Live, SafeDeleteState.Archived, "a T3 worktree is archived without breaking T3") };

    public static readonly Claim Restore = new("dydo archive restore brings an archived branch or worktree back (DYD-1082 555a8300 gap 10, DYD-1195 70df9ff8).", [Actor.DydoArchive], [])
    { Spec = ["N10"], Transition = new(SafeDeleteState.Archived, SafeDeleteState.Live, "`dydo archive restore` brings it back") };

    public static readonly Claim Purge = new("An archive is purged after 30 days.", [Actor.DydoArchive], [])
    { Spec = ["N10"], Transition = new(SafeDeleteState.Archived, SafeDeleteState.Purged, "30 days pass") };
}
