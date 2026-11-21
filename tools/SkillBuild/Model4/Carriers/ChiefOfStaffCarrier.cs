namespace DynaDocs.SkillBuild;

/// <summary>
/// The chief of staff (K22, H7), explicit-only (K27, N12 DR 045 §9). It starts or wakes a coordinator (H4,
/// DYD-1082 555a8300 gap 2), so it links the coordinator's dispatch template (K14). Its five phases are
/// the day's short list, investigating, and the night it owns from muster to morning brief (DYD-1082
/// a1b21b59 Q12, DYD-1195 33520d6c), each holding its machine state (K9). Its muster resource is truth-checked
/// ahead of the skill (DYD-1402): the rest is not yet migrated.
/// </summary>
public static class ChiefOfStaffCarrier
{
    private static readonly Resource Muster = new(ChiefOfStaffResource.Muster, "Muster",
        Slot.Paragraph(ChiefOfStaffResource.Muster), Slot.Bullets(ChiefOfStaffSlot.MusterChecklist),
        Slot.Bullets(ChiefOfStaffSlot.MusterQuestions, "Questions for him"), Slot.Paragraph(ChiefOfStaffSlot.MusterBlock, "Question block"),
        Slot.Paragraph(ChiefOfStaffSlot.MusterRequest, "Request to each coordinator"), Slot.Paragraph(ChiefOfStaffSlot.MusterList, "List for him"))
    {
        Readers = [Actor.ChiefOfStaff]
    };

    public static readonly SkillSpec Spec = SkillSpec.Officer(Skill.ChiefOfStaff, "Chief of Staff",
        "Your short list across Projects, and the overnight watch.") with
    {
        ExplicitOnly = true,
        Phases =
        [
            new("Keep his short list",
            [
                new(ChiefOfStaffSlot.Tools),
                .. HierarchyMachines.Staff.Slots(StaffState.Day),
                .. DeliveryMachines.Project.Slots(ProjectState.Unheld),
                new(Routine.Dispatching, "Dispatching a coordinator")
            ])
            {
                Resources = [CoordinatorCarrier.Brief]
            },
            new("Investigate for him", HierarchyMachines.Staff.Slots(StaffState.Investigating)),
            new("Hold the muster", HierarchyMachines.Staff.Slots(StaffState.Muster))
            {
                Resources = [Muster]
            },
            new("Keep the night watch", HierarchyMachines.Staff.Slots(StaffState.NightWatch))
            {
                Resources = [new(ChiefOfStaffResource.NightSurvey, "Night survey", new Slot(ChiefOfStaffResource.NightSurvey))]
            },
            new("Give the morning brief", HierarchyMachines.Staff.Slots(StaffState.MorningBrief))
            {
                Resources = [new(ChiefOfStaffResource.MorningBrief, "Morning brief", new Slot(ChiefOfStaffResource.MorningBrief))]
            }
        ],
        Scenarios =
        [
            new(ChiefOfStaffScenario.Liveness, "Liveness",
                [.. HierarchyMachines.Liveness.Slots(LivenessState.Quiet), ScenarioSlots.Close(ScenarioClose.Liveness)])
        ]
    };
}
