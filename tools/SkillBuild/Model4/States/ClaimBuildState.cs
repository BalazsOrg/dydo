namespace DynaDocs.SkillBuild;

/// <summary>The claim build: a claim edit to a built, approved release.</summary>
public enum ClaimBuildState
{
    Edited,
    Rejected,
    Built,
    Nudged,
    Checked,
    TlcChecked,
    InReview,
    Approved,
    Released
}
