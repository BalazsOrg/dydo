using System.Text;
using System.Text.RegularExpressions;

namespace DynaDocs.SkillBuild;

/// <summary>
/// The stray-claim lint (K24): claim text pasted by hand into a file the builder does not own. Every
/// built line of at least <see cref="MinimumLength"/> characters, whitespace collapsed, is searched
/// for in each tracked hand-written <c>.md</c> under <c>skills/</c>, <c>dydo/</c> and <c>Scaffold/</c>
/// and in the root <c>AGENTS.md</c>.
/// </summary>
public static partial class StrayClaims
{
    public const int MinimumLength = 40;

    private static readonly string[] Roots = ["skills/", "dydo/", "Scaffold/"];

    [GeneratedRegex(@"\s+")]
    private static partial Regex Whitespace();

    /// <summary>Each hit as <c>&lt;path&gt;:&lt;line&gt; claim:&lt;id&gt;[ claim:…]</c>, from the tracked paths under <paramref name="root"/>.</summary>
    public static IReadOnlyList<string> Find(string root, IEnumerable<string> tracked, BuiltTree tree)
    {
        var built = tree.Map
            .Select(line => (Text: Whitespace().Replace(line.Text, " ").Trim(), line.Claims))
            .Where(line => line.Text.Length >= MinimumLength)
            .DistinctBy(line => line.Text)
            .ToArray();
        if (built.Length == 0) return [];
        return [.. tracked.Select(path => path.Replace('\\', '/')).Where(path => IsHandWritten(root, path, tree))
            .SelectMany(path => Hits(path, File.ReadAllText(Path.Combine(root, path)), built))];
    }

    private static bool IsHandWritten(string root, string path, BuiltTree tree) =>
        path.EndsWith(".md", StringComparison.Ordinal)
        && (path == "AGENTS.md" || Roots.Any(prefix => path.StartsWith(prefix, StringComparison.Ordinal)))
        && !tree.Files.ContainsKey(path)
        && !BuiltHeader.Carries(Path.Combine(root, path));

    private static IEnumerable<string> Hits(string path, string text, IEnumerable<(string Text, IReadOnlyList<string> Claims)> built)
    {
        var (collapsed, lineOf) = Collapse(text);
        foreach (var (claimText, claims) in built)
            for (var at = collapsed.IndexOf(claimText, StringComparison.Ordinal); at >= 0;
                 at = collapsed.IndexOf(claimText, at + 1, StringComparison.Ordinal))
                yield return $"{path}:{lineOf[at]} {string.Join(' ', claims.Select(id => "claim:" + id))}";
    }

    // The text with each whitespace run one space, and the source line of every character kept.
    private static (string Text, int[] LineOf) Collapse(string text)
    {
        var kept = new StringBuilder();
        var lineOf = new List<int>();
        var line = 1;
        var gap = false;
        foreach (var character in text)
        {
            if (char.IsWhiteSpace(character))
            {
                if (character == '\n') line++;
                gap = kept.Length > 0;
                continue;
            }
            if (gap)
            {
                kept.Append(' ');
                lineOf.Add(line);
                gap = false;
            }
            kept.Append(character);
            lineOf.Add(line);
        }
        return (kept.ToString(), [.. lineOf]);
    }
}
