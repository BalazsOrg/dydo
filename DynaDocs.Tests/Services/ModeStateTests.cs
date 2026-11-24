namespace DynaDocs.Tests.Services;

using DynaDocs.Commands;
using DynaDocs.Services;

/// <summary>
/// The machine's mode store as `dydo mode` shows it: a missing or unusable store reads every flag
/// at its default, and a set persists for the next reader, even over an unusable store.
/// </summary>
public sealed class ModeStateTests : IDisposable
{
    private readonly string _root = Path.Combine(Path.GetTempPath(), "dydo-mode-" + Guid.NewGuid().ToString("N"));

    [Theory]
    [Trait("Job", "contract: DYD-351; dydo-commands-linear.md dydo mode (overnight defaults to off): show over a missing store, malformed JSON, an unknown value or a JSON null exits 0, and its trimmed output is overnight: off")]
    [InlineData(null)]
    [InlineData("{ not json")]
    [InlineData("""{"overnight":"maybe"}""")]
    [InlineData("null")]
    public async Task Show_MissingOrUnusableStore_ReadsTheDefault(string? store)
    {
        WriteStore(store);

        var (code, stdout) = await Mode("show");

        Assert.Equal(0, code);
        Assert.Equal("overnight: off", stdout.Trim());
    }

    // Over a missing store this is machine-mode.feature's "Set changes a flag" (DYD-1520 setup audit).
    [Fact]
    [Trait("Job", "contract: DYD-351; dydo-commands-linear.md dydo mode (every run reads the stored flags): after set overnight on over a malformed store, a separate run's show exits 0, and its trimmed output is overnight: on")]
    public async Task Set_PersistsForTheNextReader()
    {
        WriteStore("{ not json");

        await Mode("set", "overnight", "on");
        var (code, stdout) = await Mode("show");

        Assert.Equal(0, code);
        Assert.Equal("overnight: on", stdout.Trim());
    }

    private void WriteStore(string? content)
    {
        if (content == null)
            return;
        Directory.CreateDirectory(_root);
        File.WriteAllText(Path.Combine(_root, "mode.json"), content);
    }

    // Each run gets a fresh store object, so a value can only come back through the file.
    private async Task<(int Code, string Stdout)> Mode(params string[] args)
    {
        var (code, stdout, _) = await ConsoleCapture.AllAsync(
            () => CliParsing.Parse(ModeCommand.Create(new ModeState(_root)), args).InvokeAsync());
        return (code, stdout);
    }

    public void Dispose() => TestDirectory.Delete(_root);
}
