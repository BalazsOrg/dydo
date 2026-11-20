namespace DynaDocs.Tests.SkillBuild;

/// <summary>
/// K10 (Grilling A D7 and D8): the skill-shape limits, each red on a synthetic tree and green on the
/// real build and the migration fixture's build. Sources: Anthropic's skill authoring best practices
/// (<see cref="SkillShape.BestPractices"/>) for the line limits, links and contents list; DYD-968 and
/// K26 for what a skill folder holds.
/// </summary>
public class SkillShapeTests
{
    private const string Spine = "skills/x/demo/SKILL.md";
    private const string Resource = "skills/x/demo/resources/a.md";
    private const string Frontmatter = "---\nname: demo\ndescription: d\n---\n";
    private const string LinksA = "- [A](resources/a.md)\n";
    private const string RoleSpine = "skills/roles/crew/demo/SKILL.md";
    private const string RoleScenario = "skills/roles/crew/demo/resources/stuck.md";
    private const string RoleTemplate = "skills/roles/crew/demo/resources/brief.md";
    private const string Wrong = "\"" + SkillShape.WhenThingsGoWrong + "\"";
    private const string RealCaptain = "skills/roles/officers/ticket-captain/SKILL.md";

    // The synthetic role's scenario, as its model would declare it.
    private static readonly HashSet<string> Scenario = new([RoleScenario], StringComparer.Ordinal);

    private static readonly IReadOnlySet<string> RealScenarios = SkillShape.Scenarios(DynaDocs.SkillBuild.ProductionModel.Model);

    // The committed built files are checked as they stand on disk, so a hand edit that breaks a limit fails here.
    [Fact]
    [Trait("Job", "contract: K10 shape tests over the real committed role skills and their resources (B15)")]
    public void RealBuiltFiles_MeetEveryShapeLimit()
    {
        var tree = CommittedTree.BuiltText();

        // Never vacuous: the real tree holds the seven role spines, a scenario, a template, and a resource another
        // resource links, the council chair's members' briefs (DYD-1082, DYD-1284, DYD-1286).
        Assert.Equal(7, RoleSpines(tree).Count());
        Assert.Contains("skills/roles/officers/ticket-captain/resources/council-members.md", tree.Keys);
        Assert.Contains("skills/roles/officers/ticket-captain/resources/fix-rounds.md", RealScenarios);
        Assert.Contains("skills/roles/officers/ticket-captain/resources/implementer-brief.md", tree.Keys);
        var violations = SkillShape.Violations(tree).Concat(SkillShape.RoleSpineViolations(tree, RealScenarios)).ToArray();
        Assert.True(violations.Length == 0, string.Join('\n', violations));
    }

    // Codex's probe (DYD-1294, review r1 B7): the heading deleted, the real ticket captain's scenario links fall under its last phase.
    [Fact]
    [Trait("Job", "trap: K9 B7 the real ticket-captain spine without its When things go wrong heading fails, though its scenario links remain")]
    public void RealSpine_WithoutItsWhenThingsGoWrongHeading_Fails()
    {
        var tree = CommittedTree.BuiltText().ToDictionary(StringComparer.Ordinal);
        tree[RealCaptain] = tree[RealCaptain].Replace(SkillShape.WhenThingsGoWrong + "\n", "", StringComparison.Ordinal);

        var violations = SkillShape.RoleSpineViolations(tree, RealScenarios);

        Assert.Contains($"{RealCaptain}: no {Wrong} to link its scenarios (K9)", violations);
        Assert.Contains($"{RealCaptain}: links the scenario skills/roles/officers/ticket-captain/resources/fix-rounds.md outside {Wrong} (K9)", violations);
    }

