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
        new(OverviewState.ClaimArea, "The claim build", "K1", "K4", "K6", "K7", "K10", "K16", "K24"),
        new(OverviewState.ProjectArea, "Project and its admiral", "H4", "H5", "H10", "S14", "S15", "R25", "N13"),
        new(OverviewState.DispatchArea, "Crew dispatch and return", "H1", "H2", "H3", "H20", "K14", "R5", "R13", "R14", "R16"),
        new(OverviewState.StatusArea, "Ticket statuses", "S5", "S6", "S10", "S13"),
        new(OverviewState.HopArea, "Hops by Type label", "S3", "S4", "S9", "S12"),
        new(OverviewState.ProofArea, "Proof tests and gates", "T1", "T3", "T5", "T6", "T13", "T21", "T23", "Y4"),
        new(OverviewState.DocsArea, "Descriptive docs", "D1", "D2", "D3", "D4", "D5", "D6", "D7"),
        new(OverviewState.ReviewArea, "The review loop", "R2", "R3", "R8", "R9", "R10", "R11", "S8"),
        new(OverviewState.ShouldArea, "Shoulds", "S17", "S18"),
        new(OverviewState.MergeArea, "dydo merge and the feature-branch merge", "R18", "R19", "R20", "R21", "R22", "R23", "R24"),
        new(OverviewState.FindingsArea, "Survey, board and CI findings", "S19", "S22", "S23", "Y9", "H11", "R30"),
        new(OverviewState.LivenessArea, "Liveness", "S24", "H12"),
        new(OverviewState.EnvoyArea, "Envoys", "H15", "H16", "H18", "H19", "H22", "H23"),
        new(OverviewState.LawArea, "Law, councils and tribunal", "L7", "L11", "L14", "L16", "L18", "L22", "L23"),
        new(OverviewState.StaffArea, "Chief of staff and overnight", "H7", "L19", "L20", "L21"),
        new(OverviewState.MutationArea, "The landing and its mutation check", "N5", "N6", "N7", "T12", "N15"),
        new(OverviewState.MasterArea, "The master route", "R26", "R27", "R29", "R31"),
        new(OverviewState.SafeDeleteArea, "Safe delete", "N10"),
        new(OverviewState.Tagged, "4.0.0 tagged", "R28", "N2", "N15"),
        new(OverviewState.AdopterArea, "The adopter update", "K18", "N1", "N8", "N11")
    ], OverviewState.ClaimArea, [OverviewState.AdopterArea]);

    public static readonly Machine Project = new(MachineId.Project, "A Project and its admiral",
    [
        new(ProjectState.Unheld, "No live admiral: not started, idle or closed", "H4"),
        new(ProjectState.Charting, "Charting the spec and the map", "S14", "S15", "S16", "N12 DR 060 §1"),
        new(ProjectState.Watching, "Watching the survey, the board and liveness", "H5", "H10", "H11", "R25"),
        new(ProjectState.Replanning, "Re-planning after a fundamental blocker or a new requirement", "R9", "S10"),
        new(ProjectState.Landing, "All delivery merged: the landing runs", "N5")
    ], ProjectState.Unheld, [ProjectState.Landing])
    {
        Gaps =
        [
            new(ProjectState.Watching, ProjectState.Unheld, "H4 names idle Projects but never defines when a Project goes idle"),
            new(ProjectState.Unheld, ProjectState.Watching, "H4 says who holds a live Project, not who starts or wakes its admiral")
        ]
    };

    public static readonly Machine Landing = new(MachineId.Landing, "The landing",
    [
        new(LandingState.Integrated, "Delivery merged into the feature branch", "N5"),
        new(LandingState.Inquisition, "Inquisition round from zero, held by the admiral", "N5", "S8"),
        new(LandingState.RoundFixes, "Fixing the round's blockers and shoulds", "N5", "S9", "S17"),
        new(LandingState.Prerequisites, "Landing prerequisites", "N7", "S18", "T12", "T8", "N14", "N15", "Y2", "Y6", "L6", "L12", "N8", "N9"),
        new(LandingState.MergeReview, "Landing merge review: a Merge-confidence council", "N6", "R20", "L16"),
        new(LandingState.Walkthrough, "Balazs walks the landing candidate", "N6", "N4", "N3"),
        new(LandingState.Merging, "Landing PR into master", "N6", "R26", "R27"),
        new(LandingState.Landed, "Landed on master", "N6", "S14", "D11"),
        new(LandingState.Tagged, "Release tag pushed by Balazs", "R28", "N2", "N15")
    ], LandingState.Integrated, [LandingState.Tagged])
    {
        Extends = ["Naturals"],
        Variables =
        [
            new("round", "1", "round <= 3"),
            new("fixFrom", "\"none\"", "fixFrom \\in {\"none\", \"inquisition\", \"merge\"}"),
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
        new(HopsState.Chain, "Hop sub-tickets in step order", "S3"),
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
