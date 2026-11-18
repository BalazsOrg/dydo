namespace DynaDocs.Tests.SkillBuild.Slots;

using DynaDocs.SkillBuild;

/// <summary>A broken kind: a combined line that no longer names the claim it stated for (K4).</summary>
public static class DemoUncombined
{
    public static readonly Claim Dropped = new("Keep the combined claim.", [DemoActor.Crew], []);

    public static readonly Claim Combining = new("Combine two claims.", [DemoActor.Crew],
        [new(DemoSkill.Demo, "Combine two claims, and keep the other.", new(DemoSlot.Dispatch, 2))]);
}
