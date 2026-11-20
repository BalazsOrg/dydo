namespace DynaDocs.SkillBuild;

/// <summary>
/// The tribunal (M15), the council chair's Judge procedure (L23): a Question's call after its research,
/// routed to him, to a council or settled by a DR (DYD-1195 87c1e2e6); a neutral case file, two sealed
/// judges, one exchange, and its exits, a declined proposed DR included (DYD-1082 555a8300 gap 8).
/// </summary>
public static class Tribunal
{
    private const string Briefs = "[the council members' briefs](council-members.md)";

    public static readonly Claim Settled = new("A call a DR clearly settles is not hard: the holder decides and cites the DR.", [Actor.TicketCaptain],
        [new(Skill.TicketCaptain, "A call a DR clearly settles is not hard: decide it and cite the DR.", new(TribunalState.HardCall, 1401, MoveKind.Exit))])
    { Spec = ["L18"], Transition = new(TribunalState.HardCall, TribunalState.DrSettled, "a DR clearly settles it") };

    public static readonly Claim HisCall = new("After its research, a call goes to him only when it is worth his time to understand, as a his-call child (DYD-1195 87c1e2e6).", [Actor.TicketCaptain],
        [new(Skill.TicketCaptain, "Open a `his-call` child only when the call is worth his time and attention to understand; a rubber-stamp question never reaches him.", new(TribunalState.HardCall, 1402, MoveKind.Exit))])
    { Spec = ["L18", "H23", "H16"], Transition = new(TribunalState.HardCall, TribunalState.Prepared, "worth his time to understand: a his-call child") };

    public static readonly Claim File = new("A hard call not worth his time goes to a council child, by day or night (DYD-1195 87c1e2e6).", [Actor.TicketCaptain],
        [new(Skill.TicketCaptain, "Otherwise open a `council` child for a hard call, by day or night: a DR and the spec disagree, nuance may change a DR or the spec, or there is no clear answer.", new(TribunalState.HardCall, 1403, MoveKind.Exit))])
    {
        Spec = ["L18", "L20", "H23"],
        Transition = new(TribunalState.HardCall, TribunalState.CaseFile, "a hard call not worth his time: a council child, by day or night")
            { Effect = "night' \\in BOOLEAN /\\ twoWay' \\in BOOLEAN /\\ drGrade' \\in BOOLEAN /\\ blocking' \\in BOOLEAN" }
    };

    public static readonly Claim DrGrade = new("A DR-grade call goes to him, except overnight, when a two-way, seriously blocking one goes to a council child.", [Actor.TicketCaptain],
        [new(Skill.TicketCaptain, "A `dr-grade` call goes to a `his-call` child, except overnight, when a two-way, seriously blocking one goes to a `council` child.", new(TribunalState.HardCall, 1404, MoveKind.Nominal))])
    { Spec = ["L21", "H23"] };

    public static readonly Claim Flag = new("A captain that meets a DR-grade call flags it dr-grade on its ticket and passes it up.", [Actor.TicketCaptain],
        [new(Skill.TicketCaptain, "Flag a DR-grade call on your ticket with the `dr-grade` label, then pass it up like any other need.", new(TicketCaptainSlot.NeedPassedUp, 1405))])
    { Spec = ["L21", "H16"] };

    public static readonly Claim Clerk = new("The chair runs the tribunal mechanically and keeps its case file neutral.", [Actor.TicketCaptain],
        [new(Skill.TicketCaptain, "Write the case file neutral: only the call, the options unranked, the constraints and the primary facts with their sources, never the research's tension mapping or chosen precedent.", new(TicketCaptainSlot.Tribunal, 1406))])
    { Spec = ["L18", "L23"] };

    public static readonly Claim Seat = new("The chair seats two judges from different families from the roster.", [Actor.TicketCaptain],
        [new(Skill.TicketCaptain, $"Seat two judges from different families, picked from the roster, each briefed as a judge from {Briefs}.", new(TribunalState.CaseFile, 1407, MoveKind.Exit))])
    {
        Spec = ["L18", "R13", "R15"],
        Transition = new(TribunalState.CaseFile, TribunalState.Sealed, "two judges from different families take sealed positions") { Effect = "oneFamily' = FALSE" }
    };

    public static readonly Claim OneFamily = new("With one family out, Judge runs on the remaining family: overnight it decides; by day it prepares a call worth his time, and a council decides one that is not.", [Actor.TicketCaptain],
        [new(Skill.TicketCaptain, "With one family out, seat both judges from the remaining family; the exits below hold as with two.", new(TribunalState.CaseFile, 1408, MoveKind.Exit))])
    {
        Spec = ["L22", "L18", "L20"],
        Transition = new(TribunalState.CaseFile, TribunalState.Sealed, "one family out: the judges sit on the remaining family") { Effect = "oneFamily' = TRUE" }
    };

    public static readonly Claim Exchange = new("One exchange follows: each answers the other's strongest point or concedes.", [], [])
    { Spec = ["L18"], Transition = new(TribunalState.Sealed, TribunalState.Exchange, "both positions in; one exchange") { Effect = "agree' \\in BOOLEAN" } };

