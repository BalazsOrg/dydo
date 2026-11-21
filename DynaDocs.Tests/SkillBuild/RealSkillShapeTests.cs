namespace DynaDocs.Tests.SkillBuild;

using static DynaDocs.Tests.SkillBuild.SkillShapeTests;

/// <summary>
/// K10 and K9 over the real committed role skills, as they stand on disk, and DYD-1372 A5's shape of a
/// truth-checked spine: its phases hold paragraphs and lists, and no list is numbered 1. 1. (DYD-1295
/// 519126ab). <see cref="SkillShapeTests"/> keeps the limits on synthetic trees.
/// </summary>
public class RealSkillShapeTests
{
    private const string RealCaptain = "skills/roles/officers/ticket-captain/SKILL.md";
    private const string RealCoordinator = "skills/roles/officers/coordinator/SKILL.md";

    // The synthetic role's spine, as a truth-checked skill's model would declare it.
    private static readonly HashSet<string> Shaped = new([RoleSpine], StringComparer.Ordinal);

    private static readonly IReadOnlySet<string> RealScenarios = SkillShape.Scenarios(DynaDocs.SkillBuild.ProductionModel.Model);

    private static readonly IReadOnlySet<string> RealShaped = SkillShape.TruthChecked(DynaDocs.SkillBuild.ProductionModel.Model);

    // The committed built files are checked as they stand on disk, so a hand edit that breaks a limit fails here.
    [Fact]
    [Trait("Job", "contract: K10 shape tests over the real committed role skills and their resources (B15)")]
    public void RealBuiltFiles_MeetEveryShapeLimit()
    {
        var tree = CommittedTree.BuiltText();

        // Never vacuous: the real tree holds the seven role spines, one of them the truth-checked ticket captain, a
        // scenario and a template (DYD-1082, DYD-1284, DYD-1372). No real resource links another since DYD-1372 took the
        // legacy ticket captain's council briefs out of the build; the synthetic trees below keep that rule red.
        Assert.Equal(7, RoleSpines(tree).Count());
        Assert.Equal([RealCaptain], RealShaped);
        Assert.Contains("skills/roles/officers/coordinator/resources/re-cut.md", RealScenarios);
        Assert.Contains("skills/roles/officers/coordinator/resources/ticket-captain-brief.md", tree.Keys);
        var violations = SkillShape.Violations(tree).Concat(SkillShape.RoleSpineViolations(tree, RealScenarios, RealShaped)).ToArray();
        Assert.True(violations.Length == 0, string.Join('\n', violations));
    }

    // Codex's probe (DYD-1294, review r1 B7): the heading deleted, a real spine's scenario links fall under its last phase.
    // The coordinator's spine since DYD-1372, when the ticket captain's legacy scenarios left the build.
    [Fact]
    [Trait("Job", "trap: K9 B7 the real coordinator spine without its When things go wrong heading fails, though its scenario links remain")]
    public void RealSpine_WithoutItsWhenThingsGoWrongHeading_Fails()
    {
        var tree = CommittedTree.BuiltText().ToDictionary(StringComparer.Ordinal);
        tree[RealCoordinator] = tree[RealCoordinator].Replace(SkillShape.WhenThingsGoWrong + "\n", "", StringComparison.Ordinal);

        var violations = SkillShape.RoleSpineViolations(tree, RealScenarios, RealShaped);

        Assert.Contains($"{RealCoordinator}: no {Wrong} (K9)", violations);
        Assert.Contains($"{RealCoordinator}: links the scenario skills/roles/officers/coordinator/resources/re-cut.md outside {Wrong} (K9)", violations);
    }

    [Fact]
    [Trait("Job", "edge: DYD-1372 A5 a truth-checked spine's phases hold paragraphs, bold lead-ins, bullets, steps 1 2 3 and a table")]
    public void TruthCheckedSpine_WithShapedPhases_Passes() =>
        Assert.Empty(SkillShape.RoleSpineViolations(Tree((RoleSpine, Role(phases: ShapedPhases)), (RoleScenario, "# A\n")), Scenario, Shaped));

    [Fact]
    [Trait("Job", "edge: DYD-1372 A5 the same phases in a legacy spine still break the checklist rule")]
    public void LegacySpine_WithShapedPhases_Fails() =>
        Assert.Contains($"{RoleSpine}: \"## 1. Start\" holds \"**Briefs.**\"; a phase is one numbered checklist, then its resource links (K9)",
            SkillShape.RoleSpineViolations(Tree((RoleSpine, Role(phases: ShapedPhases)), (RoleScenario, "# A\n")), Scenario));

