namespace DynaDocs.SkillBuild;

/// <summary>
/// A carrier the model builds: a role or workflow skill, or one of the two standards. Its name is
/// the kebab-case of its id, and <see cref="PathsOf"/> holds every destination rule (K6, K12).
/// A factory names its home; <c>with { … }</c> declares its slots. A skill's spine is its job
/// paragraph, its numbered <see cref="Phases"/>, then "When things go wrong" linking its
/// <see cref="Scenarios"/> (K9); a standard holds <see cref="Sections"/> only. Two skills may share a
/// name, and so a folder, only when one replaces the other through its <see cref="Ledger"/>.
/// </summary>
public sealed record SkillSpec
{
    private readonly SkillHome _home;
    private readonly string _folder;
    private readonly string? _description;
    private readonly IReadOnlyList<string> _frontmatter;

    private SkillSpec(Enum id, SkillHome home, string title, string? description, string folder, IReadOnlyList<string> frontmatter)
    {
        Id = id;
        Title = title;
        _home = home;
        _description = description;
        _folder = folder;
        _frontmatter = frontmatter;
    }

    public static SkillSpec Officer(Enum id, string title, string description) => new(id, SkillHome.Officers, title, description, "", []);

    public static SkillSpec Crew(Enum id, string title, string description) => new(id, SkillHome.Crew, title, description, "", []);

    public static SkillSpec Workflow(Enum id, string title, string description) => new(id, SkillHome.Productivity, title, description, "", []);

    /// <summary>A standard, built to <c>dydo/&lt;folder&gt;/</c> and <c>Scaffold/dydo/&lt;folder&gt;/</c>; it holds sections only.</summary>
    public static SkillSpec Standard(Enum id, string folder, string title, params string[] frontmatter) =>
        new(id, SkillHome.Standard, title, null, folder, frontmatter);

    public Enum Id { get; }

    public string Title { get; }

    /// <summary>Explicit-only (DR 045 §9): <c>disable-model-invocation: true</c> and Codex's implicit-invocation policy off.</summary>
    public bool ExplicitOnly { get; init; }

    public OpenAiYaml? OpenAi { get; init; }

    public IReadOnlyList<Phase> Phases { get; init; } = [];

    /// <summary>One resource per scenario, each under 100 lines, linked from "When things go wrong" (K9).</summary>
    public IReadOnlyList<Resource> Scenarios { get; init; } = [];

    public IReadOnlyList<Slot> Sections { get; init; } = [];

    /// <summary>
    /// The actors this skill belongs to. Naming them makes the skill truth-checked (DYD-1295 317be5f7):
    /// each of its lines names one of them as its reader, each claim it states carries its
    /// <see cref="Truth"/>, each slot declares its shape, and every command it names exists.
    /// </summary>
    public IReadOnlyList<Enum> Readers { get; init; } = [];

    /// <summary>The legacy skill this one replaces, each of its spine's lines accounted for.</summary>
    public Ledger? Ledger { get; init; }

    internal bool TruthChecked => Readers.Count > 0;

    /// <summary>The resources truth-checked ahead of this skill (<see cref="Resource.Readers"/>); none once the skill itself is.</summary>
    internal IEnumerable<Resource> Ahead => TruthChecked ? [] : Resources.Where(resource => resource.Readers.Count > 0);

    /// <summary>Whether <paramref name="resource"/> is truth-checked: with its skill, or ahead of it.</summary>
    internal bool Checks(Resource resource) => TruthChecked || resource.Readers.Count > 0;

    /// <summary>The resource truth-checked ahead of this skill whose slot holds <paramref name="line"/>, or none.</summary>
    internal Resource? AheadOf(Line line) =>
        line.Place is { } place ? Ahead.FirstOrDefault(resource => resource.Slots.Any(slot => Equals(slot.Id, place.Slot))) : null;

    /// <summary>The readers <paramref name="line"/> answers to: this skill's, or its resource's; none for a legacy line.</summary>
    internal IReadOnlyList<Enum> ReadersOf(Line line) => TruthChecked ? Readers : AheadOf(line)?.Readers ?? [];

    internal string Name => Names.Kebab(Id);

    internal bool IsStandard => _home == SkillHome.Standard;

    internal string MainFile => IsStandard ? Name + ".md" : "SKILL.md";

    internal IReadOnlyList<string> Frontmatter => IsStandard ? _frontmatter : SkillFrontmatter();

    /// <summary>Every resource the skill builds: each phase's, then each scenario.</summary>
    internal IEnumerable<Resource> Resources => Phases.SelectMany(phase => phase.Resources).Concat(Scenarios);

    internal IEnumerable<Slot> StateSlots => Phases.SelectMany(phase => phase.Slots).Concat(Resources.SelectMany(resource => resource.Slots))
        .Where(slot => slot.IsState);

    private List<string> SkillFrontmatter()
    {
        List<string> lines = [$"name: {Name}", $"description: {YamlScalar.Of(_description!)}"];
        if (ExplicitOnly) lines.Add("disable-model-invocation: true");
        return lines;
    }

    /// <summary>Where a file of this carrier, named relative to its folder, lands in the repository.</summary>
    internal IReadOnlyList<string> PathsOf(string local) => _home switch
    {
        SkillHome.Officers => [$"skills/roles/officers/{Name}/{local}"],
        SkillHome.Crew => [$"skills/roles/crew/{Name}/{local}"],
        SkillHome.Productivity => [$"skills/productivity/{Name}/{local}"],
        _ => [$"dydo/{_folder}/{local}", $"Scaffold/dydo/{_folder}/{local}"]
    };
}
