namespace DynaDocs.SkillBuild;

/// <summary>The review loop (M5): one review of the wired parent, verdicts, routing by kind, the round-3 checkpoint, and the rules a seat judges by.</summary>
public static class Review
{
    private const string Kinds = "kind' \\in {\"none\", \"broken\", \"security\", \"fundamental\", \"balazs\"}";

    public static readonly Claim Open = new("The parent is reviewed once, after wiring and before its Merge hop, against the governing spec.",
        [Actor.TicketCaptain, Actor.Reviewer],
    [
        new(Skill.TicketCaptain, "Once the parts are wired and before the Merge hop, review the parent once: each part, the parts together, and their harmony.", new(ReviewState.Wired, 501, MoveKind.Exit)),
        new(Skill.Reviewer, "Judge the change, and the behaviour it inherits unchanged, against the governing spec's problem statement and goals, not only the contract.", new(ReviewState.Round, 501, MoveKind.Arrival))
    ])
    { Spec = ["R2", "R3", "R6"], Transition = new(ReviewState.Wired, ReviewState.Round, "the parts are wired; Review r1 opens") { Effect = "round' = 1" } };

    public static readonly Claim Skip = new("A parent that only groups, or a small leaf, may get no review.", [Actor.TicketCaptain],
        [new(Skill.TicketCaptain, "Skip the parent's review when it only groups parts.", new(ReviewState.Wired, 502, MoveKind.Exit))])
    { Spec = ["R6"], Transition = new(ReviewState.Wired, ReviewState.Ended, "the parent only groups its parts") };

    public static readonly Claim Post = new("The round's holder posts the verdict on the Review hop and the PR line together.", [Actor.Reviewer, Actor.TicketCaptain],
    [
        new(Skill.Reviewer, "Holding the round alone, post its verdict on your Review hop (PASS or FAIL, lenses, seats, shoulds) and, at the same moment, the one-line PASS naming the Review ticket in the PR body's `## Independent review`.", new(ReviewState.Round, 503, MoveKind.Exit)),
        new(Skill.TicketCaptain, "Holding a round with several seats, post one verdict naming each seat's model and provider and its blockers, and write the PR line at the same moment.", new(ReviewState.Round, 503, MoveKind.Exit)),
        new(Skill.LinearWorkspaceStandard, "The verdict lives on the Review sub-ticket: PASS or FAIL, lenses, seats and shoulds. The PR body's `## Independent review` carries only a one-line PASS naming its Review ticket, written with the verdict; the parent gets no copy, and `dydo map` and `dydo survey board` show the latest verdict on its card.", new(Section.LinearEvidence, 1))
    ])
    { Spec = ["R11", "R17", "S8"], Transition = new(ReviewState.Round, ReviewState.Verdict, "the verdict and the PR line are posted") { Effect = Kinds } };

    public static readonly Claim Broken = new("A broken or against-spec blocker goes to Fix, then the next round.", [Actor.TicketCaptain],
        [new(Skill.TicketCaptain, "A broken or against-spec blocker is fixed, then the next round runs.", new(ReviewState.Verdict, 504, MoveKind.Exit))])
    { Spec = ["R9", "S9"], Transition = new(ReviewState.Verdict, ReviewState.Fixing, "a blocker is broken or against the spec") { Guard = "kind = \"broken\" /\\ round # 3" } };

    public static readonly Claim Security = new("A security hole goes to Fix, then a round with the security lens first.", [Actor.TicketCaptain],
        [new(Skill.TicketCaptain, "A security hole is fixed, and the next round runs with the security lens first.", new(ReviewState.Verdict, 505, MoveKind.Exit))])
    {
        Spec = ["R9"],
        Transition = new(ReviewState.Verdict, ReviewState.Fixing, "a blocker is a security hole") { Guard = "kind = \"security\" /\\ round # 3", Effect = "securityFirst' = TRUE" }
    };

    public static readonly Claim Fundamental = new("A fundamental blocker or a new requirement goes to the officer above to re-plan; above a top-level captain a fresh Wayfinding captain re-plans, and the coordinator only opens its ticket.", [Actor.TicketCaptain],
        [new(Skill.TicketCaptain, "Send a fundamental blocker or a new requirement to the officer above to re-plan; when that is the coordinator, it only opens a Wayfinding ticket, and a fresh captain there re-plans.", new(ReviewState.Verdict, 506, MoveKind.Exit))])
    { Spec = ["R9", "H5"], Transition = new(ReviewState.Verdict, ReviewState.Replan, "a blocker is fundamental, or a new requirement") { Guard = "kind = \"fundamental\"" } };

