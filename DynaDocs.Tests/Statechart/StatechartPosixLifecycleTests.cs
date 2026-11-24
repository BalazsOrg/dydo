namespace DynaDocs.Tests.Statechart;

using System.Diagnostics;
using System.Runtime.InteropServices;
using DynaDocs.Services.Statechart;

/// <summary>
/// The POSIX statechart owner, judged by what it leaves: a drained success, a failed handoff and a lost parent all clean
/// their scratch; an unproven completion records no verdict; a drain stops the checking child it owns (DR 062 §6).
/// </summary>
public class StatechartPosixLifecycleTests
{
    [Fact]
    [Trait("Job", "contract: DR 062 §6 (on Linux, a registered lifetime cleans its scratch after the drain)")]
    public async Task ARegisteredLifetime_CleansItsScratchAfterTheDrain()
    {
        using var scope = new StatechartTestDirectory();
        if (!OperatingSystem.IsLinux())
        {
            await Assert.ThrowsAsync<PlatformNotSupportedException>(() =>
                PosixStatechartLifetime.StartAsync(new ProcessStartInfo("missing"), scope.Path, scope.Path, scope.Path));
            return; // Platform refusal is not Linux execution evidence.
        }
        await using var lifetime = await PosixStatechartLifetime.StartAsync(StatechartTestBinary.Start(), scope.Path, scope.Path, scope.Path);
        await lifetime.RegisterAsync();
        File.WriteAllText(Path.Combine(lifetime.Stage, "owned-file"), "checking");
        Assert.True(await lifetime.CompleteAsync());
        Assert.Empty(StatechartLeftovers.Left(scope.Path));
    }

    // The socket path only serves the first connection; the connected pair no longer needs it.
    [SkippableFact]
    [Trait("Job", "trap: a POSIX channel must not leave its socket file in the temp directory")]
    public async Task AConnectedChannelLeavesNoSocketFileBehind()
    {
        Skip.If(OperatingSystem.IsWindows(), "Windows pipes leave no socket file.");
        var name = "statechart-" + Guid.NewGuid().ToString("N");
        var socket = Path.Combine(Path.GetTempPath(), "CoreFxPipe_" + name);
        using var server = PosixStatechartChannel.Listen(name);
        var connecting = PosixStatechartChannel.ConnectAsync(name);
        await server.AcceptAsync();
        using var client = await connecting;
        Assert.False(File.Exists(socket));
        await client.SendAsync("still connected");
        Assert.Equal("still connected", await server.ReadAsync());
    }

    [Fact]
    [Trait("Job", "contract: DR 062 §6 (on Linux, a lifetime whose parent is lost still drains and cleans its scratch)")]
    public async Task ALostParent_StillDrainsAndCleansTheScratch()
    {
        using var scope = new StatechartTestDirectory();
        if (!OperatingSystem.IsLinux())
        {
            await Assert.ThrowsAsync<PlatformNotSupportedException>(() =>
                PosixStatechartLifetime.StartAsync(new ProcessStartInfo("missing"), scope.Path, scope.Path, scope.Path));
            return;
        }
        var lifetime = await PosixStatechartLifetime.StartAsync(StatechartTestBinary.Start(), scope.Path, scope.Path, scope.Path);
        await lifetime.RegisterAsync();
        var stage = lifetime.Stage;
        var temp = lifetime.Temp;
        File.WriteAllText(Path.Combine(stage, "owned-file"), "checking");
        await lifetime.DisposeAsync();
        Assert.False(Directory.Exists(stage));
        Assert.False(Directory.Exists(temp));
    }

    private const string LinuxOnly = "POSIX ownership runs on Linux.";

