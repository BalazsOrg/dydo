namespace DynaDocs.SkillBuild;

/// <summary>A built file's text and the provenance of each prose line in it.</summary>
internal sealed record RenderedFile(string Text, IReadOnlyList<MapEntry> Map)
{
    public int LineCount => Text.Count(character => character == '\n');
}
