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

    // DYD-968 keeps a skill folder to its runtime files; K26 keeps the agents/openai.yaml Codex reads.
    public const string FolderRule =
        "a skill folder holds only SKILL.md, resources/*.md and agents/openai.yaml, runtime files and never tests or fixtures (DYD-968, K26)";

    // K9: "An edge case is a resource, one short file per scenario, under 100 lines." Every role resource keeps it.
    public const int RoleResourceLimit = 100;

    public const string WhenThingsGoWrong = "## When things go wrong";

    private const string Roles = "skills/roles/";

    [GeneratedRegex(@"\]\((?<target>[^)\s#]+)(?:#[^)\s]*)?\)")]
    private static partial Regex Link();

    [GeneratedRegex(@"^## (?<number>[0-9]+)\. \S")]
    private static partial Regex PhaseHeading();

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
                violations.AddRange(path[folder.Length..] switch
                {
                    var local when ResourcePath().IsMatch(local) => ResourceViolations(tree, path, linked),
                    "agents/openai.yaml" => [],
                    _ => [$"{path}: {FolderRule}"]
                });
        }
        return violations;
    }

    /// <summary>
    /// K9's role spine, for every <c>SKILL.md</c> under <c>skills/roles/</c>: after its title, one opening
    /// job paragraph, then job phases numbered from 1 without a gap, then at most one "When things go
    /// wrong", last; every link it makes resolves in the tree; and every role resource is under 100 lines.
    /// </summary>
    public static IReadOnlyList<string> RoleSpineViolations(IReadOnlyDictionary<string, string> tree)
    {
        var violations = new List<string>();
        foreach (var spine in tree.Keys.Where(path => path.StartsWith(Roles, StringComparison.Ordinal) && IsSpine(path)))
        {
            violations.AddRange(SpineOrder(spine, tree[spine].Split('\n')));
            violations.AddRange(Links(spine, tree[spine]).Where(target => !tree.ContainsKey(target))
                .Select(target => $"{spine}: links {target}, which the tree does not hold"));
        }
        foreach (var (path, text) in tree.Where(file => file.Key.StartsWith(Roles, StringComparison.Ordinal) && file.Key.Contains("/resources/", StringComparison.Ordinal)))
            if (Lines(text) >= RoleResourceLimit)
                violations.Add($"{path}: {Lines(text)} lines; a role resource stays under {RoleResourceLimit} lines (K9)");
        return violations;
    }

    private static IEnumerable<string> SpineOrder(string spine, string[] lines)
    {
        var body = lines.SkipWhile(line => !line.StartsWith("# ", StringComparison.Ordinal)).Skip(1).ToArray();
        var opening = body.TakeWhile(line => !line.StartsWith("## ", StringComparison.Ordinal))
            .SkipWhile(line => line.Length == 0).Reverse().SkipWhile(line => line.Length == 0).ToArray();
        if (opening.Length == 0)
            yield return $"{spine}: no opening job paragraph (K9)";
        else if (opening.Any(line => line.Length == 0 || line[0] is '#' or '-' or '|' or '`' or '>'))
            yield return $"{spine}: the opening is not one job paragraph (K9)";
        var next = 1;
        var wrong = false;
        foreach (var heading in body.Where(line => line.StartsWith("## ", StringComparison.Ordinal)))
        {
            var phase = PhaseHeading().Match(heading);
            if (heading == WhenThingsGoWrong)
            {
                if (wrong) yield return $"{spine}: \"{WhenThingsGoWrong}\" twice (K9)";
                wrong = true;
            }
            else if (!phase.Success)
                yield return $"{spine}: \"{heading}\" is neither a numbered job phase nor \"{WhenThingsGoWrong}\" (K9)";
            else if (wrong)
                yield return $"{spine}: \"{heading}\" comes after \"{WhenThingsGoWrong}\" (K9)";
            else if (int.Parse(phase.Groups["number"].Value) != next)
                yield return $"{spine}: \"{heading}\" where phase {next} belongs; phases run from 1 in order (K9)";
            else next++;
        }
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
