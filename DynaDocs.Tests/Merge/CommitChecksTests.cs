namespace DynaDocs.Tests.Merge;

using System.Text.Json.Nodes;
using DynaDocs.Services.Merge;
using static FakeGitHub;
using static MergeScenario;

/// <summary>
/// The CI table (design §2, CI rule): every check suite, check run and legacy status on the SHA must
/// have finished green, every required check must be among them, and some suite must hold CI.
/// </summary>
[Collection("Integration")]
public sealed class CommitChecksTests : IDisposable
{
    private const string NotGreen = "checks on 0123456789ab are not green: ";

    private readonly MergeScenario _s = new();

    public void Dispose() => _s.Dispose();

    private string? Refusal() => CommitChecks.Refusal(_s.GitHub, Repo, Sha, Target, _s.BranchJson);

    private void RunsAre(params JsonObject[] runs) => _s.GitHub.On("GET", Runs, Reply(200, RunPage(runs)));

    private void SuitesAre(params JsonObject[] suites) => _s.GitHub.On("GET", Suites, Reply(200, SuitePage(suites)));

    [Fact]
    [Trait("Job", "contract: DYD-417 design e6a6affe §2 CI rule - the combined status counts only when its total_count is above 0")]
    public void GreenSuiteAndRunWithNoLegacyStatuses_IsGreen()
    {
        Assert.Null(Refusal());
    }

    [Theory]
    [Trait("Job", "contract: DR 061 §5 - every latest check run must finish green")]
    [InlineData("queued")]
    [InlineData("in_progress")]
    [InlineData("pending")]
    [InlineData("requested")]
    [InlineData("waiting")]
    [InlineData("mystery")]
    public void ARunThatHasNotCompleted_IsNotGreen(string status)
    {
        RunsAre(Run("build", status, null));

        Assert.Equal(NotGreen + $"build {status}", Refusal());
    }

    [Theory]
    [Trait("Job", "contract: DR 061 §5 - any unfinished suite then refuses, whatever its run count")]
    [InlineData("queued")]
    [InlineData("in_progress")]
    [InlineData("pending")]
    [InlineData("requested")]
    [InlineData("waiting")]
    [InlineData("mystery")]
    public void ASuiteWithRunsThatHasNotCompleted_IsNotGreen(string status)
    {
        SuitesAre(Suite("GitHub Actions", status, null, runs: 2));

        Assert.Equal(NotGreen + $"GitHub Actions {status}", Refusal());
    }

    [Theory]
    [Trait("Job", "contract: dydo-commands-delivery.md dydo merge Checks - suites and runs must finish with success, neutral or skipped")]
    [InlineData("success")]
    [InlineData("neutral")]
    [InlineData("skipped")]
    public void AGreenConclusion_Passes(string conclusion)
    {
        SuitesAre(Suite("GitHub Actions", "completed", conclusion));
        RunsAre(Run("build", "completed", conclusion));

        Assert.Null(Refusal());
    }

    [Theory]
    [Trait("Job", "contract: dydo-commands-delivery.md dydo merge Checks - suites and runs must finish with success, neutral or skipped")]
    [InlineData("failure")]
    [InlineData("cancelled")]
    [InlineData("timed_out")]
    [InlineData("action_required")]
    [InlineData("stale")]
    [InlineData("startup_failure")]
    [InlineData("exploded")]
    public void AnyOtherConclusion_IsNotGreen(string conclusion)
    {
        RunsAre(Run("build", "completed", conclusion));

        Assert.Equal(NotGreen + $"build {conclusion}", Refusal());
    }

    [Fact]
    [Trait("Job", "contract: dydo-commands-delivery.md dydo merge Checks - suites and runs must finish with success, neutral or skipped")]
    public void ACompletedRunWithoutAConclusion_IsNotGreen()
    {
        RunsAre(Run("build", "completed", null));

        Assert.Equal(NotGreen + "build no conclusion", Refusal());
    }

    [Fact]
    [Trait("Job", "contract: DR 061 §5 - every latest check run must finish green")]
    public void EveryProblemIsNamed()
    {
        RunsAre(Run("build", "completed", "failure"), Run("lint", "completed", "success"), Run("e2e", "in_progress", null));

        Assert.Equal(NotGreen + "build failure; e2e in_progress", Refusal());
    }

