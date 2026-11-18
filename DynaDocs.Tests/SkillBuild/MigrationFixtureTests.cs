namespace DynaDocs.Tests.SkillBuild;

using System.Reflection;
using DynaDocs.SkillBuild;
using DynaDocs.Tests.SkillBuild.Fixture;

/// <summary>
/// AR1: the inventory's 1,025 claims, translated once into a C# fixture, build into a root that mirrors
/// the real layout, with every built path one the repository holds today (DYD-1196 C16, C17, D19).
/// </summary>
public class MigrationFixtureTests
{
    // claims/inventory/README.md "Counts": 1025 claims across 15 carriers.
    private const int InventoryClaims = 1025;

    private static readonly IReadOnlySet<string> ModelIds = FixtureModel.Model.Kinds
        .SelectMany(kind => kind.GetFields(BindingFlags.Public | BindingFlags.Static)
            .Where(field => field.FieldType == typeof(Claim))
            .Select(field => $"{kind.Name}.{field.Name}"))
        .ToHashSet(StringComparer.Ordinal);

    [Fact]
    [Trait("Job", "contract: K2/AR1 the fixture holds every inventory claim, each carrier stating each of its claims (C16)")]
    public void Fixture_DeclaresEveryInventoryClaimAndStatesEachOne()
    {
        Assert.Equal(InventoryClaims, ModelIds.Count);
        Assert.Equal(15, FixtureModel.Model.Skills.Count);
        Assert.Equal(ModelIds.Order(StringComparer.Ordinal), Migration.Tree.Map.SelectMany(line => line.Claims).Distinct().Order(StringComparer.Ordinal));
    }

    [Fact]
    [Trait("Job", "contract: K6 the fixture writes exactly its own tree, with one why doc per kind (C17, B9)")]
    public void FixtureBuild_WritesItsTreeAndOneWhyDocPerKind()
    {
        using var root = new TempRoot();

        Assert.Equal(0, root.Run(FixtureModel.Model).Exit);

        var written = root.Files().Keys.Order(StringComparer.Ordinal).ToArray();
        Assert.Equal(Migration.Tree.Files.Keys.Order(StringComparer.Ordinal), written);
        // K13's why docs are new with DYD-1082: one per kind, and nowhere else.
        var whyDocs = written.Where(path => path.StartsWith("dydo/reference/claims/", StringComparison.Ordinal)).ToArray();
        Assert.Equal(FixtureModel.Model.Kinds.Count, whyDocs.Length);
        Assert.Contains("skills/roles/officers/admiral/agents/openai.yaml", written);
        Assert.Contains("skills/roles/officers/chief-of-staff/agents/openai.yaml", written);
    }

    [Theory]
    [Trait("Job", "contract: K12 one build emits both copies of each standard, byte-identical (C17, B9)")]
    [InlineData("guides/working-tree-standard.md")]
    [InlineData("reference/linear-workspace-standard.md")]
    public void FixtureBuild_EmitsBothCopiesOfEachStandardIdentically(string standard) =>
        Assert.Equal(Migration.Tree.Files["dydo/" + standard], Migration.Tree.Files["Scaffold/dydo/" + standard]);

    [Fact]
    [Trait("Job", "contract: K23 every claim-stating built line maps, at its line, to model claims (D19)")]
    public void Map_PointsAtTheLineThatStatesItsClaims()
    {
        foreach (var line in Migration.Tree.Map)
        {
            var built = Migration.Tree.Files[line.Path].Split('\n')[line.Line - 1];
            Assert.True(built == line.Text || built.StartsWith("- " + line.Text, StringComparison.Ordinal), $"{line}: {built}");
            Assert.All(line.Claims, id => Assert.Contains(id, ModelIds));
        }
    }

    [Fact]
    [Trait("Job", "contract: K23 no bullet the model states is left out of the map (D19)")]
    public void Map_CoversEveryClaimBullet()
    {
        var mapped = Migration.Tree.Map.Select(line => (line.Path, line.Line)).ToHashSet();
        foreach (var (path, text) in Migration.Tree.Files.Where(file => file.Key.EndsWith(".md", StringComparison.Ordinal)))
        {
            var heading = "";
            var lines = text.Split('\n');
            for (var index = 0; index < lines.Length; index++)
            {
                if (lines[index].StartsWith('#')) heading = lines[index];
                else if (lines[index].StartsWith("- ", StringComparison.Ordinal) && heading is not ("## Resources" or "## Contents"))
                    Assert.True(mapped.Contains((path, index + 1)), $"{path}:{index + 1} states no claim: {lines[index]}");
            }
        }
    }
}
