namespace DynaDocs.Tests.SkillBuild;

using DynaDocs.SkillBuild;

/// <summary>
/// The builder checks the envoy skill table (H23; §4 check 19): each job keeps its fixed skill, each
/// skill is one under <c>skills/</c>, and the envoy brief states the closed-thread signal.
/// </summary>
public class EnvoyTableTests
{
    private static readonly EnvoyTable Table = ProductionModel.Model.Envoys!;

    [Fact]
    [Trait("Job", "trap: H23 an envoy skill table missing a job fails the build, naming the job")]
    public void TableMissingAJob_FailsTheBuild()
    {
        var table = Table with { Jobs = [.. Table.Jobs.Where(row => row.Job != EnvoyJob.Enablement)] };

        var failure = Assert.Throws<InvalidDataException>(() => SkillBuilder.Build(With(table)));

        Assert.Contains("Enablement", failure.Message);
    }

    [Fact]
    [Trait("Job", "trap: H23 an envoy job given another skill fails the build, naming its fixed skill")]
    public void JobGivenAnotherSkill_FailsTheBuild()
    {
        var table = Table with { Jobs = [.. Table.Jobs.Select(row => row.Job == EnvoyJob.Wayfinding ? row with { Skill = "grilling" } : row)] };

        var failure = Assert.Throws<InvalidDataException>(() => SkillBuilder.Build(With(table)));

        Assert.Contains("Wayfinding `grilling`, not `wayfinder`", failure.Message);
    }

    [Theory]
    [Trait("Job", "trap: H23 a Question stage's row missing fails the build, naming the Question and its stage")]
    [InlineData(QuestionStage.Research, "Question at stage `research`")]
    [InlineData(QuestionStage.Council, "Question at stage `council`")]
    [InlineData(QuestionStage.HisCall, "Question at stage `his-call`")]
    public void TableMissingAQuestionStage_FailsTheBuild(QuestionStage stage, string key)
    {
        var table = Table with { Jobs = [.. Table.Jobs.Where(row => row.Stage != stage)] };

        var failure = Assert.Throws<InvalidDataException>(() => SkillBuilder.Build(With(table)));

        Assert.Contains($"needs exactly one {key} row, not 0", failure.Message);
    }

    [Fact]
    [Trait("Job", "trap: H23 a Question row keyed on Type alone, with no stage, fails the build")]
    public void QuestionRowWithoutAStage_FailsTheBuild()
    {
        var table = Table with { Jobs = [.. Table.Jobs, new(EnvoyJob.Question, "co-thinker")] };

        var failure = Assert.Throws<InvalidDataException>(() => SkillBuilder.Build(With(table)));

        Assert.Contains("H23: the envoy skill table has a Question row, which H23 does not name", failure.Message);
    }

    [Fact]
    [Trait("Job", "trap: H23 a research Question given co-thinker fails the build; research is the ticket captain's with research crew")]
    public void ResearchStageGivenCoThinker_FailsTheBuild()
    {
        var table = Table with { Jobs = [.. Table.Jobs.Select(row => row.Stage == QuestionStage.Research ? row with { Skill = "co-thinker" } : row)] };

        var failure = Assert.Throws<InvalidDataException>(() => SkillBuilder.Build(With(table)));

        Assert.Contains("Question at stage `research` `co-thinker`, not `ticket-captain`", failure.Message);
    }

    [Fact]
    [Trait("Job", "trap: H23 a council Question without the council-chair resource fails the build")]
    public void CouncilStageWithoutItsResource_FailsTheBuild()
    {
        var table = Table with { Jobs = [.. Table.Jobs.Select(row => row.Stage == QuestionStage.Council ? row with { Resource = null } : row)] };

        var failure = Assert.Throws<InvalidDataException>(() => SkillBuilder.Build(With(table)));

        Assert.Contains("Question at stage `council` no resource, not the `council-chair` resource", failure.Message);
    }

    [Fact]
    [Trait("Job", "trap: H23 a council-chair resource the ticket captain does not declare fails the build")]
    public void CouncilResourceTheSkillDoesNotDeclare_FailsTheBuild()
    {
        var model = ProductionModel.Model with
        {
            Skills = [.. ProductionModel.Model.Skills.Select(skill => Equals(skill.Id, Skill.TicketCaptain)
                ? skill with { Phases = [.. skill.Phases.Select(phase => phase with { Resources = [.. phase.Resources.Where(resource => !Equals(resource.Id, TicketCaptainResource.CouncilChair))] })] }
                : skill)]
        };

        var failure = Assert.Throws<InvalidDataException>(() => SkillBuilder.Build(model));

        Assert.Contains("Question at stage `council` the `council-chair` resource, which `ticket-captain` does not declare", failure.Message);
    }

    [Fact]
    [Trait("Job", "contract: H23 K14 the ticket-captain brief states the stage-keyed job table its dispatchers add a job skill from")]
    public void ProductionTicketCaptainBrief_StatesTheStageTable()
    {
        var brief = SkillBuilder.Build(ProductionModel.Model).Files["skills/roles/officers/ticket-captain/resources/ticket-captain-brief.md"];

        Assert.Contains(EnvoyTable.Render(Envoy.Jobs), brief);
        Assert.Contains("| Question at stage `research` | `/ticket-captain`; it dispatches `research` crew |", brief);
        Assert.Contains("| Question at stage `council` | `/ticket-captain`, with its `council-chair` resource |", brief);
        Assert.Contains("| Question at stage `his-call` | `/co-thinker` |", brief);
    }

