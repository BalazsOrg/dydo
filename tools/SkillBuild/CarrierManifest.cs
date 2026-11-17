namespace DynaDocs.SkillBuild;

/// <summary>claims/carriers.json: the hand-written titles and frontmatter, copied from <see cref="Base"/>.</summary>
internal sealed record CarrierManifest(string Base, IReadOnlyList<CarrierEntry> Carriers);
