namespace DynaDocs.Models;

using System.Text.Json.Serialization;

public sealed class ArchiveConfig
{
    [JsonPropertyName("reserveBytes")]
    public long ReserveBytes { get; set; } = 5368709120;

    [JsonPropertyName("reservePercent")]
    public double ReservePercent { get; set; } = 10;

    internal void Validate()
    {
        if (ReserveBytes < 0 || !double.IsFinite(ReservePercent) || ReservePercent is < 0 or > 100)
            throw new InvalidDataException("archive.reserveBytes must be nonnegative integral bytes; archive.reservePercent must be finite and between 0 and 100.");
    }
}
