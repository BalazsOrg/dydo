namespace DynaDocs.Tests.Merge;

using System.Text.Json.Nodes;
using DynaDocs.Services;
using DynaDocs.Utils;
using static DynaDocs.Tests.Integration.PushedRepository;
using static FakeGitHub;
using static MergeScenario;

/// <summary>
/// <c>dydo merge</c> end to end against the fake GitHub, judged by its outcome: the exit code, and what
/// was written to GitHub or where the target ref ended. DR 061 §1-§5: every refusal writes nothing, a
/// merge lands once as a merge commit (two parents where GitHub would fast-forward), and the target pin
/// (DYD-523) only refuses.
/// </summary>
[Collection("Integration")]
public sealed class PullRequestMergeTests : IDisposable
{
    private const string ZeroSha = "0000000000000000000000000000000000000000";
    private const string Commits = Repo + "/git/commits";

    private readonly MergeScenario _s = new();

    public void Dispose() => _s.Dispose();

    // The tip is an ancestor of the SHA, so GitHub would fast-forward: the merge commit is written by hand.
    private TargetBranch FastForwardEligible(Dictionary<string, string[]>? parents = null, string? landsMeanwhile = null)
    {
        _s.GitHub.On("GET", Compare(Tip, Sha), Reply(200, """{ "status": "ahead" }"""));
        _s.GitHub.On("GET", $"{Commits}/{Sha}", Reply(200, $$"""{ "sha": "{{Sha}}", "tree": { "sha": "4444444444444444444444444444444444444444" } }"""));
        return new TargetBranch(_s.GitHub, parents ?? new() { [Sha] = [Tip] }, landsMeanwhile);
    }

    private static string Field(JsonObject? body, string name) => body![name]!.GetValue<string>();

    private void AssertLandedOnce()
    {
        var post = Assert.Single(_s.GitHub.Writes);
        Assert.Equal(("POST", Merges, Target, Sha), (post.Method, post.Path, Field(post.Body, "base"), Field(post.Body, "head")));
    }

    #region Refusals: exit 1, nothing written