    [Theory]
    [Trait("Job", "trap: DYD-1295 519126ab a truth-checked spine or resource that numbers a list 1. 1. fails")]
    [InlineData(RoleSpine)]
    [InlineData(RoleScenario)]
    public void TruthCheckedFile_NumberedOneOne_Fails(string lazy)
    {
        var tree = Tree((RoleSpine, Role(phases: ShapedPhases)), (RoleScenario, "# A\n"));
        tree[lazy] += "\n1. First.\n1. Second.\n";

        Assert.Equal(new[] { $"{lazy}:{tree[lazy].Count(character => character == '\n')} numbers a list item 1 after 1; a list is numbered 1, 2, 3" },
            SkillShape.RoleSpineViolations(tree, Scenario, Shaped));
    }

    [Fact]
    [Trait("Job", "edge: DYD-1372 A5 r1 N8 the shape check reads a list as the build does: an example in a fenced block is no list")]
    public void TruthCheckedFile_WithNumbersInAFencedBlock_Passes()
    {
        var tree = Tree((RoleSpine, Role(phases: ShapedPhases)), (RoleScenario, "# A\n\n```text\n1. First.\n1. Second.\n```\n"));

        Assert.Empty(SkillShape.RoleSpineViolations(tree, Scenario, Shaped));
    }

    [Fact]
    [Trait("Job", "trap: K9 a truth-checked spine still needs a numbered job phase and its When things go wrong")]
    public void TruthCheckedSpine_OfOnlyAJobParagraph_Fails() =>
        Assert.Equal(new[] { $"{RoleSpine}: no numbered job phase (K9)", $"{RoleSpine}: no {Wrong} (K9)" },
            SkillShape.RoleSpineViolations(Tree((RoleSpine, Frontmatter + DynaDocs.SkillBuild.BuiltHeader.Markdown + "\n\n# Demo\n\nYou hold a ticket.\n")), new HashSet<string>(), Shaped));

    // Codex's probe (DYD-1294, review r1 B7): every role spine cut down to one job sentence, its resources left in place.
    [Fact]
    [Trait("Job", "trap: K9 B7 every real role spine cut to its job paragraph fails: no phase, and no When things go wrong")]
    public void EveryRealSpine_CutToItsJobParagraph_Fails()
    {
        var tree = CommittedTree.BuiltText().ToDictionary(StringComparer.Ordinal);
        var spines = RoleSpines(tree).ToArray();
        foreach (var spine in spines) tree[spine] = Frontmatter + DynaDocs.SkillBuild.BuiltHeader.Markdown + "\n\n# Demo\n\nYou do your job.\n";

        var violations = SkillShape.RoleSpineViolations(tree, RealScenarios, RealShaped);

        Assert.All(spines, spine => Assert.Contains($"{spine}: no numbered job phase (K9)", violations));
        Assert.All(spines, spine => Assert.Contains($"{spine}: no {Wrong} (K9)", violations));
    }

    // Review r2 B7: K9 requires the section on every role spine; research and scout declare no scenario.
    [Theory]
    [Trait("Job", "trap: K9 B7 the real research or scout spine without its When things go wrong section fails")]
    [InlineData("skills/roles/crew/research/SKILL.md")]
    [InlineData("skills/roles/crew/scout/SKILL.md")]
    public void RealSpineWithoutScenarios_WithoutItsWhenThingsGoWrongSection_Fails(string spine)
    {
        var tree = CommittedTree.BuiltText().ToDictionary(StringComparer.Ordinal);
        Assert.DoesNotContain(RealScenarios, path => path.StartsWith(spine[..^"SKILL.md".Length], StringComparison.Ordinal));
        tree[spine] = tree[spine][..tree[spine].IndexOf(SkillShape.WhenThingsGoWrong, StringComparison.Ordinal)];

        Assert.Equal(new[] { $"{spine}: no {Wrong} (K9)" }, SkillShape.RoleSpineViolations(tree, RealScenarios, RealShaped));
    }

    // A truth-checked phase as the builder lays it out: a paragraph under a lead-in, bullets, real steps, a table.
    private const string ShapedPhases =
        "## 1. Start\n\n**Briefs.**\nBrief from the template.\n\n- Seat a reviewer.\n\n**Then:**\n\n1. Begin.\n2. Return.\n\n| Kind | Resource |\n| -- | -- |\n| Change | Guide |\n";
}
