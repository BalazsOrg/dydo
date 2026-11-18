namespace DynaDocs.Tests.SkillBuild;

using DynaDocs.SkillBuild;
using DynaDocs.Tests.SkillBuild.Fixture;

/// <summary>The migration fixture's build, made once for every test that reads it.</summary>
internal static class Migration
{
    private static readonly Lazy<BuiltTree> Built = new(() => SkillBuilder.Build(FixtureModel.Model));

    public static BuiltTree Tree => Built.Value;
}
