namespace DynaDocs.Tests.SkillBuild;

using System.Text.Json;
using DynaDocs.SkillBuild;

/// <summary>
/// The envoy machine follows the Question stages (H16, H23; DYD-1195 317eb49f, 3a854e37): a need passed up becomes
/// a research Question its officer opens and waits on, whose research answers it, opens a council child, or puts a
/// his-call child on his HITL queue, from which the coordinator sends one envoy at a time (H18).
/// </summary>
public class EnvoyStagesTests
{
    [Theory]
    [Trait("Job", "contract: H16, H23, H18 a need passed up is a research Question, routed by stage to an answer, a council child or his HITL queue, then one envoy")]
    [InlineData("Need", "Research")]
    [InlineData("Research", "Answered")]
    [InlineData("Research", "Council")]
    [InlineData("Research", "Queued")]
    [InlineData("Queued", "Active")]
    public void StageMove_IsABuiltArrow(string from, string to)
    {
        var transitions = Envoy().GetProperty("transitions").EnumerateArray();

        Assert.Contains(transitions, move => move.GetProperty("from").GetString() == from && move.GetProperty("to").GetString() == to);
    }

    private static readonly Lazy<IReadOnlyDictionary<string, string>> Built = new(() => SkillBuilder.Build(ProductionModel.Model).Files);

    private static JsonElement Envoy() => JsonDocument.Parse(Built.Value["claims/statechart/envoy/transitions.json"]).RootElement;
}
