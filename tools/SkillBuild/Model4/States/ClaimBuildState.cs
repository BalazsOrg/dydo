namespace DynaDocs.SkillBuild;

/// <summary>The claim build: a claim edit to a built, approved release.</summary>
public enum ClaimBuildState
{
    Edited,
    Rejected,
    Built,
    Nudged,
    Checked,
    TlcPassed,
    TlcFailed,
    InReview,
    Approved,
    Released
}
