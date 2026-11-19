namespace DynaDocs.SkillBuild;

/// <summary>A dydo statechart run and its prerequisites (the StatechartRun machine), carried unchanged from DR 062 (N12).</summary>
public static class ChartTool
{
    public static readonly Claim Invalid = Move(StatechartRunState.Invoked, StatechartRunState.InvalidOverride,
        "an explicit invalid override is an error", "N12 DR 062 §1");
    public static readonly Claim Ready = Move(StatechartRunState.Invoked, StatechartRunState.Ready,
        "its dependencies verified compatible, it runs offline with no repeated installation or consent", "N12 DR 062 §1");
    public static readonly Claim Missing = Move(StatechartRunState.Invoked, StatechartRunState.ApprovalRequired,
        "a dependency is missing: a precise locked plan, a structured event, the saved request and exit 3, proposing dydo statechart resume --request", "N12 DR 062 §2");
    public static readonly Claim Authorize = Move(StatechartRunState.ApprovalRequired, StatechartRunState.Continuing,
        "the installed skill submits that exact continuation through the host's normal authorization, and the host grants it", "N12 DR 062 §3");
    public static readonly Claim Deny = Move(StatechartRunState.ApprovalRequired, StatechartRunState.NonSuccess,
        "denied, authorization unavailable or canceled: non-success, the request kept; a local flag, token or receipt is not host approval", "N12 DR 062 §3", "N12 DR 062 §5");
    public static readonly Claim Installed = Move(StatechartRunState.Continuing, StatechartRunState.Ready,
        "identities verified, the locked assets installed into owned roots, readiness proved and the receipt published atomically; the saved inputs continue", "N12 DR 062 §4");
    public static readonly Claim InstallFails = Move(StatechartRunState.Continuing, StatechartRunState.NonSuccess,
        "the installation fails: non-success, the request kept, and no checker starts on an unverified plan", "N12 DR 062 §5");
    public static readonly Claim Check = Move(StatechartRunState.Ready, StatechartRunState.Checking,
        "the check starts under native statechart ownership, with registered scratch and startup gates", "N12 DR 062 §6");
    public static readonly Claim Drained = Move(StatechartRunState.Checking, StatechartRunState.Charted,
        "an authoritative drain: registered-owned scratch is cleaned", "N12 DR 062 §6");
    public static readonly Claim Uncertain = Move(StatechartRunState.Checking, StatechartRunState.ScratchKept,
        "the drain is uncertain, or only notified: scratch kept with a durable reason", "N12 DR 062 §6");

    private static Claim Move(StatechartRunState from, StatechartRunState to, string arrow, params string[] spec) =>
        new($"In a dydo statechart run, {arrow}.", [Actor.DydoStatechart], []) { Spec = spec, Transition = new(from, to, arrow) };
}
