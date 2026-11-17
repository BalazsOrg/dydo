namespace DynaDocs.SkillBuild;

/// <summary>Makes a directory hold exactly the built tree: the build is the only writer of claims/built/.</summary>
public static class BuiltTree
{
    public static void Write(string directory, IReadOnlyDictionary<string, byte[]> tree)
    {
        if (Directory.Exists(directory))
        {
            foreach (var file in Directory.EnumerateFiles(directory, "*", SearchOption.AllDirectories).ToArray())
                if (!tree.ContainsKey(Path.GetRelativePath(directory, file).Replace('\\', '/'))) File.Delete(file);
            foreach (var folder in Directory.EnumerateDirectories(directory, "*", SearchOption.AllDirectories)
                         .OrderByDescending(folder => folder.Length).ToArray())
                if (!Directory.EnumerateFileSystemEntries(folder).Any()) Directory.Delete(folder);
        }
        foreach (var (path, bytes) in tree)
        {
            var target = Path.Combine(directory, path);
            Directory.CreateDirectory(Path.GetDirectoryName(target)!);
            File.WriteAllBytes(target, bytes);
        }
    }
}
