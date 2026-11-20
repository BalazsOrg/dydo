namespace DynaDocs.SkillBuild;

/// <summary>
/// The job skill table the builder checks (H23): each job's fixed skill, a Question's keyed on its stage
/// label, a council's resource of that skill, and the closed-thread-signal claim the envoy brief resource
/// must state. <see cref="Check"/> and <see cref="CheckBuilt"/> run in every build;
/// <see cref="CheckSkillsExist"/> runs in <see cref="SkillNames"/>, against the repository root.
/// </summary>
public sealed record EnvoyTable(IReadOnlyList<EnvoyJobSkill> Jobs, Claim CloseSignal, Enum Brief)
{
    // H23: "Grilling → grilling, Walkthrough → walkthrough, Wayfinding → wayfinder, Question at stage research →
    // /ticket-captain with research crew, Question at stage council → the council-chair resource, Question at stage
    // his-call → co-thinker, Enablement → wizard; the table keys on Type, and for a Question on its stage label".
    private static readonly Dictionary<(EnvoyJob Job, QuestionStage? Stage), (string Skill, Enum? Resource)> Fixed = new()
    {
        [(EnvoyJob.Grilling, null)] = ("grilling", null),
        [(EnvoyJob.Walkthrough, null)] = ("walkthrough", null),
        [(EnvoyJob.Wayfinding, null)] = ("wayfinder", null),
        [(EnvoyJob.Question, QuestionStage.Research)] = ("ticket-captain", null),
        [(EnvoyJob.Question, QuestionStage.Council)] = ("ticket-captain", TicketCaptainResource.CouncilChair),
        [(EnvoyJob.Question, QuestionStage.HisCall)] = ("co-thinker", null),
        [(EnvoyJob.Enablement, null)] = ("wizard", null)
    };

    /// <summary>The table as the skill states it: one row per job, the skill in code with its slash (K14), its resource, then its note.</summary>
    public static string Render(IEnumerable<EnvoyJobSkill> jobs) => string.Join("\n",
    [
        "| Job | Skill |",
        "|---|---|",
        .. jobs.Select(row => $"| {row.Key} | `/{row.Skill}`{(row.Resource is null ? "" : $", with its `{Names.Kebab(row.Resource)}` resource")}{(row.Note is null ? "" : "; " + row.Note)} |")
    ]);

    internal void Check(Model model)
    {
        foreach (var ((job, stage), (skill, resource)) in Fixed)
        {
            var key = EnvoyJobSkill.KeyOf(job, stage);
            var rows = Jobs.Where(row => row.Job == job && row.Stage == stage).ToArray();
            if (rows.Length != 1)
                throw new InvalidDataException($"H23: the envoy skill table needs exactly one {key} row, not {rows.Length}");
            if (rows[0].Skill != skill)
                throw new InvalidDataException($"H23: the envoy skill table gives {key} `{rows[0].Skill}`, not `{skill}`");
            if (!Equals(rows[0].Resource, resource))
                throw new InvalidDataException($"H23: the envoy skill table gives {key} {ResourceOf(rows[0].Resource)}, not {ResourceOf(resource)}");
        }
        foreach (var row in Jobs.Where(row => !Fixed.ContainsKey((row.Job, row.Stage))))
            throw new InvalidDataException($"H23: the envoy skill table has a {row.Key} row, which H23 does not name");
        foreach (var row in Jobs.Where(row => row.Resource is not null))
            if (!model.Skills.Any(skill => skill.Name == row.Skill && skill.Resources.Any(resource => Equals(resource.Id, row.Resource))))
                throw new InvalidDataException($"H23: the envoy skill table gives {row.Key} {ResourceOf(row.Resource)}, which `{row.Skill}` does not declare");
        if (!model.Skills.Any(skill => skill.Resources.Any(resource => Equals(resource.Id, Brief))))
            throw new InvalidDataException($"H23: no skill declares the envoy brief, {Names.Of(Brief)}");
    }

    private static string ResourceOf(Enum? resource) => resource is null ? "no resource" : $"the `{Names.Kebab(resource)}` resource";

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
            throw new InvalidDataException($"H23: the envoy skill table gives {row.Key} `{row.Skill}`, which is not a skill under skills/");
    }
}
