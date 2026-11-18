namespace DynaDocs.SkillBuild;

/// <summary>A model's claims in declaration order, each with its id and every line that states it, its own and combined.</summary>
internal sealed class ClaimIndex
{
    private readonly Dictionary<Claim, List<(Line Line, string Owner)>> _stated = [];

    public ClaimIndex(Dictionary<Claim, string> ids)
    {
        Claims = [.. ids.Select(pair => (pair.Value, pair.Key))];
        foreach (var (id, claim) in Claims) _stated[claim] = [];
        foreach (var (id, claim) in Claims)
            foreach (var line in claim.Lines)
                foreach (var stated in (Claim[])[claim, .. line.Also()])
                    _stated[stated].Add((line, id));
    }

    public IReadOnlyList<(string Id, Claim Claim)> Claims { get; }

    /// <summary>The lines that state <paramref name="claim"/>, each with the id of the claim that holds it.</summary>
    public IReadOnlyList<(Line Line, string Owner)> LinesOf(Claim claim) => _stated[claim];
}
