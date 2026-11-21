namespace DynaDocs.Tests.SkillBuild;

using System.Text.RegularExpressions;
using DynaDocs.SkillBuild;

/// <summary>
/// DYD-1372 G, phase 1 hop B: the ticket-captain prototype's kind resources against the user's rulings
/// 07c86aad and aa5b445e (X1, X4, X5, X6, X8). Through <see cref="SkillBuilder.Build"/> on the
/// production model, read as the built text a ticket's holder loads for its kind. Every check is read
/// from the model, by claim id, phase title or path, so an edit of the prose leaves it standing (DR 067).
/// </summary>
public partial class TicketCaptainKindTests
{
    private const string Folder = "skills/roles/officers/ticket-captain/resources/";
    private const string Review = Folder + "review.md";

    private static readonly string[] ReviewKinds = ["CaptainReviewRead.", "CaptainReviewBreak.", "CaptainReviewDrive.", "CaptainReviewModel."];

    private static readonly Lazy<BuiltTree> Built = new(() => SkillBuilder.Build(ProductionModel.Model));

    [GeneratedRegex(@"`(dydo/[^`\s]+\.md)`")]
    private static partial Regex DocPath();

    [GeneratedRegex(@"\d{2,}")]
    private static partial Regex Number();

    [Theory]
    [Trait("Job", "contract: DYD-1372 X1 each kind resource opens with a map of the spine: one row a phase, in the spine's order, each saying use or skip for its holder (07c86aad \"I like a\")")]
    [InlineData("implement")]
    [InlineData("review")]
    [InlineData("research")]
    public void KindResource_OpensWithItsPhaseMap(string kind)
    {
        var lines = Built.Value.Files[$"{Folder}{kind}.md"].Split('\n');
        var start = Array.FindIndex(lines, line => line.StartsWith("| ", StringComparison.Ordinal));
        var rows = lines.Skip(start).TakeWhile(line => line.StartsWith("| ", StringComparison.Ordinal)).Skip(2)
            .Select(row => row.Trim('|').Split('|').Select(cell => cell.Trim()).ToArray()).ToArray();
        string[] spine = [.. CaptainCarrier.Spec.Phases.Select((phase, index) => $"{index + 1}. {phase.Title}"), SkillRenderer.WhenThingsGoWrong];

        Assert.True(start > 0, $"{kind}.md holds no table");
        Assert.DoesNotContain(lines.Take(start), line => line.StartsWith("- ", StringComparison.Ordinal) || line.StartsWith("**", StringComparison.Ordinal));
        Assert.Equal(spine, rows.Select(row => row[0]));
        Assert.All(rows.SelectMany(row => row.Skip(1)), cell => Assert.Matches("^(use|skip)", cell));
    }

    [Fact]
    [Trait("Job", "contract: DYD-1372 X4 a verdict is never reviewed: the review resource says so, and its map has every Review holder skip the review phase (777d396e, 07c86aad)")]
    public void Review_SaysAVerdictIsNeverReviewed()
    {
        var reviewed = Built.Value.Files[Review].Split('\n').Single(line => line.StartsWith($"| 4. {CaptainCarrier.Spec.Phases[3].Title} |", StringComparison.Ordinal));

        Assert.Equal(Review, LineOf("CaptainReviewing.NoReview").Path);
        Assert.All(reviewed.Trim('|').Split('|').Skip(1), cell => Assert.StartsWith("skip", cell.Trim()));
    }

    [Fact]
    [Trait("Job", "contract: DYD-1372 X5 the review resource sorts a seat first: its line stands above every kind, the holder's part below them all, and only the holder's part names delegate_task (aa5b445e)")]
    public void Review_SortsASeatAboveTheKindsAndTheHolderBelow()
    {
        var seat = LineOf("CaptainReviewing.SeatLeaf").Line;
        var holder = LineOf("CaptainReviewing.Matrix").Line;
        var kinds = Stated(Review).Where(line => ReviewKinds.Any(kind => line.Claims[0].StartsWith(kind, StringComparison.Ordinal))).Select(line => line.Line).ToArray();
        var tool = Stated(Review).Where(line => line.Text.Contains("`delegate_task`", StringComparison.Ordinal)).ToArray();

        Assert.True(seat < kinds.Min() && kinds.Max() < holder, "a seat reads past the holder's part to reach its kind");
        Assert.NotEmpty(tool);
        Assert.All(tool, line => Assert.True(line.Line > holder, "a line above the holder's part names delegate_task"));
    }

