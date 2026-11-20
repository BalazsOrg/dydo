namespace DynaDocs.SkillBuild;

/// <summary>A changed file over the 5k-token limit (M20): split, ignore path (a) for files nobody writes by hand, or path (b) through its own ticket, a council and his blessing.</summary>
public static class IgnoreEntry
{
    public static readonly Claim Split = new("A file over the token limit is split.", [Actor.Implementer],
        [new(Skill.Implementer, "Split a file you change that passes the token limit.", new(IgnoreEntryState.OverLimit, 1801, MoveKind.Exit))])
    { Spec = ["T15", "T16"], Transition = new(IgnoreEntryState.OverLimit, IgnoreEntryState.Split, "the writer splits the file") };

    public static readonly Claim Generated = new("Path (a): a PR adds an exact path or generated-directory glob, with a reason, for a file nobody writes by hand.", [Actor.Implementer],
        [new(Skill.Implementer, "For a file nobody writes by hand, add its exact path or generated-directory glob to the ignore file in your PR, with a reason.", new(IgnoreEntryState.OverLimit, 1802, MoveKind.Exit))])
    { Spec = ["T17", "T14"], Transition = new(IgnoreEntryState.OverLimit, IgnoreEntryState.PathA, "nobody writes it by hand") };

    public static readonly Claim CheckA = new("The reviewer checks each path (a) entry.", [Actor.Reviewer],
        [new(Skill.Reviewer, "Check each new ignore entry: an exact path or a generated-directory glob, for a file nobody writes by hand, with a reason.", new(IgnoreEntryState.PathA, 1803, MoveKind.Exit))])
    { Spec = ["T17"], Transition = new(IgnoreEntryState.PathA, IgnoreEntryState.Ignored, "the reviewer checks the entry") };

    public static readonly Claim Authored = new("Path (b): authored code goes through its own ticket, a council and his blessing; a PR can't add its own entry.", [Actor.TicketCaptain],
        [new(Skill.TicketCaptain, "An authored file that must pass the limit takes path (b): its own ticket, a council, then his blessing; its PR never adds its own entry.", new(IgnoreEntryState.OverLimit, 1804, MoveKind.Exit))])
    { Spec = ["T18", "T19"], Transition = new(IgnoreEntryState.OverLimit, IgnoreEntryState.OwnTicket, "authored code that must stay whole") };

    public static readonly Claim Convene = new("The path (b) ticket goes to a council.", [Actor.Coordinator], [])
    { Spec = ["T18"], Transition = new(IgnoreEntryState.OwnTicket, IgnoreEntryState.CouncilB, "the ticket goes to a council") };

    public static readonly Claim Bless = new("He blesses a path (b) entry.", [Actor.Balazs], [])
    { Spec = ["T18"], Transition = new(IgnoreEntryState.CouncilB, IgnoreEntryState.Ignored, "he blesses it") };

    public static readonly Claim GoStale = new("A stale ignore entry fails the gate.", [Actor.Dytest], [])
    { Spec = ["T17"], Transition = new(IgnoreEntryState.Ignored, IgnoreEntryState.Stale, "the entry goes stale") };
}
