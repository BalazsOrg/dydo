namespace DynaDocs.SkillBuild;

/// <summary>
/// The coordinator in its 4.0.0 shape (H5 as DYD-1082 rounds 3–5 and DYD-1195 envoy 5 ruled): mechanical and
/// replaceable, woken stateless by him or the chief of staff, routing every judgement to a fresh captain's
/// ticket, and holding the map together with a Wayfinding captain. It also closes M1's two gaps, idle and wake.
/// </summary>
public static class Coordination
{
    public static readonly Claim Mechanical = new("The coordinator decides mechanically only, with no exception for obvious calls; anything needing judgement becomes a ticket for a fresh agent (DYD-1082 1b6b6a5d Q15).", [Actor.Coordinator],
        [new(Skill.Coordinator, "You make no judgement call, not even an obvious one: you sequence by blockers, restart dead lanes, pace merges and open the tickets the plan already names, and anything that needs judgement becomes a ticket for a fresh agent.", new(SpinePart.Job, 103))])
    { Spec = ["H5", "H16"] };

    public static readonly Claim MapHolder = new("The map holder is the coordinator plus a fresh captain on a Wayfinding ticket: the coordinator keeps things on track, the captain decides (DYD-1195 b46e8343).", [Actor.Coordinator],
        [new(Skill.Coordinator, "You and the captain you wake on a Wayfinding ticket are together the Project's map holder: you keep it on track, and that captain does the deciding.", new(SpinePart.Job, 104))])
    { Spec = ["H4", "S14", "S15", "H9"] };

    public static readonly Claim NotCreation = new("Creating a Project is HITL and not the coordinator's job; it comes in once the Project is up (DYD-1082 555a8300 Q21).", [Actor.Coordinator],
        [new(Skill.Coordinator, "You come in once the Project is up: creating it, with its grillings and first wayfindings, is his HITL work.", new(SpinePart.Job, 105))])
    { Spec = ["H5", "H4"] };

    public static readonly Claim Replaceable = new("The coordinator is replaceable: its state lives in Linear and dydo survey, so a fresh one takes over with no loss (DYD-1082 1b6b6a5d Q18).", [Actor.Coordinator],
        [new(Skill.Coordinator, "You are replaceable: your state lives in Linear and `dydo survey`, so a fresh coordinator takes over with no loss.", new(SpinePart.Job, 106))])
    { Spec = ["H5"] };

    public static readonly Claim DayAndNight = new("The coordinator has no overnight mode and behaves the same day and night; a stop line he wants goes in the Project plan (DYD-1195 33520d6c).", [Actor.Coordinator],
        [new(Skill.Coordinator, "You run the same way day and night, with no overnight mode; when he wants a stop line, it is in the Project plan, and you stop there.", new(SpinePart.Job, 107))])
    { Spec = ["K11", "H7"] };

    public static readonly Claim WakeUp = new("The coordinator wakes up stateless: it reads the Project from Linear and dydo survey (DYD-1082 555a8300 Q21).", [Actor.Coordinator],
        [new(Skill.Coordinator, "Read the Project from Linear, its plan included, then `dydo survey board <project>`; they, not your thread's memory, are the record.", new(CoordinatorSlot.WakeUp, 1301))])
    { Spec = ["H5", "S22"] };

    public static readonly Claim Wake = new("He starts or wakes a Project's coordinator, or the chief of staff does (DYD-1082 555a8300 gap 2).", [Actor.Balazs, Actor.ChiefOfStaff, Actor.Coordinator],
    [
        new(Skill.Coordinator, "He or the chief of staff starts or wakes you: wake up stateless, then keep the lanes full.", new(ProjectState.Unheld, 1303, MoveKind.Exit)),
        new(Skill.ChiefOfStaff, "Start or wake a Project's coordinator from the coordinator brief.", new(ProjectState.Unheld, 1303, MoveKind.Exit))
    ])
    { Spec = ["H4", "H7"], Transition = new(ProjectState.Unheld, ProjectState.Watching, "he or the chief of staff starts or wakes its coordinator") };

    public static readonly Claim Idle = new("A Project is idle when its Linear status is Paused, or when no coordinator or other agent is running in it (DYD-1082 555a8300 gap 1).", [Actor.Balazs, Actor.Coordinator],
        [new(Skill.Coordinator, "When the Project's Linear status is Paused, it is idle: stop until he or the chief of staff wakes you.", new(ProjectState.Watching, 1304, MoveKind.Exit))])
    { Spec = ["H4"], Transition = new(ProjectState.Watching, ProjectState.Unheld, "Paused in Linear, or no coordinator or agent running in it: idle") };

