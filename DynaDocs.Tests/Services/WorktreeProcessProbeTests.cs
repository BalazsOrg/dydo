namespace DynaDocs.Tests.Services;

using DynaDocs.Services;

public sealed class WorktreeProcessProbeTests : IDisposable
{
    private const string HeldOrLive = "edge: DYD-362 (in-use worktree detection needs a held folder or a live process, which driving cannot stage)";

    // dydo clean reaches the rename probe only on Windows, and CI runs this class only on Linux.
    private const string WindowsProbe = "edge: DYD-362 (the Windows rename probe, which dydo clean runs only on Windows, where CI does not run this class: ";

    private readonly string _root = Path.Combine(Path.GetTempPath(), "dydo-probe-" + Guid.NewGuid().ToString("N")[..8]);

    public WorktreeProcessProbeTests() => Directory.CreateDirectory(_root);

    public void Dispose() => LinkSafeDelete.DeleteTree(_root);

    [Fact]
    [Trait("Job", WindowsProbe + "an idle folder reads not in use and is moved back with its contents, leaving no probe sibling)")]
    public void RenameProbe_IdleFolder_IsNotInUse_AndStaysWhereItWas()
    {
        var tree = Directory.CreateDirectory(Path.Combine(_root, "tree")).FullName;
        File.WriteAllText(Path.Combine(tree, "file.txt"), "x");

        Assert.Null(WorktreeProcessProbe.RenameProbe(tree));
        Assert.True(File.Exists(Path.Combine(tree, "file.txt")));
        Assert.Single(Directory.GetDirectories(_root));
    }

    [Fact]
    [Trait("Job", WindowsProbe + "a refused rename reads in use; a missing path is the refused rename Linux can stage)")]
    public void RenameProbe_FolderThatCannotBeRenamed_ReportsInUse()
    {
        Assert.Contains("cannot be renamed", WorktreeProcessProbe.RenameProbe(Path.Combine(_root, "missing")));
    }

    [Fact]
    [Trait("Job", HeldOrLive)]
    public void RenameProbe_RetriesAMoveBackThatFailsOnce()
    {
        var tree = Directory.CreateDirectory(Path.Combine(_root, "tree")).FullName;
        var failures = 1;
        void Move(string from, string to)
        {
            if (to == tree && failures-- > 0)
                throw new IOException("held for a moment");
            Directory.Move(from, to);
        }

        Assert.Null(WorktreeProcessProbe.RenameProbe(tree, Move, TimeSpan.Zero));
        Assert.Equal([tree], Directory.GetDirectories(_root));
    }

    [Fact]
    [Trait("Job", HeldOrLive)]
    public void RenameProbe_MoveBackThatKeepsFailing_NamesWhereTheFolderIs()
    {
        var tree = Directory.CreateDirectory(Path.Combine(_root, "tree")).FullName;
        void Move(string from, string to)
        {
            if (to == tree)
                throw new IOException("held");
            Directory.Move(from, to);
        }

        var stranded = Assert.Throws<ProbeStrandedException>(() => WorktreeProcessProbe.RenameProbe(tree, Move, TimeSpan.Zero));

        Assert.True(Directory.Exists(stranded.Probe));
        Assert.StartsWith(tree + ".dydo-clean-probe-", stranded.Probe);
        Assert.Contains(stranded.Probe, stranded.Message);
        Assert.Contains("held", stranded.Message);
    }

    [Fact]
    [Trait("Job", HeldOrLive)]
    public void ProcScan_FindsAProcessWhoseWorkingDirectoryIsInside()
    {
        var tree = Directory.CreateDirectory(Path.Combine(_root, "tree")).FullName;
        var inside = Directory.CreateDirectory(Path.Combine(tree, "src")).FullName;
        var elsewhere = Directory.CreateDirectory(Path.Combine(_root, "elsewhere")).FullName;
        var proc = Path.Combine(_root, "proc");
        DirectoryLinks.Create(Path.Combine(proc, "100", "cwd"), elsewhere);
        DirectoryLinks.Create(Path.Combine(proc, "200", "cwd"), inside);
        Directory.CreateDirectory(Path.Combine(proc, "self"));

        Assert.Equal("a running process (pid 200) has its working directory inside",
            WorktreeProcessProbe.ProcScan(tree, proc));
    }

    [Fact]
    [Trait("Job", HeldOrLive)]
    public void ProcScan_IgnoresUnreadableAndUnrelatedProcesses()
    {
        var tree = Directory.CreateDirectory(Path.Combine(_root, "tree")).FullName;
        var sibling = Directory.CreateDirectory(Path.Combine(_root, "tree-sibling")).FullName;
        var proc = Path.Combine(_root, "proc");
        DirectoryLinks.Create(Path.Combine(proc, "100", "cwd"), sibling);
        Directory.CreateDirectory(Path.Combine(proc, "300"));
        Directory.CreateDirectory(Path.Combine(proc, Environment.ProcessId.ToString()));

        Assert.Null(WorktreeProcessProbe.ProcScan(tree, proc));
    }
}
