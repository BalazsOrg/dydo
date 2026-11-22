namespace DynaDocs.Tests.SkillBuild;

using DynaDocs.SkillBuild;
using DynaDocs.Tests.SkillBuild.Charts;

/// <summary>
/// The statechart model's rules, each red through <see cref="SkillBuilder.Build"/> (DYD-1082 contract,
/// "Statechart model"; K1, K9): a transition's lines are its moves, a transition and a state stay in
/// one machine, a skill's states are machine states, and a citing model cites the spec everywhere.
/// </summary>
public class StatechartRulesTests
{
    [Fact]
    [Trait("Job", "trap: K1 a transition's line that is neither its exit nor its arrival fails the build")]
    public void TransitionLineThatIsNoMove_FailsTheBuild() =>
        AssertRejected(ChartModel.With(typeof(LeaseMisplaced)),
            $"claim:{nameof(LeaseMisplaced)}.{nameof(LeaseMisplaced.Wander)} moves LeaseState.Held to LeaseState.Review; " +
            "its line in holder must be an exit in LeaseState.Held or an arrival in LeaseState.Review");

    [Fact]
    [Trait("Job", "contract: K12 DYD-1082 ruling 1, a standard carries a transition claim's line in one of its sections")]
    public void StandardLineOnATransitionClaim_BuildsIntoItsSection()
    {
        var charter = SkillSpec.Standard(ChartSkill.Charter, "guides", "Charter", "area: guides") with { Sections = [new(ChartSection.Rules, "Rules")] };

        var tree = SkillBuilder.Build(ChartModel.Of([ChartModel.Lease], [ChartModel.Holder, ChartModel.Warden, charter], typeof(LeaseCharter)));

        Assert.Contains("## Rules\n\n- A held lease may be released.\n", tree.Files["dydo/guides/charter.md"]);
    }

    [Fact]
    [Trait("Job", "trap: K1 a transition into another machine's state fails the build")]
    public void TransitionAcrossMachines_FailsTheBuild() =>
        AssertRejected(ChartModel.Of([ChartModel.Lease, ChartModel.Other], [ChartModel.Holder, ChartModel.Warden], typeof(LeaseCrossing)),
            $"claim:{nameof(LeaseCrossing)}.{nameof(LeaseCrossing.Leap)} moves to LeaseState.Held, which is not a state of ChartMachine.Other; " +
            "a transition stays inside its machine");

    [Fact]
    [Trait("Job", "trap: K1 a transition from a state no machine declares fails the build instead of building a dangling move")]
    public void TransitionFromAStateNoMachineDeclares_FailsTheBuild() =>
        AssertRejected(ChartModel.With(typeof(LeaseCrossing)), "moves from OtherState.Idle, which is no machine's state");

    [Fact]
    [Trait("Job", "contract: K1 a system transition, carried by no skill, builds into its machine's module")]
    public void SystemTransitionWithNoLine_BuildsIntoTheModule()
    {
        Assert.Empty(LeaseMoves.Land.Lines);

        var tla = SkillBuilder.Build(ChartModel.With()).Files["claims/statechart/lease/Lease.tla"];

        Assert.Contains($"\\* claim:{nameof(LeaseMoves)}.{nameof(LeaseMoves.Land)}\nLeaseMoves_Land ==\n", tla);
    }

    [Fact]
    [Trait("Job", "contract: universal rule 1 a production claim that cites no spec rule fails the build")]
    public void CitingModel_ClaimWithNoSpecId_FailsTheBuild() =>
        AssertRejected(ChartModel.With(typeof(LeaseUncited)), $"claim:{nameof(LeaseUncited)}.{nameof(LeaseUncited.Silent)} cites no spec rule");

    [Fact]
    [Trait("Job", "contract: universal rule 1 a production machine state that cites no spec rule fails the build")]
    public void CitingModel_StateWithNoSpecId_FailsTheBuild() =>
        AssertRejected(ChartModel.Of([ChartModel.Lease with { States = [.. ChartModel.Lease.States[..3], new(LeaseState.Done, "Done")] }],
            [ChartModel.Holder, ChartModel.Warden]), "state LeaseState.Done cites no spec rule");

