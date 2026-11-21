namespace DynaDocs.Tests.SkillBuild.Truths;

using DynaDocs.SkillBuild;

/// <summary>A broken kind: a line the keeper's skill holds for another reader.</summary>
public static class KeeperWrongReader
{
    public static readonly Claim Landing = new("The Project lands once every should is closed.", [KeeperActor.Keeper],
        [new(KeeperSkill.Keeper, KeeperActor.Lander, "The Project lands once every should is closed.", new(KeeperSlot.Extra, 1))])
    {
        Truth = Verified.ByRun
    };
}
