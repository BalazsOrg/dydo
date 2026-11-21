namespace DynaDocs.SkillBuild;

/// <summary>
/// The all-facts ledger of a truth-checked skill that replaces a legacy one (DYD-1295 2119cd65): one
/// <see cref="Fate"/> for every claim line of the <paramref name="Legacy"/> skill's spine and for every
/// resource it links, so no fact is lost without a reason on record. The build fails on a line or
/// resource with no entry, with two, or on an entry that names neither. The legacy skill's lines stay
/// in the model and are checked as before, but its files are no longer built: the replacing skill
/// takes its folder.
/// </summary>
public sealed record Ledger(Enum Legacy, params Fate[] Entries);
