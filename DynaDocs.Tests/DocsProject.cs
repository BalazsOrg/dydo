namespace DynaDocs.Tests;

/// <summary>
/// A throwaway dydo project with a valid config, hub and off-limits file, made the current
/// directory for the test's lifetime, so `dydo check`, `fix`, `graph` and `index` run on it.
/// Classes that use it belong to the "Integration" collection.
/// </summary>
public sealed class DocsProject : IDisposable
{
    public const string ValidConfig =
        """{"version":1,"structure":{"root":"dydo"},"scanExclude":["_system/.local/","agents/"]}""";

    public const string Hub = "---\narea: general\ntype: hub\n---\n\n# Index\n\nThe hub.\n";

    public const string OffLimits = "---\ntype: config\n---\n\n# Files Off-Limits\n\n```\n.env*\n```\n";

    private readonly string _original = Environment.CurrentDirectory;

    public DocsProject(string config = ValidConfig)
    {
        Root = Path.Combine(Path.GetTempPath(), "dydo-docs-" + Guid.NewGuid().ToString("N")[..12]);
        Directory.CreateDirectory(Path.Combine(Root, "dydo", "_system"));
        File.WriteAllText(Path.Combine(Root, "dydo.json"), config);
        Write("index.md", Hub);
        Write("files-off-limits.md", OffLimits);
        Environment.CurrentDirectory = Root;
    }

    public string Root { get; }

    public string Docs => Path.Combine(Root, "dydo");

    /// <summary>Writes a file under the docs root (`dydo/`), creating its folders.</summary>
    public string Write(string docsRelativePath, string content)
    {
        var path = DocPath(docsRelativePath);
        Directory.CreateDirectory(Path.GetDirectoryName(path)!);
        File.WriteAllText(path, content);
        return path;
    }

    public string DocPath(string docsRelativePath) =>
        Path.Combine(Docs, docsRelativePath.Replace('/', Path.DirectorySeparatorChar));

    public string Read(string docsRelativePath) => File.ReadAllText(DocPath(docsRelativePath));

    public void Dispose()
    {
        Environment.CurrentDirectory = _original;
        TestDirectory.Delete(Root);
    }
}