    [Fact]
    [Trait("Job", "contract: universal rule 1 a fixture model need not cite the spec, so the migration fixture builds")]
    public void FixtureModel_ClaimWithNoSpecId_Builds() =>
        Assert.Contains(SkillBuilder.Build(ChartModel.With(typeof(LeaseUncited)) with { CitesSpec = false }).Map,
            line => line.Claims.Contains($"{nameof(LeaseUncited)}.{nameof(LeaseUncited.Silent)}"));

    [Fact]
    [Trait("Job", "trap: universal rule 1 a malformed spec id fails the build, in any model")]
    public void ClaimCitingAMalformedId_FailsTheBuild() =>
        AssertRejected(ChartModel.With(typeof(LeaseMiscited)) with { CitesSpec = false },
            $"claim:{nameof(LeaseMiscited)}.{nameof(LeaseMiscited.Misnamed)} cites \"X4\", which is not a spec rule id");

    [Theory]
    [Trait("Job", "contract: universal rule 1 a spec id is an area id, a CA row, or N12 with its DR section")]
    [InlineData("K4", true)]
    [InlineData("R23", true)]
    [InlineData("S3", true)]
    [InlineData("H24", true)]
    [InlineData("L1", true)]
    [InlineData("T15", true)]
    [InlineData("D5", true)]
    [InlineData("Y2", true)]
    [InlineData("N14", true)]
    [InlineData("CA20", true)]
    [InlineData("N12 DR 055", true)]
    [InlineData("N12 DR 049 §2", true)]
    [InlineData("N12 DR 049 §2–4", true)]
    [InlineData("X4", false)]
    [InlineData("K", false)]
    [InlineData("K04", false)]
    [InlineData("k4", false)]
    [InlineData(" K4", false)]
    [InlineData("CA", false)]
    [InlineData("N13 DR 049", false)]
    [InlineData("N12 DR 49", false)]
    [InlineData("N12 DR 049 §", false)]
    public void SpecCitation_AcceptsOnlySpecRuleIds(string id, bool valid) => Assert.Equal(valid, SpecCitation.IsValid(id));

    [Fact]
    [Trait("Job", "trap: universal rule 1 a citing claim whose only id is a number the spec does not reach fails the build")]
    public void CitingModel_ClaimCitingK999_FailsTheBuild() =>
        AssertRejected(ChartModel.With(typeof(LeaseOutOfSpec)),
            $"claim:{nameof(LeaseOutOfSpec)}.{nameof(LeaseOutOfSpec.Unnumbered)} cites \"K999\", which is not a spec rule id");

    [Fact]
    [Trait("Job", "trap: universal rule 1 a citing claim whose only reference is N12 with a DR the spec never names fails the build")]
    public void CitingModel_ClaimCitingN12Dr999_FailsTheBuild() =>
        AssertRejected(ChartModel.With(typeof(LeaseUnknownDr)),
            $"claim:{nameof(LeaseUnknownDr)}.{nameof(LeaseUnknownDr.Uncarried)} cites \"N12 DR 999\", which is not a spec rule id");

    [Theory]
    [Trait("Job", "contract: universal rule 1 each area's ids run from 1 to the spec's last rule (third amendment, approved 2026-11-21), and no further")]
    [InlineData("K34", "K35")]
    [InlineData("R34", "R35")]
    [InlineData("S26", "S27")]
    [InlineData("H24", "H25")]
    [InlineData("L23", "L24")]
    [InlineData("T23", "T24")]
    [InlineData("D12", "D13")]
    [InlineData("Y9", "Y10")]
    [InlineData("N15", "N16")]
    [InlineData("CA24", "CA25")]
    public void SpecCitation_StopsAtEachAreasLastRule(string last, string past)
    {
        Assert.True(SpecCitation.IsValid(last), last);
        Assert.False(SpecCitation.IsValid(past), past);
    }

    [Theory]
    [Trait("Job", "contract: universal rule 1 N12 names only a DR the spec carries forward or a rule cites")]
    [InlineData("N12 DR 045 §8", true)]
    [InlineData("N12 DR 053", true)]
    [InlineData("N12 DR 072 §1", true)]
    [InlineData("N12 DR 044", false)]
    [InlineData("N12 DR 054", false)]
    [InlineData("N12 DR 073", false)]
    public void SpecCitation_AcceptsOnlyDrsTheSpecNames(string id, bool valid) => Assert.Equal(valid, SpecCitation.IsValid(id));

