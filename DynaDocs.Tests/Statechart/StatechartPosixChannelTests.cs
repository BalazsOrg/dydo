namespace DynaDocs.Tests.Statechart;

using System.Diagnostics;
using System.Runtime.Versioning;
using System.Text;
using DynaDocs.Services.Statechart;

[SupportedOSPlatform("linux")]
public class StatechartPosixChannelTests
{
    private const int SunPath = 108; // Linux sockaddr_un.sun_path, NUL included.

    // The connector is a separate dydo process; it finds the socket only through the TMPDIR it inherits.
    [SkippableFact]
    [Trait("Job", "trap: a TMPDIR too long for sun_path still lets a child process that inherits it connect to the socket")]
    public async Task AChildInheritingALongTempDirConnectsAndLeavesNoSocketBehind()
    {
        Skip.IfNot(OperatingSystem.IsLinux(), "The POSIX statechart channel runs on Linux.");
        using var scope = new StatechartTestDirectory();
        var temp = Directory.CreateDirectory(Path.Combine(scope.Path, new string('t', 80))).FullName;
        var name = "statechart-" + Guid.NewGuid().ToString("N");
        Assert.True(Bytes(Path.Combine(temp, "CoreFxPipe_" + name)) > SunPath);
        var nonce = Guid.NewGuid().ToString("N");
        var start = StatechartTestBinary.Start();
        foreach (var argument in new[] { "__statechart-posix-anchor", name, nonce }) start.ArgumentList.Add(argument);
        start.Environment["TMPDIR"] = temp;

        using var server = PosixStatechartChannel.Listen(name, temp);
        var socket = PosixStatechartSocket.For(name, temp);
        Assert.True(File.Exists(socket));
        using var child = Process.Start(start)!;
        await server.AcceptAsync();
        Assert.False(File.Exists(socket));
        await server.ExpectAsync("anchor:1:" + nonce + ":" + child.Id);
        await server.SendAsync("stop");
        await child.WaitForExitAsync();
    }

    [SkippableFact]
    [Trait("Job", "trap: sun_path is counted in UTF-8 bytes, so a TMPDIR that fits in characters but not in bytes gets a socket path that fits")]
    public void ASocketPathFitsSunPathCountedInUtf8Bytes()
    {
        Skip.IfNot(OperatingSystem.IsLinux(), "The POSIX statechart channel runs on Linux.");
        var name = "statechart-" + Guid.NewGuid().ToString("N");
        // The old path fits in characters, not in bytes.
        var temp = "/" + new string('é', 40);
        Assert.True(Path.Combine(temp, "CoreFxPipe_" + name).Length + 1 <= SunPath);
        Assert.True(Bytes(Path.Combine(temp, "CoreFxPipe_" + name)) > SunPath);
        Assert.True(Bytes(PosixStatechartSocket.For(name, temp)) <= SunPath);
    }

    // The old path, today's for every short TMPDIR, stays while it fits with its NUL.
    [SkippableFact]
    [Trait("Job", "edge: a CoreFxPipe socket path that fills sun_path exactly, NUL included, stays the socket path")]
    public void TheCoreFxPipePathStaysWhileItFits() => CoreFxPipePath(108, kept: true);

    [SkippableFact]
    [Trait("Job", "edge: a CoreFxPipe socket path one byte past sun_path, NUL included, is replaced")]
    public void TheCoreFxPipePathIsReplacedOneBytePastSunPath() => CoreFxPipePath(109, kept: false);

    private static void CoreFxPipePath(int bytes, bool kept)
    {
        Skip.IfNot(OperatingSystem.IsLinux(), "The POSIX statechart channel runs on Linux.");
        var name = "statechart-" + Guid.NewGuid().ToString("N");
        var temp = "/" + new string('t', bytes - 57);
        var old = Path.Combine(temp, "CoreFxPipe_" + name);
        Assert.Equal(bytes, Bytes(old));
        Assert.Equal(kept, PosixStatechartSocket.For(name, temp) == old);
    }

