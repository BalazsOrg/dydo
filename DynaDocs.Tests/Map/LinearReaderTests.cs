namespace DynaDocs.Tests.Map;

using DynaDocs.Services.Map;
using DynaDocs.Services.Map.Contract;
using static LinearJson;

/// <summary>
/// The graph the map and the survey read from a fake Linear: every page and overflow followed, archived
/// issues dropped with their relations, only <c>blocks</c> and <c>related</c> kept (DR 052 §1 and §5,
/// DR 063 §2). Order is not part of the read; each assertion compares sets.
/// </summary>
[Trait("Job", "contract: DR 052 §1 §5; DR 063 §2; dydo-commands-linear.md (the teams, a team's Projects and a Project's graph the map page reads; the priorities, parents and body the survey reads)")]
public class LinearReaderTests
{
    private const string Done = """{"name":"Done","type":"completed","color":"#5e6ad2"}""";
    private const string Duplicate = """{"name":"Duplicate","type":"duplicate","color":"#95a2b3"}""";

    private readonly FakeLinear _linear = new();

    private Task<MapGraph> Graph() => _linear.Reader().GetGraphAsync("p-map", CancellationToken.None);

    private static void AssertSet<T>(IEnumerable<T> expected, IEnumerable<T> actual) =>
        Assert.Equal(expected.ToHashSet(), actual.ToHashSet());

    [Fact]
    public async Task Teams_AreReadAcrossEveryPage()
    {
        _linear.Serve(call => call.Var("after") == null
            ? """{"data":{"teams":{"nodes":[{"id":"t2","key":"ZED","name":"zeta"},{"id":"t1","key":"DYD","name":"Dydo"}],"pageInfo":{"hasNextPage":true,"endCursor":"c1"}}}}"""
            : """{"data":{"teams":{"nodes":[{"id":"t3","key":"ALP","name":"Alpha"}],"pageInfo":{"hasNextPage":false,"endCursor":"c2"}}}}""");

        var teams = await _linear.Reader().GetTeamsAsync(CancellationToken.None);

        AssertSet([new MapTeam("t1", "DYD", "Dydo"), new MapTeam("t2", "ZED", "zeta"), new MapTeam("t3", "ALP", "Alpha")], teams);
    }

    [Fact]
    public async Task Projects_OfOneTeam_CarryTheirStatusAndDates_OrNullWhenUnset()
    {
        _linear.Serve(_ => """
            {"data":{"team":{"projects":{"nodes":[
              {"id":"p1","name":"Dated","url":"u1","status":{"name":"Completed","type":"completed"},
               "targetDate":"2026-10-03","completedAt":"2026-09-20T10:00:00.000Z","canceledAt":"2026-09-21T11:00:00.000Z"},
              {"id":"p2","name":"Undated","url":"u2","status":{"name":"Backlog","type":"backlog"},
               "targetDate":null,"completedAt":null,"canceledAt":null}
            ],"pageInfo":{"hasNextPage":false,"endCursor":null}}}}}
            """);

        var projects = (await _linear.Reader().GetProjectsAsync("t1", CancellationToken.None)).ToDictionary(project => project.Id);

        Assert.Equal(new MapProjectStatus("Completed", "completed"), projects["p1"].Status);
        Assert.Equal(("2026-10-03", "2026-09-20T10:00:00.000Z", "2026-09-21T11:00:00.000Z"),
            (projects["p1"].TargetDate, projects["p1"].CompletedAt, projects["p1"].CanceledAt));
        Assert.Equal((null, null, null), (projects["p2"].TargetDate, projects["p2"].CompletedAt, projects["p2"].CanceledAt));
    }

    [Fact]
    public async Task AnUnknownTeamOrProject_IsNotFound()
    {
        _linear.Serve(call => call.Operation == "TeamProjects" ? """{"data":{"team":null}}""" : """{"data":{"project":null}}""");

        var team = await Assert.ThrowsAsync<MapApiException>(() => _linear.Reader().GetProjectsAsync("nope", CancellationToken.None));
        var project = await Assert.ThrowsAsync<MapApiException>(Graph);

        Assert.Equal(("not_found", 404), (team.Code, team.Status));
        Assert.Equal(("not_found", 404), (project.Code, project.Status));
    }

    [Fact]
    public async Task Graph_ReadsEveryIssuePage()
    {
        static string Page(int from, int count) =>
            string.Join(",", Enumerable.Range(from, count).Select(n => Node(Issue(n.ToString()))));
        _linear.Serve(call => call.Var("after") switch
        {
            null => IssuesPage(Page(1, 20), more: true, cursor: "p1"),
            "p1" => IssuesPage(Page(21, 20), more: true, cursor: "p2"),
            _ => IssuesPage(Page(41, 3)),
        });

        var graph = await Graph();

        AssertSet(Enumerable.Range(1, 43).Select(n => n.ToString()), graph.Issues.Select(issue => issue.Id));
    }

