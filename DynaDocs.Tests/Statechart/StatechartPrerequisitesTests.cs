namespace DynaDocs.Tests.Statechart;

using DynaDocs.Services.Statechart;

public class StatechartPrerequisitesTests
{
    [Theory]
    [Trait("Job", "contract: DR 062 §1 (each runtime version is judged compatible or not at its boundary)")]
    [InlineData("node", "v22.17.1", false)]
    [InlineData("node", "v22.18.0", true)]
    [InlineData("node", "v23.5.0", false)]
    [InlineData("node", "v23.6.0", true)]
    [InlineData("node", "v24.0.0", true)]
    [InlineData("node", "v26.13.0", true)]
    [InlineData("node", "v21.7.3", false)]
    [InlineData("java", "openjdk version \"11.0.16.1\"", true)]
    [InlineData("java", "java version \"1.8.0\"", false)]
    [InlineData("node", "unrecognized", false)]
    public void RuntimeCompatibilityPreservesExistingBoundaries(string runtime, string version, bool expected)
    {
        Assert.Equal(expected, StatechartPrerequisites.Compatible(runtime, version));
    }

    [Fact]
    [Trait("Job", "contract: DR 062 §1 (explicit invalid overrides are errors, and no override means no explicit checker)")]
    public void InvalidExplicitOverridesCannotFallBackToOtherCandidates()
    {
        using var scope = new StatechartTestDirectory();
        Assert.Throws<IOException>(() => StatechartPrerequisites.Java(scope.Path, "./missing", Environment.GetEnvironmentVariable("PATH")));
        File.WriteAllText(Path.Combine(scope.Path, "fake.jar"), "changed");
        var pinned = new StatechartAssets().CheckerDigest;
        Assert.Throws<IOException>(() => StatechartPrerequisites.Tlc(scope.Path, "fake.jar", pinned));
        Assert.Null(StatechartPrerequisites.Tlc(scope.Path, null, pinned));
    }

    [Fact]
    [Trait("Job", "contract: DR 062 §1 (an explicit JAVA path override resolves against the caller's directory)")]
    public void CallerRelativeJavaAndBareJavaUseTheOriginalDirectoryAndPath()
    {
        using var scope = new StatechartTestDirectory();
        var name = OperatingSystem.IsWindows() ? "java.exe" : "java";
        var file = Path.Combine(scope.Path, name);
        File.WriteAllText(file, "test candidate; never executed");
        Assert.Equal(file, StatechartPrerequisites.Java(scope.Path, "./" + name, ""));
        Assert.Equal(file, StatechartPrerequisites.Java(scope.Path, "java", scope.Path));
        Assert.Null(StatechartPrerequisites.Find("java", "relative-path-is-not-a-candidate"));
    }
}
