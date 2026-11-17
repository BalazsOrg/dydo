namespace DynaDocs.Services;

using DynaDocs.Models;

internal sealed class SafeDeleteRepository
{
    internal string Directory { get; }
    internal string Common { get; }
    internal string Root => Path.Combine(Common, "dydo", "safe-delete-archive");
    internal List<WorktreeEntry> Trees { get; }

    internal SafeDeleteRepository(string directory)
    {
        Directory = Path.GetFullPath(directory);
        Common = Path.GetFullPath(SafeDeleteGit.Require(Directory, "rev-parse", "--path-format=absolute", "--git-common-dir"));
        SafeDeletePaths.NoLinks(Common);
        Trees = ReadTrees();
    }

    internal List<WorktreeEntry> ReadTrees()
    {
        var raw = SafeDeleteGit.Require(Directory, "worktree", "list", "--porcelain", "-z");
        var result = new List<WorktreeEntry>();
        foreach (var block in raw.Split("\0\0", StringSplitOptions.RemoveEmptyEntries))
        {
            var fields = block.Split('\0');
            var path = fields.Single(f => f.StartsWith("worktree ", StringComparison.Ordinal))[9..];
            var head = fields.FirstOrDefault(f => f.StartsWith("HEAD ", StringComparison.Ordinal))?[5..];
            var branch = fields.FirstOrDefault(f => f.StartsWith("branch refs/heads/", StringComparison.Ordinal))?[18..];
            result.Add(new WorktreeEntry(path, head, branch, fields.Any(f => f.StartsWith("locked", StringComparison.Ordinal)), result.Count == 0));
        }
        return result;
    }

    internal string? SelectRemote(string? branch, string? requested)
    {
        var remotes = SafeDeleteGit.Require(Directory, "remote").Split('\n', StringSplitOptions.RemoveEmptyEntries);
        var selected = requested;
        if (selected == null && branch != null)
        {
            var upstream = SafeDeleteGit.Read(Directory, "config", "--get", $"branch.{branch}.remote");
            if (upstream.Ok && upstream.Text.Trim() != ".") selected = upstream.Text.Trim();
        }
        if (selected == null)
        {
            if (remotes.Contains("origin")) selected = "origin";
            else if (remotes.Length == 1) selected = remotes[0];
            else if (remotes.Length > 1) throw new InvalidDataException("Ambiguous remote: select --remote explicitly");
        }
        if (selected != null && (!remotes.Contains(selected) || selected.StartsWith('-') || selected.Any(char.IsControl)))
            throw new InvalidDataException("Invalid or missing remote name");
        return selected;
    }

    internal string? RemoteOid(string remote, string branch)
    {
        var lines = SafeDeleteGit.Require(Directory, "ls-remote", "--symref", "--", remote, "HEAD", $"refs/heads/{branch}").Split('\n', StringSplitOptions.RemoveEmptyEntries);
        var defaultRef = lines.FirstOrDefault(line => line.StartsWith("ref: refs/heads/", StringComparison.Ordinal) && line.EndsWith("\tHEAD", StringComparison.Ordinal));
        if (defaultRef == null) throw new InvalidDataException("Remote default branch is unknown; target retained");
        if (defaultRef[5..^5] == "refs/heads/" + branch) throw new InvalidDataException("Remote default branch is protected");
        return lines.FirstOrDefault(line => line.EndsWith("\trefs/heads/" + branch, StringComparison.Ordinal))?.Split('\t')[0];
    }

    internal void Eligible(string? branch, WorktreeEntry? tree, SafeDeleteDependencies dependencies, bool dryRun)
    {
        EligibleBranch(branch, tree);
        if (tree == null) return;
        if (tree.IsMain) throw new InvalidDataException("Main worktree is protected");
        if (SafeDeletePaths.Within(Directory, tree.Path)) throw new InvalidDataException("Current worktree is protected");
        if (tree.Locked) throw new InvalidDataException("Locked worktree is protected");
        if (!System.IO.Directory.Exists(tree.Path)) throw new InvalidDataException("Missing/prunable worktree is not a complete snapshot");
        SafeDeletePaths.NoLinks(tree.Path);
        var actualCommon = SafeDeleteGit.Require(tree.Path, "rev-parse", "--path-format=absolute", "--git-common-dir");
        var actualTop = SafeDeleteGit.Require(tree.Path, "rev-parse", "--show-toplevel");
        SafeDeletePaths.NoLinks(Path.Combine(tree.Path, ".git"));
        if (!SafeDeletePaths.Same(actualCommon, Common) || !SafeDeletePaths.Same(actualTop, tree.Path))
            throw new InvalidDataException("Registered worktree administrative identity differs");
        var t3 = dependencies.T3.Refusal(tree.Path);
        if (t3 != null) throw new InvalidDataException(t3);
        if (!OperatingSystem.IsLinux() && !OperatingSystem.IsWindows()) throw new InvalidDataException("No supported busy-worktree probe on this platform");
        if (!dryRun)
        {
            try
            {
                var busy = dependencies.FindUse(tree.Path);
                if (busy != null) throw new InvalidDataException(busy);
            }
            catch (ProbeStrandedException ex)
            {
                SafeDeleteGit.Read(Directory, "worktree", "lock", "--reason", $"dydo archive probe stranded at {ex.Probe}", tree.Path);
                throw new InvalidDataException($"Busy probe stranded; registered worktree locked. Recover {ex.Probe} to {tree.Path} before unlocking. {ex.Message}");
            }
        }
    }

