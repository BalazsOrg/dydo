namespace DynaDocs.Tests.Rules;

/// <summary>
/// `dydo check` reads `dydo/files-off-limits.md` at the docs root: a missing file, a duplicate
/// pattern or a literal path that does not exist is a warning (exit 0); an unclosed code block,
/// which would hide every pattern after it from the guard, is an error.
/// </summary>
[Collection("Integration")]
public sealed class OffLimitsRuleTests
{
    private const string Head = "---\ntype: config\n---\n\n# Files Off-Limits\n\n";

    [Theory]
    [Trait("Job", "contract: dydo-commands-core.md dydo check (validates the off-limits file)")]
    [InlineData(null, 0, "files-off-limits.md not found")]
    [InlineData(Head + "```\n.env*\n", 1, "Unclosed code block")]
    [InlineData(Head + "```\n.env*\n.env*\n```\n", 0, "Duplicate pattern")]
    [InlineData(Head + "```\n.env*\nconfig/missing.json\n```\n", 0, "does not exist: config/missing.json")]
    [InlineData(Head + "```\n.env*\n```\n", 0, null)]
    public void Check_ReportsTheOffLimitsFinding(string? offLimits, int expectedExit, string? expectedFinding)
    {
        using var project = new DocsProject();
        if (offLimits is null)
            File.Delete(project.DocPath("files-off-limits.md"));
        else
            project.Write("files-off-limits.md", offLimits);

        var (code, output) = DydoCli.RunCombined("check");

        Assert.True(expectedExit == code, $"exit {code}, expected {expectedExit}:\n{output}");
        if (expectedFinding is null)
            Assert.DoesNotContain("files-off-limits.md", output);
        else
            Assert.Contains(expectedFinding, output);
    }

    [Fact]
    [Trait("Job", "contract: dydo-commands-core.md dydo check <path> (a <path> inside the docs tree scopes the report, so the root's off-limits file is not reported)")]
    public void Check_ScopedBelowTheDocsRoot_DoesNotReportTheOffLimitsFile()
    {
        using var project = new DocsProject();
        File.Delete(project.DocPath("files-off-limits.md"));
        project.Write("guides/doc.md", "---\narea: guides\ntype: guide\n---\n\n# Doc\n\nText.\n");

        var (code, output) = DydoCli.RunCombined("check", Path.Combine("dydo", "guides"));

        Assert.True(code == 0, $"exit {code}:\n{output}");
        Assert.DoesNotContain("files-off-limits.md", output);
    }
}
