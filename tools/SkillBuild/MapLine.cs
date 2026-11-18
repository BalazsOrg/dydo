namespace DynaDocs.SkillBuild;

/// <summary>
/// One built line that states claims (K23): its path and line, the model's text for it, and the ids
/// of the claims it states. Printed as <c>&lt;path&gt;:&lt;line&gt; claim:&lt;id&gt;[ claim:…]</c>.
/// </summary>
public sealed record MapLine(string Path, int Line, string Text, IReadOnlyList<string> Claims)
{
    public override string ToString() => $"{Path}:{Line} {string.Join(' ', Claims.Select(id => "claim:" + id))}";
}
