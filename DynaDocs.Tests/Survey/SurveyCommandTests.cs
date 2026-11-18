namespace DynaDocs.Tests.Survey;

using DynaDocs.Commands;
using DynaDocs.Tests.Map;
using DynaDocs.Utils;
using static DynaDocs.Tests.Map.LinearJson;

/// <summary>DR 063 §1, §2 and §5 at the command boundary: exit codes, the shared key help, and a read-only digest.</summary>
[Trait("Job", "contract: DR 063 §1 §2 §5")]
public sealed class SurveyCommandTests : IDisposable
{
    private readonly FakeLinear _linear = new();
    private readonly FakeLinearServer _server;

    public SurveyCommandTests() => _server = new FakeLinearServer(_linear);

    public void Dispose()
    {
        _server.Dispose();
        _linear.Dispose();
    }

    private Task<(int ExitCode, string Stdout, string Stderr)> Run(params string[] arguments) =>
        ConsoleCapture.AllAsync(() => SurveyCommand.RunAsync(
            arguments, FakeLinear.ApiKey, _server.Endpoint.ToString(), new FixedClock(), CancellationToken.None));

    [Theory]
    [InlineData]
    [InlineData("25acbbb9a90b", "25acbbb9a90c")]
    [InlineData("map")]
    public async Task NotExactlyOneProject_IsAUsageError_BeforeAnyRequest(params string[] arguments)
    {
        var (exitCode, stdout, stderr) = await Run(arguments);

        Assert.Equal(ExitCodes.ToolError, exitCode);
        Assert.NotEmpty(stderr);
        Assert.Empty(stdout);
        Assert.Empty(_linear.Calls);
    }

    [Theory]
    [InlineData(null)]
    [InlineData(" ")]
    public async Task MissingKey_PrintsMapsKeyHelp_BeforeAnyRequest(string? key)
    {
        var (exitCode, stdout, stderr) = await ConsoleCapture.AllAsync(() => SurveyCommand.RunAsync(
            ["25acbbb9a90b"], key, _server.Endpoint.ToString(), new FixedClock(), CancellationToken.None));

        Assert.Equal(ExitCodes.ToolError, exitCode);
        Assert.Equal(MapCommand.MissingKeyHelp, stderr);
        Assert.Empty(stdout);
        Assert.Empty(_linear.Calls);
    }

    [Fact]
    public async Task ARead_ExitsZero_WithAUtcHeader_AndOneLinePerFinding()
    {
        _linear.Serve(call => call.Operation == "ProjectIssues"
            ? IssuesPage(Node(Issue("1", labels: Labels("Feature:#BB87FC"))))
            : ProjectContent("\"\""));

        var (exitCode, stdout, stderr) = await Run("25acbbb9a90b");

        Assert.Equal(ExitCodes.Success, exitCode);
        Assert.StartsWith("dydo survey Map 2026-11-04T21:40:05Z", stdout, StringComparison.Ordinal);
        Assert.Equal(["pickable DYD-1"], KeptFindings.In(stdout));
        Assert.Empty(stderr);
    }

    [Fact]
    public async Task AnArchivedBlocker_IsNoBlocker()
    {
        _linear.Serve(call => call.Operation == "ProjectIssues"
            ? IssuesPage(Node(Issue("1", labels: Labels("Feature:#BB87FC")), inverseRelations: Connection(
                Incoming("r9", "blocks", Issue("9", state: Done, project: InOther, archivedAt: "\"2026-11-01T00:00:00.000Z\"")))))
            : ProjectContent("\"\""));

        var (_, stdout, _) = await Run("25acbbb9a90b");

        Assert.Equal(["pickable DYD-1"], KeptFindings.In(stdout));
    }

    [Fact]
    public async Task ATicketMissingMidRead_IsNamed_NotCalledAnUnknownProject()
    {
        _linear.Serve(call => call.Operation switch
        {
            "ProjectIssues" => IssuesPage(Node(Issue("1"), relations: Connection("", more: true, cursor: "c1"))),
            "IssueRelationsPage" => """{"data":{"issue":null}}""",
            _ => ProjectContent("\"\""),
        });

        var (exitCode, stdout, stderr) = await Run("25acbbb9a90b");

        Assert.Equal(ExitCodes.ToolError, exitCode);
        Assert.Contains("ticket 1", stderr);
        Assert.DoesNotContain("no project", stderr);
        Assert.Empty(stdout);
    }

    [Theory]
    [InlineData("""{"data":{"project":{"id":"p-map"}}}""")]
    [InlineData(null)]
    public async Task AnUnexpectedAnswer_IsAToolError_WithNoDigest(string? answer)
    {
        answer ??= IssuesPage(Node(Issue("1").Replace("\"title\":\"Issue 1\"", "\"title\":null", StringComparison.Ordinal)));
        _linear.Serve(_ => answer);

        var (exitCode, stdout, stderr) = await Run("25acbbb9a90b");

        Assert.Equal(ExitCodes.ToolError, exitCode);
        Assert.NotEmpty(stderr);
        Assert.Empty(stdout);
    }

    [Fact]
    [Trait("Job", "edge: a canceled run must not print a digest as Linear's answer (DYD-530)")]
    public async Task ACanceledRun_IsNotReportedAsLinearsAnswer()
    {
        await Assert.ThrowsAnyAsync<OperationCanceledException>(() => SurveyCommand.RunAsync(
            ["25acbbb9a90b"], FakeLinear.ApiKey, _server.Endpoint.ToString(), new FixedClock(), new CancellationToken(true)));
    }

    private const string Done = """{"name":"Done","type":"completed","color":"#5e6ad2"}""";

    private sealed class FixedClock : TimeProvider
    {
        public override DateTimeOffset GetUtcNow() => new(2026, 11, 4, 22, 40, 5, TimeSpan.FromHours(1));
    }
}
