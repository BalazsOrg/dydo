namespace DynaDocs.Tests.SkillBuild;

using System.Text;

/// <summary>Grilling A D3: the committed built tree is exactly what the claims build, so hand edits cannot creep in.</summary>
public class SkillBuildGoldenTests
{
    [Fact]
    public void CommittedBuiltTree_IsByteForByteTheBuildOfTheClaims()
    {
        var diff = GoldenDiff.Describe(CommittedTree.Rebuild(), CommittedTree.Built());

        Assert.True(diff.Length == 0, diff);
    }

    [Fact]
    public void Diff_NamesAChangedFileWithItsFirstDifferingLine()
    {
        var diff = GoldenDiff.Describe(Tree(("a.md", "same\nbuilt\n")), Tree(("a.md", "same\nedited\r\n")));

        Assert.Equal($"claims/built/ differs from the build; {GoldenDiff.Rebuild}.\n" +
                     "changed: claims/built/a.md line 2\n" +
                     "  built:     \"built\"\n" +
                     "  committed: \"edited\\r\"\n", diff.ReplaceLineEndings("\n"));
    }

    [Fact]
    public void Diff_NamesAFileThatEndsEarly()
    {
        var diff = GoldenDiff.Describe(Tree(("a.md", "one\ntwo\n")), Tree(("a.md", "one\n")));

        Assert.Contains("changed: claims/built/a.md line 2\n  built:     \"two\"\n  committed: \"\"\n", diff.ReplaceLineEndings("\n"));
        Assert.Contains("built:     \"\"\n  committed: (end of file)",
            GoldenDiff.Describe(Tree(("a.md", "one\n")), Tree(("a.md", "one"))).ReplaceLineEndings("\n"));
    }

    [Fact]
    public void Diff_NamesMissingAndExtraFiles()
    {
        var diff = GoldenDiff.Describe(Tree(("built.md", "x\n")), Tree(("stray.md", "x\n")));

        Assert.Contains("missing: claims/built/built.md (the build writes it)", diff);
        Assert.Contains("extra: claims/built/stray.md (the build does not write it)", diff);
    }

    [Fact]
    public void Diff_IsEmptyForEqualTrees() =>
        Assert.Equal("", GoldenDiff.Describe(Tree(("a.md", "x\n")), Tree(("a.md", "x\n"))));

    private static Dictionary<string, byte[]> Tree(params (string Path, string Text)[] files) =>
        files.ToDictionary(file => file.Path, file => Encoding.UTF8.GetBytes(file.Text), StringComparer.Ordinal);
}
