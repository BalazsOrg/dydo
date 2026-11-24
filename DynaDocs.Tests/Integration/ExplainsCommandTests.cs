namespace DynaDocs.Tests.Integration;

using DynaDocs.Commands;
using DynaDocs.Services;
using static DynaDocs.Tests.Integration.PushedRepository;

[Collection("Integration")]
[Trait("Job", "contract: DYD-1100 DECIDED bdb6b643 §3 rule 4 (DR 072: a doc declares the paths it explains, and dydo check / pr-ready flag a change to them); DYD-1187")]
public sealed class ExplainsCommandTests : IDisposable
{
    private readonly string _root = Path.Combine(Path.GetTempPath(), "dydo-explains-" + Guid.NewGuid().ToString("N"));
    private readonly string _tree;
    private readonly string _doc;

    public ExplainsCommandTests()
    {
        _tree = Path.Combine(_root, "tree");
        Create(_tree, Path.Combine(_root, "origin.git"));
        Git(_tree, "branch", "-m", "ticket");
        Git(_tree, "push", "-u", "origin", "ticket");
        _doc = Path.Combine(_tree, "dydo", "reference", "auth.md");
        Directory.CreateDirectory(Path.GetDirectoryName(_doc)!);
        File.WriteAllText(Path.Combine(_tree, "dydo", "index.md"),
            "---\narea: general\ntype: hub\n---\n\n# Documentation\n");
        File.WriteAllText(Path.Combine(_tree, "dydo.json"),
            """{"version":1,"structure":{"root":"dydo"},"scanExclude":["_system/.local/","agents/"]}""");
        WriteCode("Services/Auth/Login.cs");
        WriteCode("Services/Auth/Nested/Keys.cs");
        WriteCode("Services/Other.cs");
        WriteDoc("Services/Auth/**");
        Commit(_tree, "docs and code");
        Git(_tree, "branch", "feature/dydo-4-0-0");
        Git(_tree, "update-ref", "refs/remotes/origin/base", "HEAD");
        Git(_tree, "symbolic-ref", "refs/remotes/origin/HEAD", "refs/remotes/origin/base");
        Git(_tree, "push");
    }

    public void Dispose() => LinkSafeDelete.DeleteTree(_root);

    [Theory]
    [InlineData("[]")]
    [InlineData("[Services/Auth/**,]")]
    [InlineData("[Services/Auth/**")]
    [InlineData("{path: Services/Auth/**}")]
    [InlineData("[Services/Auth/**, null]")]
    [InlineData("\n  - Services/Auth/**\n  - 42")]
    [InlineData("../Services/**")]
    [InlineData("/Services/**")]
    [InlineData("[Services/[Auth/**]")]
    public void Check_MalformedDeclarationIsAnError(string declaration)
    {
        WriteDoc(declaration);

        var (exit, stdout, stderr) = Check();

        Assert.Equal(1, exit);
        Assert.Contains("explains", stdout + stderr);
        Assert.Contains("Found errors.", stdout);
    }

    [Theory]
    [InlineData("Missing/**/*.cs")]
    [InlineData("Services?Other.cs")]
    public void Check_EveryGlobMustMatchAFile(string unmatched)
    {
        WriteDoc($"[Services/Auth/**, {unmatched}]");

        var (exit, stdout, stderr) = Check();

        Assert.Equal(1, exit);
        Assert.Contains(unmatched, stdout + stderr);
        Assert.Contains("matches no files", stdout + stderr);
    }

    [Theory]
    [InlineData("Services/Auth/**")]
    [InlineData("'./Services/Auth/**/*.cs'")]
    [InlineData("[\"Services/Auth/*.cs\", 'Services/Auth/Nested/Keys.c?']")]
    [InlineData("\n  - 'Services/Auth/**/*.cs' # recursive\n  - Services/Other.cs")]
    [InlineData("\"Services\\\\Auth\\\\**\"")]
    public void Check_ValidDeclarationsKeepNormalSuccessOutput(string declaration)
    {
        WriteDoc(declaration);

        var (exit, stdout, stderr) = Check();

        Assert.Equal(0, exit);
        Assert.Contains("All checks passed.", stdout);
        Assert.DoesNotContain("explains", stderr);

        File.Delete(Path.Combine(_tree, "Services/Auth/Login.cs"));
        File.Delete(Path.Combine(_tree, "Services/Auth/Nested/Keys.cs"));
        File.Delete(Path.Combine(_tree, "Services/Other.cs"));
        var missing = Check();
        Assert.Equal(1, missing.Exit);
        Assert.Contains("matches no files", missing.Stdout + missing.Stderr);
    }

    [Fact]
    public async Task PrReady_UnchangedDocWarnsForAllTouchedPathsWithoutBlocking()
    {
        WriteCode("Services/Auth/Login.cs", "changed");
        WriteCode("Services/Auth/Nested/Keys.cs", "changed");
        Push();

        var (exit, stdout, stderr) = await Ready();

        Assert.Equal(0, exit);
        Assert.Contains("ready: ", stdout);
        Assert.Contains("warning: ", stderr);
        Assert.Contains("dydo/reference/auth.md", stderr);
        Assert.Contains("Services/Auth/Login.cs", stderr);
        Assert.Contains("Services/Auth/Nested/Keys.cs", stderr);
        Assert.Contains("update it or state it still holds", stderr);
    }

