namespace DynaDocs.SkillBuild;

/// <summary>The model roster and seat staffing (R12–R14): his roster, which he writes (DYD-1082 71e30c21), dydo usage, and seats scaled to the work.</summary>
public static class Roster
{
    public static readonly Claim Models = new("One model roster, his, as one easy-to-edit table in every dispatching officer's spine.", [Actor.Coordinator, Actor.TicketCaptain],
    [
        new(Skill.Coordinator, "The model roster is his to write; his text goes here.", new(Routine.Dispatching, 2101)),
        new(Skill.TicketCaptain, "The model roster is his to write; his text goes here.", new(Routine.Dispatching, 2))
    ])
    { Spec = ["R13"] };

    // The coordinator states this in Coordination.Seats' combined line: R14's fixed rule.
    public static readonly Claim Usage = new("Every dispatching officer reads dydo usage and balances vendors for every seat.", [Actor.Coordinator, Actor.TicketCaptain],
    [
        new(Skill.TicketCaptain, "Read `dydo usage` and balance vendors for every seat you fill, sub-captains and crew as well as reviewers.", new(Routine.Dispatching, 3))
    ])
    { Spec = ["R14"] };

    public static readonly Claim Seats = new("Seats scale with the breadth and depth of the work.", [Actor.TicketCaptain],
    [
        new(Skill.TicketCaptain, "Scale each round's seats with the breadth and depth of the work; there is no constant pair.", new(Routine.Dispatching, 4)),
        new(Skill.WorkingTreeStandard, "Each review round's seats scale with the breadth and depth of the work, with no constant pair; a small leaf may need no review.", new(Section.TreeDelivery, 1))
    ])
    { Spec = ["R12"] };
}
