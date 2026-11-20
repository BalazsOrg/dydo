using System.CommandLine;
using System.Text.RegularExpressions;
using DynaDocs.Commands;

namespace DynaDocs.SkillBuild;

/// <summary>
/// The <c>dydo …</c> commands a text names in code (a code span, a fenced block or a command evidence),
/// checked against the CLI's own command tree, <see cref="DydoRootCommand.Create"/>, never a hand-kept
/// list (DYD-1295 71e649c0). Every invocation in the code is read, to its end or to a shell separator.
/// Each of its words is one of four things: a named <c>--option</c>, which must be the command's; the
/// value of the option before it; an argument, written <c>&lt;placeholder&gt;</c>; or a literal word,
/// which must be a subcommand. <c>a|b</c> names two subcommands, and what follows must hold for each.
/// </summary>
internal static partial class CommandNames
{
    private static readonly Lazy<RootCommand> Tree = new(DydoRootCommand.Create);

    [GeneratedRegex(@"`([^`\n]+)`")]
    private static partial Regex CodeSpan();

    [GeneratedRegex(@"^ {0,3}(?:`{3,}|~{3,})")]
    private static partial Regex Fence();

    [GeneratedRegex(@"(?<![\w./-])dydo(?![\w./-])")]
    private static partial Regex Invocation();

    [GeneratedRegex(@"(?<![\w./-])dydo [a-z][a-z0-9-]*")]
    private static partial Regex ProseCommand();

    [GeneratedRegex(@"<[^>\n]*>|\S+")]
    private static partial Regex Token();

    /// <summary>Every command <paramref name="markdown"/> names in code that the tree does not hold, each as the command and what is wrong with it.</summary>
    public static IEnumerable<string> Missing(string markdown) => Read(markdown).Where(part => part.Code).SelectMany(part => MissingIn(part.Text));

    /// <summary>Every <c>dydo word</c> that <paramref name="markdown"/> names outside code, where <see cref="Missing"/> cannot read it.</summary>
    public static IEnumerable<string> InProse(string markdown) =>
        Read(markdown).Where(part => !part.Code).SelectMany(part => ProseCommand().Matches(part.Text)).Select(named => named.Value);

    /// <summary>The same check for a command as it is typed, outside any markup.</summary>
    public static IEnumerable<string> MissingIn(string code)
    {
        foreach (Match invocation in Invocation().Matches(code))
        {
            var tokens = Token().Matches(code, invocation.Index + invocation.Length).Select(token => token.Value)
                .TakeWhile(token => token is not ("&&" or "||" or "|" or ";" or ">" or "#" or "…" or "...")).ToArray();
            if (Walk(new CommandCursor(Tree.Value, "dydo", [], false), tokens) is { } problem)
                yield return $"`{string.Join(' ', ["dydo", .. tokens])}`: {problem}";
        }
    }

    // A text's code, each fenced line and each code span, and, as not code, each other line with its spans cut out.
    private static IEnumerable<(string Text, bool Code)> Read(string markdown)
    {
        var fenced = false;
        foreach (var line in markdown.Split('\n'))
        {
            if (Fence().IsMatch(line)) fenced = !fenced;
            else if (fenced) yield return (line, true);
            else
            {
                foreach (Match span in CodeSpan().Matches(line)) yield return (span.Groups[1].Value, true);
                yield return (CodeSpan().Replace(line, "`"), false);
            }
        }
    }

    // The first problem on any path through the tokens: `a|b` forks the walk, one cursor for each word.
    private static string? Walk(CommandCursor at, string[] tokens)
    {
        if (tokens.Length == 0) return null;
        var (next, problem) = Step(at, tokens[0]);
        return problem ?? next.Select(cursor => Walk(cursor, tokens[1..])).FirstOrDefault(later => later is not null);
    }

    private static (CommandCursor[] Next, string? Problem) Step(CommandCursor at, string token)
    {
        if (token.StartsWith('-')) return OptionNamed(at, token);
        if (at.OptionValue) return ([at with { OptionValue = false }], null);
        if (token is not ['<', .., '>']) return Literal(at, token);
        return at.Node.Arguments.Count > 0 ? ([at], null) : ([], $"`{at.Path}` takes no argument, and `{token}` is one");
    }

    // A flag takes no value, so the word after it is read as any other; the parser gives a flag only `true` or `false`.
    private static (CommandCursor[], string?) OptionNamed(CommandCursor at, string token)
    {
        var name = token.Split('=')[0];
        var option = at.Node.Options.Concat(at.Inherited).FirstOrDefault(option => option.Name == name || option.Aliases.Contains(name));
        return option is null
            ? ([], $"`{name}` is no option of `{at.Path}`")
            : ([at with { OptionValue = option.Arity.MaximumNumberOfValues > 0 && option.ValueType != typeof(bool) && !token.Contains('=') }], null);
    }

    private static (CommandCursor[], string?) Literal(CommandCursor at, string token)
    {
        var words = token.Split('|');
        var subs = words.Select(word => at.Node.Subcommands.FirstOrDefault(sub => sub.Name == word || sub.Aliases.Contains(word))).ToArray();
        var missing = Array.IndexOf(subs, null);
        if (missing >= 0)
            return ([], $"`{words[missing]}` is no subcommand of `{at.Path}`; an argument is written `<placeholder>`");
        // A recursive option, such as the root's --help, is every descendant's too.
        Option[] inherited = [.. at.Inherited, .. at.Node.Options.Where(option => option.Recursive)];
        return ([.. subs.Select((sub, index) => new CommandCursor(sub!, $"{at.Path} {words[index]}", inherited, false))], null);
    }
}
