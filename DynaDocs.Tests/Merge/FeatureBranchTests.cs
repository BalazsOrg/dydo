namespace DynaDocs.Tests.Merge;

using DynaDocs.Services.Merge;

/// <summary>
/// The ref table (design §2, feature pattern): the fixed <c>feature/*</c> pattern, then git's own
/// <c>check-ref-format --branch</c>. The default branch is refused by name in
/// <see cref="PullRequestMergeTests"/>, including one named <c>feature/x</c>.
/// </summary>
[Collection("Integration")]
public sealed class FeatureBranchTests : IDisposable
{
    private readonly MergeScenario _s = new();

    public void Dispose() => _s.Dispose();

    [Theory]
    [Trait("Job", "contract: dydo-commands-delivery.md dydo merge Base - it must be feature/ and one or more /-separated segments, each starting with a letter or digit, and pass git check-ref-format --branch")]
    [InlineData("feature/x")]
    [InlineData("feature/a/b")]
    [InlineData("feature/DYD-417_lane.A")]
    public void AFeatureBranch_Passes(string name)
    {
        Assert.True(FeatureBranch.IsValid(name, _s.Root));
    }

    [Theory]
    [Trait("Job", "contract: dydo-commands-delivery.md dydo merge Base - it must be feature/ and one or more /-separated segments, each starting with a letter or digit, and pass git check-ref-format --branch")]
    [InlineData("feature/../master")]
    [InlineData("refs/heads/feature/x")]
    [InlineData("master")]
    [InlineData("Main")]
    [InlineData("feature/x.lock")]
    [InlineData("feature/")]
    [InlineData("feature")]
    [InlineData("feature/a..b")]
    [InlineData("feature//x")]
    [InlineData("feature/x/")]
    [InlineData("feature/-x")]
    [InlineData("Feature/x")]
    [InlineData("feature/x\n")]
    [InlineData("feature/x@{-1}")]
    public void AnythingElse_IsRefused(string name)
    {
        Assert.False(FeatureBranch.IsValid(name, _s.Root));
    }
}
