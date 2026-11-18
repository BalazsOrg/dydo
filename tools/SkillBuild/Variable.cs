namespace DynaDocs.SkillBuild;

/// <summary>An extra machine variable as TLA+ text: its name, its initial value, and the bound that keeps TLC finite.</summary>
public sealed record Variable(string Name, string Initial, string Bound);
