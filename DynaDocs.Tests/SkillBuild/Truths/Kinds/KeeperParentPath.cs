namespace DynaDocs.Tests.SkillBuild.Truths;

using DynaDocs.SkillBuild;

/// <summary>A broken kind: a code evidence that walks through <c>..</c> instead of naming its path from the root.</summary>
public static class KeeperParentPath
{
    public static readonly Claim Filed = new("The keeper files its notes.", [KeeperActor.Keeper],
        [new(KeeperSkill.Keeper, KeeperActor.Keeper, "File your notes in the docs.", new(KeeperSlot.Extra, 1))])
    {
        Truth = Verified.ByRun with { Evidence = Evidence.Code("docs/../docs/keeper.md") }
    };
}