    [Theory]
    [Trait("Job", "contract: DR 061 §2 and its 2026-11-04 amendment; dydo-commands-delivery.md dydo merge (the row's input fails one of the checks dydo merge makes, in order, before it merges, or the tip reread just before the POST: a refusal exits 1 with a `refused:` line on stderr, and nothing is written to GitHub)")]
    [InlineData("no dydo project", null)]
    [InlineData("git top level is not the project root", null)]
    [InlineData("no origin", null)]
    [InlineData("origin names no owner and repository", null)]
    [InlineData("head pin", null)]
    [InlineData("head pin", "0123456")]
    [InlineData("head pin", "0123456789abcdef0123456789abcdef0123456")]
    [InlineData("head pin", "0123456789abcdef0123456789abcdef012345678")]
    [InlineData("head pin", "g123456789abcdef0123456789abcdef01234567")]
    [InlineData("no such PR", null)]
    [InlineData("PR in another repository", null)]
    [InlineData("draft", null)]
    [InlineData("fork", "mallory/widgets")]
    [InlineData("fork", null)]
    [InlineData("base outside feature/*", "develop")]
    [InlineData("base outside feature/*", "feature/x.lock")]
    [InlineData("default branch", "master")]
    [InlineData("default branch", "feature/x")]
    [InlineData("default branch", "Feature/X")]
    [InlineData("master or main", "master")]
    [InlineData("master or main", "main")]
    [InlineData("master or main", "Main")]
    [InlineData("local origin/HEAD", null)]
    [InlineData("unreadable default branch", """{ "message": "Forbidden" }""")]
    [InlineData("unreadable default branch", """{ "full_name": "acme/widgets" }""")]
    [InlineData("unreadable default branch", """{ "full_name": "acme/widgets", "default_branch": "" }""")]
    [InlineData("head moved past the review", null)]
    [InlineData("no PASS for the SHA", null)]
    [InlineData("no review section", null)]
    [InlineData("a FAIL for the SHA", null)]
    [InlineData("rubric", "spec")]
    [InlineData("rubric", "project-plan")]
    [InlineData("review base not in target", """{ "status": "diverged" }""")]
    [InlineData("review base not in target", """{ "status": "behind" }""")]
    [InlineData("review base not in target", null)]
    [InlineData("no checks", null)]
    [InlineData("checks not green", null)]
    [InlineData("inert app status", "\"mystery\"")]
    [InlineData("inert app status", null)]
    [InlineData("inert app status", "null")]
    [InlineData("inert app status", "0")]
    [InlineData("inert app status", "{}")]
    [InlineData("inert app count", null)]
    [InlineData("inert app count", "null")]
    [InlineData("inert app count", "\"0\"")]
    [InlineData("inert app count", "\"invalid\"")]
    [InlineData("inert app count", "true")]
    [InlineData("inert app count", "{}")]
    [InlineData("inert app count", "[]")]
    [InlineData("inert app count", "0.5")]
    [InlineData("inert app count", "-1")]
    [InlineData("inert app count", "1")]
    [InlineData("inert app count", "9223372036854775808")]
    [InlineData("base gone before the checks", null)]
    [InlineData("closed, not integrated", null)]
    [InlineData("closed into master", null)]
    [InlineData("closed, base deleted", null)]
    [InlineData("tip moved before the post", null)]
    public void ARefusal_ExitsOne_AndWritesNothing(string refusal, string? value)
    {
        string? directory = null;
        string? sha = Sha;
        string? bare = null;
        switch (refusal)
        {
            case "no dydo project":
                bare = Path.Combine(Path.GetTempPath(), "dydo-merge-none-" + Guid.NewGuid().ToString("N")[..8]);
                Directory.CreateDirectory(bare);
                Git(bare, "init");
                directory = bare;
                break;
            case "git top level is not the project root":
                directory = Path.Combine(_s.Root, "sub");
                Directory.CreateDirectory(directory);
                File.WriteAllText(Path.Combine(directory, "dydo.json"), "{}\n");
                break;
            case "no origin":
                Git(_s.Root, "remote", "remove", "origin");
                break;
            case "origin names no owner and repository":
                Git(_s.Root, "remote", "set-url", "origin", "C:/repositories/widgets.git");
                break;
            case "head pin":
                sha = value;
                break;
            case "no such PR":
                _s.GitHub.On("GET", Pulls, Reply(404, """{ "message": "Not Found" }"""));
                break;
            case "PR in another repository":
                _s.PullJson["base"]!["repo"]!["full_name"] = "acme/gadgets";
                break;
            case "draft":
                _s.PullJson["draft"] = true;
                break;
            case "fork":
                if (value == null)
                {
                    _s.PullJson["head"]!["repo"] = null;
                    _s.PullJson["head"]!["label"] = "mallory:frame";
                }
                else
                    _s.PullJson["head"]!["repo"]!["full_name"] = value;
                break;
            case "base outside feature/*":
                _s.PullJson["base"]!["ref"] = value;
                break;
            case "default branch":
                _s.PullJson["base"]!["ref"] = value == "master" ? "master" : "feature/x";
                _s.GitHub.On("GET", Repo, Reply(200, new JsonObject { ["full_name"] = "acme/widgets", ["default_branch"] = value }));
                break;
            case "master or main":
                _s.PullJson["base"]!["ref"] = value;
                _s.GitHub.On("GET", Repo, Reply(200, """{ "full_name": "acme/widgets", "default_branch": "trunk" }"""));
                break;
            case "local origin/HEAD":
                Git(_s.Root, "symbolic-ref", "refs/remotes/origin/HEAD", "refs/remotes/origin/feature/x");
                break;
            case "unreadable default branch":
                _s.GitHub.On("GET", Repo, Reply(value!.Contains("Forbidden") ? 403 : 200, value));
                break;
            case "head moved past the review":
                _s.PullJson["head"]!["sha"] = "5555555555555555555555555555555555555555";
                break;
            case "no PASS for the SHA":
                _s.PullJson["body"] = Body(PassLine("5555555555555555555555555555555555555555"), PassLine("abcdef1"));
                break;
            case "no review section":
                _s.PullJson["body"] = "Closes DYD-1.\n\n" + PassLine() + "\n";
                break;
            case "a FAIL for the SHA":
                _s.PullJson["body"] = Body(PassLine(),
                    "FAIL - rubric/type: code; contract DYD-1; candidate " + Sha + "; base " + ReviewBase + "; reviewer/model: r; gates 2/3.");
                break;
            case "rubric":
                _s.PullJson["body"] = Body(PassLine(rubric: value!));
                break;
            case "review base not in target":
                _s.GitHub.On("GET", Compare(ReviewBase, Tip), value == null ? Reply(404, """{ "message": "Not Found" }""") : Reply(200, value));
                break;
            case "no checks":
                _s.GitHub.On("GET", Suites, Reply(200, SuitePage()));
                break;
            case "checks not green":
                _s.GitHub.On("GET", Runs, Reply(200, RunPage(Run("build", "completed", "failure"))));
                break;
            case "inert app status":
                InertApp("status", value);
                break;
            case "inert app count":
                InertApp("latest_check_runs_count", value);
                break;
            case "base gone before the checks":
                _s.GitHub.On("GET", Branch, Reply(404, """{ "message": "Branch not found" }"""));
                break;
            case "closed, not integrated":
                _s.PullJson["state"] = "closed";
                _s.GitHub.On("GET", Compare(Sha, Tip), Reply(200, """{ "status": "diverged" }"""));
                break;
            case "closed into master":
                _s.PullJson["state"] = "closed";
                _s.PullJson["merged"] = true;
                _s.PullJson["base"]!["ref"] = "master";
                break;
            case "closed, base deleted":
                _s.PullJson["state"] = "closed";
                _s.GitHub.On("GET", Branch, Reply(404, """{ "message": "Branch not found" }"""));
                break;
            case "tip moved before the post":
                _s.GitHub.On("GET", Branch, Reply(200, _s.BranchJson), Reply(200, BranchAt(Moved)));
                break;
            default:
                throw new ArgumentException(refusal);
        }

        try
        {
            var (exit, _, stderr) = _s.Merge(sha, directory: directory);

            Assert.Equal(1, exit);
            Assert.StartsWith("refused: ", stderr, StringComparison.Ordinal);
            Assert.Empty(_s.GitHub.Writes);
        }
        finally
        {
            if (bare != null)
                LinkSafeDelete.DeleteTree(bare);
        }
    }

