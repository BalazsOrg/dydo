namespace DynaDocs.Tests.Services;

using System.Text.Json;
using DynaDocs.Tests.Integration;

/// <summary>
/// The default nudges an initialised project gets, as the guard hook applies them to a Bash call:
/// the review-block nudge warns on a `gh pr create` that carries no review block, however the
/// command is prefixed (DYD-736), and the poll-loop nudge warns on an open-ended `until` loop.
/// Bounded polls and words that merely contain "git" pass.
/// </summary>
[Collection("Integration")]
public sealed class ConfigFactoryTests : IntegrationTestBase
{
    private const string NoReviewBlock = "carries no review block";
    private const string PollLoop = "poll-loop";

    [Theory]
    [Trait("Job", "trap: DYD-736 (prefixed gh pr create slipped past the review-block nudge) and issue 0177 (open-ended until-loops crashed agents): each row asserts the default nudges' verdict at the hook, the named warning or a pass")]
    [InlineData("gh pr create --title x --body 'ships the fix'", NoReviewBlock)]
    [InlineData("FOO=1 gh pr create --fill", NoReviewBlock)]
    [InlineData("env gh pr create --fill", NoReviewBlock)]
    [InlineData("env -i -u GH_TOKEN -- GH_REPO=acme/lc gh pr create --fill", NoReviewBlock)]
    [InlineData("TITLE=release\\ notes gh pr create --fill", NoReviewBlock)]
    [InlineData("until [ -s /tmp/claude/foo ]; do sleep 1; done", PollLoop)]
    [InlineData("until  [ -e foo ]; do :; done", PollLoop)]
    [InlineData("for i in {1..30}; do test -f x; sleep 1; done", null)]
    [InlineData("while [ ! -f x ]; do sleep 1; done", null)]
    [InlineData("gh run watch 12345", null)]
    [InlineData("digit worktree add foo", null)]
    public async Task Guard_DefaultNudges_WarnOnTheCommand(string command, string? warning)
    {
        await InitProjectAsync("none");

        var result = await GuardWithStdinAsync(BashHook(command));

        AssertVerdict(result, warning);
    }

    // Each env option word has one reading, so a long option chain stays inside the guard's match budget.
    [Trait("Job", "trap: Round 1 N2 regex budget")]
    [Theory]
    [InlineData("env {0}x", null)]
    [InlineData("env {0}gh pr create --fill", NoReviewBlock)]
    public async Task Guard_LongEnvOptionChain_IsReadWithinTheMatchBudget(string template, string? warning)
    {
        await InitProjectAsync("none");
        var command = string.Format(template, string.Concat(Enumerable.Repeat("-u ", 400)));

        var result = await GuardWithStdinAsync(BashHook(command));

        AssertVerdict(result, warning);
    }

    private static void AssertVerdict(CommandResult result, string? warning)
    {
        if (warning is null)
            result.AssertPreToolUseAllowed();
        else
            Assert.Contains(warning, result.AssertPreToolUseDenied());
    }

    private static string BashHook(string command) => JsonSerializer.Serialize(new Dictionary<string, object>
    {
        ["session_id"] = TestSessionId,
        ["tool_name"] = "Bash",
        ["tool_input"] = new Dictionary<string, string> { ["command"] = command }
    });
}
