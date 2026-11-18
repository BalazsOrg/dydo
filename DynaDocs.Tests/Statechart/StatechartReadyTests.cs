namespace DynaDocs.Tests.Statechart;

using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text.Json;
using System.Text.Json.Nodes;
using DynaDocs.Services.Statechart;
using System.Runtime.Versioning;

/// <summary>
/// A ready <c>dydo statechart</c> run, judged by its outcome: it reaches its verdict offline and publishes the chart,
/// with the managed content unchanged; damaged managed content gives a repair plan and stays in place (DR 062 §1 §2 §6).
/// Most cases need STATECHART_TEST_DEPENDENCY_ROOT, a real verified installation.
/// </summary>
[Trait("Job", "contract: DR 062 §1 §2 §6")]
public class StatechartReadyTests
{
    private static readonly string Relative = Path.Combine(new StatechartAssets().LockDigest, RuntimeInformation.RuntimeIdentifier);

    [SkippableFact]
    public async Task AReadyRun_ReachesItsVerdictOffline_PublishesTheChart_AndChangesNoManagedContent()
    {
        using var scope = new StatechartTestDirectory();
        var fixture = Fixture(scope.Path);
        var before = Content(fixture);
        var result = await RunReadyModel(scope.Path, fixture);
        Assert.True(result.Exit == 1, result.Error);
        Assert.True(File.Exists(Path.Combine(scope.Path, "chart", "report.md")));
        Assert.Empty(StatechartLeftovers.Left(Path.Combine(scope.Path, "state")));
        Assert.Equal(before, Content(fixture));
    }

    [SkippableFact]
    public async Task ChangedManagedBytesYieldARepairPlanAndStayInPlace()
    {
        using var scope = new StatechartTestDirectory();
        var dependencies = Fixture(scope.Path);
        var renderer = Path.Combine(dependencies, Relative, "pele", "content", "package", "dist", "pele.js");
        File.AppendAllText(renderer, "\nchanged managed content");
        var before = Files(dependencies);
        var result = await RunReadyModel(scope.Path, dependencies);
        Assert.True(result.Exit == 3, result.Error);
        Assert.False(Directory.Exists(Path.Combine(scope.Path, "chart")));
        Assert.EndsWith("changed managed content", File.ReadAllText(renderer));
        Assert.Equal(before, Files(dependencies));
    }

    [SkippableFact]
    [UnsupportedOSPlatform("windows")]
    public async Task AManagedRuntimeThatCannotExecuteYieldsARepairPlanAndStaysInPlace()
    {
        Skip.If(OperatingSystem.IsWindows(), "POSIX modes decide whether a runtime can start.");
        using var scope = new StatechartTestDirectory();
        var dependencies = Fixture(scope.Path);
        var java = Directory.GetFiles(Path.Combine(dependencies, Relative, "java", "content"), "java", SearchOption.AllDirectories)
            .Single(path => Path.GetFileName(Path.GetDirectoryName(path)) == "bin");
        File.SetUnixFileMode(java, UnixFileMode.UserRead | UnixFileMode.UserWrite);
        var before = Files(dependencies);
        var result = await RunReadyModel(scope.Path, dependencies);
        Assert.True(result.Exit == 3, result.Error);
        Assert.True(File.Exists(JsonDocument.Parse(result.Output).RootElement.GetProperty("request").GetString()));
        Assert.Equal(before, Files(dependencies));
        Assert.Equal(UnixFileMode.UserRead | UnixFileMode.UserWrite, File.GetUnixFileMode(java));
    }

    // The run is offline, so the restored assets can only come from the payload embedded in dydo.
    [SkippableFact]
    public async Task DamagedAssetsAreExtractedAgainOfflineAndTheRunReachesItsVerdict()
    {
        using var scope = new StatechartTestDirectory();
        var fixture = Fixture(scope.Path);
        var first = await RunReadyModel(scope.Path, fixture);
        Assert.True(first.Exit == 1, first.Error);
        var assets = new StatechartAssets();
        var payload = Path.Combine(scope.Path, "assets", assets.Release, assets.Digest);
        File.AppendAllText(Path.Combine(payload, "skills", "engineering", "statechart", "scripts", "worker.ts"), "\n// tampered");
        File.Delete(Path.Combine(payload, "skills", "engineering", "statechart", "scripts", "java", "statechart-tlc-bootstrap.jar"));
        Directory.Delete(Path.Combine(scope.Path, "chart"), recursive: true);
        var second = await RunReadyModel(scope.Path, fixture);
        Assert.True(second.Exit == 1, second.Error);
        Assert.True(File.Exists(Path.Combine(scope.Path, "chart", "report.md")));
        assets.Verify(payload);
    }

