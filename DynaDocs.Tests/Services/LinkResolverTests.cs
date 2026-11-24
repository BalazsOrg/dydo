namespace DynaDocs.Tests.Services;

/// <summary>
/// `dydo check` and `dydo graph` resolve the same link through two entry points. For every relative
/// shape the check finds no broken link and the graph shows the same target, and an anchor-only link
/// passes the check without becoming a graph edge.
/// </summary>
[Collection("Integration")]
[Trait("Job", "trap: #0187 (check and graph resolved the same relative link to different documents)")]
public sealed class LinkResolverTests : IDisposable
{
    private const string Frontmatter = "---\narea: guides\ntype: guide\n---\n";
    private readonly DocsProject _project = new();

    [Theory]
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
    [InlineData("./sibling.md#anchor", "a/sibling.md")]
    [InlineData("../sibling.md#anchor", "sibling.md")]
    public void Resolve_AgreesOnAnchoredCrossFolderLinks(string target, string expectedKey)
    {
        _project.Write(expectedKey, Frontmatter + "\n# Target\n\n## Anchor\n\nText.\n");
        _project.Write("a/source.md", Frontmatter + $"\n# Source\n\n[Target]({target})\n");

        AssertCheckPasses();
        AssertGraphShowsOnly("a/source.md", expectedKey);
    }

    [Fact]
    public void Resolve_DisagreesIntentionallyOnAnchorOnlyLinks()
    {
        _project.Write("guide.md", Frontmatter + "\n# Guide\n\n## Section\n\n[Here](#section)\n");

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
