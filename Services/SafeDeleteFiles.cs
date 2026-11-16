namespace DynaDocs.Services;

using DynaDocs.Models;
using System.Runtime.InteropServices;

internal static class SafeDeleteFiles
{
    internal static List<SafeDeleteFile> Capture(string root)
    {
        SafeDeletePaths.NoLinks(root);
        var result = new List<SafeDeleteFile>();
        Walk(new DirectoryInfo(root), root, result);
        return result.OrderBy(file => file.Path, StringComparer.Ordinal).ToList();
    }

    internal static void CaptureRoot(string root, SafeDeleteManifest manifest)
    {
        ValidateKind(root, false);
        manifest.RootAttributes = (int)File.GetAttributes(root);
        manifest.RootMode = OperatingSystem.IsWindows() ? 0 : (int)File.GetUnixFileMode(root);
        manifest.RootWindowsSecurity = WindowsSecurity(root);
    }

    internal static void RestoreRoot(string root, SafeDeleteManifest manifest) => Metadata(root,
        new SafeDeleteFile("root", "directory", "", 0, manifest.RootMode, manifest.RootAttributes, null, manifest.RootWindowsSecurity));

    internal static bool RootMatches(string root, SafeDeleteManifest manifest)
    {
        var current = new SafeDeleteManifest();
        CaptureRoot(root, current);
        return current.RootMode == manifest.RootMode && (!OperatingSystem.IsWindows() || current.RootAttributes == manifest.RootAttributes)
            && current.RootWindowsSecurity == manifest.RootWindowsSecurity;
    }

    private static void Walk(DirectoryInfo directory, string root, List<SafeDeleteFile> result)
    {
        var names = new HashSet<string>(OperatingSystem.IsWindows() ? StringComparer.OrdinalIgnoreCase : StringComparer.Ordinal);
        foreach (var entry in directory.EnumerateFileSystemInfos())
        {
            if (!names.Add(entry.Name)) throw new InvalidDataException("Case-colliding files are unsupported");
            if (SkipGit(entry, directory.FullName, root)) continue;
            var path = Path.GetRelativePath(root, entry.FullName).Replace('\\', '/');
            SafeDeletePaths.Child(root, path);
            var attributes = entry.Attributes;
            var link = attributes.HasFlag(FileAttributes.ReparsePoint);
            if (OperatingSystem.IsWindows() && (link || (attributes & (FileAttributes.Device | FileAttributes.Encrypted | FileAttributes.Offline | FileAttributes.SparseFile)) != 0))
                throw new InvalidDataException($"Unsupported Windows reparse/device/encrypted/offline/sparse metadata: {path}");
            ValidateKind(entry.FullName, link);
            var mode = OperatingSystem.IsWindows() || link ? 0 : (int)File.GetUnixFileMode(entry.FullName);
            if (link)
                result.Add(new(path, "link", "", 0, 0, (int)attributes, entry.LinkTarget ?? throw new InvalidDataException("Unreadable link")));
            else if (entry is DirectoryInfo child)
            {
                result.Add(new(path, "directory", "", 0, mode, (int)attributes, null, WindowsSecurity(entry.FullName)));
                Walk(child, root, result);
            }
            else
            {
                var length = ((FileInfo)entry).Length;
                result.Add(new(path, "file", SafeDeletePaths.Hash(entry.FullName), length, mode, (int)attributes, null, WindowsSecurity(entry.FullName)));
            }
        }
    }

    private static bool SkipGit(FileSystemInfo entry, string directory, string root)
    {
        if (!entry.Name.Equals(".git", SafeDeletePaths.Comparison)) return false;
        if (SafeDeletePaths.Same(directory, root)) return true;
        throw new InvalidDataException($"Initialized nested repository is unsupported: {entry.FullName}");
    }

    private static string? WindowsSecurity(string path) => OperatingSystem.IsWindows() ? SafeDeleteWindowsMetadata.Read(path) : null;

