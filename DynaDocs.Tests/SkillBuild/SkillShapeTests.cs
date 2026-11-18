namespace DynaDocs.Tests.SkillBuild;

/// <summary>
/// Grilling A D7 and D8: the skill-shape limits, each red on a synthetic tree and green on the committed
/// built tree. Source: Anthropic's skill authoring best practices (<see cref="SkillShape.BestPractices"/>)
/// for the line limits, links and contents list.
/// </summary>
[Trait("Job", "contract: DR 064 §9 K10")]
public class SkillShapeTests
{
    private const string Spine = "skills/x/demo/SKILL.md";
    private const string Resource = "skills/x/demo/resources/a.md";
    private const string Frontmatter = "---\nname: demo\ndescription: d\n---\n";
    private const string LinksA = "- [A](resources/a.md)\n";

    [Fact]
    public void CommittedBuiltTree_MeetsEveryShapeLimit()
    {
        var violations = SkillShape.Violations(CommittedTree.BuiltText());

        Assert.True(violations.Count == 0, string.Join('\n', violations));
    }

    [Fact]
    public void CommittedBuiltTree_ExercisesTheSpineAndResourceRules()
    {
        var tree = CommittedTree.BuiltText();

        // The inventory's 15 carriers: 13 skills and the 2 system docs, which are not skill folders.
        Assert.Equal(13, tree.Keys.Count(path => path.EndsWith("/SKILL.md", StringComparison.Ordinal)));
        Assert.Contains(tree, file => file.Key.Contains("/resources/", StringComparison.Ordinal)
                                      && file.Value.Count(character => character == '\n') > SkillShape.ResourceLimit);
    }

    // "Keep the SKILL.md body under 500 lines for optimal performance."
    [Theory]
    [InlineData(SkillShape.SpineBodyLimit, false)]
    [InlineData(SkillShape.SpineBodyLimit + 1, true)]
    public void SpineBody_OverFiveHundredLines_Fails(int bodyLines, bool fails)
    {
        var violations = SkillShape.Violations(Tree((Spine, Frontmatter + Lines(bodyLines))));

        Assert.Equal(fails, violations.Any(violation => violation.Contains("SKILL.md body is over 500 lines")));
    }

    // "For reference files longer than 100 lines, include a table of contents at the top."
    [Theory]
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
    public void Resource_NoSpineLinksTo_Fails() =>
        Assert.Equal(new[] { $"{Resource}: no spine links to this resource ({SkillShape.BestPractices})" },
            SkillShape.Violations(Tree((Spine, Frontmatter + "- [Elsewhere](../other/resources/a.md)\n"), (Resource, "# A\n"))));

    private static string Lines(int count) => string.Concat(Enumerable.Repeat("- line\n", count));

    private static Dictionary<string, string> Tree(params (string Path, string Text)[] files) =>
        files.ToDictionary(file => file.Path, file => file.Text, StringComparer.Ordinal);
}
