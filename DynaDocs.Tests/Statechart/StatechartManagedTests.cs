namespace DynaDocs.Tests.Statechart;

using DynaDocs.Services.Statechart;
using System.Security.Cryptography;
using System.Text.Json.Nodes;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

/// <summary>
/// The managed dependency store, judged by what it reads as ready: an approved install is ready, content that does not
/// verify is not, and nothing the store did not prove is adopted, repaired or deleted (DR 062 §4 §5).
/// </summary>
[Trait("Job", "contract: DR 062 §4 §5")]
public class StatechartManagedTests
{
    private static readonly string[] All = ["node", "java", "tlc", "pele"];

    // Stand-in archives arrive from loopback under a test-built lock and face the real fetch, integrity check, extraction,
    // readiness and placement; a later release finds them stale and re-stamps them without reinstalling.
    [SkippableFact]
    [UnsupportedOSPlatform("windows")]
    public async Task AnApprovedInstall_IsReady_AndStaysReadyForALaterRelease()
    {
        Skip.IfNot(OperatingSystem.IsLinux(), "POSIX ownership runs on Linux.");
        StatechartExecutable.RelaunchThrough(StatechartTestBinary.Start().FileName);
        using var scope = new StatechartTestDirectory();
        var served = StatechartStandIns.Components(Path.Combine(scope.Path, "java.log"), StatechartStandIns.CleanRun);
        await StatechartStandIns.Serve(served, async root =>
        {
            var assets = StatechartStandIns.Assets(StatechartStandIns.Lock(root, served, StatechartStandIns.Sha256(served["/tlc/tla2tools.jar"])));
            var home = Home(scope.Path);
            var managed = new StatechartManaged(home, assets, RuntimeInformation.RuntimeIdentifier);
            var payload = await assets.ExtractAsync(home.Assets, CancellationToken.None);
            var installed = await managed.InstallAsync(new(null, null, null, null), payload, CancellationToken.None);
            Assert.Empty(StatechartManaged.Missing(installed));
            Assert.Equal(installed, managed.Read());
            Assert.Equal(served["/tlc/tla2tools.jar"], File.ReadAllBytes(installed.Tlc!));

            foreach (var receipt in Directory.GetFiles(managed.Root, ".ready.json", SearchOption.AllDirectories))
            {
                var identity = JsonNode.Parse(File.ReadAllText(receipt))!.AsObject();
                identity["release"] = "0.0.1";
                File.WriteAllText(receipt, identity.ToJsonString());
            }
            var later = new StatechartManaged(home, assets, RuntimeInformation.RuntimeIdentifier);
            await later.RestampAsync(later.Read(), payload, CancellationToken.None);
            Assert.Equal(installed, later.Read());
        });
    }

    [Fact]
    public async Task AServedArchiveThatDiffersFromItsLock_IsRejected_AndNothingBecomesReady()
    {
        using var scope = new StatechartTestDirectory();
        var served = StatechartStandIns.Components(Path.Combine(scope.Path, "java.log"), StatechartStandIns.CleanRun);
        await StatechartStandIns.Serve(served, async root =>
        {
            var assets = StatechartStandIns.Assets(StatechartStandIns.Lock(root, served, StatechartStandIns.Sha256(served["/tlc/tla2tools.jar"])));
            served["/node/node.tar.gz"] = StatechartStandIns.TarGz(("node-v26.13.0-linux/bin/node", "#!/bin/sh\necho substituted\n"));
            var managed = new StatechartManaged(Home(scope.Path), assets, RuntimeInformation.RuntimeIdentifier);
            await Assert.ThrowsAsync<IOException>(() =>
                managed.InstallAsync(new(null, null, null, null), Path.Combine(scope.Path, "unused-payload"), CancellationToken.None));
            Assert.Equal(All, StatechartManaged.Missing(managed.Read()));
        });
    }

    [Fact]
    public void AMissingStoreIsMissingEverything_AndIsNotCreated()
    {
        using var scope = new StatechartTestDirectory();
        var managed = Create(scope.Path);
        Assert.Equal(All, StatechartManaged.Missing(managed.Read()));
        Assert.False(Directory.Exists(managed.Root));
    }

