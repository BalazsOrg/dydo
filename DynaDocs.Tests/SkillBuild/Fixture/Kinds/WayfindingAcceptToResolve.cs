namespace DynaDocs.Tests.SkillBuild.Fixture;

using DynaDocs.SkillBuild;
using A = DynaDocs.Tests.SkillBuild.Fixture.FixtureActor;
using F = DynaDocs.Tests.SkillBuild.Fixture.FixtureSection;
using S = DynaDocs.Tests.SkillBuild.Fixture.FixtureSkill;

/// <summary>Migration fixture: the inventory's <c>wayfinding</c> claims, sub-topics accept to resolve.</summary>
public static class WayfindingAcceptToResolve
{
    public static readonly Claim AcceptCaptainLocalCourseCorrection = new(
        "Accept a captain's local course correction when later facts expose it.",
        [A.Admiral],
        [
            new(S.Admiral, "Accept a Captain's local course correction when later facts expose it; move cross-ticket and Project-wide discoveries back onto your map.", new(F.AdmiralMethod, 51))
        ]);

    public static readonly Claim AdmiralRoutineDecisions = new(
        "Within existing authority the admiral resolves routine decisions and commissions bounded engineering blocker diagnosis through the responsible captain.",
        [A.Admiral],
        [
            new(S.Admiral, "Within existing authority, resolve routine decisions and commission bounded engineering blocker diagnosis through the responsible captain.", new(F.AdmiralMethod, 47))
        ]);

    public static readonly Claim AdmiralUsesWayfinderAndRechart = new(
        "As discovery clears fog the admiral uses wayfinder to settle the visible route and recharts: creates, splits, drops or resequences tickets and records the discoveries on the Project.",
        [A.Admiral],
        [
            new(S.Admiral, "As fog clears, use `wayfinder` to settle the visible route.", new(F.AdmiralBoundary, 15))
        ]);

    public static readonly Claim AfterResolutionMapUpdate = new(
        "After a resolution, add newly surfaced tickets (create then wire, a Project's Task into its Milestone) and update or cancel tickets the resolution invalidates.",
        [A.MapHolder],
        [
            new(S.Wayfinder, "Add newly surfaced tickets (create then wire, a Project's Task into its Milestone); graduate any fog the answer has made specifiable, clearing each graduated patch from **Not yet specified** so it lives only as its new ticket.", new(F.WayfinderWorkThroughTheMap, 6), () => [WayfindingAcceptToResolve.GraduateSpecifiableFog!])
        ]);

    public static readonly Claim AtomicIntentStaysInTicket = new(
        "Atomic intent stays in its ticket, not in a spec anchor.",
        [A.MapHolder],
        [
            new(S.Wayfinder, "Atomic intent stays in its ticket.", new(F.WayfinderChartTheMap, 7))
        ]);

    public static readonly Claim CaptainLocalFogLoop = new(
        "When delivery exposes new local fog, the ticket-captain pauses the affected work, charts it as a direct Wayfinding Sub-ticket (loading wayfinder), records the resolution on it, links it from the parent delivery ticket, and only then resumes production.",
        [A.TicketCaptain],
        [
            new(S.TicketCaptain, "If delivery exposes new fog, load `wayfinder` and use its Wayfinding Tickets to course-correct.", new(F.TicketCaptainBoundary, 33)),
            new(S.Wayfinder, "A Ticket Captain may chart local fog inside one approved delivery outcome.", new(SpinePart.Job, 4))
        ]);

    public static readonly Claim CaptainRoutesHandRaise = new(
        "A Ticket Captain creates a local Sub-ticket, or escalates a Project-level packet to the Project's map holder.",
        [A.TicketCaptain],
        [
            new(S.Wayfinder, "| Ticket Captain | create a local Sub-ticket, or escalate a Project-level packet to the Project's map holder |", new(F.WayfinderDiscoveryBeforeQuestion, 3))
        ]);

    public static readonly Claim CaptainsDeliverTasks = new(
        "Ticket Captains deliver clear Tasks; Wayfinding Tickets clear the fog blocking what comes next.",
        [A.TicketCaptain],
        [
            new(S.Wayfinder, "Ticket Captains deliver clear Tasks; Wayfinding Tickets clear the fog blocking what comes next.", new(F.WayfinderChartAsYouGo, 1))
        ]);

    public static readonly Claim ChartingSessionStopsAfterResearchSubagents = new(
        "Charting is one session's work and resolves nothing itself; it stops after firing the Research subagents.",
        [A.MapHolder],
        [
            new(S.Wayfinder, "Stop: charting is one session's work; it resolves nothing itself.", new(F.WayfinderChartTheMap, 11))
        ]);

