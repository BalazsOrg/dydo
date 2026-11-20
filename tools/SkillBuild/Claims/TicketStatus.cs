namespace DynaDocs.SkillBuild;

/// <summary>
/// Linear's record of one ticket (M3), as system arrows only: each rule's prose sits on the role
/// machine's claim that makes the move (Dispatch.Start, Project.Replanned, FeatureMerge.RealRed, …).
/// </summary>
public static class TicketStatus
{
    public static readonly Claim Open = new("A hop may be opened ahead in Todo.", [Actor.Linear], [])
    { Spec = ["S3"], Transition = new(TicketStatusState.New, TicketStatusState.Todo, "opened in Todo, ahead of its turn if a hop") };

    public static readonly Claim Future = new("An unscheduled strategic possibility enters as a FutureFeature (DYD-1082 555a8300 gap 3).", [Actor.Linear], [])
    { Spec = ["S5"], Transition = new(TicketStatusState.New, TicketStatusState.FutureFeature, "an unscheduled strategic possibility") };

    public static readonly Claim Promote = new("Only he promotes a FutureFeature out; uncontracted, it waits in Backlog (DYD-1082 555a8300 gaps 3–4).", [Actor.Balazs], [])
    { Spec = ["S5"], Transition = new(TicketStatusState.FutureFeature, TicketStatusState.Backlog, "he promotes it; it waits in Backlog until contracted") };

    public static readonly Claim Filed = new("A ticket filed without a contract is Backlog (DYD-1082 555a8300 gap 4).", [Actor.Linear], [])
    { Spec = ["S5"], Transition = new(TicketStatusState.New, TicketStatusState.Backlog, "filed without a contract") };

    public static readonly Claim Contract = new("A captain on a Wayfinding ticket contracts an uncontracted Backlog ticket to Todo (DYD-1082 1b6b6a5d Q16).", [Actor.TicketCaptain], [])
    { Spec = ["N12 DR 063 §4", "S5", "H5"], Transition = new(TicketStatusState.Backlog, TicketStatusState.Todo, "a Wayfinding captain contracts it") };

    public static readonly Claim Start = new("Whoever starts a step sets it In Progress with the holder's thread link.", [Actor.Linear], [])
    { Spec = ["S6", "S13", "S3"], Transition = new(TicketStatusState.Todo, TicketStatusState.InProgress, "whoever starts the step sets it In Progress") };

    public static readonly Claim Close = new("Whoever started a step closes it.", [Actor.Linear], [])
    { Spec = ["S6"], Transition = new(TicketStatusState.InProgress, TicketStatusState.Done, "whoever started it closes it") };

    public static readonly Claim Stopped = new("A stopped hop goes back to Todo.", [Actor.Linear], [])
    { Spec = ["S24"], Transition = new(TicketStatusState.InProgress, TicketStatusState.Todo, "liveness finds it stopped") };

    public static readonly Claim CancelDead = new("A re-plan cancels a dead hop with a one-line reason.", [Actor.Linear], [])
    { Spec = ["S10"], Transition = new(TicketStatusState.Todo, TicketStatusState.Canceled, "a re-plan cancels a dead hop") };

    public static readonly Claim CancelRunning = new("A re-plan or a revert cancels a running hop with its reason.", [Actor.Linear], [])
    { Spec = ["S10", "S12"], Transition = new(TicketStatusState.InProgress, TicketStatusState.Canceled, "a re-plan, or a reverted Merge") };

    public static readonly Claim MarkDuplicate = new("A duplicate should is marked Duplicate.", [Actor.Linear], [])
    { Spec = ["S17"], Transition = new(TicketStatusState.Todo, TicketStatusState.Duplicate, "a duplicate should") };
}
