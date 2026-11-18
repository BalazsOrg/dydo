namespace DynaDocs.Tests.Steps;

using System.Collections;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using Reqnroll;

public sealed partial class CanonicalSkillSteps
{
    [Then("canonical agent guidance does not instruct agents to maintain or compare per-host skill copies")]
    public void AgentGuidanceUsesCanonicalSource()
    {
        foreach (var relative in new[]
        {
            "skills/roles/crew/docs-writer/SKILL.md",
            "skills/productivity/writing-for-agents/resources/skill-mechanics.md"
        })
        {
            var content = File.ReadAllText(Path.Combine(RepositoryRoot(), relative));
            Assert.DoesNotContain("both copies", content, StringComparison.OrdinalIgnoreCase);
            Assert.DoesNotContain("edit both", content, StringComparison.OrdinalIgnoreCase);
            Assert.DoesNotContain("each host's copy", content, StringComparison.OrdinalIgnoreCase);
        }
    }

    [Given("a human-owned Claude {string} directory")]
    public void HumanOwnedClaudeDirectory(string name) => CreateOwnedDirectory(".claude", name);

    [Given("no Claude {string} entry exists")]
    public void NoClaudeEntry(string name) => Assert.False(FileSystemInfoExists(Local(".claude", "skills", name)));

    [Given("a human-owned Codex {string} directory exists")]
    public void HumanOwnedCodexDirectory(string name) => CreateOwnedDirectory(".agents", name);

    [Then("setup fails without changing the human-owned directory")]
    public void SetupFailsAndOwnedDirectoryUnchanged()
    {
        Assert.NotEqual(0, _result.ExitCode);
        Assert.NotNull(_ownedDirectoryBody);
        var candidates = new[] { Local(".claude", "skills", "teach", "SKILL.md"), Local(".agents", "skills", "teach", "SKILL.md") };
        var existing = candidates.Single(File.Exists);
        Assert.Equal(_ownedDirectoryBody, File.ReadAllBytes(existing));
    }

    [Then("no Codex {string} projection is created")]
    public void NoCodexProjection(string name) => Assert.False(FileSystemInfoExists(Local(".agents", "skills", name)));

    [Then("no Claude {string} projection is created")]
    public void NoClaudeProjection(string name) => Assert.False(FileSystemInfoExists(Local(".claude", "skills", name)));

    [Given("the Claude {string} entry links to a different directory")]
    public void ClaudeEntryLinksElsewhere(string name)
    {
        _wrongLinkTarget = Local("elsewhere", name);
        Directory.CreateDirectory(_wrongLinkTarget);
        Directory.CreateDirectory(Local(".claude", "skills"));
        CreateDirectoryLink(Local(".claude", "skills", name), _wrongLinkTarget);
    }

    [Then("setup fails without replacing the existing link")]
    public void WrongLinkUnchanged()
    {
        Assert.NotEqual(0, _result.ExitCode);
        Assert.NotNull(_wrongLinkTarget);
        Assert.Equal(RealPath(_wrongLinkTarget), RealPath(Local(".claude", "skills", "teach")));
        RemoveLink(".claude/skills/teach");
    }

    private void CreateOwnedDirectory(string host, string name)
    {
        var body = Local(host, "skills", name, "SKILL.md");
        Directory.CreateDirectory(Path.GetDirectoryName(body)!);
        _ownedDirectoryBody = Encoding.UTF8.GetBytes("human-owned\n");
        File.WriteAllBytes(body, _ownedDirectoryBody);
    }

    private void RecordFile(string relative, string content)
    {
        var full = Local(relative);
        Directory.CreateDirectory(Path.GetDirectoryName(full)!);
        File.WriteAllText(full, content);
        _ownedFiles[relative] = Fingerprint.File(full);
    }

