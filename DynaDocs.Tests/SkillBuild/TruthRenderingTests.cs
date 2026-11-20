namespace DynaDocs.Tests.SkillBuild;

using System.Text.RegularExpressions;
using DynaDocs.SkillBuild;
using DynaDocs.Tests.SkillBuild.Truths;

/// <summary>
/// DYD-1372 A5 and A8 (DYD-1295 519126ab): a truth-checked skill renders each slot in its declared
/// shape, a title as a bold lead-in, steps with real numbers and a table verbatim, and carries a
/// transition claim's line in any slot of a phase that covers the move. Through
/// <see cref="SkillBuilder.Build"/> on the truth fixture.
/// </summary>
public partial class TruthRenderingTests
{
    private const string Folder = "skills/roles/crew/keeper/";

    private static readonly BuiltTree Built = SkillBuilder.Build(TruthModel.With());

    [GeneratedRegex(@"(?m)^1\. .*\n(?:\n)?1\. ")]
    private static partial Regex LazyNumbering();

    [Fact]
    [Trait("Job", "contract: DYD-1372 A5 a truth-checked spine renders numbered phases, each slot in its shape, a title as a bold lead-in, steps 1 2 3 and a table verbatim")]
    public void Spine_RendersEachSlotInItsDeclaredShape() =>
        Assert.Equal(
            """
            ---
            name: keeper
            description: Keeps one thing.
            ---
            <!-- built from the model; edit there: tools/SkillBuild -->

            # Keeper

            You keep one thing to its end.

            ## 1. Take it

            Read the contract first.
            Then load your kind's resource.

            | Kind | Resource |
            | -- | -- |
            | Change | [Guide](resources/guide.md) |

            ## 2. Send it

            **Briefs.**
            Brief from the template.
            Name the lenses in each brief.

            **Seats.**

            - Seat a reviewer from another family when you can.
            - Scale the seats to the work `dydo survey <project>` shows.

            **If you merge it:**

            1. Run the project's gate, then `dydo merge <pr> --dry-run`.
            2. Close the ticket.
            3. Return one line.

            ## When things go wrong

            - On a rate limit, wait, then retry.
            - [Stuck](resources/stuck.md)

            """.ReplaceLineEndings("\n"),
            Built.Files[Folder + "SKILL.md"]);

    [Fact]
    [Trait("Job", "contract: DYD-1372 hop 2 a truth-checked phase has no Resources list: a phase resource no line of the spine links fails the build")]
    public void PhaseResourceNoLineLinks_FailsTheBuild() =>
        AssertRejected(
            TruthModel.Of(TruthModel.Keeper with { Phases = [TruthModel.Phases[0], TruthModel.Phases[1] with { Resources = [new(OldSlot.Manual, "Manual", Slot.Paragraph(OldSlot.ManualLead))] }] }),
            "keeper: no line of the spine links resources/manual.md; a truth-checked skill links a phase's resource from the line that uses it");

    [Fact]
    [Trait("Job", "contract: DYD-1372 A5 a truth-checked resource reads like a phase: its slots in their shapes, no section heading")]
    public void Resource_RendersItsSlotsInTheirShapes() =>
        Assert.Equal(
            """
            <!-- built from the model; edit there: tools/SkillBuild -->

            # Guide

            A change is implemented, fixed or merged here.

            **On a fix:**

            - Fix only the blockers.

            """.ReplaceLineEndings("\n"),
            Built.Files[Folder + "resources/guide.md"]);

    [Fact]
    [Trait("Job", "contract: DYD-1372 A5 a scenario's steps are numbered 1 2 in the source")]
    public void Scenario_NumbersItsStepsForReal() =>
        Assert.Equal(BuiltHeader.Markdown + "\n\n# Stuck\n\n1. Save the work.\n2. Ask your officer.\n", Built.Files[Folder + "resources/stuck.md"]);

    [Fact]
    [Trait("Job", "trap: DYD-1295 519126ab no truth-checked file numbers a list 1. 1., while the legacy checklist still does")]
    public void NoTruthCheckedFile_NumbersAListOneOne()
    {
        Assert.All(Built.Files.Where(file => file.Key.StartsWith(Folder, StringComparison.Ordinal)), file => Assert.DoesNotMatch(LazyNumbering(), file.Value));
        Assert.Matches(LazyNumbering(), SkillBuilder.Build(TruthModel.Replacing(TruthModel.Entries)).Unbuilt[Folder + "SKILL.md"]);
    }

