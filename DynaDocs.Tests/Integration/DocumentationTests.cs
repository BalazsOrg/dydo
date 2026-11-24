namespace DynaDocs.Tests.Integration;

using System.Text.RegularExpressions;
using DynaDocs.Commands;

/// <summary>
/// `dydo fix`, `dydo index` and `dydo graph` on an initialised project, observed as files on disk
/// and exit codes. Document findings are in CheckDocValidatorTests; link graph shape in DocGraphTests.
/// </summary>
[Collection("Integration")]
[Trait("Job", "contract: dydo-commands-core.md dydo fix, dydo index, dydo graph and graph stats; exit 2 is a tool error")]
public partial class DocumentationTests : IntegrationTestBase
{
    [Fact]
    public async Task Fix_OnAnInitialisedProject_WritesNoFileAndChangesNoByte()
    {
        await InitProjectAsync("none");
        WriteFile("dydo/guides/tutorials/first.md", "---\narea: guides\ntype: guide\n---\n\n# First\n");
        WriteFile("dydo/guides/custom/_index.md", "# Custom navigation\n");
        WriteFile("dydo/guides/custom/_custom.md", "---\narea: guides\ntype: folder-meta\n---\n\n# Custom\n");
        WriteFile("dydo/guides/needs-manual.md", "# Needs Manual Fix\n");
        WriteFile("dydo/agents/sample/modes/code-writer.md", "# code-writer");
        var before = Snapshot();

        var fix = await RunAsync(FixCommand.Create());

        Assert.Equal(0, fix.ExitCode);
        Assert.Equal(before, Snapshot());
    }

    [Fact]
    public async Task Index_RegeneratesTheRoot_AndThatRootPassesCheck()
    {
        await InitProjectAsync("none");
        WriteFile("dydo/index.md", "# Old Index\n\nOld content.");

        var index = await RunAsync(IndexCommand.Create());
        var check = await CheckAsync();

        Assert.Equal(0, index.ExitCode);
        var root = ReadFile("dydo/index.md");
        Assert.DoesNotContain("Old content", root);
        Assert.Contains("./understand/", root);
        Assert.True(check.ExitCode == 0, check.Stdout + check.Stderr);
    }

    [Fact]
    public async Task Index_SectionWithoutItsOptionalNavigationPage_IsNotLinkedToIt()
    {
        await InitProjectAsync("none");
        File.Delete(Path.Combine(TestDir, "dydo", "guides", "_guides.md"));

        var index = await RunAsync(IndexCommand.Create());

        Assert.Equal(0, index.ExitCode);
        Assert.DoesNotContain("_guides.md", ReadFile("dydo/index.md"));
    }

    [Theory]
    [InlineData("0")]
    [InlineData("-1")]
    [InlineData("100")]
    public async Task Graph_AnyDegree_GetsAnAnswer(string degree)
    {
        await InitProjectAsync("none");

        var graph = await RunAsync(GraphCommand.Create(), "dydo/index.md", "--degree", degree);

        Assert.Equal(0, graph.ExitCode);
    }

    [Fact]
    public async Task GraphStats_Top_RanksAtMostThatManyDocuments()
    {
        await InitProjectAsync("none");

        var stats = await RunAsync(GraphCommand.Create(), "stats", "--top", "3");

        Assert.Equal(0, stats.ExitCode);
        Assert.InRange(RankedRow().Matches(stats.Stdout).Count, 1, 3);
    }

    [Theory]
    [InlineData("check", "nonexistent/path")]
    [InlineData("index", "nonexistent")]
    [InlineData("graph", "stats")]
    public async Task NoDocsFolder_IsAToolError(string command, string argument)
    {
        System.CommandLine.Command cli = command switch
        {
            "check" => CheckCommand.Create(),
            "index" => IndexCommand.Create(),
            _ => GraphCommand.Create()
        };

        var result = await RunAsync(cli, argument);

        Assert.Equal(2, result.ExitCode);
    }

    private SortedDictionary<string, string> Snapshot() => new(
        Directory.GetFiles(TestDir, "*", SearchOption.AllDirectories)
            .ToDictionary(path => Path.GetRelativePath(TestDir, path), path => Convert.ToBase64String(File.ReadAllBytes(path))),
        StringComparer.Ordinal);

    // A ranked stats row: "  1     2  guides/b.md".
    [GeneratedRegex(@"^\s+\d+\s+\d+\s+\S+\.md\s*$", RegexOptions.Multiline)]
    private static partial Regex RankedRow();
}
