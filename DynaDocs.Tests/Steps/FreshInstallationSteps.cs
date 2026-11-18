namespace DynaDocs.Tests.Steps;

using System.Text;
using System.Text.Json;
using Reqnroll;

[Binding]
public class FreshInstallationSteps(CliScenario scenario)
{
    private Dictionary<string, byte[]> _foundationDocuments = [];

    [Given("an empty project directory")]
    public void EmptyProject() => Assert.Empty(Directory.EnumerateFileSystemEntries(scenario.DirectoryPath));

    [When("I initialize dydo with {string}")]
    public Task Initialize(string integration) => scenario.RunAsync("init", integration);

    [Then("the command succeeds")]
    public void CommandSucceeds() => scenario.Result.AssertSuccess();

    [Then(@"^the recorded (Claude|Codex) integration is (true|false)$")]
    public void RecordedIntegration(string host, bool expected)
    {
        using var config = JsonDocument.Parse(File.ReadAllText(Path.Combine(scenario.DirectoryPath, "dydo.json")));
        var integrations = config.RootElement.GetProperty("integrations");
        var enabled = integrations.TryGetProperty(host.ToLowerInvariant(), out var value) && value.GetBoolean();
        Assert.Equal(expected, enabled);
    }

    [Then("the documentation has no validation errors but has warnings")]
    public void WarningsOnly()
    {
        var result = scenario.Result;
        var output = $"Exit: {result.ExitCode}\n{result.Stdout}\n{result.Stderr}";
        Assert.True(result.ExitCode == 0, output);
        Assert.Contains("WARNINGS:", result.Stdout);
        Assert.DoesNotContain("ERRORS:", result.Stdout);
    }

    [When("I customize the foundation documents for a tiny task-list project")]
    public void CustomizeFoundationDocuments()
    {
        _foundationDocuments = new Dictionary<string, byte[]>
        {
            ["dydo/understand/about.md"] = Encoding.UTF8.GetBytes(About.ReplaceLineEndings("\n") + "\n"),
            ["dydo/understand/architecture.md"] = Encoding.UTF8.GetBytes(Architecture.ReplaceLineEndings("\n") + "\n")
        };
        foreach (var path in _foundationDocuments.Keys)
            Assert.True(File.Exists(Path.Combine(scenario.DirectoryPath, path)), $"Missing initialized file: {path}");
        foreach (var (path, bytes) in _foundationDocuments)
            File.WriteAllBytes(Path.Combine(scenario.DirectoryPath, path), bytes);
    }

    [Then("the customized foundation documents retain their paths and bytes")]
    public void PreservedFoundationDocuments()
    {
        Assert.Equal(2, _foundationDocuments.Count);
        foreach (var (path, bytes) in _foundationDocuments)
        {
            var fullPath = Path.Combine(scenario.DirectoryPath, path);
            Assert.True(File.Exists(fullPath), $"Missing customized file: {path}");
            Assert.Equal(bytes, File.ReadAllBytes(fullPath));
        }
    }

    [Given("a user-owned file named {string} containing {string}")]
    public void UserFile(string name, string content) => File.WriteAllBytes(
        Path.Combine(scenario.DirectoryPath, name), Encoding.UTF8.GetBytes(content));

    [Then("the user-owned file named {string} still contains {string}")]
    public void PreservedUserFile(string name, string content) => Assert.Equal(
        Encoding.UTF8.GetBytes(content), File.ReadAllBytes(Path.Combine(scenario.DirectoryPath, name)));

    private const string About = """
        ---
        area: understand
        type: context
        ---

        # About This Project

        Pocket Tasks is a small command-line task list for one person's daily work.
        It lets the user add a task, list open tasks, and mark a task complete in a local JSON file.
        The project runs offline and has no accounts or external service dependencies.

        ---

        *See [architecture.md](./architecture.md) for technical structure.*
        """;

    private const string Architecture = """
        ---
        area: understand
        type: concept
        ---

        # Architecture Overview

        Pocket Tasks is a Python command-line application with a task service and a local JSON store.
        The command layer validates arguments; the task service owns task identifiers and completion state.

        ## Project Structure

        - `src/` contains command handling, task operations, and the JSON store.
        - `tests/` contains command and storage tests using temporary task files.
        - `dydo/` contains durable project documentation.

        ## Key Components

        - Commands translate add, list, and complete requests into task-service calls.
        - The task service applies task rules without reading terminal input or writing files.
        - The JSON store loads and saves task records at the path supplied by the command layer.

        ## Data Flow

        Arguments enter the command layer, which loads records through the JSON store and calls the task service.
        Read requests print the current tasks; successful changes save the updated records before printing confirmation.
        Malformed arguments leave the stored records unchanged.

        ## Knowledge and Work Boundary

        - **Linear** owns Initiatives, Projects, Issues, optional Milestones and Cycles, plus live status,
          priority, assignment, dependencies, updates, and review state.
        - **Git/dydo** owns architecture, Decisions, reviewed Project plans, guides, audits, assimilation
          evidence, and changelog. Linear owns FutureFeatures with the rest of the work graph.
        - Link between the two; do not mirror volatile Linear state into repository documents.

        ## Related

        - [dydo Glossary](../reference/dydo-glossary.md) - Work and knowledge vocabulary
        - [Coding Standards](../guides/coding-standards.md) - Code conventions
        """;
}
