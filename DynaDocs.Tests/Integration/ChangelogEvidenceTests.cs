namespace DynaDocs.Tests.Integration;

using DynaDocs.Commands;
using DynaDocs.Services;
using static DynaDocs.Tests.Integration.PushedRepository;

[Collection("Integration")]
[Trait("Job", "trap: DYD-1206 committed gate receipts passed check and pr-ready")]
public sealed class ChangelogEvidenceTests : IDisposable
{
    private readonly string _root = Path.Combine(Path.GetTempPath(), "dydo-evidence-" + Guid.NewGuid().ToString("N"));
    private readonly string _tree;

    public ChangelogEvidenceTests()
    {
        _tree = Path.Combine(_root, "tree");
        Create(_tree, Path.Combine(_root, "origin.git"));
        Directory.CreateDirectory(Path.Combine(_tree, "dydo", "project", "changelog"));
        File.WriteAllText(Path.Combine(_tree, "dydo.json"),
            """{"version":1,"structure":{"root":"dydo"},"scanExclude":["_system/.local/","agents/"]}""");
        File.WriteAllText(Path.Combine(_tree, "dydo", "index.md"),
            "---\narea: general\ntype: hub\n---\n\n# Index\n\nDocumentation.\n");
    }

    [Theory]
    [InlineData("gate.log", 0)]
    [InlineData("results.trx", 0)]
    [InlineData("coverage.xml", 0)]
    [InlineData("receipt.tar.gz", 0)]
    [InlineData("receipt", 0)]
    [InlineData("receipt.md", 1)]
    [InlineData("invalid.md", 2)]
    [InlineData("evidence/report.md", 0)]
    public async Task CommittedReceipt_RefusedByBothCommands_EvenWhenCheckScopeIsElsewhere(string name, int content)
    {
        var relative = "dydo/project/changelog/" + name;
        var file = Path.Combine(_tree, relative);
        Directory.CreateDirectory(Path.GetDirectoryName(file)!);
        File.WriteAllBytes(file, content switch { 1 => [0, 1, 2], 2 => [255], _ => "gate output\n"u8.ToArray() });
        Commit(_tree, "commit gate evidence fixture");
        Git(_tree, "push");

        var (check, _, checkError) = ConsoleCapture.All(() =>
            CheckCommand.Create().Parse(["--root", _tree, Path.Combine(_tree, "dydo", "index.md")]).Invoke());
        var (ready, _, readyError) = await ConsoleCapture.AllAsync(() =>
            PrReadyCommand.Create().Parse([Path.Combine(_tree, "dydo")]).InvokeAsync());

        Assert.Equal((1, 1), (check, ready));
        Assert.Contains("gate evidence", checkError);
        Assert.Contains(relative, checkError);
        Assert.Contains("gate evidence", readyError);
        Assert.Contains(relative, readyError);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task SymlinkedProjectPath_CannotHideCommittedEvidence(bool linkedParent)
    {
        File.WriteAllText(Path.Combine(_tree, "dydo", "project", "changelog", "gate.log"), "gate output");
        Commit(_tree, "commit receipt for symlink-root trap");
        Git(_tree, "push");
        var alias = Path.Combine(_root, "alias");
        DirectoryLinks.Create(alias, linkedParent ? _root : _tree);
        var project = linkedParent ? Path.Combine(alias, "tree") : alias;

        var (check, _, checkError) = ConsoleCapture.All(() =>
            CheckCommand.Create().Parse(["--root", project]).Invoke());
        var (ready, _, readyError) = await ConsoleCapture.AllAsync(() =>
            PrReadyCommand.Create().Parse([project]).InvokeAsync());

        Assert.Equal((1, 1), (check, ready));
        Assert.Contains("gate evidence", checkError);
        Assert.Contains("gate evidence", readyError);
    }

    [Fact]
    public async Task HumanMarkdownAndIgnoredLocalOutput_AreAllowed()
    {
        File.WriteAllText(Path.Combine(_tree, "dydo", "project", "changelog", "record.md"),
            "---\narea: project\ntype: changelog\ndate: 2026-11-18\n---\n\n# Change\n\nA human-written account.\n");
        File.AppendAllText(Path.Combine(_tree, ".gitignore"), "\nlocal-output/\n");
        Commit(_tree, "human record");
        Git(_tree, "push");
        Directory.CreateDirectory(Path.Combine(_tree, "local-output"));
        File.WriteAllText(Path.Combine(_tree, "local-output", "gate.log"), "local receipt");

        var (check, _, error) = ConsoleCapture.All(() =>
            CheckCommand.Create().Parse(["--root", _tree]).Invoke());
        var (ready, _, _) = await ConsoleCapture.AllAsync(() =>
            PrReadyCommand.Create().Parse([_tree]).InvokeAsync());

        Assert.Equal(0, check);
        Assert.DoesNotContain("gate evidence", error);
        Assert.Equal(0, ready);
    }

    public void Dispose() => LinkSafeDelete.DeleteTree(_root);
}
