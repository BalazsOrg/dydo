namespace DynaDocs.SkillBuild;

/// <summary>
/// The research crew kind (K22, DYD-1082 99774812): it pools evidence from every source family and
/// verifies it, sending scouts from the scout template for the single lookups. Its spine takes the
/// question, sends the scouts, then verifies and answers (K9); its dispatch template names its skill (K14).
/// </summary>
public static class ResearchCarrier
{
    public static readonly Resource Brief = new(ResourceName.ResearchBrief, "Research brief") { Skills = ["research"] };

    public static readonly SkillSpec Spec = SkillSpec.Crew(Skill.Research, "Research",
        "A question a choice waits on - pool evidence from every source family that could own the answer, verify each claim at its source, and cite it.") with
    {
        Phases =
        [
            new("Take the question", HierarchyMachines.Dispatch.Slots(DispatchState.Briefed)),
            new("Send scouts", new Slot(ResearchPart.Families)) { Resources = [ScoutCarrier.Brief] },
            new("Verify and answer", HierarchyMachines.Dispatch.Slots(DispatchState.Working))
        ]
    };
}
