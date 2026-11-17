using System.Text.RegularExpressions;

namespace DynaDocs.SkillBuild;

/// <summary>A source unit's closing markup, kept separate from its verbatim prose and provenance.</summary>
internal static partial class MarkdownFence
{
    [GeneratedRegex(@"^ {0,3}(?<marker>`{3,}|~{3,})(?<suffix>.*)$")]
    private static partial Regex Marker();

    public static string? Unclosed(IEnumerable<string> lines)
    {
        string? opened = null;
        foreach (var line in lines)
        {
            var match = Marker().Match(line);
            if (!match.Success) continue;
            var marker = match.Groups["marker"].Value;
            if (opened is null) opened = marker;
            else if (marker[0] == opened[0] && marker.Length >= opened.Length
                     && string.IsNullOrWhiteSpace(match.Groups["suffix"].Value)) opened = null;
        }
        return opened;
    }
}
