using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;

namespace DynaDocs.SkillBuild;

/// <summary>
/// A machine's <c>transitions.json</c> (K1), one per machine so each stays under T15's limit: its
/// states, initial and final states, variables, transitions (claim id, arrow, spec ids, guard, effect
/// and the skills that carry it) and gaps. JSON has no comments, so the built header is its first
/// member, <c>$comment</c>.
/// </summary>
internal static class TransitionsJson
{
    private static readonly JsonWriterOptions Options = new()
    {
        Indented = true,
        NewLine = "\n",
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
    };

    public static string Render(Machine machine, IReadOnlyList<(string Id, Claim Claim)> transitions, ClaimIndex claims)
    {
        using var stream = new MemoryStream();
        using (var json = new Utf8JsonWriter(stream, Options))
            Machine(json, machine, transitions, claims);
        return Encoding.UTF8.GetString(stream.ToArray()) + "\n";
    }

    private static void Machine(Utf8JsonWriter json, Machine machine, IReadOnlyList<(string Id, Claim Claim)> transitions, ClaimIndex claims)
    {
        json.WriteStartObject();
        json.WriteString("$comment", BuiltHeader.Text);
        json.WriteString("id", machine.Id.ToString());
        json.WriteString("title", machine.Title);
        json.WriteString("initial", TlaModule.Value(machine.Initial));
        Strings(json, "finals", machine.Finals.Select(TlaModule.Value));
        Array(json, "states", machine.States, state =>
        {
            json.WriteString("id", TlaModule.Value(state.Id));
            json.WriteString("title", state.Title);
            Strings(json, "spec", state.Spec);
        });
        Array(json, "variables", machine.Variables, variable =>
        {
            json.WriteString("name", variable.Name);
            json.WriteString("initial", variable.Initial);
            json.WriteString("bound", variable.Bound);
        });
        Array(json, "transitions", transitions, transition =>
        {
            var move = transition.Claim.Transition!;
            json.WriteString("claim", "claim:" + transition.Id);
            json.WriteString("from", TlaModule.Value(move.From));
            json.WriteString("to", TlaModule.Value(move.To));
            json.WriteString("arrow", move.Arrow);
            Strings(json, "spec", transition.Claim.Spec);
            json.WriteString("guard", move.Guard);
            json.WriteString("effect", move.Effect);
            json.WriteBoolean("fair", move.Fair);
            Strings(json, "skills", claims.LinesOf(transition.Claim).Select(line => Names.Kebab(line.Line.Skill)).Distinct().Order(StringComparer.Ordinal));
        });
        Array(json, "gaps", machine.Gaps.Select((gap, index) => (gap, index)).ToArray(), pair =>
        {
            json.WriteString("action", TlaModule.GapAction(pair.index + 1));
            json.WriteString("from", TlaModule.Value(pair.gap.From));
            json.WriteString("to", pair.gap.To is null ? null : TlaModule.Value(pair.gap.To));
            json.WriteString("note", pair.gap.Note);
        });
        json.WriteEndObject();
    }

    private static void Array<T>(Utf8JsonWriter json, string name, IEnumerable<T> items, Action<T> write)
    {
        json.WriteStartArray(name);
        foreach (var item in items)
        {
            json.WriteStartObject();
            write(item);
            json.WriteEndObject();
        }
        json.WriteEndArray();
    }

    private static void Strings(Utf8JsonWriter json, string name, IEnumerable<string> values)
    {
        json.WriteStartArray(name);
        foreach (var value in values) json.WriteStringValue(value);
        json.WriteEndArray();
    }
}