    // Codex's probe (DYD-1294, review r1 B7): every role spine cut down to one job sentence, its resources left in place.
    [Fact]
    [Trait("Job", "trap: K9 B7 every real role spine cut to its job paragraph fails: no phase, and no When things go wrong where it has scenarios")]
    public void EveryRealSpine_CutToItsJobParagraph_Fails()
    {
        var tree = CommittedTree.BuiltText().ToDictionary(StringComparer.Ordinal);
        var spines = RoleSpines(tree).ToArray();
        foreach (var spine in spines) tree[spine] = Frontmatter + DynaDocs.SkillBuild.BuiltHeader.Markdown + "\n\n# Demo\n\nYou do your job.\n";

        var violations = SkillShape.RoleSpineViolations(tree, RealScenarios);

        var withScenarios = spines.Where(spine => RealScenarios.Any(path => path.StartsWith(spine[..^"SKILL.md".Length], StringComparison.Ordinal))).ToArray();
        Assert.True(withScenarios.Length >= 5, string.Join(", ", withScenarios));
        Assert.All(spines, spine => Assert.Contains($"{spine}: no numbered job phase (K9)", violations));
        Assert.All(withScenarios, spine => Assert.Contains($"{spine}: no {Wrong} to link its scenarios (K9)", violations));
    }

    [Fact]
    [Trait("Job", "edge: K9 a job paragraph, numbered phases and When things go wrong, each link resolving, is a role spine")]
    public void RoleSpine_InOrder_Passes() =>
        Assert.Empty(SkillShape.RoleSpineViolations(Tree((RoleSpine, Role()), (RoleScenario, "# A\n")), Scenario));

    [Fact]
    [Trait("Job", "edge: K9 a role without scenarios needs no When things go wrong")]
    public void RoleSpine_WithoutScenariosOrWhenThingsGoWrong_Passes() =>
        Assert.Empty(SkillShape.RoleSpineViolations(Tree((RoleSpine, Role(wrong: ""))), new HashSet<string>()));

    [Fact]
    [Trait("Job", "contract: K9 B7 a role spine of only its job paragraph, with no numbered phase, fails")]
    public void RoleSpine_OfOnlyAJobParagraph_Fails() =>
        Assert.Equal(new[] { $"{RoleSpine}: no numbered job phase (K9)" },
            SkillShape.RoleSpineViolations(Tree((RoleSpine, Frontmatter + DynaDocs.SkillBuild.BuiltHeader.Markdown + "\n\n# Demo\n\nYou pool and verify.\n")), new HashSet<string>()));

    [Fact]
    [Trait("Job", "contract: K9 B7 a role with a scenario and no When things go wrong fails")]
    public void RoleSpine_WithAScenarioButNoWhenThingsGoWrong_Fails() =>
        Assert.Equal(new[] { $"{RoleSpine}: no {Wrong} to link its scenarios (K9)" },
            SkillShape.RoleSpineViolations(Tree((RoleSpine, Role(wrong: "")), (RoleScenario, "# A\n")), Scenario));

    [Fact]
    [Trait("Job", "contract: K9 B7 a When things go wrong that leaves a scenario unlinked fails")]
    public void RoleSpine_WithAScenarioWhenThingsGoWrongDoesNotLink_Fails() =>
        Assert.Equal(new[] { $"{RoleSpine}: the scenario {RoleScenario} is not linked from {Wrong} (K9)" },
            SkillShape.RoleSpineViolations(Tree((RoleSpine, Role(wrong: SkillShape.WhenThingsGoWrong + "\n\n- Save the work, then escalate.\n")), (RoleScenario, "# A\n")), Scenario));

    [Theory]
    [Trait("Job", "contract: K9 B7 a scenario linked from the job paragraph or a phase, even its resource list, fails")]
    [InlineData("You do the job; see [Stuck](resources/stuck.md).\n", "## 1. Start\n\n1. Begin.\n")]
    [InlineData("You do the job.\n", "## 1. Start\n\n1. Begin.\n\nResources:\n\n- [Stuck](resources/stuck.md)\n")]
    [InlineData("You do the job.\n", "## 1. Start\n\n1. Begin; see [Stuck](resources/stuck.md).\n")]
    public void RoleSpine_LinkingAScenarioOutsideWhenThingsGoWrong_Fails(string job, string phases) =>
        Assert.Equal(new[] { $"{RoleSpine}: links the scenario {RoleScenario} outside {Wrong} (K9)" },
            SkillShape.RoleSpineViolations(Tree((RoleSpine, Role(job, phases)), (RoleScenario, "# A\n")), Scenario));

