namespace DynaDocs.Tests.SkillBuild.Truths;

using DynaDocs.SkillBuild;

/// <summary>A claim whose evidence is a path in the repository.</summary>
public static class KeeperCodePath
{
    public static readonly Claim Filed = new("The keeper files its notes in the docs.", [KeeperActor.Keeper],
        [new(KeeperSkill.Keeper, KeeperActor.Keeper, "File your notes in the docs.", new(KeeperSlot.Extra, 1))])
    {
        Truth = Verified.ByRun with { Evidence = Evidence.Code("docs/keeper.md") }
    };
}
