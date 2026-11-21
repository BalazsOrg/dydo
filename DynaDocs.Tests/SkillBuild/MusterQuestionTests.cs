namespace DynaDocs.Tests.SkillBuild;

using System.Text.RegularExpressions;
using DynaDocs.SkillBuild;

/// <summary>
/// DYD-1402: what the built muster resource requires of every question the chief of staff puts to him, each
/// an acceptance line of the contract (A3 to A7), after his words on the muster of 2026-11-21: "you shown me
/// a question I've never seen and only given me one recommended answer". Through
/// <see cref="SkillBuilder.Build"/> on the production model, read as the built text a chief of staff loads.
/// A rule is found by its claim and held to the words the contract gives it, so other edits of the prose
/// leave the checks standing (DR 067).
/// </summary>
public partial class MusterQuestionTests
{
    private const string Muster = "skills/roles/officers/chief-of-staff/resources/muster.md";
    private const string Grilling = "skills/productivity/grilling/SKILL.md";

    private static readonly Lazy<BuiltTree> Built = new(() => SkillBuilder.Build(ProductionModel.Model));

    [GeneratedRegex("<[^>\n]*>")]
    private static partial Regex Placeholder();

    [Fact]
    [Trait("Job", "contract: DYD-1402 A3 the question block in muster.md carries the grilling skill's markers, line for line, so the two cannot drift apart")]
    public void QuestionBlock_HasTheGrillingSkillsMarkers()
    {
        var grilling = Markers(File.ReadAllText(Path.Combine(CommittedTree.Root, Grilling)));

        Assert.Equal(["❓ **Q1** - **<>**: <>", "", "➡️ <>", "", "---", "", "❓ **Q2** - **<>**: <>", "", "➡️ <>"], grilling);
        Assert.Equal(grilling, Markers(Built.Value.Files[Muster]));
    }

    [Theory]
    [Trait("Job", "trap: DYD-1402 A3 a question block whose marker drifts from the grilling skill's is no match")]
    [InlineData("➡️", "->")]
    [InlineData("❓ **Q1** - ", "**Q1.** ")]
    [InlineData("\n---\n", "\n")]
    public void QuestionBlockWithADriftedMarker_IsNoMatch(string marker, string drifted) =>
        Assert.NotEqual(Markers(File.ReadAllText(Path.Combine(CommittedTree.Root, Grilling))), Markers(Built.Value.Files[Muster].Replace(marker, drifted)));

    [Theory]
    [Trait("Job", "contract: DYD-1402 A3 to A7 muster.md says, on a line of its own claim, what every question put to him at muster holds")]
    [InlineData("A3 A7 one block in the grilling skill's format, whatever the question's source", "StaffQuestion.Shape",
        "every question", "at muster", "whatever its source", "one block", "`grilling` skill's format")]
    [InlineData("A3 the context he has not seen", "StaffQuestion.Context", "everything he needs to answer and has not seen", "from the block alone")]
    [InlineData("A3 every option with its trade-off, never one option", "StaffQuestion.Options", "every option", "each with its trade-off", "at least two")]
    [InlineData("A3 a recommendation of one of the options given, with its reason", "StaffQuestion.Recommend", "Recommend one of those options", "`➡️`", "with your reason")]
    [InlineData("A4 premises verified before the question is shown, the evidence linked in the block, a trade-off a premise too", "StaffQuestion.Premises",
        "Verify each premise the question rests on before you show it", "link the evidence in the block", "a premise is anything the block gives as fact", "what an option can do and its trade-off included")]
    [InlineData("A3 A4 a question whose answer depends on an unverified fact, an option's trade-off included, is held back, and he is told which waits", "StaffQuestion.Held",
        "Hold a question back while a fact its answer depends on is unverified, each option's trade-off included", "send the question back to its source to verify",
        "tell him in one line which question waits and on what")]
    [InlineData("A3 A4 a fact nobody can verify keeps its question held, and he is asked how to act without it only where two ways to act have verified trade-offs", "StaffQuestion.Unknown",
        "no record and no check open to you or to the question's source verifies a fact its answer depends on", "the question stays held",
        "where at least two ways to act have trade-offs verified without that fact", "ask him a question of your own, how to act without it", "say in the block what is unknown",
        "give only those ways as options")]
    [InlineData("A5 plain language, a term he has not used defined or replaced", "StaffQuestion.Plain",
        "plain language", "define a term he has not used where it first appears, or replace it with plain words that mean the same")]
    [InlineData("A6 a route that needs no money and no permission is checked first, and the block says what it showed", "StaffQuestion.Route",
        "Before a question that asks him to spend money or grant a permission", "a route that needs neither", "say in the block what you checked and what it showed")]
    public void MusterMd_SaysWhatAQuestionHolds(string rule, string claim, params string[] words)
    {
        var stated = Built.Value.Map.Where(line => line.Path == Muster && line.Claims.Contains(claim)).ToArray();

        Assert.True(stated.Length == 1, $"{rule}: claim:{claim} is stated on {stated.Length} lines of {Muster}");
        Assert.Equal([claim], stated[0].Claims);
        Assert.Contains("- " + stated[0].Text + "\n", Built.Value.Files[Muster]);
        Assert.All(words, word => Assert.Contains(word, stated[0].Text));
    }

