namespace DynaDocs.SkillBuild;

/// <summary>
/// A ticket captain on a job ticket (H15, H23; DYD-1082 1b6b6a5d Q16–Q17): a Question by its stage, a
/// Wayfinding ticket as the deciding half of the map holder (DYD-1195 b46e8343), and a HITL job with him;
/// and the Question ticket every level opens for a need passed up to it (DYD-1195 317eb49f, 3a854e37).
/// </summary>
public static class JobTickets
{
    private const string Recipe = "[Job tickets](resources/job-tickets.md)";

    public static readonly Claim Hold = new("A ticket captain holds each Question, Grilling, Walkthrough, Wayfinding and Enablement ticket with its job's skill; whether it is HITL depends on the ticket (DYD-1082 1b6b6a5d Q17).",
        [Actor.TicketCaptain],
        [new(Skill.TicketCaptain, "A Question, Grilling, Walkthrough, Wayfinding or Enablement ticket is held the same way, with its job's skill loaded, and whether you work it with him depends on the ticket.", new(SpinePart.Job, 3101))])
    { Spec = ["H15", "H23"] };

    public static readonly Claim Route = new("A job ticket's captain works it by the job-tickets resource.", [Actor.TicketCaptain],
        [new(Skill.TicketCaptain, $"Holding a Question, Grilling, Walkthrough, Wayfinding or Enablement ticket, work it by {Recipe}.", new(TicketCaptainSlot.Take, 120))])
    { Spec = ["H15", "H23", "K9"] };

    public static readonly Claim OpenQuestion = new("Every level opens a Question ticket at stage research for a need passed up to it, held by a fresh captain, and waits on it (DYD-1195 3a854e37).",
        [Actor.TicketCaptain],
        [new(Skill.TicketCaptain, "A need passed up to you from below: open a Question sub-ticket at stage `research` for a fresh captain, and wait on it; never research it yourself.", new(TicketCaptainSlot.NeedPassedUp, 3103))])
    { Spec = ["H16", "H23"] };

    public static readonly Claim Stages = new("A Question's stage label, set by whoever opens it, sets its Mode and picks how it is held, at any depth.", [Actor.TicketCaptain],
        [new(Skill.TicketCaptain, "Every Question carries a stage label, set by whoever opens it: `research` and `council` are Mode AFK, `his-call` Mode HITL, and the stage, not the parent's Type, picks how it is held.", new(TicketCaptainSlot.Question, 3104))])
    { Spec = ["H23", "S3"] };

    // The research Question's other exits, to a council child or a his-call child, are its call's routing in the tribunal
    // (Tribunal.File, Tribunal.HisCall); in the envoy machine they are that child's arrival (Council.Hold, Envoy.Open).
    public static readonly Claim Research = new("A research Question's captain researches first with research crew, and opens a council or his-call child only when the research does not resolve the call.",
        [Actor.TicketCaptain],
        [new(Skill.TicketCaptain, "At stage `research`, research the call first with `research` crew; open a `council` or `his-call` child only when the research does not resolve it.", new(EnvoyState.Research, 3105, MoveKind.Exit))])
    { Spec = ["H23", "H16", "L18", "K30"], Transition = new(EnvoyState.Research, EnvoyState.Answered, "its research resolves the call") };

    public static readonly Claim StaysResearch = new("A research Question opened under another Question, at any depth, stays at stage research.", [Actor.TicketCaptain],
        [new(Skill.TicketCaptain, "A research Question you open under another Question stays at stage `research`.", new(TicketCaptainSlot.Question, 3106))])
    { Spec = ["H23"] };

    public static readonly Claim EnvoySlots = new("The envoy's own ticket captain writes the envoy brief's seven slots from its ticket, the his-call Question or a bundle's wrapper; the coordinator writes none.", [Actor.TicketCaptain],
        [new(Skill.TicketCaptain, "Holding a `his-call` Question or a bundle's wrapper, write the envoy brief's seven slots from it before you open with him: the ticket and why it needs him · what's settled · sources to read first · the stop line · where to record, in his words · return and closing summary · the opening rule, a fresh 2–3-line picture, then the first question with its recommendation.", new(TicketCaptainSlot.Hitl, 921))])
    { Spec = ["H19", "H18", "H23"] };

    public static readonly Claim Deciding = new("A Wayfinding captain is the deciding half of the map holder; the coordinator keeps it on track (DYD-1195 b46e8343).", [Actor.TicketCaptain],
        [new(Skill.TicketCaptain, "Holding a Wayfinding ticket, you are the deciding half of the Project's map holder: the coordinator keeps the map on track, mechanically, and every call on it is yours.", new(TicketCaptainSlot.Wayfinding, 3108))])
    { Spec = ["H4", "H5"] };

    public static readonly Claim SpecGuards = new("The spec, not a DR, is the guard: every feature has one.", [Actor.TicketCaptain],
        [new(Skill.TicketCaptain, "Give every feature a spec, whether or not a DR backs it.", new(TicketCaptainSlot.Wayfinding, 3111))])
    { Spec = ["S16"] };

    public static readonly Claim Milestones = new("The map holder's Wayfinding captain places few Milestones while charting, the walking skeleton first.", [Actor.TicketCaptain],
        [new(Skill.TicketCaptain, "While charting, place the Project's Milestones in Linear: few, each a chunk where drift from his intent would show, the walking skeleton first and the landing last; every delivery ticket belongs to one.", new(TicketCaptainSlot.Wayfinding, 3115))])
    { Spec = ["N12 DR 060 §1"] };

    public static readonly Claim Skeleton = new("Only the walking skeleton's Walkthrough blocks the next Milestone.", [Actor.TicketCaptain],
        [new(Skill.TicketCaptain, "File the walking skeleton's Walkthrough with the map, as a native blocker of the next Milestone's tickets until his verdict or skip; later Milestones' Walkthroughs block nothing.", new(TicketCaptainSlot.Wayfinding, 3116))])
    { Spec = ["N12 DR 060 §3"] };

    public static readonly Claim SeatRecord = new("A seat a Wayfinding captain records on a ticket replaces the coordinator's fixed rule for it, applied unless it records why not.", [Actor.TicketCaptain],
        [new(Skill.TicketCaptain, "A seat you record on a ticket you cut replaces the coordinator's fixed rule for it: apply the same rule unless you record why not.", new(TicketCaptainSlot.Wayfinding, 3117))])
    { Spec = ["R14", "H2"] };

    public static readonly Claim FillerRelease = new("A filler blocked on a Wayfinding ticket can be pulled again only once a Wayfinding ticket records its release.", [Actor.TicketCaptain],
        [new(Skill.TicketCaptain, "A filler ticket blocked on yours stays unpullable until you record its release on your ticket.", new(TicketCaptainSlot.Wayfinding, 3118))])
    { Spec = ["H11"] };

    public static readonly Claim Result = new("A job ticket's result is recorded on the ticket.", [Actor.TicketCaptain],
        [new(Skill.TicketCaptain, "Record the result on your ticket, in his words.", new(TicketCaptainSlot.Hitl, 3119))])
    { Spec = ["H15", "H19"] };

    public static readonly Claim Wrapper = new("A bundle wrapper's captain loads every job skill its members need.", [Actor.TicketCaptain],
        [new(Skill.TicketCaptain, "Holding a bundle's wrapper, load every job skill its members need: `wizard` for an Enablement, `co-thinker` for a `his-call` Question.", new(TicketCaptainSlot.Hitl, 3120))])
    { Spec = ["H18", "H23"] };
}
