namespace DynaDocs.SkillBuild;

/// <summary>Each role's job paragraph (K9), what it is accountable for, and the tools and surfaces it touches.</summary>
public static class Role
{
    public static readonly Claim Coordinator = new("The coordinator is the live officer that keeps one open Project moving, mechanically, to its landing (DYD-1082 555a8300 Q21).", [Actor.Coordinator],
        [new(Skill.Coordinator, "You are a coordinator: you keep one open Project moving, mechanically, from the day it is up to its landing.", new(SpinePart.Job, 101))])
    { Spec = ["H4", "H5", "H1"] };

    public static readonly Claim ChiefOfStaff = new("The chief of staff keeps his short list across Projects by day and holds the night, from his muster to the morning brief (DYD-1082 a1b21b59 Q12).", [Actor.ChiefOfStaff],
        [new(Skill.ChiefOfStaff, "You are the chief of staff: by day you keep his one short list across Projects and are otherwise mostly idle, and the night is yours, from his muster to the morning brief.", new(SpinePart.Job, 102))])
    { Spec = ["H7", "K11"] };

    public static readonly Claim TicketCaptain = new("A ticket captain holds one ticket to its end; a sub-captain is one a level down.", [Actor.TicketCaptain],
        [new(Skill.TicketCaptain, "You are a ticket captain: you hold one ticket to its end, and a sub-captain is a ticket captain one level down.", new(SpinePart.Job, 103))])
    { Spec = ["H1", "H13", "H23"] };

    public static readonly Claim Implementer = new("The implementer is the one crew kind that creates, writes and modifies files, mostly code (DYD-1082 a1b21b59 Q10, 1b6b6a5d Q14).", [Actor.Implementer],
        [new(Skill.Implementer, "You are an implementer: the one crew kind that creates, writes and modifies files, mostly code, sometimes docs or anything else a ticket needs.", new(SpinePart.Job, 104))])
    { Spec = ["K31", "K30", "K22"] };

    public static readonly Claim Reviewer = new("The reviewer is one fresh seat that judges one candidate at the level its brief names (DYD-1082 a1b21b59 Q6/Q9).", [Actor.Reviewer],
        [new(Skill.Reviewer, "You are a reviewer: one fresh seat that judges one candidate at the level your brief names and returns one verdict.", new(SpinePart.Job, 105))])
    { Spec = ["R34", "K30", "R5", "N5"] };

    public static readonly Claim Scout = new("The scout is crew for one question and one lookup, off the record: research's helper and the spec's Questions pass (DYD-1082 99774812).", [Actor.Scout],
        [new(Skill.Scout, "You are a scout: crew for one question and one lookup, off the record, sent by research for one source family or by an officer for a spec's Questions pass.", new(SpinePart.Job, 106))])
    { Spec = ["K30", "S1", "R4"] };

    public static readonly Claim Research = new("Research is the crew kind that pools and verifies (DYD-1082 99774812).", [Actor.Research],
        [new(Skill.Research, "You are research: crew that answers one question a choice waits on, pooling evidence from every source that could own the answer and verifying each claim at its source.", new(SpinePart.Job, 121))])
    { Spec = ["K30"] };

    public static readonly Claim Chain = new("The officer-above chain is sub-captain, captain, coordinator, chief of staff.",
        [Actor.Coordinator, Actor.TicketCaptain, Actor.ChiefOfStaff],
    [
        new(Skill.Coordinator, "You are the officer above your captains; the chief of staff is yours.", new(SpinePart.Job, 108)),
        new(Skill.TicketCaptain, "Your officer above is whoever dispatched you; you are the officer above your crew and sub-captains.", new(SpinePart.Job, 107)),
        new(Skill.ChiefOfStaff, "You are the top of the officer-above chain, above the coordinators.", new(SpinePart.Job, 111))
    ])
    { Spec = ["H12"] };

    // The sub-captain's half of R16 is in the ticket-captain's Dispatch.SelfAuthor line (K4).
    public static readonly Claim NoAuthor = new("Coordinators never author.", [Actor.Coordinator],
        [new(Skill.Coordinator, "You never author: captains and their crew do.", new(SpinePart.Job, 109))])
    { Spec = ["R16"] };

