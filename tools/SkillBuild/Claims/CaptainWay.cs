namespace DynaDocs.SkillBuild;

/// <summary>
/// The ticket-captain prototype's third way to hold a ticket (spec H10; DYD-1372 777d396e, 07c86aad): a captain
/// that cannot see the route from its ticket to the spec finds the way before it cuts, a part at a
/// time. The hand-written wayfinder skill says how fog is charted and cleared; these claims carry only
/// what the ticket's captain does with it, each held against that skill so the two never disagree.
/// </summary>
public static class CaptainWay
{
    private const string Wayfinder = "`skills/productivity/wayfinder/SKILL.md`, read 2026-11-21: ";
    private const string Labels = "Linear, team Dydo, labels read 2026-11-21: ";

    private static readonly Enum[] Me = [Actor.TicketCaptain];

    public static readonly Claim Route = new("A captain sees the route from its ticket to the spec when it can write every part's contract now, and it checks that before it cuts.", Me,
    [
        At(Actor.TicketCaptain, CaptainSlot.Fog, 1,
            "Before you cut a bigger ticket, check that you see its route to the spec: that you can write every part's contract now.")
    ])
    {
        Spec = ["H10", "S15"],
        Truth = new(Evidence.Run(Wayfinder + "it sets the same test, \"whether you can state the question precisely now, _not_ whether you can answer it now\", " +
                                 "and has its map holder \"Create the tickets you can specify now\"."),
            Layer: "AGENTS.md says a bigger ticket splits; nothing below this skill says when its holder may not cut yet.",
            Reason: "A holder that cuts what nobody can state yet hands its children guesses, and each guess is paid for in rework.")
    };

    public static readonly Claim Fog = new("A captain that cannot see the route loads the wayfinder skill and finds the way a part at a time; it never cuts the whole at once.", Me,
    [
        At(Actor.TicketCaptain, CaptainSlot.Fog, 2,
            "If you cannot, load `/wayfinder` and find the way a part at a time, by [Finding the way](resources/wayfind.md): never cut the whole at once.")
    ])
    {
        Spec = ["H10", "H23"],
        Truth = new(Evidence.Run(Wayfinder + "its description is \"Fog in a Project map or inside one ticket\", and it lets a captain act on it: " +
                                 "\"A Ticket Captain may chart local fog inside one approved delivery outcome.\" The build fails when `/wayfinder` names no skill under `skills/`."),
            Layer: "The host lists the wayfinder among its skills and loads it only when asked, and until this line a captain was asked only when its brief named it.",
            Reason: "A feature cut whole in the fog fixes every later part before the first has shown what the next should be."),
        Transition = new(CaptainState.Taken, CaptainState.Wayfinding, "the route to the spec is not all in sight: the way is found before the cut")
    };

    public static readonly Claim Visible = new("While it finds the way, a captain cuts only the parts whose contract it can write now, each sized to one agent, and leaves the fog uncut.", Me,
    [
        At(Actor.TicketCaptain, CaptainSlot.WaySteps, 1,
            "Cut only the parts whose contract you can write now, each small enough for one agent, by [Sub-tickets](sub-tickets.md), and leave the fog uncut. " +
            "A question whose answer would clear fog is such a part.")
    ])
    {
        Spec = ["H10", "S2"],
        Truth = new(Evidence.Run(Wayfinder + "\"Create the tickets you can specify now\", \"everything you cannot yet specify stays in the fog\", " +
                                 "\"Don't pre-slice the fog into ticket-sized pieces\", a ticket's body \"carries one outcome sized to one agent session\", " +
                                 "and \"Wayfinding Tickets clear the fog blocking what comes next\"."),
            Layer: "Linear takes a sub-ticket for a part nobody can state yet as readily as for any other.",
            Reason: "Small parts keep the work moving while the rest is still dark (07c86aad: \"It keeps the tasks bite sized so it's easy to move forward\")."),
        Transition = new(CaptainState.Wayfinding, CaptainState.Split, "the parts in sight are cut, and the fog is left uncut")
    };

