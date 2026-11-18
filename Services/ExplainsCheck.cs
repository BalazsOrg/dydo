namespace DynaDocs.Services;

using DynaDocs.Models;
using DynaDocs.Utils;

internal static class ExplainsCheck
{
    public static List<Violation> Validate(List<DocFile> docs, string docsRoot)
    {
        var root = RepositoryRoot(docsRoot);
        List<string>? files = null;
        List<Violation> findings = [];
        foreach (var doc in docs)
        {
            var declaration = ExplainsDeclaration.Parse(doc.Content);
            if (declaration.Error != null)
                findings.Add(new(doc.RelativePath, "Explains", declaration.Error, ViolationSeverity.Error));
            if (declaration.Patterns.Count == 0) continue;
            files ??= Files(root);
            foreach (var pattern in declaration.Patterns.Where(pattern => !files.Any(file => ExplainsDeclaration.Matches(file, pattern))))
                findings.Add(new(doc.RelativePath, "Explains", $"explains glob '{pattern}' matches no files", ViolationSeverity.Error));
        }
        return findings;
    }

    public static List<string> Warnings(string directory, string? baseRef)
    {
        var docsRoot = PathUtils.FindDocsFolder(directory);
        if (docsRoot == null) return [];
        return Warnings(new DocScanner(new MarkdownParser()).ScanDirectory(docsRoot), docsRoot, baseRef);
    }

    public static List<string> Warnings(List<DocFile> docs, string docsRoot, string? baseRef)
    {
        var declarations = docs.Select(doc => (Doc: doc, Declaration: ExplainsDeclaration.Parse(doc.Content)))
            .Where(item => item.Declaration.Patterns.Count > 0).ToList();
        if (declarations.Count == 0) return [];
        var root = RepositoryRoot(docsRoot);
        baseRef ??= "origin/HEAD";
        var mergeBase = GitRunner.Run(root, "merge-base", "HEAD", baseRef);
        if (!mergeBase.Ok)
            return [$"explains comparison unavailable against '{baseRef}'; supply --base <merge-target>: {mergeBase.Error}"];

        // Comparing the merge base to the working tree includes committed, staged and unstaged edits.
        // No rename detection: both the old and new path must participate in the comparison.
        var diff = GitRunner.Run(root, "diff", "--name-only", "--no-renames", "-z", mergeBase.Text.Trim(), "--");
        if (!diff.Ok) return [$"explains comparison unavailable: {diff.Error}"];
        var changed = diff.Entries.ToHashSet(StringComparer.Ordinal);
        List<string> warnings = [];
        foreach (var (doc, declaration) in declarations)
        {
            var docPath = PathUtils.NormalizePath(Path.GetRelativePath(root, doc.FilePath));
            if (changed.Contains(docPath)) continue;
            var touched = changed.Where(path => declaration.Patterns.Any(pattern => ExplainsDeclaration.Matches(path, pattern)))
                .Order(StringComparer.Ordinal).ToList();
            if (touched.Count > 0)
                warnings.Add($"{docPath} explains touched paths: {string.Join(", ", touched)}; update it or state it still holds.");
        }
        return warnings;
    }

    private static string RepositoryRoot(string directory)
    {
        var git = GitRunner.Run(directory, "rev-parse", "--show-toplevel");
        return git.Ok ? git.Text.Trim() : PathUtils.FindProjectRoot(directory) ?? Path.GetDirectoryName(directory)!;
    }

    private static List<string> Files(string root)
    {
        var git = GitRunner.Run(root, "ls-files", "--cached", "--others", "--exclude-standard", "-z");
        if (git.Ok) return git.Entries.Where(path => File.Exists(Path.Combine(root, path))).ToList();
        return Directory.EnumerateFiles(root, "*", new EnumerationOptions { RecurseSubdirectories = true,
                AttributesToSkip = FileAttributes.ReparsePoint })
            .Select(path => PathUtils.NormalizePath(Path.GetRelativePath(root, path))).ToList();
    }
}
