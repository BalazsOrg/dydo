namespace DynaDocs.SkillBuild;

/// <summary>
/// One why doc per claim kind, <c>dydo/reference/claims/&lt;kind&gt;.md</c> (K13): each claim's rule, its
/// spec ids, its machine move with a link to the machine, and every line that states it, by skill and
/// place. It lives under <c>dydo/</c> only, which never ships.
/// </summary>
internal static class WhyDocs
{
    private static readonly string[] Frontmatter = ["area: reference", "type: reference"];

    public static IEnumerable<(string Path, string Text)> Render(IEnumerable<Type> kinds, ClaimIndex claims, Statechart chart) =>
        kinds.Distinct().Select(kind => ($"dydo/reference/claims/{Names.Kebab(kind.Name)}.md", Doc(kind.Name, claims, chart)));

    private static string Doc(string kind, ClaimIndex claims, Statechart chart)
    {
        var page = new Page(Frontmatter, "Claims: " + kind);
        foreach (var (id, claim) in claims.Claims.Where(claim => claim.Id.StartsWith(kind + ".", StringComparison.Ordinal)))
        {
            page.Heading("## " + id[(kind.Length + 1)..]);
            page.Block($"`claim:{id}`: {claim.Rule}");
            if (claim.Spec.Count > 0) page.Block("Spec: " + string.Join(", ", claim.Spec));
            if (claim.Transition is { } move) page.Block(Move(move, chart, claims.LinesOf(claim).Count == 0));
            foreach (var (line, owner) in claims.LinesOf(claim))
            {
                page.Block($"**{Names.Kebab(line.Skill)}** ({Place(line.Place)}{(owner == id ? "" : $", on claim:{owner}")}):");
                page.Block([.. line.Text.Split('\n').Select(text => text.Length == 0 ? ">" : "> " + text)]);
            }
        }
        return page.Done("").Text;
    }

    private static string Move(Transition move, Statechart chart, bool bySystem)
    {
        var machine = chart.MachineOf(move.From);
        var link = $"[{machine.Title}](../../../{Statechart.FolderOf(machine)}/{machine.Id}.tla)";
        return $"Transition in {link}: {move.From} → {move.To}, \"{move.Arrow}\"." + (bySystem ? " A system transition: no skill carries it." : "");
    }

    private static string Place(Place place) =>
        place.Step is { } step ? $"{place.Slot}, {step.ToString().ToLowerInvariant()}" : place.Slot.ToString();
}
