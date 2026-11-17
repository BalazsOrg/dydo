namespace DynaDocs.Services;

using System.Diagnostics;
using System.Text;
using System.Text.Json;

internal static class SafeDeleteGitHub
{
    private const int MaxPages = 100;
    private const int MaxBytes = 8 * 1024 * 1024;
    private const string PullFields = "id state headRefName headRefOid headRepository { id nameWithOwner } baseRefName baseRepository { id nameWithOwner } repository { id nameWithOwner }";

    internal static string? Check(SafeDeleteGitHubTarget target) => CheckUsing(target, start => start);

    internal static string? CheckUsing(SafeDeleteGitHubTarget target, Func<ProcessStartInfo, ProcessStartInfo> startInfo)
    {
        if (target.Branch.StartsWith("feature/", StringComparison.Ordinal)) return "remote feature/* branches are protected";
        var repository = Repository(target.Url);
        if (repository == null) return "remote host is unsupported; GitHub PR eligibility is unknown";
        try
        {
            using var deadline = new CancellationTokenSource(TimeSpan.FromSeconds(30));
            var bytes = 0;
            JsonDocument Read(params string[] arguments)
            {
                var start = new ProcessStartInfo("gh") { UseShellExecute = false, RedirectStandardOutput = true, RedirectStandardError = true, RedirectStandardInput = true };
                foreach (var arg in arguments) start.ArgumentList.Add(arg);
                var result = Run(startInfo(start), deadline.Token, MaxBytes - bytes);
                bytes = checked(bytes + Encoding.UTF8.GetByteCount(result));
                return JsonDocument.Parse(result);
            }
            using var metadata = Read("api", "--hostname", "github.com", $"repos/{repository}");
            var identity = Metadata(metadata.RootElement, repository);
            if (target.Metadata != null && target.Metadata != identity) throw new InvalidDataException("GitHub repository identity/visibility/fork metadata changed");
            if (target.Oid == null && (identity.Fork || identity.Forks != 0))
                throw new InvalidDataException("Initially absent remote ref is outside closed-world authority");
            if (target.Oid != null && !target.DeletedByInvocation) Traverse(true, target, repository, identity.Id, Read);
            Traverse(false, target, repository, identity.Id, Read);
            // Metadata is read again at the end of every eligibility check, including the
            // final pre-push check; a fork appearing during pagination invalidates authority.
            using var final = Read("api", "--hostname", "github.com", $"repos/{repository}");
            if (Metadata(final.RootElement, repository) != identity) throw new InvalidDataException("GitHub repository identity/visibility/fork metadata changed");
            target.Metadata = identity;
            return null;
        }
        catch (InvalidDataException ex) { return ex.Message + "; local target retained"; }
        catch (Exception ex) when (ex is System.ComponentModel.Win32Exception or JsonException or InvalidOperationException or KeyNotFoundException or OperationCanceledException or IOException or OverflowException or FormatException or ArgumentException)
        {
            return "GitHub PR lookup unavailable, malformed or incomplete; local target retained";
        }
    }

    private static (string Id, string Visibility, bool Fork, int Forks) Metadata(JsonElement root, string repository)
    {
        var id = Text(root, "node_id");
        if (!Same(Text(root, "full_name"), repository)) throw new InvalidDataException("GitHub repository canonical identity differs");
        var visibility = Text(root, "visibility");
        var isPrivate = root.GetProperty("private").GetBoolean();
        var fork = root.GetProperty("fork").GetBoolean();
        var forks = root.GetProperty("forks_count").GetInt32();
        if (forks < 0 || isPrivate != (visibility != "public")) throw new InvalidDataException("GitHub repository metadata is inconsistent");
        if (visibility == "internal") throw new InvalidDataException("GitHub internal visibility is unsupported");
        if (visibility != "public" && visibility != "private") throw new InvalidDataException("GitHub repository visibility is unknown");
        if (visibility == "private" && (fork || forks != 0)) throw new InvalidDataException("GitHub private repository is outside closed-world authority: fork=true or forks_count>0");
        return (id, visibility, fork, forks);
    }

    private static void Traverse(bool heads, SafeDeleteGitHubTarget target, string repository, string id, Func<string[], JsonDocument> read)
    {
        string? cursor = null;
        int? count = null;
        var ids = new HashSet<string>(StringComparer.Ordinal);
        var cursors = new HashSet<string>(StringComparer.Ordinal);
        for (var page = 0; page < MaxPages; page++)
        {
            using var document = read(Query(heads, repository, target.Branch, cursor));
            var root = document.RootElement;
            if (root.TryGetProperty("errors", out var errors) && (errors.ValueKind != JsonValueKind.Array || errors.GetArrayLength() != 0))
                throw new InvalidDataException("GitHub GraphQL errors: PR eligibility is incomplete");
            var repo = root.GetProperty("data").GetProperty("repository");
            Identity(repo, id, repository);
            var reference = repo.GetProperty("ref");
            ValidateRef(reference, target, id, repository);
            var connection = heads ? reference.GetProperty("associatedPullRequests") : repo.GetProperty("pullRequests");
            cursor = CheckPage(connection, ids, cursors, ref count, pr => CheckPullRequest(pr, heads, target.Branch, id, repository));
            if (cursor == null) return;
        }
        throw new InvalidDataException("GitHub PR pagination page budget exhausted");
    }

