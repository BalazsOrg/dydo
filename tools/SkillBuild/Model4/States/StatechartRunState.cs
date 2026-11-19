namespace DynaDocs.SkillBuild;

/// <summary>N12 DR 062: a dydo statechart invocation and its prerequisites.</summary>
public enum StatechartRunState
{
    Invoked,
    InvalidOverride,
    Ready,
    ApprovalRequired,
    Continuing,
    NonSuccess,
    Checking,
    Charted,
    ScratchKept
}
