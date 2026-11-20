namespace DynaDocs.Tests.SkillBuild.Charts;

using DynaDocs.SkillBuild;

/// <summary>A broken kind: a claim whose only id is shaped like a rule but names none the spec has.</summary>
public static class LeaseOutOfSpec
{
    public static readonly Claim Unnumbered = new("Cite a rule the spec numbers.", [ChartSkill.Holder],
        [new(ChartSkill.Holder, "Cite a rule the spec numbers.", new(SpinePart.Job, 2))]) { Spec = ["K999"] };
}
