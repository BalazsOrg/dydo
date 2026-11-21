namespace DynaDocs.SkillBuild;

/// <summary>
/// The ticket-captain prototype (DYD-1372): its job paragraph, "Take the ticket", and the one rule on
/// a problem outside the ticket. Every claim of the prototype answers the four truth questions in its
/// fields (DYD-1295 1ffbc4c8): each line names its reader where it is written, the actor who acts on
/// it; the evidence names what was checked against the running system; the layer says why nothing
/// below the skill covers it; and the reason says what it prevents or enables.
/// </summary>
public static class Captain
{
    private const string Carrier = "tools/SkillBuild/Model4/Carriers/CaptainCarrier.cs";

    private static readonly Enum[] Me = [Actor.TicketCaptain];

    public static readonly Claim Owns = new("A captain holds one ticket to its end and answers for its outcome.", Me,
        [At(Actor.TicketCaptain, SpinePart.Job, 1, "You hold one ticket to its end, and you are responsible for its outcome.")])
    {
        Spec = ["S7", "H1"],
        Truth = new(Evidence.Code("AGENTS.md"),
            Layer: "AGENTS.md gives every agent the frame; only this skill speaks to the agent just dispatched onto a ticket.",
            Reason: "An agent that takes its ticket for a task to attempt stops at the first obstacle instead of at the result.")
    };

    public static readonly Claim Kind = new("A ticket's kind names the work, and the captain skill is how any kind is held.", Me,
        [At(Actor.TicketCaptain, SpinePart.Job, 2, "Its kind says what the work is; this skill says how to hold it.")])
    {
        Spec = ["K30"],
        Truth = new(Evidence.Code(Carrier),
            Layer: "The host loads one skill by name and knows nothing of kinds.",
            Reason: "A reader told at once that the base is shared looks for its own recipe in a resource, not here.")
    };

    public static readonly Claim Officer = new("A captain's officer is its dispatcher, and it is the officer of the captains it dispatches.", Me,
        [At(Actor.TicketCaptain, SpinePart.Job, 3, "Whoever dispatched you is your officer, and you are the officer of every captain you dispatch.")])
    {
        Spec = ["H12"],
        Truth = new(Evidence.Run("This skill's writer was a `delegate_task` child whose brief said: \"Your captain is the thread that dispatched you; return to it\" (DYD-1372 hop 2)."),
            Layer: "T3 carries a child's result back to the thread that started it, and names nobody to turn to with a problem.",
            Reason: "Every problem then has one address, up or down, and none is left without an owner.")
    };

    public static readonly Claim Calls = new("A captain decides only obvious, reversible calls; any other call is a Question sub-ticket a fresh captain answers.", Me,
    [
        At(Actor.TicketCaptain, SpinePart.Job, 4,
            "Decide only obvious, reversible calls; any other call becomes a Question sub-ticket for a fresh captain, and you wait on its answer.")
    ])
    {
        Spec = ["H16"],
        Truth = new(Evidence.Run("Linear, team Dydo, read 2026-11-20: the Type label Question exists, and a ticket takes sub-tickets (DYD-1281 holds twenty)."),
            Layer: "Nothing in the host or the guard stops an agent from settling a call that was never its own.",
            Reason: "A context full of its own work judges with a bias; a fresh one investigates first and then answers.")
    };

    public static readonly Claim Read = new("A captain reads its contract, how it fits the ticket above, and the spec sections it serves; the spec governs.", Me,
    [
        At(Actor.TicketCaptain, CaptainSlot.Take, 1,
            "Read the ticket's contract, how it fits the ticket above it, and the spec sections it serves; where they disagree, the spec wins.")
    ])
    {
        Spec = ["S15", "S16"],
        Truth = new(Evidence.Run("DYD-1372's contract, read 2026-11-20, holds all three: its outcome, a paragraph \"How this fits above\", and the spec rules it serves."),
            Layer: "Linear only stores the description; weighing it against the spec is the holder's.",
            Reason: "A holder that works from its contract alone drifts from the intent a little at every hop.")
    };

    public static readonly Claim FillerBack = new("A filler ticket that fails its captain's check goes back unstarted and unassigned, with a one-line reason.", Me, [])
    {
        Spec = ["H11"],
        Truth = new(Evidence.Code("Services/Survey/SurveyRules.cs"),
            Layer: "`dydo survey <project>` offers a ticket as pickable only while it is unstarted and unassigned, and nothing puts a rejected one back in that state.",
            Reason: "A rejected ticket left In Progress looks held, and no officer is ever offered it again."),
        Transition = new(CaptainState.Dispatched, CaptainState.Returned, "a filler ticket fails its check: it goes back unstarted and unassigned")
    };

    public static readonly Claim Filler = new("A captain on a filler ticket checks, before it starts it, that the ticket is small, reversible and free of design calls.", Me,
    [
        At(Actor.TicketCaptain, CaptainSlot.StartChecks, 3,
            "Your brief calls the ticket filler: if it is not small, reversible and free of design calls, return it to your officer unstarted and unassigned, with a one-line reason.",
            () => [FillerBack])
    ])
    {
        Spec = ["H11"],
        Truth = new(Evidence.Run("Linear, team Dydo, labels read 2026-11-20: the groups are Mode, Severity and Type, with nothing for small, reversible or free of design calls, so nobody upstream can have checked them."),
            Layer: "Whoever pulls filler sorts by labels alone; these three need someone who has opened the ticket.",
            Reason: "Work that hides a design call is caught before anyone builds it.")
    };