    internal static void Copy(string source, string destination, IReadOnlyList<SafeDeleteFile> files, bool reconstruct)
    {
        SafeDeletePaths.NoLinks(destination);
        Directory.CreateDirectory(destination);
        foreach (var file in files)
        {
            var target = SafeDeletePaths.Child(destination, file.Path);
            SafeDeletePaths.NoLinks(Path.GetDirectoryName(target)!);
            Directory.CreateDirectory(Path.GetDirectoryName(target)!);
            if (file.Kind == "directory") Directory.CreateDirectory(target);
            else if (file.Kind == "file")
            {
                var original = SafeDeletePaths.Child(source, file.Path);
                SafeDeletePaths.NoLinks(original);
                File.Copy(original, target, false);
                if (SafeDeletePaths.Hash(target) != file.Hash || new FileInfo(target).Length != file.Length)
                    throw new InvalidDataException($"Copied file mismatch: {file.Path}");
                if (reconstruct) Metadata(target, file);
            }
            else if (reconstruct && file.Kind == "link")
            {
                if (OperatingSystem.IsWindows()) throw new InvalidDataException("Restoring links on Windows is unsupported");
                File.CreateSymbolicLink(target, file.LinkTarget!);
            }
        }
        if (reconstruct)
            foreach (var file in files.Where(f => f.Kind == "directory").Reverse()) Metadata(SafeDeletePaths.Child(destination, file.Path), file);
    }

    internal static void Validate(IReadOnlyList<SafeDeleteFile> files, string root)
    {
        var paths = new HashSet<string>(OperatingSystem.IsWindows() ? StringComparer.OrdinalIgnoreCase : StringComparer.Ordinal);
        foreach (var file in files)
        {
            SafeDeletePaths.Child(root, file.Path);
            if (!paths.Add(file.Path) || file.Path.Split('/').Any(part => part.Equals(".git", SafeDeletePaths.Comparison))
                || file.Kind is not ("file" or "directory" or "link") || file.Length < 0)
                throw new InvalidDataException("Invalid archive file metadata");
            if (file.Kind == "link" && (file.LinkTarget == null || OperatingSystem.IsWindows()))
                throw new InvalidDataException("Unsupported link metadata");
            var parent = Path.GetDirectoryName(file.Path)?.Replace('\\', '/');
            while (!string.IsNullOrEmpty(parent))
            {
                if (!files.Any(f => f.Path == parent && f.Kind == "directory")) throw new InvalidDataException("Archive file has an unsafe parent");
                parent = Path.GetDirectoryName(parent)?.Replace('\\', '/');
            }
        }
    }

    private static void Metadata(string path, SafeDeleteFile file)
    {
        if (!OperatingSystem.IsWindows()) File.SetUnixFileMode(path, (UnixFileMode)file.Mode);
        else
        {
            SafeDeleteWindowsMetadata.Apply(path, file.WindowsSecurity ?? throw new InvalidDataException("Windows security metadata missing"));
            File.SetAttributes(path, (FileAttributes)file.Attributes);
        }
    }

    private static void ValidateKind(string path, bool link)
    {
        if (!OperatingSystem.IsLinux()) return;
        var data = new byte[256];
        if (Statx(-100, path, 0x100, 0x7ff, data) != 0) throw new IOException($"Cannot read filesystem metadata: {path}");
        if (BitConverter.ToUInt32(data, 20) != GetEuid() || BitConverter.ToUInt32(data, 24) != GetEgid() || ListXattr(path, IntPtr.Zero, 0) != 0)
            throw new InvalidDataException($"Foreign ownership or extended metadata requires independent preservation and is unsupported: {path}");
        var kind = BitConverter.ToUInt16(data, 28) & 0xf000;
        if (kind is not (0x8000 or 0x4000 or 0xa000) || (!link && kind == 0x8000 && BitConverter.ToUInt32(data, 16) != 1))
            throw new InvalidDataException($"Special files and hard links require independent preservation and are unsupported: {path}");
    }

    [DllImport("libc", EntryPoint = "statx", SetLastError = true)]
    private static extern int Statx(int directory, [MarshalAs(UnmanagedType.LPUTF8Str)] string path, int flags, uint mask, [Out] byte[] data);

    [DllImport("libc", EntryPoint = "llistxattr", SetLastError = true)]
    private static extern long ListXattr([MarshalAs(UnmanagedType.LPUTF8Str)] string path, IntPtr list, ulong size);

    [DllImport("libc", EntryPoint = "geteuid")]
    private static extern uint GetEuid();

    [DllImport("libc", EntryPoint = "getegid")]
    private static extern uint GetEgid();
}
