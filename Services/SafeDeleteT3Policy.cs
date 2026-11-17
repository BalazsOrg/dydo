namespace DynaDocs.Services;

using System.Text.Json;

internal sealed class SafeDeleteT3Policy(string? settingsPath = null) : ISafeDeleteT3Policy
{
    public string? Refusal(string path)
    {
        var home = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        var baseDirectory = Environment.GetEnvironmentVariable("T3CODE_HOME") ?? Path.Combine(home, ".t3");
        var roots = new List<string> { Path.Combine(home, ".t3", "worktrees"), Path.Combine(baseDirectory, "worktrees") };
        var settingsFiles = settingsPath != null ? new[] { settingsPath } : new[]
        {
            Path.Combine(home, ".t3", "userdata", "settings.json"),
            Path.Combine(baseDirectory, "userdata", "settings.json"),
            Path.Combine(baseDirectory, "dev", "settings.json")
        };
        try
        {
            if (!Path.IsPathFullyQualified(baseDirectory)) throw new InvalidDataException("T3CODE_HOME is not host-absolute");
            foreach (var settings in settingsFiles.Distinct())
            {
                SafeDeletePaths.NoLinks(settings);
                string json;
                try { json = File.ReadAllText(settings); }
                catch (Exception ex) when (ex is FileNotFoundException or DirectoryNotFoundException) { continue; }
                using var document = JsonDocument.Parse(json);
                if (document.RootElement.ValueKind != JsonValueKind.Object)
                    throw new InvalidDataException("settings must be an object");
                if (document.RootElement.TryGetProperty("worktreesDirectory", out var current))
                    roots.Add(Root(current, home, baseDirectory));
                if (document.RootElement.TryGetProperty("previousWorktreesDirectories", out var previous))
                    roots.AddRange(previous.EnumerateArray().Select(value => Root(value, home, baseDirectory)));
            }
            var normalized = Path.GetFullPath(path).Replace('\\', '/');
            foreach (var root in roots) SafeDeletePaths.NoLinks(root);
            if (normalized.Contains("/.t3/worktrees/", SafeDeletePaths.Comparison) || roots.Any(root => SafeDeletePaths.Within(path, root)))
                return "T3 ownership/activity unavailable: no supported release route with all sharing threads, provider sessions, terminals and workspace lease has passed native disposable-host proof; target retained";
            return null;
        }
        catch (Exception ex) when (ex is IOException or InvalidDataException or UnauthorizedAccessException or JsonException or InvalidOperationException or ArgumentException)
        {
            return $"T3 authoritative settings unavailable: {ex.Message}; worktree retained";
        }
    }

    private static string Root(JsonElement value, string home, string baseDirectory)
    {
        var path = value.GetString() ?? throw new InvalidDataException("null T3 worktree root");
        if (path.Length == 0) return Path.Combine(baseDirectory, "worktrees");
        if (path.StartsWith("~/", StringComparison.Ordinal) || path.StartsWith("~\\", StringComparison.Ordinal))
            path = Path.Combine(home, path[2..]);
        if (!Path.IsPathFullyQualified(path) || Path.GetFullPath(path).TrimEnd('\\', '/') == Path.GetPathRoot(path)?.TrimEnd('\\', '/'))
            throw new InvalidDataException("T3 worktree root must be an absolute non-root directory");
        SafeDeletePaths.NoLinks(path);
        return Path.GetFullPath(path);
    }
}
