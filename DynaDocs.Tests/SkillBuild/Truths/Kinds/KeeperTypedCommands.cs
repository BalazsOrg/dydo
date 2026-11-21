namespace DynaDocs.Tests.SkillBuild.Truths;

using DynaDocs.SkillBuild;

/// <summary>Real commands as an author types them: an option's value in lowercase, and an option both alternatives have.</summary>
public static class KeeperTypedCommands
{
    public static readonly Claim Pinned = new("The keeper merges a pinned head.", [KeeperActor.Keeper],
        [new(KeeperSkill.Keeper, KeeperActor.Keeper, "Run `dydo merge <pr> --match-head-commit abc123 --dry-run`.", new(KeeperSlot.Extra, 1))])
    {
        Truth = Verified.ByRun
    };

    public static readonly Claim Archived = new("The keeper archives what it made.", [KeeperActor.Keeper],
        [new(KeeperSkill.Keeper, KeeperActor.Keeper, "Run `dydo archive branch|worktree <target> --dry-run`.", new(KeeperSlot.Extra, 2))])
    {
        Truth = Verified.ByRun
    };
}
