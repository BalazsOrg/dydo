namespace DynaDocs.Tests.Statechart;

using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Text.Json.Nodes;
using DynaDocs.Services.Statechart;
using System.Runtime.Versioning;

/// <summary>
/// <c>dydo statechart</c> and <c>dydo statechart resume</c> as a caller sees them (DR 062 §1-§5): the exit code, whether
/// a chart was published, and whether the saved request was kept or consumed. Missing prerequisites exit 3 with a saved
/// request; an error exits 2 and saves nothing; nothing installs or publishes without proof.
/// </summary>
public class StatechartPublicFlowTests
{
    [Fact]
    [Trait("Job", "contract: DR 062 §1-§5")]
    public async Task MissingPrerequisites_Exit3_SaveTheRequest_AndInstallAndPublishNothing()
    {
        using var scope = new StatechartTestDirectory();
        Model(scope.Path);
        var result = await Invoke(scope.Path, ["statechart", "spaced model.tla", "--out", "chart output"]);
        Assert.Equal(3, result.Exit);
        Assert.True(File.Exists(RequestOf(result)));
        Assert.False(Directory.Exists(Path.Combine(scope.Path, "dependencies")));
        Assert.False(Directory.Exists(Path.Combine(scope.Path, "chart output")));
        Assert.Empty(StatechartLeftovers.Left(Path.Combine(scope.Path, "state")));
    }

    [Fact]
    [Trait("Job", "contract: DR 062 §1-§5")]
    public async Task ChangedSavedInputs_RefuseTheResume_AndKeepTheRequest()
    {
        using var scope = new StatechartTestDirectory();
        Model(scope.Path);
        var path = RequestOf(await Invoke(scope.Path, ["statechart", "spaced model.tla"]));
        File.AppendAllText(Path.Combine(scope.Path, "spaced model.tla"), "\nchanged during approval");
        var resumed = await Invoke(scope.Path, ["statechart", "resume", "--request", path]);
        Assert.Equal(2, resumed.Exit);
        Assert.True(File.Exists(path));
        Assert.False(Directory.Exists(Path.Combine(scope.Path, "dependencies")));
    }

    // Only managed content is repaired; a JAVA the caller named that is missing, cannot start or is too old stays the
    // caller's error, before any plan.
    [SkippableTheory]
    [Trait("Job", "contract: DR 062 §1-§5")]
    [UnsupportedOSPlatform("windows")]
    [InlineData("missing")]
    [InlineData("not executable")]
    [InlineData("older than Java11")]
    public async Task AnInvalidExplicitJava_IsAnError_WithoutAPlan(string java)
    {
        Skip.If(java != "missing" && OperatingSystem.IsWindows(), "POSIX modes decide whether a runtime can start.");
        using var scope = new StatechartTestDirectory();
        Model(scope.Path);
        var (explicitJava, path) = java switch
        {
            "missing" => ("missing java.exe", ""),
            "not executable" => (NotExecutableJava(scope.Path), ""),
            _ => OlderJava(scope.Path),
        };
        var result = await Invoke(scope.Path, ["statechart", "spaced model.tla"], java: explicitJava, path: path);
        Assert.Equal(2, result.Exit);
        Assert.Equal("", result.Output);
        Assert.False(Directory.Exists(Path.Combine(scope.Path, "dependencies")));
        Assert.False(Directory.Exists(Path.Combine(scope.Path, "state", "requests")));
    }

    [UnsupportedOSPlatform("windows")]
    private static string NotExecutableJava(string root)
    {
        var java = Path.Combine(root, "java");
        File.WriteAllText(java, "#!/bin/sh\necho 'openjdk version \"21.0.1\"' >&2\n");
        File.SetUnixFileMode(java, UnixFileMode.UserRead | UnixFileMode.UserWrite);
        return java;
    }

    [UnsupportedOSPlatform("windows")]
    private static (string Java, string Path) OlderJava(string root)
    {
        var bin = Runtimes(root, "v26.13.0", "1.8.0_402");
        return (Path.Combine(bin, "java"), bin);
    }

