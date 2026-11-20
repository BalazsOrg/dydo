namespace DynaDocs.SkillBuild;

/// <summary>
/// A model's machines and the transition claims on them, checked: every state belongs to exactly one
/// machine, a transition and a gap stay inside theirs, a transition's lines are its moves, a skill's
/// states are machine states, and the spec citations hold (K1, K4, K9).
/// </summary>
internal sealed class Statechart
{
    private readonly Dictionary<Enum, Machine> _machineOf = [];
    private readonly Dictionary<Machine, List<(string Id, Claim Claim)>> _transitions = [];

    public Statechart(Model model, ClaimIndex claims)
    {
        foreach (var machine in model.Machines) Declare(machine, model.CitesSpec);
        var standards = model.Skills.Where(skill => skill.IsStandard).Select(skill => skill.Id).ToHashSet();
        foreach (var (id, claim) in claims.Claims)
        {
            Cites($"claim:{id}", claim.Spec, model.CitesSpec);
            if (claim.Transition is { } transition) Move(id, claim, transition, claims.LinesOf(claim), standards);
        }
        if (model.Machines.Count > 0)
            foreach (var skill in model.Skills) StatesAreMachineStates(skill);
    }

    public IReadOnlyList<Machine> Machines => [.. _transitions.Keys];

    /// <summary>A machine's transition claims, by claim id.</summary>
    public IReadOnlyList<(string Id, Claim Claim)> TransitionsOf(Machine machine) =>
        [.. _transitions[machine].OrderBy(transition => transition.Id, StringComparer.Ordinal)];

    public Machine MachineOf(Enum state) => _machineOf[state];

    public static string FolderOf(Machine machine) => $"claims/statechart/{Names.Kebab(machine.Id)}";

    /// <summary>Each machine's <c>transitions.json</c>, module, config and view; nothing for a model with no machines.</summary>
    public IEnumerable<(string Path, string Text)> Files(ClaimIndex claims)
    {
        foreach (var machine in Machines)
        {
            var transitions = TransitionsOf(machine);
            yield return ($"{FolderOf(machine)}/transitions.json", TransitionsJson.Render(machine, transitions, claims));
            yield return ($"{FolderOf(machine)}/{machine.Id}.tla", TlaModule.Module(machine, transitions));
            yield return ($"{FolderOf(machine)}/{machine.Id}.cfg", TlaModule.Config(machine, transitions));
            yield return ($"{FolderOf(machine)}/view.mjs", ChartView.Module(machine, transitions));
        }
    }

    private void Declare(Machine machine, bool citesSpec)
    {
        if (_transitions.Keys.Any(other => Equals(other.Id, machine.Id)))
            throw new InvalidDataException($"machine {Names.Of(machine.Id)} is declared twice");
        _transitions[machine] = [];
        foreach (var state in machine.States)
        {
            if (!_machineOf.TryAdd(state.Id, machine))
                throw new InvalidDataException($"state {Names.Of(state.Id)} is declared twice; a state belongs to exactly one machine");
            Cites($"state {Names.Of(state.Id)}", state.Spec, citesSpec);
        }
        foreach (var end in (Enum?[])[machine.Initial, .. machine.Finals, .. machine.Gaps.SelectMany(gap => (Enum?[])[gap.From, gap.To])])
            if (end is not null && _machineOf.GetValueOrDefault(end) != machine)
                throw new InvalidDataException($"machine {Names.Of(machine.Id)} names {Names.Of(end)}, which is not one of its states");
    }

    private static void Cites(string what, IReadOnlyList<string> spec, bool required)
    {
        if (required && spec.Count == 0)
            throw new InvalidDataException($"{what} cites no spec rule; a production claim or state cites at least one");
        foreach (var id in spec.Where(id => !SpecCitation.IsValid(id)))
            throw new InvalidDataException($"{what} cites \"{id}\", which is not a spec rule id");
    }

    // A standard has sections, no states, so its line on a transition claim states the rule in a section (DYD-1082 ruling 1).
    private void Move(string id, Claim claim, Transition transition, IEnumerable<(Line Line, string Owner)> lines, HashSet<Enum> standards)
    {
        var machine = _machineOf.GetValueOrDefault(transition.From)
            ?? throw new InvalidDataException($"claim:{id} moves from {Names.Of(transition.From)}, which is no machine's state");
        if (_machineOf.GetValueOrDefault(transition.To) != machine)
            throw new InvalidDataException(
                $"claim:{id} moves to {Names.Of(transition.To)}, which is not a state of {Names.Of(machine.Id)}; a transition stays inside its machine");
        foreach (var (line, _) in lines.Where(stated => !standards.Contains(stated.Line.Skill)))
            if (!IsMove(line.Place, transition))
                throw new InvalidDataException(
                    $"claim:{id} moves {Names.Of(transition.From)} to {Names.Of(transition.To)}; its line in {Names.Kebab(line.Skill)} " +
                    $"must be an exit in {Names.Of(transition.From)} or an arrival in {Names.Of(transition.To)}");
        _transitions[machine].Add((id, claim));
    }

    private static bool IsMove(Place? place, Transition transition) => place?.Step switch
    {
        MoveKind.Exit => Equals(place.Slot, transition.From),
        MoveKind.Arrival => Equals(place.Slot, transition.To),
        _ => false
    };

    private void StatesAreMachineStates(SkillSpec skill)
    {
        foreach (var slot in skill.StateSlots)
            if (!_machineOf.TryGetValue(slot.Id, out var machine) || machine.States.First(state => Equals(state.Id, slot.Id)).Title != slot.Title)
                throw new InvalidDataException(
                    $"{skill.Name}: state {Names.Of(slot.Id)} is not a machine state as its machine titles it; declare it with Machine.Slots");
    }
}