    public static readonly Claim DecideNight = new("Judges agreeing on a two-way hard call decide it, provisionally, overnight or by day for a call not worth his time (DYD-1195 87c1e2e6).", [Actor.TicketCaptain],
        [new(Skill.TicketCaptain, "When both judges agree on a two-way hard call that is not DR-grade, it is decided: record it citing its law and precedent; it is acted on, provisional, and never ratified.", new(TribunalState.DecidedOvernight, 1409, MoveKind.Arrival))])
    {
        Spec = ["L18", "L19", "L20"],
        Transition = new(TribunalState.Exchange, TribunalState.DecidedOvernight, "the judges agree on a two-way hard call") { Guard = "agree /\\ twoWay /\\ ~drGrade" }
    };

    public static readonly Claim Prepare = new("On disagreement, or for a one-way or DR-grade call, the tribunal prepares it for him with both positions.", [Actor.TicketCaptain],
        [new(Skill.TicketCaptain, "When they disagree, the precedent points both ways, or the call is one-way, return it prepared, with both positions, to the Question that opened the council, which sends it to him as a `his-call` child; a DR-grade call too, except the overnight one below.", new(TribunalState.Prepared, 1410, MoveKind.Arrival))])
    {
        Spec = ["L18", "L20", "L21", "H23"],
        Transition = new(TribunalState.Exchange, TribunalState.Prepared, "they disagree, or it is one-way or DR-grade")
            { Guard = "~agree \\/ ~twoWay \\/ (drGrade /\\ ~(night /\\ blocking))" }
    };

    public static readonly Claim ProposeDr = new("Overnight, a two-way, seriously blocking DR-grade call both judges agree on becomes a proposed DR.", [Actor.TicketCaptain],
        [new(Skill.TicketCaptain, "Overnight, when both agree on a two-way, seriously blocking DR-grade call, write the ruling as a `proposed` DR for his ratification; an irreversible call is never decided.", new(TribunalState.ProposedDr, 1411, MoveKind.Arrival))])
    {
        Spec = ["L21"],
        Transition = new(TribunalState.Exchange, TribunalState.ProposedDr, "overnight, both agree on a two-way, seriously blocking DR-grade call")
            { Guard = "night /\\ agree /\\ twoWay /\\ drGrade /\\ blocking" }
    };

    public static readonly Claim Ratify = new("He ratifies an overnight proposed DR.", [Actor.Balazs], [])
    { Spec = ["L21"], Transition = new(TribunalState.ProposedDr, TribunalState.Ratified, "he ratifies it") };

    public static readonly Claim Decline = new(
        "A proposed DR he declines is deleted: his words go on the ticket that raised the question, which returns to its raiser (DYD-1082 1bc04dce, 555a8300 gap 8).",
        [Actor.Balazs, Actor.TicketCaptain],
        [new(Skill.TicketCaptain, "When he declines a proposed DR, delete it, never keeping it beside the accepted DRs: put his decline and his words on the ticket that raised the question, which returns to its raiser.", new(TribunalState.ProposedDr, 1412, MoveKind.Exit))])
    { Spec = ["L21"], Transition = new(TribunalState.ProposedDr, TribunalState.Declined, "he declines it: deleted, his words on the raising ticket") };

    public static readonly Claim Score = new("His daytime answers are scored against a council's prediction, as evidence only.", [], [])
    { Spec = ["L20"], Transition = new(TribunalState.Prepared, TribunalState.Scored, "he answers; any council prediction is scored, as evidence only") };

    public static readonly Claim Wrong = new("A ruling found wrong is investigated: why it was made, then a measure.", [], [])
    { Spec = ["L19"], Transition = new(TribunalState.DecidedOvernight, TribunalState.Investigated, "the ruling is found wrong") };

    public static readonly Claim JudgeSlots = new("A judge's brief names every skill the seat needs with a slash, like any dispatch.", [Actor.TicketCaptain],
        [new(Skill.TicketCaptain, "Skills: every skill the seat needs, each with a slash · the case file · model and effort · return to.", new(TicketCaptainSlot.Judge, 1400))])
    { Spec = ["K14", "L18"] };

    public static readonly Claim Judge = new("Each judge alone maps options to axioms, finds precedent, names his choice and covers two duties.", [Actor.TicketCaptain],
        [new(Skill.TicketCaptain, "Alone, map each option to the axioms it serves, find precedent through `weighed:`, name what Balazs would choose and why, and answer both duties: should this be done at all, and is the frame too narrow?", new(TicketCaptainSlot.Judge, 1413))])
    { Spec = ["L18"] };

    public static readonly Claim Axioms = new("Axioms are not questioned.", [Actor.TicketCaptain],
        [new(Skill.TicketCaptain, "Take the axioms as given; never question them.", new(TicketCaptainSlot.Judge, 1414))])
    { Spec = ["L4"] };

    public static readonly Claim Rebuttal = new("One exchange: each judge answers the other's strongest point or concedes.", [Actor.TicketCaptain],
        [new(Skill.TicketCaptain, "Then one exchange: answer the other judge's strongest point, or concede.", new(TicketCaptainSlot.Judge, 1415))])
    { Spec = ["L18"] };
}
