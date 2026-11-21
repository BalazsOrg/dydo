namespace DynaDocs.Tests.SkillBuild.Truths;

using DynaDocs.SkillBuild;

/// <summary>A claim of the rota nobody has verified: its line is withheld from the resource and listed for review.</summary>
public static class ClerkRotaOpen
{
    public static readonly Claim Unseen = new("The clerk rings the bell at noon.", [KeeperActor.Keeper],
        [new(OldSkill.Clerk, KeeperActor.Keeper, "Ring the bell at noon.", new(AheadSlot.RotaRules, 2))])
    {
        Truth = Verified.ByRun with { Evidence = Evidence.Open("Nobody has watched a noon yet.") }
    };
}
