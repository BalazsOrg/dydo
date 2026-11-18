namespace DynaDocs.Tests.Steps;

using System.Diagnostics;
using System.Text.RegularExpressions;

public sealed partial class CanonicalSkillAssertionTests
{
    [Fact]
    public void ThirdPartyNotices_AdaptedInPathsExistOnDisk()
    {
        var root = RepositoryRoot();
        foreach (var relative in new[] { "THIRD-PARTY-NOTICES.md", "npm/THIRD-PARTY-NOTICES.md" })
        {
            var body = File.ReadAllText(Path.Combine(root, relative));
            var referencedPaths = Regex.Matches(body, "`([^`]+)`")
                .Select(match => match.Groups[1].Value)
                .Where(path => path.Contains('/') && !path.Contains('<') && !path.Contains('>')
                    && Regex.IsMatch(path, @"\.[A-Za-z0-9]+$"))
                .Distinct(StringComparer.Ordinal)
                .ToArray();
            Assert.NotEmpty(referencedPaths);
            foreach (var referencedPath in referencedPaths)
            {
                var target = Path.Combine(root, referencedPath.Replace('/', Path.DirectorySeparatorChar));
                Assert.True(File.Exists(target), $"{relative}: missing referenced path {referencedPath}");
            }
        }
    }

    [Theory]
    [InlineData("roles")]
    [InlineData("engineering")]
    [InlineData("productivity")]
    public void CategoryReadme_ListsEveryLocalSkillOnceWithItsInvocationMode(string category)
    {
        var root = RepositoryRoot();
        var categoryRoot = Path.Combine(root, "skills", category);
        var actualSkills = CanonicalSkillSteps.CanonicalSkills(Path.Combine(root, "skills"))
            .Where(skill => skill.Category == category || skill.Category.StartsWith($"{category}/", StringComparison.Ordinal))
            .OrderBy(skill => skill.Name, StringComparer.Ordinal)
            .ToArray();
        Assert.NotEmpty(actualSkills);

        var readme = File.ReadAllText(Path.Combine(categoryRoot, "README.md"));
        var listed = Regex.Matches(readme, @"(?m)^- \*\*([a-z0-9-]+)\*\* \((user-invoked|model-invoked)\): (.+)$")
            .Select(match => (Name: match.Groups[1].Value, Mode: match.Groups[2].Value, Description: match.Groups[3].Value))
            .ToArray();

        Assert.Equal(actualSkills.Length, listed.Length);
        Assert.Equal(actualSkills.Select(skill => skill.Name), listed.Select(entry => entry.Name).Order(StringComparer.Ordinal));
        var headings = HeadingOfEachListedSkill(readme);

        foreach (var (name, skillCategory, directory) in actualSkills)
        {
            var matches = listed.Where(entry => entry.Name == name).ToArray();
            Assert.True(matches.Length == 1, $"{category}/README.md must list {name} exactly once");
            if (skillCategory != category)
            {
                var subcategory = skillCategory[(category.Length + 1)..];
                Assert.True(headings.GetValueOrDefault(name) == char.ToUpperInvariant(subcategory[0]) + subcategory[1..],
                    $"{category}/README.md must list {name} under the heading of its {subcategory}/ folder");
            }

            var body = File.ReadAllText(Path.Combine(directory, "SKILL.md"));
            var expectDisabled = Regex.IsMatch(body, @"(?m)^disable-model-invocation:\s*true$");
            var expectedMode = expectDisabled ? "user-invoked" : "model-invoked";
            Assert.Equal(expectedMode, matches[0].Mode);

            var frontmatterDescription = DecodeYamlScalar(Regex.Match(body, @"(?m)^description:\s*(.+)$").Groups[1].Value);
            Assert.False(string.IsNullOrEmpty(frontmatterDescription), $"{name} has no frontmatter description to compare against");
            Assert.Equal(frontmatterDescription, matches[0].Description);
        }
    }

    [Fact]
    public void EveryCanonicalSkill_PairsItsClaudeInvocationFrontmatterWithCodexOpenAiYaml()
    {
        var mismatches = new List<string>();
        foreach (var (name, _, directory) in CanonicalSkillSteps.CanonicalSkills(Path.Combine(RepositoryRoot(), "skills")))
        {
            var body = File.ReadAllText(Path.Combine(directory, "SKILL.md")).ReplaceLineEndings("\n");
            var frontmatter = Regex.Match(body, @"\A---\n(.*?)\n---\n", RegexOptions.Singleline).Groups[1].Value;
            var explicitOnly = Regex.IsMatch(frontmatter, @"(?m)^disable-model-invocation:\s*true\s*$");
            var hint = Regex.Match(frontmatter, @"(?m)^argument-hint:\s*(.+?)\s*$");

            var yamlPath = Path.Combine(directory, "agents", "openai.yaml");
            if (!File.Exists(yamlPath))
            {
                if (explicitOnly || hint.Success)
                    mismatches.Add($"{name}: SKILL.md sets Claude invocation metadata but agents/openai.yaml is missing");
                continue;
            }

            var codex = NestedYamlValues(File.ReadAllText(yamlPath));
            if (explicitOnly != (codex.GetValueOrDefault("policy.allow_implicit_invocation") == "false"))
                mismatches.Add($"{name}: disable-model-invocation: true and policy.allow_implicit_invocation: false must appear together");

            var prompt = codex.GetValueOrDefault("interface.default_prompt");
            if (hint.Success != (prompt is not null)
                || hint.Success && DecodeYamlScalar(hint.Groups[1].Value) != DecodeYamlScalar(prompt!))
                mismatches.Add($"{name}: argument-hint and interface.default_prompt must appear together with the same text");
        }

        Assert.True(mismatches.Count == 0, string.Join("\n", mismatches));
    }

