namespace DynaDocs.Models;

internal sealed record SafeDeleteTip(string Id, string Oid, string Source)
{
    public string BundleRef { get; init; } = "";
    public string RecoveryRef(string archiveId) => $"refs/dydo-restored/{archiveId}/{Id}";
}
