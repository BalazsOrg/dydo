namespace DynaDocs.Tests.SkillBuild.Truths;

using DynaDocs.SkillBuild;

/// <summary>A broken kind: a claim the rota states with no truth fields.</summary>
public static class ClerkRotaNoTruth
{
    public static readonly Claim Bare = new("The rota is signed.", [KeeperActor.Keeper],
        [new(OldSkill.Clerk, KeeperActor.Keeper, "Sign the rota.", new(AheadSlot.RotaRules, 2))]);
}
