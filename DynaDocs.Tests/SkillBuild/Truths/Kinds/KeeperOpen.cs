namespace DynaDocs.Tests.SkillBuild.Truths;

using DynaDocs.SkillBuild;

/// <summary>A claim nobody has verified: its line is withheld from the skill and listed for review.</summary>
public static class KeeperOpen
{
    public static readonly Claim Unseen = new("The keeper rings the bell at noon.", [KeeperActor.Keeper],
        [new(KeeperSkill.Keeper, KeeperActor.Keeper, "Ring the bell at noon.", new(KeeperSlot.Extra, 1))])
    {
        Truth = Verified.ByRun with { Evidence = Evidence.Open("Nobody has watched a noon yet.") }
    };
}
