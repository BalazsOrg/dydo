namespace DynaDocs.SkillBuild;

/// <summary>
/// A claim's answers to three of the four truth questions (DYD-1295 317be5f7), so it can be reviewed
/// alone; the fourth, who needs it, is each line's reader. <paramref name="Evidence"/>: what verified
/// it. <paramref name="Layer"/>: why no lower layer (the host, the CLI, the guard, Linear, CLIProxy)
/// already covers it. <paramref name="Reason"/>: one sentence on what it prevents or enables, in words
/// of its own. Every claim a truth-checked skill states carries one.
/// </summary>
public sealed record Truth(Evidence Evidence, string Layer, string Reason);