    private static string? CheckPage(JsonElement connection, HashSet<string> ids, HashSet<string> cursors, ref int? count, Action<JsonElement> check)
    {
        var total = connection.GetProperty("totalCount").GetInt32();
        if (total < 0 || count != null && count != total) throw new InvalidDataException("GitHub PR pagination count changed");
        count = total;
        var nodes = connection.GetProperty("nodes");
        if (nodes.ValueKind != JsonValueKind.Array || nodes.GetArrayLength() > 100) throw new InvalidDataException("GitHub PR pagination nodes are incomplete");
        foreach (var pr in nodes.EnumerateArray())
        {
            if (!ids.Add(Text(pr, "id"))) throw new InvalidDataException("GitHub PR pagination repeated an identity");
            check(pr);
        }
        if (ids.Count > total) throw new InvalidDataException("GitHub PR pagination count is inconsistent");
        return NextCursor(connection.GetProperty("pageInfo"), nodes.GetArrayLength(), ids.Count, total, cursors);
    }

    private static string? NextCursor(JsonElement info, int length, int seen, int total, HashSet<string> cursors)
    {
        var next = info.GetProperty("hasNextPage").GetBoolean();
        var end = info.GetProperty("endCursor");
        if (end.ValueKind != JsonValueKind.Null && end.ValueKind != JsonValueKind.String) throw new InvalidDataException("GitHub PR pagination cursor is malformed");
        var cursor = end.GetString();
        if (length > 0 && (string.IsNullOrWhiteSpace(cursor) || !cursors.Add(cursor))) throw new InvalidDataException("GitHub PR pagination cursor is missing or repeated");
        if (!next)
        {
            if (seen != total) throw new InvalidDataException("GitHub PR pagination terminal count is incomplete");
            return null;
        }
        if (length == 0 || seen >= total) throw new InvalidDataException("GitHub PR pagination cursor/count is incomplete");
        return cursor;
    }

    private static void ValidateRef(JsonElement reference, SafeDeleteGitHubTarget target, string id, string repository)
    {
        if (target.Oid == null)
        {
            if (reference.ValueKind != JsonValueKind.Null) throw new InvalidDataException("Initially absent remote ref appeared; local target retained");
            return;
        }
        if (target.DeletedByInvocation)
        {
            if (target.Metadata == null || reference.ValueKind != JsonValueKind.Null) throw new InvalidDataException("Remote ref recreated or own-deletion receipt is invalid");
            return;
        }
        if (reference.ValueKind == JsonValueKind.Null) throw new InvalidDataException("Absent remote ref: PR association authority is unproven");
        _ = Text(reference, "id");
        Identity(reference.GetProperty("repository"), id, repository);
        if (Text(reference, "prefix") != "refs/heads/" || Text(reference, "name") != target.Branch || Text(reference.GetProperty("target"), "oid") != target.Oid)
            throw new InvalidDataException("GitHub exact branch identity/OID differs from live remote");
    }

    private static void CheckPullRequest(JsonElement pr, bool heads, string branch, string id, string repository)
    {
        if (Text(pr, "state") != "OPEN") throw new InvalidDataException("GitHub PR lookup returned inconsistent state");
        var head = pr.GetProperty("headRepository");
        _ = Text(head, "id"); _ = Text(head, "nameWithOwner");
        var headName = Text(pr, "headRefName");
        _ = Text(pr, "headRefOid");
        if ((Text(head, "id") == id) != Same(Text(head, "nameWithOwner"), repository)) throw new InvalidDataException("GitHub PR head repository identity is inconsistent");
        var baseRepo = pr.GetProperty("baseRepository");
        Identity(pr.GetProperty("repository"), Text(baseRepo, "id"), Text(baseRepo, "nameWithOwner"));
        var baseName = Text(pr, "baseRefName");
        if (heads)
        {
            Identity(head, id, repository);
            if (headName != branch) throw new InvalidDataException("GitHub exact head association is inconsistent");
            throw new InvalidDataException("remote branch is an open PR head");
        }
        Identity(baseRepo, id, repository);
        if (Text(head, "id") == id)
        {
            Identity(head, id, repository);
            if (headName == branch) throw new InvalidDataException("remote branch is an open PR head");
        }
        if (baseName == branch) throw new InvalidDataException("remote branch is an open PR base");
    }

