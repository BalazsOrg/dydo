namespace DynaDocs.SkillBuild;

/// <summary>
/// One row of the job skill table: a job, a Question's stage when the job is a Question (H23), the skill its
/// captain loads, a resource of that skill it works by, and an optional note after the skill.
/// </summary>
public sealed record EnvoyJobSkill(EnvoyJob Job, string Skill, string? Note = null)
{
    public QuestionStage? Stage { get; init; }

    public Enum? Resource { get; init; }

    internal string Key => KeyOf(Job, Stage);

    internal static string KeyOf(EnvoyJob job, QuestionStage? stage) => stage is { } named ? $"{job} at stage `{Names.Kebab(named)}`" : job.ToString();
}