    [SkippableFact]
    public async Task AnUpdateThatKeepsTheLockReverifiesAndRestampsWithoutInstalling()
    {
        using var scope = new StatechartTestDirectory();
        var dependencies = Fixture(scope.Path);
        var receipts = Directory.GetFiles(Path.Combine(dependencies, Relative), ".ready.json", SearchOption.AllDirectories);
        Assert.Equal(4, receipts.Length);
        foreach (var receipt in receipts)
        {
            var identity = JsonNode.Parse(File.ReadAllText(receipt))!.AsObject();
            identity["release"] = "0.0.1";
            identity["payload"] = "earlier-payload";
            File.WriteAllText(receipt, identity.ToJsonString());
        }
        var before = Content(dependencies);
        var result = await RunReadyModel(scope.Path, dependencies);
        Assert.True(result.Exit == 1, result.Error);
        Assert.Equal(before, Content(dependencies));
    }

    [SkippableFact]
    public async Task AnUnchangedSkillFromAnEarlierReleaseUpdatesThroughInstallSkillAndCharts()
    {
        using var scope = new StatechartTestDirectory();
        var fixture = Fixture(scope.Path);
        var project = Path.Combine(scope.Path, "project");
        Directory.CreateDirectory(project);
        var installed = await Run(scope.Path, fixture, ["statechart", "install-skill", "--root", project]);
        Assert.True(installed.Exit == 0, installed.Error);
        var skill = Path.Combine(project, "skills", "engineering", "statechart");
        File.AppendAllText(Path.Combine(skill, "SKILL.md"), "\nEarlier release wording.");
        var receipt = new JsonObject { ["schema"] = 1, ["protocol"] = 1, ["release"] = "0.0.1", ["payload"] = "earlier-payload",
            ["lock"] = "earlier-lock", ["files"] = StatechartInventory.Capture(skill, ".statechart-skill.json") };
        File.WriteAllText(Path.Combine(skill, ".statechart-skill.json"), receipt.ToJsonString());
        var stranded = await RunReadyModel(scope.Path, fixture, "--skill", skill);
        Assert.Equal(2, stranded.Exit);
        var updated = await Run(scope.Path, fixture, ["statechart", "install-skill", "--root", project]);
        Assert.True(updated.Exit == 0, updated.Error);
        var result = await RunReadyModel(scope.Path, fixture, "--skill", skill);
        Assert.True(result.Exit == 1, result.Error);
        Assert.DoesNotContain("Earlier release wording", File.ReadAllText(Path.Combine(skill, "SKILL.md")));
    }

    // A fake runtime speaks the native bootstrap's startup gate, so the owned launch runs without Node, Java or TLC. It exits
    // 9 unless released with the exact go frame, and 1 (its verdict) when it was.
    [SkippableFact]
    [UnsupportedOSPlatform("windows")]
    public async Task TheNativeBootstrapStartsOnlyAfterGo_AndItsExitIsTheVerdict()
    {
        Skip.IfNot(OperatingSystem.IsLinux(), "POSIX ownership runs on Linux.");
        using var scope = new StatechartTestDirectory();
        var node = FakeRuntime(scope.Path, "echo \"ready:1:$3\"\nread -r go\n[ \"$go\" = \"go:1:$3\" ] || exit 9\nexit 1\n");
        var dependencies = new StatechartDependencies(node, "/managed/java", "/managed/tla2tools.jar", "/managed/pele.js");
        await using var lifetime = await PosixStatechartLifetime.StartAsync(StatechartTestBinary.Start(), scope.Path, scope.Path, scope.Path);
        await lifetime.RegisterAsync();
        var (exit, _) = await Forwarded(() => StatechartNode.RunAsync(dependencies, "/payload",
            new JsonObject { ["mode"] = "render-ready" }, lifetime, scope.Path, CancellationToken.None));
        Assert.Equal(1, exit);
        Assert.True(await lifetime.CompleteAsync());
    }

