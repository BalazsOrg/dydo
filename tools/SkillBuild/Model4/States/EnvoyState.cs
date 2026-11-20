namespace DynaDocs.SkillBuild;

/// <summary>M10: a need for Balazs, by the Question stages (H16, H23), out by one envoy at a time (H18).</summary>
public enum EnvoyState
{
    Need,
    Decided,
    AskedInThread,
    Research,
    Answered,
    Council,
    Queued,
    Active,
    Reported,
    Closed
}
