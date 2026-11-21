namespace DynaDocs.Tests.SkillBuild.Truths;

using DynaDocs.SkillBuild;

/// <summary>A broken kind: a line of the rota that names no reader.</summary>
public static class ClerkRotaNoReader
{
    public static readonly Claim Anyone = new("The rota is pinned up.", [KeeperActor.Keeper],
        [new(OldSkill.Clerk, "Pin the rota up.", new(AheadSlot.RotaRules, 2))]) { Truth = Verified.ByRun };
}
