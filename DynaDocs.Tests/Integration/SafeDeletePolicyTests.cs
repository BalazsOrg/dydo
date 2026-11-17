namespace DynaDocs.Tests.Integration;

using DynaDocs.Services;
using System.Diagnostics;

[Collection("Integration")]
public sealed class SafeDeletePolicyTests
{
    [Theory]
    [InlineData("[[]]","cross-repository PR head eligibility is unproven")]
    [InlineData("[]","incomplete")]
    [InlineData("not json","unavailable")]
    [InlineData("[[{}]]","unavailable")]
    [InlineData("[[{\"state\":\"closed\"}]]","inconsistent")]
    [InlineData("[[{\"state\":\"open\",\"head\":{\"ref\":\"topic\",\"repo\":{\"full_name\":\"owner/repo\"}},\"base\":{\"ref\":\"main\",\"repo\":{\"full_name\":\"owner/repo\"}}}]]","PR head")]
    [InlineData("[[{\"state\":\"open\",\"head\":{\"ref\":\"another\",\"repo\":{\"full_name\":\"fork/repo\"}},\"base\":{\"ref\":\"topic\",\"repo\":{\"full_name\":\"owner/repo\"}}}]]","PR base")]
    [InlineData("[[{\"state\":\"open\",\"head\":{\"ref\":\"topic\",\"repo\":null}}]]","identity unavailable")]
    [InlineData("[[{\"state\":\"open\",\"head\":{\"ref\":null,\"repo\":{\"full_name\":\"owner/repo\"}}}]]","incomplete")]
    [InlineData("[[{\"state\":\"open\",\"head\":{\"ref\":\"topic\",\"repo\":{\"full_name\":null}}}]]","incomplete")]
    [InlineData("[[{\"state\":\"open\",\"head\":{\"ref\":\"topic\",\"repo\":{\"full_name\":\"fork/repo\"}},\"base\":{\"ref\":\"main\",\"repo\":{\"full_name\":\"owner/repo\"}}}]]","cross-repository PR head eligibility is unproven")]
    public void GitHubEligibility_RequiresCompleteValidatedPages(string response, string refusal)
    {
        using var fixture = new SafeDeleteTests();
        var config = Path.Combine(fixture.Root,"query.config");
        SafeDeleteTests.Git(fixture.Main,"config","--file",config,"test.response",response);
        var reason = SafeDeleteGitHub.CheckUsing("https://github.com/owner/repo.git","topic",_ => Start(fixture.Main,"config","--file",config,"--get","test.response"));
        Assert.Contains(refusal,reason);
    }

    [Fact]
    public void ForkBranchUsedAsUpstreamPullRequestHead_RefusesWithRemoteLocalAndFilesIntact()
    {
        using var fixture = new SafeDeleteTests(); fixture.AddRemote(); fixture.AddTree();
        var tip = SafeDeleteTests.Git(fixture.Tree,"rev-parse","HEAD");
        var file = Path.Combine(fixture.Tree,"file.txt");
        File.WriteAllText(file,"unique fork working content\n");
        var api = Path.Combine(fixture.Root,"github-api.config");
        SafeDeleteTests.Git(fixture.Main,"config","--file",api,"api.fork/repo.response","[[]]");
        SafeDeleteTests.Git(fixture.Main,"config","--file",api,"api.upstream/repo.response",
            """[[{"state":"open","head":{"ref":"topic","repo":{"full_name":"fork/repo"}},"base":{"ref":"main","repo":{"full_name":"upstream/repo"}}}]]""");
        Assert.Contains("open PR head",SafeDeleteGitHub.CheckUsing("https://github.com/fork/repo.git","topic",
            _ => Start(fixture.Main,"config","--file",api,"--get","api.upstream/repo.response")));
        fixture.Dependencies.CheckGitHub = (url, branch) =>
        {
            Assert.Equal(fixture.Remote,url);
            return SafeDeleteGitHub.CheckUsing("https://github.com/fork/repo.git",branch,
                repository => Start(fixture.Main,"config","--file",api,"--get",$"api.{repository}.response"));
        };
        Assert.Equal(2,fixture.Run("worktree",fixture.Tree));
        Assert.Equal(tip,SafeDeleteTests.Git(fixture.Main,"rev-parse","topic"));
        Assert.Equal(tip,SafeDeleteTests.Git(fixture.Remote,"rev-parse","topic"));
        Assert.Equal("unique fork working content\n",File.ReadAllText(file));
        Assert.False(Directory.Exists(fixture.ArchiveRoot));
        Assert.Contains("cross-repository PR head eligibility is unproven",fixture.Output);
    }

    [Fact]
    public void GitHubEligibility_RefusesUnsupportedHostAndProcessFailures()
    {
        Assert.Contains("unsupported",SafeDeleteGitHub.Check("file:///local.git","topic"));
        Assert.Contains("protected",SafeDeleteGitHub.Check("https://github.com/owner/repo","feature/integration"));
        using var fixture = new SafeDeleteTests();
        Assert.Contains("failed",SafeDeleteGitHub.CheckUsing("git@github.com:owner/repo.git","topic",_ => Start(fixture.Main,"not-a-git-command")));
        Assert.Contains("unavailable",SafeDeleteGitHub.CheckUsing("https://github.com/owner/repo.git","topic",_ => new ProcessStartInfo("dydo-nonexistent-gh") { UseShellExecute = false }));
    }

    private static ProcessStartInfo Start(string cwd, params string[] arguments)
    {
        var start = new ProcessStartInfo("git") { WorkingDirectory = cwd, UseShellExecute = false, RedirectStandardInput = true, RedirectStandardOutput = true, RedirectStandardError = true };
        foreach (var argument in arguments) start.ArgumentList.Add(argument);
        return start;
    }

    [Theory]
    [InlineData("{}",false)]
    [InlineData("{\"worktreesDirectory\":\"\"}",false)]
    [InlineData("{\"worktreesDirectory\":null}",true)]
    [InlineData("{\"worktreesDirectory\":\"relative\"}",true)]
    [InlineData("{\"previousWorktreesDirectories\":42}",true)]
    [InlineData("[]",true)]
    [InlineData("corrupt",true)]
    public void AuthoritativeT3Settings_InvalidOrUnreadableRefusesOrdinaryTrees(string json, bool refused)
    {
        using var fixture = new SafeDeleteTests();
        var settings = Path.Combine(fixture.Root,"settings.json"); File.WriteAllText(settings,json);
        var reason = new SafeDeleteT3Policy(settings).Refusal(fixture.Tree);
        Assert.Equal(refused,reason != null);
        if (refused) Assert.Contains("authoritative settings",reason);
    }

    [Fact]
    public void T3HistoryPathRecognition_RequiresNoSettingsFile()
    {
        using var fixture = new SafeDeleteTests();
        var settings = Path.Combine(fixture.Root,"absent.json");
        Assert.Contains("T3 ownership/activity unavailable",new SafeDeleteT3Policy(settings).Refusal(Path.Combine(fixture.Root,".t3","worktrees","project","tree")));
    }

    [Fact]
    public void Redaction_DropsUserinfoQueryAndFragment()
    {
        var redacted = SafeDeleteGit.Redact("https://user:credential@github.com/owner/repo.git?token=secret#private");
        Assert.Equal("https://github.com/owner/repo.git",redacted);
    }
}
