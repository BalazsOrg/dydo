namespace DynaDocs.SkillBuild;

/// <summary>The overview (M0): dydo 4.0.0's areas in the order work meets them, each a machine of its own.</summary>
public static class Overview
{
    public static readonly Claim Build = Step(OverviewState.ClaimArea, OverviewState.ProjectArea, "the model builds the skills the Project runs on", "K1", "K6");
    public static readonly Claim Cut = Step(OverviewState.ProjectArea, OverviewState.DispatchArea, "the admiral cuts a ticket and dispatches its captain", "H5", "H3");
    public static readonly Claim Start = Step(OverviewState.DispatchArea, OverviewState.StatusArea, "the holder sets its step In Progress", "S6", "S13");
    public static readonly Claim Hops = Step(OverviewState.StatusArea, OverviewState.HopArea, "a step with its own review loop gets its hops", "S3", "S4");
    public static readonly Claim Prove = Step(OverviewState.HopArea, OverviewState.ProofArea, "the Implement hop proves with receipts", "R5", "T3");
    public static readonly Claim Explain = Step(OverviewState.ProofArea, OverviewState.DocsArea, "a change to declared paths flags its doc", "D5");
    public static readonly Claim Review = Step(OverviewState.DocsArea, OverviewState.ReviewArea, "the parts are wired; the review opens", "R2");
    public static readonly Claim Shoulds = Step(OverviewState.ReviewArea, OverviewState.ShouldArea, "a round finds a should", "S17");
    public static readonly Claim Merge = Step(OverviewState.ShouldArea, OverviewState.MergeArea, "PASS on the head: the Merge hop runs dydo merge", "R17", "R23");
    public static readonly Claim Findings = Step(OverviewState.MergeArea, OverviewState.FindingsArea, "a real red after the merge becomes a CI finding", "Y9");
    public static readonly Claim Feed = Step(OverviewState.FindingsArea, OverviewState.ProjectArea, "findings feed the admiral's next cut", "H5", "S22");
    public static readonly Claim Quiet = Step(OverviewState.FindingsArea, OverviewState.LivenessArea, "the survey finds a quiet hop", "S23", "S24");
    public static readonly Claim Need = Step(OverviewState.LivenessArea, OverviewState.EnvoyArea, "a need only Balazs can settle goes up the chain", "H16");
    public static readonly Claim Hard = Step(OverviewState.EnvoyArea, OverviewState.LawArea, "a hard call goes to the tribunal", "H5", "L18");
    public static readonly Claim Night = Step(OverviewState.LawArea, OverviewState.StaffArea, "overnight rulings wait for the morning brief", "L19", "L21");
    public static readonly Claim Land = Step(OverviewState.StaffArea, OverviewState.MutationArea, "every delivery ticket merged: the landing runs", "N5", "T12");
    public static readonly Claim Accept = Step(OverviewState.MutationArea, OverviewState.MasterArea, "his acceptance: the landing PR goes to master", "N6");
    public static readonly Claim Archive = Step(OverviewState.MasterArea, OverviewState.SafeDeleteArea, "merged: branches and worktrees archived", "N10");
    public static readonly Claim Tag = Step(OverviewState.SafeDeleteArea, OverviewState.Tagged, "Balazs pushes the release tag", "R28", "N2");
    public static readonly Claim Ship = Step(OverviewState.Tagged, OverviewState.AdopterArea, "adopters take one ordinary update", "K18", "N8");

    private static Claim Step(OverviewState from, OverviewState to, string arrow, params string[] spec) =>
        new($"In the overview, {arrow}.", [Actor.Admiral], []) { Spec = spec, Transition = new(from, to, arrow) };
}
