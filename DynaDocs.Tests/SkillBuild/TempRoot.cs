namespace DynaDocs.Tests.SkillBuild;

using System.Diagnostics;
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

    /// <summary>Makes the root a git repository that tracks every file written so far.</summary>
    public void Track()
    {
        foreach (var arguments in new string[][] { ["init", "-q"], ["add", "-A"] })
        {
            using var git = Process.Start(new ProcessStartInfo("git", ["-C", Path, .. arguments]) { RedirectStandardOutput = true, RedirectStandardError = true })!;
            git.WaitForExit();
            Assert.Equal(0, git.ExitCode);
        }
    }

    // Git's object files are read-only, which stops a recursive delete on Windows.
    public void Dispose()
    {
        foreach (var file in Directory.EnumerateFiles(Path, "*", SearchOption.AllDirectories)) File.SetAttributes(file, FileAttributes.Normal);
        Directory.Delete(Path, recursive: true);
    }
}
