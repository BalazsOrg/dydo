namespace DynaDocs.Tests.Services;

using DynaDocs.Models;
using DynaDocs.Services;
using DynaDocs.Utils;
using Xunit;

/// <summary>
/// `dydo check` (ResolveLink) and `dydo graph` (ResolveToRelativeKey) resolve the same link through
/// two entry points; these pin that they agree, and the one place they differ on purpose.
/// </summary>
[Trait("Job", "trap: #0187 (check and graph resolved the same relative link to different documents)")]
public class LinkResolverTests
{
    private readonly LinkResolver _resolver = new();
    private static readonly string BasePath = Path.GetFullPath(Path.Combine(Path.GetTempPath(), "test-docs"));

    #region Cross-resolver agreement (#0187)

    [Theory]
    [InlineData("a/source.md", "./sibling.md", "a/sibling.md")]
    [InlineData("a/source.md", "./_index.md", "a/_index.md")]
    [InlineData("a/b/source.md", "../parent.md", "a/parent.md")]
    [InlineData("a/b/source.md", "../../other.md", "other.md")]
    [InlineData("a/b/c/source.md", "../../foo/bar.md", "a/foo/bar.md")]
    [InlineData("source.md", "./top.md", "top.md")]
    public void Resolve_HandlesEveryRelativeShape_UsedByBothCallers(
        string sourceRel, string target, string expectedKey)
    {
        var source = CreateDoc(sourceRel);
        var targetDoc = CreateDoc(expectedKey);
        var link = LinkTestFactory.Create(target, LinkType.Markdown);
        var allDocs = new List<DocFile> { source, targetDoc };

        var ruleSide = _resolver.ResolveLink(source, link, allDocs, BasePath);
        var graphSide = _resolver.ResolveToRelativeKey(source, link, BasePath);

        Assert.True(ruleSide);
        Assert.Equal(expectedKey, graphSide);
    }

    [Theory]
    [InlineData("./sibling.md#anchor")]
    [InlineData("../sibling.md#anchor")]
    public void Resolve_AgreesOnAnchoredCrossFolderLinks(string fullTarget)
    {
        var (targetPath, anchor) = SplitTarget(fullTarget);
        var source = CreateDoc("a/source.md");
        var targetDoc = CreateDoc(
            targetPath.StartsWith("..") ? "sibling.md" : "a/sibling.md",
            anchors: ["anchor"]);
        var link = LinkTestFactory.CreateWithAnchor(targetPath, anchor, LinkType.Markdown);
        var allDocs = new List<DocFile> { source, targetDoc };

        var ruleSide = _resolver.ResolveLink(source, link, allDocs, BasePath);
        var graphSide = _resolver.ResolveToRelativeKey(source, link, BasePath);

        Assert.True(ruleSide);
        Assert.NotNull(graphSide);
        Assert.Equal(PathUtils.NormalizeForKey(targetDoc.RelativePath), graphSide);
    }

    [Fact]
    public void Resolve_DisagreesIntentionallyOnAnchorOnlyLinks()
    {
        var source = CreateDoc("guide.md", anchors: ["section"]);
        var link = LinkTestFactory.CreateWithAnchor("", "section", LinkType.Markdown);

        Assert.True(_resolver.ResolveLink(source, link, [source], BasePath));
        Assert.Null(_resolver.ResolveToRelativeKey(source, link, BasePath));
    }

    private static (string target, string anchor) SplitTarget(string full)
    {
        var i = full.IndexOf('#');
        return (full[..i], full[(i + 1)..]);
    }

    #endregion

    #region Helpers

    private static DocFile CreateDoc(string relativePath, List<string>? anchors = null)
    {
        var fullPath = Path.GetFullPath(Path.Combine(BasePath, relativePath));
        return new DocFile
        {
            FilePath = fullPath.Replace('\\', '/'),
            RelativePath = relativePath,
            FileName = Path.GetFileName(relativePath),
            Content = "# Test",
            Anchors = anchors ?? []
        };
    }

    #endregion
}