    [Theory]
    [InlineData("valid")]
    [InlineData("foreign")]
    [InlineData("missing")]
    public void AForeignMarkerOrUnexpectedChild_IsRetainedAndRefused(string marker)
    {
        using var scope = new StatechartTestDirectory();
        var managed = Create(scope.Path);
        Directory.CreateDirectory(managed.Root);
        if (marker != "missing")
            File.WriteAllText(Path.Combine(managed.Root, ".statechart-store"), marker == "valid" ? new StatechartAssets().LockDigest : "foreign");
        var sentinel = Path.Combine(managed.Root, "foreign");
        File.WriteAllText(sentinel, "keep");
        Assert.Throws<IOException>(() => managed.Read());
        Assert.Equal("keep", File.ReadAllText(sentinel));
    }

    [Theory]
    [InlineData("tampered")]
    [InlineData("receipt missing")]
    [InlineData("receipt unreadable")]
    [InlineData("another release, bytes unverified")]
    public void ContentThatFailsVerification_IsNotReady_AndIsLeftInPlace(string damage)
    {
        using var scope = new StatechartTestDirectory();
        var assets = new StatechartAssets();
        var (managed, jar) = Installed(scope.Path, damage.StartsWith("another", StringComparison.Ordinal) ? "old" : assets.Release,
            assets.Digest, verified: !damage.StartsWith("another", StringComparison.Ordinal));
        var receipt = Path.Combine(Path.GetDirectoryName(jar)!, ".ready.json");
        if (damage == "tampered") File.AppendAllText(jar, "tampered");
        else if (damage == "receipt missing") File.Delete(receipt);
        else if (damage == "receipt unreadable") File.WriteAllText(receipt, "{not json");
        var bytes = File.ReadAllBytes(jar);
        Assert.Null(managed.Read().Tlc);
        Assert.Equal(bytes, File.ReadAllBytes(jar));
    }

    [Fact]
    public void AComponentWithoutAReadinessReceipt_IsNotAdopted_AndIsLeftInPlace()
    {
        using var scope = new StatechartTestDirectory();
        var managed = Create(scope.Path);
        Directory.CreateDirectory(Path.Combine(managed.Root, "node"));
        File.WriteAllText(Path.Combine(managed.Root, ".statechart-store"), new StatechartAssets().LockDigest);
        var file = Path.Combine(managed.Root, "node", "foreign.exe");
        File.WriteAllText(file, "keep");
        Assert.Null(managed.Read().Node);
        Assert.Equal("keep", File.ReadAllText(file));
    }

    [Fact]
    public void VerifiedContentFromAnEarlierReleaseOrPayload_IsReady()
    {
        using var scope = new StatechartTestDirectory();
        var (managed, jar) = Installed(scope.Path, release: "0.0.1", payload: "earlier-payload");
        Assert.Equal(jar, managed.Read().Tlc);
    }

    // The receipt records content hashes, not modes, so a runtime that cannot start still matches it.
    [SkippableTheory]
    [InlineData(false)]
    [InlineData(true)]
    [UnsupportedOSPlatform("windows")]
    public void AManagedRuntimeThatCannotExecute_IsNotReady_AndIsLeftInPlace(bool executable)
    {
        Skip.If(OperatingSystem.IsWindows(), "POSIX modes decide whether a runtime can start.");
        using var scope = new StatechartTestDirectory();
        var (managed, java) = InstalledJava(scope.Path);
        if (!executable) File.SetUnixFileMode(java, UnixFileMode.UserRead | UnixFileMode.UserWrite);
        var mode = File.GetUnixFileMode(java);
        Assert.Equal(executable ? java : null, managed.Read().Java);
        Assert.Equal(mode, File.GetUnixFileMode(java));
    }

    [Fact]
    public async Task CancellingAWaitingInstall_LeavesNothingBehind()
    {
        using var scope = new StatechartTestDirectory();
        var managed = new StatechartManaged(Home(scope.Path), new StatechartAssets(), "win-x64", Unreachable());
        var parent = Path.GetDirectoryName(managed.Root)!;
        Directory.CreateDirectory(parent);
        using var held = new FileStream(Path.Combine(parent, "win-x64.install.lock"),
            FileMode.CreateNew, FileAccess.ReadWrite, FileShare.None);
        using var cancel = new CancellationTokenSource();
        var install = managed.InstallAsync(new(null, null, null, null), Path.Combine(scope.Path, "unavailable-payload"), cancel.Token);
        await cancel.CancelAsync();
        await Assert.ThrowsAnyAsync<OperationCanceledException>(() => install);
        Assert.False(Directory.Exists(managed.Root));
        Assert.Empty(Directory.GetDirectories(parent));
    }

