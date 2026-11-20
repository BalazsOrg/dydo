namespace DynaDocs.SkillBuild;

/// <summary>Each role's job paragraph (K9), what it is accountable for, and the tools and surfaces it touches.</summary>
public static class Role
{
    public static readonly Claim Coordinator = new("The coordinator is the live officer that holds one open Project's outcome.", [Actor.Coordinator],
        [new(Skill.Coordinator, "You are a coordinator: the live officer holding one open Project's outcome, from charting to landing.", new(SpinePart.Job, 101))])
    { Spec = ["H4", "H1"] };

    public static readonly Claim ChiefOfStaff = new("The chief of staff keeps his short list across Projects and the overnight watch.", [Actor.ChiefOfStaff],
        [new(Skill.ChiefOfStaff, "You are the chief of staff: his one short list across Projects by day, the overnight watch by night, and mostly idle by day.", new(SpinePart.Job, 102))])
    { Spec = ["H7"] };

    public static readonly Claim TicketCaptain = new("A ticket captain holds one ticket to its end; a sub-captain is one a level down.", [Actor.TicketCaptain],
        [new(Skill.TicketCaptain, "You are a ticket captain: you hold one ticket to its end; a sub-captain is a ticket captain one level down.", new(SpinePart.Job, 103))])
    { Spec = ["H1", "H13", "H23"] };

    public static readonly Claim Implementer = new("The implementer is crew for one writer hop, on code or docs.", [Actor.Implementer],
        [new(Skill.Implementer, "You are an implementer: crew for one writer hop on code or docs: implement, fix, or resolve a merge conflict.", new(SpinePart.Job, 104))])
    { Spec = ["R32", "R21", "K22"] };

    public static readonly Claim Reviewer = new("The reviewer is one fresh seat judging through named lenses, also in inquisitions and councils.", [Actor.Reviewer],
        [new(Skill.Reviewer, "You are a reviewer: one fresh seat judging one candidate through the lenses your brief names, or a seat in an inquisition round or a [council](resources/council.md).", new(SpinePart.Job, 105))])
    { Spec = ["R5", "R6", "N5", "L23", "K22"] };

    public static readonly Claim Scout = new("The scout is crew for one question, off the record.", [Actor.Scout],
        [new(Skill.Scout, "You are a scout: crew for one question, off the record.", new(SpinePart.Job, 106))])
    { Spec = ["S1", "R4", "K22"] };

    public static readonly Claim Research = new("Research is the crew kind that pools and verifies.", [Actor.Research],
        [new(Skill.Research, "You are research: crew that pools and verifies.", new(SpinePart.Job, 121))])
    { Spec = ["K22"] };

    public static readonly Claim Chain = new("The officer-above chain is sub-captain, captain, coordinator, chief of staff.",
        [Actor.Coordinator, Actor.TicketCaptain, Actor.ChiefOfStaff],
    [
        new(Skill.Coordinator, "You are the officer above your captains; the chief of staff is yours.", new(SpinePart.Job, 107)),
        new(Skill.TicketCaptain, "Your officer above is whoever dispatched you; you are the officer above your crew and sub-captains.", new(SpinePart.Job, 107)),
        new(Skill.ChiefOfStaff, "You are the top of the officer-above chain, above the coordinators.", new(SpinePart.Job, 107))
    ])
    { Spec = ["H12"] };

    // The sub-captain's half of R16 is in the ticket-captain's Dispatch.SelfAuthor line (K4).
    public static readonly Claim NoAuthor = new("Coordinators never author.", [Actor.Coordinator],
        [new(Skill.Coordinator, "You never author: captains and their crew do.", new(SpinePart.Job, 108))])
    { Spec = ["R16"] };

    public static readonly Claim CoordinatorAccounts = new("The coordinator answers for the Project's outcome.", [Actor.Coordinator],
        [new(Skill.Coordinator, "The Project's outcome: its spec, its map, its feature branch, its envoys and its landing.", new(Routine.Accountable, 109))])
    { Spec = ["H4", "S14", "R25", "H16", "N6"] };

