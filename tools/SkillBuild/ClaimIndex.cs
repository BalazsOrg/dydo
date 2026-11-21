namespace DynaDocs.SkillBuild;

/// <summary>A model's claims in declaration order, each with its id and every line that states it, its own and combined.</summary>
internal sealed class ClaimIndex
{
    private readonly Dictionary<Claim, string> _ids;
    private readonly Dictionary<Claim, List<(Line Line, string Owner)>> _stated = [];

    public ClaimIndex(Dictionary<Claim, string> ids)
    {
        _ids = ids;
        Claims = [.. ids.Select(pair => (pair.Value, pair.Key))];
        ById = Claims.ToDictionary(claim => claim.Id, claim => claim.Claim, StringComparer.Ordinal);
        foreach (var (id, claim) in Claims) _stated[claim] = [];
        foreach (var (id, claim) in Claims)
            foreach (var line in claim.Lines)
                foreach (var stated in (Claim[])[claim, .. line.Also()])
                    _stated[stated].Add((line, id));
    }

    public IReadOnlyList<(string Id, Claim Claim)> Claims { get; }

    public IReadOnlyDictionary<string, Claim> ById { get; }

    /// <summary>The lines that state <paramref name="claim"/>, each with the id of the claim that holds it.</summary>
    public IReadOnlyList<(Line Line, string Owner)> LinesOf(Claim claim) => _stated[claim];

    /// <summary>A claim's id, or none for a claim outside the model's kinds.</summary>
    public string? IdOf(Claim claim) => _ids.GetValueOrDefault(claim);
}
