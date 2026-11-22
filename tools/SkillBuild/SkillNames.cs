using System.Text.RegularExpressions;

namespace DynaDocs.SkillBuild;

/// <summary>
/// The skills a dispatch may name, checked against the repository root the tool builds into: each
/// envoy job's skill (H23), each dispatch template's default skill set, and every skill the built prose
/// names with a slash (K14) must be a skill the model builds or a hand-written <c>SKILL.md</c> folder
/// under <c>skills/</c>. A built file left on disk is no skill once the model stops building it, because
/// the build deletes it.
/// </summary>
internal static partial class SkillNames
{
    // A skill named with a slash: after a line's start, a space, "(", a backtick or a quote, then any run of
    // bold or italic markers (review r2 S7), a kebab-case name that no path segment, file extension, plugin
    // name or snake_case word continues; a closing `_` run ends it like `*`.
    [GeneratedRegex(@"(?<=(?:^|[\s(`""'])[*_]*)/(?<name>[a-z][a-z0-9]*(?:-[a-z0-9]+)*)(?![^\W_]|[/-]|_+(?:[^\W_]|[/-])|[.:]\w)")]
    private static partial Regex Named();

    public static void Check(Model model, BuiltTree tree, string root)
    {
        var known = Known(model, root);
        model.Envoys?.CheckSkillsExist(known);
        foreach (var skill in model.Skills)
            foreach (var template in skill.Resources)
                foreach (var name in template.Skills.Where(name => !known.Contains(name)))
                    throw new InvalidDataException(
                        $"K14: {skill.Name}'s {template.Title} names /{name}, which is no skill under skills/, hand-written or built");
        // Review r1 S7: a skill named in prose, outside the declared sets, is checked as well.
        var unknown = tree.Files.Concat(tree.Unwritten).Where(file => file.Key.EndsWith(".md", StringComparison.Ordinal))
            .SelectMany(file => file.Value.Split('\n').SelectMany((line, index) => Named().Matches(line)
                .Select(match => match.Groups["name"].Value)
                .Where(name => !known.Contains(name))
                .Select(name => $"{file.Key}:{index + 1} /{name}")))
            .ToArray();
        if (unknown.Length > 0)
            throw new InvalidDataException(
                $"K14: built prose names a skill that is no skill under skills/, hand-written or built: {string.Join("; ", unknown)}");
    }

    private static HashSet<string> Known(Model model, string root)
    {
        var known = model.Skills.Where(skill => !skill.IsStandard).Select(skill => skill.Name).ToHashSet(StringComparer.Ordinal);
        var skills = Path.Combine(root, "skills");
        if (Directory.Exists(skills))
            known.UnionWith(Directory.EnumerateFiles(skills, "SKILL.md", SearchOption.AllDirectories)
                .Where(file => !BuiltHeader.Carries(file))
                .Select(file => Path.GetFileName(Path.GetDirectoryName(file))!));
        return known;
    }
}
