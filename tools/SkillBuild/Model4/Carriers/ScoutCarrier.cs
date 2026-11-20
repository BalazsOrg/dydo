namespace DynaDocs.SkillBuild;

/// <summary>
/// The scout crew kind (K30): research's one-lookup helper and the spec's Questions pass (R4), and its
/// dispatch template (K14). Its phases hold its machine states (K9).
/// </summary>
public static class ScoutCarrier
{
    public static readonly Resource Brief = new(ResourceName.ScoutBrief, "Scout brief", new Slot(Section.ScoutBrief)) { Skills = ["scout"] };

    public static readonly SkillSpec Spec = SkillSpec.Crew(Skill.Scout, "Scout",
        "One question to answer from its sources, off the record - a spec's scout pass, or an officer's homework.") with
    {
        Phases =
        [
            new("Take the question",
            [
                new(Routine.Accountable, "Accountable for"), new(Routine.ToolsAndSurfaces, "Tools and surfaces"),
                .. HierarchyMachines.Dispatch.Slots(DispatchState.Briefed)
            ]),
            new("Answer it", HierarchyMachines.Dispatch.Slots(DispatchState.Working))
        ]
    };
}
