namespace DynaDocs.Tests.SkillBuild.Truths;

using DynaDocs.SkillBuild;

/// <summary>A broken kind: a code evidence that is an absolute path, true on one machine only.</summary>
public static class KeeperAbsolutePath
{
    public static readonly Claim Filed = new("The keeper files its notes.", [KeeperActor.Keeper],
        [new(KeeperSkill.Keeper, KeeperActor.Keeper, "File your notes in the docs.", new(KeeperSlot.Extra, 1))])
    {
        Truth = Verified.ByRun with { Evidence = Evidence.Code("/") }
    };
}
