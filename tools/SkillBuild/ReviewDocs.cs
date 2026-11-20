namespace DynaDocs.SkillBuild;

/// <summary>
/// The lists the build emits for review beside the why docs (DYD-1372 A4), under
/// <c>dydo/reference/claim-review/</c>, which never ships: each truth-checked skill's open claims, whose
/// lines it withholds, the legacy lines that name a command the CLI does not have, and every skill and
/// standard not yet migrated. A truth-checked skill fails the build on such a line; a legacy one is
/// listed here until its skill is switched over.
/// </summary>
internal static class ReviewDocs
{
    public const string Folder = "dydo/reference/claim-review";

    public const string LegacyCommands = Folder + "/legacy-commands.md";

    public const string NotYetMigrated = Folder + "/not-yet-migrated.md";

    /// <summary>A truth-checked skill's open claims, each with its rule, why it is open, and the line withheld.</summary>
    public static (string Path, string Text) Open(SkillSpec skill, IEnumerable<Placed> withheld, ClaimIndex claims)
    {
        var page = new Page(WhyDocs.Frontmatter, "Open claims: " + skill.Name);
        page.Block($"Claims of `{skill.Name}` that nobody has verified yet. The skill does not state them: each line below is withheld until its claim has evidence.");
        var open = withheld.ToArray();
        if (open.Length == 0) page.Block("None.");
        foreach (var placed in open)
        {
            var claim = claims.ById[placed.Owner];
            page.Heading("## " + placed.Owner);
            page.Block($"`claim:{placed.Owner}`: {claim.Rule}");
            page.Block("Open: " + claim.Truth!.Evidence.Text);
            page.Block($"Withheld from {placed.Line.Place.Slot}:");
            page.Block(WhyDocs.Quote(placed.Line.Text));
        }
        return ($"{Folder}/{skill.Name}-open.md", page.Done("").Text);
    }

    /// <summary>The list of legacy lines that name a missing command, by skill; nothing when no line does.</summary>
    public static IEnumerable<(string Path, string Text)> Commands(IEnumerable<(SkillSpec Skill, bool Built, IReadOnlyList<Placed> Lines)> legacy)
    {
        var page = new Page(WhyDocs.Frontmatter, "Legacy lines that name a missing command");
        page.Block("Lines of the skills and standards not yet truth-checked that name, in code, a `dydo …` command the CLI's command tree does not hold. " +
                   "A truth-checked skill fails the build on such a line; these are listed, not hidden, until their skill is switched over.");
        var any = false;
        foreach (var (skill, built, lines) in legacy)
        {
            var missing = lines.SelectMany(placed => CommandNames.Missing(placed.Line.Text).Select(problem => $"`claim:{placed.Owner}` names {problem}")).ToArray();
            if (missing.Length == 0) continue;
            any = true;
            page.Heading($"## {skill.Name}{(built ? "" : " (legacy, not built)")}");
            // One paragraph each, never a bullet: only a claim line is a list item in a built file (K23).
            foreach (var entry in missing) page.Block(entry);
        }
        return any ? [(LegacyCommands, page.Done("").Text)] : [];
    }

    /// <summary>
    /// Every skill and standard still built without the truth checks, named as not yet migrated (DYD-1372 F),
    /// so none reads as checked beside one that is. Nothing while no skill is truth-checked, or once all are.
    /// </summary>
    public static IEnumerable<(string Path, string Text)> Unmigrated(IEnumerable<(SkillSpec Skill, IReadOnlyList<Placed> Lines)> legacy, bool begun)
    {
        var rest = legacy.ToArray();
        if (!begun || rest.Length == 0) return [];
        var page = new Page(WhyDocs.Frontmatter, "Skills and standards not yet migrated");
        page.Block("These are not truth-checked: no line of theirs names its reader, and no claim carries its evidence, layer and reason on their account. " +
                   "Each builds as it did, unchanged, until it is switched over.");
        foreach (var (skill, lines) in rest)
        {
            var missing = lines.Count(placed => CommandNames.Missing(placed.Line.Text).Any());
            page.Block($"`{skill.Name}`: not yet migrated; {lines.Count} line{(lines.Count == 1 ? "" : "s")}, {missing} naming a command the CLI does not have.");
        }
        return [(NotYetMigrated, page.Done("").Text)];
    }
}
