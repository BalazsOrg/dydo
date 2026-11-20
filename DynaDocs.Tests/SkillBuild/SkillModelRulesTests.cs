namespace DynaDocs.Tests.SkillBuild;

using DynaDocs.SkillBuild;
using DynaDocs.Tests.SkillBuild.Slots;

/// <summary>
/// The rules the build enforces on a model, each red through <see cref="SkillBuilder.Build"/>: K4's one
/// line per claim per skill, and K9's spine of a job paragraph, phases and scenarios and its placements
/// (DYD-1196 A4, A5).
/// </summary>
public class SkillModelRulesTests
{
    [Fact]
    [Trait("Job", "contract: K4 a claim with two lines in one skill fails the build (A4)")]
    public void ClaimWithTwoLinesInOneSkill_FailsTheBuild() =>
        AssertRejected(DemoModel.With(typeof(DemoTwice)), $"demo states claim:{Id(nameof(DemoTwice), nameof(DemoTwice.SaidTwice))} 2 times");

    [Fact]
    [Trait("Job", "contract: K4 a combined line counts once for each claim it names (A4)")]
    public void CombinedLineNamingAClaimTheSkillAlsoStates_FailsTheBuild() =>
        AssertRejected(DemoModel.With(typeof(DemoEcho)), $"demo states claim:{Id(nameof(DemoEcho), nameof(DemoEcho.Echo))} 2 times");

    [Theory]
    [Trait("Job", "contract: K4/A4 a claim stated zero times, its own line or its combined line left out, fails the build")]
    [InlineData(typeof(DemoSilent), nameof(DemoSilent.Unsaid))]
    [InlineData(typeof(DemoUncombined), nameof(DemoUncombined.Dropped))]
    public void ClaimNoLineStates_FailsTheBuildNamingIt(Type kind, string member) =>
        AssertRejected(DemoModel.With(kind), $"claim:{Id(kind.Name, member)} is stated by no line");

    [Fact]
    [Trait("Job", "contract: K4 a combined line states every claim it names, on one line (A4)")]
    public void CombinedLine_StatesEachClaimItNamesOnce()
    {
        var line = SkillBuilder.Build(DemoModel.With()).Map.Single(entry => entry.Text == "Save the work, then resume.");

        Assert.Equal([Id(nameof(DemoMoves), nameof(DemoMoves.SaveTheWork)), Id(nameof(DemoMoves), nameof(DemoMoves.ResumeAfterSaving))], line.Claims);
    }

    [Fact]
    [Trait("Job", "contract: K9 a line with no placement fails the build (A5)")]
    public void LineWithNoPlacement_FailsTheBuild() =>
        AssertRejected(DemoModel.With(typeof(DemoUnplaced)), $"demo: a line of claim:{Id(nameof(DemoUnplaced), nameof(DemoUnplaced.Nowhere))} has no placement");

    [Fact]
    [Trait("Job", "contract: K9 a placement in an undeclared slot fails the build (A5)")]
    public void PlacementInAnUndeclaredSlot_FailsTheBuild() =>
        AssertRejected(Model(DemoModel.Skill(phases: [DemoModel.Phases[0]])),
            $"demo: claim:{Id(nameof(DemoMoves), nameof(DemoMoves.StartAfterIntake))} is placed in DemoSlot.Work, which the skill does not declare");

    [Fact]
    [Trait("Job", "contract: K9 a scenario of 100 lines fails the build, contents list or not")]
    public void ScenarioOfOneHundredLines_FailsTheBuild() =>
        AssertRejected(DemoModel.With(typeof(DemoLongScenario)),
            "demo: scenario DemoSlot.StaleLease is 100 lines; a scenario stays under 100 lines");

    [Theory]
    [Trait("Job", "contract: K9 a state holds moves, and only a state does (A5)")]
    [InlineData(true)]
    [InlineData(false)]
    public void MoveOutsideAState_OrAStateLineWithoutAMove_FailsTheBuild(bool routineAsState)
    {
        var skill = routineAsState
            ? DemoModel.Skill(phases: [DemoModel.Phases[0], Work(Slot.State(DemoSlot.Work, "Work"), Slot.State(DemoSlot.Dispatch, "Dispatch"))])
            : DemoModel.Skill(phases: [new("Take the ticket", new Slot(DemoSlot.Accountable), new(DemoSlot.Tools, "Tools"), new(DemoSlot.Intake, "Intake")), DemoModel.Phases[1]]);

        AssertRejected(Model(skill), "a line in a state is a one-line move with its step");
    }

    [Fact]
    [Trait("Job", "trap: K9 a job line of two lines splits the job paragraph")]
    public void JobLineOfTwoLines_FailsTheBuild() =>
        AssertRejected(DemoModel.With(typeof(DemoJobBlock)), "a job line is one line");

    [Fact]
    [Trait("Job", "contract: K9 a line takes an explicit order in its slot that no other line holds (A5)")]
    public void LineTakingAnOrderAnotherLineHolds_FailsTheBuild() =>
        AssertRejected(DemoModel.With(typeof(DemoClash)), "takes order 1 in DemoSlot.Dispatch, which another line holds");