    // The coordinator's line for Roster.Usage is this combined line: R14's one fixed rule is how it balances vendors.
    public static readonly Claim Seats = new("The coordinator decides no seat: one fixed rule spends the soonest-resetting headroom in the seat's tier, skipping a vendor below 20% unless every vendor is.", [Actor.Coordinator],
        [new(Skill.Coordinator, "Staff every seat by one fixed rule, never by judgement: from the roster's models in its tier whose vendor still has quota in `dydo usage`, skipping a vendor below 20% unless every vendor is, take the vendor that resets soonest, then more remaining quota, then roster order.",
            new(Routine.Dispatching, 2104), () => [Roster.Usage])])
    { Spec = ["R14", "H21", "H5"] };

    public static readonly Claim SeatTiers = new("Wayfinding captains, Question captains and envoys take the strong tier, every other captain the normal tier; a seat a Wayfinding captain records replaces the rule.", [Actor.Coordinator],
        [new(Skill.Coordinator, "Wayfinding captains, Question captains and envoys take the strong tier, and every other captain the normal tier; a seat a Wayfinding captain recorded on its ticket replaces the rule, and you dispatch exactly that.", new(Routine.Dispatching, 2105))])
    { Spec = ["R14", "R13", "H2"] };

    public static readonly Claim Question = new("A need passed up to the coordinator becomes a research Question it opens and waits on, held by a fresh captain (DYD-1195 317eb49f, 3a854e37).", [Actor.Coordinator],
        [new(Skill.Coordinator, "A need passed up to you becomes a Question ticket at stage `research`: open it, put a fresh captain on it, and wait; that captain answers the need, sends it to a council, or puts a `his-call` Question on your HITL queue.", new(EnvoyState.Research, 1305, MoveKind.Arrival))])
    { Spec = ["H16", "H23", "H5", "L18"] };

    public static readonly Claim Rank = new("The HITL queue is triaged mechanically: the tickets waiting on a need over his estimated minutes, highest first, the oldest on a tie (DYD-1195 893e1f5b).", [Actor.Coordinator],
        [new(Skill.Coordinator, "Send next the need or bundle that unlocks the most autonomous agent work, weighed against his time: the tickets waiting on it through blocking relations, divided by the minutes its asker estimates for him, a bundle summing its estimates; a tie goes to the oldest, and a wrong pick is cheap.", new(EnvoyState.Queued, 1307, MoveKind.Nominal))])
    { Spec = ["H18"] };

    public static readonly Claim Unestimated = new("A need with no estimate gets one asked of its asker; still missing, it ranks after every estimated one.", [Actor.Coordinator],
        [new(Skill.Coordinator, "Ask the asker for a missing estimate; a need or bundle still missing one ranks after every estimated one, more waiting tickets first, then the oldest.", new(EnvoyState.Queued, 1308, MoveKind.Nominal))])
    { Spec = ["H18"] };

    public static readonly Claim NoSlots = new("The coordinator writes no envoy brief slot; on a wrapper it opens it fills only Type, stage, Mode and the members.", [Actor.Coordinator],
        [new(Skill.Coordinator, "Write none of the envoy brief's seven slots, which come from its ticket; on a wrapper you fill only its Type, stage, Mode and the members you parent.", new(EnvoyState.Active, 1309, MoveKind.Nominal))])
    { Spec = ["H19"] };

    public static readonly Claim FillerBack = new("A filler that fails the pull rule's checks is blocked on the open Wayfinding ticket, opened if none is, until one records its release or he assigns it.", [Actor.Coordinator, Actor.TicketCaptain],
    [
        new(Skill.Coordinator, "A filler its captain sends back unassigned, with a one-line reason: block it on the open Wayfinding ticket, opening one if none is open, and pull it again only once a Wayfinding ticket records its release or he assigns it.", new(CoordinatorSlot.Filler, 1310)),
        new(Skill.TicketCaptain, "Dispatched onto a filler ticket, check it is small, two-way and needs no design call; if not, send it back unassigned with a one-line reason.", new(TicketCaptainSlot.Take, 123))
    ])
    { Spec = ["H11", "H4"] };

    public static readonly Claim FillerBlockMoves = new("A Wayfinding ticket that closes without recording a filler's release does not release it; its block moves on.", [Actor.Coordinator],
        [new(Skill.Coordinator, "When that Wayfinding ticket closes without recording the release, move the block to the next open Wayfinding ticket, opening one if none is open.", new(CoordinatorSlot.Filler, 1311))])
    { Spec = ["H11"] };

    public static readonly Claim Brief = new("The coordinator dispatch template's slots (DYD-1195 e0eadc20).", [Actor.ChiefOfStaff],
        [new(Skill.ChiefOfStaff, "The Project · model and effort.", new(ChiefOfStaffSlot.CoordinatorBrief, 1306))])
    { Spec = ["K14"] };
}
