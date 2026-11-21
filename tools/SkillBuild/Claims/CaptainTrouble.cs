namespace DynaDocs.SkillBuild;

/// <summary>
/// The ticket-captain prototype's scenarios that are not about a merge or a need for the user
/// (DYD-1372): a ticket too big to hold, and fix rounds with their checkpoint. Each says where the work
/// is saved, how it resumes, and where it goes when the captain cannot settle it (K9).
/// </summary>
public static class CaptainTrouble
{
    private const string OwnBrief = "This skill's writer was briefed (DYD-1372 hop 2): ";
    private const string Rounds = "Linear, DYD-1281's sub-tickets read 2026-11-20: Fix r1, r2 and r3, each followed by a Review titled a fresh review; r4 is titled the checkpoint round.";

    private const string Fixes = "Linear, DYD-1281's sub-tickets read 2026-11-21: Fix r1 (DYD-1327), Fix r2 (DYD-1333) and Fix r3 (DYD-1339) are one ticket each and hold no sub-ticket. ";

    private static readonly Enum[] Me = [Actor.TicketCaptain];

    public static readonly Claim Save = new("A captain whose leaf proves too big first commits what is whole and posts its SHA.", Me,
        [At(Actor.TicketCaptain, CaptainSlot.TooBigSteps, 1, "Commit what is whole, and post its SHA on your ticket.")])
    {
        Spec = ["K9", "H3"],
        Truth = new(Evidence.Run(OwnBrief + "\"commit each part when it is whole and green\"."),
            Layer: "An agent near the end of its context can stop mid-file, and nothing saves the work for it.",
            Reason: "Whoever goes on, the captain or a fresh agent, starts from proven work.")
    };

    public static readonly Claim SplitRest = new("A holder whose slice proves too big divides it itself, keeping coherence: what is left becomes sub-tickets.", Me,
        [At(Actor.TicketCaptain, CaptainSlot.TooBigSteps, 2, "Divide what is left yourself, into sub-tickets that each hold together and start from that SHA, and dispatch them.")])
    {
        Spec = ["H2", "K30"],
        Truth = new(Evidence.Code("AGENTS.md"),
            Layer: "AGENTS.md lets a holder split; that a leaf already begun may still be split is said only here.",
            Reason: "The alternative is an agent finishing badly in a context that is nearly spent."),
        Transition = new(CaptainState.Leaf, CaptainState.Split, "the leaf proves too big: what is left is cut")
    };

    public static readonly Claim ReCut = new("A holder that cannot divide its slice coherently under its own ticket returns it to its officer for a re-cut, naming what is done and what is left.", Me,
    [
        At(Actor.TicketCaptain, CaptainSlot.TooBigSteps, 3,
            "If what is left cannot be divided coherently under your ticket, for example because part of it belongs with a sibling's work, " +
            "return `too big, re-cut` to your officer, naming what is done and what is left. Your ticket then stays open as it stands: your officer settles it, and you do nothing more on it.",
            () => [Left!])
    ])
    {
        Spec = ["H2"],
        Truth = new(Evidence.Run(OwnBrief + "\"return `too big, re-cut` naming what is done and what is left\"."),
            Layer: "T3 returns whatever a child says last, and a child that simply stops says nothing its officer can act on.",
            Reason: "The officer can cut again from a known point without reading the whole thread."),
        Transition = new(CaptainState.Leaf, CaptainState.Returned, "`too big, re-cut`: what is left cannot be divided coherently under the ticket")
    };

    public static readonly Claim Left = new("A ticket returned too big, re-cut stays open as it stands until its officer settles it; its holder does nothing more on it.", Me, [])
    {
        Spec = ["H2", "S6"],
        Truth = new(Evidence.Tool("delegate_task: a child's final result returns to the thread that started it, and the child's task ends with it."),
            Layer: "Linear leaves the ticket In Progress after its holder's thread has ended, and says nothing of who moves it next.",
            Reason: "The child cannot close a ticket whose contract it has not met, and narrowing that contract was never its own to do.")
    };

    public static readonly Claim Yours = new("The ticket a child returned too big, re-cut is its officer's to settle, before the officer cuts what is left.", Me,
        [At(Actor.TicketCaptain, CaptainSlot.TooBigChild, 1, "When a child returns `too big, re-cut`, the cut is yours, and so is the child's ticket, in this order:")])
    {
        Spec = ["H2", "S15"],
        Truth = new(Evidence.Run("DYD-1372's contract, read 2026-11-21: its sections G and H were written from above, by the coordinator, and no writer below edited them. " +
                                 "Spec H2: \"a change to a unit's contract, or to its siblings', goes through the officer above\"."),
            Layer: "Linear lets any agent edit any ticket, and nothing says whose a returned ticket is once its holder's thread has ended.",
            Reason: "The officer cut the ticket and wrote its contract, so it alone can say what the ticket now is.")
    };

