namespace DynaDocs.SkillBuild;

/// <summary>
/// The ticket-captain prototype's "A need for the user" (DYD-1372 X13): how a need goes up, as one
/// ordered procedure of the steps a ticket's captain performs, for its own need and for one a child
/// passes up to it (spec H16 to H18, H22, L18, S10). The user kept this process when asked how a need
/// should reach him (<see cref="Kept"/>, quoted once, on the claim that frames the procedure). What a coordinator, a council or an envoy does is theirs,
/// and is named only as where the need goes. Two of its steps are <see cref="CaptainPassedUp"/>'s: the Question a child sends up unsettled, and its close on the way down.
/// </summary>
public static class CaptainNeed
{
    private const string Kept = " The user kept this process when asked how a need should reach him. Spec section 6 records it: \"answered: (a), the current process (H16; relayed by the chief of staff; " +
                                "open to reopening on sight)\". His words, \"Yeah, probably a.\", are in the chief of staff's project update 9099daa1 on the dydo 4.0.0 Project, 2026-11-21 " +
                                "(https://linear.app/bodnar-balazs/project/dydo-400-6ab0e0094779/activity#project-update-9099daa1).";

    private const string KeptToo = " The user kept this process: spec section 6, question 1, and project update 9099daa1, quoted in claim CaptainNeed.Never.";

    private const string Wayfinder = "`skills/productivity/wayfinder/SKILL.md`, read 2026-11-21: ";

    private const string Upward = "T3, 2026-11-21: this skill's fixer sent a note from inside its turn to its captain's thread with `t3_thread_send`, both threads in mode `auto`. " +
                                  "The send came back `delivery: started`, and the fixer's turn went on. `t3_thread_list` gives the address, a thread's `parentThreadId`, " +
                                  "which the header of its brief names too.";

    private const string Downward = "T3, 2026-11-21: three messages from its captain's thread reached this skill's writer inside its running turn (DYD-1372 phase 1, hop A). " +
                                    "t3_thread_send: `mode:'auto'` \"starts an idle thread, steers a running turn, or queues\", and " +
                                    "\"A send to a finished child thread starts a fresh task and returns its `taskId`.\"";

    private static readonly Enum[] Me = [Actor.TicketCaptain];

    public static readonly Claim Never = new("A captain never takes a need to the user itself: a need goes up through its officer, and only the coordinator sends an envoy to the user.", Me,
    [
        At(Actor.TicketCaptain, CaptainSlot.NeedLead, 1,
            "You never take a need to the user yourself: it goes up through your officer, and only the coordinator sends an envoy to the user.")
    ])
    {
        Spec = ["H16", "H18", "H23"],
        Truth = new(Evidence.Run(Upward + " `skills/roles/officers/coordinator/SKILL.md`, the coordinator's skill on this branch, not yet truth-checked: " +
                                 "\"Everything that needs him goes out through your own envoy\"." + Kept),
            Layer: "The host's question tool asks the user whatever an agent sends it, and nothing counts how many agents ask at once.",
            Reason: "His attention is the one thing the swarm cannot make more of, so a single queue spends it.")
    };

    public static readonly Claim Level = new("A need a child passes up is taken by the officer that receives it through the same steps as a need of its own.", Me,
        [At(Actor.TicketCaptain, CaptainSlot.NeedLead, 2, "Your own need, and one a child sends up to you, take these steps in order.")])
    {
        Spec = ["H16", "H12"],
        Truth = new(Evidence.Run(Wayfinder + "\"A need passed up opens at stage `research`, held by a fresh ticket captain who researches it first\"." + KeptToo),
            Layer: "A child's message lands in its officer's thread, and nothing there says what the officer owes it.",
            Reason: "An officer often holds what its child lacked, so the same question is cheaper to settle one level up than at the top.")
    };

    public static readonly Claim Here = new("A call that is obvious and reversible from where its holder stands is decided there, and gets no ticket.", Me,
        [At(Actor.TicketCaptain, CaptainSlot.NeedSteps, 1, "Decide a call yourself when, from where you stand, it is obvious and reversible; it gets no ticket.")])
    {
        Spec = ["H16"],
        Truth = new(Evidence.Run(Wayfinder + "\"Decide an obvious, reversible call where it arises; it never becomes a ticket (H16).\"" + KeptToo),
            Layer: "Linear takes a Question for any call, the plain ones included.",
            Reason: "A ticket and a wait for something its reader can see at a glance stops a child for nothing.")
    };

    public static readonly Claim Enable = new("What only the user can give, a token or an authorisation, is an Enablement sub-ticket.", Me, [])
    {
        Spec = ["H17"],
        Truth = new(Evidence.Run("Linear, team Dydo, labels read 2026-11-21: the Type group holds Enablement, \"Access, environment, credentials or material other work needs\", beside Question." + KeptToo),
            Layer: "Linear holds both labels and cannot tell a thing to be given from a call to be made.",
            Reason: "Research can settle a call and can never produce a token, so the two take different routes from the start.")
    };

