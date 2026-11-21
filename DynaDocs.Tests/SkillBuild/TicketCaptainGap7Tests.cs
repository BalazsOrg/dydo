namespace DynaDocs.Tests.SkillBuild;

using System.Text.Json;
using DynaDocs.SkillBuild;

/// <summary>
/// DYD-1372 phase 1, fix g7 (gap hunt 6, J1, J2, J4 and J5): a Question a child sends up unsettled is
/// opened again as the receiving officer's own, and its answer closes each open Question on the way
/// down; who gets a Fix is one rule in one place; a research holder too big to hold has its own first
/// step and its own way back to its work; and a Review of a candidate in a PR is briefed on a pushed
/// head. Read from the production model through <see cref="SkillBuilder.Build"/>, over the claims each
/// built line states, the files' links and the built machine's arrows, never by a sentence of the
/// skill (DR 067).
/// </summary>
public class TicketCaptainGap7Tests
{
    private const string Folder = "skills/roles/officers/ticket-captain/";
    private const string Resources = Folder + "resources/";
    private const string OnAnswer = "kind = \"answer\"";

    private static readonly Lazy<BuiltTree> Built = new(() => SkillBuilder.Build(ProductionModel.Model));

    private static readonly Lazy<(string Claim, string From, string To, string? Guard, string? Effect)[]> Moves = new(() =>
    [
        .. JsonDocument.Parse(Built.Value.Files["claims/statechart/captain/transitions.json"]).RootElement.GetProperty("transitions").EnumerateArray()
            .Select(move => (move.GetProperty("claim").GetString()!, move.GetProperty("from").GetString()!, move.GetProperty("to").GetString()!,
                move.GetProperty("guard").GetString(), move.GetProperty("effect").GetString()))
    ]);

    [Fact]
    [Trait("Job", "contract: DYD-1372 g7 J1 H16 a Question a child sends up unsettled gets a Question of the receiving officer's own: its step stands apart from the one that keeps a HITL ticket as it is, above the step that dispatches the fresh captain, and names the child's Question by its key under a block")]
    public void AChildsUnsettledQuestion_IsOpenedAgainAsTheOfficersOwn()
    {
        var own = LineOf("CaptainPassedUp.Own");
        var kept = LineOf("CaptainNeed.Carried");

        Assert.Equal(Resources + "need-for-the-user.md", own.Path);
        Assert.True(kept.Line < own.Line && own.Line < LineOf("CaptainNeed.Fresh").Line, "the officer's own Question is opened on the step that keeps a ticket as it is, or below the dispatch");
        Assert.All(new[] { "key", "block" }, word => Assert.Contains(word, own.Text));
    }

    [Fact]
    [Trait("Job", "contract: DYD-1372 g7 J1 S6 H16 an answer closes each open Question on its way down: the officer sends it to the thread of the captain that started the Question, between the step that takes the answer and the step that sends up, and that captain reads its close where it reads its unsettled return")]
    public void AnAnswer_ClosesEachOpenQuestionOnItsWayDown()
    {
        var closes = LineOf("CaptainPassedUp.Closes");
        var woken = LineOf("CaptainPassedUp.Woken");

        Assert.Equal(Resources + "need-for-the-user.md", closes.Path);
        Assert.True(LineOf("CaptainNeed.Answered").Line < closes.Line && closes.Line < LineOf("CaptainNeed.Up").Line, "the close stands above the answer's step, or below the step that sends up");
        Assert.All(new[] { "`t3_thread_send`", "Done" }, word => Assert.Contains(word, closes.Text));
        Assert.Equal(closes, LineOf("CaptainPassedUp.Lost"));
        Assert.Equal(LineOf("CaptainResearch.Question"), woken);
        Assert.Contains("Done", woken.Text);
        // The close is a second run of the captain that returned: the machine keeps Returned final.
        Assert.DoesNotContain(Moves.Value, move => move.From == "Returned");
    }

    [Fact]
    [Trait("Job", "contract: DYD-1372 g7 J1 H12 a holder woken by a child's need, or by the return or the answer of a Question it opened, is sent to the need's own steps: a step of the waking resource links them, above the step that goes on from the phase")]
    public void AWakeByANeedOrItsAnswer_GoesByTheNeedsSteps()
    {
        var route = LineOf("CaptainWaking.Needs");

        Assert.Equal(Resources + "waking.md", route.Path);
        Assert.Contains("](need-for-the-user.md)", route.Text);
        Assert.True(route.Line < LineOf("CaptainWaking.GoOn").Line, "the need's route stands below the step that goes on from the phase");
    }

