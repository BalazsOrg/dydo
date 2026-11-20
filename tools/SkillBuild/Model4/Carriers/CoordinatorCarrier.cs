namespace DynaDocs.SkillBuild;

/// <summary>
/// The coordinator (K22, H5), explicit-only (K27, N12 DR 045 §9), and its dispatch template (K14), which the
/// chief of staff dispatches from. Its five job phases are the walkthrough's (DYD-1082 round 5, Q21), holding
/// its machine states in the order it meets them (K9); its failure paths are scenarios. The tribunal is the
/// council chair's Judge procedure on the ticket captain (L23), so no tribunal state is the coordinator's.
/// </summary>
public static class CoordinatorCarrier
{
    public static readonly Resource Brief = new(ResourceName.CoordinatorBrief, "Coordinator brief", new Slot(ChiefOfStaffSlot.CoordinatorBrief)) { Skills = ["coordinator"] };

    public static readonly SkillSpec Spec = SkillSpec.Officer(Skill.Coordinator, "Coordinator",
        "Hold one open Project to its landing, through its spec, map, captains, envoys and merges.") with
    {
        ExplicitOnly = true,
        Phases =
        [
            new("Wake up stateless", [new(CoordinatorSlot.WakeUp), .. DeliveryMachines.Project.Slots(ProjectState.Unheld)]),
            new("Keep the lanes full", [new(CoordinatorSlot.Lanes), new(Routine.Dispatching, "Dispatching captains")])
            {
                Resources = [TicketCaptainCarrier.Brief]
            },
            new("Watch", [.. DeliveryMachines.Project.Slots(ProjectState.Watching), .. HierarchyMachines.Liveness.Slots(LivenessState.Quiet)]),
            new("Route out every judgement",
            [
                new(CoordinatorSlot.Routing),
                .. HierarchyMachines.Envoy.Slots(EnvoyState.Research, EnvoyState.Queued, EnvoyState.Active, EnvoyState.Reported)
            ])
            {
                Resources = [new(ResourceName.EnvoyBrief, "Envoy brief", new(Section.EnvoySlots, "Slots"), new(Section.EnvoySession, "Session"), new(Section.EnvoySkills, "Skills"))]
            },
            new("Pace merges and the landing",
            [
                new(CoordinatorSlot.Merges),
                .. HierarchyMachines.Should.Slots(ShouldState.Open),
                .. MergeMachines.MasterRoute.Slots(MasterRouteState.Paced),
                .. DeliveryMachines.Landing.Slots(LandingState.Integrated, LandingState.Inquisition, LandingState.Prerequisites,
                    LandingState.MergeReview, LandingState.Walkthrough, LandingState.Merging, LandingState.Landed)
            ])
        ],
        Scenarios =
        [
            new(CoordinatorScenario.ReCut, "Too big for one agent", HierarchyMachines.Dispatch.Slots(DispatchState.ReCut)),
            new(CoordinatorScenario.Replanning, "A fundamental blocker or a new requirement", DeliveryMachines.Project.Slots(ProjectState.Replanning)),
            new(CoordinatorScenario.RejectedFiller, "A filler sent back", new Slot(CoordinatorSlot.Filler)),
            new(CoordinatorScenario.ForcedResync, "A third forced re-sync", MergeMachines.MasterRoute.Slots(MasterRouteState.Held)),
            new(CoordinatorScenario.Survivors, "Surviving mutants", LawMachines.Mutation.Slots(MutationState.Survivors))
        ]
    };
}
