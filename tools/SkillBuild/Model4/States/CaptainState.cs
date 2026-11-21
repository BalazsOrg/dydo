namespace DynaDocs.SkillBuild;

/// <summary>The states of a captain holding one ticket (DYD-1372, DYD-1295 519126ab).</summary>
public enum CaptainState
{
    Taken,
    Leaf,
    Wayfinding,
    Split,
    Waiting,
    InReview,
    Clear,
    Fixing,
    Checkpoint,
    Repairing,
    Question,
    Finishing,
    Done,
    Returned
}
