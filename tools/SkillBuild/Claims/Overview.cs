namespace DynaDocs.SkillBuild;

/// <summary>The overview (M0): dydo 4.0.0's areas on their nominal path, each a machine of its own; the side branches return to the path.</summary>
public static class Overview
{
    public static readonly Claim Build = Step(OverviewState.ClaimArea, OverviewState.HostArea,
        "the model builds the skills; a project's init turns memory off and commits the guard", "K6", "N12 DR 055 §1", "N12 DR 056 §1");
    public static readonly Claim Hold = Step(OverviewState.HostArea, OverviewState.ProjectArea, "an admiral holds a live Project", "H4");
    public static readonly Claim Cut = Step(OverviewState.ProjectArea, OverviewState.TicketArea, "the admiral cuts a ticket and dispatches its captain", "H5", "H3");
    public static readonly Claim Prove = Step(OverviewState.TicketArea, OverviewState.ProofArea, "the Implement hop proves its work with red tests and receipts", "R5", "T3");
    public static readonly Claim Review = Step(OverviewState.ProofArea, OverviewState.ReviewArea, "the parts are wired; the review opens", "R2");
    public static readonly Claim Merge = Step(OverviewState.ReviewArea, OverviewState.MergeArea,
        "the first round with no new blocker: the Merge hop runs dydo merge", "R8", "R17", "R23");
    public static readonly Claim Next = Step(OverviewState.MergeArea, OverviewState.ProjectArea, "merged: the admiral cuts the next ticket", "R20", "H5");
    public static readonly Claim Land = Step(OverviewState.ProjectArea, OverviewState.LandingArea, "every delivery ticket merged: the landing runs", "N5");
    public static readonly Claim Accept = Step(OverviewState.LandingArea, OverviewState.MasterArea, "his acceptance: the landing PR goes to master", "N6");
    public static readonly Claim Tag = Step(OverviewState.MasterArea, OverviewState.Tagged, "Balazs pushes the release tag", "R28", "N2");
    public static readonly Claim Ship = Step(OverviewState.Tagged, OverviewState.AdopterArea, "adopters take one ordinary update", "K18", "N8");

    public static readonly Claim Explain = Step(OverviewState.ProofArea, OverviewState.DocsArea, "a change to declared paths flags its doc", "D5");
    public static readonly Claim Explained = Step(OverviewState.DocsArea, OverviewState.ProofArea, "the writer updates the doc or states it holds", "D5");
    public static readonly Claim Watch = Step(OverviewState.ProjectArea, OverviewState.WatchArea, "the board, the map and liveness are read", "S19", "S22", "S24");
    public static readonly Claim Route = Step(OverviewState.WatchArea, OverviewState.ProjectArea, "findings and CI reds go to their officer", "Y9", "H11", "H5");
    public static readonly Claim Need = Step(OverviewState.ProjectArea, OverviewState.JudgementArea,
        "a need only he can settle goes up the chain, or a hard call to the tribunal", "H16", "L18");
    public static readonly Claim Decided = Step(OverviewState.JudgementArea, OverviewState.ProjectArea,
        "a Question decided, by him by day or provisionally overnight, is acted on and never ratified", "L19", "L20");
    public static readonly Claim Proposed = Step(OverviewState.JudgementArea, OverviewState.StaffArea,
        "overnight, both judges agree on a two-way, seriously blocking DR-grade call: a proposed DR", "L21");
    public static readonly Claim Ratify = Step(OverviewState.StaffArea, OverviewState.ProjectArea, "he ratifies the proposed DR", "L21");
    public static readonly Claim List = Step(OverviewState.ProjectArea, OverviewState.StaffArea,
        "the chief of staff reads the board across Projects into his short list", "H7");
    public static readonly Claim GiveOut = Step(OverviewState.StaffArea, OverviewState.ProjectArea, "he gives out a ticket from that list, or tells the chief of staff to", "H7");
    public static readonly Claim Refix = Step(OverviewState.LandingArea, OverviewState.TicketArea, "a landing round's blockers become Fix rN", "N5", "S9");

    private static Claim Step(OverviewState from, OverviewState to, string arrow, params string[] spec) =>
        new($"In the overview, {arrow}.", [], []) { Spec = spec, Transition = new(from, to, arrow) };
}
