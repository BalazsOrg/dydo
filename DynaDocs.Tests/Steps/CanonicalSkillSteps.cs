namespace DynaDocs.Tests.Steps;

using System.Collections;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using Reqnroll;

[Binding]
[Scope(Feature = "One canonical skill tree reaches every supported host")]
public sealed partial class CanonicalSkillSteps(CliScenario scenario)
{
    private readonly Dictionary<string, Fingerprint> _ownedFiles = [];
    private readonly Dictionary<string, string?> _projectionTargets = [];
    private byte[]? _ownedDirectoryBody;
    private string? _wrongLinkTarget;
    private CliResult _result = new(-1, "", "");

    [Given("a project containing the canonical {string} skill and the skill setup script")]
    public void ProjectWithCanonicalSkill(string name)
    {
        CopyCanonicalSkill(name);
        File.Copy(Path.Combine(RepositoryRoot(), "setup-skills.mjs"), Local("setup-skills.mjs"));
        File.WriteAllText(Local(".gitignore"), "/.claude/skills/\n/.agents/skills/\n");
    }

    [Given("the canonical {string} skill is also in the project")]
    public void CopyCanonicalSkill(string name)
    {
        var category = CanonicalCategory(name);
        CopyDirectory(Path.Combine(RepositoryRoot(), "skills", category, name), Local("skills", category, name));
        Assert.True(Directory.Exists(Local("skills", category, name)));
    }

    [Given("the Claude and Codex {string} entries link to the former category folder {string}")]
    public void FormerCategoryLinks(string name, string former)
    {
        var formerTarget = Local(former.Replace('/', Path.DirectorySeparatorChar));
        Assert.False(Directory.Exists(formerTarget), $"{former} must be gone for the link to be left dangling");
        foreach (var host in new[] { ".claude", ".agents" })
        {
            Directory.CreateDirectory(Local(host, "skills"));
            CreateDirectoryLink(Local(host, "skills", name), formerTarget);
        }
    }

    [Given("unrelated Claude and Codex skills and host configuration files with recorded bytes")]
    public void UnrelatedHostFiles()
    {
        RecordFile(".claude/skills/local-only/SKILL.md", "local claude skill\n");
        RecordFile(".claude/settings.local.json", "{\"permissions\":[]}\n");
        RecordFile(".agents/skills/local-only/SKILL.md", "local codex skill\n");
        RecordFile(".agents/config.json", "{\"local\":true}\n");
        Run("git", "init").AssertSuccess();
        Run("git", "config", "user.email", "dyd91@example.invalid").AssertSuccess();
        Run("git", "config", "user.name", "DYD-91 canary").AssertSuccess();
        Run("git", "add", ".").AssertSuccess();
        Run("git", "add", "-f", ".claude", ".agents").AssertSuccess();
        Run("git", "commit", "-m", "fixture").AssertSuccess();
    }

    [When("I set up the canonical skills")]
    public void Setup() => _result = Run("node", "setup-skills.mjs");

    [When("I set up the canonical skills again")]
    public void SetupAgain()
    {
        RecordProjection(".claude/skills/teach");
        RecordProjection(".agents/skills/teach");
        Setup();
    }

    [Then("setup succeeds")]
    public void SetupSucceeds() => _result.AssertSuccess();

    [Then("the Claude and Codex {string} entries resolve to the canonical skill directory")]
    public void HostEntriesResolve(string name)
    {
        var canonical = RealPath(Local("skills", CanonicalCategory(name), name));
        Assert.Equal(canonical, RealPath(Local(".claude", "skills", name)));
        Assert.Equal(canonical, RealPath(Local(".agents", "skills", name)));
        Assert.True(new DirectoryInfo(Local(".claude", "skills", name)).LinkTarget is not null);
        Assert.True(new DirectoryInfo(Local(".agents", "skills", name)).LinkTarget is not null);
    }

    [Then("no OpenCode {string} projection is created")]
    public void NoOpenCodeProjection(string name) => Assert.False(Directory.Exists(Local(".opencode", "skills", name)));

    [Then("every unrelated skill and host configuration file keeps its recorded bytes")]
    public void OwnedFilesUnchanged()
    {
        Assert.Equal(4, _ownedFiles.Count);
        foreach (var (relative, expected) in _ownedFiles)
            Assert.Equal(expected, Fingerprint.File(Local(relative)));
    }

    [Then("the host projections are unchanged")]
    public void ProjectionsUnchanged()
    {
        Assert.Equal(2, _projectionTargets.Count);
        foreach (var (relative, target) in _projectionTargets)
            Assert.Equal(target, new DirectoryInfo(Local(relative)).LinkTarget);
    }

