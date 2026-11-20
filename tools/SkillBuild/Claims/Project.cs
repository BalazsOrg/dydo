namespace DynaDocs.SkillBuild;

/// <summary>A Project and its coordinator (M1): his HITL charting, watching, re-planning, and the rules of the watch.</summary>
public static class Project
{
    public static readonly Claim Take = new("He opens a Project and charts it HITL, with its grillings and first wayfindings (DYD-1082 555a8300 Q21).", [Actor.Balazs], [])
    { Spec = ["H5", "S14"], Transition = new(ProjectState.Unheld, ProjectState.Charting, "he opens the Project and charts it, HITL") };

    public static readonly Claim Charted = new("Charting ends once the spec exists and the first tickets are contracted; the Project then waits for its coordinator (DYD-1082 555a8300 Q21).", [Actor.Balazs], [])
    { Spec = ["S14", "S15", "H4"], Transition = new(ProjectState.Charting, ProjectState.Unheld, "the Project is up: the spec exists and the first tickets are contracted") };

    public static readonly Claim Replan = new("A fundamental blocker or a new requirement goes to the officer above to re-plan.", [Actor.TicketCaptain, Actor.Coordinator],
        [new(Skill.Coordinator, "A fundamental blocker or a new requirement from below: open a Wayfinding ticket to re-plan the affected part of the map, and put a fresh captain on it.", new(ProjectState.Replanning, 203, MoveKind.Arrival))])
    { Spec = ["R9", "H5"], Transition = new(ProjectState.Watching, ProjectState.Replanning, "a blocker is fundamental, or a new requirement") };

    public static readonly Claim Replanned = new("On a re-plan, dead hops go Canceled with a one-line reason.", [Actor.Coordinator],
    [
        new(Skill.Coordinator, "Once the re-plan is filed and its dead hops are Canceled with a one-line reason, sequence what it filed and go back to watching.", new(ProjectState.Replanning, 204, MoveKind.Exit)),
        new(Skill.LinearWorkspaceStandard, "On a re-plan, dead hops go Canceled with a one-line reason.", new(Section.LinearStatuses, 5))
    ])
    { Spec = ["S10"], Transition = new(ProjectState.Replanning, ProjectState.Watching, "dead hops Canceled with a reason") };

    public static readonly Claim StartLanding = new("The landing starts once every delivery ticket is merged into the feature branch.", [Actor.Coordinator],
        [new(Skill.Coordinator, "When every delivery ticket is merged into the feature branch, start the landing.", new(ProjectState.Watching, 205, MoveKind.Exit))])
    { Spec = ["N5"], Transition = new(ProjectState.Watching, ProjectState.Landing, "every delivery ticket is merged into the feature branch") };

    // The coordinator states this in Rechart's combined line: its half of the map holder wakes the Wayfinding captain (DYD-1195 b46e8343).
    public static readonly Claim SpecDoc = new("The spec is a main Linear document on the Project, written and refined by the map holder.", [Actor.Coordinator, Actor.TicketCaptain],
    [
        new(Skill.TicketCaptain, "Write and refine `Spec: <Project>`, the Project's main Linear document with C8's headings: problem, behaviour, decisions, out of scope, acceptance checks; a sub-feature that needs defining well gets its own spec, linked from it.", new(TicketCaptainSlot.Wayfinding, 3110)),
        new(Skill.LinearWorkspaceStandard, "The spec is a main Linear document on the Project, `Spec: <Project>`, with C8's headings: problem, behaviour, decisions, out of scope, acceptance checks. A sub-feature that needs defining well gets its own spec, linked from it. The map holder writes and refines it; at acceptance it is deleted after its lasting parts move to DRs, the glossary, docs and the verification map.", new(Section.LinearSpec, 1))
    ])
    { Spec = ["S14"] };

    public static readonly Claim FeatureBranch = new("The feature branch is cut from main before the first ticket branch.", [Actor.Coordinator],
    [
        new(Skill.Coordinator, "Open `feature/<project-slug>` from main before the first ticket branch.", new(CoordinatorSlot.Lanes, 208)),
        new(Skill.WorkingTreeStandard, "A Project's feature branch, `feature/<project-slug>`, is cut from main before its first ticket branch.", new(Section.TreeBranches, 1))
    ])
    { Spec = ["N12 DR 045 §8"] };

    // The coordinator states this in Rechart's combined line (DYD-1195 b46e8343: the release harvest is the Wayfinding captain's).
    public static readonly Claim Harvest = new("A release's map holder harvests adopters' friction queues and local patches.", [Actor.Coordinator, Actor.TicketCaptain],
        [new(Skill.TicketCaptain, "When a release is planned, harvest the adopters' Agent friction queues and local patches.", new(TicketCaptainSlot.Wayfinding, 3119))])
    { Spec = ["H9"] };

