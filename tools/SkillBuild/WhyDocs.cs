using System.Text.RegularExpressions;

namespace DynaDocs.SkillBuild;

/// <summary>
/// One why doc per claim kind, <c>dydo/reference/claims/&lt;kind&gt;.md</c> (K13): each claim's rule, its
/// spec ids, its machine move with the machine named, and every line that states it, by skill and
/// place. A truth-checked claim also shows its evidence, layer and reason, and each such line its
/// reader, so the claim can be reviewed alone (DYD-1295 317be5f7). A line no built file holds says so:
/// legacy and not built, or withheld; a placeholder's line says that it is one. It lives under
/// <c>dydo/</c> only, which never ships.
/// </summary>
internal static partial class WhyDocs
{
    internal static readonly string[] Frontmatter = ["area: reference", "type: reference"];

    // A skill line's link is relative to its skill folder, so in a why doc it names its target instead.
    [GeneratedRegex(@"\[([^\]]+)\]\(([^)\s]+)\)")]
    private static partial Regex Link();

    /// <param name="unbuilt">The legacy skills a ledger replaces: their lines stand in the model, and no file states them.</param>
    public static IEnumerable<(string Path, string Text)> Render(IEnumerable<Type> kinds, ClaimIndex claims, Statechart chart, IReadOnlySet<Enum> unbuilt) =>
        kinds.Distinct().Select(kind => ($"dydo/reference/claims/{Names.Kebab(kind.Name)}.md", Doc(kind.Name, claims, chart, unbuilt)));

    /// <summary>A line's text as a block quote, each link named rather than made.</summary>
    internal static string[] Quote(string text) =>
        [.. Link().Replace(text, "$1 (`$2`)").Split('\n').Select(line => line.Length == 0 ? ">" : "> " + line)];

    private static string Doc(string kind, ClaimIndex claims, Statechart chart, IReadOnlySet<Enum> unbuilt)
    {
        var page = new Page(Frontmatter, "Claims: " + kind);
        foreach (var (id, claim) in claims.Claims.Where(claim => claim.Id.StartsWith(kind + ".", StringComparison.Ordinal)))
        {
            page.Heading("## " + id[(kind.Length + 1)..]);
            page.Block($"`claim:{id}`: {claim.Rule}");
            if (claim.Spec.Count > 0) page.Block("Spec: " + string.Join(", ", claim.Spec));
            if (claim.Transition is { } move) page.Block(Move(move, chart, claims.LinesOf(claim).Count == 0));
            if (claim.Truth is { } truth) Answers(page, truth);
            foreach (var (line, owner) in claims.LinesOf(claim))
            {
                var place = Unstated(claim, line, unbuilt) + Place(line.Place) + (line.Reader is null ? "" : $"; read by {line.Reader}");
                page.Block($"**{Names.Kebab(line.Skill)}** ({place}{(owner == id ? "" : $", on claim:{owner}")}):");
                page.Block(Quote(line.Text));
            }
        }
        return page.Done("").Text;
    }

    // Why no built file holds the line: its legacy skill is replaced, or its claim is open, alone on its line (TruthCheck).
    // An open claim's line that is built is the placeholder's.
    private static string Unstated(Claim claim, Line line, IReadOnlySet<Enum> unbuilt)
    {
        if (unbuilt.Contains(line.Skill)) return "legacy, not built; ";
        return claim.Truth?.Evidence switch { { Token: not null } => "placeholder; ", { Kind: EvidenceKind.Open } => "withheld; ", _ => "" };
    }

    private static void Answers(Page page, Truth truth)
    {
        var text = truth.Evidence.Kind is EvidenceKind.Command or EvidenceKind.Code ? $"`{truth.Evidence.Text}`" : truth.Evidence.Text;
        page.Block(truth.Evidence switch
        {
            { Token: { } token } => $"Evidence: open, a placeholder: the skill shows `{token}` where this claim's text will stand. {text}",
            { Kind: EvidenceKind.Open } => $"Evidence: open, so no skill states this claim yet. {text}",
            _ => $"Evidence ({truth.Evidence.Kind.ToString().ToLowerInvariant()}): {text}"
        });
        page.Block("Layer: " + truth.Layer);
        page.Block("Reason: " + truth.Reason);
    }

    // dydo check resolves links only inside dydo/, so the chart is named by its module path, not linked.
    private static string Move(Transition move, Statechart chart, bool bySystem)
    {
        var machine = chart.MachineOf(move.From);
        // A machine whose chart is written by hand has no built module: the model's move is named beside the hand-written chart's folder.
        var link = machine.HandWritten is { } folder
            ? $"{machine.Title} (the model's own machine, not written; the hand-written chart is in `{folder}/`)"
            : $"{machine.Title} (`{Statechart.FolderOf(machine)}/{machine.Id}.tla`)";
        return $"Transition in {link}: {move.From} → {move.To}, \"{move.Arrow}\"." + (bySystem ? " A system transition: no skill carries it." : "");
    }

    private static string Place(Place place) =>
        place.Step is { } step ? $"{place.Slot}, {step.ToString().ToLowerInvariant()}" : place.Slot.ToString();
}
