namespace DynaDocs.SkillBuild;

/// <summary>
/// A Markdown file as it is built: frontmatter, the header, the H1, then headings and lines. A
/// one-line claim line is one bullet, or one sentence line of a paragraph, so a rebuild's diff touches only the lines that changed (K18);
/// a multi-line one is emitted verbatim, with any fence it leaves open closed.
/// </summary>
internal sealed class Page
{
    private readonly List<string> _lines = [];
    private readonly List<(int Line, string Text, Placed Placed)> _stated = [];

    public Page(IReadOnlyList<string> frontmatter, string title)
    {
        if (frontmatter.Count > 0) _lines.AddRange(["---", .. frontmatter, "---"]);
        _lines.AddRange([BuiltHeader.Markdown, "", "# " + title, ""]);
    }

    public void Heading(string text) => Block(text);

    /// <summary>Lines that state no claim, set off by blank lines.</summary>
    public void Block(params string[] lines)
    {
        Blank();
        _lines.AddRange([.. lines, ""]);
    }

    public void Link(string title, string target) => _lines.Add($"- [{title}]({target})");

    /// <summary>A one-line claim line as plain text: consecutive sentences read as one paragraph.</summary>
    public void Sentence(Placed placed)
    {
        _lines.Add(placed.Line.Text);
        _stated.Add((_lines.Count, placed.Line.Text, placed));
    }

    /// <summary>A claim line as a bullet.</summary>
    public void State(Placed placed) => Item("- ", placed);

    /// <summary>
    /// A claim line as a numbered checklist item. Every item is numbered 1, which Markdown numbers in
    /// order, so a line added or dropped renumbers no other (K18).
    /// </summary>
    public void Step(Placed placed) => Item("1. ", placed);

    private void Item(string marker, Placed placed)
    {
        var block = placed.Line.Text.Split('\n');
        var fence = MarkdownFence.Unclosed(block);
        if (block.Length == 1 && fence is null)
        {
            _lines.Add(marker + placed.Line.Text);
            _stated.Add((_lines.Count, placed.Line.Text, placed));
            return;
        }
        Blank();
        foreach (var line in block)
        {
            _lines.Add(line);
            _stated.Add((_lines.Count, line, placed));
        }
        if (fence is not null) _lines.Add(fence);
        _lines.Add("");
    }

    public RenderedFile Done(string local)
    {
        while (_lines[^1].Length == 0) _lines.RemoveAt(_lines.Count - 1);
        return new RenderedFile(local, string.Join('\n', _lines) + "\n", _stated);
    }

    private void Blank()
    {
        if (_lines[^1].Length != 0) _lines.Add("");
    }
}
