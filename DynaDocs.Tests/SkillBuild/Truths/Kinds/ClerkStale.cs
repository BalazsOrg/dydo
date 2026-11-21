namespace DynaDocs.Tests.SkillBuild.Truths;

using DynaDocs.SkillBuild;

/// <summary>A legacy line that names a resource of the replaced keeper, which the keeper that replaced it does not build.</summary>
public static class ClerkStale
{
    public static readonly Claim Hand = new("The clerk hands a stale watch to the keeper.", [KeeperActor.Keeper],
        [new(OldSkill.Clerk, "Hand a stale watch to `/keeper`, with its `stale` resource.", new(OldSlot.Desk, 2))]);
}
