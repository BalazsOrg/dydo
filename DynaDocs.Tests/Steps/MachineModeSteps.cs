namespace DynaDocs.Tests.Steps;

using System.Text.Json;
using DynaDocs.Commands;
using DynaDocs.Services;
using DynaDocs.Tests.Integration;
using Reqnroll;

[Binding]
public sealed class MachineModeSteps : IntegrationTestBase
{
    private readonly string? _originalClaudeCode = Environment.GetEnvironmentVariable("CLAUDECODE");
    private string _toolName = "Bash";
    private string[] _hookArgs = [];
    private string _permissionMode = "default";
    private string? _agentId;
    private CommandResult _result = new(-1, "", "");
    private string? _initialModeBytes;
    private string? _initialValue;

    private ModeState State => new(Path.Combine(TestDir, "machine"));

    [AfterScenario]
    public void Restore()
    {
        Environment.SetEnvironmentVariable("CLAUDECODE", _originalClaudeCode);
        Dispose();
    }

    [Given("a fresh machine mode")]
    public void FreshMachineMode() => Assert.False(Directory.Exists(Path.Combine(TestDir, "machine")));

    [Given("the owned overnight mode starts as {string}")]
    public async Task OwnedModeStarts(string value)
    {
        (await RunAsync(ModeCommand.Create(State), "set", "overnight", value)).AssertSuccess();
        _initialModeBytes = ReadFile("machine/mode.json");
        _initialValue = value;
    }

    [Then("the owned mode state is unchanged")]
    public async Task OwnedModeUnchanged()
    {
        Assert.NotNull(_initialModeBytes);
        Assert.Equal(_initialModeBytes, ReadFile("machine/mode.json"));
        await Shows("overnight", _initialValue!);
    }

    [When("I run dydo mode show")]
    public async Task Show() => _result = await RunAsync(ModeCommand.Create(State), "show");

    [When("I run dydo mode set {string} {string}")]
    public async Task Set(string flag, string value) =>
        _result = await RunAsync(ModeCommand.Create(State), "set", flag, value);

    [Then("the mode set succeeds")]
    public void SetSucceeds() => _result.AssertSuccess();

    [Then("the mode set is refused naming {string}")]
    public void SetRefused(string refused)
    {
        _result.AssertExitCode(2);
        Assert.Contains($"'{refused}'", _result.Stderr);
    }

    [Then("the mode shows {string} as {string}")]
    public async Task Shows(string flag, string value)
    {
        var shown = await RunAsync(ModeCommand.Create(State), "show");
        shown.AssertSuccess();
        // Overnight is the one flag, so the whole output is that one line, byte for byte.
        Assert.Equal($"{flag}: {value}{Environment.NewLine}", shown.Stdout);
    }

    /// <summary>
    /// The host decides two things: the hook command (only <c>dydo init</c>'s Claude Code hook carries
    /// the <c>--host claude</c> marker) and the inherited environment. CLAUDECODE=1 is set wherever a
    /// Claude Code shell is in the ancestry, so it proves nothing about the host running the hook.
    /// </summary>
    [Given("an agent on {string} in {string} permission mode")]
    public async Task Agent(string host, string permissionMode)
    {
        (var claudeCodeShell, _toolName, _hookArgs) = host switch
        {
            "Claude Code" => (true, "Bash", new[] { "--host", "claude" }),
            "Codex" => (false, "shell_command", Array.Empty<string>()),
            _ => throw new ArgumentException($"Unknown host in scenario: {host}")
        };
        Environment.SetEnvironmentVariable("CLAUDECODE", claudeCodeShell ? "1" : null);
        _permissionMode = permissionMode;
        (await InitProjectAsync("none")).AssertSuccess();
    }

    [Given("a sub-agent on {string} in {string} permission mode")]
    public async Task SubAgent(string host, string permissionMode)
    {
        await Agent(host, permissionMode);
        _agentId = "sub-agent-1";
    }

    [When("the agent runs {string}")]
    public async Task AgentRuns(string command) => _result = await GuardWithStdinAsync(Hook(command), _hookArgs);

    [When(@"^the agent runs the command: (.+)$")]
    public Task AgentRunsCommand(string command) => AgentRuns(command);

    [When("the agent runs {string} twice")]
    public async Task AgentRunsTwice(string command)
    {
        await AgentRuns(command);
        AssertFirstTryBlocked();
        await AgentRuns(command);
    }

    [Then("the guard blocks it once")]
    public void AssertFirstTryBlocked() => Assert.Contains("dydo mode set", _result.AssertPreToolUseDenied());

    [Then("the guard allows it")]
    public void Allows() => _result.AssertPreToolUseAllowed();

    private string Hook(string command)
    {
        var hook = new Dictionary<string, object>
        {
            ["session_id"] = TestSessionId,
            ["hook_event_name"] = "PreToolUse",
            ["permission_mode"] = _permissionMode,
            ["tool_name"] = _toolName,
            ["tool_input"] = new Dictionary<string, string> { ["command"] = command }
        };
        if (_agentId != null)
            hook["agent_id"] = _agentId;
        return JsonSerializer.Serialize(hook);
    }
}