    // DR 061 §5: blacksmith-sh's queued suite is ignored only with a known waiting status and a valid integer zero count.
    private void InertApp(string field, string? json)
    {
        var suite = Suite("blacksmith.sh", "queued", null, runs: 0, slug: "blacksmith-sh");
        suite.Remove(field);
        if (json != null)
            suite[field] = JsonNode.Parse(json);
        _s.GitHub.On("GET", Suites, Reply(200, SuitePage(suite, Suite("GitHub Actions", "completed", "success"))));
    }

    // GitHub itself refuses the landing; the command exits 1 and nothing merges.
    [Theory]
    [Trait("Job", "contract: dydo-commands-delivery.md dydo merge exit 1 (GitHub refusing the merge POST, by row a 409 conflict or a 404 for a base gone and so off the pin, is a refusal: exit 1, and that POST is the only write)")]
    [InlineData("conflict")]
    [InlineData("base gone when the merge arrives")]
    public void GitHubRefusingThePost_ExitsOne_AndNothingElseIsWritten(string refusal)
    {
        _s.GitHub.On("POST", Merges, refusal == "conflict"
            ? Reply(409, """{ "message": "Merge conflict" }""")
            : Reply(404, """{ "message": "Base does not exist" }"""));

        var (exit, _, _) = _s.Merge();

        var write = Assert.Single(_s.GitHub.Writes);
        Assert.Equal((1, "POST", Merges), (exit, write.Method, write.Path));
    }

    #endregion

    #region Tool errors: exit 2, the target does not move

    [Theory]
    [Trait("Job", "contract: dydo-commands-delivery.md dydo merge exit 2 (an unexpected answer to the merge POST, the row's 403 or 422, is a tool error: exit 2)")]
    [InlineData(403, "Resource not accessible by integration")]
    [InlineData(422, "Validation Failed")]
    [InlineData(422, "Repository rule violations found")]
    public void AnUnexpectedMergeAnswer_IsAToolError(int status, string message)
    {
        _s.GitHub.On("POST", Merges, Reply(status, new JsonObject { ["message"] = message }));

        Assert.Equal(ExitCodes.ToolError, _s.Merge().Exit);
    }

