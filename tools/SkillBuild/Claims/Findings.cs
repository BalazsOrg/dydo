namespace DynaDocs.SkillBuild;

/// <summary>Survey, board and CI findings (the Findings machine): each S23 finding, CI reds and flakes, weeds and DYTEST needs, to their owners.</summary>
public static class Findings
{
    public static readonly Claim ReadBoard = Move(FindingsState.Watch, FindingsState.Board, Actor.Survey,
        "dydo survey board, read-only and on demand", "S22", "S26");
    public static readonly Claim SharedLink = Move(FindingsState.Board, FindingsState.SharedLink, Actor.Survey,
        "one thread link on two non-nested open tickets", "S23");
    public static readonly Claim LandedSpec = Move(FindingsState.Board, FindingsState.LandedSpec, Actor.Survey,
        "a landed Project still has a spec document", "S23");
    public static readonly Claim StaleShoulds = Move(FindingsState.Board, FindingsState.StaleShoulds, Actor.Survey,
        "open shoulds under Done parents past the threshold, 7 days by default", "S23", "T14");
    public static readonly Claim FixLink = Move(FindingsState.SharedLink, FindingsState.Acted, Actor.Admiral,
        "its officer fixes the record: one agent holds one ticket", "S7", "S13", "H5");
    public static readonly Claim DeleteSpec = Move(FindingsState.LandedSpec, FindingsState.Acted, Actor.Admiral,
        "the map holder moves its lasting parts and deletes it", "S14", "D11");
    public static readonly Claim BatchShoulds = Move(FindingsState.StaleShoulds, FindingsState.Acted, Actor.Admiral,
        "the admiral cuts a shoulds batch", "S17");
    public static readonly Claim RealRed = Move(FindingsState.Watch, FindingsState.CiRed, Actor.Ci,
        "a real CI red becomes a CI-findings ticket", "Y9", "R22");
    public static readonly Claim Route = Move(FindingsState.CiRed, FindingsState.Routed, Actor.Admiral,
        "to the open ticket's officer, or the admiral for a long-closed ticket", "H11");
    public static readonly Claim Pull = Move(FindingsState.Routed, FindingsState.Acted, Actor.TicketCaptain,
        "its officer takes it, or an officer with an open lane pulls it", "H8", "H10", "H11");
    public static readonly Claim Noise = Move(FindingsState.Watch, FindingsState.Flake, Actor.Dytest,
        "a flake or runner noise: a timeout, red then green on rerun", "Y9");
    public static readonly Claim Count = Move(FindingsState.Flake, FindingsState.Counted, Actor.Dytest,
        "it stays a DYTEST result with a count", "Y9");
    public static readonly Claim Repeat = Move(FindingsState.Flake, FindingsState.Repeated, Actor.Dytest,
        "it repeats", "Y9");
    public static readonly Claim Ticket = Move(FindingsState.Repeated, FindingsState.CiRed, Actor.Dytest,
        "a repeating flake becomes a CI-findings ticket", "Y9");
    public static readonly Claim Weed = Move(FindingsState.Watch, FindingsState.Weed, Actor.Admiral,
        "a mistake is found after landing", "R30");
    public static readonly Claim Rule = Move(FindingsState.Weed, FindingsState.Acted, Actor.Admiral,
        "it gets a lint, test or rule as well as its fix", "R30");
    public static readonly Claim Need = Move(FindingsState.Watch, FindingsState.DytestNeed, Actor.Admiral,
        "dydo needs something from DYTEST", "Y3");
    public static readonly Claim FileIt = Move(FindingsState.DytestNeed, FindingsState.Filed, Actor.Admiral,
        "filed and built in a DYTEST Project, never patched from dydo", "Y3", "Y1");

    private static Claim Move(FindingsState from, FindingsState to, Actor actor, string arrow, params string[] spec) =>
        new($"A finding: {arrow}.", [actor], []) { Spec = spec, Transition = new(from, to, arrow) };
}
