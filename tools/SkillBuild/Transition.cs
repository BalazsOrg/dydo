namespace DynaDocs.SkillBuild;

/// <summary>
/// A move between two states of one machine, with the arrow's words in the reader's language and an
/// optional one-line TLA+ guard and effect over the machine's variables (K1).
/// </summary>
public sealed record Transition(Enum From, Enum To, string Arrow)
{
    public string? Guard { get; init; }

    /// <summary>Primed assignments, such as <c>fixes' = fixes + 1</c>; a variable it does not prime stays unchanged.</summary>
    public string? Effect { get; init; }

    /// <summary>
    /// Weak fairness: once the move stays possible, it is eventually made. Set it only where the claim's
    /// spec rule promises progress; the module's <c>Fairness</c> conjoins one <c>WF_vars</c> per fair move.
    /// </summary>
    public bool Fair { get; init; }
}
