namespace DynaDocs.SkillBuild;

/// <summary>
/// The reviewer crew kind (K30), which carries the council resource (L23), and its dispatch template
/// (K14). There is no inquisitor kind: Read, Break and Drive are its phases on an area (N5). Its phases
/// hold its machine states in the order it meets them (K9).
/// </summary>
public static class ReviewerCarrier
{
    public static readonly Resource Brief = new(ResourceName.ReviewerBrief, "Reviewer brief", new Slot(Section.ReviewerBrief)) { Skills = ["reviewer"] };

    public static readonly SkillSpec Spec = SkillSpec.Crew(Skill.Reviewer, "Reviewer",
        "A seat to judge - a review round, a conflict's hunks, an equivalent-mutant claim, an inquisition phase, or a council, clerk or judge seat.") with
    {
        Phases =
        [
            new("Take the brief",
            [
                new(Routine.Accountable, "Accountable for"), new(Routine.ToolsAndSurfaces, "Tools and surfaces"),
                .. HierarchyMachines.Dispatch.Slots(DispatchState.Briefed)
            ]),
            new("Judge the round", [.. DeliveryMachines.Review.Slots(ReviewState.Round), .. HierarchyMachines.Should.Slots(ShouldState.Found)]),
            new("Other seats",
            [
                .. MergeMachines.FeatureMerge.Slots(FeatureMergeState.HunkReview),
                .. LawMachines.Mutation.Slots(MutationState.Claimed),
                .. LawMachines.IgnoreEntry.Slots(IgnoreEntryState.PathA)
            ])
            {
                Resources =
                [
                    new(ResourceName.Council, "Council",
                        new(Section.CouncilUses, "Uses"), new(Section.CouncilMember, "Member"), new(Section.CouncilSynthesis, "Synthesis"),
                        new(Section.CouncilOneFamily, "One family out"), new(Section.CouncilJudge, "Judge: the tribunal"))
                ]
            },
            new("Inquisition phases", DeliveryMachines.Landing.Slots(LandingState.ReadPhase, LandingState.BreakPhase, LandingState.DrivePhase)),
            new("Return", HierarchyMachines.Dispatch.Slots(DispatchState.Returned))
        ]
    };
}
