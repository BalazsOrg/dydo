namespace DynaDocs.Tests.Map;

using System.Net;
using System.Text.Json;
using DynaDocs.Services.Map;
using static LinearJson;

/// <summary>
/// The map server over real HTTP on localhost, with a fake Linear behind it: DR 052 §1 and §3 and its
/// 2026-10-04 amendment (<c>/api/saved</c> reads only the cache, <c>/api/graph</c> always reads Linear).
/// </summary>
public sealed class MapServerTests : IAsyncLifetime
{
    private readonly FakeLinear _linear = new();
    private readonly HttpClient _browser = new();
    private readonly CancellationTokenSource _stop = new();
    private MapServer _server = null!;
    private Task _running = Task.CompletedTask;
    private Uri _url = null!;

    public Task InitializeAsync()
    {
        _server = new MapServer(new MapApi(_linear.Reader()), new ViewerBundle(_ => null));
        _url = _server.Start();
        _running = _server.RunAsync(_stop.Token);
        return Task.CompletedTask;
    }

    public async Task DisposeAsync()
    {
        await _stop.CancelAsync();
        await _running;
        _server.Dispose();
        _browser.Dispose();
        _stop.Dispose();
    }

    private async Task<(HttpStatusCode Status, string Body)> Get(string path, HttpMethod? method = null)
    {
        using var response = await _browser.SendAsync(new HttpRequestMessage(method ?? HttpMethod.Get, new Uri(_url, path)));
        return (response.StatusCode, await response.Content.ReadAsStringAsync());
    }

    private static string ErrorCode(string body) =>
        JsonDocument.Parse(body).RootElement.GetProperty("error").GetProperty("code").GetString()!;

    [Fact]
    [Trait("Job", "contract: DR 052 §3, the CLI serves the viewer on localhost (the URL the server starts on names localhost and a port it picked)")]
    public void Start_BindsLocalhostOnAFreePort()
    {
        Assert.Equal("localhost", _url.Host);
        Assert.NotEqual(0, _url.Port);
    }

    [Fact]
    [Trait("Job", "edge: the probed free port is taken before the listener binds it: the server starts on the next probed port")]
    public void Start_RetriesWithAFreshListener_WhenTheFirstProbedPortIsTaken()
    {
        var occupied = FreeTcpPort();
        using var blocker = new HttpListener();
        blocker.Prefixes.Add($"http://localhost:{occupied}/");
        blocker.Start();
        var free = FreeTcpPort();
        var ports = new Queue<int>([occupied, free]);

        using var server = new MapServer(new MapApi(_linear.Reader()), new ViewerBundle(_ => null), () => ports.Dequeue());

        Assert.Equal(free, server.Start().Port);
    }

    private static int FreeTcpPort()
    {
        var probe = new System.Net.Sockets.TcpListener(IPAddress.Loopback, 0);
        probe.Start();
        var port = ((IPEndPoint)probe.LocalEndpoint).Port;
        probe.Stop();
        return port;
    }

    [Fact]
    [Trait("Job", "contract: DR 052 2026-10-04 amendment, /api/saved reads only the cache; dydo-commands-linear.md dydo map, `{ \"snapshot\": null }` on a miss (a first /api/saved answers 200 with a null snapshot, and Linear is not called)")]
    public async Task Saved_FirstVisitIsAnExplicitMiss_WithoutReadingLinear()
    {
        var (status, body) = await Get("/api/saved?project=p-map");

        Assert.Equal(HttpStatusCode.OK, status);
        Assert.Equal(JsonValueKind.Null, JsonDocument.Parse(body).RootElement.GetProperty("snapshot").ValueKind);
        Assert.Empty(_linear.Calls);
    }

    [Fact]
    [Trait("Job", "contract: DR 052 §3, the browser talks only to localhost and the key stays in the CLI (GET /api/graph answers 200 with the graph read from Linear, its issue, external far end and blocks relation, and the body does not hold the key)")]
    public async Task Graph_AnswersTheReadGraph_WithoutTheKey()
    {
        _linear.Serve(_ => IssuesPage(
            Node(Issue("1", labels: Labels("AFK:#30A46C")), inverseRelations: Connection(Incoming("r1", "blocks", Issue("9", project: "null"))))));

        var (status, body) = await Get("/api/graph?project=p-map");

        Assert.Equal(HttpStatusCode.OK, status);
        var graph = JsonDocument.Parse(body).RootElement;
        Assert.Equal("1", graph.GetProperty("issues")[0].GetProperty("id").GetString());
        Assert.Equal("9", graph.GetProperty("external")[0].GetProperty("id").GetString());
        var relation = graph.GetProperty("relations")[0];
        Assert.Equal(("blocks", "9", "1"),
            (relation.GetProperty("type").GetString(), relation.GetProperty("from").GetString(), relation.GetProperty("to").GetString()));
        Assert.DoesNotContain(FakeLinear.ApiKey, body);
    }

