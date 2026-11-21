namespace DynaDocs.Utils;

public static partial class PathUtils
{
    public static string? FindDydoRoot(string? startPath = null)
    {
        var projectRoot = ConfigFileLocator.FindProjectRoot(startPath);
        if (projectRoot == null) return null;
        return ConfigFileLocator.GetDydoRoot(projectRoot);
    }

    public static string? FindProjectRoot(string? startPath = null)
    {
        return ConfigFileLocator.FindProjectRoot(startPath);
    }

    public static string ResolvePath(string sourcePath, string relativePath)
    {
        var sourceDir = Path.GetDirectoryName(sourcePath) ?? "";
        var resolved = Path.GetFullPath(Path.Combine(sourceDir, relativePath));
        return NormalizePath(resolved);
    }

    public static string? FindDocsFolder(string startPath)
    {
        var projectRoot = ConfigFileLocator.FindProjectRoot(startPath);

        if (projectRoot != null)
        {
            var dydoRoot = ConfigFileLocator.GetDydoRoot(projectRoot);
            if (Directory.Exists(dydoRoot) && File.Exists(Path.Combine(dydoRoot, "index.md")))
                return dydoRoot;
        }

        return FindLegacyDocsFolder(startPath);
    }

    private static string? FindLegacyDocsFolder(string startPath)
    {
        var docsPath = Path.Combine(startPath, "docs");
        if (!Directory.Exists(docsPath)) return null;

        var indexPath = Directory.GetFiles(docsPath, "index.md", SearchOption.TopDirectoryOnly)
            .FirstOrDefault();
        if (indexPath != null) return docsPath;

        foreach (var subDir in Directory.GetDirectories(docsPath))
        {
            indexPath = Directory.GetFiles(subDir, "index.md", SearchOption.TopDirectoryOnly)
                .FirstOrDefault();
            if (indexPath != null) return subDir;
        }

        return null;
    }

    public static string GetRelativePath(string fromPath, string toPath)
    {
        var fromDir = Path.GetDirectoryName(fromPath) ?? "";
        var fromParts = fromDir.Split(['/', '\\'], StringSplitOptions.RemoveEmptyEntries);
        var toParts = toPath.Split(['/', '\\'], StringSplitOptions.RemoveEmptyEntries);

        var commonLength = 0;
        for (int i = 0; i < Math.Min(fromParts.Length, toParts.Length - 1); i++)
        {
            if (fromParts[i].Equals(toParts[i], StringComparison.OrdinalIgnoreCase))
                commonLength++;
            else
                break;
        }

        var upCount = fromParts.Length - commonLength;
        var relativeParts = new List<string>();

        for (int i = 0; i < upCount; i++)
            relativeParts.Add("..");

        for (int i = commonLength; i < toParts.Length; i++)
            relativeParts.Add(toParts[i]);

        if (relativeParts.Count == 0 || !relativeParts[0].StartsWith(".."))
            relativeParts.Insert(0, ".");

        return string.Join("/", relativeParts);
    }
}
