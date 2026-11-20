namespace DynaDocs.SkillBuild;

/// <summary>
/// A need for Balazs (M10), by the Question stages (H16, H23; DYD-1195 317eb49f, 3a854e37): decided where it arises,
/// or passed up as a research Question its officer opens and waits on, which its research answers, sends to a council
/// child or puts on his HITL queue as a his-call child; then one envoy at a time (H18; 893e1f5b). The envoy brief's
/// slots and the job skill table (H23).
/// </summary>
public static class Envoy
{
    public static readonly Claim Decide = new("An obvious, reversible call is decided where it arises and never becomes a ticket; the coordinator makes none (DYD-1195 3a854e37).", [Actor.TicketCaptain],
    [
        new(Skill.TicketCaptain, "Decide an obvious, reversible call yourself, where it arises; it never becomes a ticket.", new(EnvoyState.Need, 902, MoveKind.Exit))
    ])
    { Spec = ["H16", "H5"], Transition = new(EnvoyState.Need, EnvoyState.Decided, "the call is obvious and reversible: decided where it arises") };

    public static readonly Claim PassUp = new("Any other need is passed up, at every level: its officer, a captain or the coordinator, opens a research Question for it, held by a fresh captain, and waits on it; only the coordinator dispatches envoys (DYD-1195 317eb49f, 3a854e37).", [Actor.TicketCaptain],
        [new(Skill.TicketCaptain, "Pass any other need up to your officer, who opens a `research` Question for it; only the coordinator dispatches envoys.", new(EnvoyState.Need, 903, MoveKind.Exit))])
    {
        Spec = ["H16", "H12", "H23"],
        Transition = new(EnvoyState.Need, EnvoyState.Research, "passed up: its officer opens a research Question and waits on it") { Effect = "level' \\in {\"captain\", \"coordinator\"}" }
    };

    public static readonly Claim InThread = new("When he is already in an officer's thread, it may ask one line there with the host question tool.", [Actor.TicketCaptain],
    [
        new(Skill.TicketCaptain, "When he is already in your thread, ask the one line there with the host question tool.", new(EnvoyState.Need, 905, MoveKind.Exit))
    ])
    { Spec = ["H22", "H6"], Transition = new(EnvoyState.Need, EnvoyState.AskedInThread, "he is already in this officer's thread") };

    public static readonly Claim Open = new("What only he can settle reaches the coordinator's HITL queue as its own HITL ticket, blocking only what waits on it: the his-call child a research Question opens when the call is worth his time, a Grilling, Walkthrough or Wayfinding, or an Enablement (DYD-1195 317eb49f).", [Actor.Coordinator],
    [
        new(Skill.Coordinator, "A need for him arrives on your HITL queue as its own HITL ticket, a `his-call` Question, a Grilling, Walkthrough or Wayfinding, or an Enablement for what only he can do, a token or an authorisation; each carries the blocking relations of the work that waits on it.",
            new(EnvoyState.Queued, 907, MoveKind.Arrival), () => [Enablement!]),
        new(Skill.LinearWorkspaceStandard, "A need for Balazs becomes a Question, Grilling, Walkthrough, Wayfinding or Enablement ticket; a Question blocks only what waits on it.", new(Section.LinearTickets, 9))
    ])
    {
        Spec = ["H23", "H16", "H15", "H17", "S10"],
        Transition = new(EnvoyState.Research, EnvoyState.Queued, "worth his time: its his-call child joins his HITL queue") { Effect = "level' = \"coordinator\"" }
    };

    public static readonly Claim Send = new("One active envoy per coordinator: the next goes onto the head of the queue only once the last has reported and he has closed its thread (DYD-1195 893e1f5b).", [Actor.Coordinator],
        [new(Skill.Coordinator, "Send the next envoy only once the last has reported and he has closed its thread.", new(EnvoyState.Queued, 908, MoveKind.Exit))])
    {
        Spec = ["H18", "H19", "H23"],
        Transition = new(EnvoyState.Queued, EnvoyState.Active, "the last envoy reported and he closed its thread: the next goes onto the head of the queue")
            { Guard = "envoys = 0", Effect = "envoys' = 1" }
    };

    public static readonly Claim Report = new("The envoy records the result on its ticket and reports back.", [Actor.Coordinator],
        [new(Skill.Coordinator, "Read the envoy's result from its ticket.", new(EnvoyState.Reported, 909, MoveKind.Arrival))])
    { Spec = ["H15", "H19"], Transition = new(EnvoyState.Active, EnvoyState.Reported, "the envoy records the result and reports back") };

    public static readonly Claim Close = new("He closes the envoy's thread once it has reported, after a shared understanding; no moved-on exit frees the queue (DYD-1195 893e1f5b).", [Actor.Balazs], [])
    { Spec = ["H19", "H18"], Transition = new(EnvoyState.Reported, EnvoyState.Closed, "he closes the envoy's thread") { Effect = "envoys' = 0" } };

    // The coordinator states this in Open's combined line.
    public static readonly Claim Enablement = new("What only he can do becomes an Enablement.", [Actor.Coordinator], [])
    { Spec = ["H17"] };

    public static readonly Claim OwnEnvoy = new("An escalation that needs him goes from the coordinator through its own envoy, never through the chief of staff.",
        [Actor.Coordinator, Actor.ChiefOfStaff],
    [
        new(Skill.Coordinator, "Everything that needs him goes out through your own envoy, never through the chief of staff.", new(EnvoyState.Active, 915, MoveKind.Nominal))
    ])
    { Spec = ["H12"] };

