namespace DynaDocs.Tests.Steps;

using DynaDocs.Tests.Integration;
using DynaDocs.Services;
using Reqnroll;

[Binding]
public sealed class SafeDeleteSteps : IDisposable
{
    public SafeDeleteSteps() { }
    internal SafeDeleteSteps(SafeDeleteTests fixture) => _fixture = fixture;

    private SafeDeleteTests? _fixture;
    private Action? _verifyOutcome;
    private bool _dangerous;
    private string? _unpushedStaged;
    private string? _inputStatus;
    private string? _inputStaged;
    private byte[]? _exclude;
    private byte[]? _attributes;

    [When("I archive a worktree governed by repository-local exclude and attributes")]
    public void ArchiveWithCommonInputs()
    {
        var fixture = _fixture!;
        SafeDeleteInputsTests.Prepare(fixture);
        _inputStatus = SafeDeleteTests.Git(fixture.Tree,"status","--porcelain=v1","--ignored");
        _inputStaged = SafeDeleteTests.Git(fixture.Tree,"rev-parse",":unique.txt");
        _exclude = File.ReadAllBytes(Path.Combine(fixture.Main,".git","info","exclude"));
        _attributes = File.ReadAllBytes(Path.Combine(fixture.Main,".git","info","attributes"));
        Assert.True(fixture.Run("worktree",fixture.Tree) == 0,fixture.Output);
    }

    [Then("the archive restores ordinary ignored and attribute behavior after source garbage collection")]
    public void RestoreWithCommonInputs()
    {
        var fixture = _fixture!;
        Assert.False(Directory.Exists(fixture.Tree));
        Assert.False(fixture.Has("refs/heads/topic"));
        SafeDeleteTests.Git(fixture.Main,"reflog","expire","--expire=now","--all");
        SafeDeleteTests.Git(fixture.Main,"gc","--prune=now");
        Assert.False(GitRunner.RunIsolated(fixture.Main,new Dictionary<string,string>(),"cat-file","-e",_inputStaged!).Ok);
        Assert.True(fixture.Run("restore",fixture.Id) == 0,fixture.Output);
        Assert.Equal(_inputStatus,SafeDeleteTests.Git(fixture.Tree,"status","--porcelain=v1","--ignored"));
        Assert.Contains("!! local-only.log",_inputStatus);
        Assert.Equal("base\r\n",File.ReadAllText(Path.Combine(fixture.Tree,"file.txt")));
        Assert.Equal("private ignored bytes\n",File.ReadAllText(Path.Combine(fixture.Tree,"local-only.log")));
        Assert.Equal("unique staged bytes",SafeDeleteTests.Git(fixture.Tree,"show",":unique.txt"));
        Assert.Equal("file.txt: archive-proof: local",SafeDeleteTests.Git(fixture.Tree,"check-attr","archive-proof","--","file.txt"));
        Assert.Equal(_exclude,File.ReadAllBytes(Path.Combine(fixture.Main,".git","info","exclude")));
        Assert.Equal(_attributes,File.ReadAllBytes(Path.Combine(fixture.Main,".git","info","attributes")));
    }

    [When("I archive the unpushed worktree in a {string} closed-world repository")]
    public void ArchiveUnpushed(string visibility)
    {
        if (visibility is not ("public" or "private")) throw new ArgumentException("Unknown closed-world visibility",nameof(visibility));
        SafeDeleteGitHubTests.PrepareInitiallyAbsent(_fixture!,visibility,true);
        _unpushedStaged = SafeDeleteTests.Git(_fixture!.Tree,"rev-parse",":file.txt");
        Assert.True(_fixture!.Run("worktree",_fixture.Tree) == 0,_fixture.Output);
    }

    [Then("the absent remote ref required no push and the archived worktree restores independently")]
    public void RestoreUnpushed()
    {
        var fixture = _fixture!;
        Assert.False(Directory.Exists(fixture.Tree));
        Assert.False(fixture.Has("refs/heads/topic"));
        Assert.Contains("Independent proof passed",fixture.Output);
        SafeDeleteTests.Git(fixture.Main,"reflog","expire","--expire=now","--all");
        SafeDeleteTests.Git(fixture.Main,"gc","--prune=now");
        Assert.False(GitRunner.RunIsolated(fixture.Main,new Dictionary<string,string>(),"cat-file","-e",_unpushedStaged!).Ok);
        Assert.True(fixture.Run("restore",fixture.Id) == 0,fixture.Output);
        Assert.True(fixture.Has("refs/heads/topic"));
        Assert.Equal("staged only",SafeDeleteTests.Git(fixture.Tree,"show",":file.txt"));
        Assert.Equal("working bytes\n",File.ReadAllText(Path.Combine(fixture.Tree,"file.txt")));
        Assert.Empty(SafeDeleteTests.Git(fixture.Remote,"for-each-ref","refs/heads/topic"));
    }

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
