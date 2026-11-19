namespace DynaDocs.SkillBuild;

/// <summary>A need for Balazs (M10): homework first, decided where it can be, up the chain, and out by one envoy at a time; the envoy brief's slots.</summary>
public static class Envoy
{
    public static readonly Claim Take = new("The officer above takes a need for Balazs and does the homework first.", [Actor.TicketCaptain, Actor.Admiral],
    [
        new(Skill.TicketCaptain, "A need for Balazs from below: do the homework first in docs, code, Linear, DRs and the spec.", new(EnvoyState.Homework, 901, MoveKind.Arrival)),
        new(Skill.Admiral, "A need for Balazs, yours or passed up to you: do the homework first in docs, code, Linear, DRs and the spec.", new(EnvoyState.Homework, 901, MoveKind.Arrival))
    ])
    {
        Spec = ["H16"],
        Transition = new(EnvoyState.Need, EnvoyState.Homework, "the officer above takes the need") { Effect = "level' \\in {\"captain\", \"admiral\"}" }
    };

    public static readonly Claim Decide = new("An obvious, reversible call, or one the homework or a DR settles, is decided by the officer.", [Actor.TicketCaptain, Actor.Admiral],
    [
        new(Skill.TicketCaptain, "Decide it yourself when the homework answers it, a DR clearly settles it, or the call is obvious and reversible.", new(EnvoyState.Homework, 902, MoveKind.Exit)),
        new(Skill.Admiral, "Decide it yourself when the homework answers it or the call is obvious and reversible.", new(EnvoyState.Homework, 902, MoveKind.Exit))
    ])
    { Spec = ["H16", "L18"], Transition = new(EnvoyState.Homework, EnvoyState.Decided, "the homework answers it, or the call is obvious and reversible") };

    public static readonly Claim PassUp = new("Captains pass their needs up the chain; only the admiral sends envoys.", [Actor.TicketCaptain],
        [new(Skill.TicketCaptain, "Pass what remains up the chain to your officer; only the admiral sends envoys.", new(EnvoyState.Homework, 903, MoveKind.Exit))])
    { Spec = ["H16", "H12"], Transition = new(EnvoyState.Homework, EnvoyState.PassedUp, "a captain passes what remains up the chain") { Guard = "level = \"captain\"" } };

    public static readonly Claim NextLevel = new("The officer above takes the need passed up.", [Actor.Admiral], [])
    { Spec = ["H12"], Transition = new(EnvoyState.PassedUp, EnvoyState.Homework, "the officer above takes it") { Effect = "level' = \"admiral\"" } };

    public static readonly Claim InThread = new("When he is already in an officer's thread, it may ask one line there with the host question tool.", [Actor.TicketCaptain, Actor.Admiral],
    [
        new(Skill.TicketCaptain, "When he is already in your thread, ask the one line there with the host question tool.", new(EnvoyState.Homework, 905, MoveKind.Exit)),
        new(Skill.Admiral, "When he is already in your thread, ask the one line there with the host question tool.", new(EnvoyState.Homework, 905, MoveKind.Exit))
    ])
    { Spec = ["H22", "H6"], Transition = new(EnvoyState.Homework, EnvoyState.AskedInThread, "he is already in this officer's thread") };

    public static readonly Claim Hard = new("Hard calls go to the council: the tribunal.", [Actor.Admiral],
        [new(Skill.Admiral, "Send a hard call, one with competing answers each good through some lens, to the tribunal.", new(EnvoyState.Homework, 906, MoveKind.Exit))])
    { Spec = ["H5", "L18"], Transition = new(EnvoyState.Homework, EnvoyState.ToTribunal, "a hard call: competing answers, each good through some lens") { Guard = "level = \"admiral\"" } };

    public static readonly Claim Open = new("What only he can settle becomes an envoy job's ticket; a Question blocks only what waits on it.", [Actor.Admiral],
    [
        new(Skill.Admiral, "For what only he can settle, open a Question, Grilling, Walkthrough, Wayfinding or Enablement ticket; a Question blocks only what waits on it.", new(EnvoyState.Homework, 907, MoveKind.Exit)),
        new(Skill.LinearWorkspaceStandard, "A need for Balazs becomes a Question, Grilling, Walkthrough, Wayfinding or Enablement ticket; a Question blocks only what waits on it.", new(Section.LinearTickets, 9))
    ])
    { Spec = ["H15", "H17", "S10"], Transition = new(EnvoyState.Homework, EnvoyState.Queued, "only he can settle it") { Guard = "level = \"admiral\"" } };

    public static readonly Claim Send = new("One active envoy per admiral; the next goes once the last reported and he closed its thread.", [Actor.Admiral],
        [new(Skill.Admiral, "Dispatch the next envoy only once the last has reported and he has closed its thread.", new(EnvoyState.Queued, 908, MoveKind.Exit))])
    {
        Spec = ["H18", "H19", "H23"],
        Transition = new(EnvoyState.Queued, EnvoyState.Active, "the last envoy reported and he closed its thread") { Guard = "envoys = 0", Effect = "envoys' = 1" }
    };

    public static readonly Claim Report = new("The envoy records the result on its ticket and reports back.", [Actor.Admiral],
        [new(Skill.Admiral, "Read the envoy's result from its ticket.", new(EnvoyState.Reported, 909, MoveKind.Arrival))])
    { Spec = ["H15", "H19"], Transition = new(EnvoyState.Active, EnvoyState.Reported, "the envoy records the result and reports back") };

