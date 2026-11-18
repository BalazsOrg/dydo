namespace DynaDocs.Tests.Integration;

/// <summary>
/// The two path tiers of dydo/files-off-limits.md at the hook entry: an off-limits path refuses
/// every tool, a protected one refuses only a change, and a whitelist entry lifts only the user's
/// off-limits patterns, never dydo's own system paths.
/// </summary>
[Collection("Integration")]
[Trait("Job", "contract: files-off-limits.md tiers, DR 045 §10, Decision 024 system paths")]
public class GuardOffLimitsTests : GuardHookTestBase
{
    // Glob syntax as the file documents it: * within a segment, ** across segments, ? one character,
    // either slash, any case; a pattern without a slash also names that file in any directory.
    [Theory]
    [InlineData(".env", ".env", true)]
    [InlineData(".env.*", ".env.local", true)]
    [InlineData(".env.*", ".env.production", true)]
    [InlineData(".env", "config/.env", true)]
    [InlineData(".env", "src/config/.env", true)]
    [InlineData(".env", "README.md", false)]
    [InlineData(".env", "envfile.txt", false)]
    [InlineData(".env", "my.env.bak", false)]
    [InlineData(".env", ".env.local", false)]
    [InlineData("**/secrets.json", "secrets.json", true)]
    [InlineData("**/secrets.json", "config/secrets.json", true)]
    [InlineData("**/secrets.json", "deep/nested/path/secrets.json", true)]
    [InlineData("**/secrets.json", "config\\secrets.json", true)]
    [InlineData("**/secrets.json", "./config/secrets.json", true)]
    [InlineData("**/secrets.json", "/config/secrets.json", true)]
    [InlineData("**/secrets.json", "config.json", false)]
    [InlineData("**/credentials.*", "src/api/credentials.xml", true)]
    [InlineData("**/*.key", "private.key", true)]
    [InlineData("**/*.key", "src/main.cs", false)]
    [InlineData("**/id_ed25519", "id_ed25519", true)]
    [InlineData("src/**", "src/nested/deep/file.cs", true)]
    [InlineData("src/**", "src\\nested\\file.cs", true)]
    [InlineData("src/**", "SRC/File.cs", true)]
    [InlineData("src/**", "other/file.cs", false)]
    [InlineData("Commands/*", "Commands/Foo.cs", true)]
    [InlineData("commands/*", "Commands/FOO.cs", true)]
    [InlineData("Commands/*", "Commands/Sub/Foo.cs", false)]
    [InlineData("config/*/secret.md", "config/prod/secret.md", true)]
    [InlineData("config/*/secret.md", "config/secret.md", false)]
    [InlineData("file?.cs", "file1.cs", true)]
    [InlineData("file?.cs", "file.cs", false)]
    [InlineData("file?.cs", "fileAB.cs", false)]
    [InlineData("**/config/*.json", "src/config/secrets.json", true)]
    [InlineData("**/config/*.json", "config/secrets.json", true)]
    [InlineData("**/config/*.json", "src/secrets.json", false)]
    public async Task OffLimitsPattern_RefusesEveryToolOnlyWhereItMatches(string pattern, string path, bool offLimits)
    {
        await InitWithOffLimitsAsync($"## Off-Limits\n```\n{pattern}\n```\n");

        foreach (var tool in new[] { "Read", "Write" })
        {
            var result = await GuardWithStdinAsync(FileToolJson(tool, path));
            if (offLimits) result.AssertPreToolUseDenied();
            else result.AssertPreToolUseAllowed();
        }
    }

    // Claude Code hands absolute paths; the repo-relative pattern still matches them.
    [Fact]
    public async Task AbsolutePathInsideTheProject_IsMatchedAsRepoRelative()
    {
        await InitWithOffLimitsAsync("## Off-Limits\n```\n**/secrets.json\n```\n");

        (await GuardWithStdinAsync(FileToolJson("Read", Path.Combine(TestDir, "config", "secrets.json")))).AssertPreToolUseDenied();
        (await GuardWithStdinAsync(FileToolJson("Read", Path.Combine(TestDir, "config", "public.json")))).AssertPreToolUseAllowed();
    }

    // Every heading the file accepts for the whitelist lifts the user's pattern for the listed path
    // only; dydo/_system/** stays off-limits and dydo.json stays protected whatever it lists.
    [Theory]
    [InlineData("## Whitelist")]
    [InlineData("# Whitelist")]
    [InlineData("## Exceptions")]
    [InlineData("# Exceptions")]
    public async Task Whitelist_LiftsOnlyTheUsersPatterns(string heading)
    {
        await InitWithOffLimitsAsync(
            "## Off-Limits\n```\n.env*\n**/secrets.json\nlogs/**\n```\n\n"
            + heading + "\n```\n.env.example\ntests/fixtures/secrets.json\nlogs/**\ndydo/_system/**\ndydo.json\n```\n");

        foreach (var (path, refused) in new[]
        {
            (".env.local", true), (".env.example", false), ("config/secrets.json", true),
            ("tests/fixtures/secrets.json", false), ("logs/app/debug.log", false),
            ("dydo/_system/roles/code-writer.role.json", true),
        })
        {
            var read = await GuardWithStdinAsync(FileToolJson("Read", path));
            if (refused) read.AssertPreToolUseDenied();
            else read.AssertPreToolUseAllowed();
        }

        (await GuardWithStdinAsync(FileToolJson("Read", "dydo.json"))).AssertPreToolUseAllowed();
        (await GuardWithStdinAsync(FileToolJson("Write", "dydo.json"))).AssertPreToolUseDenied();
    }

    // A protected path is readable and unwritable, by relative or absolute path, and dydo.json is
    // protected even when the file lists nothing.
    [Theory]
    [InlineData("## Protected Patterns\n```\ndydo/index.md\n```\n", "dydo/index.md")]
    [InlineData("## Off-Limits\n```\n.env\n```\n", "dydo.json")]
    public async Task ProtectedPath_IsReadableAndUnwritable(string offLimits, string path)
    {
        await InitWithOffLimitsAsync(offLimits);

        foreach (var target in new[] { path, Path.Combine(TestDir, path) })
        {
            (await GuardWithStdinAsync(FileToolJson("Read", target))).AssertPreToolUseAllowed();
            (await GuardWithStdinAsync(FileToolJson("Write", target))).AssertPreToolUseDenied();
        }
    }

    private async Task InitWithOffLimitsAsync(string content)
    {
        await InitProjectAsync("none");
        WriteFile(Path.Combine("dydo", "files-off-limits.md"), content.Replace("\n", Environment.NewLine));
    }
}
