namespace DynaDocs.Tests.SkillBuild.Slots;

using DynaDocs.SkillBuild;

/// <summary>A broken kind: misnamed skills inside bold, italic and code, the forms review r2 S7 found unchecked (K14).</summary>
public static class DemoEmphasisedSkills
{
    public static readonly Claim Emphasised = new("Name each skill as it is, however it is formatted.", [DemoActor.Crew],
        [new(DemoSkill.Demo, "Skills: **/reviewr** __/scoutt__ */researh* _/implementr_ `/codewriter` **`/inquisitr`**", new(DemoSlot.Dispatch, 2))]);
}
