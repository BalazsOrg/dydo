namespace DynaDocs.Tests.SkillBuild.Truths;

using DynaDocs.SkillBuild;

/// <summary>A broken kind: an empty reason.</summary>
public static class KeeperEmptyReason
{
    public static readonly Claim Blank = new("The keeper files a note.", [KeeperActor.Keeper],
        [new(KeeperSkill.Keeper, KeeperActor.Keeper, "File a note.", new(KeeperSlot.Extra, 1))])
    {
        Truth = Verified.ByRun with { Reason = " " }
    };
}