    [Fact]
    public async Task PrReady_UpdatedDocAndUnrelatedCodeKeepNormalSuccessOutput()
    {
        WriteCode("Services/Auth/Login.cs", "changed");
        Push();
        var unchanged = await Ready();
        Assert.Equal(0, unchanged.Exit);
        Assert.Contains("ready: ", unchanged.Stdout);
        Assert.Contains("update it or state it still holds", unchanged.Stderr);

        File.AppendAllText(_doc, "\nStill holds.\n");
        Push();
        var updated = await Ready();
        Assert.Equal(0, updated.Exit);
        Assert.Contains("ready: ", updated.Stdout);
        Assert.Empty(updated.Stderr);

        Git(_tree, "update-ref", "refs/remotes/origin/base", "HEAD");
        WriteCode("Services/Other.cs", "changed");
        Push();
        var unrelated = await Ready();
        Assert.Equal(0, unrelated.Exit);
        Assert.Contains("ready: ", unrelated.Stdout);
        Assert.Empty(unrelated.Stderr);
    }

    [Fact]
    public async Task PrReady_ExplicitFeatureBaseAndSubdirectoryIncludeDeletedAndRenamedSources()
    {
        Git(_tree, "mv", "Services/Auth/Login.cs", "Services/Moved.cs");
        File.Delete(Path.Combine(_tree, "Services/Auth/Nested/Keys.cs"));
        Push();

        var (exit, stdout, stderr) = await ConsoleCapture.AllAsync(() => PrReadyCommand.Create()
            .Parse([Path.Combine(_tree, "Services"), "--base", "feature/dydo-4-0-0"]).InvokeAsync());

        Assert.Equal(0, exit);
        Assert.Contains("ready: ", stdout);
        Assert.Contains("dydo/reference/auth.md", stderr);
        Assert.Contains("Services/Auth/Login.cs", stderr);
        Assert.Contains("Services/Auth/Nested/Keys.cs", stderr);
    }

    [Fact]
    public void Check_DiffWarningIsAdvisoryAndClearedByUpdatingDoc()
    {
        WriteCode("Services/Auth/Login.cs", "changed");

        var changed = Check("--base", "feature/dydo-4-0-0");
        Assert.Equal(0, changed.Exit);
        Assert.Contains("Found warnings (no errors).", changed.Stdout);
        Assert.Contains("Services/Auth/Login.cs", changed.Stdout + changed.Stderr);
        Assert.Contains("update it or state it still holds", changed.Stdout + changed.Stderr);

        File.AppendAllText(_doc, "\nStill holds.\n");
        var updated = Check("--base", "feature/dydo-4-0-0");
        Assert.Equal(0, updated.Exit);
        Assert.Contains("All checks passed.", updated.Stdout);
    }

    [Fact]
    [Trait("Job", "contract: DYD-1189; DYD-1100 DECIDED bdb6b643 §3 rule 4 (advisory explains check)")]
    public async Task PrReady_MissingBaseWarnsWithoutBlocking()
    {
        var (exit, stdout, stderr) = await ConsoleCapture.AllAsync(() => PrReadyCommand.Create()
            .Parse([_tree, "--base", "refs/heads/missing-explains-base"]).InvokeAsync());

        Assert.Equal(0, exit);
        Assert.Contains("ready: ", stdout);
        Assert.Contains("warning: explains comparison unavailable", stderr);
    }

    [Fact]
    [Trait("Job", "contract: DYD-1189; DYD-1100 DECIDED bdb6b643 §3 rule 4 (advisory explains check)")]
    public void Check_MissingBaseWarnsWithoutBlocking()
    {
        var (exit, stdout, stderr) = Check("--base", "refs/heads/missing-explains-base");

        Assert.Equal(0, exit);
        Assert.Contains("Found warnings (no errors).", stdout);
        Assert.Contains("explains comparison unavailable", stdout + stderr);
    }

    private void WriteDoc(string declaration) => File.WriteAllText(_doc,
        $"---\narea: reference\ntype: reference\nexplains: {declaration}\n---\n\n# Authentication\n\nThe login flow.\n");

    private void WriteCode(string path, string text = "seed")
    {
        var file = Path.Combine(_tree, path);
        Directory.CreateDirectory(Path.GetDirectoryName(file)!);
        File.WriteAllText(file, text);
    }

    private void Push()
    {
        Commit(_tree, "change");
        Git(_tree, "push");
    }

    private (int Exit, string Stdout, string Stderr) Check(params string[] args) => ConsoleCapture.All(() =>
        CheckCommand.Create().Parse([_doc, "--root", _tree, .. args]).Invoke());

    private Task<(int Exit, string Stdout, string Stderr)> Ready() =>
        ConsoleCapture.AllAsync(() => PrReadyCommand.Create().Parse([_tree]).InvokeAsync());
}
