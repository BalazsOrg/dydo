namespace DynaDocs.Tests.EndToEnd;

using System.Diagnostics;

/// <summary>
/// True end-to-end tests that spawn the CLI as a subprocess.
/// These tests verify the application works as a user would actually run it.
///
/// Unlike the "integration" tests which call command methods directly,
/// these tests run: dotnet dydo.dll &lt;args&gt;
///
/// This catches issues like:
/// - Command construction failures (invalid Option aliases, etc.)
/// - Missing dependencies at runtime
/// - Startup crashes
/// </summary>
[Collection("EndToEnd")]
[Trait("Job", "edge: the built dydo.dll run as a subprocess, so startup, command construction and the process exit code are real (in-process parsing cannot reach them)")]
public class CliEndToEndTests : IDisposable
{
    private readonly string _testDir;
    private readonly string _dllPath;

    public CliEndToEndTests(EndToEndFixture fixture)
    {
        _testDir = Path.Combine(Path.GetTempPath(), "dydo-e2e-" + Guid.NewGuid().ToString("N")[..8]);
        Directory.CreateDirectory(_testDir);
        _dllPath = fixture.DllPath;
    }

    public void Dispose()
    {
        if (Directory.Exists(_testDir))
        {
            for (var i = 0; i < 3; i++)
            {
                try
                {
                    Directory.Delete(_testDir, true);
                    return;
                }
                catch (IOException) when (i < 2)
                {
                    Thread.Sleep(100);
                }
            }
        }
        GC.SuppressFinalize(this);
    }

    #region Startup / Help Tests

    [Fact]
    public async Task Help_DoesNotCrash()
    {
        var result = await RunDydoAsync("--help");

        Assert.True(result.ExitCode == 0,
            $"dydo --help crashed with exit code {result.ExitCode}:\nStderr: {result.Stderr}\nStdout: {result.Stdout}");
        Assert.Contains("dydo", result.Stdout);
    }

    [Theory]
    [InlineData("init")]
    [InlineData("adopt")]
    [InlineData("check")]
    [InlineData("guard")]
    [InlineData("fix")]
    [InlineData("index")]
    [InlineData("graph")]
    public async Task Subcommand_Help_DoesNotCrash(string subcommand)
    {
        var result = await RunDydoAsync($"{subcommand} --help");

        Assert.True(result.ExitCode == 0,
            $"dydo {subcommand} --help crashed with exit code {result.ExitCode}:\nStderr: {result.Stderr}");
    }

    #endregion

    #region Init Tests

    [Theory]
    [Trait("Job", "contract: dydo-commands-core.md dydo init (dydo.json and the dydo/ tree)")]
    [InlineData("claude")]
    [InlineData("none")]
    public async Task Init_CreatesProject(string integration)
    {
        var result = await RunDydoAsync($"init {integration}");

        Assert.True(result.ExitCode == 0,
            $"dydo init {integration} failed:\nStderr: {result.Stderr}\nStdout: {result.Stdout}");
        Assert.True(File.Exists(Path.Combine(_testDir, "dydo.json")),
            "dydo.json was not created");
        Assert.True(Directory.Exists(Path.Combine(_testDir, "dydo")),
            "dydo/ directory was not created");
    }

    [Fact]
    [Trait("Job", "contract: dydo-commands-core.md dydo init (a fresh init records the running dydo version)")]
    public async Task Init_RecordsTheVersionTheCliReports()
    {
        var initResult = await RunDydoAsync("init none");
        Assert.True(initResult.ExitCode == 0, $"init failed: {initResult.Stderr}");
        var version = await RunDydoAsync("version");

        var config = System.Text.Json.Nodes.JsonNode.Parse(File.ReadAllText(Path.Combine(_testDir, "dydo.json")))!;
        Assert.Equal(version.Stdout.Trim(), $"dydo version {config["dydoVersion"]!.GetValue<string>()}");
    }

