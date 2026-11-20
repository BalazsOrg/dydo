namespace DynaDocs.SkillBuild;

/// <summary>
/// The ticket-captain prototype's "Dispatch" (DYD-1372): briefs, seats, sending, lanes and returns.
/// <c>delegate_task</c> is named here, where it is used (DYD-1295 2119cd65), and the reviewer-family
/// rule sits where a reviewer is seated (df6a785e).
/// </summary>
public static class CaptainDispatch
{
    private const string Task = "delegate_task: \"Start one child agent with only `task`. No parent history.\"";

    private static readonly Enum[] Me = [Actor.TicketCaptain];

    public static readonly Claim Brief = new("A child knows only its brief, which its dispatcher writes from the briefs resource.", Me,
        [At(Actor.TicketCaptain, CaptainSlot.Briefs, 1, "A child knows only what your brief says: write it by [Briefs](resources/briefs.md).")])
    {
        Spec = ["K14"],
        Truth = new(Evidence.Tool(Task),
            Layer: "T3 passes the brief on untouched and adds nothing from the dispatcher's own context.",
            Reason: "Whatever the brief leaves out, the child has to rediscover or guess.")
    };

    public static readonly Claim Family = new("A reviewer comes from a model family other than the author's when possible, single seats included.", Me, [])
    {
        Spec = ["R15"],
        Truth = new(Evidence.Tool("delegate_task: `target.model` and `target.providerInstanceId` seat a child on another provider's model."),
            Layer: "T3 seats whichever model it is asked for; which family wrote the work is known only to the dispatcher.",
            Reason: "Two models of one family tend to share the blind spot that let the fault through.")
    };

    public static readonly Claim Scale = new("A round's seats scale with the breadth and depth of the work; there is no constant pair.", Me,
    [
        At(Actor.TicketCaptain, CaptainSlot.Seats, 1,
            "Scale each round's seats to the breadth and depth of the work, with no fixed pair, and seat reviewers from a model family other than the author's when you can, single seats included.",
            () => [Family])
    ])
    {
        Spec = ["R12"],
        Truth = new(Evidence.Run("DYD-1281's four rounds, read 2026-11-20, are one Review ticket each; nothing in Linear or T3 sets a number of seats."),
            Layer: "No tool counts seats; how much review a piece of work deserves is its dispatcher's to size.",
            Reason: "Review is expensive, and a pair on everything spends it where it finds nothing.")
    };

    public static readonly Claim Roster = new("Each seat's model comes from the user's roster.", Me,
        [At(Actor.TicketCaptain, CaptainSlot.Seats, 2, "Pick each seat's model from the roster below.")])
    {
        Spec = ["R13"],
        Truth = new(Evidence.Open("The roster is the user's to write (R13), and it is not written: there is no text to state yet."),
            Layer: "`orchestrator_capabilities` lists the models a child can run on, and says nothing of which suits which seat.",
            Reason: "The strongest models are kept for the seats that have something non-obvious to find.")
    };

    public static readonly Claim Usage = new("A dispatcher balances vendors by their remaining usage for every seat it fills.", Me,
        [At(Actor.TicketCaptain, CaptainSlot.Seats, 3, "Read `dydo usage` and balance vendors across every seat you fill.")])
    {
        Spec = ["R14"],
        Truth = new(Evidence.Open("`dydo usage` is no command of the CLI: `Commands/DydoRootCommand.cs` registers none, and R14 calls the name provisional."),
            Layer: "No command or tool an agent has reports how much of a vendor account is left.",
            Reason: "Quota that resets unused is lost, and a seat on a spent account stalls.")
    };

    public static readonly Claim Send = new("A sub-ticket is dispatched with delegate_task.", Me,
        [At(Actor.TicketCaptain, CaptainSlot.Sending, 1, "Dispatch each sub-ticket with `delegate_task`.")])
    {
        Spec = ["H3"],
        Truth = new(Evidence.Tool(Task),
            Layer: "T3 offers a native sub-agent as well; only `delegate_task` gives the child a thread that outlives the dispatcher's turn.",
            Reason: "A captain needs a thread of its own, one its ticket can be resumed from."),
        Transition = new(CaptainState.Split, CaptainState.Waiting, "each sub-ticket is dispatched")
    };

    public static readonly Claim OwnWorktree = new("A child that writes in parallel or opens its own PR gets its own worktree; serial work shares the dispatcher's.", Me,
    [
        At(Actor.TicketCaptain, CaptainSlot.Sending, 2,
            "Give a child its own worktree (`isolation: \"worktree\"`) when it writes in parallel with others or will open its own PR; serial work shares yours.")
    ])
    {
        Spec = ["H3"],
        Truth = new(Evidence.Tool("delegate_task: `isolation`, 'shared' (default) the parent checkout, 'worktree' a \"new prepared checkout from the parent commit, for a separate checkout or PR per child\"."),
            Layer: "T3 makes the checkout, but only the dispatcher knows whether the child writes beside another.",
            Reason: "Keeps two writers out of one checkout.")
    };

