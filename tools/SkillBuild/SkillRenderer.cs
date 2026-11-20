namespace DynaDocs.SkillBuild;

/// <summary>
/// Lays one carrier out (K9): a spine of its job paragraph, its numbered job phases, each with its
/// slots and resource links, then "When things go wrong" linking its scenarios; each resource by its
/// slots, a dispatch template opening with its skills (K14); and <c>agents/openai.yaml</c> when the
/// skill is explicit-only or sets its Codex metadata. A standard is one file of sections.
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
            .. skill.Phases.SelectMany(phase => phase.Resources).Select(resource => Resource(resource, layout)),
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
            Slots(page, phase.Slots, "###", layout);
            if (phase.Resources.Count > 0) page.Heading("Resources:");
            foreach (var resource in phase.Resources) page.Link(resource.Title, FileOf(resource.Id));
        }
        var opening = layout.In(SpinePart.WhenThingsGoWrong);
        if (opening.Count + skill.Scenarios.Count > 0) page.Heading("## " + WhenThingsGoWrong);
        foreach (var line in opening) page.State(line);
        foreach (var scenario in skill.Scenarios) page.Link(scenario.Title, FileOf(scenario.Id));
        return page.Done(skill.MainFile);
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
        var file = Slots(Opened(scenario), scenario.Slots, "##", layout).Done(FileOf(scenario.Id));
        return file.LineCount < ScenarioLimit
            ? file
            : throw new InvalidDataException(
                $"{skill.Name}: scenario {Names.Of(scenario.Id)} is {file.LineCount} lines; a scenario stays under {ScenarioLimit} lines");
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
