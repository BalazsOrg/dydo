namespace DynaDocs.SkillBuild;

/// <summary>One copy of a claim's prose in claims.json: its text, the source line and the unit it came from.</summary>
internal sealed record ProseEntry(string Text, string Source, string Unit);
