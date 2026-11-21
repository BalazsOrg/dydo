namespace DynaDocs.Tests.SkillBuild.Truths;

using DynaDocs.SkillBuild;

/// <summary>The clerk's rota, a resource truth-checked ahead of its skill: a line in each of its slots, one naming a real command.</summary>
public static class ClerkRota
{
    public static readonly Claim Posted = new("The rota is posted each week.", [KeeperActor.Keeper],
        [new(OldSkill.Clerk, KeeperActor.Keeper, "Post the rota each week.", new(AheadSlot.RotaLead, 1))]) { Truth = Verified.ByRun };

    public static readonly Claim Checked = new("The docs are checked before the desk opens.", [KeeperActor.Keeper],
        [new(OldSkill.Clerk, KeeperActor.Keeper, "Run `dydo check` before the desk opens.", new(AheadSlot.RotaRules, 1))]) { Truth = Verified.ByRun };
}
