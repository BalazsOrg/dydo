namespace DynaDocs.Tests.Steps;

using System.Diagnostics;
using System.Text.RegularExpressions;

public sealed partial class CanonicalSkillAssertionTests
{
    internal static readonly string[] ProjectPathGuidance =
    [
        "skills/roles/officers/admiral/SKILL.md",
        "skills/productivity/bro/SKILL.md",
        "skills/roles/officers/chief-of-staff/SKILL.md",
        "skills/productivity/co-thinker/SKILL.md",
        "skills/roles/crew/code-writer/SKILL.md",
        "skills/engineering/diagnosing-bugs/SKILL.md",
        "skills/roles/crew/docs-writer/SKILL.md",
        "skills/engineering/domain-modeling/SKILL.md",
        "skills/engineering/improve-codebase-architecture/SKILL.md",
        "skills/roles/crew/inquisitor/SKILL.md",
        "skills/roles/officers/ticket-captain/SKILL.md",
        "skills/roles/crew/research/SKILL.md",
        "skills/roles/crew/reviewer/SKILL.md",
        "skills/roles/crew/scout/SKILL.md",
        "skills/productivity/wayfinder/SKILL.md",
        "skills/productivity/to-ticket/SKILL.md",
        "skills/productivity/what-would-balazs-do/SKILL.md",
        "skills/roles/crew/reviewer/resources/code.md",
        "skills/roles/crew/reviewer/resources/docs.md",
        "skills/roles/crew/reviewer/resources/project-plan.md",
        "skills/productivity/writing-for-agents/resources/skill-mechanics.md"
    ];

    internal static readonly string[] CurrentGuidance =
    [
        "README.md",
        "THIRD-PARTY-NOTICES.md",
        "npm/README.md",
        "npm/THIRD-PARTY-NOTICES.md",
        "Scaffold/dydo/reference/about-dynadocs.md",
        "Scaffold/dydo/reference/dydo-commands.md",
        "dydo/understand/about.md",
        "dydo/understand/architecture.md",
        "dydo/understand/scaffold-and-customization.md",
        "dydo/guides/adding-a-command.md",
        "dydo/guides/customizing-roles.md",
        "dydo/guides/getting-started.md",
        "dydo/guides/orchestration-pitfalls.md",
        "dydo/guides/troubleshooting.md",
        "dydo/project/future-features/routine-admiral.md",
        "dydo/reference/about-dynadocs.md",
        "dydo/reference/configuration.md",
        "dydo/reference/dydo-commands.md"
    ];

    [Fact]
    public void CanonicalSkillTree_SortsEverySkillExactlyOnceByKind()
    {
        var expected = new Dictionary<string, string>
        {
            ["admiral"] = "roles/officers", ["ticket-captain"] = "roles/officers", ["chief-of-staff"] = "roles/officers",
            ["code-writer"] = "roles/crew", ["docs-writer"] = "roles/crew",
            ["reviewer"] = "roles/crew", ["inquisitor"] = "roles/crew", ["research"] = "roles/crew", ["scout"] = "roles/crew",
            ["codebase-design"] = "engineering", ["domain-modeling"] = "engineering", ["diagnosing-bugs"] = "engineering",
            ["pr"] = "engineering", ["prototype"] = "engineering", ["wizard"] = "engineering", ["improve-codebase-architecture"] = "engineering",
            ["create-verification-skill"] = "engineering", ["maintain-verification-skill"] = "engineering",
            ["verify-dydo"] = "engineering",
            ["statechart"] = "engineering",
            ["create-formal-verification-skill"] = "engineering", ["maintain-formal-verification-skill"] = "engineering",
            ["co-thinker"] = "productivity", ["grilling"] = "productivity", ["grill-me"] = "productivity",
            ["bro"] = "productivity", ["handoff"] = "productivity", ["teach"] = "productivity",
            ["show-me"] = "productivity", ["walkthrough"] = "productivity", ["writing-for-agents"] = "productivity",
            ["writing-for-humans"] = "productivity", ["retro"] = "productivity", ["wayfinder"] = "productivity",
            ["to-project"] = "productivity", ["to-ticket"] = "productivity", ["catch-up"] = "productivity", ["in-your-own-words"] = "productivity",
            ["overnight-mode"] = "productivity",
            ["what-would-balazs-do"] = "productivity",
            ["update-dydo"] = "productivity"
        };
        Assert.Equal(41, expected.Count);
        var root = Path.Combine(RepositoryRoot(), "skills");
        Assert.Equal(["engineering", "productivity", "roles"], ChildDirectoryNames(root));
        Assert.Equal(["crew", "officers"], ChildDirectoryNames(Path.Combine(root, "roles")));

        var skills = CanonicalSkillSteps.CanonicalSkills(root);
        Assert.Equal(
            expected.OrderBy(pair => pair.Key, StringComparer.Ordinal),
            skills.Select(skill => KeyValuePair.Create(skill.Name, skill.Category)).OrderBy(pair => pair.Key, StringComparer.Ordinal));
        foreach (var skill in skills)
        {
            var body = File.ReadAllText(Path.Combine(skill.Directory, "SKILL.md"));
            Assert.Matches(new Regex($@"(?m)^name: {Regex.Escape(skill.Name)}$"), body);
        }
    }

