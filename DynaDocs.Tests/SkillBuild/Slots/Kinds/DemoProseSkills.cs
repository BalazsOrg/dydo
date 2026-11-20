namespace DynaDocs.Tests.SkillBuild.Slots;

using DynaDocs.SkillBuild;

/// <summary>
/// Prose that names a built skill and a hand-written one with a slash, beside slashes that name no skill:
/// paths, a file, a URL and "and/or" (K14, review r1 S7).
/// </summary>
public static class DemoProseSkills
{
    public static readonly Claim Named = new("Name skills with a slash.", [DemoActor.Crew],
        [new(DemoSkill.Demo, "Load `/demo` and /codebase-design; never `tools/SkillBuild`, `/var/tmp/lock.sh`, ./notes.md, `/demo.md`, https://example.com/x and/or 1/2.", new(DemoSlot.Dispatch, 2))]);
}
