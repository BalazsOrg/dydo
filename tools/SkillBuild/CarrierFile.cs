namespace DynaDocs.SkillBuild;

/// <summary>One built file of a carrier: its source path, its H1 and, for a SKILL.md, its frontmatter.</summary>
internal sealed record CarrierFile(string Path, string Title, IReadOnlyList<string>? Frontmatter = null)
{
    public bool IsSpine => System.IO.Path.GetFileName(Path) == "SKILL.md";
}
