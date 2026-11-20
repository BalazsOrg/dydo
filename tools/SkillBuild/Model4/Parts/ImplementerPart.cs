namespace DynaDocs.SkillBuild;

/// <summary>
/// The implementer's own slots and phase resources (K9, K22): its build list, and its code, docs, Bug and
/// merge recipes (DYD-1082 1bc04dce change 4). A recipe's resource id and its one slot share a member.
/// </summary>
public enum ImplementerPart
{
    Build,
    Code,
    Docs,
    Bug,
    Merge
}
