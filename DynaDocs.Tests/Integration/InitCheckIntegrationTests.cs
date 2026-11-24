namespace DynaDocs.Tests.Integration;

/// <summary>
/// A freshly initialised project passes `dydo check`, and an agent's scratch file under
/// `dydo/agents/` never fails it.
/// </summary>
[Collection("Integration")]
[Trait("Job", "contract: dydo-commands-core.md dydo init then dydo check exits 0; trap: the scanExclude invariant keeps dydo/agents/ scratch out of check")]
public class InitCheckIntegrationTests : IntegrationTestBase
{
    // A fresh init without the scratch file is fresh-installation.feature's (DYD-1520 setup audit).
    [Fact]
    public async Task FreshInit_PassesCheck()
    {
        await InitProjectAsync("none");
        WriteFile("dydo/agents/sample/bad-doc.md", "no frontmatter at all, [broken](./nowhere.md)");

        var check = await CheckAsync();

        Assert.True(check.ExitCode == 0, $"exit {check.ExitCode}:\n{check.Stdout}{check.Stderr}");
        Assert.DoesNotContain("bad-doc.md", check.Stdout + check.Stderr);
    }
}
