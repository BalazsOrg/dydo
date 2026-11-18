namespace DynaDocs.Tests.SkillBuild;

using System.Text.RegularExpressions;
using DynaDocs.SkillBuild;
using DynaDocs.Tests.SkillBuild.Fixture;

/// <summary>
/// K4 and AR2: one C# file per claim kind, holding exactly one kind class, and every kind file under the
/// 20,000-byte cap (T15's 5k tokens at bytes ÷ 4), so DYD-1082's claims are sized to fit from the start
/// (DYD-1196 A1, A7).
/// </summary>
public partial class KindFileTests
{
    public const int SizeCap = 20_000;

    private static readonly string[] KindFolders =
    [
        "tools/SkillBuild/Claims",
        "DynaDocs.Tests/SkillBuild/Fixture/Kinds",
        "DynaDocs.Tests/SkillBuild/Slots/Kinds"
    ];

    [GeneratedRegex(@"^\s*(?:(?:public|internal|private|protected|file|static|sealed|abstract|partial|readonly|ref)\s+)*(?:class|struct|record|interface|enum)\s+(?<name>\w+)", RegexOptions.Multiline)]
    private static partial Regex TypeDeclaration();

    [GeneratedRegex(@"""(?:[^""\\\n]|\\.)*""")]
    private static partial Regex StringLiteral();

    [Fact]
    [Trait("Job", "contract: AR2 every kind file is at most 20,000 bytes and holds exactly one public static kind class (A7)")]
    public void EveryKindFile_HoldsOneKindClassUnderTheCap()
    {
        var violations = KindFiles().SelectMany(Violations).Select(violation => violation.Replace(CommittedTree.Root, "")).ToArray();

        Assert.True(violations.Length == 0, string.Join('\n', violations));
    }

    [Fact]
    [Trait("Job", "contract: K4 each kind of a model lives in its own kind file (A1)")]
    public void EveryModelKind_HasItsOwnKindFile()
    {
        var files = KindFiles().Select(Path.GetFileNameWithoutExtension).ToHashSet(StringComparer.Ordinal);

        Assert.All(FixtureModel.Model.Kinds.Concat(ProductionModel.Model.Kinds), kind => Assert.Contains(kind.Name, files));
    }

    [Theory]
    [Trait("Job", "trap: AR2 an oversized or many-class kind file fails the check (A7)")]
    [InlineData(SizeCap + 1, "public static class Big\n{\n}\n", "over 20000 bytes")]
    [InlineData(0, "public static class Big\n{\n}\npublic static class Other\n{\n}\n", "holds 2 types")]
    [InlineData(0, "public static class Other\n{\n}\n", "is not one public static class Big")]
    public void KindFileCheck_RejectsWhatTheRuleForbids(int padding, string source, string reason)
    {
        var file = Path.Combine(Directory.CreateTempSubdirectory("dydo-kind-").FullName, "Big.cs");
        File.WriteAllText(file, source + new string(' ', padding));

        Assert.Contains(Violations(file), violation => violation.Contains(reason, StringComparison.Ordinal));
        Directory.Delete(Path.GetDirectoryName(file)!, recursive: true);
    }

    private static IEnumerable<string> KindFiles() => KindFolders
        .Select(folder => Path.Combine(CommittedTree.Root, folder))
        .Where(Directory.Exists)
        .SelectMany(folder => Directory.EnumerateFiles(folder, "*.cs"));

    private static IEnumerable<string> Violations(string file)
    {
        var name = Path.GetFileNameWithoutExtension(file);
        var code = StringLiteral().Replace(
            string.Join('\n', File.ReadLines(file).Where(line => !line.TrimStart().StartsWith("//", StringComparison.Ordinal))), "\"\"");
        var size = new FileInfo(file).Length;
        if (size > SizeCap) yield return $"{file}: {size} bytes, over {SizeCap} bytes";
        var types = TypeDeclaration().Matches(code).Select(match => match.Groups["name"].Value).ToArray();
        if (types.Length != 1) yield return $"{file}: holds {types.Length} types";
        if (!code.Contains($"public static class {name}", StringComparison.Ordinal) || types.FirstOrDefault() != name)
            yield return $"{file}: is not one public static class {name}";
    }
}
