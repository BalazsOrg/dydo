namespace DynaDocs.Tests.Survey;

using DynaDocs.Services.Map;
using DynaDocs.Services.Map.Contract;
using DynaDocs.Services.Survey;

/// <summary>
/// DR 063 §4's graph findings over one Project read. The lane ledger and its findings retire in 4.0.0,
/// so the Project body here is empty and only the findings 4.0.0 keeps are read.
/// </summary>
[Trait("Job", "contract: DR 063 §4")]
public class SurveyRulesTests
{
    private static readonly Dictionary<string, string> StateNames = new()
    {
        ["triage"] = "Triage", ["backlog"] = "Backlog", ["unstarted"] = "Todo", ["started"] = "In Progress",
        ["completed"] = "Done", ["canceled"] = "Canceled", ["duplicate"] = "Duplicate",
    };

    private static MapIssue Issue(
        int number, string type = "unstarted", string? assignee = null, bool external = false, string label = "Feature") =>
        new($"id-{number}", $"DYD-{number}", $"Issue {number}", $"https://linear.app/x/issue/DYD-{number}",
            new MapIssueState(StateNames[type], type, "#000000"), assignee, null,
            new MapIssueTeam("t-dyd", "DYD"),
            external ? new MapIssueProject("p-other", "Other") : new MapIssueProject("p-map", "Map"),
            [new MapLabel(label, "#000000")]);

    private static MapRelation Blocks(int from, int to) => new($"r-{from}-{to}", "blocks", $"id-{from}", $"id-{to}");

    private static List<SurveyFinding> Survey(
        List<MapIssue> issues, List<MapIssue>? external = null, List<MapRelation>? relations = null,
        Dictionary<string, double>? priorities = null)
    {
        external ??= [];
        var graph = new MapGraph(new MapGraphProject("p-map", "Map", "https://linear.app/x/project/map"),
            issues, external, relations ?? []);
        priorities ??= [];
        foreach (var issue in issues.Concat(external))
            priorities.TryAdd(issue.Id, 0);
        return [.. SurveyRules.Find(new LinearProjectRead(graph, "", priorities, new Dictionary<string, string>()))
            .Where(finding => KeptFindings.Kinds.Contains(finding.Kind))];
    }

    private static List<string> Keys(List<SurveyFinding> findings, string kind) =>
        [.. findings.Where(finding => finding.Kind == kind).Select(finding => finding.Key)];

    [Theory]
    [InlineData("triage")]
    [InlineData("backlog")]
    [InlineData("unstarted")]
    [InlineData("started")]
    public void Unblocked_AnOpenUnassignedIssue_WhoseEveryBlockerIsClosed_IsListedOnce(string type)
    {
        var findings = Survey(
            [Issue(1, type), Issue(2, "completed"), Issue(3, "canceled")],
            relations: [Blocks(2, 1), Blocks(3, 1), new("r-again", "blocks", "id-2", "id-1")]);

        Assert.Equal(["DYD-1"], Keys(findings, "unblocked"));
    }

    [Fact]
    public void Unblocked_IncludesAnIssueLinkedFromTheProject()
    {
        var findings = Survey([Issue(1, "duplicate")], external: [Issue(9, "backlog", external: true)],
            relations: [Blocks(1, 9)]);

        Assert.Equal(["DYD-9"], Keys(findings, "unblocked"));
    }

    [Fact]
    public void Unblocked_NotWhileABlockerIsOpen_NorWhenAssigned_Closed_OrOnlyRelated()
    {
        var findings = Survey(
            [Issue(1), Issue(2, "completed"), Issue(3, "backlog"), Issue(4, assignee: "Ada"), Issue(5, "canceled"), Issue(6)],
            relations: [Blocks(2, 1), Blocks(3, 1), Blocks(2, 4), Blocks(2, 5), new("r-2-6", "related", "id-2", "id-6")]);

        Assert.Empty(Keys(findings, "unblocked"));
    }

    [Fact]
    public void Pickable_IsTheViewersRule_MinusIssuesAlreadyUnblocked()
    {
        var findings = Survey(
            [Issue(1), Issue(2), Issue(3, "completed"), Issue(4, "backlog"), Issue(5), Issue(6, "started"), Issue(7, assignee: "Ada")],
            external: [Issue(9, external: true)],
            relations: [Blocks(3, 2), Blocks(4, 5)]);

        Assert.Equal(["DYD-1"], Keys(findings, "pickable"));
        Assert.Equal(["DYD-2"], Keys(findings, "unblocked"));
    }

    [Fact]
    public void Uncontracted_AnUnassignedBacklogProjectIssue_WithNoOpenBlocker_IsNeverPickable()
    {
        var findings = Survey(
            [Issue(1, "backlog"), Issue(2, "backlog"), Issue(3, "started", "Ada"), Issue(4, "backlog", "Ada"),
                Issue(5, "backlog"), Issue(6, "completed"), Issue(7), Issue(8, "triage")],
            external: [Issue(9, "backlog", external: true)],
            relations: [Blocks(3, 2), Blocks(6, 5)]);

        Assert.Equal(["DYD-1", "DYD-5"], Keys(findings, "uncontracted"));
        Assert.Contains("uncontracted DYD-1 Issue 1 — Backlog, unassigned, no open blocker",
            findings.Select(finding => finding.ToString()));
        Assert.Equal(["DYD-7"], Keys(findings, "pickable"));
    }

    [Fact]
    public void QuestionNoPriority_AnOpenQuestionWithNoPriority()
    {
        var findings = Survey(
            [Issue(1, "backlog", "Ada", label: "Question"), Issue(2, "backlog", "Ada", label: "Question"),
                Issue(3, "completed", label: "Question"), Issue(4, "backlog", "Ada")],
            priorities: new() { ["id-2"] = 3 });

        Assert.Equal(["DYD-1"], Keys(findings, "question-no-priority"));
    }

    [Fact]
    public void Findings_AreSortedByKind_ThenByKeyNumber()
    {
        var findings = Survey(
            [Issue(10, "backlog"), Issue(9), Issue(3, "backlog"), Issue(2), Issue(1, "backlog", "Ada", label: "Question")]);

        Assert.Equal(
            ["pickable DYD-2", "pickable DYD-9", "question-no-priority DYD-1", "uncontracted DYD-3", "uncontracted DYD-10"],
            findings.Select(finding => $"{finding.Kind} {finding.Key}"));
    }

    [Fact]
    public void AQuietProject_HasNoFindings() =>
        Assert.Empty(Survey([Issue(1, "started", "Ada"), Issue(2, "completed")]));
}
