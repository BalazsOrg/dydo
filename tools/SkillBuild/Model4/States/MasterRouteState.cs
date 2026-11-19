namespace DynaDocs.SkillBuild;

/// <summary>M8: a PR into master, two-way or one-way.</summary>
public enum MasterRouteState
{
    Offered,
    Paced,
    Resync,
    Held,
    AfkMerge,
    CiWatch,
    HitlMerge,
    Merged,
    Reverted,
    Closed
}
