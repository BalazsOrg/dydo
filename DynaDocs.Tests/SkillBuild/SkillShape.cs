namespace DynaDocs.Tests.SkillBuild;

using System.Text.RegularExpressions;

/// <summary>
/// The skill-shape limits of Grilling A D7 and D8, checked over a built tree keyed by path. The line
/// limits are Anthropic's guidance, not dydo axioms, so each cites its source and moves with it:
/// https://platform.claude.com/docs/en/agents-and-tools/agent-skills/best-practices
/// </summary>
internal static partial class SkillShape
{
    public const string BestPractices = "https://platform.claude.com/docs/en/agents-and-tools/agent-skills/best-practices";

    // "Keep the SKILL.md body under 500 lines for optimal performance." (Progressive disclosure patterns)
    public const int SpineBodyLimit = 500;

    // "For reference files longer than 100 lines, include a table of contents at the top." (Structure longer
    // reference files with a table of contents). "Claude might partially read files when they're referenced
    // from other referenced files ... use commands such as `head -100`" (Avoid deeply nested references):
    // D8 keeps a resource reached from another resource within that preview, so any read is a whole read.
    public const int ResourceLimit = 100;

    [GeneratedRegex(@"\]\((?<target>[^)\s#]+)(?:#[^)\s]*)?\)")]
    private static partial Regex Link();

    [GeneratedRegex(@"^resources/[^/]+\.md$")]
    private static partial Regex ResourcePath();

    [GeneratedRegex(@"(?m)^ {0,3}(`{3,}|~{3,})(.*)$")]
    private static partial Regex Fence();

    public static IReadOnlyList<string> Violations(IReadOnlyDictionary<string, string> tree)
    {
        var violations = new List<string>();
        foreach (var (path, text) in tree)
            if (HasUnbalancedFence(text)) violations.Add($"{path}: unbalanced fenced block");
        foreach (var spine in tree.Keys.Where(IsSpine))
        {
            if (BodyLines(tree[spine]) > SpineBodyLimit)
                violations.Add($"{spine}: the SKILL.md body is over {SpineBodyLimit} lines ({BestPractices})");
            var folder = spine[..^"SKILL.md".Length];
            var linked = Links(spine, tree[spine]);
            foreach (var path in tree.Keys.Where(path => path != spine && path.StartsWith(folder, StringComparison.Ordinal)))
                violations.AddRange(path[folder.Length..] is var local && ResourcePath().IsMatch(local)
                    ? ResourceViolations(tree, path, linked)
                    : [$"{path}: a skill folder holds only SKILL.md and resources/*.md, runtime resources and never tests or fixtures (DYD-968)"]);
        }
        return violations;
    }

    private static bool HasUnbalancedFence(string text)
    {
        var pending = "";
        foreach (Match match in Fence().Matches(text))
        {
            var run = match.Groups[1].Value;
            if (pending.Length == 0) pending = run;
            else if (run.StartsWith(pending, StringComparison.Ordinal) && match.Groups[2].Value.Trim().Length == 0)
                pending = "";
        }
        return pending.Length != 0;
    }

    private static IEnumerable<string> ResourceViolations(IReadOnlyDictionary<string, string> tree, string path,
        ISet<string> linkedFromSpine)
    {
        if (!linkedFromSpine.Contains(path))
            yield return $"{path}: no spine links to this resource ({BestPractices})";
        if (Lines(tree[path]) > ResourceLimit && !HasContents(tree[path]))
            yield return $"{path}: a resource over {ResourceLimit} lines has no contents list ({BestPractices})";
        foreach (var target in Links(path, tree[path]))
            if (tree.TryGetValue(target, out var text) && !IsSpine(target) && Lines(text) > ResourceLimit)
                yield return $"{target}: reached from the resource {path} but over {ResourceLimit} lines ({BestPractices})";
    }

    private static bool IsSpine(string path) => path == "SKILL.md" || path.EndsWith("/SKILL.md", StringComparison.Ordinal);

    private static int Lines(string text) => text.Count(character => character == '\n');

    private static int BodyLines(string text)
    {
        if (!text.StartsWith("---\n", StringComparison.Ordinal)) return Lines(text);
        var end = text.IndexOf("\n---\n", 3, StringComparison.Ordinal);
        return end < 0 ? Lines(text) : Lines(text[(end + "\n---\n".Length)..]);
    }

    // The contents list is a `## Contents` section before any other section.
    private static bool HasContents(string text)
    {
        var section = text.Split('\n').SkipWhile(line => !line.StartsWith("## ", StringComparison.Ordinal)).ToArray();
        return section.FirstOrDefault() == "## Contents"
            && section.Skip(1)
                .TakeWhile(line => !line.StartsWith("## ", StringComparison.Ordinal)
                                   && !line.StartsWith("# ", StringComparison.Ordinal))
                .Any(line => line.StartsWith("- ", StringComparison.Ordinal));
    }

    private static HashSet<string> Links(string from, string text)
    {
        var folder = from.Contains('/') ? from[..(from.LastIndexOf('/') + 1)] : "";
        return Link().Matches(text)
            .Select(match => match.Groups["target"].Value)
            .Where(target => !target.Contains("://", StringComparison.Ordinal) && !target.StartsWith('/'))
            .Select(target => Normalize(folder + target))
            .ToHashSet(StringComparer.Ordinal);
    }

    private static string Normalize(string path)
    {
        var parts = new List<string>();
        foreach (var part in path.Split('/'))
        {
            if (part == ".." && parts.Count > 0) parts.RemoveAt(parts.Count - 1);
            else if (part is not ("." or "")) parts.Add(part);
        }
        return string.Join('/', parts);
    }
}
