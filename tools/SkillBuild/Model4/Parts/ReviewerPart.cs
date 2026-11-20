namespace DynaDocs.SkillBuild;

/// <summary>
/// The reviewer's toolset resource and its slots (R5–R7, DYD-1082 1bc04dce change 8): the level × tool
/// matrix, the Drive and Formal qualification, each tool's use, what makes a test or doc a finding, and
/// the lenses.
/// </summary>
public enum ReviewerPart
{
    Toolset,
    Matrix,
    DriveAndFormal,
    Tools,
    Findings,
    Lenses
}
