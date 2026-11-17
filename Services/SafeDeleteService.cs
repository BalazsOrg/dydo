namespace DynaDocs.Services;

using DynaDocs.Models;
using System.Text.Json;

internal sealed class SafeDeleteService(string directory, SafeDeleteDependencies dependencies, TextWriter output)
{
    private SafeDeleteGitHubTarget? _gitHubTarget;

    internal int Archive(string kind, string target, string? requestedRemote, bool dryRun) => Execute(() =>
    {
        var repository = new SafeDeleteRepository(directory);
        var store = new SafeDeleteStore(repository.Root, output);
        var (manifest, tree, config) = Prepare(repository, kind, target, requestedRemote, dryRun);
        var cwd = tree?.Path ?? directory;
        var branch = manifest.Branch;
        var remote = manifest.Remote;
        var remoteOid = manifest.RemoteOid;
        var estimates = Estimate(cwd, manifest);
        CheckSpace(repository.Common, estimates, config);
        if (dryRun)
        {
            var unknown = remoteOid != null && !SafeDeleteGit.Read(directory, "cat-file", "-e", remoteOid).Ok;
            output.WriteLine($"Dry run: archiveBytes={estimates.Archive}; proofScratchBytes={estimates.Scratch}; sumBytes={checked(estimates.Archive + estimates.Scratch)}; remote objects={(unknown ? "unknown; lower-bound estimate (no fetch)" : "present or absent")}; remote transfer size is not promised before transfer");
            output.WriteLine($"Would archive {target}, then delete remote={remote ?? "none"}, worktree={tree?.Path ?? "none"}, localBranch={branch ?? "none"}; busy probe not run");
            return;
        }
        using var held = store.Lock();
        store.Purge(manifest.Created);
        var fetchedRef = "refs/dydo-fetch/" + manifest.Id;
        try
        {
            if (remoteOid != null)
            {
                dependencies.Checkpoint("before-fetch");
                CheckSpace(repository.Common, Estimate(cwd, manifest), config);
                SafeDeleteGit.Require(directory, "-c", "gc.auto=0", "-c", "maintenance.auto=false", "fetch", "--refmap=", "--no-tags", "--no-write-fetch-head", "--no-auto-maintenance", "--", remote!, $"refs/heads/{branch}:{fetchedRef}");
                if (SafeDeleteGit.Ref(directory, fetchedRef) != remoteOid) throw new InvalidDataException("Remote moved during fetch; retained");
                CheckSpace(repository.Common, Estimate(cwd, manifest), config);
            }
            WriteArchive(repository, store, manifest, tree);
        }
        finally
        {
            var fetched = SafeDeleteGit.Ref(directory, fetchedRef);
            if (fetched != null && fetched == remoteOid) SafeDeleteGit.Read(directory, "update-ref", "--no-deref", "-d", fetchedRef, remoteOid!);
            else if (fetched != null) output.WriteLine($"Temporary fetch ref {fetchedRef} changed; retained for inspection");
        }
    });