    [SkippableFact]
    [UnsupportedOSPlatform("windows")]
    [Trait("Job", "trap: a runtime that does not answer the exact ready frame must never be released (DR 062 §6 startup gates)")]
    public async Task ABootstrapWithoutTheExactReadyFrameNeverReceivesGo()
    {
        Skip.IfNot(OperatingSystem.IsLinux(), "POSIX ownership runs on Linux.");
        using var scope = new StatechartTestDirectory();
        var released = Path.Combine(scope.Path, "released");
        var node = FakeRuntime(scope.Path, $"echo ready:1\nread -r go\necho \"$go\" > {released}\n");
        await using var lifetime = await PosixStatechartLifetime.StartAsync(StatechartTestBinary.Start(), scope.Path, scope.Path, scope.Path);
        await lifetime.RegisterAsync();
        await Assert.ThrowsAsync<InvalidDataException>(() => StatechartNode.RunAsync(new StatechartDependencies(node, null, null, null),
            "/payload", new JsonObject { ["mode"] = "check" }, lifetime, scope.Path, CancellationToken.None));
        Assert.True(await lifetime.CompleteAsync());
        Assert.False(File.Exists(released) && File.ReadAllText(released).StartsWith("go:", StringComparison.Ordinal));
    }

    // The exec fails here, so the handoff error reaches this process and the independent owner still drains and cleans up.
    [SkippableFact]
    public async Task AFailedRuntimeHandoffSurfacesAndTheOwnerStillCleansUp()
    {
        Skip.IfNot(OperatingSystem.IsLinux(), "POSIX ownership runs on Linux.");
        using var scope = new StatechartTestDirectory();
        var request = CapturedRequest(scope.Path);
        var home = new StatechartHome(Path.Combine(scope.Path, "assets"), Path.Combine(scope.Path, "dependencies"),
            Directory.CreateDirectory(Path.Combine(scope.Path, "state")).FullName);
        var dependencies = new StatechartDependencies(Path.Combine(scope.Path, "missing-node"), null, null, null);
        var directory = Environment.CurrentDirectory;
        try
        {
            await Assert.ThrowsAsync<System.ComponentModel.Win32Exception>(() => PosixStatechartRun.RunAsync(
                request, dependencies, "/payload", home, null, StatechartTestBinary.Start(), CancellationToken.None));
        }
        finally { Environment.CurrentDirectory = directory; }
        Assert.Empty(StatechartLeftovers.Left(request.Caller));
        Assert.False(Directory.Exists(request.Output));
    }

    // Cancellation before the handoff stops the run before any runtime starts; the owner still drains and cleans up.
    [SkippableFact]
    public async Task ACancelledRunNeverHandsOffAndTheOwnerStillCleansUp()
    {
        Skip.IfNot(OperatingSystem.IsLinux(), "POSIX ownership runs on Linux.");
        using var scope = new StatechartTestDirectory();
        var request = CapturedRequest(scope.Path);
        var home = new StatechartHome(Path.Combine(scope.Path, "assets"), Path.Combine(scope.Path, "dependencies"),
            Directory.CreateDirectory(Path.Combine(scope.Path, "state")).FullName);
        var directory = Environment.CurrentDirectory;
        await Assert.ThrowsAnyAsync<OperationCanceledException>(() => PosixStatechartRun.RunAsync(request,
            new StatechartDependencies("/managed/node", null, null, null), "/payload", home, null, StatechartTestBinary.Start(),
            new CancellationToken(canceled: true)));
        Assert.Equal(directory, Environment.CurrentDirectory);
        Assert.Empty(StatechartLeftovers.Left(request.Caller));
        Assert.False(Directory.Exists(request.Output));
    }

    // Readiness under a real owned lifetime and the real payload's sealed bootstrap: stand-in runtimes answer the version,
    // render, SANY and TLC calls, TLC replaying an upstream recording; the stand-in jar passes only under its own pin.
    [SkippableTheory]
    [UnsupportedOSPlatform("windows")]
    [InlineData(true)]
    [InlineData(false)]
    public async Task ReadinessProvesTheSelectedRuntimesRendererAndCheckerUnderOwnership(bool full)
    {
        Skip.IfNot(OperatingSystem.IsLinux(), "POSIX ownership runs on Linux.");
        using var scope = new StatechartTestDirectory();
        StatechartExecutable.RelaunchThrough(StatechartTestBinary.Start().FileName);
        var (dependencies, payload, home, _, digest) = await StandIns(scope.Path, StatechartStandIns.Node(), StatechartStandIns.CleanRun);
        await StatechartReady.VerifyAsync(dependencies, payload, home, digest, full, CancellationToken.None);
        AssertCleaned(home.State);
    }

