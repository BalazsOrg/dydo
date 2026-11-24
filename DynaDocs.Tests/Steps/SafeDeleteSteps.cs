namespace DynaDocs.Tests.Steps;

using DynaDocs.Tests.Integration;
using DynaDocs.Services;
using Reqnroll;

[Binding]
public sealed class SafeDeleteSteps : IDisposable
{
    private SafeDeleteTests? _fixture;
    private Action? _verifyOutcome;
    private bool _dangerous;

    [Given("an owned safe-delete repository")]
    public void GivenRepository() => _fixture = new SafeDeleteTests();

    [When("I archive and restore the {string} journey")]
    public void Journey(string journey) => _verifyOutcome = _fixture!.Journey(journey);

    [When("I exercise the {string} safe-delete refusal")]
    public void Refusal(string refusal) => _verifyOutcome = _fixture!.Refusal(refusal);

    [When("I exercise the {string} safe-delete policy")]
    public void Policy(string policy) => _verifyOutcome = _fixture!.Policy(policy);

    [Then("the safe-delete journey preserves all accepted state")]
    [Then("the safe-delete target refs and files remain intact")]
    [Then("the safe-delete policy outcome is verified")]
    public void Completed()
    {
        Assert.NotNull(_verifyOutcome);
        _verifyOutcome();
    }

    [When("the safe-delete guard examines {string}")]
    public void Guard(string command) => _dangerous = new BashCommandAnalyzer().CheckDangerousPatterns(command).IsDangerous;

    [Then("its safe-delete verdict is {string}")]
    public void Verdict(string verdict) => Assert.Equal(verdict == "refuse", _dangerous);

    public void Dispose() => _fixture?.Dispose();
}