    [Theory]
    [Trait("Job", "contract: dydo-commands-delivery.md dydo merge exit 2 (gh that cannot reach GitHub for the PR, through the row's HTTPS origin or SSH origin, is a tool error: exit 2, nothing written)")]
    [InlineData("https://github.com/acme/widgets.git")]
    [InlineData("git@github.com-work:acme/widgets.git")]
    public void GhThatCannotReachGitHub_IsAToolError_WithNothingWritten(string origin)
    {
        using var s = new MergeScenario(origin);
        s.GitHub.On("GET", Pulls, Reply(0));

        Assert.Equal(ExitCodes.ToolError, s.Merge().Exit);
        Assert.Empty(s.GitHub.Writes);
    }

    [Theory]
    [Trait("Job", "contract: dydo-commands-delivery.md dydo merge (a two-parent ref move that fails with the row's GraphQL answer while the base is still at the pin is a tool error: exit 2, the base still on the pin)")]
    [InlineData(200, """{ "data": { "updateRefs": null }, "errors": [ { "message": "Repository rule violations found" } ] }""")]
    [InlineData(200, """{ "data": { "updateRefs": null }, "errors": [ { "message": "one" }, { "type": "FORBIDDEN" } ] }""")]
    [InlineData(200, """{ "data": null }""")]
    [InlineData(200, """{ "data": { "updateRefs": null } }""")]
    [InlineData(200, """{ "data": { "updateRefs": { "clientMutationId": null } }, "errors": [ { "message": "partial" } ] }""")]
    [InlineData(200, "{}")]
    [InlineData(200, null)]
    [InlineData(500, """{ "data": { "updateRefs": { "clientMutationId": null } } }""")]
    [InlineData(502, null)]
    [InlineData(0, null)]
    public void ARefMoveThatFailsOnAnUnmovedTip_IsAToolError_AndTheTargetStaysOnThePin(int status, string? json)
    {
        var target = FastForwardEligible();
        _s.GitHub.On("POST", "graphql", json == null ? Reply(status) : Reply(status, json));

        Assert.Equal(ExitCodes.ToolError, _s.Merge().Exit);
        Assert.Equal(Tip, target.Tip);
    }

    // An all-zero afterOid would delete the ref, so only a real commit SHA reaches the move.
    [Theory]
    [Trait("Job", "trap: an all-zero afterOid would delete the ref: a two-parent commit answer naming the row's non-SHA, all zeros, short or none, is an unexpected GitHub answer and a tool error (dydo-commands-delivery.md dydo merge exit 2): exit 2, no ref move sent, the base on the pin")]
    [InlineData(ZeroSha)]
    [InlineData("3333")]
    [InlineData(null)]
    public void AMergeCommitGitHubDoesNotNameAsACommit_IsAToolError_BeforeTheRefMoves(string? named)
    {
        var target = FastForwardEligible();
        _s.GitHub.On("POST", Commits, Reply(201, new JsonObject { ["sha"] = named }));

        Assert.Equal(ExitCodes.ToolError, _s.Merge().Exit);
        Assert.DoesNotContain(_s.GitHub.Writes, write => write.Path == "graphql");
        Assert.Equal(Tip, target.Tip);
    }

    [Fact]
    [Trait("Job", "contract: dydo-commands-delivery.md dydo merge exit 2 (on the two-parent path, a repository answer without the node id the ref move needs is a tool error: exit 2, nothing written)")]
    public void ARepositoryWithoutANodeId_IsAToolError_BeforeAnyWrite()
    {
        FastForwardEligible();
        _s.GitHub.On("GET", Repo, Reply(200, """{ "full_name": "acme/widgets", "default_branch": "master" }"""));

        Assert.Equal(ExitCodes.ToolError, _s.Merge().Exit);
        Assert.Empty(_s.GitHub.Writes);
    }

    // The merge landed, but its first parent cannot be shown to be the pin: exit 2 for merge review.
    [Theory]
    [Trait("Job", "contract: dydo-commands-delivery.md dydo merge exit 2 (a merge whose first parent is not shown to be the pin, by the row's commit answer: another first parent, no parents, or unreadable, exits 2)")]
    [InlineData("moved")]
    [InlineData("no parents")]
    [InlineData("unreadable")]
    public void AMergeWhoseFirstParentIsNotProvenThePin_ExitsTwo(string parents)
    {
        var commit = MergeCommitJson(Moved);
        if (parents == "no parents")
            commit["parents"] = new JsonArray();
        _s.GitHub.On("GET", MergeCommit, parents == "unreadable" ? Reply(404, """{ "message": "Not Found" }""") : Reply(200, commit));

        Assert.Equal(ExitCodes.ToolError, _s.Merge().Exit);
    }

