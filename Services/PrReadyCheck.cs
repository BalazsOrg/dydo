namespace DynaDocs.Services;

using System.Text;
using DynaDocs.Utils;

/// <summary>
/// <c>dydo pr-ready</c>: a PR shows what its branch holds on the remote, so the tree must be clean and
/// HEAD must equal its upstream before a PR is opened or readied. The comparison reads the local
/// remote-tracking ref; nothing is fetched. The guard runs these tree checks on the
/// PR-opening gh calls <see cref="PrCallScanner"/> finds.
/// </summary>
internal static class PrReadyCheck
{
    private const int ListedPaths = 5;

    /// <summary>
    /// Every reason the tree at <paramref name="directory"/> is not ready, or null when it is not a git
    /// work tree (or git cannot run).
    /// </summary>
    public static List<string>? TreeProblems(string directory)
    {
        if (!GitRunner.Run(directory, "rev-parse", "--is-inside-work-tree").Text.StartsWith("true", StringComparison.Ordinal))
            return null;

        var problems = StatusProblems(GitRunner.Run(directory, "status", "--porcelain=v1", "-z", "--untracked-files=all"));
        problems.Add(UpstreamProblem(directory));
        problems.AddRange(EvidenceProblems(directory));
        return [.. problems.OfType<string>()];
    }

    private static List<string?> StatusProblems(GitResult status)
    {
        if (!status.Ok)
            return [$"git status failed: {status.Error}"];

        List<string> staged = [], unstaged = [], untracked = [];
        var entries = status.Entries;
        for (var i = 0; i < entries.Length; i++)
        {
            var (x, y, path) = (entries[i][0], entries[i][1], entries[i][3..]);
            if (x == '?')
            {
                untracked.Add(path);
                continue;
            }
            if (x != ' ') staged.Add(path);
            if (y != ' ') unstaged.Add(path);
            // A rename or copy carries its source path as the next entry.
            if (x is 'R' or 'C') i++;
        }

        return [Listed("staged changes", staged), Listed("unstaged changes", unstaged), Listed("untracked files", untracked)];
    }

    private static string? Listed(string what, List<string> paths)
    {
        if (paths.Count == 0)
            return null;
        var more = paths.Count > ListedPaths ? $" and {paths.Count - ListedPaths} more" : "";
        return $"{what}: {string.Join(", ", paths.Take(ListedPaths))}{more}";
    }

    private static string? UpstreamProblem(string directory)
    {
        var upstream = GitRunner.Run(directory, "rev-parse", "--abbrev-ref", "--symbolic-full-name", "@{upstream}");
        if (!upstream.Ok)
            return "HEAD has no upstream; push the branch with git push -u";

        var name = upstream.Text.Trim();
        var counts = GitRunner.Run(directory, "rev-list", "--left-right", "--count", "HEAD...@{upstream}").Text.Split();
        return counts is ["0", "0", ..]
            ? null
            : $"HEAD differs from its upstream {name} ({counts[0]} ahead, {counts[1]} behind); push or pull until they match";
    }

    private static readonly HashSet<string> OutputFolders = new(StringComparer.OrdinalIgnoreCase)
    {
        "artifacts", "coverage", "evidence", "logs", "receipts", "testresults"
    };

    public static List<string> EvidenceProblems(string directory)
    {
        var root = GitRunner.Run(directory, "rev-parse", "--show-toplevel");
        if (!root.Ok) return [];
        var gitRoot = root.Text.Trim();
        var prefix = ChangelogPrefix(directory, gitRoot);
        if (prefix == null) return ["cannot inspect changelog gate evidence: docs root is missing or outside this repository"];
        var tracked = GitRunner.Run(gitRoot, "ls-files", "--stage", "-z", "--", ":(top,literal)" + prefix);
        if (!tracked.Ok) return [$"cannot inspect changelog gate evidence: {tracked.Error}"];

        List<string> problems = [];
        foreach (var entry in tracked.Entries)
        {
            var split = entry.IndexOf('\t');
            var path = entry[(split + 1)..];
            var fields = entry[..split].Split(' ');
            if (fields[0] != "100644" || IsEvidencePath(path[prefix.Length..]))
                problems.Add(ReceiptProblem(path));
            else
            {
                var blob = GitRunner.Run(gitRoot, "cat-file", "blob", fields[1]);
                if (!blob.Ok) problems.Add($"cannot inspect changelog record {path}: {blob.Error}");
                else if (!IsText(blob.Output)) problems.Add(ReceiptProblem(path));
            }
        }
        return problems;
    }

    private static string? ChangelogPrefix(string directory, string gitRoot)
    {
        var project = PathUtils.FindProjectRoot(directory);
        if (project == null) return "dydo/project/changelog/";
        // A containing dydo project can belong to an enclosing repository instead.
        var owner = GitRunner.Run(project, "rev-parse", "--show-toplevel");
        if (!owner.Ok || owner.Text.Trim() != gitRoot) return "dydo/project/changelog/";
        var docs = ConfigFileLocator.GetDydoRoot(project);
        // Git resolves symlinked ancestors before reporting a repository-relative prefix.
        var location = GitRunner.Run(docs, "rev-parse", "--show-toplevel", "--show-prefix");
        var lines = location.Text.Replace("\r\n", "\n").Split('\n');
        return location.Ok && lines.Length >= 2 && lines[0] == gitRoot
            ? lines[1] + "project/changelog/"
            : null;
    }

    private static bool IsEvidencePath(string path) =>
        !path.EndsWith(".md", StringComparison.OrdinalIgnoreCase) ||
        path.Split('/').SkipLast(1).Any(OutputFolders.Contains);

    private static bool IsText(byte[] bytes)
    {
        if (bytes.Contains((byte)0)) return false;
        try
        {
            _ = new UTF8Encoding(false, true).GetCharCount(bytes);
            return true;
        }
        catch (DecoderFallbackException) { return false; }
    }

    private static string ReceiptProblem(string path) =>
        $"committed/staged gate evidence: {path}; keep only human-written Markdown records in the changelog; store gate output outside Git";
}