    public static readonly Claim NeedsBalazs = new("A blocker that needs Balazs goes up the chain at once.", [Actor.TicketCaptain],
        [new(Skill.TicketCaptain, "Pass a blocker that needs Balazs up the chain at once.", new(ReviewState.Verdict, 507, MoveKind.Exit))])
    { Spec = ["R9"], Transition = new(ReviewState.Verdict, ReviewState.Escalated, "a blocker needs Balazs") { Guard = "kind = \"balazs\"" } };

    public static readonly Claim Clean = new("By default the first round with no new blocker ends the phase.", [Actor.TicketCaptain],
        [new(Skill.TicketCaptain, "By default, the first round with no new blocker ends the review rounds.", new(ReviewState.Verdict, 508, MoveKind.Exit))])
    { Spec = ["R8"], Transition = new(ReviewState.Verdict, ReviewState.Ended, "no new blocker this round") { Guard = "kind = \"none\"" } };

    public static readonly Claim OneMore = new("The officer running the rounds may order one more round.", [Actor.TicketCaptain],
        [new(Skill.TicketCaptain, "You may still order one more round after a round with no new blocker.", new(ReviewState.Verdict, 509, MoveKind.Exit))])
    {
        Spec = ["R8"],
        Transition = new(ReviewState.Verdict, ReviewState.Round, "no new blocker, but the officer orders one more round") { Guard = "kind = \"none\"", Effect = "round' = round + 1" }
    };

    public static readonly Claim FixDone = new("Fix rN is Done when its blockers are Done and proof ran on the head; a full-lens round follows.", [Actor.TicketCaptain],
        [new(Skill.TicketCaptain, "Start the next round, a fresh full-lens look, once Fix rN's blockers are Done and its proof ran on the head.", new(ReviewState.Fixing, 510, MoveKind.Exit))])
    { Spec = ["S9", "R8"], Transition = new(ReviewState.Fixing, ReviewState.Round, "Fix rN is Done: its blockers Done, proof run on the head") { Effect = "round' = round + 1" } };

    public static readonly Claim ToCheckpoint = new("After round 3 with a blocker comes a checkpoint, not a stop.", [Actor.TicketCaptain],
        [new(Skill.TicketCaptain, "After round 3 with a blocker, stop at the checkpoint and decide on purpose.", new(ReviewState.Verdict, 511, MoveKind.Exit))])
    { Spec = ["R10"], Transition = new(ReviewState.Verdict, ReviewState.Checkpoint, "round 3 ends with a blocker") { Guard = "round = 3 /\\ kind # \"none\"" } };

    public static readonly Claim CheckpointGoOn = new("At the checkpoint the default is one more round, written on the Review.", [Actor.TicketCaptain],
    [
        new(Skill.TicketCaptain, "By default, write `checkpoint: one more round, because …` on the Review and go on.", new(ReviewState.Checkpoint, 512, MoveKind.Exit)),
        new(Skill.LinearWorkspaceStandard, "The round-3 checkpoint is one line on the Review: `checkpoint: one more round, because …` or `escalated: …`.", new(Section.LinearEvidence, 3), () => [CheckpointEscalate!])
    ])
    {
        Spec = ["R10", "S10", "R9"],
        Transition = new(ReviewState.Checkpoint, ReviewState.Fixing, "checkpoint: one more round, the security lens first for a security hole")
            { Guard = "kind \\in {\"broken\", \"security\"}", Effect = "securityFirst' = (securityFirst \\/ kind = \"security\")" }
    };

    public static readonly Claim CheckpointEscalate = new("At the checkpoint, escalate only for a fundamental break, a new requirement or a need for him.", [Actor.TicketCaptain],
        [new(Skill.TicketCaptain, "Escalate only for a fundamental break, a new requirement or a need for Balazs, writing `escalated: …` on the Review.", new(ReviewState.Checkpoint, 513, MoveKind.Exit))])
    {
        Spec = ["R10", "S10"],
        Transition = new(ReviewState.Checkpoint, ReviewState.Escalated, "a fundamental break, a new requirement, or a need for Balazs")
            { Guard = "kind \\in {\"fundamental\", \"balazs\"}" }
    };

