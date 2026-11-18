namespace DynaDocs.Tests.Statechart;

using System.Runtime.InteropServices;
using System.Text.Json.Nodes;
using DynaDocs.Services.Statechart;
using System.Runtime.Versioning;

/// <summary>
/// Readiness and the owned launch, judged by their outcome: stand-in runtimes are released only on the exact startup
/// gate and their exit is the verdict; a component that does not prove itself is refused; a failed or cancelled run
/// leaves no scratch (DR 062 §6).
/// </summary>
[Trait("Job", "contract: DR 062 §6")]
public class StatechartReadyTests
{
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

    private static void WriteReadyModel(string scope)
    {
        File.WriteAllText(Path.Combine(scope, "ReadyModel.tla"),
            "---- MODULE ReadyModel ----\nEXTENDS Naturals\nVARIABLE x\nInit == x = 0\nNext == (x < 2 /\\ x' = x + 1) \\/ (x = 2 /\\ x' = x)\nBound == x < 2\n====\n");
        File.WriteAllText(Path.Combine(scope, "ReadyModel.cfg"), "INIT Init\nNEXT Next\nINVARIANT Bound\nCHECK_DEADLOCK FALSE\n");
    }
}
