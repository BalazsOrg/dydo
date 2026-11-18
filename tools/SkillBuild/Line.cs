namespace DynaDocs.SkillBuild;

/// <summary>
/// A claim's prose in one skill, at its place. A combined line is hand-written once, on one of its
/// claims, and names the others in <paramref name="also"/>; it counts once for each. The names are a
/// lambda so kind classes never read each other's fields while they initialize; a claim declared
/// later in the same kind class takes <c>!</c>, because the compiler sees that field still unset.
/// </summary>
public sealed class Line(Enum skill, string text, Place place, Func<Claim[]>? also = null)
{
    public Enum Skill { get; } = skill;

    public string Text { get; } = text;

    public Place Place { get; } = place;

    internal Claim[] Also() => also?.Invoke() ?? [];
}
