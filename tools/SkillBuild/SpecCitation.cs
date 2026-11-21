using System.Text.RegularExpressions;

namespace DynaDocs.SkillBuild;

/// <summary>
/// A spec rule id the approved spec defines: an area letter and number (<c>K4</c>, <c>R23</c>), a
/// conformance-audit row (<c>CA20</c>), or <c>N12</c> with a DR the spec names and its section
/// (<c>N12 DR 049 §2–4</c>).
/// </summary>
public static partial class SpecCitation
{
    // "Spec: dydo 4.0.0", its third amendment approved 2026-11-21 (DYD-1195 comment 467198f4): each area's rules run
    // without a gap from 1 to these, and CA1–CA24 are the conformance-audit rows.
    private static readonly Dictionary<string, int> LastRule = new(StringComparer.Ordinal)
    {
        ["K"] = 34, ["R"] = 34, ["S"] = 26, ["H"] = 24, ["L"] = 23, ["T"] = 23, ["D"] = 12, ["Y"] = 9, ["N"] = 15, ["CA"] = 24
    };

    // The same spec's DRs: N12's carried-forward list (045–047, 049, 052, 055–058, 060–063) and every DR a
    // rule cites (045–053, 055–072). DR 054 is named only in the DR key, and no rule cites it.
    private static readonly HashSet<int> NamedDrs = [.. Enumerable.Range(45, 9), .. Enumerable.Range(55, 18)];

    [GeneratedRegex("^(?:(?<area>CA|[KRSHLTDYN])(?<rule>[1-9][0-9]*)|N12 DR (?<dr>[0-9]{3})(?: §[0-9]+(?:–[0-9]+)?)?)$")]
    private static partial Regex Id();

    public static bool IsValid(string id)
    {
        var match = Id().Match(id);
        if (!match.Success) return false;
        return match.Groups["dr"].Success
            ? NamedDrs.Contains(int.Parse(match.Groups["dr"].Value))
            : int.TryParse(match.Groups["rule"].Value, out var rule) && rule <= LastRule[match.Groups["area"].Value];
    }
}
