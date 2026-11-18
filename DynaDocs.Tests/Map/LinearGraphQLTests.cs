namespace DynaDocs.Tests.Map;

using System.Net;
using DynaDocs.Services.Map;

/// <summary>DR 052 §2 and §3 at the transport: the user's key goes to Linear as-is, and every failure maps to one error.</summary>
public class LinearGraphQLTests
{
    private readonly FakeLinear _linear = new();

    private Task<System.Text.Json.JsonElement> Query() =>
        _linear.Transport().QueryAsync("query Teams { teams { nodes { id } } }", new() { ["after"] = null }, CancellationToken.None);

    [Fact]
    [Trait("Job", "trap: a Linear personal API key is rejected with the Bearer scheme, so it goes bare (DYD-264)")]
    public async Task TheKeyIsSentBare()
    {
        _linear.Serve(_ => """{"data":{"teams":{"nodes":[]}}}""");

        await Query();

        Assert.Equal(FakeLinear.ApiKey, Assert.Single(_linear.Calls).Authorization);
    }

    [Theory]
    [Trait("Job", "contract: DR 052 §3; DR 063 §5 (a Linear failure is named, never the key)")]
    [InlineData(HttpStatusCode.OK, """{"errors":[{"message":"Field 'x' is unknown"}],"data":null}""", "linear_error", 502)]
    [InlineData(HttpStatusCode.OK, """{"errors":[{"message":"partial"}],"data":{"teams":null}}""", "linear_error", 502)]
    [InlineData(HttpStatusCode.BadRequest,
        """{"errors":[{"message":"Authentication required, not authenticated","extensions":{"type":"authentication error","userPresentableMessage":"You need to authenticate to access this operation."}}]}""",
        "linear_auth", 502)]
    [InlineData(HttpStatusCode.BadRequest, """{"errors":[{"message":"Rate limit exceeded","extensions":{"code":"RATELIMITED"}}]}""",
        "linear_rate_limited", 503)]
    [InlineData(HttpStatusCode.TooManyRequests, "slow down", "linear_rate_limited", 503)]
    [InlineData(HttpStatusCode.OK,
        """{"errors":[{"message":"Entity not found: Project","extensions":{"type":"invalid input"}}],"data":null}""", "not_found", 404)]
    [InlineData(HttpStatusCode.OK, """{"errors":[{"extensions":{"code":"X"}}]}""", "linear_error", 502)]
    [InlineData(HttpStatusCode.InternalServerError, "<html>oops</html>", "linear_error", 502)]
    [InlineData(HttpStatusCode.OK, """{"data":null}""", "linear_error", 502)]
    [InlineData(HttpStatusCode.OK, "not json", "linear_error", 502)]
    public async Task ALinearFailure_MapsToOneError(HttpStatusCode status, string body, string code, int httpStatus)
    {
        _linear.Answer = _ => (status, body);

        var error = await Assert.ThrowsAsync<MapApiException>(Query);

        Assert.Equal((code, httpStatus), (error.Code, error.Status));
        Assert.DoesNotContain(FakeLinear.ApiKey, error.Message);
    }

    [Theory]
    [Trait("Job", "contract: DR 052 §3; DR 063 §5 (a Linear failure is named, never the key)")]
    [InlineData("unreachable")]
    [InlineData("slow")]
    public async Task UnreachableOrSlowLinear_IsALinearError(string failure)
    {
        _linear.Answer = _ => failure == "slow"
            ? throw new TaskCanceledException("timeout")
            : throw new HttpRequestException("connection refused");

        var error = await Assert.ThrowsAsync<MapApiException>(Query);

        Assert.Equal(("linear_error", 502), (error.Code, error.Status));
    }
}
