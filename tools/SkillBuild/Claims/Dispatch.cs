namespace DynaDocs.SkillBuild;

/// <summary>Crew dispatch and return (M9): staffing a seat, the brief, starting the hop, a re-cut, the return, and the dispatch channel's rules.</summary>
public static class Dispatch
{
    public static readonly Claim Send = new("A staffed seat goes out briefed; any reviewer or judge comes from a family other than the author's when possible.",
        [Actor.TicketCaptain, Actor.Admiral, Actor.CodeWriter, Actor.Reviewer, Actor.Scout],
    [
        new(Skill.TicketCaptain, "Seat any reviewer from a family other than the author's when possible, single seats included.", new(DispatchState.Staffing, 801, MoveKind.Exit)),
        new(Skill.Admiral, "Seat any reviewer or judge from a family other than the author's when possible, single seats included.", new(DispatchState.Staffing, 801, MoveKind.Exit)),
        new(Skill.CodeWriter, "You arrive with a brief from the code-writer dispatch template; work only within it.", new(DispatchState.Briefed, 801, MoveKind.Arrival)),
        new(Skill.Reviewer, "You arrive with a brief from the reviewer dispatch template; judge only what it names.", new(DispatchState.Briefed, 801, MoveKind.Arrival)),
        new(Skill.Scout, "You arrive with one question and its return-to; you hold no ticket.", new(DispatchState.Briefed, 801, MoveKind.Arrival))
    ])
    { Spec = ["H3", "K14", "R15", "S1"], Transition = new(DispatchState.Staffing, DispatchState.Briefed, "one seat, cross-family where possible, sent from its kind's template") };

    public static readonly Claim Council = new("A council convenes only for stakes or value.", [Actor.TicketCaptain, Actor.Admiral],
    [
        new(Skill.TicketCaptain, "Convene a council only for a heavy PR, one that meets the stakes or value test: guard or security work, a base other tickets build on, or the late simplify review.", new(DispatchState.Staffing, 802, MoveKind.Exit)),
        new(Skill.Admiral, "Convene a council only for stakes (the last check before main, or a mistake expensive to undo) or value (a call between good options).", new(DispatchState.Staffing, 802, MoveKind.Exit))
    ])
    { Spec = ["L16", "L17"], Transition = new(DispatchState.Staffing, DispatchState.CouncilCalled, "stakes or value: a council") };

    public static readonly Claim SelfAuthor = new("A captain authors only a change cheaper to do than to brief and too small to need its own review.", [Actor.TicketCaptain],
        [new(Skill.TicketCaptain, "Author a change yourself only when it is cheaper to do than to brief and too small to need its own review; persistence, migrations, permissions, guard rules and uncertain native interfaces always go to crew, and a sub-captain authors nothing.", new(DispatchState.Staffing, 803, MoveKind.Exit))])
    { Spec = ["R16"], Transition = new(DispatchState.Staffing, DispatchState.SelfAuthored, "cheaper to do than to brief, too small to need its own review") };

    public static readonly Claim Start = new("Whoever starts a step sets its hop In Progress and attaches the holder's T3 thread link.",
        [Actor.CodeWriter, Actor.Reviewer, Actor.TicketCaptain],
    [
        new(Skill.CodeWriter, "Start your hop: set it In Progress and attach your T3 thread link titled `T3 · code-writer · <model>`.", new(DispatchState.Briefed, 804, MoveKind.Exit)),
        new(Skill.Reviewer, "Holding a Review hop alone, set it In Progress and attach your T3 thread link titled `T3 · reviewer · <model>`.", new(DispatchState.Briefed, 804, MoveKind.Exit)),
        new(Skill.TicketCaptain, "Set your ticket In Progress and attach your T3 thread link titled `T3 · ticket-captain · <model>`.", new(DispatchState.Briefed, 804, MoveKind.Exit)),
        new(Skill.LinearWorkspaceStandard, "Whoever starts a step sets it In Progress and attaches the holder's T3 thread link, titled `T3 · <role> · <model>`; neither the assignee nor Linear's `delegate` carries identity.", new(Section.LinearStatuses, 1))
    ])
    { Spec = ["S6", "S13"], Transition = new(DispatchState.Briefed, DispatchState.Working, "the holder starts its hop") };

