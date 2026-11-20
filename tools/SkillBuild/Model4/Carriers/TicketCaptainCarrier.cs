namespace DynaDocs.SkillBuild;

/// <summary>
/// The ticket captain (K22, K9: DYD-1082 1bc04dce changes 1, 5, 7, 10) and the ticket-captain dispatch
/// template, which sub-captains and job-ticket captains reuse with their H23 job skill (K14). Its five
/// phases are the nominal path; a job ticket, an inquisition, a council and a PR into master each have
/// their own resource; its failure paths are scenarios.
/// </summary>
public static class TicketCaptainCarrier
{
    public static readonly Resource Brief = new(ResourceName.TicketCaptainBrief, "Ticket-captain brief", new Slot(Section.TicketCaptainBrief))
    {
        Skills = ["ticket-captain"]
    };

    private static readonly Resource JobTicketRecipe = new(TicketCaptainResource.JobTickets, "Job tickets",
    [
        new Slot(TicketCaptainSlot.Question, "Question"), .. HierarchyMachines.Envoy.Slots(EnvoyState.Research), .. LawMachines.Tribunal.Slots(TribunalState.HardCall),
        new(TicketCaptainSlot.Wayfinding, "Wayfinding"),
        new(TicketCaptainSlot.Hitl, "With him: Grilling, Walkthrough, Enablement and his-call")
    ]);

    private static readonly Resource InquisitionRecipe = new(TicketCaptainResource.InquisitionCaptain, "Inquisition captain",
    [
        new Slot(TicketCaptainSlot.Inquisition), .. DeliveryMachines.Landing.Slots(LandingState.Mapped, LandingState.Inquisition),
        new(TicketCaptainSlot.SliceBrief, "Slice reviewer brief")
    ]);

    private static readonly Resource CouncilChairRecipe = new(TicketCaptainResource.CouncilChair, "Council chair",
    [
        new Slot(TicketCaptainSlot.Council), new(TicketCaptainSlot.Tribunal, "Judge: the tribunal"),
        .. LawMachines.Tribunal.Slots(TribunalState.CaseFile, TribunalState.DecidedOvernight, TribunalState.Prepared, TribunalState.ProposedDr)
    ]);

    private static readonly Resource CouncilMemberBriefs = new(TicketCaptainResource.CouncilMembers, "Council members' briefs",
        new(TicketCaptainSlot.Member, "Member"), new(TicketCaptainSlot.Synthesiser, "Synthesiser"), new(TicketCaptainSlot.Judge, "Judge"));

    private static readonly Resource MasterRouteRecipe = new(TicketCaptainResource.MasterRoute, "A PR into master",
        MergeMachines.MasterRoute.Slots(MasterRouteState.Offered, MasterRouteState.Resync, MasterRouteState.CiWatch, MasterRouteState.Merged));

    public static readonly SkillSpec Spec = SkillSpec.Officer(Skill.TicketCaptain, "Ticket Captain",
        "A ticket to hold to its end - cut its hops, run its review rounds, merge it. Also a sub-captain's unit.") with
    {
        Phases =
        [
            new("Take the ticket", [new(TicketCaptainSlot.Take), .. HierarchyMachines.Envoy.Slots(EnvoyState.Council), .. HierarchyMachines.Dispatch.Slots(DispatchState.Briefed)])
            {
                Resources = [JobTicketRecipe, InquisitionRecipe, CouncilChairRecipe, CouncilMemberBriefs]
            },
            new("Cut the hops", DeliveryMachines.Hops.Slots(HopsState.Cut, HopsState.OneTicket, HopsState.ReviewHop)),
            new("Staff and dispatch", [new(Routine.Dispatching), .. HierarchyMachines.Dispatch.Slots(DispatchState.Staffing, DispatchState.Returned)])
            {
                Resources = [ImplementerCarrier.Brief, ReviewerCarrier.Brief, ResearchCarrier.Brief, ScoutCarrier.Brief, Brief]
            },
            new("Run the review rounds",
            [
                .. DeliveryMachines.Review.Slots(ReviewState.Wired, ReviewState.Round, ReviewState.Verdict),
                .. HierarchyMachines.Should.Slots(ShouldState.Found, ShouldState.Open)
            ]),
            new("Merge and clean up",
            [
                .. MergeMachines.FeatureMerge.Slots(FeatureMergeState.PreGate, FeatureMergeState.Running, FeatureMergeState.Gating),
                .. LawMachines.SafeDelete.Slots(SafeDeleteState.Live)
            ])
            {
                Resources = [MasterRouteRecipe]
            }
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
            new(TicketCaptainScenario.NeedForBalazs, "A need for Balazs",
                [new Slot(TicketCaptainSlot.NeedPassedUp), .. HierarchyMachines.Envoy.Slots(EnvoyState.Need)]),
            new(TicketCaptainScenario.Liveness, "Liveness", HierarchyMachines.Liveness.Slots(LivenessState.Quiet)),
            new(TicketCaptainScenario.OverLimit, "A file over the token limit", LawMachines.IgnoreEntry.Slots(IgnoreEntryState.OverLimit)),
            new(TicketCaptainScenario.LandingNotAccepted, "A landing walkthrough not accepted",
                DeliveryMachines.Landing.Slots(LandingState.Walkthrough, LandingState.NotAccepted))
        ]
    };
}
