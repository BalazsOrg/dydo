namespace DynaDocs.SkillBuild;

/// <summary>N12 DR 055 and DR 056: a project's host settings, native memory off and the guard committed.</summary>
public enum HostSetupState
{
    Uninitialised,
    Initialised,
    Guarded,
    MemoryDenied,
    CodexBackground
}
