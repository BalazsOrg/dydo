namespace DynaDocs.Tests.SkillBuild.Slots;

using DynaDocs.SkillBuild;

/// <summary>The slot fixture's skill, declared in full unless a test swaps its phases or scenarios.</summary>
internal static class DemoModel
{
    public static readonly Resource Guide = new(DemoSlot.Guide, "Guide", new Slot(DemoSlot.GuideLead), new(DemoSlot.Commands, "Commands"));

    public static readonly Resource Brief = new(DemoSlot.Brief, "Crew brief") { Skills = ["demo"] };

    public static readonly Phase[] Phases =
    [
        new("Take the ticket", new Slot(DemoSlot.Accountable), new(DemoSlot.Tools, "Tools and surfaces"), Slot.State(DemoSlot.Intake, "Intake")),
        new("Do the work", Slot.State(DemoSlot.Work, "Work"), new(DemoSlot.Dispatch, "Dispatch crew")) { Resources = [Guide, Brief] }
    ];

    public static readonly Resource[] Scenarios = [new(DemoSlot.StaleLease, "Stale lease", new Slot(DemoSlot.StaleLeaseSteps))];

    public static SkillSpec Skill(Phase[]? phases = null, Resource[]? scenarios = null) =>
        SkillSpec.Crew(DemoSkill.Demo, "Demo", "A demo skill.") with
        {
            ExplicitOnly = true,
            OpenAi = new("display_name: Demo", "tools: []\n"),
            Phases = phases ?? Phases,
            Scenarios = scenarios ?? Scenarios
        };

    public static Model With(params Type[] kinds) => new([typeof(DemoMoves), .. kinds], [Skill()]);
}