    [Then("the Git working tree is clean")]
    public void GitClean()
    {
        var result = Run("git", "status", "--short");
        result.AssertSuccess();
        Assert.Equal("", result.Stdout);
        RemoveHostSkillLinks();
        ClearReadOnly(Local(".git"));
        Directory.Delete(Local(".git"), recursive: true);
    }

    [Then("the canonical {string} skill keeps its explicit-only lock on both hosts")]
    public void ExplicitOnlyLock(string name)
    {
        var body = File.ReadAllText(Local("skills", CanonicalCategory(name), name, "SKILL.md"));
        Assert.StartsWith("---\n", body.ReplaceLineEndings("\n"));
        Assert.Contains("disable-model-invocation: true", body);
        var metadata = File.ReadAllText(Local("skills", CanonicalCategory(name), name, "agents", "openai.yaml"));
        Assert.Contains("allow_implicit_invocation: false", metadata);
    }

    [Then("the canonical {string} skill keeps the argument hint {string} on both hosts")]
    public void ArgumentHint(string name, string hint)
    {
        var body = File.ReadAllText(Local("skills", CanonicalCategory(name), name, "SKILL.md"));
        Assert.StartsWith("---\n", body.ReplaceLineEndings("\n"));
        Assert.Contains($"argument-hint: \"{hint}\"", body);
        var metadata = File.ReadAllText(Local("skills", CanonicalCategory(name), name, "agents", "openai.yaml"));
        Assert.Contains($"default_prompt: \"{hint}\"", metadata);
    }

    [Then("every resource linked by the canonical {string} body resolves inside its skill directory")]
    public void ResourceLinksResolve(string name)
    {
        var skill = Local("skills", CanonicalCategory(name), name);
        var links = MarkdownLinks(File.ReadAllText(Path.Combine(skill, "SKILL.md")))
            .Where(link => link.StartsWith("resources/", StringComparison.Ordinal)).ToArray();
        Assert.NotEmpty(links);
        foreach (var link in links)
        {
            var target = Path.GetFullPath(Path.Combine(skill, link.Replace('/', Path.DirectorySeparatorChar)));
            Assert.StartsWith(Path.GetFullPath(skill) + Path.DirectorySeparatorChar, target);
            Assert.True(File.Exists(target), $"Missing resource link: {link}");
        }
    }

