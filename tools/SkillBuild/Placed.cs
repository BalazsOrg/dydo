namespace DynaDocs.SkillBuild;

/// <summary>A model line with the ids of the claims it states, its own claim first.</summary>
internal sealed record Placed(Line Line, IReadOnlyList<string> Claims)
{
    public string Owner => Claims[0];
}
