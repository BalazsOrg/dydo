namespace DynaDocs.Tests.SkillBuild;

/// <summary>Grilling A D3: the committed built tree is exactly what the claims build, so hand edits cannot creep in.</summary>
[Trait("Job", "contract: DR 064 §5 K7")]
public class SkillBuildGoldenTests
{
    [Fact]
    public void CommittedBuiltTree_IsByteForByteTheBuildOfTheClaims()
    {
        var diff = GoldenDiff.Describe(CommittedTree.Rebuild(), CommittedTree.Built());

        Assert.True(diff.Length == 0, diff);
    }
}
