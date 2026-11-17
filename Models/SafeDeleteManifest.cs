namespace DynaDocs.Models;

internal sealed class SafeDeleteManifest
{
    public int Version { get; set; } = 1;
    public string Id { get; set; } = "";
    public DateTimeOffset Created { get; set; }
    public DateTimeOffset Expires { get; set; }
    public bool Complete { get; set; }
    public string Repository { get; set; } = "";
    public string ObjectFormat { get; set; } = "sha1";
    public string? Worktree { get; set; }
    public string? Branch { get; set; }
    public string? LocalOid { get; set; }
    public string? Remote { get; set; }
    public string? RemoteUrl { get; set; }
    public string? RemoteOid { get; set; }
    public string? GitHubRepositoryId { get; set; }
    public string PrimaryOid { get; set; } = "";
    public List<SafeDeleteTip> Tips { get; set; } = [];
    public List<SafeDeleteFile> Files { get; set; } = [];
    public Dictionary<string, string> PayloadHashes { get; set; } = [];
    public Dictionary<string, string> GitMetadata { get; set; } = [];
    public Dictionary<string, string> GitConfig { get; set; } = [];
    public string Status { get; set; } = "";
    public string Platform { get; set; } = OperatingSystem.IsWindows() ? "windows" : "linux";
    public int RootMode { get; set; }
    public int RootAttributes { get; set; }
    public string? RootWindowsSecurity { get; set; }
    public string IndexEntries { get; set; } = "";
    public string DeleteOutcome { get; set; } = "not started";
}
