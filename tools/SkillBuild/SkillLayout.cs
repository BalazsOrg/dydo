namespace DynaDocs.SkillBuild;

/// <summary>
/// One skill's slots and the lines placed in them (K9), checked: a skill's spine is its job paragraph,
/// titled phases and scenarios, and a standard holds sections only; every line has a placement in a
/// declared slot; a move is one line in a state; a job line is one line; no two lines share an order in
/// a slot. A truth-checked skill declares each slot by its shape and holds no state; a step is one line.
/// A withheld line is checked like any other and left out of <see cref="In"/>.
/// </summary>
internal sealed class SkillLayout
{
    private readonly SkillSpec _skill;
    private readonly IReadOnlySet<Placed> _withheld;
    private readonly Dictionary<Enum, SlotKind> _kinds = [];
    private readonly Dictionary<Enum, SlotShape> _shapes = [];
    private readonly Dictionary<Enum, List<Placed>> _lines = [];

    public SkillLayout(SkillSpec skill, IEnumerable<Placed> lines, IReadOnlySet<Placed>? withheld = null)
    {
        _skill = skill;
        _withheld = withheld ?? new HashSet<Placed>();
        Declare(skill);
        foreach (var line in lines) Add(line);
    }

    /// <summary>A slot's lines: moves by step, then each by its order.</summary>
    public IReadOnlyList<Placed> In(Enum slot) => _lines.TryGetValue(slot, out var lines)
        ? [.. lines.Where(placed => !_withheld.Contains(placed)).OrderBy(placed => placed.Line.Place.Step).ThenBy(placed => placed.Line.Place.Order)]
        : [];

    private void Declare(SkillSpec skill)
    {
        if (skill.IsStandard)
        {
            if (skill.Phases.Count + skill.Scenarios.Count > 0) throw Error("a standard holds sections only");
            Declare(skill.Sections);
            return;
        }
        if (skill.Sections.Count > 0)
            throw Error("a skill's spine is its job paragraph, phases and scenarios; sections belong to a standard");
        Declare(SpinePart.Job, SlotKind.Job);
        Declare(SpinePart.WhenThingsGoWrong, SlotKind.Lines);
        foreach (var phase in skill.Phases)
        {
            if (string.IsNullOrWhiteSpace(phase.Title)) throw Error("a job phase needs a title");
            Declare(phase.Slots);
        }
        var files = new HashSet<string>(StringComparer.Ordinal);
        foreach (var resource in skill.Resources)
        {
            if (!files.Add(SkillRenderer.FileOf(resource.Id))) throw Error($"resource {Names.Of(resource.Id)} is declared twice");
            Declare(resource.Slots);
        }
    }

    // Each list's first slot may be untitled: its lines follow the phase's, the resource's or the standard's heading.
    private void Declare(IReadOnlyList<Slot> slots)
    {
        for (var index = 0; index < slots.Count; index++)
        {
            var slot = slots[index];
            if (_skill.TruthChecked != slot.Shape.HasValue)
                throw Error(_skill.TruthChecked
                    ? $"{Names.Of(slot.Id)} declares no shape; a truth-checked skill's slot is a paragraph, bullets or steps"
                    : $"{Names.Of(slot.Id)} declares a shape; only a truth-checked skill's slots have one");
            if (slot.Shape is { } shape) _shapes[slot.Id] = shape;
            else if (slot.Title is null && (index > 0 || slot.IsState))
                throw Error($"{Names.Of(slot.Id)} needs a title; only the slot that opens a phase, resource or standard may have none");
            Declare(slot.Id, slot.IsState ? SlotKind.State : SlotKind.Lines);
        }
    }

    private void Declare(Enum slot, SlotKind kind)
    {
        if (!_kinds.TryAdd(slot, kind)) throw Error($"{Names.Of(slot)} is declared twice");
    }

    private void Add(Placed line)
    {
        var place = line.Line.Place ?? throw Error($"a line of claim:{line.Owner} has no placement");
        if (!_kinds.TryGetValue(place.Slot, out var kind))
            throw Error($"claim:{line.Owner} is placed in {Names.Of(place.Slot)}, which the skill does not declare");
        var inState = kind == SlotKind.State;
        if (inState != place.Step.HasValue || (kind != SlotKind.Lines && line.Line.Text.Contains('\n')))
            throw Error($"claim:{line.Owner}: a line in a state is a one-line move with its step, only a move has a step, and a job line is one line");
        if (_shapes.GetValueOrDefault(place.Slot, SlotShape.Bullets) == SlotShape.Steps && line.Line.Text.Contains('\n'))
            throw Error($"claim:{line.Owner}: a step is one line, and {Names.Of(place.Slot)} is numbered steps");
        var lines = _lines.TryGetValue(place.Slot, out var placed) ? placed : _lines[place.Slot] = [];
        if (lines.Any(other => other.Line.Place.Step == place.Step && other.Line.Place.Order == place.Order))
            throw Error($"claim:{line.Owner} takes order {place.Order} in {Names.Of(place.Slot)}, which another line holds");
        lines.Add(line);
    }

    private InvalidDataException Error(string message) => new($"{_skill.Name}: {message}");
}
