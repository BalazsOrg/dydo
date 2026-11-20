namespace DynaDocs.SkillBuild;

/// <summary>
/// The ticket-captain prototype's "Get it reviewed" (DYD-1372). The captain dispatches reviews and
/// never reviews (DYD-1295 2119cd65); the severities are defined here, where they are used, in the
/// user's words (41a553f1).
/// </summary>
public static class CaptainReview
{
    private const string Rounds = "Linear, DYD-1281's sub-tickets read 2026-11-20: Review r1 to r4, each after the first titled a fresh review that follows Fix r1, r2 or r3.";

    private static readonly Enum[] Me = [Actor.TicketCaptain];

    public static readonly Claim Risk = new("Persistence, migrations, permissions, guard rules and uncertain native interfaces are always reviewed.", Me, [])
    {
        Spec = ["R16"],
        Truth = new(Evidence.Code("Services/Merge/PullRequestMerge.cs"),
            Layer: "`dydo merge` asks every PR for a PASS and cannot tell these five apart from any other change inside one.",
            Reason: "A mistake here is costly or impossible to undo once it has run.")
    };

    public static readonly Claim Other = new("Every change is reviewed by an agent other than its writer, unless it is too small to need it.", Me,
    [
        At(CaptainSlot.Reviewed, 1,
            "Every change is reviewed by an agent other than its writer, unless it is too small to need it; persistence, migrations, permissions, guard rules and uncertain native interfaces always are.",
            () => [Risk])
    ])
    {
        Spec = ["R33", "R12"],
        Truth = new(Evidence.Run(Rounds),
            Layer: "`dydo merge` refuses a PR with no PASS line and cannot tell who wrote the line.",
            Reason: "A writer's own context hides from it what a fresh one sees at once."),
        Transition = new(CaptainState.Leaf, CaptainState.InReview, "the leaf's change is whole: another agent reviews it")
    };

    public static readonly Claim Whose = new("Whoever merges a piece of work gets it reviewed first; a part is reviewed with the parent it joins.", Me,
        [At(CaptainSlot.Reviewed, 2, "What you merge, you get reviewed first; a part your officer merges is reviewed with its whole.")])
    {
        Spec = ["R2"],
        Truth = new(Evidence.Code("Services/Merge/PullRequestMerge.cs"),
            Layer: "`dydo merge` refuses a head with no PASS, so the merger finds out at the merge; nothing tells it before.",
            Reason: "A part reviewed alone and again inside its parent is reviewed twice for one answer.")
    };

    public static readonly Claim NoLoop = new("A plan, spec or contract gets no review loop; a spec gets at most one Questions pass by a native helper, chosen by risk.", Me,
        [At(CaptainSlot.Reviewed, 3, "A plan, spec or contract gets no review rounds; a risky spec gets at most one Questions pass by a native helper.")])
    {
        Spec = ["R4"],
        Truth = new(Evidence.Tool("Agent: a native sub-agent, awaited within the turn, returns its report to its caller alone."),
            Layer: "Nothing limits how often a document is sent for review.",
            Reason: "First make it work, then make it good: rounds on a plan polish what building will change.")
    };

    public static readonly Claim Severity = new("A blocker is fixed now; a should needs fixing, but the fix can wait; a nit is everything else.", Me,
    [
        At(CaptainSlot.Reviewed, 4,
            "A reviewer gives each finding a severity. A blocker is fixed now; a should needs fixing, but the fix can wait; a nit is everything else.")
    ])
    {
        Spec = ["R11"],
        Truth = new(Evidence.Run("Linear, team Dydo, labels read 2026-11-20: the Severity group holds blocker, should and nit."),
            Layer: "Linear holds the three labels and says nothing of what a captain does with each.",
            Reason: "The three words decide what happens next, so they are defined before the steps that use them.")
    };

    public static readonly Claim Groups = new("A ticket that only groups parts gets no review of its own.", Me, [])
    {
        Spec = ["R6"],
        Truth = new(Evidence.Run(Rounds),
            Layer: "Linear nests tickets for any reason; whether a parent integrates anything is known to its captain.",
            Reason: "A grouping adds no behaviour for a reviewer to find wrong."),
        Transition = new(CaptainState.Waiting, CaptainState.Finishing, "the ticket only groups parts: no review")
    };