    public static readonly Claim ClearFogDone = new(
        "Clearing fog is done when every visible unknown is resolved or has the right owner, record and blocker.",
        [A.Admiral],
        [
            new(S.Admiral, "**Done:** every visible unknown is resolved or has the right owner, record, and blocker.", new(F.AdmiralMethod, 54))
        ]);

    public static readonly Claim CommitNewSpecAndMakeReachable = new(
        "Commit a new spec under the map holder's custody and make that commit reachable to recipients before relying on its Project-map link; otherwise record the anchor as unresolved.",
        [A.MapHolder],
        [
            new(S.Wayfinder, "Commit a new spec under the map holder's custody and make that commit reachable to recipients before relying on its Project-map link; otherwise record the anchor as unresolved.", new(F.WayfinderChartTheMap, 6))
        ]);

    public static readonly Claim CreateThenWire = new(
        "Create the specifiable tickets (Tasks and Wayfinding Tickets) first and wire blocking edges in a second pass.",
        [A.MapHolder],
        [
            new(S.Wayfinder, "**Create the tickets you can specify now**, both Tasks and Wayfinding Tickets, then wire blocking edges in a **second pass** (tickets need keys before they can reference each other).", new(F.WayfinderChartTheMap, 8))
        ]);

    public static readonly Claim CrewReturnHandRaiseToCaptain = new(
        "Crew return a prepared hand-raise to the Ticket Captain.",
        [A.Crew],
        [
            new(S.Wayfinder, "| Code Writer or other crew | return a prepared hand-raise to the Ticket Captain |", new(F.WayfinderDiscoveryBeforeQuestion, 2))
        ]);

    public static readonly Claim DeriveTasksFromAnchorSections = new(
        "Derive affected Tasks from the anchor's relevant sections and link those sections in their bounded contracts.",
        [A.Admiral, A.MapHolder],
        [
            new(S.Admiral, "Record the shared anchor choice from `wayfinder`, when one is needed, and contract affected tickets from its relevant sections before commissioning them.", new(F.AdmiralMethod, 10), () => [WayfindingRouteToWrite.SharedAnchorAndSections]),
            new(S.Wayfinder, "Derive affected Tasks from the anchor's relevant sections and link those sections in their bounded contracts.", new(F.WayfinderChartTheMap, 9))
        ]);

    public static readonly Claim DoNotChartTheUnseen = new(
        "The map is deliberately incomplete: do not chart or pre-slice what cannot yet be seen, and defer later decisions until delivery reveals the facts they need.",
        [A.AnyAgent, A.MapHolder],
        [
            new(S.Wayfinder, "Defer later decisions until delivery reveals the facts they need.", new(F.WayfinderChartAsYouGo, 2))
        ]);

    public static readonly Claim EnablementResolution = new(
        "Enablement is resolved when the work is done, and the answer records what was done and any facts later tickets depend on.",
        [A.AnyAgent],
        [
            new(S.Wayfinder, "Resolved when the work is done; the answer records what was done and any resulting facts (credentials location, new URLs, row counts) later tickets depend on.", new(F.WayfinderTicketTypes, 9))
        ]);

    public static readonly Claim FogOrTicketTest = new(
        "Whether something is fog or a ticket depends on whether the question can be stated precisely now: a sharp question becomes a ticket even if blocked, and a question not yet phrased that sharply goes in Not yet specified.",
        [A.MapHolder],
        [
            new(S.Wayfinder, "The test is whether you can state the question precisely now, _not_ whether you can answer it now.", new(F.WayfinderFogOfWar, 2))
        ]);

    public static readonly Claim GraduateSpecifiableFog = new(
        "Graduate any fog the answer made specifiable, clearing each graduated patch from Not yet specified so it lives only as its new ticket.",
        [A.MapHolder], []);

    public static readonly Claim GrillBreadthFirst = new(
        "Grill again breadth-first: fan out across the whole space, surfacing open questions and the first steps takeable now.",
        [A.MapHolder],
        [
            new(S.Wayfinder, "Grill again, **breadth-first** this time: fan out across the whole space rather than deep on any one thread, surfacing the open questions and the first steps takeable now.", new(F.WayfinderChartTheMap, 2))
        ]);

    public static readonly Claim GrillingDefaultType = new(
        "Grilling is the default Wayfinding Ticket type, and a Grilling ticket always loads both grilling and domain-modeling.",
        [A.MapHolder],
        [
            new(S.Wayfinder, "The default case.", new(F.WayfinderTicketTypes, 6))
        ]);

