namespace DynaDocs.SkillBuild;

/// <summary>
/// The production model's parts (DYD-1082): every machine, kind class and skill. Machines come first,
/// because a skill's <see cref="Machine.Slots"/> reads its machine while this class initializes; they
/// are listed in the overview's order. A new kind is its file under <c>Claims/</c> and its one line in
/// <see cref="Kinds"/>; a new skill is its carrier under <c>Model4/Carriers/</c> and its one line in
/// <see cref="Skills"/>.
/// </summary>
public static class Registry
{
    public static IReadOnlyList<Machine> Machines { get; } =
    [
        DeliveryMachines.Overview, ProcessMachines.ClaimBuild, ProcessMachines.RoleEvaluation, ToolMachines.HostSetup, ToolMachines.StatechartRun,
        DeliveryMachines.Project, HierarchyMachines.Dispatch,
        DeliveryMachines.TicketStatus, DeliveryMachines.Hops, ProcessMachines.Proof, ProcessMachines.Docs, DeliveryMachines.Review,
        HierarchyMachines.Should, MergeMachines.MergeCommand, MergeMachines.FeatureMerge, ProcessMachines.Findings,
        HierarchyMachines.Liveness, ToolMachines.Map, HierarchyMachines.Envoy, LawMachines.Council, LawMachines.Tribunal, LawMachines.Law, LawMachines.SlugCheck,
        LawMachines.Backfill, HierarchyMachines.Staff, DeliveryMachines.Landing, LawMachines.Mutation, MergeMachines.MasterRoute,
        LawMachines.SafeDelete, LawMachines.IgnoreEntry, ProcessMachines.Adopter
    ];

    public static IReadOnlyList<Type> Kinds { get; } =
    [
        typeof(Role),
        typeof(Overview),
        typeof(Build),
        typeof(Evaluation),
        typeof(Host),
        typeof(ChartTool),
        typeof(Project),
        typeof(Coordination),
        typeof(Dispatch),
        typeof(Brief),
        typeof(Roster),
        typeof(TicketStatus),
        typeof(Hops),
        typeof(Testing),
        typeof(Gates),
        typeof(Docs),
        typeof(Fix),
        typeof(Crew),
        typeof(Implementer),
        typeof(Toolset),
        typeof(Research),
        typeof(Review),
        typeof(Should),
        typeof(MergeCommand),
        typeof(FeatureMerge),
        typeof(Findings),
        typeof(Report),
        typeof(Liveness),
        typeof(MapView),
        typeof(Envoy),
        typeof(Council),
        typeof(Tribunal),
        typeof(Law),
        typeof(Slugs),
        typeof(Backfill),
        typeof(Staff),
        typeof(Landing),
        typeof(Inquisition),
        typeof(JobTickets),
        typeof(Mutation),
        typeof(MasterRoute),
        typeof(SafeDelete),
        typeof(IgnoreEntry),
        typeof(Adopter),
        typeof(Tree),
        typeof(Linear),
        typeof(Scout),
        typeof(Recovery)
    ];

    public static IReadOnlyList<SkillSpec> Skills { get; } =
    [
        CoordinatorCarrier.Spec,
        ChiefOfStaffCarrier.Spec,
        TicketCaptainCarrier.Spec,
        ImplementerCarrier.Spec,
        ReviewerCarrier.Spec,
        ResearchCarrier.Spec,
        ScoutCarrier.Spec,
        StandardCarriers.WorkingTree,
        StandardCarriers.LinearWorkspace
    ];
}
