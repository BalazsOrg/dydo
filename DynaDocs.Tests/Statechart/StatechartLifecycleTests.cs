namespace DynaDocs.Tests.Statechart;

using System.ComponentModel;
using System.Diagnostics;
using DynaDocs.Services.Statechart;

/// <summary>
/// The scratch a statechart check runs in, judged by what is left: after a drained success nothing; after a cancel,
/// an owner loss or a forged drain the registered scratch is retained; a foreign replacement is never deleted
/// (DR 062 §6). The cleanup helper is driven through its own process; only its native owner is Windows-only.
/// </summary>
public class StatechartLifecycleTests
{
    private static ProcessStartInfo Binary()
    {
        Skip.IfNot(OperatingSystem.IsWindows(), "Windows Job Object lifetime runs only on Windows.");
        return StatechartTestBinary.Start();
    }

    private static Process Cleanup(string nonce, StatechartTestDirectory scope, string? stageParent = null)
    {
        var start = StatechartTestBinary.Start();
        start.ArgumentList.Add("__statechart-cleanup");
        foreach (var argument in new[] { nonce, stageParent ?? scope.Path, scope.Path, scope.Path }) start.ArgumentList.Add(argument);
        start.UseShellExecute = false;
        start.CreateNoWindow = true;
        start.RedirectStandardInput = true;
        start.RedirectStandardOutput = true;
        start.RedirectStandardError = true;
        return scope.Own(Process.Start(start)!);
    }

    // Drives the helper through reservation and registration, returning the scratch it created.
    private static async Task<(string Stage, string Temp)> Register(Process helper, string nonce)
    {
        await StatechartCleanup.Read(helper.StandardOutput);
        await StatechartCleanup.Say(helper.StandardInput, "reserve:" + nonce);
        var created = (await StatechartCleanup.Read(helper.StandardOutput))!.Split(':');
        await StatechartCleanup.Say(helper.StandardInput, "register:" + nonce);
        await StatechartCleanup.Read(helper.StandardOutput);
        return (StatechartCleanup.Decode(created[1]), StatechartCleanup.Decode(created[2]));
    }

    [SkippableFact]
    [Trait("Job", "contract: DR 062 §6 (on Windows, once the native owner seals and drains, the registered scratch is deleted)")]
    public async Task IndependentCleanupDeletesRegisteredScratchOnlyAfterNativeOwnerSealsAndDrains()
    {
        using var scope = new StatechartTestDirectory();
        await using var lifetime = await WindowsStatechartLifetime.StartAsync(Binary(), scope.Path, scope.Path, scope.Path);
        await lifetime.RegisterAsync();
        File.WriteAllText(Path.Combine(lifetime.Stage, "checking-file"), "owned");
        Assert.True(await lifetime.CompleteAsync());
        Assert.False(Directory.Exists(lifetime.Stage));
        Assert.False(Directory.Exists(lifetime.Temp));
    }

    [SkippableFact]
    [Trait("Job", "contract: DR 062 §6 (only authoritative drain authorizes cleanup)")]
    public async Task OwnerEofRetainsRegisteredScratchWithDurableUncertaintyReason()
    {
        using var scope = new StatechartTestDirectory();
        var lifetime = await WindowsStatechartLifetime.StartAsync(Binary(), scope.Path, scope.Path, scope.Path);
        string stage;
        // The owner's EOF is the subject, so it is disposed here, and also when registration throws, before the scope.
        try
        {
            await lifetime.RegisterAsync();
            stage = lifetime.Stage;
        }
        finally { await lifetime.DisposeAsync(); }
        Assert.True(Directory.Exists(stage));
        Assert.NotEmpty(Directory.GetFiles(scope.Path, "scratch-*.journal"));
    }

