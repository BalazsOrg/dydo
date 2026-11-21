namespace DynaDocs.SkillBuild;

/// <summary>
/// The lists the build emits for review beside the why docs (DYD-1372 A4), under
/// <c>dydo/reference/claim-review/</c>, which never ships: the open claims of each truth-checked skill, and of
/// each resource truth-checked ahead of its skill, whose lines it withholds, the legacy lines that name a command the CLI does not have or a resource no
/// longer built, and every skill and standard not yet migrated. A truth-checked skill fails the build
/// on such a command; a legacy line is listed here until its skill is switched over.
/// </summary>
internal static class ReviewDocs
{
    public const string Folder = "dydo/reference/claim-review";

    public const string LegacyCommands = Folder + "/legacy-commands.md";

    public const string NotYetMigrated = Folder + "/not-yet-migrated.md";

    public const string UnbuiltResources = Folder + "/unbuilt-resources.md";

    /// <summary>
    /// The open claims of <paramref name="of"/>, a truth-checked skill by its name or a resource truth-checked ahead of
    /// its skill as <c>skill/resource</c>, each with its rule, why it is open, and its line: withheld, or
    /// for the one placeholder stated with its token, and marked so. A skill written by hand in <paramref name="handWritten"/>
    /// (DYD-1439) is named as such: the lines are the model's own, and the build writes none of them.
    /// </summary>
    public static (string Path, string Text) Open(string of, string holder, IEnumerable<Placed> lines, ClaimIndex claims, string? handWritten = null)
    {
        var open = lines.Select(placed => (Placed: placed, claims.ById[placed.Owner].Rule, claims.ById[placed.Owner].Truth!.Evidence)).ToArray();
        var page = new Page(WhyDocs.Frontmatter, "Open claims: " + of);
        page.Block($"Claims of `{of}` that nobody has verified yet. The {holder} does not state them: each line below is withheld until its claim has evidence" +
                   (open.Any(claim => claim.Evidence.Token is not null) ? ", but for the one placeholder, whose line stands in the skill with its token unfilled." : ".") +
                   (handWritten is null ? "" : HandWrittenNote(handWritten)));
        if (open.Length == 0) page.Block("None.");
        foreach (var (placed, rule, evidence) in open)
        {
            page.Heading("## " + placed.Owner);
            page.Block($"`claim:{placed.Owner}`: {rule}");
            page.Block((evidence.Token is null ? "Open: " : "Placeholder: ") + evidence.Text);
            page.Block(evidence.Token is null
                ? $"Withheld from {placed.Line.Place.Slot}:"
                : $"Stated in {placed.Line.Place.Slot}, with `{evidence.Token}` where its text will stand:");
            page.Block(WhyDocs.Quote(placed.Line.Text));
        }
        return ($"{Folder}/{of.Replace('/', '-')}-open.md", page.Done("").Text);
    }

    /// <summary>The sentence an open list of a hand-written skill adds to its lead (DYD-1439).</summary>
    internal static string HandWrittenNote(string folder) =>
        $" The skill is hand-written, in `{folder}`: the lines below are the model's own, and the build writes none of them.";

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
    /// The model's lines, of skills not yet migrated, that name a resource of a <paramref name="replaced"/> legacy skill which
    /// the skill replacing it does not build (DYD-1372 F): a line names one when it holds the skill's name and the
    /// resource's, in code or as its file. Listed, never failed, since such a line may not be changed yet; nothing
    /// when no line does.
    /// </summary>
    public static IEnumerable<(string Path, string Text)> Unbuilt(
        IEnumerable<(SkillSpec Skill, IReadOnlyList<Placed> Lines)> legacy, IEnumerable<(SkillSpec Legacy, SkillSpec By)> replaced)
    {
        var gone = replaced.SelectMany(pair => pair.Legacy.Resources.Select(resource => SkillRenderer.FileOf(resource.Id))
            .Except(pair.By.Resources.Select(resource => SkillRenderer.FileOf(resource.Id)))
            .Select(file => (Skill: pair.By.Name, File: file, Name: System.IO.Path.GetFileNameWithoutExtension(file)))).ToArray();
        var page = new Page(WhyDocs.Frontmatter, "Lines that name a resource no longer built");
        page.Block("Lines of the model's skills and standards not yet truth-checked that name a resource of a legacy skill which the skill that replaced it does not build. " +
                   "A hand-written file is not read here. Each sends its reader to a file that is not there; these are listed, not hidden, until their skill is switched over or the resource is rebuilt.");
        var any = false;
        foreach (var (skill, lines) in legacy)
        {
            var naming = lines.SelectMany(placed => placed.Line.Text.Split('\n').SelectMany(text => gone
                .Where(resource => text.Contains(resource.Skill, StringComparison.Ordinal)
                                   && (text.Contains($"`{resource.Name}`", StringComparison.Ordinal) || text.Contains(resource.File, StringComparison.Ordinal)))
                .Select(resource => (placed.Owner, resource, text)))).ToArray();
            if (naming.Length == 0) continue;
            any = true;
            page.Heading("## " + skill.Name);
            foreach (var (owner, resource, text) in naming)
            {
                page.Block($"`claim:{owner}` names `{resource.File}` of `{resource.Skill}`, which the skill that replaced the legacy `{resource.Skill}` does not build:");
                page.Block(WhyDocs.Quote(text));
            }
        }
        return any ? [(UnbuiltResources, page.Done("").Text)] : [];
    }

    /// <summary>
    /// Every skill and standard still built without the truth checks, named as not yet migrated (DYD-1372 F),
    /// so none reads as checked beside one that is. Nothing while no skill or resource is truth-checked, or once all skills are.
    /// A resource truth-checked ahead of its skill is named on that skill's line, and its lines are not counted.
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
            Placed[] counted = [.. lines.Where(placed => skill.ReadersOf(placed.Line).Count == 0)];
            var missing = counted.Count(placed => CommandNames.Missing(placed.Line.Text).Any());
            var ahead = string.Concat(skill.Ahead.Select(resource => $" Its resource `{Names.Kebab(resource.Id)}` is truth-checked ahead of it: those lines name their reader, and are not counted here."));
            page.Block($"`{skill.Name}`: not yet migrated; {counted.Length} line{(counted.Length == 1 ? "" : "s")}, {missing} naming a command the CLI does not have.{ahead}");
        }
        return [(NotYetMigrated, page.Done("").Text)];
    }
}
