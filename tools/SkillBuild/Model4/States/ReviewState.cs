namespace DynaDocs.SkillBuild;

/// <summary>M5: the review loop.</summary>
public enum ReviewState
{
    Wired,
    Round,
    Verdict,
    Fixing,
    Checkpoint,
    Replan,
    Escalated,
    Ended
}