    [Fact]
    [Trait("Job", "contract: DYD-1372 X8 review has many types: reading, breaking, driving and modelling each have a recipe of their own, one unbroken run of lines a seat can read alone (07c86aad)")]
    public void Review_GivesEachKindOfReviewItsOwnRun()
    {
        var owners = Stated(Review).OrderBy(line => line.Line).Select(line => ReviewKinds.FirstOrDefault(kind => line.Claims[0].StartsWith(kind, StringComparison.Ordinal))).ToArray();
        var runs = owners.Where((owner, index) => owner is not null && (index == 0 || owners[index - 1] != owner)).ToArray();

        Assert.Equal(ReviewKinds, runs);
        Assert.All(ReviewKinds, kind => Assert.True(owners.Count(owner => owner == kind) >= 3, $"{kind} has no recipe"));
    }

    [Fact]
    [Trait("Job", "contract: DYD-1372 X6 the escape hatch stays: a seat's line on a slice too big for it links the too-big scenario, above the kinds (aa5b445e)")]
    public void Review_KeepsTheEscapeHatchForASeat()
    {
        var divide = LineOf("CaptainReviewing.Divide");

        Assert.Contains("](too-big.md)", divide.Text);
        Assert.True(divide.Line < LineOf("CaptainReviewing.Matrix").Line, "the escape hatch stands in the holder's part");
    }

    [Theory]
    [Trait("Job", "contract: DYD-1372 X8 what a project sets is found in its own docs: every dydo doc a kind resource names is in this repository and in the scaffold dydo ships (aa5b445e)")]
    [InlineData("implement")]
    [InlineData("review")]
    [InlineData("research")]
    public void KindResource_NamesOnlyDocsEveryProjectHas(string kind)
    {
        var paths = DocPath().Matches(Built.Value.Files[$"{Folder}{kind}.md"]).Select(match => match.Groups[1].Value).Distinct().ToArray();

        Assert.All(paths, path => Assert.True(File.Exists(Path.Combine(CommittedTree.Root, path)), $"{path} is not in this repository"));
        Assert.All(paths, path => Assert.True(File.Exists(Path.Combine(CommittedTree.Root, "Scaffold", path)), $"{path} is not in the scaffold"));
    }

    [Fact]
    [Trait("Job", "contract: DYD-1372 X8 implement carries its practice: its tests, its code and its docs each name the project doc that holds what the project sets (aa5b445e)")]
    public void Implement_PointsAtTheProjectsOwnStandards()
    {
        var implement = Built.Value.Files[Folder + "implement.md"];

        Assert.All(new[] { "dydo/guides/coding-standards.md", "dydo/guides/testing-strategy.md", "dydo/reference/writing-docs.md" }, path => Assert.Contains($"`{path}`", implement));
    }

    [Fact]
    [Trait("Job", "trap: DYD-1372 X8 a project's numbers stay in its own docs: the implement resource states no file size, complexity or parameter number (aa5b445e)")]
    public void Implement_StatesNoNumberAProjectSets() =>
        Assert.DoesNotMatch(Number(), Built.Value.Files[Folder + "implement.md"]);

    private static MapLine[] Stated(string path) => [.. Built.Value.Map.Where(line => line.Path == path)];

    // The built line that states a claim; a table's is its first.
    private static MapLine LineOf(string claim) =>
        Built.Value.Map.Where(line => line.Path.StartsWith(Folder, StringComparison.Ordinal) && line.Claims.Contains(claim)).MinBy(line => line.Line)!;
}
