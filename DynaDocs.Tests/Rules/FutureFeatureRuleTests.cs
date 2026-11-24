namespace DynaDocs.Tests.Rules;

/// <summary>
/// `dydo check` on a legacy FutureFeature record under `project/future-features/`: an idea or a
/// promoted record with one Linear reference passes; a missing or extra reference, a delivery
/// field, or a missing Rationale or Related section is an error. Sections inside code do not count.
/// </summary>
[Collection("Integration")]
public sealed class FutureFeatureRuleTests
{
    private const string Idea = "---\narea: project\ntype: concept\nstatus: idea\n---\n\n# Idea\n";
    private const string Promoted = "---\narea: project\ntype: concept\nstatus: promoted\n";
    private const string Sections = "\n## Rationale\n\nWorth retaining.\n\n## Related\n\n[Decision](../decisions/001-one.md)\n";
    private const string Fenced = "\n```md\n## Rationale\n\nExample.\n\n## Related\n\n[Decision](../decisions/001-one.md)\n```\n";

    [Theory]
    [Trait("Job", "contract: dydo-commands-core.md dydo check (legacy FutureFeature shape under project/future-features/)")]
    [InlineData(Idea + Sections, 0, null)]
    [InlineData(Promoted + "linear-reference: https://linear.app/example/issue/DYD-34/close-validators\n---\n\n# Idea\n" + Sections, 0, null)]
    [InlineData(Promoted + "linear-reference: https://linear.app/example/project/dydo-30-123456789abc\n---\n\n# Idea\n" + Sections, 0, null)]
    [InlineData(Promoted + "---\n\n# Idea\n" + Sections, 1, "linear-reference")]
    [InlineData("---\narea: project\ntype: concept\nstatus: idea\nlinear-reference: https://linear.app/example/issue/DYD-34\n---\n\n# Idea\n" + Sections, 1, "linear-reference")]
    [InlineData(Promoted + "linear-reference: https://linear.app/example/issue/DYD-34/close_validators\n---\n\n# Idea\n" + Sections, 1, "linear-reference")]
    [InlineData(Promoted + "linear-reference: https://linear.app/example/issue/DYD-34\nlinear-reference: https://linear.app/example/issue/DYD-35\n---\n\n# Idea\n" + Sections, 1, "linear-reference")]
    [InlineData("---\narea: project\ntype: concept\nstatus: idea\npriority: high\n---\n\n# Idea\n" + Sections, 1, "delivery field: priority")]
    [InlineData("---\narea: project\ntype: concept\nstatus: backlog\n---\n\n# Idea\n" + Sections, 1, "status: idea")]
    [InlineData(Idea + "\n## Related\n\n[Decision](../decisions/001-one.md)\n", 1, "Rationale")]
    [InlineData(Idea + "\n## Rationale\n\nWorth retaining.\n\n## Related\n\nNo durable link.\n", 1, "Related")]
    [InlineData(Idea + "\n## Rationale\n\nWorth retaining.\n\n## Related\n\n[External](https://example.com/knowledge)\n", 1, "resolving repository link")]
    [InlineData(Idea + Fenced, 1, "Rationale")]
    public void Check_ReportsTheFutureFeatureFinding(string content, int expectedExit, string? expectedFinding)
    {
        using var project = new DocsProject();
        project.Write("project/decisions/001-one.md",
            "---\narea: project\ntype: decision\nstatus: accepted\ndate: 2025-01-15\n---\n\n# One\n\nA decision.\n");
        project.Write("project/future-features/idea.md", content);

        var (code, output) = DydoCli.RunCombined("check");

        Assert.True(expectedExit == code, $"exit {code}, expected {expectedExit}:\n{output}");
        if (expectedFinding is null)
            Assert.DoesNotContain("idea.md", output);
        else
            Assert.Contains(expectedFinding, output);
    }

    [Theory]
    [Trait("Job", "contract: dydo-commands-core.md dydo check (legacy FutureFeature shape under project/future-features/: a document in another folder is not held to it)")]
    [InlineData("project/future-features/nested/idea.md")]
    [InlineData("project/ideas/idea.md")]
    public void Check_DocumentOutsideTheFutureFeaturesFolder_IsNotAFutureFeature(string path)
    {
        using var project = new DocsProject();
        project.Write(path, Idea + "\nNo sections.\n");

        var (code, output) = DydoCli.RunCombined("check");

        Assert.True(code == 0, $"exit {code}:\n{output}");
        Assert.DoesNotContain("Rationale", output);
    }
}
