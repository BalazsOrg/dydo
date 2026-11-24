namespace DynaDocs.Tests.Services;

/// <summary>
/// `dydo check` and `dydo graph` resolve the same link through two entry points. For every relative
/// shape the check finds no broken link and the graph shows the same target, and an anchor-only link
/// passes the check without becoming a graph edge.
/// </summary>
[Collection("Integration")]
public sealed class LinkResolverTests : IDisposable
{
    private const string Frontmatter = "---\narea: guides\ntype: guide\n---\n";

    // Markdig reads a frontmatter block's closing `---` right after its last field as a setext
    // heading, which gives the page one more anchor; a blank line before the closer leaves the
    // page's anchors exactly its headings.
    private const string FrontmatterWithoutAnchor = "---\narea: guides\ntype: guide\n\n---\n";
    private const string AgreementJob = "trap: #0187 (check and graph resolved the same relative link to different documents)";
    private readonly DocsProject _project = new();

    [Theory]
    [Trait("Job", AgreementJob)]
    [InlineData("a/source.md", "./sibling.md", "a/sibling.md")]
    [InlineData("a/source.md", "./_index.md", "a/_index.md")]
    [InlineData("a/b/source.md", "../parent.md", "a/parent.md")]
    [InlineData("a/b/source.md", "../../other.md", "other.md")]
    [InlineData("a/b/c/source.md", "../../foo/bar.md", "a/foo/bar.md")]
    [InlineData("source.md", "./top.md", "top.md")]
    public void Resolve_HandlesEveryRelativeShape_UsedByBothCallers(string source, string target, string expectedKey)
    {
        _project.Write(expectedKey, Frontmatter + "\n# Target\n");
        _project.Write(source, Frontmatter + $"\n# Source\n\n[Target]({target})\n");

        AssertCheckPasses();
        AssertGraphShowsOnly(source, expectedKey);
    }

    [Theory]
    [Trait("Job", AgreementJob)]
    [InlineData("./sibling.md#anchor", "a/sibling.md")]
    [InlineData("../sibling.md#anchor", "sibling.md")]
    public void Resolve_AgreesOnAnchoredCrossFolderLinks(string target, string expectedKey)
    {
        // The target's anchors are exactly the rule case's: ["anchor"].
        _project.Write(expectedKey, FrontmatterWithoutAnchor + "\n# Anchor\n");
        _project.Write("a/source.md", Frontmatter + $"\n# Source\n\n[Target]({target})\n");

        AssertCheckPasses();
        AssertGraphShowsOnly("a/source.md", expectedKey);
    }

    [Fact]
    [Trait("Job", "trap: #0186 (an anchor-only link to a heading on its own page was an empty-target broken link; it passes check, exit 0, and is no graph edge)")]
    public void Resolve_DisagreesIntentionallyOnAnchorOnlyLinks()
    {
        // The page's anchors are exactly the rule case's: ["section"].
        _project.Write("guide.md", FrontmatterWithoutAnchor + "\n# Section\n\n[Here](#section)\n");

        AssertCheckPasses();
        var (code, output) = DydoCli.RunCombined("graph", "guide.md");
        Assert.True(code == 0, $"exit {code}:\n{output}");
        Assert.Contains("  (no outgoing links)", Lines(output));
    }

    private static void AssertCheckPasses()
    {
        var (code, output) = DydoCli.RunCombined("check");
        Assert.True(code == 0, $"exit {code}:\n{output}");
    }

    private static void AssertGraphShowsOnly(string source, string expectedKey)
    {
        var (code, output) = DydoCli.RunCombined("graph", source);
        Assert.True(code == 0, $"exit {code}:\n{output}");
        var lines = Lines(output);
        Assert.Contains($"  [degree 1] {expectedKey}", lines);
        Assert.Contains($"Found 1 docs within 1 hops of {Path.GetFileName(source)}", lines);
    }

    private static string[] Lines(string output) => output.Replace("\r\n", "\n").Split('\n');

    public void Dispose() => _project.Dispose();
}