    // The exec fails here, so the handoff returns its error and this process keeps the owner to drain and clean up.
    [SkippableFact]
    [Trait("Job", "contract: DR 062 §6 (after a failed handoff the owner still drains and cleans the scratch)")]
    public async Task AFailedHandoffLeavesTheOwnerToDrainAndCleanUp()
    {
        Skip.IfNot(OperatingSystem.IsLinux(), LinuxOnly);
        using var scope = new StatechartTestDirectory();
        await using var lifetime = await PosixStatechartLifetime.StartAsync(StatechartTestBinary.Start(), scope.Path, scope.Path, scope.Path);
        await lifetime.RegisterAsync();
        var target = new ProcessStartInfo(Path.Combine(scope.Path, "missing-runtime")) { WorkingDirectory = Environment.CurrentDirectory };
        await Assert.ThrowsAsync<System.ComponentModel.Win32Exception>(() => lifetime.HandoffAsync(target, null));
        Assert.True(await lifetime.CompleteAsync());
        Assert.False(Directory.Exists(lifetime.Stage));
        Assert.False(Directory.Exists(lifetime.Temp));
    }

    // The exec fails here, so a process started after the handoff stands in for the runtime: the execution lock it
    // would hold is open in it. A continued request whose runtime never starts records no verdict, and the owner cleans up.
    [SkippableFact]
    [Trait("Job", "contract: DR 062 §5 §6 (a continued request whose runtime never starts records no verdict, and the owner cleans the scratch)")]
    [Trait("Job", "trap: DYD-548 the execution lock survives exec into the continued runtime")]
    public async Task AContinuedHandoff_HandsTheRuntimeItsExecutionLock_AndAFailedOneRecordsNoVerdict()
    {
        Skip.IfNot(OperatingSystem.IsLinux(), LinuxOnly);
        using var scope = new StatechartTestDirectory();
        using var roots = new ScopedRoots(scope.Path);
        var (store, request, path) = SavedRequest(scope.Path);
        using var executionLock = store.Lock(path);
        var descriptor = executionLock.SafeFileHandle.DangerousGetHandle().ToInt32();
        Assert.False(await ChildHolds(descriptor));
        await using var lifetime = await PosixStatechartLifetime.StartAsync(StatechartTestBinary.Start(), scope.Path, scope.Path, scope.Path);
        await lifetime.RegisterAsync();
        await Assert.ThrowsAsync<System.ComponentModel.Win32Exception>(() => lifetime.HandoffAsync(
            new ProcessStartInfo(Path.Combine(scope.Path, "missing-runtime")), new StatechartContinuation(path, executionLock)));
        Assert.True(await ChildHolds(descriptor));
        Assert.True(await lifetime.CompleteAsync());
        Assert.Null(store.Completed(path, request));
        Assert.False(Directory.Exists(lifetime.Stage));
    }

    // The owner validates a continued request before it acknowledges the handoff; one outside its store stops it.
    [SkippableFact]
    [Trait("Job", "contract: DR 062 §6 (the owner refuses a continued request outside its store, and cleans the scratch)")]
    public async Task AContinuationOutsideTheOwnersStoreIsRefusedBeforeExec()
    {
        Skip.IfNot(OperatingSystem.IsLinux(), LinuxOnly);
        using var scope = new StatechartTestDirectory();
        using var roots = new ScopedRoots(scope.Path);
        var foreign = Path.Combine(scope.Path, "request.json");
        File.WriteAllText(foreign, "{}");
        using var executionLock = new FileStream(Path.Combine(scope.Path, "execution.lock"), FileMode.CreateNew);
        await using var lifetime = await PosixStatechartLifetime.StartAsync(StatechartTestBinary.Start(), scope.Path, scope.Path, scope.Path);
        await lifetime.RegisterAsync();
        await Assert.ThrowsAsync<InvalidDataException>(() => lifetime.HandoffAsync(
            new ProcessStartInfo(Path.Combine(scope.Path, "missing-runtime")), new StatechartContinuation(foreign, executionLock)));
        Assert.False(Directory.Exists(lifetime.Stage));
        Assert.False(Directory.Exists(lifetime.Temp));
    }

    private static async Task<bool> ChildHolds(int descriptor)
    {
        using var child = Process.Start(new ProcessStartInfo("/bin/sh", ["-c", $"test -e /proc/self/fd/{descriptor}"]))!;
        await child.WaitForExitAsync();
        return child.ExitCode == 0;
    }

