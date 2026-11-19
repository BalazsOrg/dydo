using System.Reflection;

namespace DynaDocs.SkillBuild;

/// <summary>
/// Reads a model's kind classes: each <c>public static</c> <see cref="Claim"/> field is a claim whose id
/// is <c>Kind.Field</c>. Hands every line, with the ids it states, to the skill it names, and keeps every
/// declared id, so a claim no line states is still seen.
/// </summary>
internal static class ModelIndex
{
    public static (Dictionary<SkillSpec, List<Placed>> Lines, IReadOnlyCollection<string> Claims) Read(Model model)
    {
        var skills = new Dictionary<Enum, SkillSpec>();
        foreach (var skill in model.Skills)
            if (!skills.TryAdd(skill.Id, skill))
                throw new InvalidDataException($"skill {Names.Of(skill.Id)} is declared twice");
        var ids = ClaimIds(model.Kinds);
        var lines = model.Skills.ToDictionary(skill => skill, _ => new List<Placed>());
        foreach (var (claim, id) in ids)
            foreach (var line in claim.Lines)
            {
                if (!skills.TryGetValue(line.Skill, out var skill))
                    throw new InvalidDataException($"claim:{id} has a line for {Names.Of(line.Skill)}, which the model does not declare");
                lines[skill].Add(new Placed(line, [id, .. line.Also().Select(other => IdOf(ids, other, id))]));
            }
        return (lines, ids.Values);
    }

    public static Dictionary<Claim, string> ClaimIds(IEnumerable<Type> kinds)
    {
        var ids = new Dictionary<Claim, string>();
        var taken = new HashSet<string>(StringComparer.Ordinal);
        foreach (var kind in kinds)
        {
            if (!kind.IsClass || !kind.IsAbstract || !kind.IsSealed)
                throw new InvalidDataException($"kind {kind.Name} is not a static class");
            foreach (var field in kind.GetFields(BindingFlags.Public | BindingFlags.Static).Where(field => field.FieldType == typeof(Claim)))
            {
                var id = $"{kind.Name}.{field.Name}";
                if (!taken.Add(id) || !ids.TryAdd((Claim)field.GetValue(null)!, id))
                    throw new InvalidDataException($"claim:{id} is declared twice; each claim is one field of one kind");
            }
        }
        return ids;
    }

    private static string IdOf(Dictionary<Claim, string> ids, Claim claim, string naming) =>
        ids.TryGetValue(claim, out var id)
            ? id
            : throw new InvalidDataException($"claim:{naming} has a combined line naming a claim outside the model's kinds");
}
