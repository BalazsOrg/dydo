namespace DynaDocs.Tests.SkillBuild.Truths;

using DynaDocs.SkillBuild;

/// <summary>An open claim in the slot a placeholder shares: withheld as any open claim is, whatever stands beside it.</summary>
public static class KeeperSecondOpen
{
    public static readonly Claim Dusk = new("The keeper lights the lamp at dusk.", [KeeperActor.Keeper],
        [new(KeeperSkill.Keeper, KeeperActor.Keeper, "Light the lamp at dusk.", new(KeeperSlot.Extra, 2))])
    {
        Truth = Verified.ByRun with { Evidence = Evidence.Open("Nobody has watched a dusk yet.") }
    };
}
