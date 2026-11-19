namespace DynaDocs.SkillBuild;

/// <summary>The nine role skills evaluated (the RoleEvaluation machine, K22): each questioned, then kept and written fresh, folded or removed, and approved at the final candidate.</summary>
public static class Evaluation
{
    public static readonly Claim Keep = Move(RoleEvaluationState.Questioned, RoleEvaluationState.Kept,
        "questioned, it should exist: a survivor", "K22");
    public static readonly Claim Fold = Move(RoleEvaluationState.Questioned, RoleEvaluationState.Folded,
        "questioned, it folds into a survivor, which carries its responsibilities", "K22");
    public static readonly Claim Remove = Move(RoleEvaluationState.Questioned, RoleEvaluationState.Removed,
        "questioned, it should not exist: removed", "K22");
    public static readonly Claim Rewrite = Move(RoleEvaluationState.Kept, RoleEvaluationState.WrittenFresh,
        "only a survivor is written fresh from the 4.0.0 decisions and claims, simpler and streamlined; its current file is a checklist of what not to lose, never a source", "K22", "K19");
    public static readonly Claim Approve = Move(RoleEvaluationState.WrittenFresh, RoleEvaluationState.Approved,
        "he approves the rewrite with the other claims, at the final candidate", "K16", "K22");
    public static readonly Claim ApproveFold = Move(RoleEvaluationState.Folded, RoleEvaluationState.Approved,
        "he approves the fold with the other claims, at the final candidate", "K16", "K22");
    public static readonly Claim ApproveRemoval = Move(RoleEvaluationState.Removed, RoleEvaluationState.Approved,
        "he approves the removal with the other claims, at the final candidate", "K16", "K22");

    private static Claim Move(RoleEvaluationState from, RoleEvaluationState to, string arrow, params string[] spec) =>
        new($"In the role evaluation, {arrow}.", to == RoleEvaluationState.Approved ? [Actor.Balazs] : [], []) { Spec = spec, Transition = new(from, to, arrow) };
}
