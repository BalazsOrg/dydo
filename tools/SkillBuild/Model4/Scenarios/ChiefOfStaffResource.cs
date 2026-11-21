namespace DynaDocs.SkillBuild;

/// <summary>
/// The chief of staff's night resources, each built to <c>resources/&lt;kebab-case name&gt;.md</c> (K9, K11): the
/// muster checklist, the night survey and the morning brief, carried over from the pre-candidate chief of staff
/// without the night orders or the admiral's standing orders (DYD-1195 33520d6c). A resource's id is its opening slot.
/// The muster is rebuilt, truth-checked ahead of the skill (DYD-1402).
/// </summary>
public enum ChiefOfStaffResource
{
    Muster,
    NightSurvey,
    MorningBrief
}
