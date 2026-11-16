namespace DynaDocs.Services;

internal sealed class SafeDeleteDependencies
{
    public Func<DateTimeOffset> Now { get; set; } = () => DateTimeOffset.UtcNow;
    public Func<string, string, string?> CheckGitHub { get; set; } = SafeDeleteGitHub.Check;
    public Func<string, string?> FindUse { get; set; } = WorktreeProcessProbe.FindUse;
    public ISafeDeleteT3Policy T3 { get; set; } = new SafeDeleteT3Policy();
    public Func<string, (long Available, long Total)> Space { get; set; } = path =>
    {
        // Query the actual containing volume. An unavailable mount must fail rather than
        // silently using a ready parent volume's unrelated free-space figures.
        var drive = DriveInfo.GetDrives().Where(drive => SafeDeletePaths.Within(path, drive.Name))
            .MaxBy(drive => drive.Name.Length) ?? throw new IOException("Cannot identify allocation volume");
        return (drive.AvailableFreeSpace, drive.TotalSize);
    };
    public Action<string> Checkpoint { get; set; } = _ => { };
}
