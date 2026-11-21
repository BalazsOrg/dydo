namespace DynaDocs.Tests.SkillBuild.Truths;

using DynaDocs.SkillBuild;

/// <summary>A broken kind: a system transition with an empty layer, which no skill's check reads.</summary>
public static class KeeperBlankMove
{
    public static readonly Claim Tide = new("The tide sends a held thing.", [KeeperActor.Keeper], [])
    {
        Truth = Verified.ByRun with { Layer = " " },
        Transition = new(KeeperState.Held, KeeperState.Sent, "the tide sends it")
    };
}
