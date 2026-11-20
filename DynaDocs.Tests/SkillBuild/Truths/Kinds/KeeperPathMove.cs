namespace DynaDocs.Tests.SkillBuild.Truths;

using DynaDocs.SkillBuild;

/// <summary>A system transition whose code evidence is a path the repository may not hold.</summary>
public static class KeeperPathMove
{
    public static readonly Claim Tide = new("The tide sends a held thing.", [KeeperActor.Keeper], [])
    {
        Truth = Verified.ByRun with { Evidence = Evidence.Code("docs/tide.md") },
        Transition = new(KeeperState.Held, KeeperState.Sent, "the tide sends it")
    };
}