    public static readonly Claim Carried = new("A need that already has its ticket goes up as that ticket, with its blocking relations: a HITL job sub-ticket its captain cut, " +
                                               "a Grilling or a Walkthrough among them, or the sub-ticket a child opened for its Enablement or its call only the user can make.", Me, [])
    {
        Spec = ["H18", "H15", "H23"],
        Truth = new(Evidence.Run("`skills/roles/officers/coordinator/SKILL.md`, the coordinator's skill on this branch, not yet truth-checked, queues the tickets themselves: " +
                                 "it parents \"its member Questions and Enablements\" under one wrapper, each carrying \"the blocking relations of the work that waits\", " +
                                 "and takes a Grilling among them: \"A need for him arrives on your HITL queue as its own HITL ticket\". Spec H23 has the envoy dispatched \"onto this same ticket\", with no second one opened." + KeptToo),
            Layer: "Linear takes a second ticket for the same token at every level it passes.",
            Reason: "One ticket keeps every block on it in a single place, where whoever ranks the user's queue can count them.")
    };

    public static readonly Claim Ask = new("A call that is not obvious and reversible becomes a Question sub-ticket that blocks only what waits on it.", Me,
    [
        At(Actor.TicketCaptain, CaptainSlot.NeedSteps, 2,
            "Only these needs keep the ticket they have, with its blocking relations: a Grilling, a Walkthrough or another HITL sub-ticket you cut, and the sub-ticket a child opened for its Enablement or for a call only the user can make. " +
            "Open a sub-ticket for any other need, by [Sub-tickets](sub-tickets.md), blocking only what waits on it, and go on with the rest: an Enablement for what only the user can give, a token or an authorisation; a Question for a call.",
            () => [Enable, Carried])
    ])
    {
        Spec = ["H16", "S10"],
        Truth = new(Evidence.Run("Linear, DYD-1372 read 2026-11-20: a ticket carries `blocks` and `blockedBy` relations beside its children." + KeptToo),
            Layer: "Linear draws a block once it is set, and stops nobody from working on.",
            Reason: "One open question holds up one part, never the whole ticket."),
        Transition = new(CaptainState.Leaf, CaptainState.Question, "a call that is not obvious: a Question sub-ticket")
    };

    public static readonly Claim Skip = new("A need already known to be the user's gets no research.", Me, [])
    {
        Spec = ["H17", "R9"],
        Truth = new(Evidence.Run("Linear, team Dydo, labels read 2026-11-21: Enablement closes \"on the condition true, with evidence\", which no source can supply." + KeptToo),
            Layer: "Nothing keeps a captain from dispatching a researcher onto a need that no source holds.",
            Reason: "A captain set to study what only he can give costs a day, and his answer is the slowest of all to arrive.")
    };

    public static readonly Claim Approval = new("A call that only wants the user's approval is never a need only the user can meet.", Me, [])
    {
        Spec = ["L18"],
        Truth = new(Evidence.Tool("AskUserQuestion: \"Use this tool only when you are blocked on a decision that is genuinely the user's to make\"." + KeptToo),
            Layer: "Nothing tells a call the user alone can make from one an agent would merely like confirmed.",
            Reason: "A rubber stamp costs him the time to understand the question and returns nothing the sources could not.")
    };

    public static readonly Claim Fresh = new("What the sources can answer never reaches the user: a Question is held by a fresh captain, dispatched with delegate_task, who researches it first.", Me,
    [
        At(Actor.TicketCaptain, CaptainSlot.NeedSteps, 4,
            "Dispatch a fresh captain onto a Question you opened, with `delegate_task`, to research it first; skip this when you already know that only the user can meet the need. " +
            "A call that only wants the user's approval is never such a need.",
            () => [Skip, Approval])
    ])
    {
        Spec = ["H16", "L18"],
        Truth = new(Evidence.Run("delegate_task: \"Start one child agent with only `task`. No parent history.\" " + Wayfinder +
                                 "\"A need passed up opens at stage `research`, held by a fresh ticket captain who researches it first\"." + KeptToo),
            Layer: "The host's question tool is one call away, and the sources that may hold the answer are many.",
            Reason: "A context full of its own work answers with a bias; a fresh one reads the sources before it says anything.")
    };

    public static readonly Claim Down = new("An answer to a need a child passed up is put on the child's sub-ticket and sent to the child's thread.", Me, [])
    {
        Spec = ["H16", "H12"],
        Truth = new(Evidence.Run(Downward + KeptToo),
            Layer: "T3 wakes a thread only when something is sent to it; a comment on a ticket wakes nobody.",
            Reason: "Without the send, a child that ended its turn on a need would wait on an answer that already exists.")
    };

    public static readonly Claim Answered = new("The blocked work resumes when the answer is on the Question ticket.", Me,
    [
        At(Actor.TicketCaptain, CaptainSlot.NeedSteps, 5,
            "When the answer is on the Question ticket, resume what waited; an answer to a child's need you put on the child's sub-ticket and send to the child's thread with `t3_thread_send`.",
            () => [Down])
    ])
    {
        Spec = ["H16"],
        Truth = new(Evidence.Run("Linear, team Dydo, labels read 2026-11-20: Question is described \"Resolution: the answer, posted on close\"." + KeptToo),
            Layer: "T3 wakes the captain when the Question's captain returns; the answer itself is on the ticket.",
            Reason: "The answer outlives both threads, for whoever meets the same call next."),
        Transition = new(CaptainState.Question, CaptainState.Leaf, "the answer is on the Question ticket")
    };

