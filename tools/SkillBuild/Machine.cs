namespace DynaDocs.SkillBuild;

/// <summary>
/// A statechart the model declares (K1): its ordered states, initial and final states, and optional
/// TLA+ parts: the standard modules it extends and its bounded extra variables. Its transitions are
/// the claims that name its states; its gaps are the arrows the spec leaves open.
/// </summary>
public sealed record Machine(Enum Id, string Title, State[] States, Enum Initial, Enum[] Finals)
{
    /// <summary>Standard modules the guards, effects and bounds use, such as <c>Naturals</c>.</summary>
    public IReadOnlyList<string> Extends { get; init; } = [];

    public IReadOnlyList<Variable> Variables { get; init; } = [];

    public IReadOnlyList<Gap> Gaps { get; init; } = [];

    /// <summary>
    /// The folder that holds this machine's chart when the chart is written by hand (DYD-1437). The build
    /// still renders the machine's files for its own checks and writes none of them.
    /// </summary>
    public string? HandWritten { get; init; }

    /// <summary>A skill's state slots for these states, titled as the machine titles them.</summary>
    public Slot[] Slots(params Enum[] states) =>
        [.. states.Select(id => Slot.State(id, States.FirstOrDefault(state => Equals(state.Id, id))?.Title))];
}