    [Fact]
    [Trait("Job", "contract: dydo-commands-delivery.md dydo merge Checks - every latest check run is read to the last page")]
    public void AFailureBeyondPageOne_IsRead()
    {
        var green = Enumerable.Range(0, 100).Select(i => Run($"shard {i}", "completed", "success")).ToArray();
        var page1 = RunPage(green);
        page1["total_count"] = 101;
        _s.GitHub.On("GET", Runs, Reply(200, page1));
        _s.GitHub.On("GET", Runs.Replace("&page=1", "&page=2"), Reply(200, RunPage(Run("shard 100", "completed", "failure"))));

        Assert.Equal(NotGreen + "shard 100 failure", Refusal());
    }

    [Theory]
    [Trait("Job", "contract: dydo-commands-delivery.md dydo merge Checks - legacy statuses must be success")]
    [InlineData("pending")]
    [InlineData("failure")]
    [InlineData("error")]
    [InlineData("mystery")]
    public void ALegacyStatusThatIsNotSuccess_IsNotGreen(string state)
    {
        _s.GitHub.On("GET", Statuses, Reply(200, StatusPage(Status("ci/legacy", "success"), Status("ci/deploy", state))));

        Assert.Equal(NotGreen + $"ci/deploy {state}", Refusal());
    }

    [Fact]
    [Trait("Job", "contract: dydo-commands-delivery.md dydo merge Checks - every check the target's branch protection requires must be present")]
    public void ARequiredCheckMissing_IsNotGreen()
    {
        var required = _s.BranchJson["protection"]!["required_status_checks"]!;
        required["enforcement_level"] = "everyone";
        required["contexts"] = new JsonArray("build", "ci/legacy", "deploy-preview");
        _s.GitHub.On("GET", Statuses, Reply(200, StatusPage(Status("ci/legacy", "success"))));

        Assert.Equal(NotGreen + "deploy-preview missing", Refusal());
    }

    [Fact]
    [Trait("Job", "edge: branch protection that is off while it still lists a required context")]
    public void RequiredChecksOfProtectionThatIsOff_AreNotRequired()
    {
        _s.BranchJson["protection"]!["required_status_checks"]!["contexts"] = new JsonArray("deploy-preview");

        Assert.Null(Refusal());
    }

    [Fact]
    [Trait("Job", "contract: dydo-commands-delivery.md dydo merge Checks - every check the target's rulesets require must be present")]
    public void ARulesetsRequiredCheckMissing_IsNotGreen()
    {
        _s.GitHub.On("GET", Rules, Reply(200, """
            [
              { "type": "deletion" },
              { "type": "required_status_checks", "parameters": { "required_status_checks": [ { "context": "build" }, { "context": "lint" } ] } }
            ]
            """));

        Assert.Equal(NotGreen + "lint missing", Refusal());
    }

    // bodnarbalazs/LC is private on GitHub's free plan, which has no rulesets and answers 403 for them.
    [Fact]
    [Trait("Job", "trap: a private repository on GitHub's free plan answers rulesets with a 403 (bodnarbalazs/LC, DYD-424)")]
    public void ARepositoryWhosePlanHasNoRulesets_RequiresNoRulesetChecks()
    {
        _s.GitHub.On("GET", Rules, Reply(403, """{ "message": "Upgrade to GitHub Pro or make this repository public to enable this feature.", "status": "403" }"""));

        Assert.Null(Refusal());
    }

    [Fact]
    [Trait("Job", "contract: dydo-commands-delivery.md dydo merge - an unexpected GitHub answer such as a 403 is a tool error, with GitHub's message")]
    public void RulesetsGitHubWillNotShow_AreAToolError()
    {
        _s.GitHub.On("GET", Rules, Reply(403, """{ "message": "Resource not accessible by integration" }"""));

        var stop = Assert.Throws<MergeStop>(Refusal);

        Assert.Equal(2, stop.ExitCode);
        Assert.Contains("Resource not accessible by integration (HTTP 403)", stop.Lines[0]);
    }