    [Fact]
    [Trait("Job", "edge: K9 a phase links its own resources, such as a dispatch template, under Resources")]
    public void RoleSpine_WithAPhaseResourceList_Passes() =>
        Assert.Empty(SkillShape.RoleSpineViolations(
            Tree((RoleSpine, Role(phases: "## 1. Start\n\n1. Begin.\n1. Brief the crew.\n\nResources:\n\n- [Crew brief](resources/brief.md)\n")),
                (RoleScenario, "# A\n"), (RoleTemplate, "# Crew brief\n")),
            Scenario));

    // S5 (review r1): K9 supersedes DR 064 §7's state outline as the spine's grouping; a phase is one numbered checklist.
    [Theory]
    [Trait("Job", "contract: K9 S5 a phase holding a state heading, a move label, a plain bullet or a paragraph fails")]
    [InlineData("### Done")]
    [InlineData("Arrivals:")]
    [InlineData("Nominal path:")]
    [InlineData("Exits:")]
    [InlineData("- Return.")]
    [InlineData("Return to your officer.")]
    public void RoleSpine_WithAPhaseThatIsNotOneNumberedChecklist_Fails(string line) =>
        Assert.Equal(new[] { $"{RoleSpine}: \"## 2. Finish\" holds \"{line}\"; a phase is one numbered checklist, then its resource links (K9)" },
            SkillShape.RoleSpineViolations(Tree((RoleSpine, Role(phases: $"## 1. Start\n\n1. Begin.\n\n## 2. Finish\n\n{line}\n\n1. Return.\n")), (RoleScenario, "# A\n")), Scenario));

    [Theory]
    [Trait("Job", "contract: K9 a role spine without its opening job paragraph, or with two, fails")]
    [InlineData("", "no opening job paragraph")]
    [InlineData("You do the job.\n\nAnd a second paragraph.\n", "the opening is not one job paragraph")]
    [InlineData("- A bullet, not a paragraph.\n", "the opening is not one job paragraph")]
    [InlineData("### A heading, not a paragraph\n", "the opening is not one job paragraph")]
    public void RoleSpine_WithoutOneJobParagraph_Fails(string job, string reason) =>
        Assert.Contains(SkillShape.RoleSpineViolations(Tree((RoleSpine, Role(job: job)), (RoleScenario, "# A\n")), Scenario),
            violation => violation.Contains(reason, StringComparison.Ordinal));

    [Theory]
    [Trait("Job", "contract: K9 an unnumbered, out-of-order or missing phase, or When things go wrong out of place, fails")]
    [InlineData("## Steps\n", "\"## Steps\" is neither a numbered job phase")]
    [InlineData("## 1. Start\n\n## 3. Finish\n", "\"## 3. Finish\" where phase 2 belongs")]
    [InlineData("## 2. Finish\n\n## 1. Start\n", "\"## 2. Finish\" where phase 1 belongs")]
    [InlineData("## 1 Start\n", "\"## 1 Start\" is neither a numbered job phase")]
    [InlineData("## When things go wrong\n\n## 1. Start\n", "\"## 1. Start\" comes after \"## When things go wrong\"")]
    [InlineData("## 1. Start\n\n## When things go wrong\n", "\"## When things go wrong\" twice")]
    public void RoleSpine_WithAPhaseOutOfShape_Fails(string phases, string reason) =>
        Assert.Contains(SkillShape.RoleSpineViolations(Tree((RoleSpine, Role(phases: phases)), (RoleScenario, "# A\n")), Scenario),
            violation => violation.Contains(reason, StringComparison.Ordinal));

    [Fact]
    [Trait("Job", "contract: K9 a scenario link that resolves to no file fails")]
    public void RoleSpine_WithADanglingScenarioLink_Fails() =>
        Assert.Equal(new[] { $"{RoleSpine}: links {RoleScenario}, which the tree does not hold" },
            SkillShape.RoleSpineViolations(Tree((RoleSpine, Role())), Scenario));

    [Theory]
    [Trait("Job", "contract: K9 a role resource of 100 lines or more fails")]
    [InlineData(SkillShape.RoleResourceLimit - 1, false)]
    [InlineData(SkillShape.RoleResourceLimit, true)]
    public void RoleResource_OfOneHundredLines_Fails(int lines, bool fails) =>
        Assert.Equal(fails, SkillShape.RoleSpineViolations(Tree((RoleSpine, Role()), (RoleScenario, Lines(lines))), Scenario)
            .Contains($"{RoleScenario}: {lines} lines; a role resource stays under {SkillShape.RoleResourceLimit} lines (K9)"));

