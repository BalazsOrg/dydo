namespace DynaDocs.Services;

using System.Diagnostics;
using System.Text;

internal static class SafeDeleteGit
{
    internal static readonly IReadOnlyDictionary<string, string> Environment = new Dictionary<string, string>();

    internal static GitResult Read(string cwd, params string[] args) => GitRunner.RunIsolated(cwd, Environment, args);

    internal static string Require(string cwd, params string[] args)
    {
        var result = Read(cwd, args);
        if (!result.Ok) throw new InvalidDataException($"git {args[0]} failed; target retained where not already reported deleted");
        return result.Text.TrimEnd('\r', '\n');
    }

    internal static string? Ref(string cwd, string name)
    {
        var result = Read(cwd, "show-ref", "--verify", "--hash", name);
        if (result.Ok) return result.Text.Trim();
        if (result.ExitCode != 1 && result.ExitCode != 128) throw new InvalidDataException("Cannot read Git ref");
        return null;
    }

    internal static void BranchName(string cwd, string branch)
    {
        if (branch.StartsWith('-') || branch.Any(char.IsControl) || !Read(cwd, "check-ref-format", "refs/heads/" + branch).Ok)
            throw new InvalidDataException("Invalid branch name");
    }

    internal static void Stream(string cwd, string? textInput, string? inputFile, string outputFile, params string[] args)
    {
        using var process = Process.Start(GitRunner.IsolatedStartInfo(cwd, Environment, args))!;
        var errors = process.StandardError.ReadToEndAsync();
        var writing = Task.Run(async () =>
        {
            try
            {
                if (inputFile != null)
                {
                    await using var input = File.OpenRead(inputFile);
                    await input.CopyToAsync(process.StandardInput.BaseStream);
                }
                else if (textInput != null) await process.StandardInput.WriteAsync(textInput);
            }
            finally { process.StandardInput.Close(); }
        });
        using (var output = new FileStream(outputFile, FileMode.CreateNew, FileAccess.Write, FileShare.None))
        {
            process.StandardOutput.BaseStream.CopyTo(output);
            output.Flush(true);
        }
        process.WaitForExit();
        try { writing.GetAwaiter().GetResult(); }
        catch (IOException) when (process.ExitCode != 0) { }
        _ = errors.Result;
        if (process.ExitCode != 0) throw new InvalidDataException($"git {args[0]} stream failed");
    }

    internal static string Status(string cwd) => Convert.ToBase64String(ReadStatus(cwd).Output);

    private static GitResult ReadStatus(string cwd)
    {
        var result = Read(cwd, "status", "--porcelain=v1", "-z", "--untracked-files=all", "--ignored=matching", "--no-renames");
        if (!result.Ok) throw new InvalidDataException("Cannot capture Git status");
        return result;
    }

    internal static string Redact(string url)
    {
        if (!Uri.TryCreate(url, UriKind.Absolute, out var uri)) return url;
        if (uri.IsFile) return url;
        var builder = new UriBuilder(uri) { UserName = "", Password = "", Query = "", Fragment = "" };
        return builder.Uri.ToString();
    }
}
