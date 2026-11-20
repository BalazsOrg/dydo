namespace DynaDocs.SkillBuild;

/// <summary>
/// The ticket-captain prototype's Review kind resource (DYD-1372): a reviewer is the captain of its
/// Review ticket and owns the verdict (DYD-1295 32cc5599). The level matrix is the user's table,
/// verbatim (spec R34); the severities a reviewer assigns are defined here, where they are assigned.
/// </summary>
public static class CaptainReviewing
{
    private const string Toolset = "skills/roles/crew/reviewer/resources/toolset.md";

    private const string Table = """
        | | Read<br>local | Read<br>vs spec | Read<br>whole / gestalt | Run tests | Hypothesis<br>custom tests | Drive<br>the app | Formal<br>statechart, … |
        | --- | --- | --- | --- | --- | --- | --- | --- |
        | Leaf evidence check<br>a lane or hop, optional seat | due | if<br>drift only | – | due<br>its receipts, tagged | – | – | – |
        | Ticket review<br>a parent integrating its parts | due | due | if<br>its own area | due<br>tagged + feature; mutation on changed | if | if<br>user-facing | – |
        | Feature / Milestone<br>a subsystem is done | if | due | due | due<br>feature + suite | due<br>at the seams | due<br>its journeys | if<br>it has a state machine |
        | Inquisition<br>before landing, per slice | due<br>every slice | due | due | due<br>suite + --heavy + mutation | due<br>every hypothesis | due | due |

        **due** every time; **if** when warranted; **–** not at this level. Never re-run what the author ran the same way: a receipt for the same state counts. The security baseline is always on.
        """;

    private static readonly Enum[] Me = [Actor.TicketCaptain];

    public static readonly Claim One = new("A Review ticket is one candidate judged at the level its brief names, and its verdict is its holder's.", Me,
        [At(CaptainSlot.ReviewLead, 1, "Your ticket is a review: one candidate, judged at the level your brief names, and the verdict is yours.")])
    {
        Spec = ["R34", "S8"],
        Truth = new(Evidence.Code(Toolset),
            Layer: "The brief names the candidate and the level; that the verdict has one owner is said only here.",
            Reason: "A verdict nobody owns is a list of remarks that nobody acts on.")
    };

    public static readonly Claim NotMine = new("No agent judges work it implemented or fixed.", Me,
        [At(CaptainSlot.ReviewLead, 2, "Never judge work you implemented or fixed.")])
    {
        Spec = ["R33"],
        Truth = new(Evidence.Tool("t3_thread_send: \"A send to a finished child thread starts a fresh task\", so a writer's thread can be handed its own work to review."),
            Layer: "T3 will hand a review to any thread, the writer's included.",
            Reason: "Its own context hides from a writer exactly what a fresh reader would catch.")
    };

    public static readonly Claim Matrix = new("A reviewer works only the row of the level its brief names.", Me,
        [At(CaptainSlot.ReviewRow, 1, Table)])
    {
        Spec = ["R34", "R5"],
        Truth = new(Evidence.Code(Toolset),
            Layer: "Every tool in the row is there for the taking at every level; nothing meters review.",
            Reason: "Review is expensive, so each level spends only what it can find something with.")
    };

    public static readonly Claim Built = new("Drive and Formal are due only where the tool is built and the subject settled; a missing tool is reported, never built.", Me,
    [
        At(CaptainSlot.ReviewRules, 1,
            "Drive and Formal are due only where the tool is built and the subject is settled; report a tool the project lacks to your officer as a finding, instead of building it.")
    ])
    {
        Spec = ["R34", "R7"],
        Truth = new(Evidence.Code(Toolset),
            Layer: "The table's bare \"due\" cannot tell whether a project has the driving skill or the model it calls for.",
            Reason: "A reviewer that builds its own tooling has turned a review into a second project.")
    };

    public static readonly Claim Anchor = new("A review judges the change, and the behaviour it inherits unchanged, against the spec's problem statement and goals.", Me,
    [
        At(CaptainSlot.ReviewRules, 2,
            "Judge the change, and the behaviour it inherits unchanged, against the spec's problem statement and goals, not only the contract.")
    ])
    {
        Spec = ["R3"],
        Truth = new(Evidence.Run("This ticket exists for that reason: the candidate went through four review rounds (DYD-1281) and still failed the user's read (DYD-1295 519126ab)."),
            Layer: "A contract can pass on a wrong rule, and nothing below a reader of the spec would notice.",
            Reason: "Work can meet its contract and still drift from what the Project is for.")
    };