    public static readonly Claim CoordinatorAccounts = new("The coordinator answers for the Project's outcome.", [Actor.Coordinator],
        [new(Skill.Coordinator, "You answer for its outcome: its lanes, its HITL queue, its merges and its landing.", new(SpinePart.Job, 102))])
    { Spec = ["H4", "H18", "R25", "N6"] };

    public static readonly Claim StaffAccounts = new("The chief of staff answers for his short list and the night watch.", [Actor.ChiefOfStaff],
        [new(Skill.ChiefOfStaff, "You answer for that short list and for the night watch.", new(SpinePart.Job, 110))])
    { Spec = ["H7"] };

    public static readonly Claim CaptainAccounts = new("The ticket captain answers for its ticket's outcome.", [Actor.TicketCaptain],
        [new(Skill.TicketCaptain, "You answer for the ticket's outcome, its contract, hops, review rounds, merge and record, while crew do the work: you cut it, staff fresh seats, rule on what they return and merge.", new(SpinePart.Job, 111))])
    { Spec = ["S6", "S15", "R8", "R21"] };

    public static readonly Claim ImplementerAccounts = new("The implementer answers for its hop's commit and receipts.", [Actor.Implementer],
        [new(Skill.Implementer, "You answer for the hop's commit and the receipts that prove it.", new(SpinePart.Job, 112))])
    { Spec = ["R5", "R32"] };

    public static readonly Claim ReviewerAccounts = new("The reviewer answers for one verdict with every finding's severity.", [Actor.Reviewer],
        [new(Skill.Reviewer, "You answer for the verdict, every finding in it with its severity.", new(SpinePart.Job, 113))])
    { Spec = ["R11", "R17"] };

    public static readonly Claim ScoutAccounts = new("The scout answers for what it found and where it lives; its dispatcher concludes.", [Actor.Scout],
        [new(Skill.Scout, "You answer for what you found and where each part of it lives; pooling and concluding are your dispatcher's.", new(SpinePart.Job, 114))])
    { Spec = ["K22"] };

    public static readonly Claim CoordinatorTools = new("The coordinator's tools and surfaces.", [Actor.Coordinator],
        [new(Skill.Coordinator, "Your tools: Linear, `dydo survey board <project>` with its liveness findings, `dydo usage`, `delegate_task`, and T3 `schedule_task`.", new(CoordinatorSlot.WakeUp, 1302))])
    { Spec = ["H5", "S22", "S24", "R14", "H3", "N12 DR 047"] };

    public static readonly Claim StaffTools = new("The chief of staff's tools and surfaces.", [Actor.ChiefOfStaff],
        [new(Skill.ChiefOfStaff, "Your tools: `dydo survey board` across Projects, Linear, and T3 thread links.", new(ChiefOfStaffSlot.Tools, 116))])
    { Spec = ["H7", "S22"] };

    public static readonly Claim CaptainTools = new("The ticket captain's tools and surfaces.", [Actor.TicketCaptain],
        [new(Skill.TicketCaptain, "Your tools: Linear, `delegate_task`, `dydo usage`, `dydo test gate`, `dydo merge`, `dydo survey board`, `dydo archive`, the PR and its CI.", new(TicketCaptainSlot.Take, 117))])
    { Spec = ["H3", "R14", "R21", "R31", "N10", "S22"] };

    public static readonly Claim ImplementerTools = new("The implementer's tools and surfaces.", [Actor.Implementer],
        [new(Skill.Implementer, "You work in the brief's branch and worktree, inside its owned paths, with git, `dydo test`, `dydo code-check`, `dydo check` and the project's verification skills.", new(SpinePart.Job, 118))])
    { Spec = ["K14", "Y6", "T21", "D5"] };

    public static readonly Claim ReviewerTools = new("The reviewer's tools and surfaces.", [Actor.Reviewer],
        [new(Skill.Reviewer, "You work from the candidate at its SHA, the governing spec, DYTEST receipts, the project's verification skills, and, holding a round alone, its Review hop and the PR body's `## Independent review`.", new(SpinePart.Job, 119))])
    { Spec = ["R3", "R5", "R17", "S8"] };

    public static readonly Claim ScoutTools = new("The scout's tools and surfaces.", [Actor.Scout],
        [new(Skill.Scout, "Your source is the one family your brief names: docs, DRs and the spec, code and tests, git history, Linear, or outside sources.", new(SpinePart.Job, 120))])
    { Spec = ["K30", "S1"] };
}
