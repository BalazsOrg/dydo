namespace DynaDocs.SkillBuild;

/// <summary>
/// The production model's parts (DYD-1082): every machine, kind class and skill. Machines come first,
/// because a skill's <see cref="Machine.Slots"/> reads its machine while this class initializes; they
/// are listed in the overview's order.
/// </summary>
public static class Registry
{
    public static IReadOnlyList<Machine> Machines { get; } =
    [
        DeliveryMachines.Overview, ProcessMachines.ClaimBuild, DeliveryMachines.Project, HierarchyMachines.Dispatch,
        DeliveryMachines.TicketStatus, DeliveryMachines.Hops, ProcessMachines.Proof, ProcessMachines.Docs, DeliveryMachines.Review,
        HierarchyMachines.Should, MergeMachines.MergeCommand, MergeMachines.FeatureMerge, ProcessMachines.Findings,
        HierarchyMachines.Liveness, HierarchyMachines.Envoy, LawMachines.Council, LawMachines.Tribunal, LawMachines.Law,
        LawMachines.Backfill, HierarchyMachines.Staff, DeliveryMachines.Landing, LawMachines.Mutation, MergeMachines.MasterRoute,
        LawMachines.SafeDelete, LawMachines.IgnoreEntry, ProcessMachines.Adopter
    ];

    public static IReadOnlyList<Type> Kinds { get; } =
    [
        typeof(Role), typeof(Overview), typeof(Build), typeof(Project), typeof(Dispatch), typeof(Brief), typeof(Roster),
        typeof(TicketStatus), typeof(Hops), typeof(Testing), typeof(Gates), typeof(Docs), typeof(Fix), typeof(Review),
        typeof(Should), typeof(MergeCommand), typeof(FeatureMerge), typeof(Findings), typeof(Report), typeof(Liveness),
        typeof(Envoy), typeof(Council), typeof(Tribunal), typeof(Law), typeof(Backfill), typeof(Staff), typeof(Landing),
        typeof(Inquisition), typeof(Mutation), typeof(MasterRoute), typeof(SafeDelete), typeof(IgnoreEntry), typeof(Adopter),
        typeof(Tree), typeof(Linear), typeof(Scout)
    ];

    public static IReadOnlyList<SkillSpec> Skills { get; } =
    [
        OfficerCarriers.Admiral, OfficerCarriers.ChiefOfStaff, OfficerCarriers.TicketCaptain,
        CrewCarriers.CodeWriter, CrewCarriers.Reviewer, CrewCarriers.Scout,
        StandardCarriers.WorkingTree, StandardCarriers.LinearWorkspace
    ];
}
