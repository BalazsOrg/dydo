namespace DynaDocs.SkillBuild;

/// <summary>
/// The 4.0.0 machines for a Project's delivery (K1): the overview of every area, the Project, its
/// landing, a ticket's status, a step's hops and the review loop. Each state cites the spec rule it
/// rests on; a gap marks an arrow the spec leaves open.
/// </summary>
public static class DeliveryMachines
{
    public static readonly Machine Overview = new(MachineId.Overview, "dydo 4.0.0",
    [
        new(OverviewState.ClaimArea, "The claim build and the role evaluation", "K1", "K4", "K6", "K7", "K16", "K22", "K24"),
        new(OverviewState.HostArea, "Host settings, the guard and the statechart tool", "N12 DR 055 §1", "N12 DR 056 §1", "N12 DR 062 §1", "H20"),
        new(OverviewState.ProjectArea, "A Project and its coordinator", "H4", "H5", "H10", "S14", "S15", "R25", "N13"),
        new(OverviewState.TicketArea, "A ticket: dispatch, status and hops", "H1", "H2", "H3", "K14", "S3", "S5", "S6", "S12"),
        new(OverviewState.ProofArea, "Proof tests and gates", "T1", "T3", "T5", "T13", "T21", "T23", "Y4"),
        new(OverviewState.DocsArea, "Descriptive docs", "D1", "D2", "D3", "D4", "D5", "D6", "D7"),
        new(OverviewState.ReviewArea, "The review loop and its shoulds", "R2", "R3", "R8", "R9", "R10", "R11", "S8", "S17"),
        new(OverviewState.MergeArea, "dydo merge, the Merge hop and safe delete", "R18", "R19", "R20", "R21", "R22", "R23", "N10"),
        new(OverviewState.WatchArea, "Survey, map, liveness and CI findings", "S19", "S22", "S23", "S24", "S26", "Y9", "R30"),
        new(OverviewState.JudgementArea, "Envoys, councils, the tribunal and the law", "H15", "H16", "L7", "L14", "L18", "L22"),
        new(OverviewState.StaffArea, "Chief of staff, overnight, and DRs awaiting ratification", "H7", "L19", "L21"),
        new(OverviewState.LandingArea, "The landing: inquisition, prerequisites and mutation", "N5", "N6", "N7", "T12", "N15"),
        new(OverviewState.MasterArea, "The master route", "R26", "R27", "R29", "R31"),
        new(OverviewState.Tagged, "4.0.0 tagged", "R28", "N2", "N15"),
        new(OverviewState.AdopterArea, "The adopter update", "K18", "N1", "N8", "N11")
    ], OverviewState.ClaimArea, [OverviewState.AdopterArea]);

    public static readonly Machine Project = new(MachineId.Project, "A Project and its coordinator",
    [
        new(ProjectState.Unheld, "No live coordinator: not started, idle or closed", "H4"),
        new(ProjectState.Charting, "Charting the spec and the map", "S14", "S15", "S16", "N12 DR 060 §1", "H5"),
        new(ProjectState.Watching, "Watching the survey, the board and liveness", "H5", "H10", "H11", "R25"),
        new(ProjectState.Replanning, "Re-planning after a fundamental blocker or a new requirement", "R9", "S10"),
        new(ProjectState.Landing, "All delivery merged: the landing runs", "N5")
    ], ProjectState.Unheld, [ProjectState.Landing]);

    public static readonly Machine Landing = new(MachineId.Landing, "The landing",
    [
        new(LandingState.Integrated, "Delivery merged into the feature branch", "N5"),
        new(LandingState.Mapped, "A round from zero: a fresh area map", "N5"),
        new(LandingState.ReadPhase, "Read phase on an area", "N5"),
        new(LandingState.BreakPhase, "Break phase on the area", "N5"),
        new(LandingState.DrivePhase, "Drive phase on the area", "N5", "R3"),
        new(LandingState.Inquisition, "The round's verdict, held by the coordinator", "N5", "S8"),
        new(LandingState.RoundFixes, "Fixing the round's blockers and shoulds", "N5", "S9", "S17"),
        new(LandingState.Prerequisites, "Landing prerequisites", "N7", "S18", "T12", "T8", "N14", "N15", "Y2", "Y6", "L6", "L12", "N8", "N9"),
        new(LandingState.MergeReview, "Landing merge review: a Merge-confidence council", "N6", "R20", "L16"),
        new(LandingState.Replanned, "A fundamental blocker or a new requirement: the officer above re-plans", "R9"),
        new(LandingState.Escalated, "A blocker that needs Balazs: up the chain at once", "R9"),
        new(LandingState.Walkthrough, "Balazs walks the landing candidate", "N6", "N4", "N3"),
        new(LandingState.Merging, "Landing PR into master", "N6", "R26", "R27"),
        new(LandingState.Landed, "Landed on master", "N6", "S14", "D11"),
        new(LandingState.Tagged, "Release tag pushed by Balazs", "R28", "N2", "N15")
    ], LandingState.Integrated, [LandingState.Tagged, LandingState.Replanned, LandingState.Escalated])
    {
        Extends = ["Naturals"],
        Variables =
        [
            new("round", "1", "round <= 3"),
            new("areas", "1", "areas \\in 1..2"),
            new("fixFrom", "\"none\"", "fixFrom \\in {\"none\", \"inquisition\", \"merge\"}"),
            new("securityFirst", "FALSE", "securityFirst \\in BOOLEAN"),
            new("prereqs", "0", "prereqs <= 10"),
            new("hardened", "FALSE", "hardened \\in BOOLEAN")
        ],
        Gaps = [new(LandingState.Walkthrough, null, "N6 gives only his acceptance; the route when he does not accept is unstated")]
    };

