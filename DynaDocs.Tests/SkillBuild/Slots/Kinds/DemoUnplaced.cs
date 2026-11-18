namespace DynaDocs.Tests.SkillBuild.Slots;

using DynaDocs.SkillBuild;

/// <summary>A broken kind: a line with no placement (K9).</summary>
public static class DemoUnplaced
{
    public static readonly Claim Nowhere = new("Have a place.", [DemoActor.Crew],
        [new(DemoSkill.Demo, "Have a place.", null!)]);
}
