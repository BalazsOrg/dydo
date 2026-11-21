using System.Text;

namespace DynaDocs.SkillBuild;

/// <summary>What a model builds: each file's text by its path under the repository root, and the line map.</summary>
public sealed class BuiltTree
{
    private static readonly UTF8Encoding Utf8 = new(encoderShouldEmitUTF8Identifier: false);

    internal BuiltTree(IReadOnlyDictionary<string, string> files, IReadOnlyList<MapLine> map, IReadOnlyDictionary<string, string> unbuilt)
    {
        Files = files;
        Map = map;
        Unbuilt = unbuilt;
    }

    public IReadOnlyDictionary<string, string> Files { get; }

    /// <summary>
    /// The files of each legacy skill a ledger replaces, as its lines still render: checked like any
    /// other, never written, and outside the map.
    /// </summary>
    public IReadOnlyDictionary<string, string> Unbuilt { get; }

    /// <summary>
    /// The chart files of each machine whose chart is written by hand (DYD-1437) and the files of each skill
    /// written by hand (DYD-1439), as the model still renders them: checked by the build's tests, never written.
    /// </summary>
    public IReadOnlyDictionary<string, string> Unwritten { get; init; } = new Dictionary<string, string>();

    /// <summary>
    /// The folder of each skill written by hand (DYD-1439), ending in <c>/</c>: the build writes, deletes
    /// and compares no file under it, whatever the folder holds.
    /// </summary>
    public IReadOnlyList<string> HandWritten { get; init; } = [];

    /// <summary>The claim-stating lines of the built skill files and of the unwritten ones.</summary>
    public IReadOnlyList<MapLine> Map { get; }

    public IReadOnlyDictionary<string, byte[]> Bytes() =>
        Files.ToDictionary(file => file.Key, file => Utf8.GetBytes(file.Value), StringComparer.Ordinal);

    /// <summary>
    /// Writes every built file under <paramref name="root"/> and deletes each committed file that carries
    /// the header but is no longer built (K5, K6). A file without the header is never touched: when one
    /// sits at a built path, nothing is written and the write throws. No file under a
    /// <see cref="HandWritten"/> folder is touched, with the header or without it.
    /// </summary>
    public void Write(string root)
    {
        var committed = Owned(BuiltHeader.Committed(root));
        var handWritten = Files.Keys.FirstOrDefault(path => File.Exists(Path.Combine(root, path)) && !committed.ContainsKey(path));
        if (handWritten is not null)
            throw new InvalidDataException($"{handWritten} has no built header; the build never overwrites a hand-written file");
        foreach (var stale in committed.Keys.Where(path => !Files.ContainsKey(path)))
            Delete(root, stale);
        foreach (var (path, text) in Files)
        {
            var target = Path.Combine(root, path);
            Directory.CreateDirectory(Path.GetDirectoryName(target)!);
            File.WriteAllText(target, text, Utf8);
        }
    }

    /// <summary>The committed files outside every <see cref="HandWritten"/> folder: the ones the build owns.</summary>
    public IReadOnlyDictionary<string, byte[]> Owned(IReadOnlyDictionary<string, byte[]> committed) =>
        committed.Where(file => !IsHandWritten(file.Key)).ToDictionary(file => file.Key, file => file.Value, StringComparer.Ordinal);

    /// <summary>Whether <paramref name="path"/>, relative to the repository root, lies under a <see cref="HandWritten"/> folder.</summary>
    public bool IsHandWritten(string path) => HandWritten.Any(folder => path.StartsWith(folder, StringComparison.Ordinal));

    private static void Delete(string root, string path)
    {
        var top = Path.TrimEndingDirectorySeparator(Path.GetFullPath(root));
        var file = Path.GetFullPath(Path.Combine(top, path));
        File.Delete(file);
        for (var folder = Path.GetDirectoryName(file)!; folder != top && !Directory.EnumerateFileSystemEntries(folder).Any(); folder = Path.GetDirectoryName(folder)!)
            Directory.Delete(folder);
    }
}
