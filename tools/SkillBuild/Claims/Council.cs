namespace DynaDocs.SkillBuild;

/// <summary>
/// The council (M14): one question to sealed, read-only, cross-family seats, synthesised by a non-member,
/// on a Question ticket at stage <c>council</c> that a ticket captain chairs with its council-chair
/// resource (L23; DYD-1082 99774812, DYD-1195 ea16ea28). Nothing of it is the reviewer's.
/// </summary>
public static class Council
{
    private const string Chair = "[the council chair](resources/council-chair.md)";
    private const string Briefs = "[the council members' briefs](council-members.md)";

    // The research captain's exit to this child is its call's routing in the tribunal (Tribunal.File); the chair's arrival is the envoy machine's move.
    public static readonly Claim Hold = new("A ticket captain chairs a council with the council-chair resource (DYD-1082 99774812).", [Actor.TicketCaptain],
        [new(Skill.TicketCaptain, $"Holding a Question at stage `council`, chair it by {Chair}.", new(EnvoyState.Council, 122, MoveKind.Arrival))])
    { Spec = ["L23", "H23", "L18"], Transition = new(EnvoyState.Research, EnvoyState.Council, "a hard call not worth his time: a council child, AFK") };

    public static readonly Claim Ticket = new("A council is a Question ticket at stage council, Mode AFK, with no Council Type; its chair is no member (DYD-1195 ea16ea28).", [Actor.TicketCaptain],
        [new(Skill.TicketCaptain, "A council is a Question ticket at stage `council` and Mode AFK, with no Council Type; you chair it mechanically and sit on it as no member.", new(TicketCaptainSlot.Council, 1301))])
    { Spec = ["L23", "L18", "H23"] };

    public static readonly Claim FamilyOut = new("Merge confidence waits for both families only while nothing blocks on it.", [Actor.TicketCaptain], [])
    { Spec = ["L22"], Transition = new(CouncilState.Convened, CouncilState.Waiting, "Merge confidence with one family out") };

    public static readonly Claim Proceed = new("Once something blocks on it, Merge confidence runs single-family, never escalated to him.", [Actor.TicketCaptain], [])
    { Spec = ["L22"], Transition = new(CouncilState.Waiting, CouncilState.Answered, "something blocks: it runs on one family") };

    public static readonly Claim Answer = new("Every member answers the same question alone, sealed and read-only.", [], [])
    { Spec = ["L14", "L15", "L16"], Transition = new(CouncilState.Convened, CouncilState.Answered, "every member answers alone, sealed") };

    public static readonly Claim Synthesise = new("A non-member synthesises the answer, its confidence and the disagreements.", [], [])
    { Spec = ["L14"], Transition = new(CouncilState.Answered, CouncilState.Synthesised, "a non-member synthesises") };

    public static readonly Claim Uses = new("A council has four uses; the Evaluate use keeps the /10 score.", [Actor.TicketCaptain],
        [new(Skill.TicketCaptain, "It has four uses: Review (high stakes), Merge confidence (the landing into main), Evaluate (hill-climbs, scored out of 10) and Judge (the tribunal, below).", new(TicketCaptainSlot.Council, 1302))])
    { Spec = ["L16", "L23"] };

    public static readonly Claim Seating = new("The chair seats fresh members from different families from the roster, and a non-member synthesiser.", [Actor.TicketCaptain],
        [new(Skill.TicketCaptain, $"Seat fresh members from different families, picked from the roster, and a synthesiser who is no member, each briefed from {Briefs}.", new(TicketCaptainSlot.Council, 1303))])
    { Spec = ["L14", "L15", "R13", "R15"] };

    public static readonly Claim Once = new("Deterministic work runs once.", [Actor.TicketCaptain],
        [new(Skill.TicketCaptain, "Run deterministic work once for the whole council, never per member.", new(TicketCaptainSlot.Council, 1304))])
    { Spec = ["L16"] };

    public static readonly Claim Record = new("The council's deliberation is recorded on its ticket next to the answer, merge confidence and scores included (DYD-1195 ea16ea28).", [Actor.TicketCaptain],
        [new(Skill.TicketCaptain, "Record the deliberation on the council ticket next to its answer; merge confidence and a score are answers too.", new(TicketCaptainSlot.Council, 1305))])
    { Spec = ["L23"] };

    public static readonly Claim Seats = new("The verdict names each seat.", [Actor.TicketCaptain],
        [new(Skill.TicketCaptain, "The answer names each seat's model and provider.", new(TicketCaptainSlot.Council, 1306))])
    { Spec = ["S8", "L22"] };

    public static readonly Claim OneFamily = new("With one family out, work goes on: Review and Evaluate run on two models of the remaining family.", [Actor.TicketCaptain],
        [new(Skill.TicketCaptain, "With one family out, work goes on: Review and Evaluate run on two different models of the remaining family.", new(TicketCaptainSlot.Council, 1307))])
    { Spec = ["L22", "L15"] };

    public static readonly Claim MergeOneFamily = new("Merge confidence waits for both families only while nothing blocks, then runs on one, never escalated.", [Actor.TicketCaptain],
        [new(Skill.TicketCaptain, "With one family out, Merge confidence at the landing waits for both families only while nothing blocks on it, then runs on one; never take a missing family to him.", new(TicketCaptainSlot.Council, 1308))])
    { Spec = ["L22"] };

    public static readonly Claim MemberSlots = new("A member's brief names every skill the seat needs with a slash, like any dispatch.", [Actor.TicketCaptain],
        [new(Skill.TicketCaptain, "Skills: every skill the seat needs, each with a slash · the one question · model and effort · return to.", new(TicketCaptainSlot.Member, 1300))])
    { Spec = ["K14", "L14"] };

    public static readonly Claim SynthesiserSlots = new("The synthesiser's brief names every skill the seat needs with a slash, like any dispatch.", [Actor.TicketCaptain],
        [new(Skill.TicketCaptain, "Skills: every skill the seat needs, each with a slash · the members' sealed answers · model and effort · return to.", new(TicketCaptainSlot.Synthesiser, 1300))])
    { Spec = ["K14", "L14"] };

    public static readonly Claim Member = new("A member answers alone, sealed and read-only, on the same thing as every other member.", [Actor.TicketCaptain],
        [new(Skill.TicketCaptain, "Answer the one question alone, sealed and read-only; every member looks at the same thing.", new(TicketCaptainSlot.Member, 1309))])
    { Spec = ["L14", "L16"] };

    public static readonly Claim Synthesis = new("The synthesiser is a non-member and gives answer, confidence and disagreements.", [Actor.TicketCaptain],
        [new(Skill.TicketCaptain, "As a non-member, give the answer, its confidence (agreed, split or contested) and the disagreements.", new(TicketCaptainSlot.Synthesiser, 1310))])
    { Spec = ["L14"] };
}
