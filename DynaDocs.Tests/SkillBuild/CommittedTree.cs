namespace DynaDocs.Tests.SkillBuild;

using System.Diagnostics;
using System.Text;
using DynaDocs.SkillBuild;

/// <summary>The checkout the suite runs in: its root, its tracked files and its committed built files.</summary>
internal static class CommittedTree
{
    // DYD-1439: the folder of each skill written by hand, which the build owns no file of, whatever it holds.
    private static readonly string[] HandWritten = [.. ProductionModel.Model.Skills.Where(skill => skill.HandWritten).SelectMany(skill => skill.Folders)];

    public static string Root { get; } = FindRoot();

    /// <summary>Each committed file that carries the built header, outside every hand-written skill's folder.</summary>
    public static IReadOnlyDictionary<string, string> BuiltText() => BuiltHeader.Committed(Root)
        .Where(file => !HandWritten.Any(folder => file.Key.StartsWith(folder, StringComparison.Ordinal)))
        .ToDictionary(file => file.Key, file => Encoding.UTF8.GetString(file.Value), StringComparer.Ordinal);

    public static IReadOnlyList<string> Tracked()
    {
        using var git = Process.Start(new ProcessStartInfo("git", ["-C", Root, "ls-files", "-z"]) { RedirectStandardOutput = true })!;
        var paths = git.StandardOutput.ReadToEnd().Split('\0', StringSplitOptions.RemoveEmptyEntries);
        git.WaitForExit();
        Assert.Equal(0, git.ExitCode);
        return paths;
    }

    private static string FindRoot()
    {
        for (var directory = new DirectoryInfo(AppContext.BaseDirectory); directory is not null; directory = directory.Parent)
            if (File.Exists(Path.Combine(directory.FullName, "DynaDocs.csproj"))) return directory.FullName;
        throw new DirectoryNotFoundException($"Could not locate repository root from {AppContext.BaseDirectory}");
    }
}
