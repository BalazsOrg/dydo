namespace DynaDocs.SkillBuild;

/// <summary>
/// The ticket captain's job resources, each built to <c>resources/&lt;kebab-case name&gt;.md</c>: a job
/// ticket's recipe (H15, H23), the inquisition captain (N5), the council chair and its members' briefs
/// (L23), and a PR into master (R26, R27).
/// </summary>
public enum TicketCaptainResource
{
    JobTickets,
    InquisitionCaptain,
    CouncilChair,
    CouncilMembers,
    MasterRoute
}
