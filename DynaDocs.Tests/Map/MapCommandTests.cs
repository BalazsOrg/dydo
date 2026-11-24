namespace DynaDocs.Tests.Map;

using DynaDocs.Commands;
using DynaDocs.Utils;

/// <summary>DR 052 §2 and §3 at the command: no key exits 2 before any server; a key serves on localhost and never prints it.</summary>
[Trait("Job", "contract: DR 052 §2 (a missing key exits 2, prints nothing on stdout and opens no browser) §3 (with a key it serves a localhost URL and never prints the key)")]
public class MapCommandTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public async Task MissingKey_PrintsTheKeyHelp_AndExits2BeforeAnyServer(string? key)
    {
        var opened = new List<Uri>();

        var (exitCode, stdout, stderr) = await ConsoleCapture.AllAsync(
            () => MapCommand.RunAsync(key, "http://unused.test/graphql", opened.Add, CancellationToken.None));

        Assert.Equal(ExitCodes.ToolError, exitCode);
        Assert.Equal(MapCommand.MissingKeyHelp, stderr);
        Assert.Empty(stdout);
        Assert.Empty(opened);
    }

    [Fact]
    public async Task MissingKey_ThroughTheCommandLine_Exits2()
    {
        var saved = Environment.GetEnvironmentVariable("LINEAR_API_KEY");
        Environment.SetEnvironmentVariable("LINEAR_API_KEY", null);
        try
        {
            var (exitCode, stdout, _) = await ConsoleCapture.AllAsync(
                () => MapCommand.Create().Parse(["--no-browser"]).InvokeAsync());

            Assert.Equal(ExitCodes.ToolError, exitCode);
            Assert.Empty(stdout);
        }
        finally
        {
            Environment.SetEnvironmentVariable("LINEAR_API_KEY", saved);
        }
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task WithAKey_ServesOnLocalhost_OpensTheBrowserUnlessToldNot_AndStopsOnCancel(bool browser)
    {
        var opened = new List<Uri>();
        using var stopped = new CancellationTokenSource();
        await stopped.CancelAsync();

        var (exitCode, stdout, _) = await ConsoleCapture.AllAsync(
            () => MapCommand.RunAsync(FakeLinear.ApiKey, null, browser ? opened.Add : null, stopped.Token));

        Assert.Equal(ExitCodes.Success, exitCode);
        Assert.Contains("http://localhost:", stdout);
        Assert.DoesNotContain(FakeLinear.ApiKey, stdout);
        if (browser)
            Assert.Equal("localhost", Assert.Single(opened).Host);
    }
}