    [Fact]
    [Trait("Job", "contract: dydo-commands-core.md dydo init then dydo check exits 0")]
    public async Task Init_ThenCheck_Passes()
    {
        var initResult = await RunDydoAsync("init none");
        Assert.True(initResult.ExitCode == 0, $"init failed: {initResult.Stderr}");

        var checkResult = await RunDydoAsync("check .");

        Assert.True(checkResult.ExitCode == 0,
            $"check failed with {checkResult.ExitCode}:\nStderr: {checkResult.Stderr}\nStdout: {checkResult.Stdout}");
    }

    [Fact]
    [Trait("Job", "contract: dydo-commands-core.md dydo init and --join (the Claude and Codex agent settings; a join keeps config.toml's bytes)")]
    public async Task Init_All_WritesAndRetainsHostAgentSettingsThroughJoin()
    {
        var init = await RunDydoAsync("init all");
        Assert.Equal(0, init.ExitCode);
        Assert.Contains("CLAUDE_CODE_MAX_SUBAGENT_SPAWN_DEPTH", File.ReadAllText(Path.Combine(_testDir, ".claude", "settings.json")));
        var codexPath = Path.Combine(_testDir, ".codex", "config.toml");
        var before = File.ReadAllBytes(codexPath);
        Assert.Contains("max_concurrent_threads_per_session = 16", File.ReadAllText(codexPath));

        var join = await RunDydoAsync("init all --join");
        Assert.Equal(0, join.ExitCode);
        Assert.Equal(before, File.ReadAllBytes(codexPath));
    }

    [Fact]
    [Trait("Job", "trap: #0185-#0188 (dydo check on a subfolder reported every cross-folder link broken; fixed in 578386730)")]
    public async Task Check_Subfolder_AcceptsLinksToTargetsOutsideSubfolder()
    {
        var initResult = await RunDydoAsync("init none");
        Assert.True(initResult.ExitCode == 0, $"init failed: {initResult.Stderr}");

        var fixtureSrc = Path.Combine(AppContext.BaseDirectory, "TestData", "link-validator");
        Assert.True(Directory.Exists(fixtureSrc), $"fixture missing: {fixtureSrc}");

        var fixtureDst = Path.Combine(_testDir, "dydo", "x", "y", "z");
        Directory.CreateDirectory(Path.Combine(fixtureDst, "sub"));
        foreach (var file in Directory.GetFiles(fixtureSrc, "*.md"))
            File.Copy(file, Path.Combine(fixtureDst, Path.GetFileName(file)));
        foreach (var file in Directory.GetFiles(Path.Combine(fixtureSrc, "sub"), "*.md"))
            File.Copy(file, Path.Combine(fixtureDst, "sub", Path.GetFileName(file)));

        var checkResult = await RunDydoAsync("check dydo/x/y/z");

        Assert.True(checkResult.ExitCode <= 1,
            $"check crashed:\nStderr: {checkResult.Stderr}\nStdout: {checkResult.Stdout}");
        Assert.DoesNotContain("Broken link:", checkResult.Stdout);
    }

    [Fact]
    [Trait("Job", "contract: dydo-commands-core.md dydo check exit 2 (a tool error)")]
    public async Task Check_NonExistentPath_ReturnsToolError()
    {
        var initResult = await RunDydoAsync("init none");
        Assert.True(initResult.ExitCode == 0, $"init failed: {initResult.Stderr}");

        var result = await RunDydoAsync("check does-not-exist-path");

        Assert.Equal(2, result.ExitCode);
        Assert.Contains("Path not found", result.Stdout + result.Stderr);
    }

    [Fact]
    [Trait("Job", "contract: dydo-commands-core.md dydo check exit 2 (a path outside the docs tree)")]
    public async Task Check_PathOutsideDocsTree_ReturnsToolError()
    {
        var initResult = await RunDydoAsync("init none");
        Assert.True(initResult.ExitCode == 0, $"init failed: {initResult.Stderr}");

        var outside = Path.Combine(_testDir, "elsewhere");
        Directory.CreateDirectory(outside);

        var result = await RunDydoAsync($"check \"{outside}\"");

        Assert.Equal(2, result.ExitCode);
        Assert.Contains("outside the docs tree", result.Stdout + result.Stderr);
    }