    public static readonly Claim Tick = new("The coordinator's watch is a deterministic loop over the survey, the board and liveness.", [Actor.Coordinator],
        [new(Skill.Coordinator, "Each tick, read `dydo survey board <project>`, the board and the liveness findings, and act on every finding.", new(ProjectState.Watching, 212, MoveKind.Nominal))])
    { Spec = ["H5", "S22", "S23"] };

    public static readonly Claim EmptyTick = new("An empty tick ends in one line.", [Actor.Coordinator],
        [new(Skill.Coordinator, "An empty tick ends in one line.", new(ProjectState.Watching, 213, MoveKind.Nominal))])
    { Spec = ["R31"] };

    public static readonly Claim CheckIn = new("The coordinator keeps a 20-minute scheduled check-in while work is in flight or pickable.", [Actor.Coordinator],
        [new(Skill.Coordinator, "Keep a T3 `schedule_task` check-in every 20 minutes on your own thread while work is in flight or pickable; delete it when only his word is awaited.", new(ProjectState.Watching, 214, MoveKind.Nominal))])
    { Spec = ["N12 DR 047", "H5"] };

    public static readonly Claim LanesFull = new("Every officer keeps its lanes full toward a soft four, pulling filler on purpose.", [Actor.Coordinator, Actor.TicketCaptain],
    [
        new(Skill.Coordinator, "Fill each lane with a captain, toward a soft target of four, oldest first at each step: ready tickets the plan names (unblocked and contracted), then the open Shoulds batch, or a new one when shoulds wait outside a batch, then the survey's `pickable` side tickets, then continuous tickets, those blocking a ticket in your Project first.", new(CoordinatorSlot.Lanes, 215)),
        new(Skill.TicketCaptain, "Keep your level's lanes full, toward a soft target of four; when the mainline can't fill one, pull your own pickable side tickets first, then continuous tickets.", new(DispatchState.Staffing, 215, MoveKind.Nominal))
    ])
    { Spec = ["H10"] };

    public static readonly Claim PullRule = new("The standing rule for pulling a ticket without asking.", [Actor.Coordinator, Actor.TicketCaptain],
    [
        new(Skill.Coordinator, "Pull a filler only when its labels already meet the pull rule: Mode AFK; Type Bug or Feature at Severity should or lower, or an unrated CI finding; unblocked; oldest first; its captain checks small, two-way and no design call.", new(CoordinatorSlot.Lanes, 216)),
        new(Skill.TicketCaptain, "Pull without asking only a ticket that is Mode AFK; a Bug or small Feature at Severity should or lower, or an unrated CI finding; two-way; no design call; oldest first.", new(DispatchState.Staffing, 216, MoveKind.Nominal))
    ])
    { Spec = ["H11"] };

    public static readonly Claim Pulled = new("A pulled continuous ticket follows its officer's chain.", [Actor.Coordinator, Actor.TicketCaptain],
    [
        new(Skill.Coordinator, "A continuous ticket you pull follows your chain from then on.", new(CoordinatorSlot.Lanes, 217)),
        new(Skill.TicketCaptain, "A continuous ticket you pull follows your chain from then on.", new(DispatchState.Staffing, 217, MoveKind.Nominal))
    ])
    { Spec = ["H8"] };

    public static readonly Claim CiFinding = new("A CI finding goes to its open ticket's officer; for a closed ticket in an open Project the coordinator opens its ticket, as a child of one closed within 14 days.", [Actor.Coordinator],
        [new(Skill.Coordinator, "Send a CI finding for an open ticket to that ticket's officer; for a ticket closed more than 14 days ago, open a new ticket, and for one closed more recently, open it as a child of the closed ticket.", new(ProjectState.Watching, 218, MoveKind.Nominal))])
    { Spec = ["H11", "H8"] };

    public static readonly Claim OneTicket = new("One agent holds one ticket; several steps for one agent are wrapped in one ticket.", [Actor.TicketCaptain],
    [
        new(Skill.TicketCaptain, "One agent holds one ticket; wrap several steps for one agent in one ticket.", new(HopsState.Cut, 219, MoveKind.Nominal)),
        new(Skill.LinearWorkspaceStandard, "One agent holds one ticket, and several steps given to one agent are wrapped in one ticket; sub-tickets may nest.", new(Section.LinearTickets, 3))
    ])
    { Spec = ["S7"] };

    public static readonly Claim Contract = new("A contract is one ticket's expected outcome, quoting and linking its spec sections.", [Actor.TicketCaptain],
    [
        new(Skill.TicketCaptain, "Write each sub-ticket's contract in its description: its expected outcome, quoting and linking the spec sections it derives from.", new(HopsState.Cut, 220, MoveKind.Nominal)),
        new(Skill.LinearWorkspaceStandard, "A contract is one ticket's expected outcome, in its description. It quotes and links the spec sections it derives from, and that frozen quote replaces pinning a commit.", new(Section.LinearSpec, 2))
    ])
    { Spec = ["S15"] };

