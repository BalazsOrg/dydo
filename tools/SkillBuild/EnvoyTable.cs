namespace DynaDocs.SkillBuild;

/// <summary>
/// The envoy skill table the builder checks (H23): each job's fixed skill, and the closed-thread-signal
/// claim the envoy brief resource must state. <see cref="Check"/> runs in every build;
/// <see cref="CheckSkillsExist"/> runs against the repository root the tool builds into.
/// </summary>
public sealed record EnvoyTable(IReadOnlyList<EnvoyJobSkill> Jobs, Claim CloseSignal, Enum Brief)
{
    // H23: "Grilling → grilling, Walkthrough → walkthrough, Wayfinding → wayfinder, Question → co-thinker, Enablement → wizard".
    private static readonly Dictionary<EnvoyJob, string> Fixed = new()
    {
        [EnvoyJob.Grilling] = "grilling",
        [EnvoyJob.Walkthrough] = "walkthrough",
        [EnvoyJob.Wayfinding] = "wayfinder",
        [EnvoyJob.Question] = "co-thinker",
        [EnvoyJob.Enablement] = "wizard"
    };

    /// <summary>The table as the skill states it: one row per job, the skill in code, then its note.</summary>
    public static string Render(IEnumerable<EnvoyJobSkill> jobs) => string.Join("\n",
    [
        "| Job | Skill |",
        "|---|---|",
        .. jobs.Select(row => $"| {row.Job} | `{row.Skill}`{(row.Note is null ? "" : "; " + row.Note)} |")
    ]);

    internal void Check(Model model)
    {
        foreach (var (job, skill) in Fixed)
        {
            var rows = Jobs.Where(row => row.Job == job).ToArray();
            if (rows.Length != 1)
                throw new InvalidDataException($"H23: the envoy skill table needs exactly one {job} row, not {rows.Length}");
            if (rows[0].Skill != skill)
                throw new InvalidDataException($"H23: the envoy skill table gives {job} `{rows[0].Skill}`, not `{skill}`");
        }
        var brief = model.Skills.SelectMany(skill => skill.Resources).FirstOrDefault(resource => Equals(resource.Id, Brief))
            ?? throw new InvalidDataException($"H23: no skill declares the envoy brief, {Names.Of(Brief)}");
        var sections = brief.Sections.Select(section => section.Id).ToHashSet();
        if (!CloseSignal.Lines.Any(line => sections.Contains(line.Place.Slot)))
            throw new InvalidDataException("H23: the envoy brief states no closed-thread signal");
    }

    internal void CheckSkillsExist(string root)
    {
        var skills = Path.Combine(root, "skills");
        var present = Directory.Exists(skills)
            ? Directory.EnumerateFiles(skills, "SKILL.md", SearchOption.AllDirectories).Select(file => Path.GetFileName(Path.GetDirectoryName(file))).ToHashSet()
            : [];
        foreach (var row in Jobs.Where(row => !present.Contains(row.Skill)))
            throw new InvalidDataException($"H23: the envoy skill table gives {row.Job} `{row.Skill}`, which is not a skill under skills/");
    }
}
