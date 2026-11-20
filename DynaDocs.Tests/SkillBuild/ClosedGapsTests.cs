namespace DynaDocs.Tests.SkillBuild;

using System.Text.Json;
using DynaDocs.SkillBuild;

/// <summary>
/// The statechart GAPs his walkthrough closed in round 5 (DYD-1082 555a8300 gaps 3, 4, 5, 8, 9 and 10;
/// DYD-1195 8661b8f4, 70df9ff8) are real arrows in the built machines, and those machines draw no GAP.
/// </summary>
public class ClosedGapsTests
{
    [Theory]
    [Trait("Job", "contract: S5, N6, L21, K22, N10 each closed GAP is a built arrow between the states its ruling names")]
    [InlineData("ticket-status", "New", "FutureFeature")]
    [InlineData("ticket-status", "FutureFeature", "Backlog")]
    [InlineData("ticket-status", "New", "Backlog")]
    [InlineData("landing", "Walkthrough", "NotAccepted")]
    [InlineData("landing", "NotAccepted", "Integrated")]
    [InlineData("tribunal", "ProposedDr", "Declined")]
    [InlineData("role-evaluation", "WrittenFresh", "Changed")]
    [InlineData("role-evaluation", "Folded", "Changed")]
    [InlineData("role-evaluation", "Removed", "Changed")]
    [InlineData("role-evaluation", "Changed", "Questioned")]
    [InlineData("claim-build", "InReview", "Edited")]
    [InlineData("safe-delete", "Archived", "Live")]
    public void ClosedGap_IsABuiltArrow(string machine, string from, string to)
    {
        var transitions = Machine(machine).GetProperty("transitions").EnumerateArray();

        Assert.Contains(transitions, move => move.GetProperty("from").GetString() == from && move.GetProperty("to").GetString() == to);
    }

    [Theory]
    [Trait("Job", "contract: the machines whose round-5 GAPs are closed draw no GAP")]
    [InlineData("ticket-status")]
    [InlineData("landing")]
    [InlineData("tribunal")]
    [InlineData("role-evaluation")]
    [InlineData("safe-delete")]
    public void ClosedMachine_DrawsNoGap(string machine) => Assert.Empty(Machine(machine).GetProperty("gaps").EnumerateArray());

    private static readonly Lazy<IReadOnlyDictionary<string, string>> Built = new(() => SkillBuilder.Build(ProductionModel.Model).Files);

    private static JsonElement Machine(string folder) =>
        JsonDocument.Parse(Built.Value[$"claims/statechart/{folder}/transitions.json"]).RootElement;
}
