namespace DynaDocs.Tests.SkillBuild.Truths;

using DynaDocs.SkillBuild;

/// <summary>A broken kind: a verbatim line that numbers every item 1.</summary>
public static class KeeperLazyNumbers
{
    public static readonly Claim Lazy = new("The keeper works in two steps.", [KeeperActor.Keeper],
        [new(KeeperSkill.Keeper, KeeperActor.Keeper, "1. Open the gate.\n1. Close the gate.", new(KeeperSlot.Kinds, 2))])
    {
        Truth = Verified.ByRun
    };
}
