namespace DynaDocs.Tests.SkillBuild;

using DynaDocs.SkillBuild;

public sealed class SkillBuildToolTests : IDisposable
{
    private readonly string _root = Path.Combine(Path.GetTempPath(), "skillbuild-" + Guid.NewGuid().ToString("N"));

    public void Dispose() => TestDirectory.Delete(_root);

    [Fact]
    public void Write_LeavesExactlyTheTree_DeletingStrayFilesAndEmptiedFolders()
    {
        var built = Path.Combine(_root, "built");
        Directory.CreateDirectory(Path.Combine(built, "old", "deeper"));
        File.WriteAllText(Path.Combine(built, "old", "deeper", "stray.md"), "hand-written");
        File.WriteAllText(Path.Combine(built, "kept.md"), "stale");
        Directory.CreateDirectory(Path.Combine(built, "keep"));
        File.WriteAllText(Path.Combine(built, "keep", "stray.txt"), "x");

        BuiltTree.Write(built, new Dictionary<string, byte[]>
        {
            ["kept.md"] = "fresh\n"u8.ToArray(),
            ["keep/new/file.md"] = "new\n"u8.ToArray(),
        });

        Assert.Equal(new[] { "keep/new/file.md", "kept.md" },Directory.EnumerateFiles(built, "*", SearchOption.AllDirectories)
            .Select(file => Path.GetRelativePath(built, file).Replace('\\', '/')).Order(StringComparer.Ordinal));
        Assert.Equal("fresh\n", File.ReadAllText(Path.Combine(built, "kept.md")));
        Assert.False(Directory.Exists(Path.Combine(built, "old")));
    }

    [Fact]
    public void Run_BuildsClaimsBuiltFromTheRepositoryInputs()
    {
        var input = new SkillBuildInput();
        Directory.CreateDirectory(Path.Combine(_root, "claims", "inventory"));
        File.WriteAllBytes(Path.Combine(_root, "claims", "inventory", "claims.json"), SkillBuildInput.Bytes(input.Claims));
        File.WriteAllBytes(Path.Combine(_root, "claims", "carriers.json"), SkillBuildInput.Bytes(input.Carriers));
        var error = new StringWriter();

        Assert.Equal(0, Program.Run(_root, error));

        Assert.Empty(error.ToString());
        foreach (var (path, bytes) in input.Build())
            Assert.Equal(bytes, File.ReadAllBytes(Path.Combine(_root, "claims", "built", path)));
    }

    [Fact]
    public void Run_FailsLoudlyOnInvalidInput()
    {
        Directory.CreateDirectory(Path.Combine(_root, "claims", "inventory"));
        File.WriteAllText(Path.Combine(_root, "claims", "inventory", "claims.json"), "{");
        File.WriteAllText(Path.Combine(_root, "claims", "carriers.json"), "{}");
        var error = new StringWriter();

        Assert.Equal(1, Program.Run(_root, error));

        Assert.StartsWith("SkillBuild failed: claims.json:", error.ToString());
        Assert.False(Directory.Exists(Path.Combine(_root, "claims", "built")));
    }

    [Fact]
    public void Run_FailsLoudlyOnAMissingInput()
    {
        var error = new StringWriter();

        Assert.Equal(1, Program.Run(_root, error));

        Assert.Contains("claims.json", error.ToString());
    }
}
