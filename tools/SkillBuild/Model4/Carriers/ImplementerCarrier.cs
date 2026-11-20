namespace DynaDocs.SkillBuild;

/// <summary>
/// The implementer crew kind (K30), which absorbs docs-writer (K22), and its dispatch template (K14).
/// Its phases hold its machine states (K9); a conflict, a survivor and an over-limit file are scenarios.
/// </summary>
public static class ImplementerCarrier
{
    public static readonly Resource Brief = new(ResourceName.ImplementerBrief, "Implementer brief", new Slot(Section.ImplementerBrief))
    {
        Skills = ["implementer"]
    };

    public static readonly SkillSpec Spec = SkillSpec.Crew(Skill.Implementer, "Implementer",
        "A writer hop - implement or fix a ticket's code or docs, or resolve a merge conflict, from an implementer brief.") with
    {
        Phases =
        [
            new("Take the brief",
            [
                new(Routine.Accountable, "Accountable for"), new(Routine.ToolsAndSurfaces, "Tools and surfaces"),
                .. HierarchyMachines.Dispatch.Slots(DispatchState.Briefed)
            ]),
            new("Work the hop", [.. HierarchyMachines.Dispatch.Slots(DispatchState.Working), .. ProcessMachines.Proof.Slots(ProofState.Conformance)])
        ],
        Scenarios =
        [
            new(ImplementerScenario.MergeConflict, "A merge conflict", MergeMachines.FeatureMerge.Slots(FeatureMergeState.Resolving)),
            new(ImplementerScenario.Survivor, "A surviving mutant", LawMachines.Mutation.Slots(MutationState.Ticketed)),
            new(ImplementerScenario.OverLimit, "A file over the token limit", LawMachines.IgnoreEntry.Slots(IgnoreEntryState.OverLimit))
        ]
    };
}
