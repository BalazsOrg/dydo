namespace DynaDocs.SkillBuild;

/// <summary>Proof tests and gates: a test from written to graduated or dropped.</summary>
public enum ProofState
{
    Start,
    Written,
    Red,
    Gated,
    GateRed,
    Receipt,
    Graduated,
    Dropped,
    Finding,
    Old,
    Conformance
}
