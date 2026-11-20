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
        using var root = Skills("grilling", "walkthrough", "wayfinder", "co-thinker", "deep/wizard");

        Assert.Equal(0, root.Run(ProductionModel.Model, "map").Exit);
    }

    private static Model With(EnvoyTable table) => ProductionModel.Model with { Envoys = table };

    private static TempRoot Skills(params string[] names)
    {
        var root = new TempRoot();
        foreach (var name in names) root.Write($"skills/productivity/{name}/SKILL.md", "# skill\n");
        return root;
    }
}