    // Owners a lifetime starts inherit this process's environment; these roots keep their request store in the test.
    private sealed class ScopedRoots : IDisposable
    {
        private static readonly string[] Names = ["DYDO_STATECHART_ASSET_ROOT", "DYDO_STATECHART_DEPENDENCY_ROOT", "DYDO_STATECHART_STATE_ROOT"];
        private readonly string?[] _saved = Names.Select(Environment.GetEnvironmentVariable).ToArray();

        public ScopedRoots(string root)
        {
            foreach (var (name, folder) in Names.Zip(["assets", "dependencies", "state"]))
                Environment.SetEnvironmentVariable(name, Path.Combine(root, folder));
        }

        public void Dispose()
        {
            for (var index = 0; index < Names.Length; index++) Environment.SetEnvironmentVariable(Names[index], _saved[index]);
        }
    }

    [SkippableFact]
    [Trait("Job", "contract: DR 062 §6 (a handed-off request has no verdict until the owner reports one after the drain, and the owner then cleans the scratch)")]
    public async Task AHandedOffRequestRecordsItsVerdictOnlyAfterTheGroupDrains()
    {
        Skip.IfNot(OperatingSystem.IsLinux(), LinuxOnly);
        using var scope = new StatechartTestDirectory();
        var (store, request, path) = SavedRequest(scope.Path);
        await using var session = await OwnerSession.StartAsync(scope.Path);
        var nonce = session.Nonce;
        await session.RegisterAsync();
        await session.SendAsync($"handoff:{nonce}:{StatechartCleanup.Encode(path)}");
        await session.SendAsync("drain:" + nonce);
        Directory.CreateDirectory(request.Output);
        File.WriteAllText(Path.Combine(request.Output, "report.md"), "Bound breaks");
        Assert.Null(store.Completed(path, request));
        await session.SendAsync($"result:{nonce}:1");
        Assert.Equal(1, store.Completed(path, request));
        await session.SendAsync("complete:" + nonce);
        Assert.Equal(0, (await session.ExitAsync()).Exit);
        Assert.False(Directory.Exists(session.Stage));
        Assert.False(Directory.Exists(session.Temp));
    }

    [SkippableTheory]
    [Trait("Job", "contract: DR 062 §5 §6 (an unproven completion frame records no verdict, the owner exits non-success, and the scratch is cleaned)")]
    [InlineData("out-of-range")]
    [InlineData("changed")]
    [InlineData("anonymous")]
    [InlineData("before-drain")]
    public async Task AnUnprovenCompletedRequestFrameRecordsNoVerdict(string kind)
    {
        Skip.IfNot(OperatingSystem.IsLinux(), LinuxOnly);
        using var scope = new StatechartTestDirectory();
        var (store, request, path) = SavedRequest(scope.Path);
        await using var session = await OwnerSession.StartAsync(scope.Path);
        var nonce = session.Nonce;
        await session.RegisterAsync();
        var handed = kind == "anonymous" ? "-" : StatechartCleanup.Encode(path);
        await session.SendAsync($"handoff:{nonce}:{handed}");
        if (kind != "before-drain") await session.SendAsync("drain:" + nonce);
        Directory.CreateDirectory(request.Output);
        File.WriteAllText(Path.Combine(request.Output, "report.md"), "Bound breaks");
        if (kind == "changed") File.AppendAllText(path, " ");
        await session.SendAsync($"result:{nonce}:{(kind == "out-of-range" ? 7 : 1)}");
        // Read before waiting: an owner that wrongly accepted the frame keeps waiting for the next one.
        Assert.Null(store.Completed(path, request));
        var (exit, _) = await session.ExitAsync();
        Assert.Equal(2, exit);
        Assert.False(Directory.Exists(session.Stage));
    }

