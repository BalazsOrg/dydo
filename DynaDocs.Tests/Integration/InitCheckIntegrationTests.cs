namespace DynaDocs.Tests.Integration;

/// <summary>
/// A freshly initialised project passes `dydo check`, and an agent's scratch file under
/// `dydo/agents/` never fails it.
/// </summary>
[Collection("Integration")]
[Trait("Job", "contract: dydo-commands-core.md dydo init then dydo check exits 0; trap: the scanExclude invariant keeps dydo/agents/ scratch out of check")]
public class InitCheckIntegrationTests : IntegrationTestBase
{
    [Theory]
    [InlineData(null)]
    [InlineData("dydo/agents/sample/bad-doc.md")]
    public async Task FreshInit_PassesCheck(string? agentScratchFile)
    {
        await InitProjectAsync("none");
        if (agentScratchFile != null)
            WriteFile(agentScratchFile, "no frontmatter at all, [broken](./nowhere.md)");

        var check = await CheckAsync();

        Assert.True(check.ExitCode == 0, $"exit {check.ExitCode}:\n{check.Stdout}{check.Stderr}");
        Assert.DoesNotContain("bad-doc.md", check.Stdout + check.Stderr);
    }
}
