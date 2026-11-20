namespace DynaDocs.Tests.SkillBuild.Fixture;

using DynaDocs.SkillBuild;

/// <summary>
/// Migration fixture: a carrier file's top-level sections as job phases (K9), each holding its section's
/// lines, and its resources linked from the last.
/// </summary>
internal static class FixturePhases
{
    public static Phase[] Of(Slot[] sections, params Resource[] resources) =>
    [
        .. sections.Select((section, index) => new Phase(section.Title!, new Slot(section.Id))
        {
            Resources = index == sections.Length - 1 ? resources : []
        })
    ];
}
