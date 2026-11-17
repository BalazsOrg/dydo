using System.Text.RegularExpressions;

namespace DynaDocs.SkillBuild;

/// <summary>
/// Orders every prose unit into its file: kinds, then claims, in ordinal order of claim id, and a
/// claim's units in source order. A unit several claims share lands once, at its first claim.
/// </summary>
internal static partial class Placement
{
    [GeneratedRegex(@"^(?<path>.+)#L(?<line>[1-9][0-9]*)\.(?<index>[1-9][0-9]*)$")]
    private static partial Regex UnitId();

    public static Dictionary<string, List<PlacedUnit>> Place(IEnumerable<Claim> claims,
        IReadOnlyDictionary<string, string> carrierOfPath)
    {
        var files = carrierOfPath.Keys.ToDictionary(path => path, _ => new List<PlacedUnit>(), StringComparer.Ordinal);
        var placed = new Dictionary<string, PlacedUnit>(StringComparer.Ordinal);
        var ordered = claims.OrderBy(claim => claim.Kind, StringComparer.Ordinal).ThenBy(claim => claim.Id, StringComparer.Ordinal);
        foreach (var claim in ordered)
        {
            foreach (var unit in Units(claim, carrierOfPath))
            {
                if (!placed.TryGetValue(unit.Id, out var first))
                {
                    placed.Add(unit.Id, first = unit);
                    files[unit.Path].Add(unit);
                }
                else if (first.Text != unit.Text)
                    throw new InvalidDataException($"claims.json: unit {unit.Id} differs between {first.Claims[0]} and {claim.Id}");
                if (!first.Claims.Contains(claim.Id)) first.Claims.Add(claim.Id);
            }
        }
        var empty = files.FirstOrDefault(file => file.Value.Count == 0).Key;
        if (empty is not null)
            throw new InvalidDataException($"carriers.json: {empty} has no prose in claims.json");
        return files;
    }

    private static IEnumerable<PlacedUnit> Units(Claim claim, IReadOnlyDictionary<string, string> carrierOfPath)
    {
        if (!claim.Id.StartsWith(claim.Kind + ".", StringComparison.Ordinal))
            throw new InvalidDataException($"claims.json: claim {claim.Id} is not of its kind {claim.Kind}");
        var units = claim.Prose
            .SelectMany(carrier => carrier.Value.Select(entry => Unit(claim, carrier.Key, entry, carrierOfPath)))
            .OrderBy(unit => unit.Line).ThenBy(unit => unit.Index).ToArray();
        if (units.Length == 0)
            throw new InvalidDataException($"claims.json: claim {claim.Id} has no prose");
        return units;
    }

    private static PlacedUnit Unit(Claim claim, string carrier, ProseEntry entry,
        IReadOnlyDictionary<string, string> carrierOfPath)
    {
        var match = UnitId().Match(entry.Unit);
        var path = match.Groups["path"].Value;
        if (!match.Success || entry.Source != $"{path}:{match.Groups["line"].Value}")
            throw new InvalidDataException($"claims.json: claim {claim.Id} unit '{entry.Unit}' does not match source '{entry.Source}'");
        if (!carrierOfPath.TryGetValue(path, out var owner) || owner != carrier)
            throw new InvalidDataException($"carriers.json: carrier {carrier} file {path} (claim {claim.Id}) is missing from the manifest");
        if (string.IsNullOrWhiteSpace(entry.Text))
            throw new InvalidDataException($"claims.json: claim {claim.Id} unit {entry.Unit} has no prose");
        return new PlacedUnit(entry.Unit, path, int.Parse(match.Groups["line"].Value), int.Parse(match.Groups["index"].Value),
            entry.Text, claim.Kind);
    }
}