    // A helper that answers anything but the cleanup protocol is released, and nothing is reserved.
    [SkippableFact]
    [Trait("Job", "contract: DR 062 §6 (on Windows, a cleanup helper that does not speak the protocol, or cannot start, is rejected and reserves nothing)")]
    public async Task AHelperThatDoesNotSpeakTheCleanupProtocolIsReleasedAndRejected()
    {
        Skip.IfNot(OperatingSystem.IsWindows(), "Windows Job Object lifetime runs only on Windows.");
        using var scope = new StatechartTestDirectory();
        var impostor = new ProcessStartInfo("cmd.exe");
        foreach (var argument in new[] { "/d", "/q", "/c", "echo", "impostor" }) impostor.ArgumentList.Add(argument);

        await Assert.ThrowsAsync<InvalidDataException>(() =>
            WindowsStatechartLifetime.StartAsync(impostor, scope.Path, scope.Path, scope.Path));
        await Assert.ThrowsAsync<Win32Exception>(() => WindowsStatechartLifetime.StartAsync(
            new ProcessStartInfo(Path.Combine(scope.Path, "missing-helper.exe")), scope.Path, scope.Path, scope.Path));
        Assert.Empty(Directory.EnumerateFileSystemEntries(scope.Path));
    }

    // An attached check is owned until it exits and its scratch is then cleaned; a check on a lost helper stops with
    // its scratch retained.
    [SkippableFact]
    [Trait("Job", "contract: DR 062 §6 (on Windows, a lifetime owns an attached check until it exits and then cleans its scratch, and a wait on a check it does not own fails)")]
    public async Task ALifetimeOwnsAttachedChecksInOrderAndReportsALostHelper()
    {
        using var scope = new StatechartTestDirectory();
        await using var lifetime = await StatechartLifetime.CreateAsync(Binary(), scope.Path, scope.Path, scope.Path);
        await lifetime.RegisterAsync();
        using (var checking = Process.Start(Waiting())!)
        {
            lifetime.Attach(checking.Id);
            checking.StandardInput.Close();
            await lifetime.WaitAsync(checking, CancellationToken.None);
            Assert.True(checking.HasExited);
        }
        Assert.True(await lifetime.CompleteAsync());
        Assert.False(Directory.Exists(lifetime.Stage));

        using var orphan = Process.Start(Waiting())!;
        await Assert.ThrowsAsync<IOException>(() => lifetime.WaitAsync(orphan, CancellationToken.None));
        orphan.Kill();
        await orphan.WaitForExitAsync();
    }

    // A command interpreter that lives until its input closes.
    private static ProcessStartInfo Waiting() => new("cmd.exe")
    {
        UseShellExecute = false, CreateNoWindow = true, RedirectStandardInput = true, RedirectStandardOutput = true
    };

    [Fact]
    [Trait("Job", "contract: DR 062 §6 (a cleanup helper whose owner is lost before any reservation exits 2 and creates nothing)")]
    public Task OwnerLossBeforeAnyReservation_ExitsWithoutCreatingScratch() => OwnerLoss(reserve: false);

    [Fact]
    [Trait("Job", "contract: DR 062 §6 (only authoritative drain authorizes cleanup)")]
    public Task OwnerLossBeforeOrDuringRegistration_NeverDeletesAmbiguousDirectories() => OwnerLoss(reserve: true);

    private static async Task OwnerLoss(bool reserve)
    {
        using var scope = new StatechartTestDirectory();
        var nonce = Guid.NewGuid().ToString("N");
        var helper = Cleanup(nonce, scope);
        await StatechartCleanup.Read(helper.StandardOutput);
        if (reserve)
        {
            await StatechartCleanup.Say(helper.StandardInput, "reserve:" + nonce);
            await StatechartCleanup.Read(helper.StandardOutput);
        }
        helper.StandardInput.Close();
        await helper.WaitForExitAsync();
        Assert.Equal(2, helper.ExitCode);
        Assert.Equal(reserve ? 2 : 0, StatechartLeftovers.Left(scope.Path).Length);
    }

    [Fact]
    [Trait("Job", "contract: DR 062 §6 (after an authoritative drain the cleanup helper deletes its registered scratch and exits 0)")]
    public async Task ADrainedSuccess_LeavesNoScratch()
    {
        using var scope = new StatechartTestDirectory();
        var nonce = Guid.NewGuid().ToString("N");
        var helper = Cleanup(nonce, scope);
        var (stage, temp) = await Register(helper, nonce);
        File.WriteAllText(Path.Combine(stage, "owned-file"), "checking");
        Directory.CreateDirectory(Path.Combine(temp, "owned-directory"));
        await StatechartCleanup.Say(helper.StandardInput, "drained:" + nonce);
        await StatechartCleanup.Read(helper.StandardOutput);
        await helper.WaitForExitAsync();
        Assert.Equal(0, helper.ExitCode);
        Assert.Empty(StatechartLeftovers.Left(scope.Path));
    }

