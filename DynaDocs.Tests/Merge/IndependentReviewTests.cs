namespace DynaDocs.Tests.Merge;

using DynaDocs.Services.Merge;
using static FakeGitHub;
using static MergeScenario;

/// <summary>
/// The PASS parser (design §1 Q2): which verdict lines of a PR body's <c>## Independent review</c>
/// count, what each names, and how an abbreviated candidate is bound to the exact SHA.
/// </summary>
[Collection("Integration")]
public sealed class IndependentReviewTests
{
    private static ReviewVerdict Only(string body) => Assert.Single(IndependentReview.Parse(body));

    [Theory]
    [Trait("Job", "contract: dydo-commands-delivery.md dydo merge Review (the candidate and base field names accept an optional colon: with the row's separators, no colon, one or both, the line is one verdict equal to the canonical line's, its candidate the SHA and its base the review base)")]
    [InlineData(" ", " ")]
    [InlineData(": ", " ")]
    [InlineData(" ", ": ")]
    [InlineData(": ", ": ")]
    public void CanonicalAndColonFields_ParseIdentically(string candidateSeparator, string baseSeparator)
    {
        var line = PassLine().Replace("candidate ", "candidate" + candidateSeparator)
            .Replace("base ", "base" + baseSeparator);
        var verdict = Only(Body(line));

        Assert.Equal(Only(Body(PassLine())) with { Line = line }, verdict);
        Assert.Equal(Sha, verdict.Candidate);
        Assert.Equal(ReviewBase, verdict.Base);
    }

    [Fact]
    [Trait("Job", "contract: dydo-commands-delivery.md dydo merge Review (a PASS with a colon after both the candidate and the base field names merges: exit 0)")]
    public void ColonFields_MergeWithTheSamePinsAndBasePolicy()
    {
        using var s = new MergeScenario();
        s.PullJson["body"] = Body(PassLine().Replace("candidate ", "candidate: ").Replace("base ", "base: "));

        var (exit, _, stderr) = s.Merge();

        Assert.True(exit == 0, stderr);
    }

    [Fact]
    [Trait("Job", "contract: DR 061 §4; dydo-commands-delivery.md dydo merge Review (an unfenced PASS line in the Independent review section is its one verdict: a PASS, rubric code, the candidate, the base and the line itself)")]
    public void AnUnfencedLine_Counts()
    {
        var verdict = Only(Body(PassLine()));

        Assert.True(verdict.Pass);
        Assert.Equal("code", verdict.Rubric);
        Assert.Equal(Sha, verdict.Candidate);
        Assert.Equal(ReviewBase, verdict.Base);
        Assert.Equal(PassLine(), verdict.Line);
    }

    [Fact]
    [Trait("Job", "contract: dydo-commands-delivery.md dydo merge Review (a PASS line counts fenced or not: a fenced one is the one verdict, its candidate the SHA)")]
    public void AFencedLine_CountsAlike()
    {
        var verdict = Only(Body("```text\n" + PassLine() + "\n```"));

        Assert.Equal(Sha, verdict.Candidate);
    }

    [Theory]
    [Trait("Job", "contract: dydo-commands-delivery.md dydo merge Review (a hyphen, en dash or em dash after PASS, the row's, counts: one verdict, rubric code, its candidate the SHA)")]
    [InlineData("PASS - rubric/type: code; candidate {0}.")]
    [InlineData("PASS – rubric/type: code; candidate {0}.")]
    [InlineData("PASS — rubric/type: code; candidate {0}.")]
    [InlineData("PASS—rubric/type: code; candidate {0}.")]
    [InlineData("  PASS -  rubric/type:  Code ; candidate {0}.")]
    public void AnyDashAfterPass_AndLooseSpacing(string line)
    {
        var verdict = Only(Body(string.Format(line, Sha)));

        Assert.Equal("code", verdict.Rubric);
        Assert.Equal(Sha, verdict.Candidate);
    }

    [Fact]
    [Trait("Job", "contract: dydo-commands-delivery.md dydo merge Review (the SHA possibly backticked: a backticked candidate is read as the SHA, in the one verdict)")]
    public void ABacktickedCandidate()
    {
        Assert.Equal(Sha, Only(Body($"PASS - rubric/type: docs; contract DYD-1; candidate `{Sha}`; base `{ReviewBase}`.")).Candidate);
    }

    [Fact]
    [Trait("Job", "contract: dydo-commands-delivery.md dydo merge Review (a PASS may name no base: without contract and base fields it is one verdict, its candidate the SHA and no base)")]
    public void MissingContractAndBaseFields()
    {
        var verdict = Only(Body($"PASS - rubric/type: merge; candidate {Sha}; reviewer/model: r."));

        Assert.Equal(Sha, verdict.Candidate);
        Assert.Null(verdict.Base);
    }

