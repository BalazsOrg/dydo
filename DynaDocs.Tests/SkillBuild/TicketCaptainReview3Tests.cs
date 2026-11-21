namespace DynaDocs.Tests.SkillBuild;

using System.Text.Json;
using System.Text.RegularExpressions;
using DynaDocs.SkillBuild;

/// <summary>
/// DYD-1372 phase 1, fix r3 (review r3: B1's machine half, S1, N1, N4): the captain's machine knows who
/// merges the work, learned where the ticket is taken, and each arrow only a merging holder, or only a
/// holder that does not merge, takes is guarded on it; a should on a change nobody merges waits; and
/// the claim that marks the user's call says what its two marks stand in for. Read from the production
/// model through <see cref="SkillBuilder.Build"/>, over the built machine's variables, guards and
/// effects and the claims each built line states, never by a sentence of the skill (DR 067). A guard is
/// decided here on the result and on who merges alone; every other conjunct is left open.
/// </summary>
public partial class TicketCaptainReview3Tests
{
    private const string Folder = "skills/roles/officers/ticket-captain/";
    private const string Merger = "merger";
    private const string Holder = "holder";
    private const string Officer = "officer";
    private const string Nobody = "nobody";

    // Every holder the taking move can make: a change merged by its holder, its officer or nobody; a verdict or an answer by nobody.
    private static readonly (string Kind, string Merger)[] Holders = [("change", Holder), ("change", Officer), ("change", Nobody), ("verdict", Nobody), ("answer", Nobody)];

    private static readonly Lazy<BuiltTree> Built = new(() => SkillBuilder.Build(ProductionModel.Model));

    private static readonly Lazy<(string Claim, string From, string To, string? Guard, string? Effect)[]> Moves = new(() =>
    [
        .. Machine.GetProperty("transitions").EnumerateArray().Select(move => (move.GetProperty("claim").GetString()!, move.GetProperty("from").GetString()!,
            move.GetProperty("to").GetString()!, move.GetProperty("guard").GetString(), move.GetProperty("effect").GetString()))
    ]);

    private static JsonElement Machine => JsonDocument.Parse(Built.Value.Unwritten["claims/statechart/captain/transitions.json"]).RootElement;

    [GeneratedRegex("\"([a-z]+)\"")]
    private static partial Regex Quoted();

    [GeneratedRegex(@"merger' \\in IF kind' = ""change"" THEN \{([^}]*)\} ELSE \{([^}]*)\}")]
    private static partial Regex MergerOf();

    [Fact]
    [Trait("Job", "contract: DYD-1372 r3 B1 K14 R21 who merges is learned by the one move that takes the ticket, as its result is: a variable unread before, set by no other move, and read by no guard of the state the move leaves")]
    public void WhoMerges_IsLearnedWhereTheTicketIsTaken()
    {
        var variable = Machine.GetProperty("variables").EnumerateArray().SingleOrDefault(entry => entry.GetProperty("name").GetString() == Merger);
        var learns = Moves.Value.Where(move => move.Effect?.Contains(Merger + "'", StringComparison.Ordinal) == true).ToArray();

        Assert.True(variable.ValueKind == JsonValueKind.Object, "the machine holds no variable for who merges");
        Assert.Equal(["claim:Captain.Start"], learns.Select(move => move.Claim));
        Assert.Contains("kind'", learns[0].Effect);
        Assert.DoesNotContain(variable.GetProperty("initial").GetString()!.Trim('"'), new[] { Holder, Officer, Nobody });
        Assert.DoesNotContain(Moves.Value, move => move.From == learns[0].From && move.Guard?.Contains(Merger, StringComparison.Ordinal) == true);
    }

    [Fact]
    [Trait("Job", "contract: DYD-1372 r3 B1 phase 5 has three cases for a change and one for a verdict or an answer: the taking move lets a change be merged by its holder, its officer or nobody, and anything else by nobody alone")]
    public void AVerdictOrAnAnswer_IsMergedByNobody()
    {
        var taught = MergerOf().Match(Moves.Value.Single(move => move.Claim == "claim:Captain.Start").Effect!);

        Assert.True(taught.Success, "the taking move does not tie who merges to the result");
        Assert.Equal([Holder, Nobody, Officer], Quoted().Matches(taught.Groups[1].Value).Select(match => match.Groups[1].Value).Order());
        Assert.Equal([Nobody], Quoted().Matches(taught.Groups[2].Value).Select(match => match.Groups[1].Value));
    }

