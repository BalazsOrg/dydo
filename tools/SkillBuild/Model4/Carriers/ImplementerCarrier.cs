namespace DynaDocs.SkillBuild;

/// <summary>
/// The implementer crew kind (K22: code-writer renamed, docs-writer folded in), and its dispatch template
/// (K14). Its spine takes the brief, builds and verifies, then commits and returns (K9); the code, docs,
/// Bug and merge recipes are the build phase's resources, and a survivor and an over-limit file are
/// scenarios (DYD-1082 1bc04dce change 4).
/// </summary>
public static class ImplementerCarrier
{
    public static readonly Resource Brief = new(ResourceName.ImplementerBrief, "Implementer brief", new Slot(Section.ImplementerBrief))
    {
        Skills = ["implementer"]
    };

    public static readonly SkillSpec Spec = SkillSpec.Crew(Skill.Implementer, "Implementer",
        "A ticket hop to build: create, write or change its files, mostly code, and verify them as you go - an Implement or Fix hop, now and then a merge conflict.") with
    {
        Phases =
        [
            new("Take the brief", HierarchyMachines.Dispatch.Slots(DispatchState.Briefed)),
            new("Build and verify", new Slot(ImplementerPart.Build))
            {
                Resources =
                [
                    new(ImplementerPart.Code, "Writing code", new Slot(ImplementerPart.Code)),
                    new(ImplementerPart.Docs, "Writing docs", new Slot(ImplementerPart.Docs)),
                    new(ImplementerPart.Bug, "A Bug", new Slot(ImplementerPart.Bug)),
                    new(ImplementerPart.Merge, "A merge conflict", MergeMachines.FeatureMerge.Slots(FeatureMergeState.Resolving))
                ]
            },
            new("Commit and return", HierarchyMachines.Dispatch.Slots(DispatchState.Working))
        ],
        Scenarios =
        [
            new(ImplementerScenario.Survivor, "A surviving mutant", LawMachines.Mutation.Slots(MutationState.Ticketed)),
            new(ImplementerScenario.OverLimit, "A file over the token limit", LawMachines.IgnoreEntry.Slots(IgnoreEntryState.OverLimit))
        ]
    };
}
