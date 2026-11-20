namespace DynaDocs.SkillBuild;

/// <summary>
/// A <see cref="Ledger"/>, checked complete and built for review (DYD-1295 2119cd65): one file per
/// section of the legacy spine under <c>dydo/reference/claim-review/</c>, each claim line and each
/// linked resource with what became of it: kept, withheld with the open claim that holds it, dropped
/// or deferred. Sections keep every file under the token limit.
/// </summary>
internal static class LedgerDocs
{
    /// <summary>The legacy skills a ledger replaces: in the model, checked, and not built.</summary>
    public static HashSet<Enum> Replaced(Model model)
    {
        var replaced = new HashSet<Enum>();
        foreach (var skill in model.Skills.Where(skill => skill.Ledger is not null))
        {
            var legacy = model.Skills.FirstOrDefault(other => Equals(other.Id, skill.Ledger!.Legacy))
                ?? throw new InvalidDataException($"{skill.Name}: its ledger accounts for {Names.Of(skill.Ledger!.Legacy)}, which the model does not declare");
            if (!skill.TruthChecked || legacy.TruthChecked || legacy.IsStandard)
                throw new InvalidDataException($"{skill.Name}: a ledger is a truth-checked skill's account of the legacy skill it replaces");
            if (!replaced.Add(legacy.Id))
                throw new InvalidDataException($"{Names.Of(legacy.Id)} is replaced twice; one ledger accounts for a legacy skill");
        }
        return replaced;
    }

    /// <param name="carried">The ids of the claims <paramref name="skill"/> carries, with whether each is open.</param>
    public static IEnumerable<(string Path, string Text)> Render(
        SkillSpec skill, SkillSpec legacy, SkillLayout layout, IReadOnlyDictionary<string, bool> carried, ClaimIndex claims)
    {
        var sections = Sections(legacy, layout);
        var lines = Keyed<string>(skill, "line", skill.Ledger!.Entries.Where(fate => fate.Resource is null),
            fate => fate.Line is { } line ? claims.IdOf(line) : null, [.. sections.SelectMany(section => section.Lines).Select(line => line.Owner)]);
        var resources = Keyed<Enum>(skill, "resource", skill.Ledger.Entries.Where(fate => fate.Resource is not null),
            fate => fate.Resource, [.. legacy.Resources.Select(resource => resource.Id)]);
        foreach (var fate in skill.Ledger.Entries) Sound(skill, fate, carried, claims);
        for (var index = 0; index < sections.Count; index++)
        {
            var (title, placed, linked) = sections[index];
            var page = new Page(WhyDocs.Frontmatter, $"Ledger: {skill.Name}, {title}");
            page.Block($"What became of each line of the legacy `{legacy.Name}` spine in this section, and of each resource it links.");
            foreach (var line in placed)
            {
                page.Block($"`claim:{line.Owner}`, {Outcome(lines[line.Owner], carried, claims)}");
                page.Block(WhyDocs.Quote(line.Line.Text));
            }
            foreach (var resource in linked)
                page.Block($"Resource `{SkillRenderer.FileOf(resource.Id)}` ({resource.Title}), {Outcome(resources[resource.Id], carried, claims)}");
            yield return ($"{ReviewDocs.Folder}/{skill.Name}-ledger-{index + 1}.md", page.Done("").Text);
        }
    }

    // The legacy spine as it reads: its job paragraph, each phase with its resources, then "When things go wrong" with its scenarios.
    private static List<(string Title, IReadOnlyList<Placed> Lines, IReadOnlyList<Resource> Resources)> Sections(SkillSpec legacy, SkillLayout layout) =>
    [
        ("the job paragraph", layout.In(SpinePart.Job), []),
        .. legacy.Phases.Select((phase, index) =>
            ($"{index + 1}. {phase.Title}", (IReadOnlyList<Placed>)[.. phase.Slots.SelectMany(slot => layout.In(slot.Id))], phase.Resources)),
        (SkillRenderer.WhenThingsGoWrong, layout.In(SpinePart.WhenThingsGoWrong), legacy.Scenarios)
    ];

    // Exactly one entry for each thing the legacy spine holds, and no entry for anything else.
    private static Dictionary<TKey, Fate> Keyed<TKey>(SkillSpec skill, string what, IEnumerable<Fate> entries, Func<Fate, TKey?> key, HashSet<TKey> legacy)
        where TKey : notnull
    {
        var keyed = new Dictionary<TKey, Fate>();
        foreach (var fate in entries)
        {
            if (key(fate) is not { } named || !legacy.Contains(named))
                throw new InvalidDataException($"{skill.Name}: a ledger entry names a {what} the legacy spine does not hold: {Named(key(fate))}");
            if (!keyed.TryAdd(named, fate))
                throw new InvalidDataException($"{skill.Name}: the ledger has two entries for the {what} {Named(named)}");
        }
        var missing = legacy.Where(named => !keyed.ContainsKey(named)).Select(named => Named(named)).ToArray();
        return missing.Length == 0
            ? keyed
            : throw new InvalidDataException(
                $"{skill.Name}: the ledger has no entry for {missing.Length} {what}(s) of the legacy spine: {string.Join(", ", missing)}; each is kept, dropped or deferred");
    }

    private static string Named(object? key) => key switch
    {
        string id => "claim:" + id,
        Enum resource => Names.Of(resource),
        _ => "a claim outside the model"
    };

    private static void Sound(SkillSpec skill, Fate fate, IReadOnlyDictionary<string, bool> carried, ClaimIndex claims)
    {
        var what = Named(fate.Line is { } line ? claims.IdOf(line) : fate.Resource);
        if (fate.Kind != FateKind.Kept)
        {
            if (string.IsNullOrWhiteSpace(fate.Why))
                throw new InvalidDataException($"{skill.Name}: the ledger gives {what} no reason; a dropped or deferred entry says why in one line");
            return;
        }
        if (fate.Resource is not null && !skill.Resources.Any(resource => Equals(resource.Id, fate.ByResource)))
            throw new InvalidDataException($"{skill.Name}: the ledger keeps {what} as a resource the skill does not declare");
        if (fate.Line is not null && (fate.By.Count == 0 || fate.By.Any(by => claims.IdOf(by) is not { } id || !carried.ContainsKey(id))))
            throw new InvalidDataException($"{skill.Name}: the ledger keeps {what} in no claim, or in a claim the skill does not carry");
    }

    private static string Outcome(Fate fate, IReadOnlyDictionary<string, bool> carried, ClaimIndex claims) => fate.Kind switch
    {
        FateKind.Dropped => $"dropped, it fails {fate.Fails}: {fate.Why}",
        FateKind.Deferred => $"deferred: {fate.Why}",
        _ when fate.ByResource is { } resource => $"kept as `{SkillRenderer.FileOf(resource)}`",
        _ => Stated(fate.By.Select(by => claims.IdOf(by)!).ToLookup(id => carried[id]))
    };

    // Kept means a built file states it. An open claim's line is withheld, so what the ledger keeps there no file states yet.
    private static string Stated(ILookup<bool, string> byOpen) => string.Join("; ",
        new[] { (Open: false, Outcome: "kept in "), (Open: true, Outcome: "withheld (open) in ") }.Where(part => byOpen.Contains(part.Open))
            .Select(part => part.Outcome + string.Join(", ", byOpen[part.Open].Select(id => $"`claim:{id}`"))));
}