    // The IDE runtime lives under Program Files, which Linux lacks, so a JetBrains folder in the caller's directory is no Java.
    [SkippableFact]
    [UnsupportedOSPlatform("windows")]
    [Trait("Job", "contract: DR 062 §1-§5")]
    [Trait("Job", "trap: a JetBrains runtime in the caller's directory must not be taken for Java outside Windows")]
    public async Task AnIdeJavaIsNeverLookedForOutsideWindows()
    {
        Skip.If(OperatingSystem.IsWindows(), "Windows looks for the IDE runtime under Program Files.");
        using var scope = new StatechartTestDirectory();
        Model(scope.Path);
        var java = Path.Combine(scope.Path, "JetBrains", "ide", "jbr", "bin", "java.exe");
        Directory.CreateDirectory(Path.GetDirectoryName(java)!);
        File.WriteAllText(java, "#!/bin/sh\necho 'openjdk version \"21.0.1\"' >&2\n");
        File.SetUnixFileMode(java, UnixFileMode.UserRead | UnixFileMode.UserWrite | UnixFileMode.UserExecute);
        var result = await Invoke(scope.Path, ["statechart", "spaced model.tla"]);
        Assert.Equal(3, result.Exit);
        Assert.Contains("java", Missing(result));
    }

    // Fake runtimes answer the version probes, so discovery runs its real owned probes without Node or Java.
    [SkippableTheory]
    [Trait("Job", "contract: DR 062 §1-§5")]
    [UnsupportedOSPlatform("windows")]
    [InlineData("v26.13.0", "21.0.1", "tlc,pele")]
    [InlineData("v22.17.0", "1.8.0_402", "node,java,tlc,pele")]
    public async Task ADiscoveredRuntime_IsUsed_OnlyWhenItsProbedVersionIsSupported(string node, string java, string missing)
    {
        Skip.If(OperatingSystem.IsWindows(), "POSIX modes decide whether a runtime can start.");
        using var scope = new StatechartTestDirectory();
        Model(scope.Path);
        var result = await Invoke(scope.Path, ["statechart", "spaced model.tla"], path: Runtimes(scope.Path, node, java));
        Assert.Equal(3, result.Exit);
        Assert.Equal(missing.Split(','), Missing(result));
        Assert.Empty(StatechartLeftovers.Left(Path.Combine(scope.Path, "state")));
    }

    [Fact]
    [Trait("Job", "contract: DR 062 §1-§5")]
    public async Task ADamagedManagedComponent_IsPlannedForRepair_AndLeftInPlace()
    {
        using var scope = new StatechartTestDirectory();
        Model(scope.Path);
        var assets = new StatechartAssets();
        var root = Path.Combine(scope.Path, "dependencies", assets.LockDigest, RuntimeInformation.RuntimeIdentifier);
        Directory.CreateDirectory(Path.Combine(root, "tlc"));
        File.WriteAllText(Path.Combine(root, ".statechart-store"), assets.LockDigest);
        var damaged = Path.Combine(root, "tlc", "tla2tools.jar");
        File.WriteAllText(damaged, "damaged managed bytes");
        var result = await Invoke(scope.Path, ["statechart", "spaced model.tla"]);
        Assert.Equal(3, result.Exit);
        Assert.Contains("tlc", Missing(result));
        Assert.Equal("damaged managed bytes", File.ReadAllText(damaged));
    }

    [Fact]
    [Trait("Job", "contract: DR 062 §1-§5")]
    public async Task ResumingACompletedRequest_ExitsWithItsRecordedVerdict_WithoutPreparingAgain()
    {
        using var scope = new StatechartTestDirectory();
        Model(scope.Path);
        var path = RequestOf(await Invoke(scope.Path, ["statechart", "spaced model.tla", "--out", "chart"]));
        var store = new StatechartRequests(Home(scope.Path), new StatechartAssets(), RuntimeInformation.RuntimeIdentifier);
        var request = store.Load(path, validateInputs: false);
        Directory.CreateDirectory(request.Output);
        File.WriteAllText(Path.Combine(request.Output, "report.md"), "Bound breaks");
        store.Complete(path, request, 1);
        var resumed = await Invoke(scope.Path, ["statechart", "resume", "--request", path]);
        Assert.Equal(1, resumed.Exit);
        Assert.False(Directory.Exists(Path.Combine(scope.Path, "dependencies")));
    }

