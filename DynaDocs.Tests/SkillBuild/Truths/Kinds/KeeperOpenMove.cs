namespace DynaDocs.Tests.SkillBuild.Truths;

using DynaDocs.SkillBuild;

/// <summary>A broken kind: an open system transition, which no line could withhold from its machine.</summary>
public static class KeeperOpenMove
{
    public static readonly Claim Tide = new("The tide sends a held thing.", [KeeperActor.Keeper], [])
    {
        Truth = new(Evidence.Open("Nobody has watched a tide yet."), Layer: "Nothing below the skill covers it.", Reason: "x"),
        Transition = new(KeeperState.Held, KeeperState.Sent, "the tide sends it")
    };
}
