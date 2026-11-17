namespace DynaDocs.SkillBuild;

/// <summary>claims/inventory/claims.json, as DYD-1080 renders it.</summary>
internal sealed record ClaimInventory(string Base, IReadOnlyList<Claim> Claims);
