namespace DynaDocs.SkillBuild;

/// <summary>A built file, named relative to its carrier's folder, with the claim-stating lines in it.</summary>
internal sealed record RenderedFile(string Local, string Text, IReadOnlyList<(int Line, string Text, Placed Placed)> Stated)
{
    public int LineCount => Text.Count(character => character == '\n');
}
