using System.Text.RegularExpressions;

namespace DynaDocs.SkillBuild;

/// <summary>
/// A spec rule id: an area letter and number (<c>K4</c>, <c>R23</c>), a conformance-audit row
/// (<c>CA19</c>), or <c>N12</c> with the carried-forward DR and section it names (<c>N12 DR 049 §2–4</c>).
/// </summary>
public static partial class SpecCitation
{
    [GeneratedRegex("^(?:[KRSHLTDYN][1-9][0-9]*|CA[1-9][0-9]*|N12 DR [0-9]{3}(?: §[0-9]+(?:–[0-9]+)?)?)$")]
    private static partial Regex Id();

    public static bool IsValid(string id) => Id().IsMatch(id);
}
