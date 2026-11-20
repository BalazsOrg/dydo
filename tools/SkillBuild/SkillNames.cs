namespace DynaDocs.SkillBuild;

/// <summary>
/// The skills a dispatch may name, checked against the repository root the tool builds into: each
/// envoy job's skill (H23) and each dispatch template's default skill set (K14) must be a skill the
/// model builds or a hand-written <c>SKILL.md</c> folder under <c>skills/</c>. A built file left on disk
/// is no skill once the model stops building it, because the build deletes it.
/// </summary>
internal static class SkillNames
{
    public static void Check(Model model, string root)
    {
        var known = Known(model, root);
        model.Envoys?.CheckSkillsExist(known);
        foreach (var skill in model.Skills)
            foreach (var template in skill.Resources)
                foreach (var name in template.Skills.Where(name => !known.Contains(name)))
                    throw new InvalidDataException(
                        $"K14: {skill.Name}'s {template.Title} names /{name}, which is no skill under skills/, hand-written or built");
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
