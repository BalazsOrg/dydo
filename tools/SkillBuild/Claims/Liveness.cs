namespace DynaDocs.SkillBuild;

/// <summary>Liveness (M12): a hop quiet for 2 hours, checked and healed by the officer above.</summary>
public static class Liveness
{
    private const string Alive = "Alive: comment `liveness <UTC time>: alive`, and the finding snoozes one window.";
    private const string Behind = "Record behind: comment `liveness <UTC time>: record behind` and bring the record up to date (Done, Fix rN, …).";
    private const string Stopped = "Stopped: comment `liveness <UTC time>: stopped`, set the hop back to Todo, then resume the thread or dispatch a fresh agent; the old thread link stays as history.";

    public static readonly Claim Quiet = new("liveness fires on a hop In Progress quiet for 2 hours, or a ticket In Progress with no open hop for 2 hours.", [Actor.Survey],
        [new(Skill.LinearWorkspaceStandard, "`liveness` fires on a hop In Progress and quiet for 2 hours, or a ticket In Progress with no open hop for 2 hours, counted from the later of its last change and its latest `liveness <UTC time>: <outcome>` comment.", new(Section.LinearStatuses, 3))])
    { Spec = ["S24", "S23"], Transition = new(LivenessState.Live, LivenessState.Quiet, "2 hours with no change and no liveness comment") };

    public static readonly Claim FoundAlive = Heal(1102, "An alive finding snoozes for one window.", Alive, LivenessState.Live, "alive: the finding snoozes one window");

    public static readonly Claim FoundBehind = Heal(1103, "A record behind is brought up to date.", Behind, LivenessState.Live, "the record is behind: brought up to date");

    // S24: the coordinator resumes the same thread once, and a second stop of the same hop gets a fresh agent.
    private const string CoordinatorStopped = "Stopped: comment `liveness <UTC time>: stopped` and set the hop back to Todo; resume the same agent's thread the first time, and dispatch a fresh agent when the same hop stops again; the old thread link stays as history.";

    public static readonly Claim FoundStopped = Heal(1104, "A stopped hop goes back to Todo and is resumed or restarted.", Stopped, LivenessState.Restarted, "stopped: the hop back to Todo", CoordinatorStopped);

    public static readonly Claim Resumed = new("The resumed or fresh agent attaches its thread link and works.", [Actor.TicketCaptain], [])
    { Spec = ["S24", "S13"], Transition = new(LivenessState.Restarted, LivenessState.Live, "the resumed or fresh agent attaches its thread link") { Fair = true } };

    // The officer above heals its own level: the captain its crew, the coordinator its captains, the chief of staff overnight only (H12).
    // Each finding's move is fair (S24: it "heals what it finds"; a stale item "gets picked up again").
    private static Claim Heal(int order, string rule, string text, LivenessState to, string arrow, string? coordinator = null) =>
        new(rule, [Actor.TicketCaptain, Actor.Coordinator, Actor.ChiefOfStaff],
        [
            new(Skill.TicketCaptain, text, new(LivenessState.Quiet, order, MoveKind.Exit)),
            new(Skill.Coordinator, coordinator ?? text, new(LivenessState.Quiet, order, MoveKind.Exit)),
            new(Skill.ChiefOfStaff, text, new(LivenessState.Quiet, order, MoveKind.Exit))
        ])
        { Spec = ["S24", "H12"], Transition = new(LivenessState.Quiet, to, arrow) { Fair = true } };
}
