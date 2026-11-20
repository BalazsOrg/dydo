namespace DynaDocs.Tests.SkillBuild.Truths;

using DynaDocs.SkillBuild;

/// <summary>A broken kind: an empty layer.</summary>
public static class KeeperEmptyLayer
{
    public static readonly Claim Blank = new("The keeper files a note.", [KeeperActor.Keeper],
        [new(KeeperSkill.Keeper, KeeperActor.Keeper, "File a note.", new(KeeperSlot.Extra, 1))])
    {
        Truth = Verified.ByRun with { Layer = "" }
    };
}
