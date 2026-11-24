namespace DynaDocs.Tests.Statechart;

using DynaDocs.Services.Statechart;

public class StatechartLockTests
{
    [Fact]
    [Trait("Job", "edge: a lock waiter keeps waiting while another handle holds the lock file, then takes it exclusively once it is released")]
    public async Task AWaiterTakesTheLockOnceItsHolderReleasesIt()
    {
        using var scope = new StatechartTestDirectory();
        var path = Path.Combine(scope.Path, "x.lock");
        var held = Hold(path);
        var waiting = StatechartLock.AcquireAsync(path, TimeSpan.FromMinutes(1), CancellationToken.None);
        Assert.False(waiting.IsCompleted);
        held.Dispose();
        await using var acquired = await waiting;
        Assert.Throws<IOException>(() => Hold(path));
    }

    [Fact]
    [Trait("Job", "edge: a lock file still held when the wait bound runs out fails with an error naming its path")]
    public async Task AHolderPastTheBoundIsReportedByItsPath()
    {
        using var scope = new StatechartTestDirectory();
        var path = Path.Combine(scope.Path, "x.lock");
        using var held = Hold(path);
        var error = await Assert.ThrowsAsync<IOException>(() => StatechartLock.AcquireAsync(path, TimeSpan.Zero, CancellationToken.None));
        Assert.Contains(path, error.Message);
    }

    [Fact]
    [Trait("Job", "edge: cancelling a wait on a held lock file ends it with a cancellation")]
    public async Task CancellationEndsTheWait()
    {
        using var scope = new StatechartTestDirectory();
        var path = Path.Combine(scope.Path, "x.lock");
        using var held = Hold(path);
        using var cancel = new CancellationTokenSource();
        var waiting = StatechartLock.AcquireAsync(path, TimeSpan.FromMinutes(1), cancel.Token);
        await cancel.CancelAsync();
        await Assert.ThrowsAnyAsync<OperationCanceledException>(() => waiting);
    }

    [Fact]
    [Trait("Job", "edge: a lock file whose directory is missing fails with DirectoryNotFoundException, not as a held lock")]
    public async Task AMissingDirectoryIsAnErrorNotAWait()
    {
        using var scope = new StatechartTestDirectory();
        await Assert.ThrowsAsync<DirectoryNotFoundException>(() =>
            StatechartLock.AcquireAsync(Path.Combine(scope.Path, "missing", "x.lock"), TimeSpan.FromMinutes(1), CancellationToken.None));
    }

    private static FileStream Hold(string path) => new(path, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None);
}
