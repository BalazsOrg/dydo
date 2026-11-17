namespace DynaDocs.SkillBuild;

/// <summary>A claim in claims.json, with its prose keyed by carrier.</summary>
internal sealed record Claim(string Id, string Kind, IReadOnlyDictionary<string, IReadOnlyList<ProseEntry>> Prose);
