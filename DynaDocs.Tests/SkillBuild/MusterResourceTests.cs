namespace DynaDocs.Tests.SkillBuild;

using DynaDocs.SkillBuild;
using DynaDocs.Tests.SkillBuild.Truths;

/// <summary>
/// DYD-1402 over the production model: the chief of staff's muster resource is built from claims that answer
/// for themselves, truth-checked ahead of its skill, and nothing else the build writes moves by a byte from
/// the base, <c>c07d4ac3</c>. Each digest below was taken from that commit's built files, as
/// <see cref="TicketCaptainSwapTests"/> takes its own. Checks are read from the model, by claim, so an edit
/// of the prose leaves them standing (DR 067).
/// </summary>
public class MusterResourceTests
{
    private const string Muster = "skills/roles/officers/chief-of-staff/resources/muster.md";
    private const string OpenList = "dydo/reference/claim-review/chief-of-staff-muster-open.md";

    // The muster resource, the pages that list its claims, and the list that counts its skill's unchecked lines.
    private static readonly string[] Rebuilt =
    [
        Muster, OpenList, "dydo/reference/claims/staff.md", "dydo/reference/claims/staff-muster.md", "dydo/reference/claims/staff-question.md",
        ReviewDocs.NotYetMigrated
    ];

    private static readonly Lazy<BuiltTree> Built = new(() => SkillBuilder.Build(ProductionModel.Model));

    private static SkillSpec Chief => ProductionModel.Model.Skills.Single(skill => Equals(skill.Id, Skill.ChiefOfStaff));

    private static Resource Resource => Chief.Resources.Single(resource => Equals(resource.Id, ChiefOfStaffResource.Muster));

    /// <summary>Every claim with a line in one of the muster resource's slots, with that line.</summary>
    internal static (string Id, Claim Claim, Line Line)[] Claims =>
    [
        .. ModelIndex.ClaimIds(ProductionModel.Model.Kinds).SelectMany(claim => claim.Key.Lines
            .Where(line => Equals(line.Skill, Skill.ChiefOfStaff) && Resource.Slots.Any(slot => Equals(slot.Id, line.Place.Slot)))
            .Select(line => (claim.Value, claim.Key, line)))
    ];

    [Fact]
    [Trait("Job", "contract: DYD-1402 A1 the muster resource names the chief of staff as its reader, ahead of a skill that names none yet")]
    public void Muster_IsTruthCheckedAheadOfItsSkill()
    {
        Assert.Equal([Actor.ChiefOfStaff], Resource.Readers);
        Assert.Empty(Chief.Readers);
    }

    [Fact]
    [Trait("Job", "contract: DYD-1402 A1 every muster claim carries its evidence, layer and reason, every muster line names its reader, and muster.md states each verified claim")]
    public void EveryMusterClaim_AnswersForItself_AndEveryLineNamesItsReader()
    {
        var stated = Built.Value.Map.Where(line => line.Path == Muster).SelectMany(line => line.Claims).Distinct().ToArray();

        Assert.True(Claims.Length >= 10, $"the muster resource holds {Claims.Length} claims");
        Assert.All(Claims, claim =>
        {
            Assert.True(claim.Claim.Truth is { } truth && new[] { truth.Evidence.Text, truth.Layer, truth.Reason }.All(text => !string.IsNullOrWhiteSpace(text)),
                $"claim:{claim.Id} lacks its evidence, layer or reason");
            Assert.Equal(Actor.ChiefOfStaff, claim.Line.Reader);
        });
        Assert.Equal(Claims.Where(claim => claim.Claim.Truth!.Evidence.Kind != EvidenceKind.Open).Select(claim => claim.Id).Order(), stated.Order());
    }

