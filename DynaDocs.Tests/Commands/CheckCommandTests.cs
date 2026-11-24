namespace DynaDocs.Tests.Commands;

/// <summary>
/// `--root` on the documentation commands: the command reads the project that contains the given
/// directory or file, never the current one, and a root in no dydo project is a tool error.
/// </summary>
[Collection("Integration")]
[Trait("Job", "contract: dydo-commands-utilities.md Exit Codes (a run's exit code matches its outcome)")]
public sealed class CheckCommandTests : IDisposable
{
    private static string Hub(string guide) => $"---\narea: general\ntype: hub\n---\n\n# Index\n\n[Guide](./guides/{guide})\n";
    private const string Guide = "---\narea: guides\ntype: guide\n---\n\n# Setup\n\nHow to set up.\n";

    // The current project is valid and still carries the retired audit exclusion, which stays accepted.
    private const string ValidConfig =
        """{"version":1,"structure":{"root":"dydo"},"integrations":{"claude":false,"codex":false},"scanExclude":["_system/.local/","_system/audit/","agents/"],"nudges":[]}""";

    // The other project's config breaks the scanExclude invariant.
    private const string BrokenConfig =
        """{"version":1,"structure":{"root":"dydo"},"scanExclude":["agents/"]}""";

    // A third project's config holds an empty testing runner, which `dydo validate` rejects.
    private const string InvalidRunnerConfig = """{"testing":{"runner":[]}}""";

    private readonly string _root = Path.Combine(Path.GetTempPath(), "dydo-root-option-" + Guid.NewGuid().ToString("N"));
    private readonly string _original = Environment.CurrentDirectory;

    public CheckCommandTests()
    {
        Directory.CreateDirectory(_root);
        Environment.CurrentDirectory = CreateProject("here", ValidConfig, "intro.md");
        CreateProject("other", BrokenConfig);
        CreateProject("invalid", InvalidRunnerConfig);
        Directory.CreateDirectory(Path.Combine(_root, "nowhere"));
    }

    [Theory]
    [InlineData(0, null, "check")]
    [InlineData(1, "_system/.local/", "check", "--root", "{other}")]
    [InlineData(1, "_system/.local/", "check", "--root", "{other}/dydo/guides")]
    [InlineData(1, "_system/.local/", "check", "--root", "{other}/dydo/index.md")]
    [InlineData(2, "No dydo project found", "check", "--root", "{nowhere}")]
    [InlineData(2, "No dydo project found", "check", "--root", "{missing}")]
    [InlineData(2, "outside the docs tree", "check", "{other}/dydo")]
    [InlineData(0, null, "validate")]
    [InlineData(1, "testing.runner", "validate", "--root", "{invalid}/dydo/guides")]
    [InlineData(2, "No dydo project found", "validate", "--root", "{nowhere}")]
    [InlineData(0, "setup.md", "graph", "stats", "--root", "{other}")]
    [InlineData(0, "index.md", "graph", "guides/setup.md", "--incoming", "--root", "{other}/dydo/guides")]
    [InlineData(2, "No dydo project found", "graph", "stats", "--root", "{nowhere}")]
    public void RootOption_ReadsTheProjectContainingTheRoot(int expectedExit, string? expectedFinding, params string[] args)
    {
        var (code, output) = DydoCli.RunCombined(args.Select(Expand).ToArray());

        Assert.True(expectedExit == code, $"exit {code}, expected {expectedExit}:\n{output}");
        if (expectedFinding is null)
            Assert.DoesNotContain("_system/.local/", output);
        else
            Assert.Contains(expectedFinding, output);
    }

    private string Expand(string arg) => arg
        .Replace("{other}", Path.Combine(_root, "other"))
        .Replace("{invalid}", Path.Combine(_root, "invalid"))
        .Replace("{nowhere}",Path.Combine(_root, "nowhere"))
        .Replace("{missing}", Path.Combine(_root, "missing"))
        .Replace('/', Path.DirectorySeparatorChar);

    private string CreateProject(string name, string config, string guide = "setup.md")
    {
        var project = Path.Combine(_root, name);
        Directory.CreateDirectory(Path.Combine(project, "dydo", "guides"));
        Directory.CreateDirectory(Path.Combine(project, "dydo", "_system"));
        File.WriteAllText(Path.Combine(project, "dydo.json"), config);
        File.WriteAllText(Path.Combine(project, "dydo", "index.md"), Hub(guide));
        File.WriteAllText(Path.Combine(project, "dydo", "guides", guide), Guide);
        return project;
    }

    public void Dispose()
    {
        Environment.CurrentDirectory = _original;
        TestDirectory.Delete(_root);
    }
}
