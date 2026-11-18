namespace DynaDocs.SkillBuild;

/// <summary>A should (M13): registered under the ticket where it was found, collected at set moments, closed by the landing.</summary>
public static class Should
{
    public static readonly Claim Register = new("A should is registered as a child of the ticket where found, after a duplicate check.", [Actor.Reviewer, Actor.TicketCaptain],
    [
        new(Skill.Reviewer, "Holding the round alone, check the open shoulds under the ticket and its ancestors, then register each new should as a child of the ticket where you found it.", new(ShouldState.Found, 1201, MoveKind.Exit)),
        new(Skill.TicketCaptain, "Holding the round, check the open shoulds under the ticket and its ancestors, then register each new should as a child of the ticket where it was found.", new(ShouldState.Found, 1201, MoveKind.Exit)),
        new(Skill.LinearWorkspaceStandard, "A should is registered as a child of the ticket where it was found, after the round holder checks the open shoulds under that ticket and its ancestors for a duplicate.", new(Section.LinearShoulds, 1))
    ])
    { Spec = ["S17"], Transition = new(ShouldState.Found, ShouldState.Open, "no duplicate open: registered under the ticket where found") };

    public static readonly Claim AlreadyOpen = new("A should already open under the ticket or an ancestor is not registered again.", [Actor.Reviewer], [])
    { Spec = ["S17"], Transition = new(ShouldState.Found, ShouldState.Known, "a duplicate is already open") };

    public static readonly Claim Fixed = new("A should is fixed in place when cheap, else at a set moment.", [Actor.TicketCaptain],
        [new(Skill.TicketCaptain, "Fix a cheap should in place before its parent merges; the rest wait for a shoulds batch.", new(ShouldState.Open, 1203, MoveKind.Exit))])
    { Spec = ["S17"], Transition = new(ShouldState.Open, ShouldState.Done, "fixed in place, when a lane is free, before a round, or at the landing") };

    public static readonly Claim Dup = new("The batch captain marks duplicates Duplicate.", [Actor.TicketCaptain],
    [
        new(Skill.TicketCaptain, "In a shoulds batch, mark each duplicate Duplicate.", new(ShouldState.Open, 1204, MoveKind.Exit)),
        new(Skill.LinearWorkspaceStandard, "The batch captain marks a duplicate should Duplicate.", new(Section.LinearShoulds, 5))
    ])
    { Spec = ["S17"], Transition = new(ShouldState.Open, ShouldState.Duplicate, "the batch captain finds it a duplicate") };

    public static readonly Claim Cancel = new("At the landing a should is canceled only with a reason.", [Actor.Admiral],
        [new(Skill.Admiral, "At the landing, cancel a should only with a reason.", new(ShouldState.Open, 1205, MoveKind.Exit))])
    { Spec = ["S18"], Transition = new(ShouldState.Open, ShouldState.Canceled, "at the landing: canceled with a reason") };

    public static readonly Claim Move = new("At the landing a should moves out only by his call.", [Actor.Balazs, Actor.Admiral],
        [new(Skill.Admiral, "At the landing, move a should out of the Project only on his call.", new(ShouldState.Open, 1206, MoveKind.Exit))])
    { Spec = ["S18"], Transition = new(ShouldState.Open, ShouldState.Moved, "at the landing: moved out by his call") };

    public static readonly Claim Batch = new("The admiral cuts a Shoulds batch N root ticket for one captain at the set moments.", [Actor.Admiral],
    [
        new(Skill.Admiral, "Collect shoulds into a `Shoulds batch N` root ticket (Type `Feature` by default) cut for one captain when a lane is free, before an inquisition round, or at the landing.", new(ShouldState.Open, 1207, MoveKind.Nominal)),
        new(Skill.LinearWorkspaceStandard, "Shoulds are collected into a `Shoulds batch N` root ticket the admiral cuts for one captain (Type `Feature` by default); there is no standing collection ticket.", new(Section.LinearShoulds, 3))
    ])
    { Spec = ["S17"] };

    public static readonly Claim Reparent = new("The batch captain re-parents the shoulds, keeping a link to where each was found.", [Actor.TicketCaptain],
    [
        new(Skill.TicketCaptain, "Holding a Shoulds batch, re-parent each should into it, keeping a `related: found in <KEY> rN` link on it.", new(ShouldState.Open, 1208, MoveKind.Nominal)),
        new(Skill.LinearWorkspaceStandard, "The batch captain re-parents the shoulds into it and keeps a `related: found in <KEY> rN` link on each.", new(Section.LinearShoulds, 4))
    ])
    { Spec = ["S17"] };

    public static readonly Claim ParentDone = new("A should's parent may close Done while it stays open.", [Actor.TicketCaptain],
    [
        new(Skill.TicketCaptain, "A parent may close Done while its shoulds stay open.", new(ShouldState.Open, 1209, MoveKind.Nominal)),
        new(Skill.LinearWorkspaceStandard, "Its parent may close Done while it stays open.", new(Section.LinearShoulds, 2))
    ])
    { Spec = ["S17"] };
}
