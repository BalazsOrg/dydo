namespace DynaDocs.Tests;

internal static class RepositoryFiles
{
    internal static string Locate(params string[] parts)
    {
        for (var directory = new DirectoryInfo(AppContext.BaseDirectory); directory != null; directory = directory.Parent)
        {
            var candidate = Path.Combine([directory.FullName, .. parts]);
            if (File.Exists(candidate))
                return candidate;
        }
        throw new FileNotFoundException($"Could not locate repository file {Path.Combine(parts)}.");
    }
}