    public static readonly Claim Narrow = new("The officer rewrites the returned ticket's contract down to what the child finished at the SHA it returned, and notes the re-cut on the ticket.", Me,
    [
        At(Actor.TicketCaptain, CaptainSlot.TooBigOfficer, 1,
            "Settle the child's ticket on what it finished at the SHA it returned: rewrite its contract, in the ticket's description, down to that, and close the ticket Done on it; " +
            "if the child finished nothing whole, cancel it instead. Note the re-cut on the ticket in one line.",
            () => [Settle!])
    ])
    {
        Spec = ["S15", "H2"],
        Truth = new(Evidence.Tool("save_issue: `description`, the ticket's body, where spec S15 puts its contract; DYD-1372's description, read 2026-11-21, holds its own."),
            Layer: "Linear keeps the old contract on the ticket until someone rewrites it.",
            Reason: "A ticket closed against a contract it never met tells whoever reads it later that the whole part was built.")
    };

    public static readonly Claim Settle = new("The officer closes the returned ticket Done on the contract it now meets, or cancels it with a one-line reason when the child finished nothing whole.", Me, [])
    {
        Spec = ["S6", "S10"],
        Truth = new(Evidence.Run("Linear, team Dydo, statuses read 2026-11-21: Done and Canceled are both statuses, and DYD-1372 stayed In Progress through every return of its writers."),
            Layer: "Linear leaves a returned ticket In Progress, and its holder's thread has ended, so nobody is left to close it but its officer.",
            Reason: "A card In Progress with no holder reads as work under way, and the replacements finish beside a ticket that never does.")
    };

    public static readonly Claim ChildReCut = new("An officer whose child returns too big, re-cut cuts what the child left.", Me,
        [At(Actor.TicketCaptain, CaptainSlot.TooBigOfficer, 2, "Open sub-tickets for what it left, starting from that SHA.")])
    {
        Spec = ["H2", "H14"],
        Truth = new(Evidence.Tool("delegate_task: a child's final result returns to the thread that started it."),
            Layer: "The return lands in the officer's thread, and nothing re-opens the work.",
            Reason: "The officer knows how the part fits the whole, which the child did not."),
        Transition = new(CaptainState.Waiting, CaptainState.Split, "a child returns `too big, re-cut`: its ticket is settled on what it finished, and what it left is cut")
    };

    public static readonly Claim Contracts = new("A change to a unit's contract, or to a sibling's, goes through the officer above.", Me,
        [At(Actor.TicketCaptain, CaptainSlot.TooBigEnd, 1, "A change to your contract, or to a sibling's, is never part of your cut: take it to your officer.")])
    {
        Spec = ["H2", "S15"],
        Truth = new(Evidence.Run("DYD-1372's contract, read 2026-11-21: its section G, the phase-1 changes, was written from above, by the coordinator, and no writer below it edited the contract."),
            Layer: "Linear lets any agent edit any ticket's description, its own or a sibling's.",
            Reason: "A contract its holder may rewrite stops being a promise to the officer that cut it.")
    };

    public static readonly Claim FixOwner = new("A Fix goes to its writer's thread while that thread has room, and to a fresh agent otherwise.", Me,
    [
        At(Actor.TicketCaptain, CaptainSlot.FixRules, 3,
            "Give a Fix to its writer's thread, with `t3_thread_send`, while that thread is alive and has room in its context; dispatch a fresh agent for it when the context is heavy or a blocker says the approach was wrong.")
    ])
    {
        Spec = ["S9"],
        Truth = new(Evidence.Run("This skill's own Fix came so: its captain sent fix r0 to the thread that wrote hop 2. t3_thread_send: \"A send to a finished child thread starts a fresh task and returns its `taskId`.\""),
            Layer: "T3 will resume any thread, and cannot tell a useful context from a spent one.",
            Reason: "The writer already understands the code, until its context is what stands in the way.")
    };

    public static readonly Claim FixName = new("A Fix ticket is opened at verdict time, titled after its round, Fix rN, and lists every blocker it is to close.", Me,
    [
        At(Actor.TicketCaptain, CaptainSlot.FixRules, 1,
            "Open one Fix when its round's verdict is posted, unless you opened it ahead, titled after the round, Fix rN, and list on it every blocker it is to close.")
    ])
    {
        Spec = ["S9"],
        Truth = new(Evidence.Run("Linear, DYD-1281's sub-tickets read 2026-11-21: Fix r1, Fix r2 and Fix r3, each after the Review of its number, such as \"Fix r2: close review r2's 2 blockers and 5 shoulds\". " +
                                 "Fix r1, DYD-1327, was created at 04:06 on 2026-11-20, a minute before Review r1, DYD-1294, closed with its verdict, and its description lists that round's blockers by number, B1 to B7."),
            Layer: "Linear has no Type label for a Fix, so only its title says which round it closes.",
            Reason: "Anyone reading the board finds the round a Fix belongs to without opening it.")
    };

