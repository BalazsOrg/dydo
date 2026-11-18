using System.Text.Json.Nodes;
using DynaDocs.Commands;

namespace DynaDocs.Tests.Integration;

/// <summary>
/// `dydo adopt <version>` records the adopted release as `dydoVersion` in dydo.json. Anything that
/// is not a release, a missing project or a config that does not parse exits 2 and leaves the
/// file's bytes as they were.
/// </summary>
[Collection("Integration")]
[Trait("Job", "contract: dydo-commands-core.md dydo adopt (exit 2 leaves the file as it was); trap: a failed adopt must not rewrite dydo.json")]
public class AdoptCommandTests : IntegrationTestBase
{
    private string ConfigPath => Path.Combine(TestDir, "dydo.json");

    [Theory]
    [InlineData("1.2.3", "3.6.0", "3.6.0")]
    [InlineData("1.2.3", "3.6.0-beta.3", "3.6.0-beta.3")]
    [InlineData("1.2.3", "v3.6.0", "3.6.0")]
    [InlineData(null, "3.6.0", "3.6.0")]
    public async Task Adopt_Release_IsRecorded(string? recorded, string version, string expected)
    {
        await InitProjectAsync("none");
        SetDydoVersion(recorded);

        var result = await RunAsync(AdoptCommand.Create(), version);

        Assert.Equal(0, result.ExitCode);
        Assert.Equal(expected, JsonNode.Parse(File.ReadAllText(ConfigPath))!["dydoVersion"]?.GetValue<string>());
    }

    [Theory]
    [InlineData("latest")]
    [InlineData("3.6")]
    [InlineData("vv3.6.0")]
    [InlineData("3.6.0 extra")]
    [InlineData("")]
    public async Task Adopt_NotARelease_ExitsTwoAndKeepsTheConfigBytes(string version)
    {
        await InitProjectAsync("none");
        var original = File.ReadAllBytes(ConfigPath);

        var result = await RunAsync(AdoptCommand.Create(), version);

        Assert.Equal(2, result.ExitCode);
        Assert.Equal(original, File.ReadAllBytes(ConfigPath));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("{ not json")]
    public async Task Adopt_NoUsableConfig_ExitsTwoAndWritesNothing(string? config)
    {
        if (config != null)
            File.WriteAllText(ConfigPath, config);

        var result = await RunAsync(AdoptCommand.Create(), "3.6.0");

        Assert.Equal(2, result.ExitCode);
        if (config == null)
            Assert.False(File.Exists(ConfigPath));
        else
            Assert.Equal(config, File.ReadAllText(ConfigPath));
    }

    private void SetDydoVersion(string? version)
    {
        var raw = JsonNode.Parse(File.ReadAllText(ConfigPath))!.AsObject();
        raw.Remove("dydoVersion");
        if (version != null)
            raw["dydoVersion"] = version;
        File.WriteAllText(ConfigPath, raw.ToJsonString());
    }
}