    // Review r1 nit 2: a dispatch names each skill with a slash (K14), the job table's skills included.
    [Fact]
    [Trait("Job", "contract: H23 K14 the job table names each job's skill with a slash")]
    public void Table_NamesEachSkillWithASlash()
    {
        var rows = EnvoyTable.Render(Envoy.Jobs).Split('\n')[2..];

        Assert.Equal(Envoy.Jobs.Count, rows.Length);
        Assert.All(Envoy.Jobs.Zip(rows), pair => Assert.StartsWith($"| {pair.First.Key} | `/{pair.First.Skill}`", pair.Second));
    }

    [Fact]
    [Trait("Job", "trap: H23 an envoy brief that states no closed-thread signal fails the build")]
    public void BriefWithoutTheCloseSignal_FailsTheBuild()
    {
        var failure = Assert.Throws<InvalidDataException>(() => SkillBuilder.Build(With(Table with { CloseSignal = Envoy.Template })));

        Assert.Contains("closed-thread signal", failure.Message);
    }

    [Fact]
    [Trait("Job", "trap: H23 a closed-thread signal the registered claims do not hold fails the build, though its line names the brief")]
    public void CloseSignalNotAmongTheRegisteredClaims_FailsTheBuild()
    {
        var unregistered = new Claim(Envoy.CloseSignal.Rule, [.. Envoy.CloseSignal.Actors], [.. Envoy.CloseSignal.Lines]) { Spec = Envoy.CloseSignal.Spec };

        var failure = Assert.Throws<InvalidDataException>(() => SkillBuilder.Build(With(Table with { CloseSignal = unregistered })));

        Assert.Contains("H23: the closed-thread signal is not a claim of the model", failure.Message);
    }

    [Fact]
    [Trait("Job", "trap: H23 a checked job table the envoy brief does not state fails the build")]
    public void JobTableTheBriefDoesNotState_FailsTheBuild()
    {
        var table = Table with { Jobs = [.. Table.Jobs.Select(row => row with { Note = null })] };

        var failure = Assert.Throws<InvalidDataException>(() => SkillBuilder.Build(With(table)));

        Assert.Contains("H23: the envoy brief does not state the checked skill table", failure.Message);
    }

    [Fact]
    [Trait("Job", "contract: H23 the production envoy brief states the closed-thread signal and the checked table")]
    public void ProductionEnvoyBrief_StatesTheSignalAndTheTable()
    {
        Assert.Equal((Envoy.Jobs, Envoy.CloseSignal), (ProductionModel.Model.Envoys?.Jobs, ProductionModel.Model.Envoys?.CloseSignal));
        var tree = SkillBuilder.Build(ProductionModel.Model);
        const string brief = "skills/roles/officers/coordinator/resources/envoy-brief.md";

        Assert.Contains(tree.Map, line => line.Path == brief && line.Claims.Contains($"{nameof(Envoy)}.{nameof(Envoy.CloseSignal)}"));
        Assert.Contains(EnvoyTable.Render(Envoy.Jobs), tree.Files[brief]);
    }

    [Fact]
    [Trait("Job", "trap: H23 a table skill that is not under skills/ fails the run, which writes nothing")]
    public void TableSkillNotUnderSkills_FailsTheRun()
    {
        using var root = Skills("grilling", "walkthrough", "wayfinder", "co-thinker");

        var (exit, _, error) = root.Run(ProductionModel.Model);

        Assert.Equal(1, exit);
        Assert.Contains("Enablement `wizard`, which is not a skill under skills/", error);
        Assert.Equal(4, root.Files().Count);
    }

    [Fact]
    [Trait("Job", "edge: H23 every table skill under skills/, at any depth, lets the run go on")]
    public void TableSkillsAllUnderSkills_TheRunGoesOn()
    {
        // The other skills the production prose names (review r1 S7) are the checkout's own, so only the table's decide.
        using var root = Skills(["grilling", "walkthrough", "wayfinder", "co-thinker", "deep/wizard", .. HandWrittenOutsideTheTable()]);

        var (exit, _, error) = root.Run(ProductionModel.Model, "map");

        Assert.True(exit == 0, error);
    }

    private static Model With(EnvoyTable table) => ProductionModel.Model with { Envoys = table };

    private static IEnumerable<string> HandWrittenOutsideTheTable() => Directory
        .EnumerateFiles(Path.Combine(CommittedTree.Root, "skills"), "SKILL.md", SearchOption.AllDirectories)
        .Where(file => !BuiltHeader.Carries(file))
        .Select(file => Path.GetFileName(Path.GetDirectoryName(file))!)
        .Except(Table.Jobs.Select(row => row.Skill), StringComparer.Ordinal);

    private static TempRoot Skills(params string[] names)
    {
        var root = new TempRoot();
        foreach (var name in names) root.Write($"skills/productivity/{name}/SKILL.md", "# skill\n");
        return root;
    }
}
