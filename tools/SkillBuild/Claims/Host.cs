namespace DynaDocs.SkillBuild;

/// <summary>A project's host settings and guard (the HostSetup machine), carried unchanged from DR 055 and DR 056 (N12).</summary>
public static class Host
{
    public static readonly Claim Init = Move(HostSetupState.Uninitialised, HostSetupState.Initialised, Actor.DydoInit,
        "dydo init or --join turns native memory off for both hosts, commits one Claude guard entry and keeps the allow list local",
        "N12 DR 055 §1", "N12 DR 056 §1–2");
    public static readonly Claim Call = Move(HostSetupState.Initialised, HostSetupState.Guarded, Actor.Guard,
        "an agent's tool call runs through dydo guard, one policy for Claude and Codex", "N12 DR 056 §1", "N12 DR 056 §3");
    public static readonly Claim Deny = Move(HostSetupState.Guarded, HostSetupState.MemoryDenied, Actor.Guard,
        "a write to a host memory folder is blocked", "N12 DR 055 §2");
    public static readonly Claim Pass = Move(HostSetupState.Guarded, HostSetupState.Initialised, Actor.Guard,
        "not a memory write: the guard's other rules decide the call", "N12 DR 056 §1");
    public static readonly Claim Refused = Move(HostSetupState.MemoryDenied, HostSetupState.Initialised, Actor.Guard,
        "the write is refused and the session goes on: the guard stops accidents, not rogue agents", "N12 DR 055 §2–3");
    public static readonly Claim Background = Move(HostSetupState.Initialised, HostSetupState.CodexBackground, Actor.Codex,
        "Codex's memory runs from a background process the guard never sees", "N12 DR 055 §3");
    public static readonly Claim FlagOff = Move(HostSetupState.CodexBackground, HostSetupState.Initialised, Actor.Codex,
        "with memories = false it writes nothing: the Codex half rests on that flag", "N12 DR 055 §1", "N12 DR 055 §3");

    private static Claim Move(HostSetupState from, HostSetupState to, Actor actor, string arrow, params string[] spec) =>
        new($"In a project's host settings, {arrow}.", [actor], []) { Spec = spec, Transition = new(from, to, arrow) };
}
