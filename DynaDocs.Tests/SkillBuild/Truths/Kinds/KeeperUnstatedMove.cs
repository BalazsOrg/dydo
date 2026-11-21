namespace DynaDocs.Tests.SkillBuild.Truths;

using DynaDocs.SkillBuild;

/// <summary>A broken kind: a transition that answers its fields and that no line of the keeper states.</summary>
public static class KeeperUnstatedMove
{
    public static readonly Claim Tide = new("The tide sends a held thing.", [KeeperActor.Keeper], [])
    {
        Truth = Verified.ByRun,
        Transition = new(KeeperState.Held, KeeperState.Sent, "the tide sends it")
    };
}
