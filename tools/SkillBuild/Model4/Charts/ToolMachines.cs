namespace DynaDocs.SkillBuild;

/// <summary>
/// The 4.0.0 machines for dydo's own tools (K1): a project's host settings and guard (N12 DR 055,
/// N12 DR 056), a <c>dydo statechart</c> run and its prerequisites (N12 DR 062), and <c>dydo map</c>
/// (S19–S21, S26).
/// </summary>
public static class ToolMachines
{
    public static readonly Machine HostSetup = new(MachineId.HostSetup, "Host settings and the guard",
    [
        new(HostSetupState.Uninitialised, "A project before dydo init", "N12 DR 056 §1"),
        new(HostSetupState.Initialised, "Settings written: native memory off, the Claude guard committed, the allow list local",
            "N12 DR 055 §1", "N12 DR 056 §1–2"),
        new(HostSetupState.Guarded, "An agent tool call meets the guard", "N12 DR 056 §1", "N12 DR 056 §3"),
        new(HostSetupState.MemoryDenied, "A write to a host memory folder refused", "N12 DR 055 §2"),
        new(HostSetupState.CodexBackground, "Codex's background memory writer, unseen by the guard", "N12 DR 055 §3")
    ], HostSetupState.Uninitialised, []);

    public static readonly Machine StatechartRun = new(MachineId.StatechartRun, "A dydo statechart run",
    [
        new(StatechartRunState.Invoked, "dydo statechart invoked", "N12 DR 062 §1"),
        new(StatechartRunState.InvalidOverride, "An explicit invalid override: an error", "N12 DR 062 §1"),
        new(StatechartRunState.Ready, "Ready: verified compatible dependencies, offline, no consent asked again", "N12 DR 062 §1"),
        new(StatechartRunState.ApprovalRequired, "Approval required: a locked plan, the saved request, exit 3", "N12 DR 062 §2"),
        new(StatechartRunState.Continuing, "Authorized continuation: identities verified, locked assets installed", "N12 DR 062 §3", "N12 DR 062 §4"),
        new(StatechartRunState.NonSuccess, "Non-success: denied, unavailable, failed or canceled, the request kept", "N12 DR 062 §5"),
        new(StatechartRunState.Checking, "TLC checking under native statechart ownership", "N12 DR 062 §6"),
        new(StatechartRunState.Charted, "Charted; drain authoritative, registered scratch cleaned", "N12 DR 062 §6"),
        new(StatechartRunState.ScratchKept, "Drain uncertain: scratch kept with a durable reason", "N12 DR 062 §6")
    ], StatechartRunState.Invoked, [StatechartRunState.InvalidOverride, StatechartRunState.NonSuccess, StatechartRunState.Charted, StatechartRunState.ScratchKept]);

    public static readonly Machine Map = new(MachineId.Map, "dydo map",
    [
        new(MapState.Closed, "Map not open", "S19", "H6"),
        new(MapState.Snapshot, "A revisit: the last visit's disposable, machine-local snapshot shown first", "S26"),
        new(MapState.Fresh, "A fresh read: top-level tickets, open plus closed in the last 3 days faded; the view in the URL", "S19", "S20", "S21", "S26"),
        new(MapState.Shown, "The fresh read stands, its snapshot saved or not; Linear stays authoritative", "S26"),
        new(MapState.Expanded, "A card expanded: the same layout at any depth", "S19")
    ], MapState.Closed, [MapState.Shown, MapState.Expanded]);
}
