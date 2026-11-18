namespace DynaDocs.SkillBuild;

/// <summary>The spine headers (K9): each role, what it is accountable for, and the tools and surfaces it touches.</summary>
public static class Role
{
    public static readonly Claim Admiral = new("The admiral is the live officer that holds one open Project's outcome.", [Actor.Admiral],
        [new(Skill.Admiral, "You are an admiral: the live officer holding one open Project's outcome, from charting to landing.", new(HeaderPart.Role, 101))])
    { Spec = ["H4", "H1"] };

    public static readonly Claim ChiefOfStaff = new("The chief of staff keeps his short list across Projects and the overnight watch.", [Actor.ChiefOfStaff],
        [new(Skill.ChiefOfStaff, "You are the chief of staff: his one short list across Projects by day, the overnight watch by night, and mostly idle by day.", new(HeaderPart.Role, 102))])
    { Spec = ["H7"] };

    public static readonly Claim TicketCaptain = new("A ticket captain holds one ticket to its end; a sub-captain is one a level down.", [Actor.TicketCaptain],
        [new(Skill.TicketCaptain, "You are a ticket captain: you hold one ticket to its end; a sub-captain is a ticket captain one level down.", new(HeaderPart.Role, 103))])
    { Spec = ["H1", "H13", "H23"] };

    public static readonly Claim CodeWriter = new("The code writer is crew for one writer hop, on code or docs.", [Actor.CodeWriter],
        [new(Skill.CodeWriter, "You are a code writer: crew for one writer hop on code or docs: implement, fix, or resolve a merge conflict.", new(HeaderPart.Role, 104))])
    { Spec = ["R32", "R21", "K22"] };

    public static readonly Claim Reviewer = new("The reviewer is one fresh seat judging through named lenses, also in inquisitions and councils.", [Actor.Reviewer],
        [new(Skill.Reviewer, "You are a reviewer: one fresh seat judging one candidate through the lenses your brief names, or a seat in an inquisition round or a [council](resources/council.md).", new(HeaderPart.Role, 105))])
    { Spec = ["R5", "R6", "N5", "L23", "K22"] };

    public static readonly Claim Scout = new("The scout is crew for one question, off the record.", [Actor.Scout],
        [new(Skill.Scout, "You are a scout: crew for one question, off the record.", new(HeaderPart.Role, 106))])
    { Spec = ["S1", "R4", "K22"] };

    public static readonly Claim Chain = new("The officer-above chain is sub-captain, captain, admiral, chief of staff.",
        [Actor.Admiral, Actor.TicketCaptain, Actor.ChiefOfStaff],
    [
        new(Skill.Admiral, "You are the officer above your captains; the chief of staff is yours.", new(HeaderPart.Role, 107)),
        new(Skill.TicketCaptain, "Your officer above is whoever dispatched you; you are the officer above your crew and sub-captains.", new(HeaderPart.Role, 107)),
        new(Skill.ChiefOfStaff, "You are the top of the officer-above chain, above the admirals.", new(HeaderPart.Role, 107))
    ])
    { Spec = ["H12"] };

    public static readonly Claim NoAuthor = new("Admirals and sub-captains never author.", [Actor.Admiral, Actor.TicketCaptain],
    [
        new(Skill.Admiral, "You never author: captains and their crew do.", new(HeaderPart.Role, 108)),
        new(Skill.TicketCaptain, "As a sub-captain, you author nothing.", new(HeaderPart.Role, 108))
    ])
    { Spec = ["R16"] };

    public static readonly Claim AdmiralAccounts = new("The admiral answers for the Project's outcome.", [Actor.Admiral],
        [new(Skill.Admiral, "The Project's outcome: its spec, its map, its feature branch, its envoys and its landing.", new(HeaderPart.Accountable, 109))])
    { Spec = ["H4", "S14", "R25", "H16", "N6"] };

