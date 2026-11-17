namespace DynaDocs.Services;

using DynaDocs.Models;
using System.Diagnostics;
using System.Text;

internal static class SafeDeleteHistory
{
    internal static void Capture(string cwd, SafeDeleteManifest manifest)
    {
        var tips = new SortedDictionary<string, List<string>>(StringComparer.Ordinal);
        void Add(string? oid, string source)
        {
            if (oid == null || oid.All(c => c == '0')) return;
            if (!tips.TryGetValue(oid, out var sources)) tips.Add(oid, sources = []);
            sources.Add(source);
        }
        Add(manifest.LocalOid, "local"); Add(manifest.RemoteOid, "same-name remote");
        if (manifest.Worktree != null) Add(SafeDeleteGit.Require(cwd, "rev-parse", "HEAD"), "worktree HEAD");
        var names = new List<string>();
        if (manifest.Branch != null) names.Add("logs/refs/heads/" + manifest.Branch);
        if (manifest.Worktree != null) names.AddRange(["HEAD", "logs/HEAD"]);
        foreach (var name in names)
        {
            var path = SafeDeleteGit.Require(cwd, "rev-parse", "--path-format=absolute", "--git-path", name);
            SafeDeletePaths.NoLinks(path);
            if (!File.Exists(path)) continue;
            var bytes = File.ReadAllBytes(path);
            manifest.GitMetadata[name] = Convert.ToBase64String(bytes);
            if (!name.StartsWith("logs/", StringComparison.Ordinal)) continue;
            ReadReflog(bytes, name, Add);
        }
        if (manifest.Worktree != null) CaptureWorktreeRefs(cwd, manifest, Add);
        manifest.Tips = tips.Select((pair, index) => new SafeDeleteTip($"tip-{index:D4}", pair.Key, string.Join(", ", pair.Value.Distinct()))
        {
            BundleRef = $"refs/dydo-archive-staging/{manifest.Id}/tip-{index:D4}"
        }).ToList();
        if (manifest.Tips.Count == 0) throw new InvalidDataException("Empty commit archive is forbidden");
        foreach (var tip in manifest.Tips)
            if (SafeDeleteGit.Require(cwd, "cat-file", "-t", tip.Oid) != "commit") throw new InvalidDataException("Captured tip is not a commit");
    }

    private static void CaptureWorktreeRefs(string cwd, SafeDeleteManifest manifest, Action<string?, string> add)
    {
        foreach (var line in SafeDeleteGit.Require(cwd, "for-each-ref", "--format=%(objectname) %(refname)", "refs/worktree", "refs/bisect", "refs/rewritten").Split('\n', StringSplitOptions.RemoveEmptyEntries))
        {
            var words = line.Split(' ', 2);
            add(words[0], words[1]); manifest.GitMetadata[words[1]] = words[0];
        }
    }

    private static void ReadReflog(byte[] bytes, string name, Action<string?, string> add)
    {
        foreach (var line in Encoding.UTF8.GetString(bytes).Split('\n', StringSplitOptions.RemoveEmptyEntries))
        {
            var words = line.Split(' ');
            if (words.Length < 2) throw new InvalidDataException("Malformed reflog");
            add(words[0], name); add(words[1], name);
        }
    }

    internal static void Write(string cwd, string entry, SafeDeleteManifest manifest)
    {
        var created = new List<(string Ref, string Oid)>();
        try
        {
            foreach (var tip in manifest.Tips)
            {
                var name = tip.BundleRef;
                SafeDeleteGit.Require(cwd, "update-ref", name, tip.Oid, new string('0', tip.Oid.Length));
                created.Add((name, tip.Oid));
            }
            SafeDeleteGit.Require(cwd, ["bundle", "create", Path.Combine(entry, "commits.bundle"), .. created.Select(t => t.Ref)]);
        }
        finally
        {
            foreach (var (name, oid) in created) SafeDeleteGit.Read(cwd, "update-ref", "--no-deref", "-d", name, oid);
        }
        if (manifest.Worktree == null) return;
        var index = SafeDeleteGit.Require(cwd, "rev-parse", "--path-format=absolute", "--git-path", "index");
        SafeDeletePaths.NoLinks(index);
        var shared = SafeDeleteGit.Require(cwd, "rev-parse", "--shared-index-path");
        if (!string.IsNullOrEmpty(shared)) throw new InvalidDataException("Split index is unsupported; expand the index first");
        SafeDeletePaths.Write(Path.Combine(entry, "index"), File.ReadAllBytes(index));
        manifest.IndexEntries = IndexListing(cwd, "--stage");
        manifest.ResolveUndoEntries = IndexListing(cwd, "--resolve-undo");
        var oids = IndexObjects(manifest);
        SafeDeleteGit.Stream(cwd, string.Concat(oids.Select(oid => oid + "\n")), null, Path.Combine(entry, "index-objects.pack"), "pack-objects", "--stdout");
    }

    // The index's blob dependencies: every current stage and every resolve-undo stage. A resolved
    // conflict's undo record can be the only reference to a version, such as a dropped stash's.
    internal static string IndexListing(string cwd, string kind)
    {
        var result = SafeDeleteGit.Read(cwd, "ls-files", kind, "-z");
        if (!result.Ok) throw new InvalidDataException("Cannot read index entries");
        return Convert.ToBase64String(result.Output);
    }

