namespace DynaDocs.Tests.SkillBuild;

using DynaDocs.SkillBuild;
using DynaDocs.Tests.SkillBuild.Slots;

/// <summary>
/// <c>dotnet run --project tools/SkillBuild</c> through <see cref="Program.Run"/>: it writes straight
/// into the real layout, deletes only header-carrying files it no longer builds, and never touches a
/// hand-written file (K5, K6, K8; DYD-1196 B12, B13, D19).
/// </summary>
public class SkillBuildWriteTests
{
    private const string Spine = "skills/roles/crew/demo/SKILL.md";

    [Fact]
    [Trait("Job", "contract: K6 the build writes every built file under the root (B12)")]
    public void Run_WritesEveryBuiltFileUnderTheRoot()
    {
        using var root = new TempRoot();

        var (exit, output, error) = root.Run(DemoModel.With());

        Assert.Equal((0, "", ""), (exit, output, error));
        Assert.Equal(SkillBuilder.Build(DemoModel.With()).Files, root.Files());
    }

    [Fact]
    [Trait("Job", "contract: K6 a header-carrying file the model no longer builds is deleted, with its emptied folders (B12)")]
    public void Run_DeletesAStaleBuiltFileAndItsEmptiedFolders()
    {
        using var root = new TempRoot();
        root.Write("skills/roles/crew/gone/SKILL.md", "---\nname: gone\n---\n" + BuiltHeader.Markdown + "\n");
        root.Write("skills/roles/crew/gone/agents/openai.yaml", BuiltHeader.Yaml + "\n");

        Assert.Equal(0, root.Run(DemoModel.With()).Exit);

        Assert.False(Directory.Exists(root["skills/roles/crew/gone"]));
        Assert.True(Directory.Exists(root["skills/roles/crew/demo"]));
    }

    [Fact]
    [Trait("Job", "contract: K5 a file without the header is never touched (B12)")]
    public void Run_LeavesAHandWrittenFileAlone()
    {
        using var root = new TempRoot();
        root.Write("skills/productivity/hand/SKILL.md", "# Hand\n\n<!-- mentions built from the model; edit there -->\n");

        Assert.Equal(0, root.Run(DemoModel.With()).Exit);

        Assert.Equal("# Hand\n\n<!-- mentions built from the model; edit there -->\n", File.ReadAllText(root["skills/productivity/hand/SKILL.md"]));
    }

    [Fact]
    [Trait("Job", "trap: K5 a hand-written file at a built path stops the build before it writes anything (B12)")]
    public void Run_WithAHandWrittenFileAtABuiltPath_FailsAndWritesNothing()
    {
        using var root = new TempRoot();
        root.Write(Spine, "# Hand-written\n");
        root.Write("skills/roles/crew/gone/SKILL.md", BuiltHeader.Markdown + "\n");

        var (exit, _, error) = root.Run(DemoModel.With());

        Assert.Equal(1, exit);
        Assert.Equal($"SkillBuild failed: {Spine} has no built header; the build never overwrites a hand-written file\n", error.ReplaceLineEndings("\n"));
        Assert.Equal(["skills/roles/crew/demo/SKILL.md", "skills/roles/crew/gone/SKILL.md"], root.Files().Keys.Order(StringComparer.Ordinal));
        Assert.Equal("# Hand-written\n", File.ReadAllText(root[Spine]));
    }

    [Fact]
    [Trait("Job", "contract: DYD-1439 a hand-written skill's folder is rendered and never written: no file in it is written, overwritten or deleted, with the header or without it")]
    public void Run_LeavesAHandWrittenSkillsFolderAlone()
    {
        using var root = new TempRoot();
        var model = new Model([typeof(DemoMoves)], [DemoModel.Skill() with { HandWritten = true }]);
        root.Write(Spine, "# Demo, by hand\n");
        root.Write("skills/roles/crew/demo/resources/stale.md", BuiltHeader.Markdown + "\n\n# Stale\n");

        Assert.Equal((0, "", ""), root.Run(model));

        Assert.Equal(new Dictionary<string, string>
        {
            [Spine] = "# Demo, by hand\n",
            ["skills/roles/crew/demo/resources/stale.md"] = BuiltHeader.Markdown + "\n\n# Stale\n"
        }, root.Files().Where(file => file.Key.StartsWith("skills/roles/crew/demo/", StringComparison.Ordinal)));
        Assert.Contains(Spine, SkillBuilder.Build(model).Unwritten.Keys);
    }

    [Fact]
    [Trait("Job", "trap: a model the rules reject exits 1 with the reason and writes nothing, so it never reaches the tree")]
    public void Run_WithAModelTheRulesReject_ExitsOneWithTheReason()
    {
        using var root = new TempRoot();

        var (exit, _, error) = root.Run(DemoModel.With(typeof(DemoClash)));

        Assert.Equal(1, exit);
        Assert.StartsWith($"SkillBuild failed: demo: claim:{nameof(DemoClash)}.{nameof(DemoClash.SameOrder)} takes order 1", error);
        Assert.Empty(root.Files());
    }

    [Fact]
    [Trait("Job", "contract: K23 `-- map` prints one line per claim-stating built line and writes nothing (D19)")]
    public void Map_PrintsPathLineAndClaimIdsAndWritesNothing()
    {
        using var root = new TempRoot();

        var (exit, output, _) = root.Run(DemoModel.With(), "map");

        Assert.Equal(0, exit);
        Assert.Empty(root.Files());
        var lines = output.ReplaceLineEndings("\n").Split('\n', StringSplitOptions.RemoveEmptyEntries);
        Assert.Equal(SkillBuilder.Build(DemoModel.With()).Map.Select(line => line.ToString()), lines);
        Assert.Contains($"skills/roles/crew/demo/resources/stale-lease.md:5 claim:{nameof(DemoMoves)}.{nameof(DemoMoves.SaveTheWork)} " +
                        $"claim:{nameof(DemoMoves)}.{nameof(DemoMoves.ResumeAfterSaving)}", lines);
    }

    [Theory]
    [Trait("Job", "trap: a mistyped argument prints usage and writes nothing, never a silent build or map")]
    [InlineData("build")]
    [InlineData("map", "extra")]
    public void Run_WithUnknownArguments_PrintsUsage(params string[] args)
    {
        using var root = new TempRoot();

        var (exit, output, error) = root.Run(DemoModel.With(), args);

        Assert.Equal((2, "", "usage: dotnet run --project tools/SkillBuild [-- map]\n"), (exit, output, error.ReplaceLineEndings("\n")));
        Assert.Empty(root.Files());
    }
}
