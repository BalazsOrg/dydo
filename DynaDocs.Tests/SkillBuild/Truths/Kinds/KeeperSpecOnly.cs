namespace DynaDocs.Tests.SkillBuild.Truths;

using DynaDocs.SkillBuild;

/// <summary>A broken kind: a reason that only restates its spec citation.</summary>
public static class KeeperSpecOnly
{
    public static readonly Claim Cited = new("The keeper files a note.", [KeeperActor.Keeper],
        [new(KeeperSkill.Keeper, KeeperActor.Keeper, "File a note.", new(KeeperSlot.Extra, 1))])
    {
        Spec = ["K4", "N12 DR 058 §3"],
        Truth = Verified.ByRun with { Reason = "Per K4 and DR 058 §3." }
    };
}
