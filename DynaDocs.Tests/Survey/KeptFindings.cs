namespace DynaDocs.Tests.Survey;

/// <summary>
/// The survey findings 4.0.0 keeps (DR 063 §4). The lane ledger and its findings retire, so tests read
/// only these kinds and stay true whether or not the retired kinds are still printed.
/// </summary>
internal static class KeptFindings
{
    public static readonly HashSet<string> Kinds = ["unblocked", "pickable", "uncontracted", "question-no-priority"];

    /// <summary>Each printed finding of a kept kind, as <c>kind KEY</c>, in printed order.</summary>
    public static List<string> In(string stdout) =>
        [.. stdout.ReplaceLineEndings("\n").Split('\n', StringSplitOptions.RemoveEmptyEntries)
            .Select(line => line.Split(' '))
            .Where(words => words.Length > 1 && Kinds.Contains(words[0]))
            .Select(words => $"{words[0]} {words[1]}")];
}