    #endregion

    #region Error Handling Tests

    [Fact]
    public async Task Check_WithoutInit_ShowsError()
    {
        var result = await RunDydoAsync("check .");

        // Should fail gracefully (no dydo.json)
        // Exit code 2 indicates tool error (crash), which we don't want
        Assert.True(result.ExitCode != 2,
            $"check crashed on empty directory:\nStderr: {result.Stderr}");
        Assert.False(string.IsNullOrEmpty(result.Stderr) && string.IsNullOrEmpty(result.Stdout),
            "Should show some output explaining the error");
    }

    #endregion

    private async Task<CliResult> RunDydoAsync(string args)
    {
        var psi = new ProcessStartInfo
        {
            FileName = "dotnet",
            Arguments = $"\"{_dllPath}\" {args}",
            WorkingDirectory = _testDir,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using var process = Process.Start(psi)!;
        var stdout = await process.StandardOutput.ReadToEndAsync();
        var stderr = await process.StandardError.ReadToEndAsync();
        await process.WaitForExitAsync();

        return new CliResult(process.ExitCode, stdout, stderr);
    }

    private record CliResult(int ExitCode, string Stdout, string Stderr);
}

public class EndToEndFixture : IDisposable
{
    public string DllPath { get; }

    public EndToEndFixture()
    {
        var projectDir = FindProjectDirectory();
        DllPath = Path.Combine(projectDir, "bin", "Debug", "net10.0", "dydo.dll");

        if (!File.Exists(DllPath))
        {
            var psi = new ProcessStartInfo
            {
                FileName = "dotnet",
                Arguments = "build -c Debug",
                WorkingDirectory = projectDir,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false
            };

            using var process = Process.Start(psi)!;
            process.WaitForExit(120000);

            if (process.ExitCode != 0 || !File.Exists(DllPath))
            {
                throw new InvalidOperationException(
                    $"Failed to build project. DLL not found at: {DllPath}");
            }
        }
    }

    public void Dispose() { }

    private static string FindProjectDirectory()
    {
        var dir = AppContext.BaseDirectory;
        while (dir != null)
        {
            if (File.Exists(Path.Combine(dir, "DynaDocs.csproj")))
                return dir;

            // Check parent (we're in DynaDocs.Tests/bin/Debug/net10.0)
            var parent = Path.GetDirectoryName(dir);
            if (parent != null)
            {
                var siblingPath = Path.Combine(parent, "DynaDocs", "DynaDocs.csproj");
                if (File.Exists(siblingPath))
                    return Path.Combine(parent, "DynaDocs");

                var grandparent = Path.GetDirectoryName(parent);
                if (grandparent != null)
                {
                    siblingPath = Path.Combine(grandparent, "DynaDocs.csproj");
                    if (File.Exists(siblingPath))
                        return grandparent;
                }
            }

            dir = parent;
        }

        // Last resort: use relative path from test output directory
        // DynaDocs.Tests/bin/Debug/net10.0 -> DynaDocs (4 levels up, then into DynaDocs)
        var fallback = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", ".."));
        if (File.Exists(Path.Combine(fallback, "DynaDocs.csproj")))
            return fallback;

        throw new InvalidOperationException(
            $"Could not find DynaDocs.csproj. Started from: {AppContext.BaseDirectory}");
    }
}

/// <summary>
/// Shares one fixture so the CLI builds once for all E2E classes. Parallelism is controlled
/// assembly-wide via AssemblyInfo.cs (DisableTestParallelization).
/// </summary>
[CollectionDefinition("EndToEnd")]
public class EndToEndCollection : ICollectionFixture<EndToEndFixture> { }