    public static readonly Claim StaffAccounts = new("The chief of staff answers for his short list and the overnight watch.", [Actor.ChiefOfStaff],
        [new(Skill.ChiefOfStaff, "His short list, and the overnight watch.", new(HeaderPart.Accountable, 110))])
    { Spec = ["H7"] };

    public static readonly Claim CaptainAccounts = new("The ticket captain answers for its ticket's outcome.", [Actor.TicketCaptain],
        [new(Skill.TicketCaptain, "The ticket's outcome: its contract, hops, review rounds, merge and record.", new(HeaderPart.Accountable, 111))])
    { Spec = ["S6", "S15", "R8", "R21"] };

    public static readonly Claim WriterAccounts = new("The code writer answers for its hop's commit and receipts.", [Actor.CodeWriter],
        [new(Skill.CodeWriter, "The hop's commit and the receipts that prove it.", new(HeaderPart.Accountable, 112))])
    { Spec = ["R5", "R32"] };

    public static readonly Claim ReviewerAccounts = new("The reviewer answers for one verdict with every finding's severity.", [Actor.Reviewer],
        [new(Skill.Reviewer, "One verdict, every finding with its severity.", new(HeaderPart.Accountable, 113))])
    { Spec = ["R11", "R17"] };

    public static readonly Claim ScoutAccounts = new("The scout answers for the answer and where it lives.", [Actor.Scout],
        [new(Skill.Scout, "The answer, and where each part of it lives.", new(HeaderPart.Accountable, 114))])
    { Spec = ["K22"] };

    public static readonly Claim AdmiralTools = new("The admiral's tools and surfaces.", [Actor.Admiral],
        [new(Skill.Admiral, "`dydo survey board <project>`, the board and its liveness findings, Linear, `dydo usage`, `delegate_task`, T3 `schedule_task`, `dydo.json`.", new(HeaderPart.ToolsAndSurfaces, 115))])
    { Spec = ["H5", "S22", "S24", "R14", "H3", "R27", "N12 DR 047"] };

    public static readonly Claim StaffTools = new("The chief of staff's tools and surfaces.", [Actor.ChiefOfStaff],
        [new(Skill.ChiefOfStaff, "`dydo survey board` across Projects, Linear, T3 thread links.", new(HeaderPart.ToolsAndSurfaces, 116))])
    { Spec = ["H7", "S22"] };

    public static readonly Claim CaptainTools = new("The ticket captain's tools and surfaces.", [Actor.TicketCaptain],
        [new(Skill.TicketCaptain, "Linear, `delegate_task`, `dydo usage`, `dydo test gate`, `dydo merge`, `dydo survey board`, `dydo archive`, the PR and its CI.", new(HeaderPart.ToolsAndSurfaces, 117))])
    { Spec = ["H3", "R14", "R21", "R31", "N10", "S22"] };

    public static readonly Claim WriterTools = new("The code writer's tools and surfaces.", [Actor.CodeWriter],
        [new(Skill.CodeWriter, "The brief's branch and worktree, its owned paths, git, `dydo test`, `dydo code-check`, `dydo check`.", new(HeaderPart.ToolsAndSurfaces, 118))])
    { Spec = ["K14", "Y6", "T21", "D5"] };

    public static readonly Claim ReviewerTools = new("The reviewer's tools and surfaces.", [Actor.Reviewer],
        [new(Skill.Reviewer, "The candidate at its SHA, the governing spec, DYTEST receipts, and, holding a round alone, its Review hop and the PR body's `## Independent review`.", new(HeaderPart.ToolsAndSurfaces, 119))])
    { Spec = ["R3", "R5", "R17", "S8"] };

    public static readonly Claim ScoutTools = new("The scout's tools and surfaces.", [Actor.Scout],
        [new(Skill.Scout, "Docs, code, Linear, DRs and the spec.", new(HeaderPart.ToolsAndSurfaces, 120))])
    { Spec = ["H16"] };
}