    private void CopyProjectPathTargets()
    {
        var repository = RepositoryRoot();
        var targets = CanonicalSkillAssertionTests.ProjectPathGuidance
            .SelectMany(relative => ProjectPathLiterals(File.ReadAllText(Path.Combine(repository, relative))))
            .Distinct(StringComparer.Ordinal);
        foreach (var relative in targets)
        {
            var source = Path.Combine(repository, FileLinkPath(relative));
            var target = Local(FileLinkPath(relative));
            Directory.CreateDirectory(Path.GetDirectoryName(target)!);
            File.Copy(source, target);
        }
    }

    private void CopyRemainingSkills()
    {
        foreach (var skill in CanonicalSkills(Path.Combine(RepositoryRoot(), "skills")))
        {
            var target = Local("skills", skill.Category, skill.Name);
            if (!Directory.Exists(target)) CopyDirectory(skill.Directory, target);
        }
    }

    private static string CanonicalCategory(string name) =>
        CanonicalSkills(Path.Combine(RepositoryRoot(), "skills")).SingleOrDefault(skill => skill.Name == name)?.Category
        ?? throw new DirectoryNotFoundException($"Canonical skill not found in any category: {name}");

    private void RecordProjection(string relative) => _projectionTargets[relative] = new DirectoryInfo(Local(relative)).LinkTarget;
    private void RemoveLink(string relative)
    {
        var full = Local(relative.Replace('/', Path.DirectorySeparatorChar));
        if (new DirectoryInfo(full).LinkTarget is not null) Directory.Delete(full);
    }

    // Runs before CliScenario.Cleanup (default Order 10000): its recursive delete cannot remove a
    // directory junction on Windows, so any projection a scenario leaves behind is unlinked first.
    [AfterScenario(Order = 0)]
    public void RemoveHostSkillLinks()
    {
        foreach (var host in new[] { ".claude", ".agents" })
        {
            var root = Local(host, "skills");
            if (!Directory.Exists(root)) continue;
            foreach (var directory in Directory.EnumerateDirectories(root))
                if (new DirectoryInfo(directory).LinkTarget is not null) Directory.Delete(directory);
        }
    }

    private void AssertProjectPathsUseRepositoryRoot(Table locations)
    {
        var projectRoot = Path.GetFullPath(scenario.DirectoryPath);
        var skills = CanonicalSkills(Local("skills"));
        foreach (var relative in CanonicalSkillAssertionTests.ProjectPathGuidance)
        {
            var (name, category, _) = skills.Single(skill => relative.StartsWith($"skills/{skill.Category}/{skill.Name}/", StringComparison.Ordinal));
            var relativeFile = relative[$"skills/{category}/{name}/".Length..].Replace('/', Path.DirectorySeparatorChar);
            var canonicalFile = Local(relative.Replace('/', Path.DirectorySeparatorChar));
            var canonicalBody = File.ReadAllText(canonicalFile);
            var projectPaths = ProjectPathLiterals(canonicalBody).ToArray();
            Assert.NotEmpty(projectPaths);
            foreach (var row in locations.Rows)
            {
                var location = row["location"]
                    .Replace("<category>", category, StringComparison.Ordinal)
                    .Replace("<name>", name, StringComparison.Ordinal);
                var viewFile = Path.Combine(Local(location.Replace('/', Path.DirectorySeparatorChar)), relativeFile);
                Assert.Equal(Fingerprint.File(canonicalFile), Fingerprint.File(viewFile));
                var viewBody = File.ReadAllText(viewFile);
                Assert.Equal(projectPaths, ProjectPathLiterals(viewBody).ToArray());
                foreach (var projectPath in projectPaths)
                {
                    var target = Path.GetFullPath(Path.Combine(projectRoot, FileLinkPath(projectPath)));
                    AssertInside(projectRoot, target, $"{location}/{relativeFile}: {projectPath}");
                    Assert.True(File.Exists(target), $"Missing repository-root project path: {projectPath}");
                    Assert.NotEmpty(File.ReadAllBytes(target));
                }
            }
        }
    }

