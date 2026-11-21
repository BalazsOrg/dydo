namespace DynaDocs.Tests.SkillBuild.Truths;

using DynaDocs.SkillBuild;

/// <summary>A broken kind: a line naming a command the CLI does not have.</summary>
public static class KeeperMissingCommand
{
    public static readonly Claim Usage = new("The keeper reads usage.", [KeeperActor.Keeper],
        [new(KeeperSkill.Keeper, KeeperActor.Keeper, "Read `dydo usage` first.", new(KeeperSlot.Extra, 1))])
    {
        Truth = Verified.ByRun
    };
}
