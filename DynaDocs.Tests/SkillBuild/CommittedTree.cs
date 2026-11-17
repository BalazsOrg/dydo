namespace DynaDocs.Tests.SkillBuild;

using System.Text;

/// <summary>The checkout's skill-build inputs and its committed <c>claims/built/</c> tree.</summary>
internal static class CommittedTree
{
    public static string Root { get; } = FindRoot();

    public static IReadOnlyDictionary<string, byte[]> Rebuild() => DynaDocs.SkillBuild.SkillBuilder.Build(
        File.ReadAllBytes(Path.Combine(Root, "claims", "inventory", "claims.json")),
        File.ReadAllBytes(Path.Combine(Root, "claims", "carriers.json")));

    public static IReadOnlyDictionary<string, byte[]> Built()
    {
        var built = Path.Combine(Root, "claims", "built");
        return Directory.EnumerateFiles(built, "*", SearchOption.AllDirectories).ToDictionary(
            file => Path.GetRelativePath(built, file).Replace('\\', '/'), File.ReadAllBytes, StringComparer.Ordinal);
    }

    public static IReadOnlyDictionary<string, string> BuiltText() =>
        Built().ToDictionary(file => file.Key, file => Encoding.UTF8.GetString(file.Value), StringComparer.Ordinal);

    private static string FindRoot()
    {
        for (var directory = new DirectoryInfo(AppContext.BaseDirectory); directory is not null; directory = directory.Parent)
            if (File.Exists(Path.Combine(directory.FullName, "DynaDocs.csproj"))) return directory.FullName;
        throw new DirectoryNotFoundException($"Could not locate repository root from {AppContext.BaseDirectory}");
    }
}
