namespace DynaDocs.SkillBuild;

/// <summary>
/// The header every built file carries (K6), and the committed files that carry it. The guard keeps
/// its own copy of both header lines, because the shipped binary holds no builder; a test keeps them equal.
/// </summary>
public static class BuiltHeader
{
    public const string Markdown = "<!-- built from the model; edit there: tools/SkillBuild -->";

    public const string Yaml = "# built from the model; edit there: tools/SkillBuild";

    /// <summary>A file carries the header when one of its first <see cref="Window"/> lines is a header line.</summary>
    public const int Window = 16;

    private static readonly string[] Roots = ["skills", "dydo", "Scaffold"];

    private static readonly EnumerationOptions Recursive = new() { RecurseSubdirectories = true };

    public static bool Carries(string file)
    {
        using var reader = new StreamReader(file);
        for (var index = 0; index < Window && reader.ReadLine() is { } line; index++)
            if (line.Trim() is Markdown or Yaml) return true;
        return false;
    }

    /// <summary>Every <c>.md</c> and <c>.yaml</c> file under <c>skills/</c>, <c>dydo/</c> and <c>Scaffold/</c> that carries the header.</summary>
    public static IReadOnlyDictionary<string, byte[]> Committed(string root) => Roots
        .Select(folder => Path.Combine(root, folder))
        .Where(Directory.Exists)
        .SelectMany(folder => Directory.EnumerateFiles(folder, "*", Recursive))
        .Where(file => Path.GetExtension(file) is ".md" or ".yaml" && Carries(file))
        .ToDictionary(file => Path.GetRelativePath(root, file).Replace('\\', '/'), File.ReadAllBytes, StringComparer.Ordinal);
}
