namespace DynaDocs.Tests.Commands;

using System.CommandLine;
using System.Text.RegularExpressions;
using DynaDocs.Commands;

public partial class CommandDocConsistencyTests
{
    private static readonly HashSet<string> ExcludedFromDocs = ["_complete"];
    // Commands intentionally hidden from agent-facing quick reference
    private static readonly HashSet<string> ExcludedPaths =
    [
        "guard lift", "guard restore", "completions", "help",
        "worktree cleanup", "worktree merge", "worktree init-settings", "worktree prune", "worktree status"
    ];
    private static readonly HashSet<string> BuiltInOptionNames = ["--help", "-h", "-?", "--version"];

    private static List<(string Path, Command Cmd)> GetAllCommands() =>
        WalkCommands(DydoRootCommand.Create()).ToList();

    private static List<(string Path, Command Cmd)> GetDocumentedCommands() =>
        GetAllCommands().Where(c => !ExcludedFromDocs.Contains(c.Cmd.Name) && !ExcludedPaths.Contains(c.Path)).ToList();

    private static IEnumerable<(string Path, Command Cmd)> WalkCommands(Command root, string prefix = "")
    {
        foreach (var sub in root.Subcommands)
        {
            var path = string.IsNullOrEmpty(prefix) ? sub.Name : $"{prefix} {sub.Name}";
            yield return (path, sub);
            foreach (var child in WalkCommands(sub, path))
                yield return child;
        }
    }

    private static string FindRepoFile(string relativePath)
    {
        var dir = AppContext.BaseDirectory;
        while (dir != null)
        {
            var candidate = Path.Combine(dir, relativePath);
            if (File.Exists(candidate)) return candidate;
            dir = Path.GetDirectoryName(dir);
        }
        throw new FileNotFoundException(
            $"Could not find {relativePath} walking up from {AppContext.BaseDirectory}");
    }

    private static string FindRepoDir(string relativePath)
    {
        var dir = AppContext.BaseDirectory;
        while (dir != null)
        {
            var candidate = Path.Combine(dir, relativePath);
            if (Directory.Exists(candidate)) return candidate;
            dir = Path.GetDirectoryName(dir);
        }
        throw new DirectoryNotFoundException(
            $"Could not find {relativePath} walking up from {AppContext.BaseDirectory}");
    }

    private static HashSet<string> GetAllNames(Option opt)
    {
        var names = new HashSet<string>(opt.Aliases) { opt.Name };
        return names;
    }

    private static List<Option> GetUserOptions(Command cmd) =>
        cmd.Options.Where(o => !GetAllNames(o).Any(BuiltInOptionNames.Contains)).ToList();

    /// <summary>
    /// Extract sections keyed by command path from a reference doc.
    /// Sections start with "### dydo &lt;path&gt;" and end at the next "### " or "---" or "## ".
    /// </summary>
    private static Dictionary<string, string> ExtractDocSections(string content)
    {
        var sections = new Dictionary<string, string>();
        var headerPattern = new Regex(@"^### dydo (.+?)$", RegexOptions.Multiline);
        var matches = headerPattern.Matches(content);

        for (int i = 0; i < matches.Count; i++)
        {
            var name = matches[i].Groups[1].Value.Trim();
            var start = matches[i].Index + matches[i].Length;
            var end = i + 1 < matches.Count ? matches[i + 1].Index : content.Length;

            var nextBoundary = Regex.Match(content[start..end], @"^(---|## )", RegexOptions.Multiline);
            if (nextBoundary.Success)
                end = start + nextBoundary.Index;

            sections.Add(name, content[start..end]);
        }

        return sections;
    }

    private static Dictionary<string, string> ReadCommandSections(string relativeHub)
    {
        var sections = new Dictionary<string, string>();
        foreach (var page in CommandReferenceFamily.Paths(FindRepoFile(relativeHub), includeHub: false))
        {
            var parsed = ExtractDocSections(CommandReferenceFamily.ReadNonempty(page));
            Assert.NotEmpty(parsed);
            foreach (var (name, content) in parsed)
            {
                Assert.False(string.IsNullOrWhiteSpace(content), $"Empty command section: {name} in {page}");
                Assert.True(sections.TryAdd(name, content), $"Duplicate command section: {name} in {page}");
            }
        }
        Assert.NotEmpty(sections);
        Assert.NotEmpty(sections.Values.SelectMany(ExtractFlags));
        return sections;
    }

    private static HashSet<string> ExtractFlags(string text)
    {
        var pattern = new Regex(@"`(--[\w-]+)(?:\s[^`]*)?`");
        return pattern.Matches(text).Select(m => m.Groups[1].Value).ToHashSet();
    }

}