    [SkippableFact]
    [Trait("Job", "contract: DR 062 §6 (the drain stops the checking child the owner attached)")]
    public async Task OnlyACheckingChildInsideTheOwnedGroupIsAttachedAndTheDrainStopsIt()
    {
        Skip.IfNot(OperatingSystem.IsLinux(), LinuxOnly);
        using var scope = new StatechartTestDirectory();
        await using var session = await OwnerSession.StartAsync(scope.Path);
        var nonce = session.Nonce;
        await session.RegisterAsync();
        await session.SendAsync($"handoff:{nonce}:-");
        using var child = Process.Start(Binary(scope.Path, "__statechart-posix-child", session.Group.ToString(),
            "/bin/sh", "-c", "echo joined; exec cat"))!;
        await child.StandardOutput.ReadLineAsync();
        await session.SendAsync($"attach:{nonce}:{child.Id}");
        await session.SendAsync("drain:" + nonce);
        // A child the drain did not stop ends at EOF with 0 instead of being killed.
        child.StandardInput.Close();
        await child.WaitForExitAsync();
        Assert.Equal(128 + 9, child.ExitCode);
        await session.SendAsync("complete:" + nonce);
        Assert.Equal(0, (await session.ExitAsync()).Exit);
    }

    [SkippableTheory]
    [Trait("Job", "contract: DR 062 §6 (a malformed or foreign control frame stops the owner with exit 2, and the scratch is cleaned)")]
    [InlineData("handoff:{0}:a:b")]
    [InlineData("attach:{0}:checker")]
    [InlineData("attach:{0}:1")]
    [InlineData("go:{0}")]
    public async Task AMalformedOrForeignControlFrameStopsTheOwnerBeforeAnyRelease(string frame)
    {
        Skip.IfNot(OperatingSystem.IsLinux(), LinuxOnly);
        using var scope = new StatechartTestDirectory();
        await using var session = await OwnerSession.StartAsync(scope.Path);
        await session.RegisterAsync();
        if (frame.StartsWith("attach", StringComparison.Ordinal))
            await session.SendAsync($"handoff:{session.Nonce}:-");
        await session.SendAsync(string.Format(frame, session.Nonce));
        Assert.Equal(2, (await session.ExitAsync()).Exit);
        Assert.False(Directory.Exists(session.Stage));
    }

    [SkippableFact]
    [Trait("Job", "contract: DR 062 §5 §6 (losing the npm lifetime ends the owner non-success, and the scratch is still cleaned)")]
    public async Task LosingTheNpmLifetimeStopsControlAndStillCleansAfterTheDrain()
    {
        Skip.IfNot(OperatingSystem.IsLinux(), LinuxOnly);
        using var scope = new StatechartTestDirectory();
        await using var session = await OwnerSession.StartAsync(scope.Path, npmLifetime: true);
        await session.RegisterAsync();
        session.Owner.StandardInput.Close();
        Assert.Equal(2, (await session.ExitAsync()).Exit);
        Assert.False(Directory.Exists(session.Stage));
        Assert.False(Directory.Exists(session.Temp));
    }

    [SkippableTheory]
    [Trait("Job", "contract: DR 062 §6 (a private mode without a valid channel exits 2, prints nothing and creates nothing)")]
    [InlineData("__statechart-posix-owner")]
    [InlineData("__statechart-posix-anchor")]
    [InlineData("__statechart-posix-child")]
    public async Task APrivateModeWithoutAValidChannelStartsNothing(string mode)
    {
        Skip.IfNot(OperatingSystem.IsLinux(), LinuxOnly);
        using var scope = new StatechartTestDirectory();
        using var process = Process.Start(Binary(scope.Path, mode, "statechart-missing", "not-a-nonce"))!;
        var output = process.StandardOutput.ReadToEndAsync();
        var errors = process.StandardError.ReadToEndAsync();
        await process.WaitForExitAsync();
        Assert.Equal(2, process.ExitCode);
        await errors;
        Assert.Equal("", await output);
        Assert.Empty(Directory.EnumerateFileSystemEntries(scope.Path));
    }

