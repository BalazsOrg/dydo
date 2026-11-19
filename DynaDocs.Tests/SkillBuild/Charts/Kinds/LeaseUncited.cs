namespace DynaDocs.Tests.SkillBuild.Charts;

using DynaDocs.SkillBuild;

/// <summary>A kind a citing model rejects: a claim that names no spec rule.</summary>
public static class LeaseUncited
{
    public static readonly Claim Silent = new("Say nothing of the spec.", [ChartSkill.Holder],
        [new(ChartSkill.Holder, "Say nothing of the spec.", new(HeaderPart.Role, 2))]);
}
