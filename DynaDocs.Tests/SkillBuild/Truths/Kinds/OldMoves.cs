namespace DynaDocs.Tests.SkillBuild.Truths;

using DynaDocs.SkillBuild;

/// <summary>
/// The legacy fixture skills' claims, none with truth fields: the old keeper's spine, resource and
/// scenario, which a ledger accounts for, and a clerk that stays built. Two lines name a missing command.
/// </summary>
public static class OldMoves
{
    public static readonly Claim Owns = new("The keeper keeps one thing.", [KeeperActor.Keeper],
        [new(OldSkill.Keeper, "You keep one thing.", new(SpinePart.Job, 1))]);

    public static readonly Claim Cap = new("Runs are capped at eight.", [KeeperActor.Keeper],
        [new(OldSkill.Keeper, "Cap the runs at eight.", new(OldSlot.Work, 1))]);

    public static readonly Claim Survey = new("The board is read first.", [KeeperActor.Keeper],
        [new(OldSkill.Keeper, "Read `dydo survey board` first, by [the manual](resources/manual.md).", new(OldSlot.Work, 2))]);

    public static readonly Claim Manual = new("The manual is read on demand.", [KeeperActor.Keeper],
        [new(OldSkill.Keeper, "Read the manual when unsure.", new(OldSlot.ManualLead, 1))]);

    public static readonly Claim Stale = new("A stale watch stops.", [KeeperActor.Keeper],
        [new(OldSkill.Keeper, "Stop when the watch goes stale.", new(OldSlot.StaleSteps, 1))]);

    public static readonly Claim Desk = new("The clerk reads usage.", [KeeperActor.Keeper],
        [new(OldSkill.Clerk, "Read `dydo usage`, then run `dydo check`.", new(OldSlot.Desk, 1))]);
}