    internal static string[] IndexObjects(SafeDeleteManifest manifest) =>
        ListedObjects(manifest.IndexEntries).Concat(ListedObjects(manifest.ResolveUndoEntries)).Distinct().ToArray();

    private static IEnumerable<string> ListedObjects(string encoded)
    {
        foreach (var entry in Encoding.UTF8.GetString(Convert.FromBase64String(encoded)).Split('\0', StringSplitOptions.RemoveEmptyEntries))
        {
            var fields = entry.Split(' ', 3);
            if (fields.Length != 3 || fields[0] == "160000") throw new InvalidDataException("Gitlink/submodule index requires independent preservation and is unsupported");
            yield return fields[1];
        }
    }

    internal static void Import(string cwd, string entry, SafeDeleteManifest manifest)
    {
        var heads = SafeDeleteGit.Require(cwd, "bundle", "list-heads", Path.Combine(entry, "commits.bundle"))
            .Split('\n', StringSplitOptions.RemoveEmptyEntries).Order(StringComparer.Ordinal);
        if (!heads.SequenceEqual(manifest.Tips.Select(t => t.Oid + " " + t.BundleRef).Order(StringComparer.Ordinal)))
            throw new InvalidDataException("Bundle tip mapping differs from manifest");
        SafeDeleteGit.Require(cwd, "bundle", "verify", Path.Combine(entry, "commits.bundle"));
        SafeDeleteGit.Require(cwd, "bundle", "unbundle", Path.Combine(entry, "commits.bundle"));
        if (manifest.Worktree != null)
        {
            var output = Path.Combine(entry, ".index-import-" + Guid.NewGuid().ToString("N"));
            try { SafeDeleteGit.Stream(cwd, null, Path.Combine(entry, "index-objects.pack"), output, "index-pack", "--stdin"); }
            finally { if (File.Exists(output)) File.Delete(output); }
        }
        foreach (var tip in manifest.Tips)
            if (SafeDeleteGit.Require(cwd, "cat-file", "-t", tip.Oid) != "commit") throw new InvalidDataException("Archived commit missing");
        foreach (var oid in IndexObjects(manifest)) SafeDeleteGit.Require(cwd, "cat-file", "-e", oid);
    }

    internal static void CheckDependencies(string cwd, SafeDeleteManifest manifest)
    {
        var objects = SafeDeleteGit.Require(cwd, ["rev-list", "--objects", "--no-object-names", .. manifest.Tips.Select(t => t.Oid)]).Split('\n', StringSplitOptions.RemoveEmptyEntries)
            .Concat(IndexObjects(manifest)).Distinct().ToArray();
        using var process = Process.Start(GitRunner.IsolatedStartInfo(cwd, SafeDeleteGit.Environment, "cat-file", "--batch"))!;
        var errors = process.StandardError.ReadToEndAsync();
        var input = Task.Run(async () =>
        {
            foreach (var oid in objects) await process.StandardInput.WriteLineAsync(oid);
            process.StandardInput.Close();
        });
        string? failure = null;
        foreach (var _ in objects)
        {
            var header = Header(process.StandardOutput.BaseStream).Split(' ');
            if (header.Length != 3 || !long.TryParse(header[2], out var remaining)) throw new InvalidDataException("Missing archived object");
            var objectFailure = InspectObject(process.StandardOutput.BaseStream, header[1], remaining);
            failure ??= objectFailure;
            if (process.StandardOutput.BaseStream.ReadByte() != '\n') throw new InvalidDataException("Malformed Git batch output");
        }
        process.WaitForExit(); input.GetAwaiter().GetResult(); _ = errors.Result;
        if (process.ExitCode != 0 || failure != null) throw new InvalidDataException(failure ?? "Cannot verify objects");
    }

    private static string? InspectObject(Stream stream, string type, long length)
    {
        var buffer = new byte[65536];
        var prefix = "";
        var remaining = length;
        string? failure = null;
        while (remaining > 0)
        {
            var count = stream.Read(buffer, 0, (int)Math.Min(remaining, buffer.Length));
            if (count == 0) throw new InvalidDataException("Truncated Git object");
            var text = Encoding.UTF8.GetString(buffer, 0, count);
            if (type == "blob" && remaining == length && text.StartsWith("version https://git-lfs.github.com/spec/v1", StringComparison.Ordinal)) failure = "LFS pointer history requires independently preserved LFS data; unsupported";
            if (type == "tree" && (prefix + text).Contains("160000 ", StringComparison.Ordinal)) failure = "Gitlink history requires independently preserved submodules; unsupported";
            prefix = text.Length > 10 ? text[^10..] : text;
            remaining -= count;
        }
        return failure;
    }

    private static string Header(Stream stream)
    {
        var bytes = new List<byte>();
        for (var value = stream.ReadByte(); value >= 0 && value != '\n'; value = stream.ReadByte())
        {
            if (bytes.Count > 200) throw new InvalidDataException("Invalid Git batch header");
            bytes.Add((byte)value);
        }
        return Encoding.ASCII.GetString(bytes.ToArray());
    }
}