    [Fact]
    public void CanonicalSkills_WalkCategoriesToAnyDepthButNeverIntoASkill()
    {
        using var tree = new SkillTree();
        tree.Skill("roles/crew/reviewer");
        tree.Skill("engineering/prototype");
        Directory.CreateDirectory(Path.Combine(tree.Root, "roles", "crew", "reviewer", "resources", "nested"));

        var skills = CanonicalSkillSteps.CanonicalSkills(tree.Root);

        Assert.Equal(
            [("prototype", "engineering"), ("reviewer", "roles/crew")],
            skills.Select(skill => (skill.Name, skill.Category)).OrderBy(pair => pair.Name, StringComparer.Ordinal));
    }

    [Fact]
    public void CanonicalSkills_RefuseAnEmptyNestedCategory()
    {
        using var tree = new SkillTree();
        tree.Skill("roles/crew/reviewer");
        Directory.CreateDirectory(Path.Combine(tree.Root, "roles", "officers"));

        var error = Assert.Throws<InvalidOperationException>(() => CanonicalSkillSteps.CanonicalSkills(tree.Root));
        Assert.Contains("officers", error.Message);
    }

    [Fact]
    public void CanonicalSkills_RefuseOneNameReachedThroughTwoCategories()
    {
        using var tree = new SkillTree();
        tree.Skill("engineering/scout");
        tree.Skill("roles/crew/scout");

        var error = Assert.Throws<InvalidOperationException>(() => CanonicalSkillSteps.CanonicalSkills(tree.Root));
        Assert.Contains("Duplicate skill name across categories: scout", error.Message);
    }

    private sealed class SkillTree : IDisposable
    {
        public string Root { get; } = Directory.CreateTempSubdirectory("dyd229-skills-").FullName;

        public void Skill(string relative)
        {
            var directory = Path.Combine(Root, relative.Replace('/', Path.DirectorySeparatorChar));
            Directory.CreateDirectory(directory);
            File.WriteAllText(Path.Combine(directory, "SKILL.md"), $"---\nname: {Path.GetFileName(directory)}\n---\n");
        }

        public void Dispose() => Directory.Delete(Root, recursive: true);
    }

    private static string[] ChildDirectoryNames(string directory) =>
        Directory.EnumerateDirectories(directory).Select(child => Path.GetFileName(child)!).Order(StringComparer.Ordinal).ToArray();

    [Fact]
    public void CanonicalSkillTree_HasNoAuthoredHostCopies()
    {
        var repositoryRoot = RepositoryRoot();
        var canonicalSkills = Path.Combine(repositoryRoot, "skills");
        var trackedFiles = TrackedFilesBelow(repositoryRoot, ".claude/skills", ".agents/skills");

        foreach (var relative in new[] { ".claude/skills", ".agents/skills" })
        {
            var root = Path.Combine(repositoryRoot, relative);
            if (!Directory.Exists(root)) continue;

            foreach (var entry in Directory.EnumerateFileSystemEntries(root))
            {
                var target = new DirectoryInfo(entry).ResolveLinkTarget(returnFinalTarget: true)?.FullName;
                Assert.True(target is not null,
                    $"{Path.GetRelativePath(repositoryRoot, entry)} is a real file or directory, not a link into skills/. " +
                    "DR 049 forbids a second authored copy of a canonical skill outside the skills/ tree.");

                var resolved = Path.GetFullPath(target!);
                var insidePrefix = canonicalSkills + Path.DirectorySeparatorChar;
                Assert.True(resolved.StartsWith(insidePrefix, StringComparison.OrdinalIgnoreCase),
                    $"{Path.GetRelativePath(repositoryRoot, entry)} resolves to {resolved}, which is outside {canonicalSkills}.");
            }
        }

        Assert.Empty(trackedFiles);
    }

