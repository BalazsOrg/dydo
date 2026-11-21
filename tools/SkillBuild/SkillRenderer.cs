namespace DynaDocs.SkillBuild;

/// <summary>
/// Lays one carrier out (K9): a spine of its job paragraph, its numbered job phases, each one numbered
/// checklist of its slots' lines followed by its resource links, then "When things go wrong" linking its
/// scenarios; each resource by its slots, a dispatch template opening with its skills (K14); and
/// <c>agents/openai.yaml</c> when the skill is explicit-only or sets its Codex metadata. A standard is
/// one file of sections. A truth-checked skill keeps the same spine and files, and inside each phase and
/// resource renders every slot in its declared shape (DYD-1295 519126ab): a paragraph, bullets, or steps
/// numbered 1, 2, 3, its title a bold lead-in. Its phases list no resources: the line that uses a
/// resource links it, and a phase resource no line of the spine links fails the build. A resource
/// truth-checked ahead of its skill (<see cref="Resource.Readers"/>) renders in its shapes too, and its legacy spine still lists it.
/// </summary>
internal static class SkillRenderer
{
    // Anthropic's skill authoring best practices: a reference file over 100 lines gets a table of contents.
    private const int ContentsThreshold = 100;

    // K9: "An edge case is a resource, one short file per scenario, under 100 lines." A contents list does not lift it.
    private const int ScenarioLimit = 100;

    public const string WhenThingsGoWrong = "When things go wrong";

    public static IEnumerable<RenderedFile> Render(SkillSpec skill, SkillLayout layout)
    {
        if (skill.IsStandard)
            return [Slots(new Page(skill.Frontmatter, skill.Title), skill.Sections, "##", layout).Done(skill.MainFile)];
        return [
            Spine(skill, layout),
            .. skill.Phases.SelectMany(phase => phase.Resources).Select(resource => skill.Checks(resource) ? Shaped(skill, resource, layout) : Resource(resource, layout)),
            .. skill.Scenarios.Select(scenario => Scenario(skill, scenario, layout)),
            .. OpenAi(skill)
        ];
    }

    private static RenderedFile Spine(SkillSpec skill, SkillLayout layout)
    {
        var page = new Page(skill.Frontmatter, skill.Title);
        foreach (var line in layout.In(SpinePart.Job)) page.Sentence(line);
        foreach (var (phase, index) in skill.Phases.Select((phase, index) => (phase, index)))
        {
            page.Heading($"## {index + 1}. {phase.Title}");
            // K9 supersedes DR 064 §7's state outline as the spine's grouping (review r1 S5): no slot heading and
            // no move label, only the slots in order, a state's arrivals, then its nominal path, then its exits.
            if (skill.TruthChecked) Shaped(page, phase.Slots, layout);
            else foreach (var line in phase.Slots.SelectMany(slot => layout.In(slot.Id))) page.Step(line);
            // A truth-checked phase has no list of its resources: the line that uses one links it (DYD-1372 hop 2).
            if (skill.TruthChecked) continue;
            if (phase.Resources.Count > 0) page.Heading("Resources:");
            foreach (var resource in phase.Resources) page.Link(resource.Title, FileOf(resource.Id));
        }
        var opening = layout.In(SpinePart.WhenThingsGoWrong);
        if (opening.Count + skill.Scenarios.Count > 0) page.Heading("## " + WhenThingsGoWrong);
        foreach (var line in opening) page.State(line);
        foreach (var scenario in skill.Scenarios) page.Link(scenario.Title, FileOf(scenario.Id));
        var spine = page.Done(skill.MainFile);
        if (skill.TruthChecked) LinksEachPhaseResource(skill, spine);
        return spine;
    }

    // A resource no line links would be a file nothing in the spine leads to.
    private static void LinksEachPhaseResource(SkillSpec skill, RenderedFile spine)
    {
        foreach (var file in skill.Phases.SelectMany(phase => phase.Resources).Select(resource => FileOf(resource.Id)))
            if (!spine.Stated.Any(line => line.Text.Contains($"]({file})", StringComparison.Ordinal)))
                throw new InvalidDataException(
                    $"{skill.Name}: no line of the spine links {file}; a truth-checked skill links a phase's resource from the line that uses it");
    }