    public static readonly Claim StaffAccounts = new("The chief of staff answers for his short list and the overnight watch.", [Actor.ChiefOfStaff],
        [new(Skill.ChiefOfStaff, "His short list, and the overnight watch.", new(Routine.Accountable, 110))])
    { Spec = ["H7"] };

    public static readonly Claim CaptainAccounts = new("The ticket captain answers for its ticket's outcome.", [Actor.TicketCaptain],
        [new(Skill.TicketCaptain, "The ticket's outcome: its contract, hops, review rounds, merge and record.", new(Routine.Accountable, 111))])
    { Spec = ["S6", "S15", "R8", "R21"] };

    public static readonly Claim ImplementerAccounts = new("The implementer answers for its hop's commit and receipts.", [Actor.Implementer],
        [new(Skill.Implementer, "The hop's commit and the receipts that prove it.", new(Routine.Accountable, 112))])
    { Spec = ["R5", "R32"] };

    public static readonly Claim ReviewerAccounts = new("The reviewer answers for one verdict with every finding's severity.", [Actor.Reviewer],
        [new(Skill.Reviewer, "One verdict, every finding with its severity.", new(Routine.Accountable, 113))])
    { Spec = ["R11", "R17"] };

    public static readonly Claim ScoutAccounts = new("The scout answers for the answer and where it lives.", [Actor.Scout],
        [new(Skill.Scout, "The answer, and where each part of it lives.", new(Routine.Accountable, 114))])
    { Spec = ["K22"] };

    public static readonly Claim CoordinatorTools = new("The coordinator's tools and surfaces.", [Actor.Coordinator],
        [new(Skill.Coordinator, "`dydo survey board <project>`, the board and its liveness findings, Linear, `dydo usage`, `delegate_task`, T3 `schedule_task`, `dydo.json`.", new(Routine.ToolsAndSurfaces, 115))])
    { Spec = ["H5", "S22", "S24", "R14", "H3", "R27", "N12 DR 047"] };

    public static readonly Claim StaffTools = new("The chief of staff's tools and surfaces.", [Actor.ChiefOfStaff],
        [new(Skill.ChiefOfStaff, "`dydo survey board` across Projects, Linear, T3 thread links.", new(Routine.ToolsAndSurfaces, 116))])
    { Spec = ["H7", "S22"] };

    public static readonly Claim CaptainTools = new("The ticket captain's tools and surfaces.", [Actor.TicketCaptain],
        [new(Skill.TicketCaptain, "Linear, `delegate_task`, `dydo usage`, `dydo test gate`, `dydo merge`, `dydo survey board`, `dydo archive`, the PR and its CI.", new(Routine.ToolsAndSurfaces, 117))])
    { Spec = ["H3", "R14", "R21", "R31", "N10", "S22"] };

    public static readonly Claim ImplementerTools = new("The implementer's tools and surfaces.", [Actor.Implementer],
        [new(Skill.Implementer, "The brief's branch and worktree, its owned paths, git, `dydo test`, `dydo code-check`, `dydo check`.", new(Routine.ToolsAndSurfaces, 118))])
    { Spec = ["K14", "Y6", "T21", "D5"] };

    public static readonly Claim ReviewerTools = new("The reviewer's tools and surfaces.", [Actor.Reviewer],
        [new(Skill.Reviewer, "The candidate at its SHA, the governing spec, DYTEST receipts, and, holding a round alone, its Review hop and the PR body's `## Independent review`.", new(Routine.ToolsAndSurfaces, 119))])
    { Spec = ["R3", "R5", "R17", "S8"] };

    public static readonly Claim ScoutTools = new("The scout's tools and surfaces.", [Actor.Scout],
        [new(Skill.Scout, "Docs, code, Linear, DRs and the spec.", new(Routine.ToolsAndSurfaces, 120))])
    { Spec = ["H16"] };
}
