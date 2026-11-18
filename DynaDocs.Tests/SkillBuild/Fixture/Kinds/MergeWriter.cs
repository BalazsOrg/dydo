namespace DynaDocs.Tests.SkillBuild.Fixture;

using DynaDocs.SkillBuild;
using A = DynaDocs.Tests.SkillBuild.Fixture.FixtureActor;
using F = DynaDocs.Tests.SkillBuild.Fixture.FixtureSection;
using S = DynaDocs.Tests.SkillBuild.Fixture.FixtureSkill;

/// <summary>Migration fixture: the inventory's <c>merge</c> claims, sub-topics writer.</summary>
public static class MergeWriter
{
    public static readonly Claim WriterMapsConflicts = new(
        "The merge writer maps the conflicts, the shared seams and what changed since the source review; mapping is done when the merge can be made without choosing policy.",
        [A.CodeWriter],
        [
            new(S.CodeWriter, "Map the conflicts, the shared seams and what changed since the source review.", new(F.CodeWriterMergeLead, 1))
        ]);

    public static readonly Claim WriterPinsAndReadsBothSides = new(
        "A merge hop pins source and target branches at their SHAs, the source's PASS, the governing contract and the combined gates (proved before the first edit in place of the five working-tree checks) and reads both sides before naming a resolution.",
        [A.CodeWriter],
        [
            new(S.CodeWriter, "Merge proves the pins in [merge](resources/merge.md); proof-only, `HEAD` on its brief's child branch off the brief's pinned source SHA, a clean worktree and only the brief's test path touched.", new(F.CodeWriterBeforeTheFirstEdit, 7), () => [WorktreeAuditToReviewer.ProofWorkHeadOnProofBranch, WorktreeAuditToReviewer.ProofWorkTouchesTestPathsOnly]),
            new(S.WorkingTreeStandard, "Merge work proves instead the pins of the code-writer's `merge` resource: source and target at their SHAs, the source's PASS, the governing contract and the combined gates.", new(F.WorkingTreeStandardBeforeTheFirstEdit, 7))
        ]);

    public static readonly Claim WriterStatesResolutionBehaviour = new(
        "The merge writer states each resolution's intended behaviour.",
        [A.CodeWriter], []);
}
