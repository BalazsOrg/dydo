namespace DynaDocs.SkillBuild;

/// <summary>
/// The ticket-captain prototype's "Do it or split it" (DYD-1372). AGENTS.md says when a ticket is a
/// leaf and how work is cut; these claims say only what the captain then does (DYD-1295 0f340abd).
/// </summary>
public static class CaptainSplit
{
    private const string Children = "Linear, DYD-1281's sub-tickets read 2026-11-20: twenty, titled Review r1 to r4, Fix r1 to r3, Merge … and by part, each a child of the ticket.";

    private static readonly Enum[] Me = [Actor.TicketCaptain];

    public static readonly Claim Leaf = new("A ticket that fits one agent is a leaf, done by its holder with native helpers.", Me,
        [At(Actor.TicketCaptain, CaptainSlot.Leaf, 1, "A ticket that fits you is a leaf: do it as your kind's resource says, with native helpers for lookups and side work.")])
    {
        Spec = ["S2", "S7"],
        Truth = new(Evidence.Code("AGENTS.md"),
            Layer: "AGENTS.md sizes a leaf; what its holder then does is said nowhere below this skill.",
            Reason: "Below one agent's size, a split costs more in hand-offs than it saves."),
        Transition = new(CaptainState.Taken, CaptainState.Leaf, "the ticket fits one agent")
    };

    public static readonly Claim Helpers = new("A native helper's work is committed by its holder.", Me,
        [At(Actor.TicketCaptain, CaptainSlot.Leaf, 2, "You commit what a helper changes.")])
    {
        Spec = ["H3", "S1"],
        Truth = new(Evidence.Tool("Agent: a native sub-agent runs inside its caller's session and reports only to the caller."),
            Layer: "The host starts the helper; whether its work is recorded and committed is left to the caller.",
            Reason: "A helper's edits are part of the holder's change, so they land under the holder's commit and its review.")
    };

    public static readonly Claim Uses = new("Work gets a ticket only when someone other than its asker uses what it leaves.", Me,
        [At(Actor.TicketCaptain, CaptainSlot.Split, 2, "Work gets a ticket only when someone other than its asker uses what it leaves.")])
    {
        Spec = ["S1"],
        Truth = new(Evidence.Run(Children),
            Layer: "Linear takes any ticket anyone files; only the cutter knows who will use what the work leaves.",
            Reason: "A record nobody else reads is bookkeeping that slows the work it describes.")
    };

    public static readonly Claim OffRecord = new("A helper, a lookup, a command, a file and one seat among several are always off the record.", Me,
        [At(Actor.TicketCaptain, CaptainSlot.Split, 3, "A helper, a lookup, a command, a file and one seat among several never get a ticket.")])
    {
        Spec = ["S1"],
        Truth = new(Evidence.Run(Children + " None of the twenty is a helper's errand, a lookup, a command, a file or a single seat of a round."),
            Layer: "Linear takes a ticket for a lookup or a file as readily as for a feature, and nothing stops a captain from filing one.",
            Reason: "Each of the five is a piece of some ticket's work, and a card for every piece would bury the board.")
    };

    public static readonly Claim OneTicket = new("Several steps given to one agent are one ticket.", Me,
        [At(Actor.TicketCaptain, CaptainSlot.Split, 4, "Several steps for one agent are one ticket.")])
    {
        Spec = ["S7"],
        Truth = new(Evidence.Code("AGENTS.md"),
            Layer: "AGENTS.md says one agent holds one ticket; the cutter is who keeps a second from being opened.",
            Reason: "An agent that understands something once uses it across every step it was given.")
    };

    public static readonly Claim Cut = new("A ticket bigger than one agent is cut into sub-tickets, one per part another captain holds.", Me,
    [
        At(Actor.TicketCaptain, CaptainSlot.Split, 1,
            "Cut a bigger ticket into sub-tickets, one per part another captain will hold.")
    ])
    {
        Spec = ["H13", "H14"],
        Truth = new(Evidence.Run(Children),
            Layer: "AGENTS.md says to cut along what one agent understands together; opening the tickets is the holder's act.",
            Reason: "Each part then has a single owner who answers for it."),
        Transition = new(CaptainState.Taken, CaptainState.Split, "the ticket is bigger than one agent: it is cut")
    };

    public static readonly Claim Contract = new("A sub-ticket's description is its contract: its outcome, how it fits the ticket above, and the spec sections it serves, quoted and linked.", Me,
    [
        At(Actor.TicketCaptain, CaptainSlot.Split, 5,
            "Write each sub-ticket's contract in its description: its outcome, how it fits your ticket, and the spec sections it serves, quoted and linked.")
    ])
    {
        Spec = ["S15"],
        Truth = new(Evidence.Run("DYD-1372's contract, read 2026-11-20, is written this way and was the whole of what its writer was given to build to."),
            Layer: "A child starts from its ticket and its brief; what the ticket omits, the child never learns.",
            Reason: "A quoted section still says what was meant after the spec has moved on.")
    };

    public static readonly Claim Mode = new("A sub-ticket's Mode label is AFK, or HITL when the user must take part.", Me,
        [At(Actor.TicketCaptain, CaptainSlot.Split, 6, "Label its Mode: AFK, or HITL when the user must take part.")])
    {
        Spec = ["S6", "S12"],
        Truth = new(Evidence.Run("Linear, team Dydo, labels read 2026-11-20: the Mode group holds AFK, \"an agent can produce the work\", and HITL, \"active human participation is required\"."),
            Layer: "Linear holds the label; only the cutter knows whether a part needs the user.",
            Reason: "The label tells everyone who the part waits on.")
    };

    public static readonly Claim Steps = new("Work with its own review rounds is cut into Implement, Review, Fix and Merge sub-tickets blocked in order; a small leaf and a batched should are each one ticket.", Me,
    [
        At(Actor.TicketCaptain, CaptainSlot.Split, 7,
            "Work with its own review rounds gets a sub-ticket per step, blocked in this order: Implement, Review (one per round), Fix, Merge. You may open them ahead, in Todo. " +
            "A small leaf or a batched should is one ticket, with no steps. (open: his call)")
    ])
    {
        Spec = ["S3", "S4"],
        Truth = new(Evidence.Run(Children + " Linear has no Type label for Implement, Review or Fix, so the title carries the step. " +
                                 "No Shoulds batch has been cut in team Dydo's Linear yet, read 2026-11-21, so the batched should rests on spec S4 alone, under the same open point."),
            Layer: "Linear draws the blocking order once it is set; setting it is the cutter's.",
            Reason: "The board then shows which step a piece of work has reached without anyone asking its captain.")
    };

    public static readonly Claim Small = new("A captain that has split writes only a change cheaper to do than to brief and too small to need review.", Me,
        [At(Actor.TicketCaptain, CaptainSlot.Split, 8, "Once you split, write a change yourself only when it is cheaper than briefing it and too small to need review.")])
    {
        Spec = ["R16"],
        Truth = new(Evidence.Code("AGENTS.md"),
            Layer: "AGENTS.md says a splitting captain only orchestrates; this is the one exception, and no layer states it.",
            Reason: "A captain that wrote the work can no longer be impartial about the findings on it.")
    };

    private static Line At(Actor reader, Enum slot, int order, string text, Func<Claim[]>? also = null) =>
        new(CaptainSkill.TicketCaptain, reader, text, new(slot, order), also);
}