    [Fact]
    public async Task Graph_CarriesEachIssuesFields()
    {
        _linear.Serve(_ => IssuesPage(
            Node(Issue("1", assignee: """{"name":"Balazs"}""", labels: Labels("Feature:#BB87FC"))) + "," +
            Node(Issue("2", state: Duplicate, parent: """{"id":"1","identifier":"DYD-1"}""")) + "," +
            Node(Issue("3", labels: "null")) + "," +
            Node(Issue("4", labels: """{"nodes":[]}"""))));

        var graph = await Graph();

        Assert.Equal(new MapGraphProject("p-map", "Map", "https://linear.app/x/project/map"), graph.Project);
        var issues = graph.Issues.ToDictionary(issue => issue.Id);
        Assert.Equal(new MapIssue("1", "DYD-1", "Issue 1", "https://linear.app/x/issue/DYD-1",
            new MapIssueState("Todo", "unstarted", "#e2e2e2"), "Balazs", null,
            new MapIssueTeam("t-dyd", "DYD"), new MapIssueProject("p-map", "Map"), issues["1"].Labels), issues["1"]);
        Assert.Equal([new MapLabel("Feature", "#BB87FC")], issues["1"].Labels);
        Assert.Equal(("duplicate", "1"), (issues["2"].State.Type, issues["2"].ParentId));
        // Absent, null and empty label connections all read as no labels.
        Assert.All(new[] { "2", "3", "4" }, id => Assert.Empty(issues[id].Labels));
    }

    [Fact]
    public async Task Graph_KeepsBlocksAndRelatedOnce_DropsDuplicateAndSimilar()
    {
        _linear.Serve(_ => IssuesPage(
            Node(Issue("1"), relations: Connection(string.Join(",",
                Outgoing("r-blocks", "blocks", Issue("2")),
                Outgoing("r-related", "related", Issue("2")),
                Outgoing("r-dup", "duplicate", Issue("2")),
                Outgoing("r-sim", "similar", Issue("2"))))) + "," +
            Node(Issue("2"), inverseRelations: Connection(Incoming("r-blocks", "blocks", Issue("1"))))));

        var graph = await Graph();

        Assert.Equal(2, graph.Relations.Count);
        AssertSet([new MapRelation("r-blocks", "blocks", "1", "2"), new MapRelation("r-related", "related", "1", "2")],
            graph.Relations);
        Assert.Empty(graph.External);
    }

    [Fact]
    public async Task Graph_FarEndsOutsideTheProject_AreExternal_WithOrWithoutAProject()
    {
        _linear.Serve(_ => IssuesPage(
            Node(Issue("1"),
                relations: Connection(Outgoing("r-other", "related", Issue("9", project: InOther))),
                inverseRelations: Connection(string.Join(",",
                    Incoming("r-none", "blocks", Issue("8", project: "null")),
                    Incoming("r-other-2", "blocks", Issue("9", project: InOther)))))));

        var graph = await Graph();

        Assert.Equal(["1"], graph.Issues.Select(issue => issue.Id));
        var external = graph.External.ToDictionary(issue => issue.Id);
        AssertSet(["9", "8"], external.Keys);
        Assert.Equal(new MapIssueProject("p-other", "Other"), external["9"].Project);
        Assert.Null(external["8"].Project);
        AssertSet(
            [new MapRelation("r-other", "related", "1", "9"), new MapRelation("r-none", "blocks", "8", "1"),
                new MapRelation("r-other-2", "blocks", "9", "1")],
            graph.Relations);
    }

    [Fact]
    public async Task Graph_DropsArchivedIssues_AndEveryRelationTouchingOne()
    {
        const string archived = "\"2026-09-01T00:00:00.000Z\"";
        _linear.Serve(_ => IssuesPage(
            Node(Issue("1"),
                relations: Connection(Outgoing("r-to-archived", "blocks", Issue("9", project: InOther, archivedAt: archived))),
                inverseRelations: Connection(Incoming("r-from-archived-member", "blocks", Issue("3", archivedAt: archived)))) + "," +
            Node(Issue("3", archivedAt: archived),
                relations: Connection(Outgoing("r-from-archived-member", "blocks", Issue("1"))))));

        var graph = await Graph();

        Assert.Equal(["1"], graph.Issues.Select(issue => issue.Id));
        Assert.Empty(graph.External);
        Assert.Empty(graph.Relations);
    }

