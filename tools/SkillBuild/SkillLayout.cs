namespace DynaDocs.SkillBuild;

/// <summary>
/// One skill's slots and the lines placed in them (K9), checked: every line has a placement in a
/// declared slot, a move is one line in a state, no two lines share an order in a slot, and every
/// appendix is linked from a move.
/// </summary>
internal sealed class SkillLayout
{
    private readonly SkillSpec _skill;
    private readonly Dictionary<Enum, SlotKind> _kinds = [];
    private readonly Dictionary<Enum, List<Placed>> _lines = [];

    public SkillLayout(SkillSpec skill, IEnumerable<Placed> lines)
    {
        _skill = skill;
        Declare(skill);
        foreach (var line in lines) Add(line);
        foreach (var appendix in skill.Appendices)
            if (!_lines.Values.SelectMany(placed => placed).Any(placed => Equals(placed.Line.Place.Appendix, appendix.Id)))
                throw Error($"appendix {Names.Of(appendix.Id)} is linked from no move");
    }

    /// <summary>A slot's lines: moves by step, then each by its order.</summary>
    public IReadOnlyList<Placed> In(Enum slot) => _lines.TryGetValue(slot, out var lines)
        ? [.. lines.OrderBy(placed => placed.Line.Place.Step).ThenBy(placed => placed.Line.Place.Order)]
        : [];

    private void Declare(SkillSpec skill)
    {
        if (skill.IsStandard && skill.States.Count + skill.Routines.Count + skill.Resources.Count + skill.Appendices.Count > 0)
            throw Error("a standard holds sections only");
        if (!skill.IsStandard)
            foreach (var part in Enum.GetValues<HeaderPart>()) Declare(part, SlotKind.Header);
        Declare(skill.States, SlotKind.State);
        Declare(skill.Routines, SlotKind.Routine);
        Declare(skill.Sections, SlotKind.Section, opensFile: skill.IsStandard);
        foreach (var resource in skill.Resources) Declare(resource.Sections, SlotKind.Section, opensFile: true);
        Declare(skill.Appendices, SlotKind.Appendix);
    }

    private void Declare(IReadOnlyList<Slot> slots, SlotKind kind, bool opensFile = false)
    {
        for (var index = 0; index < slots.Count; index++)
        {
            var mayBeUntitled = kind == SlotKind.Section && opensFile && index == 0;
            if (slots[index].Title is null && !mayBeUntitled)
                throw Error($"{Names.Of(slots[index].Id)} needs a title; only a file's opening section may have none");
            Declare(slots[index].Id, kind);
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
        if (inState != place.Step.HasValue || (inState ? line.Line.Text.Contains('\n') : place.Appendix is not null))
            throw Error($"claim:{line.Owner}: a line in a state is a one-line move with its step, and only a move has a step or an appendix");
        if (place.Appendix is not null && _kinds.GetValueOrDefault(place.Appendix) != SlotKind.Appendix)
            throw Error($"claim:{line.Owner} links {Names.Of(place.Appendix)}, which the skill does not declare as an appendix");
        var lines = _lines.TryGetValue(place.Slot, out var placed) ? placed : _lines[place.Slot] = [];
        if (lines.Any(other => other.Line.Place.Step == place.Step && other.Line.Place.Order == place.Order))
            throw Error($"claim:{line.Owner} takes order {place.Order} in {Names.Of(place.Slot)}, which another line holds");
        lines.Add(line);
    }

    private InvalidDataException Error(string message) => new($"{_skill.Name}: {message}");
}
