namespace DynaDocs.Tests.SkillBuild;

using System.Text;
using DynaDocs.SkillBuild;
using DynaDocs.Tests.SkillBuild.Fixture;

/// <summary>
/// K7: every committed file that carries the built header is byte for byte what the model builds, so
/// hand edits cannot creep in. Over the real repository both sides are empty until DYD-1082 (AR1); over
/// a fixture root the diff names each changed, missing and extra file.
/// </summary>
public class SkillBuildGoldenTests
{
    [Fact]
    [Trait("Job", "contract: K7 golden test over the real repository (B14)")]
    public void CommittedBuiltFiles_AreByteForByteTheProductionBuild()
    {
        var diff = GoldenDiff.Describe(SkillBuilder.Build(ProductionModel.Model).Bytes(), BuiltHeader.Committed(CommittedTree.Root));

        Assert.True(diff.Length == 0, diff);
    }

    [Fact]
    [Trait("Job", "contract: K7 the fixture written to a root reads back as its own build (B14)")]
    public void FixtureRoot_AfterTheBuildWrites_MatchesTheBuild()
    {
        using var root = new TempRoot();

        Assert.Equal(0, root.Run(FixtureModel.Model).Exit);
        Assert.Equal("", GoldenDiff.Describe(Migration.Tree.Bytes(), BuiltHeader.Committed(root.Path)));
    }

    [Fact]
    [Trait("Job", "trap: K7 a one-byte hand edit of a built file fails the golden test, naming it (B14)")]
    public void FixtureRoot_WithAOneByteHandEdit_FailsNamingTheFile()
    {
        using var root = new TempRoot();
        root.Run(FixtureModel.Model);
        const string edited = "skills/roles/crew/scout/SKILL.md";
        var bytes = File.ReadAllBytes(root[edited]);
        bytes[^2] ^= 1;
        File.WriteAllBytes(root[edited], bytes);

        var diff = GoldenDiff.Describe(Migration.Tree.Bytes(), BuiltHeader.Committed(root.Path));

        Assert.StartsWith($"the built files differ from the build; {GoldenDiff.Rebuild}.\nchanged: {edited} line ", diff);
    }

    [Fact]
    [Trait("Job", "trap: K7 a deleted built file fails the golden test as missing (B14)")]
    public void FixtureRoot_MissingABuiltFile_FailsNamingIt()
    {
        using var root = new TempRoot();
        root.Run(FixtureModel.Model);
        File.Delete(root["dydo/guides/working-tree-standard.md"]);

        var diff = GoldenDiff.Describe(Migration.Tree.Bytes(), BuiltHeader.Committed(root.Path));

        Assert.Contains("missing: dydo/guides/working-tree-standard.md (the build writes it)", diff);
    }

    [Fact]
    [Trait("Job", "trap: K7 a header-carrying file the build does not write fails the golden test as extra (B14)")]
    public void FixtureRoot_WithAnExtraBuiltFile_FailsNamingIt()
    {
        using var root = new TempRoot();
        root.Run(FixtureModel.Model);
        root.Write("skills/roles/crew/scout/resources/stray.md", BuiltHeader.Markdown + "\n\n# Stray\n");

        var diff = GoldenDiff.Describe(Migration.Tree.Bytes(), BuiltHeader.Committed(root.Path));

        Assert.Contains("extra: skills/roles/crew/scout/resources/stray.md (carries the built header, but the build does not write it)", diff);
    }

    [Fact]
    public void Diff_NamesAChangedFileWithItsFirstDifferingLine()
    {
        var diff = GoldenDiff.Describe(Tree(("a.md", "same\nbuilt\n")), Tree(("a.md", "same\nedited\r\n")));

        Assert.Equal($"the built files differ from the build; {GoldenDiff.Rebuild}.\n" +
                     "changed: a.md line 2\n" +
                     "  built:     \"built\"\n" +
                     "  committed: \"edited\\r\"\n", diff.ReplaceLineEndings("\n"));
    }

    [Fact]
    public void Diff_NamesAFileThatEndsEarly()
    {
        var diff = GoldenDiff.Describe(Tree(("a.md", "one\ntwo\n")), Tree(("a.md", "one\n")));

        Assert.Contains("changed: a.md line 2\n  built:     \"two\"\n  committed: \"\"\n", diff.ReplaceLineEndings("\n"));
        Assert.Contains("built:     \"\"\n  committed: (end of file)",
            GoldenDiff.Describe(Tree(("a.md", "one\n")), Tree(("a.md", "one"))).ReplaceLineEndings("\n"));
    }

    [Fact]
    public void Diff_IsEmptyForEqualTrees() =>
        Assert.Equal("", GoldenDiff.Describe(Tree(("a.md", "x\n")), Tree(("a.md", "x\n"))));

    private static Dictionary<string, byte[]> Tree(params (string Path, string Text)[] files) =>
        files.ToDictionary(file => file.Path, file => Encoding.UTF8.GetBytes(file.Text), StringComparer.Ordinal);
}