    public static readonly Claim Severities = new("A reviewer gives every finding one severity: blocker, should or nit.", Me,
    [
        At(CaptainSlot.ReviewRules, 3,
            "Give every finding one severity: a blocker, the intended case is wrong; a should, a real bug callers will hit, inside the intent; a nit, everything else.")
    ])
    {
        Spec = ["R11"],
        Truth = new(Evidence.Run("Linear, team Dydo, Severity labels read 2026-11-20, describe them so: \"the intended case is wrong\", \"a real bug callers will hit, inside the intent\", \"everything else\"."),
            Layer: "Linear holds the three labels; sorting a finding into one is the reviewer's judgement.",
            Reason: "The severity is what the captain routes on, so a finding without one cannot move.")
    };

    public static readonly Claim OnePass = new("A reviewer reports every visible blocker in one pass.", Me,
        [At(CaptainSlot.ReviewRules, 4, "Report every visible blocker in one pass.")])
    {
        Spec = ["R8"],
        Truth = new(Evidence.Run("Linear, DYD-1327 read 2026-11-20: review r1 returned 7 blockers, 12 shoulds and 9 nits together, closed by one Fix."),
            Layer: "Nothing stops a reviewer at the first blocker it finds.",
            Reason: "Blockers handed over one a round cost a fix and a round each.")
    };

    public static readonly Claim Lanes = new("A reviewer may run lanes through native helpers and puts their findings together itself.", Me,
        [At(CaptainSlot.ReviewRules, 5, "Run lanes through native helpers when your row calls for both reading and driving, and put their findings together yourself.")])
    {
        Spec = ["K30", "S1"],
        Truth = new(Evidence.Tool("Agent: a native sub-agent, awaited within the turn, returns its report to its caller alone."),
            Layer: "AGENTS.md allows the lanes; that their findings become one verdict is the holder's work.",
            Reason: "Reading and driving need different attention, and one verdict still has to answer for both.")
    };

    public static readonly Claim Alone = new("A lone reviewer holds its round whole and records it itself.", Me,
        [At(CaptainSlot.ReviewHolding, 1, "Holding the round alone: record it yourself, by [A round's record](round.md).")])
    {
        Spec = ["S8", "R17"],
        Truth = new(Evidence.Run("Linear, DYD-1281's sub-tickets read 2026-11-20: each round is one Review ticket."),
            Layer: "A lone reviewer's officer sees only one line back; the record has to be on the ticket already.",
            Reason: "The verdict is written by the one who formed it, with nothing lost in a relay.")
    };

    public static readonly Claim Seat = new("One seat among several returns its findings to the captain holding the round.", Me,
        [At(CaptainSlot.ReviewHolding, 2, "Sitting as one seat among several: return your findings, each with its severity, to the captain holding the round.")])
    {
        Spec = ["S8", "S1"],
        Truth = new(Evidence.Tool("delegate_task: a child's final result returns to the thread that started it."),
            Layer: "Each seat sees only its own findings, so none of them can write the round's verdict.",
            Reason: "Several seats posting each a verdict would leave the round with none.")
    };

    public static readonly Claim Resolution = new("A conflict's resolution is judged by its hunks, and its PASS names the new head.", Me,
        [At(CaptainSlot.ReviewHolding, 3, "Asked to judge a conflict's resolution: judge its hunks, the only new work on the branch, and name the new head in your PASS.")])
    {
        Spec = ["R19"],
        Truth = new(Evidence.Code("Services/Merge/PullRequestMerge.cs"),
            Layer: "`dydo merge` refuses a head no PASS names, and a resolution always makes a new head.",
            Reason: "Everything else on the branch already passed, so judging it again spends a round on nothing.")
    };

    private static Line At(Enum slot, int order, string text) =>
        new(CaptainSkill.TicketCaptain, Actor.TicketCaptain, text, new(slot, order));
}