    // The approved continuation installs only through the network; offline, it fails without placing any component.
    [Fact]
    [Trait("Job", "contract: DR 062 §1-§5")]
    public async Task AnApprovedResumeWithoutNetwork_KeepsTheRequest_AndInstallsNothing()
    {
        using var scope = new StatechartTestDirectory();
        Model(scope.Path);
        var path = RequestOf(await Invoke(scope.Path, ["statechart", "spaced model.tla"]));
        var resumed = await Invoke(scope.Path, ["statechart", "resume", "--request", path]);
        Assert.Equal(2, resumed.Exit);
        Assert.True(File.Exists(path));
        var managed = new StatechartManaged(Home(scope.Path), new StatechartAssets(), RuntimeInformation.RuntimeIdentifier);
        Assert.Equal(["node", "java", "tlc", "pele"], StatechartManaged.Missing(managed.Read()));
    }

    // A framework-dependent dydo runs under the dotnet host; the continuation it proposes must start that same host and
    // assembly, or the approved resume would launch nothing.
    [SkippableFact]
    [Trait("Job", "contract: DR 062 §1-§5")]
    [Trait("Job", "trap: a framework-dependent dydo's continuation must name its dotnet host and assembly (DR 062 §2)")]
    public async Task AFrameworkDependentLaunchContinuesThroughTheSameHostAndAssembly()
    {
        var assembly = Path.ChangeExtension(StatechartTestBinary.Start().FileName, ".dll");
        Skip.IfNot(File.Exists(assembly), "The selected dydo build has no framework-dependent assembly.");
        var host = Path.GetFileNameWithoutExtension(Environment.ProcessPath) == "dotnet"
            ? Environment.ProcessPath : Environment.GetEnvironmentVariable("DOTNET_HOST_PATH");
        Skip.If(host is null, "No dotnet host is known to this test process.");
        using var scope = new StatechartTestDirectory();
        Model(scope.Path);
        var launcher = new ProcessStartInfo(host!);
        launcher.ArgumentList.Add(assembly);
        var result = await Invoke(scope.Path, ["statechart", "spaced model.tla"], launcher: launcher);
        Assert.True(result.Exit == 3, result.Error);
        using var plan = JsonDocument.Parse(result.Output);
        var continuation = plan.RootElement.GetProperty("installAndContinue").EnumerateArray().Select(item => item.GetString()!).ToArray();
        Assert.Equal("dotnet", Path.GetFileNameWithoutExtension(continuation[0]));
        Assert.Equal(assembly, continuation[1]);
    }

    // The flow in-process under a test-built lock: stand-in components arrive from loopback and face every real check. Only
    // the run is substituted, because the real one replaces the calling process with the checking runtime.
    [SkippableFact]
    [Trait("Job", "contract: DR 062 §1-§5")]
    [UnsupportedOSPlatform("windows")]
    public async Task AnApprovedResume_InstallsAndContinues_ThenLaterRunsReuseTheStore()
    {
        Skip.IfNot(OperatingSystem.IsLinux(), "POSIX ownership runs on Linux.");
        StatechartExecutable.RelaunchThrough(StatechartTestBinary.Start().FileName);
        using var scope = new StatechartTestDirectory();
        var caller = Directory.CreateDirectory(Path.Combine(scope.Path, "caller")).FullName;
        Model(caller);
        var served = StatechartStandIns.Components(Path.Combine(scope.Path, "java.log"), StatechartStandIns.CleanRun);
        var status = 1;
        Task<int> Run(StatechartRequest request, StatechartDependencies dependencies, string payload, StatechartHome home,
            TextWriter output, CancellationToken token, StatechartContinuation? continuation)
        {
            Directory.CreateDirectory(request.Output);
            File.WriteAllText(Path.Combine(request.Output, "report.md"), "Bound breaks");
            return Task.FromResult(status);
        }
        await StatechartStandIns.WithoutCallerRuntimes(() => StatechartStandIns.Serve(served, async root =>
        {
            var assets = StatechartStandIns.Assets(StatechartStandIns.Lock(root, served, StatechartStandIns.Sha256(served["/tlc/tla2tools.jar"])));
            var rid = RuntimeInformation.RuntimeIdentifier;
            var home = Home(scope.Path);
            var flow = new StatechartFlow(home, assets, rid, Run);
            var store = new StatechartRequests(home, assets, rid);
            using var plan = new StringWriter();
            Assert.Equal(3, await flow.StartAsync(["spaced model.tla", "--out", "first"], caller, plan, CancellationToken.None));
            var path = JsonNode.Parse(plan.ToString())!["request"]!.GetValue<string>();
            Assert.Equal(1, await flow.ResumeAsync(path, TextWriter.Null, CancellationToken.None));
            Assert.Equal(1, store.Completed(path, store.Load(path, validateInputs: false)));
            Assert.Empty(StatechartManaged.Missing(new StatechartManaged(home, assets, rid).Read()));

            status = 0;
            Assert.Equal(0, await flow.StartAsync(["spaced model.tla", "--out", "second"], caller, TextWriter.Null, CancellationToken.None));
            foreach (var receipt in Directory.GetFiles(new StatechartManaged(home, assets, rid).Root, ".ready.json", SearchOption.AllDirectories))
            {
                var identity = JsonNode.Parse(File.ReadAllText(receipt))!.AsObject();
                identity["release"] = "0.0.1";
                File.WriteAllText(receipt, identity.ToJsonString());
            }
            Assert.Equal(0, await flow.StartAsync(["spaced model.tla", "--out", "third"], caller, TextWriter.Null, CancellationToken.None));

            var request = StatechartRequest.Capture(["spaced model.tla", "--out", "fourth"], caller, assets, rid);
            var saved = store.Save(request);
            status = 2;
            Assert.Equal(2, await flow.ResumeAsync(saved, TextWriter.Null, CancellationToken.None));
            Assert.Null(store.Completed(saved, request));
        }));
    }

