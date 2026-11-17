namespace DynaDocs.Tests.Steps;

using DynaDocs.Tests.Integration;
using Xunit.Sdk;

[Collection("Integration")]
public sealed class SafeDeleteBindingsTests
{
    [Theory]
    [InlineData("local bra nch")]
    [InlineData("detached HEAD typo")]
    [InlineData("proof-enabled settled T3 eligibility typo")]
    [InlineData("LOCAL BRANCH")]
    [InlineData("")]
    public void UnknownJourney_RefusesBeforeArchiving(string journey)
    {
        using var fixture = new SafeDeleteTests();
        var original = SafeDeleteTests.Git(fixture.Main,"rev-parse","topic");
        var error = Assert.Throws<XunitException>(() => fixture.Journey(journey));
        Assert.Equal("Unwired journey: " + journey,error.Message);
        Assert.False(Directory.Exists(fixture.ArchiveRoot));
        Assert.Equal(original,SafeDeleteTests.Git(fixture.Main,"rev-parse","topic"));
    }

    [Theory]
    [InlineData("journey")]
    [InlineData("refusal")]
    [InlineData("policy")]
    public void Then_DetectsMissingTargetRefAfterWhen(string binding)
    {
        using var fixture = new SafeDeleteTests();
        using var target = new SafeDeleteSteps(fixture);
        switch (binding)
        {
            case "journey": target.Journey("local branch"); break;
            case "refusal": target.Refusal("locked worktree"); break;
            case "policy": target.Policy("dry-run estimates"); break;
        }
        target.Completed();
        SafeDeleteTests.Git(fixture.Main,"update-ref","-d","refs/heads/topic");
        Assert.ThrowsAny<XunitException>(target.Completed);
    }
}
