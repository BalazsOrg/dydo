namespace DynaDocs.SkillBuild;

/// <summary>The slot every scenario resource closes on, last after its decisions (K9).</summary>
public static class ScenarioSlots
{
    public static Slot Close(ScenarioClose scenario) => new(scenario, "Save, resume and end");
}
