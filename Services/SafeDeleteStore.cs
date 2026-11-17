namespace DynaDocs.Services;

using DynaDocs.Models;
using DynaDocs.Serialization;
using System.Text;
using System.Text.Json;

internal sealed class SafeDeleteStore(string root, TextWriter output)
{
    internal FileStream Lock()
    {
        SafeDeletePaths.NoLinks(root);
        Directory.CreateDirectory(root);
        var path = Path.Combine(root, ".lock");
        SafeDeletePaths.NoLinks(path);
        return new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None);
    }

    internal void Save(string entry, SafeDeleteManifest manifest)
    {
        var path = Path.Combine(entry, "manifest.json");
        var temporary = Path.Combine(entry, ".manifest-" + Guid.NewGuid().ToString("N"));
        SafeDeletePaths.Write(temporary, JsonSerializer.SerializeToUtf8Bytes(manifest, SafeDeleteJsonContext.Default.SafeDeleteManifest));
        SafeDeletePaths.NoLinks(path);
        File.Move(temporary, path, true);
    }

    internal SafeDeleteManifest Load(string id, bool complete = true)
    {
        var entry = SafeDeletePaths.Child(root, id);
        if (id.Contains('/')) throw new InvalidDataException("Archive ID cannot contain separators");
        CheckContainer(entry);
        var manifest = JsonSerializer.Deserialize(File.ReadAllBytes(Path.Combine(entry, "manifest.json")), SafeDeleteJsonContext.Default.SafeDeleteManifest)
            ?? throw new InvalidDataException("Empty manifest");
        ValidateFields(manifest);
        if (manifest.Version != 1 || manifest.Id != id || complete && !manifest.Complete || manifest.Tips.Count == 0
            || manifest.Created.Offset != TimeSpan.Zero || manifest.Expires != manifest.Created.AddDays(30)
            || manifest.ObjectFormat is not ("sha1" or "sha256")) throw new InvalidDataException("Invalid/incomplete archive manifest");
        var width = manifest.ObjectFormat == "sha256" ? 64 : 40;
        if (!Oid(manifest.PrimaryOid, width) || manifest.Tips.Any(t => !Oid(t.Oid, width) || !t.Id.StartsWith("tip-", StringComparison.Ordinal) || t.Id.Any(c => !char.IsAsciiLetterOrDigit(c) && c != '-'))
            || manifest.Tips.Select(t => t.Id).Distinct().Count() != manifest.Tips.Count
            || !manifest.Tips.Any(t => t.Oid == manifest.PrimaryOid)) throw new InvalidDataException("Invalid archive tip mapping");
        if (manifest.Tips.Any(t => t.BundleRef != $"refs/dydo-archive-staging/{id}/{t.Id}")) throw new InvalidDataException("Invalid bundle ref mapping");
        SafeDeleteFiles.Validate(manifest.Files, entry);
        foreach (var (relative, hash) in manifest.PayloadHashes)
        {
            var file = SafeDeletePaths.Child(entry, relative);
            SafeDeletePaths.NoLinks(file);
            if (SafeDeletePaths.Hash(file) != hash) throw new InvalidDataException($"Corrupt archive payload: {relative}");
        }
        CheckRequired(entry, manifest);
        return manifest;
    }

    private static void CheckRequired(string entry, SafeDeleteManifest manifest)
    {
        foreach (var required in manifest.Worktree == null ? new[] { "commits.bundle" } : new[] { "commits.bundle", "index", "index-objects.pack" })
            if (!manifest.PayloadHashes.ContainsKey(required) || new FileInfo(Path.Combine(entry, required)).Length == 0) throw new InvalidDataException("Empty/missing archive payload");
        foreach (var file in manifest.Files.Where(f => f.Kind == "file"))
            if (!manifest.PayloadHashes.TryGetValue("files/" + file.Path, out var hash) || hash != file.Hash) throw new InvalidDataException("Unverified file payload");
    }

    private static bool Oid(string? value, int width) => value != null && value.Length == width && value.All(char.IsAsciiHexDigit);

    private static void ValidateFields(SafeDeleteManifest manifest)
    {
        if (manifest.Tips == null || manifest.Files == null || manifest.PayloadHashes == null || manifest.GitMetadata == null || manifest.GitConfig == null
            || manifest.Tips.Any(t => t == null || t.Id == null) || manifest.Files.Any(f => f == null)
            || manifest.Status == null || manifest.IndexEntries == null || manifest.ResolveUndoEntries == null)
            throw new InvalidDataException("Null archive metadata");
        var keys = new[] { "core.filemode", "core.ignorecase", "core.symlinks", "core.autocrlf", "core.eol" };
        if (manifest.GitConfig.Any(pair => !keys.Contains(pair.Key, StringComparer.Ordinal) || pair.Value == null || pair.Value.Any(char.IsControl)))
            throw new InvalidDataException("Unsupported archive Git configuration");
        var width = manifest.ObjectFormat == "sha256" ? 64 : 40;
        if (manifest.LocalOid != null && !Oid(manifest.LocalOid, width) || manifest.RemoteOid != null && !Oid(manifest.RemoteOid, width))
            throw new InvalidDataException("Invalid original tip");
        if (manifest.Worktree == null && manifest.PrimaryOid != (manifest.LocalOid ?? manifest.RemoteOid))
            throw new InvalidDataException("Invalid primary branch mapping");
    }

    internal static void CheckContainer(string path)
    {
        SafeDeletePaths.NoLinks(path);
        foreach (var entry in new DirectoryInfo(path).EnumerateFileSystemInfos())
        {
            if (LinkSafeDelete.IsLink(entry)) throw new InvalidDataException("Linked archive entry refused");
            if (entry is DirectoryInfo directory) CheckContainer(directory.FullName);
        }
    }

    internal void HashPayloads(string entry, SafeDeleteManifest manifest)
    {
        CheckContainer(entry);
        foreach (var path in Directory.EnumerateFiles(entry, "*", SearchOption.AllDirectories))
        {
            var relative = Path.GetRelativePath(entry, path).Replace('\\', '/');
            if (relative != "manifest.json") manifest.PayloadHashes[relative] = SafeDeletePaths.Hash(path);
        }
    }

    internal void Purge(DateTimeOffset now)
    {
        foreach (var entry in Directory.EnumerateDirectories(root))
        {
            var id = Path.GetFileName(entry);
            if (id.StartsWith('.')) continue;
            try
            {
                var manifest = Load(id);
                if (manifest.Expires < now)
                {
                    CheckContainer(entry);
                    LinkSafeDelete.DeleteTree(entry);
                    output.WriteLine($"Purged expired archive {id}");
                }
            }
            catch (Exception ex) when (ex is IOException or InvalidDataException or UnauthorizedAccessException or JsonException)
            {
                output.WriteLine($"Retained unrecognized/incomplete archive {id}: {ex.Message}");
            }
        }
    }

    internal static void Describe(TextWriter writer, SafeDeleteManifest manifest)
    {
        writer.WriteLine($"{manifest.Id}: {manifest.Worktree ?? manifest.Branch}; created {manifest.Created:O}; expires {manifest.Expires:O}; complete={manifest.Complete}; {manifest.DeleteOutcome}");
        foreach (var tip in manifest.Tips) writer.WriteLine($"  {tip.Id} {tip.Oid} bundle={tip.BundleRef} restore={tip.RecoveryRef(manifest.Id)} ({tip.Source})");
    }
}
