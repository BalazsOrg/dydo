namespace DynaDocs.SkillBuild;

/// <summary>
/// The ticket-captain prototype's Review kind resource (DYD-1372): what every Review holder reads, the
/// review's captain and each of its seats. It opens with the map of the spine and the rule that a
/// verdict is never reviewed (07c86aad), sorts a seat to its one kind (aa5b445e), and holds the user's
/// level table verbatim (spec R34) at the head of the part only the review's captain reads. The four
/// kinds are <see cref="CaptainReviewRead"/>, <see cref="CaptainReviewBreak"/>,
/// <see cref="CaptainReviewDrive"/> and <see cref="CaptainReviewModel"/>; the seating is
/// <see cref="CaptainReviewSeats"/>.
/// </summary>
public static class CaptainReviewing
{
    private const string Toolset = "skills/roles/crew/reviewer/resources/toolset.md";
    private const string Machine = "claims/statechart/captain/transitions.json";

    private const string Map = """
        | Spine phase | You hold the review | You are one seat of it |
        | -- | -- | -- |
        | 1. Take the ticket | use | use |
        | 2. Do it or split it | use: a leaf, or a split into seats | use: a leaf, unless your slice proves too big |
        | 3. Dispatch | skip, unless you send seats | skip, unless you divide a slice too big to hold |
        | 4. Get it reviewed | skip | skip |
        | 5. Finish | use: the Nobody case | use: the Nobody case |
        | When things go wrong | use | use |
        """;

    private const string Table = """
        Legend: **due** every time; **if** when warranted; **–** not at this level.

        | | Read<br>local | Read<br>vs spec | Read<br>whole / gestalt | Run tests | Hypothesis<br>custom tests | Drive<br>the app | Formal<br>statechart, … |
        | --- | --- | --- | --- | --- | --- | --- | --- |
        | Leaf evidence check<br>a lane or hop, optional seat | due | if<br>drift only | – | due<br>its receipts, tagged | – | – | – |
        | Ticket review<br>a parent integrating its parts | due | due | if<br>its own area | due<br>tagged + feature; mutation on changed | if | if<br>user-facing | – |
        | Feature / Milestone<br>a subsystem is done | if | due | due | due<br>feature + suite | due<br>at the seams | due<br>its journeys | if<br>it has a state machine |
        | Inquisition<br>before landing, per slice | due<br>every slice | due | due | due<br>suite + --heavy + mutation | due<br>every hypothesis | due | due |

        > Every row: never re-run what the author ran the same way; a test receipt for the same state counts (spec R5). The security baseline is always on.
        >
        > How it's used: the officer's brief names the level; the reviewer works its row.
        """;

    private static readonly Enum[] Me = [Actor.TicketCaptain];

    public static readonly Claim One = new("A Review ticket is one candidate judged at the level its brief names, and its verdict is its holder's.", Me,
    [
        At(Actor.TicketCaptain, CaptainSlot.ReviewLead, 1,
            "Your ticket is a review: one candidate, judged at the level your brief names. Hold the review, and its verdict is yours; sit as one seat of it, and your findings are.")
    ])
    {
        Spec = ["R34", "S8"],
        Truth = new(Evidence.Code(Toolset),
            Layer: "The brief names the candidate and the level; that the verdict has one owner is said only here.",
            Reason: "A verdict nobody owns is a list of remarks that nobody acts on.")
    };

    public static readonly Claim NotMine = new("No agent judges work it implemented or fixed.", Me,
        [At(Actor.TicketCaptain, CaptainSlot.ReviewLead, 2, "Never judge work you implemented or fixed.")])
    {
        Spec = ["R33"],
        Truth = new(Evidence.Tool("t3_thread_send: \"A send to a finished child thread starts a fresh task\", so a writer's thread can be handed its own work to review."),
            Layer: "T3 will hand a review to any thread, the writer's included.",
            Reason: "Its own context hides from a writer exactly what a fresh reader would catch.")
    };

    public static readonly Claim Phases = new("A Review holder uses the spine's first, second and last phases, dispatches only to send seats, and skips the review phase.", Me,
        [At(Actor.TicketCaptain, CaptainSlot.ReviewMap, 1, Map)])
    {
        Spec = ["K30", "R34"],
        Truth = new(Evidence.Code(Machine),
            Layer: "The spine is one text for every kind of ticket, and nothing in it marks the phases a kind leaves out.",
            Reason: "Without the map a reviewer works out for itself that the phase named for review is about somebody else's change.")
    };

