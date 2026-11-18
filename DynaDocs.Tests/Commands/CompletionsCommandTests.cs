namespace DynaDocs.Tests.Commands;

/// <summary>
/// `dydo completions <shell>` prints a script that registers completion for dydo; `dydo _complete`
/// is what that script calls. An unknown shell is a tool error. Completion itself never fails the
/// shell: an unknown command line yields no candidates and exit 0.
/// </summary>
[Collection("Integration")]
[Trait("Job", "contract: dydo-commands-utilities.md dydo completions (bash, zsh, powershell); DR 015 (completions keep code)")]
public sealed class CompletionsCommandTests
{
    [Theory]
    [InlineData("bash", "complete -F _dydo_completions dydo")]
    [InlineData("zsh", "compdef _dydo_completions dydo")]
    [InlineData("powershell", "Register-ArgumentCompleter")]
    public void Completions_KnownShell_PrintsItsRegistrationScript(string shell, string registration)
    {
        var (code, stdout, _) = DydoCli.Run("completions", shell);

        Assert.Equal(0, code);
        Assert.Contains(registration, stdout);
        Assert.Contains("_complete", stdout);
    }

    [Fact]
    public void Completions_UnknownShell_IsAToolError()
    {
        var (code, stdout, _) = DydoCli.Run("completions", "fish");

        Assert.Equal(2, code);
        Assert.Equal("", stdout);
    }

    // Completion does not offer every registered command yet (`mode` and `statechart` are missing;
    // reported on DYD-1199), so the first word is checked against the documented core commands.
    [Theory]
    [InlineData("1", "dydo", "init", "check", "fix", "graph", "validate", "clean", "completions")]
    [InlineData("2", "init", "claude", "codex", "all", "none")]
    [InlineData("2", "completions", "bash", "zsh", "powershell")]
    [InlineData("2", "graph", "stats")]
    public void Complete_OffersTheWordsValues(string position, string word, params string[] expected)
    {
        var candidates = position == "1" ? Candidates(position, word) : Candidates(position, "dydo", word);

        Assert.All(expected, value => Assert.Contains(value, candidates));
    }

    [Theory]
    [InlineData("2", "dydo", "nonexistent")]
    [InlineData("999", "dydo", "nonexistent", "garbage")]
    public void Complete_UnknownInput_OffersNothingAndExitsZero(params string[] words)
    {
        var (code, stdout, _) = DydoCli.Run(["_complete", .. words]);

        Assert.Equal(0, code);
        Assert.Equal("", stdout);
    }

    private static List<string> Candidates(params string[] words)
    {
        var (code, stdout, _) = DydoCli.Run(["_complete", .. words]);
        Assert.Equal(0, code);
        return stdout.Split(['\r', '\n'], StringSplitOptions.RemoveEmptyEntries).ToList();
    }
}
