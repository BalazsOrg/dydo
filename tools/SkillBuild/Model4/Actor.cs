namespace DynaDocs.SkillBuild;

/// <summary>Who performs a claim's move: a role, Balazs, or a system (a command, the builder, the guard, Linear, DYTEST, CI, the survey, Codex).</summary>
public enum Actor
{
    Admiral,
    ChiefOfStaff,
    TicketCaptain,
    CodeWriter,
    Reviewer,
    Scout,
    Balazs,
    DydoMerge,
    DydoArchive,
    DydoCheck,
    SkillBuild,
    Guard,
    UpdateDydo,
    Dytest,
    Survey,
    Linear,
    Ci,
    DydoInit,
    DydoStatechart,
    DydoMap,
    Codex
}
