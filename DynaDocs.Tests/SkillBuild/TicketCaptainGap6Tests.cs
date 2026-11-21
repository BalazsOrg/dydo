namespace DynaDocs.Tests.SkillBuild;

using System.Text.Json;
using System.Text.RegularExpressions;
using DynaDocs.SkillBuild;

/// <summary>
/// DYD-1372 phase 1, fix g6 (gap hunt 5, H1 to H4): a research holder hands the user's call on as a
/// ticket of its own; one order takes a PR from a draft to the merge, whatever the brief names; a
/// should's repair after a clear round is a move of the machine with an end; and a review brief names
/// the ticket whose work is judged. Read from the production model through
/// <see cref="SkillBuilder.Build"/>, over the built machine's arrows, the built files' links and the
/// commands they name, never by a sentence of the skill (DR 067).
/// </summary>
public partial class TicketCaptainGap6Tests
{
    private const string Folder = "skills/roles/officers/ticket-captain/";
    private const string BodyCheck = "`dydo pr-ready --body <file>`";
    private const string TreeCheck = "`dydo pr-ready`";
    private const string Merge = "`dydo merge <pr-number>";

    private static readonly Lazy<BuiltTree> Built = new(() => SkillBuilder.Build(ProductionModel.Model));

    private static readonly Lazy<(string Claim, string From, string To, string? Guard, string? Effect)[]> Moves = new(() =>
    [
        .. Machine.GetProperty("transitions").EnumerateArray().Select(move => (move.GetProperty("claim").GetString()!, move.GetProperty("from").GetString()!,
            move.GetProperty("to").GetString()!, move.GetProperty("guard").GetString(), move.GetProperty("effect").GetString()))
    ]);

    private static JsonElement Machine => JsonDocument.Parse(Built.Value.Files["claims/statechart/captain/transitions.json"]).RootElement;

    [GeneratedRegex(@"^(\d+)\. ", RegexOptions.Multiline)]
    private static partial Regex Numbered();

    [Fact]
    [Trait("Job", "contract: DYD-1372 g6 H1 H23 H16 a research holder has three ways out of its leaf, each a claim of the research resource: the answer, the call handed on to the user's queue with the research ticket finished, and the rest returned unsettled")]
    public void Research_HandsTheUsersCallOn_AndStillReturnsTheRestUnsettled()
    {
        var research = Moves.Value.Where(move => move.Claim.StartsWith("claim:CaptainResearch.", StringComparison.Ordinal) && move.From == "Leaf").ToArray();

        Assert.Single(research, move => move.To == "Returned");
        Assert.Single(research, move => move.To == "Finishing");
    }

    [Fact]
    [Trait("Job", "contract: DYD-1372 g6 H1 the research resource sends the user's call up by the need's own resource and opens its ticket by the sub-ticket rules, under the Mode that marks a Question the user settles")]
    public void Research_LinksTheWayUp()
    {
        var research = Resource("research");

        Assert.Contains("](need-for-the-user.md)", research);
        Assert.Contains("](sub-tickets.md)", research);
        Assert.Contains("HITL", research);
    }

    [Fact]
    [Trait("Job", "contract: DYD-1372 g6 H2 R17 the one place the PR's order is written is the merging resource, and every file that has a holder open a PR links it: the spine's review phase, the briefs and the red gate")]
    public void EveryFileThatOpensAPr_LinksTheOneOrder()
    {
        var review = Built.Value.Files[Folder + "SKILL.md"].Split("\n## ").Single(section => section.StartsWith("4. ", StringComparison.Ordinal));

        Assert.Contains("](resources/merging.md)", review);
        Assert.Contains("](merging.md)", Resource("briefs"));
        Assert.Contains("](merging.md)", Resource("red-gate"));
    }

    [Fact]
    [Trait("Job", "contract: DYD-1372 g6 H2 the PR is opened on the tree check alone, before any step of the merge: the check of a body that needs a PASS is named in one step only, and that step comes before the merge")]
    public void TheBodyCheck_ComesAfterTheReview_AndBeforeTheMerge()
    {
        var merging = Resource("merging");
        var first = Numbered().Match(merging).Index;
        var steps = Steps(merging);
        var body = steps.Single(step => step.Text.Contains(BodyCheck, StringComparison.Ordinal));
        var merge = steps.Single(step => step.Text.Contains(Merge, StringComparison.Ordinal));

        Assert.InRange(merging.IndexOf(TreeCheck, StringComparison.Ordinal), 0, first);
        Assert.DoesNotContain(BodyCheck, merging[..first]);
        Assert.True(body.Number < merge.Number, $"the body check is step {body.Number} and the merge step {merge.Number}");
        Assert.All(Built.Value.Files.Where(file => file.Key.StartsWith(Folder, StringComparison.Ordinal) && !file.Key.EndsWith("/merging.md", StringComparison.Ordinal)),
            file => Assert.DoesNotContain("pr-ready --body", file.Value));
    }

