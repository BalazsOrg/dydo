namespace DynaDocs.Tests.SkillBuild.Truths;

using DynaDocs.SkillBuild;

/// <summary>A broken kind: a reason that restates its rule but for one word.</summary>
public static class KeeperMostlyRestated
{
    public static readonly Claim Bell = new("The keeper rings the bell at noon.", [KeeperActor.Keeper],
        [new(KeeperSkill.Keeper, KeeperActor.Keeper, "Ring the bell at noon.", new(KeeperSlot.Extra, 1))])
    {
        Truth = Verified.ByRun with { Reason = "The keeper rings the bell at midday." }
    };
}
