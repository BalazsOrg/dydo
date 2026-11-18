namespace DynaDocs.Tests.SkillBuild.Slots.Kinds;

using DynaDocs.SkillBuild;

/// <summary>A broken kind: a line that takes an order another line in its slot already holds.</summary>
public static class DemoClash
{
    public static readonly Claim SameOrder = new("Take your own order.", [DemoActor.Crew],
        [new(DemoSkill.Demo, "Take your own order.", new(DemoSlot.Dispatch, 1))]);
}
