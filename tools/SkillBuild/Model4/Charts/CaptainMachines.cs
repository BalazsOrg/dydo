namespace DynaDocs.SkillBuild;

/// <summary>
/// The ticket-captain prototype's one machine (DYD-1372 A8): the states of a captain holding one
/// ticket, from taking it to returning. Its arrows are the prototype's own claims. <c>round</c> counts
/// review rounds and is bounded only so TLC ends; the skill sets no cap.
/// </summary>
public static class CaptainMachines
{
    public static readonly Machine Captain = new(MachineId.Captain, "A captain and its ticket",
    [
        new(CaptainState.Taken, "Taken", "S6", "S15"),
        new(CaptainState.Leaf, "Working it as a leaf", "S2", "S7"),
        new(CaptainState.Split, "Split into sub-tickets", "S1", "S15"),
        new(CaptainState.Waiting, "Waiting on children", "H3"),
        new(CaptainState.InReview, "In review", "R2", "R8"),
        new(CaptainState.Fixing, "Fixing", "S9", "R8"),
        new(CaptainState.Question, "Waiting on a Question", "H16"),
        new(CaptainState.Finishing, "Finishing", "R21", "S6"),
        new(CaptainState.Done, "Done", "S6"),
        new(CaptainState.Returned, "Returned to its officer", "H23")
    ], CaptainState.Taken, [CaptainState.Returned])
    {
        Extends = ["Naturals"],
        Variables = [new("round", "1", "round <= 4")]
    };
}