    public static readonly Claim TooBig = new("Crew return too big, re-cut; the cutter decides whether a unit gets children; no agent promotes itself.",
        [Actor.CodeWriter, Actor.Reviewer, Actor.TicketCaptain, Actor.Admiral],
    [
        new(Skill.CodeWriter, "A job too big for one agent goes back as `too big, re-cut`.", new(DispatchState.Briefed, 805, MoveKind.Exit)),
        new(Skill.Reviewer, "A candidate too big for one seat goes back as `too big, re-cut`.", new(DispatchState.Briefed, 805, MoveKind.Exit)),
        new(Skill.TicketCaptain, "On `too big, re-cut` from your crew, you are the cutter: decide whether the unit gets children; a ticket too big for you goes back to your officer the same way, and you never promote yourself.", new(DispatchState.ReCut, 805, MoveKind.Arrival)),
        new(Skill.Admiral, "On `too big, re-cut`, you are the cutter: decide whether the unit gets children.", new(DispatchState.ReCut, 805, MoveKind.Arrival)),
        new(Skill.WorkingTreeStandard, "The cutter, the officer above, decides whether a unit gets children; no agent promotes itself, and crew return `too big, re-cut`.", new(Section.TreeDelegation, 7))
    ])
    { Spec = ["H2"], Transition = new(DispatchState.Briefed, DispatchState.ReCut, "too big for one agent") };

    public static readonly Claim ReCut = new("The cutter cuts only coherent units, never to pass work down.", [Actor.TicketCaptain, Actor.Admiral],
    [
        new(Skill.TicketCaptain, "Cut only coherent units too big for one agent or able to run in parallel, never to pass work down.", new(DispatchState.ReCut, 806, MoveKind.Exit)),
        new(Skill.Admiral, "Cut only coherent units too big for one agent or able to run in parallel, never to pass work down.", new(DispatchState.ReCut, 806, MoveKind.Exit))
    ])
    { Spec = ["H2", "H14"], Transition = new(DispatchState.ReCut, DispatchState.Staffing, "the cutter cuts coherent children") };

    public static readonly Claim Return = new("Crew return to their dispatcher, and whoever started a step closes it.",
        [Actor.CodeWriter, Actor.Reviewer, Actor.Scout, Actor.TicketCaptain],
    [
        new(Skill.CodeWriter, "Close your hop Done and return its commit SHA and receipts to the brief's return-to.", new(DispatchState.Working, 807, MoveKind.Exit)),
        new(Skill.Reviewer, "Return your verdict, or your seat's findings, to the brief's return-to.", new(DispatchState.Returned, 807, MoveKind.Arrival)),
        new(Skill.Scout, "Return the answer and its sources to the brief's return-to.", new(DispatchState.Working, 807, MoveKind.Exit)),
        new(Skill.TicketCaptain, "Every crew return comes back to you: post its hop's SHA on the hop as the resume point.", new(DispatchState.Returned, 807, MoveKind.Arrival)),
        new(Skill.LinearWorkspaceStandard, "Whoever started a step closes it; a parent's In Progress means its officer holds it.", new(Section.LinearStatuses, 2))
    ])
    { Spec = ["K14", "R5", "S6", "N12 DR 047 §7"], Transition = new(DispatchState.Working, DispatchState.Returned, "the crew returns to its dispatcher") };

    public static readonly Claim Template = new("An officer briefs crew from the crew kind's dispatch template and never reads a crew skill to brief it.",
        [Actor.Admiral, Actor.TicketCaptain],
    [
        new(Skill.Admiral, "Brief from the crew kind's dispatch template; the crew loads its own skill, so never read a crew skill to brief it.", new(Routine.Dispatching, 808)),
        new(Skill.TicketCaptain, "Brief from the crew kind's dispatch template; the crew loads its own skill, so never read a crew skill to brief it.", new(Routine.Dispatching, 808))
    ])
    { Spec = ["K14", "K29"] };

    public static readonly Claim Channel = new("A Linear unit goes out by delegate_task; a native sub-agent shares its holder's checkout and the holder commits.",
        [Actor.Admiral, Actor.TicketCaptain, Actor.CodeWriter],
    [
        new(Skill.Admiral, "A Linear unit goes out by `delegate_task`; a native sub-agent never gets one, shares your checkout, and you commit its work.", new(Routine.Dispatching, 809)),
        new(Skill.TicketCaptain, "A Linear unit goes out by `delegate_task`; a native sub-agent never gets one, shares your checkout, and you commit its work.", new(Routine.Dispatching, 809)),
        new(Skill.CodeWriter, "A native helper you start shares your checkout; you commit its work.", new(DispatchState.Working, 809, MoveKind.Nominal)),
        new(Skill.WorkingTreeStandard, "A Linear unit goes out by `delegate_task`. A native sub-agent never gets one: it shares its holder's checkout, and the holder commits.", new(Section.TreeDelegation, 1))
    ])
    { Spec = ["H3"] };

