namespace DynaDocs.Tests.Steps;

using Reqnroll;
using System.Text.RegularExpressions;

[Binding]
public class OptionalSummarySteps(CliScenario scenario)
{
    private const string Frontmatter = "---\narea: general\ntype: reference\n---\n\n";
    private string DocumentPath => Path.Combine(scenario.DirectoryPath, "dydo", "document.md");
    private byte[] _beforeFix = [];

    [Given("a documentation tree whose frontmatter, navigation and links are valid")]
    public void ValidTree()
    {
        File.WriteAllText(Path.Combine(scenario.DirectoryPath, "dydo.json"),
            """{"version":1,"structure":{"root":"dydo"},"scanExclude":["_system/.local/","agents/"]}""");
        Directory.CreateDirectory(Path.GetDirectoryName(DocumentPath)!);
        File.WriteAllText(Path.Combine(scenario.DirectoryPath, "dydo", "index.md"),
            "---\narea: general\ntype: hub\n---\n\n# Index\n\n[Document](./document.md)\n[Off-limits](./files-off-limits.md)\n");
        File.WriteAllText(Path.Combine(scenario.DirectoryPath, "dydo", "files-off-limits.md"),
            "---\ntype: config\n---\n\n# Files Off-Limits\n\n## Default Patterns\n\n```\n.env*\n```\n");
    }

    // Example cells spell line breaks as \n.
    [Given("a document whose body after its frontmatter is {string}")]
    public void Body(string body) => File.WriteAllText(DocumentPath, Frontmatter + body.Replace("\\n", "\n"));

    [Given("a document with an H1 title and no frontmatter or opening summary")]
    public void MissingFrontmatter() => File.WriteAllText(DocumentPath, "# Document\n\n## Details");

    [When("I check the documentation")]
    public Task Check() => scenario.RunAsync("check");

    [When("I fix the documentation")]
    public async Task Fix()
    {
        _beforeFix = File.ReadAllBytes(DocumentPath);
        await scenario.RunAsync("fix");
        scenario.Result.AssertSuccess();
    }

    [Then("the documentation has no validation errors or warnings")]
    public void ValidDocumentation()
    {
        var result = scenario.Result;
        Assert.True(result.ExitCode == 0, result.Stdout + result.Stderr);
        Assert.DoesNotContain("WARNINGS:", result.Stdout);
        Assert.DoesNotContain("ERRORS:", result.Stdout);
    }

    [Then("checking reports nothing")]
    public void NoFinding()
    {
        var result = scenario.Result;
        Assert.True(result.ExitCode == 0, result.Stdout + result.Stderr);
        Assert.DoesNotContain("document.md", result.Stdout + result.Stderr);
    }

    [Then("checking reports {string}")]
    public void CheckFailure(string message)
    {
        Assert.Equal(1, scenario.Result.ExitCode);
        var lines = scenario.Result.Stdout.Replace("\r\n", "\n").Split('\n');
        var document = Array.FindIndex(lines, line => line.Trim() == "document.md");
        Assert.True(document >= 0, scenario.Result.Stdout);
        var violations = lines.Skip(document + 1).TakeWhile(line => line.StartsWith("    - ", StringComparison.Ordinal));
        Assert.Contains(violations, line => Regex.Replace(line[6..], @"^Line [1-9][0-9]*: ", "") == message);
    }

    [Then("that document's bytes are unchanged")]
    public void UnchangedDocument() => Assert.Equal(_beforeFix, File.ReadAllBytes(DocumentPath));
}
