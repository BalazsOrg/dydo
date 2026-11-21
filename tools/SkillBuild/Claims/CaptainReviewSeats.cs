namespace DynaDocs.SkillBuild;

/// <summary>
/// The ticket-captain prototype's Review kind, the part of the review's captain (DYD-1372 07c86aad,
/// aa5b445e; spec R34, S1, S8): it chooses the kinds of review its row calls for, does them itself or
/// seats them, sends a seat from another model family on a leaf sub-ticket of its own, and posts the
/// round's one verdict. Two of its claims are arrows of the captain's machine.
/// </summary>
public static class CaptainReviewSeats
{
    private const string Helper = "Agent: \"It knows only what you put in the prompt, and you see only the summary it sends back\".";

    private static readonly Enum[] Me = [Actor.TicketCaptain];

    public static readonly Claim Columns = new("The four kinds of review are the table's columns: reading its three Read columns, breaking Run tests and Hypothesis, driving Drive, and modelling Formal.", Me, [])
    {
        Spec = ["R34"],
        Truth = new(Evidence.Run("The user's table has seven columns, and he names four kinds (DYD-1372 07c86aad): \"some just reads, some tries to break it, some drives the feature like a user, " +
                                 "some models it as a statechart\". Spec R34 groups them the same way: \"Read, Break and Drive are reviewer lenses\"."),
            Layer: "The table heads its columns by tool and this page heads its parts by kind, and nothing else joins the two.",
            Reason: "A holder that cannot tell which part does which column runs a kind twice, or leaves a due cell undone.")
    };

    public static readonly Claim Pick = new("The review's captain works the row of the level its brief names: a due cell is owed, and an if cell is its call.", Me,
    [
        At(Actor.TicketCaptain, CaptainSlot.ReviewRules, 1,
            "Work the row of the level your brief names: every **due** cell is owed, and each **if** cell is your call. The kinds above are its columns: Reading, the three Read columns; " +
            "Trying to break it, Run tests and Hypothesis; Driving the feature, Drive; Modelling it as a statechart, Formal.",
            () => [Columns])
    ])
    {
        Spec = ["R34"],
        Truth = new(Evidence.Run("His words (DYD-1372 aa5b445e): \"let's say there is a captain for the review, which makes sense (it will decide what kind of a review matrix to use " +
                                 "and how to manage it's sub agents)\". The table's own footer leaves the level to the brief: \"the officer's brief names the level; the reviewer works its row.\""),
            Layer: "The table marks a cell \"if\" and leaves whether this candidate warrants it to whoever holds the review.",
            Reason: "One holder decides how much looking a candidate deserves, so no kind is run by habit and none is forgotten.")
    };

    public static readonly Claim Lanes = new("A review that fits its holder is a leaf: the holder does its kinds itself, or runs them as lanes through native helpers, and puts their findings together.", Me,
    [
        At(Actor.TicketCaptain, CaptainSlot.ReviewRules, 3,
            "A review that fits you is a leaf: do its kinds yourself, or run them as lanes through native helpers, and put their findings together yourself.")
    ])
    {
        Spec = ["K30", "S1"],
        Truth = new(Evidence.Tool("Agent: a native sub-agent, awaited within the turn, returns its report to its caller alone."),
            Layer: "AGENTS.md allows the lanes; that their findings become one verdict is the holder's work.",
            Reason: "Reading and driving need different attention, and one verdict still has to answer for both.")
    };

    public static readonly Claim Parallel = new("The review's captain runs the kinds it seats in parallel, one seat a kind.", Me, [])
    {
        Spec = ["R34"],
        Truth = new(Evidence.Tool("delegate_task: `mode`, default 'async', so a dispatcher runs several children at once and is woken as each returns."),
            Layer: "T3 runs children side by side when asked; that the kinds of one review need not wait on each other is the review captain's to see.",
            Reason: "The kinds look at one candidate from different sides, so none of them needs another's result to begin.")
    };

    public static readonly Claim Prefer = new("The review's captain sizes its round and fills its seats itself, preferring a model family other than the author's.", Me,
    [
        At(Actor.TicketCaptain, CaptainSlot.ReviewRules, 4,
            "Scale your round's seats to the breadth and depth of the work, with no fixed pair: seat the kinds you do not do yourself, one seat a kind, in parallel, " +
            "each from a model family other than the author's when you can, by phase 3 of the spine.",
            () => [CaptainDispatch.Scale, Parallel])
    ])
    {
        Spec = ["R34", "R15"],
        Truth = new(Evidence.Run("His words (DYD-1372 07c86aad): a review owned by a captain is \"probably better (since it will fill the two seats prefering a different model family to the one who wrote the code)\"."),
            Layer: "T3 seats whichever model it is asked for; the author's family reaches the review's captain only in its brief.",
            Reason: "A seat of the author's own family tends to pass the fault that family already let through once.")
    };