    [SkippableFact]
    [Trait("Job", "trap: a TMPDIR too long for sun_path sends the socket to a directory under /tmp named for the effective user")]
    public void ALongTempDirFallsBackToOnePrivateDirectoryPerUser()
    {
        Skip.IfNot(OperatingSystem.IsLinux(), "The POSIX statechart channel runs on Linux.");
        var name = "statechart-" + Guid.NewGuid().ToString("N");
        var temp = "/" + new string('t', 60);
        var socket = PosixStatechartSocket.For(name, temp);
        Assert.Equal(Path.Combine("/tmp/dydo-statechart-" + EffectiveUser(), name), socket);
        Assert.Equal(socket, PosixStatechartSocket.For(name, temp + "/"));
        Assert.Equal(UnixFileMode.UserRead | UnixFileMode.UserWrite | UnixFileMode.UserExecute,
            File.GetUnixFileMode(Path.GetDirectoryName(socket)!));
    }

    [SkippableFact]
    [Trait("Job", "edge: a socket directory that does not exist yet is accepted for use")]
    public void AMissingSocketDirectoryIsCreatedPrivate()
    {
        Skip.IfNot(OperatingSystem.IsLinux(), "The POSIX statechart channel runs on Linux.");
        using var scope = new StatechartTestDirectory();
        var directory = Path.Combine(scope.Path, "sockets");
        Assert.Equal(directory, PosixStatechartSocket.Private(directory, EffectiveUser()));
        Assert.Equal(UnixFileMode.UserRead | UnixFileMode.UserWrite | UnixFileMode.UserExecute,
            File.GetUnixFileMode(directory));
    }

    [SkippableFact]
    [Trait("Job", "edge: an existing socket directory with mode 0500 is used, even without the owner's write bit")]
    public void AnExistingSocketDirectoryOnlyItsOwnerCanEnterIsUsed() => ExistingSocketDirectory("500", used: true);

    [SkippableTheory]
    [Trait("Job", "trap: an existing socket directory with a group, other or special mode bit set is refused")]
    [InlineData("750")]
    [InlineData("702")]
    [InlineData("1700")]
    public void AnExistingSocketDirectoryIsUsedOnlyWhenNoOneElseCanEnterIt(string mode) => ExistingSocketDirectory(mode, used: false);

    private static void ExistingSocketDirectory(string mode, bool used)
    {
        Skip.IfNot(OperatingSystem.IsLinux(), "The POSIX statechart channel runs on Linux.");
        using var scope = new StatechartTestDirectory();
        var directory = Directory.CreateDirectory(Path.Combine(scope.Path, "sockets")).FullName;
        File.SetUnixFileMode(directory, (UnixFileMode)Convert.ToInt32(mode, 8));
        if (used) Assert.Equal(directory, PosixStatechartSocket.Private(directory, EffectiveUser()));
        else Refused(directory, EffectiveUser());
    }

    [SkippableFact]
    [Trait("Job", "trap: a socket directory another user owns is refused")]
    public void ASocketDirectoryOwnedBySomeoneElseIsRefused()
    {
        Skip.IfNot(OperatingSystem.IsLinux(), "The POSIX statechart channel runs on Linux.");
        using var scope = new StatechartTestDirectory();
        var directory = Directory.CreateDirectory(Path.Combine(scope.Path, "sockets")).FullName;
        File.SetUnixFileMode(directory, UnixFileMode.UserRead | UnixFileMode.UserWrite | UnixFileMode.UserExecute);
        Refused(directory, EffectiveUser() + 1);
    }

    [SkippableFact]
    [Trait("Job", "trap: a link and a file in place of the socket directory are each refused")]
    public void ALinkOrFileInPlaceOfTheSocketDirectoryIsRefused()
    {
        Skip.IfNot(OperatingSystem.IsLinux(), "The POSIX statechart channel runs on Linux.");
        using var scope = new StatechartTestDirectory();
        var link = Path.Combine(scope.Path, "link");
        File.CreateSymbolicLink(link, scope.Path);
        Refused(link, EffectiveUser());
        var file = Path.Combine(scope.Path, "file");
        File.WriteAllText(file, "");
        File.SetUnixFileMode(file, UnixFileMode.UserRead | UnixFileMode.UserWrite);
        Refused(file, EffectiveUser());
    }

    private static void Refused(string directory, uint owner)
    {
        var error = Assert.Throws<IOException>(() => PosixStatechartSocket.Private(directory, owner));
        Assert.Contains(directory, error.Message);
        Assert.Contains("TMPDIR", error.Message);
    }

    private static uint EffectiveUser() => uint.Parse(File.ReadLines("/proc/self/status")
        .First(line => line.StartsWith("Uid:")).Split('\t', StringSplitOptions.RemoveEmptyEntries)[2]);

    private static int Bytes(string path) => Encoding.UTF8.GetByteCount(path) + 1;
}