    [Fact]
    [Trait("Job", "contract: dydo-commands-delivery.md dydo merge Review (a `PASS - rubric/type: <rubric>; ...` line: deferred findings trailing it leave one verdict, its candidate the SHA and its base the review base)")]
    public void TrailingDeferredFindings()
    {
        var verdict = Only(Body(PassLine() + " Deferred: 1. Where: a.cs:3; severity: should; wrong: w; why: y; required correction: c; owner: code-writer."));

        Assert.Equal(Sha, verdict.Candidate);
        Assert.Equal(ReviewBase, verdict.Base);
    }

    [Fact]
    [Trait("Job", "contract: DR 061 §4 (a PASS whose candidate is the SHA: the SHA in upper-case hex is read as the SHA, in the one verdict)")]
    public void AnUpperCaseSha_ReadsLowerCase()
    {
        Assert.Equal(Sha, Only(Body($"PASS - rubric/type: code; candidate {Sha.ToUpperInvariant()}.")).Candidate);
    }

    [Theory]
    [Trait("Job", "contract: dydo-commands-delivery.md dydo merge Review (a candidate counts from 7 hex digits up to the full SHA: by row, 6 digits, 41 digits, or a candidate named only in the prose after the fields leave the one verdict with no candidate)")]
    [InlineData("PASS - rubric/type: code; candidate 012345.")]
    [InlineData("PASS - rubric/type: code; candidate 0123456789abcdef0123456789abcdef012345678.")]
    [InlineData("PASS - rubric/type: code; reviewer/model: r. Checks: the candidate 0123456 holds.")]
    public void NoCandidateField_NamesNoCandidate(string line)
    {
        Assert.Null(Only(Body(line)).Candidate);
    }

    [Fact]
    [Trait("Job", "contract: DR 061 §4 (a FAIL for the SHA refuses, so a FAIL line is read as a verdict: one verdict, not a pass, its candidate the SHA)")]
    public void AFailLine_IsAVerdictToo()
    {
        var verdict = Only(Body($"FAIL - rubric/type: code; candidate {Sha}; gates 1/3."));

        Assert.False(verdict.Pass);
        Assert.Equal(Sha, verdict.Candidate);
    }

    [Fact]
    [Trait("Job", "contract: dydo-commands-delivery.md dydo merge Review (only the `## Independent review` section, up to the next level-two heading, counts: of three PASS lines, the one verdict is the section's)")]
    public void OnlyTheIndependentReviewSectionCounts()
    {
        var body = $"""
            Closes DYD-1.

            {PassLine("5555555555555555555555555555555555555555")}

            ## Independent review

            {PassLine()}

            ## Notes

            {PassLine("6666666666666666666666666666666666666666")}
            """;

        Assert.Equal(Sha, Only(body).Candidate);
    }

    [Fact]
    [Trait("Job", "contract: dydo-commands-delivery.md dydo merge Review (the section must hold a `PASS - rubric/type:` line: a PASS mentioned inside a prose line is no verdict)")]
    public void ProseThatMentionsAPass_IsNoVerdict()
    {
        Assert.Empty(IndependentReview.Parse(Body("Round 1 (b4becad1) FAILed on one nit; round 2 PASS - rubric/type: code; candidate " + Sha + ".")));
    }

    // bodnarbalazs/dydo#205: a docs PASS and a merge PASS, abbreviated, the merge one naming two parents.
    [Fact]
    [Trait("Job", "trap: bodnarbalazs/dydo#205 (a real body with a docs PASS and a merge PASS, abbreviated, the merge one naming two parents, reads as exactly two verdicts: docs then merge, each with its abbreviated candidate and its first base)")]
    public void ARealBodyWithTwoAbbreviatedPasses()
    {
        var verdicts = IndependentReview.Parse(Body(
            "PASS - rubric/type: docs; contract DYD-400 description as of 2026-10-28T23:24:48.401Z; candidate d7573cb7; base f4d8627c; reviewer/model: reviewer/gpt-6.1-sol; applicable tests 89/89 (writer-reported), gates 1/2 independently verified (dydo check: 0 errors, 0 warnings; CI unreachable). Checks: The round-2 blocker is closed.",
            "Merge review (severities branch at 7f11ef46, carrying DYD-399 and DYD-402, merged in as 0335a0cf):",
            "PASS - rubric/type: merge; contract DYD-400 description as of 2026-10-28T23:24:48.401Z; candidate 0335a0cf; base d7573cb7, second parent 7f11ef46; reviewer/model: reviewer/gpt-6.1-sol; applicable tests 89/89 (writer-reported), gates 1/2 independently verified. Checks: All five conflict hunks preserve both sides' intent."));

        Assert.Equal([("docs", "d7573cb7", "f4d8627c"), ("merge", "0335a0cf", "d7573cb7")],
            verdicts.Select(verdict => (verdict.Rubric, verdict.Candidate, verdict.Base)));
    }

