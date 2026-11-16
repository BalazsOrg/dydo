namespace DynaDocs.Tests.Steps;

using DynaDocs.Tests.Integration;
using DynaDocs.Services;
using Reqnroll;

[Binding]
public sealed class SafeDeleteSteps : IDisposable
{
    private SafeDeleteTests? _fixture;
    private bool _completed;
    private bool _dangerous;

    [Given("an owned safe-delete repository")]
    public void GivenRepository() => _fixture = new SafeDeleteTests();

    [When("I archive and restore the {string} journey")]
    public void Journey(string journey) { _fixture!.Journey(journey); _completed = true; }

    [When("I exercise the {string} safe-delete refusal")]
    public void Refusal(string refusal) { _fixture!.Refusal(refusal); _completed = true; }

    [When("I exercise the {string} safe-delete policy")]
    public void Policy(string policy) { _fixture!.Policy(policy); _completed = true; }

    [Then("the safe-delete journey preserves all accepted state")]
    [Then("the safe-delete target refs and files remain intact")]
    [Then("the safe-delete policy outcome is verified")]
    public void Completed() => Assert.True(_completed);

    [When("the safe-delete guard examines {string}")]
    public void Guard(string command) => _dangerous = new BashCommandAnalyzer().CheckDangerousPatterns(command).IsDangerous;

    [Then("its safe-delete verdict is {string}")]
    public void Verdict(string verdict) => Assert.Equal(verdict == "refuse", _dangerous);

    public void Dispose() => _fixture?.Dispose();
}
