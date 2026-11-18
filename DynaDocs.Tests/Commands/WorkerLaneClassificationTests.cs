namespace DynaDocs.Tests.Commands;

using DynaDocs.Commands;
using DynaDocs.Tests.Integration;

/// <summary>
/// Decision 024's worker lane: a sub-agent call (one carrying agent_id) may run only some dydo
/// commands. One expectation table names every command the root registers, so a new command fails
/// here until its author decides, and the guard hook is held to each decision.
/// </summary>
[Collection("Integration")]
[Trait("Job", "contract: Decision 024 worker lane, every registered dydo command is classified")]
public class WorkerLaneClassificationTests : GuardHookTestBase
{
    // true: a sub-agent may run it; false: it stays with the top-level session.
    private static readonly Dictionary<string, bool> Expected = new(StringComparer.Ordinal)
    {
        ["check"] = true, ["fix"] = true, ["gap-check"] = true, ["graph"] = true, ["validate"] = true,
        ["mode"] = true, ["clean"] = true, ["pr-ready"] = true, ["merge"] = true, ["survey"] = true,
        ["statechart"] = true, ["archive"] = true, ["help"] = true, ["version"] = true,
        ["init"] = false, ["adopt"] = false, ["index"] = false, ["guard"] = false, ["map"] = false,
        ["completions"] = false, ["_complete"] = false,
    };

    [Fact]
    public void ExpectationTable_NamesEveryRegisteredCommand()
    {
        var registered = DydoRootCommand.Create().Subcommands.Select(command => command.Name).Order(StringComparer.Ordinal);

        Assert.Equal(Expected.Keys.Order(StringComparer.Ordinal), registered);
    }

    public static TheoryData<string, bool> Words()
    {
        var data = new TheoryData<string, bool>();
        foreach (var (name, allowed) in Expected)
            data.Add(name, allowed);
        foreach (var option in new[] { "-h", "--help", "--version" })
            data.Add(option, true);
        return data;
    }

    [Theory]
    [MemberData(nameof(Words))]
    public async Task WorkerLane_RunsOnlyTheCommandsItMay(string word, bool allowed)
    {
        await InitProjectAsync("none");
        var payload = System.Text.Json.JsonSerializer.Serialize(new Dictionary<string, object>
        {
            ["session_id"] = TestSessionId,
            ["agent_id"] = "wkr-classification-1",
            ["agent_type"] = "code-writer",
            ["tool_name"] = "Bash",
            ["tool_input"] = new Dictionary<string, string> { ["command"] = "dydo " + word },
        });

        var result = await GuardWithStdinAsync(payload);

        if (allowed) result.AssertPreToolUseAllowed();
        else result.AssertPreToolUseDenied();
    }
}