    public static readonly Claim AtOnce = new("A need only the user can meet goes to the officer at once.", Me, [])
    {
        Spec = ["R9", "S10"],
        Truth = new(Evidence.Run(Upward + KeptToo),
            Layer: "Nothing above a captain learns of a need until the captain sends it.",
            Reason: "His answer is the slowest route there is, so every hour before it is asked for lengthens the wait.")
    };

    public static readonly Claim Address = new("A holder finds its officer's thread as the parent of its own thread, which the thread list shows.", Me, [])
    {
        Spec = ["H12", "H16"],
        Truth = new(Evidence.Tool("t3_thread_list, called 2026-11-21 from the thread of this skill's third fixer: the result names `currentThreadId`, and that thread's row carries `parentThreadId`, " +
                                  "there its captain's thread, with `relationshipToParent` \"subagent\". t3_thread_send: `threadId` is required."),
            Layer: "T3 carries a child's last message back to its dispatcher unasked; a send from inside a turn needs the thread's id, and nothing hands a child that id.",
            Reason: "With a need and no address, an agent ends its turn on it and stops work that one answer from above would have kept moving.")
    };

    public static readonly Claim Up = new("A Question that comes back unsettled goes to the officer's thread with what was found and stays open: its holder still waits on the answer.", Me,
    [
        At(Actor.TicketCaptain, CaptainSlot.NeedSteps, 7,
            "Send what stays open, by its key and with what was found, to your officer's thread, which `t3_thread_list` shows as your own thread's parent, with `t3_thread_send`: " +
            "at once for a need only the user can meet, and for a Question when it comes back unsettled. Leave the sub-ticket open; what waited still waits on its answer.",
            () => [AtOnce, Address])
    ])
    {
        Spec = ["H16", "H12", "S10"],
        Truth = new(Evidence.Run(Upward + KeptToo),
            Layer: "A captain has no tool that opens a thread with the user, and its officer is the only one above it.",
            Reason: "Whether a call is worth his time is weighed once, above, instead of by every captain that meets one."),
        Transition = new(CaptainState.Question, CaptainState.Question, "the Question comes back unsettled: it goes to the officer and stays open, and the work still waits on its answer")
    };

    public static readonly Claim Refused = new("A send to the officer's thread is refused when the sender's mode is narrower than the officer's; the holder then ends its turn on the message, " +
                                               "and its work stops until the officer sends to its thread again.", Me,
    [
        At(Actor.TicketCaptain, CaptainSlot.NeedSteps, 8,
            "If that send is refused, because your thread's mode is narrower than your officer's, end your turn on the message instead; " +
            "your work then stops until your officer sends to your thread again.")
    ])
    {
        Spec = ["H12", "H23"],
        Truth = new(Evidence.Run("t3_thread_send: \"The target cannot run with broader modes than yours.\" The way round it ran in this ticket: each return of this skill's fixer, " +
                                 "the last message of its turn, reached its captain, whose next `t3_thread_send` to the finished thread started it again with its context, twice by 2026-11-21. " +
                                 "No thread here ran in a narrower mode than its officer, so the refusal itself is the schema's word."),
            Layer: "T3 refuses the send and names no other way up; a child's return is the one route that no mode closes.",
            Reason: "Without it a narrower child would sit on a need that nobody above ever hears of.")
    };

    public static readonly Claim InThread = new("When the user is already in an officer's thread, it may ask a one-line question there.", Me,
    [
        At(Actor.TicketCaptain, CaptainSlot.NeedRules, 1,
            "When the user is already in your thread, you may ask one line there instead, with the host's question tool: never what the sources can answer, and never a call that only wants the user's approval.")
    ])
    {
        Spec = ["H22", "L18"],
        Truth = new(Evidence.Tool("AskUserQuestion: \"Use this tool only when you are blocked on a decision that is genuinely the user's to make\"."),
            Layer: "The tool asks whenever it is called; whether the user is there to answer, only the thread shows.",
            Reason: "A ticket and a wait for something the user can answer in a word would be overkill.")
    };

    public static readonly Claim Cite = new("A question put to the user names what each decision record says, never only its number.", Me,
        [At(Actor.TicketCaptain, CaptainSlot.NeedRules, 2, "Say what a decision record decides whenever you cite one to the user, never only its number.")])
    {
        Spec = ["H24"],
        Truth = new(Evidence.Code("dydo/project/decisions"),
            Layer: "A record's number resolves to a file for an agent, and to nothing for a person reading a question.",
            Reason: "The user answers the question in front of him without looking anything up.")
    };

    private static Line At(Actor reader, Enum slot, int order, string text, Func<Claim[]>? also = null) =>
        new(CaptainSkill.TicketCaptain, reader, text, new(slot, order), also);
}
