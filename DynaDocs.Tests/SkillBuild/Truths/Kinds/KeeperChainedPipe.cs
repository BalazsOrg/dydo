namespace DynaDocs.Tests.SkillBuild.Truths;

using DynaDocs.SkillBuild;

/// <summary>A broken kind: a missing command after a pipe.</summary>
public static class KeeperChainedPipe
{
    public static readonly Claim Usage = new("The keeper pipes the check into usage.", [KeeperActor.Keeper],
        [new(KeeperSkill.Keeper, KeeperActor.Keeper, "Run `dydo check | dydo usage`.", new(KeeperSlot.Extra, 1))])
    {
        Truth = Verified.ByRun
    };
}
