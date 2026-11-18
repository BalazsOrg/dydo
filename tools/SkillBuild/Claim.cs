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
}
