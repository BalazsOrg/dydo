namespace DynaDocs.SkillBuild;

/// <summary>
/// The header every built file carries (K6), and the committed files that carry it. The guard keeps
/// its own copy of the Markdown and YAML header lines, because the shipped binary holds no builder; a
/// test keeps them equal.
/// </summary>
public static class BuiltHeader
{
    public const string Text = "built from the model; edit there: tools/SkillBuild";

    public const string Markdown = "<!-- " + Text + " -->";

    public const string Yaml = "# " + Text;

    /// <summary>A TLA+ module's or a TLC config's comment line.</summary>
    public const string Tla = @"\* " + Text;

    public const string Script = "// " + Text;

    /// <summary>JSON has no comments: the header is the document's first member.</summary>
    public const string Json = "\"$comment\": \"" + Text + "\",";

    /// <summary>A file carries the header when one of its first <see cref="Window"/> lines is a header line.</summary>
    public const int Window = 16;

    private static readonly string[] Documents = [".md", ".yaml"];

    private static readonly (string Folder, string[] Extensions)[] Roots =
    [
        ("skills", Documents),
        ("dydo", Documents),
        ("Scaffold", Documents),
        ("claims/statechart", [".json", ".tla", ".cfg", ".mjs"])
    ];

    private static readonly EnumerationOptions Recursive = new() { RecurseSubdirectories = true };

    public static bool Carries(string file)
    {
        using var reader = new StreamReader(file);
        for (var index = 0; index < Window && reader.ReadLine() is { } line; index++)
            if (line.Trim() is Markdown or Yaml or Tla or Script or Json) return true;
        return false;
    }

    /// <summary>
    /// Every <c>.md</c> and <c>.yaml</c> file under <c>skills/</c>, <c>dydo/</c> and <c>Scaffold/</c>, and every
    /// chart file under <c>claims/statechart/</c>, that carries the header.
    /// </summary>
    public static IReadOnlyDictionary<string, byte[]> Committed(string root) => Roots
        .Select(rootFolder => (Folder: Path.Combine(root, rootFolder.Folder), rootFolder.Extensions))
        .Where(rootFolder => Directory.Exists(rootFolder.Folder))
        .SelectMany(rootFolder => Directory.EnumerateFiles(rootFolder.Folder, "*", Recursive)
            .Where(file => rootFolder.Extensions.Contains(Path.GetExtension(file))))
        .Where(Carries)
        .ToDictionary(file => Path.GetRelativePath(root, file).Replace('\\', '/'), File.ReadAllBytes, StringComparer.Ordinal);
}