    [Theory]
    [Trait("Job", "contract: DR 062 §6 (only authoritative drain authorizes cleanup)")]
    [InlineData(true)]
    [InlineData(false)]
    public async Task AForgedDrainOrALostOwner_RetainsTheRegisteredScratch(bool forged)
    {
        using var scope = new StatechartTestDirectory();
        var nonce = Guid.NewGuid().ToString("N");
        var helper = Cleanup(nonce, scope);
        var (stage, temp) = await Register(helper, nonce);
        if (forged) await StatechartCleanup.Say(helper.StandardInput, "drained:" + Guid.NewGuid().ToString("N"));
        helper.StandardInput.Close();
        await helper.WaitForExitAsync();
        Assert.Equal(2, helper.ExitCode);
        Assert.True(Directory.Exists(stage));
        Assert.True(Directory.Exists(temp));
    }

    [Fact]
    [Trait("Job", "contract: DR 062 §6 (only authoritative drain authorizes cleanup)")]
    public async Task AReplacedScratchPathIsRetainedEvenAfterTheDrain()
    {
        using var scope = new StatechartTestDirectory();
        var nonce = Guid.NewGuid().ToString("N");
        if (OperatingSystem.IsWindows())
        {
            await ARegisteredScratchPathCannotBeReplacedOnWindows(scope, nonce);
            return;
        }
        var helper = Cleanup(nonce, scope);
        var (stage, _) = await Register(helper, nonce);
        Directory.Move(stage, stage + "-registered");
        Directory.CreateDirectory(stage);
        File.WriteAllText(Path.Combine(stage, "foreign"), "keep");
        await StatechartCleanup.Say(helper.StandardInput, "drained:" + nonce);
        await StatechartCleanup.Read(helper.StandardOutput);
        await helper.WaitForExitAsync();
        Assert.Equal(2, helper.ExitCode);
        Assert.Equal("keep", File.ReadAllText(Path.Combine(stage, "foreign")));
        Assert.True(Directory.Exists(stage + "-registered"));
    }

    // Windows refuses the replacement itself: the registered handle denies delete sharing, and a directory above an
    // open handle cannot be renamed. So nothing foreign can take the path, and only the drain deletes the scratch.
    private static async Task ARegisteredScratchPathCannotBeReplacedOnWindows(StatechartTestDirectory scope, string nonce)
    {
        var parent = Directory.CreateDirectory(Path.Combine(scope.Path, "parent")).FullName;
        var helper = Cleanup(nonce, scope, parent);
        var (stage, temp) = await Register(helper, nonce);
        Assert.Throws<IOException>(() => Directory.Move(stage, stage + "-registered"));
        Assert.Throws<IOException>(() => Directory.Delete(stage));
        Assert.Throws<IOException>(() => Directory.Move(parent, parent + "-moved"));
        await StatechartCleanup.Say(helper.StandardInput, "drained:" + nonce);
        await StatechartCleanup.Read(helper.StandardOutput);
        await helper.WaitForExitAsync();
        Assert.Equal(0, helper.ExitCode);
        Assert.False(Directory.Exists(stage));
        Assert.False(Directory.Exists(temp));
    }

    [Fact]
    [Trait("Job", "contract: DR 062 §6 (a cleanup helper started without a valid channel nonce exits 2 and creates nothing)")]
    public async Task AHelperWithoutAValidChannelNonceReservesNothing()
    {
        using var scope = new StatechartTestDirectory();
        var helper = Cleanup("not-a-nonce", scope);
        var errors = helper.StandardError.ReadToEndAsync();
        await StatechartCleanup.Read(helper.StandardOutput);
        await helper.WaitForExitAsync();
        await errors;
        Assert.Equal(2, helper.ExitCode);
        Assert.Empty(Directory.EnumerateFileSystemEntries(scope.Path));
    }
}