    [Then("every skill link resolves when followed from each location")]
    public void EveryLinkResolves(Table locations)
    {
        CopyRemainingSkills();
        CopyProjectPathTargets();
        Setup();
        SetupSucceeds();
        try
        {
            Assert.Equal(3, locations.Rows.Count);
            foreach (var (name, category, canonicalSkill) in CanonicalSkills(Local("skills")))
            {
                var markdownFiles = Directory.EnumerateFiles(canonicalSkill, "*.md", SearchOption.AllDirectories).ToArray();
                Assert.NotEmpty(markdownFiles);
                foreach (var canonicalFile in markdownFiles)
                {
                    var relativeFile = Path.GetRelativePath(canonicalSkill, canonicalFile);
                    var canonicalBytes = Fingerprint.File(canonicalFile);
                    var links = MarkdownLinks(File.ReadAllText(canonicalFile)).Where(IsRepositoryRelativeFileLink).ToArray();
                    foreach (var link in links)
                    {
                        var canonicalTarget = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(canonicalFile)!, FileLinkPath(link)));
                        AssertInside(canonicalSkill, canonicalTarget, $"{Path.GetRelativePath(Local(), canonicalFile)}: {link}");
                        Assert.True(File.Exists(canonicalTarget), $"Missing skill-local link: {canonicalFile}: {link}");
                    }

                    foreach (var row in locations.Rows)
                    {
                        var location = row["location"]
                            .Replace("<category>", category, StringComparison.Ordinal)
                            .Replace("<name>", name, StringComparison.Ordinal);
                        var viewRoot = Local(location.Replace('/', Path.DirectorySeparatorChar));
                        var viewFile = Path.Combine(viewRoot, relativeFile);
                        Assert.Equal(canonicalBytes, Fingerprint.File(viewFile));
                        Assert.Equal(Path.GetFullPath(canonicalSkill), RealPath(viewRoot));
                        foreach (var link in links)
                        {
                            var lexicalTarget = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(viewFile)!, FileLinkPath(link)));
                            Assert.True(File.Exists(lexicalTarget), $"{location}/{relativeFile}: {link}");
                            var projectedCanonicalTarget = Path.GetFullPath(Path.Combine(RealPath(viewRoot), Path.GetDirectoryName(relativeFile) ?? "", FileLinkPath(link)));
                            var expectedCanonicalTarget = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(canonicalFile)!, FileLinkPath(link)));
                            Assert.Equal(expectedCanonicalTarget, projectedCanonicalTarget);
                            Assert.Equal(Fingerprint.File(expectedCanonicalTarget), Fingerprint.File(lexicalTarget));
                        }
                    }
                }
            }

            Assert.False(FileSystemInfoExists(Local(".claude", "dydo")));
            Assert.False(FileSystemInfoExists(Local(".agents", "dydo")));
            AssertProjectPathsUseRepositoryRoot(locations);
        }
        finally
        {
            RemoveHostSkillLinks();
        }
    }

    [Then(@"^current documentation and template mirrors describe skills/<category>/<name> as the only editable source$")]
    public void CurrentGuidanceUsesCanonicalSource()
    {
        var root = RepositoryRoot();
        foreach (var relative in CanonicalSkillAssertionTests.CurrentGuidance)
        {
            var file = Path.Combine(root, relative);
            var content = relative.EndsWith("/dydo-commands.md", StringComparison.Ordinal)
                ? Commands.CommandReferenceFamily.Read(file) : File.ReadAllText(file);
            Assert.Contains("skills/<category>/", content);
            Assert.DoesNotMatch(@"(?<!\.claude/)(?<!\.agents/)skills/<name>", content);
            Assert.DoesNotMatch(@"(?<!\.claude/)(?<!\.agents/)skills/<role>", content);
            Assert.Empty(StaleSkillPaths(root, content));
            Assert.DoesNotContain("orchestration/", content);
            Assert.DoesNotContain("edit both", content, StringComparison.OrdinalIgnoreCase);
            Assert.DoesNotContain("edit each host", content, StringComparison.OrdinalIgnoreCase);
            Assert.DoesNotContain("committed copy", content, StringComparison.OrdinalIgnoreCase);
        }
    }

    /// <summary>Every literal <c>skills/…/&lt;name&gt;</c> reference to a real skill name -- flat, or
    /// under any category prefix -- that is not that skill's canonical path, such as a flat
    /// <c>skills/reviewer</c> or a former-category <c>skills/orchestration/reviewer</c>. Host
    /// projection paths (<c>.claude/skills/&lt;name&gt;/</c>, <c>.agents/skills/&lt;name&gt;/</c>) stay
    /// flat by design and are never reported. The trailing boundary is a negative lookahead rather
    /// than a required <c>/</c>, so a bare mention like <c>skills/admiral</c> is still caught.</summary>
    internal static string[] StaleSkillPaths(string repositoryRoot, string content)
    {
        var canonical = CanonicalSkills(Path.Combine(repositoryRoot, "skills"))
            .ToDictionary(skill => skill.Name, skill => $"skills/{skill.Category}/{skill.Name}", StringComparer.Ordinal);
        var names = string.Join("|", canonical.Keys.Select(Regex.Escape));
        return Regex.Matches(content, $@"(?<!\.claude/)(?<!\.agents/)\bskills/(?:[a-z0-9-]+/)*?(?<name>{names})(?![A-Za-z0-9-])")
            .Where(match => match.Value != canonical[match.Groups["name"].Value])
            .Select(match => match.Value)
            .ToArray();
    }

    /// <summary>Every canonical skill under <paramref name="skillsRoot"/>, walked by setup-skills.mjs's
    /// own rule: a folder holding <c>SKILL.md</c> is a skill (its subfolders are never walked), and any
    /// other folder is a category to walk into, at any depth. <c>Category</c> is the category path
    /// with <c>/</c> separators, such as <c>roles/crew</c>. An empty category or a skill name reached
    /// through two categories is an error, as it is for setup.</summary>
    internal static IReadOnlyList<CanonicalSkill> CanonicalSkills(string skillsRoot)
    {
        var skills = new List<CanonicalSkill>();
        Walk(skillsRoot);
        return skills;

        void Walk(string categoryRoot)
        {
            var entries = Directory.EnumerateDirectories(categoryRoot).Order(StringComparer.Ordinal).ToArray();
            if (entries.Length == 0) throw new InvalidOperationException($"No canonical skills found in {categoryRoot}");
            var category = Path.GetRelativePath(skillsRoot, categoryRoot).Replace(Path.DirectorySeparatorChar, '/');
            foreach (var entry in entries)
            {
                if (!File.Exists(Path.Combine(entry, "SKILL.md")))
                {
                    Walk(entry);
                    continue;
                }
                var name = Path.GetFileName(entry);
                if (skills.Any(skill => skill.Name == name))
                    throw new InvalidOperationException($"Duplicate skill name across categories: {name}");
                skills.Add(new CanonicalSkill(name, category, entry));
            }
        }
    }

    internal sealed record CanonicalSkill(string Name, string Category, string Directory);

}