    [SkippableTheory]
    [UnsupportedOSPlatform("windows")]
    [InlineData("checker")]
    [InlineData("node")]
    [InlineData("render")]
    [InlineData("sany")]
    [InlineData("tlc")]
    public async Task ReadinessRefusesAnyComponentThatDoesNotProveItself(string failing)
    {
        Skip.IfNot(OperatingSystem.IsLinux(), "POSIX ownership runs on Linux.");
        using var scope = new StatechartTestDirectory();
        StatechartExecutable.RelaunchThrough(StatechartTestBinary.Start().FileName);
        var node = StatechartStandIns.Node(failing == "node" ? "v22.17.0" : "v26.13.0", failing == "render" ? 1 : 0);
        var (dependencies, payload, home, _, digest) = await StandIns(scope.Path, node,
            failing == "tlc" ? StatechartStandIns.ViolatedRun : StatechartStandIns.CleanRun, failing == "sany" ? 1 : 0);
        if (failing == "checker") digest = new StatechartAssets().CheckerDigest;
        await Assert.ThrowsAsync<IOException>(() =>
            StatechartReady.VerifyAsync(dependencies, payload, home, digest, full: true, CancellationToken.None));
        AssertCleaned(home.State);
    }

    // Stand-in Node and Java scripts, the stand-in jar and its pin, and the real payload extracted under a private home.
    // Callers name the relaunch build first: an async helper's own setting would not reach them.
    [UnsupportedOSPlatform("windows")]
    private static async Task<(StatechartDependencies Dependencies, string Payload, StatechartHome Home, string Log, string Digest)>
        StandIns(string root, string node, string tlc, int sany = 0)
    {
        var home = new StatechartHome(Path.Combine(root, "assets"), Path.Combine(root, "dependencies"), Path.Combine(root, "state"));
        var payload = await new StatechartAssets().ExtractAsync(home.Assets, CancellationToken.None);
        var log = Path.Combine(root, "java.log");
        var bin = Path.Combine(root, "runtimes");
        var jar = Path.Combine(bin, "tla2tools.jar");
        Directory.CreateDirectory(bin);
        File.WriteAllBytes(jar, StatechartStandIns.Jar());
        var dependencies = new StatechartDependencies(StatechartStandIns.Executable(Path.Combine(bin, "node"), node),
            StatechartStandIns.Executable(Path.Combine(bin, "java"), StatechartStandIns.Java(log, tlc, sany: sany)), jar,
            Path.Combine(bin, "pele.js"));
        return (dependencies, payload, home, log, StatechartStandIns.Sha256(File.ReadAllBytes(jar)));
    }

    private static void AssertCleaned(string state)
    {
        Assert.NotEmpty(Directory.GetFiles(state, "scratch-*.journal"));
        Assert.Empty(StatechartLeftovers.Left(state));
    }

    private static StatechartRequest CapturedRequest(string root)
    {
        var caller = Directory.CreateDirectory(Path.Combine(root, "caller")).FullName;
        WriteReadyModel(caller);
        return StatechartRequest.Capture(["ReadyModel.tla", "--out", "chart"], caller, new StatechartAssets(), RuntimeInformation.RuntimeIdentifier);
    }

    [UnsupportedOSPlatform("windows")]
    private static string FakeRuntime(string root, string script)
    {
        var path = Path.Combine(root, "fake-node");
        File.WriteAllText(path, "#!/bin/sh\n" + script);
        File.SetUnixFileMode(path, UnixFileMode.UserRead | UnixFileMode.UserExecute);
        return path;
    }

    // The native bootstrap forwards the child's output to this process's standard error.
    private static async Task<(int Exit, string Forwarded)> Forwarded(Func<Task<int>> run)
    {
        var original = Console.Error;
        using var captured = new StringWriter();
        Console.SetError(captured);
        try { return (await run(), captured.ToString()); }
        finally { Console.SetError(original); }
    }

    private static string[] Content(string dependencies) => Files(Path.Combine(dependencies, Relative))
        .Where(file => !file.Contains(".ready.json", StringComparison.Ordinal)).ToArray();

    private static string Fixture(string scope, string? source = null) => PrivateCopy(source ?? FixtureSource(), scope);

    private static string FixtureSource()
    {
        var fixture = Environment.GetEnvironmentVariable("STATECHART_TEST_DEPENDENCY_ROOT");
        Skip.If(fixture is null, "STATECHART_TEST_DEPENDENCY_ROOT must name a real verified managed installation (Node, Java, TLC, Pele).");
        Assert.True(Path.IsPathFullyQualified(fixture) && Directory.Exists(fixture),
            "STATECHART_TEST_DEPENDENCY_ROOT must name a real verified source installation; tests copy it before running.");
        return fixture;
    }

