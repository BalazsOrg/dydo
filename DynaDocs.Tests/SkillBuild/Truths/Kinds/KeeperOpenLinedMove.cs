namespace DynaDocs.Tests.SkillBuild.Truths;

using DynaDocs.SkillBuild;

/// <summary>A broken kind: an open transition with a line; the line is withheld, the arrow would not be.</summary>
public static class KeeperOpenLinedMove
{
    public static readonly Claim Tide = new("The keeper sends a held thing at high tide.", [KeeperActor.Keeper],
        [new(KeeperSkill.Keeper, KeeperActor.Keeper, "Send it at high tide.", new(KeeperSlot.Extra, 1))])
    {
        Truth = Verified.ByRun with { Evidence = Evidence.Open("Nobody has watched a tide yet.") },
        Transition = new(KeeperState.Held, KeeperState.Sent, "the keeper sends it at high tide")
    };
}
