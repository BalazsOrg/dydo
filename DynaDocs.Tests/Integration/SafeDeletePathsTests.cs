namespace DynaDocs.Tests.Integration;

using DynaDocs.Services;

[Collection("Integration")]
public sealed class SafeDeletePathsTests
{
    [Theory]
    [InlineData("../escape")]
    [InlineData("nested/../../escape")]
    [InlineData("nested/../file")]
    [InlineData("nested/./file")]
    [InlineData("nested//file")]
    [InlineData("/absolute")]
    [InlineData("//server/share")]
    [InlineData("C:\\escape")]
    [InlineData("nested\\escape")]
    [InlineData("")]
    public void UnsafePaths_RefuseTraversalAndForeignSeparators(string relative)
    {
        var root = Path.Combine(Path.GetTempPath(), "dydo-path-boundary");
        Assert.Throws<InvalidDataException>(() => SafeDeletePaths.Child(root, relative));
    }

    [Theory]
    [InlineData("logs/run-2026-11-16T21:25:15.log")]
    [InlineData("logs:today/run.log")]
    [InlineData("file.txt:hidden")]
    [InlineData("file.txt::$DATA")]
    [InlineData("C:relative")]
    [InlineData("C:/escape")]
    [InlineData("nested/C:/escape")]
    public void ColonPaths_AllowNativeNamesAndRefuseWindowsDrivesAndStreams(string relative)
    {
        var root = Path.Combine(Path.GetTempPath(), "dydo-path-boundary");
        if (OperatingSystem.IsWindows())
            Assert.Throws<InvalidDataException>(() => SafeDeletePaths.Child(root, relative));
        else
            Assert.Equal(Path.Combine(root, relative), SafeDeletePaths.Child(root, relative));
    }

    [Fact]
    public void LinuxIgnoredTimestamp_ArchivesDeletesAndRestoresExactNameAndBytes()
    {
        if (!OperatingSystem.IsLinux()) return; // Native Linux filename; Windows rejects it above.
        using var fixture = new SafeDeleteTests();
        fixture.AddTree();
        const string relative = "ignored/logs/run-2026-11-16T21:25:15.log";
        byte[] bytes = [0, 255, 13, 10, 58, 128, 0];
        var path = Path.Combine(fixture.Tree, relative);
        Directory.CreateDirectory(Path.GetDirectoryName(path)!);
        File.WriteAllBytes(path, bytes);
        Assert.Contains("!! " + relative, SafeDeleteTests.Git(fixture.Tree, "status", "--porcelain=v1", "--ignored", "--untracked-files=all"));
        Assert.True(fixture.Run("worktree", fixture.Tree) == 0, fixture.Output);
        Assert.False(Directory.Exists(fixture.Tree));
        Assert.False(fixture.Has("refs/heads/topic"));
        Assert.Contains("Independent proof passed", fixture.Output);
        SafeDeleteTests.Git(fixture.Main, "reflog", "expire", "--expire=now", "--all");
        SafeDeleteTests.Git(fixture.Main, "gc", "--prune=now");
        Assert.True(fixture.Run("restore", fixture.Id) == 0, fixture.Output);
        Assert.Equal(bytes, File.ReadAllBytes(path));
        Assert.Equal("run-2026-11-16T21:25:15.log", Path.GetFileName(Directory.GetFiles(Path.GetDirectoryName(path)!).Single()));
        Assert.Contains("!! " + relative, SafeDeleteTests.Git(fixture.Tree, "status", "--porcelain=v1", "--ignored", "--untracked-files=all"));
    }
}
