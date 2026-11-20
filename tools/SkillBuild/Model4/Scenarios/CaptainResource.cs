namespace DynaDocs.SkillBuild;

/// <summary>
/// The ticket-captain prototype's resources (DYD-1372), each built to <c>resources/&lt;kebab-case
/// name&gt;.md</c>: the three kind resources a captain loads for its ticket's kind (option A, DYD-1295
/// 0f340abd), the resources a phase's line links, and the scenarios "When things go wrong" links.
/// </summary>
public enum CaptainResource
{
    Implement,
    Review,
    Research,
    Briefs,
    Filler,
    Round,
    Merging,
    TooBig,
    FixRounds,
    MergeConflict,
    RedGate,
    NeedForTheUser
}
