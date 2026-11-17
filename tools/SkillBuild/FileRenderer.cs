namespace DynaDocs.SkillBuild;

/// <summary>
/// The raw layout: frontmatter on a spine, the header comment, the H1, one section per kind with one
/// line per single-line unit and multi-line units verbatim, then a spine's links to its resources.
/// </summary>
internal static class FileRenderer
{
    public const string Header = "<!-- Built by tools/SkillBuild from claims/; edit the claims and rebuild. -->";

    // Anthropic's skill authoring best practices: a reference file over 100 lines gets a table of contents.
    private const int ContentsThreshold = 100;

    public static RenderedFile Render(BuiltFileSpec spec, IReadOnlyList<PlacedUnit> units)
    {
        var plain = Render(spec, units, contents: false);
        return !spec.File.IsSpine && plain.LineCount > ContentsThreshold ? Render(spec, units, contents: true) : plain;
    }

    private static RenderedFile Render(BuiltFileSpec spec, IReadOnlyList<PlacedUnit> units, bool contents)
    {
        var lines = new List<string>();
        var map = new List<MapEntry>();
        var file = spec.File;
        if (file.IsSpine) lines.AddRange(["---", $"name: {file.Name}", $"description: {file.Description}", "---"]);
        lines.AddRange([Header, "", $"# {file.Title}", ""]);
        var sections = units.GroupBy(unit => unit.Kind).ToArray();
        if (contents) lines.AddRange(["## Contents", "", ..sections.Select(section => $"- [{section.Key}](#{section.Key})"), ""]);
        foreach (var section in sections)
        {
            lines.AddRange([$"## {section.Key}", ""]);
            foreach (var unit in section) Emit(lines, map, unit);
            Blank(lines);
        }
        var folder = Path.GetDirectoryName(file.Path)!.Length + 1;
        if (spec.Resources.Count > 0)
            lines.AddRange(["## Resources", "", ..spec.Resources.Select(resource => $"- [{resource.Title}]({resource.Path[folder..]})")]);
        while (lines[^1].Length == 0) lines.RemoveAt(lines.Count - 1);
        return new RenderedFile(string.Join('\n', lines) + "\n", map);
    }

    private static void Emit(List<string> lines, List<MapEntry> map, PlacedUnit unit)
    {
        var block = unit.Text.Split('\n');
        if (block.Length == 1)
        {
            lines.Add("- " + unit.Text);
            map.Add(new MapEntry([lines.Count, lines.Count], unit.Id, unit.Claims));
            return;
        }
        Blank(lines);
        var start = lines.Count + 1;
        lines.AddRange(block);
        map.Add(new MapEntry([start, lines.Count], unit.Id, unit.Claims));
        lines.Add("");
    }

    private static void Blank(List<string> lines)
    {
        if (lines[^1].Length != 0) lines.Add("");
    }
}
