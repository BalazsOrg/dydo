namespace DynaDocs.SkillBuild;

/// <summary>
/// The four truth questions a skill line answers (DYD-1295 1ffbc4c8, 41a553f1), by the one a dropped
/// legacy line fails: <see cref="Need"/>, its reader does not act on it; <see cref="Verify"/>, the
/// running system does not show it; <see cref="Handled"/>, a lower layer already does it;
/// <see cref="Reason"/>, nobody knows what it prevents or enables.
/// </summary>
public enum Question
{
    Need,
    Verify,
    Handled,
    Reason
}
