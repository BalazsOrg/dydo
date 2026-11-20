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

    // DYD-1082 555a8300 gap 9 and DYD-1195 8661b8f4: the captains who built a rejected change are done.
    public static readonly Claim RejectRewrite = Reject(RoleEvaluationState.WrittenFresh, "rewrite");
    public static readonly Claim RejectFold = Reject(RoleEvaluationState.Folded, "fold");
    public static readonly Claim RejectRemoval = Reject(RoleEvaluationState.Removed, "removal");

    public static readonly Claim Rebuild = new("In the role evaluation, new captains on new tickets take a rejected change: the role is questioned again with his verdict, and the candidate is rebuilt (DYD-1195 8661b8f4).",
        [Actor.TicketCaptain], [])
    { Spec = ["K22", "K16"], Transition = new(RoleEvaluationState.Changed, RoleEvaluationState.Questioned, "new captains on new tickets question it again with his verdict, and rebuild") };

    private static Claim Reject(RoleEvaluationState from, string what) =>
        new($"In the role evaluation, a {what} he rejects at his walkthrough is a change for new captains on new tickets (DYD-1195 8661b8f4).", [Actor.Balazs], [])
        { Spec = ["K22", "K16"], Transition = new(from, RoleEvaluationState.Changed, $"he rejects the {what}: a change for new captains on new tickets") };

    private static Claim Move(RoleEvaluationState from, RoleEvaluationState to, string arrow, params string[] spec) =>
        new($"In the role evaluation, {arrow}.", to == RoleEvaluationState.Approved ? [Actor.Balazs] : [], []) { Spec = spec, Transition = new(from, to, arrow) };
}
