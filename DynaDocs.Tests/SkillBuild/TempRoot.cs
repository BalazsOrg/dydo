namespace DynaDocs.Tests.SkillBuild;

using DynaDocs.SkillBuild;

/// <summary>A disposable repository root for builds, and the tool's <c>Program.Run</c> against it.</summary>
internal sealed class TempRoot : IDisposable
{
    public string Path { get; } = Directory.CreateTempSubdirectory("dydo-skillbuild-").FullName;

    public string this[string relative] => System.IO.Path.Combine(Path, relative);

    public (int Exit, string Output, string Error) Run(Model model, params string[] args)
    {
        var output = new StringWriter();
        var error = new StringWriter();
        var exit = Program.Run(model, Path, args, output, error);
        return (exit, output.ToString(), error.ToString());
    }

    public void Write(string relative, string text)
    {
        Directory.CreateDirectory(System.IO.Path.GetDirectoryName(this[relative])!);
        File.WriteAllText(this[relative], text);
    }

    /// <summary>Every file under the root, by its forward-slash relative path.</summary>
    public IReadOnlyDictionary<string, string> Files() => Directory
        .EnumerateFiles(Path, "*", SearchOption.AllDirectories)
        .ToDictionary(file => System.IO.Path.GetRelativePath(Path, file).Replace('\\', '/'), File.ReadAllText, StringComparer.Ordinal);

    public void Dispose() => Directory.Delete(Path, recursive: true);
}
