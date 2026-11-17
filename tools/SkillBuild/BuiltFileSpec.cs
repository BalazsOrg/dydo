namespace DynaDocs.SkillBuild;

/// <summary>One file to build: its carrier, its manifest entry and, for a spine, the resources it links.</summary>
internal sealed record BuiltFileSpec(string Carrier, CarrierFile File, IReadOnlyList<CarrierFile> Resources);
