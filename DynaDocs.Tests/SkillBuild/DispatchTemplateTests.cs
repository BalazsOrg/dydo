namespace DynaDocs.Tests.SkillBuild;

using DynaDocs.SkillBuild;
using DynaDocs.Tests.SkillBuild.Slots;

/// <summary>
/// K14: one built dispatch template per crew kind and per officer kind, each naming its default skill
/// set with a slash, and a named skill that is no skill under <c>skills/</c> fails the run (DYD-1082
/// 01266a65, DYD-1195 e0eadc20).
/// </summary>
public class DispatchTemplateTests
{
    private const string Message = "K14: demo's Crew brief names /codebase-design, which is no skill under skills/, hand-written or built";

    // The demo skill, its brief naming itself and a hand-written skill.
    private static readonly Model Naming = new([typeof(DemoMoves)],
    [
        DemoModel.Skill(phases:
        [
            DemoModel.Phases[0],
            DemoModel.Phases[1] with { Resources = [DemoModel.Guide, DemoModel.Brief with { Skills = ["demo", "codebase-design"] }] }
        ])
    ]);

    [Fact]
    [Trait("Job", "contract: K14 a template skill that is no skill under skills/ fails the run, which writes nothing")]
    public void TemplateSkillUnderNoSkillFolder_FailsTheRun()
    {
        using var root = new TempRoot();

        var (exit, _, error) = root.Run(Naming);

        Assert.Equal(1, exit);
        Assert.Contains(Message, error);
        Assert.Empty(root.Files());
    }

    [Fact]
    [Trait("Job", "edge: K14 a hand-written skill under skills/, at any depth, and a skill the model builds both resolve")]
    public void TemplateSkillsHandWrittenOrBuilt_TheRunGoesOn()
    {
        using var root = new TempRoot();
        root.Write("skills/engineering/codebase-design/SKILL.md", "# Codebase design\n");

        Assert.Equal(0, root.Run(Naming).Exit);
        Assert.Contains("\nSkills: /demo /codebase-design\n", root.Files()["skills/roles/crew/demo/resources/brief.md"]);
    }

    [Fact]
    [Trait("Job", "trap: K14 a built skill the model no longer builds is deleted by the run, so it resolves no template skill")]
    public void TemplateSkillOnlyLeftOverFromAnEarlierBuild_FailsTheRun()
    {
        using var root = new TempRoot();
        root.Write("skills/roles/crew/codebase-design/SKILL.md", BuiltHeader.Markdown + "\n\n# Codebase design\n");

        var (exit, _, error) = root.Run(Naming);

        Assert.Equal(1, exit);
        Assert.Contains(Message, error);
    }

    [Fact]
    [Trait("Job", "contract: K14/K30 each of the six kinds has one template, its default skill set opening with the kind's own skill")]
    public void ProductionTemplates_OnePerKindOpeningWithItsOwnSkill()
    {
        var templates = ProductionModel.Model.Skills
            .SelectMany(skill => skill.Phases.SelectMany(phase => phase.Resources).Concat(skill.Scenarios))
            .Where(resource => resource.Skills.Count > 0)
            .DistinctBy(template => template.Id)
            .ToDictionary(template => template.Id, template => template.Skills[0]);

        Assert.Equal(new Dictionary<Enum, string>
        {
            [ResourceName.CoordinatorBrief] = "coordinator",
            [ResourceName.TicketCaptainBrief] = "ticket-captain",
            [ResourceName.ImplementerBrief] = "implementer",
            [ResourceName.ReviewerBrief] = "reviewer",
            [ResourceName.ResearchBrief] = "research",
            [ResourceName.ScoutBrief] = "scout"
        }, templates);
    }

    [Fact]
    [Trait("Job", "contract: K14 every production template skill resolves in the committed tree")]
    public void ProductionTemplateSkills_ResolveInTheCommittedTree()
    {
        var error = new StringWriter();

        Assert.Equal(0, Program.Run(ProductionModel.Model, CommittedTree.Root, ["map"], TextWriter.Null, error));
        Assert.Equal("", error.ToString());
    }
}
