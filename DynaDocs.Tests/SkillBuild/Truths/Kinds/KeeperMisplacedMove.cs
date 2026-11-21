namespace DynaDocs.Tests.SkillBuild.Truths;

using DynaDocs.SkillBuild;

/// <summary>A broken kind: a transition's line in a phase that covers neither of its states.</summary>
public static class KeeperMisplacedMove
{
    public static readonly Claim Early = new("A sent thing is finished.", [KeeperActor.Keeper],
        [new(KeeperSkill.Keeper, KeeperActor.Keeper, "Finish what you sent.", new(KeeperSlot.Take, 9))])
    {
        Truth = Verified.ByRun,
        Transition = new(KeeperState.Sent, KeeperState.Done, "it is finished")
    };
}
