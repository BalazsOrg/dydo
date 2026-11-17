namespace DynaDocs.Commands;

/// <summary>
/// The worker lane's classification of dydo commands: the one place that says which dydo commands a
/// sub-agent (a call carrying agent_id) may run. The guard's allowlist, its refusal text, the registry
/// test (every command <see cref="DydoRootCommand"/> registers sits on exactly one list) and the skill
/// lint (every <c>dydo</c> command in prose addressed to a sub-agent is allowed) all read these lists.
/// A new command is classified here, or the registry test fails.
///
/// <para>Who the worker lane is: a call whose hook payload carries <c>agent_id</c>, which the host
/// sends only for a native in-process sub-agent (Claude's <c>Agent</c> tool, Codex's
/// <c>spawn_agent</c>). A T3 <c>delegate_task</c> child is a separate session whose payload carries no
/// <c>agent_id</c>, so the guard treats it as a top-level call (DYD-1072): it is neither refused by these
/// lists nor evidence of what they do.</para>
/// </summary>
public static partial class GuardCommand
{
    /// <summary>
    /// dydo subcommands a sub-agent may run: the documentation and coverage gates (check, fix,
    /// gap-check), read-only inspection (graph, validate, survey), help/version and the machine mode every
    /// agent reads (mode); mode set still meets its nudge; clean is what Ticket Captains run on close-out,
    /// pr-ready before they open a PR, merge what a Merge Sub-ticket's code-writer runs to integrate a
    /// reviewed PR into its feature branch; survey is what the admiral check-in's surveyor runs;
    /// statechart is what crew run to draw a checked model. The guard reads only the word after dydo, so
    /// statechart's install-skill and resume verbs pass with it: install-skill writes the project's
    /// statechart skill and its projections, and resume installs locked prerequisites after the host's own
    /// authorization.
    /// </summary>
    internal static readonly string[] WorkerAllowedDydoSubcommands =
    [
        "check", "fix", "gap-check", "graph", "validate", "mode", "clean", "pr-ready", "merge",
        "survey", "statechart", "help", "version"
    ];

    /// <summary>
    /// dydo subcommands that stay with the top-level session: setup (init, adopt), generation (index),
    /// the hook itself (guard), the localhost viewer (map) and the shell-completion plumbing
    /// (completions, _complete).
    /// </summary>
    internal static readonly string[] TopLevelOnlyDydoSubcommands =
        ["init", "adopt", "index", "guard", "map", "completions", "_complete"];

    /// <summary>The options a sub-agent may pass to bare dydo: its built-in help and version.</summary>
    internal static readonly string[] WorkerAllowedDydoOptions = ["-h", "--help", "--version"];

    private static readonly HashSet<string> WorkerAllowedDydoWords =
        new([.. WorkerAllowedDydoSubcommands, .. WorkerAllowedDydoOptions], StringComparer.Ordinal);

    /// <summary>The refusal for a sub-agent that passed <paramref name="blockedWord"/> to dydo; its lists are the classification.</summary>
    internal static string[] WorkerRefusalLines(string blockedWord) =>
    [
        "BLOCKED: Sub-agents may run only these dydo commands: "
            + string.Join(", ", WorkerAllowedDydoSubcommands) + $" (plus {string.Join("/", WorkerAllowedDydoOptions)}).",
        $"  Not allowed here: dydo {blockedWord}",
        "  These belong to the top-level session that spawned you: "
            + string.Join(", ", TopLevelOnlyDydoSubcommands) + "."
    ];
}
