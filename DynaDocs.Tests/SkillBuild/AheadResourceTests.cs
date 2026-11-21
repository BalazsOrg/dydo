namespace DynaDocs.Tests.SkillBuild;

using DynaDocs.SkillBuild;
using DynaDocs.Tests.SkillBuild.Truths;

/// <summary>
/// DYD-1402: a resource that names its readers is truth-checked ahead of its skill. Its lines, claims, slots
/// and file are held to what a truth-checked skill's are, its open claims are withheld and listed, and the
/// rest of the skill builds as it did. Through <see cref="SkillBuilder.Build"/> on the ahead fixture, a
/// legacy clerk whose one resource, the rota, is checked.
/// </summary>
public class AheadResourceTests
{
    private const string Spine = "skills/roles/crew/clerk/SKILL.md";
    private const string Rota = "skills/roles/crew/clerk/resources/rota.md";
    private const string OpenList = "dydo/reference/claim-review/clerk-rota-open.md";

    [Fact]
    [Trait("Job", "contract: DYD-1402 A1 a resource truth-checked ahead of its skill renders in its shapes, and its legacy spine builds as it did, a missing command and all")]
    public void AheadResource_RendersShaped_BesideItsLegacySpine()
    {
        var tree = SkillBuilder.Build(AheadModel.With());

        Assert.Equal(StatechartFilesTests.Text("""
            <!-- built from the model; edit there: tools/SkillBuild -->

            # Rota

            Post the rota each week.

            **Each morning**

            - Run `dydo check` before the desk opens.
            """), tree.Files[Rota]);
        Assert.EndsWith("## 1. Desk\n\n1. Read `dydo usage`, then run `dydo check`.\n\nResources:\n\n- [Rota](resources/rota.md)\n", tree.Files[Spine]);
        Assert.Equal(["ClerkRota.Posted", "ClerkRota.Checked"], tree.Map.Where(line => line.Path == Rota).SelectMany(line => line.Claims));
    }

    [Theory]
    [Trait("Job", "contract: DYD-1402 A1 in a resource truth-checked ahead of its skill, a claim with no truth fields, a line with no reader or another's, and a command the CLI lacks each fail the build")]
    [InlineData(typeof(ClerkRotaNoTruth), "claim:ClerkRotaNoTruth.Bare is stated by clerk in rota, a truth-checked resource, and carries no Truth: its evidence, layer and reason")]
    [InlineData(typeof(ClerkRotaNoReader), "clerk: the line of claim:ClerkRotaNoReader.Anyone names no reader; a truth-checked line names the actor who acts on it")]
    [InlineData(typeof(ClerkRotaWrongReader),
        "clerk: the line of claim:ClerkRotaWrongReader.Landing is read by KeeperActor.Lander, and this resource belongs to KeeperActor.Keeper; a line belongs in the skill of the reader who acts on it")]
    [InlineData(typeof(ClerkRotaMissingCommand), "clerk: the line of claim:ClerkRotaMissingCommand.Usage names `dydo usage`: `usage` is no subcommand of `dydo`")]
    public void BrokenLineInAnAheadResource_FailsTheBuild(Type kind, string message) => AssertRejected(AheadModel.With(kind), message);

    [Fact]
    [Trait("Job", "contract: DYD-1402 A2 an open claim of a resource truth-checked ahead of its skill is withheld from the resource, unmapped, and listed in a review file of its own")]
    public void OpenClaimInAnAheadResource_IsWithheldAndListed()
    {
        var tree = SkillBuilder.Build(AheadModel.With(typeof(ClerkRotaOpen)));

        Assert.Equal(SkillBuilder.Build(AheadModel.With()).Files[Rota], tree.Files[Rota]);
        Assert.DoesNotContain(tree.Map, line => line.Claims.Contains("ClerkRotaOpen.Unseen"));
        Assert.Equal(StatechartFilesTests.Text("""
            ---
            area: reference
            type: reference
            ---
            <!-- built from the model; edit there: tools/SkillBuild -->

            # Open claims: clerk/rota

            Claims of `clerk/rota` that nobody has verified yet. The resource does not state them: each line below is withheld until its claim has evidence.

            ## ClerkRotaOpen.Unseen

            `claim:ClerkRotaOpen.Unseen`: The clerk rings the bell at noon.

            Open: Nobody has watched a noon yet.

            Withheld from RotaRules:

            > Ring the bell at noon.
            """), tree.Files[OpenList]);
    }