    [Theory]
    [Trait("Job", "contract: DYD-1402 A1 the production build fails on a muster claim without its truth fields, and on a muster line naming a command the CLI lacks")]
    [InlineData(typeof(MusterBare), "claim:MusterBare.Line is stated by chief-of-staff in muster, a truth-checked resource, and carries no Truth: its evidence, layer and reason")]
    [InlineData(typeof(MusterUsage), "chief-of-staff: the line of claim:MusterUsage.Line names `dydo usage`: `usage` is no subcommand of `dydo`")]
    public void BrokenMusterClaim_FailsTheProductionBuild(Type kind, string message) =>
        Assert.Contains(message, Assert.Throws<InvalidDataException>(
            () => SkillBuilder.Build(ProductionModel.Model with { Kinds = [.. ProductionModel.Model.Kinds, kind] })).Message);

    [Fact]
    [Trait("Job", "contract: DYD-1402 A2 a muster claim nobody could verify is withheld from muster.md and listed, with why, in the muster's built review file")]
    public void OpenMusterClaim_IsWithheldFromTheResourceAndListed()
    {
        var open = Claims.Where(claim => claim.Claim.Truth?.Evidence.Kind == EvidenceKind.Open).ToArray();
        var list = Built.Value.Files[OpenList];

        Assert.Equal(open.Select(claim => "## " + claim.Id), list.Split('\n').Where(line => line.StartsWith("## ", StringComparison.Ordinal)));
        Assert.All(open, claim =>
        {
            Assert.DoesNotContain(claim.Line.Text, Built.Value.Files[Muster]);
            Assert.Contains($"Open: {claim.Claim.Truth!.Evidence.Text}\n\nWithheld from {claim.Line.Place.Slot}:\n\n> {claim.Line.Text}\n", list);
        });
    }

    [Fact]
    [Trait("Job", "trap: DYD-1402 A7 the rule moves no call: the request a coordinator answers is the base's, with no Questions part, and the list for him is still a link and a readiness")]
    public void TheRequestAndTheList_AreTheBases()
    {
        var muster = Built.Value.Files[Muster];

        Assert.Contains(StatechartFilesTests.Text("""
            ```text
            His muster request for <date>, <Project>. Return:
            Intent: his goals and purpose for the Project in your own words, by /in-your-own-words, apart from the work.
            Work: your next tickets in fill order, the first runnable one, and the plan's stop line, or none.
            Blockers: each operational obstacle, and the evidence that would show it fixed.
            Permissions: each action the night's work needs, with a safe exercise for it, or the gap.
            ```
            """), muster);
        Assert.Contains("```text\n<Project> — <coordinator thread link> — <ready, or not ready>\n```\n", muster);
        Assert.DoesNotContain("\nQuestions:", muster);
    }

    [Fact]
    [Trait("Job", "contract: DYD-1402 A8 muster.md is rebuilt, and every built file but it and the pages that list or count its claims is the bytes of the base c07d4ac3")]
    public void OnlyTheMusterAndItsClaimPages_MoveFromTheBase()
    {
        var files = Built.Value.Files;
        // DYD-1437: the captain's chart is written by hand. Its four machine files are rendered and not written, and a
        // captain why doc names the hand-written chart's folder where the base named the built module. With those two
        // put back, the rest is still the base's bytes.
        var steady = files.Where(file => !Rebuilt.Contains(file.Key)).Concat(Built.Value.Unwritten)
            .Select(file => KeyValuePair.Create(file.Key, file.Value.Replace(
                "(the model's own machine, not written; the hand-written chart is in `statechart/captain/`)", "(`claims/statechart/captain/Captain.tla`)")))
            .ToArray();

        Assert.All(Rebuilt, path => Assert.Contains(path, files.Keys));
        Assert.NotEqual("851e343231cfb60b2a331d6022157d0a5aa3bb053645f31ae6c3624d88762cb8", TicketCaptainSwapTests.Digest(files.Where(file => file.Key == Muster)));
        Assert.Equal(4, Built.Value.Unwritten.Count);
        Assert.All(Built.Value.Unwritten.Keys, path => Assert.StartsWith("claims/statechart/captain/", path));
        Assert.Equal(264, steady.Length);
        Assert.Equal("578691ade739c7ad7ca01b5fb799024348d5fd127114058598c21c2e86d25dde", TicketCaptainSwapTests.Digest(steady));
    }
}
