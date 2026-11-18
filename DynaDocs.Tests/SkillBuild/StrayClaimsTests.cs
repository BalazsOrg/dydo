namespace DynaDocs.Tests.SkillBuild;

using DynaDocs.SkillBuild;
using DynaDocs.Tests.SkillBuild.Slots;

/// <summary>
/// K24: claim text pasted by hand outside the built files is reported with its path, line and claim.
/// Over the real repository the lint has no built lines until DYD-1082 (DYD-1196 F21).
/// </summary>
public class StrayClaimsTests
{
    private const string Escalate = "Escalate to the officer when the lease cannot be renewed.";

    private static readonly BuiltTree Demo = SkillBuilder.Build(DemoModel.With());

    [Fact]
    [Trait("Job", "contract: K24 the stray-claim lint runs over the real repository beside the golden test (F21)")]
    public void RealRepository_HasNoStrayClaimText() =>
        Assert.Empty(StrayClaims.Find(CommittedTree.Root, CommittedTree.Tracked(), SkillBuilder.Build(ProductionModel.Model)));

    [Fact]
    [Trait("Job", "trap: K24 a claim line pasted into a hand-written doc, even rewrapped, is caught with path, line and claim (F21)")]
    public void PastedClaimLine_IsReportedAtItsLine()
    {
        using var root = Root(("dydo/guides/notes.md", "# Notes\n\nIntro.\n\nEscalate to the officer when the lease\n   cannot be renewed.\n"));

        Assert.Equal([$"dydo/guides/notes.md:5 claim:{nameof(DemoMoves)}.{nameof(DemoMoves.EscalateWhenStuck)}"], Find(root));
    }

    [Theory]
    [Trait("Job", "edge: K24 only hand-written .md files under skills/, dydo/, Scaffold/ and the root AGENTS.md are read (F21)")]
    [InlineData("AGENTS.md", 1)]
    [InlineData("skills/productivity/x/SKILL.md", 1)]
    [InlineData("Scaffold/dydo/guides/x.md", 1)]
    [InlineData("claims/inventory/README.md", 0)]
    [InlineData("tools/SkillBuild/Claims/X.cs", 0)]
    [InlineData("docs/AGENTS.md", 0)]
    [InlineData("dydo/guides/x.txt", 0)]
    public void OnlyHandWrittenDocsInTheLintedRoots_AreRead(string path, int hits)
    {
        using var root = Root((path, Escalate + "\n"));

        Assert.Equal(hits, Find(root).Count);
    }

    [Fact]
    [Trait("Job", "trap: K24 two claims that build the same text are both named when it is pasted (F21)")]
    public void PastedTextTwoClaimsBuild_NamesBothClaims()
    {
        var tree = SkillBuilder.Build(DemoModel.With(typeof(DemoRepeated)));
        using var root = Root(("AGENTS.md", DemoRepeated.Text + "\n"));

        Assert.Equal([$"AGENTS.md:1 claim:{nameof(DemoRepeated)}.{nameof(DemoRepeated.One)} claim:{nameof(DemoRepeated)}.{nameof(DemoRepeated.Two)}"],
            StrayClaims.Find(root.Path, [.. root.Files().Keys], tree));
    }

    [Fact]
    [Trait("Job", "contract: K24 built files, by path or by header, are not linted (F21)")]
    public void BuiltFiles_AreNotLinted()
    {
        using var root = Root(("skills/productivity/x/SKILL.md", BuiltHeader.Markdown + "\n" + Escalate + "\n"));
        root.Write("skills/roles/crew/demo/resources/stale-lease.md", Demo.Files["skills/roles/crew/demo/resources/stale-lease.md"]);

        Assert.Empty(StrayClaims.Find(root.Path, [.. root.Files().Keys], Demo));
    }

    [Fact]
    [Trait("Job", "edge: K24 a line shorter than 40 characters or not a claim is never a stray (F21)")]
    public void ShortClaimLinesAndOtherText_AreNotStrays()
    {
        using var root = Root(("dydo/guides/notes.md", "Read the brief first.\nEscalate to the officer when it is late.\n"));

        Assert.Empty(Find(root));
    }

    private static TempRoot Root(params (string Path, string Text)[] files)
    {
        var root = new TempRoot();
        foreach (var (path, text) in files) root.Write(path, text);
        return root;
    }

    private static IReadOnlyList<string> Find(TempRoot root) => StrayClaims.Find(root.Path, [.. root.Files().Keys], Demo);
}
