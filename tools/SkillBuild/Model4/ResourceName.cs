namespace DynaDocs.SkillBuild;

/// <summary>
/// The role resources a phase links, each built to <c>resources/&lt;kebab-case name&gt;.md</c>: one dispatch
/// template per crew and officer kind (K14), the envoy brief and the council. Scenarios have their own
/// enum per role.
/// </summary>
public enum ResourceName
{
    EnvoyBrief,
    CoordinatorBrief,
    TicketCaptainBrief,
    ImplementerBrief,
    ReviewerBrief,
    ResearchBrief,
    ScoutBrief,
    Council
}
