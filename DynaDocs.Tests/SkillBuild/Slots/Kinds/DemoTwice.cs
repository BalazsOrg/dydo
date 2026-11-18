namespace DynaDocs.Tests.SkillBuild.Slots;

using DynaDocs.SkillBuild;

/// <summary>A broken kind: one claim with two lines in the same skill (K4).</summary>
public static class DemoTwice
{
    public static readonly Claim SaidTwice = new("Say it once.", [DemoActor.Crew],
    [
        new(DemoSkill.Demo, "Say it once.", new(DemoSlot.Dispatch, 2)),
        new(DemoSkill.Demo, "Say it once, really.", new(DemoSlot.Dispatch, 3))
    ]);
}
