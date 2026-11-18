using System.Text.RegularExpressions;

namespace DynaDocs.SkillBuild;

/// <summary>Names the builder derives from the compiler's: kebab-case file names, claim ids, heading anchors.</summary>
internal static partial class Names
{
    [GeneratedRegex("(?<=[a-z0-9])(?=[A-Z])|(?<=[A-Z])(?=[A-Z][a-z])")]
    private static partial Regex WordBreak();

    [GeneratedRegex(@"[^\p{L}\p{N} _-]")]
    private static partial Regex AnchorPunctuation();

    public static string Kebab(Enum value) => WordBreak().Replace(value.ToString(), "-").ToLowerInvariant();

    public static string Of(Enum value) => $"{value.GetType().Name}.{value}";

    /// <summary>GitHub's heading anchor: lower case, punctuation dropped, spaces to hyphens.</summary>
    public static string Anchor(string heading) =>
        AnchorPunctuation().Replace(heading.ToLowerInvariant(), "").Replace(' ', '-');
}
