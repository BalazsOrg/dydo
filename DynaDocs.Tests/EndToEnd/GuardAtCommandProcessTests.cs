namespace DynaDocs.Tests.EndToEnd;

using System.Diagnostics;

/// <summary>
/// DYD-675 through the built CLI: Program.cs is the one entry the in-process guard tests bypass, so
/// a '@' text reaching `dydo guard` must be shown to survive the real parse, not only a test's.
/// </summary>
[Collection("EndToEnd")]
[Trait("Job", "edge: DYD-675 text starting with @ through Program.cs's own parse in a real dydo process")]
public class GuardAtCommandProcessTests : IDisposable
{
    private readonly string _projectDir;
    private readonly string _dllPath;

    public GuardAtCommandProcessTests(EndToEndFixture fixture)
    {
        _dllPath = fixture.DllPath;
        _projectDir = Path.Combine(Path.GetTempPath(), "dydo-e2e-at-" + Guid.NewGuid().ToString("N")[..8]);
        Directory.CreateDirectory(_projectDir);
        Assert.Equal(0, Run("", "init", "none").ExitCode);
    }

    public void Dispose()
    {
        try { Directory.Delete(_projectDir, true); }
        catch (IOException) { /* a temp directory another process still holds is left for the OS */ }
        GC.SuppressFinalize(this);
    }

    private (int ExitCode, string Stdout, string Stderr) Run(string stdin, params string[] arguments)
    {
        var start = new ProcessStartInfo("dotnet")
        {
            WorkingDirectory = _projectDir,
            RedirectStandardInput = true,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };
        start.ArgumentList.Add(_dllPath);
        foreach (var argument in arguments) start.ArgumentList.Add(argument);

        using var process = Process.Start(start)!;
        process.StandardInput.Write(stdin);
        process.StandardInput.Close();
        var stdout = process.StandardOutput.ReadToEndAsync();
        var stderr = process.StandardError.ReadToEndAsync();
        process.WaitForExit();
        return (process.ExitCode, stdout.Result, stderr.Result);
    }

    // The refusal's exit 2 must survive Program.cs too: a host may read exit 1 as a non-blocking error.
    [Fact]
    public void GuardCommand_TextStartingWithAt_GetsAVerdictNotAResponseFileError()
    {
        var result = Run("", "guard", "--command", "@'\ngit push --force origin main\n'@");

        Assert.DoesNotContain("Response file", result.Stderr);
        Assert.Equal(2, result.ExitCode);
        Assert.Contains("Force push", result.Stderr);
    }

    [Fact]
    public void GuardCommand_AtExistingFile_IsTextAndTheFileIsNotRead()
    {
        var file = Path.Combine(_projectDir, "response.rsp");
        File.WriteAllText(file, "\"git push --force origin main\"\n");

        var result = Run("", "guard", "--command", "@" + file);

        Assert.Equal(0, result.ExitCode);
        Assert.Equal("", result.Stderr);
    }

    [Fact]
    public void GuardHookLane_HereStringOnStdin_IsDenied()
    {
        var payload = "{\"session_id\":\"e2e-at\",\"tool_name\":\"PowerShell\",\"tool_input\":{\"command\":\"@'\\ngit push --force origin main\\n'@\"}}";

        var result = Run(payload, "guard", "--host", "claude");

        Assert.Equal(0, result.ExitCode);
        Assert.Contains("\"permissionDecision\":\"deny\"", result.Stdout);
    }
}