    private static Dictionary<string, string> NestedYamlValues(string yaml)
    {
        var values = new Dictionary<string, string>(StringComparer.Ordinal);
        string? parent = null;
        foreach (var line in yaml.ReplaceLineEndings("\n").Split('\n'))
        {
            var top = Regex.Match(line, @"^([A-Za-z_][\w-]*):\s*$");
            if (top.Success) { parent = top.Groups[1].Value; continue; }
            var child = Regex.Match(line, @"^[ \t]+([A-Za-z_][\w-]*):\s*(.+?)\s*$");
            if (child.Success && parent is not null) values[$"{parent}.{child.Groups[1].Value}"] = child.Groups[2].Value;
            else if (line.Length > 0 && !char.IsWhiteSpace(line[0])) parent = null;
        }
        return values;
    }

    private static Dictionary<string, string> HeadingOfEachListedSkill(string readme)
    {
        var headings = new Dictionary<string, string>(StringComparer.Ordinal);
        string? heading = null;
        foreach (var line in readme.ReplaceLineEndings("\n").Split('\n'))
        {
            if (line.StartsWith("## ", StringComparison.Ordinal)) heading = line[3..].Trim();
            var entry = Regex.Match(line, @"^- \*\*([a-z0-9-]+)\*\*");
            if (entry.Success && heading is not null) headings[entry.Groups[1].Value] = heading;
        }
        return headings;
    }

    private static string DecodeYamlScalar(string raw)
    {
        var trimmed = raw.Trim();
        return trimmed.Length >= 2 && trimmed[0] == '"' && trimmed[^1] == '"'
            ? trimmed[1..^1]
            : trimmed;
    }

    [Fact]
    public void SkillsReadme_CategoryCountsMatchTheRealDirectoriesAndSumTo41()
    {
        var root = RepositoryRoot();
        var skillsRoot = Path.Combine(root, "skills");
        var categories = new[] { "roles", "engineering", "productivity" };
        var actualCounts = CanonicalSkillSteps.CanonicalSkills(skillsRoot)
            .GroupBy(skill => skill.Category.Split('/')[0])
            .ToDictionary(group => group.Key, group => group.Count());
        Assert.Equal(categories.Order(StringComparer.Ordinal), actualCounts.Keys.Order(StringComparer.Ordinal));
        Assert.Equal(41, actualCounts.Values.Sum());

        var readme = File.ReadAllText(Path.Combine(skillsRoot, "README.md"));
        foreach (var category in categories)
        {
            var match = Regex.Match(readme, $@"`{category}/`\]\({category}/README\.md\) - (\d+) skills\.");
            Assert.True(match.Success, $"skills/README.md does not cite a skill count for {category}/");
            Assert.Equal(actualCounts[category], int.Parse(match.Groups[1].Value));
        }

        Assert.Contains(
            $"{actualCounts["roles"]} + {actualCounts["engineering"]} + {actualCounts["productivity"]} = 41",
            readme);
    }

    [Theory]
    [InlineData("see `skills/reviewer/SKILL.md` for the shape", "skills/reviewer")]
    [InlineData("edit `skills/teach/resources/mission-format.md` directly", "skills/teach")]
    [InlineData("authored at `skills/admiral`", "skills/admiral")]
    [InlineData("canonical folder is `skills/orchestration/reviewer/SKILL.md`", "skills/orchestration/reviewer")]
    [InlineData("see `skills/engineering/code-writer/` for the method", "skills/engineering/code-writer")]
    [InlineData("see `skills/roles/scout/`", "skills/roles/scout")]
    public void StaleSkillPaths_ReportAFlatOrFormerCategoryReference(string content, string stale) =>
        Assert.Equal([stale], CanonicalSkillSteps.StaleSkillPaths(RepositoryRoot(), content));

    [Theory]
    [InlineData("canonical folder is `skills/roles/crew/reviewer/SKILL.md`")]
    [InlineData("edit `skills/productivity/teach/resources/mission-format.md` directly")]
    [InlineData("authored at `skills/roles/officers/admiral`")]
    [InlineData("see `skills/roles/officers/` and `skills/engineering/` for the categories")]
    [InlineData("projected at `.claude/skills/reviewer/` and `.agents/skills/reviewer/`")]
    [InlineData("nothing here names a skills/<category>/<name>/ literal")]
    public void StaleSkillPaths_IgnoreACanonicalCategoryOrHostProjectionReference(string content) =>
        Assert.Empty(CanonicalSkillSteps.StaleSkillPaths(RepositoryRoot(), content));

    private static string RepositoryRoot()
    {
        for (var directory = new DirectoryInfo(AppContext.BaseDirectory); directory is not null; directory = directory.Parent)
            if (File.Exists(Path.Combine(directory.FullName, "DynaDocs.csproj"))) return directory.FullName;
        throw new DirectoryNotFoundException($"Could not locate repository root from {AppContext.BaseDirectory}");
    }
}
