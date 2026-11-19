namespace DynaDocs.Tests.SkillBuild;

using DynaDocs.SkillBuild;

/// <summary>T15: every file the production model builds is at most 20,000 bytes (5k tokens at bytes ÷ 4).</summary>
public class BuiltFileSizeTests
{
    [Fact]
    [Trait("Job", "contract: T15 no file the production build writes is over 20,000 bytes")]
    public void EveryProductionBuiltFile_IsUnderTheTokenLimit()
    {
        var over = SkillBuilder.Build(ProductionModel.Model).Bytes()
            .Where(file => file.Value.Length > KindFileTests.SizeCap)
            .Select(file => $"{file.Key}: {file.Value.Length} bytes")
            .ToArray();

        Assert.True(over.Length == 0, $"over {KindFileTests.SizeCap} bytes:\n" + string.Join('\n', over));
    }
}
