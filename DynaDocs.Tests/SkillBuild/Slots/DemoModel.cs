namespace DynaDocs.Tests.SkillBuild.Slots;

using DynaDocs.SkillBuild;
using DynaDocs.Tests.SkillBuild.Slots.Kinds;

/// <summary>The slot fixture's skill, declared in full unless a test swaps one of its slot lists.</summary>
internal static class DemoModel
{
    public static readonly Slot[] States = [new(DemoSlot.Intake, "Intake"), new(DemoSlot.Work, "Work")];

    public static readonly Slot[] Routines = [new(DemoSlot.Dispatch, "Dispatch crew")];

    public static readonly Slot[] Appendices = [new(DemoSlot.StaleLease, "Stale lease")];

    public static SkillSpec Skill(Slot[]? states = null, Slot[]? routines = null, Slot[]? appendices = null) =>
        SkillSpec.Crew(DemoSkill.Demo, "Demo", "A demo skill.") with
        {
            ExplicitOnly = true,
            OpenAi = new("display_name: Demo", "tools: []\n"),
            States = states ?? States,
            Routines = routines ?? Routines,
            Resources = [new(DemoSlot.Guide, "Guide", [new(DemoSlot.GuideLead), new(DemoSlot.Commands, "Commands")])],
            Appendices = appendices ?? Appendices
        };

    public static Model With(params Type[] kinds) => new([typeof(DemoMoves), .. kinds], [Skill()]);
}
