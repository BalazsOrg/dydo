namespace DynaDocs.SkillBuild;

/// <summary>M7: the Merge hop into the feature branch.</summary>
public enum FeatureMergeState
{
    MergeTodo,
    PreGate,
    PreFix,
    Rereview,
    Running,
    Resolving,
    HunkReview,
    Gating,
    Rerun,
    Reverted,
    FixRound,
    MergeDone
}
