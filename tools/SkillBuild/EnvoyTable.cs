namespace DynaDocs.SkillBuild;

/// <summary>
/// The envoy skill table the builder checks (H23): each job's fixed skill, and the closed-thread-signal
/// claim the envoy brief resource must state. <see cref="Check"/> and <see cref="CheckBuilt"/> run in
/// every build; <see cref="CheckSkillsExist"/> runs in <see cref="SkillNames"/>, against the repository root.
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
        if (!model.Skills.Any(skill => skill.Resources.Any(resource => Equals(resource.Id, Brief))))
            throw new InvalidDataException($"H23: no skill declares the envoy brief, {Names.Of(Brief)}");
    }

    /// <summary>
    /// The built envoy brief, not the objects this table holds, must carry both: a line mapped to the
    /// registered closed-thread-signal claim, and the job table exactly as <see cref="Render"/> gives it.
    /// </summary>
    internal void CheckBuilt(Model model, ClaimIndex claims, IReadOnlyDictionary<string, string> files, IEnumerable<MapLine> map)
    {
        var owner = model.Skills.First(skill => skill.Resources.Any(resource => Equals(resource.Id, Brief)));
        var briefs = owner.PathsOf(SkillRenderer.FileOf(Brief)).ToHashSet(StringComparer.Ordinal);
        var signal = claims.Claims.FirstOrDefault(claim => claim.Claim == CloseSignal).Id
            ?? throw new InvalidDataException("H23: the closed-thread signal is not a claim of the model");
        if (!map.Any(line => briefs.Contains(line.Path) && line.Claims.Contains(signal)))
            throw new InvalidDataException("H23: the envoy brief states no closed-thread signal");
        var table = Render(Jobs);
        if (!briefs.Any(path => files[path].Contains(table, StringComparison.Ordinal)))
            throw new InvalidDataException("H23: the envoy brief does not state the checked skill table");
    }

    /// <summary>Each job's skill is one of <paramref name="known"/>, the skills under <c>skills/</c> (<see cref="SkillNames"/>).</summary>
    internal void CheckSkillsExist(IReadOnlySet<string> known)
    {
        foreach (var row in Jobs.Where(row => !known.Contains(row.Skill)))
            throw new InvalidDataException($"H23: the envoy skill table gives {row.Job} `{row.Skill}`, which is not a skill under skills/");
    }
}
