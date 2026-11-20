using System.CommandLine;
using System.Text.RegularExpressions;
using DynaDocs.Commands;

namespace DynaDocs.SkillBuild;

/// <summary>
/// The <c>dydo …</c> commands a text names in code (a code span, a fenced block or a command evidence),
/// checked against the CLI's own command tree, <see cref="DydoRootCommand.Create"/>, never a hand-kept
/// list (DYD-1295 71e649c0). A literal word after a command must be a subcommand, an argument is written
/// <c>&lt;placeholder&gt;</c>, and a named <c>--option</c> must be the command's.
/// </summary>
internal static partial class CommandNames
{
    private static readonly Lazy<RootCommand> Tree = new(DydoRootCommand.Create);

    [GeneratedRegex(@"`([^`\n]+)`")]
    private static partial Regex CodeSpan();

    [GeneratedRegex(@"^ {0,3}(?:`{3,}|~{3,})")]
    private static partial Regex Fence();

    [GeneratedRegex(@"(?<![\w./-])dydo(?![\w./-])(?<rest>.*)$")]
    private static partial Regex Invocation();

    [GeneratedRegex(@"<[^>\n]*>|\[[^\]\n]*\]|\S+")]
    private static partial Regex Token();

    [GeneratedRegex(@"^[a-z][a-z0-9-]*(?:\|[a-z][a-z0-9-]*)*$")]
    private static partial Regex Word();

    /// <summary>Every command <paramref name="markdown"/> names in code that the tree does not hold, each as the command and what is wrong with it.</summary>
    public static IEnumerable<string> Missing(string markdown)
    {
        var fenced = false;
        foreach (var line in markdown.Split('\n'))
        {
            if (Fence().IsMatch(line)) fenced = !fenced;
            else if (fenced) foreach (var problem in MissingIn(line)) yield return problem;
            else foreach (var problem in CodeSpan().Matches(line).SelectMany(span => MissingIn(span.Groups[1].Value))) yield return problem;
        }
    }

    /// <summary>The same check for a command as it is typed, outside any markup.</summary>
    public static IEnumerable<string> MissingIn(string code)
    {
        var invocation = Invocation().Match(code);
        if (!invocation.Success) return [];
        var tokens = Token().Matches(invocation.Groups["rest"].Value).Select(token => token.Value)
            .TakeWhile(token => token is not ("&&" or "||" or "|" or ";" or ">" or "#" or "…" or "...")).ToArray();
        var problem = Walk(tokens);
        return problem is null ? [] : [$"`{string.Join(' ', ["dydo", .. tokens])}`: {problem}"];
    }

    private static string? Walk(string[] tokens)
    {
        var at = new CommandCursor(Tree.Value, "dydo", [], false);
        foreach (var token in tokens)
        {
            var (next, problem) = Step(at, token);
            if (next is null) return problem;
            at = next;
        }
        return null;
    }

    // The cursor after a token, or none with the problem that ends the walk; `a|b` ends it with none.
    private static (CommandCursor? Next, string? Problem) Step(CommandCursor at, string token)
    {
        if (token.StartsWith('-')) return Named(at, token);
        if (Word().IsMatch(token)) return Literal(at, token);
        return at.OptionValue || at.Node.Arguments.Count > 0
            ? (at with { OptionValue = false }, null)
            : (null, $"`{at.Path}` takes no argument, and `{token}` is one");
    }

    private static (CommandCursor?, string?) Named(CommandCursor at, string token)
    {
        var name = token.Split('=')[0];
        var option = at.Node.Options.Concat(at.Inherited).FirstOrDefault(option => option.Name == name || option.Aliases.Contains(name));
        return option is null
            ? (null, $"`{name}` is no option of `{at.Path}`")
            : (at with { OptionValue = option.Arity.MaximumNumberOfValues > 0 && !token.Contains('=') }, null);
    }

    private static (CommandCursor?, string?) Literal(CommandCursor at, string token)
    {
        var words = token.Split('|');
        var subs = words.Select(word => at.Node.Subcommands.FirstOrDefault(sub => sub.Name == word || sub.Aliases.Contains(word))).ToArray();
        var missing = Array.IndexOf(subs, null);
        if (missing >= 0)
            return (null, $"`{words[missing]}` is no subcommand of `{at.Path}`; an argument is written `<placeholder>`");
        // A recursive option, such as the root's --help, is every descendant's too.
        return words.Length > 1
            ? (null, null)
            : (new CommandCursor(subs[0]!, $"{at.Path} {token}", [.. at.Inherited, .. at.Node.Options.Where(option => option.Recursive)], false), null);
    }
}