    public static readonly Claim Next = new("Each part that returns clears fog: the captain cuts the parts whose contract the result lets it write, until no fog is left.", Me,
    [
        At(Actor.TicketCaptain, CaptainSlot.WaySteps, 2,
            "When a part returns, cut the parts whose contract its result lets you write, and repeat until no fog is left.")
    ])
    {
        Spec = ["H10", "S15"],
        Truth = new(Evidence.Run(Wayfinder + "\"Resolving a ticket clears the fog ahead of it, graduating whatever's now specifiable into fresh tickets, one at a time, " +
                                 "until the way to the destination is clear\"; and \"Defer later decisions until delivery reveals the facts they need.\""),
            Layer: "T3 wakes a captain when a child returns, and nothing looks again at what that result has made plain.",
            Reason: "A finished part is the cheapest evidence of what the next one should be (07c86aad: \"the wisdom of removing the fog gradually\")."),
        Transition = new(CaptainState.Waiting, CaptainState.Wayfinding, "a part returns and fog remains: the way is found again from what it showed")
    };

    public static readonly Claim Ahead = new("What a captain can see ahead needs the user is gathered into one Grilling sub-ticket, Mode HITL, so one sitting settles the large sections before their parts are cut.", Me,
    [
        At(Actor.TicketCaptain, CaptainSlot.WayUser, 1,
            "Settle ahead what you can see ahead: gather every call you already know is the user's into one Grilling sub-ticket, Mode HITL, " +
            "so that one sitting settles the large sections before their parts are cut.")
    ])
    {
        Spec = ["H10", "H15"],
        Truth = new(Evidence.Run(Labels + "the Type group holds Grilling, \"Intent or specification choices resolved with the human and recorded.\", and the Mode group HITL. " +
                                 Wayfinder + "\"a session the user drives never resolves more than one Wayfinding Ticket\", so one ticket is one sitting. " +
                                 "The user, 07c86aad: \"sometimes I am needed for that in a HITL grilling session. Most of the time that's seen ahead of time. So large sections can be preemptively settled\"."),
            Layer: "Linear takes a HITL ticket for every single call, and nothing gathers the ones a captain could have seen together.",
            Reason: "His attention comes in sittings, and a point that could have joined an earlier sitting costs him a second start.")
    };

    public static readonly Claim Timed = new("A later point that needs the user, a call or a prototype to look at, is a HITL sub-ticket of its own that blocks only the parts its answer could change.", Me,
    [
        At(Actor.TicketCaptain, CaptainSlot.WayUser, 2,
            "Give each later point that needs the user, a call to make or a prototype to look at, a HITL sub-ticket of its own, " +
            "blocking only the parts its answer could change: the user then sees it before that work is built.")
    ])
    {
        Spec = ["H10", "S10"],
        Truth = new(Evidence.Run(Labels + "the Type group holds Prototype, \"A design question tested through a sketch; closes on the human's verdict.\" " +
                                 "Linear, DYD-1372 read 2026-11-20: a ticket carries `blocks` and `blockedBy` relations. " +
                                 "The user, 07c86aad: \"there may be still some calls made, or prototypes to be looked at which saves a lot of work down the line. " +
                                 "So these human points in the workflow have to be timed very cleverly.\""),
            Layer: "Linear draws a block wherever it is set; where a look saves the most work is known only to the captain that sees the route.",
            Reason: "A look that comes after the work is built can only order it rebuilt.")
    };

    public static readonly Claim Reach = new("A HITL sub-ticket is a need for the user and goes up by that procedure.", Me,
        [At(Actor.TicketCaptain, CaptainSlot.WayUser, 3, "Each HITL sub-ticket is a need for the user: it goes up by [A need for the user](need-for-the-user.md).")])
    {
        Spec = ["H16", "H15"],
        Truth = new(Evidence.Run(Wayfinder + "\"A HITL ticket only resolves through that live exchange; the agent never stands in for the user's side of it\". " +
                                 "`skills/roles/officers/coordinator/SKILL.md`, the coordinator's skill on this branch, not yet truth-checked, takes each one on its queue: " +
                                 "\"A need for him arrives on your HITL queue as its own HITL ticket\", a Grilling among them."),
            Layer: "`delegate_task` starts an agent on any ticket, a HITL one included, and an agent alone cannot give the user's side of it.",
            Reason: "A captain that staffs its own HITL ticket gets an agent talking to itself where the user was meant to speak.")
    };

    private static Line At(Actor reader, Enum slot, int order, string text) =>
        new(CaptainSkill.TicketCaptain, reader, text, new(slot, order));
}