    [Fact]
    [Trait("Job", "contract: K10 shape tests over the fixture build (B15, C17)")]
    public void MigrationFixtureBuild_MeetsEveryShapeLimit()
    {
        var violations = SkillShape.Violations(Migration.Tree.Files);

        Assert.True(violations.Count == 0, string.Join('\n', violations));
    }

    [Fact]
    [Trait("Job", "trap: the fixture shape test is vacuous unless the build holds 13 spines and a resource over 100 lines (B15)")]
    public void MigrationFixtureBuild_ExercisesTheSpineAndResourceRules()
    {
        var tree = Migration.Tree.Files;

        // The inventory's 15 carriers: 13 skills and the 2 standards, which are not skill folders.
        Assert.Equal(13, tree.Keys.Count(path => path.EndsWith("/SKILL.md", StringComparison.Ordinal)));
        Assert.Contains(tree, file => file.Key.Contains("/resources/", StringComparison.Ordinal)
                                      && file.Value.Count(character => character == '\n') > SkillShape.ResourceLimit);
    }

    // "Keep the SKILL.md body under 500 lines for optimal performance."
    [Theory]
    [Trait("Job", "contract: K10/D7 a SKILL.md body over 500 lines fails")]
    [InlineData(SkillShape.SpineBodyLimit, false)]
    [InlineData(SkillShape.SpineBodyLimit + 1, true)]
    public void SpineBody_OverFiveHundredLines_Fails(int bodyLines, bool fails)
    {
        var violations = SkillShape.Violations(Tree((Spine, Frontmatter + Lines(bodyLines))));

        Assert.Equal(fails, violations.Any(violation => violation.Contains("SKILL.md body is over 500 lines")));
    }

    // "For reference files longer than 100 lines, include a table of contents at the top."
    [Theory]
    [Trait("Job", "contract: K10/D7 a resource over 100 lines needs a contents list at its top")]
    [InlineData(SkillShape.ResourceLimit - 2, "", false)]
    [InlineData(SkillShape.ResourceLimit - 1, "", true)]
    [InlineData(SkillShape.ResourceLimit - 1, "## Contents\n\n", false)]
    [InlineData(SkillShape.ResourceLimit - 1, "## Other\n\n", true)]
    public void Resource_OverOneHundredLinesWithoutAContentsList_Fails(int bodyLines, string section, bool fails)
    {
        var resource = "# A\n\n" + section + Lines(bodyLines);
        var violations = SkillShape.Violations(Tree((Spine, Frontmatter + LinksA), (Resource, resource)));

        Assert.Equal(fails, violations.Any(violation => violation.Contains("has no contents list")));
    }

    [Theory]
    [Trait("Job", "trap: K10 a Contents heading with no entries is not a contents list")]
    [InlineData("")]
    [InlineData("No entries.\n")]
    [InlineData("## Later\n- A later list item\n")]
    [InlineData("# Later\n- A later list item\n")]
    public void Resource_OverOneHundredLinesWithEmptyContents_Fails(string afterContents)
    {
        var resource = "# A\n\n## Contents\n\n" + afterContents
            + string.Concat(Enumerable.Repeat("Body text.\n", SkillShape.ResourceLimit + 1));

        Assert.Equal(new[] { $"{Resource}: a resource over 100 lines has no contents list ({SkillShape.BestPractices})" },
            SkillShape.Violations(Tree((Spine, Frontmatter + LinksA), (Resource, resource))));
    }

    // "Claude might partially read files when they're referenced from other referenced files ... `head -100`";
    // D8 keeps a resource reached through another resource short enough for that preview to read whole.
    [Theory]
    [Trait("Job", "contract: K10/D8 a resource reached from another resource stays within 100 lines")]
    [InlineData(SkillShape.ResourceLimit, false)]
    [InlineData(SkillShape.ResourceLimit + 1, true)]
    public void Resource_ReachedFromAnotherResource_OverOneHundredLines_Fails(int targetLines, bool fails)
    {
        var target = "## Contents\n\n" + Lines(targetLines - 2);
        var violations = SkillShape.Violations(Tree(
            (Spine, Frontmatter + LinksA + "- [B](resources/b.md#top)\n"),
            (Resource, "See [B](./b.md), [the spine](../SKILL.md) and [the web](https://example.com/x.md).\n"),
            ("skills/x/demo/resources/b.md", target)));

        Assert.Equal(fails ? new[] { "skills/x/demo/resources/b.md: reached from the resource skills/x/demo/resources/a.md but over 100 lines" } : [],
            violations.Select(violation => violation.Split(" (")[0]));
    }