    private (SafeDeleteManifest Manifest, WorktreeEntry? Tree, ArchiveConfig Config) Prepare(SafeDeleteRepository repository, string kind, string target, string? requestedRemote, bool dryRun)
    {
        WorktreeEntry? tree = null;
        string? branch;
        if (kind == "worktree")
        {
            var path = Path.GetFullPath(target, directory);
            tree = repository.Trees.SingleOrDefault(t => SafeDeletePaths.Same(t.Path, path))
                ?? throw new InvalidDataException("Target is not a registered linked worktree");
            branch = tree.Branch;
        }
        else { branch = target; SafeDeleteGit.BranchName(directory, branch); }
        var config = new ConfigService().LoadConfigStrict(directory)?.Archive ?? new ArchiveConfig();
        config.Validate();
        var remote = branch == null ? null : repository.SelectRemote(branch, requestedRemote);
        repository.Eligible(branch, tree, dependencies, dryRun);
        var cwd = tree?.Path ?? directory;
        SafeDeleteRepository.SupportedState(cwd);
        var local = branch == null ? null : SafeDeleteGit.Ref(directory, "refs/heads/" + branch);
        var remoteOid = remote == null ? null : repository.RemoteOid(remote, branch!);
        if (local == null && remoteOid == null && tree == null) throw new InvalidDataException("Branch is absent locally and on the selected remote");
        var remoteUrl = EligibleRemote(remote, branch, remoteOid);
        var now = dependencies.Now().ToUniversalTime();
        var manifest = new SafeDeleteManifest
        {
            Id = now.ToString("yyyyMMddTHHmmssfffffffZ") + "-" + Guid.NewGuid().ToString("N"),
            Created = now, Expires = now.AddDays(30), Repository = repository.Common,
            ObjectFormat = SafeDeleteGit.Require(directory, "rev-parse", "--show-object-format"),
            Worktree = tree?.Path, Branch = branch, LocalOid = local, Remote = remote,
            RemoteUrl = remoteUrl == null ? null : SafeDeleteGit.Redact(remoteUrl), RemoteOid = remoteOid,
            GitHubRepositoryId = _gitHubTarget?.Metadata?.Id,
            PrimaryOid = tree?.Head ?? local ?? remoteOid!
        };
        CaptureWorktree(tree, cwd, manifest);
        ReportUpstream(branch);
        return (manifest, tree, config);
    }

    private static void CaptureWorktree(WorktreeEntry? tree, string cwd, SafeDeleteManifest manifest)
    {
        if (tree != null)
        {
            manifest.GitInputs = SafeDeleteFiles.CaptureGitInputs(cwd);
            manifest.Files = SafeDeleteFiles.Capture(tree.Path);
            SafeDeleteFiles.CaptureRoot(tree.Path, manifest);
            manifest.Status = SafeDeleteGit.Status(cwd);
            foreach (var key in new[] { "core.filemode", "core.ignorecase", "core.symlinks", "core.autocrlf", "core.eol" })
            {
                var value = SafeDeleteGit.Read(cwd, "config", "--get", key);
                if (value.Ok) manifest.GitConfig[key] = value.Text.Trim();
            }
        }
    }

    private string? EligibleRemote(string? remote, string? branch, string? oid)
    {
        _gitHubTarget = null;
        var remoteUrl = remote == null ? null : SafeDeleteGit.Require(directory, "remote", "get-url", "--push", remote);
        if (remote != null)
        {
            // Fetch and deletion must address the identical server, not a different pushurl.
            var fetchUrl = SafeDeleteGit.Require(directory, "remote", "get-url", remote);
            var pushUrls = SafeDeleteGit.Require(directory, "remote", "get-url", "--push", "--all", remote).Split('\n');
            if (fetchUrl != remoteUrl || pushUrls.Length != 1) throw new InvalidDataException("Different/multiple remote push URLs are unsupported");
            if (branch!.StartsWith("feature/", StringComparison.Ordinal)) throw new InvalidDataException("Remote feature/* branch is protected");
            _gitHubTarget = new SafeDeleteGitHubTarget(remoteUrl!, branch!, oid);
            var refusal = dependencies.CheckGitHub(_gitHubTarget);
            if (refusal != null) throw new InvalidDataException(refusal);
        }
        return remoteUrl;
    }

    private (string? Branch, string? Path) PrepareRestore(SafeDeleteRepository repository, SafeDeleteManifest manifest, string? requestedBranch, string? requestedPath, bool remote)
    {
        var branch = requestedBranch ?? manifest.Branch;
        var path = requestedPath == null ? manifest.Worktree : Path.GetFullPath(requestedPath, directory);
        var platform = "unsupported";
        if (OperatingSystem.IsWindows()) platform = "windows";
        else if (OperatingSystem.IsLinux()) platform = "linux";
        if (manifest.Worktree != null && manifest.Platform != platform)
            throw new InvalidDataException("Worktree metadata belongs to a different platform; native reconstruction required");
        if (manifest.Worktree != null) SafeDeleteFiles.CheckGitInputs(directory, manifest);
        if (branch != null)
        {
            SafeDeleteGit.BranchName(directory, branch);
            if (SafeDeleteGit.Ref(directory, "refs/heads/" + branch) != null) throw new InvalidDataException("Restore branch collision");
        }
        if (path != null) CheckRestorePath(repository, path);
        if (manifest.ObjectFormat != SafeDeleteGit.Require(directory, "rev-parse", "--show-object-format")) throw new InvalidDataException("Repository object format differs");
        foreach (var tip in manifest.Tips)
        {
            var existing = SafeDeleteGit.Ref(directory, tip.RecoveryRef(manifest.Id));
            if (existing != null && existing != tip.Oid) throw new InvalidDataException("Recovery ref collision");
        }
        if (remote) CheckRemoteRestore(manifest);
        return (branch, path);
    }

