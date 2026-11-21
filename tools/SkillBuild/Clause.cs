namespace DynaDocs.SkillBuild;

/// <summary>
/// A part of a kept legacy line that the claims keeping the line do not state, quoted from the line,
/// with its own fate: dropped, with the truth question it fails and why; deferred, with why it is not
/// rebuilt yet; or withheld, in the open claim that holds it. Written through a kept <see cref="Fate"/>.
/// </summary>
public sealed record Clause
{
    internal Clause(string quote, Question? fails, string? why, Claim? open)
    {
        Quote = quote;
        Fails = fails;
        Why = why;
        Open = open;
    }

    public string Quote { get; }

    public Question? Fails { get; }

    public string? Why { get; }

    public Claim? Open { get; }
}