    // The coordinator states this in Rechart's combined line (DYD-1195 b46e8343: reconciling contracts is the Wayfinding captain's).
    public static readonly Claim Reconcile = new("When the spec changes, the map holder reconciles the open contracts.", [Actor.Coordinator, Actor.TicketCaptain],
        [new(Skill.TicketCaptain, "When the spec changes, reconcile the open contracts with it.", new(TicketCaptainSlot.Wayfinding, 3112))])
    { Spec = ["S15"] };

    // The coordinator states this in Rechart's combined line: writing a contract is cutting, a Wayfinding captain's (DYD-1082 1b6b6a5d Q16).
    public static readonly Claim Uncontracted = new("An uncontracted Backlog ticket is contracted to Todo by a captain on a Wayfinding ticket (DYD-1082 1b6b6a5d Q16).", [Actor.Coordinator, Actor.TicketCaptain],
        [new(Skill.TicketCaptain, "Contract each `uncontracted` Backlog ticket, so it moves to Todo.", new(TicketCaptainSlot.Wayfinding, 3114))])
    { Spec = ["N12 DR 063 §4", "H5", "S5"] };

    public static readonly Claim FeatureBranchYours = new("The feature branch is the coordinator's, to merge into as it wishes.", [Actor.Coordinator],
    [
        new(Skill.Coordinator, "Merges into your feature branch need no approval but yours.", new(CoordinatorSlot.Merges, 223)),
        new(Skill.WorkingTreeStandard, "The feature branch is the coordinator's: it merges into it as it wishes.", new(Section.TreeBranches, 2))
    ])
    { Spec = ["R25"] };

    public static readonly Claim OneWayPaths = new("The guard-path list in dydo.json is his policy; the coordinator opens a ticket carrying each decision of his, for an implementer to make.", [Actor.Coordinator],
        [new(Skill.Coordinator, "When he decides a guard-path addition or removal, open a ticket carrying his decision for an implementer to edit `dydo.json`; its PR is one-way.", new(CoordinatorSlot.Merges, 224))])
    { Spec = ["R27", "R16"] };

    public static readonly Claim MilestoneWalk = new("A Milestone's walkthrough is offered when its last ticket merges.", [Actor.Coordinator],
        [new(Skill.Coordinator, "When a Milestone's last ticket merges into the feature branch, its Walkthrough ticket joins your HITL queue for a fresh envoy; a skip is a verdict to go on.", new(CoordinatorSlot.Merges, 225))])
    { Spec = ["N12 DR 060 §2", "H22"] };

    public static readonly Claim OnMaster = new("Walkthroughs run on master whenever possible; the landing's walks the candidate.", [Actor.Coordinator],
        [new(Skill.Coordinator, "Run a walkthrough on master whenever possible.", new(CoordinatorSlot.Merges, 227))])
    { Spec = ["N4"] };

    public static readonly Claim Weeds = new("Every weed becomes a rule.", [Actor.Coordinator],
        [new(Skill.Coordinator, "Give each mistake found after landing a lint, test or rule as well as its fix.", new(CoordinatorSlot.Merges, 228))])
    { Spec = ["R30"] };

    public static readonly Claim Usage = new("The work never stops for usage, and no officer brings a usage limit to him.",
        [Actor.Coordinator, Actor.TicketCaptain, Actor.ChiefOfStaff],
    [
        new(Skill.Coordinator, "Keep the work going through any usage limit, and never bring one to him.", new(ProjectState.Watching, 229, MoveKind.Nominal)),
        new(Skill.TicketCaptain, "Keep the work going through any usage limit, and never bring one to him.", new(Routine.Dispatching, 13)),
        new(Skill.ChiefOfStaff, "Keep the work going through any usage limit, and never bring one to him.", new(StaffState.Day, 1012, MoveKind.Nominal))
    ])
    { Spec = ["H21"] };

    public static readonly Claim Rechart = new("Re-charting, cutting and every other map call go to a fresh captain on a Wayfinding ticket; the coordinator sequences what it files (DYD-1082 1b6b6a5d Q16, DYD-1195 b46e8343).", [Actor.Coordinator, Actor.TicketCaptain],
    [
        new(Skill.Coordinator, "Send every map call to a fresh captain on a Wayfinding ticket: refining the spec, reconciling the open contracts after it changes, re-charting as fog clears, contracting an `uncontracted` ticket, and the release harvest; open the ticket, wake its captain, and sequence what it files.",
            new(CoordinatorSlot.Routing, 230), () => [SpecDoc, Reconcile, Uncontracted, Harvest]),
        new(Skill.TicketCaptain, "Re-chart as fog clears and cut the tickets the map needs, deciding each cut the coordinator would otherwise face; file what you cut and return, and the coordinator only sequences it.", new(TicketCaptainSlot.Wayfinding, 3113))
    ])
    { Spec = ["H5", "H2", "H14", "N12 DR 045 §5"] };
}
