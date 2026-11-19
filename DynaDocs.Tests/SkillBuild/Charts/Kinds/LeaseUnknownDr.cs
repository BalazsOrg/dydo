namespace DynaDocs.Tests.SkillBuild.Charts;

using DynaDocs.SkillBuild;

/// <summary>A broken kind: a claim whose only reference is N12 with a DR the spec never names.</summary>
public static class LeaseUnknownDr
{
    public static readonly Claim Uncarried = new("Cite a DR the spec carries.", [ChartSkill.Holder],
        [new(ChartSkill.Holder, "Cite a DR the spec carries.", new(HeaderPart.Role, 2))]) { Spec = ["N12 DR 999"] };
}
