namespace DynaDocs.Tests.SkillBuild.Truths;

using DynaDocs.SkillBuild;

/// <summary>A broken kind: a missing command after <c>||</c>.</summary>
public static class KeeperChainedOr
{
    public static readonly Claim Usage = new("The keeper checks, or reads usage.", [KeeperActor.Keeper],
        [new(KeeperSkill.Keeper, KeeperActor.Keeper, "Run `dydo check || dydo usage`.", new(KeeperSlot.Extra, 1))])
    {
        Truth = Verified.ByRun
    };
}