    private static string PrivateCopy(string fixture, string scope)
    {
        var dependencies = Path.Combine(scope, "dependencies");
        Copy(Path.Combine(fixture, Relative), Path.Combine(dependencies, Relative));
        return dependencies;
    }

    // Runs the built dydo offline on a model whose Bound breaks after two steps.
    private static Task<(int Exit, string Output, string Error)> RunReadyModel(string scope, string dependencies, params string[] options)
    {
        WriteReadyModel(scope);
        return Run(scope, dependencies, ["statechart", "ReadyModel.tla", "--out", "chart", .. options]);
    }

    private static void WriteReadyModel(string scope)
    {
        File.WriteAllText(Path.Combine(scope, "ReadyModel.tla"),
            "---- MODULE ReadyModel ----\nEXTENDS Naturals\nVARIABLE x\nInit == x = 0\nNext == (x < 2 /\\ x' = x + 1) \\/ (x = 2 /\\ x' = x)\nBound == x < 2\n====\n");
        File.WriteAllText(Path.Combine(scope, "ReadyModel.cfg"), "INIT Init\nNEXT Next\nINVARIANT Bound\nCHECK_DEADLOCK FALSE\n");
    }

    // Runs the built dydo offline with the given managed dependencies and private asset and state roots.
    private static async Task<(int Exit, string Output, string Error)> Run(string scope, string dependencies, string[] arguments,
        string? java = null)
    {
        var start = StatechartTestBinary.Start();
        start.WorkingDirectory = scope;
        start.UseShellExecute = false;
        start.CreateNoWindow = true;
        start.RedirectStandardOutput = true;
        start.RedirectStandardError = true;
        foreach (var argument in arguments) start.ArgumentList.Add(argument);
        foreach (var name in start.Environment.Keys.Where(name => name.StartsWith("DYDO_", StringComparison.Ordinal)).ToArray())
            start.Environment.Remove(name);
        start.Environment["DYDO_STATECHART_DEPENDENCY_ROOT"] = dependencies;
        start.Environment["DYDO_STATECHART_ASSET_ROOT"] = Path.Combine(scope, "assets");
        start.Environment["DYDO_STATECHART_STATE_ROOT"] = Path.Combine(scope, "state");
        foreach (var name in new[] { "HTTP_PROXY", "HTTPS_PROXY", "ALL_PROXY" }) start.Environment[name] = "http://127.0.0.1:9";
        start.Environment["NO_PROXY"] = "";
        start.Environment["PATH"] = "";
        if (java is null) start.Environment.Remove("JAVA");
        else start.Environment["JAVA"] = java;
        start.Environment.Remove("TLA2TOOLS");
        using var child = Process.Start(start)!;
        var output = child.StandardOutput.ReadToEndAsync();
        var errors = child.StandardError.ReadToEndAsync();
        await child.WaitForExitAsync();
        return (child.ExitCode, await output, await errors);
    }

    private static string[] Files(string root) => Directory.GetFiles(root, "*", SearchOption.AllDirectories)
        .Order(StringComparer.Ordinal).Select(file => Path.GetRelativePath(root, file) + ":" +
            Convert.ToHexStringLower(SHA256.HashData(File.ReadAllBytes(file)))).ToArray();

    private static void Copy(string source, string target)
    {
        Assert.True(Directory.Exists(source), "Missing verified managed fixture component root.");
        Directory.CreateDirectory(target);
        foreach (var entry in Directory.EnumerateFileSystemEntries(source))
        {
            var attributes = File.GetAttributes(entry);
            var directory = (attributes & FileAttributes.Directory) != 0;
            var destination = Path.Combine(target, Path.GetFileName(entry));
            // POSIX runtime archives carry relative links; the managed inventory records them as links.
            if ((attributes & FileAttributes.ReparsePoint) != 0)
            {
                var link = directory ? new DirectoryInfo(entry).LinkTarget! : new FileInfo(entry).LinkTarget!;
                if (directory) Directory.CreateSymbolicLink(destination, link);
                else File.CreateSymbolicLink(destination, link);
            }
            else if (directory) Copy(entry, destination);
            else File.Copy(entry, destination, overwrite: false);
        }
    }
}
