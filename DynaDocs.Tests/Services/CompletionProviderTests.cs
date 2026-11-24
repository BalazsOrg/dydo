namespace DynaDocs.Tests.Services;

using DynaDocs.Services;

// Kept `unsure` by the DYD-1520 setup audit (dydo/project/reviews/dyd-1520-test-audit/setup.md):
// each pins a first-word candidate no kept test covers. The other first words are asserted at the
// `dydo _complete` boundary in CompletionsCommandTests.
public class CompletionProviderTests
{
    [Theory]
    [InlineData("complete")]
    public void TopLevelCommands_ContainsCommand(string command)
    {
        var completions = CompletionProvider.GetCompletions(1, ["dydo"]).ToList();
        Assert.Contains(command, completions);
    }

    [Fact]
    public void TopLevelCommands_ExcludesRetiredWorkCommands()
    {
        var completions = CompletionProvider.GetCompletions(1, ["dydo"]).ToList();

        Assert.DoesNotContain("task", completions);
        Assert.DoesNotContain("issue", completions);
        Assert.DoesNotContain("review", completions);
    }
}