    public static readonly Claim Load = new("A captain loads its kind's resource and every skill its brief names.", Me, [])
    {
        Spec = ["K14"],
        Truth = new(Evidence.Run("This skill's writer was briefed to load `code-writer`, then `writing-for-agents`, and loaded both by name with the Skill tool."),
            Layer: "The host lists the skills it has and loads none for a kind of ticket.",
            Reason: "Naming them in the brief keeps the process from resting on an agent's guess at which ones apply.")
    };

    public static readonly Claim Start = new("Whoever starts a ticket sets it In Progress.", Me,
    [
        At(Actor.TicketCaptain, CaptainSlot.Take, 3, "Set the ticket In Progress, then load your kind's resource and every skill your brief names.", () => [Load])
    ])
    {
        Spec = ["S6"],
        Truth = new(Evidence.Run("Linear, team Dydo, statuses read 2026-11-20: In Progress is a started status and Todo the unstarted one, beside Done, Canceled and Duplicate; DYD-1372 was In Progress while this was written."),
            Layer: "T3's `link_issue` only shows the ticket beside its thread; it changes no status in Linear.",
            Reason: "The status is how every other agent sees that someone already has the ticket."),
        Transition = new(CaptainState.Dispatched, CaptainState.Taken, "the ticket is set In Progress and its kind's resource loaded: its result is a change, a verdict or an answer")
        {
            Effect = CaptainMachines.Learn
        }
    };

    public static readonly Claim Thread = new("Whoever sets a ticket In Progress attaches the holder's T3 thread link.", Me,
        [At(Actor.TicketCaptain, CaptainSlot.Take, 4, "Attach your thread's link to the ticket, titled `T3 · ticket-captain · <kind> · <model>`.")])
    {
        Spec = ["S13"],
        Truth = new(Evidence.Open("Spec S13 leaves it to verify whether Linear takes a T3 thread link as an attachment, and nobody has: " +
                                  "DYD-1372, DYD-1281 and DYD-1339, read 2026-11-20, carry none. T3's `link_issue` already shows a ticket beside its thread."),
            Layer: "T3 links ticket and thread on its own side only; from Linear, nobody can tell which thread has a ticket.",
            Reason: "A dead thread can be found and resumed from the ticket it was working.")
    };

    public static readonly Claim Kinds = new("Implement, Review and Research are the captain's kind resources.", Me,
    [
        At(Actor.TicketCaptain, CaptainSlot.Kinds, 1,
            "| Your ticket is | Load |\n| -- | -- |\n" +
            "| a change: an Implement, a Fix, a merge conflict | [Implement](resources/implement.md) |\n" +
            "| a Review | [Review](resources/review.md) |\n" +
            "| a question you are briefed to research | [Research](resources/research.md) |")
    ])
    {
        Spec = ["K30", "K31"],
        Truth = new(Evidence.Code(Carrier),
            Layer: "The build checks that each linked file exists; choosing among them is the reader's.",
            Reason: "Each kind carries only its own recipe, so no captain reads past two others to find it.")
    };

    public static readonly Claim Stage = new("A Question is held with the Research kind only when its brief sends its holder to research it; one whose brief names a job's skill is a job ticket, held with that skill.", Me, [])
    {
        Spec = ["H23", "S3"],
        Truth = new(Evidence.Run("Linear, team Dydo, labels read 2026-11-21: the Type group holds Question and Research, and no label names a Question's stage, " +
                                 "which spec H23 calls `research`, `council` and `his-call` and S3 leaves to the user's Enablement. So a Question's holder has its brief alone to tell which it holds. " +
                                 "`skills/` holds co-thinker, the skill H23 gives the Question the user settles."),
            Layer: "Linear shows that a ticket is a Question and nothing of how it is to be settled.",
            Reason: "A call the user alone can make, sent through the sources once more, costs a pass that cannot answer it and puts off his sitting.")
    };

    public static readonly Claim Jobs = new("A Grilling, Walkthrough, Wayfinding or Enablement ticket is held as any other, with its job's skill; its Mode says whether the user takes part.", Me,
    [
        At(Actor.TicketCaptain, CaptainSlot.Hold, 1,
            "A Grilling, Walkthrough, Wayfinding or Enablement ticket, or any other Question, is held the same way, with its job's skill from your brief; its Mode label says whether the user takes part.",
            () => [Stage])
    ])
    {
        Spec = ["H15", "H23"],
        Truth = new(Evidence.Run("`skills/` holds grilling, walkthrough, wayfinder and wizard; Linear's Mode group, read 2026-11-20, holds AFK and HITL."),
            Layer: "The brief names the job's skill, and nothing but this line says the ticket is still held like every other.",
            Reason: "One way of holding every kind keeps a job ticket from growing a process of its own.")
    };

    public static readonly Claim ReportUp = new("A captain reports a problem outside its ticket to its officer.", Me,
        [At(Actor.TicketCaptain, SpinePart.WhenThingsGoWrong, 3, "Report a problem outside your ticket to your officer.")])
    {
        Spec = ["R5"],
        Truth = new(Evidence.Tool("delegate_task: a child's final result returns to the thread that started it, which reads it with `task_status`."),
            Layer: "No tool routes a finding to its owner; the one address a captain surely has is its officer.",
            Reason: "What matters is that it surfaces, even when the finder cannot tell whose it is.")
    };

    private static Line At(Actor reader, Enum slot, int order, string text, Func<Claim[]>? also = null) =>
        new(CaptainSkill.TicketCaptain, reader, text, new(slot, order), also);
}
