namespace DynaDocs.Tests.SkillBuild;

using System.Text;
using System.Text.Json;

/// <summary>A readable account of how the committed built files differ from the build: changed, missing and extra files.</summary>
internal static class GoldenDiff
{
    public const string Rebuild = "edit the model in tools/SkillBuild, then run `dotnet run --project tools/SkillBuild` and commit the result";

    public static string Describe(IReadOnlyDictionary<string, byte[]> built, IReadOnlyDictionary<string, byte[]> committed)
    {
        var report = new StringBuilder();
        foreach (var path in built.Keys.Union(committed.Keys).Order(StringComparer.Ordinal))
        {
            if (!committed.TryGetValue(path, out var actual))
                report.AppendLine($"missing: {path} (the build writes it)");
            else if (!built.TryGetValue(path, out var expected))
                report.AppendLine($"extra: {path} (carries the built header, but the build does not write it)");
            else if (!expected.AsSpan().SequenceEqual(actual))
                report.AppendLine(Changed(path, expected, actual));
        }
        return report.Length == 0 ? "" : report.Insert(0, $"the built files differ from the build; {Rebuild}.\n").ToString();
    }

    private static string Changed(string path, byte[] expected, byte[] actual)
    {
        var built = Encoding.UTF8.GetString(expected).Split('\n');
        var committed = Encoding.UTF8.GetString(actual).Split('\n');
        var line = Enumerable.Range(0, Math.Max(built.Length, committed.Length))
            .First(index => index >= built.Length || index >= committed.Length || built[index] != committed[index]);
        return $"changed: {path} line {line + 1}\n" +
               $"  built:     {Quote(built, line)}\n" +
               $"  committed: {Quote(committed, line)}";
    }

    private static string Quote(string[] lines, int index) =>
        index < lines.Length ? JsonSerializer.Serialize(lines[index]) : "(end of file)";
}
