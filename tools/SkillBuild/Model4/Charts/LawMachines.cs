namespace DynaDocs.SkillBuild;

/// <summary>
/// The 4.0.0 machines for judgement and the landing's checks (K1): the council, the tribunal, how
/// the law grows, the one-time DR backfill, the landing's mutation check, safe delete, and a changed
/// file over the token limit.
/// </summary>
public static class LawMachines
{
    public static readonly Machine Council = new(MachineId.Council, "A council",
    [
        new(CouncilState.Convened, "Council convened", "L14", "L15", "L16"),
        new(CouncilState.Waiting, "Merge confidence waiting for both families", "L22"),
        new(CouncilState.Answered, "Every member answered, sealed", "L14"),
        new(CouncilState.Synthesised, "Synthesised by a non-member", "L14")
    ], CouncilState.Convened, [CouncilState.Synthesised]);

    public static readonly Machine Tribunal = new(MachineId.Tribunal, "The tribunal",
    [
        new(TribunalState.HardCall, "A hard call", "L18", "H5"),
        new(TribunalState.DrSettled, "Settled by a DR", "L18"),
        new(TribunalState.CaseFile, "Clerk holds the case file", "L18"),
        new(TribunalState.Sealed, "Two judges, sealed", "L18"),
        new(TribunalState.Exchange, "One exchange", "L18"),
        new(TribunalState.DecidedOvernight, "Decided overnight", "L18", "L19", "L20"),
        new(TribunalState.Prepared, "Prepared for him", "L18", "L20"),
        new(TribunalState.ProposedDr, "Proposed DR", "L21"),
        new(TribunalState.Scored, "His answer scored", "L20"),
        new(TribunalState.Investigated, "Ruling found wrong, investigated", "L19")
    ], TribunalState.HardCall,
    [TribunalState.DrSettled, TribunalState.DecidedOvernight, TribunalState.ProposedDr, TribunalState.Scored, TribunalState.Investigated])
    {
        Variables =
        [
            new("night", "FALSE", "night \\in BOOLEAN"),
            new("twoWay", "FALSE", "twoWay \\in BOOLEAN"),
            new("drGrade", "FALSE", "drGrade \\in BOOLEAN"),
            new("blocking", "FALSE", "blocking \\in BOOLEAN"),
            new("agree", "FALSE", "agree \\in BOOLEAN")
        ],
        Gaps =
        [
            new(TribunalState.Prepared, TribunalState.Scored, "L20 scores his daytime answers against the prediction but names no scorer"),
            new(TribunalState.DecidedOvernight, TribunalState.Investigated, "L19 has a ruling found wrong investigated but names no investigator")
        ]
    };

    public static readonly Machine Law = new(MachineId.Law, "How the law grows",
    [
        new(LawState.Evidence, "Evidence for the law", "L11"),
        new(LawState.Candidate, "Amendment candidate", "L7", "L11"),
        new(LawState.Mechanical, "A test, lint or gate", "L7", "L13"),
        new(LawState.Proposed, "Principle proposed to him", "L7", "L13"),
        new(LawState.Enacted, "Principle enacted", "L3", "L6", "L13"),
        new(LawState.Retired, "Principle retired", "L11"),
        new(LawState.CaseStudy, "Case study prepared", "L9"),
        new(LawState.CaseLaw, "Case study approved", "L9")
    ], LawState.Evidence, [LawState.Mechanical, LawState.Enacted, LawState.Retired, LawState.CaseLaw]);

    public static readonly Machine Backfill = new(MachineId.Backfill, "The DR backfill",
    [
        new(BackfillState.Untagged, "Accepted DR, untagged", "L12"),
        new(BackfillState.Drafted, "Tags drafted", "L12"),
        new(BackfillState.Checked, "Citations checked", "L12"),
        new(BackfillState.OnPage, "On his review page", "L12"),
        new(BackfillState.Merged, "Row approved and merged", "L12"),
        new(BackfillState.Rejected, "Row rejected", "L12")
    ], BackfillState.Untagged, [BackfillState.Merged, BackfillState.Rejected]);

    public static readonly Machine Mutation = new(MachineId.Mutation, "The landing's mutation check",
    [
        new(MutationState.Background, "Background campaigns in spare capacity", "T12"),
        new(MutationState.Queued, "Landing work queued for the exact tree", "T12"),
        new(MutationState.Running, "Running: incomplete, the landing blocked", "T12", "N15"),
        new(MutationState.Survivors, "A mutant survived", "T12"),
        new(MutationState.Ticketed, "A survivor to fix or claim", "T12", "H5", "N6"),
        new(MutationState.Claimed, "Claimed equivalent", "T12"),
        new(MutationState.Dispute, "Claim disputed: a council", "T12", "L14"),
        new(MutationState.Clean, "No survivor without a fix or an approved equivalent", "T12", "N15")
    ], MutationState.Background, [MutationState.Clean])
    {
        Variables = [new("cloudRouteNotMuchMoreWork", "FALSE", "cloudRouteNotMuchMoreWork \\in BOOLEAN")]
    };

    public static readonly Machine SafeDelete = new(MachineId.SafeDelete, "Safe delete",
    [
        new(SafeDeleteState.Live, "Branch or worktree in use, a T3 worktree included", "N10"),
        new(SafeDeleteState.Archived, "Archived for 30 days", "N10"),
        new(SafeDeleteState.Purged, "Purged", "N10")
    ], SafeDeleteState.Live, [SafeDeleteState.Purged])
    {
        Gaps = [new(SafeDeleteState.Archived, SafeDeleteState.Live, "N10 names no restore from the archive; the built command has one")]
    };

    public static readonly Machine IgnoreEntry = new(MachineId.IgnoreEntry, "A file over the token limit",
    [
        new(IgnoreEntryState.OverLimit, "A changed file over the token limit", "T15", "T16"),
        new(IgnoreEntryState.Split, "Split under the limit", "T15"),
        new(IgnoreEntryState.PathA, "Ignore path (a): a file nobody writes by hand", "T17"),
        new(IgnoreEntryState.OwnTicket, "Ignore path (b): its own ticket", "T18"),
        new(IgnoreEntryState.CouncilB, "Path (b) before a council", "T18"),
        new(IgnoreEntryState.Ignored, "In the ignore file", "T17", "T18"),
        new(IgnoreEntryState.Stale, "Stale entry: the gate fails", "T17")
    ], IgnoreEntryState.OverLimit, [IgnoreEntryState.Split, IgnoreEntryState.Ignored, IgnoreEntryState.Stale]);
}