    public static readonly Claim Whole = new("Once its parts are wired, a parent is reviewed once: each part, the parts together, their harmony.", Me,
    [
        At(CaptainSlot.Rounds, 1,
            "Once the parts are wired, dispatch a Review ticket for the whole: each part, the parts together, their harmony. A ticket that only groups parts needs none.",
            () => [Groups])
    ])
    {
        Spec = ["R2", "R1"],
        Truth = new(Evidence.Run(Rounds),
            Layer: "Nothing dispatches a review when the last part returns.",
            Reason: "Early enough that nothing is built on a fault, late enough that the whole can be seen working."),
        Transition = new(CaptainState.Waiting, CaptainState.InReview, "the parts are wired: a Review of the whole is dispatched")
    };

    public static readonly Claim Record = new("A lone reviewer records its own round; a round of several seats is recorded by the officer that holds it.", Me,
    [
        At(CaptainSlot.Rounds, 2,
            "A lone reviewer records its own round; a round of several seats is yours to record, by [A round's record](resources/round.md).")
    ])
    {
        Spec = ["S8"],
        Truth = new(Evidence.Run(Rounds),
            Layer: "Each seat returns its findings to its dispatcher alone, so nobody else can put them together.",
            Reason: "One verdict per round, however many seats looked.")
    };

    public static readonly Claim Fix = new("A blocker that is broken, against the spec or a security hole goes to a Fix ticket holding one sub-ticket per blocker.", Me,
    [
        At(CaptainSlot.Rounds, 3,
            "Send a blocker that is broken, against the spec or a security hole to a Fix ticket, one sub-ticket per blocker inside it. (open: his call)")
    ])
    {
        Spec = ["R9", "S9"],
        Truth = new(Evidence.Run(Rounds + " Fix r3, DYD-1339, holds no sub-ticket, so one per blocker is the spec's rule and not yet the practice."),
            Layer: "A FAIL stops `dydo merge`; it opens no ticket for the fix.",
            Reason: "Each blocker can be closed, and seen closed, on its own."),
        Transition = new(CaptainState.InReview, CaptainState.Fixing, "a round returns a blocker: a Fix ticket")
    };

    public static readonly Claim Replan = new("A fundamental blocker or a new requirement goes to the officer above for a re-plan.", Me,
        [At(CaptainSlot.Rounds, 4, "Send a fundamental blocker or a new requirement to your officer, for a re-plan.")])
    {
        Spec = ["R9"],
        Truth = new(Evidence.Tool("delegate_task: a child's final result returns to the thread that started it."),
            Layer: "A Fix can only repair what the plan asked for; nothing below the officer can change the plan.",
            Reason: "Fixing toward a plan that is itself wrong builds more of the wrong thing."),
        Transition = new(CaptainState.InReview, CaptainState.Returned, "a blocker is fundamental or a new requirement: back to the officer")
    };

    public static readonly Claim OneMore = new("The officer running the rounds may order one more after a round with no new blocker.", Me, [])
    {
        Spec = ["R8"],
        Truth = new(Evidence.Run(Rounds),
            Layer: "Nothing counts rounds or closes the loop.",
            Reason: "A round that barely passed can be looked at again without anyone inventing a blocker.")
    };

    public static readonly Claim Clean = new("The first round with no new blocker ends the review rounds.", Me,
        [At(CaptainSlot.Rounds, 5, "Repeat until a round finds no new blocker; you may order one more.", () => [OneMore])])
    {
        Spec = ["R8"],
        Truth = new(Evidence.Run(Rounds),
            Layer: "`dydo merge` needs one PASS on the head and knows nothing of earlier rounds.",
            Reason: "The loop ends on evidence, with no cap that would let a known blocker through."),
        Transition = new(CaptainState.InReview, CaptainState.Finishing, "a round finds no new blocker")
    };

    public static readonly Claim Mine = new("A blocker found in a change a captain wrote goes to a fresh writer.", Me,
        [At(CaptainSlot.Mine, 1, "A blocker found in a change you wrote goes to a fresh writer.")])
    {
        Spec = ["R16", "R33"],
        Truth = new(Evidence.Tool("delegate_task: \"Start one child agent with only `task`. No parent history.\""),
            Layer: "Nothing keeps an agent from repairing its own work and calling it fixed.",
            Reason: "Whoever wrote the fault tends to defend the thinking that produced it.")
    };

    private static Line At(Enum slot, int order, string text, Func<Claim[]>? also = null) =>
        new(CaptainSkill.TicketCaptain, Actor.TicketCaptain, text, new(slot, order), also);
}