    public static readonly Machine TicketStatus = new(MachineId.TicketStatus, "A ticket's status",
    [
        new(TicketStatusState.New, "Not yet on the board", "S2"),
        new(TicketStatusState.FutureFeature, "FutureFeature", "S5"),
        new(TicketStatusState.Backlog, "Backlog", "S5"),
        new(TicketStatusState.Todo, "Todo", "S3", "S5"),
        new(TicketStatusState.InProgress, "In Progress", "S5", "S6"),
        new(TicketStatusState.Done, "Done", "S5", "S6"),
        new(TicketStatusState.Canceled, "Canceled, with its reason", "S5", "S10", "S12"),
        new(TicketStatusState.Duplicate, "Duplicate", "S5", "S17")
    ], TicketStatusState.New, [TicketStatusState.Done, TicketStatusState.Canceled, TicketStatusState.Duplicate])
    {
        Gaps =
        [
            new(TicketStatusState.New, TicketStatusState.FutureFeature, "S5 lists FutureFeature, but no rule says what enters it"),
            new(TicketStatusState.FutureFeature, null, "S5 lists FutureFeature, but no rule says how a ticket leaves it"),
            new(TicketStatusState.New, TicketStatusState.Backlog, "S5 lists Backlog, but no rule says when a ticket is filed there")
        ]
    };

    public static readonly Machine Hops = new(MachineId.Hops, "A step's hops, by Type label",
    [
        new(HopsState.Cut, "A step is cut", "S1", "S2", "S7"),
        new(HopsState.OneTicket, "One ticket, no hops", "S4", "R12"),
        new(HopsState.EvidenceCheck, "Evidence check beside the leaf", "S4", "R6"),
        new(HopsState.CheckFix, "Fix rN beside the leaf", "S9", "R8"),
        new(HopsState.ImplementHop, "Implement hop", "S3"),
        new(HopsState.ReviewHop, "Review hop, one per round", "S3", "S8"),
        new(HopsState.FixHop, "Fix hop, Fix rN", "S3", "S9"),
        new(HopsState.StepDone, "Step Done", "S6"),
        new(HopsState.AfkMerge, "AFK Merge hop", "S12"),
        new(HopsState.HitlMerge, "HITL Merge hop", "S12", "R27")
    ], HopsState.Cut, [HopsState.StepDone, HopsState.AfkMerge, HopsState.HitlMerge]);

    public static readonly Machine Review = new(MachineId.Review, "The review loop",
    [
        new(ReviewState.Wired, "Parts wired, before the Merge hop", "R1", "R2", "R6"),
        new(ReviewState.Round, "Review round", "R3", "R5", "R6", "R8", "R12", "R15", "R33"),
        new(ReviewState.Verdict, "Verdict posted", "R11", "R17", "S8"),
        new(ReviewState.Fixing, "Fix rN", "S9", "R8"),
        new(ReviewState.Checkpoint, "Round-3 checkpoint", "R10", "S10"),
        new(ReviewState.Replan, "Sent up to re-plan", "R9", "S10"),
        new(ReviewState.Escalated, "Escalated up the chain", "R9", "R10", "S10"),
        new(ReviewState.Ended, "Review phase ended", "R8", "R6")
    ], ReviewState.Wired, [ReviewState.Ended, ReviewState.Replan, ReviewState.Escalated])
    {
        Extends = ["Naturals"],
        Variables =
        [
            new("round", "1", "round <= 4"),
            new("kind", "\"none\"", "kind \\in {\"none\", \"broken\", \"security\", \"fundamental\", \"balazs\"}"),
            new("securityFirst", "FALSE", "securityFirst \\in BOOLEAN")
        ]
    };
}
