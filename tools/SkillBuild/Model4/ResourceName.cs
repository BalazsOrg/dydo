namespace DynaDocs.SkillBuild;

/// <summary>
/// The role resources a phase links, each built to <c>resources/&lt;kebab-case name&gt;.md</c>: one dispatch
/// template per crew and officer kind (K14), and the envoy brief. Scenarios and a role's own resources
/// have their own enum per role.
/// </summary>
public enum ResourceName
{
    EnvoyBrief,
    CoordinatorBrief,
    TicketCaptainBrief,
    ImplementerBrief,
    ReviewerBrief,
    ResearchBrief,
    ScoutBrief
}
