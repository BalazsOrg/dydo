namespace DynaDocs.Tests.SkillBuild.Slots;

using DynaDocs.SkillBuild;

/// <summary>
/// A broken kind: a block that brings the stale-lease scenario to exactly 100 lines, one past K9's
/// "under 100 lines", opening with its own contents list.
/// </summary>
public static class DemoLongScenario
{
    public static readonly Claim Overlong = new("Keep a scenario short.", [DemoActor.Crew],
        [new(DemoSkill.Demo, string.Join('\n', ["## Contents", "", "- [Steps](#steps)", "", "## Steps", "",
            .. Enumerable.Range(1, 87).Select(step => $"Step {step} of the long scenario.")]), new(DemoSlot.StaleLeaseSteps, 3))]);
}
