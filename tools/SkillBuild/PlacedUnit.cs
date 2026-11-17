namespace DynaDocs.SkillBuild;

/// <summary>
/// One source unit as it is emitted: once, under the kind of the first claim that carries it,
/// with every claim that shares it.
/// </summary>
internal sealed record PlacedUnit(string Id, string Path, int Line, int Index, string Text, string Kind)
{
    public List<string> Claims { get; } = [];
}
