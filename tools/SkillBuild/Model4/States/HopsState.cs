namespace DynaDocs.SkillBuild;

/// <summary>M4: a step's shape and its typed hop chain, by Type label.</summary>
public enum HopsState
{
    Cut,
    OneTicket,
    EvidenceCheck,
    CheckFix,
    ImplementHop,
    ReviewHop,
    FixHop,
    StepDone,
    AfkMerge,
    HitlMerge
}