    private static RenderedFile Resource(Resource resource, SkillLayout layout)
    {
        var plain = Slots(Opened(resource), resource.Slots, "##", layout).Done(FileOf(resource.Id));
        if (plain.LineCount <= ContentsThreshold) return plain;
        var lead = resource.Slots.TakeWhile(slot => slot.Title is null).ToArray();
        var titled = resource.Slots.Skip(lead.Length).ToArray();
        var page = Slots(Opened(resource), lead, "##", layout);
        page.Heading("## Contents");
        foreach (var slot in titled)
            page.Link(slot.Title!, "#" + Names.Anchor(slot.Title!));
        return Slots(page, titled, "##", layout).Done(FileOf(resource.Id));
    }

    private static RenderedFile Scenario(SkillSpec skill, Resource scenario, SkillLayout layout)
    {
        var file = skill.Checks(scenario) ? Shaped(skill, scenario, layout) : Slots(Opened(scenario), scenario.Slots, "##", layout).Done(FileOf(scenario.Id));
        return file.LineCount < ScenarioLimit
            ? file
            : throw new InvalidDataException(
                $"{skill.Name}: scenario {Names.Of(scenario.Id)} is {file.LineCount} lines; a scenario stays under {ScenarioLimit} lines");
    }

    // A truth-checked resource reads like a phase: its slots in their shapes, no section headings, so it stays one short read.
    private static RenderedFile Shaped(SkillSpec skill, Resource resource, SkillLayout layout)
    {
        var file = Shaped(Opened(resource), resource.Slots, layout).Done(FileOf(resource.Id));
        return file.LineCount <= ContentsThreshold
            ? file
            : throw new InvalidDataException(
                $"{skill.Name}: resource {Names.Of(resource.Id)} is {file.LineCount} lines; a truth-checked resource stays within {ContentsThreshold} lines");
    }

    // DYD-1295 519126ab: each slot in its declared shape, its title a bold lead-in; a slot with no line to show is left out, lead-in and all.
    private static Page Shaped(Page page, IEnumerable<Slot> slots, SkillLayout layout)
    {
        foreach (var slot in slots)
        {
            var lines = layout.In(slot.Id);
            if (lines.Count == 0) continue;
            page.Open(slot.Title, slot.Shape == SlotShape.Paragraph);
            for (var index = 0; index < lines.Count; index++) Shaped(page, slot.Shape, index + 1, lines[index]);
        }
        return page;
    }

    private static void Shaped(Page page, SlotShape? shape, int number, Placed line)
    {
        if (shape == SlotShape.Paragraph) page.Prose(line);
        else if (shape == SlotShape.Steps) page.Numbered(number, line);
        else page.State(line);
    }

    // A dispatch template's first line names its skill set, each with a slash (K14).
    private static Page Opened(Resource resource)
    {
        var page = new Page([], resource.Title);
        if (resource.Skills.Count > 0) page.Block("Skills: " + string.Join(' ', resource.Skills.Select(skill => "/" + skill)));
        return page;
    }

    private static Page Slots(Page page, IEnumerable<Slot> slots, string level, SkillLayout layout)
    {
        foreach (var slot in slots)
        {
            if (slot.Title is not null) page.Heading($"{level} {slot.Title}");
            if (!slot.IsState)
                foreach (var placed in layout.In(slot.Id)) page.State(placed);
            else
                foreach (var step in Enum.GetValues<MoveKind>())
                    Moves(page, step, [.. layout.In(slot.Id).Where(placed => placed.Line.Place.Step == step)]);
        }
        return page;
    }

    private static void Moves(Page page, MoveKind step, IReadOnlyList<Placed> moves)
    {
        if (moves.Count == 0) return;
        page.Heading(step switch { MoveKind.Arrival => "Arrivals:", MoveKind.Nominal => "Nominal path:", _ => "Exits:" });
        foreach (var move in moves) page.State(move);
    }

    private static IEnumerable<RenderedFile> OpenAi(SkillSpec skill)
    {
        if (!skill.ExplicitOnly && skill.OpenAi is null) yield break;
        var yaml = new List<string> { BuiltHeader.Yaml };
        yaml.AddRange(Block("interface", skill.OpenAi?.Interface));
        yaml.AddRange(Block("dependencies", skill.OpenAi?.Dependencies));
        if (skill.ExplicitOnly) yaml.AddRange(["policy:", "  allow_implicit_invocation: false"]);
        yield return new RenderedFile("agents/openai.yaml", string.Join('\n', yaml) + "\n", []);
    }

    private static IEnumerable<string> Block(string key, string? body) => body is null
        ? []
        : [key + ":", .. body.TrimEnd('\n').Split('\n').Select(line => "  " + line)];

    internal static string FileOf(Enum resource) => $"resources/{Names.Kebab(resource)}.md";
}