    [Fact]
    public async Task Graph_AResolvedBlocker_StaysABlocksRelation()
    {
        _linear.Serve(_ => IssuesPage(
            Node(Issue("1"), inverseRelations: Connection(Incoming("r1", "blocks", Issue("2", state: Done)))) + "," +
            Node(Issue("2", state: Done), relations: Connection(Outgoing("r1", "blocks", Issue("1"))))));

        var graph = await Graph();

        Assert.Equal([new MapRelation("r1", "blocks", "2", "1")], graph.Relations);
    }

    [Fact]
    [Trait("Job", "edge: an issue with more than ten relations each way overflows into IssueRelationsPage (DYD-264)")]
    public async Task Graph_RelationOverflow_IsReadUntilBothListsEnd()
    {
        _linear.Serve(call => (call.Operation, call.Var("after"), call.Var("invAfter")) switch
        {
            ("ProjectIssues", _, _) => IssuesPage(Node(Issue("1"),
                relations: Connection(Outgoing("r-a", "blocks", Issue("9", project: InOther)), more: true, cursor: "out1"),
                inverseRelations: Connection(Incoming("r-b", "related", Issue("8", project: InOther)), more: true, cursor: "in1"))),
            ("IssueRelationsPage", "out1", "in1") => $$"""
                {"data":{"issue":{
                  "relations":{{Connection(Outgoing("r-c", "blocks", Issue("7", project: InOther)), cursor: "out2")}},
                  "inverseRelations":{{Connection(Incoming("r-d", "blocks", Issue("6", project: InOther)), more: true, cursor: "in2")}} } } }
                """,
            // The finished outgoing list answers an empty page after its last cursor.
            ("IssueRelationsPage", "out2", "in2") => $$"""
                {"data":{"issue":{
                  "relations":{{Connection("", cursor: null)}},
                  "inverseRelations":{{Connection(Incoming("r-e", "blocks", Issue("5", project: "null")), cursor: "in3")}} } } }
                """,
            _ => throw new InvalidOperationException($"unexpected {call.Operation} {call.Var("after")} {call.Var("invAfter")}")
        });

        var graph = await Graph();

        AssertSet(["r-a", "r-b", "r-c", "r-d", "r-e"], graph.Relations.Select(relation => relation.Id));
        AssertSet(["9", "8", "7", "6", "5"], graph.External.Select(issue => issue.Id));
    }

    [Fact]
    public async Task Graph_OverflowOnAnIssueLinearNoLongerHas_IsNotFound()
    {
        _linear.Serve(call => call.Operation == "ProjectIssues"
            ? IssuesPage(Node(Issue("1"), relations: Connection("", more: true, cursor: "c")))
            : """{"data":{"issue":null}}""");

        var error = await Assert.ThrowsAsync<MapApiException>(Graph);

        Assert.Equal("not_found", error.Code);
    }

    [Fact]
    public async Task ReadProject_CarriesTheBody_EveryIssuesPriority_AndEachParentIdentifier()
    {
        _linear.Serve(call => call.Operation == "ProjectIssues"
            ? IssuesPage(Node(WithPriority(Issue("1", parent: """{"id":"99","identifier":"DYD-99"}"""), "2"),
                inverseRelations: Connection(Incoming("r1", "blocks", WithPriority(Issue("9", project: InOther), "1.0")))) + "," +
                Node(Issue("2")))
            : ProjectContent("\"## Goal\\n- DYD-1\""));

        var read = await _linear.Reader().ReadProjectAsync("slug1234abcd", CancellationToken.None);

        Assert.Equal("p-map", read.Graph.Project.Id);
        Assert.Equal("## Goal\n- DYD-1", read.Content);
        Assert.Equal(new Dictionary<string, double> { ["1"] = 2, ["2"] = 0, ["9"] = 1 }, read.Priorities);
        Assert.Equal(new Dictionary<string, string> { ["1"] = "DYD-99" }, read.Parents);
    }

    [Theory]
    [InlineData("""{"data":{"project":{"content":null}}}""", null)]
    [InlineData("""{"data":{"project":null}}""", "not_found")]
    public async Task ReadProject_WithoutABody_IsEmpty_AndWithoutAProject_IsNotFound(string answer, string? code)
    {
        _linear.Serve(call => call.Operation == "ProjectIssues" ? IssuesPage("") : answer);

        var read = _linear.Reader().ReadProjectAsync("p-map", CancellationToken.None);

        if (code is null)
            Assert.Equal("", (await read).Content);
        else
            Assert.Equal(code, (await Assert.ThrowsAsync<MapApiException>(() => read)).Code);
    }
}
