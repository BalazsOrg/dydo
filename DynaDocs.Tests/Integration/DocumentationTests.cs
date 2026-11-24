namespace DynaDocs.Tests.Integration;

using System.Text.RegularExpressions;
using DynaDocs.Commands;

/// <summary>
/// `dydo fix`, `dydo index` and `dydo graph` on an initialised project, observed as files on disk
/// and exit codes. Document findings are in CheckDocValidatorTests; link graph shape in DocGraphTests.
/// </summary>
[Collection("Integration")]
public partial class DocumentationTests : IntegrationTestBase
{
    private const string ToolErrorJob = "contract: dydo-commands-utilities.md Exit Codes (2 is a tool error)";

    [Fact]
    [Trait("Job", "contract: dydo-commands-core.md dydo fix (supported repairs only: an initialised project, its navigation pages and a document needing a manual fix gain no file and change no byte, exit 0)")]
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
    [Trait("Job", "contract: dydo-commands-core.md dydo index (regenerates the root index from the configured structure, and dydo check passes it)")]
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
    [Trait("Job", "contract: dydo-commands-core.md dydo index; writing-docs.md Rules Enforced (navigation pages are optional, so a missing one is not linked)")]
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
    [Trait("Job", "contract: dydo-commands-core.md dydo graph stats --top N (ranks at most N documents)")]
    public async Task GraphStats_Top_RanksAtMostThatManyDocuments()
    {
        await InitProjectAsync("none");

        var stats = await RunAsync(GraphCommand.Create(), "stats", "--top", "3");

        Assert.Equal(0, stats.ExitCode);
        Assert.InRange(RankedRow().Matches(stats.Stdout).Count, 1, 3);
    }

    [Theory]
    [Trait("Job", ToolErrorJob + ": check, index and graph stats where no docs folder is found")]
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

    [Fact]
    [Trait("Job", ToolErrorJob + ": dydo graph on an unknown bare filename")]
    public async Task Graph_UnknownFile_IsAToolError()
    {
        await InitProjectAsync("none");

        var result = await RunAsync(GraphCommand.Create(), "nonexistent.md");

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
