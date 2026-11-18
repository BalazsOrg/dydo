namespace DynaDocs.Tests.Integration;

/// <summary>
/// Builds each named project once per test class and copies it into a row's own directory, so
/// every row still meets a fresh tree without paying the dozen git processes building one costs;
/// on Windows CI those processes were most of the guard-windows job (DYD-1199). A copied tree
/// keeps the template's absolute paths only where git wrote them, which the caller re-points: a
/// local origin's URL, a linked worktree's back-links.
/// </summary>
public sealed class PushedProjectTemplates : IDisposable
{
    private readonly string _root = Path.Combine(Path.GetTempPath(), "dydo-template-" + Guid.NewGuid().ToString("N")[..8]);
    private readonly HashSet<string> _built = [];

    /// <summary>
    /// Copies the template <paramref name="name"/> into <paramref name="target"/>, first building it
    /// with <paramref name="build"/>, run from inside the template directory.
    /// </summary>
    public async Task CopyInto(string name, string target, Func<string, Task> build)
    {
        var template = Path.Combine(_root, name);
        if (!_built.Contains(name))
        {
            if (Directory.Exists(template)) Directory.Delete(template, recursive: true);
            Directory.CreateDirectory(template);
            var cwd = Environment.CurrentDirectory;
            Environment.CurrentDirectory = template;
            try { await build(template); }
            finally { Environment.CurrentDirectory = cwd; }
            _built.Add(name);
        }
        Copy(new DirectoryInfo(template), target);
    }

    private static void Copy(DirectoryInfo source, string target)
    {
        Directory.CreateDirectory(target);
        foreach (var file in source.EnumerateFiles())
            file.CopyTo(Path.Combine(target, file.Name));
        foreach (var directory in source.EnumerateDirectories())
            Copy(directory, Path.Combine(target, directory.Name));
    }

    public void Dispose()
    {
        try
        {
            // Git marks its objects read-only, which Windows will not delete.
            foreach (var file in new DirectoryInfo(_root).EnumerateFiles("*", SearchOption.AllDirectories))
                file.Attributes = FileAttributes.Normal;
            Directory.Delete(_root, recursive: true);
        }
        catch (IOException) { }
        catch (UnauthorizedAccessException) { }
    }
}