    #region Publication: only a complete verdict the checker's exit agrees with reaches the caller's destination

    [Theory]
    [Trait("Job", "contract: dydo-commands-utilities.md dydo statechart (a complete checker verdict of 0 or 1 publishes the chart)")]
    [InlineData(1, 1, false, 1)]
    [InlineData(0, 0, false, 0)]
    [InlineData(1, 0, true, 0)]
    public Task OnlyACompleteVerdictTheCheckerExitAgreesWithIsPublished(int verdict, int status, bool renderError, int expected) =>
        Publication(verdict, status, renderError, expected);

    [Theory]
    [Trait("Job", "contract: dydo-commands-utilities.md dydo statechart (a verdict that is not a complete checker verdict publishes nothing and exits 2)")]
    [InlineData(1, 0, false, 2)]
    [InlineData(7, 7, false, 2)]
    public Task AnIncompleteVerdictIsNotPublished(int verdict, int status, bool renderError, int expected) =>
        Publication(verdict, status, renderError, expected);

    private static async Task Publication(int verdict, int status, bool renderError, int expected)
    {
        using var scope = new StatechartTestDirectory();
        var done = Directory.CreateDirectory(Path.Combine(scope.Path, "stage", "done")).FullName;
        File.WriteAllText(Path.Combine(scope.Path, "stage", "verdict.json"), "{\"verdict\":" + verdict + "}");
        File.WriteAllText(Path.Combine(done, "report.md"), "Bound breaks");
        if (renderError) File.WriteAllText(Path.Combine(done, "render-error.json"), "{}");
        var destination = Path.Combine(scope.Path, "caller", "chart");
        Assert.Equal(expected, await StatechartRunner.PublishAsync(Path.Combine(scope.Path, "stage"), status, destination,
            TextWriter.Null, CancellationToken.None));
        Assert.Equal(expected != 2, File.Exists(Path.Combine(destination, "report.md")));
    }

    [Theory]
    [Trait("Job", "contract: dydo-commands-utilities.md dydo statechart (a run that cannot publish a complete checker verdict creates no destination)")]
    [InlineData("verdict.json")]
    [InlineData("done/report.md")]
    [InlineData("cancelled")]
    public async Task AnIncompleteStageOrACancelledRun_PublishesNothing(string missing)
    {
        using var scope = new StatechartTestDirectory();
        Directory.CreateDirectory(Path.Combine(scope.Path, "done"));
        File.WriteAllText(Path.Combine(scope.Path, "verdict.json"), "{\"verdict\":1}");
        File.WriteAllText(Path.Combine(scope.Path, "done", "report.md"), "Bound breaks");
        var destination = Path.Combine(scope.Path, "chart");
        if (missing == "cancelled")
            await Assert.ThrowsAnyAsync<OperationCanceledException>(() =>
                StatechartRunner.PublishAsync(scope.Path, 1, destination, TextWriter.Null, new CancellationToken(canceled: true)));
        else
        {
            File.Delete(Path.Combine(scope.Path, missing));
            Assert.Equal(2, await StatechartRunner.PublishAsync(scope.Path, 1, destination, TextWriter.Null, CancellationToken.None));
        }
        Assert.False(Directory.Exists(destination));
    }