    [Fact]
    [Trait("Job", "trap: DYD-1372 r3 B1 R23 dydo merge takes no head without a PASS in its PR (Services/Merge/PullRequestMerge.cs), so a holder that merges never leaves a mend for finishing: its mended head gets a round, and it reaches finishing from a clear round alone")]
    public void AMergingHolder_FinishesOnlyAfterAClearRound()
    {
        var into = Open("change", Holder).Where(move => move.To == "Finishing").Select(move => move.From).Distinct().ToArray();

        Assert.Equal(["Clear"], into);
        Assert.Contains(Open("change", Holder), move => move is { From: "Mending", To: "InReview" });
        Assert.DoesNotContain(Open("change", Holder), move => move is { From: "Mending", To: "Finishing" });
    }

    [Fact]
    [Trait("Job", "contract: DYD-1372 r3 B1 only the holder its brief gives the merge takes the merging steps: a round of its own on a leaf's change, a repair before the merge and the round on a mended head are closed to every holder that does not merge")]
    public void OnlyAMergingHolder_TakesTheMergingSteps()
    {
        (string From, string To)[] merging = [("Leaf", "InReview"), ("Finishing", "Repairing"), ("Mending", "InReview")];

        Assert.All(merging, step => Assert.Contains(Open("change", Holder), move => move.From == step.From && move.To == step.To));
        Assert.All(Holders.Where(holder => holder.Merger != Holder), holder =>
            Assert.All(merging, step => Assert.DoesNotContain(Open(holder.Kind, holder.Merger), move => move.From == step.From && move.To == step.To)));
    }

    [Fact]
    [Trait("Job", "contract: DYD-1372 r3 B1 N1 R2 S17 a mended head is handed back without a round only where the officer merges it, and a should on a change nobody merges is never mended after a clear round: it waits, stated on the line that routes a cheap should")]
    public void AMend_IsHandedBackOnlyWhereTheOfficerMerges()
    {
        Assert.Contains(Open("change", Officer), move => move is { From: "Mending", To: "Finishing" });
        Assert.DoesNotContain(Open("change", Nobody), move => move is { From: "Mending", To: "Finishing" });
        Assert.DoesNotContain(Open("change", Nobody), move => move.To == "Mending");
        Assert.All(new[] { Holder, Officer }, merger => Assert.Contains(Open("change", merger), move => move is { From: "Clear", To: "Mending" }));
        Assert.Equal(LineOf("CaptainRound.Cheap"), LineOf("CaptainRound.Unmerged"));
    }

    [Fact]
    [Trait("Job", "trap: DYD-1372 r3 B1 a guard on who merges strands no holder: for each holder the taking move can make, every state it reaches short of the final one has a move open to it, and it reaches finishing and the return")]
    public void NoHolder_IsStrandedByAGuard()
    {
        Assert.All(Holders, holder =>
        {
            var open = Open(holder.Kind, holder.Merger);
            var reached = new HashSet<string> { Machine.GetProperty("initial").GetString()! };
            var grew = true;
            while (grew) grew = open.Where(move => reached.Contains(move.From)).Count(move => reached.Add(move.To)) > 0;

            Assert.All(new[] { "Finishing", "Done", "Returned" }, state => Assert.Contains(state, reached));
            Assert.All(reached.Where(state => state != "Returned"), state => Assert.Contains(open, move => move.From == state));
        });
    }

    [Fact]
    [Trait("Job", "contract: DYD-1372 r3 S1 N4 L18 the claim that marks the user's call says its two marks stand in for L18's own test, sends no other call to a council, and rests on a file of the repository, never on a run")]
    public void TheUsersCall_IsMarkedByWhatAResearcherCanCheck()
    {
        var truth = CaptainResearch.Users.Truth!;

        Assert.Contains("worth his time", CaptainResearch.Users.Rule);
        Assert.DoesNotContain("council", truth.Layer);
        Assert.Equal(EvidenceKind.Code, truth.Evidence.Kind);
    }

    // The moves open to one holder: a conjunct on its result or on who merges decides, and any other conjunct is left open.
    private static (string Claim, string From, string To, string? Guard, string? Effect)[] Open(string kind, string merger) =>
        [.. Moves.Value.Where(move => move.Guard is null || move.Guard.Split(" /\\ ").All(conjunct => Holds(conjunct, "kind", kind) && Holds(conjunct, Merger, merger)))];

    private static bool Holds(string conjunct, string variable, string value)
    {
        foreach (var (sign, equal) in new[] { (" = \"", true), (" # \"", false) })
            if (conjunct.StartsWith(variable + sign, StringComparison.Ordinal) && conjunct.EndsWith('"'))
                return equal == (conjunct[(variable.Length + sign.Length)..^1] == value);
        return true;
    }

    private static MapLine LineOf(string claim)
    {
        var stated = Built.Value.Map.Where(line => line.Path.StartsWith(Folder, StringComparison.Ordinal) && line.Claims.Contains(claim)).MinBy(line => line.Line);
        Assert.True(stated is not null, $"no line of the skill states claim:{claim}");
        return stated!;
    }
}
