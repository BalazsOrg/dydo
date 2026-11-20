namespace DynaDocs.SkillBuild;

/// <summary>
/// The ticket-captain prototype's scenarios that are not about a merge (DYD-1372): a ticket too big to
/// hold, fix rounds and their checkpoint, and a need for the user. Each says where the work is saved,
/// how it resumes, and where it goes when the captain cannot settle it (K9).
/// </summary>
public static class CaptainTrouble
{
    private const string OwnBrief = "This skill's writer was briefed (DYD-1372 hop 2): ";
    private const string Rounds = "Linear, DYD-1281's sub-tickets read 2026-11-20: Fix r1, r2 and r3, each followed by a Review titled a fresh review; r4 is titled the checkpoint round.";

    private static readonly Enum[] Me = [Actor.TicketCaptain];

    public static readonly Claim Save = new("A captain whose leaf proves too big first commits what is whole and posts its SHA.", Me,
        [At(CaptainSlot.TooBigSteps, 1, "Commit what is whole, and post its SHA on your ticket.")])
    {
        Spec = ["K9", "H3"],
        Truth = new(Evidence.Run(OwnBrief + "\"commit each part when it is whole and green\"."),
            Layer: "An agent near the end of its context can stop mid-file, and nothing saves the work for it.",
            Reason: "Whoever goes on, the captain or a fresh agent, starts from proven work.")
    };

    public static readonly Claim SplitRest = new("A leaf that proves too big is split: what is left becomes sub-tickets.", Me,
        [At(CaptainSlot.TooBigSteps, 2, "Cut what is left into sub-tickets that start from that SHA, and dispatch them.")])
    {
        Spec = ["H13", "H14"],
        Truth = new(Evidence.Code("AGENTS.md"),
            Layer: "AGENTS.md lets a holder split; that a leaf already begun may still be split is said only here.",
            Reason: "The alternative is an agent finishing badly in a context that is nearly spent."),
        Transition = new(CaptainState.Leaf, CaptainState.Split, "the leaf proves too big: what is left is cut")
    };

    public static readonly Claim ReCut = new("A captain that cannot hold even the split returns too big, re-cut, naming what is done and what is left.", Me,
        [At(CaptainSlot.TooBigSteps, 3, "If you cannot hold the split either, return `too big, re-cut` to your officer, naming what is done and what is left.")])
    {
        Spec = ["H2"],
        Truth = new(Evidence.Run(OwnBrief + "\"return `too big, re-cut` naming what is done and what is left\"."),
            Layer: "T3 returns whatever a child says last, and a child that simply stops says nothing its officer can act on.",
            Reason: "The officer can cut again from a known point without reading the whole thread."),
        Transition = new(CaptainState.Leaf, CaptainState.Returned, "`too big, re-cut`: neither the work nor its split fits")
    };

    public static readonly Claim ChildReCut = new("An officer whose child returns too big, re-cut cuts what the child left.", Me,
        [At(CaptainSlot.TooBigChild, 1, "When a child returns `too big, re-cut`, the cut is yours: open sub-tickets for what it left, starting from the SHA it returned.")])
    {
        Spec = ["H2", "H14"],
        Truth = new(Evidence.Tool("delegate_task: a child's final result returns to the thread that started it."),
            Layer: "The return lands in the officer's thread, and nothing re-opens the work.",
            Reason: "The officer knows how the part fits the whole, which the child did not.")
    };

    public static readonly Claim FixOwner = new("A Fix goes to its writer's thread while that thread has room, and to a fresh agent otherwise.", Me,
    [
        At(CaptainSlot.FixRules, 1,
            "Give a Fix to its writer's thread while that thread is alive and has room in its context; give it to a fresh agent when the context is heavy or a blocker says the approach was wrong.")
    ])
    {
        Spec = ["S9"],
        Truth = new(Evidence.Tool("t3_thread_send: \"A send to a finished child thread starts a fresh task and returns its `taskId`.\""),
            Layer: "T3 will resume any thread, and cannot tell a useful context from a spent one.",
            Reason: "The writer already understands the code, until its context is what stands in the way.")
    };

    public static readonly Claim Parallel = new("A Fix is split only between parallel writers.", Me,
        [At(CaptainSlot.FixRules, 2, "Split a Fix only between writers who work in parallel.")])
    {
        Spec = ["S9"],
        Truth = new(Evidence.Run(Rounds + " Each Fix is one ticket."),
            Layer: "Nothing stops a captain from opening a Fix ticket for every finding.",
            Reason: "One writer closing all of a round's blockers understands the code once.")
    };

    public static readonly Claim Security = new("After a security hole, the next round leads with the security lens.", Me, [])
    {
        Spec = ["R9"],
        Truth = new(Evidence.Run(Rounds),
            Layer: "The next reviewer starts fresh and knows nothing of what the last round found.",
            Reason: "A hole that was missed once is the first thing to look for again.")
    };

    public static readonly Claim NextRound = new("The next round starts once every blocker of the Fix is Done and its proof ran on the head: a fresh look through every lens.", Me,
    [
        At(CaptainSlot.FixRules, 3,
            "Dispatch the next round once every blocker of the Fix is Done and its proof ran on the head: a fresh reviewer, every lens, and after a security hole the security lens first.",
            () => [Security])
    ])
    {
        Spec = ["R8", "S9"],
        Truth = new(Evidence.Run(Rounds),
            Layer: "`dydo merge` asks for a PASS on the head and cannot tell a fresh look from a glance at the diff.",
            Reason: "A fix can break what the last round passed, so the next one looks at everything again."),
        Transition = new(CaptainState.Fixing, CaptainState.InReview, "the Fix is Done: a fresh round") { Effect = "round' = round + 1" }
    };