    [Fact]
    [Trait("Job", "contract: DR 061 §5 - with no suite left, the command refuses no checks reported")]
    public void ZeroSuites_IsNoChecks()
    {
        SuitesAre();
        RunsAre();

        Assert.Equal("no checks reported on 0123456789ab; wait for CI, or this repo runs no CI for PRs into feature/x", Refusal());
    }

    // blacksmith-sh leaves a queued suite with no runs on every bodnarbalazs/dydo and LC commit.
    [Theory]
    [Trait("Job", "contract: DR 061 §5 - the inert app's suite is ignored only when its status is queued, in_progress, pending, requested or waiting and latest_check_runs_count is a valid integer zero")]
    [InlineData("queued")]
    [InlineData("in_progress")]
    [InlineData("pending")]
    [InlineData("requested")]
    [InlineData("waiting")]
    public void AnInertAppsKnownUnfinishedSuiteThatHoldsNoRun_IsIgnored(string status)
    {
        SuitesAre(Suite("blacksmith.sh", status, null, runs: 0, slug: "blacksmith-sh"), Suite("GitHub Actions", "completed", "success"));

        Assert.Null(Refusal());
    }

    [Fact]
    [Trait("Job", "contract: DR 061 §5 - with no suite left after the exception, the command refuses no checks reported")]
    public void OnlyAnInertAppsQueuedSuiteThatHoldsNoRun_IsNoChecks()
    {
        SuitesAre(Suite("blacksmith.sh", "queued", null, runs: 0, slug: "blacksmith-sh"));
        RunsAre();

        Assert.Equal("no checks reported on 0123456789ab; wait for CI, or this repo runs no CI for PRs into feature/x", Refusal());
    }

    [Fact]
    [Trait("Job", "contract: DR 061 §5 - every non-exempt suite is judged normally")]
    public void AnInertAppsSuiteWithARun_IsJudged()
    {
        SuitesAre(Suite("blacksmith.sh", "in_progress", null, runs: 1, slug: "blacksmith-sh"), Suite("GitHub Actions", "completed", "success"));

        Assert.Equal(NotGreen + "blacksmith-sh in_progress", Refusal());
    }

    [Theory]
    [Trait("Job", "contract: DR 061 Consequences - an unlisted app's unfinished suite refuses even with zero runs")]
    [InlineData("queued")]
    [InlineData("requested")]
    [InlineData("mystery")]
    public void AnotherAppsUnfinishedSuiteThatHoldsNoRun_IsNotGreen(string status)
    {
        SuitesAre(Suite("CircleCI Checks", status, null, runs: 0, slug: "circleci-checks"), Suite("GitHub Actions", "completed", "success"));

        Assert.Equal(NotGreen + $"circleci-checks {status}", Refusal());
    }

    [Fact]
    [Trait("Job", "contract: DR 061 §5 - every non-exempt suite is judged normally")]
    public void ACompletedSuiteThatHoldsNoRun_StillCounts()
    {
        SuitesAre(Suite("GitHub Actions", "completed", "startup_failure", runs: 0));
        RunsAre();

        Assert.Equal(NotGreen + "GitHub Actions startup_failure", Refusal());
    }

    [Theory]
    [Trait("Job", "contract: DR 061 §5 - every non-exempt suite is judged normally")]
    [InlineData("success", null)]
    [InlineData("failure", "blacksmith-sh failure")]
    [InlineData(null, "blacksmith-sh no conclusion")]
    public void AnInertAppsCompletedEmptySuite_IsJudged(string? conclusion, string? problem)
    {
        SuitesAre(Suite("blacksmith.sh", "completed", conclusion, runs: 0, slug: "blacksmith-sh"), Suite("GitHub Actions", "completed", "success"));

        Assert.Equal(problem == null ? null : NotGreen + problem, Refusal());
    }

    [Fact]
    [Trait("Job", "contract: dydo-commands-delivery.md dydo merge - the network is a tool error")]
    public void UnreadableChecks_AreAToolError()
    {
        _s.GitHub.On("GET", Runs, Reply(0));

        var stop = Assert.Throws<MergeStop>(Refusal);

        Assert.Equal(2, stop.ExitCode);
        Assert.StartsWith("error: could not read the checks on 0123456789ab", stop.Lines[0]);
    }
}