    public static readonly Claim Worktrees = new("Parallel lanes and sub-tickets with children get their own worktree; serial hops share their ticket's.",
        [Actor.TicketCaptain],
    [
        new(Skill.TicketCaptain, "Give parallel lanes, and sub-tickets with children, their own worktree; serial hops share their ticket's.", new(Routine.Dispatching, 810)),
        new(Skill.WorkingTreeStandard, "Each parallel lane, and each sub-ticket with children, works in its own worktree; serial hops reuse their ticket's.", new(Section.TreeDelegation, 2))
    ])
    { Spec = ["H3"] };

    public static readonly Claim Foreground = new("A native foreground sub-agent only for a result awaited within the turn; no native background sub-agents.",
        [Actor.Admiral, Actor.TicketCaptain, Actor.ChiefOfStaff],
    [
        new(Skill.Admiral, "Use a native foreground sub-agent only for a result you wait on within your turn, and stay in your turn until it arrives; start no native background sub-agent.", new(Routine.Dispatching, 811)),
        new(Skill.TicketCaptain, "Use a native foreground sub-agent only for a result you wait on within your turn, and stay in your turn until it arrives; start no native background sub-agent.", new(Routine.Dispatching, 811)),
        new(Skill.ChiefOfStaff, "Use a native foreground sub-agent only for a result you wait on within your turn, and stay in your turn until it arrives; start no native background sub-agent.", new(Routine.Dispatching, 811)),
        new(Skill.WorkingTreeStandard, "Under T3, a native foreground sub-agent only when its dispatcher waits for it within its own turn, and no native background sub-agents; a dispatcher waiting on a native child, or a child awaiting a result it owns, stays in its turn until the result arrives.", new(Section.TreeDelegation, 3))
    ])
    { Spec = ["N12 DR 057 §1", "N12 DR 057 §3", "H3"] };

    public static readonly Claim ParentNotActive = new("A native child cannot delegate_task; on parent_not_active the dispatch goes up.",
        [Actor.Admiral, Actor.TicketCaptain, Actor.ChiefOfStaff],
    [
        new(Skill.Admiral, "On a captain's `parent_not_active`, dispatch its crew for it.", new(Routine.Dispatching, 812)),
        new(Skill.TicketCaptain, "A native child can't `delegate_task`: on `parent_not_active`, ask your officer to dispatch it.", new(Routine.Dispatching, 812)),
        new(Skill.ChiefOfStaff, "On `parent_not_active`, ask him to dispatch it.", new(Routine.Dispatching, 812)),
        new(Skill.WorkingTreeStandard, "A native child lives only inside its dispatcher's turn and cannot call `delegate_task`: on `parent_not_active`, a captain asks its officer, the chief of staff asks the user, and the admiral dispatches a captain's crew for it.", new(Section.TreeDelegation, 4))
    ])
    { Spec = ["N12 DR 057 §2"] };

    public static readonly Claim CodexCap = new("A brief that sends out Codex agents names a concurrency cap.", [Actor.Admiral, Actor.TicketCaptain],
    [
        new(Skill.Admiral, "A brief that sends out Codex agents names a cap: about 8 at once, toward 16 only while runs stay clean, fewer on `429 turn_rate_limit_exceeded`, which means retry in a few minutes, never a blocker.", new(Routine.Dispatching, 813)),
        new(Skill.TicketCaptain, "A brief that sends out Codex agents names a cap: about 8 at once, toward 16 only while runs stay clean, fewer on `429 turn_rate_limit_exceeded`, which means retry in a few minutes, never a blocker.", new(Routine.Dispatching, 813)),
        new(Skill.WorkingTreeStandard, "A brief that sends out Codex agents names a cap: start around 8, go toward 16 only while runs stay clean, and back off on `429 turn_rate_limit_exceeded`; a capacity 429 means retry after a few minutes, never a blocker.", new(Section.TreeDelegation, 5))
    ])
    { Spec = ["N12 DR 057 §4"] };

    public static readonly Claim AskUp = new("Crew hand their questions up and never ask the user directly.", [Actor.CodeWriter, Actor.Reviewer, Actor.Scout],
    [
        new(Skill.CodeWriter, "Hand a question up to your captain, never to Balazs.", new(DispatchState.Working, 814, MoveKind.Nominal)),
        new(Skill.Reviewer, "Hand a question up to your captain, never to Balazs.", new(ReviewState.Round, 814, MoveKind.Nominal)),
        new(Skill.Scout, "Hand a question about your question up to your dispatcher, never to Balazs.", new(DispatchState.Working, 814, MoveKind.Nominal)),
        new(Skill.WorkingTreeStandard, "Crew take their questions to their captain, never to the user.", new(Section.TreeDelegation, 6))
    ])
    { Spec = ["N12 DR 057 §6"] };
}
