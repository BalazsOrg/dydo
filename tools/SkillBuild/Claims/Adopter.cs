namespace DynaDocs.SkillBuild;

/// <summary>The adopter update (the Adopter machine): one ordinary update, the user consulted on tailored files, 4.0.0 as a major release, line-stable builds after.</summary>
public static class Adopter
{
    public static readonly Claim Update = Move(AdopterState.Released, AdopterState.Updating, Actor.UpdateDydo,
        "one ordinary update with an Upgrade note, through the update-dydo wizard", "K18", "N8");
    public static readonly Claim Tailored = Move(AdopterState.Updating, AdopterState.Tailored, Actor.UpdateDydo,
        "a shipped file the project keeps tailored, docs included", "N11");
    public static readonly Claim Consult = Move(AdopterState.Tailored, AdopterState.Consulted, Actor.UpdateDydo,
        "update-dydo consults the user", "N11");
    public static readonly Claim Decided = Move(AdopterState.Consulted, AdopterState.Updating, Actor.Balazs,
        "the user decides what happens to it", "N11");
    public static readonly Claim Major = Move(AdopterState.Updating, AdopterState.Major, Actor.UpdateDydo,
        "4.0.0: statuses deleted, DYTEST a prerequisite, the rebuilt skills swapped in", "N1", "N9", "Y5");
    public static readonly Claim Swapped = Move(AdopterState.Major, AdopterState.Current, Actor.UpdateDydo,
        "dydo's law and glossary read-only, the two-law template, descriptive-doc rules on by default", "N8", "L2", "D9", "D1");
    public static readonly Claim Done = Move(AdopterState.Updating, AdopterState.Current, Actor.UpdateDydo,
        "updated; later builds stay line-stable, one claim per bullet", "K18");
    public static readonly Claim Harvest = Move(AdopterState.Current, AdopterState.Harvested, Actor.Coordinator,
        "a release is planned: its map holder harvests their Agent friction queues and local patches", "H9");

    private static Claim Move(AdopterState from, AdopterState to, Actor actor, string arrow, params string[] spec) =>
        new($"An adopter's update: {arrow}.", [actor], []) { Spec = spec, Transition = new(from, to, arrow) };
}