    public static readonly Claim Bundle = new("Every waiting need from one origin goes in one wrapper ticket the coordinator opens, which takes the members' place in the queue (DYD-1195 893e1f5b).", [Actor.Coordinator],
        [new(Skill.Coordinator, "Bundle every waiting need from one origin, the ticket whose captain passed them up: open a wrapper ticket, Type Question at stage `his-call` and Mode HITL, parent its member Questions and Enablements under it, and let it take their place in the queue; L13's proposals ride in a wrapper whose origin is the Project.", new(EnvoyState.Queued, 913, MoveKind.Nominal))])
    { Spec = ["H18", "L13"] };

    public static readonly Claim Template = new("An envoy is a fresh ticket captain the coordinator dispatches onto the queued ticket itself, the his-call Question or a bundle's wrapper, opening no other and loading every job skill it needs (DYD-1082 1b6b6a5d Q17, DYD-1195 317eb49f).", [Actor.Coordinator],
        [new(Skill.Coordinator, "Dispatch each envoy onto the ticket at the head of the queue, opening no other: a fresh ticket captain from the ticket-captain brief, naming every job skill from [the envoy brief](resources/envoy-brief.md)'s table that its ticket or a wrapper's members need.", new(EnvoyState.Active, 914, MoveKind.Nominal))])
    { Spec = ["H15", "H16", "H18", "H23", "K14"] };

    public static readonly Claim SlotTicket = Slot(1, "The ticket, and why it needs him.");
    public static readonly Claim SlotSettled = Slot(2, "What's settled.");
    public static readonly Claim SlotSources = Slot(3, "Sources to read first.");
    public static readonly Claim SlotStop = Slot(4, "The stop line.");
    public static readonly Claim SlotRecord = Slot(5, "Where to record, in his words.");
    public static readonly Claim SlotReturn = Slot(6, "Return and closing summary.");
    public static readonly Claim SlotOpening = Slot(7, "The opening rule: a fresh 2–3-line picture, then the first question with its recommendation.");

    public static readonly Claim Updates = new("Changes during a session reach the envoy as dispatcher updates; it never asks him to repeat them.", [Actor.Coordinator, Actor.TicketCaptain],
    [
        new(Skill.Coordinator, "Changes during the session reach you as dispatcher updates; never ask him to repeat them.", new(Section.EnvoySession, 922)),
        new(Skill.TicketCaptain, "Changes during a session with him reach you as dispatcher updates; never ask him to repeat them.", new(TicketCaptainSlot.Hitl, 922))
    ])
    { Spec = ["H19"] };

    public static readonly Claim NameDrs = new("A question put to him names what each DR says, never only its number.", [Actor.Coordinator, Actor.TicketCaptain, Actor.ChiefOfStaff],
    [
        new(Skill.Coordinator, "Name what each DR says whenever you cite one to him, never only its number.", new(Section.EnvoySession, 923)),
        new(Skill.TicketCaptain, "Name what each DR says whenever you cite one to him, never only its number.", new(EnvoyState.Need, 923, MoveKind.Nominal)),
        new(Skill.ChiefOfStaff, "Name what each DR says whenever you cite one to him, never only its number.", new(StaffState.Day, 1011, MoveKind.Nominal))
    ])
    { Spec = ["H24"] };

    public static readonly Claim CloseSignal = new("After a shared understanding, and once it has reported back, the envoy tells him the thread can be closed.", [Actor.Coordinator, Actor.TicketCaptain],
    [
        new(Skill.Coordinator, "After a shared understanding, and once you have reported back, tell him the thread can be closed.", new(Section.EnvoySession, 924)),
        new(Skill.TicketCaptain, "After a shared understanding, and once you have reported back, tell him the thread can be closed.", new(TicketCaptainSlot.Hitl, 924))
    ])
    { Spec = ["H19", "H23"] };

    /// <summary>The job skill table, model data the builder checks against H23: a Question's row is its stage's.</summary>
    public static readonly IReadOnlyList<EnvoyJobSkill> Jobs =
    [
        new(EnvoyJob.Grilling, "grilling"),
        new(EnvoyJob.Walkthrough, "walkthrough"),
        new(EnvoyJob.Wayfinding, "wayfinder"),
        new(EnvoyJob.Question, "ticket-captain", "it dispatches `research` crew") { Stage = QuestionStage.Research },
        new(EnvoyJob.Question, "ticket-captain") { Stage = QuestionStage.Council, Resource = TicketCaptainResource.CouncilChair },
        new(EnvoyJob.Question, "co-thinker") { Stage = QuestionStage.HisCall },
        new(EnvoyJob.Enablement, "wizard", "when it is not a setup procedure he runs (credentials, cutover), just ask")
    ];

    public static readonly Claim SkillTable = new("Each job ticket loads a fixed skill; a Question's stage label picks its row.", [Actor.Coordinator, Actor.TicketCaptain],
    [
        new(Skill.Coordinator, EnvoyTable.Render(Jobs), new(Section.EnvoySkills, 925)),
        new(Skill.TicketCaptain, EnvoyTable.Render(Jobs), new(Section.TicketCaptainBrief, 3005))
    ])
    { Spec = ["H23"] };

    private static Claim Slot(int slot, string text) => new($"The envoy brief's slot {slot}.", [Actor.Coordinator],
        [new(Skill.Coordinator, text, new(Section.EnvoySlots, 914 + slot))])
    { Spec = ["H19"] };
}
