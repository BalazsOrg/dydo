namespace DynaDocs.SkillBuild;

/// <summary>The model roster and seat staffing (R12–R14): his one table, dydo usage, and seats scaled to the work.</summary>
public static class Roster
{
    private const string Table = """
        Pick each seat's model from this roster; it is his to edit, and he fills or approves the strengths, fits and not-for columns.

        | Model | Vendor | Tier (B7) | Strengths | Fits | Not for |
        |---|---|---|---|---|---|
        | Sonnet | Anthropic | check: evidence and behaviour checks | | | |
        | Luna | OpenAI | check: evidence and behaviour checks | | | |
        | Opus 5.5 | Anthropic | normal: ordinary reviews at lower and middle levels | | | |
        | GPT 6.2 Sol | OpenAI | normal: ordinary reviews at lower and middle levels | | | |
        | Fable | Anthropic | strong: non-obvious discovery, and the late whole-system "simplify" review | | | |
        | Astra | OpenAI | strong: non-obvious discovery, and the late whole-system "simplify" review | | | |
        """;

    public static readonly Claim Models = new("One model roster, his, as one easy-to-edit table in every dispatching officer's spine.", [Actor.Coordinator, Actor.TicketCaptain],
    [
        new(Skill.Coordinator, Table, new(Routine.Dispatching, 2101)),
        new(Skill.TicketCaptain, Table, new(Routine.Dispatching, 2101))
    ])
    { Spec = ["R13"] };

    public static readonly Claim Usage = new("Every dispatching officer reads dydo usage and balances vendors for every seat.", [Actor.Coordinator, Actor.TicketCaptain],
    [
        new(Skill.Coordinator, "Read `dydo usage` and balance vendors for every seat you fill, captains and crew as well as reviewers.", new(Routine.Dispatching, 2102)),
        new(Skill.TicketCaptain, "Read `dydo usage` and balance vendors for every seat you fill, sub-captains and crew as well as reviewers.", new(Routine.Dispatching, 2102))
    ])
    { Spec = ["R14"] };

    public static readonly Claim Seats = new("Seats scale with the breadth and depth of the work.", [Actor.Coordinator, Actor.TicketCaptain],
    [
        new(Skill.Coordinator, "Scale each round's seats with the breadth and depth of the work; there is no constant pair.", new(Routine.Dispatching, 2103)),
        new(Skill.TicketCaptain, "Scale each round's seats with the breadth and depth of the work; there is no constant pair.", new(Routine.Dispatching, 2103)),
        new(Skill.WorkingTreeStandard, "Each review round's seats scale with the breadth and depth of the work, with no constant pair; a small leaf may need no review.", new(Section.TreeDelivery, 1))
    ])
    { Spec = ["R12"] };
}