    public static readonly Claim Ticket = new("A seat from another model family is sent with delegate_task on a leaf sub-ticket of its own under the Review ticket.", Me,
    [
        At(Actor.TicketCaptain, CaptainSlot.ReviewRules, 5,
            "A seat from a model family other than your own goes out with `delegate_task`, on a leaf sub-ticket of its own under your Review ticket.")
    ])
    {
        Spec = ["S1", "R34", "R15"],
        Truth = new(Evidence.Tool("delegate_task: `target.model` and `target.providerInstanceId` seat a child on another provider's model; the native Agent tool runs only its caller's provider. " +
                                  "His words (DYD-1372 aa5b445e): \"by virtue of having to use the other model family it has to go through T3 delegate task which means that it's not a native sub agent. " +
                                  "And then therefore those agents get their own tickets.\""),
            Layer: "The host seats another family only through a dispatched child, and a dispatched child holds a ticket; nothing opens that ticket for the review's captain.",
            Reason: "A model of the author's own family tends to share the blind spot that let the fault through."),
        Transition = new(CaptainState.Taken, CaptainState.Split, "a review's seats come from another model family: each gets a leaf sub-ticket under the Review ticket")
        {
            Guard = CaptainMachines.Verdict
        }
    };

    public static readonly Claim Native = new("A native helper that runs a kind of review gets no ticket and loads no skill, so its prompt carries its kind's part.", Me, [])
    {
        Spec = ["S1", "K30"],
        Truth = new(Evidence.Tool(Helper),
            Layer: "The host starts a helper with its prompt alone: no ticket, no skill and nothing of its caller's reading.",
            Reason: "A helper told only to drive the app improvises the recipe its caller had open and never passed on.")
    };

    public static readonly Claim SeatBrief = new("A seat's brief adds its one kind of review and the cells of the row it works.", Me,
    [
        At(Actor.TicketCaptain, CaptainSlot.ReviewRules, 6,
            "Brief each seat by [Briefs](briefs.md), adding its one kind and the cells of your row it works; a native helper gets no ticket and loads no skill, so put its kind's part in its prompt.",
            () => [Native])
    ])
    {
        Spec = ["K14", "R34"],
        Truth = new(Evidence.Tool("delegate_task: \"Start one child agent with only `task`. No parent history.\""),
            Layer: "The briefs resource names the level and the lenses of a review, and has no word for which kind one seat of several is to do.",
            Reason: "A seat that is not told its kind reads the whole row and does a thin version of every column.")
    };

    public static readonly Claim Alone = new("The Review ticket's holder records its round itself, whether it reviewed alone or seated others.", Me, [])
    {
        Spec = ["S8", "R17"],
        Truth = new(Evidence.Run("Linear, DYD-1281's sub-tickets read 2026-11-20: each round is one Review ticket, and its verdict is on that ticket."),
            Layer: "A reviewer's officer sees only one line back; the record has to be on the ticket already.",
            Reason: "The verdict is written by the one who formed it, with nothing lost in a relay.")
    };

    public static readonly Claim Verdict = new("Once its seats are back, the review's captain puts their findings together into the round's one verdict, which names each seat.", Me,
    [
        At(Actor.TicketCaptain, CaptainSlot.ReviewRules, 7,
            "Post the round's one verdict yourself, by [A round's record](round.md): with seats, once they are back, from their findings put together, naming each seat. A seat posts none.",
            () => [Alone])
    ])
    {
        Spec = ["S8", "S1", "R34"],
        Truth = new(Evidence.Tool("delegate_task: a child's final result returns to the thread that started it, so every seat's findings meet only in the thread of the review's captain."),
            Layer: "Each seat's findings sit on its own sub-ticket, and nothing gathers them onto the Review ticket.",
            Reason: "The captain running the rounds routes on a single PASS or FAIL, and four partial answers give it nothing to route."),
        Transition = new(CaptainState.Waiting, CaptainState.Finishing, "a review's seats are back: their findings become the round's one verdict, which no round reviews")
        {
            Guard = CaptainMachines.Verdict
        }
    };

    private static Line At(Actor reader, Enum slot, int order, string text, Func<Claim[]>? also = null) =>
        new(CaptainSkill.TicketCaptain, reader, text, new(slot, order), also);
}
