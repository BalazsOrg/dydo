namespace DynaDocs.Tests.SkillBuild.Fixture;

using DynaDocs.SkillBuild;
using A = DynaDocs.Tests.SkillBuild.Fixture.FixtureActor;
using F = DynaDocs.Tests.SkillBuild.Fixture.FixtureSection;
using S = DynaDocs.Tests.SkillBuild.Fixture.FixtureSkill;

/// <summary>Migration fixture: the inventory's <c>wayfinding</c> claims, sub-topics route to write.</summary>
public static class WayfindingRouteToWrite
{
    public static readonly Claim RouteOneTicketAtATime = new(
        "Wayfinding charts the visible route as Wayfinding Tickets and resolves them one at a time, graduating what becomes specifiable into fresh tickets, until the destination is reached; it finds the route and does not charge at the destination.",
        [A.AnyAgent, A.MapHolder],
        [
            new(S.Wayfinder, "Chart the visible route as Wayfinding Tickets and resolve them one at a time until the destination is reached.", new(HeaderPart.Role, 1))
        ]);

    public static readonly Claim ScaffoldChartAndApproval = new(
        "Chart the Project and get approval by Method step 2.",
        [A.Admiral],
        [
            new(S.Admiral, "[ ] Chart and get approval by Method step 2.", new(F.AdmiralScaffoldProjectLead, 4))
        ]);

    public static readonly Claim SessionTicketLimit = new(
        "A session the user drives resolves at most one Wayfinding Ticket (Research Tickets excepted); an invoked admiral is exempt, and Tasks may run concurrently under separate Ticket Captains.",
        [A.User, A.Admiral, A.TicketCaptain],
        [
            new(S.Wayfinder, "Either way, a session the user drives never resolves more than one Wayfinding Ticket, with the exception of Research Tickets: one ticket is sized to one session.", new(F.WayfinderInvocation, 1))
        ]);

    public static readonly Claim SettleDestinationFirst = new(
        "Use grilling and domain-modeling to pin down what the map is finding its way to (spec, Decision Record, or change), settling the destination first since it fixes the scope.",
        [A.MapHolder],
        [
            new(S.Wayfinder, "Use `grilling` and `domain-modeling` to pin down what this map is finding its way to: the spec, Decision Record, or change.", new(F.WayfinderChartTheMap, 1))
        ]);

    public static readonly Claim SharedAnchorAndSections = new(
        "For shared behavior or decisions needing one current anchor, choose or create a reviewed plan or feature spec (under feature-spec-convention) and record the anchor choice.",
        [A.Admiral, A.MapHolder],
        [
            new(S.Wayfinder, "Link shared feature anchors following `dydo/reference/feature-spec-convention.md`, read from the repository root when creating or changing one.", new(F.WayfinderTheMap, 2))
        ]);

    public static readonly Claim TicketVsDrDivision = new(
        "Linear records the decision-making work and dydo records the qualifying decision: the ticket carries the question and its working resolution, the Decision Record the durable decision and its rationale, and the two are linked, never copied into each other.",
        [A.MapHolder, A.AnyAgent],
        [
            new(S.LinearWorkspaceStandard, "Linear records the decision-making work; dydo records a qualifying decision.", new(F.LinearWorkspaceStandardDecisionRecords, 1)),
            new(S.Wayfinder, "The ticket carries the question and its working resolution; the Decision Record carries the durable decision.", new(F.WayfinderDiscoveryBeforeQuestion, 5))
        ]);

    public static readonly Claim TypeHolders = new(
        "Each Wayfinding Ticket type has a holder: the map holder (admiral when invoked) holds Research, Grilling and Question, a ticket-captain holds Enablement and Prototype.",
        [A.Admiral, A.TicketCaptain, A.MapHolder],
        [
            new(S.Admiral, "Hold Research, Grilling, Question and Walkthrough yourself; commission captains for Prototype and Enablement.", new(F.AdmiralBoundary, 16)),
            new(S.Wayfinder, "A captain drives Enablement alone where it can (AFK); for human-only steps it uses `wizard` in a top-level HITL session.", new(F.WayfinderTicketTypes, 8))
        ]);

    public static readonly Claim WayfindDoneMapMatches = new(
        "Wayfinding is done when the Project map matches the work in flight.",
        [A.Admiral],
        [
            new(S.Admiral, "**Done:** the Project map matches the work in flight.", new(F.AdmiralMethod, 46))
        ]);

    public static readonly Claim WireBlockersBeforeCommissioning = new(
        "Every blocker is wired and what is visible is settled before the affected delivery ticket is commissioned.",
        [A.Admiral],
        [
            new(S.Admiral, "Wire every blocker and settle what is visible before commissioning the affected delivery ticket.", new(F.AdmiralMethod, 50))
        ]);

    public static readonly Claim WriteTheMap = new(
        "Write the map with Destination and Notes filled in, Resolutions so far empty, and the fog sketched into Not yet specified.",
        [A.MapHolder],
        [
            new(S.Wayfinder, "**Write the map:** Destination and Notes filled in, Resolutions so far empty, the fog sketched into **Not yet specified**.", new(F.WayfinderChartTheMap, 4))
        ]);
}