    [SkippableFact]
    [Trait("Job", "edge: publication replaces an empty plain destination directory with the finished chart")]
    public void PublicationReplacesAnEmptyPlainDestination() => Destination("an empty directory", published: true);

    [SkippableTheory]
    [Trait("Job", "trap: publication refuses a destination that holds anything foreign, and keeps both it and the finished chart")]
    [InlineData("a directory holding a foreign file")]
    [InlineData("a foreign file")]
    [InlineData("a dangling link")]
    public void PublicationReplacesOnlyAnEmptyPlainDestination_AndRetainsAnythingForeign(string destinationState) =>
        Destination(destinationState, published: false);

    private static void Destination(string destinationState, bool published)
    {
        Skip.If(destinationState == "a dangling link" && OperatingSystem.IsWindows(), "Creating symbolic links needs Windows developer mode.");
        using var scope = new StatechartTestDirectory();
        var done = Directory.CreateDirectory(Path.Combine(scope.Path, "done")).FullName;
        File.WriteAllText(Path.Combine(done, "report.md"), "complete");
        var destination = Path.Combine(scope.Path, "output");
        var foreign = destination;
        switch (destinationState)
        {
            case "an empty directory":
                Directory.CreateDirectory(destination);
                break;
            case "a directory holding a foreign file":
                Directory.CreateDirectory(destination);
                foreign = Path.Combine(destination, "foreign");
                File.WriteAllText(foreign, "keep");
                break;
            case "a foreign file":
                File.WriteAllText(destination, "keep");
                break;
            case "a dangling link":
                File.CreateSymbolicLink(destination, Path.Combine(scope.Path, "missing"));
                break;
        }

        if (published)
        {
            StatechartRunner.Publish(done, destination);
            Assert.Equal("complete", File.ReadAllText(Path.Combine(destination, "report.md")));
            return;
        }
        Assert.Throws<IOException>(() => StatechartRunner.Publish(done, destination));
        Assert.Equal("complete", File.ReadAllText(Path.Combine(done, "report.md")));
        if (destinationState == "a dangling link")
            Assert.NotNull(new FileInfo(destination).LinkTarget);
        else
            Assert.Equal("keep", File.ReadAllText(foreign));
    }

    #endregion

    private static StatechartHome Home(string root) =>
        new(Path.Combine(root, "assets"), Path.Combine(root, "dependencies"), Path.Combine(root, "state"));

    private static string RequestOf((int Exit, string Output, string Error) result)
    {
        Assert.True(result.Exit == 3, result.Error);
        using var plan = JsonDocument.Parse(result.Output);
        return plan.RootElement.GetProperty("request").GetString()!;
    }

    private static string?[] Missing((int Exit, string Output, string Error) result)
    {
        using var plan = JsonDocument.Parse(result.Output);
        return [.. plan.RootElement.GetProperty("missing").EnumerateArray().Select(item => item.GetProperty("name").GetString())];
    }

    [UnsupportedOSPlatform("windows")]
    private static string Runtimes(string root, string node, string java)
    {
        var bin = Directory.CreateDirectory(Path.Combine(root, "bin")).FullName;
        foreach (var (name, script) in new[] { ("node", $"echo {node}"), ("java", $"echo 'openjdk version \"{java}\"' >&2") })
        {
            File.WriteAllText(Path.Combine(bin, name), "#!/bin/sh\n" + script + "\n");
            File.SetUnixFileMode(Path.Combine(bin, name), UnixFileMode.UserRead | UnixFileMode.UserExecute);
        }
        return bin;
    }

    private static void Model(string root)
    {
        File.WriteAllText(Path.Combine(root, "spaced model.tla"), "---- MODULE Model ----\nVARIABLE x\nInit == x = 0\nNext == x' = x\n====\n");
        File.WriteAllText(Path.Combine(root, "spaced model.cfg"), "INIT Init\nNEXT Next\nCHECK_DEADLOCK FALSE\n");
    }

    private static Task<(int Exit, string Output, string Error)> Invoke(string root, string[] arguments, string? java = null,
        string path = "", ProcessStartInfo? launcher = null) =>
        StatechartOfflineProcess.Run(StatechartOfflineProcess.Start(root, path, arguments, java, launcher));
}
