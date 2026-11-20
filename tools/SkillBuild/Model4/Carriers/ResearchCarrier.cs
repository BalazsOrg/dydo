namespace DynaDocs.SkillBuild;

/// <summary>
/// The research crew kind (K30): it pools and verifies, and its spine is that one job line. Its
/// dispatch template names its skill only (K14).
/// </summary>
public static class ResearchCarrier
{
    public static readonly Resource Brief = new(ResourceName.ResearchBrief, "Research brief") { Skills = ["research"] };

    public static readonly SkillSpec Spec = SkillSpec.Crew(Skill.Research, "Research", "Pool and verify.");
}
