namespace DynaDocs.SkillBuild;

/// <summary>One row of the envoy skill table: a job, the skill its envoy loads, and an optional note after the skill.</summary>
public sealed record EnvoyJobSkill(EnvoyJob Job, string Skill, string? Note = null);
