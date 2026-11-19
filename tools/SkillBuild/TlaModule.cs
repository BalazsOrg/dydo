using System.Text.RegularExpressions;

namespace DynaDocs.SkillBuild;

/// <summary>
/// A machine as a TLA+ module and its TLC config (K1, K3): one <c>state</c> variable and the machine's
/// own, <c>Init</c>, one action per transition named after its claim, one per gap, a <c>Done</c>
/// stutter so final states do not deadlock, <c>Next</c> as their disjunction, and <c>Fairness</c> over
/// the fair moves in <c>Spec</c>. It holds no invariant or property: those are written by hand from
/// the axioms (K3).
/// </summary>
internal static class TlaModule
{
    /// <summary>A transition's action: its claim id with the dot TLA+ forbids as an underscore.</summary>
    public static string Action(string claimId) => claimId.Replace('.', '_');

    public static string GapAction(int number) => $"Gap{number}";

    /// <summary>The state an open gap leads to; a space keeps it apart from every enum-named state.</summary>
    public static string GapEnd(int number) => $"GAP {number}";

    public static string Value(Enum state) => state.ToString();

    public static string Module(Machine machine, IReadOnlyList<(string Id, Claim Claim)> transitions)
    {
        var extras = machine.Variables.Select(variable => variable.Name).ToArray();
        var lines = new List<string> { BuiltHeader.Tla,$"---- MODULE {machine.Id} ----" };
        if (machine.Extends.Count > 0) lines.Add("EXTENDS " + string.Join(", ", machine.Extends));
        lines.AddRange(["", $"VARIABLES {string.Join(", ", ["state", .. extras])}", "", $"vars == <<{string.Join(", ", ["state", .. extras])}>>", ""]);
        lines.AddRange([$"Finals == {Set(machine.Finals.Select(Value))}", ""]);
        var open = machine.Gaps.Select((gap, index) => (gap, index)).Where(pair => pair.gap.To is null).Select(pair => GapEnd(pair.index + 1)).ToArray();
        if (open.Length > 0) lines.AddRange([$"Open == {Set(open)}", ""]);
        lines.AddRange(["Init ==", $"    /\\ state = {Quote(Value(machine.Initial))}", .. machine.Variables.Select(variable => $"    /\\ {variable.Name} = {variable.Initial}"), ""]);
        foreach (var (id, claim) in transitions)
        {
            var move = claim.Transition!;
            lines.AddRange(Action($"claim:{id}", Action(id), Value(move.From), Value(move.To), move.Guard, move.Effect, extras));
        }
        for (var index = 0; index < machine.Gaps.Count; index++)
        {
            var gap = machine.Gaps[index];
            var to = gap.To is null ? GapEnd(index + 1) : Value(gap.To);
            lines.AddRange(Action("GAP: " + gap.Note, GapAction(index + 1), Value(gap.From), to, null, null, extras));
        }
        lines.AddRange([$"Done == state \\in Finals{(open.Length > 0 ? " \\cup Open" : "")} /\\ UNCHANGED vars", ""]);
        var actions = transitions.Select(transition => Action(transition.Id))
            .Concat(machine.Gaps.Select((_, index) => GapAction(index + 1))).Append("Done");
        lines.AddRange(["Next ==", .. actions.Select(action => "    \\/ " + action), ""]);
        if (machine.Variables.Count > 0)
            lines.AddRange(["Bounds == " + string.Join(" /\\ ", machine.Variables.Select(variable => variable.Bound)), ""]);
        lines.AddRange([Fairness(transitions), "", "Spec == Init /\\ [][Next]_vars /\\ Fairness", "===="]);
        return string.Join('\n', lines) + "\n";
    }

    /// <summary>The config: <c>SPECIFICATION Spec</c> when a move is fair, so TLC assumes its fairness; else <c>INIT</c> and <c>NEXT</c>.</summary>
    public static string Config(Machine machine, IReadOnlyList<(string Id, Claim Claim)> transitions) =>
        string.Join('\n',
        [
            BuiltHeader.Tla,
            .. transitions.Any(transition => transition.Claim.Transition!.Fair) ? ["SPECIFICATION Spec"] : (string[])["INIT Init", "NEXT Next"],
            .. machine.Variables.Count > 0 ? ["CONSTRAINT Bounds"] : Array.Empty<string>()
        ]) + "\n";

    private static string Fairness(IEnumerable<(string Id, Claim Claim)> transitions)
    {
        var fair = transitions.Where(transition => transition.Claim.Transition!.Fair).Select(transition => $"WF_vars({Action(transition.Id)})").ToArray();
        return "Fairness == " + (fair.Length == 0 ? "TRUE" : string.Join(" /\\ ", fair));
    }

    private static IEnumerable<string> Action(string comment, string name, string from, string to, string? guard, string? effect, string[] extras)
    {
        var unchanged = extras.Where(extra => effect is null || !Regex.IsMatch(effect, $@"\b{Regex.Escape(extra)}'", RegexOptions.None, TimeSpan.FromSeconds(1))).ToArray();
        return
        [
            @"\* " + comment,
            name + " ==",
            $"    /\\ state = {Quote(from)}",
            .. guard is null ? Array.Empty<string>() : ["    /\\ " + guard],
            $"    /\\ state' = {Quote(to)}",
            .. effect is null ? Array.Empty<string>() : ["    /\\ " + effect],
            .. unchanged.Length == 0 ? Array.Empty<string>() : [$"    /\\ UNCHANGED <<{string.Join(", ", unchanged)}>>"],
            ""
        ];
    }

    private static string Set(IEnumerable<string> values) => "{" + string.Join(", ", values.Select(Quote)) + "}";

    private static string Quote(string value) => $"\"{value}\"";
}
