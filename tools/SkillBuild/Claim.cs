namespace DynaDocs.SkillBuild;

/// <summary>
/// One rule, with one line of prose in each skill that carries it (K4). A claim is a
/// <c>public static readonly</c> field of a kind class; its id, <c>Kind.Member</c>, is the compiler's
/// name for that field, so no id is ever typed by hand.
/// </summary>
public sealed class Claim(string rule, Enum[] actors, Line[] lines)
{
    public string Rule { get; } = rule;

    public IReadOnlyList<Enum> Actors { get; } = actors;

    public IReadOnlyList<Line> Lines { get; } = lines;

    /// <summary>The spec rule ids the claim states, such as <c>K4</c> (<see cref="SpecCitation"/>).</summary>
    public IReadOnlyList<string> Spec { get; init; } = [];

    /// <summary>
    /// The machine move the claim is (K1). Its lines are moves: an exit in its from-state or an arrival
    /// in its to-state. With no lines it is a system transition, run by a command or by Linear.
    /// </summary>
    public Transition? Transition { get; init; }

    /// <summary>
    /// Its evidence, layer and reason. Required once a truth-checked skill states the claim, and checked
    /// wherever the claim's lines sit; a claim only legacy skills state needs none.
    /// </summary>
    public Truth? Truth { get; init; }
}
