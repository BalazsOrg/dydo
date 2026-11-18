namespace DynaDocs.SkillBuild;

/// <summary>
/// The build: a model in, every built file out, keyed by its path under the repository root, with
/// the map of each claim-stating skill line: the skills, the machines' chart files and one why doc per
/// kind. A model the rules reject throws <see cref="InvalidDataException"/>; the build never emits a
/// partial tree.
/// </summary>
public static class SkillBuilder
{
    public static BuiltTree Build(Model model)
    {
        var files = new SortedDictionary<string, string>(StringComparer.Ordinal);
        var map = new List<MapLine>();
        var (bySkill, _) = ModelIndex.Read(model);
        var claims = new ClaimIndex(ModelIndex.ClaimIds(model.Kinds));
        var chart = new Statechart(model, claims);
        foreach (var (skill, lines) in bySkill)
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
        // A system transition, a move a command or Linear makes, is carried by no role (AR4).
        StatesEveryClaim(claims.Claims.Where(c => c.Claim is not { Transition: not null, Lines.Count: 0 }).Select(c => c.Id), map);
        foreach (var (path, text) in chart.Files(claims).Concat(WhyDocs.Render(model.Kinds, claims, chart)))
            files.Add(path, text);
        return new BuiltTree(files, map);
    }

    // K4 and DYD-1196 A4's "zero": a declared claim that no line of any built skill states, neither
    // its own line nor a combined line naming it, would vanish from the prose and the map.
    private static void StatesEveryClaim(IEnumerable<string> claims, IEnumerable<MapLine> map)
    {
        var stated = map.SelectMany(line => line.Claims).ToHashSet(StringComparer.Ordinal);
        foreach (var id in claims)
            if (!stated.Contains(id))
                throw new InvalidDataException($"claim:{id} is stated by no line; a claim is stated in every skill that carries it");
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