    public static readonly Claim Lanes = new("An officer keeps its lanes full toward a soft target of four, its own sub-tickets first, then filler.", Me,
        [At(Actor.TicketCaptain, CaptainSlot.Lanes, 1, "Keep about four lanes full: your sub-tickets first, then filler, by [Filler](resources/filler.md).")])
    {
        Spec = ["H10"],
        Truth = new(Evidence.Tool("delegate_task: `mode`, default 'async', so a dispatcher runs several children at once and is woken as each returns."),
            Layer: "T3 runs as many children as it is given and fills no lane by itself.",
            Reason: "An idle lane is throughput lost; four is the user's goal to strive for, never a cap.")
    };

    public static readonly Claim Order = new("Filler is an officer's pickable side tickets first, then continuous tickets.", Me,
    [
        At(Actor.TicketCaptain, CaptainSlot.FillerLead, 1,
            "Filler fills a lane your own sub-tickets cannot: first the side tickets `dydo survey <project>` lists as pickable, then the team's continuous tickets, CI findings and Agent friction.")
    ])
    {
        Spec = ["H10", "H8"],
        Truth = new(Evidence.Command("dydo survey <project>"),
            Layer: "The survey lists what could be picked up and starts nothing.",
            Reason: "Work near the captain's own ticket is cheaper for it to hold than a stranger's.")
    };

    public static readonly Claim Pull = new("Filler is pulled unasked only when AFK, reversible and free of design calls, oldest first: a Bug or small Feature at should or lower, or an unrated CI finding.", Me,
    [
        At(Actor.TicketCaptain, CaptainSlot.FillerRules, 1,
            "Pull unasked only a ticket that is AFK, reversible and free of design calls, oldest first: a Bug or small Feature at should or lower, or an unrated CI finding.")
    ])
    {
        Spec = ["H11"],
        Truth = new(Evidence.Run("Linear, team Dydo, labels read 2026-11-20: Mode holds AFK, Type holds Bug and Feature, Severity holds should and nit."),
            Layer: "The labels sort what may be pulled; nothing stops an agent from pulling the rest.",
            Reason: "Anything riskier is the user's to hand out.")
    };

    public static readonly Claim Pulled = new("A continuous ticket an officer pulls follows that officer's chain.", Me,
        [At(Actor.TicketCaptain, CaptainSlot.FillerRules, 2, "A ticket you pull follows your chain from then on.")])
    {
        Spec = ["H8"],
        Truth = new(Evidence.Run("Linear, team Dydo, projects read 2026-11-20: \"CI findings\" and \"Agent friction\" exist as Projects that nobody holds."),
            Layer: "Linear groups those tickets under no officer, so a pulled one has an owner only if its puller takes it.",
            Reason: "Without it, a pulled ticket's questions and returns would have nowhere to go.")
    };

    public static readonly Claim Flag = new("A filler ticket's brief says so, and its captain checks what the labels cannot show.", Me,
        [At(Actor.TicketCaptain, CaptainSlot.FillerRules, 3, "Say in its brief that the ticket is filler, so its captain checks what no label shows.")])
    {
        Spec = ["H11"],
        Truth = new(Evidence.Run("Linear's labels, read 2026-11-20, carry Mode and Severity and nothing for small, reversible or free of design calls."),
            Layer: "A puller reads a ticket's labels, and its captain reads the ticket.",
            Reason: "The one check a label cannot make is made by the first agent that opens the work.")
    };

    public static readonly Claim Route = new("An out-of-scope report from a child goes to its owner's ticket or a new Bug.", Me, [])
    {
        Spec = ["R5"],
        Truth = new(Evidence.Run("Linear, team Dydo, labels read 2026-11-20: the Type group holds Bug."),
            Layer: "A child reports to its dispatcher only; reaching the owner is the dispatcher's.",
            Reason: "A report that stops at the captain is lost with the captain's thread.")
    };

    public static readonly Claim Resume = new("A child's returned SHA is posted on its ticket as the point to resume from.", Me,
    [
        At(Actor.TicketCaptain, CaptainSlot.Returns, 1,
            "Post the SHA a child returns on its ticket, to resume from, and send what it reports outside its scope to the owner's ticket or a new Bug.",
            () => [Route])
    ])
    {
        Spec = ["H3", "S6"],
        Truth = new(Evidence.Run("This skill's writer was briefed to return `IMPLEMENTED DYD-1372 hop 2 at <SHA>` to its captain, which holds the ticket's record."),
            Layer: "T3 returns the child's words to the dispatcher's thread, and that thread can die with what it knows.",
            Reason: "A fresh agent can pick the work up from the ticket alone.")
    };

    private static Line At(Actor reader, Enum slot, int order, string text, Func<Claim[]>? also = null) =>
        new(CaptainSkill.TicketCaptain, reader, text, new(slot, order), also);
}