    public static readonly Claim HandRaiseRouting = new(
        "The map holder creates and wires fog tickets while charting or as delivery clears Project fog.",
        [A.MapHolder],
        [
            new(S.Wayfinder, "| Project's map holder | create and wire it while charting, or as delivery clears Project fog |", new(F.WayfinderDiscoveryBeforeQuestion, 1))
        ]);

    public static readonly Claim HitlLiveExchange = new(
        "A HITL ticket resolves only through the live exchange with the user; the agent never stands in for the user's side.",
        [A.AnyAgent],
        [
            new(S.Wayfinder, "A HITL ticket only resolves through that live exchange; the agent never stands in for the user's side of it (a grilling agent that answers its own questions has broken this).", new(F.WayfinderTicketTypes, 2))
        ]);

    public static readonly Claim HolderClaimsTicketBeforeWork = new(
        "The holder of a Wayfinding Ticket claims it before work.",
        [A.TicketCaptain, A.MapHolder],
        [
            new(S.Wayfinder, "A captain-held Type goes to its Ticket Captain, which claims it.", new(F.WayfinderWorkThroughTheMap, 3))
        ]);

    public static readonly Claim LinkAssets = new(
        "Assets created while resolving a ticket, including a Prototype, are linked from the ticket, not pasted in.",
        [A.AnyAgent],
        [
            new(S.Wayfinder, "Assets created while resolving a ticket are linked from it, not pasted in.", new(F.WayfinderTickets, 6))
        ]);

    public static readonly Claim LoadMapThenZoom = new(
        "Load the map (the low-resolution view), not every ticket body, and fetch a related or closed ticket in full on demand by the methods the map's Notes block names.",
        [A.AnyAgent],
        [
            new(S.Wayfinder, "Load the **map**: the low-resolution view, not every ticket body.", new(F.WayfinderWorkThroughTheMap, 2))
        ]);

    public static readonly Claim MapHolderCharts = new(
        "The map holder charts with wayfinder (and grills) before dividing the work.",
        [A.MapHolder],
        [
            new(S.Wayfinder, "The **map holder** charts and grills.", new(SpinePart.Job, 2))
        ]);

    public static readonly Claim MapHolderPreClearsFog = new(
        "The Project's map holder clears most Project fog and captures the relevant answers in the ticket contract before delivery starts, so the parent Task serves as the captain's local map.",
        [A.MapHolder, A.TicketCaptain],
        [
            new(S.TicketCaptain, "**Wayfinding:** the Project's map holder, the user or an invoked admiral, should have cleared most Project fog and captured relevant answers in the ticket contract.", new(F.TicketCaptainBoundary, 32)),
            new(S.Wayfinder, "At delivery scale, the parent Task is the local map; the Project's map holder should already have cleared most of its fog.", new(F.WayfinderTheMap, 4))
        ]);

    public static readonly Claim NoMapNeeded = new(
        "If charting surfaces no fog (the way is clear and the journey fits one session), no map is needed.",
        [A.MapHolder],
        [
            new(S.Wayfinder, "**If this surfaces no fog** (the way to the destination is already clear, the whole journey small enough for one session), no map is needed.", new(F.WayfinderChartTheMap, 3))
        ]);

    public static readonly Claim NotYetSpecifiedSection = new(
        "The map's Not yet specified section records the dim view (suspected question, area to revisit), as loosely or fully as the view allows, and excludes what is resolved, what is already a live ticket and what is out of scope.",
        [A.MapHolder],
        [
            new(S.Wayfinder, "The map's **Not yet specified** section is where that dim view is written down: the suspected question, the area to revisit later.", new(F.WayfinderFogOfWar, 1))
        ]);

    public static readonly Claim OpenTicketsViaLinear = new(
        "Open tickets are found through Linear queries and are not listed in the map.",
        [A.MapHolder],
        [
            new(S.Wayfinder, "Open tickets are found through Linear queries.", new(F.WayfinderTheMap, 3))
        ]);

    public static readonly Claim OutOfScopeExistingTicket = new(
        "When an existing ticket sits past the destination, mark it Canceled and leave one line in Out of scope with the gist, why, and a link to the ticket.",
        [A.MapHolder],
        [
            new(S.Wayfinder, "When a ticket that already exists turns out to sit past the destination (mis-scoped while charting, or exposed by a resolution), mark it `Canceled` and leave one line in the **Out of scope** section: the gist plus why it's out of scope, linking the ticket.", new(F.WayfinderOutOfScope, 3))
        ]);

