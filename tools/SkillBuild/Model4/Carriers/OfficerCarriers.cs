namespace DynaDocs.SkillBuild;

/// <summary>
/// The three officer skills (K22): each spine's states are machine states, in the order the role
/// meets them (K9); admiral and chief of staff are explicit-only (K27, N12 DR 045 §9).
/// </summary>
public static class OfficerCarriers
{
    private static readonly Slot[] Dispatching = [new(Routine.Dispatching, "Dispatching crew with tickets")];

    private static readonly Resource ReviewerBrief = new(ResourceName.ReviewerBrief, "Reviewer brief", [new(Section.ReviewerBrief)]);
    private static readonly Resource ScoutBrief = new(ResourceName.ScoutBrief, "Scout brief", [new(Section.ScoutBrief)]);
    private static readonly Resource CaptainBrief = new(ResourceName.TicketCaptainBrief, "Ticket-captain brief", [new(Section.TicketCaptainBrief)]);

    public static readonly SkillSpec Admiral = SkillSpec.Officer(Skill.Admiral, "Admiral",
        "Hold one open Project to its landing, through its spec, map, captains, envoys and merges.") with
    {
        ExplicitOnly = true,
        States =
        [
            .. DeliveryMachines.Project.Slots(ProjectState.Charting, ProjectState.Watching),
            .. HierarchyMachines.Dispatch.Slots(DispatchState.Staffing, DispatchState.ReCut),
            .. DeliveryMachines.Hops.Slots(HopsState.Cut),
            .. HierarchyMachines.Liveness.Slots(LivenessState.Quiet),
            .. DeliveryMachines.Project.Slots(ProjectState.Replanning),
            .. HierarchyMachines.Envoy.Slots(EnvoyState.Homework, EnvoyState.Queued, EnvoyState.Active, EnvoyState.Reported),
            .. LawMachines.Tribunal.Slots(TribunalState.HardCall, TribunalState.DecidedOvernight, TribunalState.Prepared, TribunalState.ProposedDr),
            .. LawMachines.Law.Slots(LawState.Evidence, LawState.Candidate),
            .. HierarchyMachines.Should.Slots(ShouldState.Open),
            .. MergeMachines.MasterRoute.Slots(MasterRouteState.Paced, MasterRouteState.Held),
            .. DeliveryMachines.Landing.Slots(LandingState.Integrated, LandingState.Inquisition, LandingState.Prerequisites),
            .. LawMachines.Mutation.Slots(MutationState.Survivors),
            .. DeliveryMachines.Landing.Slots(LandingState.MergeReview, LandingState.Walkthrough, LandingState.Merging, LandingState.Landed)
        ],
        Routines = [new(Routine.Wayfinding, "Wayfinding"), .. Dispatching],
        Resources =
        [
            new(ResourceName.EnvoyBrief, "Envoy brief",
                [new(Section.EnvoySlots, "Slots"), new(Section.EnvoySession, "Session"), new(Section.EnvoySkills, "Skills")]),
            CaptainBrief, ReviewerBrief, ScoutBrief
        ]
    };

    public static readonly SkillSpec ChiefOfStaff = SkillSpec.Officer(Skill.ChiefOfStaff, "Chief of Staff",
        "Your short list across Projects, and the overnight watch.") with
    {
        ExplicitOnly = true,
        States =
        [
            .. HierarchyMachines.Staff.Slots(StaffState.Day, StaffState.Investigating, StaffState.Muster, StaffState.NightWatch, StaffState.MorningBrief),
            .. HierarchyMachines.Liveness.Slots(LivenessState.Quiet)
        ],
        Routines = Dispatching
    };

    public static readonly SkillSpec TicketCaptain = SkillSpec.Officer(Skill.TicketCaptain, "Ticket Captain",
        "A ticket to hold to its end - cut its hops, run its review rounds, merge it. Also a sub-captain's unit.") with
    {
        States =
        [
            .. HierarchyMachines.Dispatch.Slots(DispatchState.Briefed),
            .. DeliveryMachines.Hops.Slots(HopsState.Cut, HopsState.OneTicket, HopsState.ReviewHop),
            .. HierarchyMachines.Dispatch.Slots(DispatchState.Staffing, DispatchState.ReCut, DispatchState.Returned),
            .. DeliveryMachines.Review.Slots(ReviewState.Wired, ReviewState.Round, ReviewState.Verdict, ReviewState.Fixing, ReviewState.Checkpoint),
            .. HierarchyMachines.Should.Slots(ShouldState.Found, ShouldState.Open),
            .. MergeMachines.FeatureMerge.Slots(FeatureMergeState.PreGate, FeatureMergeState.Running,
                FeatureMergeState.Resolving, FeatureMergeState.Gating, FeatureMergeState.Rerun, FeatureMergeState.Reverted, FeatureMergeState.FixRound),
            .. MergeMachines.MasterRoute.Slots(MasterRouteState.Offered, MasterRouteState.Resync, MasterRouteState.Held,
                MasterRouteState.CiWatch, MasterRouteState.Merged, MasterRouteState.Reverted),
            .. LawMachines.SafeDelete.Slots(SafeDeleteState.Live),
            .. HierarchyMachines.Envoy.Slots(EnvoyState.Homework),
            .. HierarchyMachines.Liveness.Slots(LivenessState.Quiet),
            .. LawMachines.IgnoreEntry.Slots(IgnoreEntryState.OverLimit)
        ],
        Routines = Dispatching,
        Resources = [new(ResourceName.CodeWriterBrief, "Code-writer brief", [new(Section.CodeWriterBrief)]), ReviewerBrief, ScoutBrief, CaptainBrief]
    };
}