    public static readonly Claim Big = new("A blocker gets a sub-ticket of its own inside the Fix only when it is too big to fix with the rest.", Me,
        [At(Actor.TicketCaptain, CaptainSlot.FixRules, 2, "A blocker too big to fix with the rest gets a sub-ticket of its own inside the Fix; no other blocker gets one.")])
    {
        Spec = ["S9", "S7"],
        Truth = new(Evidence.Run(Fixes + "Fix r1 closed 7 blockers so, and no blocker has needed a ticket of its own yet. Spec S9 as amended: one Fix ticket \"listing all of them, with a sub-ticket only for a big one\" (DYD-1372 07c86aad, \"4. Lgtm\")."),
            Layer: "Linear takes a ticket for every blocker as readily as for none.",
            Reason: "A card per small repair buries the board, while a large one hidden in a list has no owner to answer for it.")
    };

    public static readonly Claim Parallel = new("A Fix is split only between parallel writers, as Fix rNa and Fix rNb.", Me,
        [At(Actor.TicketCaptain, CaptainSlot.FixRules, 4, "Split a Fix only between writers who work in parallel, as Fix rNa and Fix rNb.")])
    {
        Spec = ["S9"],
        Truth = new(Evidence.Run(Rounds + " Each Fix is one ticket. This skill's own round 1 was split between two writers in parallel, Fix r1a on the generator and Fix r1b on the skill: " +
                                 "T3 thread titles and `git log`, read 2026-11-21."),
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
        At(Actor.TicketCaptain, CaptainSlot.FixRules, 6,
            "Dispatch the next round once the Fix is Done, each blocker it lists closed and its proof run on the head: a fresh reviewer, every lens, and after a security hole the security lens first.",
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
        [At(Actor.TicketCaptain, CaptainSlot.FixRules, 7, "When your officer re-plans, cancel each sub-ticket the new plan drops, with a one-line reason.")])
    {
        Spec = ["S10"],
        Truth = new(Evidence.Run("Linear, team Dydo, statuses read 2026-11-20: Canceled exists."),
            Layer: "Linear leaves an abandoned ticket open for as long as nobody touches it.",
            Reason: "A dropped ticket left open looks like work someone still owes.")
    };

    public static readonly Claim Third = new("A third round that still has a blocker goes to the checkpoint before anything else.", Me, [])
    {
        Spec = ["R10"],
        Truth = new(Evidence.Run(Rounds),
            Layer: "Nothing counts rounds, so nothing would notice a loop that never ends.",
            Reason: "Three blocked rounds say something other than the code may be wrong, and a fourth should not start unasked."),
        Transition = new(CaptainState.InReview, CaptainState.Checkpoint, "a third round still has a blocker: the checkpoint") { Guard = "round = 3" }
    };

    public static readonly Claim Checkpoint = new("At the checkpoint the default is one more round, noted on the Review ticket.", Me,
    [
        At(Actor.TicketCaptain, CaptainSlot.FixCheckpoint, 1,
            "After a third round that still has a blocker, write `checkpoint: one more round, because …` on the Review ticket; its blockers then go to a Fix, as in any round.",
            () => [Third])
    ])
    {
        Spec = ["R10", "S10"],
        Truth = new(Evidence.Run(Rounds),
            Layer: "The Review ticket takes any comment, and nothing asks for this one.",
            Reason: "A written reason makes the captain look at why three rounds were not enough."),
        Transition = new(CaptainState.Checkpoint, CaptainState.Fixing, "`checkpoint: one more round, because …`: the blockers go to a Fix")
    };

    public static readonly Claim Escalate = new("At the checkpoint a captain escalates only for a fundamental break, a new requirement or a need for the user.", Me,
    [
        At(Actor.TicketCaptain, CaptainSlot.FixCheckpoint, 2,
            "Write `escalated: …` instead, and take it to your officer, only for a fundamental break, a new requirement or a need for the user.")
    ])
    {
        Spec = ["R10"],
        Truth = new(Evidence.Tool("delegate_task: a child's final result returns to the thread that started it."),
            Layer: "An officer hears nothing of a loop below it until the captain speaks.",
            Reason: "Most of the time the right answer is one more round, and an escalation stops work that could go on."),
        Transition = new(CaptainState.Checkpoint, CaptainState.Returned, "`escalated: …`: a fundamental break, a new requirement or a need for the user goes to the officer")
    };

    private static Line At(Actor reader, Enum slot, int order, string text, Func<Claim[]>? also = null) =>
        new(CaptainSkill.TicketCaptain, reader, text, new(slot, order), also);
}