    #endregion

    #region Landing

    [Theory]
    [Trait("Job", "contract: DR 061 §1 §3; dydo-commands-delivery.md dydo merge (a reviewed green PR, its SHA given in the row's letter case, lands once: exit 0, gh asked on origin's host, one write, a POST /merges naming the base and the SHA)")]
    [InlineData(Sha)]
    [InlineData("0123456789ABCDEF0123456789ABCDEF01234567")]
    public void AReviewedGreenPr_LandsOnceAsAMergeIntoItsFeatureBase(string sha)
    {
        var (exit, _, stderr) = _s.Merge(sha);

        Assert.True(exit == 0, stderr);
        Assert.Equal("github.com", _s.Host);
        AssertLandedOnce();
    }

    [Fact]
    [Trait("Job", "contract: DR 061 §4; dydo-commands-delivery.md dydo merge Review (a PASS naming no base still lands: exit 0, one write, a POST /merges naming the base and the SHA)")]
    public void APassNamingNoBase_StillLands()
    {
        _s.PullJson["body"] = Body(PassLine(reviewBase: null));

        var (exit, _, stderr) = _s.Merge();

        Assert.True(exit == 0, stderr);
        AssertLandedOnce();
    }

    [Theory]
    [Trait("Job", "contract: dydo-commands-delivery.md dydo merge --dry-run (a dry run exits 0 when the checks pass and 1 when a run failed, by row, and writes nothing)")]
    [InlineData(false, 0)]
    [InlineData(true, 1)]
    public void ADryRun_ChecksLikeAMerge_AndWritesNothing(bool failingChecks, int expected)
    {
        if (failingChecks)
            _s.GitHub.On("GET", Runs, Reply(200, RunPage(Run("build", "completed", "failure"))));

        Assert.Equal(expected, _s.Merge(dryRun: true).Exit);
        Assert.Empty(_s.GitHub.Writes);
    }

    [Fact]
    [Trait("Job", "contract: DR 061 §3; dydo-commands-delivery.md dydo merge (when the pin is an ancestor of the SHA, one commit with parents the pin then the SHA is written and the base moves onto it: exit 0)")]
    public void BaseTipAncestorOfTheSha_LandsATwoParentCommit_OnThePin()
    {
        var target = FastForwardEligible();

        var (exit, _, stderr) = _s.Merge();

        Assert.True(exit == 0, stderr);
        Assert.Equal(MergeSha, target.Tip);
        var commit = Assert.Single(_s.GitHub.Writes, write => write.Path == Commits);
        Assert.Equal([Tip, Sha], commit.Body!["parents"]!.AsArray().Select(parent => parent!.GetValue<string>()));
    }

    // DYD-523 review round 1: another merge landing first fails the move, even when its tip is in the SHA's
    // history, where an unforced fast-forward from that tip to the merge commit would still have been allowed.
    [Theory]
    [Trait("Job", "contract: dydo-commands-delivery.md dydo merge (a base another merge moved before the ref move, by row to a commit in the SHA's history or not, fails the move and refuses: exit 1, the base left where that merge put it)")]
    [InlineData(true)]
    [InlineData(false)]
    public void ATargetThatMovesBeforeTheRefMove_RefusesAndNothingMerges(bool movedIsInTheShasHistory)
    {
        var target = FastForwardEligible(
            movedIsInTheShasHistory ? new() { [Moved] = [Tip], [Sha] = [Moved] } : new() { [Moved] = [Tip], [Sha] = [Tip] },
            landsMeanwhile: Moved);

        Assert.Equal(1, _s.Merge().Exit);
        Assert.Equal(Moved, target.Tip);
    }

