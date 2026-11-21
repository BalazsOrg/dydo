namespace DynaDocs.SkillBuild;

/// <summary>What a claim's truth was checked against (DYD-1295 317be5f7), or that nothing could check it yet.</summary>
public enum EvidenceKind
{
    Command,
    Tool,
    Code,
    Run,
    Open
}
