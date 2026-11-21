namespace DynaDocs.SkillBuild;

/// <summary>
/// A claim's prose in one skill, at its place. A combined line is hand-written once, on one of its
/// claims, and names the others in <paramref name="also"/>; it counts once for each. The names are a
/// lambda so kind classes never read each other's fields while they initialize; a claim declared
/// later in the same kind class takes <c>!</c>, because the compiler sees that field still unset.
/// </summary>
public sealed class Line(Enum skill, string text, Place place, Func<Claim[]>? also = null)
{
    /// <summary>
    /// A line of a truth-checked skill names its <paramref name="reader"/>, the actor who acts on it
    /// (DYD-1295 41a553f1): the build fails when the skill does not belong to that reader.
    /// </summary>
    public Line(Enum skill, Enum reader, string text, Place place, Func<Claim[]>? also = null) : this(skill, text, place, also) =>
        Reader = reader;

    public Enum Skill { get; } = skill;

    public Enum? Reader { get; }

    public string Text { get; } = text;

    public Place Place { get; } = place;

    internal Claim[] Also() => also?.Invoke() ?? [];
}
