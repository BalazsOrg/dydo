namespace DynaDocs.SkillBuild;

/// <summary>
/// The chief of staff (K22, H7), explicit-only (K27, N12 DR 045 §9). It starts or wakes a coordinator,
/// so it links the coordinator's dispatch template (K14). Its phases hold its machine states in the
/// order it meets them (K9).
/// </summary>
public static class ChiefOfStaffCarrier
{
    public static readonly SkillSpec Spec = SkillSpec.Officer(Skill.ChiefOfStaff, "Chief of Staff",
        "Your short list across Projects, and the overnight watch.") with
    {
        ExplicitOnly = true,
        Phases =
        [
            new("Day",
            [
                new(Routine.Accountable, "Accountable for"), new(Routine.ToolsAndSurfaces, "Tools and surfaces"),
                .. HierarchyMachines.Staff.Slots(StaffState.Day, StaffState.Investigating),
                new(Routine.Dispatching, "Dispatching crew with tickets")
            ])
            {
                Resources = [CoordinatorCarrier.Brief]
            },
            new("Night", HierarchyMachines.Staff.Slots(StaffState.Muster, StaffState.NightWatch, StaffState.MorningBrief))
        ],
        Scenarios = [new(ChiefOfStaffScenario.Liveness, "Liveness", HierarchyMachines.Liveness.Slots(LivenessState.Quiet))]
    };
}
