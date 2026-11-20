namespace DynaDocs.Tests.SkillBuild;

using DynaDocs.SkillBuild;
using DynaDocs.Tests.SkillBuild.Fixture;
using DynaDocs.Tests.SkillBuild.Truths;

/// <summary>
/// DYD-1372 F: once a model holds a truth-checked skill, the build names every other skill and
/// standard as not yet migrated, beside the legacy lines that name a missing command, so none reads as
/// checked when it is not. Through <see cref="SkillBuilder.Build"/> on the truth fixture and the
/// production model.
/// </summary>
public class NotYetMigratedTests
{
    private const string List = "dydo/reference/claim-review/not-yet-migrated.md";

    [Fact]
    [Trait("Job", "contract: DYD-1372 F the build lists each skill that is not truth-checked as not yet migrated, with its lines and those naming a missing command")]
    public void SkillBesideATruthCheckedOne_IsListedNotYetMigrated() =>
        Assert.Equal(StatechartFilesTests.Text("""
            ---
            area: reference
            type: reference
            ---
            <!-- built from the model; edit there: tools/SkillBuild -->

            # Skills and standards not yet migrated

            These are not truth-checked: no line of theirs names its reader, and no claim carries its evidence, layer and reason on their account. Each builds as it did, unchanged, until it is switched over.

            `clerk`: not yet migrated; 1 line, 1 naming a command the CLI does not have.
            """), SkillBuilder.Build(TruthModel.Replacing(TruthModel.Entries)).Files[List]);

    [Fact]
    [Trait("Job", "contract: DYD-1372 F the production build names every other role skill and both standards, and not the ticket captain")]
    public void ProductionBuild_NamesEveryOtherSkillAndStandard()
    {
        var listed = SkillBuilder.Build(ProductionModel.Model).Files[List].Split('\n').Where(line => line.Contains(": not yet migrated; ", StringComparison.Ordinal))
            .Select(line => line[..line.IndexOf(':')]);

        Assert.Equal(["`coordinator`", "`chief-of-staff`", "`implementer`", "`reviewer`", "`research`", "`scout`", "`working-tree-standard`", "`linear-workspace-standard`"], listed);
    }

    [Fact]
    [Trait("Job", "edge: DYD-1372 F a model with nothing left to migrate, or with no truth-checked skill yet, builds no list")]
    public void ModelWithNothingToMigrateOrNoMigrationBegun_BuildsNoList()
    {
        Assert.DoesNotContain(List, SkillBuilder.Build(TruthModel.With()).Files.Keys);
        Assert.DoesNotContain(List, SkillBuilder.Build(FixtureModel.Model).Files.Keys);
    }
}
