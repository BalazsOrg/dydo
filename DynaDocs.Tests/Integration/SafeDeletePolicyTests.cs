namespace DynaDocs.Tests.Integration;

using DynaDocs.Services;

[Collection("Integration")]
[Trait("Job", "contract: spec N10; DYD-1029 (unproven PR heads are refused)")]
public sealed class SafeDeletePolicyTests
{

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
