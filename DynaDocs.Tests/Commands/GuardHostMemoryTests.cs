namespace DynaDocs.Tests.Commands;

using DynaDocs.Tests.Integration;

/// <summary>
/// DR 055 at the hook: a file-tool write into a host memory folder is refused, anything beside one
/// is not. Claude's folders sit under every ~/.claude* profile and CLAUDE_CONFIG_DIR (projects/*/memory,
/// agent-memory*), in any .claude/agent-memory*, and at each autoMemoryDirectory the settings name;
/// Codex's at $CODEX_HOME/memories. {H} is the user's home, {C} CLAUDE_CONFIG_DIR, {X} CODEX_HOME,
/// {A} the project's autoMemoryDirectory.
/// </summary>
[Collection("Integration")]
[Trait("Job", "contract: DR 055 native agent memory is off and guarded")]
public class GuardHostMemoryTests : GuardHookTestBase
{
    [Theory]
    [InlineData("{H}/.claude/projects/my-proj/memory/MEMORY.md", true)]
    [InlineData("{H}/.claude/projects/my-proj/memory", true)]
    [InlineData("{H}/.claude-personal2/projects/p/memory/MEMORY.md", true)]
    [InlineData("{H}/.claude/agent-memory/reviewer/MEMORY.md", true)]
    [InlineData("{H}/.claude-personal2/agent-memory-local/x.md", true)]
    [InlineData("{C}/projects/p/memory/MEMORY.md", true)]
    [InlineData("{C}/agent-memory/reviewer/MEMORY.md", true)]
    [InlineData("{R}/.claude/agent-memory/reviewer/MEMORY.md", true)]
    [InlineData(".claude/agent-memory-local/reviewer/MEMORY.md", true)]
    [InlineData("{X}/memories/note.md", true)]
    [InlineData("{X}/memories", true)]
    [InlineData("{A}/MEMORY.md", true)]
    [InlineData("~/.claude/projects/p/memory/MEMORY.md", true)]
    [InlineData("{H}/.claude/projects/p/other/../memory/MEMORY.md", true)]
    [InlineData("{H}/.claude/projects/p/memory/../../../secret.md", false)]
    [InlineData("{H}/../other/.claude/projects/p/memory/MEMORY.md", false)]
    [InlineData("dydo/.claude/projects/x/memory/y.md", false)]
    [InlineData("dydo/scratch/memory/notes.md", false)]
    [InlineData("{H}/.claude/settings.json", false)]
    [InlineData("{H}/.claude.json", false)]
    [InlineData("{H}/.claude/projects/p/session.jsonl", false)]
    [InlineData("{H}/.codex/memories/note.md", false)]
    [InlineData("{X}/config.toml", false)]
    [InlineData("{A}-other/MEMORY.md", false)]
    [InlineData("src/Foo.cs", false)]
    public async Task FileToolWrite_IsRefusedOnlyInAHostMemoryFolder(string template, bool refused)
    {
        await InitProjectAsync("none");
        var scratch = Directory.CreateTempSubdirectory("dydo-host-memory-").FullName.Replace('\\', '/');
        var autoMemory = $"{scratch}/notes-memory";
        WriteFile(Path.Combine(".claude", "settings.local.json"), $$"""{ "autoMemoryDirectory": "{{autoMemory}}" }""");
        var home = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile).Replace('\\', '/');
        var saved = new[] { "CLAUDE_CONFIG_DIR", "CODEX_HOME" }.ToDictionary(name => name, Environment.GetEnvironmentVariable);
        try
        {
            Environment.SetEnvironmentVariable("CLAUDE_CONFIG_DIR", $"{scratch}/profile");
            Environment.SetEnvironmentVariable("CODEX_HOME", $"{scratch}/codex-home");
            var path = template.Replace("{H}", home).Replace("{C}", $"{scratch}/profile")
                .Replace("{X}", $"{scratch}/codex-home").Replace("{A}", autoMemory).Replace("{R}", TestDir.Replace('\\', '/'));

            var result = await GuardWithStdinAsync(FileToolJson("Write", path));

            if (refused) result.AssertPreToolUseDenied();
            else result.AssertPreToolUseAllowed();
        }
        finally
        {
            foreach (var (name, value) in saved)
                Environment.SetEnvironmentVariable(name, value);
            Directory.Delete(scratch, recursive: true);
        }
    }

    // A settings file the guard cannot read names no memory folder, and refuses nothing for it.
    [Fact]
    public async Task UnreadableSettings_NameNoMemoryFolder()
    {
        await InitProjectAsync("none");
        WriteFile(Path.Combine(".claude", "settings.json"), "{ not json");

        (await GuardWithStdinAsync(FileToolJson("Write", Path.Combine(TestDir, "src", "Foo.cs")))).AssertPreToolUseAllowed();
    }
}
