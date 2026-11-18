namespace DynaDocs.Tests.Commands;

/// <summary>The command hub and its bodies form one logical reference document.</summary>
internal static class CommandReferenceFamily
{
    internal static string[] Paths(string hub, bool includeHub = true)
    {
        string[] suffixes = ["core", "delivery", "linear", "utilities", "worktrees"];
        var directory = Path.GetDirectoryName(hub)!;
        var bodies = suffixes.Select(suffix => Path.Combine(directory, $"dydo-commands-{suffix}.md")).ToArray();
        Assert.Equal(bodies, Directory.GetFiles(directory, "dydo-commands-*.md").Order(StringComparer.Ordinal));
        Assert.True(File.Exists(hub), $"Missing command reference hub: {hub}");
        return includeHub ? [hub, .. bodies] : bodies;
    }

    internal static string Read(string hub) => string.Join("\n", Paths(hub).Select(ReadNonempty));

    internal static string ReadNonempty(string path)
    {
        var content = File.ReadAllText(path);
        Assert.False(string.IsNullOrWhiteSpace(content), $"Empty reference page: {path}");
        return content;
    }
}