    [Fact]
    [Trait("Job", "trap: DYD-1372 g6 H2 dydo merge refuses a draft (Services/Merge/PullRequestMerge.cs), so the step that ends the draft comes before the step that merges, and the PR is a draft from the moment it is opened")]
    public void TheDraft_IsEndedBeforeTheMerge()
    {
        var merging = Resource("merging");
        var steps = Steps(merging);
        var merge = steps.Single(step => step.Text.Contains(Merge, StringComparison.Ordinal));

        Assert.Contains("draft", merging[..Numbered().Match(merging).Index]);
        Assert.Contains(steps, step => step.Number < merge.Number && step.Text.Contains("draft", StringComparison.Ordinal));
    }

    [Fact]
    [Trait("Job", "contract: DYD-1372 g6 H3 S17 a should's repair after a clear round is a move of the machine: a clear round leaves to a state of its own, which is entered from nowhere else and left only into a review round or on to finishing")]
    public void AClearRound_HasAMoveForAShouldsRepair()
    {
        var known = new[] { "Finishing", "InReview" };
        var repair = Assert.Single(Moves.Value, move => move.From == "Clear" && !known.Contains(move.To));

        Assert.All(Moves.Value.Where(move => move.To == repair.To), move => Assert.Equal("Clear", move.From));
        Assert.Equal(known, Moves.Value.Where(move => move.From == repair.To).Select(move => move.To).Order());
    }

    [Fact]
    [Trait("Job", "contract: DYD-1372 g6b H3 R2 R23 a should mended on a candidate in no PR of its holder's gets no round of its own: the repair has a way on to finishing that no marker guards and that touches neither the round's count nor either marker, stated where the repair stands")]
    public void AMendedPart_GoesOnToFinishing_WithNoRoundOfItsOwn()
    {
        var repair = Moves.Value.Single(move => move.From == "Clear" && move.To is not ("Finishing" or "InReview"));
        var on = Assert.Single(Moves.Value, move => move.From == repair.To && move.To == "Finishing");

        Assert.Null(on.Guard);
        Assert.Null(on.Effect);
        Assert.Equal(LineOf(repair.Claim["claim:".Length..]), LineOf(on.Claim["claim:".Length..]));
    }

    [Fact]
    [Trait("Job", "contract: DYD-1372 g6 H3 R8 the round after a should's repair is a round on a new head, not the one more round the officer may order: it is counted, and neither move of the repair reads or spends the marker of that one more round")]
    public void TheRepairsRound_IsNotTheOneMoreRound()
    {
        var repair = Moves.Value.Single(move => move.From == "Clear" && move.To is not ("Finishing" or "InReview"));
        var round = Moves.Value.Single(move => move.From == repair.To && move.To == "InReview");

        Assert.Contains("round' = round + 1", round.Effect);
        Assert.All(new[] { repair.Guard, repair.Effect, round.Guard, round.Effect }, text => Assert.DoesNotContain("extra", text ?? ""));
        Assert.Equal("~extra", Moves.Value.Single(move => move.Claim == "claim:CaptainReview.OneMore").Guard);
    }

    [Fact]
    [Trait("Job", "contract: DYD-1372 g6 H3 the repair loop has an end: the move into the repair is guarded by a marker that its own effect sets, and no move of the machine clears it")]
    public void TheRepairLoop_Ends()
    {
        var repair = Moves.Value.Single(move => move.From == "Clear" && move.To is not ("Finishing" or "InReview"));
        var marker = Assert.IsType<string>(repair.Guard).TrimStart('~');

        Assert.StartsWith("~", repair.Guard);
        Assert.Contains(Machine.GetProperty("variables").EnumerateArray(), variable => variable.GetProperty("name").GetString() == marker);
        Assert.Equal($"{marker}' = TRUE", repair.Effect);
        Assert.All(Moves.Value, move => Assert.DoesNotContain($"{marker}' = FALSE", move.Effect ?? ""));
    }

    [Fact]
    [Trait("Job", "contract: DYD-1372 g6 H4 S17 a review brief names the ticket whose work is judged, as a claim of the briefs resource stated in the review brief's own list, and the round's record sends its holder to the brief for it")]
    public void AReviewBrief_NamesTheJudgedTicket()
    {
        var judged = LineOf("CaptainBriefs.Judged");

        Assert.Equal($"{Folder}resources/briefs.md", judged.Path);
        Assert.InRange(judged.Line, LineOf("CaptainBriefs.Candidate").Line + 1, LineOf("CaptainBriefs.Lenses").Line - 1);
        Assert.Contains("brief", LineOf("CaptainRound.Shoulds").Text);
    }

    private static MapLine LineOf(string claim)
    {
        var stated = Built.Value.Map.Where(line => line.Path.StartsWith(Folder, StringComparison.Ordinal) && line.Claims.Contains(claim)).MinBy(line => line.Line);
        Assert.True(stated is not null, $"no line of the skill states claim:{claim}");
        return stated!;
    }

    private static string Resource(string name) => Built.Value.Files[$"{Folder}resources/{name}.md"];

    // A resource's numbered steps, in order: each line that opens with its number.
    private static (int Number, string Text)[] Steps(string file) =>
        [.. file.Split('\n').Select(line => (Line: line, Match: Numbered().Match(line))).Where(step => step.Match.Success).Select(step => (int.Parse(step.Match.Groups[1].Value), step.Line))];
}
