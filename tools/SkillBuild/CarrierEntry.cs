namespace DynaDocs.SkillBuild;

/// <summary>One carrier in claims/carriers.json: a skill or system doc and the files built for it.</summary>
internal sealed record CarrierEntry(string Carrier, IReadOnlyList<CarrierFile> Files);