    [Fact]
    [Trait("Job", "contract: dydo-commands-delivery.md dydo merge (a ref move rejected while the base was deleted, the reread finding it gone, refuses: exit 1)")]
    public void ABaseGoneBeforeTheRefMove_RefusesAndNothingMerges()
    {
        FastForwardEligible();
        _s.GitHub.On("POST", "graphql", Reply(200, """{ "data": { "updateRefs": null }, "errors": [ { "message": "A ref update was rejected" } ] }"""));
        _s.GitHub.On("GET", Branch, Reply(200, _s.BranchJson), Reply(404, """{ "message": "Branch not found" }"""));

        Assert.Equal(1, _s.Merge().Exit);
    }

    // The re-read finds the ref on the merge commit: the move landed, though its answer was lost.
    [Fact]
    [Trait("Job", "contract: dydo-commands-delivery.md dydo merge exit 0 (a merge that succeeded exits 0: the ref move's answer was lost, and the reread ref is on the merge commit)")]
    public void ARefMoveWhoseAnswerWasLost_ButLanded_ExitsZero()
    {
        FastForwardEligible();
        _s.GitHub.On("POST", "graphql", Reply(0));
        _s.GitHub.On("GET", Branch, Reply(200, _s.BranchJson), Reply(200, BranchAt(MergeSha)));

        var (exit, _, stderr) = _s.Merge();

        Assert.True(exit == 0, stderr);
    }

    // GitHub marks a PR merged asynchronously (DYD-535): whatever the rereads after a good write say, it landed once.
    [Theory]
    [Trait("Job", "contract: dydo-commands-delivery.md dydo merge exit 0; DYD-535 (whatever the row's PR rereads after a landing say, marked at once or on the second reread, never marked, or failing, the run exits 0; on a POST row that POST /merges is the only write, on a two-parent row the base is on the merge commit)")]
    [InlineData("marked at once", false)]
    [InlineData("marked on the second reread", false)]
    [InlineData("marked on the second reread", true)]
    [InlineData("never marked", false)]
    [InlineData("never marked", true)]
    [InlineData("reread fails", false)]
    [InlineData("reread fails", true)]
    [InlineData("reread fails after not yet", false)]
    public void AfterALanding_WhateverTheRereadsSay_ItExitsZero(string rereads, bool twoParent)
    {
        var target = twoParent ? FastForwardEligible() : null;
        switch (rereads)
        {
            case "marked on the second reread":
                _s.GitHub.On("GET", Pulls, Reply(200, _s.PullJson), Reply(200, _s.PullJson), Reply(200, MergedPull()));
                break;
            case "never marked":
                _s.GitHub.On("GET", Pulls, Reply(200, _s.PullJson));
                break;
            case "reread fails":
                _s.GitHub.On("GET", Pulls, Reply(200, _s.PullJson), Reply(0));
                break;
            case "reread fails after not yet":
                _s.GitHub.On("GET", Pulls, Reply(200, _s.PullJson), Reply(200, _s.PullJson), Reply(502, """{ "message": "Bad Gateway" }"""));
                break;
        }

        var (exit, _, stderr) = _s.Merge();

        Assert.True(exit == 0, stderr);
        if (target == null)
            AssertLandedOnce();
        else
            Assert.Equal(MergeSha, target.Tip);
    }

    #endregion

    #region Reconcile first: already integrated exits 0 and writes nothing new

    [Theory]
    [Trait("Job", "contract: DR 061 §2; dydo-commands-delivery.md dydo merge (a closed PR whose SHA its feature base holds is already integrated whatever the pin, by row the tip or a stale one: exit 0, nothing written)")]
    [InlineData(Tip)]
    [InlineData(Stale)]
    public void AClosedPrAlreadyInItsFeatureBase_ExitsZero_WithoutAWrite_WhateverThePin(string pin)
    {
        _s.PullJson["state"] = "closed";
        _s.PullJson["merged"] = true;
        _s.GitHub.On("GET", Compare(Sha, Tip), Reply(200, """{ "status": "ahead" }"""));

        var (exit, _, stderr) = _s.Merge(pin: pin);

        Assert.True(exit == 0, stderr);
        Assert.Empty(_s.GitHub.Writes);
    }

    [Fact]
    [Trait("Job", "contract: dydo-commands-delivery.md dydo merge exit 0 (a merge POST GitHub answers 204, nothing to merge, is a SHA already integrated: exit 0)")]
    public void AMergeGitHubAnswers204_IsAlreadyIntegrated_AndExitsZero()
    {
        _s.GitHub.On("POST", Merges, Reply(204));

        var (exit, _, stderr) = _s.Merge();

        Assert.True(exit == 0, stderr);
    }

