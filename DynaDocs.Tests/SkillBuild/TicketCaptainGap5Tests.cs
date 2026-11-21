namespace DynaDocs.Tests.SkillBuild;

using System.Text.Json;
using System.Text.RegularExpressions;
using DynaDocs.SkillBuild;

/// <summary>
/// DYD-1372 phase 1, fix g5 (the phase 1 review's B1): the captain's machine tells an answer from a
/// verdict. Its sources forbid a review of a verdict only (spec R34; 777d396e, "Reviewing the reviews
/// is probably not a good idea"), so the machine holds the two apart: what it shows for an answer is
/// what the skill as built states, and no more. Read from the production model through
/// <see cref="SkillBuilder.Build"/>, over the built machine's arrows and the built why docs, never by
/// a sentence of the skill (DR 067).
/// </summary>
public partial class TicketCaptainGap5Tests
{
    private const string Seats = "claim:CaptainReviewSeats.";

    private static readonly string[] ReviewLoop = ["InReview", "Clear", "Fixing", "Checkpoint", "Repairing"];

    private static readonly Lazy<BuiltTree> Built = new(() => SkillBuilder.Build(ProductionModel.Model));

    private static readonly Lazy<(string Claim, string From, string To, string? Guard, string? Effect)[]> Moves = new(() =>
    [
        .. Machine.GetProperty("transitions").EnumerateArray().Select(move => (move.GetProperty("claim").GetString()!, move.GetProperty("from").GetString()!,
            move.GetProperty("to").GetString()!, move.GetProperty("guard").GetString(), move.GetProperty("effect").GetString()))
    ]);

    private static JsonElement Machine => JsonDocument.Parse(Built.Value.Files["claims/statechart/captain/transitions.json"]).RootElement;

    [GeneratedRegex("\"([a-z]+)\"")]
    private static partial Regex Quoted();

    // The set the taking move draws the result from; who merges is drawn beside it, from a set of its own (review r3, B1).
    [GeneratedRegex(@"kind' \\in \{[^}]*\}")]
    private static partial Regex KindSet();

    [Fact]
    [Trait("Job", "contract: DYD-1372 g5 B1 the move that takes a ticket teaches its holder one of three results, a change, a verdict or an answer, and every guard on the result names one of them")]
    public void Kind_IsOneOfThreeResults()
    {
        var guarded = Moves.Value.Where(move => OnKind(move.Guard)).Select(move => Quoted().Match(move.Guard!).Groups[1].Value).Distinct().ToArray();

        Assert.Equal(["answer", "change", "verdict"], Learned());
        Assert.All(guarded, kind => Assert.Contains(kind, Learned()));
    }

    [Fact]
    [Trait("Job", "contract: DYD-1372 g5 B1 R34 a verdict is never reviewed: no path of the machine leads a holder whose result is a verdict into a review round or a repair, and a change still reaches both (777d396e)")]
    public void Verdict_ReachesNoReviewRound()
    {
        var verdict = Reached("verdict");
        var change = Reached("change");

        Assert.All(ReviewLoop, state => Assert.DoesNotContain(state, verdict));
        Assert.All(ReviewLoop, state => Assert.Contains(state, change));
        Assert.All(new[] { verdict, change }, reached => Assert.Contains("Returned", reached));
    }

    [Fact]
    [Trait("Job", "contract: DYD-1372 g5 B1 an answer reaches no review round, as the skill is built: no arrow opens a round or a repair on an answer, and its holder still finishes and returns")]
    public void Answer_ReachesNoReviewRound()
    {
        var answer = Reached("answer");

        Assert.Contains("answer", Learned());
        Assert.All(ReviewLoop, state => Assert.DoesNotContain(state, answer));
        Assert.All(new[] { "Leaf", "Finishing", "Done", "Returned" }, state => Assert.Contains(state, answer));
    }

    [Fact]
    [Trait("Job", "contract: DYD-1372 g5 B1 an answer is not a verdict: seating a round and forming its verdict are open to a verdict's holder alone, never to an answer's or a change's")]
    public void Answer_IsNoVerdict()
    {
        var seats = Moves.Value.Where(move => move.Claim.StartsWith(Seats, StringComparison.Ordinal) && OnKind(move.Guard)).ToArray();

        Assert.Equal([Seats + "Ticket", Seats + "Verdict"], seats.Select(move => move.Claim).Order());
        Assert.All(seats, move => Assert.Contains(move, Open("verdict")));
        Assert.All(new[] { "answer", "change" }, kind =>
        {
            Assert.Contains(kind, Learned());
            Assert.All(seats, move => Assert.DoesNotContain(move, Open(kind)));
        });
    }

    [Fact]
    [Trait("Job", "trap: DYD-1372 g5 B1 a machine built from the claims is evidence for none of them: no claim's evidence in the built why docs names a file under claims/statechart")]
    public void NoClaim_CitesABuiltMachineAsItsEvidence()
    {
        var cited = Built.Value.Files.Where(file => file.Key.StartsWith("dydo/reference/claims/", StringComparison.Ordinal))
            .SelectMany(file => file.Value.Split('\n').Where(line => line.StartsWith("Evidence", StringComparison.Ordinal)).Select(line => (file.Key, Line: line)))
            .Where(evidence => evidence.Line.Contains("claims/statechart/", StringComparison.Ordinal)).Select(evidence => evidence.Key).ToArray();

        Assert.True(cited.Length == 0, "cites a built machine as evidence: " + string.Join(", ", cited));
    }

    private static bool OnKind(string? guard) => guard?.StartsWith("kind = ", StringComparison.Ordinal) == true;

    // The results the taking move can teach, read from its effect, in order.
    private static string[] Learned() =>
        [.. Quoted().Matches(KindSet().Match(Moves.Value.Single(move => move.Effect?.Contains("kind'", StringComparison.Ordinal) == true).Effect!).Value).Select(match => match.Groups[1].Value).Order()];

    // The moves open to the holder of one kind of result: a move guarded on the result is open only when its guard names that kind, whatever else it asks.
    private static (string Claim, string From, string To, string? Guard, string? Effect)[] Open(string kind) =>
        [.. Moves.Value.Where(move => !OnKind(move.Guard) || move.Guard!.Split(" /\\ ")[0] == $"kind = \"{kind}\"")];

    // Every state that holder can reach; the guards on anything but the result are left open, so nothing reachable is missed.
    private static HashSet<string> Reached(string kind)
    {
        var reached = new HashSet<string> { Machine.GetProperty("initial").GetString()! };
        var open = Open(kind);
        var grew = true;
        while (grew) grew = open.Where(move => reached.Contains(move.From)).Count(move => reached.Add(move.To)) > 0;
        return reached;
    }
}
