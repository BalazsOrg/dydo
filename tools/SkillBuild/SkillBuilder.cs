namespace DynaDocs.SkillBuild;

/// <summary>
/// The build: a model in, every built file out, keyed by its path under the repository root, with
/// the map of each claim-stating skill line: the skills, the machines' chart files, one why doc per
/// kind and the review lists. A model the rules reject throws <see cref="InvalidDataException"/>; the
/// build never emits a partial tree.
/// </summary>
public static class SkillBuilder
{
    public static BuiltTree Build(Model model)
    {
        var files = new SortedDictionary<string, string>(StringComparer.Ordinal);
        var unbuilt = new SortedDictionary<string, string>(StringComparer.Ordinal);
        var map = new List<MapLine>();
        // Every claim id a line states, built or not: a legacy line a ledger replaces and a withheld open line still state theirs.
        var stated = new HashSet<string>(StringComparer.Ordinal);
        var layouts = new Dictionary<SkillSpec, SkillLayout>();
        var open = new Dictionary<SkillSpec, HashSet<Placed>>();
        model.Envoys?.Check(model);
        var (bySkill, _) = ModelIndex.Read(model);
        var claims = new ClaimIndex(ModelIndex.ClaimIds(model.Kinds));
        var chart = new Statechart(model, claims);
        var replaced = LedgerDocs.Replaced(model);
        TruthCheck.Answers(claims, model.Skills);
        chart.OwnsItsArrows(claims);
        foreach (var (skill, lines) in bySkill)
        {
            open[skill] = TruthCheck.Check(skill, lines, claims.ById);
            layouts[skill] = new SkillLayout(skill, lines, open[skill]);
            var rendered = SkillRenderer.Render(skill, layouts[skill]).ToArray();
            StatesEachClaimOnce(skill, lines.Where(placed => !open[skill].Contains(placed)), rendered);
            stated.UnionWith(lines.SelectMany(placed => placed.Claims));
            foreach (var (path, file) in rendered.SelectMany(file => skill.PathsOf(file.Local).Select(path => (path, file))))
                if (replaced.Contains(skill.Id)) unbuilt.Add(path, file.Text);
                else Emit(skill, path, file, files, map);
        }
        // A system transition, a move a command or Linear makes, is carried by no role (AR4).
        StatesEveryClaim(claims.Claims.Where(c => c.Claim is not { Transition: not null, Lines.Count: 0 }).Select(c => c.Id), stated);
        model.Envoys?.CheckBuilt(model, claims, files, map);
        foreach (var (path, text) in chart.Files(claims).Concat(WhyDocs.Render(model.Kinds, claims, chart, replaced)).Concat(Review(bySkill, replaced, layouts, open, claims)))
            files.Add(path, text);
        var unwritten = new SortedDictionary<string, string>(chart.Files(claims, handWritten: true).ToDictionary(file => file.Path, file => file.Text), StringComparer.Ordinal);
        return new BuiltTree(files, map, unbuilt) { Unwritten = unwritten };
    }

    private static void Emit(SkillSpec skill, string path, RenderedFile file, SortedDictionary<string, string> files, List<MapLine> map)
    {
        // The whole file, so a lead-in, a title and the frontmatter are read as a claim line is (DYD-1372 C).
        if ((skill.TruthChecked || skill.Ahead.Any(resource => SkillRenderer.FileOf(resource.Id) == file.Local)) && (TruthCheck.Misnumbered(path, file.Text) ?? TruthCheck.Miscommanded(path, file.Text)) is { } problem)
            throw new InvalidDataException(problem);
        if (!files.TryAdd(path, file.Text))
            throw new InvalidDataException($"{path} is built twice; two skills share a folder only when one replaces the other through its ledger");
        map.AddRange(file.Stated.Select(line => new MapLine(path, line.Line, line.Text, line.Placed.Claims)));
    }

    // DYD-1372 A4, A7, F: each truth-checked skill's open claims and its ledger, the open claims of each resource truth-checked ahead of
    // its skill (DYD-1402), the legacy lines that name a missing command or an unbuilt resource, then every skill not yet migrated.
    private static IEnumerable<(string Path, string Text)> Review(Dictionary<SkillSpec, List<Placed>> bySkill, HashSet<Enum> replaced,
        Dictionary<SkillSpec, SkillLayout> layouts, Dictionary<SkillSpec, HashSet<Placed>> open, ClaimIndex claims)
    {
        foreach (var (skill, lines) in bySkill.Where(pair => pair.Key.TruthChecked))
        {
            // An open claim is stated alone, so its line's own claim is the open one: withheld, or the one placeholder.
            yield return ReviewDocs.Open(skill.Name, "skill", lines.Where(placed => claims.ById[placed.Owner].Truth!.Evidence.Kind == EvidenceKind.Open), claims);
            if (skill.Ledger is null) continue;
            var legacy = bySkill.Keys.First(other => Equals(other.Id, skill.Ledger.Legacy));
            var carried = lines.SelectMany(placed => placed.Claims.Select(id => (id, Open: open[skill].Contains(placed)))).ToDictionary(pair => pair.id, pair => pair.Open);
            foreach (var file in LedgerDocs.Render(skill, legacy, layouts[legacy], carried, claims)) yield return file;
        }
        foreach (var (skill, lines) in bySkill)
            foreach (var resource in skill.Ahead)
                yield return ReviewDocs.Open($"{skill.Name}/{Names.Kebab(resource.Id)}", "resource",
                    lines.Where(placed => ReferenceEquals(skill.AheadOf(placed.Line), resource) && claims.ById[placed.Owner].Truth!.Evidence.Kind == EvidenceKind.Open), claims);
        foreach (var file in ReviewDocs.Commands(bySkill.Where(pair => !pair.Key.TruthChecked)
                     .Select(pair => (pair.Key, !replaced.Contains(pair.Key.Id), (IReadOnlyList<Placed>)pair.Value))))
            yield return file;
        foreach (var file in ReviewDocs.Unbuilt(
                     bySkill.Where(pair => !pair.Key.TruthChecked && !replaced.Contains(pair.Key.Id)).Select(pair => (pair.Key, (IReadOnlyList<Placed>)pair.Value)),
                     bySkill.Keys.Where(skill => skill.Ledger is not null).Select(skill => (bySkill.Keys.First(other => Equals(other.Id, skill.Ledger!.Legacy)), skill))))
            yield return file;
        foreach (var file in ReviewDocs.Unmigrated(
                     bySkill.Where(pair => !pair.Key.TruthChecked && !replaced.Contains(pair.Key.Id)).Select(pair => (pair.Key, (IReadOnlyList<Placed>)pair.Value)),
                     bySkill.Keys.Any(skill => skill.TruthChecked || skill.Ahead.Any())))
            yield return file;
    }

    // K4 and DYD-1196 A4's "zero": a declared claim that no line of any skill states, neither its own
    // line nor a combined line naming it, would vanish from the prose and the map.
    private static void StatesEveryClaim(IEnumerable<string> claims, HashSet<string> stated)
    {
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