    // "Keep references one level deep from SKILL.md. All reference files should link directly from SKILL.md."
    [Fact]
    [Trait("Job", "contract: K10/D7 every resource is linked from its spine, one level deep")]
    public void Resource_NoSpineLinksTo_Fails() =>
        Assert.Equal(new[] { $"{Resource}: no spine links to this resource ({SkillShape.BestPractices})" },
            SkillShape.Violations(Tree((Spine, Frontmatter + "- [Elsewhere](../other/resources/a.md)\n"), (Resource, "# A\n"))));

    // DYD-968: a skill folder ships its runtime files only, never dydo's own tests or fixtures.
    [Theory]
    [Trait("Job", "contract: K10/DYD-968 a skill folder holds only its runtime files")]
    [InlineData("skills/x/demo/scripts/test/check.test.mjs")]
    [InlineData("skills/x/demo/resources/fixture.json")]
    [InlineData("skills/x/demo/resources/deep/a.md")]
    [InlineData("skills/x/demo/README.md")]
    [InlineData("skills/x/demo/agents/other.yaml")]
    public void SkillFolder_HoldingAnythingButItsRuntimeFiles_Fails(string path) =>
        Assert.Equal(new[] { $"{path}: {SkillShape.FolderRule}" },
            SkillShape.Violations(Tree((Spine, Frontmatter), (path, "x\n"))));

    // K26: Codex reads agents/openai.yaml from the skill folder, so the builder emits it there.
    [Fact]
    [Trait("Job", "edge: K26 agents/openai.yaml is a runtime file of a built skill folder")]
    public void SkillFolder_HoldingItsCodexMetadata_Passes() =>
        Assert.Empty(SkillShape.Violations(Tree((Spine, Frontmatter), ("skills/x/demo/agents/openai.yaml", "policy:\n"))));

    [Fact]
    [Trait("Job", "edge: K10 the skill limits apply to skill folders only, never to a standard")]
    public void FilesOutsideSkillFolders_AreNotSkillShaped() =>
        Assert.Empty(SkillShape.Violations(Tree(("dydo/guides/g.md", Lines(900)), ("claims.map.json", "{}\n"))));

    [Theory]
    [Trait("Job", "trap: an unbalanced fence swallows the rest of a built file")]
    [InlineData("skills/x/demo/SKILL.md")]
    [InlineData("skills/x/demo/resources/a.md")]
    [InlineData("dydo/guides/g.md")]
    public void AnyBuiltFile_WithAnUnbalancedFence_Fails(string path) =>
        Assert.Contains($"{path}: unbalanced fenced block", SkillShape.Violations(Tree((path, "```text\nopen\n"))));

    private static string Lines(int count) => string.Concat(Enumerable.Repeat("- line\n", count));

    // A role spine as the builder lays it out, each part swappable.
    private static string Role(string job = "You do the job.\nYou return to your officer.\n",
        string phases = "## 1. Start\n\n1. Begin.\n\n## 2. Finish\n\n1. Return.\n",
        string wrong = SkillShape.WhenThingsGoWrong + "\n\n- [Stuck](resources/stuck.md)\n") =>
        Frontmatter + DynaDocs.SkillBuild.BuiltHeader.Markdown + "\n\n# Demo\n\n" + job + "\n" + phases + "\n" + wrong;

    private static IEnumerable<string> RoleSpines(IReadOnlyDictionary<string, string> tree) =>
        tree.Keys.Where(path => path.StartsWith("skills/roles/", StringComparison.Ordinal) && path.EndsWith("/SKILL.md", StringComparison.Ordinal));

    private static Dictionary<string, string> Tree(params (string Path, string Text)[] files) =>
        files.ToDictionary(file => file.Path, file => file.Text, StringComparer.Ordinal);
}