    public static readonly Claim OutOfScopeNeverGraduates = new(
        "Out-of-scope work never graduates unless the destination is redrawn, then as a fresh effort.",
        [A.MapHolder],
        [
            new(S.Wayfinder, "Out-of-scope work never graduates (the frontier stops at the destination), so it returns only if the destination is redrawn, and then as a fresh effort, not a resumption.", new(F.WayfinderOutOfScope, 2))
        ]);

    public static readonly Claim OutOfScopeSection = new(
        "Work beyond the destination is out of scope, not fog: it gets its own Out of scope section on the map, decided by scope not sharpness, and stays out of Not yet specified.",
        [A.MapHolder],
        [
            new(S.Wayfinder, "The destination fixes the scope, so work beyond it is **out of scope**: it isn't fog, and it doesn't belong in **Not yet specified**.", new(F.WayfinderOutOfScope, 1))
        ]);

    public static readonly Claim OutOfScopeStaysOutOfResolutions = new(
        "Out-of-scope entries stay out of Resolutions so far, which records only the route actually walked.",
        [A.MapHolder],
        [
            new(S.Wayfinder, "It stays out of **Resolutions so far**, which records the route actually walked; a scope boundary isn't a step on it.", new(F.WayfinderOutOfScope, 4))
        ]);

    public static readonly Claim PickNextTicket = new(
        "Working starts from a Project or parent delivery ticket: use a named frontier ticket if given, otherwise take the first pickable frontier ticket in Todo without asking the user to choose.",
        [A.AnyAgent],
        [
            new(S.Wayfinder, "Working starts from a Project or parent delivery ticket.", new(F.WayfinderWorkThroughTheMap, 1))
        ]);

    public static readonly Claim PrototypeTicket = new(
        "A Prototype ticket (HITL) makes a cheap, rough, concrete artifact to react to via the prototype skill, and is used when \"how should it look\" or \"how should it behave\" is the key question.",
        [A.MapHolder],
        [
            new(S.Wayfinder, "**Prototype** (HITL): Raise the fidelity of the discussion by making a cheap, rough, concrete artifact to react to (throwaway UI or logic code) by calling the Skill tool with \"prototype\".", new(F.WayfinderTicketTypes, 5))
        ]);

    public static readonly Claim PullDiscoveriesToProjectMap = new(
        "Discoveries that are shared, Project-wide or cross-ticket are moved back onto the Project map.",
        [A.Admiral],
        [
            new(S.Admiral, "Pull shared or Project-wide discoveries back to the Project map.", new(F.AdmiralBoundary, 17))
        ]);

    public static readonly Claim QuestionWaitsForUserAnswer = new(
        "A Question still waits for the user's answer.",
        [A.MapHolder],
        [
            new(S.Wayfinder, "For a map-holder-held Type, claim it before work; a Question still waits for the user's answer.", new(F.WayfinderWorkThroughTheMap, 4))
        ]);

    public static readonly Claim RereadLinearBeforeMutation = new(
        "Other sessions may work unblocked tickets in parallel, so re-read Linear before every claim or map mutation.",
        [A.AnyAgent],
        [
            new(S.Wayfinder, "Other sessions may work unblocked tickets in parallel, so re-read Linear before every claim or map mutation.", new(F.WayfinderWorkThroughTheMap, 7))
        ]);

    public static readonly Claim ResearchTicketSubagent = new(
        "A Research Ticket is resolved by a subagent that loads research; after charting, each new unassigned, Todo, unblocked Research Ticket is claimed and given a subagent whose cited findings are recorded as a comment.",
        [A.MapHolder],
        [
            new(S.Wayfinder, "Resolved by a subagent that calls the Skill tool with \"research\".", new(F.WayfinderTicketTypes, 3))
        ]);

    public static readonly Claim ResolutionRecord = new(
        "When the Type's outcome is reached, post the answer as a resolution comment, mark the ticket Done, append a context pointer to Resolutions so far, and the map holder closes the ticket and updates the map.",
        [A.MapHolder, A.AnyAgent],
        [
            new(S.Research, "On a Wayfinding Ticket, the map holder closes the ticket and updates the map.", new(F.ResearchReturn, 2)),
            new(S.Wayfinder, "The resolution is recorded as a named evidence comment using the `DECIDED` form in the communication protocol.", new(F.WayfinderTickets, 5))
        ]);

    public static readonly Claim ResolveByTypeRules = new(
        "Resolve the ticket following its Type's rules.",
        [A.AnyAgent],
        [
            new(S.Wayfinder, "Follow the ticket Type above.", new(F.WayfinderWorkThroughTheMap, 5))
        ]);
}
