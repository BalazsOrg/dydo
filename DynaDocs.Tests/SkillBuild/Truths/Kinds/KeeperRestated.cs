namespace DynaDocs.Tests.SkillBuild.Truths;

using DynaDocs.SkillBuild;

/// <summary>A broken kind: a reason that only restates its rule.</summary>
public static class KeeperRestated
{
    public static readonly Claim Echo = new("The keeper files a note after each watch.", [KeeperActor.Keeper],
        [new(KeeperSkill.Keeper, KeeperActor.Keeper, "File a note after each watch.", new(KeeperSlot.Extra, 1))])
    {
        Truth = Verified.ByRun with { Reason = "Because the keeper files a note after each watch." }
    };
}