    public static readonly Claim FixTicket = new("At verdict time blockers become Fix rN with one Blocker ticket each.", [Actor.TicketCaptain],
    [
        new(Skill.TicketCaptain, "At verdict time, turn the blockers into Fix rN with one Blocker ticket per blocker inside it; for parallel writers, split it into Fix rNb.", new(ReviewState.Verdict, 514, MoveKind.Nominal)),
        new(Skill.LinearWorkspaceStandard, "At verdict time, blockers become a Fix rN ticket with one Blocker ticket per blocker inside it; Fix rN is Done when its blockers are Done and proof ran on the head; for parallel writers it splits into Fix rNb.", new(Section.LinearTickets, 7))
    ])
    { Spec = ["S9"] };

    public static readonly Claim Nit = new("A nit is one line on its record.", [Actor.TicketCaptain, Actor.Reviewer],
    [
        new(Skill.TicketCaptain, "Record each nit as one line on its record.", new(ReviewState.Verdict, 515, MoveKind.Nominal)),
        new(Skill.Reviewer, "Holding the round alone, record each nit as one line on its record.", new(ReviewState.Round, 525, MoveKind.Nominal)),
        new(Skill.LinearWorkspaceStandard, "A nit is one line on its record; an inquisition round's nits go on that round's nit ticket.", new(Section.LinearEvidence, 4))
    ])
    { Spec = ["S11"] };

    public static readonly Claim CaptainMade = new("A blocker later found in a captain-made change goes to a crew writer.", [Actor.TicketCaptain],
        [new(Skill.TicketCaptain, "A blocker a later review finds in a change you made goes to a crew writer.", new(ReviewState.Verdict, 516, MoveKind.Nominal))])
    { Spec = ["R16"] };

    public static readonly Claim Labels = new("The captain records every severity a fix-hop author changed, with its reason.", [Actor.TicketCaptain],
        [new(Skill.TicketCaptain, "Record every severity a fix-hop author changed, with its reason.", new(ReviewState.Verdict, 517, MoveKind.Nominal))])
    { Spec = ["N12 DR 058 §3"] };

    public static readonly Claim FixOwner = new("Who gets the Fix is the officer's call; the next round is a fresh look either way.", [Actor.TicketCaptain],
        [new(Skill.TicketCaptain, "Give the Fix to the implementer's thread while it is alive and has room in its context, and to a fresh agent when its context is heavy or the blocker says the approach was wrong; the next round is a fresh look either way.", new(ReviewState.Fixing, 518, MoveKind.Nominal))])
    { Spec = ["S9"] };

    public static readonly Claim Severities = new("Severities are blocker, should and nit; FAIL means at least one blocker.", [Actor.Reviewer],
    [
        new(Skill.Reviewer, "Give every finding one severity: blocker, the intended case is wrong; should, a real bug callers will hit inside the intent; nit, everything else. FAIL means at least one blocker.", new(ReviewState.Round, 519, MoveKind.Nominal)),
        new(Skill.LinearWorkspaceStandard, "Severity is `blocker`, `should` or `nit`: blocker, the intended case is wrong; should, a real bug callers will hit inside the intent; nit, everything else. FAIL means at least one blocker.", new(Section.LinearLabels, 3))
    ])
    { Spec = ["R11", "N12 DR 058 §1"] };

    public static readonly Claim OnePass = new("All visible blockers are reported in one pass.", [Actor.Reviewer],
        [new(Skill.Reviewer, "Report every visible blocker in one pass.", new(ReviewState.Round, 520, MoveKind.Nominal))])
    { Spec = ["R8"] };

    public static readonly Claim NoRerun = new("A fresh reviewer judges through lenses and re-runs nothing the same way.", [Actor.Reviewer],
        [new(Skill.Reviewer, "Re-run what the author already ran, driving included, only when enough has changed since that its result is in doubt.", new(ReviewerPart.Tools, 3215))])
    { Spec = ["R5"] };

    // The reviewer states it on Toolset.FormalNeeds's combined line (R34).
    public static readonly Claim Statechart = new("A statechart is used only for a finished subsystem that has a state machine.", [Actor.Reviewer], [])
    { Spec = ["R7"] };
}
