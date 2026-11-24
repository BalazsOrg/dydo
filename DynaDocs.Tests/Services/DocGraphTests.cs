namespace DynaDocs.Tests.Services;

/// <summary>
/// The document link graph as `dydo graph` shows it: outgoing links to a degree, incoming links,
/// and repository statistics. External links are not edges; a cycle neither repeats nor lists the
/// start document.
/// </summary>
[Collection("Integration")]
public sealed class DocGraphTests : IDisposable
{
    private const string Guide = "---\narea: guides\ntype: guide\n---\n";
    private readonly DocsProject _project = new();

    public DocGraphTests()
    {
        // a -> b -> deep/c, b -> a (a cycle), d -> b, a -> an external page
        _project.Write("guides/a.md", Guide + "\n# A\n\n[B](./b.md) [Ext](https://example.com/page)\n");
        _project.Write("guides/b.md", Guide + "\n# B\n\n[C](./deep/c.md) [A](./a.md)\n");
        _project.Write("guides/deep/c.md", Guide + "\n# C\n\nNo links.\n");
        _project.Write("guides/d.md", Guide + "\n# D\n\n[B](./b.md)\n");
    }

    [Theory]
    [Trait("Job", "contract: dydo-commands-core.md Documentation Commands (`dydo graph` shows the document link graph as the docs hold it)")]
    [InlineData("guides/a.md", "guides/b.md", "guides/deep/c.md")]
    [InlineData("guides/a.md --degree 2", "guides/deep/c.md", "guides/a.md")]
    [InlineData("guides/a.md --degree 2", "guides/b.md", "example.com")]
    [InlineData("guides/b.md --incoming", "guides/a.md", "guides/deep/c.md:")]
    [InlineData("guides/b.md --incoming", "guides/d.md", "index.md:")]
    [InlineData("guides/deep/c.md", "no outgoing links", "guides/")]
    [InlineData("stats", "4 internal links", "example.com")]
    public void Graph_ShowsTheLinkedDocuments(string args, string shown, string notShown)
    {
        var (code, output) = DydoCli.RunCombined(["graph", .. args.Split(' ')]);

        Assert.True(code == 0, $"exit {code}:\n{output}");
        Assert.Contains(shown, output);
        Assert.DoesNotContain(notShown, output);
    }

    [Fact]
    [Trait("Job", "contract: dydo-commands-utilities.md Exit Codes (2 is a tool error): dydo graph on an unknown document path, named in the output")]
    public void Graph_UnknownDocument_IsAToolError()
    {
        var (code, output) = DydoCli.RunCombined("graph", "guides/missing.md");

        Assert.Equal(2, code);
        Assert.Contains("guides/missing.md", output);
    }

    public void Dispose() => _project.Dispose();
}
