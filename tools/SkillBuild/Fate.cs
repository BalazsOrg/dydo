namespace DynaDocs.SkillBuild;

/// <summary>
/// One <see cref="Ledger"/> entry: what became of one line of a legacy spine, named by the claim that
/// holds the line, or of one legacy resource, named by its id. Kept: the new claims that state it, or
/// the new resource that replaces it. Dropped: the truth question it fails, and why in one line.
/// Deferred: why it is not rebuilt yet.
/// </summary>
public sealed record Fate
{
    private Fate(FateKind kind, Claim? line, Enum? resource)
    {
        Kind = kind;
        Line = line;
        Resource = resource;
    }

    public FateKind Kind { get; }

    public Claim? Line { get; }

    public Enum? Resource { get; }

    public IReadOnlyList<Claim> By { get; private init; } = [];

    public Enum? ByResource { get; private init; }

    public Question? Fails { get; private init; }

    public string? Why { get; private init; }

    public static Fate Kept(Claim line, params Claim[] by) => new(FateKind.Kept, line, null) { By = by };

    public static Fate Kept(Enum resource, Enum by) => new(FateKind.Kept, null, resource) { ByResource = by };

    public static Fate Dropped(Claim line, Question fails, string why) => new(FateKind.Dropped, line, null) { Fails = fails, Why = why };

    public static Fate Dropped(Enum resource, Question fails, string why) => new(FateKind.Dropped, null, resource) { Fails = fails, Why = why };

    public static Fate Deferred(Claim line, string why) => new(FateKind.Deferred, line, null) { Why = why };

    public static Fate Deferred(Enum resource, string why) => new(FateKind.Deferred, null, resource) { Why = why };
}
