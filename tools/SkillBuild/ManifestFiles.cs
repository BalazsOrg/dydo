namespace DynaDocs.SkillBuild;

/// <summary>
/// Turns claims/carriers.json into the files to build, refusing any entry the layout cannot place:
/// a carrier is one spine with resources under its <c>resources/</c> folder, or one system doc.
/// </summary>
internal static class ManifestFiles
{
    public static IReadOnlyList<BuiltFileSpec> Read(CarrierManifest manifest)
    {
        var specs = new List<BuiltFileSpec>();
        var carriers = new HashSet<string>(StringComparer.Ordinal);
        var paths = new HashSet<string>(StringComparer.Ordinal);
        foreach (var carrier in manifest.Carriers)
        {
            if (string.IsNullOrWhiteSpace(carrier.Carrier) || !carriers.Add(carrier.Carrier))
                throw new InvalidDataException($"carriers.json: blank or duplicate carrier '{carrier.Carrier}'");
            foreach (var file in carrier.Files)
            {
                CheckFile(carrier.Carrier, file);
                if (!paths.Add(file.Path))
                    throw new InvalidDataException($"carriers.json: {file.Path} is listed twice");
            }
            specs.AddRange(Specs(carrier));
        }
        return specs;
    }

    private static void CheckFile(string carrier, CarrierFile file)
    {
        if (string.IsNullOrWhiteSpace(file.Title))
            throw new InvalidDataException($"carriers.json: {carrier} {file.Path} has no title");
        var valid = file.IsSpine
            ? !string.IsNullOrWhiteSpace(file.Name) && !string.IsNullOrWhiteSpace(file.Description)
            : file.Name is null && file.Description is null;
        if (!valid)
            throw new InvalidDataException(
                $"carriers.json: {carrier} {file.Path} needs a name and description exactly when it is a SKILL.md");
    }

    private static IEnumerable<BuiltFileSpec> Specs(CarrierEntry carrier)
    {
        var spines = carrier.Files.Where(file => file.IsSpine).ToArray();
        if (spines.Length == 0 && carrier.Files.Count == 1)
            return [new BuiltFileSpec(carrier.Carrier, carrier.Files[0], [])];
        if (spines.Length != 1)
            throw new InvalidDataException(
                $"carriers.json: {carrier.Carrier} must hold one SKILL.md with its resources, or one system doc");
        var spine = spines[0];
        var resources = carrier.Files.Where(file => file != spine).ToArray();
        var folder = Path.GetDirectoryName(spine.Path)!.Replace('\\', '/') + "/resources/";
        var stray = resources.FirstOrDefault(file => !file.Path.StartsWith(folder, StringComparison.Ordinal)
            || file.Path[folder.Length..].Contains('/') || !file.Path.EndsWith(".md", StringComparison.Ordinal));
        if (stray is not null)
            throw new InvalidDataException($"carriers.json: {stray.Path} is not a resources/*.md of {spine.Path}");
        return [new BuiltFileSpec(carrier.Carrier, spine, resources),
                ..resources.Select(file => new BuiltFileSpec(carrier.Carrier, file, []))];
    }
}
