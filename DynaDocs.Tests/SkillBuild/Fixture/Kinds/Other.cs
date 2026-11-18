namespace DynaDocs.Tests.SkillBuild.Fixture;

using DynaDocs.SkillBuild;
using A = DynaDocs.Tests.SkillBuild.Fixture.FixtureActor;
using F = DynaDocs.Tests.SkillBuild.Fixture.FixtureSection;
using S = DynaDocs.Tests.SkillBuild.Fixture.FixtureSkill;

/// <summary>Migration fixture: the inventory's <c>other</c> claims, sub-topics leave to recurring.</summary>
public static class Other
{
    public static readonly Claim LeaveNoWorse = new(
        "Leave what you touched no worse than you found it.",
        [A.AnyAgent],
        [
            new(S.CodeWriter, "Leave what you touched no worse than you found it.", new(F.CodeWriterBuild, 9))
        ]);

    public static readonly Claim LoadSkillWhenTriggerFires = new(
        "Load a skill when its trigger fires.",
        [A.CodeWriter],
        [
            new(S.CodeWriter, "Load a skill when its trigger fires:", new(F.CodeWriterBuild, 1))
        ]);

    public static readonly Claim LoadWayfinderBeforeFirstReport = new(
        "Load wayfinder and read its Refer by name section before the first human-facing Project or ticket report.",
        [A.Admiral],
        [
            new(S.Admiral, "Load `wayfinder` and read its Refer by name section before the first human-facing Project or ticket report.", new(F.AdmiralMustReads, 2))
        ]);

    public static readonly Claim RecurringFrictionToRetro = new(
        "Friction that keeps recurring across sessions goes to retro with the occurrences named.",
        [A.ChiefOfStaff],
        [
            new(S.ChiefOfStaff, "Friction that keeps recurring across sessions goes to `retro` with the occurrences named.", new(F.ChiefOfStaffMethod, 17))
        ]);
}
