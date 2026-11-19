namespace DynaDocs.SkillBuild;

/// <summary>
/// Lays one carrier out (K9): a spine of header parts, states with their moves, routines, sections
/// and its resource list; each resource by its sections; each appendix; and <c>agents/openai.yaml</c>
/// when the skill is explicit-only or sets its Codex metadata. A standard is one file of sections.
/// </summary>
internal static class SkillRenderer
{
    // Anthropic's skill authoring best practices: a reference file over 100 lines gets a table of contents.
    private const int ContentsThreshold = 100;

    // K9 / DR 064 §9: "Appendices stay under 100 lines." A contents list does not lift it.
    private const int AppendixLimit = 100;

    public static IEnumerable<RenderedFile> Render(SkillSpec skill, SkillLayout layout)
    {
        if (skill.IsStandard)
            return [Sections(new Page(skill.Frontmatter, skill.Title), skill.Sections, layout).Done(skill.MainFile)];
        return [
            Spine(skill, layout),
            .. skill.Resources.Select(resource => Resource(resource, layout)),
            .. skill.Appendices.Select(appendix => Appendix(skill, appendix, layout)),
            .. OpenAi(skill)
        ];
    }

    private static RenderedFile Spine(SkillSpec skill, SkillLayout layout)
    {
        var page = new Page(skill.Frontmatter, skill.Title);
        foreach (var part in Enum.GetValues<HeaderPart>())
            if (layout.In(part) is { Count: > 0 } lines) Lines(page, PartTitle(part), lines);
        if (skill.States.Count > 0) page.Heading("## States");
        foreach (var state in skill.States)
        {
            page.Heading("### " + state.Title);
            foreach (var step in Enum.GetValues<MoveKind>())
                Moves(page, skill, step, [.. layout.In(state.Id).Where(placed => placed.Line.Place.Step == step)]);
        }
        if (skill.Routines.Count > 0) page.Heading("## Routines");
        foreach (var routine in skill.Routines)
            Lines(page, "### " + routine.Title, layout.In(routine.Id));
        Sections(page, skill.Sections, layout);
        if (skill.Resources.Count > 0) page.Heading("## Resources");
        foreach (var resource in skill.Resources)
            page.Link(resource.Title, FileOf(resource.Id));
        return page.Done(skill.MainFile);
    }

    private static void Moves(Page page, SkillSpec skill, MoveKind step, IReadOnlyList<Placed> moves)
    {
        if (moves.Count == 0) return;
        page.Heading(step switch { MoveKind.Arrival => "Arrivals:", MoveKind.Nominal => "Nominal path:", _ => "Exits:" });
        foreach (var move in moves)
            page.State(move, move.Line.Place.Appendix is { } appendix
                ? $" ([{skill.Appendices.First(slot => Equals(slot.Id, appendix)).Title}]({FileOf(appendix)}))"
                : "");
    }

    private static RenderedFile Resource(Resource resource, SkillLayout layout)
    {
        var plain = Sections(new Page([], resource.Title), resource.Sections, layout).Done(FileOf(resource.Id));
        if (plain.LineCount <= ContentsThreshold) return plain;
        var lead = resource.Sections.TakeWhile(section => section.Title is null).ToArray();
        var titled = resource.Sections.Skip(lead.Length).ToArray();
        var page = Sections(new Page([], resource.Title), lead, layout);
        page.Heading("## Contents");
        foreach (var section in titled)
            page.Link(section.Title!, "#" + Names.Anchor(section.Title!));
        return Sections(page, titled, layout).Done(FileOf(resource.Id));
    }

    private static RenderedFile Appendix(SkillSpec skill, Slot appendix, SkillLayout layout)
    {
        var page = new Page([], appendix.Title!);
        foreach (var placed in layout.In(appendix.Id)) page.State(placed);
        var file = page.Done(FileOf(appendix.Id));
        return file.LineCount < AppendixLimit
            ? file
            : throw new InvalidDataException(
                $"{skill.Name}: appendix {Names.Of(appendix.Id)} is {file.LineCount} lines; an appendix stays under {AppendixLimit} lines");
    }

    private static Page Sections(Page page, IEnumerable<Slot> sections, SkillLayout layout)
    {
        foreach (var section in sections)
            Lines(page, section.Title is null ? null : "## " + section.Title, layout.In(section.Id));
        return page;
    }

    private static void Lines(Page page, string? heading, IReadOnlyList<Placed> lines)
    {
        if (heading is not null) page.Heading(heading);
        foreach (var placed in lines) page.State(placed);
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

    private static string PartTitle(HeaderPart part) => part switch
    {
        HeaderPart.Role => "## Role",
        HeaderPart.Accountable => "## Accountable for",
        _ => "## Tools and surfaces"
    };

    internal static string FileOf(Enum resource) => $"resources/{Names.Kebab(resource)}.md";
}
