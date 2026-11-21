namespace DynaDocs.Tests.SkillBuild.Truths;

using DynaDocs.SkillBuild;

/// <summary>A broken kind: a multi-line line in a steps slot.</summary>
public static class KeeperStepBlock
{
    public static readonly Claim Block = new("The keeper lists two things.", [KeeperActor.Keeper],
        [new(KeeperSkill.Keeper, KeeperActor.Keeper, "One.\nTwo.", new(KeeperSlot.Merge, 9))])
    {
        Truth = Verified.ByRun
    };
}