    public static readonly Claim Dead = new("On a re-plan, dead sub-tickets go Canceled with a one-line reason.", Me,
        [At(CaptainSlot.FixRules, 4, "When your officer re-plans, cancel each sub-ticket the new plan drops, with a one-line reason.")])
    {
        Spec = ["S10"],
        Truth = new(Evidence.Run("Linear, team Dydo, statuses read 2026-11-20: Canceled exists."),
            Layer: "Linear leaves an abandoned ticket open for as long as nobody touches it.",
            Reason: "A dropped ticket left open looks like work someone still owes.")
    };

    public static readonly Claim Checkpoint = new("After round 3 with a blocker comes a checkpoint: by default one more round, noted on the Review ticket.", Me,
        [At(CaptainSlot.FixCheckpoint, 1, "After a third round that still has a blocker, write `checkpoint: one more round, because …` on the Review ticket, and run it.")])
    {
        Spec = ["R10", "S10"],
        Truth = new(Evidence.Run(Rounds),
            Layer: "Nothing counts rounds, so nothing would notice a loop that never ends.",
            Reason: "A written reason makes the captain look at why three rounds were not enough.")
    };

    public static readonly Claim Escalate = new("At the checkpoint a captain escalates only for a fundamental break, a new requirement or a need for the user.", Me,
    [
        At(CaptainSlot.FixCheckpoint, 2,
            "Write `escalated: …` instead, and take it to your officer, only for a fundamental break, a new requirement or a need for the user.")
    ])
    {
        Spec = ["R10"],
        Truth = new(Evidence.Tool("delegate_task: a child's final result returns to the thread that started it."),
            Layer: "An officer hears nothing of a loop below it until the captain speaks.",
            Reason: "Most of the time the right answer is one more round, and an escalation stops work that could go on.")
    };

    public static readonly Claim Sources = new("What research can answer never reaches the user.", Me,
        [At(CaptainSlot.NeedLead, 1, "Most calls never reach the user: whatever the sources can answer is a Question sub-ticket.")])
    {
        Spec = ["H16"],
        Truth = new(Evidence.Run("Linear, team Dydo, labels read 2026-11-20: the Type group holds Question."),
            Layer: "The host's question tool asks the user anything an agent cares to send.",
            Reason: "The user's attention is the one thing the swarm cannot make more of.")
    };

    public static readonly Claim Ask = new("A call that is not obvious and reversible becomes a Question sub-ticket that blocks only what waits on it.", Me,
        [At(CaptainSlot.NeedRules, 1, "Open the Question sub-ticket, block only what waits on it, and go on with the rest.")])
    {
        Spec = ["H16", "S10"],
        Truth = new(Evidence.Run("Linear, DYD-1372 read 2026-11-20: a ticket carries `blocks` and `blockedBy` relations beside its children."),
            Layer: "Linear draws a block once it is set, and stops nobody from working on.",
            Reason: "One open question holds up one part, never the whole ticket."),
        Transition = new(CaptainState.Leaf, CaptainState.Question, "a call that is not obvious: a Question sub-ticket")
    };

    public static readonly Claim Answered = new("The blocked work resumes when the answer is on the Question ticket.", Me,
        [At(CaptainSlot.NeedRules, 2, "Resume what waited once the answer is on the Question ticket.")])
    {
        Spec = ["H16"],
        Truth = new(Evidence.Run("Linear, team Dydo, labels read 2026-11-20: Question is described \"Resolution: the answer, posted on close\"."),
            Layer: "T3 wakes the captain when the Question's captain returns; the answer itself is on the ticket.",
            Reason: "The answer outlives both threads, for whoever meets the same call next."),
        Transition = new(CaptainState.Question, CaptainState.Leaf, "the answer is on the Question ticket")
    };

    public static readonly Claim Up = new("A need only the user can settle is passed up to the officer.", Me,
        [At(CaptainSlot.NeedRules, 3, "When the answer is that only the user can settle it, pass the need to your officer and leave the Question open.")])
    {
        Spec = ["H16", "H12"],
        Truth = new(Evidence.Tool("delegate_task: a child's final result returns to the thread that started it."),
            Layer: "A captain has no tool that opens a thread with the user.",
            Reason: "Needs reach the user one at a time, from one place, instead of from every captain at once.")
    };

    public static readonly Claim InThread = new("When the user is already in an officer's thread, it may ask a one-line question there.", Me,
        [At(CaptainSlot.NeedRules, 4, "When the user is already in your thread, ask the one line there, with the host's question tool.")])
    {
        Spec = ["H22"],
        Truth = new(Evidence.Tool("AskUserQuestion: \"Use this tool only when you are blocked on a decision that is genuinely the user's to make\"."),
            Layer: "The tool asks whenever it is called; whether the user is there to answer, only the thread shows.",
            Reason: "A ticket and a wait for something the user can answer in a word would be overkill.")
    };

    public static readonly Claim Cite = new("A question put to the user names what each decision record says, never only its number.", Me,
        [At(CaptainSlot.NeedRules, 5, "Say what a decision record decides whenever you cite one to the user, never only its number.")])
    {
        Spec = ["H24"],
        Truth = new(Evidence.Code("dydo/project/decisions"),
            Layer: "A record's number resolves to a file for an agent, and to nothing for a person reading a question.",
            Reason: "The user answers the question in front of him without looking anything up.")
    };

    private static Line At(Enum slot, int order, string text, Func<Claim[]>? also = null) =>
        new(CaptainSkill.TicketCaptain, Actor.TicketCaptain, text, new(slot, order), also);
}
