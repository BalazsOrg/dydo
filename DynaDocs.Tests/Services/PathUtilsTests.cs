namespace DynaDocs.Tests.Services;

using DynaDocs.Utils;
using Xunit;

/// <summary>
/// Path collapsing is what the guard compares against protected and off-limits paths, so a '..'
/// that climbs above a root, or a UNC share root read as a plain folder, is a bypass. The main
/// project root is how a worktree finds the checkout that owns its shared state.
/// </summary>
[Trait("Job", "trap: DYD-396, DYD-547 ('..' and UNC/device roots in guarded paths); edge: both host readings on any host")]
public class PathUtilsTests
{
    [Theory]
    [InlineData("a/b/c", "a/b/c")]
    [InlineData("a/./b", "a/b")]
    [InlineData("a/b/../c", "a/c")]
    [InlineData("a/b/../../c", "c")]
    [InlineData("C:/x/y/../z", "C:/x/z")]
    [InlineData("C:\\x\\y\\..\\z", "C:/x/z")]
    [InlineData("/home/u/.claude/projects/p/memory/../../../../etc/secret", "/home/u/etc/secret")]
    [InlineData("a/../../escape", "../escape")]      // relative escape kept (can't be mistaken for a trusted root)
    [InlineData("/a/../../escape", "/escape")]        // rooted '..' at top is dropped
    public void CollapseRelativeSegments_ResolvesDotDot(string input, string expected)
    {
        Assert.Equal(expected, PathUtils.CollapseRelativeSegments(input));
    }

    // On Windows a UNC share root (and a \\?\ or \\.\ device root) is the root '..' cannot climb
    // above, kept as written after slash normalisation (DYD-396). Elsewhere a leading '//' is
    // just '/', as the OS reads it. Columns: input, Windows reading, other hosts' reading.
    public static TheoryData<string, string, string> RootForms => new()
    {
        { @"\\server\share\repo\dydo\index.md", "//server/share/repo/dydo/index.md", "/server/share/repo/dydo/index.md" },
        { "//server/share/repo/dydo/index.md", "//server/share/repo/dydo/index.md", "/server/share/repo/dydo/index.md" },
        { @"\\server\share\repo\src\..\dydo\index.md", "//server/share/repo/dydo/index.md", "/server/share/repo/dydo/index.md" },
        { "//server/share/repo/./src/../dydo/index.md", "//server/share/repo/dydo/index.md", "/server/share/repo/dydo/index.md" },
        { @"\\server\share\repo\..\..\..\etc", "//server/share/etc", "/etc" },
        { "//server/share/../other/x", "//server/share/other/x", "/server/other/x" },
        { @"\\server\share\..", "//server/share/", "/server" },
        { @"\\server\share", "//server/share/", "/server/share" },
        { @"\\server", "//server/", "/server" },
        { @"\\?\C:\x\..\y", "//?/C:/y", "/?/C:/y" },
        { @"\\?\C:\..\..\y", "//?/C:/y", "/y" },
        { @"\\?\UNC\server\share\..\x", "//?/UNC/server/share/x", "/?/UNC/server/x" },
        { "//./pipe/../x", "//./x", "/x" },
        { "///x/../y", "/y", "/y" },                // an empty server name is no UNC root
        { "//", "/", "/" },
        { "//a", "//a/", "/a" },
        { "//?", "//?/", "/?" },                    // too short for a device root: '?' is a server
        { "//?x/../y", "//?x/../y", "/y" },         // no device slash: server '?x', share '..'
        { "//./C:/..", "//./C:/", "/" },
        { "//?/unc/s/sh/x", "//?/unc/s/sh/x", "/?/unc/s/sh/x" },
        { "//?/unc/s/sh/..", "//?/unc/s/sh/", "/?/unc/s" }, // a lowercase unc is still a share root
        { "//?/UNC/server", "//?/UNC/server/", "/?/UNC/server" },
        { "//?/UNC//x/..", "//?/UNC", "/?/UNC" },   // UNC without a server: the device root only
        { "//?/UNC/", "//?/UNC", "/?/UNC" },
        { "//./", "//./", "/" },
        { "//?/", "//?/", "/?" },
        { "//?/x/../..", "//?/", "/" },
        { "C:", "C:", "C:" },
        { "c:..", "c:..", "c:.." },
        { "/a/../../escape", "/escape", "/escape" },
        { "a/../../escape", "../escape", "../escape" },
    };

    // The host-independent seam runs both readings on every host, so a Linux run exercises the
    // Windows root forms too (DYD-547).
    [Theory]
    [MemberData(nameof(RootForms))]
    public void CollapseRelativeSegments_BothReadings_OnAnyHost(string input, string windowsExpected, string otherExpected)
    {
        Assert.Equal(windowsExpected, PathUtils.CollapseRelativeSegments(input, windows: true));
        Assert.Equal(otherExpected, PathUtils.CollapseRelativeSegments(input, windows: false));
    }

    #region GetMainProjectRoot

    [Theory]
    [InlineData("C:/Projects/DynaDocs/dydo/_system/.local/worktrees/fix-auth", "C:/Projects/DynaDocs")]
    [InlineData("C:/Projects/DynaDocs/dydo/_system/.local/worktrees/fix-auth/", "C:/Projects/DynaDocs")]
    [InlineData("C:\\Projects\\DynaDocs\\dydo\\_system\\.local\\worktrees\\fix-auth", "C:/Projects/DynaDocs")]
    public void GetMainProjectRoot_WorktreeCwd_ReturnsMainRoot(string cwd, string expected)
    {
        Assert.Equal(expected, PathUtils.GetMainProjectRoot(cwd));
    }

    [Theory]
    [InlineData("C:/Projects/DynaDocs")]
    [InlineData("C:/Projects/DynaDocs/Commands")]
    [InlineData("/home/user/project")]
    public void GetMainProjectRoot_NonWorktreeCwd_ReturnsNull(string cwd)
    {
        Assert.Null(PathUtils.GetMainProjectRoot(cwd));
    }

    #endregion
}