    [Fact]
    [Trait("Job", "contract: DYD-1372 g7 J2 S9 R33 who gets a Fix is one rule in one place: the line of the fix rounds that names the writer's thread also holds the case of a holder that wrote the change itself, with the tool of each, and the line about a holder's own change links it")]
    public void WhoGetsAFix_IsOneRuleInOnePlace()
    {
        var owner = LineOf("CaptainTrouble.FixOwner");
        var own = LineOf("CaptainReview.OwnFinding");

        Assert.Equal(Resources + "fix-rounds.md", owner.Path);
        Assert.Equal(owner, LineOf("CaptainReview.Mine"));
        Assert.All(new[] { "`t3_thread_send`", "`delegate_task`" }, tool => Assert.Contains(tool, owner.Text));
        Assert.Equal(Resources + "findings.md", own.Path);
        Assert.Contains("](fix-rounds.md)", own.Text);
    }

    [Fact]
    [Trait("Job", "contract: DYD-1372 g7 J4 H2 K30 a research holder whose question is too big has its own first step: the research resource links the too-big scenario from a line of its own, and says below it what becomes of the parts' answers")]
    public void AResearchHolderTooBigToHold_HasItsOwnFirstStep()
    {
        var big = LineOf("CaptainResearch.Big");
        var parts = LineOf("CaptainResearch.Parts");

        Assert.Equal(Resources + "research.md", big.Path);
        Assert.Contains("](too-big.md)", big.Text);
        Assert.Equal(big.Path, parts.Path);
        Assert.True(big.Line < parts.Line, "the parts' answers are taken in above the line that has the question divided");
    }

    [Fact]
    [Trait("Job", "contract: DYD-1372 g7 J4 H16 a split holder whose result is an answer leaves its wait by one move of its own, back to its own work, from which each of a research holder's ways out still leaves: no move takes an answer from the wait straight to finishing")]
    public void ASplitResearchHolder_TakesUpItsOwnWorkAgain()
    {
        var back = Assert.Single(Moves.Value, move => move.From == "Waiting" && move.Guard == OnAnswer);
        var research = Moves.Value.Where(move => move.Claim.StartsWith("claim:CaptainResearch.", StringComparison.Ordinal) && move.From == "Leaf").Select(move => move.To).Order();

        Assert.Equal("Leaf", back.To);
        Assert.Null(back.Effect);
        Assert.Equal("claim:CaptainResearch.Parts", back.Claim);
        Assert.Equal(["Finishing", "Returned"], research);
        // Review r4: read by what a guard lets through, never by its text. An answer is merged by nobody.
        Assert.DoesNotContain(Moves.Value, move => move.From == "Waiting" && move.To == "Finishing" && Open(move.Guard, "answer", "nobody"));
        Assert.Equal(["claim:CaptainReviewSeats.Verdict"],
            Moves.Value.Where(move => move.From == "Waiting" && move.To == "Finishing" && Open(move.Guard, "verdict", "nobody")).Select(move => move.Claim));
        Assert.Contains(Moves.Value, move => move.From == "Waiting" && move.To == "Finishing" && Open(move.Guard, "change", "officer"));
    }

    /// <summary>
    /// Whether a guard lets a holder with this result and this merger through. It reads the conjuncts the
    /// captain machine's guards are made of, and fails on one it does not know, so a new shape of guard
    /// cannot pass unread.
    /// </summary>
    private static bool Open(string? guard, string kind, string merger) =>
        guard is null || guard.Split(" /\\ ").All(part => part switch
        {
            _ when part.StartsWith("kind = ", StringComparison.Ordinal) => part == $"kind = \"{kind}\"",
            _ when part.StartsWith("merger = ", StringComparison.Ordinal) => part == $"merger = \"{merger}\"",
            _ when part.StartsWith("merger # ", StringComparison.Ordinal) => part != $"merger # \"{merger}\"",
            _ => throw new InvalidOperationException($"a guard this test cannot read: {part}")
        });

    [Fact]
    [Trait("Job", "trap: DYD-1372 g7 J5 R23 dydo merge refuses a PR whose hosted head is not the reviewed SHA (Services/Merge/PullRequestMerge.cs), so the review brief's PR line has the candidate pushed as that PR's head, with the command that pushes and the check that proves it, and the folder says so once")]
    public void AReviewInAPr_IsBriefedOnAPushedHead()
    {
        var pushed = LineOf("CaptainBriefs.Pushed");

        Assert.Equal(LineOf("CaptainBriefs.Pr"), pushed);
        Assert.All(new[] { "`git push`", "`dydo pr-ready`" }, command => Assert.Contains(command, pushed.Text));
        Assert.Equal(1, Built.Value.Files.Where(file => file.Key.StartsWith(Folder, StringComparison.Ordinal)).Sum(file => file.Value.Split("`git push`").Length - 1));
    }

    private static MapLine LineOf(string claim)
    {
        var stated = Built.Value.Map.Where(line => line.Path.StartsWith(Folder, StringComparison.Ordinal) && line.Claims.Contains(claim)).MinBy(line => line.Line);
        Assert.True(stated is not null, $"no line of the skill states claim:{claim}");
        return stated!;
    }
}
