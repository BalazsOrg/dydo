namespace DynaDocs.SkillBuild;

/// <summary>
/// A carrier the model builds: a role or workflow skill, or one of the two standards. Its name is
/// the kebab-case of its id, and <see cref="PathsOf"/> holds every destination rule (K6, K12).
/// A factory names its home; <c>with { … }</c> declares its slots.
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

    public IReadOnlyList<Slot> States { get; init; } = [];

    public IReadOnlyList<Slot> Routines { get; init; } = [];

    public IReadOnlyList<Slot> Sections { get; init; } = [];

    public IReadOnlyList<Resource> Resources { get; init; } = [];

    public IReadOnlyList<Slot> Appendices { get; init; } = [];

    internal string Name => Names.Kebab(Id);

    internal bool IsStandard => _home == SkillHome.Standard;

    internal string MainFile => IsStandard ? Name + ".md" : "SKILL.md";

    internal IReadOnlyList<string> Frontmatter => IsStandard ? _frontmatter : SkillFrontmatter();

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
