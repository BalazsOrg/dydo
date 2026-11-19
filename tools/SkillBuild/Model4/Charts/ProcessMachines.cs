namespace DynaDocs.SkillBuild;

/// <summary>
/// The 4.0.0 machines for the system around the work (K1): the claim build, proof tests and gates,
/// descriptive docs, the survey and CI findings, and the adopter update.
/// </summary>
public static class ProcessMachines
{
    public static readonly Machine ClaimBuild = new(MachineId.ClaimBuild, "The claim build",
    [
        new(ClaimBuildState.Edited, "A claim edited in the C# model", "K1", "K2", "K4", "K17", "K19", "K21"),
        new(ClaimBuildState.Rejected, "The build or its tests refuse", "K4", "K7", "K10", "K24"),
        new(ClaimBuildState.Built, "Built files written by the builder", "K5", "K6", "K8", "K9", "K12", "K13", "K23", "K26"),
        new(ClaimBuildState.Nudged, "A built file edited by hand: the guard nudges", "K6"),
        new(ClaimBuildState.Checked, "Golden, shape and stray-claim checks pass", "K7", "K10", "K24"),
        new(ClaimBuildState.TlcPassed, "TLC passes the moves against the hand-written properties", "K3", "K20"),
        new(ClaimBuildState.TlcFailed, "TLC fails: the push is not done", "K3", "K20"),
        new(ClaimBuildState.InReview, "Model PR read through its built-skill diff", "K16"),
        new(ClaimBuildState.Approved, "Claims approved by Balazs", "K16", "K20"),
        new(ClaimBuildState.Released, "Built skills shipped downstream", "K18", "K15", "K25", "K27", "K28", "K29")
    ], ClaimBuildState.Edited, [ClaimBuildState.Released]);

    public static readonly Machine RoleEvaluation = new(MachineId.RoleEvaluation, "The nine role skills evaluated",
    [
        new(RoleEvaluationState.Questioned, "A role skill questioned: should it exist, or fold into another?", "K22", "K19"),
        new(RoleEvaluationState.Kept, "Kept as a survivor", "K22"),
        new(RoleEvaluationState.Folded, "Folded: a survivor carries its responsibilities", "K22"),
        new(RoleEvaluationState.Removed, "Removed", "K22"),
        new(RoleEvaluationState.WrittenFresh, "Written fresh from the 4.0.0 decisions and claims", "K22", "K19", "K20"),
        new(RoleEvaluationState.Approved, "Approved by him at the final candidate, folds and removals included", "K16", "K22")
    ], RoleEvaluationState.Questioned, [RoleEvaluationState.Approved])
    {
        Gaps = [new(RoleEvaluationState.WrittenFresh, null, "K16 and K22 give his approval at the final candidate, not the route when he rejects a fold or a rewrite")]
    };

    public static readonly Machine Proof = new(MachineId.Proof, "Proof tests and gates",
    [
        new(ProofState.Start, "A test enters the suite", "T1"),
        new(ProofState.Written, "A proof test in the ticket's commits", "T5", "T4"),
        new(ProofState.Red, "Shown failing without the code", "T3"),
        new(ProofState.Gated, "dydo test gate and dydo code-check on changed files", "T2", "T13", "T21", "Y6", "T11", "T10", "T14", "Y7"),
        new(ProofState.GateRed, "A gate red: a failing test, or a static gate", "T23", "T19"),
        new(ProofState.Receipt, "A green receipt keyed by tree; coverage only reported", "Y4", "T9", "T23"),
        new(ProofState.Graduated, "Graduated with its job tag", "T5", "T1", "T7"),
        new(ProofState.Dropped, "Dropped", "T5", "T6", "T8"),
        new(ProofState.Finding, "A finding: an untraced pin, or a missing tag", "T1", "R3", "T5"),
        new(ProofState.Old, "An old test measured against the new requirements", "T8", "T20", "T22"),
        new(ProofState.Conformance, "A rewrite's conformance assertion", "T6")
    ], ProofState.Start, [ProofState.Graduated, ProofState.Dropped, ProofState.Finding]);

    public static readonly Machine Docs = new(MachineId.Docs, "Descriptive docs",
    [
        new(DocsState.Start, "A descriptive doc", "D1", "D2"),
        new(DocsState.Proposed, "A new descriptive doc proposed", "D4"),
        new(DocsState.NotWritten, "Not written", "D4", "D3", "D8"),
        new(DocsState.Invited, "Invited", "D4"),
        new(DocsState.Admitted, "Admitted: solid, important, expensive to rebuild from code", "D3"),
        new(DocsState.Declared, "Its explained code declared in explains:", "D5", "D2"),
        new(DocsState.Flagged, "A change to its paths flagged", "D5"),
        new(DocsState.Finding, "A review finding on it", "D6", "D2"),
        new(DocsState.Grandfathered, "An existing doc, grandfathered", "D7"),
        new(DocsState.CutDoc, "Cut", "D7")
    ], DocsState.Start, [DocsState.NotWritten, DocsState.CutDoc]);

    public static readonly Machine Findings = new(MachineId.Findings, "Survey, board and CI findings",
    [
        new(FindingsState.Watch, "Findings arrive", "S22", "S26", "Y9", "Y8"),
        new(FindingsState.Board, "dydo survey board read", "S22", "S25", "S26", "H7"),
        new(FindingsState.SharedLink, "One thread link on two non-nested open tickets", "S23", "S13"),
        new(FindingsState.LandedSpec, "A landed Project that still has its spec", "S23", "S14"),
        new(FindingsState.StaleShoulds, "Open shoulds under Done parents past the threshold", "S23", "T14"),
        new(FindingsState.CiRed, "A real CI red: a CI-findings ticket", "Y9", "H8"),
        new(FindingsState.Routed, "Routed to its officer", "H11", "H8"),
        new(FindingsState.Flake, "A flake or runner noise", "Y9", "R22"),
        new(FindingsState.Counted, "A DYTEST result with a count", "Y9"),
        new(FindingsState.Repeated, "A flake that repeats", "Y9"),
        new(FindingsState.Weed, "A mistake found after landing", "R30"),
        new(FindingsState.DytestNeed, "Something dydo needs from DYTEST", "Y3", "Y1"),
        new(FindingsState.Filed, "Filed in a DYTEST Project", "Y3"),
        new(FindingsState.Acted, "Its owner acts", "H5")
    ], FindingsState.Watch, [FindingsState.Counted, FindingsState.Filed, FindingsState.Acted]);

    public static readonly Machine Adopter = new(MachineId.Adopter, "The adopter update",
    [
        new(AdopterState.Released, "A dydo release ships", "K18", "N1"),
        new(AdopterState.Updating, "One ordinary update, with its Upgrade note", "K18", "N8", "L2"),
        new(AdopterState.Tailored, "A shipped file the project keeps tailored", "N11"),
        new(AdopterState.Consulted, "update-dydo consults the user", "N11"),
        new(AdopterState.Major, "4.0.0 as a major update", "N1", "N9", "Y5", "D1"),
        new(AdopterState.Current, "Up to date; later builds line-stable", "K18"),
        new(AdopterState.Harvested, "Friction and local patches harvested for the next release", "H9")
    ], AdopterState.Released, [AdopterState.Current, AdopterState.Harvested]);
}