    [Fact]
    [Trait("Job", "edge: DYD-1402 A2 a resource truth-checked ahead of its skill with no open claim says so in its list")]
    public void AheadResourceWithNoOpenClaim_ListsNone() =>
        Assert.EndsWith("is withheld until its claim has evidence.\n\nNone.\n", SkillBuilder.Build(AheadModel.With()).Files[OpenList]);

    [Fact]
    [Trait("Job", "trap: DYD-1402 A2 only the checked resource withholds: an open claim in a legacy slot of the same skill still fails the build")]
    public void OpenClaimInALegacySlotBesideAnAheadResource_FailsTheBuild() =>
        AssertRejected(AheadModel.With(typeof(ClerkOpen)),
            "claim:ClerkOpen.Unseen is open, and its line in clerk would state it; only a truth-checked skill withholds an open claim's line, or a resource truth-checked ahead of its skill");

    [Fact]
    [Trait("Job", "trap: DYD-1402 A1 a resource truth-checked ahead of its skill declares each slot by its shape")]
    public void AheadResourceWithALegacySlot_FailsTheBuild() =>
        AssertRejected(
            AheadModel.Of(new Resource(AheadSlot.Rota, "Rota", new Slot(AheadSlot.RotaLead), new Slot(AheadSlot.RotaRules, "Each morning")) { Readers = [KeeperActor.Keeper] }),
            "clerk: AheadSlot.RotaLead declares no shape; a truth-checked resource's slot is a paragraph, bullets or steps");

    [Fact]
    [Trait("Job", "trap: DYD-1402 A1 the checked resource's file is read whole: a lead-in naming a command the CLI lacks fails the build, naming the file")]
    public void AheadResourceLeadInNamingAMissingCommand_FailsTheBuild() =>
        AssertRejected(
            AheadModel.Of(new Resource(AheadSlot.Rota, "Rota", Slot.Paragraph(AheadSlot.RotaLead), Slot.Bullets(AheadSlot.RotaRules, "After `dydo usage`")) { Readers = [KeeperActor.Keeper] }),
            Rota + " names `dydo usage`: `usage` is no subcommand of `dydo`");

    [Fact]
    [Trait("Job", "contract: DYD-1402 A1 the not-yet-migrated list counts a skill's unchecked lines only, and names the resource checked ahead of it")]
    public void SkillWithAnAheadResource_IsListedNotYetMigrated_LessThatResource() =>
        Assert.Equal(StatechartFilesTests.Text("""
            ---
            area: reference
            type: reference
            ---
            <!-- built from the model; edit there: tools/SkillBuild -->

            # Skills and standards not yet migrated

            These are not truth-checked: no line of theirs names its reader, and no claim carries its evidence, layer and reason on their account. Each builds as it did, unchanged, until it is switched over.

            `keeper`: not yet migrated; 5 lines, 1 naming a command the CLI does not have.

            `clerk`: not yet migrated; 1 line, 1 naming a command the CLI does not have. Its resource `rota` is truth-checked ahead of it: those lines name their reader, and are not counted here.
            """), SkillBuilder.Build(AheadModel.With()).Files[ReviewDocs.NotYetMigrated]);

    private static void AssertRejected(Model model, string message) =>
        Assert.Contains(message, Assert.Throws<InvalidDataException>(() => SkillBuilder.Build(model)).Message);
}
