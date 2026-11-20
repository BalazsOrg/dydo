namespace DynaDocs.SkillBuild;

/// <summary>What an officer's brief carries (K14): one dispatch template per crew kind, the lenses, the governing spec, and the spec's scout pass.</summary>
public static class Brief
{
    public static readonly Claim ImplementerBrief = new("The implementer dispatch template's slots.", [Actor.TicketCaptain],
        [new(Skill.TicketCaptain, "Ticket and contract · branch, worktree and base · owned paths · gates · proof recipe · findings (Fix only) · model and effort · return to.", new(Section.ImplementerBrief, 3001))])
    { Spec = ["K14"] };

    public static readonly Claim ReviewerBrief = new("The reviewer dispatch template's slots.", [Actor.TicketCaptain],
    [
        new(Skill.TicketCaptain, "Ticket and contract · the governing spec, or none · lenses · branch, worktree and base (the candidate SHA) · owned paths · gates · proof recipe · model and effort · return to.", new(Section.ReviewerBrief, 3002))
    ])
    { Spec = ["K14", "R3", "R6"] };

    public static readonly Claim ScoutBrief = new("The scout dispatch template's slots.", [Actor.TicketCaptain],
    [
        new(Skill.TicketCaptain, "The question, or the spec for a scout pass · model and effort · return to.", new(Section.ScoutBrief, 3003))
    ])
    { Spec = ["K14", "R4"] };

    public static readonly Claim CaptainBrief = new("The ticket-captain dispatch template's slots; a sub-captain is briefed from it too.", [Actor.TicketCaptain, Actor.Coordinator],
    [
        new(Skill.TicketCaptain, "Ticket and contract · branch, worktree and base · owned paths · gates · proof recipe · model and effort · return to. A sub-captain is briefed from this template too.", new(Section.TicketCaptainBrief, 3004)),
        new(Skill.Coordinator, "Ticket and contract · branch, worktree and base · owned paths · gates · proof recipe · model and effort · return to. A sub-captain is briefed from this template too.", new(Section.TicketCaptainBrief, 3004))
    ])
    { Spec = ["K14", "H23"] };

    public static readonly Claim Lenses = new("The officer names the lenses in the brief; lenses shift up the tree.", [Actor.TicketCaptain],
    [
        new(Skill.TicketCaptain, "Name the lenses in each brief, shifting up the tree: it works (with a security baseline), then it integrates, then optimal, secure, performant and in harmony; never review a first version as if it were final.", new(Routine.Dispatching, 3005))
    ])
    { Spec = ["R6"] };

    public static readonly Claim SpecInBrief = new("A reviewer's brief names the governing spec or says there is none.", [Actor.TicketCaptain],
    [
        new(Skill.TicketCaptain, "Name the governing spec in each reviewer's brief, or say there is none.", new(Routine.Dispatching, 3006))
    ])
    { Spec = ["R3"] };

    public static readonly Claim ScoutPass = new("No review loop on a plan, spec or contract; a spec gets at most one scout pass.", [Actor.TicketCaptain],
    [
        new(Skill.TicketCaptain, "A plan, spec or contract gets no review loop; give a spec at most one scout pass, chosen by risk, which returns Questions and is never repeated.", new(Routine.Dispatching, 3007))
    ])
    { Spec = ["R4"] };
}
