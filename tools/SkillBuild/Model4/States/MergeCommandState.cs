namespace DynaDocs.SkillBuild;

/// <summary>M6: <c>dydo merge</c> and its refusals.</summary>
public enum MergeCommandState
{
    Invoked,
    Refused,
    Conflict,
    Merged,
    AlreadyIn
}