    [Fact]
    [Trait("Job", "contract: DR 052 2026-10-04 amendment, /api/graph always reads Linear (two requests each answer Linear's state at the time: Todo, then In Progress)")]
    public async Task Graph_IsNotCached_EachRequestReadsLinearAgain()
    {
        var state = "Todo";
        _linear.Serve(_ => IssuesPage(Node(Issue("1", state: $$"""{"name":"{{state}}","type":"unstarted","color":"#fff"}"""))));

        var first = await Get("/api/graph?project=p-map");
        state = "In Progress";
        var second = await Get("/api/graph?project=p-map");

        Assert.Contains("\"name\":\"Todo\"", first.Body);
        Assert.Contains("\"name\":\"In Progress\"", second.Body);
    }

    [Theory]
    [Trait("Job", "contract: DYD-264 Outcome, the plan §3 HTTP API contract and its error envelope (a request missing its team or project parameter, by row, answers 400 missing_parameter)")]
    [InlineData("/api/projects")]
    [InlineData("/api/projects?team=")]
    [InlineData("/api/graph")]
    public async Task MissingParameter_Is400_WithoutCallingLinear(string path)
    {
        var (status, body) = await Get(path);

        Assert.Equal((HttpStatusCode.BadRequest, "missing_parameter"), (status, ErrorCode(body)));
        Assert.Empty(_linear.Calls);
    }

    [Theory]
    [Trait("Job", "contract: DYD-264 Outcome, the error envelope; DR 052 §3, the key stays in the CLI (the row's Linear failure answers its HTTP status and error code in the envelope, and the body does not hold the key)")]
    [InlineData(HttpStatusCode.OK, """{"data":{"project":null}}""", HttpStatusCode.NotFound, "not_found")]
    [InlineData(HttpStatusCode.BadRequest,
        """{"errors":[{"message":"Authentication required, not authenticated","extensions":{"type":"authentication error"}}]}""",
        HttpStatusCode.BadGateway, "linear_auth")]
    [InlineData(HttpStatusCode.BadRequest,
        """{"errors":[{"message":"Rate limited","extensions":{"code":"RATELIMITED"}}]}""",
        HttpStatusCode.ServiceUnavailable, "linear_rate_limited")]
    [InlineData(HttpStatusCode.OK, """{"errors":[{"message":"boom"}]}""", HttpStatusCode.BadGateway, "linear_error")]
    [InlineData(HttpStatusCode.OK, """{"data":{"project":{"id":"p"}}}""", HttpStatusCode.BadGateway, "linear_error")]
    [InlineData(HttpStatusCode.OK, """{"data":{"project":{"id":"p","name":"n","url":"u","issues":{"pageInfo":{"hasNextPage":false},"nodes":[{"id":null}]}}}}""",
        HttpStatusCode.BadGateway, "linear_error")]
    public async Task LinearFailures_AnswerTheErrorEnvelope_WithoutTheKey(
        HttpStatusCode linearStatus, string linearBody, HttpStatusCode status, string code)
    {
        _linear.Answer = _ => (linearStatus, linearBody);

        var (actualStatus, body) = await Get("/api/graph?project=p-map");

        Assert.Equal((status, code), (actualStatus, ErrorCode(body)));
        Assert.DoesNotContain(FakeLinear.ApiKey, body);
    }

    [Theory]
    [Trait("Job", "contract: DYD-264 Outcome, the server only answers GET and the API exactly as plan §3 fixes it (by row, an unknown API path, a missing asset and each non-GET method answer 404 not_found)")]
    [InlineData("GET", "/api/nothing")]
    [InlineData("GET", "/assets/missing.js")]
    [InlineData("POST", "/api/teams")]
    [InlineData("DELETE", "/api/graph?project=p-map")]
    [InlineData("PUT", "/")]
    public async Task AnUnknownPathOrANonGetMethod_Is404_AndNeverReachesLinear(string method, string path)
    {
        var (status, body) = await Get(path, new HttpMethod(method));

        Assert.Equal((HttpStatusCode.NotFound, "not_found"), (status, ErrorCode(body)));
        Assert.Empty(_linear.Calls);
    }
}