    [Fact]
    [Trait("Job", "contract: universal rule 1 and K22 the production model cites the spec and builds two officer skills, the four crew kinds and both standards, and renders the hand-written ticket captain unwritten (DYD-1439)")]
    public void ProductionModel_CitesTheSpecAndBuilds()
    {
        Assert.True(ProductionModel.Model.CitesSpec);

        var tree = SkillBuilder.Build(ProductionModel.Model);
        var files = tree.Files.Keys;

        Assert.Equal(
        [
            "skills/roles/crew/implementer/SKILL.md", "skills/roles/crew/research/SKILL.md", "skills/roles/crew/reviewer/SKILL.md", "skills/roles/crew/scout/SKILL.md",
            "skills/roles/officers/chief-of-staff/SKILL.md", "skills/roles/officers/coordinator/SKILL.md"
        ], files.Where(path => path.EndsWith("/SKILL.md", StringComparison.Ordinal)).Order(StringComparer.Ordinal));
        Assert.Contains("skills/roles/officers/ticket-captain/SKILL.md", tree.Unwritten.Keys);
        Assert.Contains("dydo/guides/working-tree-standard.md", files);
        Assert.Contains("Scaffold/dydo/reference/linear-workspace-standard.md", files);
    }

    [Theory]
    [Trait("Job", "contract: K9 a skill's states are machine states, titled as their machine titles them")]
    [InlineData("retitled", "state LeaseState.Held")]
    [InlineData("no machine's", "state OtherState.Idle")]
    [InlineData("other machine's", "state LeaseState.Held")]
    public void SkillStateThatIsNotItsMachinesState_FailsTheBuild(string state, string named)
    {
        var holder = ChartModel.Holder with
        {
            Phases =
            [
                new("Hold", state switch
                {
                    "retitled" => [Slot.State(LeaseState.Held, "Holding")],
                    "no machine's" => [Slot.State(OtherState.Idle, "Idle")],
                    _ => ChartModel.Other.Slots(LeaseState.Held)
                })
            ]
        };

        AssertRejected(ChartModel.Of([ChartModel.Lease], [holder, ChartModel.Warden]),
            $"holder: {named} is not a machine state as its machine titles it; declare it with Machine.Slots");
    }

    [Fact]
    [Trait("Job", "contract: K1 every state belongs to exactly one machine")]
    public void StateInTwoMachines_FailsTheBuild() =>
        AssertRejected(ChartModel.Of([ChartModel.Lease, ChartModel.Other with { States = [new(LeaseState.Free, "Free", "K1")], Initial = LeaseState.Free }], Skills),
            "state LeaseState.Free is declared twice; a state belongs to exactly one machine");

    [Fact]
    [Trait("Job", "trap: K1 a machine declared twice fails the build, so one machine id never names two charts")]
    public void MachineDeclaredTwice_FailsTheBuild() =>
        AssertRejected(ChartModel.Of([ChartModel.Lease, ChartModel.Lease], Skills), "machine ChartMachine.Lease is declared twice");

    [Theory]
    [Trait("Job", "contract: K1 a machine's initial state, final states and gaps are its own states")]
    [InlineData("initial")]
    [InlineData("final")]
    [InlineData("gap from")]
    [InlineData("gap to")]
    public void MachineNamingAnotherMachinesState_FailsTheBuild(string part)
    {
        var lease = part switch
        {
            "initial" => ChartModel.Lease with { Initial = OtherState.Idle },
            "final" => ChartModel.Lease with { Finals = [OtherState.Idle] },
            "gap from" => ChartModel.Lease with { Gaps = [new(OtherState.Idle, null, "open")] },
            _ => ChartModel.Lease with { Gaps = [new(LeaseState.Held, OtherState.Idle, "open")] }
        };

        AssertRejected(ChartModel.Of([lease, ChartModel.Other], [ChartModel.Holder, ChartModel.Warden]),
            "machine ChartMachine.Lease names OtherState.Idle, which is not one of its states");
    }

    private static readonly SkillSpec[] Skills = [ChartModel.Holder, ChartModel.Warden];

    private static void AssertRejected(Model model, string message) =>
        Assert.Contains(message, Assert.Throws<InvalidDataException>(() => SkillBuilder.Build(model)).Message);
}
