namespace DynaDocs.SkillBuild;

/// <summary>The claim build (the ClaimBuild machine, DR 064): a claim edit, the builder, its checks, the guard's nudge, his approval and the downstream update.</summary>
public static class Build
{
    public static readonly Claim Refused = Move(ClaimBuildState.Edited, ClaimBuildState.Rejected, Actor.SkillBuild,
        "the build refuses: a claim not on exactly one line per carrier, a misplaced move, a shape limit", "K4", "K10", "K9");
    public static readonly Claim Write = Move(ClaimBuildState.Edited, ClaimBuildState.Built, Actor.SkillBuild,
        "dotnet run --project tools/SkillBuild writes every built file with its header", "K6", "K8", "K5", "K12", "K13", "K23");
    public static readonly Claim Golden = Move(ClaimBuildState.Built, ClaimBuildState.Rejected, Actor.SkillBuild,
        "the golden test finds a byte difference, or the stray-claim lint finds pasted claim text", "K7", "K24");
    public static readonly Claim Pass = Move(ClaimBuildState.Built, ClaimBuildState.Checked, Actor.SkillBuild,
        "golden, shape and stray-claim checks pass, in the suite and in CI", "K7", "K10", "K24");
    public static readonly Claim Mend = Move(ClaimBuildState.Rejected, ClaimBuildState.Edited, Actor.Implementer,
        "the model is fixed", "K6");
    public static readonly Claim HandEdit = Move(ClaimBuildState.Built, ClaimBuildState.Nudged, Actor.Guard,
        "an Edit or Write to a built path", "K6");
    public static readonly Claim EditThere = Move(ClaimBuildState.Nudged, ClaimBuildState.Edited, Actor.Implementer,
        "the edit goes into the model instead", "K6");
    public static readonly Claim Tlc = Move(ClaimBuildState.Checked, ClaimBuildState.TlcPassed, Actor.SkillBuild,
        "TLC passes the generated moves against the hand-written properties; the result is recorded with the candidate", "K3", "K20", "N14");
    public static readonly Claim TlcFail = Move(ClaimBuildState.Checked, ClaimBuildState.TlcFailed, Actor.SkillBuild,
        "TLC fails a property: the push is not done and nothing goes to his approval", "K3", "K20");
    public static readonly Claim Remodel = Move(ClaimBuildState.TlcFailed, ClaimBuildState.Edited, Actor.Implementer,
        "the push is not done: the model is fixed and built again", "K20", "K6");
    public static readonly Claim Read = Move(ClaimBuildState.TlcPassed, ClaimBuildState.InReview, Actor.Balazs,
        "the model PR is read through its built-skill diff", "K16");
    public static readonly Claim Approve = Move(ClaimBuildState.InReview, ClaimBuildState.Approved, Actor.Balazs,
        "he approves: in the 4.0.0 push once, at the final candidate; after it, each model PR", "K16", "K20", "K22");
    public static readonly Claim Rejected = Move(ClaimBuildState.InReview, ClaimBuildState.Edited, Actor.Balazs,
        "he rejects a change at his walkthrough: new captains on new tickets edit the model, and the candidate is rebuilt (DYD-1195 8661b8f4)", "K16", "K22");
    public static readonly Claim Ship = Move(ClaimBuildState.Approved, ClaimBuildState.Released, Actor.SkillBuild,
        "downstream gets the built files in one ordinary update", "K18");

    private static Claim Move(ClaimBuildState from, ClaimBuildState to, Actor actor, string arrow, params string[] spec) =>
        new($"In the claim build, {arrow}.", [actor], []) { Spec = spec, Transition = new(from, to, arrow) };
}
