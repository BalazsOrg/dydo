using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;

namespace DynaDocs.SkillBuild;

/// <summary>
/// claims.map.json: for every built file, each emitted line range with its unit and claim ids, one
/// range per line of JSON so a rebuild's diff touches only the ranges that moved.
/// </summary>
internal static class ProvenanceMap
{
    private static readonly JsonSerializerOptions Options = new(JsonSerializerDefaults.Web)
    {
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
    };

    public static string Render(IEnumerable<KeyValuePair<string, RenderedFile>> files)
    {
        var json = new StringBuilder("{\n  \"note\": \"Built by tools/SkillBuild: each built file's prose line ranges, with their unit and claim ids.\",\n  \"files\": [");
        var separator = "\n";
        foreach (var (path, file) in files)
        {
            json.Append(separator).Append("    {\n      \"path\": ").Append(JsonSerializer.Serialize(path, Options))
                .Append(",\n      \"entries\": [\n        ")
                .AppendJoin(",\n        ", file.Map.Select(entry => JsonSerializer.Serialize(entry, Options)))
                .Append("\n      ]\n    }");
            separator = ",\n";
        }
        return json.Append("\n  ]\n}\n").ToString();
    }
}
