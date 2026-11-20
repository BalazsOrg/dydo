namespace DynaDocs.Tests.SkillBuild.Truths;

using DynaDocs.SkillBuild;

/// <summary>A reason at the floor: two of its four words are its rule's, two are its own.</summary>
public static class KeeperHalfOwnReason
{
    public static readonly Claim Bell = new("The keeper rings the bell at noon.", [KeeperActor.Keeper],
        [new(KeeperSkill.Keeper, KeeperActor.Keeper, "Ring the bell at noon.", new(KeeperSlot.Extra, 1))])
    {
        Truth = Verified.ByRun with { Reason = "Rings the bell so sleepers wake." }
    };
}