    private static StatechartHome Home(string root) =>
        new(Path.Combine(root, "assets"), Path.Combine(root, "dependencies"), Path.Combine(root, "state"));

    // A verified linux-x64 java component with a synthetic lock row.
    [UnsupportedOSPlatform("windows")]
    private static (StatechartManaged Managed, string Java) InstalledJava(string root)
    {
        var assets = new StatechartAssets();
        byte[] archive = [5, 6, 7];
        var downloads = Unreachable().Select(item => item.Name == "java"
            ? item with { Integrity = Convert.ToHexStringLower(SHA256.HashData(archive)) } : item).ToArray();
        var managed = new StatechartManaged(Home(root), assets, "linux-x64", downloads);
        var component = Path.Combine(managed.Root, "java");
        var java = Path.Combine(component, "content", "jre", "bin", "java");
        Directory.CreateDirectory(Path.GetDirectoryName(java)!);
        File.WriteAllText(java, "#!/bin/sh\n");
        File.SetUnixFileMode(java, UnixFileMode.UserRead | UnixFileMode.UserWrite | UnixFileMode.UserExecute);
        File.WriteAllBytes(Path.Combine(component, "archive.tar.gz"), archive);
        File.WriteAllText(Path.Combine(managed.Root, ".statechart-store"), assets.LockDigest);
        var receipt = new JsonObject { ["schema"] = 1, ["protocol"] = 1, ["release"] = assets.Release, ["rid"] = "linux-x64",
            ["payload"] = assets.Digest, ["lock"] = assets.LockDigest, ["integrity"] = downloads.Single(item => item.Name == "java").Integrity,
            ["files"] = StatechartInventory.Capture(component) };
        File.WriteAllText(Path.Combine(component, ".ready.json"), receipt.ToJsonString());
        return (managed, java);
    }

    // A tlc component whose receipt names the given release and payload. Verified, the lock row is synthetic and matches
    // the bytes; unverified, the bytes are not the locked component's.
    private static (StatechartManaged Managed, string Jar) Installed(string root, string release, string payload, bool verified = true)
    {
        var assets = new StatechartAssets();
        byte[] bytes = [1, 2, 3, 4];
        var downloads = verified
            ? Unreachable().Select(item => item.Name == "tlc"
                ? item with { Integrity = Convert.ToHexStringLower(SHA256.HashData(bytes)) } : item).ToArray()
            : null;
        var managed = downloads is null
            ? new StatechartManaged(Home(root), assets, "win-x64")
            : new StatechartManaged(Home(root), assets, "win-x64", downloads);
        var component = Path.Combine(managed.Root, "tlc");
        Directory.CreateDirectory(component);
        File.WriteAllText(Path.Combine(managed.Root, ".statechart-store"), assets.LockDigest);
        var jar = Path.Combine(component, "tla2tools.jar");
        File.WriteAllBytes(jar, bytes);
        var integrity = (downloads ?? StatechartDownloads.Select(assets, "win-x64")).Single(item => item.Name == "tlc").Integrity;
        var receipt = new JsonObject { ["schema"] = 1, ["protocol"] = 1, ["release"] = release, ["rid"] = "win-x64",
            ["payload"] = payload, ["lock"] = assets.LockDigest, ["integrity"] = integrity,
            ["files"] = StatechartInventory.Capture(component) };
        File.WriteAllText(Path.Combine(component, ".ready.json"), receipt.ToJsonString());
        return (managed, jar);
    }

    private static StatechartDownload[] Unreachable() => All
        .Select(name => new StatechartDownload(name,
            $"http://127.0.0.1:9/{name}/{(name == "tlc" ? "tla2tools.jar" : "archive.tar.gz")}", new string('a', 64), 1024))
        .ToArray();

    private static StatechartManaged Create(string root) => new(Home(root), new StatechartAssets(), "win-x64");
}