    private static void CheckRestorePath(SafeDeleteRepository repository, string path)
    {
        SafeDeletePaths.NoLinks(path);
        if (SafeDeletePaths.Within(path, repository.Common)) throw new InvalidDataException("Restore path is inside Git administrative data");
        if (Directory.Exists(path) || File.Exists(path) || repository.ReadTrees().Any(t => SafeDeletePaths.Same(t.Path, path))) throw new InvalidDataException("Restore path collision");
    }

    private void RestoreWorktree(string entry, SafeDeleteManifest manifest, string? branch, string? path)
    {
        if (path != null)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(path)!);
            SafeDeleteGit.Require(directory, "worktree", "add", "--no-checkout", "--", path, branch ?? manifest.PrimaryOid);
            if (manifest.Worktree != null)
            {
                Reconstruct(entry, path, manifest);
                CheckReconstruction(path, manifest);
            }
            else SafeDeleteGit.Require(path, "reset", "--hard", manifest.PrimaryOid);
        }
    }

    private void WriteArchive(SafeDeleteRepository repository, SafeDeleteStore store, SafeDeleteManifest manifest, WorktreeEntry? tree)
    {
        var cwd = tree?.Path ?? directory;
        SafeDeleteHistory.Capture(cwd, manifest);
        var entry = Path.Combine(repository.Root, ".incomplete-" + manifest.Id);
        SafeDeletePaths.NoLinks(entry);
        Directory.CreateDirectory(entry);
        store.Save(entry, manifest);
        dependencies.Checkpoint("before-copy");
        SafeDeleteHistory.Write(cwd, entry, manifest);
        if (tree != null)
        {
            SafeDeleteFiles.CopyGitInputs(cwd, entry, manifest, independent: false);
            SafeDeleteFiles.Copy(tree.Path, Path.Combine(entry, "files"), manifest.Files, false);
        }
        store.HashPayloads(entry, manifest);
        store.Save(entry, manifest);
        dependencies.Checkpoint("before-proof");
        Verify(repository, entry, manifest);
        Recheck(repository, manifest, tree, dependencies);
        manifest.Complete = true;
        store.Save(entry, manifest);
        var published = Path.Combine(repository.Root, manifest.Id);
        Directory.Move(entry, published);
        output.WriteLine($"Verified archive {manifest.Id}: {published}");
        Delete(repository, store, published, manifest, tree);
        SafeDeleteStore.Describe(output, manifest);
    }

    private void Delete(SafeDeleteRepository repository, SafeDeleteStore store, string entry, SafeDeleteManifest manifest, WorktreeEntry? tree)
    {
        try
        {
            dependencies.Checkpoint("before-delete");
            Recheck(repository, manifest, tree, dependencies);
            // Revalidate persisted bytes, not just the previously verified in-memory snapshot.
            _ = store.Load(manifest.Id);
            if (manifest.RemoteOid != null)
            {
                manifest.DeleteOutcome = "remote deletion pending; local target retained"; store.Save(entry, manifest);
                dependencies.Checkpoint("before-remote-delete");
                RecheckRemote(repository, manifest);
                Recheck(repository, manifest, tree, dependencies);
                SafeDeleteGit.Require(directory, "push", $"--force-with-lease=refs/heads/{manifest.Branch}:{manifest.RemoteOid}", "--", manifest.Remote!, $":refs/heads/{manifest.Branch}");
                _gitHubTarget!.DeletedByInvocation = true;
                manifest.DeleteOutcome = "remote deleted; worktree/local branch retained"; store.Save(entry, manifest);
            }
            if (tree != null)
            {
                dependencies.Checkpoint("before-worktree-delete");
                RecheckRemote(repository, manifest);
                Recheck(repository, manifest, tree, dependencies);
                manifest.DeleteOutcome += "; worktree removal pending"; store.Save(entry, manifest);
                // No fallback recursive deletion: Git must successfully unregister exactly this worktree.
                SafeDeleteGit.Require(directory, "worktree", "remove", "--force", "--", tree.Path);
                manifest.DeleteOutcome = "remote step complete; worktree removed; local branch retained"; store.Save(entry, manifest);
            }
            if (manifest.LocalOid != null)
            {
                dependencies.Checkpoint("before-local-delete");
                RecheckRemote(repository, manifest);
                new SafeDeleteRepository(directory).Eligible(manifest.Branch, null, dependencies, false);
                if (repository.ReadTrees().Any(t => t.Branch == manifest.Branch)) throw new InvalidDataException("Branch became checked out; retained");
                if (tree == null) Recheck(repository, manifest, null, dependencies);
                else CheckBranchReflog(repository.Directory, manifest);
                manifest.DeleteOutcome += "; local deletion pending"; store.Save(entry, manifest);
                SafeDeleteGit.Require(directory, "update-ref", "--no-deref", "-d", "refs/heads/" + manifest.Branch, manifest.LocalOid);
            }
            manifest.DeleteOutcome = "complete: captured remote/worktree/local targets removed (absent components required no deletion)";
            store.Save(entry, manifest);
        }
        catch (Exception ex) when (ex is IOException or InvalidDataException or UnauthorizedAccessException)
        {
            output.WriteLine($"Partial/refused deletion: {manifest.DeleteOutcome}; verified archive retained at {entry}");
            output.WriteLine($"Remaining local={SafeDeleteGit.Ref(directory, "refs/heads/" + manifest.Branch) ?? "absent"}; worktreeExists={tree != null && Directory.Exists(tree.Path)}; remote deletion uses the recorded lease and may require inspection after transport failure");
            throw new InvalidDataException(ex.Message, ex);
        }
    }

    internal int List() => Execute(() =>
    {
        var repository = new SafeDeleteRepository(directory);
        SafeDeletePaths.NoLinks(repository.Root);
        if (!Directory.Exists(repository.Root)) { output.WriteLine("No safe-delete archives"); return; }
        foreach (var entry in Directory.EnumerateDirectories(repository.Root))
        {
            SafeDeletePaths.NoLinks(entry);
            var id = Path.GetFileName(entry);
            if (id.StartsWith('.')) { output.WriteLine($"{id}: incomplete/unpublished; retained"); continue; }
            SafeDeleteStore.Describe(output, new SafeDeleteStore(repository.Root, output).Load(id));
        }
    });

    internal int Restore(string id, string? requestedBranch, string? requestedPath, bool remote) => Execute(() =>
    {
        var repository = new SafeDeleteRepository(directory);
        var store = new SafeDeleteStore(repository.Root, output);
        SafeDeletePaths.NoLinks(repository.Root);
        if (!Directory.Exists(repository.Root)) throw new InvalidDataException("Archive root is absent");
        using var held = store.Lock();
        var manifest = store.Load(id);
        var entry = SafeDeletePaths.Child(repository.Root, id);
        var (branch, path) = PrepareRestore(repository, manifest, requestedBranch, requestedPath, remote);
        var config = new ConfigService().LoadConfigStrict(directory)?.Archive ?? new ArchiveConfig(); config.Validate();
        var allocation = Estimate(entry, manifest, false);
        CheckSpace(repository.Common, allocation, config);
        if (path != null) CheckSpace(Path.GetDirectoryName(path)!, allocation, config);
        Verify(repository, entry, manifest);
        dependencies.Checkpoint("before-restore");
        _ = store.Load(id);
        if (manifest.Worktree != null) SafeDeleteFiles.CheckGitInputs(directory, manifest);
        SafeDeleteHistory.Import(directory, entry, manifest);
        foreach (var tip in manifest.Tips)
            if (SafeDeleteGit.Ref(directory, tip.RecoveryRef(id)) == null)
                SafeDeleteGit.Require(directory, "update-ref", tip.RecoveryRef(id), tip.Oid, new string('0', tip.Oid.Length));
        if (branch != null) SafeDeleteGit.Require(directory, "update-ref", "refs/heads/" + branch, manifest.PrimaryOid, new string('0', manifest.PrimaryOid.Length));
        RestoreWorktree(entry, manifest, branch, path);
        if (remote)
        {
            CheckRemoteRestore(manifest);
            SafeDeleteGit.Require(directory, "push", $"--force-with-lease=refs/heads/{manifest.Branch}:", "--", manifest.Remote!, $"{manifest.RemoteOid}:refs/heads/{manifest.Branch}");
        }
        output.WriteLine($"Restored primary {manifest.PrimaryOid} to {branch ?? "detached HEAD"}{(path == null ? "" : " at " + path)}; archive retained");
        SafeDeleteStore.Describe(output, manifest);
    });

    private void CheckRemoteRestore(SafeDeleteManifest manifest)
    {
        if (manifest.RemoteOid == null || manifest.Remote == null || manifest.Branch == null) throw new InvalidDataException("Archive has no original same-name remote tip");
        var url = SafeDeleteGit.Require(directory, "remote", "get-url", "--push", manifest.Remote);
        if (SafeDeleteGit.Redact(url) != manifest.RemoteUrl || SafeDeleteGit.Require(directory, "remote", "get-url", manifest.Remote) != url) throw new InvalidDataException("Remote identity changed");
        var result = SafeDeleteGit.Require(directory, "ls-remote", "--", manifest.Remote, "refs/heads/" + manifest.Branch);
        if (result.Length != 0) throw new InvalidDataException("Remote restore collision: original same-name ref exists");
    }

    private void Verify(SafeDeleteRepository repository, string entry, SafeDeleteManifest manifest)
    {
        var scratch = Path.Combine(repository.Root, ".proof-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(scratch);
        try
        {
            SafeDeleteGit.Require(scratch, "-c", "init.templateDir=", "init", "-b", "dydo-proof", "--object-format=" + manifest.ObjectFormat);
            SafeDeleteHistory.Import(scratch, entry, manifest);
            foreach (var tip in manifest.Tips) SafeDeleteGit.Require(scratch, "update-ref", tip.RecoveryRef(manifest.Id), tip.Oid, new string('0', tip.Oid.Length));
            SafeDeleteGit.Require(scratch, "update-ref", "HEAD", manifest.PrimaryOid);
            SafeDeleteGit.Require(scratch, "fsck", "--full", "--strict", "--no-reflogs");
            SafeDeleteHistory.CheckDependencies(scratch, manifest);
            if (manifest.Worktree != null)
            {
                Reconstruct(entry, scratch, manifest, independent: true);
                CheckReconstruction(scratch, manifest);
            }
            output.WriteLine("Independent proof passed: fresh object store; no alternates or inherited GIT_* variables; captured history, files, index and status verified");
        }
        finally { LinkSafeDelete.DeleteTree(scratch); }
    }

    private static void Reconstruct(string entry, string path, SafeDeleteManifest manifest, bool independent = false)
    {
        if (independent) SafeDeleteFiles.CopyGitInputs(path, entry, manifest, independent: true);
        else SafeDeleteFiles.CheckGitInputs(path, manifest);
        RestoreGitConfig(path, manifest, independent);
        SafeDeleteFiles.Copy(Path.Combine(entry, "files"), path, manifest.Files, true);
        var index = SafeDeleteGit.Require(path, "rev-parse", "--path-format=absolute", "--git-path", "index");
        SafeDeletePaths.NoLinks(index);
        File.Copy(Path.Combine(entry, "index"), index, true);
        SafeDeleteFiles.RestoreRoot(path, manifest);
    }

    // The copied index's stage and resolve-undo listings must equal the manifest's, whose every
    // object Import and CheckDependencies already proved present in the fresh proof object store.
    private static void CheckReconstruction(string path, SafeDeleteManifest manifest)
    {
        if (!SafeDeleteFiles.RootMatches(path, manifest) || !SafeDeleteFiles.Capture(path).SequenceEqual(manifest.Files) || SafeDeleteGit.Status(path) != manifest.Status
            || SafeDeleteHistory.IndexListing(path, "--stage") != manifest.IndexEntries
            || SafeDeleteHistory.IndexListing(path, "--resolve-undo") != manifest.ResolveUndoEntries)
            throw new InvalidDataException("Reconstructed file/index/status snapshot differs");
    }

    private static void RestoreGitConfig(string path, SafeDeleteManifest manifest, bool independent)
    {
        var scoped = SafeDeleteGit.Read(path, "config", "--bool", "--get", "extensions.worktreeConfig").Text.Trim() == "true";
        if (!independent && !scoped)
        {
            foreach (var (key, value) in manifest.GitConfig)
                if (SafeDeleteGit.Read(path, "config", "--get", key).Text.Trim() != value)
                    throw new InvalidDataException("Saved Git settings require extensions.worktreeConfig; shared configuration retained");
            return;
        }
        var config = SafeDeleteGit.Require(path, "rev-parse", "--path-format=absolute", "--git-path", independent ? "config" : "config.worktree");
        SafeDeletePaths.NoLinks(config);
        foreach (var (key, value) in manifest.GitConfig)
            SafeDeleteGit.Require(path, "config", independent ? "--local" : "--worktree", "--replace-all", key, value);
    }

    private void RecheckRemote(SafeDeleteRepository repository, SafeDeleteManifest manifest)
    {
        if (_gitHubTarget == null) return;
        var remote = manifest.Remote!;
        var pushUrls = SafeDeleteGit.Require(directory, "remote", "get-url", "--push", "--all", remote).Split('\n');
        if (pushUrls.Length != 1 || pushUrls[0] != _gitHubTarget.Url || SafeDeleteGit.Require(directory, "remote", "get-url", remote) != _gitHubTarget.Url)
            throw new InvalidDataException("Remote identity changed");
        var expected = _gitHubTarget.DeletedByInvocation ? null : _gitHubTarget.Oid;
        if (repository.RemoteOid(remote, manifest.Branch!) != expected) throw new InvalidDataException("Remote ref changed/recreated; remaining local targets retained");
        var reason = dependencies.CheckGitHub(_gitHubTarget);
        if (reason != null) throw new InvalidDataException(reason);
        if (repository.RemoteOid(remote, manifest.Branch!) != expected) throw new InvalidDataException("Remote ref changed/recreated during eligibility check; remaining local targets retained");
    }

    private static void Recheck(SafeDeleteRepository repository, SafeDeleteManifest manifest, WorktreeEntry? tree, SafeDeleteDependencies dependencies)
    {
        var currentRepository = new SafeDeleteRepository(repository.Directory);
        if (!SafeDeletePaths.Same(currentRepository.Common, repository.Common)) throw new InvalidDataException("Repository identity changed");
        var current = tree == null ? null : currentRepository.Trees.SingleOrDefault(t => SafeDeletePaths.Same(t.Path, tree.Path));
        currentRepository.Eligible(manifest.Branch, current, dependencies, false);
        if (manifest.Branch != null && SafeDeleteGit.Ref(repository.Directory, "refs/heads/" + manifest.Branch) != manifest.LocalOid)
            throw new InvalidDataException("Local ref changed; target retained");
        SafeDeleteRepository.SupportedState(tree?.Path ?? repository.Directory);
        var history = new SafeDeleteManifest { Id = manifest.Id, LocalOid = manifest.LocalOid, RemoteOid = manifest.RemoteOid, Branch = manifest.Branch, Worktree = manifest.Worktree };
        SafeDeleteHistory.Capture(tree?.Path ?? repository.Directory, history);
        if (!history.Tips.SequenceEqual(manifest.Tips) || !history.GitMetadata.OrderBy(p => p.Key).SequenceEqual(manifest.GitMetadata.OrderBy(p => p.Key)))
            throw new InvalidDataException("Source reflog/per-worktree history changed; target retained");
        if (tree == null) return;
        if (current == null || current.Head != tree.Head || current.Branch != tree.Branch) throw new InvalidDataException("Registered worktree changed");
        SafeDeleteFiles.CheckGitInputs(tree.Path, manifest);
        if (!SafeDeleteFiles.RootMatches(tree.Path, manifest) || !SafeDeleteFiles.Capture(tree.Path).SequenceEqual(manifest.Files) || SafeDeleteGit.Status(tree.Path) != manifest.Status)
            throw new InvalidDataException("Source file/status snapshot changed; target retained");
        var index = SafeDeleteGit.Require(tree.Path, "rev-parse", "--path-format=absolute", "--git-path", "index");
        if (manifest.PayloadHashes.TryGetValue("index", out var hash) && SafeDeletePaths.Hash(index) != hash) throw new InvalidDataException("Source index changed");
    }

    private static void CheckBranchReflog(string directory, SafeDeleteManifest manifest)
    {
        var name = "logs/refs/heads/" + manifest.Branch;
        var path = SafeDeleteGit.Require(directory, "rev-parse", "--path-format=absolute", "--git-path", name);
        SafeDeletePaths.NoLinks(path);
        var current = File.Exists(path) ? Convert.ToBase64String(File.ReadAllBytes(path)) : null;
        manifest.GitMetadata.TryGetValue(name, out var captured);
        if (current != captured) throw new InvalidDataException("Source branch reflog changed; local branch retained");
    }

    private void ReportUpstream(string? branch)
    {
        if (branch == null) return;
        var upstream = SafeDeleteGit.Read(directory, "config", "--get", $"branch.{branch}.merge");
        if (upstream.Ok && upstream.Text.Trim() != "refs/heads/" + branch)
            output.WriteLine($"Differently named integration upstream {upstream.Text.Trim()} kept; only refs/heads/{branch} is targeted");
    }

    private static (long Archive, long Scratch) Estimate(string cwd, SafeDeleteManifest manifest, bool source = true)
    {
        long objects = 0;
        if (source)
        {
            var sizes = SafeDeleteGit.Require(cwd, "cat-file", "--batch-all-objects", "--batch-check=%(objectsize)");
            foreach (var size in sizes.Split('\n', StringSplitOptions.RemoveEmptyEntries)) objects = checked(objects + long.Parse(size) + 128);
            foreach (var name in new[] { "index", "HEAD", "logs/HEAD", "logs/refs/heads/" + manifest.Branch, "info/exclude", "info/attributes" })
            {
                var path = SafeDeleteGit.Require(cwd, "rev-parse", "--path-format=absolute", "--git-path", name);
                SafeDeletePaths.NoLinks(path);
                if (File.Exists(path)) objects = checked(objects + new FileInfo(path).Length * 4);
            }
        }
        else objects = Directory.EnumerateFiles(cwd, "*", SearchOption.AllDirectories).Sum(file => new FileInfo(file).Length);
        var files = manifest.Files.Sum(file => checked(file.Length + 4096));
        var amount = checked(objects * 4 + files + 1048576);
        return (amount, amount);
    }

    private void CheckSpace(string volumePath, (long Archive, long Scratch) estimates, ArchiveConfig config)
    {
        var (available, total) = dependencies.Space(volumePath);
        var reserve = Math.Max(config.ReserveBytes, Math.Ceiling(config.ReservePercent * total / 100));
        output.WriteLine($"Space {volumePath}: archiveBytes={estimates.Archive}; proofScratchBytes={estimates.Scratch}; availableBytes={available}; totalBytes={total}; reserveBytes={config.ReserveBytes}; reservePercent={config.ReservePercent}; requiredReserveBytes={reserve}");
        if (available - (double)estimates.Archive - estimates.Scratch < reserve) throw new InvalidDataException("Free-space reserve would be violated; target retained");
    }

    private int Execute(Action action)
    {
        try { action(); return 0; }
        catch (Exception ex) when (ex is IOException or InvalidDataException or UnauthorizedAccessException or JsonException or ArgumentException or InvalidOperationException or System.ComponentModel.Win32Exception or OverflowException or FormatException)
        {
            output.WriteLine($"archive: {ex.Message}; archives retained; inspect reported deletion progress before retrying");
            return 2;
        }
    }
}
