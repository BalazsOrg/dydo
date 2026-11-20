namespace DynaDocs.SkillBuild;

/// <summary>A Project and its coordinator (M1): charting, watching, re-planning, and the rules of the watch.</summary>
public static class Project
{
    // The coordinator's line for this is its role line, Role.Coordinator (K4).
    public static readonly Claim Take = new("Every open Project has a live coordinator holding its outcome.", [Actor.Coordinator], [])
    { Spec = ["H4"], Transition = new(ProjectState.Unheld, ProjectState.Charting, "the Project opens; it has a live coordinator") };

    public static readonly Claim Charted = new("Charting ends when the spec exists and the first tickets are contracted.", [Actor.Coordinator],
        [new(Skill.Coordinator, "Leave charting once `Spec: <Project>` exists and the first tickets carry contracts that quote and link it.", new(ProjectState.Charting, 202, MoveKind.Exit))])
    { Spec = ["S14", "S15"], Transition = new(ProjectState.Charting, ProjectState.Watching, "the spec exists and the first tickets are contracted") };

    public static readonly Claim Replan = new("A fundamental blocker or a new requirement goes to the officer above to re-plan.", [Actor.TicketCaptain, Actor.Coordinator],
        [new(Skill.Coordinator, "A fundamental blocker or a new requirement from below: re-plan the affected part of the map.", new(ProjectState.Replanning, 203, MoveKind.Arrival))])
    { Spec = ["R9"], Transition = new(ProjectState.Watching, ProjectState.Replanning, "a blocker is fundamental, or a new requirement") };

    public static readonly Claim Replanned = new("On a re-plan, dead hops go Canceled with a one-line reason.", [Actor.Coordinator],
    [
        new(Skill.Coordinator, "Cancel each dead hop with a one-line reason, then go back to watching.", new(ProjectState.Replanning, 204, MoveKind.Exit)),
        new(Skill.LinearWorkspaceStandard, "On a re-plan, dead hops go Canceled with a one-line reason.", new(Section.LinearStatuses, 5))
    ])
    { Spec = ["S10"], Transition = new(ProjectState.Replanning, ProjectState.Watching, "dead hops Canceled with a reason") };

    public static readonly Claim StartLanding = new("The landing starts once every delivery ticket is merged into the feature branch.", [Actor.Coordinator],
        [new(Skill.Coordinator, "When every delivery ticket is merged into the feature branch, start the landing.", new(ProjectState.Watching, 205, MoveKind.Exit))])
    { Spec = ["N5"], Transition = new(ProjectState.Watching, ProjectState.Landing, "every delivery ticket is merged into the feature branch") };

    public static readonly Claim SpecDoc = new("The spec is a main Linear document on the Project, written and refined by the map holder.", [Actor.Coordinator],
    [
        new(Skill.Coordinator, "Write and refine `Spec: <Project>`, a main Linear document on the Project with C8's headings: problem, behaviour, decisions, out of scope, acceptance checks; a sub-feature that needs defining well gets its own spec, linked from it.", new(ProjectState.Charting, 206, MoveKind.Nominal)),
        new(Skill.LinearWorkspaceStandard, "The spec is a main Linear document on the Project, `Spec: <Project>`, with C8's headings: problem, behaviour, decisions, out of scope, acceptance checks. A sub-feature that needs defining well gets its own spec, linked from it. The map holder writes and refines it; at acceptance it is deleted after its lasting parts move to DRs, the glossary, docs and the verification map.", new(Section.LinearSpec, 1))
    ])
    { Spec = ["S14"] };

    public static readonly Claim SpecGuards = new("The spec, not a DR, is the guard: every feature has one.", [Actor.Coordinator],
        [new(Skill.Coordinator, "Give every feature a spec, whether or not a DR backs it.", new(ProjectState.Charting, 207, MoveKind.Nominal))])
    { Spec = ["S16"] };

    public static readonly Claim FeatureBranch = new("The feature branch is cut from main before the first ticket branch.", [Actor.Coordinator],
    [
        new(Skill.Coordinator, "Open `feature/<project-slug>` from main before the first ticket branch.", new(ProjectState.Charting, 208, MoveKind.Nominal)),
        new(Skill.WorkingTreeStandard, "A Project's feature branch, `feature/<project-slug>`, is cut from main before its first ticket branch.", new(Section.TreeBranches, 1))
    ])
    { Spec = ["N12 DR 045 §8"] };

