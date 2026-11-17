namespace DynaDocs.Services;

using System.Diagnostics;
using System.Text.Json;

internal static class SafeDeleteGitHub
{
    internal static string? Check(string remoteUrl, string branch)
        => CheckUsing(remoteUrl, branch, StartInfo);

    internal static string? CheckUsing(string remoteUrl, string branch, Func<string, ProcessStartInfo> startInfo)
    {
        if (branch.StartsWith("feature/", StringComparison.Ordinal)) return "remote feature/* branches are protected";
        var repository = Repository(remoteUrl);
        if (repository == null) return "remote host is unsupported; GitHub PR eligibility is unknown";
        try
        {
            var start = startInfo(repository);
            using var process = Process.Start(start)!;
            process.StandardInput.Close();
            var output = process.StandardOutput.ReadToEndAsync();
            var error = process.StandardError.ReadToEndAsync();
            process.WaitForExit();
            if (process.ExitCode != 0) return "GitHub PR lookup failed; local target retained";
            _ = error.Result;
            using var doc = JsonDocument.Parse(output.Result);
            return CheckPages(doc.RootElement, repository, branch);
        }
        catch (Exception ex) when (ex is System.ComponentModel.Win32Exception or JsonException or InvalidOperationException or KeyNotFoundException)
        {
            return "GitHub PR lookup unavailable or incomplete; local target retained";
        }
    }

    private static ProcessStartInfo StartInfo(string repository)
    {
        var start = new ProcessStartInfo("gh") { UseShellExecute = false, RedirectStandardOutput = true, RedirectStandardError = true, RedirectStandardInput = true };
        foreach (var arg in new[] { "api", "--paginate", "--slurp", $"repos/{repository}/pulls?state=open&per_page=100" }) start.ArgumentList.Add(arg);
        return start;
    }

    private static string? CheckPages(JsonElement root, string repository, string branch)
    {
        var pages = root.EnumerateArray().ToArray();
        if (pages.Length == 0) return "GitHub PR lookup was incomplete";
        foreach (var page in pages)
            foreach (var pr in page.EnumerateArray())
            {
                var refusal = CheckPullRequest(pr, repository, branch);
                if (refusal != null) return refusal;
            }
        // Repository pull lists omit heads used in other repositories. Search indexing is
        // not exhaustive proof either, including across private repositories/fork networks.
        return "cross-repository PR head eligibility is unproven; local target retained";
    }

    private static string? CheckPullRequest(JsonElement pr, string repository, string branch)
    {
        if (pr.GetProperty("state").GetString() != "open") return "GitHub PR lookup returned inconsistent state";
        foreach (var side in new[] { "head", "base" })
        {
            var candidate = pr.GetProperty(side);
            var repo = candidate.GetProperty("repo");
            if (repo.ValueKind == JsonValueKind.Null) return "GitHub PR repository identity unavailable";
            var reference = candidate.GetProperty("ref").GetString();
            var identity = repo.GetProperty("full_name").GetString();
            if (string.IsNullOrWhiteSpace(reference) || string.IsNullOrWhiteSpace(identity)) return "GitHub PR lookup was incomplete";
            if (reference == branch && string.Equals(identity, repository, StringComparison.OrdinalIgnoreCase))
                return $"remote branch is an open PR {side}";
        }
        return null;
    }

    private static string? Repository(string url)
    {
        var path = url.StartsWith("git@github.com:", StringComparison.OrdinalIgnoreCase) ? url[15..] : null;
        if (Uri.TryCreate(url, UriKind.Absolute, out var uri) && uri.Host.Equals("github.com", StringComparison.OrdinalIgnoreCase)) path = uri.AbsolutePath.Trim('/');
        path = path?.TrimEnd('/');
        if (path?.EndsWith(".git", StringComparison.Ordinal) == true) path = path[..^4];
        return path != null && path.Split('/').Length == 2 && path.All(c => char.IsAsciiLetterOrDigit(c) || c is '-' or '_' or '.' or '/') ? path : null;
    }
}
