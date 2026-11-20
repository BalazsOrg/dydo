namespace DynaDocs.SkillBuild;

/// <summary>The tribunal (M15): a hard call to a neutral case file, two sealed judges, one exchange, and its exits by day and overnight.</summary>
public static class Tribunal
{
    public static readonly Claim Settled = new("A call a DR clearly settles is not hard: the holder decides and cites the DR.", [Actor.Coordinator],
        [new(Skill.Coordinator, "A call a DR clearly settles is not hard: decide it and cite the DR.", new(TribunalState.HardCall, 1401, MoveKind.Exit))])
    { Spec = ["L18"], Transition = new(TribunalState.HardCall, TribunalState.DrSettled, "a DR clearly settles it") };

    public static readonly Claim File = new("A hard call goes to a fresh clerk with a neutral case file; the coordinator is no member.", [Actor.Coordinator],
        [new(Skill.Coordinator, "Give a fresh clerk the Question with only the call, the options unranked, the constraints and the primary facts with sources; you sit on no tribunal.", new(TribunalState.HardCall, 1402, MoveKind.Exit))])
    {
        Spec = ["L18"],
        Transition = new(TribunalState.HardCall, TribunalState.CaseFile, "hard: a fresh clerk takes the Question")
            { Effect = "night' \\in BOOLEAN /\\ twoWay' \\in BOOLEAN /\\ drGrade' \\in BOOLEAN /\\ blocking' \\in BOOLEAN" }
    };

    public static readonly Claim Seat = new("The clerk seats two judges from different families from the roster.", [Actor.Reviewer], [])
    {
        Spec = ["L18", "R13", "R15"],
        Transition = new(TribunalState.CaseFile, TribunalState.Sealed, "two judges from different families take sealed positions") { Effect = "oneFamily' = FALSE" }
    };

    public static readonly Claim OneFamily = new("With one family out, Judge decides overnight and prepares by day on one family.", [Actor.Coordinator],
        [new(Skill.Coordinator, "With one family out, the tribunal still runs, on the remaining family: overnight it decides, by day it prepares and he decides.", new(TribunalState.CaseFile, 1410, MoveKind.Exit))])
    {
        Spec = ["L22"],
        Transition = new(TribunalState.CaseFile, TribunalState.Sealed, "one family out: the judges sit on the remaining family") { Effect = "oneFamily' = TRUE" }
    };

    public static readonly Claim Exchange = new("One exchange follows: each answers the other's strongest point or concedes.", [Actor.Reviewer], [])
    { Spec = ["L18"], Transition = new(TribunalState.Sealed, TribunalState.Exchange, "both positions in; one exchange") { Effect = "agree' \\in BOOLEAN" } };

    public static readonly Claim DecideNight = new("Overnight, judges agreeing on a two-way hard call decide it, provisionally.", [Actor.Coordinator],
        [new(Skill.Coordinator, "Act on an overnight decision, citing its law and precedent; it stays provisional and is never ratified.", new(TribunalState.DecidedOvernight, 1405, MoveKind.Arrival))])
    {
        Spec = ["L18", "L19", "L20"],
        Transition = new(TribunalState.Exchange, TribunalState.DecidedOvernight, "overnight, the judges agree on a two-way hard call") { Guard = "night /\\ agree /\\ twoWay /\\ ~drGrade" }
    };

    public static readonly Claim Prepare = new("By day, on disagreement, or for a one-way or DR-grade call, the tribunal prepares it for him.", [Actor.Coordinator],
        [new(Skill.Coordinator, "Send a prepared call to him through an envoy, with both positions; he decides.", new(TribunalState.Prepared, 1406, MoveKind.Arrival))])
    {
        Spec = ["L18", "L20", "L21"],
        Transition = new(TribunalState.Exchange, TribunalState.Prepared, "by day, or they disagree, or it is one-way or DR-grade")
            { Guard = "~(night /\\ agree /\\ twoWay) \\/ (drGrade /\\ ~blocking)" }
    };

    public static readonly Claim ProposeDr = new("Overnight, a two-way, seriously blocking DR-grade call both judges agree on becomes a proposed DR.", [Actor.Coordinator],
        [new(Skill.Coordinator, "Write the overnight ruling as a `proposed` DR for his ratification; an irreversible call is never decided.", new(TribunalState.ProposedDr, 1407, MoveKind.Arrival))])
    {
        Spec = ["L21"],
        Transition = new(TribunalState.Exchange, TribunalState.ProposedDr, "overnight, both agree on a two-way, seriously blocking DR-grade call")
            { Guard = "night /\\ agree /\\ twoWay /\\ drGrade /\\ blocking" }
    };

    public static readonly Claim Ratify = new("He ratifies an overnight proposed DR.", [Actor.Balazs], [])
    { Spec = ["L21"], Transition = new(TribunalState.ProposedDr, TribunalState.Ratified, "he ratifies it") };

    public static readonly Claim Score = new("His daytime answers are scored against the prediction, as evidence only.", [], [])
    { Spec = ["L20"], Transition = new(TribunalState.Prepared, TribunalState.Scored, "he answers; scored against the prediction") };

    public static readonly Claim Wrong = new("A ruling found wrong is investigated: why it was made, then a measure.", [], [])
    { Spec = ["L19"], Transition = new(TribunalState.DecidedOvernight, TribunalState.Investigated, "the ruling is found wrong") };

    public static readonly Claim Clerk = new("The clerk holds the Question, runs it mechanically, keeps the case file neutral and seats two judges.", [Actor.Reviewer],
        [new(Skill.Reviewer, "As the clerk, hold the Question and run it mechanically: keep the case file neutral (the call, the options unranked, the constraints, the primary facts with their sources) and seat two judges from different families from the roster.", new(Section.CouncilJudge, 1411))])
    { Spec = ["L18", "L23"] };

    public static readonly Claim Judge = new("Each judge alone maps options to axioms, finds precedent, names his choice and covers two duties.", [Actor.Reviewer],
        [new(Skill.Reviewer, "As a judge, alone: map each option to the axioms it serves, find precedent through `weighed:`, name what Balazs would choose and why, and answer both duties: should this be done at all, and is the frame too narrow?", new(Section.CouncilJudge, 1412))])
    { Spec = ["L18"] };

    public static readonly Claim Axioms = new("Axioms are not questioned.", [Actor.Reviewer],
        [new(Skill.Reviewer, "Take the axioms as given; never question them.", new(Section.CouncilJudge, 1413))])
    { Spec = ["L4"] };

    public static readonly Claim Rebuttal = new("One exchange: each judge answers the other's strongest point or concedes.", [Actor.Reviewer],
        [new(Skill.Reviewer, "Then one exchange: answer the other judge's strongest point, or concede.", new(Section.CouncilJudge, 1414))])
    { Spec = ["L18"] };

    public static readonly Claim ClerkReturn = new("The clerk returns both positions and the exchange to the coordinator.", [Actor.Reviewer],
        [new(Skill.Reviewer, "The clerk returns both positions and the exchange to the coordinator.", new(Section.CouncilJudge, 1415))])
    { Spec = ["L18"] };
}
