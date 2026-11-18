namespace DynaDocs.SkillBuild;

/// <summary>
/// The build: a model in, every built file out, keyed by its path under the repository root, with
/// the map of each claim-stating line. A model the rules reject throws <see cref="InvalidDataException"/>;
/// the build never emits a partial tree.
/// </summary>
public static class SkillBuilder
{
    public static BuiltTree Build(Model model)
    {
        var files = new SortedDictionary<string, string>(StringComparer.Ordinal);
        var map = new List<MapLine>();
        foreach (var (skill, lines) in ModelIndex.LinesBySkill(model))
        {
            var rendered = SkillRenderer.Render(skill, new SkillLayout(skill, lines)).ToArray();
            StatesEachClaimOnce(skill, lines, rendered);
            foreach (var file in rendered)
                foreach (var path in skill.PathsOf(file.Local))
                {
                    files.Add(path, file.Text);
                    map.AddRange(file.Stated.Select(stated => new MapLine(path, stated.Line, stated.Text, stated.Placed.Claims)));
                }
        }
        return new BuiltTree(files, map);
    }

    // K4: each claim a skill carries is stated by exactly one line of its built files; a combined
    // line counts once for each claim it names.
    private static void StatesEachClaimOnce(SkillSpec skill, IEnumerable<Placed> carried, IEnumerable<RenderedFile> rendered)
    {
        var stated = rendered.SelectMany(file => file.Stated.Select(line => line.Placed)).Distinct()
            .SelectMany(placed => placed.Claims).CountBy(id => id).ToDictionary();
        foreach (var id in carried.SelectMany(placed => placed.Claims).Distinct())
            if (stated.GetValueOrDefault(id) != 1)
                throw new InvalidDataException(
                    $"{skill.Name} states claim:{id} {stated.GetValueOrDefault(id)} times; a skill states each claim it carries on exactly one line");
    }
}
