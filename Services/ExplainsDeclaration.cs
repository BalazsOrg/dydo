namespace DynaDocs.Services;

using System.Text;
using System.Text.RegularExpressions;
using DynaDocs.Utils;

internal sealed record ExplainsDeclaration(List<string> Patterns, string? Error = null)
{
    // The shared GlobMatcher's '?' can span a slash; explained paths need segment semantics.
    public static bool Matches(string path, string pattern)
    {
        var expression = "^" + Regex.Escape(pattern).Replace(@"\*\*/", "(.*/)?")
            .Replace(@"\*\*", ".*").Replace(@"\*", "[^/]*").Replace(@"\?", "[^/]") + "$";
        return Regex.IsMatch(path, expression, RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.NonBacktracking);
    }

    public static ExplainsDeclaration Parse(string content)
    {
        var lines = FrontmatterParser.ExtractYamlBlock(content)?.Split('\n') ?? [];
        var index = Array.FindIndex(lines, line => line.TrimStart().StartsWith("explains:", StringComparison.OrdinalIgnoreCase));
        if (index < 0) return new([]);

        try
        {
            var value = WithoutComment(lines[index][(lines[index].IndexOf(':') + 1)..]).Trim();
            var continuation = lines.Skip(index + 1).TakeWhile(line =>
                string.IsNullOrWhiteSpace(line) || char.IsWhiteSpace(line[0]) || line.StartsWith('-') ||
                line.StartsWith('#')).ToList();
            var items = value.Length == 0 ? BlockItems(continuation) : InlineItems(value);
            if (value.Length > 0 && continuation.Any(line => WithoutComment(line).Trim().Length > 0))
                throw new FormatException();
            var patterns = items.Select(ReadPattern).ToList();
            if (patterns.Count == 0) throw new FormatException();
            return new(patterns);
        }
        catch (FormatException)
        {
            return new([], "explains must be a nonempty glob string or list of glob strings, relative to the repository root; "
                + "use *, ** and ? (no absolute paths, '..', mappings or empty items)");
        }
    }

    private static IEnumerable<string> BlockItems(List<string> lines)
    {
        foreach (var line in lines)
        {
            var item = WithoutComment(line).Trim();
            if (item.Length == 0) continue;
            if (!item.StartsWith("- ", StringComparison.Ordinal)) throw new FormatException();
            yield return item[2..].Trim();
        }
    }

    private static List<string> InlineItems(string value)
    {
        if (!value.StartsWith('[')) return [value];
        if (!value.EndsWith(']')) throw new FormatException();
        var items = new List<string>();
        var start = 1;
        char quote = '\0';
        for (var i = 1; i < value.Length - 1; i++)
        {
            var c = value[i];
            if (quote != '\0')
            {
                if (c == '\\' && quote == '"') i++;
                else if (c == quote) quote = '\0';
            }
            else if (c is '\'' or '"') quote = c;
            else if (c == ',')
            {
                items.Add(value[start..i].Trim());
                start = i + 1;
            }
        }
        if (quote != '\0') throw new FormatException();
        items.Add(value[start..^1].Trim());
        return items;
    }

    private static string WithoutComment(string value)
    {
        char quote = '\0';
        for (var i = 0; i < value.Length; i++)
        {
            var c = value[i];
            if (quote != '\0')
            {
                if (c == '\\' && quote == '"') i++;
                else if (c == quote) quote = '\0';
            }
            else if (c is '\'' or '"') quote = c;
            else if (c == '#' && (i == 0 || char.IsWhiteSpace(value[i - 1]))) return value[..i];
        }
        return value;
    }

    private static string ReadPattern(string value)
    {
        if (value.Length == 0) throw new FormatException();
        if (value[0] is '\'' or '"') value = Unquote(value);
        else if (value is "null" or "true" or "false" or "~" || double.TryParse(value, out _))
            throw new FormatException();
        value = PathUtils.NormalizePath(value);
        while (value.StartsWith("./", StringComparison.Ordinal)) value = value[2..];
        if (string.IsNullOrWhiteSpace(value) || value.StartsWith('/') || value.EndsWith('/') ||
            value.Split('/').Any(segment => segment is ".." or "." or "") ||
            value.Any(c => char.IsControl(c) || "[]{}:!\"'".Contains(c))) throw new FormatException();
        return value;
    }

    private static string Unquote(string value)
    {
        var quote = value[0];
        if (value.Length < 2 || value[^1] != quote) throw new FormatException();
        var result = new StringBuilder();
        for (var i = 1; i < value.Length - 1; i++)
        {
            var c = value[i];
            if (quote == '"' && c == '\\')
            {
                if (++i >= value.Length - 1 || value[i] is not ('\\' or '"' or '/')) throw new FormatException();
                c = value[i];
            }
            else if (c == quote)
            {
                if (quote != '\'' || ++i >= value.Length - 1 || value[i] != quote) throw new FormatException();
            }
            result.Append(c);
        }
        return result.ToString();
    }
}