    [Fact]
    [Trait("Job", "contract: DYD-1402 A4 no line of muster.md lets a decision reach him on a premise nobody verified: the premises line has no exception, a question that depends on an unverified fact waits, and a fact nobody can verify is asked about as an unknown")]
    public void NoDecisionReachesHim_OnAPremiseNobodyVerified()
    {
        var (premises, held, unknown) = (Stated("StaffQuestion.Premises"), Stated("StaffQuestion.Held"), Stated("StaffQuestion.Unknown"));

        Assert.DoesNotContain("could not verify", premises);
        Assert.DoesNotContain("say so", premises);
        Assert.StartsWith("Hold a question back while a fact its answer depends on is unverified, each option's trade-off included: verify it, or send the question back to its source to verify,", held);
        Assert.StartsWith("Where no record and no check open to you or to the question's source verifies a fact its answer depends on, the question stays held; " +
                          "where at least two ways to act have trade-offs verified without that fact, ask him a question of your own, how to act without it:", unknown);
        Assert.EndsWith("give only those ways as options.", unknown);
    }

    [Fact]
    [Trait("Job", "contract: DYD-1402 A3 no line of muster.md lets an option reach him without its trade-off, or a recommendation land on an option whose cost nobody verified: a trade-off is named only where it is required, a missing one holds the question, and the question asked in a held one's place offers only options whose trade-offs are verified")]
    public void NoOptionReachesHim_WithoutItsVerifiedTradeOff()
    {
        var naming = Built.Value.Map.Where(line => line.Path == Muster && line.Text.Contains("trade-off", StringComparison.Ordinal)).SelectMany(line => line.Claims).Order().ToArray();

        Assert.Equal(["StaffQuestion.Form", "StaffQuestion.Held", "StaffQuestion.Options", "StaffQuestion.Premises", "StaffQuestion.Unknown"], naming);
        Assert.StartsWith("Give every option, each with its trade-off:", Stated("StaffQuestion.Options"));
        Assert.EndsWith("what an option can do and its trade-off included.", Stated("StaffQuestion.Premises"));
        Assert.StartsWith("Hold a question back while a fact its answer depends on is unverified, each option's trade-off included:", Stated("StaffQuestion.Held"));
        Assert.Contains("where at least two ways to act have trade-offs verified without that fact, ask him a question of your own,", Stated("StaffQuestion.Unknown"));
        Assert.EndsWith("give only those ways as options.", Stated("StaffQuestion.Unknown"));
        Assert.StartsWith("Recommend one of those options", Stated("StaffQuestion.Recommend"));
        Assert.DoesNotContain("say so", Built.Value.Files[Muster]);
    }

    // The one line of muster.md that states the claim, or nothing where no line does.
    private static string Stated(string claim) =>
        string.Join('\n', Built.Value.Map.Where(line => line.Path == Muster && line.Claims.Contains(claim)).Select(line => line.Text));

    // The fenced block that opens with the question marker, each placeholder emptied: what is left is the format's markers.
    private static string[] Markers(string markdown)
    {
        var lines = markdown.Split('\n');
        var open = Array.FindIndex(lines, line => line.StartsWith("❓ ", StringComparison.Ordinal));
        if (open < 1 || !lines[open - 1].StartsWith("```", StringComparison.Ordinal)) return [];
        return [.. lines.Skip(open).TakeWhile(line => !line.StartsWith("```", StringComparison.Ordinal)).Select(line => Placeholder().Replace(line, "<>"))];
    }
}