    public static readonly Claim NoReview = new("A verdict is never reviewed, and neither are a seat's findings.", Me,
    [
        At(Actor.TicketCaptain, CaptainSlot.ReviewAll, 1,
            "A verdict is never reviewed: phase 4 is for a change, so no round is opened on your verdict or on a seat's findings, and what you return is your result.")
    ])
    {
        Spec = ["R34", "K30"],
        Truth = new(Evidence.Run("`" + Machine + "`: a leaf that is \"a verdict or an answer\" goes from Working it as a leaf to Finishing, and no arrow leads a verdict into In review. " +
                                 "Linear, DYD-1281's sub-tickets read 2026-11-20: Review r1 to r4 have no Review ticket beneath or beside them that judges them. " +
                                 "His words: \"Reviewing the reviews is probably not a good idea.\" (DYD-1372 777d396e)"),
            Layer: "The spine's fourth phase speaks to every captain alike and cannot tell a holder whose result is a judgement.",
            Reason: "A judgement sent on to be judged again has no end, and each further pass costs as much as the first.")
    };

    public static readonly Claim Anchor = new("A review judges the change, and the behaviour it inherits unchanged, against the spec's problem statement and goals.", Me,
    [
        At(Actor.TicketCaptain, CaptainSlot.ReviewAll, 2,
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
        At(Actor.TicketCaptain, CaptainSlot.ReviewAll, 3,
            "Give every finding one severity: a blocker, the intended case is wrong; a should, a real bug callers will hit, inside the intent; a nit, everything else.")
    ])
    {
        Spec = ["R11"],
        Truth = new(Evidence.Run("Linear, team Dydo, Severity labels read 2026-11-20, describe them so: \"the intended case is wrong\", \"a real bug callers will hit, inside the intent\", \"everything else\"."),
            Layer: "Linear holds the three labels; sorting a finding into one is the reviewer's judgement.",
            Reason: "The severity is what the captain routes on, so a finding without one cannot move.")
    };

    public static readonly Claim OnePass = new("A reviewer reports every visible blocker in one pass.", Me,
        [At(Actor.TicketCaptain, CaptainSlot.ReviewAll, 4, "Report every visible blocker in one pass.")])
    {
        Spec = ["R8"],
        Truth = new(Evidence.Run("Linear, DYD-1327 read 2026-11-20: review r1 returned 7 blockers, 12 shoulds and 9 nits together, closed by one Fix."),
            Layer: "Nothing stops a reviewer at the first blocker it finds.",
            Reason: "Blockers handed over one a round cost a fix and a round each.")
    };

    public static readonly Claim Seat = new("A seat returns its findings, each with its severity, to the captain holding the round.", Me, [])
    {
        Spec = ["S8", "S1"],
        Truth = new(Evidence.Tool("delegate_task: a child's final result returns to the thread that started it."),
            Layer: "Each seat sees only its own findings, so none of them can write the round's verdict.",
            Reason: "Several seats posting each a verdict would leave the round with none.")
    };

    public static readonly Claim SeatLeaf = new("A seat does one kind of review as a known leaf that stays one, and reads only its kind's part of the review resource.", Me,
    [
        At(Actor.TicketCaptain, CaptainSlot.ReviewSeat, 1,
            "Your brief names your one kind of review, and you are a leaf that stays one: read the part below headed by your kind, leave \"If you hold the review\" unread, " +
            "and return your findings, each with its severity, to the captain holding the round.",
            () => [Seat])
    ])
    {
        Spec = ["R34", "K30"],
        Truth = new(Evidence.Run("His words (DYD-1372 aa5b445e): \"Yes, they're leafs. And we know that they are. And they will stay that way.\" and, of a seat that drives the app, " +
                                 "\"Do they really have to know everything about what a \"ticket-captain\" does? They're not meant do delegate into further sub agents for sure.\" " +
                                 "T3 tells every child the same by default: \"do the work yourself. Start no subagents unless your task says to.\""),
            Layer: "The spine shows every holder how a ticket is split, and only its dispatcher knows that this one was cut to stay whole.",
            Reason: "A reviewer sent to do one thing spends its attention on that thing, and none on orchestration it will never use.")
    };

    public static readonly Claim Divide = new("A seat whose slice is too big for one agent divides it itself, keeping each part coherent, and returns it for a re-cut only when it cannot.", Me,
    [
        At(Actor.TicketCaptain, CaptainSlot.ReviewSeat, 2,
            "A leaf divides only what is too big for one agent, as a large read can be. Divide such a slice by [Too big to hold](too-big.md), your findings so far standing in for its commit " +
            "and each part a slice that reads whole on its own, and return the parts' findings with yours.")
    ])
    {
        Spec = ["H2", "K30"],
        Truth = new(Evidence.Run("His words (DYD-1372 aa5b445e): \"let's take the read review then it makes sense to keep the escape hatch, because if it's a too big of a slice " +
                                 "it can be further divided (while keeping the coherence).\" and \"So keeping the escape hatch is probably right.\" " +
                                 "`" + Machine + "` holds both ways out of a leaf that proves too big: cut what is left, or `too big, re-cut` back to the officer."),
            Layer: "A reviewer learns how large its slice really is only once it has opened it, long after the cut was made.",
            Reason: "A reader pushed past what it can hold skims, and a skimmed slice passes with its faults unread.")
    };

    public static readonly Claim Matrix = new("A reviewer works only the row of the level its brief names.", Me,
        [At(Actor.TicketCaptain, CaptainSlot.ReviewRow, 1, Table)])
    {
        Spec = ["R34", "R5"],
        Truth = new(Evidence.Run("The user's table, verbatim from spec R34 with its legend and two footer rules, as `" + Toolset + "` holds it today. " +
                                 "Three of its cells name what this repository cannot run yet; claim CaptainReviewing.Built names them and says what a reviewer does with such a cell."),
            Layer: "Every tool in the row is there for the taking at every level; nothing meters review.",
            Reason: "Review is expensive, so each level spends only what it can find something with.")
    };

    public static readonly Claim Built = new("A cell is due only as far as the project has what it names; what is missing is reported to the officer as a finding, never built.", Me,
    [
        At(Actor.TicketCaptain, CaptainSlot.ReviewRules, 2,
            "A cell is due only as far as the project has what it names: Drive and Formal where the tool is built and the subject is settled, any other cell where its tools, selectors and results exist. " +
            "Report what is missing to your officer as a finding, instead of building it.")
    ])
    {
        Spec = ["R34", "R7"],
        Truth = new(Evidence.Run("Read 2026-11-20: `dydo/guides/testing-strategy.md` says this repository's test facade \"has no `--feature`, `--plan`, `--heavy` or Ripple support\" and that mutation is " +
                                 "\"unavailable on every stack\"; no file under `DynaDocs.Tests/coverage` takes either flag, and no command of the CLI is DYTEST. So the cells \"tagged + feature\", " +
                                 "\"mutation on changed\" and \"suite + --heavy + mutation\" cannot run here as written. The table is the user's, approved verbatim, and he kept it whole " +
                                 "with this line below it (DYD-1372 aa5b445e, \"2. Okay\", on the walkthrough brief's recommendation)."),
            Layer: "The table's bare \"due\" cannot tell whether a project has the tool, the selector or the result a cell calls for.",
            Reason: "Without it a reviewer stalls on a cell nothing can run, or builds the tooling and turns its review into a second project.")
    };

    public static readonly Claim Resolution = new("A conflict's resolution is judged by its hunks, and its PASS names the new head.", Me,
        [At(Actor.TicketCaptain, CaptainSlot.ReviewRules, 8, "Asked to judge a conflict's resolution: judge its hunks, the only new work on the branch, and name the new head in your PASS.")])
    {
        Spec = ["R19"],
        Truth = new(Evidence.Code("Services/Merge/PullRequestMerge.cs"),
            Layer: "`dydo merge` refuses a head no PASS names, and a resolution always makes a new head.",
            Reason: "Everything else on the branch already passed, so judging it again spends a round on nothing.")
    };

    private static Line At(Actor reader, Enum slot, int order, string text, Func<Claim[]>? also = null) =>
        new(CaptainSkill.TicketCaptain, reader, text, new(slot, order), also);
}