    [Theory]
    [Trait("Job", "contract: dydo-commands-delivery.md dydo merge A closed PR first (a rerun after a lost merge answer is safe: the lost answer exits 2, the rerun exits 0, and the writes are the row's: one POST when the PR reads closed and integrated, a second POST answered 204 when it still shows open)")]
    [InlineData("closed and integrated")]
    [InlineData("still shows open, GitHub answers 204")]
    public void ARerunAfterALostMergeAnswer_ExitsZero_WithoutASecondMerge(string rerun)
    {
        if (rerun == "closed and integrated")
        {
            _s.GitHub.On("POST", Merges, Reply(0));
            _s.GitHub.On("GET", Compare(Sha, Tip), Reply(200, """{ "status": "ahead" }"""));
        }
        else
        {
            _s.GitHub.On("GET", Pulls, Reply(200, _s.PullJson));
            _s.GitHub.On("POST", Merges, Reply(0), Reply(204));
        }

        var lost = _s.Merge();
        var again = _s.Merge();

        Assert.Equal(ExitCodes.ToolError, lost.Exit);
        Assert.Equal(0, again.Exit);
        Assert.Equal(rerun == "closed and integrated" ? 1 : 2, _s.GitHub.Writes.Count());
    }

    #endregion

    #region The target pin (DYD-523): read from the ref, and it only refuses

    // GitHub's PR metadata may keep a tip from before the ref advanced; only the ref decides.
    [Fact]
    [Trait("Job", "contract: DR 061 2026-11-04 amendment; dydo-commands-delivery.md dydo merge Target tip (a stale PR base.sha with the pin on the branch ref lands: exit 0, one write, a POST /merges naming the base and the SHA)")]
    public void AStaleBaseShaWithThePinOnTheActualRef_LandsOntoTheRef()
    {
        _s.PullJson["base"]!["sha"] = Stale;

        var (exit, _, stderr) = _s.Merge(pin: Tip);

        Assert.True(exit == 0, stderr);
        AssertLandedOnce();
    }

    [Theory]
    [Trait("Job", "contract: DR 061 §2 and its 2026-11-04 amendment (the pin only refuses, by row: on the tip a real run exits 0 and writes, each write on the base and the pin, the POST /merges naming the base and the SHA, the two-parent commit's first parent the pin, the ref move taking the base from the pin to the merge commit; a dry run on the tip exits 0 and writes nothing; a pin off the tip, stale or moved, exits 1 and writes nothing)")]
    [InlineData(false, Tip, false)]
    [InlineData(true, Tip, false)]
    [InlineData(false, Tip, true)]
    [InlineData(false, Stale, false)]
    [InlineData(true, Stale, false)]
    [InlineData(false, Stale, true)]
    [InlineData(false, Moved, false)]
    [InlineData(true, Moved, false)]
    public void ThePinOnlyRefuses_EveryWriteTargetsTheBaseRefAndAPinOffTheTipWritesNothing(bool twoParent, string pin, bool dryRun)
    {
        if (twoParent)
            FastForwardEligible();
        _s.PullJson["base"]!["sha"] = pin;

        var (exit, _, _) = _s.Merge(dryRun: dryRun, pin: pin);

        var writes = _s.GitHub.Writes.ToList();
        Assert.Equal(pin == Tip ? 0 : 1, exit);
        Assert.Equal(pin == Tip && !dryRun, writes.Count > 0);
        Assert.All(writes, write => Assert.True(write switch
        {
            ("POST", Merges, var body) => Field(body, "base") == Target && Field(body, "head") == Sha,
            ("POST", Commits, var body) => body!["parents"]!.AsArray()[0]!.GetValue<string>() == Tip,
            ("POST", "graphql", var body) => RefMove(body) == (TargetBranch.RefName, Tip, MergeSha),
            _ => false
        }, $"{write.Method} {write.Path}"));
    }

    private static (string Name, string Before, string After) RefMove(JsonObject? request)
    {
        var update = request!["variables"]!["input"]!["refUpdates"]!.AsArray().Single()!.AsObject();
        return (Field(update, "name"), Field(update, "beforeOid"), Field(update, "afterOid"));
    }

    #endregion
}
