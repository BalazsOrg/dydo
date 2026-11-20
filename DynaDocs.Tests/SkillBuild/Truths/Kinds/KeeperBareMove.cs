namespace DynaDocs.Tests.SkillBuild.Truths;

using DynaDocs.SkillBuild;

/// <summary>A broken kind: a system transition with no truth fields, in a machine the truth-checked keeper covers.</summary>
public static class KeeperBareMove
{
    public static readonly Claim Tide = new("The tide sends a held thing.", [KeeperActor.Keeper], [])
    {
        Transition = new(KeeperState.Held, KeeperState.Sent, "the tide sends it")
    };
}
