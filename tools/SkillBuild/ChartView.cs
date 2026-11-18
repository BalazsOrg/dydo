using System.Text.Encodings.Web;
using System.Text.Json;

namespace DynaDocs.SkillBuild;

/// <summary>
/// A machine's <c>view.mjs</c> for the statechart skill: each state labelled with its title and spec
/// ids, each step with its arrow and spec ids, each gap <c>GAP: note</c>, <c>Done</c> hidden, and the
/// machine's title.
/// </summary>
internal static class ChartView
{
    private static readonly JsonSerializerOptions Literal = new() { Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping };

    public static string Module(Machine machine, IReadOnlyList<(string Id, Claim Claim)> transitions)
    {
        var states = machine.States.Select(state => (TlaModule.Value(state.Id), Label(state.Title, state.Spec)))
            .Concat(machine.Gaps.Select((gap, index) => (gap, index)).Where(pair => pair.gap.To is null)
                .Select(pair => (TlaModule.GapEnd(pair.index + 1), "GAP: unknown state")));
        var steps = transitions.Select(transition => (TlaModule.Action(transition.Id), Label(transition.Claim.Transition!.Arrow, transition.Claim.Spec)))
            .Concat(machine.Gaps.Select((gap, index) => (TlaModule.GapAction(index + 1), "GAP: " + gap.Note)));
        return string.Join('\n',
        [
            BuiltHeader.Script,
            .. Map("states", states),
            .. Map("steps", steps),
            $"export const title = {Js(machine.Title)};",
            "export const state = (v) => states[v.state];",
            "export const step = (action) => (action === \"Done\" ? null : steps[action]);"
        ]) + "\n";
    }

    /// <summary>A label with the spec ids it rests on: <c>title · K1, R2</c>.</summary>
    public static string Label(string text, IReadOnlyList<string> spec) => spec.Count == 0 ? text : $"{text} · {string.Join(", ", spec)}";

    private static IEnumerable<string> Map(string name, IEnumerable<(string Key, string Label)> entries)
    {
        var body = entries.Select(entry => $"  {Js(entry.Key)}: {Js(entry.Label)}").ToArray();
        return [$"const {name} = {{", .. body.Select((line, index) => index < body.Length - 1 ? line + "," : line), "};"];
    }

    private static string Js(string text) => JsonSerializer.Serialize(text, Literal);
}
