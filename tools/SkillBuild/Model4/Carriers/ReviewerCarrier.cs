namespace DynaDocs.SkillBuild;

/// <summary>
/// The reviewer crew kind (K22), and its dispatch template (K14). Its spine takes the brief, works the
/// row of the level the brief names, and returns (K9). The toolset resource holds the level × tool
/// matrix with each tool's use and the Read, Break and Drive lenses (R5–R7, DYD-1082 1bc04dce change 8);
/// the inquisition's phase states sit beside the lenses. A conflict's hunks, an equivalent-mutant claim
/// and a new ignore entry are scenarios.
/// </summary>
public static class ReviewerCarrier
{
    public static readonly Resource Brief = new(ResourceName.ReviewerBrief, "Reviewer brief", new Slot(Section.ReviewerBrief)) { Skills = ["reviewer"] };

    public static readonly Resource Toolset = new(ReviewerPart.Toolset, "Toolset",
    [
        new Slot(ReviewerPart.Matrix), new(ReviewerPart.DriveAndFormal, "Drive and Formal"), new(ReviewerPart.Tools, "Using each tool"),
        new(ReviewerPart.Findings, "Tests and docs"), new(ReviewerPart.Lenses, "Lenses"),
        .. DeliveryMachines.Landing.Slots(LandingState.ReadPhase, LandingState.BreakPhase, LandingState.DrivePhase)
    ]);

    public static readonly SkillSpec Spec = SkillSpec.Crew(Skill.Reviewer, "Reviewer",
        "One candidate to judge at the level your brief names - a leaf's evidence check, a ticket review, a feature or Milestone review, or an inquisition slice; also a conflict's hunks, an equivalent-mutant claim or a new ignore entry.") with
    {
        Phases =
        [
            new("Take the brief", HierarchyMachines.Dispatch.Slots(DispatchState.Briefed)),
            new("Work your level's row", [.. DeliveryMachines.Review.Slots(ReviewState.Round), .. HierarchyMachines.Should.Slots(ShouldState.Found)])
            {
                Resources = [Toolset]
            },
            new("Return", HierarchyMachines.Dispatch.Slots(DispatchState.Returned))
        ],
        Scenarios =
        [
            new(ReviewerScenario.Hunks, "A conflict's hunks", MergeMachines.FeatureMerge.Slots(FeatureMergeState.HunkReview)),
            new(ReviewerScenario.Equivalent, "An equivalent-mutant claim", LawMachines.Mutation.Slots(MutationState.Claimed)),
            new(ReviewerScenario.IgnoreEntry, "A new ignore entry", LawMachines.IgnoreEntry.Slots(IgnoreEntryState.PathA))
        ]
    };
}