    private static string[] TrackedFilesBelow(string repositoryRoot, params string[] relativePaths)
    {
        var start = new ProcessStartInfo("git")
        {
            WorkingDirectory = repositoryRoot,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };
        start.ArgumentList.Add("ls-files");
        start.ArgumentList.Add("--");
        foreach (var relativePath in relativePaths) start.ArgumentList.Add(relativePath);

        using var process = Process.Start(start)!;
        var stdout = process.StandardOutput.ReadToEnd();
        var stderr = process.StandardError.ReadToEnd();
        process.WaitForExit();
        if (process.ExitCode != 0)
            throw new InvalidOperationException(
                $"git ls-files exited {process.ExitCode} in {repositoryRoot}: {stderr}");
        return stdout.Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
    }

    [Fact]
    public void ProjectKnowledgePaths_AreRepositoryRootLiteralsNotSkillRelativeLinks()
    {
        var root = RepositoryRoot();
        Assert.Equal(21, ProjectPathGuidance.Length);
        foreach (var relative in ProjectPathGuidance)
        {
            var body = File.ReadAllText(Path.Combine(root, relative));
            Assert.DoesNotMatch(@"\]\((?:\.\./)+dydo/", body);
            var projectPaths = Regex.Matches(body, @"`(dydo/[^`]+\.md)`")
                .Select(match => match.Groups[1].Value)
                .Where(path => !path.Contains('<') && !path.Contains('>'))
                .Distinct(StringComparer.Ordinal)
                .ToArray();
            Assert.NotEmpty(projectPaths);
            foreach (var projectPath in projectPaths)
            {
                var target = Path.GetFullPath(Path.Combine(root, projectPath.Replace('/', Path.DirectorySeparatorChar)));
                Assert.StartsWith(Path.GetFullPath(root) + Path.DirectorySeparatorChar, target);
                Assert.True(File.Exists(target), $"{relative}: missing repository-root path {projectPath}");
            }
        }
    }

    [Fact]
    public void CodeWriter_InlinesTheWorkingTreeChecksAndTheImplementedFormVerbatim()
    {
        var root = RepositoryRoot();
        var skill = File.ReadAllText(Path.Combine(root, "skills", "roles", "crew", "code-writer", "SKILL.md"));
        var contract = File.ReadAllText(Path.Combine(root, "dydo", "guides", "working-tree-standard.md"));
        var standard = File.ReadAllText(Path.Combine(root, "dydo", "reference", "linear-workspace-standard.md"));

        var checks = NumberedItemsUnder(contract, "## Before the first edit");
        Assert.Equal(5, checks.Length);
        Assert.Equal(checks, NumberedItemsUnder(skill, "## Before the first edit"));
        Assert.Equal(ImplementedForm(standard), ImplementedForm(skill));
    }

    private static string[] NumberedItemsUnder(string body, string heading)
    {
        var section = Regex.Match(body.ReplaceLineEndings("\n"), $@"(?ms)^{Regex.Escape(heading)}\n(.*?)(?=^## |\z)");
        Assert.True(section.Success, $"no {heading} section");
        return Regex.Matches(section.Groups[1].Value, @"(?m)^\s*\d+\.\s+(.+?)\s*$")
            .Select(match => match.Groups[1].Value)
            .ToArray();
    }

    private static string ImplementedForm(string body)
    {
        var form = Regex.Match(body, "`(IMPLEMENTED - [^`]+)`");
        Assert.True(form.Success, "no `IMPLEMENTED - ...` form line");
        return form.Groups[1].Value;
    }

}
