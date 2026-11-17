namespace DynaDocs.Services;

internal sealed class SafeDeleteGitHubTarget(string url, string branch, string? oid)
{
    internal string Url { get; } = url;
    internal string Branch { get; } = branch;
    internal string? Oid { get; } = oid;
    internal (string Id, string Visibility, bool Fork, int Forks)? Metadata { get; set; }
    // Set only after this invocation's expected-OID leased push succeeds. Initial
    // absence never supplies the pre-deletion head-association proof.
    internal bool DeletedByInvocation { get; set; }
}
