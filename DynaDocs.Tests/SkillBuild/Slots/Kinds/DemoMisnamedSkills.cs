namespace DynaDocs.Tests.SkillBuild.Slots;

using DynaDocs.SkillBuild;

/// <summary>A broken kind: prose that names two skills by typos, as review r1 S7's /reviewer and /diagnosing-bugs could be (K14).</summary>
public static class DemoMisnamedSkills
{
    public static readonly Claim Misnamed = new("Name each skill as it is.", [DemoActor.Crew],
        [new(DemoSkill.Demo, "Brief the crew with /reviewr, and on a Bug add /diagnosing-bugz.", new(DemoSlot.Dispatch, 2))]);
}