    public static readonly Claim Milestones = new("The map holder places few Milestones while charting, the walking skeleton first.", [Actor.Coordinator],
        [new(Skill.Coordinator, "Place the Project's Milestones in Linear while charting: few, each a chunk where drift from his intent would show, the walking skeleton first and the landing last; every delivery ticket belongs to one.", new(ProjectState.Charting, 209, MoveKind.Nominal))])
    { Spec = ["N12 DR 060 §1"] };

    public static readonly Claim Skeleton = new("Only the walking skeleton's Walkthrough blocks the next Milestone.", [Actor.Coordinator],
        [new(Skill.Coordinator, "File the walking skeleton's Walkthrough with the map, as a native blocker of the next Milestone's tickets until his verdict or skip; later Milestones' Walkthroughs block nothing.", new(ProjectState.Charting, 210, MoveKind.Nominal))])
    { Spec = ["N12 DR 060 §3"] };

    public static readonly Claim Harvest = new("A release's map holder harvests adopters' friction queues and local patches.", [Actor.Coordinator],
        [new(Skill.Coordinator, "Charting a release, first harvest the adopters' Agent friction queues and local patches.", new(ProjectState.Charting, 211, MoveKind.Nominal))])
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
        new(Skill.Coordinator, "Keep your level's lanes full, toward a soft target of four; when the mainline can't fill one, pull your own pickable side tickets first, then continuous tickets.", new(ProjectState.Watching, 215, MoveKind.Nominal)),
        new(Skill.TicketCaptain, "Keep your level's lanes full, toward a soft target of four; when the mainline can't fill one, pull your own pickable side tickets first, then continuous tickets.", new(DispatchState.Staffing, 215, MoveKind.Nominal))
    ])
    { Spec = ["H10"] };

    public static readonly Claim PullRule = new("The standing rule for pulling a ticket without asking.", [Actor.Coordinator, Actor.TicketCaptain],
    [
        new(Skill.Coordinator, "Pull without asking only a ticket that is Mode AFK; a Bug or small Feature at Severity should or lower, or an unrated CI finding; two-way; no design call; oldest first.", new(ProjectState.Watching, 216, MoveKind.Nominal)),
        new(Skill.TicketCaptain, "Pull without asking only a ticket that is Mode AFK; a Bug or small Feature at Severity should or lower, or an unrated CI finding; two-way; no design call; oldest first.", new(DispatchState.Staffing, 216, MoveKind.Nominal))
    ])
    { Spec = ["H11"] };

    public static readonly Claim Pulled = new("A pulled continuous ticket follows its officer's chain.", [Actor.Coordinator, Actor.TicketCaptain],
    [
        new(Skill.Coordinator, "A continuous ticket you pull follows your chain from then on.", new(ProjectState.Watching, 217, MoveKind.Nominal)),
        new(Skill.TicketCaptain, "A continuous ticket you pull follows your chain from then on.", new(DispatchState.Staffing, 217, MoveKind.Nominal))
    ])
    { Spec = ["H8"] };

    public static readonly Claim CiFinding = new("A CI finding goes to its open ticket's officer, or to the coordinator for a long-closed one.", [Actor.Coordinator],
        [new(Skill.Coordinator, "Send a CI finding for an open ticket to that ticket's officer; one for a long-closed ticket is yours.", new(ProjectState.Watching, 218, MoveKind.Nominal))])
    { Spec = ["H11"] };

    public static readonly Claim OneTicket = new("One agent holds one ticket; several steps for one agent are wrapped in one ticket.", [Actor.Coordinator, Actor.TicketCaptain],
    [
        new(Skill.Coordinator, "One agent holds one ticket; wrap several steps for one agent in one ticket.", new(ProjectState.Watching, 219, MoveKind.Nominal)),
        new(Skill.TicketCaptain, "One agent holds one ticket; wrap several steps for one agent in one ticket.", new(HopsState.Cut, 219, MoveKind.Nominal)),
        new(Skill.LinearWorkspaceStandard, "One agent holds one ticket, and several steps given to one agent are wrapped in one ticket; sub-tickets may nest.", new(Section.LinearTickets, 3))
    ])
    { Spec = ["S7"] };

    public static readonly Claim Contract = new("A contract is one ticket's expected outcome, quoting and linking its spec sections.", [Actor.Coordinator, Actor.TicketCaptain],
    [
        new(Skill.Coordinator, "Write each ticket's contract in its description: its expected outcome, quoting and linking the spec sections it derives from.", new(ProjectState.Watching, 220, MoveKind.Nominal)),
        new(Skill.TicketCaptain, "Write each sub-ticket's contract in its description: its expected outcome, quoting and linking the spec sections it derives from.", new(HopsState.Cut, 220, MoveKind.Nominal)),
        new(Skill.LinearWorkspaceStandard, "A contract is one ticket's expected outcome, in its description. It quotes and links the spec sections it derives from, and that frozen quote replaces pinning a commit.", new(Section.LinearSpec, 2))
    ])
    { Spec = ["S15"] };

    public static readonly Claim Reconcile = new("When the spec changes, the map holder reconciles the open contracts.", [Actor.Coordinator],
        [new(Skill.Coordinator, "When the spec changes, reconcile the open contracts.", new(ProjectState.Watching, 221, MoveKind.Nominal))])
    { Spec = ["S15"] };

    public static readonly Claim Uncontracted = new("The coordinator contracts an uncontracted Backlog ticket to Todo.", [Actor.Coordinator],
        [new(Skill.Coordinator, "Contract an `uncontracted` ticket (Backlog, unassigned, no open blocker) to Todo.", new(ProjectState.Watching, 222, MoveKind.Nominal))])
    { Spec = ["N12 DR 063 §4"] };

    public static readonly Claim FeatureBranchYours = new("The feature branch is the coordinator's, to merge into as it wishes.", [Actor.Coordinator],
    [
        new(Skill.Coordinator, "Merges into your feature branch need no approval but yours.", new(ProjectState.Watching, 223, MoveKind.Nominal)),
        new(Skill.WorkingTreeStandard, "The feature branch is the coordinator's: it merges into it as it wishes.", new(Section.TreeBranches, 2))
    ])
    { Spec = ["R25"] };

    public static readonly Claim OneWayPaths = new("The coordinator keeps the one-way path list in dydo.json.", [Actor.Coordinator],
        [new(Skill.Coordinator, "Keep the one-way path list in `dydo.json`.", new(ProjectState.Watching, 224, MoveKind.Nominal))])
    { Spec = ["R27"] };

    public static readonly Claim MilestoneWalk = new("A Milestone's walkthrough is offered when its last ticket merges.", [Actor.Coordinator],
        [new(Skill.Coordinator, "When a Milestone's last ticket merges into the feature branch, offer its walkthrough through a fresh envoy; a skip is a verdict to go on.", new(ProjectState.Watching, 225, MoveKind.Nominal))])
    { Spec = ["N12 DR 060 §2", "H22"] };

    public static readonly Claim NightSkeleton = new("Overnight, the skeleton's Walkthrough blocks nothing.", [Actor.Coordinator],
        [new(Skill.Coordinator, "Overnight, the skeleton's Walkthrough blocks nothing: post `DECIDED` on it, carry on, and treat a wrong direction as a revert.", new(ProjectState.Watching, 226, MoveKind.Nominal))])
    { Spec = ["N12 DR 060 §4"] };

    public static readonly Claim OnMaster = new("Walkthroughs run on master whenever possible; the landing's walks the candidate.", [Actor.Coordinator],
        [new(Skill.Coordinator, "Run a walkthrough on master whenever possible.", new(ProjectState.Watching, 227, MoveKind.Nominal))])
    { Spec = ["N4"] };

    public static readonly Claim Weeds = new("Every weed becomes a rule.", [Actor.Coordinator],
        [new(Skill.Coordinator, "Give each mistake found after landing a lint, test or rule as well as its fix.", new(ProjectState.Watching, 228, MoveKind.Nominal))])
    { Spec = ["R30"] };

    public static readonly Claim Usage = new("The work never stops for usage, and no officer brings a usage limit to him.",
        [Actor.Coordinator, Actor.TicketCaptain, Actor.ChiefOfStaff],
    [
        new(Skill.Coordinator, "Keep the work going through any usage limit, and never bring one to him.", new(ProjectState.Watching, 229, MoveKind.Nominal)),
        new(Skill.TicketCaptain, "Keep the work going through any usage limit, and never bring one to him.", new(Routine.Dispatching, 229)),
        new(Skill.ChiefOfStaff, "Keep the work going through any usage limit, and never bring one to him.", new(StaffState.Day, 229, MoveKind.Nominal))
    ])
    { Spec = ["H21"] };

    public static readonly Claim Rechart = new("The coordinator re-charts as fog clears.", [Actor.Coordinator],
        [new(Skill.Coordinator, "Re-chart as fog clears: create, split, drop or resequence tickets.", new(Routine.Wayfinding, 230))])
    { Spec = ["H5", "N12 DR 045 §5"] };
}