    public static readonly Claim Close = new("After a shared understanding he closes the envoy's thread.", [Actor.Balazs], [])
    { Spec = ["H19", "H18"], Transition = new(EnvoyState.Reported, EnvoyState.Closed, "he closes the envoy's thread") { Effect = "envoys' = 0" } };

    public static readonly Claim MovedOn = new("When he has plainly moved on without closing the thread, the next envoy may go at the admiral's discretion.", [Actor.Admiral],
        [new(Skill.Admiral, "When he has plainly moved on without closing its thread, use your discretion on sending the next envoy.", new(EnvoyState.Reported, 910, MoveKind.Exit))])
    {
        Spec = ["H18"],
        Transition = new(EnvoyState.Reported, EnvoyState.MovedOn, "he has plainly moved on, the thread still open: the next envoy may go, at the admiral's discretion") { Effect = "envoys' = 0" }
    };

    public static readonly Claim Enablement =new("What only he can do becomes an Enablement.", [Actor.Admiral],
        [new(Skill.Admiral, "What only he can do, a token or an authorisation, becomes an Enablement ticket.", new(EnvoyState.Homework, 911, MoveKind.Nominal))])
    { Spec = ["H17"] };

    public static readonly Claim OwnEnvoy = new("An escalation that needs him goes from the admiral through its own envoy, never through the chief of staff.",
        [Actor.Admiral, Actor.ChiefOfStaff],
    [
        new(Skill.Admiral, "An escalation that needs him goes out through your own envoy, never through the chief of staff.", new(EnvoyState.Homework, 912, MoveKind.Nominal))
    ])
    { Spec = ["H12"] };

    public static readonly Claim Bundle = new("The queue behind the active envoy is triaged at the admiral's discretion, bundled per origin.", [Actor.Admiral],
        [new(Skill.Admiral, "Triage the queue behind the active envoy at your discretion, bundling needs from one origin into one envoy.", new(EnvoyState.Queued, 913, MoveKind.Nominal))])
    { Spec = ["H18"] };

    public static readonly Claim Template = new("Each envoy is briefed from the envoy brief and loads its job's skill.", [Actor.Admiral],
        [new(Skill.Admiral, "Brief each envoy from [the envoy brief](resources/envoy-brief.md), with its job's skill from the table there.", new(EnvoyState.Active, 914, MoveKind.Nominal))])
    { Spec = ["H19", "H23"] };

    public static readonly Claim SlotTicket = Slot(1, "The ticket, and why it needs him.");
    public static readonly Claim SlotSettled = Slot(2, "What's settled.");
    public static readonly Claim SlotSources = Slot(3, "Sources to read first.");
    public static readonly Claim SlotStop = Slot(4, "The stop line.");
    public static readonly Claim SlotRecord = Slot(5, "Where to record, in his words.");
    public static readonly Claim SlotReturn = Slot(6, "Return and closing summary.");
    public static readonly Claim SlotOpening = Slot(7, "The opening rule: a fresh 2–3-line picture, then the first question with its recommendation.");

    public static readonly Claim Updates = new("Changes during a session reach the envoy as dispatcher updates; it never asks him to repeat them.", [Actor.Admiral],
        [new(Skill.Admiral, "Changes during the session reach you as dispatcher updates; never ask him to repeat them.", new(Section.EnvoySession, 922))])
    { Spec = ["H19"] };

    public static readonly Claim NameDrs = new("A question put to him names what each DR says, never only its number.", [Actor.Admiral, Actor.TicketCaptain, Actor.ChiefOfStaff],
    [
        new(Skill.Admiral, "Name what each DR says whenever you cite one to him, never only its number.", new(Section.EnvoySession, 923)),
        new(Skill.TicketCaptain, "Name what each DR says whenever you cite one to him, never only its number.", new(EnvoyState.Homework, 923, MoveKind.Nominal)),
        new(Skill.ChiefOfStaff, "Name what each DR says whenever you cite one to him, never only its number.", new(StaffState.Day, 923, MoveKind.Nominal))
    ])
    { Spec = ["H24"] };

    public static readonly Claim CloseSignal = new("After a shared understanding, and once it has reported back, the envoy tells him the thread can be closed.", [Actor.Admiral],
        [new(Skill.Admiral, "After a shared understanding, and once you have reported back, tell him the thread can be closed.", new(Section.EnvoySession, 924))])
    { Spec = ["H19", "H23"] };

    /// <summary>The envoy skill table, model data the builder checks against H23.</summary>
    public static readonly IReadOnlyList<EnvoyJobSkill> Jobs =
    [
        new(EnvoyJob.Grilling, "grilling"),
        new(EnvoyJob.Walkthrough, "walkthrough"),
        new(EnvoyJob.Wayfinding, "wayfinder"),
        new(EnvoyJob.Question, "co-thinker"),
        new(EnvoyJob.Enablement, "wizard", "when it is not a setup procedure he runs (credentials, cutover), just ask")
    ];

    public static readonly Claim SkillTable = new("Each envoy job loads a fixed skill.", [Actor.Admiral],
        [new(Skill.Admiral, EnvoyTable.Render(Jobs), new(Section.EnvoySkills, 925))])
    { Spec = ["H23"] };

    private static Claim Slot(int slot, string text) => new($"The envoy brief's slot {slot}.", [Actor.Admiral],
        [new(Skill.Admiral, text, new(Section.EnvoySlots, 914 + slot))])
    { Spec = ["H19"] };
}
