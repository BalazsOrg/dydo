namespace DynaDocs.Tests.SkillBuild.Charts;

using DynaDocs.SkillBuild;

/// <summary>A broken kind: a claim citing an id the spec has no area for.</summary>
public static class LeaseMiscited
{
    public static readonly Claim Misnamed = new("Cite a real rule.", [ChartSkill.Holder],
        [new(ChartSkill.Holder, "Cite a real rule.", new(HeaderPart.Role, 2))]) { Spec = ["K9", "X4"] };
}
