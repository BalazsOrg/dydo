using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace DynaDocs.SkillBuild;

/// <summary>
/// A frontmatter value as a YAML string scalar. It stays plain when YAML reads it back as the same
/// string: it starts with a letter, holds no colon, <c>#</c>, quote, backslash or line break, ends
/// on a non-space, and is not a word YAML 1.1 reads as a boolean or null. Anything else is
/// double-quoted, with JSON's escapes, which YAML's double-quoted style shares.
/// </summary>
internal static partial class YamlScalar
{
    private static readonly JsonSerializerOptions Quoting = new() { Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping };

    [GeneratedRegex(@"^(?!(?i:y|n|yes|no|true|false|on|off|null)$)\p{L}[^\p{Cc}\u2028\u2029:#""'\\]*(?<!\s)\z")]
    private static partial Regex Plain();

    public static string Of(string value) => Plain().IsMatch(value) ? value : JsonSerializer.Serialize(value, Quoting);
}