    private static string[] Query(bool heads, string repository, string branch, string? cursor)
    {
        var connection = $"(first:100,after:$endCursor,states:[OPEN],orderBy:{{field:CREATED_AT,direction:ASC}}) {{ totalCount pageInfo {{ hasNextPage endCursor }} nodes {{ {PullFields} }} }}";
        var headField = heads ? "associatedPullRequests" + connection : "";
        var ownField = heads ? "" : "pullRequests" + connection;
        var query = "query($owner:String!,$name:String!,$qualifiedName:String!,$endCursor:String) { repository(owner:$owner,name:$name) { id nameWithOwner ref(qualifiedName:$qualifiedName) { id name prefix repository { id nameWithOwner } target { oid } " + headField + " } " + ownField + " } }";
        var names = repository.Split('/');
        return ["api", "--hostname", "github.com", "graphql", "-f", "query=" + query, "-f", "owner=" + names[0], "-f", "name=" + names[1], "-f", "qualifiedName=refs/heads/" + branch, cursor == null ? "-F" : "-f", "endCursor=" + (cursor ?? "null")];
    }

    private static string Run(ProcessStartInfo start, CancellationToken cancellation, int remaining)
    {
        using var process = Process.Start(start) ?? throw new InvalidDataException("GitHub PR lookup process did not start");
        process.StandardInput.Close();
        using var readCancellation = CancellationTokenSource.CreateLinkedTokenSource(cancellation);
        try
        {
            var output = ReadBounded(process.StandardOutput.BaseStream, remaining, readCancellation.Token);
            var error = ReadBounded(process.StandardError.BaseStream, 64 * 1024, readCancellation.Token);
            // Read errors/byte exhaustion must kill the process instead of waiting for an
            // unlimited producer to exit. No response or credential-bearing stderr is logged.
            var exit = process.WaitForExitAsync(readCancellation.Token);
            _ = AwaitReads(output, error, exit, readCancellation).GetAwaiter().GetResult();
            if (process.ExitCode != 0) throw new InvalidDataException("GitHub PR lookup failed");
            return new UTF8Encoding(false, true).GetString(output.Result);
        }
        finally
        {
            readCancellation.Cancel();
            if (!process.HasExited) { process.Kill(entireProcessTree: true); process.WaitForExit(1000); }
        }
    }

    private static async Task<bool> AwaitReads(Task<byte[]> output, Task<byte[]> error, Task exit, CancellationTokenSource cancellation)
    {
        var pending = new List<Task> { output, error, exit };
        while (pending.Count != 0)
        {
            var task = await Task.WhenAny(pending);
            try { await task; }
            catch { cancellation.Cancel(); throw; }
            pending.Remove(task);
        }
        return true;
    }

    private static async Task<byte[]> ReadBounded(Stream stream, int maximum, CancellationToken cancellation)
    {
        using var result = new MemoryStream();
        var buffer = new byte[8192];
        int length;
        while ((length = await stream.ReadAsync(buffer, cancellation)) != 0)
        {
            if (result.Length + length > maximum) throw new InvalidDataException("GitHub PR lookup response byte budget exhausted");
            result.Write(buffer, 0, length);
        }
        return result.ToArray();
    }

    private static void Identity(JsonElement value, string id, string repository)
    {
        if (Text(value, "id") != id || !Same(Text(value, "nameWithOwner"), repository)) throw new InvalidDataException("GitHub repository identity differs");
    }

    private static string Text(JsonElement value, string name)
    {
        var result = value.GetProperty(name).GetString();
        return !string.IsNullOrWhiteSpace(result) ? result : throw new InvalidDataException("GitHub required identity/name is incomplete");
    }

    private static bool Same(string first, string second) => string.Equals(first, second, StringComparison.OrdinalIgnoreCase);

    private static string? Repository(string url)
    {
        var path = url.StartsWith("git@github.com:", StringComparison.OrdinalIgnoreCase) ? url[15..] : null;
        if (Uri.TryCreate(url, UriKind.Absolute, out var uri) && uri.Host.Equals("github.com", StringComparison.OrdinalIgnoreCase)) path = uri.AbsolutePath.Trim('/');
        path = path?.TrimEnd('/');
        if (path?.EndsWith(".git", StringComparison.Ordinal) == true) path = path[..^4];
        return path != null && path.Split('/').Length == 2 && path.Split('/').All(p => p.Length > 0 && p != "." && p != "..") && path.All(c => char.IsAsciiLetterOrDigit(c) || c is '-' or '_' or '.' or '/') ? path : null;
    }
}
