namespace DynaDocs.Tests.SkillBuild.Truths;

using DynaDocs.SkillBuild;

/// <summary>A broken kind: a missing command after <c>&amp;&amp;</c>.</summary>
public static class KeeperChainedAnd
{
    public static readonly Claim Usage = new("The keeper checks, then reads usage.", [KeeperActor.Keeper],
        [new(KeeperSkill.Keeper, KeeperActor.Keeper, "Run `dydo check && dydo usage`.", new(KeeperSlot.Extra, 1))])
    {
        Truth = Verified.ByRun
    };
}
