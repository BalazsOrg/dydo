namespace DynaDocs.SkillBuild;

/// <summary>
/// The ticket captain (K22), and the ticket-captain dispatch template, which sub-captains and HITL
/// captains reuse with their H23 job skill (K14). Its phases hold its machine states in the order it
/// meets them (K9); its failure paths are scenarios.
/// </summary>
public static class TicketCaptainCarrier
{
    public static readonly Resource Brief = new(ResourceName.TicketCaptainBrief, "Ticket-captain brief", new Slot(Section.TicketCaptainBrief))
    {
        Skills = ["ticket-captain"]
    };

    public static readonly SkillSpec Spec = SkillSpec.Officer(Skill.TicketCaptain, "Ticket Captain",
        "A ticket to hold to its end - cut its hops, run its review rounds, merge it. Also a sub-captain's unit.") with
    {
        Phases =
        [
            new("Take the ticket",
            [
                new(Routine.Accountable, "Accountable for"), new(Routine.ToolsAndSurfaces, "Tools and surfaces"),
                .. HierarchyMachines.Dispatch.Slots(DispatchState.Briefed)
            ]),
            new("Cut the hops", DeliveryMachines.Hops.Slots(HopsState.Cut, HopsState.OneTicket, HopsState.ReviewHop)),
            new("Staff and dispatch",
            [
                .. HierarchyMachines.Dispatch.Slots(DispatchState.Staffing, DispatchState.Returned),
                new(Routine.Dispatching, "Dispatching crew with tickets")
            ])
            {
                Resources = [ImplementerCarrier.Brief, ReviewerCarrier.Brief, ResearchCarrier.Brief, ScoutCarrier.Brief, Brief]
            },
            new("Run the review rounds",
            [
                .. DeliveryMachines.Review.Slots(ReviewState.Wired, ReviewState.Round, ReviewState.Verdict),
                .. HierarchyMachines.Should.Slots(ShouldState.Found, ShouldState.Open)
            ]),
            new("Merge into the feature branch", MergeMachines.FeatureMerge.Slots(FeatureMergeState.PreGate, FeatureMergeState.Running, FeatureMergeState.Gating)),
            new("Route a PR into master",
                MergeMachines.MasterRoute.Slots(MasterRouteState.Offered, MasterRouteState.Resync, MasterRouteState.CiWatch, MasterRouteState.Merged)),
            new("Clean up", LawMachines.SafeDelete.Slots(SafeDeleteState.Live))
        ],
        Scenarios =
        [
            new(TicketCaptainScenario.ReCut, "Too big for one agent", HierarchyMachines.Dispatch.Slots(DispatchState.ReCut)),
            new(TicketCaptainScenario.FixRounds, "Fix rounds", DeliveryMachines.Review.Slots(ReviewState.Fixing, ReviewState.Checkpoint)),
            new(TicketCaptainScenario.MergeConflict, "A merge conflict", MergeMachines.FeatureMerge.Slots(FeatureMergeState.Resolving)),
            new(TicketCaptainScenario.RedGate, "A red gate after the merge",
                MergeMachines.FeatureMerge.Slots(FeatureMergeState.Rerun, FeatureMergeState.Reverted, FeatureMergeState.FixRound)),
            new(TicketCaptainScenario.ForcedResync, "A third forced re-sync", MergeMachines.MasterRoute.Slots(MasterRouteState.Held)),
            new(TicketCaptainScenario.MasterRevert, "A master merge reverted", MergeMachines.MasterRoute.Slots(MasterRouteState.Reverted)),
            new(TicketCaptainScenario.NeedForBalazs, "A need for Balazs", HierarchyMachines.Envoy.Slots(EnvoyState.Homework)),
            new(TicketCaptainScenario.Liveness, "Liveness", HierarchyMachines.Liveness.Slots(LivenessState.Quiet)),
            new(TicketCaptainScenario.OverLimit, "A file over the token limit", LawMachines.IgnoreEntry.Slots(IgnoreEntryState.OverLimit))
        ]
    };
}
