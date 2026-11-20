namespace DynaDocs.SkillBuild;

/// <summary>
/// The coordinator (K22, H5), explicit-only (K27, N12 DR 045 §9), and its dispatch template (K14). Its
/// five job phases are the walkthrough's (DYD-1082 round 5, Q21), holding its machine states in the order
/// it meets them (K9); its failure paths are scenarios.
/// </summary>
public static class CoordinatorCarrier
{
    public static readonly Resource Brief = new(ResourceName.CoordinatorBrief, "Coordinator brief") { Skills = ["coordinator"] };

    public static readonly SkillSpec Spec = SkillSpec.Officer(Skill.Coordinator, "Coordinator",
        "Hold one open Project to its landing, through its spec, map, captains, envoys and merges.") with
    {
        ExplicitOnly = true,
        Phases =
        [
            new("Wake up stateless",
            [
                new(Routine.Accountable, "Accountable for"), new(Routine.ToolsAndSurfaces, "Tools and surfaces"),
                .. DeliveryMachines.Project.Slots(ProjectState.Charting)
            ]),
            new("Keep the lanes full",
            [
                .. HierarchyMachines.Dispatch.Slots(DispatchState.Staffing),
                .. DeliveryMachines.Hops.Slots(HopsState.Cut),
                new(Routine.Dispatching, "Dispatching crew with tickets")
            ])
            {
                Resources = [TicketCaptainCarrier.Brief, ReviewerCarrier.Brief, ScoutCarrier.Brief]
            },
            new("Watch", DeliveryMachines.Project.Slots(ProjectState.Watching)),
            new("Route out every judgement",
            [
                new(Routine.Wayfinding, "Wayfinding"),
                .. HierarchyMachines.Envoy.Slots(EnvoyState.Homework, EnvoyState.Queued, EnvoyState.Active, EnvoyState.Reported),
                .. LawMachines.Tribunal.Slots(TribunalState.HardCall, TribunalState.CaseFile, TribunalState.DecidedOvernight, TribunalState.Prepared, TribunalState.ProposedDr),
                .. LawMachines.Law.Slots(LawState.Evidence, LawState.Candidate)
            ])
            {
                Resources = [new(ResourceName.EnvoyBrief, "Envoy brief", new(Section.EnvoySlots, "Slots"), new(Section.EnvoySession, "Session"), new(Section.EnvoySkills, "Skills"))]
            },
            new("Pace merges and the landing",
            [
                .. HierarchyMachines.Should.Slots(ShouldState.Open),
                .. MergeMachines.MasterRoute.Slots(MasterRouteState.Paced),
                .. DeliveryMachines.Landing.Slots(LandingState.Integrated, LandingState.Inquisition, LandingState.Prerequisites,
                    LandingState.MergeReview, LandingState.Walkthrough, LandingState.Merging, LandingState.Landed)
            ])
        ],
        Scenarios =
        [
            new(CoordinatorScenario.ReCut, "Too big for one agent", HierarchyMachines.Dispatch.Slots(DispatchState.ReCut)),
            new(CoordinatorScenario.Liveness, "Liveness", HierarchyMachines.Liveness.Slots(LivenessState.Quiet)),
            new(CoordinatorScenario.Replanning, "A fundamental blocker or a new requirement", DeliveryMachines.Project.Slots(ProjectState.Replanning)),
            new(CoordinatorScenario.ForcedResync, "A third forced re-sync", MergeMachines.MasterRoute.Slots(MasterRouteState.Held)),
            new(CoordinatorScenario.Survivors, "Surviving mutants", LawMachines.Mutation.Slots(MutationState.Survivors))
        ]
    };
}