    [Fact]
    [Trait("Job", "trap: a slot declared twice renders its lines twice, past K4's count of distinct lines")]
    public void SlotDeclaredTwice_FailsTheBuild() =>
        AssertRejected(Model(DemoModel.Skill(phases: [DemoModel.Phases[0], Work(Slot.State(DemoSlot.Work, "Work"), new(DemoSlot.Dispatch, "Dispatch crew"), new(DemoSlot.Work, "Again"))])),
            "demo: DemoSlot.Work is declared twice");

    [Fact]
    [Trait("Job", "trap: two resources with one id build one file twice")]
    public void ResourceDeclaredTwice_FailsTheBuild() =>
        AssertRejected(Model(DemoModel.Skill(scenarios: [.. DemoModel.Scenarios, new(DemoSlot.Guide, "Again")])), "demo: resource DemoSlot.Guide is declared twice");

    [Theory]
    [Trait("Job", "trap: an untitled slot past a phase's first, or an untitled state, renders an empty heading")]
    [InlineData(false)]
    [InlineData(true)]
    public void UntitledSlotThatDoesNotOpenItsPhase_FailsTheBuild(bool state) =>
        AssertRejected(Model(DemoModel.Skill(phases: state
                ? [new("Take the ticket", Slot.State(DemoSlot.Intake, null), new(DemoSlot.Accountable, "Accountable"), new(DemoSlot.Tools, "Tools")), DemoModel.Phases[1]]
                : [DemoModel.Phases[0], Work(Slot.State(DemoSlot.Work, "Work"), new(DemoSlot.Dispatch))])),
            state ? "DemoSlot.Intake needs a title" : "DemoSlot.Dispatch needs a title");

    [Fact]
    [Trait("Job", "contract: K9 a job phase is numbered by its title, so it needs one")]
    public void UntitledPhase_FailsTheBuild() =>
        AssertRejected(Model(DemoModel.Skill(phases: [DemoModel.Phases[0], DemoModel.Phases[1] with { Title = " " }])), "demo: a job phase needs a title");

    [Fact]
    [Trait("Job", "contract: K9 a spine is its job paragraph, phases and scenarios, never a standard's sections")]
    public void SkillWithSections_FailsTheBuild() =>
        AssertRejected(Model(DemoModel.Skill() with { Sections = [new(DemoSlot.GuideLead)] }), "demo: a skill's spine is its job paragraph, phases and scenarios");

    [Fact]
    [Trait("Job", "trap: a standard renders only its sections, so a declared phase would vanish without a word")]
    public void StandardWithMoreThanSections_FailsTheBuild() =>
        AssertRejected(new([], [SkillSpec.Standard(DemoSkill.Demo, "guides", "Demo") with { Phases = DemoModel.Phases }]),
            "demo: a standard holds sections only");

    [Fact]
    [Trait("Job", "trap: a line for a skill the model does not build crashes the tool without naming the claim")]
    public void LineForAnUndeclaredSkill_FailsTheBuild() =>
        AssertRejected(new([typeof(DemoMoves)], []), "has a line for DemoSkill.Demo, which the model does not declare");

    [Fact]
    [Trait("Job", "trap: a skill declared twice crashes the tool without naming the skill")]
    public void SkillDeclaredTwice_FailsTheBuild() =>
        AssertRejected(new([], [DemoModel.Skill(), DemoModel.Skill()]), "skill DemoSkill.Demo is declared twice");

    [Fact]
    [Trait("Job", "contract: a kind is a static class (A1)")]
    public void KindThatIsNotAStaticClass_FailsTheBuild() =>
        AssertRejected(new([typeof(string)], []), "kind String is not a static class");

    [Fact]
    [Trait("Job", "trap: one claim object behind two fields would carry two ids (A2)")]
    public void ClaimBehindTwoFields_FailsTheBuild() =>
        AssertRejected(new([typeof(DemoAlias)], []), $"claim:{Id(nameof(DemoAlias), nameof(DemoAlias.Alias))} is declared twice");

    [Fact]
    [Trait("Job", "contract: K23 every id a line states resolves to a model claim (D19)")]
    public void CombinedLineNamingAClaimOutsideTheModel_FailsTheBuild() =>
        AssertRejected(Model(DemoModel.Skill(), typeof(DemoStray)), "has a combined line naming a claim outside the model's kinds");

    private static Model Model(SkillSpec skill, params Type[] kinds) => new([typeof(DemoMoves), .. kinds], [skill]);

    private static Phase Work(params Slot[] slots) => DemoModel.Phases[1] with { Slots = slots };

    private static string Id(string kind, string member) => $"{kind}.{member}";

    private static void AssertRejected(Model model, string message) =>
        Assert.Contains(message, Assert.Throws<InvalidDataException>(() => SkillBuilder.Build(model)).Message);
}
