namespace DynaDocs.SkillBuild;

/// <summary>
/// The 4.0.0 merge machines (K1): <c>dydo merge</c> and its refusals, the Merge hop into the feature
/// branch, and the route into master, two-way or one-way.
/// </summary>
public static class MergeMachines
{
    public static readonly Machine MergeCommand = new(MachineId.MergeCommand, "dydo merge and its refusals",
    [
        new(MergeCommandState.Invoked, "dydo merge run on a PR", "R23"),
        new(MergeCommandState.Refused, "Refused with a stated reason", "R19", "R23", "R26", "R27"),
        new(MergeCommandState.Conflict, "CONFLICT: the merge with the tip is not clean", "R21", "R23"),
        new(MergeCommandState.Merged, "A merge commit on the target", "R32", "N12 DR 061 §3"),
        new(MergeCommandState.AlreadyIn, "Already integrated: exit 0", "N12 DR 061 §2")
    ], MergeCommandState.Invoked, [MergeCommandState.Refused, MergeCommandState.Conflict, MergeCommandState.Merged, MergeCommandState.AlreadyIn]);

    public static readonly Machine FeatureMerge = new(MachineId.FeatureMerge, "The Merge hop into the feature branch",
    [
        new(FeatureMergeState.MergeTodo, "Merge hop in Todo", "S3", "S12"),
        new(FeatureMergeState.PreGate, "Running dydo test gate on the combined tree", "R21", "Y4"),
        new(FeatureMergeState.PreFix, "An implementer fixing a red gate", "R21"),
        new(FeatureMergeState.Rereview, "A fresh round on the new head", "R8", "R17", "R23", "R24"),
        new(FeatureMergeState.Running, "Running dydo merge", "R21", "R23"),
        new(FeatureMergeState.Resolving, "Resolving a CONFLICT on the ticket branch", "R19", "R21"),
        new(FeatureMergeState.HunkReview, "Judging a resolution's hunks", "R19"),
        new(FeatureMergeState.Gating, "Gates on the merge", "R18", "R20"),
        new(FeatureMergeState.Rerun, "Red gate rerun once", "R22"),
        new(FeatureMergeState.Reverted, "Merge reverted", "R22", "S12", "Y9"),
        new(FeatureMergeState.FixRound, "Fix and fix-scoped round", "R22"),
        new(FeatureMergeState.MergeDone, "Merge hop Done", "R20")
    ], FeatureMergeState.MergeTodo, [FeatureMergeState.MergeDone]);

    public static readonly Machine MasterRoute = new(MachineId.MasterRoute, "A PR into master",
    [
        new(MasterRouteState.Offered, "PR into master offered", "R26", "R27"),
        new(MasterRouteState.Paced, "Waiting for its turn", "R29"),
        new(MasterRouteState.Resync, "Re-checking against current master", "R29"),
        new(MasterRouteState.Held, "Held after a third forced re-sync", "R29"),
        new(MasterRouteState.AfkMerge, "AFK merge by its captain", "R26", "S12"),
        new(MasterRouteState.CiWatch, "One-way: watching its CI", "R31"),
        new(MasterRouteState.HitlMerge, "HITL Merge waiting on his click", "R27", "R31", "S12"),
        new(MasterRouteState.Merged, "Merged into master", "R26", "R31"),
        new(MasterRouteState.Reverted, "Reverted after a real red", "R22", "Y9"),
        new(MasterRouteState.Closed, "Master Merge hop Done", "R20", "R31")
    ], MasterRouteState.Offered, [MasterRouteState.Closed])
    {
        Extends = ["Naturals"],
        Variables = [new("resyncs", "0", "resyncs <= 3"), new("oneWay", "FALSE", "oneWay \\in BOOLEAN")]
    };
}
