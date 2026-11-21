namespace DynaDocs.Tests.SkillBuild.Truths;

using DynaDocs.SkillBuild;

/// <summary>A broken kind: a literal word after a flag that takes no value.</summary>
public static class KeeperFlagValue
{
    public static readonly Claim Dry = new("The keeper dry-runs the merge.", [KeeperActor.Keeper],
        [new(KeeperSkill.Keeper, KeeperActor.Keeper, "Run `dydo merge <pr> --dry-run nonsense`.", new(KeeperSlot.Extra, 1))])
    {
        Truth = Verified.ByRun
    };
}
