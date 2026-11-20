namespace DynaDocs.SkillBuild;

/// <summary>
/// Each scenario resource's closing slot, "Save, resume and end" (K9): the work saved, where it resumes, and
/// whether the scenario ends resolved or escalated. A role declares only its own scenarios' members.
/// </summary>
public enum ScenarioClose
{
    ReCut,
    FixRounds,
    MergeConflict,
    RedGate,
    ForcedResync,
    MasterRevert,
    NeedForBalazs,
    Liveness,
    OverLimit,
    LandingNotAccepted,
    Replanning,
    RejectedFiller,
    Survivors,
    Survivor,
    Hunks,
    Equivalent,
    IgnoreEntry
}

