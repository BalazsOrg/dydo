namespace DynaDocs.Tests.Integration;

using System.Text.Json;
using DynaDocs.Commands;
using DynaDocs.SkillBuild;

/// <summary>
/// DR 064 §5 / K6: an Edit or Write to a file the skill builder wrote gets the guard's warn nudge,
/// through the real hook entry point, main and worker lanes (DYD-1196 E20).
/// </summary>
[Collection("Integration")]
public class GuardBuiltFileNudgeTests : IntegrationTestBase
{
    private const string Spine = "skills/roles/crew/demo/SKILL.md";
    private const string Metadata = "skills/roles/crew/demo/agents/openai.yaml";
    private const string Nudge = "is built from the model; edit the model in tools/SkillBuild, then rebuild with `dotnet run --project tools/SkillBuild`.";

    private static readonly string BuiltSpine = "---\nname: demo\ndescription: d\n---\n" + BuiltHeader.Markdown + "\n\n# Demo\n";

    private string Payload(string toolName, object toolInput, string? agentId = null) => JsonSerializer.Serialize(
        new Dictionary<string, object?>
        {
            ["session_id"] = TestSessionId,
            ["agent_id"] = agentId,
            ["tool_name"] = toolName,
            ["tool_input"] = toolInput,
            ["cwd"] = TestDir,
        });

    private string Edit(string path, string? agentId = null) =>
        Payload("Edit", new { file_path = path, old_string = "Demo", new_string = "Demo!" }, agentId);

    [Theory]
    [Trait("Job", "contract: K6 an Edit to a built file warns, naming it and the model, and its retry proceeds (E20)")]
    [InlineData(null)]
    [InlineData("wkr-1")]
    public async Task EditToABuiltFile_WarnsOnceThenProceedsOnRetry(string? agentId)
    {
        await InitProjectAsync("none");
        WriteFile(Spine, BuiltSpine);

        var first = await GuardWithStdinAsync(Edit(Spine, agentId));
        var retry = await GuardWithStdinAsync(Edit(Spine, agentId));

        first.AssertPreToolUseDenied();
        first.AssertStderrContains($"BLOCKED: {Spine} {Nudge}");
        first.AssertStderrContains("(Retry the same write to proceed anyway.)");
        retry.AssertPreToolUseAllowed();
    }

    [Fact]
    [Trait("Job", "contract: K6 a Write over a built YAML file warns (E20)")]
    public async Task WriteOverABuiltYamlFile_Warns()
    {
        await InitProjectAsync("none");
        WriteFile(Metadata, BuiltHeader.Yaml + "\npolicy:\n  allow_implicit_invocation: false\n");

        var result = await GuardWithStdinAsync(Payload("Write", new { file_path = Path.Combine(TestDir, Metadata), content = "x" }));

        result.AssertPreToolUseDenied();
        result.AssertStderrContains($"{Metadata} {Nudge}");
    }

    [Theory]
    [Trait("Job", "edge: K5 a hand-written file, a header past the first lines, and a new file are never nudged (E20)")]
    [InlineData("hand-written")]
    [InlineData("late header")]
    [InlineData("new file")]
    public async Task WriteToAFileWithoutTheHeader_IsNotNudged(string target)
    {
        await InitProjectAsync("none");
        if (target == "hand-written") WriteFile(Spine, "# Demo\n\nHand-written; mentions built from the model; edit there.\n");
        if (target == "late header") WriteFile(Spine, "# Demo" + new string('\n', BuiltHeader.Window) + BuiltHeader.Markdown + "\n");

        var result = await GuardWithStdinAsync(Payload("Write", new { file_path = Spine, content = BuiltSpine }));

        result.AssertPreToolUseAllowed();
    }

    [Fact]
    [Trait("Job", "contract: the guard's header copy is the builder's header (E20, K6)")]
    public void GuardHeaderCopy_IsTheBuildersHeader() =>
        Assert.Equal((BuiltHeader.Markdown, BuiltHeader.Yaml, BuiltHeader.Window),
            (GuardCommand.BuiltHeaderMarkdown, GuardCommand.BuiltHeaderYaml, GuardCommand.BuiltHeaderWindow));
}