    #region Binding the candidate to the SHA (through dydo merge)

    [Fact]
    [Trait("Job", "contract: DR 061 §4 (the PASS must be for the SHA: a PASS for another full SHA is none, and dydo merge refuses, exit 1, its `refused:` line naming the SHA)")]
    public void AFullShaMustEqualTheHead()
    {
        using var s = new MergeScenario();
        s.PullJson["body"] = Body(PassLine("0123456789abcdef0123456789abcdef01234568"));

        var (exit, _, stderr) = s.Merge();

        Assert.Equal(1, exit);
        Assert.StartsWith("refused: PR #7's Independent review holds no PASS for candidate 0123456789ab", stderr);
    }

    [Fact]
    [Trait("Job", "contract: DR 061 §4; dydo-commands-delivery.md dydo merge Review (an abbreviated candidate of 7 hex digits that GitHub resolves to exactly the SHA counts: dydo merge exits 0)")]
    public void ASevenHexPrefixGitHubResolvesToTheSha_Counts()
    {
        using var s = new MergeScenario();
        s.PullJson["body"] = Body(PassLine("0123456"));
        s.GitHub.On("GET", $"{Repo}/commits/0123456", Reply(200, $$"""{ "sha": "{{Sha}}" }"""));

        var (exit, _, stderr) = s.Merge();

        Assert.True(exit == 0, stderr);
    }

    [Fact]
    [Trait("Job", "contract: DR 061 §4; dydo-commands-delivery.md dydo merge Review (an abbreviation GitHub does not resolve to the SHA, answering 422, does not count: exit 1, its `refused:` line naming the SHA, nothing written)")]
    public void AnAmbiguousPrefix_DoesNotCount()
    {
        using var s = new MergeScenario();
        s.PullJson["body"] = Body(PassLine("0123456"));
        s.GitHub.On("GET", $"{Repo}/commits/0123456", Reply(422, """{ "message": "No commit found for SHA: 0123456" }"""));

        var (exit, _, stderr) = s.Merge();

        Assert.Equal(1, exit);
        Assert.StartsWith("refused: PR #7's Independent review holds no PASS for candidate 0123456789ab", stderr);
        Assert.Empty(s.GitHub.Writes);
    }

    [Fact]
    [Trait("Job", "contract: DR 061 §4; dydo-commands-delivery.md dydo merge Review (a FAIL line for the same candidate refuses, its abbreviation resolving to the SHA, beside a PASS for it: exit 1, its `refused:` line naming the FAIL)")]
    public void AFailWhosePrefixResolvesToTheSha_Refuses()
    {
        using var s = new MergeScenario();
        s.PullJson["body"] = Body(PassLine(), "FAIL - rubric/type: code; candidate 01234567; gates 2/3.");
        s.GitHub.On("GET", $"{Repo}/commits/01234567", Reply(200, $$"""{ "sha": "{{Sha}}" }"""));

        var (exit, _, stderr) = s.Merge();

        Assert.Equal(1, exit);
        Assert.StartsWith("refused: the review section also holds a FAIL for 0123456789ab", stderr);
    }

    [Theory]
    [Trait("Job", "contract: DR 061 §4; dydo-commands-delivery.md dydo merge Review (the rubric must be code, docs or merge: a PASS with the row's rubric merges, exit 0)")]
    [InlineData("code")]
    [InlineData("docs")]
    [InlineData("merge")]
    public void ACodeDocsOrMergePass_Merges(string rubric)
    {
        using var s = new MergeScenario();
        s.PullJson["body"] = Body(PassLine(rubric: rubric));

        var (exit, _, stderr) = s.Merge();

        Assert.True(exit == 0, stderr);
    }

    [Fact]
    [Trait("Job", "contract: DR 061 §4; dydo-commands-delivery.md dydo merge (a merge PASS after a spec PASS for the SHA merges, exit 0, and the commit's Review trailer is the merge PASS line's first 200 characters)")]
    public void APassWithAnAllowedRubric_WinsOverAnEarlierSpecPass()
    {
        using var s = new MergeScenario();
        s.PullJson["body"] = Body(PassLine(rubric: "spec"), PassLine(rubric: "merge"));

        var (exit, _, stderr) = s.Merge();

        Assert.True(exit == 0, stderr);
        Assert.EndsWith("Review: " + PassLine(rubric: "merge")[..200], s.GitHub.Writes.Single().Body!["commit_message"]!.GetValue<string>());
    }

    #endregion
}
