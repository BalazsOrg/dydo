namespace DynaDocs.SkillBuild;

/// <summary>
/// The three crew skills (K22): code-writer absorbs docs-writer, reviewer absorbs inquisitor and
/// carries the council resource (L23), scout absorbs research. Each spine's states are machine states.
/// </summary>
public static class CrewCarriers
{
    public static readonly SkillSpec CodeWriter = SkillSpec.Crew(Skill.CodeWriter, "Code Writer",
        "A writer hop - implement or fix a ticket's code or docs, or resolve a merge conflict, from a code-writer brief.") with
    {
        States =
        [
            .. HierarchyMachines.Dispatch.Slots(DispatchState.Briefed, DispatchState.Working),
            .. ProcessMachines.Proof.Slots(ProofState.Conformance),
            .. MergeMachines.FeatureMerge.Slots(FeatureMergeState.Resolving),
            .. LawMachines.Mutation.Slots(MutationState.Ticketed),
            .. LawMachines.IgnoreEntry.Slots(IgnoreEntryState.OverLimit)
        ]
    };

    public static readonly SkillSpec Reviewer = SkillSpec.Crew(Skill.Reviewer, "Reviewer",
        "A seat to judge - a review round, a conflict's hunks, an equivalent-mutant claim, an inquisition phase, or a council, clerk or judge seat.") with
    {
        States =
        [
            .. HierarchyMachines.Dispatch.Slots(DispatchState.Briefed),
            .. DeliveryMachines.Review.Slots(ReviewState.Round),
            .. HierarchyMachines.Should.Slots(ShouldState.Found),
            .. MergeMachines.FeatureMerge.Slots(FeatureMergeState.HunkReview),
            .. LawMachines.Mutation.Slots(MutationState.Claimed),
            .. LawMachines.IgnoreEntry.Slots(IgnoreEntryState.PathA),
            .. DeliveryMachines.Landing.Slots(LandingState.ReadPhase, LandingState.BreakPhase, LandingState.DrivePhase),
            .. HierarchyMachines.Dispatch.Slots(DispatchState.Returned)
        ],
        Resources =
        [
            new(ResourceName.Council, "Council",
            [
                new(Section.CouncilUses, "Uses"), new(Section.CouncilMember, "Member"), new(Section.CouncilSynthesis, "Synthesis"),
                new(Section.CouncilOneFamily, "One family out"), new(Section.CouncilJudge, "Judge: the tribunal")
            ])
        ]
    };

    public static readonly SkillSpec Scout = SkillSpec.Crew(Skill.Scout, "Scout",
        "One question to answer from its sources, off the record - a spec's scout pass, or an officer's homework.") with
    {
        States = [.. HierarchyMachines.Dispatch.Slots(DispatchState.Briefed, DispatchState.Working)]
    };
}