    private static void ClearReadOnly(string directory)
    {
        foreach (var path in Directory.EnumerateFileSystemEntries(directory, "*", SearchOption.AllDirectories))
            File.SetAttributes(path, File.GetAttributes(path) & ~FileAttributes.ReadOnly);
    }

    private CliResult Run(string executable, params string[] arguments)
    {
        var start = new ProcessStartInfo(executable)
        {
            WorkingDirectory = scenario.DirectoryPath,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };
        foreach (var argument in arguments) start.ArgumentList.Add(argument);
        using var process = Process.Start(start)!;
        var stdout = process.StandardOutput.ReadToEnd();
        var stderr = process.StandardError.ReadToEnd();
        process.WaitForExit();
        return new CliResult(process.ExitCode, stdout, stderr);
    }

    private void CreateDirectoryLink(string link, string target)
    {
        if (!OperatingSystem.IsWindows())
        {
            Directory.CreateSymbolicLink(link, target);
            return;
        }
        var result = Run("cmd.exe", "/d", "/c", "mklink", "/J", link, target);
        result.AssertSuccess();
    }

    private string Local(params string[] parts) => Path.Combine([scenario.DirectoryPath, .. parts]);
    private static bool FileSystemInfoExists(string path)
    {
        if (File.Exists(path) || Directory.Exists(path)) return true;
        try { return new DirectoryInfo(path).LinkTarget is not null; }
        catch (IOException) { return false; }
    }
    private static string RealPath(string path) => new DirectoryInfo(path).ResolveLinkTarget(true)?.FullName ?? Path.GetFullPath(path);
    private static bool IsRepositoryRelativeFileLink(string link) => link != "Linear URL" && !link.StartsWith('#') && !Uri.TryCreate(link, UriKind.Absolute, out _);
    private static string FileLinkPath(string link) => link.Replace('/', Path.DirectorySeparatorChar);
    private static IEnumerable<string> MarkdownLinks(string markdown) => Regex.Matches(markdown, @"\[[^\]]+\]\(([^)#]+)(?:#[^)]+)?\)")
        .Select(match => match.Groups[1].Value);
    private static IEnumerable<string> ProjectPathLiterals(string markdown) => Regex.Matches(markdown, @"`(dydo/[^`]+\.md)`")
        .Select(match => match.Groups[1].Value)
        .Where(path => !path.Contains('<') && !path.Contains('>'))
        .Distinct(StringComparer.Ordinal);

    private static void AssertInside(string root, string target, string description)
    {
        var relative = Path.GetRelativePath(Path.GetFullPath(root), Path.GetFullPath(target));
        Assert.False(relative == ".." || relative.StartsWith($"..{Path.DirectorySeparatorChar}", StringComparison.Ordinal) || Path.IsPathRooted(relative),
            $"Path escaped {root}: {description}");
    }

    private static void CopyDirectory(string source, string target)
    {
        Directory.CreateDirectory(target);
        foreach (var file in Directory.EnumerateFiles(source)) File.Copy(file, Path.Combine(target, Path.GetFileName(file)));
        foreach (var directory in Directory.EnumerateDirectories(source))
            CopyDirectory(directory, Path.Combine(target, Path.GetFileName(directory)));
    }

    private static string RepositoryRoot()
    {
        for (var directory = new DirectoryInfo(AppContext.BaseDirectory); directory is not null; directory = directory.Parent)
            if (File.Exists(Path.Combine(directory.FullName, "DynaDocs.csproj"))) return directory.FullName;
        throw new DirectoryNotFoundException($"Could not locate repository root from {AppContext.BaseDirectory}");
    }

    private readonly record struct Fingerprint(long Length, string Sha256)
    {
        internal static Fingerprint File(string path)
        {
            var bytes = System.IO.File.ReadAllBytes(path);
            return new(bytes.LongLength, Convert.ToHexString(SHA256.HashData(bytes)));
        }
    }
}
