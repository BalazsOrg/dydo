namespace DynaDocs.SkillBuild;

/// <summary>
/// The scout crew kind (K22, DYD-1082 99774812): one question and one lookup, research's helper for one
/// source family and the spec's Questions pass (R4), and its dispatch template (K14). Its spine takes the
/// question and looks it up (K9).
/// </summary>
public static class ScoutCarrier
{
    public static readonly Resource Brief = new(ResourceName.ScoutBrief, "Scout brief", new Slot(Section.ScoutBrief)) { Skills = ["scout"] };

    public static readonly SkillSpec Spec = SkillSpec.Crew(Skill.Scout, "Scout",
        "One question, one lookup, off the record - one source family for research, or a spec's Questions pass.") with
    {
        Phases =
        [
            new("Take the question", HierarchyMachines.Dispatch.Slots(DispatchState.Briefed)),
            new("Look it up", HierarchyMachines.Dispatch.Slots(DispatchState.Working))
        ]
    };
}
