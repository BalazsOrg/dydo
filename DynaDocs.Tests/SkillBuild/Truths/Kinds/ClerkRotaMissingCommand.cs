namespace DynaDocs.Tests.SkillBuild.Truths;

using DynaDocs.SkillBuild;

/// <summary>A broken kind: a line of the rota naming a command the CLI does not have.</summary>
public static class ClerkRotaMissingCommand
{
    public static readonly Claim Usage = new("Usage is read before the rota is posted.", [KeeperActor.Keeper],
        [new(OldSkill.Clerk, KeeperActor.Keeper, "Read `dydo usage` first.", new(AheadSlot.RotaRules, 2))]) { Truth = Verified.ByRun };
}