    [Fact]
    [Trait("Job", "trap: DYD-1295 519126ab a verbatim line that numbers its items 1. 1. fails the build, naming the built line")]
    public void VerbatimLineNumberedOneOne_FailsTheBuild() =>
        AssertRejected(TruthModel.With(typeof(KeeperLazyNumbers)), Folder + "SKILL.md:21 numbers a list item 1 after 1; a list is numbered 1, 2, 3");

    [Fact]
    [Trait("Job", "trap: DYD-1372 A5 a step of two lines would break the numbering, so it fails the build")]
    public void StepOfTwoLines_FailsTheBuild() =>
        AssertRejected(TruthModel.With(typeof(KeeperStepBlock)), "a step is one line, and KeeperSlot.Merge is numbered steps");

    [Fact]
    [Trait("Job", "contract: K23 every claim line of a truth-checked skill maps, at its built line, to its claims, a combined line to each")]
    public void Map_PointsAtEachShapedLine()
    {
        var spine = Built.Files[Folder + "SKILL.md"].Split('\n');

        Assert.All(Built.Map.Where(line => line.Path == Folder + "SKILL.md"), line => Assert.EndsWith(line.Text, spine[line.Line - 1]));
        Assert.Equal(["KeeperMoves.Gate", "KeeperMoves.Merge"], Built.Map.Single(line => line.Text.StartsWith("Run the project's gate", StringComparison.Ordinal)).Claims);
        Assert.Equal(3, Built.Map.Count(line => line.Text.StartsWith('|')));
    }

    [Fact]
    [Trait("Job", "contract: DYD-1372 A5 a truth-checked skill's slot declares its shape")]
    public void TruthCheckedSlotWithNoShape_FailsTheBuild() =>
        AssertRejected(
            TruthModel.Of(TruthModel.Keeper with { Phases = [TruthModel.Phases[0] with { Slots = [new(KeeperSlot.Take), Slot.Paragraph(KeeperSlot.Kinds)] }, TruthModel.Phases[1]] }),
            "keeper: KeeperSlot.Take declares no shape; a truth-checked skill's slot is a paragraph, bullets or steps");

    [Fact]
    [Trait("Job", "trap: DYD-1372 A5 the legacy renderer knows no shape, so a shaped slot in a legacy skill fails the build")]
    public void LegacySlotWithAShape_FailsTheBuild() =>
        AssertRejected(new([typeof(OldMoves)], [TruthModel.OldKeeper, TruthModel.Clerk with { Phases = [new("Desk", Slot.Bullets(OldSlot.Desk))] }]),
            "clerk: OldSlot.Desk declares a shape; only a truth-checked skill's slots have one");

    [Fact]
    [Trait("Job", "contract: DYD-1372 A8 a transition claim's line sits in a plain slot of a phase or scenario that covers the move, and builds its machine's arrow")]
    public void TransitionLine_SitsInAnySlotOfAPhaseThatCoversIt()
    {
        Assert.Null(KeeperMoves.Send.Lines[0].Place.Step);

        var arrows = Built.Files["claims/statechart/watch/transitions.json"];

        Assert.Contains("\"claim\": \"claim:KeeperMoves.Send\",\n      \"from\": \"Held\",\n      \"to\": \"Sent\"", arrows);
        Assert.Contains("\"claim\": \"claim:KeeperMoves.Close\",\n      \"from\": \"Sent\",\n      \"to\": \"Done\"", arrows);
    }

    [Fact]
    [Trait("Job", "trap: DYD-1372 A8 a transition claim's line in a phase that covers neither of its states fails the build")]
    public void TransitionLineInAPhaseThatCoversNeitherState_FailsTheBuild() =>
        AssertRejected(TruthModel.With(typeof(KeeperMisplacedMove)),
            "claim:KeeperMisplacedMove.Early moves KeeperState.Sent to KeeperState.Done; its line in keeper must sit in a phase or scenario that covers one of them");

    [Fact]
    [Trait("Job", "trap: DYD-1372 A8 a phase that covers a state no machine declares fails the build")]
    public void PhaseCoveringAStateNoMachineDeclares_FailsTheBuild() =>
        AssertRejected(TruthModel.Of(TruthModel.Keeper with { Phases = [TruthModel.Phases[0] with { States = [OldSlot.Desk] }, TruthModel.Phases[1]] }),
            "keeper: phase \"Take it\" covers OldSlot.Desk, which is no machine's state");

    private static void AssertRejected(Model model, string message) =>
        Assert.Contains(message, Assert.Throws<InvalidDataException>(() => SkillBuilder.Build(model)).Message);
}
