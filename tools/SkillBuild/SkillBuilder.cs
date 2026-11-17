using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace DynaDocs.SkillBuild;

/// <summary>
/// The build: claims.json and carriers.json bytes in, the built tree out, keyed by path under
/// <c>claims/built/</c>. Any missing or inconsistent input throws <see cref="InvalidDataException"/>;
/// the build never emits a silent empty.
/// </summary>
public static class SkillBuilder
{
    public const string MapPath = "claims.map.json";

    private static readonly UTF8Encoding Utf8 = new(encoderShouldEmitUTF8Identifier: false);

    public static IReadOnlyDictionary<string, byte[]> Build(byte[] claimsJson, byte[] carriersJson)
    {
        var inventory = Read<ClaimInventory>(claimsJson, "claims.json", JsonUnmappedMemberHandling.Skip);
        var manifest = Read<CarrierManifest>(carriersJson, "carriers.json", JsonUnmappedMemberHandling.Disallow);
        if (manifest.Base != inventory.Base)
            throw new InvalidDataException($"carriers.json: base {manifest.Base} is not the inventory's base {inventory.Base}");
        var specs = ManifestFiles.Read(manifest);
        var units = Placement.Place(inventory.Claims, specs.ToDictionary(spec => spec.File.Path, spec => spec.Carrier, StringComparer.Ordinal));
        var files = new SortedDictionary<string, RenderedFile>(StringComparer.Ordinal);
        foreach (var spec in specs) files.Add(spec.File.Path, FileRenderer.Render(spec, units[spec.File.Path]));
        var tree = files.ToDictionary(file => file.Key, file => Utf8.GetBytes(file.Value.Text), StringComparer.Ordinal);
        tree.Add(MapPath, Utf8.GetBytes(ProvenanceMap.Render(files)));
        return tree;
    }

    private static T Read<T>(byte[] json, string name, JsonUnmappedMemberHandling unmapped)
    {
        var options = new JsonSerializerOptions(JsonSerializerDefaults.Web)
        {
            AllowDuplicateProperties = false,
            RespectNullableAnnotations = true,
            RespectRequiredConstructorParameters = true,
            UnmappedMemberHandling = unmapped
        };
        try
        {
            return JsonSerializer.Deserialize<T>(json, options) ?? throw new InvalidDataException($"{name}: the document is null");
        }
        catch (JsonException error)
        {
            throw new InvalidDataException($"{name}: {error.Message}", error);
        }
    }
}