    private static ProcessStartInfo Binary(string root, params string[] arguments)
    {
        var start = StatechartTestBinary.Start();
        foreach (var argument in arguments) start.ArgumentList.Add(argument);
        start.UseShellExecute = false;
        start.CreateNoWindow = true;
        start.RedirectStandardInput = true;
        start.RedirectStandardOutput = true;
        start.RedirectStandardError = true;
        foreach (var name in start.Environment.Keys.Where(name => name.StartsWith("DYDO_", StringComparison.Ordinal)).ToArray())
            start.Environment.Remove(name);
        start.Environment["DYDO_STATECHART_ASSET_ROOT"] = Path.Combine(root, "assets");
        start.Environment["DYDO_STATECHART_DEPENDENCY_ROOT"] = Path.Combine(root, "dependencies");
        start.Environment["DYDO_STATECHART_STATE_ROOT"] = Path.Combine(root, "state");
        return start;
    }

    // A request the owner's own store, rooted by the same environment, accepts at handoff.
    private static (StatechartRequests Store, StatechartRequest Request, string Path) SavedRequest(string root)
    {
        var assets = new StatechartAssets();
        var rid = RuntimeInformation.RuntimeIdentifier;
        var caller = Directory.CreateDirectory(Path.Combine(root, "caller")).FullName;
        File.WriteAllText(Path.Combine(caller, "model.tla"), "---- MODULE model ----\n====\n");
        File.WriteAllText(Path.Combine(caller, "model.cfg"), "\n");
        var home = new StatechartHome(Path.Combine(root, "assets"), Path.Combine(root, "dependencies"), Path.Combine(root, "state"));
        var store = new StatechartRequests(home, assets, rid);
        var request = StatechartRequest.Capture(["model.tla", "--out", "chart"], caller, assets, rid);
        return (store, request, store.Save(request));
    }

    // Plays the public process and, after the handoff, the checking runtime against a real independent owner.
    private sealed class OwnerSession : IAsyncDisposable
    {
        private readonly PosixStatechartChannel _channel;
        private readonly Task<string> _errors;
        public Process Owner { get; }
        public string Nonce { get; }
        public int Group { get; }
        public string Stage { get; private set; } = "";
        public string Temp { get; private set; } = "";

        private OwnerSession(PosixStatechartChannel channel, Process owner, string nonce, int group)
        {
            _channel = channel;
            Owner = owner;
            Nonce = nonce;
            Group = group;
            _errors = owner.StandardError.ReadToEndAsync();
        }

        public static async Task<OwnerSession> StartAsync(string root, bool npmLifetime = false)
        {
            var name = "statechart-" + Guid.NewGuid().ToString("N");
            var nonce = Guid.NewGuid().ToString("N");
            var channel = PosixStatechartChannel.Listen(name);
            var start = Binary(root, "__statechart-posix-owner", name, nonce, root, root, root);
            if (npmLifetime) start.Environment["DYDO_STATECHART_NPM_LIFETIME"] = "1";
            var owner = Process.Start(start)!;
            await channel.AcceptAsync();
            var ready = (await channel.ReadAsync())!.Split(':');
            Assert.Equal(["ready", "1", nonce], ready[..3]);
            return new OwnerSession(channel, owner, nonce, int.Parse(ready[3]));
        }

        public async Task<string?> SendAsync(string frame)
        {
            await _channel.SendAsync(frame);
            return await _channel.ReadAsync();
        }

        public async Task RegisterAsync()
        {
            var created = (await SendAsync("reserve:" + Nonce))!.Split(':');
            Stage = StatechartCleanup.Decode(created[1]);
            Temp = StatechartCleanup.Decode(created[2]);
            await SendAsync("register:" + Nonce);
        }

        public async Task<(int Exit, string Errors)> ExitAsync()
        {
            await Owner.WaitForExitAsync();
            return (Owner.ExitCode, await _errors);
        }

        // Control EOF settles an owner a failed assertion left running.
        public async ValueTask DisposeAsync()
        {
            _channel.Dispose();
            await Owner.WaitForExitAsync();
            await _errors;
            Owner.Dispose();
        }
    }
}
