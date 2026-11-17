namespace DynaDocs.Services;

using System.Security.Cryptography;

internal static class SafeDeletePaths
{
    internal static StringComparison Comparison => OperatingSystem.IsWindows() ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;
    internal static bool Same(string a, string b) => string.Equals(Path.TrimEndingDirectorySeparator(Path.GetFullPath(a)), Path.TrimEndingDirectorySeparator(Path.GetFullPath(b)), Comparison);
    internal static bool Within(string path, string root)
    {
        // A volume root such as "/" or "C:\" already ends in its separator; never double it.
        var parent = Path.GetFullPath(root);
        if (!Path.EndsInDirectorySeparator(parent)) parent += Path.DirectorySeparatorChar;
        return Same(path, root) || Path.GetFullPath(path).StartsWith(parent, Comparison);
    }

    internal static void NoLinks(string path)
    {
        for (string? current = Path.GetFullPath(path); current != null; current = Path.GetDirectoryName(current))
        {
            if ((File.Exists(current) || Directory.Exists(current) || new FileInfo(current).LinkTarget != null)
                && File.GetAttributes(current).HasFlag(FileAttributes.ReparsePoint))
                throw new InvalidDataException($"Linked path is unsafe: {current}");
        }
    }

    internal static string Child(string root, string relative)
    {
        if (string.IsNullOrWhiteSpace(relative) || relative.Contains('\\') || Path.IsPathRooted(relative)
            || relative.Split('/').Any(part => part is "" or "." or ".." || part.Contains(':')))
            throw new InvalidDataException($"Unsafe archive path: {relative}");
        var path = Path.GetFullPath(Path.Combine(root, relative.Replace('/', Path.DirectorySeparatorChar)));
        if (!Within(path, root) || Same(path, root)) throw new InvalidDataException("Archive traversal refused");
        return path;
    }

    internal static string Hash(string path)
    {
        using var stream = File.OpenRead(path);
        return Convert.ToHexString(SHA256.HashData(stream));
    }

    internal static void Write(string path, byte[] bytes)
    {
        NoLinks(path);
        using var file = new FileStream(path, FileMode.CreateNew, FileAccess.Write, FileShare.None);
        file.Write(bytes);
        file.Flush(true);
    }
}
