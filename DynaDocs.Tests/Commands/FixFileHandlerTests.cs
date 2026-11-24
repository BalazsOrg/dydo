namespace DynaDocs.Tests.Commands;

/// <summary>
/// `dydo fix` repairs files on disk: a resolvable wikilink becomes a relative link and a
/// non-kebab file is renamed. It never overwrites an existing file and never touches a document it
/// only resolved against. A valid document's bytes: optional-summaries.feature.
/// </summary>
[Collection("Integration")]
public sealed class FixFileHandlerTests
{
    private const string Guide = "---\narea: guides\ntype: guide\n---\n";

    [Fact]
    [Trait("Job", "contract: dydo-commands-core.md dydo fix (wikilink conversion: a resolvable wikilink becomes a relative link, an unresolvable one stays)")]
    public void Fix_ConvertsResolvableWikilinks_AndLeavesTheTargetUntouched()
    {
        using var project = new DocsProject();
        var target = project.Write("reference/target.md", "---\narea: reference\ntype: reference\n---\n\n# Target\n");
        project.Write("guides/source.md", Guide + "\n# Source\n\nSee [[target]] and [[nowhere]].\n");
        var targetBytes = File.ReadAllBytes(target);

        DydoCli.RunCombined("fix");

        var source = project.Read("guides/source.md");
        Assert.Contains("[target](../reference/target.md)", source);
        Assert.Contains("[[nowhere]]", source);
        Assert.Equal(targetBytes, File.ReadAllBytes(target));
    }

    [Fact]
    [Trait("Job", "trap: dydo fix rename onto an existing file loses it; contract: dydo-commands-core.md dydo fix (filename normalization)")]
    public void Fix_RenamesNonKebabFiles_ButNeverOntoAnExistingFile()
    {
        using var project = new DocsProject();
        project.Write("guides/My Task.md", Guide + "\n# First\n");
        project.Write("guides/my-task.md", Guide + "\n# Existing\n");
        project.Write("guides/Other Thing.md", Guide + "\n# Other\n");

        DydoCli.RunCombined("fix");

        Assert.Equal(Guide + "\n# First\n", project.Read("guides/My Task.md"));
        Assert.Equal(Guide + "\n# Existing\n", project.Read("guides/my-task.md"));
        Assert.False(File.Exists(project.DocPath("guides/Other Thing.md")));
        Assert.Equal(Guide + "\n# Other\n", project.Read("guides/other-thing.md"));
    }
}