    private void EligibleBranch(string? branch, WorktreeEntry? tree)
    {
        if (branch == null) return;
        var mainBranch = Trees.FirstOrDefault(t => t.IsMain)?.Branch;
        var configured = SafeDeleteGit.Read(Directory, "config", "--get", "init.defaultBranch");
        var configuredBranch = configured.Ok && !string.IsNullOrWhiteSpace(configured.Text) ? configured.Text.Trim() : null;
        SafeDeleteGit.BranchName(Directory, branch);
        CheckLocalBranchSpelling(branch);
        if (SafeDeleteGit.Read(Directory, "symbolic-ref", "-q", "refs/heads/" + branch).Ok)
            throw new InvalidDataException("Symbolic local branch refs are unsupported");
        if (mainBranch == null && configuredBranch == null) throw new InvalidDataException("Authoritative local default is unavailable");
        if (string.Equals(branch, mainBranch, SafeDeletePaths.Comparison) || string.Equals(branch, configuredBranch, SafeDeletePaths.Comparison))
            throw new InvalidDataException("Local default branch is protected");
        if (Trees.Any(t => string.Equals(t.Branch, branch, SafeDeletePaths.Comparison) && (tree == null || !SafeDeletePaths.Same(t.Path, tree.Path))))
            throw new InvalidDataException("Branch is checked out elsewhere");
    }

    private void CheckLocalBranchSpelling(string branch)
    {
        var name = "refs/heads/" + branch;
        var matches = SafeDeleteGit.Require(Directory, "for-each-ref", "--format=%(refname)", "refs/heads")
            .Split('\n', StringSplitOptions.RemoveEmptyEntries).Where(value => value.Equals(name, SafeDeletePaths.Comparison)).ToArray();
        if (matches.Length > 1 || (matches.Length == 1 && matches[0] != name)
            || (matches.Length == 0 && SafeDeleteGit.Ref(Directory, name) != null))
            throw new InvalidDataException("Ambiguous local branch spelling; use the exact canonical ref name");
    }

    internal static void SupportedState(string cwd)
    {
        if (SafeDeleteGit.Require(cwd, "rev-parse", "--is-shallow-repository") != "false") throw new InvalidDataException("Shallow history is unsupported");
        var gitDirectory = SafeDeleteGit.Require(cwd, "rev-parse", "--absolute-git-dir");
        SafeDeletePaths.NoLinks(gitDirectory);
        foreach (var name in new[] { "objects/info/alternates", "objects/info/http-alternates", "shallow", "info/grafts" })
        {
            var path = SafeDeleteGit.Require(cwd, "rev-parse", "--path-format=absolute", "--git-path", name);
            if (File.Exists(path)) throw new InvalidDataException($"Borrowed/incomplete object history is unsupported: {name}");
        }
        if (SafeDeleteGit.Read(cwd, "config", "--get", "extensions.partialClone").Ok
            || SafeDeleteGit.Read(cwd, "config", "--get-regexp", "remote\\..*\\.promisor").Ok)
            throw new InvalidDataException("Partial clone/promisor object history is unsupported");
        foreach (var name in new[] { "MERGE_HEAD", "CHERRY_PICK_HEAD", "REVERT_HEAD", "REBASE_HEAD", "BISECT_START", "rebase-merge", "rebase-apply", "sequencer", "index.lock", "HEAD.lock" })
            if (File.Exists(Path.Combine(gitDirectory, name)) || System.IO.Directory.Exists(Path.Combine(gitDirectory, name)))
                throw new InvalidDataException($"In-progress Git operation is unsupported: {name}");
        foreach (var key in new[] { "core.sparseCheckout", "core.splitIndex" })
            if (SafeDeleteGit.Read(cwd, "config", "--bool", "--get", key).Text.Trim() == "true") throw new InvalidDataException($"Unsupported {key}; expand the index first");
        if (SafeDeleteGit.Require(cwd, "for-each-ref", "--format=%(refname)", "refs/replace").Length > 0)
            throw new InvalidDataException("Replace refs require independent preservation and are unsupported");
    }
}
