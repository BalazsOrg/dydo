namespace DynaDocs.Tests.SkillBuild;

using DynaDocs.SkillBuild;
using DynaDocs.Tests.SkillBuild.Truths;

/// <summary>
/// DYD-1372 A8 and C, review round 2 (S1, S2): the truth checks reach past a skill's claim lines. Every
/// arrow of a machine the skill covers is one of its own answered claims, and every built file of its
/// folder is read whole. Through <see cref="SkillBuilder.Build"/> on the truth fixture.
/// </summary>
public class TruthReachTests
{
    private const string Spine = "skills/roles/crew/keeper/SKILL.md";

    [Fact]
    [Trait("Job", "trap: DYD-1372 A8 r2 S1 an arrow with no truth fields in a machine a truth-checked skill covers fails the build")]
    public void ArrowWithNoTruth_FailsTheBuild() =>
        AssertRejected(TruthModel.With(typeof(KeeperBareMove)),
            "claim:KeeperBareMove.Tide is an arrow of KeeperMachine.Watch, which keeper covers, and it carries no Truth");

    [Fact]
    [Trait("Job", "trap: DYD-1372 A8 r2 S1 an arrow no line of the covering skill states fails the build: the arrows are the skill's own claims")]
    public void ArrowNoLineOfTheSkillStates_FailsTheBuild() =>
        AssertRejected(TruthModel.With(typeof(KeeperUnstatedMove)),
            "claim:KeeperUnstatedMove.Tide is an arrow of KeeperMachine.Watch, which keeper covers, and no line of that skill states it");

    [Theory]
    [Trait("Job", "trap: DYD-1372 C r2 S2 a slot's lead-in is read as a claim line is: a command the CLI lacks, or one in plain prose, fails the build, naming the file")]
    [InlineData("Briefs: run `dydo survey board <p>`.", Spine + " names `dydo survey board <p>`: `board` is no subcommand of `dydo survey`; an argument is written `<placeholder>`")]
    [InlineData("Briefs: run dydo usage first.", Spine + " names `dydo usage` outside code; a command is written in code, where the build checks it against the CLI")]
    public void LeadInNamingAMissingCommand_FailsTheBuild(string leadIn, string message)
    {
        var send = new Phase("Send it", Slot.Paragraph(KeeperSlot.Briefs, leadIn), Slot.Bullets(KeeperSlot.Seats, "Seats."),
            Slot.Steps(KeeperSlot.Merge, "If you merge it:"), Slot.Bullets(KeeperSlot.Extra)) { States = [KeeperState.Sent] };

        AssertRejected(TruthModel.Of(TruthModel.Keeper with { Phases = [TruthModel.Phases[0], send] }), message);
    }

    [Fact]
    [Trait("Job", "trap: DYD-1372 C r2 S2 the frontmatter description is read too: a command the CLI lacks there fails the build")]
    public void DescriptionNamingAMissingCommand_FailsTheBuild() =>
        AssertRejected(
            TruthModel.Of(SkillSpec.Crew(KeeperSkill.Keeper, "Keeper", "Keeps one thing, by `dydo usage`.") with
            {
                Readers = TruthModel.Keeper.Readers, Phases = TruthModel.Phases, Scenarios = TruthModel.Keeper.Scenarios
            }),
            Spine + " names `dydo usage`: `usage` is no subcommand of `dydo`");

    private static void AssertRejected(Model model, string message) =>
        Assert.Contains(message, Assert.Throws<InvalidDataException>(() => SkillBuilder.Build(model)).Message);
}
