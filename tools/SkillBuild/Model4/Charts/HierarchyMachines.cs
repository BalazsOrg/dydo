namespace DynaDocs.SkillBuild;

/// <summary>
/// The 4.0.0 hierarchy machines (K1): crew dispatch and return, a need for Balazs and its envoy,
/// the chief of staff by day and overnight, liveness, and a should from finding to closed.
/// </summary>
public static class HierarchyMachines
{
    public static readonly Machine Dispatch = new(MachineId.Dispatch, "Crew dispatch and return",
    [
        new(DispatchState.Staffing, "Staffing a seat", "R12", "R13", "R14", "R15", "K14"),
        new(DispatchState.Briefed, "Briefed", "H3", "K14"),
        new(DispatchState.Working, "Working the hop", "H1", "R5"),
        new(DispatchState.ReCut, "Too big: re-cut", "H2"),
        new(DispatchState.Returned, "Returned", "K14", "R5"),
        new(DispatchState.CouncilCalled, "A council instead of one seat", "L16", "L17"),
        new(DispatchState.SelfAuthored, "Authored by the captain", "R16")
    ], DispatchState.Staffing, [DispatchState.Returned, DispatchState.CouncilCalled, DispatchState.SelfAuthored]);

    public static readonly Machine Envoy = new(MachineId.Envoy, "A need for Balazs",
    [
        new(EnvoyState.Need, "A need for Balazs", "H16"),
        new(EnvoyState.Homework, "Homework first", "H16"),
        new(EnvoyState.PassedUp, "Passed up the chain", "H16", "H12"),
        new(EnvoyState.Decided, "Decided by the officer", "H16", "L18"),
        new(EnvoyState.AskedInThread, "Asked in one line, in his thread", "H22", "H6"),
        new(EnvoyState.ToTribunal, "Sent to the tribunal", "H5", "L18"),
        new(EnvoyState.Queued, "Queued behind the active envoy", "H15", "H18"),
        new(EnvoyState.Active, "Envoy out", "H15", "H19", "H23"),
        new(EnvoyState.Reported, "Envoy reported back", "H15", "H19"),
        new(EnvoyState.Closed, "Envoy thread closed", "H18", "H19"),
        new(EnvoyState.MovedOn, "He has moved on, the thread left open", "H18")
    ], EnvoyState.Need, [EnvoyState.Decided, EnvoyState.AskedInThread, EnvoyState.ToTribunal, EnvoyState.Closed, EnvoyState.MovedOn])
    {
        Variables =
        [
            new("level", "\"captain\"", "level \\in {\"captain\", \"admiral\"}"),
            new("envoys", "0", "envoys \\in {0, 1}")
        ]
    };

    public static readonly Machine Staff = new(MachineId.Staff, "The chief of staff",
    [
        new(StaffState.Day, "By day", "H7"),
        new(StaffState.Investigating, "Investigating for him", "H7"),
        new(StaffState.Muster, "Muster", "L21", "K11"),
        new(StaffState.NightWatch, "Overnight watch", "H7", "H12"),
        new(StaffState.MorningBrief, "Morning brief", "L19", "L21")
    ], StaffState.Day, [])
    {
        Gaps =
        [
            new(StaffState.Day, StaffState.Muster, "H7 says the chief of staff runs the overnight watch, not what starts it"),
            new(StaffState.NightWatch, StaffState.MorningBrief, "H7 and L19 name the watch and the morning brief, not what ends the night")
        ]
    };

    public static readonly Machine Liveness = new(MachineId.Liveness, "Liveness",
    [
        new(LivenessState.Live, "Hop In Progress, recently changed", "S24"),
        new(LivenessState.Quiet, "A liveness finding", "S24", "S23"),
        new(LivenessState.Restarted, "Hop back in Todo, restarting", "S24")
    ], LivenessState.Live, []);

    public static readonly Machine Should = new(MachineId.Should, "A should",
    [
        new(ShouldState.Found, "A round finds a should", "R11", "S17"),
        new(ShouldState.Known, "Already open", "S17"),
        new(ShouldState.Open, "Open should", "S17"),
        new(ShouldState.Done, "Should fixed", "S17", "S18"),
        new(ShouldState.Duplicate, "Should marked Duplicate", "S17"),
        new(ShouldState.Canceled, "Should canceled with a reason", "S18"),
        new(ShouldState.Moved, "Should moved out by his call", "S18")
    ], ShouldState.Found, [ShouldState.Known, ShouldState.Done, ShouldState.Duplicate, ShouldState.Canceled, ShouldState.Moved]);
}
