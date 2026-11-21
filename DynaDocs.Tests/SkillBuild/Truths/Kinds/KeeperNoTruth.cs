namespace DynaDocs.Tests.SkillBuild.Truths;

using DynaDocs.SkillBuild;

/// <summary>A broken kind: a claim a truth-checked skill states with no truth fields.</summary>
public static class KeeperNoTruth
{
    public static readonly Claim Bare = new("The keeper keeps a log.", [KeeperActor.Keeper],
        [new(KeeperSkill.Keeper, KeeperActor.Keeper, "Keep a log.", new(KeeperSlot.Extra, 1))]);
}
