namespace DynaDocs.Tests.SkillBuild.Charts;

using DynaDocs.SkillBuild;

/// <summary>A transition claim whose only line sits in a standard's section, as the 4.0.0 standards render the merge and status rules.</summary>
public static class LeaseCharter
{
    public static readonly Claim Release = new("A held lease may be released.", [],
        [new(ChartSkill.Charter, "A held lease may be released.", new(ChartSection.Rules, 1))])
    {
        Spec = ["K12"],
        Transition = new(LeaseState.Held, LeaseState.Free, "the holder releases it")
    };
}
