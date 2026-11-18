namespace DynaDocs.Tests.Statechart;

using DynaDocs.Services.Statechart;

/// <summary>
/// The payload embedded in dydo, through an offline <c>dydo statechart</c> run: damaged extracted assets are restored
/// from the embedded bytes and the run still reaches its outcome; a directory this payload did not write is retained.
/// </summary>
[Trait("Job", "contract: DR 062 §1 (a ready invocation runs offline)")]
public class StatechartAssetsTests
{
    private const string Helper = "skills/engineering/statechart/scripts/java/statechart-tlc-bootstrap.jar";

    [Theory]
    [InlineData("changed")]
    [InlineData("deleted")]
    [InlineData("added")]
    [InlineData("changed and deleted")]
    public async Task DamagedAssetsAreExtractedAgainOffline_AndTheRunReachesItsOutcome(string damage)
    {
        using var scope = new StatechartTestDirectory();
        Model(scope.Path);
        Assert.Equal(3, (await Run(scope.Path)).Exit);
        var assets = new StatechartAssets();
        var payload = Path.Combine(scope.Path, "assets", assets.Release, assets.Digest);
        var setup = Path.Combine(payload, "setup-skills.mjs");
        var (helperBytes, setupBytes) = (File.ReadAllBytes(Path.Combine(payload, Helper)), File.ReadAllBytes(setup));
        if (damage.Contains("changed")) File.AppendAllText(setup, "changed");
        if (damage.Contains("deleted")) File.Delete(Path.Combine(payload, Helper));
        if (damage == "added") File.WriteAllText(Path.Combine(payload, "foreign.txt"), "added");

        Assert.Equal(3, (await Run(scope.Path)).Exit);

        assets.Verify(payload);
        Assert.Equal(helperBytes, File.ReadAllBytes(Path.Combine(payload, Helper)));
        Assert.Equal(setupBytes, File.ReadAllBytes(setup));
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task ADirectoryWithoutThisPayloadsReceipt_IsRetained_AndTheRunFails(bool otherReceipt)
    {
        using var scope = new StatechartTestDirectory();
        Model(scope.Path);
        var assets = new StatechartAssets();
        var target = Path.Combine(scope.Path, "assets", assets.Release, assets.Digest);
        Directory.CreateDirectory(target);
        var sentinel = Path.Combine(target, "setup-skills.mjs");
        File.WriteAllText(sentinel, "foreign sentinel");
        if (otherReceipt) File.WriteAllText(Path.Combine(target, ".statechart-receipt"), "another payload");

        Assert.Equal(2, (await Run(scope.Path)).Exit);

        Assert.Equal("foreign sentinel", File.ReadAllText(sentinel));
        Assert.False(Directory.Exists(Path.Combine(scope.Path, "state", "requests")));
    }

    // Every extraction runs under the extract lock, so a staging directory the next run finds there was left by a crash.
    [Fact]
    [Trait("Job", "trap: DYD-415 an extraction a crash left behind is swept by the next run")]
    public async Task AnExtractionACrashLeftBehind_IsSweptByTheNextRun()
    {
        using var scope = new StatechartTestDirectory();
        Model(scope.Path);
        var stale = Path.Combine(scope.Path, "assets", new StatechartAssets().Release, ".extract-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(stale);
        File.WriteAllText(Path.Combine(stale, "partial"), "interrupted");

        Assert.Equal(3, (await Run(scope.Path)).Exit);

        Assert.False(Directory.Exists(stale));
    }

    private static void Model(string root)
    {
        File.WriteAllText(Path.Combine(root, "model.tla"), "---- MODULE model ----\nVARIABLE x\nInit == x = 0\nNext == x' = x\n====\n");
        File.WriteAllText(Path.Combine(root, "model.cfg"), "INIT Init\nNEXT Next\n");
    }

    private static Task<(int Exit, string Output, string Error)> Run(string root) =>
        StatechartOfflineProcess.Run(StatechartOfflineProcess.Start(root, "", ["statechart", "model.tla"]));
}
