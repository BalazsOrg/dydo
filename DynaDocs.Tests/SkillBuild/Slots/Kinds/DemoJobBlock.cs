namespace DynaDocs.Tests.SkillBuild.Slots;

using DynaDocs.SkillBuild;

/// <summary>A broken kind: a job line of two lines, which would split the one job paragraph (K9).</summary>
public static class DemoJobBlock
{
    public static readonly Claim TwoLines = new("Say the job in one line.", [DemoActor.Crew],
        [new(DemoSkill.Demo, "You own one ticket.\n\nYou also own another.", new(SpinePart.Job, 3))]);
}
